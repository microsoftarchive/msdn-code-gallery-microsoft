// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using ViewManagement = Windows.UI.ViewManagement;

namespace HighContrast
{
    partial class TargetButton : Button
    {
        public TargetButton()
        {
            InitializeComponent();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Apply correct coloring when in High Contrast

            ViewManagement.AccessibilitySettings accessibilitySettings = new ViewManagement.AccessibilitySettings();
            if (!(accessibilitySettings.HighContrast) /*Off*/)
            {
                // Use default colors

                this.Background = new SolidColorBrush(Colors.Red);
                this.BorderBrush = new SolidColorBrush(Colors.Black);

                this.Circle4.Fill = new SolidColorBrush(Colors.Blue);
                this.Circle3.Fill = new SolidColorBrush(Colors.Green);
                this.Circle2.Fill = new SolidColorBrush(Colors.Yellow);
                this.Circle1.Fill = new SolidColorBrush(Colors.White);
                this.Circle4.Stroke = this.Circle3.Stroke = this.Circle2.Stroke = this.Circle1.Stroke = new SolidColorBrush(Colors.Black);
            }
            else
            {
                // Use High Contrast Colors

                ViewManagement.UISettings uiSettings = new ViewManagement.UISettings();

                switch (accessibilitySettings.HighContrastScheme)
                {
                    case "High Contrast Black":
                        this.Background = this.Circle4.Fill = this.Circle3.Fill = this.Circle2.Fill = this.Circle1.Fill = new SolidColorBrush(Colors.Black);
                        this.BorderBrush = this.Circle4.Stroke = this.Circle3.Stroke = this.Circle2.Stroke = this.Circle1.Stroke = new SolidColorBrush(Colors.White);
                        break;

                    case "High Contrast White":
                        this.Background = this.Circle4.Fill = this.Circle3.Fill = this.Circle2.Fill = this.Circle1.Fill = new SolidColorBrush(Colors.White);
                        this.BorderBrush = this.Circle4.Stroke = this.Circle3.Stroke = this.Circle2.Stroke = this.Circle1.Stroke = new SolidColorBrush(Colors.Black);
                        break;

                    default: // For all other High Contrast schemes
                        this.Background = new SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.ButtonFace));
                        this.BorderBrush = new SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.ButtonText));
                        this.Circle4.Fill = this.Circle3.Fill = this.Circle2.Fill = this.Circle1.Fill = new SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.Hotlight));
                        this.Circle4.Stroke = this.Circle3.Stroke = this.Circle2.Stroke = this.Circle1.Stroke = new SolidColorBrush(uiSettings.UIElementColor(ViewManagement.UIElementType.HighlightText));
                        break;
                }


            }
        }
    }
}
