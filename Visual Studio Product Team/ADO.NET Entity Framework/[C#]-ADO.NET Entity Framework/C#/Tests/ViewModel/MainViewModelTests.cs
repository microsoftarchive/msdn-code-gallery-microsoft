// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel
{
    using System.Linq;
    using EmployeeTracker.Common;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model.Interfaces;
    using EmployeeTracker.ViewModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for MainViewModel
    /// </summary>
    [TestClass]
    public class MainViewModelTests
    {
        /// <summary>
        /// Test creation of a new entry point
        /// </summary>
        [TestMethod]
        public void Initialization()
        {
            using (FakeEmployeeContext ctx = Generation.BuildFakeSession())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                IDepartmentRepository departmentRepository = new DepartmentRepository(ctx);
                IEmployeeRepository employeeRepository = new EmployeeRepository(ctx);

                int departmentCount = departmentRepository.GetAllDepartments().Count();
                int employeeCount = employeeRepository.GetAllEmployees().Count();

                MainViewModel main = new MainViewModel(unit, departmentRepository, employeeRepository);

                Assert.IsNotNull(main.DepartmentWorkspace, "Department workspace should be initialized.");
                Assert.AreEqual(
                    departmentCount,
                    main.DepartmentWorkspace.AllDepartments.Count,
                    "Department workspace should contain all departments from repository.");

                Assert.IsNotNull(main.EmployeeWorkspace, "Employee workspace should be initialized.");
                Assert.AreEqual(
                    employeeCount,
                    main.EmployeeWorkspace.AllEmployees.Count,
                    "Employee workspace should contain all employees from repository.");

                Assert.IsNotNull(main.LongServingEmployees, "Long serving employee list should be initialized.");
                Assert.AreEqual(5, main.LongServingEmployees.Count(), "Long serving employee list should be restricted to five employees.");

                Assert.AreSame(
                    main.DepartmentWorkspace.AllDepartments,
                    main.EmployeeWorkspace.AllEmployees[0].DepartmentLookup,
                    "A single instance of the department list should be used so that adds/removes flow throughout the application.");

                Assert.AreSame(
                   main.EmployeeWorkspace.AllEmployees,
                   main.EmployeeWorkspace.AllEmployees[0].ManagerLookup,
                   "A single instance of the employee list should be used so that adds/removes flow throughout the application.");

                Assert.IsNotNull(main.SaveCommand, "SaveCommand should be initialized.");
            }
        }

        /// <summary>
        /// Test save command causes a save on the backing unit of work
        /// </summary>
        [TestMethod]
        public void SaveCommand()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                IDepartmentRepository departmentRepository = new DepartmentRepository(ctx);
                IEmployeeRepository employeeRepository = new EmployeeRepository(ctx);
                MainViewModel main = new MainViewModel(unit, departmentRepository, employeeRepository);

                bool called = false;
                ctx.SaveCalled += (sender, e) => { called = true; };
                main.SaveCommand.Execute(null);
                Assert.IsTrue(called, "SaveCommand should result in save on underlying UnitOfWork.");
            }
        }
    }
}
