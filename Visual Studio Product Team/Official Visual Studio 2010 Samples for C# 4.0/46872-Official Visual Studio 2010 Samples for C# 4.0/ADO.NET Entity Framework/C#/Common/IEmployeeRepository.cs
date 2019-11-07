// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Model.Interfaces
{
    using System.Collections.Generic;
    using EmployeeTracker.Model;

    /// <summary>
    /// Repository for retrieving employee data
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>
        /// All employees for the company
        /// </summary>
        /// <returns>Enumerable of all employees</returns>  
        IEnumerable<Employee> GetAllEmployees();

        /// <summary>
        /// Gets the longest serving employees
        /// </summary>
        /// <param name="quantity">The number of employees to return</param>
        /// <returns>Enumerable of the longest serving employees</returns>
        IEnumerable<Employee> GetLongestServingEmployees(int quantity);
    }
}
