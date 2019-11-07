// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using EmployeeTracker.Model;

    /// <summary>
    /// ViewModel of an individual Phone
    /// </summary>
    public class PhoneViewModel : ContactDetailViewModel
    {
        /// <summary>
        /// The Phone object backing this ViewModel
        /// </summary>
        private Phone phone;

        /// <summary>
        /// Initializes a new instance of the PhoneViewModel class.
        /// </summary>
        /// <param name="detail">The underlying Phone this ViewModel is to be based on</param>
        public PhoneViewModel(Phone detail)
        {
            if (detail == null)
            {
                throw new ArgumentNullException("detail");
            }

            this.phone = detail;
        }

        /// <summary>
        /// The underlying Phone this ViewModel is based on
        /// </summary>
        public override ContactDetail Model
        {
            get { return this.phone; }
        }

        /// <summary>
        /// Gets or sets the actual number
        /// </summary>
        public string Number
        {
            get
            {
                return this.phone.Number;
            }

            set
            {
                this.phone.Number = value;
                this.OnPropertyChanged("Number");
            }
        }

        /// <summary>
        /// Gets or sets the extension to be used with this phone number
        /// </summary>
        public string Extension
        {
            get
            {
                return this.phone.Extension;
            }

            set
            {
                this.phone.Extension = value;
                this.OnPropertyChanged("Extension");
            }
        }
    }
}
