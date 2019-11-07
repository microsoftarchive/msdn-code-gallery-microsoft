// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="Constants.cs">
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

namespace CameraSampleCS
{
    using System;
    using System.Windows;
    using CameraSampleCS.Views;
    using Microsoft.Phone.Shell;

    /// <summary>
    /// Single place to define constants used in application.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Default application bar height, in pixels.
        /// </summary>
        public const int ApplicationBarHeight = 72;

        /// <summary>
        /// Default <see cref="ApplicationBar"/> opacity, when menu is not shown.
        /// </summary>
        public const double DefaultApplicationBarOpacity = 0.15;

        /// <summary>
        /// We will only support images up to 1M in size.
        /// </summary>
        public const uint MaxSupportedResolution = 1 * 1024 * 1024;

        /// <summary>
        /// Default <c>DPI X</c> value for <c>JPEG</c> conversion.
        /// </summary>
        public const uint DefaultDpiX = 96;

        /// <summary>
        /// Default <c>DPI Y</c> value for <c>JPEG</c> conversion.
        /// </summary>
        public const uint DefaultDpiY = 96;

        /// <summary>
        /// Default video preview size.
        /// </summary>
        public static readonly Size DefaultPreviewResolution = new Size(480, 800);

        /// <summary>
        /// <see cref="AboutPage"/> page location.
        /// </summary>
        public static readonly Uri AboutPageUri = new Uri("/Views/AboutPage.xaml", UriKind.Relative);
    }
}
