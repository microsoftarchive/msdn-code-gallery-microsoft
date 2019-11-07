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
// GetAccountPicture.xaml.h
// Declaration of the GetAccountPicture class
//

#pragma once

#include "pch.h"
#include "GetAccountPicture.g.h"
#include "MainPage.xaml.h"

namespace AccountPictureName
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class GetAccountPicture sealed
    {
    public:
        GetAccountPicture();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;
        void GetSmallImageButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void GetLargeImageButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void GetVideoButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void HideImageAndVideoControls();
    };
}
