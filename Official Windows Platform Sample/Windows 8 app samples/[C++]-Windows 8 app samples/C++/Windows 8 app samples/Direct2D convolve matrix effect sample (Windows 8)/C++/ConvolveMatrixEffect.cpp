//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ConvolveMatrixEffect.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Popups;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;

ConvolveMatrixEffect::ConvolveMatrixEffect() :
    m_selectedKernel(KernelSelection::Passthrough),
    m_imageSize(D2D1::SizeF()),
    m_bitmapPixelSize(D2D1::SizeU())
{
}

void ConvolveMatrixEffect::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Decode the image using WIC. The output of the format converter
    // is accessible via m_wicConvertedSource.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"beach.jpg",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    ComPtr<IWICBitmapFrameDecode> frame;
    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frame)
        );

    // Store the image size in pixels. This remains constant regardless of the app's DPI.
    UINT width, height;
    DX::ThrowIfFailed(
        frame->GetSize(&width, &height)
        );

    m_bitmapPixelSize = D2D1::SizeU(width, height);

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&m_wicConvertedSource)
        );

    // Format convert to a pixel format that is compatible with Direct2D.
    DX::ThrowIfFailed(
        m_wicConvertedSource->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );
}

void ConvolveMatrixEffect::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D convolve matrix effect sample"
        );

    // Use the BitmapSource effect to load the image from WIC and make it usable
    // in the Direct2D effects graph.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect)
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
            m_wicConvertedSource.Get()
            )
        );

    // Cache the BitmapSource output as it never needs to change.
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_PROPERTY_CACHED, TRUE)
        );

    // Create the ConvolveMatrix effect and add it to the end of the effect graph.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1ConvolveMatrix, &m_convolveMatrixEffect)
        );

    m_convolveMatrixEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());
}

void ConvolveMatrixEffect::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    D2D1_SIZE_F contextSize = m_d2dContext->GetSize();

    // m_imageSize is in DIPs and changes depending on the app DPI.
    // This app ignores the source bitmap's DPI and assumes it is 96.
    m_imageSize = D2D1::SizeF(
        static_cast<float>(m_bitmapPixelSize.width) * 96.0f / m_dpi,
        static_cast<float>(m_bitmapPixelSize.height) * 96.0f / m_dpi
        );

    // Resize the image to fit the screen.
    float zoom = max(
        contextSize.width / m_imageSize.width,
        contextSize.height / m_imageSize.height
        );

    D2D1_VECTOR_2F zoomVector = D2D1::Vector2F(zoom, zoom);
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, zoomVector)
        );
}

