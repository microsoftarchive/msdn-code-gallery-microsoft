// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an Employees address
    /// </summary>
    public class Address : ContactDetail
    {
        /// <summary>
        /// Usage values that are valid for addresses
        /// </summary>
        private static string[] validUsageValues = new string[] { "Business", "Home", "Mailing" };

        /// <summary>
        /// Gets a list of usage values that are valid for addresses
        /// </summary>
        public override IEnumerable<string> ValidUsageValues
        {
            get { return validUsageValues; }
        }

        /// <summary>
        /// Gets or sets the first line of this Address
        /// </summary>
        public virtual string LineOne { get; set; }

        /// <summary>
        /// Gets or sets the second line of this Address
        /// </summary>
        public virtual string LineTwo { get; set; }

        /// <summary>
        /// Gets or sets the city of this Address
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        /// Gets or sets the state of this Address
        /// </summary>
        public virtual string State { get; set; }

        /// <summary>
        /// Gets or sets the zipc code of this Address
        /// </summary>
        public virtual string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the country of this Address
        /// </summary>
        public virtual string Country { get; set; }
    }
}
