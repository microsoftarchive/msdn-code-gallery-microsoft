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
// GetUserFirstAndLastName.xaml.h
// Declaration of the GetUserFirstAndLastName class
//

#pragma once

#include "pch.h"
#include "GetUserFirstAndLastName.g.h"
#include "MainPage.xaml.h"

namespace AccountPictureName
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class GetUserFirstAndLastName sealed
    {
    public:
        GetUserFirstAndLastName();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;
        void GetFirstNameButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void GetLastNameButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
