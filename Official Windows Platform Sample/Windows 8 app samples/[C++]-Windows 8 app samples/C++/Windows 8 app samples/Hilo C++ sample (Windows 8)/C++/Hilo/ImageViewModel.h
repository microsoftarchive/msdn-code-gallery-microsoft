// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "ViewModelBase.h"
#include "PhotoPathComparer.h"

namespace Hilo
{
    interface class IPhoto;
    class ExceptionPolicy;
    class Repository;

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view model classes interact 
    // with view classes (pages) and model classes (the state and operations of business objects).

    // The ImageViewModel class contains the presentation logic for the image view page (ImageView.xaml).
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ImageViewModel sealed : public ViewModelBase
    {
    internal:
        ImageViewModel(std::shared_ptr<Repository> repository, std::shared_ptr<ExceptionPolicy> exceptionPolicy);
        
        virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ stateMap) override;
        virtual void LoadState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ stateMap) override;
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        concurrency::task<void> QueryPhotosAsync();
        concurrency::task<void> QuerySinglePhotoAsync();
        void Initialize(Platform::String^ filePath, Windows::Foundation::DateTime fileDate, Platform::String^ query);
        Platform::String^ GetStateFilePath();
        Platform::String^ GetStateQuery();
        Windows::Foundation::DateTime GetStateFileDate();

    public:
        virtual ~ImageViewModel();

        property Windows::Foundation::Collections::IObservableVector<IPhoto^>^ Photos
        { 
            Windows::Foundation::Collections::IObservableVector<IPhoto^>^ get();
        }

        property Platform::Object^ SelectedItem
        {
            Platform::Object^ get();
            void set(Platform::Object^ value);
        }

        property Platform::String^ MonthAndYear
        {
            Platform::String^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ CropImageCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ RotateImageCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ CartoonizeImageCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
        }

    private:
        std::shared_ptr<Repository> m_repository;
        bool m_runningQuerySinglePhotoAsync;
        bool m_runningQueryPhotosAsync;
        Windows::Foundation::DateTime m_fileDate;
        Platform::String^ m_filePath;
        Platform::String^ m_query;
        IPhoto^ m_photo;
        Platform::Collections::Vector<IPhoto^, PhotoPathComparer>^ m_photos;
        concurrency::cancellation_token_source m_photosCts;
        concurrency::cancellation_token_source m_photoCts;
        Windows::UI::Xaml::Input::ICommand^ m_cropImageCommand;
        Windows::UI::Xaml::Input::ICommand^ m_rotateImageCommand;
        Windows::UI::Xaml::Input::ICommand^ m_cartoonizeImageCommand;
        bool m_receivedChangeWhileRunning;

        void CropImage(Platform::Object^ parameter);
        void RotateImage(Platform::Object^ parameter);
        void CartoonizeImage(Platform::Object^ parameter);
        bool CanProcessImage(Object^ parameter);
        void OnDataChanged();
        void ClearCachedData();
    };
}
