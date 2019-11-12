//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3-LastPrefetchTime.xaml.cpp
// Implementation of the LastPrefetchTimeScenario class
//

#include "pch.h"
#include "S3-LastPrefetchTime.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::SDKTemplate;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

LastPrefetchTimeScenario::LastPrefetchTimeScenario()
{
    InitializeComponent();
}

void SDKTemplate::LastPrefetchTimeScenario::GetLastPrefetchTime_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        Windows::Globalization::DateTimeFormatting::DateTimeFormatter ^dateFormatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("longdate longtime");

        Platform::IBox<Windows::Foundation::DateTime> ^lastPrefetchTime = Windows::Networking::BackgroundTransfer::ContentPrefetcher::LastSuccessfulPrefetchTime;
        if (lastPrefetchTime != nullptr)
        {
            LastPrefetchTime->Text = "The last successful prefetch time was " + dateFormatter->Format(lastPrefetchTime->Value);
        }
        else
        {
            LastPrefetchTime->Text = "There have been no successful prefetches yet";
        }
    }
}
