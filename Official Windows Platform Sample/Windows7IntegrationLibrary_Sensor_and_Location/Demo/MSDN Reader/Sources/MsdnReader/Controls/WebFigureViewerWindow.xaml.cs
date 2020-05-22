// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.SceReader.Data;
using System.Windows.Threading;

namespace MsdnReader
{
    /// <summary>
    /// Interaction logic for WebFigureViewerWindow.xaml. This class is responsible for displaying a web page containing scripts
    /// or other code figures associated with an MSDN magazine article. The web page is displayed in a chromeless frame.
    /// </summary>
    public partial class WebFigureViewerWindow : ViewerWindowBase
    {
        #region Methods

        public WebFigureViewerWindow() : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Mouse down over title grid - fakes Drag since window has no style
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDownOverTitleGrid(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        /// <summary>
        /// Event handler for Loaded event
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Event data</param>
        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            this.DataContext = this;
            this.closeButton.Focus();
            this.webContentHost.NavigationFailed += OnWebContentHostNavigationFailed;
        }

        /// <summary>
        /// Event handler for Unloaded event
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Event data</param>
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            this.webContentHost.NavigationFailed -= OnWebContentHostNavigationFailed;
        }

        /// <summary>
        /// Handler for Frame's navigation failed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebContentHostNavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            // Handle the event - no need to log here, since we're navigating to web content this will only work when
            // online and may fail for any number of reasons
            // If event is not handled, exception will be thrown by the Frame element.
            if (!e.Handled)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Command handler for Restore command
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        protected override void OnRestore(ExecutedRoutedEventArgs e)
        {
            base.OnRestore(e);
            this.maximizeButton.Visibility = Visibility.Visible;
            this.restoreButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Command handler for Maximize command
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        protected override void OnMaximize(ExecutedRoutedEventArgs e)
        {
            base.OnMaximize(e);
            this.maximizeButton.Visibility = Visibility.Collapsed;
            this.restoreButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Invalidates source on web content host Frame and resets it. This is necessary because Frame will
        /// not navigate again if the Uri is the same, even if the query string indicating the position of the image is different
        /// </summary>
        private object InvalidateSource(object arg)
        {
            this.webContentHost.Source = this.Source;
            return null;
        }


        /// <summary>
        /// Invoked when Source property is changed.
        /// </summary>
        private static void OnSourceChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            WebFigureViewerWindow window = element as WebFigureViewerWindow;
            if (window != null)
            {
                window.webContentHost.Navigate(null);
                window.Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(window.InvalidateSource), null);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// DependencyProperty for <see cref="Source" /> property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
                DependencyProperty.Register(
                        "Source",
                        typeof(Uri),
                        typeof(WebFigureViewerWindow),
                        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// The Source pointing to the Uri where web figure is located
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Story" /> property.
        /// </summary>
        public static readonly DependencyProperty StoryProperty =
                DependencyProperty.Register(
                        "Story",
                        typeof(Story),
                        typeof(WebFigureViewerWindow),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The Story object associated with ImageReference.
        /// </summary>
        public Story Story
        {
            get { return (Story)GetValue(StoryProperty); }
            set { SetValue(StoryProperty, value); }
        }

        #endregion
    }
}