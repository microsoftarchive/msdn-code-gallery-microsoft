//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

// The material types for each of the collision meshes
enum class MeshID
{
    Wall,
    Ground,
    Floor,
};

// A contact or penetration of the mesh with the ball
// Using a struct so that this can be stack allocated
struct Contact
{
    XMFLOAT4 plane;
    XMFLOAT3 contactPosition;
    float closingVelocity;
    float penetrationDistance;
    BOOL contactIsEdge;
    Triangle triangle;
    MeshID meshID;
    size_t triIndex;

    Contact(const Triangle& tri, size_t index, MeshID mesh) :
        triangle(tri),
        triIndex(index),
        meshID(mesh)
    {
        plane = tri.plane;
    }

    FORCEINLINE XMVECTOR GetSurfaceNormal() const
    {
        return XMLoadFloat3((XMFLOAT3*) &plane);
    }

    BOOL IsColliding() const
    {
        const float MIN_COLLISION_DISTANCE = -1.0E-5f;
        return penetrationDistance <= MIN_COLLISION_DISTANCE;
    }

    void Invalidate()
    {
        penetrationDistance = FLT_MAX;
    }

    XMVECTOR Resolve(FXMVECTOR position, FXMVECTOR radius);

    BOOL CalculateContact(FXMVECTOR position, FXMVECTOR radiusIn, FXMVECTOR path);
};

// The collision engine
class Collision
{
private:
    BOOL m_intersectsGround;

public:
    std::vector<Triangle> m_groundTriList;
    std::vector<Triangle> m_wallTriList;
    std::vector<Triangle> m_floorTriList;
    std::vector<Contact> m_collisions;

    Collision() :
        m_intersectsGround(FALSE)
    {
    }

    inline BOOL IntersectsWithGround() const
    {
        return m_intersectsGround;
    }

    BOOL BuildCollisionListForSphere(const Sphere& meshLocalSpace, FXMVECTOR path);
    Contact* FindWorstInterpenetration();
    void UpdateInterpenetrations(FXMVECTOR newPosition, FXMVECTOR radius, FXMVECTOR path);

private:
    void MergeSharedEdgeCoplanarContacts(FXMVECTOR sphere, FXMVECTOR radius, FXMVECTOR path);
    BOOL AccumulateSphereTriangleIntersections(
        FXMVECTOR sphere,
        FXMVECTOR radius,
        FXMVECTOR path,
        MeshID mesh,
        const std::vector<Triangle>& triList
        );
};
