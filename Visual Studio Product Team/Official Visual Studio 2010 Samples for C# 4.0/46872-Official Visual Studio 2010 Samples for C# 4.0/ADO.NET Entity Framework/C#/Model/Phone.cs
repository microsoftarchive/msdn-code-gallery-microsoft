// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an employees phone number
    /// </summary>
    public class Phone : ContactDetail
    {
        /// <summary>
        /// Usage values that are valid for phone numbers
        /// </summary>
        private static string[] validUsageValues = new string[] { "Business", "Home", "Cell" };

        /// <summary>
        /// Gets a list of usage values that are valid for phone numbers
        /// </summary>
        public override IEnumerable<string> ValidUsageValues
        {
            get { return validUsageValues; }
        }

        /// <summary>
        /// Gets or sets the actual phone number
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// Gets or sets the extension associated with this phone number
        /// </summary>
        public virtual string Extension { get; set; }
    }
}
