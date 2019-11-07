// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CameraSettings.cs">
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
    using CameraSampleCS.Models.Camera;
    using Microsoft.Devices;

    using FlashMode = CameraSampleCS.Models.Camera.FlashMode;

    /// <summary>
    /// Provides access to supported stored camera settings.
    /// </summary>
    public class CameraSettings : SettingsBase, ICameraSettings
    {
        #region Fields

        /// <summary>
        /// Name of the <see cref="FlashMode"/> property.
        /// </summary>
        public const string FlashModePropertyName = "FlashMode";

        /// <summary>
        /// Name of the <see cref="CaptureMode"/> property.
        /// </summary>
        public const string CaptureModePropertyName = "CaptureMode";

        /// <summary>
        /// Name of the <see cref="ScreenFormat"/> property.
        /// </summary>
        public const string ScreenFormatPropertyName = "ScreenFormat";

        /// <summary>
        /// Camera-specific setting key prefix.
        /// </summary>
        private readonly string cameraKeyPrefix;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraSettings"/> class.
        /// </summary>
        /// <param name="cameraType">Type of the camera for the current settings.</param>
        public CameraSettings(CameraType cameraType)
        {
            this.cameraKeyPrefix = "__cs_" + cameraType + "_";
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the flash mode.
        /// </summary>
        public FlashMode FlashMode
        {
            get
            {
                return this.GetValueOrDefault(CameraSettings.FlashModePropertyName, FlashMode.Off);
            }

            set
            {
                if (this.AddOrUpdateValue(CameraSettings.FlashModePropertyName, value))
                {
                    this.Save();
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the camera capture mode.
        /// </summary>
        public CaptureMode CaptureMode
        {
            get
            {
                return this.GetValueOrDefault(CameraSettings.CaptureModePropertyName, CaptureMode.LowLag);
            }

            set
            {
                if (this.AddOrUpdateValue(CameraSettings.CaptureModePropertyName, value))
                {
                    this.Save();
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the screen format.
        /// </summary>
        public ScreenFormat ScreenFormat
        {
            get
            {
                return this.GetValueOrDefault(CameraSettings.ScreenFormatPropertyName, ScreenFormat.FourByThree);
            }

            set
            {
                if (this.AddOrUpdateValue(CameraSettings.ScreenFormatPropertyName, value))
                {
                    this.Save();
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Updates a setting value for the current application.<br />
        /// If the setting does not exist, adds a new one.
        /// </summary>
        /// <param name="key">Setting key.</param>
        /// <param name="value">Setting value.</param>
        /// <returns>
        ///   <see langword="true" />, if a <b>new</b> <paramref name="value" /> has been set for the setting; otherwise, <see langword="false" />.
        /// </returns>
        public override bool AddOrUpdateValue(string key, object value)
        {
            return base.AddOrUpdateValue(this.cameraKeyPrefix + key, value);
        }

        /// <summary>
        /// Gets the current value of the setting, or if it is not found, sets the
        /// setting to the default value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="key">Setting key.</param>
        /// <param name="defaultValue">The default setting value.</param>
        /// <returns>
        /// Setting value or <paramref name="defaultValue" />, if original value not found.
        /// </returns>
        public override T GetValueOrDefault<T>(string key, T defaultValue)
        {
            return base.GetValueOrDefault(this.cameraKeyPrefix + key, defaultValue);
        }

        #endregion // Public methods
    }
}
