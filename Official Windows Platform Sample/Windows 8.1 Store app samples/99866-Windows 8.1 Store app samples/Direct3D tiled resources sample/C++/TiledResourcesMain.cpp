//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "TiledResourcesMain.h"
#include "SampleSettings.h"

using namespace TiledResources;

using namespace concurrency;
using namespace DirectX;
using namespace Windows::System;

// Loads and initializes application assets when the application is loaded.
TiledResourcesMain::TiledResourcesMain(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_renderersLoaded(false),
    m_debugMode(false),
    m_pointerDown(false),
    m_pointerId(-1)
{
    // Register to be notified if the Device is lost or recreated.
    m_deviceResources->RegisterDeviceNotify(this);

    // Create the renderers and the residency manager.
    m_terrainRenderer = std::unique_ptr<TerrainRenderer>(new TerrainRenderer(m_deviceResources));
    m_samplingRenderer = std::unique_ptr<SamplingRenderer>(new SamplingRenderer(m_deviceResources));
    m_residencyManager = std::unique_ptr<ResidencyManager>(new ResidencyManager(m_deviceResources));

    m_samplingRenderer->SetDebugMode(m_debugMode);
    m_residencyManager->SetDebugMode(m_debugMode);

    CreateDeviceDependentResourcesAsync().then([this]()
    {
        m_renderersLoaded = true;
    });

    m_camera.SetViewParameters(XMFLOAT3(0.0f, 1.7f, -0.8f), XMFLOAT3(0.0f, 0.0f, 0.0f), XMFLOAT3(0.0f, 0.0f, 1.0f));
    m_camera.SetProjectionParameters(m_deviceResources->GetScreenViewport().Width, m_deviceResources->GetScreenViewport().Height);

    ZeroMemory(&m_controlState, sizeof(m_controlState));

    m_controlsOverlay = std::unique_ptr<ControlsOverlay>(new ControlsOverlay(m_deviceResources));

    m_sampleOverlay = std::unique_ptr<SampleOverlay>(new SampleOverlay(m_deviceResources, L"Direct3D Tiled Resources"));
}

TiledResourcesMain::~TiledResourcesMain()
{
    // Deregister device notification.
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

task<void> TiledResourcesMain::CreateDeviceDependentResourcesAsync()
{
    auto terrainRendererCreationTask = m_terrainRenderer->CreateDeviceDependentResourcesAsync();
    auto samplingRendererCreationTask = m_samplingRenderer->CreateDeviceDependentResourcesAsync();
    auto residencyManagerCreationTask = m_residencyManager->CreateDeviceDependentResourcesAsync();

    // Register the terrain renderer's tiled resources with the residency manager.
    auto diffuseResidencyMap = m_residencyManager->ManageTexture(m_terrainRenderer->GetDiffuseTexture(), L"diffuse.bin");
    m_terrainRenderer->SetDiffuseResidencyMap(diffuseResidencyMap);
    auto normalResidencyMap = m_residencyManager->ManageTexture(m_terrainRenderer->GetNormalTexture(), L"normal.bin");
    m_terrainRenderer->SetNormalResidencyMap(normalResidencyMap);

    m_residencyManager->ResetTileMappings();

    return (terrainRendererCreationTask && samplingRendererCreationTask && residencyManagerCreationTask);
}

// Updates application state when the window size changes (e.g. device orientation change)
void TiledResourcesMain::UpdateForWindowSizeChange()
{
    m_samplingRenderer->CreateWindowSizeDependentResources();
    m_sampleOverlay->CreateWindowSizeDependentResources();
    m_camera.SetProjectionParameters(m_deviceResources->GetScreenViewport().Width, m_deviceResources->GetScreenViewport().Height);
}

// Updates the application state once per frame.
void TiledResourcesMain::Update()
{
    // Update scene objects.
    m_timer.Tick([&]()
    {
        // Update camera control.
        float timeFactor = static_cast<float>(m_timer.GetElapsedSeconds());

        m_camera.ApplyTranslation(XMFLOAT3(
            ((m_controlState.vxp ? 1.0f : 0.0f) - (m_controlState.vxn ? 1.0f : 0.0f)) * SampleSettings::CameraDynamics::TranslationSpeed * timeFactor,
            ((m_controlState.vyp ? 1.0f : 0.0f) - (m_controlState.vyn ? 1.0f : 0.0f)) * SampleSettings::CameraDynamics::TranslationSpeed * timeFactor,
            ((m_controlState.vzp ? 1.0f : 0.0f) - (m_controlState.vzn ? 1.0f : 0.0f)) * SampleSettings::CameraDynamics::TranslationSpeed * timeFactor
            ));

        m_camera.ApplyRotation(XMFLOAT3(
            m_controlState.vrx * SampleSettings::CameraDynamics::TransientRotationMultiplier,
            m_controlState.vry * SampleSettings::CameraDynamics::TransientRotationMultiplier,
            ((m_controlState.vrzp ? 1.0f : 0.0f) - (m_controlState.vrzn ? 1.0f : 0.0f)) * SampleSettings::CameraDynamics::RotationSpeed * timeFactor
            ));

        // Clear transient control state.
        if (!m_pointerDown)
        {
            m_controlState.vrx = 0.0f;
            m_controlState.vry = 0.0f;
        }
    });
}

// Renders the current frame according to the current application state.
// Returns true if the frame was rendered and is ready to be displayed.
bool TiledResourcesMain::Render()
{
    // Don't try to render anything before the first Update.
    if (m_timer.GetFrameCount() == 0 || !m_renderersLoaded)
    {
        return false;
    }

    // Render the scene objects.
    m_terrainRenderer->SetSourceGeometry(m_camera);
    m_samplingRenderer->SetTargetsForSampling();
    m_terrainRenderer->Draw();
    m_terrainRenderer->SetSourceGeometry(m_camera);
    m_terrainRenderer->SetTargetsForRendering(m_camera, m_timer);
    m_terrainRenderer->Draw();
    m_samplingRenderer->RenderVisualization();
    auto samples = m_samplingRenderer->CollectSamples();
    m_residencyManager->EnqueueSamples(samples, m_timer);
    m_residencyManager->ProcessQueues();
    m_residencyManager->RenderVisualization();
    m_controlsOverlay->Render();

    // Render the sample title text.
    m_sampleOverlay->Render();

    return true;
}

// Notifies renderers that device resources need to be released.
void TiledResourcesMain::OnDeviceLost()
{
    m_renderersLoaded = false;

    m_terrainRenderer->ReleaseDeviceDependentResources();
    m_samplingRenderer->ReleaseDeviceDependentResources();
    m_residencyManager->ReleaseDeviceDependentResources();
    m_controlsOverlay->ReleaseDeviceDependentResources();

    m_sampleOverlay->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be re-created.
void TiledResourcesMain::OnDeviceRestored()
{
    m_terrainRenderer->CreateDeviceDependentResources();
    m_samplingRenderer->CreateDeviceDependentResources();
    m_residencyManager->CreateDeviceDependentResources();

    CreateDeviceDependentResourcesAsync().then([this]()
    {
        m_renderersLoaded = true;
    });

    m_controlsOverlay->CreateDeviceDependentResources();

    m_sampleOverlay->CreateDeviceDependentResources();
    UpdateForWindowSizeChange();
}

void TiledResourcesMain::OnMouseMoved(float dx, float dy)
{
    m_controlState.vrx += -dy;
    m_controlState.vry += -dx;
}

void TiledResourcesMain::OnKeyChanged(VirtualKey key, bool down)
{
    switch (key)
    {
    case SampleSettings::Controls::Right:
        m_controlState.vxp = down;
        break;
    case SampleSettings::Controls::Left:
        m_controlState.vxn = down;
        break;
    case SampleSettings::Controls::Up:
        m_controlState.vyp = down;
        break;
    case SampleSettings::Controls::Down:
        m_controlState.vyn = down;
        break;
    case SampleSettings::Controls::Back:
        m_controlState.vzp = down;
        break;
    case SampleSettings::Controls::Forward:
        m_controlState.vzn = down;
        break;
    case SampleSettings::Controls::RollLeft:
        m_controlState.vrzp = down;
        break;
    case SampleSettings::Controls::RollRight:
        m_controlState.vrzn = down;
        break;
    case SampleSettings::Controls::ToggleDebug:
        if (down)
        {
            m_debugMode = !m_debugMode;
            m_samplingRenderer->SetDebugMode(m_debugMode);
            m_residencyManager->SetDebugMode(m_debugMode);
        }
        break;
    case SampleSettings::Controls::ResetMappings:
        if (down)
        {
            m_residencyManager->ResetTileMappings();
        }
    default:
        break;
    }
}

void TiledResourcesMain::OnRightClick(float x, float y)
{
    m_samplingRenderer->DebugSample(x, y);
}

void TiledResourcesMain::OnPointerPressed(unsigned int id)
{
    if (!m_pointerDown)
    {
        m_pointerDown = true;
        m_pointerId = id;
    }
}

void TiledResourcesMain::OnPointerReleased(unsigned int id)
{
    if (id == m_pointerId)
    {
        m_pointerDown = false;
    }
}

void TiledResourcesMain::UpdatePointerPosition(unsigned int id, float x, float y)
{
    if (id == m_pointerId)
    {
        float ox, oy, oz, orx, ory, orz;
        m_controlsOverlay->EvaluateTouchControl(x, y, ox, oy, oz, orx, ory, orz);
        m_controlState.vxp = ox > 0;
        m_controlState.vxn = ox < 0;
        m_controlState.vyp = oy > 0;
        m_controlState.vyn = oy < 0;
        m_controlState.vzp = oz > 0;
        m_controlState.vzn = oz < 0;
        m_controlState.vrx = orx * 20.0f;
        m_controlState.vry = ory * 20.0f;
        m_controlState.vrzp = orz > 0;
        m_controlState.vrzn = orz < 0;
    }
}
