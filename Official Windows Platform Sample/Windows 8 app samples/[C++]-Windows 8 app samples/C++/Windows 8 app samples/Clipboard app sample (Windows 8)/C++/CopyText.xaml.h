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
// CopyText.xaml.h
// Declaration of the CopyText class
//

#pragma once

#include "pch.h"
#include "CopyText.g.h"
#include "MainPage.xaml.h"

namespace Clipboard
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CopyText sealed
    {
    public:
        CopyText();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;

        // ScenarioInput1 click events
        void CopyButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PasteButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        Platform::String^ htmlDescription;
        Platform::String^ textDescription;
        Platform::String^ imgSrc;
    };
}
