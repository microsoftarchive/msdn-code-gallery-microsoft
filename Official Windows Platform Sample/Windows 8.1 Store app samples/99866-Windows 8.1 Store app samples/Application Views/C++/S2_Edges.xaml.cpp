//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2_Edges.xaml.cpp
// Implementation of the S2_Edges class
//

#include "pch.h"
#include "S2_Edges.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ApplicationViews;

using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ViewManagement;

S2_Edges::S2_Edges()
{
    InitializeComponent();
}

void S2_Edges::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Subscribe to window resize events
    sizeChangedToken = Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler(this, &S2_Edges::WindowSizeChanged);
    // Run layout logic when the page first loads
    InvalidateLayout();
}

void S2_Edges::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Unsubscribe from window resize events
    Window::Current->SizeChanged -= sizeChangedToken;
}

void S2_Edges::WindowSizeChanged(Platform::Object ^sender, WindowSizeChangedEventArgs ^e)
{
    InvalidateLayout();
}

void S2_Edges::InvalidateLayout()
{
    // Get an instance of ApplicationView for the current window
    ApplicationView^ currentAppView = ApplicationView::GetForCurrentView();
    if (currentAppView->IsFullScreen)
    {
        // If app is full screen, center the control
        Output->HorizontalAlignment = Xaml::HorizontalAlignment::Center;
        MainPage::Current->NotifyUser("App window is full screen.", NotifyType::StatusMessage);
    }
    else if (currentAppView->AdjacentToLeftDisplayEdge)
    {
        // If app is adjacent to the left edge, align control to the left
        Output->HorizontalAlignment = Xaml::HorizontalAlignment::Left;
        MainPage::Current->NotifyUser("App window is adjacent to the left display edge.", NotifyType::StatusMessage);
    }
    else if (currentAppView->AdjacentToRightDisplayEdge)
    {
        // If app is adjacent to the right edge, align control to the right
        Output->HorizontalAlignment = Xaml::HorizontalAlignment::Right;
        MainPage::Current->NotifyUser("App window is adjacent to the right display edge.", NotifyType::StatusMessage);
    }
    else
    {
        // If app is not adjacent to either side of the screen, center the control
        Output->HorizontalAlignment = Xaml::HorizontalAlignment::Center;
        MainPage::Current->NotifyUser("App window is not adjacent to any edges.", NotifyType::StatusMessage);
    }
}