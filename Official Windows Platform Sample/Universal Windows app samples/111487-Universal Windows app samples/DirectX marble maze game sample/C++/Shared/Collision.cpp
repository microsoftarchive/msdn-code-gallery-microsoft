//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace DirectX;

#include "Primitives.h"
#include "Collision.h"
#include "Physics.h"

#pragma region Inline math functions

// Return the point on the line segment (S1, S2) nearest the point P.
static inline XMVECTOR PointOnLineSegmentNearestPoint(FXMVECTOR S1, FXMVECTOR S2, FXMVECTOR P)
{
    XMVECTOR dir = S2 - S1;
    XMVECTOR projection = (XMVector3Dot(P, dir) - XMVector3Dot(S1, dir));
    XMVECTOR lengthSq = XMVector3Dot(dir, dir);

    XMVECTOR t = projection * XMVectorReciprocal(lengthSq);
    XMVECTOR point = S1 + t * dir;

    // t < 0
    XMVECTOR selectS1 = XMVectorLess(projection, XMVectorZero());
    point = XMVectorSelect(point, S1, selectS1);

    // t > 1
    XMVECTOR selectS2 = XMVectorGreater(projection, lengthSq);
    point = XMVectorSelect(point, S2, selectS2);

    return point;
}

// Test if the point (P) on the plane of the triangle is inside the triangle
// (V0, V1, V2).
static inline XMVECTOR PointOnPlaneInsideTriangle(FXMVECTOR P, FXMVECTOR V0, FXMVECTOR V1, CXMVECTOR V2)
{
    // Compute the triangle normal.
    XMVECTOR N = XMVector3Cross(V2 - V0, V1 - V0);

    // Compute the cross products of the vector from the base of each edge to
    // the point with each edge vector.
    XMVECTOR C0 = XMVector3Cross(P - V0, V1 - V0);
    XMVECTOR C1 = XMVector3Cross(P - V1, V2 - V1);
    XMVECTOR C2 = XMVector3Cross(P - V2, V0 - V2);

    // If the cross product points in the same direction as the normal the the
    // point is inside the edge (it is zero if is on the edge).
    XMVECTOR zero = XMVectorZero();
    XMVECTOR inside0 = XMVectorGreaterOrEqual(XMVector3Dot(C0, N), zero);
    XMVECTOR inside1 = XMVectorGreaterOrEqual(XMVector3Dot(C1, N), zero);
    XMVECTOR inside2 = XMVectorGreaterOrEqual(XMVector3Dot(C2, N), zero);

    // If the point inside all of the edges it is inside.
    return XMVectorAndInt(XMVectorAndInt(inside0, inside1), inside2);
}

#pragma endregion

BOOL Collision::AccumulateSphereTriangleIntersections(
    FXMVECTOR sphere,
    FXMVECTOR radius,
    FXMVECTOR path,
    MeshID mesh,
    const std::vector<Triangle>& triList
    )
{
    size_t triCount = triList.size();

    BOOL found = FALSE;
    for (size_t i = 0; i < triCount; ++i)
    {
        Contact f(triList[i], i, mesh);

        if (f.CalculateContact(sphere, radius, path))
        {
            m_collisions.push_back(f);
            found = TRUE;
        }
    }

    return found;
}

// Builds a list of triangles which are fully or partially enclosed by the sphere.
BOOL Collision::BuildCollisionListForSphere(const Sphere& meshLocalSpace, FXMVECTOR path)
{
    m_collisions.clear();
    XMVECTOR sphere = XMLoadFloat4(&meshLocalSpace.center);
    XMVECTOR radius = XMVectorSplatW(sphere);

    m_intersectsGround = AccumulateSphereTriangleIntersections(sphere, radius, path, MeshID::Ground, m_groundTriList);

    BOOL intersected = m_intersectsGround;
    intersected |= AccumulateSphereTriangleIntersections(sphere, radius, path, MeshID::Wall, m_wallTriList);

    // NOTE: We don't collide against the floor, as the mesh has some edges which interfere with normal rolling.

    // intersected |= AccumulateSphereTriangleIntersections(sphere, radius, path, MeshID::Floor, m_floorTriList);

    MergeSharedEdgeCoplanarContacts(sphere, radius, path);

    return intersected;
}

// After the worst interpenetration has been handled, recalculate the distances.
void Collision::UpdateInterpenetrations(FXMVECTOR newPosition, FXMVECTOR radius, FXMVECTOR path)
{
    const size_t contacts = m_collisions.size();
    for (size_t i = 0; i < contacts; ++i)
    {
        Contact& c = m_collisions[i];

        if (!c.CalculateContact(newPosition, radius, path))
        {
            c.Invalidate();
        }
    }

    MergeSharedEdgeCoplanarContacts(newPosition, radius, path);
}

