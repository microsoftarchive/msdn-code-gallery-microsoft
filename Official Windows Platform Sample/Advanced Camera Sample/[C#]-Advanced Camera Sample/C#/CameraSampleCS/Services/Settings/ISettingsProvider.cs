// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ISettingsProvider.cs">
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

namespace CameraSampleCS.Services.Settings
{
    using CameraSampleCS.Models.Settings;
    using Microsoft.Devices;

    /// <summary>
    /// Defines methods for application settings management.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Gets the global application settings.
        /// </summary>
        /// <returns>Global application settings.</returns>
        IApplicationSettings GetApplicationSettings();

        /// <summary>
        /// Gets the settings for the <paramref name="cameraType"/> specified.
        /// </summary>
        /// <param name="cameraType">Type of the camera to get settings for.</param>
        /// <returns>Settings for the <paramref name="cameraType"/> specified.</returns>
        ICameraSettings GetCameraSettings(CameraType cameraType);
    }
}
