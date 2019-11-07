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
    /// Represents a person employeed by the company
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Contact details belonging to this Employee
        /// </summary>
        private ICollection<ContactDetail> details;

        /// <summary>
        /// The Employees that report to this Employee
        /// </summary>
        private ICollection<Employee> reports;

        /// <summary>
        /// The Department this Employee belongs to
        /// </summary>
        private Department department;

        /// <summary>
        /// The manager of this Employee
        /// </summary>
        private Employee manager;

        /// <summary>
        /// Initializes a new instance of the Employee class.
        /// </summary>
        public Employee()
        {
            // NOTE: No fixup is required as this is a uni-directional navigation
            this.details = new ObservableCollection<ContactDetail>();

            // Wire up the reports collection to sync references
            // NOTE: When running against Entity Framework with change tracking proxies this logic will not get executed
            //       because the Reports property will get over-ridden and replaced with an EntityCollection<Employee>.
            //       The EntityCollection will perform this fixup instead.
            ObservableCollection<Employee> reps = new ObservableCollection<Employee>();
            this.reports = reps;
            reps.CollectionChanged += (sender, e) =>
            {
                // Set the reference on any employees being added to this manager
                if (e.NewItems != null)
                {
                    foreach (Employee item in e.NewItems)
                    {
                        if (item.Manager != this)
                        {
                            item.Manager = this;
                        }
                    }
                }

                // Clear the reference on any employees being removed that still points to this manager
                if (e.OldItems != null)
                {
                    foreach (Employee item in e.OldItems)
                    {
                        if (item.Manager == this)
                        {
                            item.Manager = null;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Gets or sets the Id of this Employee
        /// </summary>
        public virtual int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets this Employees title
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets this Employees first name
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Gets or sets this Employees last name
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Gets or sets this Employees position
        /// </summary>
        public virtual string Position { get; set; }

        /// <summary>
        /// Gets or sets the date this Employees was hired
        /// </summary>
        public virtual DateTime HireDate { get; set; }

        /// <summary>
        /// Gets or sets the date this Employees left the company
        /// Returns null if they are a current employee
        /// </summary>
        public virtual DateTime? TerminationDate { get; set; }

        /// <summary>
        /// Gets or sets this Employees date of birth
        /// </summary>
        public virtual DateTime BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the Id of the Department this Employees belongs to
        /// </summary>
        public virtual int? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the Id of this Employees manager
        /// </summary>
        public virtual int? ManagerId { get; set; }

        /// <summary>
        /// Gets or sets the contact details of this Employee
        /// No fixup is performed as this is a uni-directional navigation
        /// </summary>
        public virtual ICollection<ContactDetail> ContactDetails
        {
            get { return this.details; }
            set { this.details = value; }
        }

        /// <summary>
        /// Gets or sets the employees that report to this Employee
        /// Adding or removing will fixup the manager property on the affected employee
        /// </summary>
        public virtual ICollection<Employee> Reports
        {
            get { return this.reports; }
            set { this.reports = value; }
        }

        /// <summary>
        /// Gets or sets the Department this Employees belongs to
        /// Setting this property will fixup the collection on the original and new department
        /// </summary>
        public virtual Department Department
        {
            get
            {
                return this.department;
            }

            set
            {
                if (value != this.department)
                {
                    Department original = this.department;
                    this.department = value;

                    // Remove from old collection
                    if (original != null && original.Employees.Contains(this))
                    {
                        original.Employees.Remove(this);
                    }

                    // Add to new collection
                    if (value != null && !value.Employees.Contains(this))
                    {
                        value.Employees.Add(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets this Employees manager
        /// Setting this property will fixup the collection on the original and new manager
        /// </summary>
        public virtual Employee Manager
        {
            get
            {
                return this.manager;
            }

            set
            {
                if (value != this.manager)
                {
                    Employee original = this.manager;
                    this.manager = value;

                    // Remove from old collection
                    if (original != null && original.Reports.Contains(this))
                    {
                        original.Reports.Remove(this);
                    }

                    // Add to new collection
                    if (value != null && !value.Reports.Contains(this))
                    {
                        value.Reports.Add(this);
                    }
                }
            }
        }
    }
}
