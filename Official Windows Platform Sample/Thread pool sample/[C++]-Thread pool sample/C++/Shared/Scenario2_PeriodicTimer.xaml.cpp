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
// Scenario2_PeriodicTimer.xaml.cpp
// Implementation of the PeriodicTimerScenario class
//

#include "pch.h"
#include "Scenario2_PeriodicTimer.xaml.h"

using namespace SDKSample::ThreadPool;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

PeriodicTimerScenario::PeriodicTimerScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void SDKSample::ThreadPool::PeriodicTimerScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    SDKSample::ThreadPool::PeriodicScenario = this;
    PeriodMs->Text = PeriodicTimerMilliseconds.ToString();
    UpdateUI(SDKSample::ThreadPool::PeriodicTimerStatus);
}

void SDKSample::ThreadPool::PeriodicTimerScenario::OnNavigatedFrom(NavigationEventArgs^ e)
{
    ThreadPoolSample::ParseTimeValue(PeriodMs->Text, &PeriodicTimerMilliseconds);
}

void SDKSample::ThreadPool::PeriodicTimerScenario::CreatePeriodicTimer(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto timerDelegate = [this](Windows::System::Threading::ThreadPoolTimer^ timer)
    {
        auto uiDelegate = [this]()
        {
            SDKSample::ThreadPool::PeriodicTimerCount++;
            SDKSample::ThreadPool::PeriodicScenario->UpdateUI(Status::Completed);
        };

        Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
            ref new Windows::UI::Core::DispatchedHandler(uiDelegate));
    };

    ThreadPoolSample::ParseTimeValue(PeriodMs->Text, &PeriodicTimerMilliseconds);

    Windows::Foundation::TimeSpan Period = ThreadPoolSample::MillisecondsToTimeSpan(PeriodicTimerMilliseconds);
    UpdateUI(Status::Started);

    PeriodicTimer = Windows::System::Threading::ThreadPoolTimer::CreatePeriodicTimer(ref new Windows::System::Threading::TimerElapsedHandler(timerDelegate), Period);
}

void SDKSample::ThreadPool::PeriodicTimerScenario::CancelPeriodicTimer(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (PeriodicTimer != nullptr)
    {
        PeriodicTimer->Cancel();
        PeriodicTimerScenario::UpdateUI(Status::Canceled);
    }
}

void SDKSample::ThreadPool::PeriodicTimerScenario::UpdateUI(Status status)
{
    SDKSample::ThreadPool::PeriodicTimerStatus = status;

    switch (status)
    {
    case Status::Completed:
        PeriodicTimerStatus->Text = "Completion count: " + SDKSample::ThreadPool::PeriodicTimerCount.ToString();
        break;
    default:
        PeriodicTimerStatus->Text = ThreadPoolSample::GetStatusString(status);
        break;
    }

    PeriodicTimerInfo->Text = "Timer Period = " + PeriodicTimerMilliseconds.ToString() + " ms.";

    if (((status != Status::Started) && (status != Status::Completed)) ||
        ((status == Status::Completed) && (PeriodicTimerMilliseconds == 0)))
    {
        CreatePeriodicTimerButton->IsEnabled = true;
        CreatePeriodicTimerButton->Focus(Windows::UI::Xaml::FocusState::Keyboard);
        CancelPeriodicTimerButton->IsEnabled = false;
    }
    else
    {
        CancelPeriodicTimerButton->IsEnabled = true;
        CancelPeriodicTimerButton->Focus(Windows::UI::Xaml::FocusState::Keyboard);
        CreatePeriodicTimerButton->IsEnabled = false;
    }
}

void SDKSample::ThreadPool::PeriodicTimerScenario::PeriodMs_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e)
{
    ThreadPoolSample::ParseTimeValue(PeriodMs->Text, &PeriodicTimerMilliseconds);
    PeriodMs->Text = PeriodicTimerMilliseconds.ToString();
}
