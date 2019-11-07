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

static const XMVECTOR wallFrictionCoeff = XMVectorReplicate(WallFriction);

// Updates the physics simulation for the timestep provided.
void Physics::UpdatePhysicsSimulation(float deltaT)
{
    // If the simulation is not advancing, do nothing.
    if (deltaT >= 0.0f && deltaT <= FLT_EPSILON)
    {
        return;
    }

    // Clamp the maximum timestep in case we're debugging.
    deltaT = min(deltaT, MaxPhysicsTimestep);

    // Keep the previous position
    m_previousPosition = m_position;

    // Update physics with fixed timestep, multiple iterations up to MaxPhysicsTimestep total
    m_accumulatedTime += deltaT;

    while (m_accumulatedTime >= PhysicsTimestep)
    {
        m_accumulatedTime -= PhysicsTimestep;
        ApplyForces();  // Gravity.
        IntegrateSimulation(PhysicsTimestep);    // Integrate for this frame.
        FindInitialCollisions(PhysicsTimestep);
        HandleCollisions(PhysicsTimestep);
    }

    CalculateCurrentRoomSize();
}

// Updates the forces acting on the sphere, including acceleration due to gravity and friction.
void Physics::ApplyForces()
{
    const XMVECTOR gravity = XMLoadFloat3(&m_gravity);

    // We only have gravity acting on the ball - so this step is really simple.
    // More forces could be added here on a per-frame basis.

    XMStoreFloat3(&m_acceleration, gravity);
}

Physics::Physics()
{
    ZeroMemory(&m_velDueToAccel, sizeof(XMFLOAT3));
    ZeroMemory(&m_position, sizeof(m_position));
    ZeroMemory(&m_velocity, sizeof(m_velocity));
    ZeroMemory(&m_velDueToAccel, sizeof(m_velDueToAccel));
    ZeroMemory(&m_acceleration, sizeof(m_acceleration));
    ZeroMemory(&m_gravity, sizeof(m_gravity));
    ZeroMemory(&m_maxCollisionSpeed, sizeof(m_maxCollisionSpeed));
    ZeroMemory(&m_maxRollingSpeed, sizeof(m_maxRollingSpeed));

    m_radius = 0.0f;
    m_accumulatedTime = 0.0f;

    // Default the room size in case room size is static.
    m_currentRoomSize = 300.0f;
}

// Integrates the acceleration to give the ball's new velocity, and the velocity to give the new position.
void Physics::IntegrateSimulation(float deltaT)
{
    XMVECTOR deltaTime = XMVectorReplicate(deltaT);
    XMVECTOR position = XMLoadFloat3(&m_position);
    XMVECTOR velocity = XMLoadFloat3(&m_velocity);
    XMVECTOR acceleration = XMLoadFloat3(&m_acceleration);

    // Calculate new velocity due to acceleration for the time-slice.

    XMVECTOR velDueToAccel =  acceleration * deltaTime;
    XMStoreFloat3(&m_velDueToAccel, velDueToAccel);
    velocity = velocity + velDueToAccel;

    // const XMVECTOR vHalf = {0.5f, 0.5f, 0.5f, 0.5f};
    XMVECTOR newPosition = position + velocity * deltaTime;
    XMStoreFloat3(&m_position, newPosition);

    // Damping & stabilization

#if defined(ENABLE_VELOCITY_DAMPING)

    // Continuously drain energy from the ball.

    XMVECTOR speed = XMVector3Length(velocity);

    const XMVECTOR velDampAmount = XMVectorReplicate(Damping);
    XMVECTOR velUnit = XMVector3Normalize(velocity);

    XMVECTOR dampTotal = XMVectorMin(velDampAmount, speed);
    velocity = velocity - (dampTotal * velUnit);

#endif // defined(ENABLE_VELOCITY_DAMPING)

    // Make sure velocity stays within a safe range and doesn't spiral off, by clamping it.
    XMVECTOR velSpeed = XMVector3Length(velocity);

    const XMVECTOR maxVelocity = XMVectorReplicate(MaxVelocity);
    velSpeed = XMVectorMin(maxVelocity, velSpeed);
    velocity = velSpeed * XMVector3Normalize(velocity);

#if defined(ENABLE_VELOCITY_CLAMP_TO_ZERO)

    // If velocity is small enough (-1 <= v <= 1), set it to 0.

    const XMVECTOR velSq = XMVector3LengthSq(velocity);

    const XMVECTOR minVelocitySq = XMVectorReplicate(MinVelocity * MinVelocity);

    const XMVECTOR select = XMVectorLess(velSq, minVelocitySq);
    velocity = XMVectorSelect(velocity, XMVectorZero(), select);

#endif // defined(ENABLE_VELOCITY_CLAMP_TO_ZERO)

    XMStoreFloat3(&m_velocity, velocity);
}

