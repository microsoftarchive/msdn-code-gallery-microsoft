// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "ImageBase.h"

struct AmpPixel;

namespace Hilo
{
    interface class IPhoto;
    class Repository;
    class ExceptionPolicy;

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view model classes interact 
    // with view classes (pages) and model classes (the state and operations of business objects).

    // The CartoonizeImageViewModel class contains the presentation logic of the cartoon effect page (CartoonizeImageView.xaml).
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CartoonizeImageViewModel sealed : public ImageBase
    {
    internal:
        CartoonizeImageViewModel(std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);

        virtual void LoadState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ stateMap) override;
        virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ stateMap) override;
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        void Initialize(Platform::String^ photoPath);

    public:
        virtual ~CartoonizeImageViewModel();

        property Windows::UI::Xaml::Media::ImageSource^ Image
        {
            Windows::UI::Xaml::Media::ImageSource^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ CartoonizeCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ ResumeCartoonizeCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ SaveCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ CancelCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }
        
        property bool InProgress { bool get(); }
        property float64 NeighborWindow { float64 get(); void set(float64 value); }
        property float64 Phases { float64 get(); void set(float64 value); }

    private:
        Windows::UI::Xaml::Media::Imaging::WriteableBitmap^ m_image;
        concurrency::task<void> m_initializationTask;
        std::shared_ptr<Repository> m_repository;
        Platform::String^ m_photoPath;
        bool m_inProgress;
        bool m_isSaving;
        bool m_isAmpPixelArrayPopulated;
        bool m_isSourcePixelsPopulated;
        bool m_checkedForHardwareAcceleration;
        bool m_useHardwareAcceleration;
        concurrency::cancellation_token_source m_cts;
        float64 m_neighborWindow;
        float64 m_phases;
        AmpPixel* m_pOriginalPixels;
        byte* m_pSourcePixels;
        unsigned int m_scaledWidth;
        unsigned int m_scaledHeight;

        Windows::UI::Xaml::Input::ICommand^ m_cartoonizeCommand;
        Windows::UI::Xaml::Input::ICommand^ m_resumeCartoonizeCommand;
        Windows::UI::Xaml::Input::ICommand^ m_saveCommand;
        Windows::UI::Xaml::Input::ICommand^ m_cancelCommand;
        
        void ChangeInProgress(bool value);
        void CheckForCancellation();
        void EvaluateCommands();
        void CopyPixelDataToPlatformArray(Windows::Storage::Streams::IBuffer^ buffer, Platform::Array<unsigned char, 1>^ pixels, unsigned int width, unsigned int height);
        concurrency::task<IPhoto^> GetImagePhotoAsync();
        concurrency::task<Windows::Storage::Streams::IRandomAccessStream^> EncodeImageAsync(Windows::Storage::Streams::IRandomAccessStream^ sourceStream);
        bool ScaleImageDimensions(unsigned int origWidth, unsigned int origHeight);

        AmpPixel* CopyPixelDataToAmpPixelArray(byte* sourcePixels, unsigned int width, unsigned int height, const unsigned int size);
        void CopyAmpPixelArrayToPixelData(byte* destPixels, AmpPixel* newPixels, unsigned int width, unsigned int height);
        concurrency::task<void> CartoonizeImageAmpAsync(concurrency::cancellation_token token);
        concurrency::task<void> CartoonizeImagePPLAsync(concurrency::cancellation_token token);

        void CartoonizeImage(Platform::Object^ parameter);
        void Unsnap(Platform::Object^ parameter);
        void SaveImage(Platform::Object^ parameter);
        void CancelCartoonize(Platform::Object^ parameter);
        bool IsCartoonizing(Platform::Object^ parameter);
    };
}
