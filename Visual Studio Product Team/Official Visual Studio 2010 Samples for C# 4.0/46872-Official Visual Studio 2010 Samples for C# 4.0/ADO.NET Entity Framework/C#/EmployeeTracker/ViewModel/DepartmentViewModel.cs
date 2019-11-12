// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel.Helpers;

    /// <summary>
    /// ViewModel of an individual Department
    /// </summary>
    public class DepartmentViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the DepartmentViewModel class.
        /// </summary>
        /// <param name="department">The underlying Department this ViewModel is to be based on</param>
        public DepartmentViewModel(Department department)
        {
            if (department == null)
            {
                throw new ArgumentNullException("department");
            }

            this.Model = department;
        }

        /// <summary>
        /// Gets the underlying Department this ViewModel is based on
        /// </summary>
        public Department Model { get; private set; }

        /// <summary>
        /// Gets or sets the name of this department
        /// </summary>
        public string DepartmentName
        {
            get
            {
                return this.Model.DepartmentName;
            }

            set
            {
                this.Model.DepartmentName = value;
                this.OnPropertyChanged("DepartmentName");
            }
        }

        /// <summary>
        /// Gets or sets the code of this department
        /// </summary>
        public string DepartmentCode
        {
            get
            {
                return this.Model.DepartmentCode;
            }

            set
            {
                this.Model.DepartmentCode = value;
                this.OnPropertyChanged("DepartmentCode");
            }
        }

        /// <summary>
        /// Gets or sets the date this department was last audited on
        /// </summary>
        public DateTime? LastAudited
        {
            get
            {
                return this.Model.LastAudited;
            }

            set
            {
                this.Model.LastAudited = value;
                this.OnPropertyChanged("LastAudited");
            }
        }
    }
}
