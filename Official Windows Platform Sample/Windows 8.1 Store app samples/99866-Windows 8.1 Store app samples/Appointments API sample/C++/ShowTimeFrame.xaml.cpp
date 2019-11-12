//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ShowTimeFrame.xaml.cpp
// Implementation of the ShowTimeFrame class
//

#include "pch.h"
#include "ShowTimeFrame.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Appointments;

using namespace concurrency;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

ShowTimeFrame::ShowTimeFrame()
{
    InitializeComponent();
}

void Appointments::ShowTimeFrame::Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Show the appointment provider app at the present time
    Windows::Globalization::Calendar^ cal = ref new Windows::Globalization::Calendar();
    DateTime timeToShow = cal->GetDateTime();

    Windows::Foundation::TimeSpan duration;
    duration.Duration = (60LL * 60 * 1000000000) / 100; // 1 hour in 100-nanosecond units

    create_task(Windows::ApplicationModel::Appointments::AppointmentManager::ShowTimeFrameAsync(timeToShow, duration)).then([=]()
    {
        ResultTextBlock->Text = L"The default appointments provider should have appeared on screen.";
    });
}
