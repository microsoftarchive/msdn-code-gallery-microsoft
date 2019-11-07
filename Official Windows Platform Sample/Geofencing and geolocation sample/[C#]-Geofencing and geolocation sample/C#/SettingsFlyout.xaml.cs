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
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Geofencing4SqSample
{
    public enum SettingsType
    {
        QueryLimit = 0,
        GeofenceRadius,
        AutoCheckinEnabled
    }
    public sealed class SettingsChangedEventArgs
    {
        public SettingsType Type { get; set; }
        public Object NewValue { get; set; }
        public Object OldValue { get; set; }
    }
    public delegate void SettingsChangedEventHandler(SettingsChangedEventArgs e);

    public sealed class Settings
    {
        public static event SettingsChangedEventHandler Changed;

        public static uint[] QueryLimitOptions = { 10, 25, 50, 99 };
        public const string QueryLimitKey = "Foursquare.MaxQuerySize";
        public const int QueryIndexDefault = 1;
        public static uint QueryLimit
        {
            get
            {
                Object value = ApplicationData.Current.LocalSettings.Values[QueryLimitKey];
                return (null == value) ? QueryLimitOptions[QueryIndexDefault] : (uint)value;
            }
            set
            {
                Object oldValue = ApplicationData.Current.LocalSettings.Values[QueryLimitKey];
                bool changed = (null == oldValue || ((uint)value != (uint)oldValue));

                if (changed)
                {
                    ApplicationData.Current.LocalSettings.Values[QueryLimitKey] = value;
                    FireChanged(oldValue, value, SettingsType.QueryLimit);
                }
            }
        }

        public const string AutoCheckinEnabledKey = "Foursquare.AutoCheckinEnabled";
        public const bool AutoCheckinEnabledDefault = false;
        public static bool AutoCheckinEnabled
        {
            get
            {
                Object value = ApplicationData.Current.LocalSettings.Values[AutoCheckinEnabledKey];
                return (null == value) ? AutoCheckinEnabledDefault : (bool)value;
            }
            set
            {
                Object oldValue = ApplicationData.Current.LocalSettings.Values[AutoCheckinEnabledKey];
                bool changed = (null == oldValue || ((bool)value != (bool)oldValue));

                if (changed)
                {
                    ApplicationData.Current.LocalSettings.Values[AutoCheckinEnabledKey] = value;
                    FireChanged(oldValue, value, SettingsType.AutoCheckinEnabled);
                }
            }
        }
        public static bool AutoCheckinStateSet
        {
            get
            {
                Object value = ApplicationData.Current.LocalSettings.Values[AutoCheckinEnabledKey];
                return (null != value);
            }
        }
        public static void ResetAutoCheckinState()
        {
            ApplicationData.Current.LocalSettings.Values[AutoCheckinEnabledKey] = null;
        }

        public const string GeofenceRadiusKey = "Geofencing.RadiusMeters";
        public const double GeofenceRadiusDefault = 25.0;
        public static double GeofenceRadiusMeters
        {
            get
            {
                Object value = ApplicationData.Current.LocalSettings.Values[GeofenceRadiusKey];
                return (null == value) ? GeofenceRadiusDefault : (double)value;
            }
            set
            {
                Object oldValue = ApplicationData.Current.LocalSettings.Values[GeofenceRadiusKey];
                bool changed = (null == oldValue || ((double)value != (double)oldValue));

                if (changed)
                {
                    ApplicationData.Current.LocalSettings.Values[GeofenceRadiusKey] = value;
                    FireChanged(oldValue, value, SettingsType.GeofenceRadius);
                }
            }
        }

        private static void FireChanged(Object oldValue, Object newValue, SettingsType type)
        {
            if (Changed != null)
            {
                var args = new SettingsChangedEventArgs()
                {
                    Type = type,
                    OldValue = oldValue,
                    NewValue = newValue
                };
                Changed(args);
            }
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsFlyout : Page
    {
        // The guidelines recommend using 100px offset for the content animation.
        private const int _ContentAnimationOffset = 100;

        public SettingsFlyout()
        {
            this.InitializeComponent();
            FlyoutContent.Transitions = new TransitionCollection();
            FlyoutContent.Transitions.Add(new EntranceThemeTransition()
            {
                FromHorizontalOffset = (SettingsPane.Edge == SettingsEdgeLocation.Right) ? _ContentAnimationOffset : (_ContentAnimationOffset * -1)
            });
            LoadSettings();
        }

        /// <summary>
        /// This is the click handler for the back button on the Flyout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsBack_Clicked(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            // First close our Flyout.
            Popup parent = this.Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            SettingsPane.Show();
        }

        /// <summary>
        /// This is the click handler for the Reset to defaults button on the Flyout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetDefaults_Clicked(object sender, RoutedEventArgs e)
        {
            double radius = Settings.GeofenceRadiusDefault;
            SetRadiusSliderText(radius);
            RadiusSlider.Value = radius;

            QueryLimitCombo.SelectedIndex = Settings.QueryIndexDefault;

            Settings.ResetAutoCheckinState();
            AutoCheckinToggleSwitch.IsOn = Settings.AutoCheckinEnabledDefault;
        }

        /// <summary>
        /// This is the value changed handler for the Radius Slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadiusSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetRadiusSliderText(e.NewValue);
        }

        private void SetRadiusSliderText(double radius)
        {
            string msg = String.Format("{0} meters", radius);
            if (RadiusSliderText != null)
            {
                RadiusSliderText.Text = msg;
            }
        }

        /// <summary>
        /// This is the toggled handler for the Auto-checkin Toggle Switch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoCheckinToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var autoCheckinToggleSwitch = sender as ToggleSwitch;
            Settings.AutoCheckinEnabled = autoCheckinToggleSwitch.IsOn;
        }

        private void LoadSettings()
        {
            double radius = Settings.GeofenceRadiusMeters;
            SetRadiusSliderText(radius);
            RadiusSlider.Value = radius;

            uint queryLimit = Settings.QueryLimit;
            for (int i = 0; i < Settings.QueryLimitOptions.Length; i++)
            {
                if (queryLimit == Settings.QueryLimitOptions[i])
                {
                    QueryLimitCombo.SelectedIndex = i;
                    break;
                }
            }

            AutoCheckinToggleSwitch.IsOn = Settings.AutoCheckinEnabled;
        }

        public void SaveSettings()
        {
            Settings.GeofenceRadiusMeters = RadiusSlider.Value;
            Settings.QueryLimit = Settings.QueryLimitOptions[QueryLimitCombo.SelectedIndex];
        }
    }
}
