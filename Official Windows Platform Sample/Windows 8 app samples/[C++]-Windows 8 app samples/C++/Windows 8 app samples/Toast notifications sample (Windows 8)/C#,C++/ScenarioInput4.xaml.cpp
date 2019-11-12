// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Input.xaml.cpp
// Implementation of the Scenario1Input class
//

#include "pch.h"
#include "ScenarioInput4.xaml.h"

using namespace ToastsSampleCPP;

using namespace NotificationsExtensions::ToastContent;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput4::ScenarioInput4()
{
	InitializeComponent();

	Scenario4DisplayToastSilent->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSound(ToastAudioContent::Silent, "Silent"); 
	});
	Scenario4DisplayToastDefault->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSound(ToastAudioContent::Default, "Default"); 
	});
	Scenario4DisplayToastMail->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSound(ToastAudioContent::Mail, "Mail"); 
	});
	Scenario4DisplayToastSMS->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSound(ToastAudioContent::SMS, "SMS"); 
	});
	Scenario4DisplayToastIM->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSound(ToastAudioContent::IM, "IM"); 
	});
	Scenario4DisplayToastReminder->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSound(ToastAudioContent::Reminder, "Reminder"); 
	});


	Scenario4DisplayToastSilentString->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSoundWithStringManipulation(ToastAudioContent::Silent, "Silent"); 
	});
	Scenario4DisplayToastDefaultString->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSoundWithStringManipulation(ToastAudioContent::Default, "Default"); 
	});
	Scenario4DisplayToastMailString->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) { 
		Scenario4DisplayToastWithSoundWithStringManipulation(ToastAudioContent::Mail, "Mail"); 
	});
}

ScenarioInput4::~ScenarioInput4()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput4::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

void ScenarioInput4::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

#pragma endregion

void ScenarioInput4::OutputText(String^ text) {
	if (outputText == nullptr) 
	{
		// Get a pointer to the content within the OutputFrame.
		Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

		// Go find the elements that we need for this scenario
		outputText = dynamic_cast<TextBlock^>(outputFrame->FindName("Scenario4OutputText"));
	}

	outputText->Text = text;
}

void ScenarioInput4::Scenario4DisplayToastWithSound(ToastAudioContent audioContent, String^ audioSrc)
{
	IToastText02^ toastContent = ToastContentFactory::CreateToastText02();
	toastContent->TextHeading->Text = "Sound:";
	toastContent->TextBodyWrap->Text = audioSrc;

	toastContent->Audio->Content = audioContent;

	OutputText(toastContent->GetContent());

	// Create a toast, then create a ToastNotifier object to show
	// the toast
	auto toast = toastContent->CreateNotification();

	// If you have other applications in your package, you can specify the AppId of
	// the app to create a ToastNotifier for that application
	ToastNotificationManager::CreateToastNotifier()->Show(toast);
}

void ScenarioInput4::Scenario4DisplayToastWithSoundWithStringManipulation(ToastAudioContent audioContent, String^ audioSrc)
{
	String^ toastXmlString = nullptr;

	if (audioSrc->Equals("Silent"))
	{
		toastXmlString = "<toast>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText02'>"
			+ "<text id='1'>Sound:</text>"
			+ "<text id='2'>" + audioSrc + "</text>"
			+ "</binding>"
			+ "</visual>"
			+ "<audio silent='true'/>"
			+ "</toast>";

	}
	else if (audioSrc->Equals("Default"))
	{
		toastXmlString = "<toast>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText02'>"
			+ "<text id='1'>Sound:</text>"
			+ "<text id='2'>" + audioSrc + "</text>"
			+ "</binding>"
			+ "</visual>"
			+ "</toast>";

	}
	else
	{
		toastXmlString = "<toast>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText02'>"
			+ "<text id='1'>Sound:</text>"
			+ "<text id='2'>" + audioSrc + "</text>"
			+ "</binding>"
			+ "</visual>"
			+ "<audio src='ms-winsoundevent:Notification." + audioSrc + "'/>"
			+ "</toast>";
	}

	auto toastDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
	try 
	{
		toastDOM->LoadXml(toastXmlString);

		OutputText(toastDOM->GetXml());

		// Create a toast, then create a ToastNotifier object to show
		// the toast
		auto toast = ref new ToastNotification(toastDOM);

		// If you have other applications in your package, you can specify the AppId of
		// the app to create a ToastNotifier for that application
		ToastNotificationManager::CreateToastNotifier()->Show(toast);
	}
	catch (Exception^ e)
	{
		rootPage->NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType::ErrorMessage);
	}
}