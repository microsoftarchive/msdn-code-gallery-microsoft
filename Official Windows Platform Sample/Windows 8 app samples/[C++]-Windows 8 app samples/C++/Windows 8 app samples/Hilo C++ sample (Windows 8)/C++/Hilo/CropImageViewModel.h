// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "ImageBase.h"

namespace Hilo
{
    interface class IPhoto;
    class ImageNavigationData;
    class Repository;
    class ExceptionPolicy;

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view model classes interact 
    // with view classes (pages) and model classes (the state and operations of business objects).

    // The CropImageViewModel class contains the presentation logic for the crop image page.
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CropImageViewModel sealed : public ImageBase
    {
    internal:
        CropImageViewModel(std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);

        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        void Initialize(Platform::String^ photoPath);
        void CalculateInitialCropOverlayPosition(Windows::UI::Xaml::Media::GeneralTransform^ transform, float width, float height);
        void UpdateCropOverlayPostion(Windows::UI::Xaml::Controls::Primitives::Thumb^ thumb, float64 verticalChange, float64 horizontalChange, float64 minWidth, float64 minHeight);
        concurrency::task<void> CropImageAsync(float64 actualWidth);

    public:
        property Windows::UI::Xaml::Media::ImageSource^ Image
        {
            Windows::UI::Xaml::Media::ImageSource^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ SaveCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ CancelCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ ResumeCropCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property bool InProgress { bool get(); }
        property float64 CropOverlayLeft { float64 get(); }
        property float64 CropOverlayTop { float64 get(); }
        property float64 CropOverlayHeight { float64 get(); }
        property float64 CropOverlayWidth { float64 get();} 
        property bool IsCropOverlayVisible { bool get(); }

    private:
        std::shared_ptr<Repository> m_repository;
        Windows::UI::Xaml::Media::Imaging::WriteableBitmap^ m_image;
        Windows::UI::Xaml::Input::ICommand^ m_resumeCropCommand;
        Windows::UI::Xaml::Input::ICommand^ m_saveCommand;
        Windows::UI::Xaml::Input::ICommand^ m_cancelCommand;
        bool m_inProgress;
        bool m_isCropOverlayVisible;
        bool m_isSaving;
        Platform::String^ m_photoPath;

        float64 m_left;
        float64 m_top;
        float64 m_right;
        float64 m_bottom;

        float64 m_cropOverlayLeft;
        float64 m_cropOverlayTop;
        float64 m_cropOverlayHeight;
        float64 m_cropOverlayWidth;
        float64 m_actualHeight;
        float64 m_actualWidth;

        unsigned int m_cropX;
        unsigned int m_cropY;

        void ChangeInProgress(bool value);
        concurrency::task<IPhoto^> GetImagePhotoAsync();
        concurrency::task<Windows::Storage::Streams::IRandomAccessStream^> EncodeImageAsync(Windows::Storage::Streams::IRandomAccessStream^ sourceStream);
        void DoCrop(uint32_t xOffset, uint32_t yOffset, uint32_t newHeight, uint32_t newWidth, uint32_t oldWidth, byte* pSrcPixels, byte* pDestPixels);

        // Member functions that implement Commands
        void SaveImage(Platform::Object^ parameter);
        void CancelCrop(Platform::Object^ parameter);
        void Unsnap(Platform::Object^ parameter);
    };
}
