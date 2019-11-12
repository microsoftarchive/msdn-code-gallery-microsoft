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
using namespace ControlChannelTrigger;
using namespace CppSamplesUtils;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Core;

MainPage::MainPage()
{
    InitializeComponent();

    _scenariosFrame = ScenarioList;
    _inputFrame = ScenarioInput;
    _outputFrame = ScenarioOutput;

    SetFeatureName(FEATURE_NAME);
    Loaded += ref new RoutedEventHandler(this,&MainPage::MainPage_Loaded);
    Window::Current->SizeChanged += ref new Windows::UI::Xaml::WindowSizeChangedEventHandler(this, &MainPage::Page_SizeChanged);
    DisplayProperties::LogicalDpiChanged += ref new DisplayPropertiesEventHandler(this,&MainPage::DisplayProperties_LogicalDpiChanged);
    NavigationCacheMode = Windows::UI::Xaml::Navigation::NavigationCacheMode::Required;
}

MainPage::~MainPage()
{
}

void MainPage::MainPage_Loaded(Object^ sender, RoutedEventArgs^ e)
{
    // Figure out what resolution and orientation we are in and respond appropriately
    CheckResolutionAndViewState();

    // Load the ScenarioList page into the proper frame
    TypeName pageType = { _rootNamespace + ".ScenarioList", TypeKind::Custom };
    ScenarioList->Navigate(pageType, this);
}

void MainPage::CheckResolutionAndViewState()
{
    VisualStateManager::GoToState(this, ApplicationView::Value.ToString() + DisplayProperties::ResolutionScale.ToString(), false);
}

void MainPage::DisplayProperties_LogicalDpiChanged(Object^ sender)
{
    CheckResolutionAndViewState();
}

void MainPage::SetFeatureName(String^ strFeature)
{
    FeatureName->Text = strFeature;
}

void MainPage::Page_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args)
{
    CheckResolutionAndViewState();
}
void MainPage::NotifyUser(String^ strMessage, NotifyType type)
{
    switch (type)
    {
    case NotifyType::StatusMessage:
        // Use the status message style.
        StatusBlock->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("StatusStyle"));
        break;
    case NotifyType::ErrorMessage:
        // Use the error message style.
        StatusBlock->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("ErrorStyle"));
        break;
    default:
        break;
    }
    StatusBlock->Text = strMessage;
}

void MainPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto uri = ref new Uri((String^)((HyperlinkButton^)sender)->Tag);
    Windows::System::Launcher::LaunchUriAsync(uri);
}

void MainPage::DoNavigation(TypeName inPageType, Windows::UI::Xaml::Controls::Frame^ inFrame, TypeName outPageType, Windows::UI::Xaml::Controls::Frame^ outFrame)
{
    inFrame->Navigate(inPageType, this);
    outFrame->Navigate(outPageType, this);

    // Raise InputFrameLoaded so downstream pages know that the input frame content has been loaded.
    InputFrameLoaded(this, nullptr);
    // Raise OutputFrameLoaded so downstream pages know that the output frame content has been loaded.
    OutputFrameLoaded(this, nullptr);
}
