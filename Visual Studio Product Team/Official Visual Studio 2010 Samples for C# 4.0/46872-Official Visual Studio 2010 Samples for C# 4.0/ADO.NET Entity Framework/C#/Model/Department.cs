// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a Department within the company
    /// </summary>
    public class Department
    {
        /// <summary>
        /// The Employees that belong to this Department
        /// </summary>
        private ICollection<Employee> employees;

        /// <summary>
        /// Initializes a new instance of the Department class.
        /// </summary>
        public Department()
        {
            // Wire up the employees collection to sync references
            // NOTE: When running against Entity Framework with change tracking proxies this logic will not get executed
            //       because the Employees property will get over-ridden and replaced with an EntityCollection<Employee>.
            //       The EntityCollection will perform this fixup instead.
            ObservableCollection<Employee> emps = new ObservableCollection<Employee>();
            this.employees = emps;
            emps.CollectionChanged += (sender, e) =>
            {
                // Set the reference on any employees being added to this department
                if (e.NewItems != null)
                {
                    foreach (Employee item in e.NewItems)
                    {
                        if (item.Department != this)
                        {
                            item.Department = this;
                        }
                    }
                }

                // Clear the reference on any employees being removed that still points to this department
                if (e.OldItems != null)
                {
                    foreach (Employee item in e.OldItems)
                    {
                        if (item.Department == this)
                        {
                            item.Department = null;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Gets or sets the Id of this Department
        /// </summary>
        public virtual int DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the Name of this Department
        /// </summary>
        public virtual string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the Code of this Department
        /// </summary>
        public virtual string DepartmentCode { get; set; }

        /// <summary>
        /// Gets or sets the date this Department was last audited
        /// </summary>
        public virtual DateTime? LastAudited { get; set; }

        /// <summary>
        /// Gets or sets the employees that belong to this Department
        /// Adding or removing will fixup the department property on the affected employee
        /// </summary>
        public virtual ICollection<Employee> Employees
        {
            get { return this.employees; }
            set { this.employees = value; }
        }
    }
}
