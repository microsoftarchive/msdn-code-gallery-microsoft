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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "SampleDataSource.h"
#include "Scenario2ItemViewer.xaml.h"

using namespace SDKSample::ListViewInteraction;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media;
using namespace SDKSample::ListViewInteractionSampleDataSource;

Scenario2::Scenario2()
{
    InitializeComponent();
    messageData = ref new MessageData();
    ItemListView->ItemsSource = messageData->Items;
    ItemListView->SelectedIndex = 0;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

#pragma region Data Visualization
// <summary>
// We will visualize the data item in asynchronously in multiple phases for improved panning user experience 
// of large lists.  In this sample scneario, we will visualize different parts of the data item
// in the following order:
// 
//     1) Title and placeholder for Image (visualized synchronously - Phase 0)
//     2) Subtitle (visualized asynchronously - Phase 1)
//     3) Image (visualized asynchronously - Phase 1)
//
// </summary>
void SDKSample::ListViewInteraction::Scenario2::ItemListView_ContainerContentChanging(
    ListViewBase^ sender,
    Windows::UI::Xaml::Controls::ContainerContentChangingEventArgs^ args)
{
    Scenario2ItemViewer^ iv = safe_cast<Scenario2ItemViewer^>(args->ItemContainer->ContentTemplateRoot);

    if (iv != nullptr)
    {
        // if the container is being added to the recycle queue (meaning it will not particiapte in 
        // visualizig data items for the time being), we clear out the data item
        if (args->InRecycleQueue == true)
        {
            iv->ClearData();
        }
        else if (args->Phase == 0)
        {
            iv->ShowTitle(safe_cast<Item^>(args->Item));

            // Register for async callback to visualize Title asynchronously
            args->RegisterUpdateCallback(ContainerContentChangingDelegate);
        }
        else if (args->Phase == 1)
        {
            iv->ShowSubtitle();
            iv->ShowImage();
        }
    }

    args->Handled = true;
}
#pragma endregion



