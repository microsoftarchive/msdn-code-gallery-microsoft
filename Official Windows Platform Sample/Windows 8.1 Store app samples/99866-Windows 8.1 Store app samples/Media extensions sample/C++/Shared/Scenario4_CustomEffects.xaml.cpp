//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// CustomEffects.xaml.cpp
// Implementation of the CustomEffects class
//

#include "pch.h"
#include "Scenario4_CustomEffects.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MediaExtensions;

using namespace Platform;
using namespace Platform::Collections;

using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

CustomEffects::CustomEffects()
{
    InitializeComponent();
}

void CustomEffects::OpenGrayscale_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->RemoveAllEffects();
    Video->AddVideoEffect("GrayscaleTransform.GrayscaleEffect", true, nullptr);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(Video);
    SampleUtilities::PickSingleFileAndSet(v, me);
}

void CustomEffects::OpenFisheye_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OpenVideoWithPolarEffect("Fisheye");
}

void CustomEffects::OpenPinch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OpenVideoWithPolarEffect("Pinch");
}

void CustomEffects::OpenWarp_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OpenVideoWithPolarEffect("Warp");
}

void CustomEffects::OpenInvert_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->RemoveAllEffects();
    Video->AddVideoEffect("InvertTransform.InvertEffect", true, nullptr);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(Video);
    SampleUtilities::PickSingleFileAndSet(v, me);
}

void CustomEffects::Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->Source = nullptr;
}

void CustomEffects::OpenVideoWithPolarEffect(String^ effectName)
{
    Video->RemoveAllEffects();
    auto configuration = ref new PropertySet();
    configuration->Insert("effect", effectName);
    Video->AddVideoEffect("PolarTransform.PolarEffect", true, configuration);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(Video);
    SampleUtilities::PickSingleFileAndSet(v, me);
}