//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PostcardRenderer.h"

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::DataTransfer;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::ApplicationSettings;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Input::Inking;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Input;

PostcardRenderer::PostcardRenderer() :
    m_sampleMode(SampleMode::Ready)
{
    m_imageRenderer = ref new ImageRenderer();
    m_paperRenderer = ref new PaperRenderer();
    m_extrudedTextRenderer = ref new ExtrudedTextRenderer();
    m_inkRenderer = ref new InkRenderer();
}

void PostcardRenderer::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    m_imageRenderer->CreateDeviceIndependentResources(m_wicFactory);
    m_paperRenderer->CreateDeviceIndependentResources(m_d2dFactory);
    m_extrudedTextRenderer->CreateDeviceIndependentResources(m_d2dFactory, m_dwriteFactory);
    m_inkRenderer->CreateDeviceIndependentResources(m_d2dFactory);
}

void PostcardRenderer::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_imageRenderer->CreateDeviceResources(m_d2dContext);
    m_paperRenderer->CreateDeviceResources(m_d2dContext);
    m_extrudedTextRenderer->CreateDeviceResources(m_d3dDevice, m_d3dContext);
    m_inkRenderer->CreateDeviceResources(m_d2dContext);
}

void PostcardRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Create a descriptor for the depth/stencil buffer.
    CD3D11_TEXTURE2D_DESC depthStencilDesc(
        DXGI_FORMAT_D24_UNORM_S8_UINT,
        static_cast<uint32>(m_renderTargetSize.Width),
        static_cast<uint32>(m_renderTargetSize.Height),
        1,
        1,
        D3D11_BIND_DEPTH_STENCIL
        );

    // Allocate a 2-D surface as the depth/stencil buffer.
    ComPtr<ID3D11Texture2D> depthStencil;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
            &depthStencilDesc,
            nullptr,
            &depthStencil
            )
        );

    // Create a DepthStencil view on this surface to use on bind.
    DX::ThrowIfFailed(
        m_d3dDevice->CreateDepthStencilView(
            depthStencil.Get(),
            &CD3D11_DEPTH_STENCIL_VIEW_DESC(D3D11_DSV_DIMENSION_TEXTURE2D),
            &m_d3dDepthStencilView
            )
        );

    m_imageRenderer->CreateWindowSizeDependentResources(m_dpi);

    m_paperRenderer->CreateWindowSizeDependentResources(m_dpi);

    m_extrudedTextRenderer->CreateWindowSizeDependentResources(
        m_dpi,
        m_d3dRenderTargetView,
        m_d3dDepthStencilView,
        m_renderTargetSize
        );

    m_inkRenderer->CreateWindowSizeDependentResources(m_dpi);
}

void PostcardRenderer::UpdateForWindowSizeChange()
{
    // Release extra references to Direct3D resources before attempting
    // to resize the swap chain.
    m_extrudedTextRenderer->ReleaseBufferResources();

    DirectXBase::UpdateForWindowSizeChange();
}

