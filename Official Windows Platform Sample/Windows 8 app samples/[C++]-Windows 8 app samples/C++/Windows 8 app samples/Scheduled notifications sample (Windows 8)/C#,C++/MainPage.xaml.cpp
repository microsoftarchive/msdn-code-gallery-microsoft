// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// MainPage.xaml.cpp
// Implementation of the MainPage.xaml class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "App.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace ScheduledNotificationsSampleCPP;
using namespace CppSamplesUtils;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;

MainPage::MainPage()
{
    InitializeComponent();

    SetFeatureName("Scheduled Notifications");

    _scenariosFrame = ScenarioList;
    _inputFrame = ScenarioInput;
    _outputFrame = ScenarioOutput;

    _pageLoadedHandlerToken = Loaded += ref new RoutedEventHandler(this, &MainPage::Page_Loaded);
    _logicalDpiChangedToken = DisplayProperties::LogicalDpiChanged += ref new DisplayPropertiesEventHandler(this, &MainPage::DisplayProperties_LogicalDpiChanged);
    _resizeHandlerToken = Window::Current->SizeChanged += ref new Windows::UI::Xaml::WindowSizeChangedEventHandler(this, &MainPage::Page_SizeChanged);
}

MainPage::~MainPage()
{
}

void MainPage::Page_Loaded(Object^ sender, RoutedEventArgs^ e)
{
    // Load the ScenarioList page into the proper frame
    TypeName pageType = { "ScheduledNotificationsSampleCPP.ScenarioList", TypeKind::Custom };
    ScenarioList->Navigate(pageType, this);

    // Figure out what resolution and orientation we are in and respond appropriately.
    CheckResolutionAndViewState();
}

void MainPage::SetFeatureName(String^ strFeature)
{
    FeatureName->Text = strFeature;
}

void MainPage::DisplayProperties_LogicalDpiChanged(Object^ sender)
{
    CheckResolutionAndViewState();
}

void MainPage::CheckResolutionAndViewState()
{
    ApplicationViewState state = ApplicationView::Value;
    String^ stateString = ConvertViewState(state);

    ResolutionScale scale = DisplayProperties::ResolutionScale;
    String^ scaleString = ConvertResolution(scale);

    VisualStateManager::GoToState(this, stateString + scaleString, false);
}

String^ MainPage::ConvertViewState(ApplicationViewState state)
{
    switch(state)
    {
    case ApplicationViewState::Filled:
        return "Filled";
    case ApplicationViewState::FullScreenLandscape:
        return "FullScreenLandscape";
    case ApplicationViewState::FullScreenPortrait:
        return "FullScreenPortrait";
    case ApplicationViewState::Snapped:
        return "Snapped";
    }
    return "";
}

String^ MainPage::ConvertResolution(ResolutionScale scale)
{
    switch(scale)
    {
    case ResolutionScale::Scale100Percent:
        return "Scale100Percent";
    case ResolutionScale::Scale140Percent:
        return "Scale140Percent";
    case ResolutionScale::Scale180Percent:
        return "Scale180Percent";
    }
    return "";
}

void MainPage::DoNavigation(TypeName inPageType, Windows::UI::Xaml::Controls::Frame^ inFrame, TypeName outPageType, Windows::UI::Xaml::Controls::Frame^ outFrame)
{
    inFrame->Navigate(inPageType, this);
    outFrame->Navigate(outPageType, this);

    // Raise OutputFrameLoaded so downstream pages know that the output frame content has been loaded
    OutputFrameLoaded(this, nullptr);
    // Raise InputFrameLoaded so downstream pages know that the input frame content has been loaded
    InputFrameLoaded(this, nullptr);
}

void MainPage::NotifyUser(String^ strMessage, NotifyType type)
{
    switch (type)
    {
    case NotifyType::StatusMessage:
        StatusBlock->Style = dynamic_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("StatusStyle"));
        break;
    case NotifyType::ErrorMessage:
        StatusBlock->Style = dynamic_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("ErrorStyle"));
        break;
    default:
        break;
    }
    StatusBlock->Text = strMessage;
}

void MainPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto _hyperlinkButton = (HyperlinkButton^)sender;
    auto uri = ref new Windows::Foundation::Uri((String^)_hyperlinkButton->Tag);

    Windows::System::Launcher::LaunchUriAsync(uri);
}

void MainPage::Page_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args)
{
    switch (Windows::UI::ViewManagement::ApplicationView::Value)
    {
    case ApplicationViewState::Filled:
        VisualStateManager::GoToState(this, "Filled", false);
        break;
    case ApplicationViewState::FullScreenLandscape:
        VisualStateManager::GoToState(this, "FullScreenLandscape", false);
        break;
    case ApplicationViewState::Snapped:
        VisualStateManager::GoToState(this, "Snapped", false);
        break;
    case ApplicationViewState::FullScreenPortrait:
        VisualStateManager::GoToState(this, "FullScreenPortrait", false);
        break;
    }
}
