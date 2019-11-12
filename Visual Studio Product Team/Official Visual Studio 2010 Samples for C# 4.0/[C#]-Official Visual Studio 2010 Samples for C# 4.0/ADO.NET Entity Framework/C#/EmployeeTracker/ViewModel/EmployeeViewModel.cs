// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using EmployeeTracker.Common;
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel.Helpers;

    /// <summary>
    /// ViewModel of an individual <see cref="Employee"/>
    /// </summary>
    public class EmployeeViewModel : BasicEmployeeViewModel
    {
        /// <summary>
        /// The department currently assigned to this Employee
        /// </summary>
        private DepartmentViewModel department;

        /// <summary>
        /// The manager currently assigned to this Employee
        /// </summary>
        private EmployeeViewModel manager;

        /// <summary>
        /// The contact detail currently selected
        /// </summary>
        private ContactDetailViewModel currentContactDetail;

        /// <summary>
        /// UnitOfWork for managing changes
        /// </summary>
        private IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the EmployeeViewModel class.
        /// </summary>
        /// <param name="employee">The underlying Employee this ViewModel is to be based on</param>
        /// <param name="managerLookup">Existing collection of employees to use as a manager lookup</param>
        /// <param name="departmentLookup">Existing collection of departments to use as a department lookup</param>
        /// <param name="unitOfWork">UnitOfWork for managing changes</param>
        public EmployeeViewModel(Employee employee, ObservableCollection<EmployeeViewModel> managerLookup, ObservableCollection<DepartmentViewModel> departmentLookup, IUnitOfWork unitOfWork)
            : base(employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            this.unitOfWork = unitOfWork;
            this.ManagerLookup = managerLookup;
            this.DepartmentLookup = departmentLookup;

            // Build data structures for contact details
            this.ContactDetails = new ObservableCollection<ContactDetailViewModel>();
            foreach (var detail in employee.ContactDetails)
            {
                ContactDetailViewModel vm = ContactDetailViewModel.BuildViewModel(detail);
                if (vm != null)
                {
                    this.ContactDetails.Add(vm);
                }
            }

            // Re-act to any changes from outside this ViewModel
            this.DepartmentLookup.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null && e.OldItems.Contains(this.Department))
                {
                    this.Department = null;
                }
            };
            this.ManagerLookup.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null && e.OldItems.Contains(this.Manager))
                {
                    this.Manager = null;
                }
            };

            this.AddEmailAddressCommand = new DelegateCommand((o) => this.AddContactDetail<Email>());
            this.AddPhoneNumberCommand = new DelegateCommand((o) => this.AddContactDetail<Phone>());
            this.AddAddressCommand = new DelegateCommand((o) => this.AddContactDetail<Address>());
            this.DeleteContactDetailCommand = new DelegateCommand((o) => this.DeleteCurrentContactDetail(), (o) => this.CurrentContactDetail != null);
        }

        /// <summary>
        /// Gets the command for adding a new Email address
        /// </summary>
        public ICommand AddEmailAddressCommand { get; private set; }

        /// <summary>
        /// Gets the command for adding a new phone number
        /// </summary>
        public ICommand AddPhoneNumberCommand { get; private set; }

        /// <summary>
        /// Gets the command for adding a new address
        /// </summary>
        public ICommand AddAddressCommand { get; private set; }

        /// <summary>
        /// Gets the command for deleting the current employee
        /// </summary>
        public ICommand DeleteContactDetailCommand { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected contact detail
        /// </summary>
        public ContactDetailViewModel CurrentContactDetail
        {
            get
            {
                return this.currentContactDetail;
            }

            set
            {
                this.currentContactDetail = value;
                this.OnPropertyChanged("CurrentContactDetail");
            }
        }
        
        /// <summary>
        /// Gets or sets the department currently assigned to this Employee
        /// </summary>
        public DepartmentViewModel Department
        {
            get
            {
                // We need to reflect any changes made in the model so we check the current value before returning
                if (this.Model.Department == null)
                {
                    return null;
                }
                else if (this.department == null || this.department.Model != this.Model.Department)
                {
                    this.department = this.DepartmentLookup.Where(d => d.Model == this.Model.Department).SingleOrDefault();
                }

                return this.department;
            }

            set
            {
                this.department = value;
                this.Model.Department = (value == null) ? null : value.Model;
                this.OnPropertyChanged("Department");
            }
        }

        /// <summary>
        /// Gets or sets the manager currently assigned to this Employee
        /// </summary>
        public EmployeeViewModel Manager
        {
            get
            {
                // We need to reflect any changes made in the model so we check the current value before returning
                if (this.Model.Manager == null)
                {
                    return null;
                }
                else if (this.manager == null || this.manager.Model != this.Model.Manager)
                {
                    this.manager = this.ManagerLookup.Where(e => e.Model == this.Model.Manager).SingleOrDefault();
                }

                return this.manager;
            }

            set
            {
                this.manager = value;
                this.Model.Manager = (value == null) ? null : value.Model;
                this.OnPropertyChanged("Manager");
            }
        }

        /// <summary>
        /// Gets a collection of departments this employee could be assigned to
        /// </summary>
        public ObservableCollection<DepartmentViewModel> DepartmentLookup { get; private set; }

        /// <summary>
        /// Gets a collection of employees who could be this employee's manager
        /// </summary>
        public ObservableCollection<EmployeeViewModel> ManagerLookup { get; private set; }

        /// <summary>
        /// Gets the contact details on file for this employee
        /// </summary>
        public ObservableCollection<ContactDetailViewModel> ContactDetails { get; private set; }

        /// <summary>
        /// Handles addition a new contact detail to this employee
        /// </summary>
        /// <typeparam name="T">The type of contact detail to be added</typeparam>
        private void AddContactDetail<T>() where T : ContactDetail
        {
            ContactDetail detail = this.unitOfWork.CreateObject<T>();
            this.unitOfWork.AddContactDetail(this.Model, detail);

            ContactDetailViewModel vm = ContactDetailViewModel.BuildViewModel(detail);
            this.ContactDetails.Add(vm);
            this.CurrentContactDetail = vm;
        }

        /// <summary>
        /// Handles deletion of the current employee
        /// </summary>
        private void DeleteCurrentContactDetail()
        {
            this.unitOfWork.RemoveContactDetail(this.Model, this.CurrentContactDetail.Model);
            this.ContactDetails.Remove(this.CurrentContactDetail);
            this.CurrentContactDetail = null;
        }
    }
}
