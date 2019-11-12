//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include "DirectXBase.h"

#include "ImageRenderer.h"
#include "PaperRenderer.h"
#include "ExtrudedTextRenderer.h"
#include "InkRenderer.h"

// Enum that describes the current mode of the sample.
enum class SampleMode
{
    Ready,
    AddConstructionPaper,
    AddImage,
    AddEffect,
    AddText,
    AddSignature
};

ref class PostcardRenderer : public DirectXBase
{
internal:
    PostcardRenderer();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void UpdateForWindowSizeChange() override;
    virtual void Render() override;

    void OnPointerPressed(
        _In_ Platform::Object^ /* sender */,
        _In_ Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args
        );
    void OnPointerReleased(
        _In_ Platform::Object^ /* sender */,
        _In_ Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args
        );
    void OnPointerMoved(
        _In_ Platform::Object^ /* sender */,
        _In_ Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args
        );
    void OnManipulationStarted(
        _In_ Platform::Object^ /* sender */,
        _In_ Windows::UI::Xaml::Input::ManipulationStartedRoutedEventArgs^ args
        );
    void OnManipulationCompleted(
        _In_ Platform::Object^ /* sender */,
        _In_ Windows::UI::Xaml::Input::ManipulationCompletedRoutedEventArgs^ args
        );
    void OnManipulationDelta(
        _In_ Platform::Object^ /* sender */,
        _In_ Windows::UI::Xaml::Input::ManipulationDeltaRoutedEventArgs^ args
        );
    void OnTapped(
        _In_ Platform::Object^ /* sender */,
        _In_ Windows::UI::Xaml::Input::TappedRoutedEventArgs^ args
        );
    void OnDataRequested(
        _In_ Windows::ApplicationModel::DataTransfer::DataTransferManager^ sender,
        _In_ Windows::ApplicationModel::DataTransfer::DataRequestedEventArgs^ args
        );

    void UpdateForViewStateChanged(Windows::UI::ViewManagement::ApplicationViewState state);
    void SetSampleMode(SampleMode mode);

    // Public image methods.
    void LoadImage(Windows::Storage::Streams::IRandomAccessStream^ randomAccessStream);
    void SetEffectIntensity(float value);

    // Public paper methods.
    void AddPaper();
    void RemovePaper();
    void MovePaper();
    void StampPaper();

    // Public text extrusion methods.
    void SetExtrudedText(Platform::String^ text);

    // Public signature methods.
    void StartSignature();
    void ResetSignature();

    // Public saving methods.
    void SavePostcard(Windows::Storage::Streams::IRandomAccessStream^ randomAccessStream, GUID wicFormat);

    // Public reset methods.
    void ResetPostcard();

private:
    void SaveBitmapToStream(
        _In_ ID2D1Bitmap1* d2dBitmap,
        _In_ IWICImagingFactory2* wicFactory2,
        _In_ ID2D1DeviceContext* d2dContext,
        _In_ REFGUID wicFormat,
        _In_ IStream* stream
        );

    SampleMode                                              m_sampleMode;

    ImageRenderer^                                          m_imageRenderer;
    PaperRenderer^                                          m_paperRenderer;
    ExtrudedTextRenderer^                                   m_extrudedTextRenderer;
    InkRenderer^                                            m_inkRenderer;

    Windows::UI::Xaml::Controls::SwapChainBackgroundPanel^  m_swapChainPanel;
    Windows::UI::ViewManagement::ApplicationViewState       m_windowState;
};
