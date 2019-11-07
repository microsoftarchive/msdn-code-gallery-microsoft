// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ICameraSettings.cs">
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

namespace CameraSampleCS.Models.Settings
{
    using System.ComponentModel;
    using CameraSampleCS.Models.Camera;

    /// <summary>
    /// Defines camera-specific settings used by the application.
    /// </summary>
    public interface ICameraSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the flash mode.
        /// </summary>
        FlashMode FlashMode { get; set; }

        /// <summary>
        /// Gets or sets the camera capture mode.
        /// </summary>
        CaptureMode CaptureMode { get; set; }

        /// <summary>
        /// Gets or sets the screen format.
        /// </summary>
        ScreenFormat ScreenFormat { get; set; }
    }
}
