// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using EmployeeTracker.Model;

    /// <summary>
    /// ViewModel of an individual Address
    /// </summary>
    public class AddressViewModel : ContactDetailViewModel
    {
        /// <summary>
        /// The Address object backing this ViewModel
        /// </summary>
        private Address address;
        
        /// <summary>
        /// Initializes a new instance of the AddressViewModel class.
        /// </summary>
        /// <param name="detail">The underlying Address this ViewModel is to be based on</param>
        public AddressViewModel(Address detail)
        {
            if (detail == null)
            {
                throw new ArgumentNullException("detail");
            }

            this.address = detail;
        }

        /// <summary>
        /// The underlying Address this ViewModel is based on
        /// </summary>
        public override ContactDetail Model
        {
            get { return this.address; }
        }

        /// <summary>
        /// Gets or sets the first address line
        /// </summary>
        public string LineOne
        {
            get
            {
                return this.address.LineOne;
            }

            set
            {
                this.address.LineOne = value;
                this.OnPropertyChanged("LineOne");
            }
        }

        /// <summary>
        /// Gets or sets the second address line
        /// </summary>
        public string LineTwo
        {
            get
            {
                return this.address.LineTwo;
            }

            set
            {
                this.address.LineTwo = value;
                this.OnPropertyChanged("LineTwo");
            }
        }

        /// <summary>
        /// Gets or sets the city of this address
        /// </summary>
        public string City
        {
            get
            {
                return this.address.City;
            }

            set
            {
                this.address.City = value;
                this.OnPropertyChanged("City");
            }
        }

        /// <summary>
        /// Gets or sets the state of this address
        /// </summary>
        public string State
        {
            get
            {
                return this.address.State;
            }

            set
            {
                this.address.State = value;
                this.OnPropertyChanged("State");
            }
        }

        /// <summary>
        /// Gets or sets the zip code of this address
        /// </summary>
        public string ZipCode
        {
            get
            {
                return this.address.ZipCode;
            }

            set
            {
                this.address.ZipCode = value;
                this.OnPropertyChanged("ZipCode");
            }
        }

        /// <summary>
        /// Gets or sets the country of this address
        /// </summary>
        public string Country
        {
            get
            {
                return this.address.Country;
            }

            set
            {
                this.address.Country = value;
                this.OnPropertyChanged("Country");
            }
        }
    }
}
