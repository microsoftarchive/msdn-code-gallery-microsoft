//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using SDKTemplate;
using System;

namespace EdgeGestureSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        #region Class Variables
        private EdgeGesture edgeGesture;
        #endregion


        public Scenario1()
        {
            this.InitializeComponent();

            this.edgeGesture = EdgeGesture.GetForCurrentView();
            Scenario1Init();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #region EdgeGesture Event Handlers

        private void OnContextMenu(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            Scenario1OutputText.Text = "Invoked with right-click.";
        }

        void EdgeGesture_Canceled(object sender, EdgeGestureEventArgs e)
        {
            Scenario1OutputText.Text = "Canceled with touch.";
        }

        void EdgeGesture_Completed(object sender, EdgeGestureEventArgs e)
        {
            if (e.Kind == EdgeGestureKind.Touch)
            {
                Scenario1OutputText.Text = "Invoked with touch.";
            }
            else if (e.Kind == EdgeGestureKind.Keyboard)
            {
                Scenario1OutputText.Text = "Invoked with keyboard.";
            }
            else if (e.Kind == EdgeGestureKind.Mouse)
            {
                Scenario1OutputText.Text = "Invoked with right-click.";
            }
        }

        void EdgeGesture_Starting(object sender, EdgeGestureEventArgs e)
        {
            Scenario1OutputText.Text = "Invoking with touch.";
        }

        void Scenario1Init()
        {
            rootPage.RightTapped += new RightTappedEventHandler(this.OnContextMenu);
            this.edgeGesture.Canceled += new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Canceled);
            this.edgeGesture.Completed += new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Completed);
            this.edgeGesture.Starting += new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Starting);

            Scenario1OutputText.Text = "Sample initialized and events registered.";
        }

        void Scenario1Reset()
        {
            rootPage.RightTapped -= new RightTappedEventHandler(this.OnContextMenu);
            this.edgeGesture.Canceled -= new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Canceled);
            this.edgeGesture.Completed -= new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Completed);
            this.edgeGesture.Starting -= new TypedEventHandler<EdgeGesture, EdgeGestureEventArgs>(this.EdgeGesture_Starting);
        }

        #endregion
    }
}