// Finds the contact which exhibits the worst inclusion with the sphere with a velocity that is moving the sphere
// into that triangle. If none match, returns nullptr.
Contact* Collision::FindWorstInterpenetration()
{
    Contact* worst = nullptr;
    float worstDistance = FLT_MAX;

    for (size_t i = 0; i < m_collisions.size(); ++i)
    {
        Contact& c = m_collisions[i];

        if (c.IsColliding() && c.penetrationDistance < worstDistance && c.closingVelocity <= 0.0f)
        {
            worst = &c;
            worstDistance = c.penetrationDistance;
        }
    }

    return worst;
}

// Merges any collisions we find together, depending on if they're coplanar or not (face vs. edge contacts), and
// whether or not they share edges.
void Collision::MergeSharedEdgeCoplanarContacts(FXMVECTOR sphere, FXMVECTOR radius, FXMVECTOR path)
{
    size_t count = m_collisions.size();
    for (size_t j = 0; j < count; ++j)
    {
        Contact& c1 = m_collisions[j];
        if (!c1.IsColliding())
            continue;

        XMVECTOR A = XMLoadFloat3(&c1.triangle.A);
        XMVECTOR B = XMLoadFloat3(&c1.triangle.B);
        XMVECTOR C = XMLoadFloat3(&c1.triangle.C);
        XMVECTOR N = c1.triangle.Normal();

        XMVECTOR c1Contact = XMLoadFloat3(&c1.contactPosition);

        for (size_t i = j + 1; i < count; ++i)
        {
            Contact& c2 = m_collisions[i];
            if (!c2.IsColliding())
                continue;

            BOOL coplanar, sharesverts;
            c2.triangle.CheckSharesVertsOrCoplanar(A, B, C, N, sharesverts, coplanar);

            if (sharesverts)
            {
                if (coplanar)
                {
                    // If we share vertices with the other tri, and we're coplanar, then
                    // we're on a larger "plane" stretch, so we should use that.

                    c2.Invalidate();

                    // Update normal
                    XMStoreFloat3((XMFLOAT3*)&c1.plane, N);

                    // Update closing velocity.
                    XMStoreFloat(&c1.closingVelocity, XMVector3Dot(path, N));

                    XMVECTOR dist = XMVector3Dot(sphere - A, N);
                    XMVECTOR newContact = sphere - (dist * N);
                    XMVECTOR penetration = dist - radius;

                    // Find new contact point - project sphere center down onto plane.
                    XMStoreFloat3(&c1.contactPosition, newContact);
                    c1.contactIsEdge = FALSE;
                    XMStoreFloat(&c1.penetrationDistance, penetration);

                }
                else if (c1.contactIsEdge || c2.contactIsEdge)
                {
                    // We've got a contact position that's most likely on a corner.
                    const XMVECTOR epsilon = XMVectorSet(1E-4f, 1E-4f, 1E-4f, 1E-4f);
                    const XMVECTOR contactPos1 = XMLoadFloat3(&c1.contactPosition);
                    const XMVECTOR contactPos2 = XMLoadFloat3(&c2.contactPosition);

                    // If they're nearly equal, recalculate normal as average of two tris.
                    if (XMVector3NearEqual(contactPos1, contactPos2, epsilon))
                    {
                        c1.contactIsEdge = FALSE;
                        XMVECTOR C1Normal = c1.triangle.Normal();
                        XMVECTOR C2Normal = c2.triangle.Normal();
                        XMVECTOR newNormal = XMVector3Normalize(C1Normal + C2Normal);
                        XMStoreFloat3((XMFLOAT3*)&c1.plane, newNormal);

                        XMStoreFloat(&c1.closingVelocity, XMVector3Dot(path, newNormal));

                        // Calculate new penetration...
                        XMVECTOR dist = XMVector3Dot(sphere - contactPos1, newNormal);
                        XMVECTOR penetration = dist - radius;
                        XMStoreFloat(&c1.penetrationDistance, penetration);
                        c2.Invalidate();
                    }
                }
            }
            else
            {
                // If we don't share an edge, but we're coplanar, check if we're the same distance... If
                // we are, we're probably on a flat plane, and can merge the two.
                if (coplanar)
                {
                    const float PENETRATION_DIST_TOLERANCE = 1E-4f;
                    if (fabs(c1.penetrationDistance - c2.penetrationDistance) <= PENETRATION_DIST_TOLERANCE)
                    {
                        // Same plane!
                        c2.Invalidate();
                    }
                }
            }
        }
    }
}

// Push the sphere out of the surface along its surface normal until it is just touching.
XMVECTOR Contact::Resolve(FXMVECTOR position, FXMVECTOR radius)
{
    XMVECTOR N = XMLoadFloat3((XMFLOAT3*) &plane);
    return position - (penetrationDistance * N);
}

