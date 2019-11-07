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
// LaunchedFromSecondaryTile.xaml.cpp
// Implementation of the LaunchedFromSecondaryTile class
//

#include "pch.h"
#include "Scenario5_LaunchedFromSecondaryTile.xaml.h"

using namespace SDKSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

LaunchedFromSecondaryTile::LaunchedFromSecondaryTile()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void LaunchedFromSecondaryTile::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    if (rootPage->LaunchParam != nullptr)
    {
        rootPage->NotifyUser("Application was activated from a Secondary Tile with the following Activation Arguments : " + rootPage->LaunchParam, NotifyType::StatusMessage);
        rootPage->LaunchParam = nullptr;
    } 
}
