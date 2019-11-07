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
// MainPage.xaml.h
// Declaration of the MainPage.xaml class.
//

#pragma once

#include "pch.h"
#include "MainPage.g.h"
#include "Common\LayoutAwarePage.h" // Required by generated header
#include "Constants.h"
#include "PrintHelper.h"

namespace DeviceAppForPrinters
{
    public enum class NotifyType
    {
        StatusMessage,
        ErrorMessage,
        WaitMessage
    };

    public ref class MainPageSizeChangedEventArgs sealed
    {
    public:
        property double Width
        {
            double get()
            {
                return width;
            }

            void set(double value)
            {
                width = value;
            }
        }

    private:
        double width;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainPage sealed
    {
    public:
        MainPage();

        property Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ Configuration
        {
            Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ get() { return _configuration; }
            void set(Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ value)
            {
                _configuration = value;
                SetContext(_configuration->PrinterExtensionContext);
            }
        }		
        property DeviceAppForPrinters::PrintHelper^ PrintHelper
        {
            DeviceAppForPrinters::PrintHelper^ get() { return _printHelper; }
        }
        property Platform::Object^ PrinterExtensionContext
        {
            Platform::Object^ get() { return _printerExtensionContext; }
            void set(Platform::Object^ context) { SetContext(context); }
        }
        property Platform::String^ Notification
        {
            Platform::String^ get() { return _notification; }
            void set(Platform::String^ value) { _notification = value; }
        }

        property Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ LaunchArgs;

        void NotifyUser(Platform::String^ strMessage, NotifyType type);
        void AddMessage(Platform::String^ strMessage);
        void LoadScenario(Platform::String^ scenarioName);
        event Windows::Foundation::EventHandler<Platform::Object^>^ ScenarioLoaded;
        event Windows::Foundation::EventHandler<MainPageSizeChangedEventArgs^>^ MainPageResized;


    private:
        bool _isDebugMode;
        Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ _configuration;
        Platform::Object^ _printerExtensionContext;
        DeviceAppForPrinters::PrintHelper^ _printHelper;
        Platform::String^ _currentScenario;
        Platform::String^ _notification;

        bool SetContext(Platform::Object^ context);

        void SetDeviceAppForPrinters(Platform::String^ strFeature);
        void PopulateScenarios();
        void InvalidateSize();
        void InvalidateLayout();
        ~MainPage();

        Platform::Collections::Vector<Object^>^ ScenarioList;
        Windows::UI::Xaml::Controls::Frame^ HiddenFrame;
        void Footer_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);


    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        void MainPage_SizeChanged(Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
        void Scenarios_SelectionChanged(Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

    internal:
        static MainPage^ Current;

    };

}
