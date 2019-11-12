// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using EmployeeTracker.Model;
    using EmployeeTracker.Model.Interfaces;

    /// <summary>
    /// Repository for retrieving department data from an IObjectSet
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        /// <summary>
        /// Underlying ObjectSet to retrieve data from
        /// </summary>
        private IObjectSet<Department> objectSet;

        /// <summary>
        /// Initializes a new instance of the DepartmentRepository class.
        /// </summary>
        /// <param name="context">Context to retrieve data from</param>
        public DepartmentRepository(IEmployeeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.objectSet = context.Departments;
        }

        /// <summary>
        /// All departments for the company
        /// </summary>
        /// <returns>Enumerable of all departments</returns>
        public IEnumerable<Department> GetAllDepartments()
        {
            // NOTE: Some points considered during implementation of data access methods:
            //    -  ToList is used to ensure any data access related exceptions are thrown
            //       during execution of this method rather than when the data is enumerated.
            //    -  Returning IEnumerable rather than IQueryable ensures the repository has full control
            //       over how data is retrieved from the store, returning IQueryable would allow consumers
            //       to add additional operators and affect the query sent to the store.
            return this.objectSet.ToList();
        }
    }
}
