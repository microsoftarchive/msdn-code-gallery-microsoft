// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MsdnReader
{
    public abstract class ViewerWindowBase : Window
    {
        #region Methods

        static ViewerWindowBase()
        {
            CommandManager.RegisterClassCommandBinding(typeof(ViewerWindowBase), new CommandBinding(Maximize, OnMaximizeCommand));
            CommandManager.RegisterClassCommandBinding(typeof(ViewerWindowBase), new CommandBinding(Restore, OnRestoreCommand));
            CommandManager.RegisterClassCommandBinding(typeof(ViewerWindowBase), new CommandBinding(CloseWindow, OnCloseWindowCommand));
        }

        protected ViewerWindowBase()
        {
        }

        /// <summary>
        /// On Initialized, register for Loaded event
        /// </summary>
        /// <param name="e">Event data</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Virtual handler for Restore command
        /// </summary>
        /// <param name="e">EventArgs containing event data</param>
        protected virtual void OnRestore(ExecutedRoutedEventArgs e)
        {
            // Restore state to normal
            if (this.WindowState != WindowState.Normal)
            {
                this.WindowState = WindowState.Normal;
            }

        }

        /// <summary>
        /// Virtual handler for Maximize command
        /// </summary>
        /// <param name="e">EventArgs containing event data</param>
        protected virtual void OnMaximize(ExecutedRoutedEventArgs e)
        {
            // Set state to maximized
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }

        }

        /// <summary>
        /// Virtual handler for CloseWindow command
        /// </summary>
        /// <param name="e">EventArgs containing event data</param>
        protected virtual void OnClose(ExecutedRoutedEventArgs e)
        {
            // Close window
            this.Close();
        }

        /// <summary>
        /// Virtual handler for Loaded event
        /// </summary>
        /// <param name="e">EventArgs containing event data</param>
        protected virtual void OnLoaded(RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Virtual handler for Unloaded event
        /// </summary>
        /// <param name="e">EventArgs containing event data</param>
        protected virtual void OnUnloaded(RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Event handler for Loaded event
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Event data</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewerWindowBase window = sender as ViewerWindowBase;
            if (window != null)
            {
                window.OnLoaded(e);
            }
        }

        /// <summary>
        /// Event handler for Unloaded event
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Event data</param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewerWindowBase window = sender as ViewerWindowBase;
            if (window != null)
            {
                window.OnUnloaded(e);
            }
        }

        /// <summary>
        /// Command handler for Restore command
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnRestoreCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ViewerWindowBase window = sender as ViewerWindowBase;
            if (window != null)
            {
                window.OnRestore(e);
            }
        }

        /// <summary>
        /// Command handler for Maximize command
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnMaximizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ViewerWindowBase window = sender as ViewerWindowBase;
            if (window != null)
            {
                window.OnMaximize(e);
            }
        }

        /// <summary>
        /// Command handler for Close command
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnCloseWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ViewerWindowBase window = sender as ViewerWindowBase;
            if (window != null)
            {
                window.OnClose(e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Restore command
        /// </summary>
        public static RoutedCommand Restore
        {
            get { return _restore; }
        }

        /// <summary>
        /// Maximize command
        /// </summary>
        public static RoutedCommand Maximize
        {
            get { return _maximize; }
        }
        /// <summary>
        /// Close command
        /// </summary>
        public static RoutedCommand CloseWindow
        {
            get { return _close; }
        }

        #endregion

        #region Fields

        private static RoutedCommand _restore = new RoutedCommand("Restore", typeof(ImageViewerWindow));
        private static RoutedCommand _maximize = new RoutedCommand("Maximize", typeof(ImageViewerWindow));
        private static RoutedCommand _close = new RoutedCommand("Close", typeof(ImageViewerWindow));

        #endregion
    }
}