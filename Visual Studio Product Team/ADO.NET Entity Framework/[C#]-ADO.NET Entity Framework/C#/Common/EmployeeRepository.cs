// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using EmployeeTracker.Model;
    using EmployeeTracker.Model.Interfaces;

    /// <summary>
    /// Repository for retrieving employee data from an ObjectSet
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        /// <summary>
        /// Underlying ObjectSet to retrieve data from
        /// </summary>
        private IObjectSet<Employee> objectSet;

        /// <summary>
        /// Initializes a new instance of the EmployeeRepository class.
        /// </summary>
        /// <param name="context">Context to retrieve data from</param>
        public EmployeeRepository(IEmployeeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.objectSet = context.Employees;
        }

        /// <summary>
        /// All employees for the company
        /// </summary>
        /// <returns>Enumerable of all employees</returns>  
        public IEnumerable<Employee> GetAllEmployees()
        {
            // NOTE: Some points considered during implementation of data access methods:
            //    -  ToList is used to ensure any data access related exceptions are thrown
            //       during execution of this method rather than when the data is enumerated.
            //    -  Returning IEnumerable rather than IQueryable ensures the repository has full control
            //       over how data is retrieved from the store, returning IQueryable would allow consumers
            //       to add additional operators and affect the query sent to the store.
            return this.objectSet.ToList();
        }

        /// <summary>
        /// Gets the longest serving employees
        /// </summary>
        /// <param name="quantity">The number of employees to return</param>
        /// <returns>Enumerable of the longest serving employees</returns>
        public IEnumerable<Employee> GetLongestServingEmployees(int quantity)
        {
            // NOTE: When running against a fake object set the sort on tenure will happen in memory
            //       When running against EF the Model Defined Function declared in EmployeeModel.edmx
            //       will be used and the sort will be processed in the store
            return this.objectSet
                .Where(e => e.TerminationDate == null)
                .OrderByDescending(e => GetTenure(e))
                .Take(quantity)
                .ToList();
        }

        /// <summary>
        /// Calculates the duration of employment of an employee at the comapny
        /// </summary>
        /// <param name="employee">The employee to calculate tenure for</param>
        /// <returns>Tenure expressed in years</returns>
        [EdmFunction("EmployeeTracker.EntityFramework", "GetTenure")]
        private static int GetTenure(Employee employee)
        {
            // NOTE: The body for this method is included to facilitate running against in-memory fakes
            //       EF does not require an implementation, see notes in GetLongestServingEmployees()
            DateTime endDate = employee.TerminationDate ?? DateTime.Today;
            return endDate.Subtract(employee.HireDate).Days / 365;
        }
    }
}
