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

using namespace ScheduledNotificationsSampleCPP;

using namespace NotificationsExtensions::TileContent;
using namespace NotificationsExtensions::ToastContent;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput1::ScenarioInput1()
{
	InitializeComponent();
	ScheduleButton->Click += ref new RoutedEventHandler(this, &ScenarioInput1::Schedule_Click);
	ScheduleButtonString->Click += ref new RoutedEventHandler(this, &ScenarioInput1::Schedule_Click);

}

void ScenarioInput1::Schedule_Click(Object^ sender, RoutedEventArgs^ e)
{
	try
	{
		int dueTimeInSeconds = _wtoi(FutureTimeBox->Text->Data());      
		if (dueTimeInSeconds <= 0) throw ref new InvalidArgumentException();

		String^ updateString = StringBox->Text;

		// Use the DateTime type to specify the display time
		FILETIME fileTime;
		GetSystemTimeAsFileTime(&fileTime);

		ULARGE_INTEGER universalTime;
		universalTime.LowPart = fileTime.dwLowDateTime;
		universalTime.HighPart = fileTime.dwHighDateTime;
		universalTime.QuadPart += dueTimeInSeconds * 10000000;  

		DateTime dueTime;
		dueTime.UniversalTime = universalTime.QuadPart;

		wchar_t pszIdNumber[6];
		HRESULT hr = StringCchPrintf(pszIdNumber, 6, L"%d", std::rand() + RAND_MAX);

		auto idNumberString = ref new String(pszIdNumber);

		if (FAILED(hr)) throw ref new FailureException();

		if (ToastRadio->IsChecked->Value)
		{
			ScheduleToast(updateString, dueTime, idNumberString);
		}
		else
		{
			ScheduleTile(updateString, dueTime, idNumberString);
		}
	}
	catch (...)
	{
		rootPage->NotifyUser("You must input a valid time in seconds.", NotifyType::ErrorMessage);
	}
}

void ScenarioInput1::ScheduleToast(String^ updateString, DateTime dueTime, String^ idNumberString)
{
	auto time = Windows::Globalization::DateTimeFormatting::DateTimeFormatter::LongTime;

	// Scheduled toasts use the same toast templates as all other kinds of toasts.
	IToastText02^ toastContent = ToastContentFactory::CreateToastText02();

	toastContent->TextHeading->Text = updateString;
	toastContent->TextBodyWrap->Text = "Received: " + time->Format(dueTime);

	ScheduledToastNotification^ toast;
	if (RepeatBox->IsChecked->Value)
	{
		TimeSpan interval;
		interval.Duration = 60 * 10000000;
		toast = ref new ScheduledToastNotification(toastContent->GetXml(), dueTime, interval, 5);

		// You can specify an ID so that you can manage toasts later.
		// Make sure the ID is 15 characters or less.
		toast->Id = "Repeat" + idNumberString;
	}
	else
	{
		toast = ref new ScheduledToastNotification(toastContent->GetXml(), dueTime);
		toast->Id = "Toast" + idNumberString;
	}

	ToastNotificationManager::CreateToastNotifier()->AddToSchedule(toast);
	rootPage->NotifyUser("Scheduled a toast with ID: " + toast->Id, NotifyType::StatusMessage);
}

void ScenarioInput1::ScheduleTile(String^ updateString, DateTime dueTime, String^ idNumberString)
{
	auto time = Windows::Globalization::DateTimeFormatting::DateTimeFormatter::LongTime;

	// Scheduled tiles use the same tile templates as all other kinds of tiles.
	ITileWideText09^ tileContent = TileContentFactory::CreateTileWideText09();
	tileContent->TextHeading->Text = updateString;
	tileContent->TextBodyWrap->Text = "Received: " + time->Format(dueTime);

	// Set up square tile content
	ITileSquareText04^ squareContent = TileContentFactory::CreateTileSquareText04();
	squareContent->TextBodyWrap->Text = updateString;

	tileContent->SquareContent = squareContent;

	// Create the notification object
	auto futureTile = ref new ScheduledTileNotification(tileContent->GetXml(), dueTime);
	futureTile->Id = "Tile" + idNumberString;

	// Add to schedule
	// You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
	// See "Tiles" sample for more details
	TileUpdateManager::CreateTileUpdaterForApplication()->AddToSchedule(futureTile);
	rootPage->NotifyUser("Scheduled a tile with ID: " + futureTile->Id, NotifyType::StatusMessage);
}

void ScenarioInput1::ScheduleToastWithStringManipulation(String^ updateString, DateTime dueTime, String^ idNumberString)
{
	auto time = Windows::Globalization::DateTimeFormatting::DateTimeFormatter::LongTime;

	// Scheduled toasts use the same toast templates as all other kinds of toasts.
	auto toastXmlString = "<toast>"
		+ "<visual>"
		+ "<binding template='ToastText02'>"
		+ "<text id='2'>" + updateString + "</text>"
		+ "<text id='1'>" + "Received: " + time->Format(dueTime) + "</text>"
		+ "</binding>"
		+ "</visual>"
		+ "</toast>";

	auto toastDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
	try
	{
		toastDOM->LoadXml(toastXmlString);

		ScheduledToastNotification^ toast;
		if (RepeatBox->IsChecked->Value)
		{
			TimeSpan interval;
			interval.Duration = 60 * 10000000;
			toast = ref new ScheduledToastNotification(toastDOM, dueTime, interval, 5);

			// You can specify an ID so that you can manage toasts later.
			// Make sure the ID is 15 characters or less.
			toast->Id = "Repeat" + idNumberString;
		}
		else
		{
			toast = ref new ScheduledToastNotification(toastDOM, dueTime);
			toast->Id = "Toast" + idNumberString;
		}

		ToastNotificationManager::CreateToastNotifier()->AddToSchedule(toast);
		rootPage->NotifyUser("Scheduled a toast with ID: " + toast->Id, NotifyType::StatusMessage);
	}
	catch (Exception^)
	{
		rootPage->NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType::ErrorMessage);
	}
}

void ScenarioInput1::ScheduleTileWithStringManipulation(String^ updateString, DateTime dueTime, String^ idNumberString)
{
	auto time = Windows::Globalization::DateTimeFormatting::DateTimeFormatter::LongTime;

	auto tileXmlString = "<tile>"
		+ "<visual>"
		+ "<binding template='TileWideText09'>"
		+ "<text id='1'>" + updateString + "</text>"
		+ "<text id='2'>" + "Received: " + time->Format(dueTime) + "</text>"
		+ "</binding>"
		+ "<binding template='TileSquareText04'>"
		+ "<text id='1'>" + updateString + "</text>"
		+ "</binding>"
		+ "</visual>"
		+ "</tile>";

	auto tileDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
	try
	{
		tileDOM->LoadXml(tileXmlString);

		// Create the notification object
		auto futureTile = ref new ScheduledTileNotification(tileDOM, dueTime);
		futureTile->Id = "Tile" + idNumberString;

		// Add to schedule
		// You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
		// See "Tiles" sample for more details
		TileUpdateManager::CreateTileUpdaterForApplication()->AddToSchedule(futureTile);
		rootPage->NotifyUser("Scheduled a tile with ID: " + futureTile->Id, NotifyType::StatusMessage);
	}
	catch (Exception^)
	{
		rootPage->NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType::ErrorMessage);
	}
}

void ScenarioInput1::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

