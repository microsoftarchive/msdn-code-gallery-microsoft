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
using namespace SDKSample::LockScreenCall;

using namespace Platform;
using namespace Concurrency;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Toast::Toast()
{
    InitializeComponent();
}

void Toast::OnNavigatedTo(NavigationEventArgs^ e)
{
    __super::OnNavigatedTo(e);

    // Failure to gain background access is not fatal. Catch and ignore
    // all failure points.
    try
    {
        create_task(BackgroundExecutionManager::RequestAccessAsync()).then([](task<BackgroundAccessStatus> t)
        {
            try
            {
                t.get();
            }
            catch (Exception^ /* e */)
            {
            }
        });
    }
    catch (Exception^ /* e */)
    {
    }
}

void Toast::Toast_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        // Get some data from the button the user clicked on.
        std::wstring toastTemplate(b->Name->Data());
        std::wstring videoButtonXml(L"");
        std::wstring voiceButtonXml(L"");
        std::wstring infoString;
        std::wstring callerType(dynamic_cast<FrameworkElement^>(CallerType->SelectedItem)->Name->Data());
        std::wstring callDuration(dynamic_cast<FrameworkElement^>(CallDuration->SelectedItem)->Name->Data() + 1);

        if (toastTemplate.find(L"Video") != std::wstring::npos)
        {
            videoButtonXml = L"  <command id=\"video\" arguments=\"Video " + callerType + L" " + callDuration + L"\"/>\n";
            infoString = L"Incoming video call";
        }
        else
        {
            infoString = L"Incoming voice call";
        }
        if (toastTemplate.find(L"Voice") != std::wstring::npos)
        {
            voiceButtonXml = L"  <command id=\"voice\" arguments=\"Voice " + callerType + L" " + callDuration + L"\"/>\n";
        }

        // Create the toast content by direct string manipulation.
        // See the Toasts SDK Sample for other ways of generating toasts.
        std::wostringstream xmlPayloadBuilder;
        xmlPayloadBuilder << 
            L"<toast duration=\"long\">\n"
         << L" <audio loop=\"true\" src=\"ms-winsoundevent:Notification.Looping.Call3\"/>\n"
         << L"  <visual>\n"
         << L"   <binding template=\"ToastText02\">\n"
         << L"    <text id=\"1\">" << callerType << L"</text>\n"
         << L"    <text id=\"2\">" << infoString << L"</text>\n"
         << L"   </binding>\n"
         << L"  </visual>\n"
         << L" <commands scenario=\"incomingCall\">\n"
         << videoButtonXml
         << voiceButtonXml
         << L"  <command id=\"decline\"/>\n"
         << L" </commands>\n"
         << L"</toast>";

        std::wstring xmlPayload = xmlPayloadBuilder.str();

        // Display the generated XML for demonstration purposes.
        MainPage::Current->NotifyUser(ref new String(xmlPayload.c_str()), NotifyType::StatusMessage);

        // Create an XML document from the XML.
        auto toastDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
        toastDOM->LoadXml(ref new String(xmlPayload.c_str()));

        // Prepare to raise the toast.
        auto toastNotifier = ToastNotificationManager::CreateToastNotifier();

        int delay = _wtoi(dynamic_cast<FrameworkElement^>(ToastDelay->SelectedItem)->Name->Data() + 1);
        if (delay > 0) {
            // Schedule the toast in the future.
            FILETIME now;
            GetSystemTimeAsFileTime(&now);
            ULARGE_INTEGER universalTime;
            universalTime.LowPart = now.dwLowDateTime;
            universalTime.HighPart = now.dwHighDateTime;
            universalTime.QuadPart += delay * 10000000L;
            
            Windows::Foundation::DateTime dueTime;
            dueTime.UniversalTime = universalTime.QuadPart;

            auto scheduledToast = ref new ScheduledToastNotification(toastDOM, dueTime);
            toastNotifier->AddToSchedule(scheduledToast);
        } else {
            // Raise the toast immediately.
            auto toast = ref new ToastNotification(toastDOM);
            toastNotifier->Show(toast);
        }                
    }
}
