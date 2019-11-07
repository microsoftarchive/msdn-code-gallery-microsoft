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
// PinFromAppbar.xaml.h
// Declaration of the PinFromAppbar class
//

#pragma once

#include "pch.h"
#include "PinFromAppbar.g.h"
#include "MainPage.xaml.h"

namespace SecondaryTiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PinFromAppbar sealed
    {
    public:
        PinFromAppbar();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;

    private:
        MainPage^ rootPage;
        Windows::UI::Xaml::Controls::Button^ pinToAppBar;
        Windows::UI::Xaml::Controls::StackPanel^ rightPanel;
        void Init();
        void ToggleAppBarButton(bool showPinButton);
        void BottomAppBar_Opened(Platform::Object^ sender, Platform::Object^ e);
        void Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        Windows::Foundation::EventRegistrationToken appBarOpenedToken;
        Windows::Foundation::EventRegistrationToken buttonClickToken;
    };
}
