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

        // 4. Methods allowing interaction with the rendering thread.
        void StartRenderThread();
        void StopRenderThread();
        void BeginInvoke(std::function<void()> function);

        // IDeviceNotify
        virtual void OnDeviceLost();
        virtual void OnDeviceRestored();

    private:
        // 4. Threading support for the render loop.
        void ProcessPendingWork();
        bool IsRendering()
        {
            return m_renderLoopWorker != nullptr && m_renderLoopWorker->Status == Windows::Foundation::AsyncStatus::Started;
        }

        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;
        std::shared_ptr<GameState> m_state;

        // The game content renderer.
        std::unique_ptr<JigsawPuzzleRenderer> m_sceneRenderer;

        // 4. Threading support for the render loop.
        Windows::Foundation::IAsyncAction^ m_renderLoopWorker;
        std::queue<std::function<void()>> m_pendingWork;
        Concurrency::critical_section m_criticalSection;
    };
}