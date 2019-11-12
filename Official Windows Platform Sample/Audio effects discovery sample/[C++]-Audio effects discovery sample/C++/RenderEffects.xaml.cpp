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
// RenderEffects.xaml.cpp
// Implementation of the RenderEffects class
//

#include "pch.h"
#include "RenderEffects.xaml.h"
#include "ppl.h"

using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Platform;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Media;
using namespace Windows::Media::MediaProperties;
using namespace Windows::Media::Capture;
using namespace Windows::Media::Devices;
using namespace Windows::Media::Effects;

using namespace SDKSample::AudioEffects;
using namespace concurrency;

String^ RenderEffects::CategoryTypeToString(AudioRenderCategory categoryType)
{
	switch (categoryType)
	{
		case AudioRenderCategory::Other:
			return L"Other";
		case AudioRenderCategory::ForegroundOnlyMedia:
			return L"ForegroundOnlyMedia";
		case AudioRenderCategory::BackgroundCapableMedia:
			return L"BackgroundCapableMedia";
		case AudioRenderCategory::Communications:
			return L"Communications";
		case AudioRenderCategory::Alerts:
			return L"Alerts";
		case AudioRenderCategory::SoundEffects:
			return L"SoundEffects";
		case AudioRenderCategory::GameEffects:
			return L"GameEffects";
		case AudioRenderCategory::GameMedia:
			return L"GameMedia";
		default:
			return L"Unknown";
	}
}

String^ RenderEffects::EffectTypeToString(AudioEffectType effectType)
{
	switch (effectType)
	{
		case AudioEffectType::Other:
			return L"Other";
		case AudioEffectType::AcousticEchoCancellation:
			return L"AcousticEchoCancellation";
		case AudioEffectType::NoiseSuppression:
			return L"NoiseSuppression";
		case AudioEffectType::AutomaticGainControl:
			return L"AutomaticGainControl";
		case AudioEffectType::BeamForming:
			return L"BeamForming";
		case AudioEffectType::ConstantToneRemoval:
			return L"ConstantToneRemoval";
		case AudioEffectType::Equalizer:
			return L"Equalizer";
		case AudioEffectType::LoudnessEqualizer:
			return L"LoudnessEqualizer";
		case AudioEffectType::BassBoost:
			return L"BassBoost";
		case AudioEffectType::VirtualSurround:
			return L"VirtualSurround";
		case AudioEffectType::VirtualHeadphones:
			return L"VirtualHeadphones";
		case AudioEffectType::SpeakerFill:
			return L"SpeakerFill";
		case AudioEffectType::RoomCorrection:
			return L"RoomCorrection";
		case AudioEffectType::BassManagement:
			return L"BassManagement";
		case AudioEffectType::EnvironmentalEffects:
			return L"EnvironmentalEffects";
		case AudioEffectType::SpeakerProtection:
			return L"SpeakerProtection";
		case AudioEffectType::SpeakerCompensation:
			return L"SpeakerCompensation";
		case AudioEffectType::DynamicRangeCompression:
			return L"DynamicRangeCompression";
		default:
			return L"Unknown";
	}
}

RenderEffects::RenderEffects()
{
	m_DevicesListBox = nullptr;
	m_EffectsListBox = nullptr;
	m_EffectsLabel = nullptr;
	m_CategoriesList = nullptr;
	InitializeComponent();
    ScenarioInit();
}

void  RenderEffects::ScenarioInit()
{
	if (m_DevicesListBox == nullptr)
	{
		m_DevicesListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("DevicesList"));
	}
	if (m_EffectsListBox == nullptr)
	{
		m_EffectsListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("EffectsList"));
	}
	if (m_EffectsLabel == nullptr)
	{
		m_EffectsLabel = safe_cast<TextBlock^>(static_cast<IFrameworkElement^>(this)->FindName("lblEffects"));
	}
	if (m_CategoriesList == nullptr)
	{
		m_CategoriesList = safe_cast<ComboBox^>(static_cast<IFrameworkElement^>(this)->FindName("CategoriesList"));
	}
	m_MonitorStarted = false;
	m_RenderEffectsManager = nullptr;
	m_DeviceList = nullptr;

	m_EffectsLabel->Text = L"Effects";
	DisplayEmptyEffectsList ();
	DisplayEmptyDevicesList();
	DisplayCategoriesList ();
}

