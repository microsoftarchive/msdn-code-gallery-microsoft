// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using EmployeeTracker.Common;
    using EmployeeTracker.Model.Interfaces;
    using EmployeeTracker.ViewModel.Helpers;

    /// <summary>
    /// ViewModel for accessing all data for the company
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// UnitOfWork for co-ordinating changes
        /// </summary>
        private IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork for co-ordinating changes</param>
        /// <param name="departmentRepository">Repository for querying department data</param>
        /// <param name="employeeRepository">Repository for querying employee data</param>
        public MainViewModel(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (departmentRepository == null)
            {
                throw new ArgumentNullException("departmentRepository");
            }

            if (employeeRepository == null)
            {
                throw new ArgumentNullException("employeeRepository");
            }

            this.unitOfWork = unitOfWork;

            // Build data structures to populate areas of the application surface
            ObservableCollection<EmployeeViewModel> allEmployees = new ObservableCollection<EmployeeViewModel>();
            ObservableCollection<DepartmentViewModel> allDepartments = new ObservableCollection<DepartmentViewModel>();

            foreach (var dep in departmentRepository.GetAllDepartments())
            {
                allDepartments.Add(new DepartmentViewModel(dep));
            }

            foreach (var emp in employeeRepository.GetAllEmployees())
            {
                allEmployees.Add(new EmployeeViewModel(emp, allEmployees, allDepartments, this.unitOfWork));
            }

            this.DepartmentWorkspace = new DepartmentWorkspaceViewModel(allDepartments, unitOfWork);
            this.EmployeeWorkspace = new EmployeeWorkspaceViewModel(allEmployees, allDepartments, unitOfWork);

            // Build non-interactive list of long serving employees
            List<BasicEmployeeViewModel> longServingEmployees = new List<BasicEmployeeViewModel>();
            foreach (var emp in employeeRepository.GetLongestServingEmployees(5))
            {
                longServingEmployees.Add(new BasicEmployeeViewModel(emp));
            }

            this.LongServingEmployees = longServingEmployees;

            this.SaveCommand = new DelegateCommand((o) => this.Save());
        }

        /// <summary>
        /// Gets the command to save all changes made in the current sessions UnitOfWork
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets the workspace for managing employees of the company
        /// </summary>
        public EmployeeWorkspaceViewModel EmployeeWorkspace { get; private set; }

        /// <summary>
        /// Gets the workspace for managing departments of the company
        /// </summary>
        public DepartmentWorkspaceViewModel DepartmentWorkspace { get; private set; }

        /// <summary>
        /// Gets the list of employees for the Loyalty Board
        /// </summary>
        public IEnumerable<BasicEmployeeViewModel> LongServingEmployees { get; private set; }

        /// <summary>
        /// Saves all changes made in the current sessions UnitOfWork
        /// </summary>
        private void Save()
        {
            this.unitOfWork.Save();
        }
    }
}