void ConvolveMatrixEffect::Render()
{
    // Select the convolution kernel and set up the ConvolveMatrix properties accordingly.
    // The kernel definitions and parameters are defined in ConvolutionKernels.h.
    switch (m_selectedKernel)
    {
        case KernelSelection::Passthrough:
            // Because D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX is a "BLOB" type in the
            // Direct2D property type system, we need to specify the byte size of the
            // data as the third parameter.
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX,
                    (byte *) ConvolutionKernels::Passthrough,
                    sizeof(float) * ConvolutionKernels::PassthroughWidth *
                        ConvolutionKernels::PassthroughWidth
                    )
                );

            // Specify the size of the kernel.
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_X,
                    ConvolutionKernels::PassthroughWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_Y,
                    ConvolutionKernels::PassthroughWidth
                    )
                );

            // The divisor allows us to easily normalize a kernel's output. Many
            // common kernels have a divisor of 1 (the default value).
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_DIVISOR,
                    ConvolutionKernels::DefaultDivisor
                    )
                );
            break;

        case KernelSelection::BoxBlur:
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX,
                    (byte *) ConvolutionKernels::BoxBlur,
                    sizeof(float) * ConvolutionKernels::BoxBlurWidth *
                        ConvolutionKernels::BoxBlurWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_X,
                    ConvolutionKernels::BoxBlurWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_Y,
                    ConvolutionKernels::BoxBlurWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_DIVISOR,
                    ConvolutionKernels::BoxBlurDivisor
                    )
                );
            break;

        case KernelSelection::SimpleEdgeDetect:
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX,
                    (byte *) ConvolutionKernels::SimpleEdgeDetect,
                    sizeof(float) * ConvolutionKernels::SimpleEdgeDetectWidth *
                        ConvolutionKernels::SimpleEdgeDetectWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_X,
                    ConvolutionKernels::SimpleEdgeDetectWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_Y,
                    ConvolutionKernels::SimpleEdgeDetectWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_DIVISOR,
                    ConvolutionKernels::DefaultDivisor
                    )
                );
            break;

        case KernelSelection::SimpleSharpen:
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX,
                    (byte *) ConvolutionKernels::SimpleSharpen,
                    sizeof(float) * ConvolutionKernels::SimpleSharpenWidth *
                        ConvolutionKernels::SimpleSharpenWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_X,
                    ConvolutionKernels::SimpleSharpenWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_Y,
                    ConvolutionKernels::SimpleSharpenWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_DIVISOR,
                    ConvolutionKernels::SimpleSharpenDivisor
                    )
                );
            break;

        case KernelSelection::Emboss:
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX,
                    (byte *) ConvolutionKernels::Emboss,
                    sizeof(float) * ConvolutionKernels::EmbossWidth *
                        ConvolutionKernels::EmbossWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_X,
                    ConvolutionKernels::EmbossWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_Y,
                    ConvolutionKernels::EmbossWidth
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_DIVISOR,
                    ConvolutionKernels::DefaultDivisor
                    )
                );
            break;

        case KernelSelection::HorizontalSmear:
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_MATRIX,
                    (byte *) ConvolutionKernels::HorizontalSmear,
                    sizeof(float) * ConvolutionKernels::HorizontalSmearWidth *
                        ConvolutionKernels::HorizontalSmearHeight
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_X,
                    ConvolutionKernels::HorizontalSmearWidth
                    )
                );

            // The horizontal smear kernel has a different width and height.
            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_KERNEL_SIZE_Y,
                    ConvolutionKernels::HorizontalSmearHeight
                    )
                );

            DX::ThrowIfFailed(
                m_convolveMatrixEffect->SetValue(
                    D2D1_CONVOLVEMATRIX_PROP_DIVISOR,
                    ConvolutionKernels::HorizontalSmearDivisor
                    )
                );
            break;

        default:
            throw ref new Platform::FailureException();
    }

    m_d2dContext->BeginDraw();

    m_d2dContext->DrawImage(
        m_convolveMatrixEffect.Get(),
        D2D1_INTERPOLATION_MODE_LINEAR,
        D2D1_COMPOSITE_MODE_SOURCE_COPY
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void ConvolveMatrixEffect::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(
            this,
            &ConvolveMatrixEffect::OnActivated
            );

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &ConvolveMatrixEffect::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &ConvolveMatrixEffect::OnResuming);
}

void ConvolveMatrixEffect::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &ConvolveMatrixEffect::OnWindowSizeChanged);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &ConvolveMatrixEffect::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &ConvolveMatrixEffect::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &ConvolveMatrixEffect::OnPointerMoved);

    m_gestureRecognizer = ref new GestureRecognizer();
    m_gestureRecognizer->GestureSettings = GestureSettings::Hold;

    m_gestureRecognizer->Holding +=
        ref new TypedEventHandler<GestureRecognizer^, HoldingEventArgs^>(this, &ConvolveMatrixEffect::OnHolding);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &ConvolveMatrixEffect::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &ConvolveMatrixEffect::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void ConvolveMatrixEffect::Load(
    _In_ Platform::String^ entryPoint
    )
{
    IPropertySet^ appSettings = ApplicationData::Current->LocalSettings->Values;

    if (appSettings->HasKey("kernelIndex"))
    {
        m_selectedKernel = static_cast<KernelSelection>(
            safe_cast<IPropertyValue^>(appSettings->Lookup("kernelIndex"))->GetUInt32()
            );
    }
}

void ConvolveMatrixEffect::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void ConvolveMatrixEffect::Uninitialize()
{
}