void  RenderEffects::ScenarioClose()
{
	if (m_RenderEffectsManager != nullptr)
	{
		m_RenderEffectsManager->AudioRenderEffectsChanged -= m_renderEffectsRegistrationToken;
		m_RenderEffectsManager = nullptr;
	}
	if (m_CategoriesList != nullptr)
	{
		m_CategoriesList->LayoutUpdated -= m_renderLayoutRegistrationToken;
	}
}

void RenderEffects::ShowStatusMessage(String^ text)
{
	rootPage->NotifyUser(text, NotifyType::StatusMessage);
}

void RenderEffects::ShowExceptionMessage(Exception^ ex)
{
	rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void RenderEffects::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void RenderEffects::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()

    ScenarioClose();
}

void RenderEffects::OnRenderEffectsChanged(AudioRenderEffectsManager ^ sender, Object^ e)
{
	create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
		ref new Windows::UI::Core::DispatchedHandler([this]()
	{
		IVectorView<AudioEffect^>^ EffectsList = m_RenderEffectsManager->GetAudioRenderEffects();
		DisplayEffectsList( EffectsList );
	})));
}

IVectorView<AudioEffect^>^ RenderEffects::UpdateEffectsList(AudioRenderEffectsManager ^& EffectsManager, Windows::Foundation::EventRegistrationToken& evttoken, String^ DeviceId, AudioRenderCategory cat, bool enableMonitor)
{
	EffectsManager = AudioEffectsManager::CreateAudioRenderEffectsManager( DeviceId, cat, AudioProcessing::Default );

	if (EffectsManager != nullptr)
	{
		if (enableMonitor)
		{
			evttoken = EffectsManager->AudioRenderEffectsChanged += ref new TypedEventHandler<AudioRenderEffectsManager ^, Object ^>(this, &RenderEffects::OnRenderEffectsChanged);
		}

		return EffectsManager->GetAudioRenderEffects();
	}
	else
	{
		return nullptr;
	}
}

void RenderEffects::StartStopMonitor(Object^ sender, RoutedEventArgs^ e)
{
	if ( m_MonitorStarted == true )
	{
		// Stop Monitoring
		if (m_RenderEffectsManager != nullptr)
		{
			m_RenderEffectsManager->AudioRenderEffectsChanged -= m_renderEffectsRegistrationToken;
			m_RenderEffectsManager = nullptr;
		}
		m_MonitorStarted = false;
		// No longer in monitoring mode, re-enable "Refresh Effects List" button
		btnRefreshEffects->IsEnabled = true;

		m_EffectsLabel->Text = L"Effects";
		DisplayEmptyEffectsList();
		btnStartStopMonitor->Content = "Start Monitoring";
	}
	else
	{
		int j = -1;
		AudioRenderCategory category = CategorySelected();
		IVectorView<AudioEffect^>^ EffectsList = nullptr;

		if ( m_DevicesListBox != nullptr )
		{
			j = m_DevicesListBox->SelectedIndex;
		}

		if (j < 0)
		{
			// No item selected in device list; just query audio effects active on the default render device
			EffectsList = UpdateEffectsList(m_RenderEffectsManager, m_renderEffectsRegistrationToken, MediaDevice::GetDefaultAudioRenderId(AudioDeviceRole::Communications),
				category, true);

			m_EffectsLabel->Text = L"Effects Active on {Default Device}";
		}
		else
		{
			EffectsList = UpdateEffectsList(m_RenderEffectsManager, m_renderEffectsRegistrationToken, m_DeviceList->GetAt(j)->Id,
				category, true);

			m_EffectsLabel->Text = L"Effects Active on {" + m_DeviceList->GetAt(j)->Name + "}";
		}
		m_MonitorStarted = true;
		// Now in monitoring mode, disable "Refresh Effects List" button
		btnRefreshEffects->IsEnabled = false;
		DisplayEffectsList(EffectsList);
		btnStartStopMonitor->Content = "Stop Monitoring";
	}
}

void RenderEffects::RefreshList(Object^ sender, RoutedEventArgs^ e)
{
	int j = -1;
	AudioRenderEffectsManager^ RenderEffectsManagerLocal = nullptr;
	IVectorView<AudioEffect^>^ EffectsList = nullptr;
	AudioRenderCategory category = CategorySelected();

	if (m_DevicesListBox != nullptr)
	{
		j = m_DevicesListBox->SelectedIndex;
	}

	if (j < 0)
	{
		// No item selected in device list; just query audio effects active on the default render device
		EffectsList = UpdateEffectsList(RenderEffectsManagerLocal, m_renderEffectsRegistrationToken, Windows::Media::Devices::MediaDevice::GetDefaultAudioRenderId(AudioDeviceRole::Communications),
			category, false);

		m_EffectsLabel->Text = L"Effects Active on {Default Device}";
	}
	else
	{

		EffectsList = UpdateEffectsList(RenderEffectsManagerLocal, m_renderEffectsRegistrationToken, m_DeviceList->GetAt(j)->Id,
			category, false);

		m_EffectsLabel->Text = L"Effects Active on {" + m_DeviceList->GetAt(j)->Name + "}";
	}

	DisplayEffectsList(EffectsList);
	RenderEffectsManagerLocal = nullptr;
}

