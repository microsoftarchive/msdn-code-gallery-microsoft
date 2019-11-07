// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ICameraProvider.cs">
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

namespace CameraSampleCS.Services.Camera
{
    using System.Threading.Tasks;
    using Microsoft.Devices;

    using PhotoCamera = CameraSampleCS.Models.Camera.PhotoCamera;

    /// <summary>
    /// Defines methods for camera devices management.
    /// </summary>
    public interface ICameraProvider
    {
        /// <summary>
        /// Gets a value indicating whether front facing camera is supported on the current device.
        /// </summary>
        bool FrontFacingPhotoCameraSupported { get; }

        /// <summary>
        /// Gets a value indicating whether the primary camera is supported on the current device.
        /// </summary>
        bool PrimaryPhotoCameraSupported { get; }

        /// <summary>
        /// Gets the photo camera of the <paramref name="cameraType"/> specified.
        /// </summary>
        /// <param name="cameraType">Photo camera type.</param>
        /// <returns>Photo camera.</returns>
        Task<PhotoCamera> GetCameraAsync(CameraType cameraType);
    }
}