// Finds the initial set of collisions
BOOL Physics::FindInitialCollisions(float deltaT)
{
    XMVECTOR velocity = XMLoadFloat3(&m_velocity);
    Sphere bounds(m_position, m_radius);
    return m_collision->BuildCollisionListForSphere(bounds, velocity);
}

// Handles elastic collisions, static contacts (resting against surface) and moving interpenetrating objects
// out of each other. This is all done as a single step, as it improves the mathematical stability of the
// engine.
void Physics::HandleCollisions(float deltaT)
{
    XMVECTOR velDueToGrav = XMLoadFloat3(&m_velDueToAccel);
    XMVECTOR velocity = XMLoadFloat3(&m_velocity);
    XMVECTOR position = XMLoadFloat3(&m_position);
    XMVECTOR radius = XMVectorReplicatePtr(&m_radius);
    XMVECTOR maxRollingSpeed = XMVectorZero();
    XMVECTOR maxCollisionSpeed = XMVectorZero();
    XMVECTOR deltaTime = XMVectorReplicate(deltaT);

    const size_t MAX_COLLISION_ITERATIONS = 8;
    for (size_t i = 0; i < MAX_COLLISION_ITERATIONS; ++i)
    {
        // Find the collision with the biggest intrusion into the mesh, that is facing in the opposite direction to the
        // way the ball is moving (i.e. has a positive closing velocity).
        Contact* contact = m_collision->FindWorstInterpenetration();
        if (contact == nullptr)
            break;

        XMVECTOR surfaceNormal = contact->GetSurfaceNormal();

        XMVECTOR velocityDotNormal = XMVector3Dot(surfaceNormal, velocity);
        XMVECTOR velGravDotNormal = XMVector3Dot(surfaceNormal, velDueToGrav);

        // Estimate how much the ball would have fallen into the mesh due to gravity this frame, if it started out
        // resting against it. If it's almost the same as the actual penetration, we must have been resting to start
        // with, so we can immediately cancel out this contribution.
        XMVECTOR penetrationDistance = XMVectorReplicatePtr(&contact->penetrationDistance);
        XMVECTOR distDueToGrav = velGravDotNormal * deltaTime;
        const XMVECTOR tolerance = XMVectorSet(0.1f, 0.1f, 0.1f, 0.1f);
        if (XMVector3NearEqual(penetrationDistance, distDueToGrav, tolerance))
        {
            // Contact

            // Kill velocity in surface-normal direction entirely.
            velocity = velocity - surfaceNormal * velocityDotNormal;

            XMVECTOR speed = XMVector3Length(velocity);
            XMVECTOR velocityDir = XMVector3Normalize(velocity); // could optimize...

            // Find acceleration into surface
            XMVECTOR accel = XMLoadFloat3(&m_acceleration);
            XMVECTOR ADotN = XMVector3Dot(accel, surfaceNormal);
            XMVECTOR select = XMVectorLessOrEqual(ADotN, XMVectorZero());
            XMVECTOR accelSurface = XMVectorSelect(XMVectorZero(), -ADotN, select);

            // Handle friction
            XMVECTOR frictionResponse =  accelSurface * wallFrictionCoeff * deltaTime;
            velocity = velocity - velocityDir * XMVectorMin(speed, frictionResponse);

            maxRollingSpeed = XMVectorMax(XMVector3LengthSq(velocityDir * velocity), maxRollingSpeed);

            // Make sure we're no longer in the mesh.
            position = contact->Resolve(position, radius);
        }
        else
        {
            // Elastic collision - reflect our velocity in the direction of the normal.
            float negRestitutionScalar = - (1.0f + WallRestitutionCoefficient);
            XMVECTOR negRestitution = XMVectorReplicate(negRestitutionScalar);

            // Keep track of max collision speed for the audio sim.
            maxCollisionSpeed = XMVectorMax(maxCollisionSpeed, velocityDotNormal * velocityDotNormal);

            // Update the velocity.
            velocity = velocity + (negRestitution * velocityDotNormal * surfaceNormal);

            // Make sure we're no longer in the mesh.
            position = contact->Resolve(position, radius);
        }

        // Remove this contact from consideration for this iteration.
        contact->Invalidate();

        // Update the penetration of the ball.
        m_collision->UpdateInterpenetrations(position, radius, velocity);
    }

    XMStoreFloat3(&m_velocity, velocity);
    XMStoreFloat3(&m_position, position);
    XMStoreFloat3(&m_maxRollingSpeed, XMVectorSqrt(maxRollingSpeed));
    XMStoreFloat3(&m_maxCollisionSpeed, XMVectorSqrt(maxCollisionSpeed));
}

