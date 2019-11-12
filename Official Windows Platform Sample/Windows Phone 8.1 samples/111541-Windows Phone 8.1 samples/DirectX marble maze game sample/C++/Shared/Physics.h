//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

// #define DISABLE_RAYCAST_REVERB

const float Gravity = 981.0f;                               // Strength of gravity (x100 for this simulation scale)
const float WallFriction = 0.05f;                           // Friction coefficient to apply to the walls.
const float MaxVelocity = 250.0f;                           // Upper bounds on marble velocity
const float MinVelocity = 0.1f;                             // Only used if you #define ENABLE_VELOCITY_CLAMP_TO_ZERO
const float MinAudibleCollisionSpeed = 10.0f;               // Minimum velocity a collision must have before reporting.
const float WallRestitutionCoefficient = 0.5f;              // How elastic the walls/floor are, from 0.0f to 1.0f
const float Damping = 1.0f;                                 // Only used if you #define ENABLE_VELOCITY_DAMPING
const float MaxPhysicsTimestep = 1.0f / 8.0f;               // Maximum time delta per physics update
const float PhysicsTimestep = 1.0f / 60.0f;                 // Time for single physics iteration


// Contains information regarding any contacts that occurred during the physics simulation. Used for audio feedback.
struct CollisionInfo
{
    BOOL        isRollingOnFloor;       // TRUE if the ball is currently rolling along the floor.
    BOOL        elasticCollision;       // TRUE if an elastic collision occurred (check maxCollisionSpeed for intensity)
    float       maxRollingSpeed;        // The absolute speed the ball was rolling along the surface
    float       maxCollisionSpeed;      // The strength of the collision
};

// Performs simple rigid-body collision of the ball against the mesh.
class Physics
{
    Collision*  m_collision;
    XMFLOAT3    m_previousPosition;
    XMFLOAT3    m_position;
    XMFLOAT3    m_velocity;
    XMFLOAT3    m_velDueToAccel;
    XMFLOAT3    m_acceleration;
    XMFLOAT3    m_gravity;
    float       m_radius;
    XMFLOAT3    m_maxRollingSpeed;
    XMFLOAT3    m_maxCollisionSpeed;
    float       m_currentRoomSize;
    float       m_wallDistances[8];
    float       m_accumulatedTime;

public:
    Physics();

    // Quick & dirty reset-teleport to a specific location in the mesh.
    void DebugReset()
    {
        ZeroMemory(&m_velocity, sizeof(XMFLOAT3));
        ZeroMemory(&m_velDueToAccel, sizeof(XMFLOAT3));
        ZeroMemory(&m_acceleration, sizeof(XMFLOAT3));

        SetPosition(XMFLOAT3(286.39178f, -191.74507f, -42.355251f));
    }

    // Calculates collision data for use by the audio sim (ricochet sounds, rolling along ground sounds)
    CollisionInfo GetCollisionInfo() const
    {
        CollisionInfo ci;
        float maxRoll = m_maxRollingSpeed.x;
        float maxColl = m_maxCollisionSpeed.x;
        ci.maxRollingSpeed = maxRoll;
        ci.maxCollisionSpeed = maxColl;
        ci.isRollingOnFloor = maxRoll > MinVelocity;
        ci.elasticCollision = maxColl > MinAudibleCollisionSpeed;
        return ci;
    }

    // Gets the current instance of the collision handling object associated with the physics simulator.
    Collision* GetCollisionEngine()
    {
        return m_collision;
    }

    // Gets the size of the room from the current position.
    float GetRoomSize()
    {
        return m_currentRoomSize;
    }

    // Gets the dimensions of the room (N, S, E, W, NE, NW, SE, SW) from the current position.
    int GetRoomDimensions(float *wallDistances, int size)
    {
        int returnedCount = min(ARRAYSIZE(m_wallDistances), size);
        memcpy(wallDistances, m_wallDistances, returnedCount*sizeof(float));
        return returnedCount;
    }

    // Sets the collision engine instance.
    void SetCollision(Collision *collision)
    {
        m_collision = collision;
    }

    // Sets the radius of the marble.
    void SetRadius(float radius)
    {
        m_radius = radius;
    }

    // Gets the radius of the marble.
    float GetRadius() const
    {
        return m_radius;
    }

    // Sets the unit normal associated with gravity, in mesh-local space.
    void SetGravity(const XMFLOAT3& gravityUnitNormal)
    {
        const XMVECTOR gravityMagnitude = XMVectorReplicate(Gravity);
        XMStoreFloat3(&m_gravity, XMLoadFloat3(&gravityUnitNormal) * gravityMagnitude);
    }

    // Sets the current position of the marble.
    void SetPosition(const XMFLOAT3& position)
    {
        m_position = position;
        m_previousPosition = position;
    }

    // Gets the current position of the marble.
    const XMFLOAT3& GetPosition() const
    {
        return m_position;
    }

