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
// DeviceProperties.xaml.h
// Declaration of the DeviceProperties class
//

#pragma once

#include "pch.h"
#include "DeviceProperties.g.h"
#include "MainPage.xaml.h"

using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Devices::Enumeration;

namespace SDKSample
{
    namespace MIDI
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeviceProperties sealed
        {
        public:
            DeviceProperties();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:

            void ScenarioInit();
            void ScenarioClose();

            void ShowStatusMessage(String^ text);
            void ShowExceptionMessage(Exception^ ex);

            void DisplayEmptyInPropertiesBox();
            void DisplayEmptyOutPropertiesBox();
            void DisplayEmptyInPortsBox();
            void DisplayEmptyOutPortsBox();

            void DisplayPropertiesList(DeviceInformation^ devInfo, ListBox^ listBox);

            void EnumeratePorts(Object^ sender, RoutedEventArgs^ e);
            void InPortsListSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e);
            void OutPortsListSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e);
            void TogglePortMonitor(Object^ sender, RoutedEventArgs^ e);

            MainPage^ _rootPage;

            ListBox^ _inPortsListBox;
            ListBox^ _outPortsListBox;
            ListBox^ _inPropertiesListBox;
            ListBox^ _outPropertiesListBox;

            Button^ _enumeratePortsButton;

            bool _enumWithWatcher;

            ref class MidiDeviceWatcher sealed
            {
            public:
                MidiDeviceWatcher(String ^ midiSelector, Windows::UI::Core::CoreDispatcher ^ dispatcher, ListBox ^ portListBox);
                virtual ~MidiDeviceWatcher();
                void Start();
                void Stop();
                DeviceInformationCollection^ GetDeviceInformationCollection();
                void UpdatePorts();

            private:

                void OnPortAdded(DeviceWatcher^ deviceWatcher, DeviceInformation^ devInfo);
                void OnPortRemoved(DeviceWatcher^ deviceWatcher, DeviceInformationUpdate^ devInfoUpdate);
                void OnPortUpdated(DeviceWatcher^ deviceWatcher, DeviceInformationUpdate^ devInfoUpdate);
                void OnPortEnumCompleted(DeviceWatcher^ deviceWatcher, Object^ obj);

                Windows::Foundation::EventRegistrationToken _portAddedToken;
                Windows::Foundation::EventRegistrationToken _portRemovedToken;
                Windows::Foundation::EventRegistrationToken _portUpdatedToken;
                Windows::Foundation::EventRegistrationToken _portEnumCompleteToken;

                String ^ _midiSelectorString;
                Windows::UI::Core::CoreDispatcher^ _coreDispatcher;
                ListBox ^ _portListBox;
                DeviceWatcher ^ _deviceWatcher;
                DeviceInformationCollection^ _devInfoCollection;
                bool _enumCompleted;
            };

            MidiDeviceWatcher ^ _midiInDeviceWatcher;
            MidiDeviceWatcher ^ _midiOutDeviceWatcher;
        };
    }
}