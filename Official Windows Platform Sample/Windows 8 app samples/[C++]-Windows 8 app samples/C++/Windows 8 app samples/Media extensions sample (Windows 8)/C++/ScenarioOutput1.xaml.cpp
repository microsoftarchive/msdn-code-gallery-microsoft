// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Output.xaml.cpp
// Implementation of the Scenario1Output class
//

#include "pch.h"
#include "ScenarioOutput1.xaml.h"
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

ScenarioOutput1::ScenarioOutput1()
{
    InitializeComponent();
}

ScenarioOutput1::~ScenarioOutput1()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioOutput1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    _mediaFailedToken = Video->MediaFailed += ref new ExceptionRoutedEventHandler(rootPage, &MediaExtensionsCPP::MainPage::VideoOnError);
}

void ScenarioOutput1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    Video->MediaFailed -= _mediaFailedToken;
    Video->Source = nullptr;
}
#pragma endregion
