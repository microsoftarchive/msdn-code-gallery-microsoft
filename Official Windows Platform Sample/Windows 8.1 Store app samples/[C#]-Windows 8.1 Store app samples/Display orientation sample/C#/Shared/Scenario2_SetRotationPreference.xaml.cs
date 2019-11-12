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
    public sealed partial class Scenario2 : Page
    {
        private double m_rotationAngle = 0.0;
        Accelerometer m_accelerometer;

        public Scenario2()
        {
            InitializeComponent();
            m_accelerometer = Accelerometer.GetDefault();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceRotation.Text = m_rotationAngle.ToString();

            if (m_accelerometer != null)
            {
                m_accelerometer.ReadingChanged += CalculateDeviceRotation;
            }

            if (DisplayInformation.AutoRotationPreferences == DisplayOrientations.None)
            {
                LockButton.Content = "Lock";
            }
            else
            {
                LockButton.Content = "Unlock";
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If the navigation is external to the app don't deregister the accelerometer.
            // This can occur on Phone when suspending the app.
            if (e.NavigationMode == NavigationMode.Forward && e.Uri == null)
            {
                return;
            }

            if (m_accelerometer != null)
            {
                m_accelerometer.ReadingChanged -= CalculateDeviceRotation;
            }
        }

        private void Scenario2Button_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayInformation.AutoRotationPreferences == DisplayOrientations.None)
            {
                // Get the current screen orientation and set it as the preference.
                DisplayInformation.AutoRotationPreferences = DisplayInformation.GetForCurrentView().CurrentOrientation;
                LockButton.Content = "Unlock";
            }
            else
            {
                // Reset to no preference.
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
                LockButton.Content = "Lock";
            }
        }

        /// <summary>
        /// Compute the difference, in degrees, between the device's orientation and the up direction (against gravity).
        /// We only take into account the X and Y dimensions, i.e. device screen is perpendicular to the ground.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void CalculateDeviceRotation(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            m_rotationAngle = Constants.UIAngleOffset -
                Math.Atan2(args.Reading.AccelerationY, args.Reading.AccelerationX) * 180.0 / Math.PI;

            // Ensure that the range of the value is within [0, 360).
            if (m_rotationAngle >= 360)
            {
                m_rotationAngle -= 360;
            }

            // Update the UI with the new value.
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    deviceRotation.Text = Math.Floor(m_rotationAngle).ToString();
                    UpdateArrowForRotation();
                });
        }

        /// <summary>
        /// Rotate the UI arrow image to point up, adjusting for the accelerometer and screen rotation.
        /// </summary>
        private void UpdateArrowForRotation()
        {
            double screenRotation = 0;

            // Adjust the UI steering angle to account for screen rotation.
            switch (DisplayInformation.GetForCurrentView().CurrentOrientation)
            {
                case DisplayOrientations.Landscape:
                    screenRotation = 0;
                    break;

                case DisplayOrientations.Portrait:
                    screenRotation = 270;
                    break;

                case DisplayOrientations.LandscapeFlipped:
                    screenRotation = 180;
                    break;

                case DisplayOrientations.PortraitFlipped:
                    screenRotation = 90;
                    break;

                default:
                    screenRotation = 0;
                    break;
            }

            double steeringAngle = m_rotationAngle - screenRotation;

            // Ensure the steering angle is positive.
            if (steeringAngle < 0)
            {
                steeringAngle += 360;
            }

            // Update the UI based on steering action.
            RotateTransform transform = new RotateTransform();
            transform.Angle = steeringAngle;
            transform.CenterX = scenario2Image.ActualWidth / 2;
            transform.CenterY = scenario2Image.ActualHeight / 2;
            scenario2Image.RenderTransform = transform;
        }
    }
}
