// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using EmployeeTracker.Model;

    /// <summary>
    /// ViewModel of an individual Email
    /// </summary>
    public class EmailViewModel : ContactDetailViewModel
    {
        /// <summary>
        /// The Email object backing this ViewModel
        /// </summary>
        private Email email;

        /// <summary>
        /// Initializes a new instance of the EmailViewModel class.
        /// </summary>
        /// <param name="detail">The underlying Email this ViewModel is to be based on</param>
        public EmailViewModel(Email detail)
        {
            if (detail == null)
            {
                throw new ArgumentNullException("detail");
            }

            this.email = detail;
        }

        /// <summary>
        /// Gets the underlying Email this ViewModel is based on
        /// </summary>
        public override ContactDetail Model
        {
            get { return this.email; }
        }

        /// <summary>
        /// Gets or sets the actual email address
        /// </summary>
        public string Address
        {
            get
            {
                return this.email.Address;
            }

            set
            {
                this.email.Address = value;
                this.OnPropertyChanged("Address");
            }
        }
    }
}