void PostcardRenderer::Render()
{
    m_d2dContext->BeginDraw();
    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Black));

    if (m_windowState != ApplicationViewState::Snapped)
    {
        // Render each part of the postcard in order.
        m_imageRenderer->DrawImageAndEffects();
        m_paperRenderer->DrawPaper();
        m_d2dContext->EndDraw(); // Ignore any HRESULT. It will be dealt with after the next EndDraw().

        // The extruded text renderer does not draw between a BeginDraw and
        // EndDraw because it uses Direct3D. It is inappropriate to perform
        // Direct3D operations while Direct2D is active.
        m_extrudedTextRenderer->DrawExtrudedText();

        m_d2dContext->BeginDraw();
        m_inkRenderer->DrawInk();
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

void PostcardRenderer::UpdateForViewStateChanged(ApplicationViewState state)
{
    m_windowState = state;
}

void PostcardRenderer::SavePostcard(IRandomAccessStream^ randomAccessStream, GUID wicFormat)
{
    // Convert the RandomAccessStream to an IStream.
    ComPtr<IStream> stream;
    DX::ThrowIfFailed(
        CreateStreamOverRandomAccessStream(randomAccessStream, IID_PPV_ARGS(&stream))
        );

    Render();

    // Save the rendered postcard to the IStream.
    SaveBitmapToStream(
        m_d2dTargetBitmap.Get(),
        m_wicFactory.Get(),
        m_d2dContext.Get(),
        wicFormat,
        stream.Get()
        );
}

void PostcardRenderer::OnDataRequested(_In_ DataTransferManager^ sender, _In_ DataRequestedEventArgs^ args)
{
    DataPackage^ package = args->Request->Data;

    // Set the title, description, and text to be shared.
    package->Properties->ApplicationName = "DirectX Postcard app sample";
    package->Properties->Title = "A postcard for you!";
    package->Properties->Description = "This is a postcard made with the DirectX Postcard app C++ sample.";

    // Create an in-memory stream to contain the shared data.
    IRandomAccessStream^ randomAccessStream = ref new InMemoryRandomAccessStream();

    // Create an IStream over that random access stream.
    ComPtr<IStream> stream;
    DX::ThrowIfFailed(
        CreateStreamOverRandomAccessStream(randomAccessStream, IID_PPV_ARGS(&stream))
        );

    Render();

    // Save the bitmap to the stream.
    SaveBitmapToStream(
        m_d2dTargetBitmap.Get(),
        m_wicFactory.Get(),
        m_d2dContext.Get(),
        GUID_ContainerFormatPng,
        stream.Get()
        );

    // Create a stream reference for the random access stream and give it to the data package.
    RandomAccessStreamReference^ streamReference = RandomAccessStreamReference::CreateFromStream(randomAccessStream);
    package->SetBitmap(streamReference);
}

// Save render target bitmap to a stream using WIC.
void PostcardRenderer::SaveBitmapToStream(
    _In_ ID2D1Bitmap1* d2dBitmap,
    _In_ IWICImagingFactory2* wicFactory2,
    _In_ ID2D1DeviceContext* d2dContext,
    _In_ REFGUID wicFormat,
    _In_ IStream* stream
    )
{
    // Create and initialize WIC bitmap encoder.
    ComPtr<IWICBitmapEncoder> wicBitmapEncoder;
    DX::ThrowIfFailed(
        wicFactory2->CreateEncoder(
            wicFormat,
            nullptr,
            &wicBitmapEncoder
            )
        );

    DX::ThrowIfFailed(
        wicBitmapEncoder->Initialize(
            stream,
            WICBitmapEncoderNoCache
            )
        );

    // Create and initialize WIC frame encoder.
    ComPtr<IWICBitmapFrameEncode> wicFrameEncode;
    DX::ThrowIfFailed(
        wicBitmapEncoder->CreateNewFrame(
            &wicFrameEncode,
            nullptr     // No encoder options.
            )
        );

    DX::ThrowIfFailed(
        wicFrameEncode->Initialize(nullptr)
        );

    // Retrieve Direct2D device.
    ComPtr<ID2D1Device> d2dDevice;
    d2dContext->GetDevice(&d2dDevice);

    // Create WIC image encoder.
    ComPtr<IWICImageEncoder> imageEncoder;
    DX::ThrowIfFailed(
        wicFactory2->CreateImageEncoder(
            d2dDevice.Get(),
            &imageEncoder
            )
        );

    DX::ThrowIfFailed(
        imageEncoder->WriteFrame(
            d2dBitmap,
            wicFrameEncode.Get(),
            nullptr     // Use default options.
            )
        );

    DX::ThrowIfFailed(
        wicFrameEncode->Commit()
        );

    DX::ThrowIfFailed(
        wicBitmapEncoder->Commit()
        );

    // Flush all memory buffers to the next-level storage object.
    DX::ThrowIfFailed(
        stream->Commit(STGC_DEFAULT)
        );
}

void PostcardRenderer::AddPaper()
{
    m_paperRenderer->AddPaper();
    Render();
    Present();
}

void PostcardRenderer::RemovePaper()
{
    m_paperRenderer->RemovePaper();
}

void PostcardRenderer::MovePaper()
{
    m_paperRenderer->MovePaper();
}

void PostcardRenderer::StampPaper()
{
    m_paperRenderer->StampPaper();
}

void PostcardRenderer::LoadImage(IRandomAccessStream^ randomAccessStream)
{
    m_imageRenderer->LoadImage(randomAccessStream);
    Render();
    Present();
}

void PostcardRenderer::SetEffectIntensity(float value)
{
    m_imageRenderer->SetEffectIntensity(value);
    Render();
    Present();
}

void PostcardRenderer::SetExtrudedText(String^ text)
{
    m_extrudedTextRenderer->SetExtrudedText(text);
    Render();
    Present();
}

void PostcardRenderer::StartSignature()
{
    m_inkRenderer->StartSignature();
}

void PostcardRenderer::ResetSignature()
{
    m_inkRenderer->Reset();
    Render();
    Present();
}

void PostcardRenderer::ResetPostcard()
{
    m_imageRenderer->Reset();
    m_paperRenderer->Reset();
    m_extrudedTextRenderer->Reset();
    m_inkRenderer->Reset();

    Render();
    Present();
}

void PostcardRenderer::OnPointerPressed(
    _In_ Object^ /* sender */,
    _In_ PointerRoutedEventArgs^ args
    )
{
    if (m_sampleMode == SampleMode::AddSignature)
    {
        m_inkRenderer->OnPointerPressed(args);
        Render();
        Present();
    }
}

void PostcardRenderer::OnPointerReleased(
    _In_ Object^ /* sender */,
    _In_ PointerRoutedEventArgs^ args
    )
{
    if (m_sampleMode == SampleMode::AddSignature)
    {
        m_inkRenderer->OnPointerReleased(args);
        Render();
        Present();
    }
}

void PostcardRenderer::OnPointerMoved(
    _In_ Object^ /* sender */,
    _In_ PointerRoutedEventArgs^ args
    )
{
    if (m_sampleMode == SampleMode::AddSignature)
    {
        m_inkRenderer->OnPointerMoved(args);
        Render();
        Present();
    }
}

void PostcardRenderer::OnManipulationStarted(
    _In_ Object^ /* sender */,
    _In_ ManipulationStartedRoutedEventArgs^ args
    )
{
    if (m_sampleMode == SampleMode::AddConstructionPaper)
    {
        m_paperRenderer->OnManipulationStarted(args);
        Render();
        Present();
    }
}

void PostcardRenderer::OnManipulationCompleted(
    _In_ Object^ /* sender */,
    _In_ ManipulationCompletedRoutedEventArgs^ args
    )
{
    if (m_sampleMode == SampleMode::AddConstructionPaper)
    {
        m_paperRenderer->OnManipulationCompleted(args);
        Render();
        Present();
    }
}

void PostcardRenderer::OnManipulationDelta(
    _In_ Object^ /* sender */,
    _In_ ManipulationDeltaRoutedEventArgs^ args
    )
{
    if (m_sampleMode == SampleMode::AddConstructionPaper)
    {
        m_paperRenderer->OnManipulationDelta(args);
        Render();
        Present();
    }
    else if (m_sampleMode == SampleMode::AddText)
    {
        m_extrudedTextRenderer->OnManipulationDelta(
            args->Position,
            args->Delta.Translation,
            args->Delta.Scale,
            args->Delta.Rotation
            );
        Render();
        Present();
    }
}

void PostcardRenderer::OnTapped(
    _In_ Object^ /* sender */,
    _In_ TappedRoutedEventArgs^ args
    )
{
    if (m_sampleMode == SampleMode::AddConstructionPaper)
    {
        m_paperRenderer->OnTapped(args->GetPosition(nullptr));
        Render();
        Present();
    }
}

void PostcardRenderer::SetSampleMode(SampleMode mode)
{
    m_sampleMode = mode;
}
