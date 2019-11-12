//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Toast.xaml.cpp
// Implementation of the Toast class
//

#include "pch.h"
#include "Toast.xaml.h"
#include "MainPage.xaml.h"
#include <string>
#include <sstream>
#include <stdlib.h>

using namespace SDKSample;
using namespace SDKSample::AlarmNotifications;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Toast::Toast()
{
    InitializeComponent();
}

void AlarmNotifications::Toast::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    __super::OnNavigatedTo(e);
    try
    {
        Windows::ApplicationModel::Background::AlarmApplicationManager::RequestAccessAsync();
    }
    catch (...)
    {
        // RequestAccessAsync may throw an exception if the app is not currently in the foreground
    }
}

void AlarmNotifications::Toast::SendToast_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        // Get some data from the button the user clicked on.
        std::wstring toastTemplate(b->Name->Data());
        std::wstring alarmName(L"");

        if (toastTemplate.find(L"Custom") != std::string::npos)
        {
            alarmName = L"Wake up time with custom snooze!";
        }
        else
        {
            alarmName = L"Wake up time with default snooze!";
        }

        // Create the toast content by direct string manipulation.
        // See the Toasts SDK Sample for other ways of generating toasts.
        std::wostringstream xmlPayloadBuilder;
        xmlPayloadBuilder << 
            L"<toast duration=\"long\">\n"
         << L" <audio loop=\"true\" src=\"ms-winsoundevent:Notification.Looping.Alarm2\"/>\n"
         << L"  <visual>\n"
         << L"   <binding template=\"ToastText02\">\n"
         << L"    <text id=\"1\">Alarms Notifications SDK Sample App</text>\n"
         << L"    <text id=\"2\">" << alarmName << L"</text>\n"
         << L"   </binding>\n"
         << L"  </visual>\n"
         << L" <commands scenario=\"alarm\">\n"
         << L"  <command id=\"snooze\"/>\n"
         << L"  <command id=\"dismiss\"/>\n"
         << L" </commands>\n"
         << L"</toast>";

        std::wstring xmlPayload = xmlPayloadBuilder.str();

        // Display the generated XML for demonstration purposes.
        MainPage::Current->NotifyUser(ref new Platform::String(xmlPayload.c_str()), NotifyType::StatusMessage);

        // Create an XML document from the XML.
        auto toastDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
        toastDOM->LoadXml(ref new Platform::String(xmlPayload.c_str()));

        // Prepare to raise the toast.
        auto toastNotifier = Windows::UI::Notifications::ToastNotificationManager::CreateToastNotifier();

        // Set the toast due time per user selection
        int delay = _wtoi(dynamic_cast<FrameworkElement^>(ToastDelay->SelectedItem)->Name->Data() + 1);
        FILETIME now;
        GetSystemTimeAsFileTime(&now);
        ULARGE_INTEGER universalTime;
        universalTime.LowPart = now.dwLowDateTime;
        universalTime.HighPart = now.dwHighDateTime;
        universalTime.QuadPart += delay * 10000000L;
            
        Windows::Foundation::DateTime dueTime;
        dueTime.UniversalTime = universalTime.QuadPart;

        if (toastTemplate.find(L"Custom") != std::string::npos) 
        {
            // Schedule the toast with custom snooze.       
            Windows::Foundation::TimeSpan snoozeInterval;
            snoozeInterval.Duration = _wtoll(CustomSnoozeTime->Text->Data()) * 60 * 10000000;
            auto scheduledToast = ref new Windows::UI::Notifications::ScheduledToastNotification(toastDOM, dueTime, snoozeInterval, 0);
            toastNotifier->AddToSchedule(scheduledToast);
        }
        else
        {
            // Schedule the toast with default snooze
            auto scheduledToast = ref new Windows::UI::Notifications::ScheduledToastNotification(toastDOM, dueTime);
            toastNotifier->AddToSchedule(scheduledToast);
        }                
    }
}

