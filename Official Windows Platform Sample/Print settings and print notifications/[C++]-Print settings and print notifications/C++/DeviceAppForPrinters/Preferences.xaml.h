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
// Preferences.xaml.h
// Declaration of the Preferences class
//

#pragma once

#include "pch.h"
#include "Preferences.g.h"
#include "MainPage.xaml.h"

namespace DeviceAppForPrinters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class Preferences sealed
    {
    public:
        Preferences();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MainPage^ rootPage;
        Windows::UI::Core::CoreDispatcher^ _dispatcher;
        Windows::Devices::Printers::Extensions::PrintTaskConfigurationSaveRequest^ _request;
        Windows::Devices::Printers::Extensions::PrintTaskConfigurationSaveRequestedDeferral^ _deferral;
        Windows::Foundation::EventRegistrationToken _saveRequestedToken;
        Platform::Collections::Vector<Platform::String^>^ _features;
        Platform::Collections::Vector<int>^ _selections;
        Platform::Collections::Vector<Windows::UI::Xaml::Controls::TextBlock^>^ _featureLabels;
        Platform::Collections::Vector<Windows::UI::Xaml::Controls::ComboBox^>^ _featureBoxes;
        Platform::Collections::Vector<Windows::UI::Xaml::Controls::TextBlock^>^ _featureConstraints;

        void Initialize();
        void NotifyUser(Platform::String^ strMessage, NotifyType type);
        void AddMessage(Platform::String^ strMessage);
        void AllowSettingsChange(bool bAllow);
        void CheckConstraint(void);
        void Save();
        void OnFeatureOptionsChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ args);
        void OnSaveRequested(Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ sender, Windows::Devices::Printers::Extensions::PrintTaskConfigurationSaveRequestedEventArgs^ args);
    };
}
