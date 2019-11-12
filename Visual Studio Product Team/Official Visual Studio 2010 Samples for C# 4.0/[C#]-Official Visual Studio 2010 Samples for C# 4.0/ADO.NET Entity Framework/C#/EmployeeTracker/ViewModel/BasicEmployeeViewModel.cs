// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using System.Globalization;
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel.Helpers;

    /// <summary>
    /// ViewModel of an individual Employee without associations
    /// EmployeeViewModel should be used if associations need to be displayed or edited
    /// </summary>
    public class BasicEmployeeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the BasicEmployeeViewModel class.
        /// </summary>
        /// <param name="employee">The underlying Employee this ViewModel is to be based on</param>
        public BasicEmployeeViewModel(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            this.Model = employee;
        }

        /// <summary>
        /// Gets the underlying Employee this ViewModel is based on
        /// </summary>
        public Employee Model { get; private set; }

        /// <summary>
        /// Gets or sets the first name of this employee
        /// </summary>
        public string FirstName
        {
            get
            {
                return this.Model.FirstName;
            }

            set
            {
                this.Model.FirstName = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        /// <summary>
        /// Gets or sets the title of this employee
        /// </summary>
        public string Title
        {
            get
            {
                return this.Model.Title;
            }

            set
            {
                this.Model.Title = value;
                this.OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Gets or sets the last name of this employee
        /// </summary>
        public string LastName
        {
            get
            {
                return this.Model.LastName;
            }

            set
            {
                this.Model.LastName = value;
                this.OnPropertyChanged("LastName");
            }
        }

        /// <summary>
        /// Gets or sets the position this employee holds in the company
        /// </summary>
        public string Position
        {
            get
            {
                return this.Model.Position;
            }

            set
            {
                this.Model.Position = value;
                this.OnPropertyChanged("Position");
            }
        }

        /// <summary>
        /// Gets or sets this employees date of birth
        /// </summary>
        public DateTime BirthDate
        {
            get
            {
                return this.Model.BirthDate;
            }

            set
            {
                this.Model.BirthDate = value;
                this.OnPropertyChanged("BirthDate");
            }
        }

        /// <summary>
        /// Gets or sets the date this employee was hired by the company
        /// </summary>
        public DateTime HireDate
        {
            get
            {
                return this.Model.HireDate;
            }

            set
            {
                this.Model.HireDate = value;
                this.OnPropertyChanged("HireDate");
            }
        }

        /// <summary>
        /// Gets or sets the date this employee left the company
        /// </summary>
        public DateTime? TerminationDate
        {
            get
            {
                return this.Model.TerminationDate;
            }

            set
            {
                this.Model.TerminationDate = value;
                this.OnPropertyChanged("TerminationDate");
            }
        }

        /// <summary>
        /// Gets the text to display when referring to this employee
        /// </summary>
        public string DisplayName
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", this.Model.LastName, this.Model.FirstName); }
        }

        /// <summary>
        /// Gets the text to display for a readonly version of this employees hire date
        /// </summary>
        public string DisplayHireDate
        {
            get { return this.Model.HireDate.ToShortDateString(); }
        }
    }
}
