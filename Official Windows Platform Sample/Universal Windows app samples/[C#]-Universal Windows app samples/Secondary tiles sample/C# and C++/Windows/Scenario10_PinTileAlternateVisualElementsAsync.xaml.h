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
// PinTileAlternateVisualElements.xaml.h
// Declaration of the PinTileAlternateVisualElementsAsync class
//

#pragma once

#include "pch.h"
#include "Scenario10_PinTileAlternateVisualElementsAsync.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PinTileAlternateVisualElementsAsync sealed
    {
    public:
        PinTileAlternateVisualElementsAsync();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;
        Windows::UI::StartScreen::VisualElementsRequestedEventArgs^ visualElementArgs;
        Windows::UI::StartScreen::VisualElementsRequestDeferral^ deferral;
        bool pinAsync;
        void PinButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void VisualElementsRequestedHandler(Windows::UI::StartScreen::SecondaryTile^ tile, Windows::UI::StartScreen::VisualElementsRequestedEventArgs^ args);
        void DelayHandler(Windows::System::Threading::ThreadPoolTimer^ sender);
        void SetAlternativeElements();
    };
}