void RenderEffects::DisplayEmptyEffectsList()
{
	m_EffectsListBox->Items->Clear();
	m_EffectsListBox->Items->Append(L"Click \"Refresh Effects List\" or \"Start Monitoring\" to display audio effects");
	m_EffectsListBox->IsEnabled = false;
}

void RenderEffects::DisplayEmptyDevicesList()
{
	m_DevicesListBox->Items->Append(L"Click \"Enumerate Devices\" to display audio devices");
	m_DevicesListBox->IsEnabled = false;
}

void RenderEffects::DisplayCategoriesList()
{
	m_CategoriesList->Items->Clear();
	for (AudioRenderCategory category = AudioRenderCategory::Other; category <= AudioRenderCategory::GameMedia; category++ )
	{
		m_CategoriesList->Items->Append(CategoryTypeToString(category));
	}

	m_renderLayoutRegistrationToken = m_CategoriesList->LayoutUpdated += ref new EventHandler<Object ^>(this, &RenderEffects::OnLayoutUpdated);
}

void RenderEffects::DisplayEffectsList( IVectorView<AudioEffect^>^ EffectsList )
{
	AudioEffect^ effect = nullptr;
	AudioEffectType effectType;

	if (EffectsList != nullptr)
	{
		m_EffectsListBox->Items->Clear();
		if (EffectsList->Size > 0)
		{
			for (unsigned int i = 0; i < EffectsList->Size; i++)
			{
				effect = EffectsList->GetAt(i);
				effectType = effect->AudioEffectType;
				String ^message = ref new String(EffectTypeToString(effectType)->Data());
				m_EffectsListBox->Items->Append(message);
			}
		}
		else
		{
			m_EffectsListBox->Items->Append(L"[No Effects]");
		}
		m_EffectsListBox->IsEnabled = true;
	}
}

AudioRenderCategory RenderEffects::CategorySelected()
{
	if (m_CategoriesList != nullptr && m_CategoriesList->SelectedIndex >= 0)
	{
		return (AudioRenderCategory) m_CategoriesList->SelectedIndex;
	}
	else
	{
		// Default Category
		return AudioRenderCategory::Communications;
	}
}

void RenderEffects::EnumerateDevices(Object^ sender, RoutedEventArgs^ e)
{
	// Get the string identifier of the audio renderer
	String^ AudioSelector = MediaDevice::GetAudioRenderSelector();

	create_task(Windows::Devices::Enumeration::DeviceInformation::FindAllAsync(AudioSelector)).then([this] (DeviceInformationCollection^ DeviceInfoCollection)
	{
		if ((DeviceInfoCollection == nullptr) || (DeviceInfoCollection->Size == 0))
		{
			this->ShowStatusMessage("No Devices Found.");
		}
		else
		{
			m_DeviceList = DeviceInfoCollection;
			m_DevicesListBox->Items->Clear();

			// Enumerate through the devices and the custom properties
			for (unsigned int i = 0; i < DeviceInfoCollection->Size; i++)
			{
				DeviceInformation^ deviceInfo = DeviceInfoCollection->GetAt(i);
				String^ DeviceInfoString = deviceInfo->Name;
				m_DevicesListBox->Items->Append(DeviceInfoString);
			}
			m_DevicesListBox->IsEnabled = true;
		}
	});
}

void RenderEffects::OnLayoutUpdated(Object ^sender, Object ^args)
{
	// This Cpp sample does not include "Audio" in "Background Tasks" declaration; so disable the "BackgroundCapableMedia" category from drop-down list
	ComboBoxItem ^cb = (ComboBoxItem ^) m_CategoriesList->ContainerFromIndex((int) AudioRenderCategory::BackgroundCapableMedia);

	// Perform disabling only after ComboBoxItem has been rendered
	if (cb != nullptr)
	{
		cb->IsEnabled = FALSE;
	}
}
