//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_Orientation.xaml.cpp
// Implementation of the S1_Orientation class
//

#include "pch.h"
#include "S1_Orientation.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ApplicationViews;

using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ViewManagement;

S1_Orientation::S1_Orientation()
{
    InitializeComponent();
}

void S1_Orientation::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Subscribe to window resize events
    sizeChangedToken = Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler(this, &S1_Orientation::WindowSizeChanged);
    // Run layout logic when the page first loads
    InvalidateLayout();
}

void S1_Orientation::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Unsubscribe to window resize events
    Window::Current->SizeChanged -= sizeChangedToken;
}

void S1_Orientation::WindowSizeChanged(Platform::Object ^sender, WindowSizeChangedEventArgs ^e)
{
    InvalidateLayout();
}

void S1_Orientation::InvalidateLayout()
{
    // Get window orientation
    ApplicationViewOrientation winOrientation = ApplicationView::GetForCurrentView()->Orientation;
    if (winOrientation == ApplicationViewOrientation::Landscape)
    {
        // Update grid to stack the boxes horizontally in landscape orientation
        Grid::SetColumn(Box1, 0);
        Grid::SetRow(Box1, 0);
        Grid::SetColumnSpan(Box1, 1);
        Grid::SetRowSpan(Box1, 2);

        Grid::SetColumn(Box2, 1);
        Grid::SetRow(Box2, 0);
        Grid::SetColumnSpan(Box2, 1);
        Grid::SetRowSpan(Box2, 2);

        MainPage::Current->NotifyUser("Windows orientation is landscape.", NotifyType::StatusMessage);
    }
    else if (winOrientation == ApplicationViewOrientation::Portrait)
    {
        // Update grid to stack the boxes vertically in portrait orientation
        Grid::SetColumn(Box1, 0);
        Grid::SetRow(Box1, 0);
        Grid::SetColumnSpan(Box1, 2);
        Grid::SetRowSpan(Box1, 1);

        Grid::SetColumn(Box2, 0);
        Grid::SetRow(Box2, 1);
        Grid::SetColumnSpan(Box2, 2);
        Grid::SetRowSpan(Box2, 1);

        MainPage::Current->NotifyUser("Windows orientation is portrait.", NotifyType::StatusMessage);
    }
}
