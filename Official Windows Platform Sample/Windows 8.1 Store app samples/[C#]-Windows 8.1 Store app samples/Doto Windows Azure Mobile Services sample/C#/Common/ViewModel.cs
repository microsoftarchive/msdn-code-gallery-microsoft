// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Doto
{
    /// <summary>
    /// A base class for Doto's various view models which provides an implementation of the INotifyPropertyChanged. 
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the value of the target parameter (replacing the reference) if the value has changed. Also fires
        /// a PropertyChanged event if the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="target">The target to be swapped out, if different to the value parameter</param>
        /// <param name="value">The new value</param>
        /// <param name="changedProperties">A list of properties whose values are impacted by this change and whose PropertyChanged event should be raised</param>
        /// <returns>True if the value is changed, False otherwise</returns>
        protected virtual bool SetValue<T>(ref T target, T value, params string[] changedProperties)
        {
            if (Object.Equals(target, value))
            {
                return false;
            }

            target = value;

            foreach (string property in changedProperties)
            {
                OnPropertyChanged(property);
            }

            return true;
        }

        /// <summary>
        /// Sets the value of the target parameter (replacing the reference) if the value has changed. Also fires
        /// a PropertyChanged event if the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="target">The target to be swapped out, if different to the value parameter</param>
        /// <param name="value">The new value</param>
        /// <param name="changedProperty">Property whose value is impacted by this change and whose PropertyChanged event should be raised</param>
        /// <returns>True if the value is changed, False otherwise</returns>
        protected virtual bool SetValue<T>(ref T target, T value, [CallerMemberName] string changedProperty = "")
        {
            if (Object.Equals(target, value))
            {
                return false;
            }

            target = value;

            OnPropertyChanged(changedProperty);

            return true;
        }
    }
}
