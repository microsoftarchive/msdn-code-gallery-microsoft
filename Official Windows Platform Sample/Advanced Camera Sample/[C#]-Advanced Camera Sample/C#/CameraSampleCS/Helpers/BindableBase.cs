// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="BindableBase.cs">
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
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    /// <summary>
    /// Implementation of the <see cref="INotifyPropertyChanged"/> interface for
    /// model classes simplification.
    /// </summary>
    public abstract class BindableBase : DependencyObject, INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value.<br/>
        /// Sets the property and notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        /// Name of the property used to notify listeners.<br/>
        /// This value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/>, if the value was changed, <see langword="false"/>,
        /// if the existing value matched the desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            bool different = !object.Equals(storage, value);
            if (different)
            {
                this.NotifyPropertyChanging(propertyName);
                storage = value;
                this.NotifyPropertyChanged(propertyName);
            }

            return different;
        }

        /// <summary>
        /// Notifies listeners that a property value is changing.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property used to notify listeners.<br/>
        /// This value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        protected void NotifyPropertyChanging([CallerMemberName] string propertyName = "")
        {
            PropertyChangingEventHandler eventHandler = this.PropertyChanging;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property used to notify listeners.<br/>
        /// This value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Notifies listeners that all property values have potentially changed.
        /// </summary>
        protected void NotifyPropertiesChanged()
        {
            // Empty property name causes any registered listeners to fire.
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
