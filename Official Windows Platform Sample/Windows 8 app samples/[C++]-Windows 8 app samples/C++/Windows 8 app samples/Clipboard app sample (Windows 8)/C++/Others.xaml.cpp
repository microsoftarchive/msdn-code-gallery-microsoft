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
// Others.xaml.cpp
// Implementation of the Others class
//

#include "pch.h"
#include "Others.xaml.h"

using namespace Clipboard;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::DataTransfer;

bool Others::registerContentChanged = false;
bool Others::needToPrintClipboardFormat = false;
bool Others::windowVisible = false;

EventRegistrationToken Others::contentChangedToken;
EventRegistrationToken Others::visibilityChangedToken;

Others::Others()
{
    InitializeComponent();
}

void Others::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
    windowVisible = true;
    RegisterClipboardContentChanged->IsChecked = registerContentChanged;

    if (needToPrintClipboardFormat)
    {
        HandleClipboardChanged();
    }
}

void Others::OnNavigatedFrom(NavigationEventArgs^ e)
{
    windowVisible = false;
}

void Others::ClearOutput()
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);
    OutputText->Text = "";
    DisplayFormatOutputText->Text = "";
}

void Others::DisplayFormats(DataPackageView^ dataPackageView)
{
    if ((dataPackageView != nullptr) && (dataPackageView->AvailableFormats->Size > 0))
    {
        OutputText->Text = "";
        DisplayFormatOutputText->Text = "Clipboard contains following data formats:\n";
        auto iterator = dataPackageView->AvailableFormats->First();
        while (iterator->HasCurrent)
        {
            DisplayFormatOutputText->Text += " * " + iterator->Current + "\n";
            iterator->MoveNext();
        }
    }
    else
    {
        OutputText->Text = "The clipboard is empty";
    }

    needToPrintClipboardFormat = false;
}

void Others::HandleClipboardChanged()
{
    if (windowVisible)
    {
        rootPage->NotifyUserBackgroundThread("Clipboard content has changed!", NotifyType::StatusMessage);
        DisplayFormats(DataTransfer::Clipboard::GetContent());
    }
    else
    {
        // Background applications can't access clipboard
        // Deferring processing of update notification until the application returns to foreground
        needToPrintClipboardFormat = true;
    }
}

void Others::OnVisibilityChanged(Object^ sender, VisibilityChangedEventArgs^ e)
{
    windowVisible = e->Visible;
    if (windowVisible && needToPrintClipboardFormat)
    {
        HandleClipboardChanged();
    }
}

void Others::OnClipboardChanged(Platform::Object^ sender, Object^ e)
{
    HandleClipboardChanged();
}

#pragma region Others handlers

void Others::ShowFormatButton_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    DisplayFormats(DataTransfer::Clipboard::GetContent());
}

void Others::EmptyClipboardButton_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    DisplayFormatOutputText->Text = "";
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
        if (registerContentChanged)
        {
            contentChangedToken = DataTransfer::Clipboard::ContentChanged += ref new EventHandler<Object^>(this, &Others::OnClipboardChanged);
            visibilityChangedToken = Window::Current->VisibilityChanged += ref new WindowVisibilityChangedEventHandler(this, &Others::OnVisibilityChanged);
            OutputText->Text = "Successfully registered for clipboard update notification.";
        }
        else
        {
            DataTransfer::Clipboard::ContentChanged -= contentChangedToken;
            Window::Current->VisibilityChanged -= visibilityChangedToken;
            OutputText->Text = "Successfully un-registered for clipboard update notification.";
        }
    }
}

#pragma endregion
