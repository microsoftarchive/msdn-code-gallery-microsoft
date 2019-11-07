//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


// WinRTComponent.cpp

#include "pch.h"
#include "SampleMft0.h"
#include "WinRTComponent.h"
#include <Windows.h>
using namespace Wrapper;

WinRTComponent::WinRTComponent ()
{
}

WinRTComponent::~WinRTComponent ()
{
}

bool WinRTComponent::Initialize(Object^ o)
{
    if (o == nullptr)
    {
        throw ref new Platform::COMException(E_INVALIDARG);
        return false;
    }
    Microsoft::WRL::ComPtr<IUnknown> spUnk(reinterpret_cast<IUnknown*>(o));
    if (spUnk == __nullptr)
    {
        throw ref new Platform::COMException(E_FAIL);
        return false;
    }
    HRESULT hr = spUnk.As(&m_spImpl);
    if (FAILED(hr))
    {
        hr = spUnk.Get()->QueryInterface(__uuidof(IMft0),  &m_spImpl);
        if(FAILED(hr))
        {
            throw ref new Platform::COMException(hr);
            return false;
        }
    }
    return true;
}

void WinRTComponent::UpdateDsp(int percentOfScreen)
{
    if (m_spImpl != __nullptr)
    {
        HRESULT hr = m_spImpl->UpdateDsp(percentOfScreen);
        if (FAILED(hr))
        {
            throw ref new Platform::COMException(hr);
        }
    }
}

void WinRTComponent::Enable()
{
    if (m_spImpl != __nullptr)
    {
        HRESULT hr = m_spImpl->Enable();
        if (FAILED(hr))
        {
            throw ref new Platform::COMException(hr);
        }
    }
}

void WinRTComponent::Disable()
{
    if (m_spImpl != __nullptr)
    {
        HRESULT hr = m_spImpl->Disable();
        if (FAILED(hr))
        {
            throw ref new Platform::COMException(hr);
        }
    }
}

DspSettings WinRTComponent::GetDspSetting()
{
    DspSettings settings = {0, false};

    if (m_spImpl != __nullptr)
    {
        UINT uiPercentOfScreen;
        BOOL bIsEnabled;
        HRESULT hr = m_spImpl->GetDspSetting(&uiPercentOfScreen, &bIsEnabled);
        if (SUCCEEDED(hr))
        {
            settings.percentOfScreen = uiPercentOfScreen;
            settings.isEnabled = bIsEnabled;
        }
        if (FAILED(hr))
        {
            throw ref new Platform::COMException(hr);
        }
    }

    return settings;
}