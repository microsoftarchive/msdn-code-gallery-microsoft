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
#include "ScenarioInput1.xaml.h"

using namespace ToastsSampleCPP;

using namespace NotificationsExtensions::ToastContent;
using namespace Platform;
using namespace Windows::Data::Xml::Dom;
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

ScenarioInput1::ScenarioInput1()
{
	InitializeComponent();

	Scenario1DisplayToastText01->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToast(ToastTemplateType::ToastText01); 
	});
	Scenario1DisplayToastText02->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToast(ToastTemplateType::ToastText02);
	});

	Scenario1DisplayToastText03->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToast(ToastTemplateType::ToastText03); 
	});

	Scenario1DisplayToastText04->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToast(ToastTemplateType::ToastText04);
	});

	Scenario1DisplayToastText01String->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToastWithStringManipulation(ToastTemplateType::ToastText01); 
	});
	Scenario1DisplayToastText02String->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToastWithStringManipulation(ToastTemplateType::ToastText02);
	});

	Scenario1DisplayToastText03String->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToastWithStringManipulation(ToastTemplateType::ToastText03); 
	});

	Scenario1DisplayToastText04String->Click += ref new RoutedEventHandler([this](Object^ sender, RoutedEventArgs^ e) {
		Scenario1DisplayToastWithStringManipulation(ToastTemplateType::ToastText04);
	});
}

ScenarioInput1::~ScenarioInput1()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput1::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

void ScenarioInput1::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

#pragma endregion

void ScenarioInput1::OutputText(String^ text) {
	if (outputText == nullptr) 
	{
		// Get a pointer to the content within the OutputFrame.
		Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

		// Go find the elements that we need for this scenario
		outputText = dynamic_cast<TextBlock^>(outputFrame->FindName("Scenario1OutputText"));
	}

	outputText->Text = text;
}

void ScenarioInput1::Scenario1DisplayToast(ToastTemplateType templateType)
{
	// Creates a toast using the notification object model, which is another project
	// in this solution.  For an example using Xml manipulation, see the function
	// DisplayToastUsingXmlManipulation below.
	IToastNotificationContent^ toastContent = nullptr;

	if (templateType == ToastTemplateType::ToastText01)
	{
		IToastText01^ specializedContent = ToastContentFactory::CreateToastText01();
		specializedContent->TextBodyWrap->Text = "Body text that wraps over three lines";
		toastContent = specializedContent;
	}
	else if (templateType == ToastTemplateType::ToastText02)
	{
		IToastText02^ specializedContent = ToastContentFactory::CreateToastText02();
		specializedContent->TextHeading->Text = "Heading text";
		specializedContent->TextBodyWrap->Text = "Body text that wraps over two lines";
		toastContent = specializedContent;
	}
	else if (templateType == ToastTemplateType::ToastText03)
	{
		IToastText03^ specializedContent = ToastContentFactory::CreateToastText03();
		specializedContent->TextHeadingWrap->Text = "Heading text that is very long and wraps over two lines";
		specializedContent->TextBody->Text = "Body text";
		toastContent = specializedContent;
	}
	else if (templateType == ToastTemplateType::ToastText04)
	{
		IToastText04^ specializedContent = ToastContentFactory::CreateToastText04();
		specializedContent->TextHeading->Text = "Heading text";
		specializedContent->TextBody1->Text = "First body text";
		specializedContent->TextBody2->Text = "Second body text";
		toastContent = specializedContent;
	}

    if (toastContent != nullptr)
    {
        OutputText(toastContent->GetContent());
        // Create a toast, then create a ToastNotifier object to show
        // the toast
        ToastNotification^ toast = toastContent->CreateNotification();

        // If you have other applications in your package, you can specify the AppId of
        // the app to create a ToastNotifier for that application
        ToastNotificationManager::CreateToastNotifier()->Show(toast);
    }
}

void ScenarioInput1::Scenario1DisplayToastWithStringManipulation(ToastTemplateType templateType)
{
	String^ toastXmlString = nullptr;
	if (templateType == ToastTemplateType::ToastText01)
	{
		toastXmlString = "<toast>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText01'>"
			+ "<text id='1'>Body text that wraps over three lines</text>"
			+ "</binding>"
			+ "</visual>"
			+ "</toast>";
	}
	else if (templateType == ToastTemplateType::ToastText02)
	{
		toastXmlString = "<toast>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText02'>"
			+ "<text id='1'>Heading text</text>"
			+ "<text id='2'>Body text that wraps over two lines</text>"
			+ "</binding>"
			+ "</visual>"
			+ "</toast>";
	}
	else if (templateType == ToastTemplateType::ToastText03)
	{
		toastXmlString = "<toast>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText03'>"
			+ "<text id='1'>Heading text that is very long and wraps over two lines</text>"
			+ "<text id='2'>Body text</text>"
			+ "</binding>"
			+ "</visual>"
			+ "</toast>";
	}
	else if (templateType == ToastTemplateType::ToastText04)
	{
		toastXmlString = "<toast>"
			+ "<visual version='1'>"
			+ "<binding template='ToastText04'>"
			+ "<text id='1'>Heading text</text>"
			+ "<text id='2'>First body text</text>"
			+ "<text id='3'>Second body text</text>"
			+ "</binding>"
			+ "</visual>"
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
