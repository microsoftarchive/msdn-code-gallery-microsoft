// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2Input.xaml.cpp
// Implementation of the Scenario2Input class.
//

#include "pch.h"
#include "ScenarioInput2.xaml.h"
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

ScenarioInput2::ScenarioInput2()
{
    InitializeComponent();
}

ScenarioInput2::~ScenarioInput2()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput2::rootPage_OutputFrameLoaded);

    rootPage->ExtensionManager->RegisterSchemeHandler("GeometricSource.GeometricSchemeHandler", "myscheme:");
}

void ScenarioInput2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

#pragma endregion

#pragma region Event handlers
void ScenarioInput2::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    // Go find the elements that we need for this scenario.
    outputVideo = dynamic_cast<MediaElement^>(outputFrame->FindName("Video"));

}

void ScenarioInput2::Circle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->Source = ref new Uri("myscheme://circle");
    outputVideo->Play();
}

void ScenarioInput2::Square_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->Source = ref new Uri("myscheme://square");
    outputVideo->Play();
}

void ScenarioInput2::Triangle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->Source = ref new Uri("myscheme://triangle");
    outputVideo->Play();
}
#pragma endregion
