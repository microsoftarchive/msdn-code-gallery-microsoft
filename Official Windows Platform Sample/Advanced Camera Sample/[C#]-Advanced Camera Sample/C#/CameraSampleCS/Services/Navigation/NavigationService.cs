// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="NavigationService.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.Services.Navigation
{
    using System;
    using System.Windows;
    using System.Windows.Navigation;
    using CameraSampleCS.Helpers;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Basic <see cref="INavigationService"/> implementation.
    /// </summary>
    public sealed class NavigationService : INavigationService
    {
        #region Fields

        /// <summary>
        /// Lazily-initialized application frame instance.
        /// </summary>
        private readonly Lazy<PhoneApplicationFrame> appFrame;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        public NavigationService()
        {
            this.appFrame = new Lazy<PhoneApplicationFrame>(this.GetCurrentApplicationFrame);
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when the current application is navigating to another page.
        /// </summary>
        public event NavigatingCancelEventHandler Navigating;

        #endregion // Events

        #region Public methods

        /// <summary>
        /// Navigates to the <paramref name="pageUri"/> specified.
        /// </summary>
        /// <param name="pageUri">URI of the page to navigate to.</param>
        /// <returns><see langword="true"/>, if the navigation started successfully; otherwise, <see langword="false"/>.</returns>
        public bool NavigateTo(Uri pageUri)
        {
            Tracing.Trace("NavigationService: Navigating to the page: {0}", pageUri);
            return this.EnsureApplicationFrameExists() && this.appFrame.Value.Navigate(pageUri);
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history,
        /// or throws an exception, if navigation backstack is empty.
        /// </summary>
        public void GoBack()
        {
            Tracing.Trace("NavigationService: Going back.");

            if (this.EnsureApplicationFrameExists() && this.appFrame.Value.CanGoBack)
            {
                this.appFrame.Value.GoBack();
            }
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Gets the current <see cref="Application"/> frame.
        /// </summary>
        /// <returns>Current <see cref="Application"/> frame.</returns>
        private PhoneApplicationFrame GetCurrentApplicationFrame()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;

            // Could be null, if the app runs inside a design tool.
            if (frame != null)
            {
                frame.Navigating += (s, e) =>
                {
                    NavigatingCancelEventHandler handler = this.Navigating;
                    if (handler != null)
                    {
                        handler(s, e);
                    }
                };
            }

            return frame;
        }

        /// <summary>
        /// Ensures that the <see cref="appFrame"/> is ready.
        /// </summary>
        /// <returns>
        /// <see langword="true"/>, if the <see cref="appFrame"/> is initialized;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private bool EnsureApplicationFrameExists()
        {
            return this.appFrame.Value != null;
        }

        #endregion // Private methods
    }
}
