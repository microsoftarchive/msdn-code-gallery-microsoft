//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1-DirectContentUris.xaml.cpp
// Implementation of the DirectContentUriScenario class
//

#include "pch.h"
#include <ppltasks.h>
#include "S1-DirectContentUris.xaml.h"
#include "MainPage.xaml.h"
#include <string>
using namespace SDKSample;
using namespace SDKSample::SDKTemplate;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

DirectContentUriScenario::DirectContentUriScenario()
{
    InitializeComponent();
    UpdateUriTable();
}

void SDKTemplate::DirectContentUriScenario::AddDirectUris_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser("", NotifyType::StatusMessage);

    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        Platform::String ^uriString = DirectContentUri->Text;
        if (uriString != "")
        {
            Windows::Foundation::Uri ^uri;
            try
            {
                uri = ref new Windows::Foundation::Uri(this->DirectContentUri->Text);
            }
            catch (Platform::InvalidArgumentException ^)
            {
                MainPage::Current->NotifyUser("A URI must be provided that has the form scheme://address", NotifyType::ErrorMessage);
            }

            if (uri)
            {
                Windows::Networking::BackgroundTransfer::ContentPrefetcher::ContentUris->Append(uri);
                UpdateUriTable();
                this->DirectContentUri->Text = "";
            }
        }
    }
}

void SDKTemplate::DirectContentUriScenario::ClearDirectUris_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser("", NotifyType::StatusMessage);

    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        Windows::Networking::BackgroundTransfer::ContentPrefetcher::ContentUris->Clear();
        UpdateUriTable();
    }
}

void SDKTemplate::DirectContentUriScenario::UpdateUriTable()
{
    DirectContentUris->Children->Clear();
    UriCacheStatus->Children->Clear();

    for (Windows::Foundation::Uri ^uri : Windows::Networking::BackgroundTransfer::ContentPrefetcher::ContentUris)
    {
        TextBlock ^uriTextBlock = ref new TextBlock();
        uriTextBlock->Text = uri->AbsoluteUri;
        DirectContentUris->Children->Append(uriTextBlock);

        TextBlock ^cacheStatusTextBlock = ref new TextBlock();
        UriCacheStatus->Children->Append(cacheStatusTextBlock);

        UpdateIfUriIsInCache(uri, cacheStatusTextBlock);
    }
}


void SDKTemplate::DirectContentUriScenario::UpdateIfUriIsInCache(Windows::Foundation::Uri ^uri, TextBlock ^cacheStatusTextBlock)
{
    auto filter = ref new Windows::Web::Http::Filters::HttpBaseProtocolFilter();
    filter->CacheControl->ReadBehavior = Windows::Web::Http::Filters::HttpCacheReadBehavior::OnlyFromCache;

    auto httpClient = ref new Windows::Web::Http::HttpClient(filter);
    auto request = ref new Windows::Web::Http::HttpRequestMessage(Windows::Web::Http::HttpMethod::Get, uri);

    try
    {
        auto asyncRequest = concurrency::create_task(httpClient->SendRequestAsync(request));
        asyncRequest.then([=](concurrency::task<Windows::Web::Http::HttpResponseMessage^> t)
        {
            try
            {
                t.get();
                cacheStatusTextBlock->Text = "Yes";
                cacheStatusTextBlock->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Green);
            }
            catch (...)
            {
                cacheStatusTextBlock->Text = "No";
                cacheStatusTextBlock->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Red);
            }
        });
    }
    catch (Platform::COMException ^)
    {
        cacheStatusTextBlock->Text = "No";
        cacheStatusTextBlock->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Red);
    }
}