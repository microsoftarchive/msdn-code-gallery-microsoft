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
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace EdgeGestureSample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        #region Class Variables
        private EdgeGesture edgeGesture;
        #endregion

        public Scenario2()
        {
            this.InitializeComponent();
            this.edgeGesture = EdgeGesture.GetForCurrentView();
            Scenario2Init();
        }

        #region EdgeGesture Event Handlers
        private void OnContextMenu(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            Scenario2OutputText.Text = "Invoked with right-click.";
        }

        void EdgeGesture_Canceled(object sender, EdgeGestureEventArgs e)
        {
            Scenario2OutputText.Text = "Canceled with touch.";
        }

        void EdgeGesture_Completed(object sender, EdgeGestureEventArgs e)
        {
            if (e.Kind == EdgeGestureKind.Touch)
            {
                Scenario2OutputText.Text = "Invoked with touch.";
            }
            else if (e.Kind == EdgeGestureKind.Keyboard)
            {
                Scenario2OutputText.Text = "Invoked with keyboard.";
            }
            else if (e.Kind == EdgeGestureKind.Mouse)
            {
                Scenario2OutputText.Text = "Invoked with right-click.";
            }
        }

        void EdgeGesture_Starting(object sender, EdgeGestureEventArgs e)
        {
            Scenario2OutputText.Text = "Invoking with touch.";
        }

        void RightClickOverride(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            Scenario2OutputText.Text = "The ContextMenu event was handled.  The EdgeGesture event will not fire.";
            e.Handled = true;
        }

        void Scenario2Init()
        {
            rootPage.RightTapped += new RightTappedEventHandler(this.OnContextMenu);
            this.edgeGesture.Canceled += new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Canceled);
            this.edgeGesture.Completed += new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Completed);
            this.edgeGesture.Starting += new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Starting);
            RightClickRegion.RightTapped += new RightTappedEventHandler(this.RightClickOverride);

            Scenario2OutputText.Text = "Sample initialized and events registered.";
        }

        void Scenario2Reset()
        {
            rootPage.RightTapped -= new RightTappedEventHandler(this.OnContextMenu);
            this.edgeGesture.Canceled -= new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Canceled);
            this.edgeGesture.Completed -= new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Completed);
            this.edgeGesture.Starting -= new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Starting);
            RightClickRegion.RightTapped -= new RightTappedEventHandler(this.RightClickOverride);
        }
        #endregion

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
