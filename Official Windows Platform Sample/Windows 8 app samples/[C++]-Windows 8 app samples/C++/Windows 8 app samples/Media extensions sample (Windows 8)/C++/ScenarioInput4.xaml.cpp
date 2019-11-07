// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ScenarioInput4.xaml.cpp
// Implementation of the ScenarioInput4 class
//

#include "pch.h"
#include "ScenarioInput4.xaml.h"

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

ScenarioInput4::ScenarioInput4()
{
    InitializeComponent();
}

ScenarioInput4::~ScenarioInput4()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput4::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput4::rootPage_OutputFrameLoaded);
}

void ScenarioInput4::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

#pragma endregion

#pragma region Event handlers
void ScenarioInput4::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    // Go find the elements that we need for this scenario.
    outputVideo = dynamic_cast<MediaElement^>(outputFrame->FindName("Video"));

}

void ScenarioInput4::OpenGrayscale_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->RemoveAllEffects();
    outputVideo->AddVideoEffect("GrayscaleTransform.GrayscaleEffect", true, nullptr);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(outputVideo);
    rootPage->PickSingleFileAndSet(v, me);
}

void ScenarioInput4::OpenFisheye_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OpenVideoWithPolarEffect("Fisheye");
}

void ScenarioInput4::OpenPinch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OpenVideoWithPolarEffect("Pinch");
}

void ScenarioInput4::OpenWarp_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OpenVideoWithPolarEffect("Warp");
}

void ScenarioInput4::OpenInvert_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->RemoveAllEffects();
    outputVideo->AddVideoEffect("InvertTransform.InvertEffect", true, nullptr);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(outputVideo);
    rootPage->PickSingleFileAndSet(v, me);
}

void ScenarioInput4::Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->Source = nullptr;
}
#pragma endregion

void ScenarioInput4::OpenVideoWithPolarEffect(String^ effectName)
{
    outputVideo->RemoveAllEffects();
    auto configuration = ref new PropertySet();
    configuration->Insert("effect", effectName);
    outputVideo->AddVideoEffect("PolarTransform.PolarEffect", true, configuration);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(outputVideo);
    rootPage->PickSingleFileAndSet(v, me);
}