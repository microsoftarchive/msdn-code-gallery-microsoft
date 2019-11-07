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
using namespace AssociationLaunching;
using namespace CppSamplesUtils;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Interop;

MainPage::MainPage()
{
    InitializeComponent();

    SetFeatureName("AssociationLaunching");

    _scenariosFrame = ScenarioList;
    _inputFrame = ScenarioInput;
    _outputFrame = ScenarioOutput;

    _layoutHandlerToken = Window::Current->SizeChanged += ref new Windows::UI::Xaml::WindowSizeChangedEventHandler(this, &MainPage::Page_SizeChanged);

    _pageLoadedHandlerToken = Loaded += ref new RoutedEventHandler(this, &MainPage::Page_Loaded);
    _logicalDpiChangedToken = DisplayProperties::LogicalDpiChanged += ref new DisplayPropertiesEventHandler(this, &MainPage::DisplayProperties_LogicalDpiChanged);
}

MainPage::~MainPage()
{
}

void MainPage::Page_Loaded(Object^ sender, RoutedEventArgs^ e)
{
    // Load the ScenarioList page into the proper frame.
    TypeName pageType = { "AssociationLaunching.ScenarioList", TypeKind::Custom };
    ScenarioList->Navigate(pageType, this);

    // Figure out what resolution and orientation we are in and respond appropriately.
    CheckResolutionAndViewState();
}

void MainPage::SetFeatureName(String^ strFeature)
{
    FeatureName->Text = strFeature;
}

void MainPage::Page_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
    CheckResolutionAndViewState();
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
    switch (state)
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
    switch (scale)
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

void MainPage::DoNavigation(TypeName pageType, Windows::UI::Xaml::Controls::Frame^ frame)
{
    frame->Navigate(pageType, this);

    std::wstring PageName(pageType.Name->Data());
    std::basic_string <wchar_t>::size_type indexSubstring;
    indexSubstring = PageName.find(L"Output");
    if (indexSubstring != std::wstring::npos)
    {
        // Raise OutputFrameLoaded so downstream pages know that the output frame content has been loaded.
        OutputFrameLoaded(this, nullptr);
    }
    else
    {
        // Raise InputFrameLoaded so downstream pages know that the input frame content has been loaded.
        InputFrameLoaded(this, nullptr);
    }
}

void MainPage::NotifyUser(String^ strMessage, NotifyType type)
{
    switch (type)
    {
    case NotifyType::StatusMessage:
        StatusBlock->Style = dynamic_cast<Windows::UI::Xaml::Style^>(App::Current->Resources->Lookup("StatusStyle"));
        break;
    case NotifyType::ErrorMessage:
        StatusBlock->Style = dynamic_cast<Windows::UI::Xaml::Style^>(App::Current->Resources->Lookup("ErrorStyle"));
        break;
    default:
        break;
    }
    StatusBlock->Text = strMessage;
}

void MainPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto uri = ref new Windows::Foundation::Uri((String^)((HyperlinkButton^)sender)->Tag);
    Windows::System::Launcher::LaunchUriAsync(uri);
}
