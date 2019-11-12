// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Model.Interfaces
{
    using System.Collections.Generic;
    using EmployeeTracker.Model;

    /// <summary>
    /// Repository for retrieving department data
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// All departments for the company
        /// </summary>
        /// <returns>Enumerable of all departments</returns>
        IEnumerable<Department> GetAllDepartments();
    }
}
