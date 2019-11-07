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
// MainPage.xaml.cpp
// Implementation of the MainPage.xaml class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "App.xaml.h"

#include <collection.h>

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Platform;
using namespace DeviceAppForWebcam;
using namespace CppSamplesUtils;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;

MainPage^ MainPage::Current = nullptr;

MainPage::MainPage()
{
    InitializeComponent();

    // This frame is hidden, meaning it is never shown.  It is simply used to load
    // each scenario page and then pluck out the input and output sections and
    // place them into the UserControls on the main page.
    HiddenFrame = ref new Windows::UI::Xaml::Controls::Frame();
    HiddenFrame->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    LayoutRoot->Children->Append(HiddenFrame);

    // Populate the sample title from the constant in the Constants.h file.
    SetDeviceAppForWebcam(FEATURE_NAME);

    this->SizeChanged += ref new SizeChangedEventHandler(this, &MainPage::MainPage_SizeChanged);

    MainPage::Current = this;
}

MainPage::~MainPage()
{
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    InvalidateViewState();
}

/// <summary>
/// We need to handle SizeChanged so that we can make the sample layout property
/// in the various layouts.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void MainPage::MainPage_SizeChanged(Object^ sender, SizeChangedEventArgs^ e)
{
    InvalidateSize();
}

void MainPage::InvalidateSize()
{
    // Get the window width
    double windowWidth = this->ActualWidth;

    if (windowWidth != 0.0)
    {
        // Get the width of the ContentPanel.
        double contentPanelWidth = ContentPanel->ActualWidth;

        // Is the ContentPanel using any margins that we need to consider?
        double contentPanelMarginLeft = ContentPanel->Margin.Left;
        double contentPanelMarginRight = ContentPanel->Margin.Right;

        // Figure out how much room is left after considering the ContentPanel width
        double availableWidth = windowWidth;

        // Is the top most child using margins?
        double layoutRootMarginLeft = ContentRoot->Margin.Left;
        double layoutRootMarginRight = ContentRoot->Margin.Right;

        // We have different widths to use depending on the view state
        if (ApplicationView::Value != ApplicationViewState::Snapped)
        {
            // Make us as big as the the left over space, factoring in the ContentPanel width, the ContentPanel margins,
            // and the LayoutRoot's margins
            ContentPanel->Width = ((availableWidth) - 
                (layoutRootMarginLeft + layoutRootMarginRight + contentPanelMarginLeft + contentPanelMarginRight));
        }
        else
        {
            // Make us as big as the left over space, factoring in just the LayoutRoot's margins.
            ContentPanel->Width = (windowWidth - (layoutRootMarginLeft + layoutRootMarginRight));
        }
    }
    InvalidateViewState();
}

void MainPage::InvalidateViewState()
{
    // Are we going to snapped mode?
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        Grid::SetRow(FooterPanel, 2);
        Grid::SetColumn(FooterPanel, 0);
    }
    else
    {
        Grid::SetRow(FooterPanel, 1);
        Grid::SetColumn(FooterPanel, 1);
    }
}

void MainPage::SetDeviceAppForWebcam(String^ strFeature)
{
    DeviceAppForWebcam->Text = strFeature;
}

void MainPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto uri = ref new Uri((String^)((HyperlinkButton^)sender)->Tag);
    Windows::System::Launcher::LaunchUriAsync(uri);
}


