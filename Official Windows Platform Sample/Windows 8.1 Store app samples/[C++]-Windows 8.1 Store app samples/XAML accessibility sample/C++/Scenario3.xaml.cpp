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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//
#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample::Accessibility;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;


MediaContainerAP::MediaContainerAP(MediaElementContainer^ owner, MediaElement^ mediaElement)
    :  FrameworkElementAutomationPeer(owner)
{
    _mediaElement = mediaElement;
}


Scenario3::Scenario3()
{
    InitializeComponent();
    MediaElementContainer^ me = ref new MediaElementContainer(Scenario3Output);
    Windows::UI::Xaml::Automation::AutomationProperties::SetName(me, "CustomMediaElement");

}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


