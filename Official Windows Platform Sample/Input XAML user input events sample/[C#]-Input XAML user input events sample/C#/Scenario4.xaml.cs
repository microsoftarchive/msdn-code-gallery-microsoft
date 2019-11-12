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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.Foundation;

namespace Input
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private TransformGroup _transformGroup;
        private MatrixTransform _previousTransform;
        private CompositeTransform _compositeTransform;
        private bool forceManipulationsToEnd;

        public Scenario4()
        {
            this.InitializeComponent();
            forceManipulationsToEnd = false;
            ManipulateMe.ManipulationStarting += new ManipulationStartingEventHandler(ManipulateMe_ManipulationStarting);
            ManipulateMe.ManipulationStarted += new ManipulationStartedEventHandler(ManipulateMe_ManipulationStarted);
            ManipulateMe.ManipulationDelta += new ManipulationDeltaEventHandler(ManipulateMe_ManipulationDelta);
            ManipulateMe.ManipulationCompleted += new ManipulationCompletedEventHandler(ManipulateMe_ManipulationCompleted);
            ManipulateMe.ManipulationInertiaStarting += new ManipulationInertiaStartingEventHandler(ManipulateMe_ManipulationInertiaStarting);
            InitManipulationTransforms();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void InitManipulationTransforms()
        {
            _transformGroup = new TransformGroup();
            _compositeTransform = new CompositeTransform();
            _previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };

            _transformGroup.Children.Add(_previousTransform);
            _transformGroup.Children.Add(_compositeTransform);

            ManipulateMe.RenderTransform = _transformGroup;
        }

        void ManipulateMe_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            forceManipulationsToEnd = false;
            e.Handled = true;
        }

        void ManipulateMe_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void ManipulateMe_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void ManipulateMe_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (forceManipulationsToEnd)
            {
                e.Complete();
                return;
            }

            _previousTransform.Matrix = _transformGroup.Value;

            Point center = _previousTransform.TransformPoint(new Point(e.Position.X, e.Position.Y));
            _compositeTransform.CenterX = center.X;
            _compositeTransform.CenterY = center.Y;

            _compositeTransform.Rotation = e.Delta.Rotation;
            _compositeTransform.ScaleX = _compositeTransform.ScaleY = e.Delta.Scale;
            _compositeTransform.TranslateX = e.Delta.Translation.X;
            _compositeTransform.TranslateY = e.Delta.Translation.Y;

            e.Handled = true;
        }

        void ManipulateMe_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void Scenario4Reset(object sender, RoutedEventArgs e)
        {
            Scenario4Reset();
        }

        void Scenario4Reset()
        {
            forceManipulationsToEnd = true;
            ManipulateMe.RenderTransform = null;
            InitManipulationTransforms();
        }
    }
}
