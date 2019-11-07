//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Others.xaml.cpp
// Implementation of the Others class
//

#include "pch.h"
#include "Others.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Clipboard;

using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::DataTransfer;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;

bool Others::registerContentChanged = false;

Others::Others()
{
    InitializeComponent();
}

void Others::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
    RegisterClipboardContentChanged->IsChecked = registerContentChanged;
}

void Others::ClearOutput()
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);
    OutputText->Text = "";
}

void Others::DisplayFormats()
{
    ClearOutput();
    OutputText->Text = rootPage->BuildClipboardFormatsOutputString();
}

#pragma region Others handlers

void Others::ShowFormatButton_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    DisplayFormats();
}

void Others::EmptyClipboardButton_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    try
    {
        DataTransfer::Clipboard::Clear();
        OutputText->Text = "Clipboard has been emptied.";
    }
    catch (Exception^ ex)
    {
        // Emptying Clipboard can potentially fail - for example, if another application is holding Clipboard open
        rootPage->NotifyUserBackgroundThread("Error emptying the clipboard - " + ex->Message, NotifyType::ErrorMessage);
    }
}

void Others::ClearOutputButton_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    ClearOutput();
}

void Others::RegisterClipboardContentChanged_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    if (registerContentChanged != RegisterClipboardContentChanged->IsChecked->Value)
    {
        ClearOutput();
        registerContentChanged = RegisterClipboardContentChanged->IsChecked->Value;

        // In this sample, we register for Clipboard update notifications on the MainPage so that we can
        // navigate between scenarios and still receive updates. The sample also registers for window activated 
        // notifications since the app needs to be in the foreground to access the clipboard. Once we receive
        // a clipboard update notification, we display the new content (if the app is in the foreground). If the
        // sample is not in the foreground, we defer displaying it until it is.
        rootPage->EnableClipboardContentChangedNotifications(registerContentChanged);
        if (registerContentChanged)
        {
            OutputText->Text = "Successfully registered for clipboard update notification.";
        }
        else
        {
            OutputText->Text = "Successfully un-registered for clipboard update notification.";
        }
    }
}

#pragma endregion