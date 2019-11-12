// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an Employees email address
    /// </summary>
    public class Email : ContactDetail
    {
        /// <summary>
        /// Usage values that are valid for email addresses
        /// </summary>
        private static string[] validUsageValues = new string[] { "Business", "Personal" };
        
        /// <summary>
        /// Gets a list of usage values that are valid for email addresses
        /// </summary>
        public override IEnumerable<string> ValidUsageValues
        {
            get { return validUsageValues; }
        }

        /// <summary>
        /// Gets or sets the actual email address
        /// </summary>
        public virtual string Address { get; set; }
    }
}
