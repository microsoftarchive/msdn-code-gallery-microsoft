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
// AuthByBackgroundTask.xaml.cpp
// Implementation of the AuthByBackgroundTask class
//

#include "pch.h"
#include "AuthByBackgroundTask.xaml.h"

using namespace SDKSample::HotspotAuthenticationApp;
using namespace HotspotAuthenticationTask;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

AuthByBackgroundTask::AuthByBackgroundTask()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void AuthByBackgroundTask::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Configure background task handler to perform authentication
    ConfigStore::AuthenticateThroughBackgroundTask = true;
}
