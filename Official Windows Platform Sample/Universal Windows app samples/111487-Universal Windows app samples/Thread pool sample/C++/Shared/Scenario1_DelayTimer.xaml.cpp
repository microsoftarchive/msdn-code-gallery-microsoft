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
// Scenario1_DelayTimer.xaml.cpp
// Implementation of the DelayTimerScenario class
//

#include "pch.h"
#include "Scenario1_DelayTimer.xaml.h"

using namespace SDKSample::ThreadPool;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

DelayTimerScenario::DelayTimerScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DelayTimerScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    SDKSample::ThreadPool::DelayScenario = this;
    DelayMs->Text = DelayTimerMilliseconds.ToString();
    UpdateUI(SDKSample::ThreadPool::DelayTimerStatus);
}

void DelayTimerScenario::OnNavigatedFrom(NavigationEventArgs^ e)
{
    ThreadPoolSample::ParseTimeValue(DelayMs->Text, &DelayTimerMilliseconds);
}

void SDKSample::ThreadPool::DelayTimerScenario::CreateDelayTimer(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto timerDelegate = [this](Windows::System::Threading::ThreadPoolTimer^ timer)
    {
        auto uiDelegate = [this]()
        {
            SDKSample::ThreadPool::DelayScenario->UpdateUI(SDKSample::ThreadPool::Status::Completed);
        };

        Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
                             ref new Windows::UI::Core::DispatchedHandler(uiDelegate));
    };

    ThreadPoolSample::ParseTimeValue(DelayMs->Text, &DelayTimerMilliseconds);

    Windows::Foundation::TimeSpan delay = SDKSample::ThreadPool::ThreadPoolSample::MillisecondsToTimeSpan(DelayTimerMilliseconds);
    UpdateUI(SDKSample::ThreadPool::Status::Started);

    SDKSample::ThreadPool::DelayTimer = Windows::System::Threading::ThreadPoolTimer::CreateTimer(ref new Windows::System::Threading::TimerElapsedHandler(timerDelegate), delay);
}

void SDKSample::ThreadPool::DelayTimerScenario::CancelDelayTimer(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (SDKSample::ThreadPool::DelayTimer != nullptr)
    {
        SDKSample::ThreadPool::DelayTimer->Cancel();
        SDKSample::ThreadPool::DelayTimerScenario::UpdateUI(SDKSample::ThreadPool::Status::Canceled);
    }
}

void SDKSample::ThreadPool::DelayTimerScenario::UpdateUI(SDKSample::ThreadPool::Status status)
{
    SDKSample::ThreadPool::DelayTimerStatus = status;
    DelayTimerInfo->Text = "Timer delay = " + DelayTimerMilliseconds.ToString() + " ms.";
    DelayTimerStatus->Text = ThreadPoolSample::GetStatusString(status);

    if (status == SDKSample::ThreadPool::Status::Started)
    {
        CancelDelayTimerButton->IsEnabled = true;
        CancelDelayTimerButton->Focus(Windows::UI::Xaml::FocusState::Keyboard);
        CreateDelayTimerButton->IsEnabled = false;
    }
    else
    {
        CreateDelayTimerButton->IsEnabled = true;
        CreateDelayTimerButton->Focus(Windows::UI::Xaml::FocusState::Keyboard);
        CancelDelayTimerButton->IsEnabled = false;
    }
}

void SDKSample::ThreadPool::DelayTimerScenario::DelayMs_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e)
{
    ThreadPoolSample::ParseTimeValue(DelayMs->Text, &DelayTimerMilliseconds);
    DelayMs->Text = DelayTimerMilliseconds.ToString();
}
