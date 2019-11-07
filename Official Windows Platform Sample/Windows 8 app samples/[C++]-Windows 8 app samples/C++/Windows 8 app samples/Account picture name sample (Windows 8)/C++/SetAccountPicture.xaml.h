//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// SetAccountPicture.xaml.h
// Declaration of the SetAccountPicture class
//

#pragma once

#include "pch.h"
#include "SetAccountPicture.g.h"
#include "MainPage.xaml.h"

namespace AccountPictureName
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class SetAccountPicture sealed
    {
    public:
        SetAccountPicture();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;
        void SetImageButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SetVideoButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PictureChanged(Platform::Object^ sender, Platform::Object^ e);

        static Windows::Foundation::EventRegistrationToken SetAccountPicture::accountPictureChangedToken;
    };
}
