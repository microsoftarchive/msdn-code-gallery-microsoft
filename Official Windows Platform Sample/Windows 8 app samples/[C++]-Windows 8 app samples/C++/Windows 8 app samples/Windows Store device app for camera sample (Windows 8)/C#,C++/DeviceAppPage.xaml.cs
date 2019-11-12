// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.ApplicationModel.Activation;
using Windows.Media.Devices;
using System.Runtime.InteropServices;
using Wrapper;

namespace SDKTemplate
{
    partial class DeviceAppPage
    {
        VideoDeviceController videoDevController = null;
        Wrapper.WinRTComponent lcWrapper = null;

        public DeviceAppPage()
        {
            InitializeComponent();
        }

        public void Initialize(CameraSettingsActivatedEventArgs args)
        {
            videoDevController = (VideoDeviceController)args.VideoDeviceController;

            if (args.VideoDeviceExtension != null)
            {
                lcWrapper = new WinRTComponent();
                lcWrapper.Initialize(args.VideoDeviceExtension);
            }

            bool bAuto = false;
            double value = 0.0;

            if (videoDevController.Brightness.Capabilities.Step != 0)
            {
                slBrt.Minimum = videoDevController.Brightness.Capabilities.Min;
                slBrt.Maximum = videoDevController.Brightness.Capabilities.Max;
                slBrt.StepFrequency = videoDevController.Brightness.Capabilities.Step;
                videoDevController.Brightness.TryGetValue(out value);
                slBrt.Value = value;
            }
            else
            {
                slBrt.IsEnabled = false;
            }
            if (videoDevController.Brightness.Capabilities.AutoModeSupported)
            {
                videoDevController.Brightness.TryGetAuto(out bAuto);
                tsBrtAuto.IsOn = bAuto;
            }
            else
            {
                tsBrtAuto.IsOn = false;
                tsBrtAuto.IsEnabled = false;
            }

            if (videoDevController.Contrast.Capabilities.Step != 0)
            {
                slCrt.Minimum = videoDevController.Contrast.Capabilities.Min;
                slCrt.Maximum = videoDevController.Contrast.Capabilities.Max;
                slCrt.StepFrequency = videoDevController.Contrast.Capabilities.Step;
                videoDevController.Contrast.TryGetValue(out value);
                slCrt.Value = value;
            }
            else
            {
                slCrt.IsEnabled = false;
            }
            if (videoDevController.Contrast.Capabilities.AutoModeSupported)
            {
                videoDevController.Contrast.TryGetAuto(out bAuto);
                tsCrtAuto.IsOn = bAuto;
            }
            else
            {
                tsCrtAuto.IsOn = false;
                tsCrtAuto.IsEnabled = false;
            }

            if (videoDevController.Focus.Capabilities.Step != 0)
            {
                slFocus.Minimum = videoDevController.Focus.Capabilities.Min;
                slFocus.Maximum = videoDevController.Focus.Capabilities.Max;
                slFocus.StepFrequency = videoDevController.Focus.Capabilities.Step;
                videoDevController.Focus.TryGetValue(out value);
                slFocus.Value = value;
            }
            else
            {
                slFocus.IsEnabled = false;
            }
            if (videoDevController.Focus.Capabilities.AutoModeSupported)
            {
                videoDevController.Focus.TryGetAuto(out bAuto);
                tsFocusAuto.IsOn = bAuto;
            }
            else
            {
                tsFocusAuto.IsOn = false;
                tsFocusAuto.IsEnabled = false;
            }

            if (videoDevController.Exposure.Capabilities.Step != 0)
            {
                slExp.Minimum = videoDevController.Exposure.Capabilities.Min;
                slExp.Maximum = videoDevController.Exposure.Capabilities.Max;
                slExp.StepFrequency = videoDevController.Exposure.Capabilities.Step;
                videoDevController.Exposure.TryGetValue(out value);
                slExp.Value = value;
            }
            else
            {
                slExp.IsEnabled = false;
            }
            if (videoDevController.Exposure.Capabilities.AutoModeSupported)
            {
                videoDevController.Exposure.TryGetAuto(out bAuto);
                tsExpAuto.IsOn = bAuto;
            }
            else
            {
                tsExpAuto.IsOn = false;
                tsExpAuto.IsEnabled = false;
            }

            if (lcWrapper != null)
            {
                slEffect.Minimum = 0;
                slEffect.Maximum = 100;
                slEffect.StepFrequency = 1;

                DspSettings dspSettings = lcWrapper.GetDspSetting();
                slEffect.Value = dspSettings.percentOfScreen;

                if (dspSettings.isEnabled == 1)
                {
                    tsEffectEnabled.IsOn = true;
                }
                else
                {
                    tsEffectEnabled.IsOn = false;
                    slEffect.IsEnabled = false;
                }
            }
            else
            {
                tsEffectEnabled.IsEnabled = false;
                slEffect.IsEnabled = false;
            }
        }

        protected void OnBrtAutoToggleChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Brightness.TrySetAuto(tsBrtAuto.IsOn);
            slBrt.IsEnabled = !tsBrtAuto.IsOn;
        }

        protected void OnBrtSliderValueChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Brightness.TrySetValue(slBrt.Value);
        }

        protected void OnCrtAutoToggleChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Contrast.TrySetAuto(tsCrtAuto.IsOn);
            slCrt.IsEnabled = !tsCrtAuto.IsOn;
        }

        protected void OnCrtSliderValueChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Contrast.TrySetValue(slCrt.Value);
        }

        protected void OnFocusAutoToggleChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Focus.TrySetAuto(tsFocusAuto.IsOn);
            slFocus.IsEnabled = !tsFocusAuto.IsOn;
        }

        protected void OnFocusSliderValueChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Focus.TrySetValue(slFocus.Value);
        }

        protected void OnExpAutoToggleChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Exposure.TrySetAuto(tsExpAuto.IsOn);
            slExp.IsEnabled = !tsExpAuto.IsOn;
        }

        protected void OnExpSliderValueChanged(object sender, RoutedEventArgs e)
        {
            videoDevController.Exposure.TrySetValue(slExp.Value);
        }

        protected void OnEffectEnabledToggleChanged(object sender, RoutedEventArgs e)
        {
            if (tsEffectEnabled.IsOn)
            {
                lcWrapper.Enable();
            }
            else
            {
                lcWrapper.Disable();
            }
            slEffect.IsEnabled = tsEffectEnabled.IsOn;
        }

        protected void OnEffectSliderValueChanged(object sender, RoutedEventArgs e)
        {
            lcWrapper.UpdateDsp(Convert.ToInt32(slEffect.Value));
        }
    }
}
