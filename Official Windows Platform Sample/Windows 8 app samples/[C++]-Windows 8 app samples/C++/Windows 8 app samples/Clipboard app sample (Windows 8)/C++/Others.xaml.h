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
// Others.xaml.h
// Declaration of the Others class
//

#pragma once

#include "pch.h"
#include "Others.g.h"
#include "MainPage.xaml.h"

namespace Clipboard
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class Others sealed
    {
    public:
        Others();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;

        void ShowFormatButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void EmptyClipboardButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ClearOutputButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void RegisterClipboardContentChanged_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void OnClipboardChanged(Platform::Object^ sender, Platform::Object^ e);
        void OnVisibilityChanged(Platform::Object^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ e);

        void DisplayFormats(Windows::ApplicationModel::DataTransfer::DataPackageView^ dataPackageView);
        void ClearOutput();
        void HandleClipboardChanged();

        static Windows::Foundation::EventRegistrationToken contentChangedToken;
        static Windows::Foundation::EventRegistrationToken visibilityChangedToken;

        static bool windowVisible;
        static bool registerContentChanged;
        static bool needToPrintClipboardFormat;
    };
}

