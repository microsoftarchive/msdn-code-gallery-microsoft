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
// DeviceAppPage.xaml.h
// Declaration of the DeviceAppPage.xaml class.
//

#pragma once

#include "pch.h"
#include "DeviceAppPage.g.h"
#include <agile.h>
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Media::Devices;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Wrapper;

namespace DeviceAppForWebcam
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DeviceAppPage sealed
    {
        ~DeviceAppPage();
        Platform::Agile<Windows::Media::Devices::VideoDeviceController> videoDevController;
        Wrapper::WinRTComponent^ lcWrapper;

    public:
        DeviceAppPage();
        void Initialize(CameraSettingsActivatedEventArgs^ args);
        void OnBrtAutoToggleChanged(Object^ sender, RoutedEventArgs^ e);
        void OnBrtSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e);
        void OnCrtAutoToggleChanged(Object^ sender, RoutedEventArgs^ e);
        void OnCrtSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e);
        void OnFocusAutoToggleChanged(Object^ sender, RoutedEventArgs^ e);
        void OnFocusSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e);
        void OnExpAutoToggleChanged(Object^ sender, RoutedEventArgs^ e);
        void OnExpSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e);
        void OnEffectEnabledToggleChanged(Object^ sender, RoutedEventArgs^ e);
        void OnEffectSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e);
    };
}
