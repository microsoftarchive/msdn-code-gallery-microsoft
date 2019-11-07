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
// RenderEffects.xaml.h
// Declaration of the RenderEffects class
//

#pragma once

#include "pch.h"
#include "RenderEffects.g.h"
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
using namespace Windows::Media::Render;

namespace SDKSample
{
	namespace AudioEffects
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class RenderEffects sealed
        {
        public:
            RenderEffects();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
			String^ EffectTypeToString(AudioEffectType effectType);
			String^ CategoryTypeToString(AudioRenderCategory categoryType);
			void ScenarioInit();
            void ScenarioClose();

			void ShowStatusMessage(String^ text);
			void ShowExceptionMessage(Exception^ ex);

			void OnRenderEffectsChanged(AudioRenderEffectsManager ^ sender, Object^ e);
			void StartStopMonitor(Object^ sender, RoutedEventArgs^ e);
			void RefreshList(Object^ sender, RoutedEventArgs^ e);

			void DisplayEmptyEffectsList();
			void DisplayEmptyDevicesList();
			void DisplayCategoriesList();
			void DisplayEffectsList(IVectorView<AudioEffect^>^ EffectsList);
			IVectorView<AudioEffect^>^ UpdateEffectsList(AudioRenderEffectsManager ^&, Windows::Foundation::EventRegistrationToken&, String^, AudioRenderCategory, bool );

			AudioRenderCategory CategorySelected();

			void EnumerateDevices(Object^ sender, RoutedEventArgs^ e);

            Windows::Foundation::EventRegistrationToken m_eventRegistrationToken;
			Windows::Foundation::EventRegistrationToken m_renderEffectsRegistrationToken;
			ListBox^   m_DevicesListBox;
			ListBox^   m_EffectsListBox;
			TextBlock^ m_EffectsLabel;
			ComboBox^  m_CategoriesList;
			AudioRenderEffectsManager^ m_RenderEffectsManager;
			DeviceInformationCollection^ m_DeviceList;
			bool m_MonitorStarted;
			Windows::Foundation::EventRegistrationToken m_renderLayoutRegistrationToken;
			void OnLayoutUpdated(Object ^sender, Object ^args);
		};
    }
}
