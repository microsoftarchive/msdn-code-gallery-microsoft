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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"
#include "SampleDataSource.h"
#include "ItemViewer.xaml.h"

using namespace SDKSample::ListViewInteraction;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;

using namespace SDKSample::ListViewInteractionSampleDataSource;

Scenario4::Scenario4()
{
    InitializeComponent();
    toppingsData = ref new ToppingsData();
    storeData = ref new StoreData();
    FlavorGrid->ItemsSource = storeData->Items;
    FixinsGrid->ItemsSource = toppingsData->Items;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


// <summary>
// We will visualize the data item in asynchronously in multiple phases for improved panning user experience 
// of large lists.  In this sample scneario, we will visualize different parts of the data item
// in the following order:
// 
//     1) Placeholders (visualized synchronously - Phase 0)
//     2) Tilte (visualized asynchronously - Phase 1)
//     3) Category and Image (visualized asynchronously - Phase 2)
//
// </summary>
// <param name="sender"></param>
// <param name="args"></param>
void SDKSample::ListViewInteraction::Scenario4::GridView_ContainerContentChanging(
    ListViewBase^ sender,
    Windows::UI::Xaml::Controls::ContainerContentChangingEventArgs^ args)
{
    ItemViewer^ iv = safe_cast<ItemViewer^>(args->ItemContainer->ContentTemplateRoot);

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
            iv->ShowPlaceholder(safe_cast<Item^>(args->Item));

            // Register for async callback to visualize Title asynchronously
            args->RegisterUpdateCallback(ContainerContentChangingDelegate);
        }
        else if (args->Phase == 1)
        {
            iv->ShowTitle();
            args->RegisterUpdateCallback(ContainerContentChangingDelegate);
        }
        else if (args->Phase == 2)
        {
            iv->ShowCategory();
            iv->ShowImage();
        }

        args->Handled = true;
    }
}


void SDKSample::ListViewInteraction::Scenario4::CreateCustomCarton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);
    
    if(FlavorGrid->SelectedItems->Size >0)
    {
        CustomCarton->Text = "Custom Carton: ";
        CustomCarton->Text += ((Item^)FlavorGrid->SelectedItem)->Title;
        if(FixinsGrid->SelectedItems->Size >0)
        {
            UINT i=0;
            CustomCarton->Text += " with ";
            while(i<FixinsGrid->SelectedItems->Size)
            {
                Item^ item = dynamic_cast<Item^>(FixinsGrid->SelectedItems->GetAt(i));
                CustomCarton->Text += item->Title;
                if(i<FixinsGrid->SelectedItems->Size-1)
                    CustomCarton->Text += ", ";
                i++;
            }
            CustomCarton->Text += " toppings";
        }
    }
    else
    {
        rootPage->NotifyUser("Please select at least a flavor...", NotifyType::StatusMessage);
    }
}


