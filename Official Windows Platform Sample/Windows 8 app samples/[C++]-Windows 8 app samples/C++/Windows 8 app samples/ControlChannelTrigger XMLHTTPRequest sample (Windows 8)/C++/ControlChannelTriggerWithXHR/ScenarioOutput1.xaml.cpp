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
// ScenarioOutput1.xaml.cpp
// Implementation of the ScenarioOutput1 class
//

#include "pch.h"
#include "ScenarioOutput1.xaml.h"

using namespace ControlChannelTrigger;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;

ScenarioOutput1::ScenarioOutput1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ScenarioOutput1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    rootPage->InputFrameLoaded += ref new EventHandler<Platform::Object^>(this,&ScenarioOutput1::rootPage_InputFrameLoaded);
}


void ScenarioOutput1::rootPage_InputFrameLoaded(Object^ sender, Object^ e)
{
    // At this point, we know that the Input Frame has been loaded and we can go ahead
    // and reference elements in the page contained in the Input Frame.

    // Get a pointer to the content within the IntputFrame.
    Page^ inputFrame = dynamic_cast<Page^>(rootPage->InputFrame->Content);

    // Go find the elements that we need for this scenario
    // ex: flipView1 = inputFrame.FindName("FlipView1") as FlipView;
}
