//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using SDKTemplate;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DisplayOrientation
{
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        private double rotationAngle = 0.0;
        private const double toDegrees = 180.0 / Math.PI;
        Accelerometer accelerometer;

        public Scenario1()
        {
            InitializeComponent();
            accelerometer = Accelerometer.GetDefault();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceRotation.Text = rotationAngle.ToString();

            if (accelerometer != null)
            {
                accelerometer.ReadingChanged += CalculateDeviceRotation;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (accelerometer != null)
            {
                accelerometer.ReadingChanged -= CalculateDeviceRotation;
            }
        }

        private async void CalculateDeviceRotation(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            // Compute the rotation angle based on the accelerometer's position
            var angle = Math.Atan2(args.Reading.AccelerationY, args.Reading.AccelerationX) * toDegrees;
            
            // Since our arrow points upwards insted of the right, we rotate the coordinate system by 90 degrees
            angle += 90;

            // Ensure that the range of the value is between [0, 360)
            if (angle < 0)
            {
                angle += 360;
            }

            rotationAngle = angle;

            // Update the UI with the new value
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    deviceRotation.Text = rotationAngle.ToString();
                });

            UpdateArrowForRotation();
        }

        private async void UpdateArrowForRotation()
        {
            // Obtain current rotation taking into account a Landscape first or a Portrait first device
            var screenRotation = 0;

            // Native orientation can only be Landscape or Portrait
            if (DisplayProperties.NativeOrientation == DisplayOrientations.Landscape)
            {
                switch (DisplayProperties.CurrentOrientation)
                {
                    case DisplayOrientations.Landscape:
                        screenRotation = 0;
                        break;

                    case DisplayOrientations.Portrait:
                        screenRotation = 90;
                        break;

                    case DisplayOrientations.LandscapeFlipped:
                        screenRotation = 180;
                        break;

                    case DisplayOrientations.PortraitFlipped:
                        screenRotation = 270;
                        break;

                    default:
                        screenRotation = 0;
                        break;
                }
            }
            else
            {
                switch (DisplayProperties.CurrentOrientation)
                {
                    case DisplayOrientations.Landscape:
                        screenRotation = 270;
                        break;

                    case DisplayOrientations.Portrait:
                        screenRotation = 0;
                        break;

                    case DisplayOrientations.LandscapeFlipped:
                        screenRotation = 90;
                        break;

                    case DisplayOrientations.PortraitFlipped:
                        screenRotation = 180;
                        break;

                    default:
                        screenRotation = 270;
                        break;
                }
            }

            double steeringAngle = rotationAngle - screenRotation;

            // Keep the steering angle positive             
            if (steeringAngle < 0)
            {
                steeringAngle += 360;
            }

            // Update the UI based on steering action
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    RotateTransform transform = new RotateTransform();
                    transform.Angle = -steeringAngle;
                    transform.CenterX = scenario1Image.ActualWidth / 2;
                    transform.CenterY = scenario1Image.ActualHeight / 2;

                    scenario1Image.RenderTransform = transform;
                });
        }
    }
}
