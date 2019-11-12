// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using EmployeeTracker.Common;
    using EmployeeTracker.Model;
    using EmployeeTracker.Model.Interfaces;

    /// <summary>
    /// In-memory implementation of IEmployeeContext
    /// </summary>
    public sealed class FakeEmployeeContext : IEmployeeContext
    {
        /// <summary>
        /// Initializes a new instance of the FakeEmployeeContext class.
        /// The context contains empty initial data.
        /// </summary>
        public FakeEmployeeContext()
        {
            this.Employees = new FakeObjectSet<Employee>();
            this.Departments = new FakeObjectSet<Department>();
            this.ContactDetails = new FakeObjectSet<ContactDetail>();
        }

        /// <summary>
        /// Initializes a new instance of the FakeEmployeeContext class.
        /// The context contains the supplied initial data.
        /// </summary>
        /// <param name="employees">Employees to include in the context</param>
        /// <param name="departments">Departments to include in the context</param>
        public FakeEmployeeContext(IEnumerable<Employee> employees, IEnumerable<Department> departments)
        {
            if (employees == null)
            {
                throw new ArgumentNullException("employees");
            }

            if (departments == null)
            {
                throw new ArgumentNullException("departments");
            }

            this.Employees = new FakeObjectSet<Employee>(employees);
            this.Departments = new FakeObjectSet<Department>(departments);

            // Derive contact detail from supplied employee data
            this.ContactDetails = new FakeObjectSet<ContactDetail>();
            foreach (var emp in employees)
            {
                foreach (var det in emp.ContactDetails)
                {
                    this.ContactDetails.AddObject(det);
                }
            }
        }

        /// <summary>
        /// Raised whenever Save() is called
        /// </summary>
        public event EventHandler<EventArgs> SaveCalled;

        /// <summary>
        /// Raised whenever Dispose() is called
        /// </summary>
        public event EventHandler<EventArgs> DisposeCalled;

        /// <summary>
        /// Gets all employees tracked by this context
        /// </summary>
        public IObjectSet<Employee> Employees { get; private set; }

        /// <summary>
        /// Gets all departments tracked by this context
        /// </summary>
        public IObjectSet<Department> Departments { get; private set; }

        /// <summary>
        /// Gets all contact details tracked by this context
        /// </summary>
        public IObjectSet<ContactDetail> ContactDetails { get; private set; }

        /// <summary>
        /// Save all pending changes in this context
        /// </summary>
        public void Save()
        {
            this.OnSaveCalled(EventArgs.Empty);
        }

        /// <summary>
        /// Release all resources used by this context
        /// </summary>
        public void Dispose()
        {
            this.OnDisposeCalled(EventArgs.Empty);
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
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Checks if the specified object is tracked by this context
        /// </summary>
        /// <param name="entity">The object to search for</param>
        /// <returns>True if the object is tracked, false otherwise</returns>
        public bool IsObjectTracked(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return this.Employees.Contains(entity) 
                || this.Departments.Contains(entity) 
                || this.ContactDetails.Contains(entity);
        }

        /// <summary>
        /// Raises the SaveCalled event
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        private void OnSaveCalled(EventArgs e)
        {
            var handler = this.SaveCalled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the DisposeCalled event
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        private void OnDisposeCalled(EventArgs e)
        {
            var handler = this.DisposeCalled;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
