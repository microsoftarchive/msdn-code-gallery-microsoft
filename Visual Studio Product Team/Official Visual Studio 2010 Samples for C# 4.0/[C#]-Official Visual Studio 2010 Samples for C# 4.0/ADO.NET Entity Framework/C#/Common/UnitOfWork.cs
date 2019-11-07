// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Common
{
    using System;
    using System.Globalization;
    using System.Linq;
    using EmployeeTracker.Model;

    /// <summary>
    /// Encapsulates changes to underlying data stored in an EmployeeContext
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The underlying context tracking changes
        /// </summary>
        private IEmployeeContext underlyingContext;

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class.
        /// Changes registered in the UnitOfWork are recorded in the supplied context
        /// </summary>
        /// <param name="context">The underlying context for this UnitOfWork</param>
        public UnitOfWork(IEmployeeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.underlyingContext = context;
        }

        /// <summary>
        /// Save all pending changes in this UnitOfWork
        /// </summary>
        public void Save()
        {
            this.underlyingContext.Save();
        }

        /// <summary>
        /// Creates a new instance of the specified object type
        /// NOTE: This pattern is used to allow the use of change tracking proxies
        ///       when running against the Entity Framework.
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        /// <returns>The newly created object</returns>
        public T CreateObject<T>() where T : class
        {
            return this.underlyingContext.CreateObject<T>();
        }

        /// <summary>
        /// Registers the addition of a new department
        /// </summary>
        /// <param name="department">The department to add</param>
        /// <exception cref="InvalidOperationException">Thrown if department is already added to UnitOfWork</exception>
        public void AddDepartment(Department department)
        {
            if (department == null)
            {
                throw new ArgumentNullException("department");
            }
            
            this.CheckEntityDoesNotBelongToUnitOfWork(department);
            this.underlyingContext.Departments.AddObject(department);
        }

        /// <summary>
        /// Registers the addition of a new employee
        /// </summary>
        /// <param name="employee">The employee to add</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is already added to UnitOfWork</exception>
        public void AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            this.CheckEntityDoesNotBelongToUnitOfWork(employee);
            this.underlyingContext.Employees.AddObject(employee);
        }

        /// <summary>
        /// Registers the addition of a new contact detail
        /// </summary>
        /// <param name="employee">The employee to add the contact detail to</param>
        /// <param name="detail">The contact detail to add</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        /// <exception cref="InvalidOperationException">Thrown if contact detail is already added to UnitOfWork</exception>
        public void AddContactDetail(Employee employee, ContactDetail detail)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            if (detail == null)
            {
                throw new ArgumentNullException("detail");
            }

            this.CheckEntityDoesNotBelongToUnitOfWork(detail);
            this.CheckEntityBelongsToUnitOfWork(employee);

            this.underlyingContext.ContactDetails.AddObject(detail);
            employee.ContactDetails.Add(detail);
        }

        /// <summary>
        /// Registers the removal of an existing department
        /// </summary>
        /// <param name="department">The department to remove</param>
        /// <exception cref="InvalidOperationException">Thrown if department is not tracked by this UnitOfWork</exception>
        public void RemoveDepartment(Department department)
        {
            if (department == null)
            {
                throw new ArgumentNullException("department");
            }

            this.CheckEntityBelongsToUnitOfWork(department);
            foreach (var emp in department.Employees.ToList())
            {
                emp.Department = null;
            }

            this.underlyingContext.Departments.DeleteObject(department);
        }

        /// <summary>
        /// Registers the removal of an existing employee
        /// </summary>
        /// <param name="employee">The employee to remove</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        public void RemoveEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            this.CheckEntityBelongsToUnitOfWork(employee);
            employee.Manager = null;
            foreach (var e in employee.Reports.ToList())
            {
                e.Manager = null;
            }

            this.underlyingContext.Employees.DeleteObject(employee);
        }

        /// <summary>
        /// Registers the removal of an existing contact detail
        /// </summary>
        /// <param name="employee">The employee to remove the contact detail from</param>
        /// <param name="detail">The contact detail to remove</param>
        /// <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        /// <exception cref="InvalidOperationException">Thrown if contact detail is not tracked by this UnitOfWork</exception>
        public void RemoveContactDetail(Employee employee, ContactDetail detail)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            if (detail == null)
            {
                throw new ArgumentNullException("detail");
            }

            this.CheckEntityBelongsToUnitOfWork(detail);
            this.CheckEntityBelongsToUnitOfWork(employee);
            if (!employee.ContactDetails.Contains(detail))
            {
                throw new InvalidOperationException("The supplied ContactDetail does not belong to the supplied Employee");
            }

            employee.ContactDetails.Remove(detail);
            this.underlyingContext.ContactDetails.DeleteObject(detail);
        }

        /// <summary>
        /// Verifies that the specified entity is tracked in this UnitOfWork
        /// </summary>
        /// <param name="entity">The object to search for</param>
        /// <exception cref="InvalidOperationException">Thrown if object is not tracked by this UnitOfWork</exception>
        private void CheckEntityBelongsToUnitOfWork(object entity)
        {
            if (!this.underlyingContext.IsObjectTracked(entity))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The supplied {0} is not part of this Unit of Work.", entity.GetType().Name));
            }
        }

        /// <summary>
        /// Verifies that the specified entity is not tracked in this UnitOfWork
        /// </summary>
        /// <param name="entity">The object to search for</param>
        /// <exception cref="InvalidOperationException">Thrown if object is tracked by this UnitOfWork</exception>
        private void CheckEntityDoesNotBelongToUnitOfWork(object entity)
        {
            if (this.underlyingContext.IsObjectTracked(entity))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The supplied {0} is already part of this Unit of Work.", entity.GetType().Name));
            }
        }
    }
}
