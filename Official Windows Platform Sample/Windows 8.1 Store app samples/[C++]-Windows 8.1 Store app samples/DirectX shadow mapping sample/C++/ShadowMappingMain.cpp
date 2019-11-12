//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ShadowMappingMain.h"

#include <DirectXColors.h>
#include "DirectXHelper.h"

using namespace ShadowMapping;

using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;

// Loads and initializes application assets when the application is loaded.
ShadowMapSampleMain::ShadowMapSampleMain(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources)
{
    // Register to be notified if the device is lost or recreated.
    m_deviceResources->RegisterDeviceNotify(this);

    m_shadowSceneRenderer = std::unique_ptr<ShadowSceneRenderer>(new ShadowSceneRenderer(m_deviceResources));
}

ShadowMapSampleMain::~ShadowMapSampleMain()
{
    // Deregister device notification.
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

// Updates application state when the window size changes (e.g. device orientation change)
void ShadowMapSampleMain::UpdateForWindowSizeChange()
{
    m_shadowSceneRenderer->CreateWindowSizeDependentResources();
}

// Updates the application state once per frame.
void ShadowMapSampleMain::Update()
{
    // Update scene objects.
    m_timer.Tick([&]()
    {
        m_shadowSceneRenderer->Update(m_timer);
    });
}

// Renders the current frame according to the current application state.
// Returns true if the frame was rendered and is ready to be displayed.
bool ShadowMapSampleMain::Render()
{
    // Don't try to render anything before the first Update.
    if (m_timer.GetFrameCount() == 0)
    {
        return false;
    }

    // Render the scene objects.
    m_shadowSceneRenderer->Render();

    return true;
}

// Notifies renderers that device resources need to be released.
void ShadowMapSampleMain::OnDeviceLost()
{
    m_shadowSceneRenderer->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be re-created.
void ShadowMapSampleMain::OnDeviceRestored()
{
    m_shadowSceneRenderer->CreateDeviceDependentResources();
    UpdateForWindowSizeChange();
}

// Saves the current state of the app for suspend and terminate events.
void ShadowMapSampleMain::SaveInternalState(IPropertySet^ state)
{
    // Save filtering setting.
    bool filtering = GetFiltering();
    Platform::String^ filteringKeyName = "SdkSample:EdgeFiltering";
    if (state->HasKey(filteringKeyName))
    {
        state->Remove(filteringKeyName);
    }
    state->Insert(filteringKeyName, PropertyValue::CreateBoolean(filtering));

    // Save shadow buffer dimension setting.
    float shadowSize = GetShadowSize();
    Platform::String^ shadowKeyName = "SdkSample:ShadowSize";
    if (state->HasKey(shadowKeyName))
    {
        state->Remove(shadowKeyName);
    }
    state->Insert(shadowKeyName, PropertyValue::CreateSingle(shadowSize));
}

// Loads the current state of the app for resume events.
void ShadowMapSampleMain::LoadInternalState(IPropertySet^ state)
{
    // Load filtering setting.
    bool filtering = false;
    Platform::String^ filteringKeyName = "SdkSample:EdgeFiltering";
    if (state->HasKey(filteringKeyName))
    {
        filtering = safe_cast<IPropertyValue^>(state->Lookup(filteringKeyName))->GetBoolean();
    }
    SetFiltering(filtering);

    // Load shadow buffer dimension setting.
    float shadowSize = 1024.f;
    Platform::String^ shadowKeyName = "SdkSample:ShadowSize";
    if (state->HasKey(shadowKeyName))
    {
        shadowSize = safe_cast<IPropertyValue^>(state->Lookup(shadowKeyName))->GetSingle();
    }
    SetShadowSize(shadowSize);
}

// Set the filtering type for the shadow map.
void ShadowMapSampleMain::SetFiltering(bool useLinear)
{
    m_shadowSceneRenderer->SetFiltering(useLinear);
}

// Get the filtering type for the shadow map.
bool ShadowMapSampleMain::GetFiltering()
{
    return m_shadowSceneRenderer->GetFiltering();
}

// Set the shadow buffer dimension.
void ShadowMapSampleMain::SetShadowSize(float size)
{
    m_shadowSceneRenderer->SetShadowDimension(size);
}

// Get the shadow buffer dimension.
float ShadowMapSampleMain::GetShadowSize()
{
    return m_shadowSceneRenderer->GetShadowDimension();
}

// Get device feature support info for D3D9 shadows.
bool ShadowMapSampleMain::GetD3D9ShadowsSupported()
{
    return m_shadowSceneRenderer->GetD3D9ShadowsSupported();
}