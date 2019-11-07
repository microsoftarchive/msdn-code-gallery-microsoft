//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Animation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
        {
            this.InitializeComponent();

            Scenario3FunctionSelector.SelectionChanged += Scenario3EasingFunctionChanged;
            Scenario3EasingModeSelector.SelectionChanged += Scenario3EasingFunctionChanged;
        }

        void rootPage_MainPageResized(object sender, MainPageSizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width < 768)
            {
                    InputPanel.Orientation = Orientation.Vertical;
                    FunctionPanel.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
            }
            else
            {
                    InputPanel.Orientation = Orientation.Horizontal;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage.MainPageResized += rootPage_MainPageResized;

            Scenario3FunctionSelector.SelectedIndex = 0;
            Scenario3EasingModeSelector.SelectedIndex = 0;
        }

        /// <summary>
        /// Invoked when this page is about to be navigated away from in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.MainPageResized -= rootPage_MainPageResized;
        }

        private void Scenario3EasingFunctionChanged(object sender, SelectionChangedEventArgs e)
        {
            // stop the storyboard
            Scenario3Storyboard.Stop();

            EasingFunctionBase easingFunction = null;

            // select an easing function based on the user's selection
            ComboBoxItem selectedFunctionItem = Scenario3FunctionSelector.SelectedItem as ComboBoxItem;
            if (selectedFunctionItem != null)
            {
                switch (selectedFunctionItem.Content.ToString())
                {
                    case "BounceEase":
                        easingFunction = new BounceEase();
                        break;
                    case "CircleEase":
                        easingFunction = new CircleEase();
                        break;
                    case "ExponentialEase":
                        easingFunction = new ExponentialEase();
                        break;
                    case "PowerEase":
                        easingFunction = new PowerEase() { Power = 0.5 };
                        break;
                    default:
                        break;
                }
            }

            // if no valid easing function was specified, let the storyboard stay stopped and do not continue
            if (easingFunction == null)
                return;

            ComboBoxItem selectedEasingModeItem = Scenario3EasingModeSelector.SelectedItem as ComboBoxItem;
            // select an easing mode based on the user's selection, defaulting to EaseIn if no selection was given
            if (selectedEasingModeItem != null)
            {
                switch (selectedEasingModeItem.Content.ToString())
                {
                    case "EaseOut":
                        easingFunction.EasingMode = EasingMode.EaseOut;
                        break;
                    case "EaseInOut":
                        easingFunction.EasingMode = EasingMode.EaseInOut;
                        break;
                    default:
                        easingFunction.EasingMode = EasingMode.EaseIn;
                        break;
                }
            }

            // plot a graph of the easing function
            PlotEasingFunctionGraph(easingFunction, 0.005);

            RectanglePositionAnimation.EasingFunction = easingFunction;
            GraphPositionMarkerYAnimation.EasingFunction = easingFunction;

            // start the storyboard
            Scenario3Storyboard.Begin();
        }

        // Plots a graph of the passed easing function using the given sampling interval on the "Graph" Canvas control 
        private void PlotEasingFunctionGraph(EasingFunctionBase easingFunction, double samplingInterval)
        {
            UISettings UserSettings = new UISettings();
            Graph.Children.Clear();

            Path path = new Path();
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure() { StartPoint = new Point(0, 0) };
            PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();

            // Note that an easing function is just like a regular function that operates on doubles.
            // Here we plot the range of the easing function's output on the y-axis of a graph.
            for (double i = 0; i < 1; i += samplingInterval)
            {
                double x = i * GraphContainer.Width;
                double y = easingFunction.Ease(i) * GraphContainer.Height;

                LineSegment segment = new LineSegment();
                segment.Point = new Point(x, y);
                pathSegmentCollection.Add(segment);
            }

            pathFigure.Segments = pathSegmentCollection;
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            path.Stroke = new SolidColorBrush(UserSettings.UIElementColor(UIElementType.ButtonText));
            path.StrokeThickness = 1;

            // Add the path to the Canvas
            Graph.Children.Add(path);
        }

    }
}
