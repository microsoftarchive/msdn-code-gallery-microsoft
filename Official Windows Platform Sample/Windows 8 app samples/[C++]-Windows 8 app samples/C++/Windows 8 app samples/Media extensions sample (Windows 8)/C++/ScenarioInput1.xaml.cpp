// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Input.xaml.cpp
// Implementation of the Scenario1Input class
//

#include "pch.h"
#include "ScenarioInput1.xaml.h"

#define INITGUID // for MFVideoFormat_MPG1
#include <guiddef.h>
#include <cguid.h>
#include <mfapi.h>

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

ScenarioInput1::ScenarioInput1()
{
    InitializeComponent();
}

ScenarioInput1::~ScenarioInput1()
{
}

#pragma region Template-Related Code - Do not remove
void ScenarioInput1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // We want to be notified with the OutputFrame is loaded so we can get to the content.
    _frameLoadedToken = rootPage->OutputFrameLoaded += ref new Windows::Foundation::EventHandler<Platform::Object^>(this, &ScenarioInput1::rootPage_OutputFrameLoaded);

    rootPage->ExtensionManager->RegisterByteStreamHandler("MPEG1Source.MPEG1ByteStreamHandler", ".mpg", "video/mpeg");
    rootPage->ExtensionManager->RegisterVideoDecoder("MPEG1Decoder.MPEG1Decoder", Guid(MFVideoFormat_MPG1), Guid(GUID_NULL));
}

void ScenarioInput1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    rootPage->OutputFrameLoaded -= _frameLoadedToken;
}

#pragma endregion

#pragma region Event handlers
void ScenarioInput1::rootPage_OutputFrameLoaded(Object^ sender, Object^ e)
{
    // Get a pointer to the content within the OutputFrame.
    Page^ outputFrame = dynamic_cast<Page^>(rootPage->OutputFrame->Content);

    // Go find the elements that we need for this scenario.
    outputVideo = dynamic_cast<MediaElement^>(outputFrame->FindName("Video"));

}

void ScenarioInput1::Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto v = ref new Vector<String^>();
    v->Append(".mpg");
    auto me = ref new Vector<MediaElement^>();
    me->Append(outputVideo);
    rootPage->PickSingleFileAndSet(v, me);
}

void ScenarioInput1::Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    outputVideo->Source = nullptr;
}
#pragma endregion
