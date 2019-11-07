// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "ViewModelBase.h"

namespace Hilo
{
    ref class HubPhotoGroup;
    interface class IPhoto;
    class ExceptionPolicy;

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view model classes interact 
    // with view classes (pages) and model classes (the state and operations of business objects).

    // The MainHubViewModel class contains the presentation logic for the main hub page (MainHubView.xaml).
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainHubViewModel sealed : public ViewModelBase
    {
    internal:
        MainHubViewModel(Windows::Foundation::Collections::IObservableVector<HubPhotoGroup^>^ photoGroups, std::shared_ptr<ExceptionPolicy> exceptionPolicy);
        
    public:
        virtual ~MainHubViewModel();

        property Windows::Foundation::Collections::IObservableVector<HubPhotoGroup^>^ PhotoGroups
        {
            Windows::Foundation::Collections::IObservableVector<HubPhotoGroup^>^ get();
        }

        property Windows::UI::Xaml::Input::ICommand^ NavigateToPicturesCommand
        {
            Windows::UI::Xaml::Input::ICommand^ get();
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

        // These are really IPhoto^ but XAML cannot convert any underlying
        // real object (e.g. Photo) back to IPhoto which results in strange
        // side effects (such as the AppBar not working).
        property Platform::Object^ SelectedItem
        {
            Platform::Object^ get();
            void set(Platform::Object^ value);
        }

    private:
        Windows::Foundation::Collections::IObservableVector<HubPhotoGroup^>^ m_photoGroups;
        Windows::UI::Xaml::Input::ICommand^ m_navigateToPicturesCommand;
        Windows::UI::Xaml::Input::ICommand^ m_cropImageCommand;
        Windows::UI::Xaml::Input::ICommand^ m_rotateImageCommand;
        Windows::UI::Xaml::Input::ICommand^ m_cartoonizeImageCommand;
        IPhoto^ m_photo;
        HubPhotoGroup^ m_pictureGroup;
        Windows::Foundation::EventRegistrationToken m_pictureGroupPropertyChangedToken;

        void NavigateToPictures(Platform::Object^ parameter);
        bool CanNavigateToPictures(Platform::Object^ parameter);
        void CropImage(Platform::Object^ parameter);
        void RotateImage(Platform::Object^ parameter);
        void CartoonizeImage(Platform::Object^ parameter);
        bool CanProcessImage(Platform::Object^ parameter);
        void OnPictureGroupPropertyChanged(Platform::Object^ sender, Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e);
    };
}
