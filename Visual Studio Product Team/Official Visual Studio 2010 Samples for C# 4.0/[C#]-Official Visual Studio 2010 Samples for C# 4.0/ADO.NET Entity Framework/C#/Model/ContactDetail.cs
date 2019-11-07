// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class representing a contact detail of an Employee
    /// </summary>
    public abstract class ContactDetail
    {
        /// <summary>
        /// Gets values that are valid for the usage property
        /// </summary>
        public abstract IEnumerable<string> ValidUsageValues { get; }

        /// <summary>
        /// Gets or sets the Id of this ContactDetail
        /// </summary>
        public virtual int ContactDetailId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the Employee this ContactDetail belongs to
        /// </summary>
        public virtual int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets how this contact detail is used i.e. Home/Business etc.
        /// </summary>
        public virtual string Usage { get; set; }
    }
}
