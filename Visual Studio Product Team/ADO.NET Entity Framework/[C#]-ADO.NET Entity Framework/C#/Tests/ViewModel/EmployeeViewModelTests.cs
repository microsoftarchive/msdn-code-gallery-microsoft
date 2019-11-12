// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using EmployeeTracker.Common;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for EmployeeViewModel
    /// </summary>
    [TestClass]
    public class EmployeeViewModelTests
    {
        /// <summary>
        /// Verify getters and setters on ViewModel affect underlying data and notify changes
        /// </summary>
        [TestMethod]
        public void Initialization()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);

                Assert.IsNotNull(vm.ManagerLookup, "Manager lookup should be initialized.");
                Assert.IsTrue(vm.ManagerLookup.Contains(vm), "ViewModel must be capable of containing itself in the manager lookup.");

                Assert.IsNotNull(vm.DepartmentLookup, "Department lookup should be initialized.");
                Assert.IsNotNull(vm.AddAddressCommand, "AddAddressCommand should be initialized.");
                Assert.IsNotNull(vm.AddEmailAddressCommand, "AddEmailAddressCommand should be initialized.");
                Assert.IsNotNull(vm.AddPhoneNumberCommand, "AddPhoneNumberCommand should be initialized.");
                Assert.IsNotNull(vm.DeleteContactDetailCommand, "DeleteContactDetailCommand should be initialized.");
            }
        }

        /// <summary>
        /// Verify getters reflect changes in model
        /// </summary>
        [TestMethod]
        public void ReferencesGetAndSet()
        {
            // Scalar properties are inherited from BasicEmployeeViewModel and are already tested
            Department d1 = new Department();
            Department d2 = new Department();

            Employee e1 = new Employee();
            Employee e2 = new Employee();
            Employee employee = new Employee { Department = d1, Manager = e1 };
            employee.ContactDetails.Add(new Phone());
            employee.ContactDetails.Add(new Email());

            using (FakeEmployeeContext ctx = new FakeEmployeeContext(new Employee[] { e1, e2, employee }, new Department[] { d1, d2 }))
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                DepartmentViewModel dvm1 = new DepartmentViewModel(d1);
                DepartmentViewModel dvm2 = new DepartmentViewModel(d2);
                ObservableCollection<DepartmentViewModel> departments = new ObservableCollection<DepartmentViewModel> { dvm1, dvm2 };

                ObservableCollection<EmployeeViewModel> employees = new ObservableCollection<EmployeeViewModel>();
                EmployeeViewModel evm1 = new EmployeeViewModel(e1, employees, departments, unit);
                EmployeeViewModel evm2 = new EmployeeViewModel(e2, employees, departments, unit);
                EmployeeViewModel employeeViewModel = new EmployeeViewModel(employee, employees, departments, unit);
                employees.Add(evm1);
                employees.Add(evm2);
                employees.Add(employeeViewModel);

                // Test initial references are surfaced in ViewModel
                Assert.AreEqual(evm1, employeeViewModel.Manager, "ViewModel did not return ViewModel representing current manager.");
                Assert.AreEqual(e1, employeeViewModel.Manager.Model, "ViewModel did not return ViewModel representing current manager.");
                Assert.AreEqual(dvm1, employeeViewModel.Department, "ViewModel did not return ViewModel representing current department.");
                Assert.AreEqual(d1, employeeViewModel.Department.Model, "ViewModel did not return ViewModel representing current department.");
                Assert.AreEqual(2, employeeViewModel.ContactDetails.Count, "Contact details have not been populated on ViewModel.");

                // Test changing properties updates Model and raises PropertyChanged
                string lastProperty;
                employeeViewModel.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

                lastProperty = null;
                employeeViewModel.Department = dvm2;
                Assert.AreEqual("Department", lastProperty, "Setting Department property did not raise correct PropertyChanged event.");
                Assert.AreEqual(d2, employee.Department, "Setting Department property in ViewModel is not reflected in Model.");

                lastProperty = null;
                employeeViewModel.Manager = evm2;
                Assert.AreEqual("Manager", lastProperty, "Setting Manager property did not raise correct PropertyChanged event.");
                Assert.AreEqual(e2, employee.Manager, "Setting Manager property in ViewModel is not reflected in Model.");

                // Test ViewModel returns current value from model
                employee.Manager = e1;
                Assert.AreEqual(evm1, employeeViewModel.Manager, "ViewModel did not return correct manager when model was updated outside of ViewModel.");
                employee.Department = d1;
                Assert.AreEqual(dvm1, employeeViewModel.Department, "ViewModel did not return correct department when model was updated outside of ViewModel.");

                // Test ViewModel returns current value from model when set to null
                employee.Manager = null;
                Assert.AreEqual(null, employeeViewModel.Manager, "ViewModel did not return correct manager when model was updated outside of ViewModel.");
                employee.Department = null;
                Assert.AreEqual(null, employeeViewModel.Department, "ViewModel did not return correct department when model was updated outside of ViewModel.");
            }
        }

        /// <summary>
        /// Verify a new email address can be added
        /// </summary>
        [TestMethod]
        public void AddEmail()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);
                List<ContactDetailViewModel> originalDetails = vm.ContactDetails.ToList();

                Assert.IsTrue(vm.AddEmailAddressCommand.CanExecute(null), "AddEmailAddressCommand should always be enabled.");
                vm.AddEmailAddressCommand.Execute(null);

                Assert.IsNotNull(vm.CurrentContactDetail, "New email should be selected.");
                Assert.IsFalse(originalDetails.Contains(vm.CurrentContactDetail), "New email should be selected.");
                Assert.IsInstanceOfType(vm.CurrentContactDetail, typeof(EmailViewModel), "New contact should be an email.");
                Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentContactDetail.Model), "New email should have been added to context.");
                Assert.AreEqual(originalDetails.Count + 1, vm.ContactDetails.Count, "New email should have been added to AllContactDetails property.");
            }
        }

        /// <summary>
        /// Verify a new phone number can be added
        /// </summary>
        [TestMethod]
        public void AddPhone()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);
                List<ContactDetailViewModel> originalDetails = vm.ContactDetails.ToList();

                Assert.IsTrue(vm.AddPhoneNumberCommand.CanExecute(null), "AddPhoneNumberCommand should always be enabled.");
                vm.AddPhoneNumberCommand.Execute(null);

                Assert.IsNotNull(vm.CurrentContactDetail, "New phone should be selected.");
                Assert.IsFalse(originalDetails.Contains(vm.CurrentContactDetail), "New phone should be selected.");
                Assert.IsInstanceOfType(vm.CurrentContactDetail, typeof(PhoneViewModel), "New contact should be a phone.");
                Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentContactDetail.Model), "New phone should have been added to context.");
                Assert.AreEqual(originalDetails.Count + 1, vm.ContactDetails.Count, "New phone should have been added to AllContactDetails property.");
            }
        }

        /// <summary>
        /// Verify a new address can be added
        /// </summary>
        [TestMethod]
        public void AddAddress()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);
                List<ContactDetailViewModel> originalDetails = vm.ContactDetails.ToList();

                Assert.IsTrue(vm.AddAddressCommand.CanExecute(null), "AddAddressCommand should always be enabled.");
                vm.AddAddressCommand.Execute(null);

                Assert.IsNotNull(vm.CurrentContactDetail, "New address should be selected.");
                Assert.IsFalse(originalDetails.Contains(vm.CurrentContactDetail), "New address should be selected.");
                Assert.IsInstanceOfType(vm.CurrentContactDetail, typeof(AddressViewModel), "New contact should be an address.");
                Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentContactDetail.Model), "New address should have been added to context.");
                Assert.AreEqual(originalDetails.Count + 1, vm.ContactDetails.Count, "New address should have been added to AllContactDetails property.");
            }
        }

        /// <summary>
        /// Verify delete is only available when a contact is selected
        /// </summary>
        [TestMethod]
        public void DeleteContactDetailOnlyAvailableWhenDetailIsSelected()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);
                List<ContactDetailViewModel> originalDetails = vm.ContactDetails.ToList();


                vm.CurrentContactDetail = null;
                Assert.IsFalse(vm.DeleteContactDetailCommand.CanExecute(null), "DeleteContactDetailCommand should be disabled when no detail is selected.");

                vm.CurrentContactDetail = vm.ContactDetails.First();
                Assert.IsTrue(vm.DeleteContactDetailCommand.CanExecute(null), "DeleteContactDetailCommand should be enabled when a detail is selected.");
            }
        }

        /// <summary>
        /// Verify contact detail can be deleted
        /// </summary>
        [TestMethod]
        public void DeleteContactDetail()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);
                List<ContactDetailViewModel> originalDetails = vm.ContactDetails.ToList();


                ContactDetailViewModel toDelete = vm.ContactDetails.First();
                vm.CurrentContactDetail = toDelete;
                vm.DeleteContactDetailCommand.Execute(null);

                Assert.IsNull(vm.CurrentContactDetail, "No detail should be selected after deleting.");
                Assert.IsFalse(vm.ContactDetails.Contains(toDelete), "Detail should be removed from ContactDetails property.");
                Assert.IsFalse(ctx.IsObjectTracked(toDelete), "Detail should be deleted from context.");
            }
        }

        /// <summary>
        /// Verify additions to department collection are reflected
        /// </summary>
        [TestMethod]
        public void ExternalAddToDepartmentLookup()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);

                DepartmentViewModel currentDepartment = vm.Department;
                DepartmentViewModel newDepartment = new DepartmentViewModel(new Department());

                vm.DepartmentLookup.Add(newDepartment);
                Assert.IsTrue(vm.DepartmentLookup.Contains(newDepartment), "New department should have been added to DepartmentLookup.");
                Assert.AreSame(currentDepartment, vm.Department, "Assigned Department should not have changed.");
                Assert.IsFalse(ctx.IsObjectTracked(newDepartment.Model), "ViewModel is not responsible for adding departments created externally.");
            }
        }

        /// <summary>
        /// Verify removals from department collection are reflected
        /// When current department is remains in collection
        /// </summary>
        [TestMethod]
        public void ExternalRemoveDepartmentLookup()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);

                DepartmentViewModel currentDepartment = vm.DepartmentLookup.First();
                DepartmentViewModel toDelete = vm.DepartmentLookup.Skip(1).First();
                vm.Department = currentDepartment;

                vm.DepartmentLookup.Remove(toDelete);
                Assert.IsFalse(vm.DepartmentLookup.Contains(toDelete), "Department should have been removed from DepartmentLookup.");
                Assert.AreSame(currentDepartment, vm.Department, "Assigned Department should not have changed.");
                Assert.IsTrue(ctx.IsObjectTracked(toDelete.Model), "ViewModel is not responsible for deleting departments removed externally.");
            }
        }

        /// <summary>
        /// Verify removals from department collection are reflected
        /// When current department is removed
        /// </summary>
        [TestMethod]
        public void ExternalRemoveDepartmentLookupSelectedDepartment()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);

                DepartmentViewModel currentDepartment = vm.Department;

                string lastProperty = null;
                vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

                vm.DepartmentLookup.Remove(currentDepartment);
                Assert.IsFalse(vm.DepartmentLookup.Contains(currentDepartment), "Department should have been removed from DepartmentLookup.");
                Assert.IsNull(vm.Department, "Assigned Department should have been nulled as it was removed from the collection.");
                Assert.AreEqual("Department", lastProperty, "Department should have raised a PropertyChanged.");
                Assert.IsTrue(ctx.IsObjectTracked(currentDepartment.Model), "ViewModel is not responsible for deleting departments removed externally.");
            }
        }

        /// <summary>
        /// Verify additions to employee collection are reflected
        /// </summary>
        [TestMethod]
        public void ExternalAddToManagerLookup()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                EmployeeViewModel vm = BuildViewModel(ctx, unit);

                EmployeeViewModel currentManager = vm.Manager;
                EmployeeViewModel newManager = new EmployeeViewModel(new Employee(), vm.ManagerLookup, vm.DepartmentLookup, unit);

                vm.ManagerLookup.Add(newManager);
                Assert.IsTrue(vm.ManagerLookup.Contains(newManager), "New department should have been added to ManagerLookup.");
                Assert.AreSame(currentManager, vm.Manager, "Assigned Manager should not have changed.");
                Assert.IsFalse(ctx.IsObjectTracked(newManager.Model), "ViewModel is not responsible for adding Employees created externally.");
            }
        }

        /// <summary>
        /// Verify removals from employee collection are reflected
        /// When current manager is remains in collection
        /// </summary>
        [TestMethod]
        public void ExternalRemoveManagerLookup()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);

                EmployeeViewModel currentManager = vm.ManagerLookup.First();
                EmployeeViewModel toDelete = vm.ManagerLookup.Skip(1).First();
                vm.Manager = currentManager;

                vm.ManagerLookup.Remove(toDelete);
                Assert.IsFalse(vm.ManagerLookup.Contains(toDelete), "Employee should have been removed from ManagerLookup.");
                Assert.AreSame(currentManager, vm.Manager, "Assigned Manager should not have changed.");
                Assert.IsTrue(ctx.IsObjectTracked(toDelete.Model), "ViewModel is not responsible for deleting Employees removed externally.");
            }
        }

        /// <summary>
        /// Verify removals from employee collection are reflected
        /// When current manager is removed
        /// </summary>
        [TestMethod]
        public void ExternalRemoveManagerLookupSelectedManager()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                EmployeeViewModel vm = BuildViewModel(ctx);

                EmployeeViewModel currentManager = vm.Manager;

                string lastProperty = null;
                vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

                vm.ManagerLookup.Remove(currentManager);
                Assert.IsFalse(vm.ManagerLookup.Contains(currentManager), "Employee should have been removed from ManagerLookup.");
                Assert.IsNull(vm.Manager, "Assigned Manager should have been nulled as it was removed from the collection.");
                Assert.AreEqual("Manager", lastProperty, "Manager should have raised a PropertyChanged.");
                Assert.IsTrue(ctx.IsObjectTracked(currentManager.Model), "ViewModel is not responsible for deleting Employees removed externally.");
            }
        }

        /// <summary>
        /// Creates a fake context with seed data
        /// </summary>
        /// <returns>The fake context</returns>
        private static FakeEmployeeContext BuildContextWithData()
        {
            Department d1 = new Department();
            Department d2 = new Department();

            Employee e1 = new Employee { Department = d1 };
            Employee e2 = new Employee { Department = d1 };

            e1.Manager = e2;

            e1.ContactDetails.Add(new Phone());
            e1.ContactDetails.Add(new Email());
            e1.ContactDetails.Add(new Address());
            e2.ContactDetails.Add(new Phone());

            return new FakeEmployeeContext(new Employee[] { e1, e2 }, new Department[] { d1, d2 });
        }

        /// <summary>
        /// Creates a ViewModel based on a fake context
        /// </summary>
        /// <param name="ctx">Context to base view model on</param>
        /// <returns>The new ViewModel</returns>
        private static EmployeeViewModel BuildViewModel(FakeEmployeeContext ctx)
        {
            return BuildViewModel(ctx, new UnitOfWork(ctx));
        }

        /// <summary>
        /// Creates a ViewModel based on a fake context using an existing unit of work
        /// </summary>
        /// <param name="ctx">Context to base view model on</param>
        /// <param name="unit">Current unit of work</param>
        /// <returns>The new ViewModel</returns>
        private static EmployeeViewModel BuildViewModel(FakeEmployeeContext ctx, UnitOfWork unit)
        {
            ObservableCollection<DepartmentViewModel> departments = new ObservableCollection<DepartmentViewModel>(ctx.Departments.Select(d => new DepartmentViewModel(d)));
            ObservableCollection<EmployeeViewModel> employees = new ObservableCollection<EmployeeViewModel>();
            foreach (var e in ctx.Employees)
            {
                employees.Add(new EmployeeViewModel(e, employees, departments, unit));
            }

            return employees[0];
        }
    }
}
