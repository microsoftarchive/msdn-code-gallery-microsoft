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
// CaptureEffects.xaml.h
// Declaration of the CaptureEffects class
//

#pragma once

#include "pch.h"
#include "CaptureEffects.g.h"
#include "MainPage.xaml.h"

using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Media::Devices;
using namespace Windows::Media::Effects;
using namespace Windows::Media::Capture;

namespace SDKSample
{
	namespace AudioEffects
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class CaptureEffects sealed
        {
        public:
            CaptureEffects();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
			String^ EffectTypeToString(AudioEffectType effectType);
			String^ CategoryTypeToString(MediaCategory categoryType);
			void ScenarioInit();
            void ScenarioClose();

			void ShowStatusMessage(String^ text);
			void ShowExceptionMessage(Exception^ ex);

			void OnCaptureEffectsChanged(AudioCaptureEffectsManager ^ sender, Object^ e);
			void StartStopMonitor(Object^ sender, RoutedEventArgs^ e);
			void RefreshList(Object^ sender, RoutedEventArgs^ e);

			void DisplayEmptyEffectsList();
			void DisplayEmptyDevicesList();
			void DisplayCategoriesList();
			void DisplayEffectsList(IVectorView<AudioEffect^>^ EffectsList);
			IVectorView<AudioEffect^>^ UpdateEffectsList(AudioCaptureEffectsManager ^&, Windows::Foundation::EventRegistrationToken&, String^, MediaCategory, bool);

			MediaCategory CategorySelected();

			void EnumerateDevices(Object^ sender, RoutedEventArgs^ e);

            Windows::Foundation::EventRegistrationToken m_eventRegistrationToken;
			Windows::Foundation::EventRegistrationToken m_captureEffectsRegistrationToken;
			ListBox^   m_DevicesListBox;
			ListBox^   m_EffectsListBox;
			TextBlock^ m_EffectsLabel;
			ComboBox^  m_CategoriesList;
			AudioCaptureEffectsManager^ m_CaptureEffectsManager;
			DeviceInformationCollection^ m_DeviceList;
			bool m_MonitorStarted;
		};
    }
}
