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
    /// ViewModel for managing Departments within the company
    /// </summary>
    public class DepartmentWorkspaceViewModel : ViewModelBase
    {
        /// <summary>
        /// The deprtment currently selected in the workspace
        /// </summary>
        private DepartmentViewModel currentDepartment;

        /// <summary>
        /// UnitOfWork for managing changes
        /// </summary>
        private IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the DepartmentWorkspaceViewModel class.
        /// </summary>
        /// <param name="departments">The departments to be managed</param>
        /// <param name="unitOfWork">UnitOfWork for managing changes</param>
        public DepartmentWorkspaceViewModel(ObservableCollection<DepartmentViewModel> departments, IUnitOfWork unitOfWork)
        {
            if (departments == null)
            {
                throw new ArgumentNullException("departments");
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            this.unitOfWork = unitOfWork;
            this.AllDepartments = departments;
            this.CurrentDepartment = this.AllDepartments.Count > 0 ? this.AllDepartments[0] : null;

            // Re-act to any changes from outside this ViewModel
            this.AllDepartments.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null && e.OldItems.Contains(this.CurrentDepartment))
                {
                    this.CurrentDepartment = null;
                }
            };

            this.AddDepartmentCommand = new DelegateCommand((o) => this.AddDepartment());
            this.DeleteDepartmentCommand = new DelegateCommand((o) => this.DeleteCurrentDepartment(), (o) => this.CurrentDepartment != null);
        }

        /// <summary>
        /// Gets the command for adding a new department
        /// </summary>
        public ICommand AddDepartmentCommand { get; private set; }

        /// <summary>
        /// Gets the command for deleting the current department
        /// </summary>
        public ICommand DeleteDepartmentCommand { get; private set; }

        /// <summary>
        /// Gets all departments whithin the company
        /// </summary>
        public ObservableCollection<DepartmentViewModel> AllDepartments { get; private set; }

        /// <summary>
        /// Gets or sets the deprtment currently selected in the workspace
        /// </summary>
        public DepartmentViewModel CurrentDepartment
        {
            get
            {
                return this.currentDepartment;
            }

            set
            {
                this.currentDepartment = value;
                this.OnPropertyChanged("CurrentDepartment");
            }
        }

        /// <summary>
        /// Handles addition a new department to the workspace and model
        /// </summary>
        private void AddDepartment()
        {
            Department dep = this.unitOfWork.CreateObject<Department>();
            this.unitOfWork.AddDepartment(dep);

            DepartmentViewModel vm = new DepartmentViewModel(dep);
            this.AllDepartments.Add(vm);
            this.CurrentDepartment = vm;
        }

        /// <summary>
        /// Handles deletion of the current department
        /// </summary>
        private void DeleteCurrentDepartment()
        {
            this.unitOfWork.RemoveDepartment(this.CurrentDepartment.Model);
            this.AllDepartments.Remove(this.CurrentDepartment);
            this.CurrentDepartment = null;
        }
    }
}
