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

namespace SwapChainPanel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();
            rootPage.SizeChanged += MainPage_SizeChanged;
            ParagraphText.SizeChanged += ParagraphText_SizeChanged;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateOutputLayout();
            HighlighterPanel.StartProcessingInput();
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HighlighterPanel.StopProcessingInput();
        }

        /// <summary>
        /// Event handler for the main page SizeChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOutputLayout();
        }

        /// <summary>
        /// Updates size and position of elements on the page when the size changes.
        /// </summary>
        private void UpdateOutputLayout()
        {
            Output.Width = (MainPage.Current.FindName("ContentRoot") as FrameworkElement).ActualWidth - (ParagraphBorder.BorderThickness.Left + ParagraphBorder.BorderThickness.Right);
        }

        /// <summary>
        /// Event handler for the ParagraphText SizeChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ParagraphText_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HighlighterPanel.Width = e.NewSize.Width;
            HighlighterPanel.Height = e.NewSize.Height;
        }
    }
}
