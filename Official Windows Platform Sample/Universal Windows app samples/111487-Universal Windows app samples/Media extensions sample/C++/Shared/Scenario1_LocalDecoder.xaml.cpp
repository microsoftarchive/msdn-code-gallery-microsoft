//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LocalDecoder.xaml.cpp
// Implementation of the LocalDecoder class
//

#include "pch.h"
#include "Scenario1_LocalDecoder.xaml.h"
#include "MainPage.xaml.h"

#define INITGUID // for MFVideoFormat_MPG1
#include <guiddef.h>
#include <cguid.h>
#include <mfapi.h>

using namespace SDKSample;
using namespace SDKSample::MediaExtensions;

using namespace Platform;
using namespace Platform::Collections;

using namespace Windows::Media;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

LocalDecoder::LocalDecoder()
{
    InitializeComponent();

	extensionManager = ref new MediaExtensionManager();
    extensionManager->RegisterByteStreamHandler("MPEG1Source.MPEG1ByteStreamHandler", ".mpg", "video/mpeg");
    extensionManager->RegisterVideoDecoder("MPEG1Decoder.MPEG1Decoder", Guid(MFVideoFormat_MPG1), Guid(GUID_NULL));
}

void LocalDecoder::Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto v = ref new Vector<String^>();
    v->Append(".mpg");
    auto me = ref new Vector<MediaElement^>();
    me->Append(Video);
    SampleUtilities::PickSingleFileAndSet(v, me);
}

void LocalDecoder::Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->Source = nullptr;
}