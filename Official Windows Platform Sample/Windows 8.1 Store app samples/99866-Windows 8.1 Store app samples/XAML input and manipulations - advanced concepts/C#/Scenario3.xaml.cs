//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
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

namespace AdvancedManipulations
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        //private variables
        private TransformGroup transformGroup;
        private MatrixTransform previousTransform;
        private CompositeTransform compositeTransform;
        private bool forceManipulationsToEnd;

        public Scenario3()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ManipulateMe.ManipulationStarting += new ManipulationStartingEventHandler(ManipulateMe_ManipulationStarting);
            ManipulateMe.ManipulationStarted += new ManipulationStartedEventHandler(ManipulateMe_ManipulationStarted);
            ManipulateMe.ManipulationDelta += new ManipulationDeltaEventHandler(ManipulateMe_ManipulationDelta);
            ManipulateMe.ManipulationCompleted += new ManipulationCompletedEventHandler(ManipulateMe_ManipulationCompleted);
            ManipulateMe.ManipulationInertiaStarting += new ManipulationInertiaStartingEventHandler(ManipulateMe_ManipulationInertiaStarting);
            Scenario3Initialize();
        }

        private void mixManipCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mixManipCombo != null)
            {
                switch (mixManipCombo.SelectedIndex)
                {
                    case 0: //Default
                    default:
                        ManipulateMe.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.System;
                        outputText.Text = "When ManipulationMode = System, you can pan the canvas by touching anywhere inside it. The blue rectangle does not respond in any specific way to touch input";
                        break;
                    case 1: //TranslateX , System
                        ManipulateMe.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.System | Windows.UI.Xaml.Input.ManipulationModes.TranslateX;
                        outputText.Text = "Now, the rectangle gets the custom TranslateX components which can be used to move the rectangle along the horizontal direction. You can also pan the canvas in the vertical direction by touching the rectangle or anywhere in the canvas.";
                        break;
                    case 2: //TranslateY , System
                        ManipulateMe.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.System | Windows.UI.Xaml.Input.ManipulationModes.TranslateY;
                        outputText.Text = "Now, the rectangle gets the custom TranslateY components which can be used to move the rectangle along the vertical direction. You can also pan the canvas in the horizontal direction by touching the rectangle or anywhere in the canvas.";
                        break;
                    case 3: // System , TranslateX , TranslateY
                        ManipulateMe.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.System | Windows.UI.Xaml.Input.ManipulationModes.TranslateX | Windows.UI.Xaml.Input.ManipulationModes.TranslateY;
                        outputText.Text = "Now, the rectangle gets the custom TranslateX and TranslateY components which can be used to move the rectangle along the horizontal and vertical direction. You can also pinch-to-zoom the canvas by touching the rectangle or anywhere in the canvas.";
                        break;
                }
            }
        }

        void Scenario3Initialize()
        {
            //Setup with defaults
            mixManipCombo.SelectedIndex = 0;
            forceManipulationsToEnd = true;
            ManipulateMe.RenderTransform = null;
            InitManipulationTransforms();

	    //Initialize the scrollviewer to origin and min zoom factor
            scrollViewer.ChangeView(0, 0, 1.0F);
	    //Setup the rectangle to original sizes
            ManipulateMe.Height = 200;
            ManipulateMe.Width = 200;

            outputText.Text = "When ManipulationMode = System, you can pan the canvas by touching anywhere inside it. The blue rectangle does not respond in any specific way to touch input";
        }

        private void InitManipulationTransforms()
        {
            //Initialize the transforms
            transformGroup = new TransformGroup();
            compositeTransform = new CompositeTransform();
            previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };

            transformGroup.Children.Add(previousTransform);
            transformGroup.Children.Add(compositeTransform);

            ManipulateMe.RenderTransform = transformGroup;
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
            //Set the new transform values based on user action
            previousTransform.Matrix = transformGroup.Value;
            compositeTransform.TranslateX = e.Delta.Translation.X / scrollViewer.ZoomFactor;
            compositeTransform.TranslateY = e.Delta.Translation.Y / scrollViewer.ZoomFactor;
            e.Handled = true;
        }

        void ManipulateMe_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
        }

    }
}
