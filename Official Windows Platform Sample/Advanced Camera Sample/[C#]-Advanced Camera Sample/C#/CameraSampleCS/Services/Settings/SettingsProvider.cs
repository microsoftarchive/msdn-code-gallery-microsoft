// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SettingsProvider.cs">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CameraSampleCS.Models.Settings;
    using Microsoft.Devices;

    /// <summary>
    /// Default <see cref="ISettingsProvider"/> implementation.
    /// </summary>
    public sealed class SettingsProvider : ISettingsProvider, IDisposable
    {
        #region Fields

        /// <summary>
        /// Lazily-created application settings object.
        /// </summary>
        private readonly Lazy<IApplicationSettings> lazyApplicationSettings = new Lazy<IApplicationSettings>(() => new ApplicationSettings());

        /// <summary>
        /// Collection of lazily-created camera-specific settings.
        /// </summary>
        private readonly IDictionary<CameraType, ICameraSettings> cameraSettingsMap = new Dictionary<CameraType, ICameraSettings>();

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Finalizes an instance of the <see cref="SettingsProvider"/> class.
        /// </summary>
        ~SettingsProvider()
        {
            this.Dispose(false);
        }

        #endregion // Constructor

        #region Public methods

        /// <summary>
        /// Gets the global application settings.
        /// </summary>
        /// <returns>Global application settings.</returns>
        public IApplicationSettings GetApplicationSettings()
        {
            return this.lazyApplicationSettings.Value;
        }

        /// <summary>
        /// Gets the settings for the <paramref name="cameraType"/> specified.
        /// </summary>
        /// <param name="cameraType">Type of the camera to get settings for.</param>
        /// <returns>Settings for the <paramref name="cameraType" /> specified.</returns>
        public ICameraSettings GetCameraSettings(CameraType cameraType)
        {
            lock (this.cameraSettingsMap)
            {
                if (!this.cameraSettingsMap.ContainsKey(cameraType))
                {
                    this.cameraSettingsMap[cameraType] = new CameraSettings(cameraType);
                }

                return this.cameraSettingsMap[cameraType];
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.lazyApplicationSettings.IsValueCreated)
            {
                IDisposable disposable = this.lazyApplicationSettings.Value as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            foreach (IDisposable disposable in this.cameraSettingsMap.Values.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }

        #endregion // Private methods
    }
}
