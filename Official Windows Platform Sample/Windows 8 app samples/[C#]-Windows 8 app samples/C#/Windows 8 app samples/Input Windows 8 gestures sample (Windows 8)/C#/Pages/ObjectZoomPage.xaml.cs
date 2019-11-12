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

namespace GesturesApp.Pages
{
    public sealed partial class ObjectZoomPage : GesturePageBase
    {
        private Manipulations.ManipulationManager _manipulationManager;
        private Windows.UI.Xaml.Controls.Button _resetButton;

        public ObjectZoomPage() :
            base(
                "ObjectZoom",
                "Pinch and stretch to zoom",
                "Pinch or stretch two or more fingers on the screen to resize that object. If an object also supports panning, the user should be able to zoom and pan at the same time.",
                "Similar to when you use Ctrl+scrollwheel on a mouse.",
                "Assets/pinch.png")
        {
            this.InitializeComponent();

            // Configure the app bar items for this page
            // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.
            
            // Links button
            this._links["Doc: Guidelines for optical zoom and resizing"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465307.aspx");
            this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));

            // Reset button
            this._resetButton = new Windows.UI.Xaml.Controls.Button
            {
                Style = App.Current.Resources["ResetAppBarButtonStyle"] as Windows.UI.Xaml.Style
            };
            this._resetButton.Click += (object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            {
                this._manipulationManager.ResetManipulation();
            };
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

            // If resized object is a canvas, update clipping geometry to its new size.
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

            // Make the image 75% of the canvas and position it at the center of the canvas
            var scale = 0.75 * System.Math.Min(
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

            this._manipulationManager = new Manipulations.ManipulationManager(image, canvas);
            this._manipulationManager.OnFilterManipulation = Manipulations.ManipulationFilter.Clamp;
            this._manipulationManager.Configure(true, false, true, true);
        }
    }
}