void ConvolveMatrixEffect::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void ConvolveMatrixEffect::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void ConvolveMatrixEffect::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void ConvolveMatrixEffect::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void ConvolveMatrixEffect::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // This state is restored during Load in the event that the app is terminated.

    IPropertySet^ appSettings = ApplicationData::Current->LocalSettings->Values;

    if (appSettings->HasKey("kernelIndex"))
    {
        appSettings->Remove("kernelIndex");
    }

    appSettings->Insert(
        "kernelIndex",
        PropertyValue::CreateUInt32(static_cast<unsigned int>(m_selectedKernel))
        );
}

void ConvolveMatrixEffect::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void ConvolveMatrixEffect::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void ConvolveMatrixEffect::OnPointerMoved(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void ConvolveMatrixEffect::OnPointerReleased(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);

    if (args->CurrentPoint->Properties->PointerUpdateKind == PointerUpdateKind::RightButtonReleased)
    {
        ShowMenu(args->CurrentPoint->Position);
    }
}

void ConvolveMatrixEffect::OnHolding(
    _In_ GestureRecognizer^ sender,
    _In_ HoldingEventArgs^ args
    )
{
    if (args->HoldingState == HoldingState::Started)
    {
        ShowMenu(args->Position);
    }
}

void ConvolveMatrixEffect::ShowMenu(Point position)
{
    PopupMenu^ popupMenu = ref new PopupMenu();

    popupMenu->Commands->Append(
        ref new UICommand(
            ConvolutionKernels::PassthroughTitle,
            ref new UICommandInvokedHandler(this, &ConvolveMatrixEffect::SwitchKernel)
            )
        );

    popupMenu->Commands->Append(
        ref new UICommand(
            ConvolutionKernels::BoxBlurTitle,
            ref new UICommandInvokedHandler(this, &ConvolveMatrixEffect::SwitchKernel)
            )
        );

    popupMenu->Commands->Append(
        ref new UICommand(
            ConvolutionKernels::SimpleEdgeDetectTitle,
            ref new UICommandInvokedHandler(this, &ConvolveMatrixEffect::SwitchKernel)
            )
        );

    popupMenu->Commands->Append(
        ref new UICommand(
            ConvolutionKernels::SimpleSharpenTitle,
            ref new UICommandInvokedHandler(this, &ConvolveMatrixEffect::SwitchKernel)
            )
        );

    popupMenu->Commands->Append(
        ref new UICommand(
            ConvolutionKernels::EmbossTitle,
            ref new UICommandInvokedHandler(this, &ConvolveMatrixEffect::SwitchKernel)
            )
        );

    popupMenu->Commands->Append(
        ref new UICommand(
            ConvolutionKernels::HorizontalSmearTitle,
            ref new UICommandInvokedHandler(this, &ConvolveMatrixEffect::SwitchKernel)
            )
        );

    popupMenu->ShowAsync(position);
}

// When the user selects a convolution kernel from the popup menu, update the ConvolveMatrix
// properties and render the output.
void ConvolveMatrixEffect::SwitchKernel(
    _In_ IUICommand^ command
    )
{
    if (command->Label == ConvolutionKernels::PassthroughTitle)
    {
        m_selectedKernel = KernelSelection::Passthrough;
    }
    else if (command->Label == ConvolutionKernels::BoxBlurTitle)
    {
        m_selectedKernel = KernelSelection::BoxBlur;
    }
    else if (command->Label == ConvolutionKernels::SimpleEdgeDetectTitle)
    {
        m_selectedKernel = KernelSelection::SimpleEdgeDetect;
    }
    else if (command->Label == ConvolutionKernels::SimpleSharpenTitle)
    {
        m_selectedKernel = KernelSelection::SimpleSharpen;
    }
    else if (command->Label == ConvolutionKernels::EmbossTitle)
    {
        m_selectedKernel = KernelSelection::Emboss;
    }
    else if (command->Label == ConvolutionKernels::HorizontalSmearTitle)
    {
        m_selectedKernel = KernelSelection::HorizontalSmear;
    }

    Render();
    Present();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new ConvolveMatrixEffect();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
