// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CameraProvider.cs">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using CameraSampleCS.Helpers;
    using Microsoft.Devices;

    using PhotoCamera = CameraSampleCS.Models.Camera.PhotoCamera;

    /// <summary>
    /// Default <see cref="ICameraProvider"/> implementation.
    /// </summary>
    public sealed class CameraProvider : ICameraProvider
    {
        #region Fields

        /// <summary>
        /// Checks, whether the primary camera is supported on the phone.
        /// </summary>
        private readonly Lazy<bool> primaryCameraSupported = new Lazy<bool>(() => DeviceHelper.GetCameraDeviceInfoAsync(CameraType.Primary).GetAwaiter().GetResult() != null);

        /// <summary>
        /// Checks, whether the front-facing camera is supported on the phone.
        /// </summary>
        private readonly Lazy<bool> frontFacingCameraSupported = new Lazy<bool>(() => DeviceHelper.GetCameraDeviceInfoAsync(CameraType.FrontFacing).GetAwaiter().GetResult() != null);

        /// <summary>
        /// Collection of cameras created.
        /// </summary>
        private readonly IDictionary<CameraType, PhotoCamera> cameras = new Dictionary<CameraType, PhotoCamera>();

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the primary camera is supported on the current device.
        /// </summary>
        public bool PrimaryPhotoCameraSupported
        {
            get
            {
                return this.primaryCameraSupported.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether front facing camera is supported on the current device.
        /// </summary>
        public bool FrontFacingPhotoCameraSupported
        {
            get
            {
                return this.frontFacingCameraSupported.Value;
            }
        }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Gets the photo camera of the <paramref name="cameraType"/> specified.
        /// </summary>
        /// <param name="cameraType">Photo camera type.</param>
        /// <returns>Photo camera.</returns>
        /// <exception cref="ArgumentException"><paramref name="cameraType"/> is not supported.</exception>
        public async Task<PhotoCamera> GetCameraAsync(CameraType cameraType)
        {
            if ((await DeviceHelper.GetCameraDeviceInfoAsync(cameraType)) == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} camera is not supported.", cameraType), "cameraType");
            }

            lock (this.cameras)
            {
                if (!this.cameras.ContainsKey(cameraType))
                {
                    this.cameras[cameraType] = new PhotoCamera(cameraType);
                }

                return this.cameras[cameraType];
            }
        }

        #endregion // Public methods
    }
}
