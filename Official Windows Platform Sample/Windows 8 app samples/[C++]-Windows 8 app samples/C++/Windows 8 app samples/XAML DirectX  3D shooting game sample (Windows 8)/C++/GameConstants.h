//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

namespace GameConstants
{
    static const int MaxAmmo                    = 10;
    static const int MaxCylinders               = 5;
    static const int MaxTargets                 = 10;
    static const float AmmoSize                 = 0.2f;
    static const float AmmoRadius               = AmmoSize * 0.5f;
    static const float TouchRectangleSize       = 300.0f;
    static const int MaxBackgroundTextures      = 3;

    static const float CrossHairHalfSize        = 20.0f;
    static const float HudSafeWidth             = 300.0f;
    static const float HudRightOffset           = 250.0f;
    static const float HudTopOffset             = 50.0f;
    static const float Margin                   = 5.0f;
    static const float HudBodyPointSize         = 24.0f;
    static const float HudTitleHeaderPointSize  = 16.0f;
    static const float HudTitleBodyPointSize    = 36.0f;
    static const float HudLicensePointSize      = 20.0f;

    static const int InitialLoadingDelay        = 2000;     // Number of ms to wait before completion of initial load.
    static const int LevelLoadingDelay          = 500;      // Number of ms to wait before completion of level load.

    static const int WorldFloorId               = 80001;
    static const int WorldCeilingId             = 80002;
    static const int WorldWallsId               = 80003;

    namespace Physics
    {
        static const float GroundRestitution    = 0.8f;     // Percentage of the velocity transmitted by ground and walls when an ammo hit.
        static const float Friction             = 0.9f;     // The amount of velocity retained after friction is applied.
        static const float AutoFireDelay        = 0.2f;     // The period between two successive ammo firing.
        static const float Gravity              = 5.0f;     // The magnitude of the downward force applied to ammos.
        static const float BounceTransfer       = 0.8f;     // The proportion of velocity transferred during a collision between 2 ammos.
        static const float BounceLost           = 0.1f;     // The proportion of velocity lost during a collision between 2 ammos.
        static const float RestThreshold        = 0.02f;    // The energy below which the ball is flagged as laying on ground.
                                                            // It is defined as Gravity * Height_above_ground + 0.5 * Velocity * Velocity.
        static const float FrameLength          = 0.003f;   // The duration of a frame for physics handling when the graphics frame length is too long.
    }

    namespace Sound
    {
        static const float MaxVelocity          = 10.0f;    // The velocity at which the bouncing sound is played at maximum volume.
        static const float MinVelocity          = 0.05f;    // The minimum contact velocity required to make a sound.
        static const float MinAdjustment        = 0.2f;     // The minimum volume adjustment based on contact velocity.
    }
};
