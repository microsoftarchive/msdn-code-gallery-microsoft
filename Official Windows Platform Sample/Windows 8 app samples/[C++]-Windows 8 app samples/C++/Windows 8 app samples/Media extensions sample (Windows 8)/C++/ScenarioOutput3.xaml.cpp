// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3Output.xaml.cpp
// Implementation of the Scenario3Output class
//

#include "pch.h"
#include "ScenarioOutput3.xaml.h"
#include "MainPage.xaml.h"

using namespace MediaExtensionsCPP;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

ScenarioOutput3::ScenarioOutput3()
{
    InitializeComponent();
}

ScenarioOutput3::~ScenarioOutput3()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioOutput3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    _mediaFailedToken = Video->MediaFailed += ref new ExceptionRoutedEventHandler(rootPage, &MediaExtensionsCPP::MainPage::VideoOnError);
    _stabilizedMediaFailedToken = VideoStabilized->MediaFailed += ref new ExceptionRoutedEventHandler(rootPage, &MediaExtensionsCPP::MainPage::VideoOnError);
}

void ScenarioOutput3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    Video->MediaFailed -= _mediaFailedToken;
    Video->Source = nullptr;

    VideoStabilized->MediaFailed -= _stabilizedMediaFailedToken;
    VideoStabilized->Source = nullptr;
}
#pragma endregion