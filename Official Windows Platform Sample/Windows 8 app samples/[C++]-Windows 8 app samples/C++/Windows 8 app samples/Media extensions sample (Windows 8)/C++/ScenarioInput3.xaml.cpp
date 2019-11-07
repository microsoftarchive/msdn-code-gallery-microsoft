// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3Input.xaml.cpp
// Implementation of the Scenario3Input class.
//

#include "pch.h"
#include "ScenarioInput3.xaml.h"
#include "MainPage.xaml.h"

using namespace MediaExtensionsCPP;

using namespace Platform;
using namespace Platform::Collections;
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

ScenarioInput3::ScenarioInput3()
{
    InitializeComponent();
}

ScenarioInput3::~ScenarioInput3()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput3::rootPage_OutputFrameLoaded);
}

void ScenarioInput3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

#pragma endregion

#pragma region Event handlers
void ScenarioInput3::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    // Go find the elements that we need for this scenario.
    outputVideo = dynamic_cast<MediaElement^>(outputFrame->FindName("Video"));
    outputVideoStabilized = dynamic_cast<MediaElement^>(outputFrame->FindName("VideoStabilized"));
}

void ScenarioInput3::Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideoStabilized->RemoveAllEffects();
    outputVideoStabilized->AddVideoEffect(Windows::Media::VideoEffects::VideoStabilization, true, nullptr);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(outputVideo);
    me->Append(outputVideoStabilized);
    rootPage->PickSingleFileAndSet(v, me);
}

void ScenarioInput3::Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->Source = nullptr;
    outputVideoStabilized->Source = nullptr;
}
#pragma endregion
