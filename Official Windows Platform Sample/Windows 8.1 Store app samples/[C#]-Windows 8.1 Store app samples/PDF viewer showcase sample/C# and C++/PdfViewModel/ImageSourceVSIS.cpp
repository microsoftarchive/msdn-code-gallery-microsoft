// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PdfViewContext.h"
#include "ImageSourceVSIS.h"

using namespace Microsoft::WRL;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace PdfViewModel
{
    class VSISCallBack : public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::ClassicCom>,
        IVirtualSurfaceUpdatesCallbackNative>
    {
    public:
        HRESULT RuntimeClassInitialize(_In_ Platform::WeakReference imageSource)
        {
            imgSource = imageSource;
            return S_OK;
        }

        /// <summary>
        /// Callback function for UpdatesNeeded for VSIS surface
        /// This function is invoked whenever item is in view
        /// </summary>
        IFACEMETHODIMP UpdatesNeeded()
        {
            ImageSourceVSIS^ imageSource = imgSource.Resolve<ImageSourceVSIS>();
            if (imageSource != nullptr)
            {
                imageSource->UpdatesNeeded();
            }
            return S_OK;
        }

    private:
        Platform::WeakReference imgSource;
    };

    /// <summary>
    /// Creates required resources VSIS
    /// </summary>
    void ImageSourceVSIS::CreateSurface()
    {
        auto vsisForeground = ref new VirtualSurfaceImageSource(static_cast<int>(width), static_cast<int>(height), false /* isOpaque */);
        ComPtr<IVirtualSurfaceImageSourceNative> vsisNative;
        reinterpret_cast<IInspectable*>(vsisForeground)->QueryInterface(IID_PPV_ARGS(&vsisNative));
        if (vsisNative != nullptr)
        {
            sisNative = vsisNative; // save the native image surface in the base class
            ComPtr<IDXGIDevice> dxgiDevice;
            pdfViewContext->Renderer->GetDXGIDevice(&dxgiDevice);
            if (dxgiDevice != nullptr)
            {
                DX::ThrowIfFailed(vsisNative->SetDevice(dxgiDevice.Get()));
                Platform::WeakReference that(this);
                ComPtr<VSISCallBack> spCallBack;
                DX::ThrowIfFailed(MakeAndInitialize<VSISCallBack>(&spCallBack, that));
                vsisNative->RegisterForUpdatesNeeded(spCallBack.Get());
            }
        }
    }

    /// <summary>
    /// Following function creates a new VSIS resource and bind it to foreground image source.
    /// Existing VSIS surface is bound to background image source. 
    /// Page at current zoom level is rendered on the Foreground image source giving a crisper image
    /// </summary>
    /// <param name="widthLocal">Width of the scaled VSIS surface</param>
    /// <param name="heightLocal">Height of the scaled VSIS surface</param>
    void ImageSourceVSIS::SwapVsis(_In_ float widthLocal, _In_ float heightLocal)
    {
        ComPtr<IVirtualSurfaceImageSourceNative> vsisForeground;
        if (SUCCEEDED(sisNative.As(&vsisForeground)))
        {
            // Unregister current vsis for updates needed.
            vsisForeground->RegisterForUpdatesNeeded(nullptr);
            // Assigning current VSIS surface to the background VSIS which is binded to a image source
            vsisBackground = AsObject<VirtualSurfaceImageSource>(vsisForeground.Get());
            vsisForeground = nullptr;
        }

        // Updating height and width with the new value
        SetPageDimensions(widthLocal, heightLocal);

        // Creating new VSIS with new dimensions
        CreateSurface();
    }

    /// <summary>
    /// Following function returns the background VSIS surface
    /// </summary>
    /// <returns>Background VSIS surface</returns>
    VirtualSurfaceImageSource^ ImageSourceVSIS::GetImageSourceVsisBackground()
    {
        return vsisBackground;
    }

    /// <summary>
    /// Following function is a Callback registered for VSIS surface
    /// This method is invoked by the UI thread whenever corresponding page is in view
    /// </summary>
    void ImageSourceVSIS::UpdatesNeeded()
    {
        ComPtr<IVirtualSurfaceImageSourceNative> vsisNative;
        if (SUCCEEDED(sisNative.As(&vsisNative)))
        {
            if ((pdfPage != nullptr) && (vsisNative != nullptr))
            {
                ULONG drawingBoundsCount = 0;
                DX::ThrowIfFailed(vsisNative->GetUpdateRectCount(&drawingBoundsCount));

                std::unique_ptr<RECT[]> drawingBounds(new RECT[drawingBoundsCount]);
                DX::ThrowIfFailed(vsisNative->GetUpdateRects(drawingBounds.get(), drawingBoundsCount));
                
                for (ULONG i = 0; i < drawingBoundsCount; ++i)
                {
                    RenderPageRect(drawingBounds[i]);
                }
            }
        }
        vsisBackground = nullptr;
    }
}