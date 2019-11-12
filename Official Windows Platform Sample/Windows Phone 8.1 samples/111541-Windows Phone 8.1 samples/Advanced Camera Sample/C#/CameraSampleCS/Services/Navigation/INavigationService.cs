// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="INavigationService.cs">
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
    using System.Windows.Navigation;

    /// <summary>
    /// Defines methods used for navigation inside the application.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Occurs when the current application is navigating to another page.
        /// </summary>
        event NavigatingCancelEventHandler Navigating;

        /// <summary>
        /// Navigates to the <paramref name="pageUri"/> specified.
        /// </summary>
        /// <param name="pageUri">URI of the page to navigate to.</param>
        /// <returns><see langword="true"/>, if the navigation started successfully; otherwise, <see langword="false"/>.</returns>
        bool NavigateTo(Uri pageUri);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history,
        /// or throws an exception, if navigation backstack is empty.
        /// </summary>
        void GoBack();
    }
}
