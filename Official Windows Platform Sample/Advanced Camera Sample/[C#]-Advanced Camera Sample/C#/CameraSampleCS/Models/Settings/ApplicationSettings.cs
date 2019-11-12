// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ApplicationSettings.cs">
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
    using Microsoft.Devices;

    /// <summary>
    /// Global application settings.
    /// </summary>
    public class ApplicationSettings : SettingsBase, IApplicationSettings
    {
        #region Fields

        /// <summary>
        /// Name of the <see cref="CameraType"/> property.
        /// </summary>
        public const string CameraTypePropertyName = "CameraType";

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the type of the active camera.
        /// </summary>
        public CameraType CameraType
        {
            get
            {
                return this.GetValueOrDefault(ApplicationSettings.CameraTypePropertyName, CameraType.Primary);
            }

            set
            {
                if (this.AddOrUpdateValue(ApplicationSettings.CameraTypePropertyName, value))
                {
                    this.Save();
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion // Properties
    }
}
