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
// InkLevel.xaml.h
// Declaration of the InkLevel class
//

#pragma once

#include "pch.h"
#include "InkLevel.g.h"
#include "MainPage.xaml.h"

namespace DeviceAppForPrinters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class InkLevel sealed
    {
    public:
        InkLevel();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MainPage^ rootPage;
        Platform::String^ _keyPrinterName;
        Platform::String^ _keyAsyncUIXML;
        Platform::String^ _selfPackageFamilyName;
        Windows::Foundation::IAsyncOperation<Platform::String^>^ _asyncOperation;

        void EnumeratePrinters(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void GetInkLevel(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Cancel(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void EnumerateInterface(Windows::Devices::Enumeration::DeviceInformationCollection^ interfaces);
        bool EnumerateContainer(Windows::Devices::Enumeration::Pnp::PnpObject^ pnpObject);
    };
}
