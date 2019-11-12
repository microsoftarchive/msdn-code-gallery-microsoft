// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="DeviceHelper.cs">
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

namespace CameraSampleCS.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Devices;
    using Windows.Devices.Enumeration;

    /// <summary>
    /// Contains helper methods for working with the system devices.
    /// </summary>
    public static class DeviceHelper
    {
        #region Fields

        /// <summary>
        /// Synchronization root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Contains cached device information for the camera types.
        /// </summary>
        private static volatile IDictionary<Panel, DeviceInformation> CameraDeviceInfoCache;

        #endregion // Fields

        #region Public methods

        /// <summary>
        /// Gets the camera device information with the <paramref name="cameraType"/> specified.
        /// </summary>
        /// <param name="cameraType">Camera type.</param>
        /// <returns>Device information, or <see langword="null"/>, if no suitable devices found.</returns>
        /// <exception cref="ArgumentException"><paramref name="cameraType"/> is not supported.</exception>
        public static async Task<DeviceInformation> GetCameraDeviceInfoAsync(CameraType cameraType)
        {
            Panel desiredPanel;
            switch (cameraType)
            {
                case CameraType.Primary:
                    desiredPanel = Panel.Back;
                    break;
                case CameraType.FrontFacing:
                    desiredPanel = Panel.Front;
                    break;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Camera type {0} is not supported.", cameraType), "cameraType");
            }

            // Make sure the device info cache is populated.
            await DeviceHelper.EnumerateCameraDevicesAsync();

            return DeviceHelper.CameraDeviceInfoCache.ContainsKey(desiredPanel) ? DeviceHelper.CameraDeviceInfoCache[desiredPanel] : null;
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Enumerates the existing camera devices and stores them into the
        /// <see cref="CameraDeviceInfoCache"/> collection, if needed.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        private static async Task EnumerateCameraDevicesAsync()
        {
            if (DeviceHelper.CameraDeviceInfoCache != null)
            {
                return;
            }

            IDictionary<Panel, DeviceInformation> devices = new Dictionary<Panel, DeviceInformation>();

            foreach (DeviceInformation deviceInfo in (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)).Where(d => d.IsEnabled && d.EnclosureLocation != null))
            {
                devices[deviceInfo.EnclosureLocation.Panel] = deviceInfo;
            }

            lock (DeviceHelper.SyncRoot)
            {
                if (DeviceHelper.CameraDeviceInfoCache != null)
                {
                    return;
                }

                DeviceHelper.CameraDeviceInfoCache = devices;
            }
        }

        #endregion // Private methods
    }
}
