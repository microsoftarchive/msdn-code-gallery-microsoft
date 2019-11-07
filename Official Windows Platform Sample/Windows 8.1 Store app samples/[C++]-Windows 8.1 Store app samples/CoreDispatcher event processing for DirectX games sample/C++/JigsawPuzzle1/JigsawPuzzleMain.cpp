//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "JigsawPuzzleMain.h"
#include "Common\DirectXHelper.h"

using namespace JigsawPuzzle;
using namespace Windows::Foundation;
using namespace Windows::System::Threading;
using namespace Concurrency;

// Loads and initializes application assets when the application is loaded.
JigsawPuzzleMain::JigsawPuzzleMain(const std::shared_ptr<DX::DeviceResources>& deviceResources, const std::shared_ptr<GameState>& gameState) :
    m_deviceResources(deviceResources)
{
    // Register to be notified if the Device is lost or recreated
    m_deviceResources->RegisterDeviceNotify(this);

    m_sceneRenderer = std::unique_ptr<JigsawPuzzleRenderer>(new JigsawPuzzleRenderer(m_deviceResources, gameState));
}

JigsawPuzzleMain::~JigsawPuzzleMain()
{
    // Deregister device notification
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

// Updates application state when the window size changes (e.g. device orientation change)
void JigsawPuzzleMain::CreateWindowSizeDependentResources() 
{
    m_sceneRenderer->CreateWindowSizeDependentResources();
}

// Renders a frame that represents the current state of the game.
void JigsawPuzzleMain::Render()
{
    m_sceneRenderer->Render();
}

// Notifies renderers that device resources need to be released.
void JigsawPuzzleMain::OnDeviceLost()
{
    m_sceneRenderer->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be recreated.
void JigsawPuzzleMain::OnDeviceRestored()
{
    m_sceneRenderer->CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}
