//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Projection;

using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // An app can query if a second display is available...
    UpdateTextBlock(ProjectionManager::ProjectionDisplayAvailable);
    // ...or listen for when a display is attached or detached
    displayChangeToken = ProjectionManager::ProjectionDisplayAvailableChanged += ref new Windows::Foundation::EventHandler<Object^>(this, &Scenario2::OnProjectionDisplayAvailableChanged);
    dispatcher = Window::Current->Dispatcher;
}

void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    ProjectionManager::ProjectionDisplayAvailableChanged -= displayChangeToken;
}

void Scenario2::OnProjectionDisplayAvailableChanged(Object^ sender, Object^ e)
{
    dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this] () {
        UpdateTextBlock(ProjectionManager::ProjectionDisplayAvailable);
    }));
}

void Scenario2::UpdateTextBlock(bool screenAvailable)
{
    if (screenAvailable)
    {
        ScreenAvailabilityBlock->Text = "A second screen is available";
    }
    else
    {
        ScreenAvailabilityBlock->Text = "No second screen is available";
    }
}