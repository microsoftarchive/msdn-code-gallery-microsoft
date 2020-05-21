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
using System.ComponentModel;
using Microsoft.SceReader;
using System.IO;
using System.Windows;

namespace MsdnReader
{
    /// <summary>
    /// 
    /// </summary>
    public class MsdnReaderSettings : SceReaderSettings
    {
        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        public static bool TransitionsEnabled
        {
            get { return Properties.Settings.Default.TransitionsEnabled; }
            set { Properties.Settings.Default.TransitionsEnabled = value; }
        }

        /// <summary>
        /// Name of the channel exposed by subscription service, which applications must connect to to communicate 
        /// with the service
        /// </summary>
        public static string SubscriptionServiceChannelName
        {
            get { return Properties.Settings.Default.SubscriptionServiceUri; }
        }

        /// <summary>
        /// Name of the IPC channel port used by this application to receive event notifications from the subscription service.
        /// </summary>
        public static string ChannelPortName
        {
            get { return Properties.Settings.Default.ChannelPortName; }
        }

        /// <summary>
        /// Name of system-wide event that is used by the subscription service to signal channel activation for this user.
        /// Once this event has been signaled, communication with the service may proceed. 
        /// Subscription service events are named by concatenating the service prefix with the current user name
        /// </summary>
        public static string SubscriptionServiceSignalName
        {
            get { return String.Concat(Properties.Settings.Default.SubscriptionServiceSignalPrefix, Environment.UserName); }
        }

        /// <summary>
        /// If this is set to true, the application does not consume navigation journal commands Back and Forward, i.e., the 
        /// BrowseBack and BrowseForward navigation commands behave as they do in the browser, to navigate through journaled history.
        /// If set to false, BrowseBack and BrowseForward are used by the application to navigate according to it's own next/previous ordering
        /// </summary>
        public static bool EnableJournalNavigationOnInput
        {
            get { return Properties.Settings.Default.EnableJournalNavigationOnInput; }
        }

        /// <summary>
        /// True if size and lcoation of main window should be persisted across instances of the application
        /// </summary>
        public static bool SaveMainWindowBounds
        {
            get { return Properties.Settings.Default.SaveMainWindowBounds; }
            set { Properties.Settings.Default.SaveMainWindowBounds = value; }
        }

        /// <summary>
        /// Rect with persisted values of MainWindow bounds, so position and size of the MainWindow can be remembered across instances
        /// </summary>
        public static Rect MainWindowBounds
        {
            get { return Properties.Settings.Default.MainWindowBounds; }
            set { Properties.Settings.Default.MainWindowBounds = value; }
        }

        /// <summary>
        /// Persisted value of WindowState to be restored in subsequent instances
        /// </summary>
        public static WindowState MainWindowState
        {
            get { return Properties.Settings.Default.MainWindowState; }
            set { Properties.Settings.Default.MainWindowState = value; }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Initializes settings values.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            base.StoryViewerZoomCore = Properties.Settings.Default.StoryViewerZoom;
            _dataFeedUri = new Uri(Properties.Settings.Default.DataFeedUri);
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Properties
        //
        //-------------------------------------------------------------------

        #region Protected Properties

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        protected override string ApplicationNameCore
        {
            get { return Properties.Settings.Default.ApplicationName; }
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        protected override string CompanyNameCore
        {
            get { return Properties.Settings.Default.CompanyName; }
        }

        /// <summary>
        /// Gets the URI of the data feed.
        /// </summary>
        protected override Uri DataFeedUriCore
        {
            get { return _dataFeedUri; }
        }

        /// <summary>
        /// Gets the file extension for package files containing saved data.
        /// </summary>
        protected override string SavedDataExtensionCore
        {
            get { return Properties.Settings.Default.SavedDataExtension; }
        }

        /// <summary>
        /// Gets or sets the zoom setting applied to the story viewer.
        /// </summary>
        protected override double StoryViewerZoomCore
        {
            set
            {
                base.StoryViewerZoomCore = value;
                if (!DoubleUtil.AreClose(Properties.Settings.Default.StoryViewerZoom, value))
                {
                    Properties.Settings.Default.StoryViewerZoom = value;
                }
            }
        }

        /// <summary>
        /// True if application authors wish the application to fetch data from the design feed in debug configuration. 
        /// This can be used in design mode so that web requests are not made from designer.
        /// </summary>
        protected override bool UseDesignFeedInDebugCore
        {
            get { return Properties.Settings.Default.UseDesignFeedInDebug; }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        /// <summary>
        /// The URI of the data feed.
        /// </summary>
        private Uri _dataFeedUri;

        #endregion

    }
}