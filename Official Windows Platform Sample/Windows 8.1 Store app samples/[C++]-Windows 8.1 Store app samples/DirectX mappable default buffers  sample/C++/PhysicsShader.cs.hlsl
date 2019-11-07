//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// This number squared must match the 'ProjectileCountPerDispatch'
// value in the Renderer class.
static const int ThreadCountPerDimension = 256;
static const int ProjectileCount = ThreadCountPerDimension * 4;

struct Projectile
{
    float2 position;
    float2 velocity;
};

RWStructuredBuffer<Projectile> projectiles;
RWBuffer<int> collisions;

[numthreads(ThreadCountPerDimension, 1, 1)]
void main( uint3 DTid : SV_DispatchThreadID )
{
    // The DispatchThreadID determines which projectile in the
    // projectile Buffer to perform calculations on.
    int projectileIndex = DTid.x;

    projectiles[projectileIndex].position.x += projectiles[projectileIndex].velocity.x;
    projectiles[projectileIndex].position.y += projectiles[projectileIndex].velocity.y;

    // Perform simplified gravity calculations.
    for (int i = 0; i < ProjectileCount; i++)
    {
        projectiles[projectileIndex].velocity.x += (projectiles[i].position.x - projectiles[projectileIndex].position.x) / 10000000.0f;
        projectiles[projectileIndex].velocity.y += (projectiles[i].position.y - projectiles[projectileIndex].position.y) / 10000000.0f;
    }
    
    // If projectile is about to leave the left side of the screen, reverse velocity.
    if (projectiles[projectileIndex].position.x <= 0)
    {
        projectiles[projectileIndex].velocity.x *= -1;
        projectiles[projectileIndex].position.x = 0.001f;
        InterlockedAdd(collisions[0], 1); // Increment collision in DirectCompute thread-safe manner.
    }

    // If projectile is about to leave the right side of the screen, reverse velocity.
    if (projectiles[projectileIndex].position.x >= 1)
    {
        projectiles[projectileIndex].velocity.x *= -1;
        projectiles[projectileIndex].position.x = 0.999f;
        InterlockedAdd(collisions[0], 1); // Increment collision in DirectCompute thread-safe manner.
    }

    // If projectile is about to leave the top side of the screen, reverse velocity.
    if (projectiles[projectileIndex].position.y <= 0)
    {
        projectiles[projectileIndex].velocity.y *= -1;
        projectiles[projectileIndex].position.y = 0.001f;
        InterlockedAdd(collisions[0], 1); // Increment collision in DirectCompute thread-safe manner.
    }

    // If projectile is about to leave the bottom side of the screen, reverse velocity.
    if (projectiles[projectileIndex].position.y >= 1)
    {
        projectiles[projectileIndex].velocity.y *= -1;
        projectiles[projectileIndex].position.y = 0.999f;
        InterlockedAdd(collisions[0], 1); // Increment collision in DirectCompute thread-safe manner.
    }
}