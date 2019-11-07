// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SettingsBase.cs">
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
    using System;
    using System.IO.IsolatedStorage;
    using CameraSampleCS.Helpers;

    /// <summary>
    /// Helper class to work with the stored application settings.
    /// </summary>
    public abstract class SettingsBase : BindableBase, IDisposable
    {
        #region Fields

        /// <summary>
        /// Actual settings object stored on the device.
        /// </summary>
        private readonly IsolatedStorageSettings settings;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        protected SettingsBase()
        {
            this.settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        ~SettingsBase()
        {
            this.Dispose(false);
        }

        #endregion // Constructor

        #region Public methods

        /// <summary>
        /// Updates a setting value for the current application.<br/>
        /// If the setting does not exist, adds a new one.
        /// </summary>
        /// <param name="key">Setting key.</param>
        /// <param name="value">Setting value.</param>
        /// <returns>
        /// <see langword="true"/>, if a <b>new</b> <paramref name="value"/> has been set for the setting; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><param name="key"> is <see langword="null"/> or empty.</param></exception>
        public virtual bool AddOrUpdateValue(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            bool valueChanged = false;

            if (this.settings.Contains(key))
            {
                // Update value, if needed.
                if (this.settings[key] != value)
                {
                    this.settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                this.settings.Add(key, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        /// <summary>
        /// Gets the current value of the setting, or if it is not found, sets the
        /// setting to the default value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="key">Setting key.</param>
        /// <param name="defaultValue">The default setting value.</param>
        /// <returns>Setting value or <paramref name="defaultValue"/>, if original value not found.</returns>
        /// <exception cref="ArgumentNullException"><param name="key"> is <see langword="null"/> or empty.</param></exception>
        public virtual T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return this.settings.Contains(key) ? (T)this.settings[key] : defaultValue;
        }

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        public void Save()
        {
            this.settings.Save();
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
            if (disposing)
            {
                this.Save();
            }
        }

        #endregion // Private methods
    }
}
