// Copyright (c) Microsoft Corporation. All rights reserved

//
// PdfPageViewModel.h
// Declaration of the PdfPageViewModel class
//
#pragma once
#include "PdfViewContext.h"

namespace PdfViewModel
{
    ref class ImageSourceBase;

    [Windows::Foundation::Metadata::WebHostHidden]
    [Windows::UI::Xaml::Data::Bindable] // in c++, adding this attribute to ref classes enables data binding for more info search for 'Bindable' on the page http://go.microsoft.com/fwlink/?LinkId=254639 
    public ref class PdfPageViewModel sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    public:
        PdfPageViewModel(_In_ Windows::Data::Pdf::PdfDocument^ document, _In_ unsigned int index);

        property unsigned int PageIndex  { unsigned int get(); void set(_In_ unsigned int); };
        property float Width  { float get(); void set(_In_ float); };
        property float Height { float get(); void set(_In_ float); };
        property Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ ImageSourceVsisBackground
        { 
            Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ get(); 
            void set(_In_ Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^); 
        };
        property Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ ImageSourceVsisForeground
        {
            Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ get(); 
            void set(_In_ Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^); 
        };
        property Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^ ImageSourceSis 
        {
            Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^ get(); 
            void set(_In_ Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^); 
        };

#pragma region INotifyPropertyChanged
        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged
        {
            Windows::Foundation::EventRegistrationToken add(_In_ Windows::UI::Xaml::Data::PropertyChangedEventHandler^ e);
            void remove(_In_ Windows::Foundation::EventRegistrationToken t);
        protected:
            void raise(_In_ Object^ sender, _In_ Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e);
        }

    protected:
        void RaisePropertyChanged(_In_ Platform::String^ propertyName);
    
    private:
        bool isPropertyChangedObserved;
        event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ privatePropertyChanged;

#pragma endregion

    internal:
        void CreateImageSourceIfNotAvailable();
        void SetImageSource(_In_ ImageSourceBase^ imageSourceLocal);
        bool IsUnloaded();
        void UpdateSurface();
        Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^ GetImageSource();
        // We need this as PdfViewContext is not public class
        void SetViewContext(_In_ PdfViewContext^ viewContext)
        {
            pdfViewContext = viewContext;
        }

    private:
        ImageSourceBase^ imageSource;
        Windows::Data::Pdf::PdfDocument^ pdfDocument;
        PdfViewContext^ pdfViewContext;
        unsigned int pageIndex;
        // This stores the subscription count for all properties of the 
        // item. This variable is need to virtualize the item, so as to
        // clean the image source when the item is off view.
        unsigned int propertySubscriptionCount;
    };
}