// Calculates the size of the current room, used to determine reverb parameters.
// This function calculates the size of the current room, using both a square and a diamond raycast,
// which allows for smoother transitions when rounding corners.
void Physics::CalculateCurrentRoomSize()
{
#if !defined(DISABLE_RAYCAST_REVERB)
    static BOOL squarePattern = true;
    float wallDistances[4] = {FLT_MAX, FLT_MAX, FLT_MAX, FLT_MAX};
    XMVECTOR position = XMLoadFloat3(&m_position);
    size_t triCount = m_collision->m_wallTriList.size();
    XMVECTOR rayDirections[4];
    if (squarePattern)
    {
        rayDirections[0] = XMVectorSet( 0.0f,  1.0f, 0.0f, 0.0f);    // North
        rayDirections[1] = XMVectorSet( 0.0f, -1.0f, 0.0f, 0.0f);    // South
        rayDirections[2] = XMVectorSet(-1.0f,  0.0f, 0.0f, 0.0f);    // East
        rayDirections[3] = XMVectorSet( 1.0f,  0.0f, 0.0f, 0.0f);    // West
    }
    else
    {
        rayDirections[0] = XMVectorSet( 0.5f, -0.5f, 0.0f, 0.0f);    // NE
        rayDirections[1] = XMVectorSet( 0.5f,  0.5f, 0.0f, 0.0f);    // NW
        rayDirections[2] = XMVectorSet(-0.5f, -0.5f, 0.0f, 0.0f);    // SE
        rayDirections[3] = XMVectorSet(-0.5f,  0.5f, 0.0f, 0.0f);    // SW
    }
    float distance = 0.0f;

    // Loop through all triangles in the mesh, looking for intersections with a cast ray.
    for (size_t i = 0; i < triCount; ++i)
    {
        XMVECTOR A = XMLoadFloat3(&m_collision->m_wallTriList[i].A);
        XMVECTOR B = XMLoadFloat3(&m_collision->m_wallTriList[i].B);
        XMVECTOR C = XMLoadFloat3(&m_collision->m_wallTriList[i].C);

        // Loop through the cardinal directions, testing this triangle to see if it's in the path of a cast ray.
        for (int j = 0; j < 4; j++)
        {
            if (IntersectRayTriangle(position, rayDirections[j], A, B, C, &distance))
            {
                if (wallDistances[j] > distance)
                {
                    wallDistances[j] = distance;
                }
            }

        }
    }

    if (squarePattern)
    {
        memcpy(&m_wallDistances, wallDistances, 4*sizeof(float));
    }
    else
    {
        memcpy(&m_wallDistances[4], wallDistances, 4*sizeof(float));
    }

    // Default the room size to 1000.0f
    float roomSize = 1000.0f;
    for (int i = 0; i < 8; i++)
    {
        if (m_wallDistances[i] == FLT_MAX) // Some walls will not exist if the marble is outside.
        {
            m_wallDistances[i] = 1000.0f;
        }
    }

    // Calculate the size of the room, averaging the rectangular with the diagonal measurments.
    roomSize = sqrt(
        ((m_wallDistances[0] + m_wallDistances[1]) * (m_wallDistances[2]+m_wallDistances[3]) +
         (m_wallDistances[4] + m_wallDistances[5]) * (m_wallDistances[6]+m_wallDistances[7]))
        / 2.0f
        );

    if (roomSize > 1000.0f)
    {
        roomSize = 1000.0f;
    }

    m_currentRoomSize = roomSize;
    squarePattern = !squarePattern;
#endif
}
