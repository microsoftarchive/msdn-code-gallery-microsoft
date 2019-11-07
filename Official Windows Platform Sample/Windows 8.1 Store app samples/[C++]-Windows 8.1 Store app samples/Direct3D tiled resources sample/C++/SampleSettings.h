//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

namespace TiledResources
{
    namespace SampleSettings
    {
        namespace Controls
        {
            static const Windows::System::VirtualKey Forward = Windows::System::VirtualKey::W;
            static const Windows::System::VirtualKey Left = Windows::System::VirtualKey::A;
            static const Windows::System::VirtualKey Back = Windows::System::VirtualKey::S;
            static const Windows::System::VirtualKey Right = Windows::System::VirtualKey::D;
            static const Windows::System::VirtualKey Up = Windows::System::VirtualKey::Space;
            static const Windows::System::VirtualKey Down = Windows::System::VirtualKey::Control;
            static const Windows::System::VirtualKey RollLeft = Windows::System::VirtualKey::Q;
            static const Windows::System::VirtualKey RollRight = Windows::System::VirtualKey::E;
            static const Windows::System::VirtualKey ToggleDebug = Windows::System::VirtualKey::Pause;
            static const Windows::System::VirtualKey ResetMappings = Windows::System::VirtualKey::Delete;
        }
        namespace CameraDynamics
        {
            static const float TranslationSpeed = 1.0f;
            static const float RotationSpeed = 60.0f * DirectX::XM_PI / 180.0f;
            static const float TransientRotationMultiplier = 1.0f / 500.0f;
        }
        namespace TileResidency
        {
            static const unsigned int PoolSizeInTiles = 256;
            static const unsigned int MaxSimultaneousFileLoadTasks = 10;
            static const unsigned int MaxTilesLoadedPerFrame = 100;
        }
        namespace TerrainAssets
        {
            namespace Diffuse
            {
                static const unsigned int DimensionSize = 1024;
                static const DXGI_FORMAT Format = DXGI_FORMAT_BC1_UNORM;
                static const unsigned int UnpackedMipCount = 2; // Set to Log2(DimensionSize / Standard Width of Format) + 1.
            }
            namespace Normal
            {
                static const unsigned int DimensionSize = 1024;
                static const DXGI_FORMAT Format =  DXGI_FORMAT_BC5_SNORM;
                static const unsigned int UnpackedMipCount = 3; // Set to Log2(DimensionSize / Standard Width of Format) + 1.
            }
        }
        namespace Sampling
        {
            static const float Ratio = 8.0f; // Ratio of screen size to sample target size.
            static const unsigned int SamplesPerFrame = 100;
        }
        static const UINT TileSizeInBytes = 0x10000; // Tiles are always 65536 Bytes.
    }
}
