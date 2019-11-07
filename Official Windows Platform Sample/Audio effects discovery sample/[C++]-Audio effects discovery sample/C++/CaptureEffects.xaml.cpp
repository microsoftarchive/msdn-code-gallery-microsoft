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
// CaptureEffects.xaml.cpp
// Implementation of the CaptureEffects class
//

#include "pch.h"
#include "CaptureEffects.xaml.h"
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

String^ CaptureEffects::CategoryTypeToString( MediaCategory categoryType )
{
	switch (categoryType)
	{
		case MediaCategory::Other:
			return L"Other";
		case MediaCategory::Communications:
			return L"Communications";
		default:
			return L"Unknown";
	}
}

String^ CaptureEffects::EffectTypeToString(AudioEffectType effectType)
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

CaptureEffects::CaptureEffects()
{
	m_DevicesListBox = nullptr;
	m_EffectsListBox = nullptr;
	m_EffectsLabel = nullptr;
	m_CategoriesList = nullptr;
	InitializeComponent();
    ScenarioInit();
}

void  CaptureEffects::ScenarioInit()
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
	m_CaptureEffectsManager = nullptr;
	m_DeviceList = nullptr;

	m_EffectsLabel->Text = L"Effects";
	DisplayEmptyEffectsList ();
	DisplayEmptyDevicesList();
	DisplayCategoriesList ();
}

void  CaptureEffects::ScenarioClose()
{
	if (m_CaptureEffectsManager != nullptr)
	{
		m_CaptureEffectsManager->AudioCaptureEffectsChanged -= m_captureEffectsRegistrationToken;
		m_CaptureEffectsManager = nullptr;
	}
}

void CaptureEffects::ShowStatusMessage(String^ text)
{
	rootPage->NotifyUser(text, NotifyType::StatusMessage);
}

void CaptureEffects::ShowExceptionMessage(Exception^ ex)
{
	rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CaptureEffects::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void CaptureEffects::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()

    ScenarioClose();
}

void CaptureEffects::OnCaptureEffectsChanged(AudioCaptureEffectsManager ^ sender, Object^ e)
{
	create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
		ref new Windows::UI::Core::DispatchedHandler([this]()
	{
		IVectorView<AudioEffect^>^ EffectsList = m_CaptureEffectsManager->GetAudioCaptureEffects();	
		DisplayEffectsList( EffectsList );
	})));
}

IVectorView<AudioEffect^>^ CaptureEffects::UpdateEffectsList(AudioCaptureEffectsManager ^& EffectsManager, Windows::Foundation::EventRegistrationToken& evttoken, String^ DeviceId, MediaCategory cat, bool enableMonitor)
{
	EffectsManager = AudioEffectsManager::CreateAudioCaptureEffectsManager( DeviceId, cat, AudioProcessing::Default );

	if (EffectsManager != nullptr)
	{
		if (enableMonitor)
		{
			evttoken = EffectsManager->AudioCaptureEffectsChanged += ref new TypedEventHandler<AudioCaptureEffectsManager ^, Object ^>(this, &CaptureEffects::OnCaptureEffectsChanged);
		}

		return EffectsManager->GetAudioCaptureEffects();
	}
	else
	{
		return nullptr;
	}
}

void CaptureEffects::StartStopMonitor(Object^ sender, RoutedEventArgs^ e)
{
	if ( m_MonitorStarted == true )
	{
		// Stop Monitoring
		if (m_CaptureEffectsManager != nullptr)
		{
			m_CaptureEffectsManager->AudioCaptureEffectsChanged -= m_captureEffectsRegistrationToken;
			m_CaptureEffectsManager = nullptr;
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
		MediaCategory category = CategorySelected();
		IVectorView<AudioEffect^>^ EffectsList = nullptr;

		if ( m_DevicesListBox != nullptr )
		{
			j = m_DevicesListBox->SelectedIndex;
		}

		if (j < 0)
		{
			// No item selected in device list; just query audio effects active on the default render device
			EffectsList = UpdateEffectsList(m_CaptureEffectsManager, m_captureEffectsRegistrationToken, MediaDevice::GetDefaultAudioCaptureId(AudioDeviceRole::Communications),
				category, true);

			m_EffectsLabel->Text = L"Effects Active on {Default Device}";
		}
		else
		{
			EffectsList = UpdateEffectsList(m_CaptureEffectsManager, m_captureEffectsRegistrationToken, m_DeviceList->GetAt(j)->Id,
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

void CaptureEffects::RefreshList(Object^ sender, RoutedEventArgs^ e)
{
	int j = -1;
	AudioCaptureEffectsManager^ CaptureEffectsManagerLocal = nullptr;
	IVectorView<AudioEffect^>^ EffectsList = nullptr;
	MediaCategory category = CategorySelected();

	if (m_DevicesListBox != nullptr)
	{
		j = m_DevicesListBox->SelectedIndex;
	}

	if (j < 0)
	{
		// No item selected in device list; just query audio effects active on the default capture device
		EffectsList = UpdateEffectsList(CaptureEffectsManagerLocal, m_captureEffectsRegistrationToken, MediaDevice::GetDefaultAudioCaptureId(AudioDeviceRole::Communications),
			category, false);

		m_EffectsLabel->Text = L"Effects Active on {Default Device}";
	}
	else
	{

		EffectsList = UpdateEffectsList(CaptureEffectsManagerLocal, m_captureEffectsRegistrationToken, m_DeviceList->GetAt(j)->Id,
			category, false);

		m_EffectsLabel->Text = L"Effects Active on {" + m_DeviceList->GetAt(j)->Name + "}";
	}

	DisplayEffectsList(EffectsList);
	CaptureEffectsManagerLocal = nullptr;
}

void CaptureEffects::DisplayEmptyEffectsList()
{
	m_EffectsListBox->Items->Clear();
	m_EffectsListBox->Items->Append(L"Click \"Refresh Effects List\" or \"Start Monitoring\" to display audio effects");
	m_EffectsListBox->IsEnabled = false;
}

void CaptureEffects::DisplayEmptyDevicesList()
{
	m_DevicesListBox->Items->Append(L"Click \"Enumerate Devices\" to display audio devices");
	m_DevicesListBox->IsEnabled = false;
}

void CaptureEffects::DisplayCategoriesList()
{
	m_CategoriesList->Items->Clear();
	for (MediaCategory category = MediaCategory::Other; category <= MediaCategory::Communications; category++)
	{
		m_CategoriesList->Items->Append(CategoryTypeToString(category));
	}
}

void CaptureEffects::DisplayEffectsList(IVectorView<AudioEffect^>^ EffectsList)
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

MediaCategory CaptureEffects::CategorySelected()
{
	if (m_CategoriesList != nullptr && m_CategoriesList->SelectedIndex >= 0)
	{
		return (MediaCategory) m_CategoriesList->SelectedIndex;
	}
	else
	{
		// Default Category
		return MediaCategory::Communications;
	}
}

void CaptureEffects::EnumerateDevices(Object^ sender, RoutedEventArgs^ e)
{
	// Get the string identifier of the audio capturer
	String^ AudioSelector = MediaDevice::GetAudioCaptureSelector();

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
