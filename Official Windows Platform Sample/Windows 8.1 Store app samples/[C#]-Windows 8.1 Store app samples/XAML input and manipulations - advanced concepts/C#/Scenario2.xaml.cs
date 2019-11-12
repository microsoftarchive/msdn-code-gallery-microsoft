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

namespace AdvancedManipulations
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
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
            scrollViewer.ViewChanging += scrollViewer_ViewChanging;
            scrollViewer.ViewChanged += scrollViewer_ViewChanged;
        }

        void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //Update the actual view values when the viewport comes to rest after a pan/zoom
            actualNextHOText.Text = scrollViewer.HorizontalOffset.ToString();
            actualNextVOText.Text = scrollViewer.VerticalOffset.ToString();
            actualNextZFText.Text = scrollViewer.ZoomFactor.ToString();

            if (e.IsIntermediate == false)
            {
                actualFinalHOText.Text = scrollViewer.HorizontalOffset.ToString();
                actualFinalVOText.Text = scrollViewer.VerticalOffset.ToString();
                actualFinalZFText.Text = scrollViewer.ZoomFactor.ToString();
            }
            else
            {
                actualFinalHOText.Text = String.Empty;
                actualFinalVOText.Text = String.Empty;
                actualFinalZFText.Text = String.Empty;
            }
        }

        void scrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            //Update the predicted next and final view values while a zoom/pan is occuring
            predictedNextHOText.Text = e.NextView.HorizontalOffset.ToString();
            predictedNextVOText.Text = e.NextView.VerticalOffset.ToString();
            predictedNextZFText.Text = e.NextView.ZoomFactor.ToString();

            predictedFinalHOText.Text = e.FinalView.HorizontalOffset.ToString();
            predictedFinalVOText.Text = e.FinalView.VerticalOffset.ToString();
            predictedFinalZFText.Text = e.FinalView.ZoomFactor.ToString();

            if (e.IsInertial)
            {
                isInertialOutputText.Text = "Inertial";
            }
            else
            {
                isInertialOutputText.Text = "Not Inertial";
            }

        }

    }
}