// Calculates the penetration distance of the sphere into the triangle.
BOOL Contact::CalculateContact(FXMVECTOR position, FXMVECTOR radiusIn, FXMVECTOR path)
{
    XMVECTOR V0 = XMLoadFloat3(&triangle.A);
    XMVECTOR V1 = XMLoadFloat3(&triangle.B);
    XMVECTOR V2 = XMLoadFloat3(&triangle.C);

    // Load the sphere.
    XMVECTOR center = position;
    XMVECTOR radius = radiusIn;

    // Compute the plane of the triangle (has to be normalized).
    XMVECTOR N = XMVector3Normalize(XMVector3Cross(V1 - V0, V2 - V0));

    // Is velocity in
    XMVECTOR VDotN = XMVector3Dot(path, N);
    XMStoreFloat(&closingVelocity, VDotN);

    // Find the nearest feature on the triangle to the sphere.
    XMVECTOR dist = XMVector3Dot(center - V0, N);

    // If the center of the sphere is farther from the plane of the triangle than
    // the radius of the sphere, then there cannot be an intersection.
    XMVECTOR noIntersection = XMVectorLess(dist, -radius);
    noIntersection = XMVectorOrInt(noIntersection, XMVectorGreater(dist, radius));

    // Project the center of the sphere onto the plane of the triangle.
    XMVECTOR pointOnPlane = center - (N * dist);

    // Is it inside all the edges? If so we intersect because the distance
    // to the plane is less than the radius.
    XMVECTOR isOnPlaneInsideTriangle = PointOnPlaneInsideTriangle(pointOnPlane, V0, V1, V2);

    // Find the nearest point on each edge.
    XMVECTOR radiusSq = radius * radius;

    // Edge 0, 1
    XMVECTOR pointEdge01 = PointOnLineSegmentNearestPoint(V0, V1, center);

    XMVECTOR distEdge01 = XMVector3LengthSq(center - pointEdge01);

    // If the distance to the center of the sphere to the point is less than
    // the radius of the sphere then it must intersect.
    XMVECTOR intersection = XMVectorOrInt(isOnPlaneInsideTriangle, XMVectorLessOrEqual(distEdge01, radiusSq));

    // Edge 1, 2
    XMVECTOR pointEdge12 = PointOnLineSegmentNearestPoint(V1, V2, center);
    XMVECTOR distEdge12 = XMVector3LengthSq(center - pointEdge12);

    // If the distance to the center of the sphere to the point is less than
    // the radius of the sphere then it must intersect.
    intersection = XMVectorOrInt(intersection, XMVectorLessOrEqual(distEdge12, radiusSq));

    // Edge 2, 0
    XMVECTOR pointEdge20 = PointOnLineSegmentNearestPoint(V2, V0, center);
    XMVECTOR distEdge20 = XMVector3LengthSq(center - pointEdge20);

    // If the distance to the center of the sphere to the point is less than
    // the radius of the sphere then it must intersect.
    intersection = XMVectorOrInt(intersection, XMVectorLessOrEqual(distEdge20, radiusSq));

    XMVECTOR pointOnPlaneDist = XMVector3LengthSq(center - pointOnPlane);
    XMVECTOR contactPointDist = XMVectorSet(FLT_MAX, FLT_MAX, FLT_MAX, FLT_MAX);
    XMVECTOR contactPoint = pointOnPlane;
    XMVECTOR isEdge = XMVectorFalseInt();
    // if intersects plane, start with point on plane distance
    contactPointDist = XMVectorSelect(contactPointDist, pointOnPlaneDist, isOnPlaneInsideTriangle);

    XMVECTOR select = XMVectorLessOrEqual(distEdge01, contactPointDist);
    contactPointDist = XMVectorSelect(contactPointDist, distEdge01, select);
    contactPoint = XMVectorSelect(contactPoint, pointEdge01, select);
    isEdge = XMVectorOrInt(isEdge, select);

    select = XMVectorLessOrEqual(distEdge12, contactPointDist);
    contactPointDist = XMVectorSelect(contactPointDist, distEdge12, select);
    contactPoint = XMVectorSelect(contactPoint, pointEdge12, select);
    isEdge = XMVectorOrInt(isEdge, select);

    select = XMVectorLessOrEqual(distEdge20, contactPointDist);
    contactPointDist = XMVectorSelect(contactPointDist, distEdge20, select);
    contactPoint = XMVectorSelect(contactPoint, pointEdge20, select);
    isEdge = XMVectorOrInt(isEdge, select);

    contactPointDist = XMVectorSqrt(contactPointDist);

    // Store the contact point, and the contact edge.
    XMStoreFloat3(&contactPosition, contactPoint);
    contactIsEdge = XMVector4EqualInt(isEdge, XMVectorTrueInt());

    // Is the sphere at the contact point? In which case we just use the surface normal.
    XMVECTOR isContactAtSphereCenter = XMVectorInBounds(contactPointDist, XMVectorSplatEpsilon());
    XMVECTOR contactToCenterNormal = (center - contactPoint) / contactPointDist;
    XMVECTOR contactNormal = XMVectorSelect(contactToCenterNormal, N, isContactAtSphereCenter);

    // Store the contact normal
    XMStoreFloat3((XMFLOAT3*)&plane, contactNormal);

    // Figure out the penetration distance...

    XMVECTOR penetrationDistTemp = XMVector3Dot(center - contactPoint, contactNormal) - radius;

    XMStoreFloat(&penetrationDistance, penetrationDistTemp);

    return XMVector4EqualInt(XMVectorAndCInt(intersection, noIntersection), XMVectorTrueInt());
}
