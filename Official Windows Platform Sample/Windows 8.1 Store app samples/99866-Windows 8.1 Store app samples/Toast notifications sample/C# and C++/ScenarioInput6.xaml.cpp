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
#include "ScenarioInput6.xaml.h"

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

ScenarioInput6::ScenarioInput6()
{
	InitializeComponent();

	Scenario6Looping->Click += ref new RoutedEventHandler([this] (Object^ sender, RoutedEventArgs^ e) {
		Scenario6DisplayLongToast(true);
	});
	Scenario6NoLooping->Click += ref new RoutedEventHandler([this] (Object^ sender, RoutedEventArgs^ e) {
		Scenario6DisplayLongToast(false);
	});


	Scenario6LoopingString->Click += ref new RoutedEventHandler([this] (Object^ sender, RoutedEventArgs^ e) {
		Scenario6DisplayLongToastWithStringManipulation(true);
	});
	Scenario6NoLoopingString->Click += ref new RoutedEventHandler([this] (Object^ sender, RoutedEventArgs^ e) {
		Scenario6DisplayLongToastWithStringManipulation(false);
	});
}

ScenarioInput6::~ScenarioInput6()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput6::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);

}

void ScenarioInput6::OnNavigatedFrom(NavigationEventArgs^ e)
{

}

#pragma endregion

void ScenarioInput6::OutputText(String^ text) {
	if (outputText == nullptr) 
	{
		// Get a pointer to the content within the OutputFrame.
		Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

		// Go find the elements that we need for this scenario
		outputText = dynamic_cast<TextBlock^>(outputFrame->FindName("Scenario6OutputText"));
	}

	outputText->Text = text;
}

void ScenarioInput6::Scenario6DisplayLongToast(bool loopAudio)
{
	IToastText02^ toastContent = ToastContentFactory::CreateToastText02();
	toastContent->TextHeading->Text = "Long Duration Toast";
	toastContent->Duration = ToastDuration::Long;

	if (loopAudio)
	{
		toastContent->Audio->Content = ToastAudioContent::LoopingAlarm;
		toastContent->Audio->Loop = true;
		// Long-duration Toasts can optionally loop audio using the 'loop' attribute
		toastContent->TextBodyWrap->Text = "Looping audio";
	}
	else 
	{
		toastContent->Audio->Content = ToastAudioContent::IM;
	}

	OutputText(toastContent->GetContent());

	// Create a toast, then create a ToastNotifier object to show
	// the toast
	auto toast = toastContent->CreateNotification();
	ToastNotificationManager::CreateToastNotifier()->Show(toast);
}

void ScenarioInput6::Scenario6DisplayLongToastWithStringManipulation(bool loopAudio)
{
	String^ toastXmlString = nullptr;
	if (loopAudio)
	{
		toastXmlString = "<toast duration='long'>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText02'>"
			+ "<text id='1'>Long Duration Toast</text>"
			+ "<text id='2'>Looping audio</text>"
			+ "</binding>"
			+ "</visual>"
			+ "<audio loop='true' src='ms-winsoundevent:Notification.Looping.Alarm'/>"
			+ "</toast>";
	}
	else
	{
		toastXmlString = "<toast duration='long'>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText02'>"
			+ "<text id='1'>Long Toast</text>"
			+ "</binding>"
			+ "</visual>"
			+ "<audio loop='true' src='ms-winsoundevent:Notification.IM'/>"
			+ "</toast>";
	}

	auto toastDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
	toastDOM->LoadXml(toastXmlString);

	OutputText(toastDOM->GetXml());

	// Create a toast, then create a ToastNotifier object to show
	// the toast
	auto toast = ref new ToastNotification(toastDOM);

	// If you have other applications in your package, you can specify the AppId of
	// the app to create a ToastNotifier for that application
	ToastNotificationManager::CreateToastNotifier()->Show(toast);
}

