// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Common
{
    using System;
    using EmployeeTracker.Model;

    /// <summary>
    /// Encapsulates changes to underlying data
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Save all pending changes in this UnitOfWork
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
        /// Registers the addition of a new department
        /// </summary>
        /// <param name="department">The department to add</param>
        /// <exception cref="InvalidOperationException">Thrown if department is already added to UnitOfWork</exception>
        void AddDepartment(Department department);

        /// <summary>
        /// Registers the addition of a new employee
        /// </summary>
        /// <param name="employee">The employee to add</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is already added to UnitOfWork</exception>
        void AddEmployee(Employee employee);

        /// <summary>
        /// Registers the addition of a new contact detail
        /// </summary>
        /// <param name="employee">The employee to add the contact detail to</param>
        /// <param name="detail">The contact detail to add</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        /// <exception cref="InvalidOperationException">Thrown if contact detail is already added to UnitOfWork</exception>
        void AddContactDetail(Employee employee, ContactDetail detail);

        /// <summary>
        /// Registers the removal of an existing department
        /// </summary>
        /// <param name="department">The department to remove</param>
        /// <exception cref="InvalidOperationException">Thrown if department is not tracked by this UnitOfWork</exception>
        void RemoveDepartment(Department department);

        /// <summary>
        /// Registers the removal of an existing employee
        /// </summary>
        /// <param name="employee">The employee to remove</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        void RemoveEmployee(Employee employee);

        /// <summary>
        /// Registers the removal of an existing contact detail
        /// </summary>
        /// <param name="employee">The employee to remove the contact detail from</param>
        /// <param name="detail">The contact detail to remove</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        /// <exception cref="InvalidOperationException">Thrown if contact detail is not tracked by this UnitOfWork</exception>
        void RemoveContactDetail(Employee employee, ContactDetail detail);
    }
}
