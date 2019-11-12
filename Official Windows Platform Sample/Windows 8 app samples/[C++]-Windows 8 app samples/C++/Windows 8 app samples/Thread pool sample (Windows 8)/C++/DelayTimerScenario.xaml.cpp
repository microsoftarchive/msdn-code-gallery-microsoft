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
// DelayTimerScenario.xaml.cpp
// Implementation of the DelayTimerScenario class
//

#include "pch.h"
#include "DelayTimerScenario.xaml.h"

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
	DelayMs->SelectedIndex = SDKSample::ThreadPool::DelayTimerSelectedIndex;
    UpdateUI(SDKSample::ThreadPool::DelayTimerStatus);
}

void DelayTimerScenario::OnNavigatedFrom(NavigationEventArgs^ e)
{
	SDKSample::ThreadPool::DelayTimerSelectedIndex = DelayMs->SelectedIndex;
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

    auto time = safe_cast<Platform::String^>(DelayMs->SelectionBoxItem);
    DelayTimerMilliseconds = _wtol(time->Data());

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
    DelayTimerInfo->Text = "Timer delay = " + SDKSample::ThreadPool::DelayTimerMilliseconds.ToString() + " ms.";
    DelayTimerStatus->Text = ThreadPoolSample::GetStatusString(status);

    auto createButtonEnabled = (status == SDKSample::ThreadPool::Status::Started);
    CreateDelayTimerButton->IsEnabled = !createButtonEnabled;
    CancelDelayTimerButton->IsEnabled = createButtonEnabled;
}
