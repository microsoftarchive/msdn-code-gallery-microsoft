// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Common
{
    using System;
    using System.Data.Objects;
    using EmployeeTracker.Model;

    /// <summary>
    /// Data context containing data for the EmployeeTracker model
    /// </summary>
    public interface IEmployeeContext : IDisposable
    {
        /// <summary>
        /// Gets Employees in the data context
        /// </summary>
        IObjectSet<Employee> Employees { get; }

        /// <summary>
        /// Gets Departments in the data context
        /// </summary>
        IObjectSet<Department> Departments { get; }

        /// <summary>
        /// Gets ContactDetails in the data context
        /// </summary>
        IObjectSet<ContactDetail> ContactDetails { get; }

        /// <summary>
        /// Save all pending changes to the data context
        /// </summary>
        void Save();

        /// <summary>
        /// Creates a new instance of the specified object type
        /// NOTE: This pattern is used to allow the use of change tracking proxies
        ///       when running against the Entity Framework.
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        /// <returns>The newly created object</returns>
        T CreateObject<T>() where T : class;

        /// <summary>
        /// Checks if the supplied object is tracked in this data context
        /// </summary>
        /// <param name="obj">The object to check for</param>
        /// <returns>True if the object is tracked, false otherwise</returns>
        bool IsObjectTracked(object obj);
    }
}
