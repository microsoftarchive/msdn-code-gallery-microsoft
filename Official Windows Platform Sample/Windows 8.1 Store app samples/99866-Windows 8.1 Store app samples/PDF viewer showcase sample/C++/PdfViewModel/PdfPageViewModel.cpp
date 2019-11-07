// Copyright (c) Microsoft Corporation. All rights reserved

//
// PdfPageViewModel.cpp
// Definition of the PdfPageViewModel class
//
#include "pch.h"
#include "PdfPageViewModel.h"
#include "ImageSourceSIS.h"
#include "ImageSourceVSIS.h"

using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::Data::Pdf;
using namespace Windows::Foundation;
using namespace Platform;

namespace PdfViewModel
{
    PdfPageViewModel::PdfPageViewModel(_In_ PdfDocument^ document, _In_ unsigned int index)
        : pdfDocument(document), pageIndex(index), isPropertyChangedObserved(false), propertySubscriptionCount(0)
    {
    }

    unsigned int PdfPageViewModel::PageIndex::get()
    {
        return pageIndex;
    }

    void PdfPageViewModel::PageIndex::set(_In_ unsigned int /*value*/)
    {
        throw ref new Exception(E_ILLEGAL_METHOD_CALL);
    }

    float PdfPageViewModel::Width::get()
    {
        return pdfViewContext->PageViewSize.Width;
    }

    void PdfPageViewModel::Width::set(_In_ float /*value*/)
    {
        throw ref new Exception(E_ILLEGAL_METHOD_CALL);
    }

    float PdfPageViewModel::Height::get()
    {
        return pdfViewContext->PageViewSize.Height;
    }

    void PdfPageViewModel::Height::set(_In_ float /*value*/)
    {
        throw ref new Exception(E_ILLEGAL_METHOD_CALL);
    }

    VirtualSurfaceImageSource^ PdfPageViewModel::ImageSourceVsisBackground::get()
    {
        auto imageSourceVSIS = dynamic_cast<ImageSourceVSIS^>(imageSource);
        return (imageSourceVSIS != nullptr) ? imageSourceVSIS->GetImageSourceVsisBackground() : nullptr;
    }

    void PdfPageViewModel::ImageSourceVsisBackground::set(_In_ VirtualSurfaceImageSource^ /*value*/)
    {
        throw ref new Exception(E_ILLEGAL_METHOD_CALL);
    }

    VirtualSurfaceImageSource^ PdfPageViewModel::ImageSourceVsisForeground::get()
    {
        return dynamic_cast<VirtualSurfaceImageSource^>(GetImageSource());
    }

    void PdfPageViewModel::ImageSourceVsisForeground::set(_In_ VirtualSurfaceImageSource^ /*value*/)
    {
        throw ref new Exception(E_ILLEGAL_METHOD_CALL);
    }

    SurfaceImageSource^ PdfPageViewModel::ImageSourceSis::get()
    {
        return GetImageSource();
    }

    void PdfPageViewModel::ImageSourceSis::set(_In_ SurfaceImageSource^ /*value*/)
    {
        throw ref new Exception(E_ILLEGAL_METHOD_CALL);
    }

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property used to notify listeners.</param>
    void PdfPageViewModel::RaisePropertyChanged(_In_ String^ propertyName)
    {
        if (isPropertyChangedObserved)
        {
            PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
        }
    }

    SurfaceImageSource^ PdfPageViewModel::GetImageSource()
    {
        CreateImageSourceIfNotAvailable();
        SurfaceImageSource^ surface;
        if (!IsUnloaded())
        {
            imageSource->RenderPage();
            surface = imageSource->GetImageSource();
        }
        return surface;
    }

    void PdfPageViewModel::CreateImageSourceIfNotAvailable()
    {
        if (IsUnloaded())
        {
            Size pageSize = pdfViewContext->GetZoomedPageSize();
            ImageSourceBase^ imgSource;
            PdfPage^ pdfPage = pdfDocument->GetPage(pageIndex);
            if (pdfViewContext->SurfaceType == SurfaceType::SurfaceImageSource)
            {
                imgSource = ref new ImageSourceSIS(pageSize, pdfPage, pdfViewContext);
            }
            else
            {
                imgSource = ref new ImageSourceVSIS(pageSize, pdfPage, pdfViewContext);
            }
            imgSource->CreateSurface();
            SetImageSource(imgSource);
        }
    }

    void PdfPageViewModel::SetImageSource(_In_ ImageSourceBase^ imgSource)
    {
        imageSource = imgSource;
        switch (pdfViewContext->SurfaceType)
        {
        case SurfaceType::SurfaceImageSource:
            RaisePropertyChanged("ImageSourceSis");
            break;

        case SurfaceType::VirtualSurfaceImageSource:
            RaisePropertyChanged("ImageSourceVsisBackground");
            RaisePropertyChanged("ImageSourceVsisForeground");
            break;
        }
    }

    void PdfPageViewModel::UpdateSurface()
    {
        // Updating size of rendered items based on current zoom factor
        Size pgSize = pdfViewContext->GetZoomedPageSize();
        if ((pdfViewContext->SurfaceType == SurfaceType::VirtualSurfaceImageSource) && !IsUnloaded())
        {
            // Recreating rendering resources with new page dimensions only in case when image source has been initialized
            ImageSourceVSIS^ currentImageSource = dynamic_cast<ImageSourceVSIS^>(imageSource);
            currentImageSource->SwapVsis(pgSize.Width, pgSize.Height);
            SetImageSource(static_cast<ImageSourceBase^>(currentImageSource));
        }
    }

    bool PdfPageViewModel::IsUnloaded()
    {
        return (imageSource == nullptr);
    }

    // In c++, it is not neccessary to include definitions of add, remove, and raise.
    // These definitions have been made explicitly here so that we can release resources 
    // when the item is removed
    Windows::Foundation::EventRegistrationToken PdfPageViewModel::PropertyChanged::add(_In_ PropertyChangedEventHandler^ e)
    {
        propertySubscriptionCount++;
        isPropertyChangedObserved = true;
        return privatePropertyChanged += e;
    }

    void PdfPageViewModel::PropertyChanged::remove(_In_ Windows::Foundation::EventRegistrationToken t)
    {
        privatePropertyChanged -= t;
        propertySubscriptionCount--;
        if (propertySubscriptionCount == 0)
        {
            imageSource = nullptr;
        }
    }

    void PdfPageViewModel::PropertyChanged::raise(_In_ Object^ sender, _In_ PropertyChangedEventArgs^ e)
    {
        if (isPropertyChangedObserved)
        {
            privatePropertyChanged(sender, e);
        }
    }
}