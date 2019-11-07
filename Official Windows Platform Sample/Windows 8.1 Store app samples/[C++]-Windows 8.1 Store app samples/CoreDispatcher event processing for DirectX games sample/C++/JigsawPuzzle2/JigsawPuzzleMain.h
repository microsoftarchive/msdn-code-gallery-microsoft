//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "Common\DeviceResources.h"
#include "Content\JigsawPuzzleRenderer.h"

// Renders Direct2D and 3D content on the screen.
namespace JigsawPuzzle
{
    class JigsawPuzzleMain : public DX::IDeviceNotify
    {
    public:
        JigsawPuzzleMain(const std::shared_ptr<DX::DeviceResources>& deviceResources, const std::shared_ptr<GameState>& gameState);
        ~JigsawPuzzleMain();
        void CreateWindowSizeDependentResources();
        void Render();

        // IDeviceNotify
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // The game content renderer.
        std::unique_ptr<JigsawPuzzleRenderer> m_sceneRenderer;
    };
}