//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using GesturesApp.Controls;
using System;
using System.Collections.Generic;

namespace GesturesApp.Pages
{
    public sealed partial class RotatePage : GesturePageBase
    {
        private Dictionary<Windows.UI.Xaml.UIElement, Manipulations.ManipulationManager> _manipulationManager;
        private Windows.UI.Xaml.Controls.Button _resetButton;

        public RotatePage() :
            base(
                "Rotate",
                "Turn to rotate",
                "Put two or more fingers on an object and turn your hand to rotate it. An object must be coded to support rotation for this to work.",
                "Similar to when you use Shift+Ctrl+scrollwheel on a mouse.",
                "Assets/rotate.png")
        {
            this.InitializeComponent();

            // Configure the app bar items for this page
            // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.

            // Links button
            this._links["Doc: Guidelines for rotation"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465315.aspx");
            this._links["API: Pivot center property"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.gesturerecognizer.pivotcenter.aspx");
            this._links["API: Pivot radius property"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.gesturerecognizer.pivotradius.aspx");
            this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));

            // Reset button
            this._resetButton = new Windows.UI.Xaml.Controls.Button
            {
                Style = App.Current.Resources["ResetAppBarButtonStyle"] as Windows.UI.Xaml.Style
            };
            this._resetButton.Click += (object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            {
                this._manipulationManager[this.leftImage].ResetManipulation();
                this._manipulationManager[this.rightImage].ResetManipulation();
            };

            this._manipulationManager = new Dictionary<Windows.UI.Xaml.UIElement, Manipulations.ManipulationManager>();       
        }

        protected override void OnSelected()
        {
            base.OnSelected();

            this._contextualItemsPanel.Children.Add(this._resetButton);
        }

        protected override void OnUnselected()
        {
            base.OnUnselected();

            this._contextualItemsPanel.Children.Remove(this._resetButton);
        }

        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            var canvas = sender as Windows.UI.Xaml.Controls.Canvas;

            // If resized object is a canvas, update clipping geometry to its new size
            if (canvas != null)
            {
                canvas.Clip = new Windows.UI.Xaml.Media.RectangleGeometry
                {
                    Rect = new Windows.Foundation.Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight)
                };
            }
        }

        private void OnImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var image = sender as Windows.UI.Xaml.Controls.Image;
            var canvas = image.Parent as Windows.UI.Xaml.Controls.Canvas;

            // Make the image 70% of the canvas and position it at the center of the canvas
            var scale = 0.7 * System.Math.Min(
                (canvas.ActualHeight / image.ActualHeight),
                (canvas.ActualWidth / image.ActualWidth));
            image.RenderTransform = new Windows.UI.Xaml.Media.TransformGroup
            {
                Children = {                 
                    new Windows.UI.Xaml.Media.TranslateTransform
                    {
                        X = (canvas.ActualWidth - image.ActualWidth) / 2,
                        Y = (canvas.ActualHeight - image.ActualHeight) / 2
                    },
                    new Windows.UI.Xaml.Media.ScaleTransform
                    {
                        CenterX = canvas.ActualWidth / 2,
                        CenterY = canvas.ActualHeight / 2,
                        ScaleX = scale,
                        ScaleY = scale
                    },  
                }
            };

            // Create and configure manipulation manager for this image
            // leftImage can only be rotated, while rightImage can also be translated
            var manManager = new Manipulations.ManipulationManager(image, canvas);
            if (image == leftImage)
            {
                manManager.OnFilterManipulation = Manipulations.ManipulationFilter.RotateAboutCenter;
                manManager.Configure(false, true, false, true);
            }
            else if (image == rightImage)
            {
                manManager.OnFilterManipulation = Manipulations.ManipulationFilter.ClampCenterOfMass;
                manManager.Configure(false, true, true, true);
            }
            else
            {
                // Should never happen
                throw new System.Exception("RotatePage.OnImageOpened");
            }            
            this._manipulationManager[image] = manManager;
        }
    }
}
