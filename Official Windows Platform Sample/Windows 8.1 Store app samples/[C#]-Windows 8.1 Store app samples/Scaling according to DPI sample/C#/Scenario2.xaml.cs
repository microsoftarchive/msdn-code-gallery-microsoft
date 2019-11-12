//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;

using System;
using Windows.Graphics.Display;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Scaling
{
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        private static FontFamily defaultFontFamily = new FontFamily("Segoe UI");
        private static FontFamily overrideFontFamily = new FontFamily("Segoe Script");

        public Scenario2()
        {
            InitializeComponent();
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
            displayInformation.DpiChanged += DisplayProperties_DpiChanged;
            DefaultLayoutText.FontSize = PxFromPt(20);  // xaml fontsize is in pixels.
            DefaultLayoutText.FontFamily = defaultFontFamily;
        }

        // Helpers to convert between points and pixels.
        double PtFromPx(double pixel)
        {
            return pixel * 72 / 96;
        }

        double PxFromPt(double pt)
        {
            return pt * 96 / 72;
        }

        void SetOverrideRectSize(double sizeInPhysicalPx, double scaleFactor)
        {
            // Set the size of OverrideLayoutRect based on the desired size in physical pixels and the scale factor.
            // The code here is to demonstrate how to override default scaling behavior to keep the physical pixel size of a control.
            double sizeInRelativePx = sizeInPhysicalPx / scaleFactor;
            OverrideLayoutRect.Width = sizeInRelativePx;
            OverrideLayoutRect.Height = sizeInRelativePx;
        }

        void SetOverrideTextFont(double size, FontFamily fontFamily)
        {
            OverrideLayoutText.FontSize = PxFromPt(size);  // xaml fontsize is in pixels.
            OverrideLayoutText.FontFamily = fontFamily;
        }

        void OutputSettings(double scaleFactor, FrameworkElement rectangle, TextBlock relativePxText, TextBlock physicalPxText, TextBlock fontTextBlock)
        {
            // Get the size of the rectangle in relative pixels and calulate the size in physical pixels.
            double sizeInRelativePx = rectangle.Width;
            double sizeInPhysicalPx = sizeInRelativePx * scaleFactor;

            relativePxText.Text = sizeInRelativePx.ToString("F1") + " relative px";
            physicalPxText.Text = sizeInPhysicalPx.ToString("F0") + " physical px";

            double fontSize = PtFromPx(fontTextBlock.FontSize);
            fontTextBlock.Text = fontSize.ToString("F0") + "pt " + fontTextBlock.FontFamily.Source;
        }

        void ResetOutput()
        {
            ResolutionTextBlock.Text = Window.Current.Bounds.Width.ToString("F1") + "x" + Window.Current.Bounds.Height.ToString("F1");

            double scaleFactor;
            double fontSize;
            FontFamily fontFamily;
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
            switch (displayInformation.ResolutionScale)
            {
                case ResolutionScale.Invalid:
                case ResolutionScale.Scale100Percent:
                default:
                    scaleFactor = 1.0;
                    fontSize = 20;
                    fontFamily = defaultFontFamily;
                    break;

                case ResolutionScale.Scale140Percent:
                    scaleFactor = 1.4;
                    fontSize = 11;
                    fontFamily = overrideFontFamily;
                    break;

                case ResolutionScale.Scale180Percent:
                    scaleFactor = 1.8;
                    fontSize = 9;
                    fontFamily = overrideFontFamily;
                    break;
            }

            // Set the override rectangle size and override text font.
            const double rectSizeInPhysicalPx = 100;
            SetOverrideRectSize(rectSizeInPhysicalPx, scaleFactor);
            SetOverrideTextFont(fontSize, fontFamily);

            // Output settings for controls with default scaling behavior.
            OutputSettings(scaleFactor, DefaultLayoutRect, DefaultRelativePx, DefaultPhysicalPx, DefaultLayoutText);
            // Output settings for override controls.
            OutputSettings(scaleFactor, OverrideLayoutRect, OverrideRelativePx, OverridePhysicalPx, OverrideLayoutText);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ResetOutput();
        }

        void DisplayProperties_DpiChanged(DisplayInformation sender, object args)
        {
            ResetOutput();
        }
    }
}
