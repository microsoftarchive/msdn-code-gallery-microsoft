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
    m_deviceResources(deviceResources),
    m_state(gameState),
    m_renderLoopWorker(nullptr)
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

// 4. Support for rendering on a background thread.

void JigsawPuzzleMain::StartRenderThread()
{
    // If the render thread is already running then do not start another one.
    if (IsRendering())
    {
        return;
    }

    // Create a task that will be run on a background thread.
    auto workItemHandler = ref new WorkItemHandler([this](IAsyncAction^ action)
    {
        // Notify the swap chain that this app intends to render each frame faster
        // than the display's vertical refresh rate (typically 60Hz). Apps that cannot
        // deliver frames this quickly should set this to 2.
        m_deviceResources->SetMaximumFrameLatency(1);

        // Calculate the updated frame and render once per vertical blanking interval.
        while (action->Status == AsyncStatus::Started)
        {
            // Execute any work items that have been queued by the input thread.
            ProcessPendingWork();

            // Take a snapshot of the current game state. This allows the renderers to work with a
            // set of values that won't be changed while the input thread continues to process events.
            m_state->SnapState();

            m_sceneRenderer->Render();
            m_deviceResources->Present();
        }

        // Ensure that all pending work items have been processed before terminating the thread.
        ProcessPendingWork();
    });

    // Run the task on a dedicated high priority background thread.
    m_renderLoopWorker = ThreadPool::RunAsync(workItemHandler, WorkItemPriority::High, WorkItemOptions::TimeSliced);
}

void JigsawPuzzleMain::StopRenderThread()
{
    if (IsRendering())
    {
        m_renderLoopWorker->Cancel();
    }
}

// Schedule work to be performed on the render thread.  If the render thread is no longer active,
// perform the work item on the current thread.
void JigsawPuzzleMain::BeginInvoke(std::function<void()> function)
{
    {
        critical_section::scoped_lock lock(m_criticalSection);
        m_pendingWork.push(function);
    }

    if (!IsRendering())
    {
        ProcessPendingWork();
    }
}

// Process any outstanding work items coming from the input thread.
void JigsawPuzzleMain::ProcessPendingWork()
{
    critical_section::scoped_lock lock(m_criticalSection);
    while (!m_pendingWork.empty())
    {
        m_pendingWork.front()();
        m_pendingWork.pop();
    }
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
