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
// DeviceAppPage.xaml.cpp
// Implementation of the DeviceAppPage.xaml class.
//

#include "pch.h"
#include "DeviceAppPage.xaml.h"

using namespace DeviceAppForWebcam;

DeviceAppPage::DeviceAppPage()
{
    InitializeComponent();
}

DeviceAppPage::~DeviceAppPage()
{
}

void DeviceAppPage::Initialize(CameraSettingsActivatedEventArgs^ args)
{
    videoDevController = Platform::Agile<Windows::Media::Devices::VideoDeviceController>();
    videoDevController = (Windows::Media::Devices::VideoDeviceController^)args->VideoDeviceController;

    lcWrapper = nullptr;
    if (args->VideoDeviceExtension != nullptr)
    {
        lcWrapper = ref new WinRTComponent();
        lcWrapper->Initialize(args->VideoDeviceExtension);
    }

    bool bAuto = false;
    double value = 0.0;

    if (videoDevController->Brightness->Capabilities->Step != 0)
    {
        slBrt->Minimum = videoDevController->Brightness->Capabilities->Min;
        slBrt->Maximum = videoDevController->Brightness->Capabilities->Max;
        slBrt->StepFrequency = videoDevController->Brightness->Capabilities->Step;
        videoDevController->Brightness->TryGetValue(&value);
        slBrt->Value = value;
    }
    else
    {
        slBrt->IsEnabled = false;
    }
    if (videoDevController->Brightness->Capabilities->AutoModeSupported)
    {
        videoDevController->Brightness->TryGetAuto(&bAuto);
        tsBrtAuto->IsOn = bAuto;
    }
    else
    {
        tsBrtAuto->IsOn = false;
        tsBrtAuto->IsEnabled = false;
    }

    if (videoDevController->Contrast->Capabilities->Step != 0)
    {
        slCrt->Minimum = videoDevController->Contrast->Capabilities->Min;
        slCrt->Maximum = videoDevController->Contrast->Capabilities->Max;
        slCrt->StepFrequency = videoDevController->Contrast->Capabilities->Step;
        videoDevController->Contrast->TryGetValue(&value);
        slCrt->Value = value;
    }
    else
    {
        slCrt->IsEnabled = false;
    }
    if (videoDevController->Contrast->Capabilities->AutoModeSupported)
    {
        videoDevController->Contrast->TryGetAuto(&bAuto);
        tsCrtAuto->IsOn = bAuto;
    }
    else
    {
        tsCrtAuto->IsOn = false;
        tsCrtAuto->IsEnabled = false;
    }

    if (videoDevController->Focus->Capabilities->Step != 0)
    {
        slFocus->Minimum = videoDevController->Focus->Capabilities->Min;
        slFocus->Maximum = videoDevController->Focus->Capabilities->Max;
        slFocus->StepFrequency = videoDevController->Focus->Capabilities->Step;
        videoDevController->Focus->TryGetValue(&value);
        slFocus->Value = value;
    }
    else
    {
        slFocus->IsEnabled = false;
    }
    if (videoDevController->Focus->Capabilities->AutoModeSupported)
    {
        videoDevController->Focus->TryGetAuto(&bAuto);
        tsFocusAuto->IsOn = bAuto;
    }
    else
    {
        tsFocusAuto->IsOn = false;
        tsFocusAuto->IsEnabled = false;
    }

    if (videoDevController->Exposure->Capabilities->Step != 0)
    {
        slExp->Minimum = videoDevController->Exposure->Capabilities->Min;
        slExp->Maximum = videoDevController->Exposure->Capabilities->Max;
        slExp->StepFrequency = videoDevController->Exposure->Capabilities->Step;
        videoDevController->Exposure->TryGetValue(&value);
        slExp->Value = value;
    }
    else
    {
        slExp->IsEnabled = false;
    }
    if (videoDevController->Exposure->Capabilities->AutoModeSupported)
    {
        videoDevController->Exposure->TryGetAuto(&bAuto);
        tsExpAuto->IsOn = bAuto;
    }
    else
    {
        tsExpAuto->IsOn = false;
        tsExpAuto->IsEnabled = false;
    }

    if (lcWrapper != nullptr)
    {
        slEffect->Minimum = 0;
        slEffect->Maximum = 100;
        slEffect->StepFrequency = 1;

        DspSettings dspSettings = lcWrapper->GetDspSetting();
        slEffect->Value = dspSettings.percentOfScreen;

        if (dspSettings.isEnabled == 1)
        {
            tsEffectEnabled->IsOn = true;
        }
        else
        {
            tsEffectEnabled->IsOn = false;
            slEffect->IsEnabled = false;
        }
    }
    else
    {
        tsEffectEnabled->IsEnabled = false;
        slEffect->IsEnabled = false;
    }
}

void DeviceAppPage::OnBrtAutoToggleChanged(Object^ sender, RoutedEventArgs^ e)
{
    videoDevController->Brightness->TrySetAuto(tsBrtAuto->IsOn);
    slBrt->IsEnabled = !tsBrtAuto->IsOn;
}

void DeviceAppPage::OnBrtSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    videoDevController->Brightness->TrySetValue(slBrt->Value);
}

void DeviceAppPage::OnCrtAutoToggleChanged(Object^ sender, RoutedEventArgs^ e)
{
    videoDevController->Contrast->TrySetAuto(tsCrtAuto->IsOn);
    slCrt->IsEnabled = !tsCrtAuto->IsOn;
}

void DeviceAppPage::OnCrtSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    videoDevController->Contrast->TrySetValue(slCrt->Value);
}

void DeviceAppPage::OnFocusAutoToggleChanged(Object^ sender, RoutedEventArgs^ e)
{
    videoDevController->Focus->TrySetAuto(tsFocusAuto->IsOn);
    slFocus->IsEnabled = !tsFocusAuto->IsOn;
}

void DeviceAppPage::OnFocusSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    videoDevController->Focus->TrySetValue(slFocus->Value);
}

void DeviceAppPage::OnExpAutoToggleChanged(Object^ sender, RoutedEventArgs^ e)
{
    videoDevController->Exposure->TrySetAuto(tsExpAuto->IsOn);
    slExp->IsEnabled = !tsExpAuto->IsOn;
}

void DeviceAppPage::OnExpSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    videoDevController->Exposure->TrySetValue(slExp->Value);
}

void DeviceAppPage::OnEffectEnabledToggleChanged(Object^ sender, RoutedEventArgs^ e)
{
    if (tsEffectEnabled->IsOn)
    {
        lcWrapper->Enable();
    }
    else
    {
        lcWrapper->Disable();
    }
    slEffect->IsEnabled = tsEffectEnabled->IsOn;
}

void DeviceAppPage::OnEffectSliderValueChanged(Object^ sender, RangeBaseValueChangedEventArgs^ e)
{
    lcWrapper->UpdateDsp((int)slEffect->Value);
}
