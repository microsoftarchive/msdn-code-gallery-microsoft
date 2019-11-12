// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using EmployeeTracker.Common;
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel.Helpers;

    /// <summary>
    /// ViewModel for managing Employees within the company
    /// </summary>
    public class EmployeeWorkspaceViewModel : ViewModelBase
    {
        /// <summary>
        /// The employee currently selected in this workspace
        /// </summary>
        private EmployeeViewModel currentEmployee;

        /// <summary>
        /// UnitOfWork for managing changes
        /// </summary>
        private IUnitOfWork unitOfWork;

        /// <summary>
        /// Departments to be used for lookups
        /// </summary>
        private ObservableCollection<DepartmentViewModel> departmentLookup;

        /// <summary>
        /// Initializes a new instance of the EmployeeWorkspaceViewModel class.
        /// </summary>
        /// <param name="employees">Employees to be managed</param>
        /// <param name="departmentLookup">The departments to be used for lookups</param>
        /// <param name="unitOfWork">UnitOfWork for managing changes</param>
        public EmployeeWorkspaceViewModel(ObservableCollection<EmployeeViewModel> employees, ObservableCollection<DepartmentViewModel> departmentLookup, IUnitOfWork unitOfWork)
        {
            if (employees == null)
            {
                throw new ArgumentNullException("employees");
            }

            if (departmentLookup == null)
            {
                throw new ArgumentNullException("departmentLookup");
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            this.unitOfWork = unitOfWork;
            this.AllEmployees = employees;
            this.departmentLookup = departmentLookup;
            this.CurrentEmployee = employees.Count > 0 ? employees[0] : null;

            // Re-act to any changes from outside this ViewModel
            this.AllEmployees.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null && e.OldItems.Contains(this.CurrentEmployee))
                {
                    this.CurrentEmployee = null;
                }
            };

            this.AddEmployeeCommand = new DelegateCommand((o) => this.AddEmployee());
            this.DeleteEmployeeCommand = new DelegateCommand((o) => this.DeleteCurrentEmployee(), (o) => this.CurrentEmployee != null);
        }

        /// <summary>
        /// Gets the command for adding a new employee
        /// </summary>
        public ICommand AddEmployeeCommand { get; private set; }

        /// <summary>
        /// Gets the command for deleting the current employee
        /// </summary>
        public ICommand DeleteEmployeeCommand { get; private set; }

        /// <summary>
        /// Gets all employees whithin the company
        /// </summary>
        public ObservableCollection<EmployeeViewModel> AllEmployees { get; private set; }

        /// <summary>
        /// Gets or sets the employee currently selected in this workspace
        /// </summary>
        public EmployeeViewModel CurrentEmployee
        {
            get
            {
                return this.currentEmployee;
            }

            set
            {
                this.currentEmployee = value;
                this.OnPropertyChanged("CurrentEmployee");
            }
        }

        /// <summary>
        /// Handles addition a new employee to the workspace and model
        /// </summary>
        private void AddEmployee()
        {
            Employee emp = this.unitOfWork.CreateObject<Employee>();
            this.unitOfWork.AddEmployee(emp);

            EmployeeViewModel vm = new EmployeeViewModel(emp, this.AllEmployees, this.departmentLookup, this.unitOfWork);
            this.AllEmployees.Add(vm);
            this.CurrentEmployee = vm;
        }

        /// <summary>
        /// Handles deletion of the current employee
        /// </summary>
        private void DeleteCurrentEmployee()
        {
            this.unitOfWork.RemoveEmployee(this.CurrentEmployee.Model);
            this.AllEmployees.Remove(this.CurrentEmployee);
            this.CurrentEmployee = null;
        }
    }
}
