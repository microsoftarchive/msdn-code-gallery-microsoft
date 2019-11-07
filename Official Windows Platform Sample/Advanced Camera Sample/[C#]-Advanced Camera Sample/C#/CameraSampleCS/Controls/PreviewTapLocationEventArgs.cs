// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="PreviewTapLocationEventArgs.cs">
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

namespace CameraSampleCS.Controls
{
    using System;
    using System.Windows;

    /// <summary>
    /// Contains data for the <see cref="Viewfinder.PreviewTap"/> event.
    /// </summary>
    public class PreviewTapLocationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewTapLocationEventArgs"/> class.
        /// </summary>
        /// <param name="positionPercentage">Relative tap position.</param>
        public PreviewTapLocationEventArgs(Point positionPercentage)
        {
            this.PositionPercentage = positionPercentage;
        }

        /// <summary>
        /// Gets the relative tap position.
        /// </summary>
        public Point PositionPercentage { get; private set; }
    }
}
