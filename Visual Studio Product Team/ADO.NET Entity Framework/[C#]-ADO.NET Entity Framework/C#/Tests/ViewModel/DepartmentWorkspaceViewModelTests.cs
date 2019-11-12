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
    using Tests;

    /// <summary>
    /// Unit tests for DepartmentWorkspaceViewModel
    /// </summary>
    [TestClass]
    public class DepartmentWorkspaceViewModelTests
    {
        /// <summary>
        /// Verify creation of a workspace with no data
        /// </summary>
        [TestMethod]
        public void InitializeWithEmptyData()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                ObservableCollection<DepartmentViewModel> departments = new ObservableCollection<DepartmentViewModel>();
                DepartmentWorkspaceViewModel vm = new DepartmentWorkspaceViewModel(departments, unit);

                Assert.IsNull(vm.CurrentDepartment, "Current department should not be set if there are no department.");
                Assert.AreSame(departments, vm.AllDepartments, "ViewModel should expose the same instance of the collection so that changes outside the ViewModel are reflected.");
                Assert.IsNotNull(vm.AddDepartmentCommand, "AddDepartmentCommand should be initialized");
                Assert.IsNotNull(vm.DeleteDepartmentCommand, "DeleteDepartmentCommand should be initialized");
            }
        }

        /// <summary>
        /// Verify creation of a workspace with data
        /// </summary>
        [TestMethod]
        public void InitializeWithData()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                ObservableCollection<DepartmentViewModel> departments = new ObservableCollection<DepartmentViewModel>(ctx.Departments.Select(d => new DepartmentViewModel(d)));
                DepartmentWorkspaceViewModel vm = new DepartmentWorkspaceViewModel(departments, unit);

                Assert.IsNotNull(vm.CurrentDepartment, "Current department should be set if there are departments.");
                Assert.AreSame(departments, vm.AllDepartments, "ViewModel should expose the same instance of the collection so that changes outside the ViewModel are reflected.");
                Assert.IsNotNull(vm.AddDepartmentCommand, "AddDepartmentCommand should be initialized");
                Assert.IsNotNull(vm.DeleteDepartmentCommand, "DeleteDepartmentCommand should be initialized");
            }
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void CheckNullArgumentExceptions()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Utilities.CheckNullArgumentException(() => { new DepartmentWorkspaceViewModel(null, unit); }, "departments", "ctor");
                Utilities.CheckNullArgumentException(() => { new DepartmentWorkspaceViewModel(new ObservableCollection<DepartmentViewModel>(), null); }, "unitOfWork", "ctor");
            }
        }

        /// <summary>
        /// Verify current department getter and setter
        /// </summary>
        [TestMethod]
        public void CurrentDepartment()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                string lastProperty;
                vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

                lastProperty = null;
                vm.CurrentDepartment = null;
                Assert.IsNull(vm.CurrentDepartment, "CurrentDepartment should have been nulled.");
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged when set to null.");

                lastProperty = null;
                var department = vm.AllDepartments.First();
                vm.CurrentDepartment = department;
                Assert.AreSame(department, vm.CurrentDepartment, "CurrentDepartment has not been set to specified value.");
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged when set to a value.");
            }
        }

        /// <summary>
        /// Verify additions to department collection are reflected
        /// </summary>
        [TestMethod]
        public void ExternalAddToDepartmentCollection()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                DepartmentViewModel currentDepartment = vm.CurrentDepartment;
                DepartmentViewModel newDepartment = new DepartmentViewModel(new Department());

                vm.AllDepartments.Add(newDepartment);
                Assert.IsTrue(vm.AllDepartments.Contains(newDepartment), "New department should have been added to AllDepartments.");
                Assert.AreSame(currentDepartment, vm.CurrentDepartment, "CurrentDepartment should not have changed.");
                Assert.IsFalse(ctx.IsObjectTracked(newDepartment.Model), "ViewModel is not responsible for adding departments created externally.");
            }
        }

        /// <summary>
        /// Verify removals from department collection are reflected
        /// When current department is remains in collection
        /// </summary>
        [TestMethod]
        public void ExternalRemoveDepartmentFromCollection()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                DepartmentViewModel currentDepartment = vm.AllDepartments.First();
                DepartmentViewModel toDelete = vm.AllDepartments.Skip(1).First();
                vm.CurrentDepartment = currentDepartment;

                vm.AllDepartments.Remove(toDelete);
                Assert.IsFalse(vm.AllDepartments.Contains(toDelete), "Department should have been removed from AllDepartments.");
                Assert.AreSame(currentDepartment, vm.CurrentDepartment, "CurrentDepartment should not have changed.");
                Assert.IsTrue(ctx.IsObjectTracked(toDelete.Model), "ViewModel is not responsible for deleting departments removed externally.");
            }
        }

        /// <summary>
        /// Verify removals from department collection are reflected
        /// When current department is removed
        /// </summary>
        [TestMethod]
        public void ExternalRemoveSelectedDepartmentFromCollection()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                DepartmentViewModel currentDepartment = vm.CurrentDepartment;

                string lastProperty = null;
                vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

                vm.AllDepartments.Remove(currentDepartment);
                Assert.IsFalse(vm.AllDepartments.Contains(currentDepartment), "Department should have been removed from AllDepartments.");
                Assert.IsNull(vm.CurrentDepartment, "CurrentDepartment should have been nulled as it was removed from the collection.");
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged.");
                Assert.IsTrue(ctx.IsObjectTracked(currentDepartment.Model), "ViewModel is not responsible for deleting departments removed externally.");
            }
        }

        /// <summary>
        /// Verify department delete command is only available when a department is selected
        /// </summary>
        [TestMethod]
        public void DeleteCommandOnlyAvailableWhenDepartmentSelected()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                vm.CurrentDepartment = null;
                Assert.IsFalse(vm.DeleteDepartmentCommand.CanExecute(null), "Delete command should be disabled when no department is selected.");

                vm.CurrentDepartment = vm.AllDepartments.First();
                Assert.IsTrue(vm.DeleteDepartmentCommand.CanExecute(null), "Delete command should be enabled when department is selected.");
            }
        }

        /// <summary>
        /// Cerify department can be deleted
        /// </summary>
        [TestMethod]
        public void DeleteDepartment()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                vm.CurrentDepartment = vm.AllDepartments.First();
                DepartmentViewModel toDelete = vm.CurrentDepartment;
                int originalCount = vm.AllDepartments.Count;

                string lastProperty = null;
                vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

                vm.DeleteDepartmentCommand.Execute(null);

                Assert.AreEqual(originalCount - 1, vm.AllDepartments.Count, "One department should have been removed from the AllDepartments property.");
                Assert.IsFalse(vm.AllDepartments.Contains(toDelete), "The selected department should have been removed.");
                Assert.IsFalse(ctx.IsObjectTracked(toDelete.Model), "The selected department has not been removed from the UnitOfWork.");
                Assert.IsNull(vm.CurrentDepartment, "No department should be selected after deletion.");
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged.");
            }
        }

        /// <summary>
        /// Verify a new department can be added when another department is selected
        /// </summary>
        [TestMethod]
        public void AddWhenDepartmentSelected()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                vm.CurrentDepartment = vm.AllDepartments.First();
                TestAddDepartment(ctx, vm);
            }
        }

        /// <summary>
        /// Verify a new department can be added when no department is selected
        /// </summary>
        [TestMethod]
        public void AddWhenDepartmentNotSelected()
        {
            using (FakeEmployeeContext ctx = BuildContextWithData())
            {
                DepartmentWorkspaceViewModel vm = BuildViewModel(ctx);

                vm.CurrentDepartment = null;
                TestAddDepartment(ctx, vm);
            }
        }

        /// <summary>
        /// Verifies addition of department to workspace and unit of work
        /// </summary>
        /// <param name="ctx">Context the department should get added to</param>
        /// <param name="vm">Workspace to add department to</param>
        private static void TestAddDepartment(FakeEmployeeContext ctx, DepartmentWorkspaceViewModel vm)
        {
            List<DepartmentViewModel> originalDepartments = vm.AllDepartments.ToList();

            string lastProperty = null;
            vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

            Assert.IsTrue(vm.AddDepartmentCommand.CanExecute(null), "Add command should always be enabled.");
            vm.AddDepartmentCommand.Execute(null);

            Assert.AreEqual(originalDepartments.Count + 1, vm.AllDepartments.Count, "One new department should have been added to the AllDepartments property.");
            Assert.IsFalse(originalDepartments.Contains(vm.CurrentDepartment), "The new department should be selected.");
            Assert.IsNotNull(vm.CurrentDepartment, "The new department should be selected.");
            Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged.");
            Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentDepartment.Model), "The new department has not been added to the context.");
        }

        /// <summary>
        /// Build a fake context with seeded data
        /// </summary>
        /// <returns>The populated context</returns>
        private static FakeEmployeeContext BuildContextWithData()
        {
            Department d1 = new Department();
            Department d2 = new Department();

            DepartmentViewModel dvm1 = new DepartmentViewModel(d1);
            DepartmentViewModel dvm2 = new DepartmentViewModel(d2);

            return new FakeEmployeeContext(new Employee[] { }, new Department[] { d1, d2 });
        }

        /// <summary>
        /// Creates a ViewModel based on a fake context
        /// </summary>
        /// <param name="ctx">Context to base view model on</param>
        /// <returns>The new ViewModel</returns>
        private static DepartmentWorkspaceViewModel BuildViewModel(FakeEmployeeContext ctx)
        {
            UnitOfWork unit = new UnitOfWork(ctx);
            ObservableCollection<DepartmentViewModel> departments = new ObservableCollection<DepartmentViewModel>(ctx.Departments.Select(d => new DepartmentViewModel(d)));
            DepartmentWorkspaceViewModel vm = new DepartmentWorkspaceViewModel(departments, unit);
            return vm;
        }
    }
}