    // Gets the previous position (from last frame) for the marble.
    const XMFLOAT3& GetPreviousPosition()
    {
        return m_previousPosition;
    }

    // Sets the current velocity of the marble.
    void SetVelocity(const XMFLOAT3& velocity)
    {
        m_velocity = velocity;
        m_velDueToAccel = XMFLOAT3(0.0f, 0.0f, 0.0f);
    }

    // Gets the current velocity of the marble.
    const XMFLOAT3& GetVelocity() const
    {
        return m_velocity;
    }

    // Gets the current acceleration operating on the marble (including resultant forces)
    const XMFLOAT3& GetAcceleration() const
    {
        return m_acceleration;
    }

    // Returns TRUE if the marble is moving at all. You may want to consider using GetCollisionInfo instead, as
    // that will tell you if it's rolling on or colliding with a surface.
    BOOL IsMoving() const
    {
        return !XMVector3InBounds(XMLoadFloat3(&m_velocity), XMVectorReplicate(FLT_EPSILON));
    }

    // Updates the physics simulation for the provided timestep.
    void UpdatePhysicsSimulation(float deltaT);

private:
    void ApplyForces();
    void IntegrateSimulation(float deltaT);
    BOOL FindInitialCollisions(float deltaT);
    void HandleCollisions(float deltaT);
    void Physics::CalculateCurrentRoomSize();
};

// Compute the intersection of a ray (Origin, Direction) with a triangle
// (V0, V1, V2).  Return TRUE if there is an intersection and also set *pDist
// to the distance along the ray to the intersection.
//
// The algorithm is based on Moller, Tomas and Trumbore, "Fast, Minimum Storage
// Ray-Triangle Intersection", Journal of Graphics Tools, vol. 2, no. 1,
// pp 21-28, 1997.
inline BOOL IntersectRayTriangle(
    FXMVECTOR origin,
    FXMVECTOR direction,
    FXMVECTOR V0,
    CXMVECTOR V1,
    CXMVECTOR V2,
    float* dist
    )
{

    static const XMVECTOR epsilon = XMVectorSet(1e-20f, 1e-20f, 1e-20f, 1e-20f);

    XMVECTOR zero = XMVectorZero();

    XMVECTOR e1 = V1 - V0;
    XMVECTOR e2 = V2 - V0;

    // p = direction ^ e2;
    XMVECTOR p = XMVector3Cross(direction, e2);

    // det = e1 * p;
    XMVECTOR det = XMVector3Dot(e1, p);

    XMVECTOR u, v, t;

    if (XMVector3GreaterOrEqual(det, epsilon))
    {
        // Determinate is positive (front side of the triangle).
        XMVECTOR s = origin - V0;

        // u = s * p;
        u = XMVector3Dot(s, p);

        XMVECTOR noIntersection = XMVectorLess(u, zero);
        noIntersection = XMVectorOrInt(noIntersection, XMVectorGreater(u, det));

        // q = s ^ e1;
        XMVECTOR q = XMVector3Cross(s, e1);

        // v = direction * q;
        v = XMVector3Dot(direction, q);

        noIntersection = XMVectorOrInt(noIntersection, XMVectorLess(v, zero));
        noIntersection = XMVectorOrInt(noIntersection, XMVectorGreater(u + v, det));

        // t = e2 * q;
        t = XMVector3Dot(e2, q);

        noIntersection = XMVectorOrInt(noIntersection, XMVectorLess(t, zero));

        if (XMVector4EqualInt(noIntersection, XMVectorTrueInt()))
        {
            return FALSE;
        }
    }
    else if (XMVector3LessOrEqual(det, -epsilon))
    {
        // Determinate is negative (back side of the triangle).
        XMVECTOR s = origin - V0;

        // u = s * p;
        u = XMVector3Dot(s, p);

        XMVECTOR noIntersection = XMVectorGreater(u, zero);
        noIntersection = XMVectorOrInt(noIntersection, XMVectorLess(u, det));

        // q = s ^ e1;
        XMVECTOR q = XMVector3Cross(s, e1);

        // v = direction * q;
        v = XMVector3Dot(direction, q);

        noIntersection = XMVectorOrInt(noIntersection, XMVectorGreater(v, zero));
        noIntersection = XMVectorOrInt(noIntersection, XMVectorLess(u + v, det));

        // t = e2 * q;
        t = XMVector3Dot(e2, q);

        noIntersection = XMVectorOrInt(noIntersection, XMVectorGreater(t, zero));

        if (XMVector4EqualInt(noIntersection, XMVectorTrueInt()))
        {
            return FALSE;
        }
    }
    else
    {
        // Parallel ray.
        return FALSE;
    }

    XMVECTOR inv_det = XMVectorReciprocal(det);

    t *= inv_det;

    // u * inv_det and v * inv_det are the barycentric cooridinates of the intersection.

    // Store the x-component to dist
    XMStoreFloat(dist, t);

    return TRUE;
};
