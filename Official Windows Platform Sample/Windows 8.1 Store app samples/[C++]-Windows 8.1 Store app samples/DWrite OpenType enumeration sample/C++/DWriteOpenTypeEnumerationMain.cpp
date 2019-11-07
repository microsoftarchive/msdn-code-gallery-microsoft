//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteOpenTypeEnumerationMain.h"

#include <DirectXColors.h>
#include "DirectXHelper.h"

using namespace DWriteOpenTypeEnumeration;

// Loads and initializes application assets when the application is loaded.
DWriteOpenTypeEnumerationMain::DWriteOpenTypeEnumerationMain(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources)
{
    // Register to be notified if the device is lost or recreated.
    m_deviceResources->RegisterDeviceNotify(this);

    m_sceneRenderer = std::unique_ptr<DWriteOpenTypeEnumerationRenderer>(new DWriteOpenTypeEnumerationRenderer(m_deviceResources));

    // Create variables to store the font family and index in the collection
    Microsoft::WRL::ComPtr<IDWriteFontFamily> tempFontFamily;
    Microsoft::WRL::ComPtr<IDWriteFont> tempFont;
    Microsoft::WRL::ComPtr<IDWriteFontFace> tempFontFace;
    Microsoft::WRL::ComPtr<IDWriteFontCollection> fontCollection;
    Microsoft::WRL::ComPtr<IDWriteTextAnalyzer> textAnalyzer;

    UINT32 fontIndex = 0;
    BOOL isPresent = false;

    // Get a copy of the system font collection
    m_deviceResources->GetDWriteFactory()->GetSystemFontCollection(&fontCollection);

    WCHAR* fontFaceNames[] = {L"Arial", L"Times New Roman", L"Meiryo", L"Gabriola"};
    Microsoft::WRL::ComPtr<IDWriteFontFace2>* fontFaces[4] = {&m_arial, &m_times, &m_meiryo, &m_gabriola};

    for (int i = 0; i < ARRAYSIZE(fontFaceNames); i++)
    {
        fontCollection->FindFamilyName(fontFaceNames[i], &fontIndex, &isPresent);
        if (isPresent)
        {
            DX::ThrowIfFailed(
                fontCollection->GetFontFamily(fontIndex, &tempFontFamily)
                );

            DX::ThrowIfFailed(
                tempFontFamily->GetFirstMatchingFont(
                    DWRITE_FONT_WEIGHT_NORMAL,
                    DWRITE_FONT_STRETCH_NORMAL,
                    DWRITE_FONT_STYLE_NORMAL,
                    &tempFont
                    )
                );

            DX::ThrowIfFailed(
                tempFont->CreateFontFace(&tempFontFace)
                );

            DX::ThrowIfFailed(
                tempFontFace.As(fontFaces[i])
                );
        }
    }

    // Create the IDWriteTextAnalyzer that we'll need to get OpenType feature coverage from
    m_deviceResources->GetDWriteFactory()->CreateTextAnalyzer(&textAnalyzer);

    textAnalyzer.As(&m_textAnalyzer);
}

DWriteOpenTypeEnumerationMain::~DWriteOpenTypeEnumerationMain()
{
    // Deregister device notification.
    m_deviceResources->RegisterDeviceNotify(nullptr);
}

// Updates application state when the window size changes (e.g. device orientation change)
void DWriteOpenTypeEnumerationMain::UpdateForWindowSizeChange()
{
    m_sceneRenderer->CreateWindowSizeDependentResources();
}

// Renders the current frame according to the current application state.
// Returns true if the frame was rendered and is ready to be displayed.
bool DWriteOpenTypeEnumerationMain::Render()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Reset the viewport to target the whole screen.
    auto viewport = m_deviceResources->GetScreenViewport();
    context->RSSetViewports(1, &viewport);

    // Reset render targets to the screen.
    ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };
    context->OMSetRenderTargets(1, targets, m_deviceResources->GetDepthStencilView());

    // Clear the back buffer and depth stencil view.
    context->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), DirectX::Colors::MidnightBlue);
    context->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);

    // Render the scene objects.
    m_sceneRenderer->Render();

    return true;
}

// Notifies renderers that device resources need to be released.
void DWriteOpenTypeEnumerationMain::OnDeviceLost()
{
    m_sceneRenderer->ReleaseDeviceDependentResources();
}

// Notifies renderers that device resources may now be re-created.
void DWriteOpenTypeEnumerationMain::OnDeviceRestored()
{
    m_sceneRenderer->CreateDeviceDependentResources();
    UpdateForWindowSizeChange();
}

Platform::Array<bool>^ DWriteOpenTypeEnumerationMain::ReturnSupportedFeatures(int fontNumber)
{
    UINT32 actualTagCount = 0;
    DWRITE_SCRIPT_SHAPES shapes = {DWRITE_SCRIPT_SHAPES_DEFAULT};
    DWRITE_SCRIPT_ANALYSIS analysis = {0, shapes};
    DWRITE_FONT_FEATURE_TAG tag[255];

    // Choose the appropriate font based on the font selected in the XAML combobox
    switch (fontNumber)
    {
    case 0:
        m_textAnalyzer->GetTypographicFeatures(m_arial.Get(), analysis, L"en-us", 255, &actualTagCount, tag);
        break;
    case 1:
        m_textAnalyzer->GetTypographicFeatures(m_times.Get(), analysis, L"en-us", 255, &actualTagCount, tag);
        break;
    case 2:
        m_textAnalyzer->GetTypographicFeatures(m_meiryo.Get(), analysis, L"en-us", 255, &actualTagCount, tag);
        break;
    case 3:
        m_textAnalyzer->GetTypographicFeatures(m_gabriola.Get(), analysis, L"en-us", 255, &actualTagCount, tag);
        break;
    }
    auto result = ref new Platform::Array<bool>(8);

    // Iterate over the returned tags and determine which are present
    for (int i = 0; i < 255; i++)
    {
        switch (tag[i])
        {
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_1:
            result[0] = true;
            break;
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_2:
            result[1] = true;
            break;
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_3:
            result[2] = true;
            break;
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_4:
            result[3] = true;
            break;
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_5:
            result[4] = true;
            break;
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_6:
            result[5] = true;
            break;
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_7:
            result[6] = true;
            break;
        case DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_20:
            result[7] = true;
            break;
        }
    }

    m_sceneRenderer->UpdateFontFace(fontNumber);

    return result;
}

void DWriteOpenTypeEnumerationMain::SetStylisticSet(int stylisticSet)
{
    m_sceneRenderer->UpdateStylisticSet(stylisticSet);
}