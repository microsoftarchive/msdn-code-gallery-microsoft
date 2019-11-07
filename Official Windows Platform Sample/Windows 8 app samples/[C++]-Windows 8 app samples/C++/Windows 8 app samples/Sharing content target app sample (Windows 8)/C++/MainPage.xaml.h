// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// MainPage.xaml.h
// Declaration of the MainPage.xaml class.
//

#pragma once

#include "pch.h"
#include <agile.h>
#include "MainPage.g.h"
#include <agile.h>

#define DATA_FORMAT_NAME "http://schema.org/Book"

namespace ShareTarget
{
    public enum class NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainPage sealed
    {
    public:
        MainPage();

        void NotifyUser(Platform::String^ strmessage, NotifyType type);
        void NotifyUserBackgroundThread(Platform::String^ message, NotifyType type);
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~MainPage();
        Platform::Agile<Windows::ApplicationModel::DataTransfer::ShareTarget::ShareOperation> shareOperation;
        Platform::String^ sharedDataTitle;
        Platform::String^ sharedDataDescription;
        Platform::String^ shareQuickLinkId;
        Platform::String^ sharedText;
        Windows::Foundation::Uri^ sharedUri;
        Windows::Foundation::Collections::IVectorView<Windows::Storage::IStorageItem^>^ sharedStorageItems;
        Platform::String^ sharedCustomData;
        Platform::String^ sharedHtmlFormat;
        Windows::Foundation::Collections::IMapView<Platform::String^, Windows::Storage::Streams::RandomAccessStreamReference^>^ sharedResourceMap;
        Windows::Storage::Streams::IRandomAccessStreamReference^ sharedBitmapStreamRef;
        Windows::Storage::Streams::IRandomAccessStreamReference^ sharedThumbnailStreamRef;

        void QuickLinkSectionLabel_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
        void AddQuickLink_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void AddQuickLink_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ReportCompleted_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LongRunningShareLabel_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
        void ExpandLongRunningShareSection_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ExpandLongRunningShareSection_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ReportStarted_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ReportDataRetrieved_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ReportSubmittedBackgroundTask_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ReportErrorButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void AddContentValue(Platform::String^ title);
        void AddContentValue(Platform::String^ title, Platform::String^ description);
    };
 }