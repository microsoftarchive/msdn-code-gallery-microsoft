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
// WorkItemScenario.xaml.cpp
// Implementation of the WorkItemScenario class
//

#include "pch.h"
#include "WorkItemScenario.xaml.h"

using namespace SDKSample::ThreadPool;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

WorkItemScenario::WorkItemScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void  SDKSample::ThreadPool::WorkItemScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
    SDKSample::ThreadPool::WorkScenario = this;
	Priority->SelectedIndex = SDKSample::ThreadPool::WorkItemSelectedIndex;
    UpdateUI(SDKSample::ThreadPool::WorkItemStatus);
}

void  SDKSample::ThreadPool::WorkItemScenario::OnNavigatedFrom(NavigationEventArgs^ e)
{
	SDKSample::ThreadPool::WorkItemSelectedIndex = Priority->SelectedIndex;
}

void  SDKSample::ThreadPool::WorkItemScenario::CreateThreadPoolWorkItem(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto workItemDelegate = [this](IAsyncAction^ workItem)
    {
        int count = 0;
        int maxcount = 10000000;
        int oldProgress = 0;

        while (count < maxcount)
        {
            auto status = workItem->Status;
            if (status == AsyncStatus::Canceled)
            {
                break;
            }

            //
            // Simulate doing work.
            //

            count++;

            int currentProgress = (int)(((double)count / (double)maxcount) * 100);
            if (currentProgress > oldProgress)
            {
                auto uiDelegate = [this, currentProgress]()
                {
                    SDKSample::ThreadPool::WorkScenario->UpdateWorkItemProgressUI(currentProgress);
                };
                auto handler = ref new Windows::UI::Core::DispatchedHandler(uiDelegate);
                Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, handler);
            }

            oldProgress = currentProgress;
        }
    };

    auto completionDelegate = [this](IAsyncAction^ action, AsyncStatus status)
    {
        switch (action->Status)
        {
        case AsyncStatus::Started:
            SDKSample::ThreadPool::WorkScenario->UpdateUI(SDKSample::ThreadPool::Status::Started);
            break;
        case AsyncStatus::Completed:
            SDKSample::ThreadPool::WorkScenario->UpdateUI(SDKSample::ThreadPool::Status::Completed);
            break;
        case AsyncStatus::Canceled:
            SDKSample::ThreadPool::WorkScenario->UpdateUI(SDKSample::ThreadPool::Status::Canceled);
            break;
        case AsyncStatus::Error:
            //UpdateUI->Text = "Error";
            break;
        }
    };

    auto workItemHandler = ref new Windows::System::Threading::WorkItemHandler(workItemDelegate);
    auto completionHandler = ref new Windows::Foundation::AsyncActionCompletedHandler(completionDelegate, Platform::CallbackContext::Same);
    auto priority = safe_cast<String^>(Priority->SelectionBoxItem);

    if (priority == "Low")
    {
        SDKSample::ThreadPool::WorkItemInfo = "Work item priority = Low";
        SDKSample::ThreadPool::WorkPriority = Windows::System::Threading::WorkItemPriority::Low;
    }
    else if (priority == "Normal")
    {
        SDKSample::ThreadPool::WorkItemInfo = "Work item priority = Normal";
        SDKSample::ThreadPool::WorkPriority = Windows::System::Threading::WorkItemPriority::Normal;
    }
    else if (priority == "High")
    {
        SDKSample::ThreadPool::WorkItemInfo = "Work item priority = High";
        SDKSample::ThreadPool::WorkPriority = Windows::System::Threading::WorkItemPriority::High;
    }

    SDKSample::ThreadPool::WorkItem = Windows::System::Threading::ThreadPool::RunAsync(workItemHandler, SDKSample::ThreadPool::WorkPriority);
    SDKSample::ThreadPool::WorkItem->Completed = completionHandler;
    UpdateUI(SDKSample::ThreadPool::Status::Started);
}

void  SDKSample::ThreadPool::WorkItemScenario::CancelThreadPoolWorkItem(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (SDKSample::ThreadPool::WorkItem != nullptr)
    {
        SDKSample::ThreadPool::WorkItem->Cancel();
    }
}

void  SDKSample::ThreadPool::WorkItemScenario::UpdateUI(Status status)
{
    SDKSample::ThreadPool::WorkItemStatus = status;
    WorkItemStatus->Text = ThreadPoolSample::GetStatusString(status);
    WorkItemInfo->Text = SDKSample::ThreadPool::WorkItemInfo;

    auto createButtonEnabled = (status == SDKSample::ThreadPool::Status::Started);
    CreateThreadPoolWorkItemButton->IsEnabled = !createButtonEnabled;
    CancelThreadPoolWorkItemButton->IsEnabled = createButtonEnabled;
}

void  SDKSample::ThreadPool::WorkItemScenario::UpdateWorkItemProgressUI(int percentComplete)
{
    WorkItemStatus->Text = "Progress: " + percentComplete + "%";
}
