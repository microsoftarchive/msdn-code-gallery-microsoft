// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.Common
{
    using System;
    using System.Linq;
    using EmployeeTracker.Common;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Verify change traking abilities of UnitOfWork
    /// </summary>
    [TestClass]
    public class UnitOfWorkTests
    {
        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void NullArgumentChecks()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                Utilities.CheckNullArgumentException(() => { new UnitOfWork(null); }, "context", "ctor");

                Utilities.CheckNullArgumentException(() => { unit.AddEmployee(null); }, "employee", "AddEmployee");
                Utilities.CheckNullArgumentException(() => { unit.AddDepartment(null); }, "department", "AddDepartment");
                Utilities.CheckNullArgumentException(() => { unit.AddContactDetail(new Employee(), null); }, "detail", "AddContactDetail");
                Utilities.CheckNullArgumentException(() => { unit.AddContactDetail(null, new Phone()); }, "employee", "AddContactDetail");

                Utilities.CheckNullArgumentException(() => { unit.RemoveEmployee(null); }, "employee", "RemoveEmployee");
                Utilities.CheckNullArgumentException(() => { unit.RemoveDepartment(null); }, "department", "RemoveDepartment");
                Utilities.CheckNullArgumentException(() => { unit.RemoveContactDetail(null, new Phone()); }, "employee", "RemoveContactDetail");
                Utilities.CheckNullArgumentException(() => { unit.RemoveContactDetail(new Employee(), null); }, "detail", "RemoveContactDetail");
            }
        }

        /// <summary>
        /// Verify CreateObject returns a valid object for types in model
        /// </summary>
        [TestMethod]
        public void CreateObject()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                object entity = unit.CreateObject<Department>();
                Assert.IsInstanceOfType(entity, typeof(Department), "Department did not get created.");

                entity = unit.CreateObject<Employee>();
                Assert.IsInstanceOfType(entity, typeof(Employee), "Employee did not get created.");

                entity = unit.CreateObject<Email>();
                Assert.IsInstanceOfType(entity, typeof(Email), "Email did not get created.");

                entity = unit.CreateObject<Phone>();
                Assert.IsInstanceOfType(entity, typeof(Phone), "Phone did not get created.");

                entity = unit.CreateObject<Address>();
                Assert.IsInstanceOfType(entity, typeof(Address), "Address did not get created.");
            }
        }

        /// <summary>
        /// Verify department gets added to underlying context
        /// </summary>
        [TestMethod]
        public void AddDepartment()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                Department dep = new Department();
                unit.AddDepartment(dep);
                Assert.IsTrue(ctx.Departments.Contains(dep), "Department was not added to underlying context.");
            }
        }

        /// <summary>
        /// Verify exception on adding a department that is already in the underlying context
        /// </summary>
        [TestMethod]
        public void AddDepartmentAlreadyInUnitOfWork()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                Department dep = new Department();
                unit.AddDepartment(dep);

                try
                {
                    unit.AddDepartment(dep);
                    Assert.Fail("Adding an Department that was already added did not throw.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual("The supplied Department is already part of this Unit of Work.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Verify employee gets added to underlying context
        /// </summary>
        [TestMethod]
        public void AddEmployee()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                Employee emp = new Employee();
                unit.AddEmployee(emp);
                Assert.IsTrue(ctx.Employees.Contains(emp), "Employee was not added to underlying context.");
            }
        }

        /// <summary>
        /// Verify exception on adding an employee that is already in the underlying context
        /// </summary>
        [TestMethod]
        public void AddEmployeeAlreadyInUnitOfWork()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                Employee emp = new Employee();
                unit.AddEmployee(emp);

                try
                {
                    unit.AddEmployee(emp);
                    Assert.Fail("Adding an Employee that was already added did not throw.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual("The supplied Employee is already part of this Unit of Work.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Verify contact detail gets added to underlying context
        /// Contact detail created by calling constructor on class
        /// </summary>
        [TestMethod]
        public void AddContactDetailFromDefaultConstructor()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Employee emp = new Employee();
                unit.AddEmployee(emp);

                ContactDetail cd = new Address();
                unit.AddContactDetail(emp, cd);
                Assert.IsTrue(ctx.ContactDetails.Contains(cd), "ContactDetail was not added to underlying context.");
            }
        }

        /// <summary>
        /// Verify exception on adding a contact detail that is already in the underlying context
        /// </summary>
        [TestMethod]
        public void AddContactDetailAlreadyInUnitOfWork()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                Employee emp = new Employee();
                ContactDetail detail = new Phone();
                unit.AddEmployee(emp);
                unit.AddContactDetail(emp, detail);

                try
                {
                    unit.AddContactDetail(emp, detail);
                    Assert.Fail("Adding an ContactDetail that was already added did not throw.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual("The supplied Phone is already part of this Unit of Work.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Verify exception on adding a contact detail to an employee not in the underlying context
        /// </summary>
        [TestMethod]
        public void AddContactDetailToEmployeeOutsideUnitOfWork()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Employee emp = new Employee();
                ContactDetail detail = new Email();

                try
                {
                    unit.AddContactDetail(emp, detail);
                    Assert.Fail("Adding a contact detail to an employee outside the Unit of Work did not throw.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual("The supplied Employee is not part of this Unit of Work.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Verify department can be removed from underlying context
        /// </summary>
        [TestMethod]
        public void RemoveDepartment()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Department dep = new Department();
                unit.AddDepartment(dep);

                unit.RemoveDepartment(dep);
                Assert.IsFalse(ctx.Departments.Contains(dep), "Department was not removed from underlying context.");
            }
        }

        /// <summary>
        /// Verify employees get department cleared when their department is deleted
        /// </summary>
        [TestMethod]
        public void RemoveDepartmentWithEmployees()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Department dep = new Department();
                Employee emp = new Employee();
                unit.AddDepartment(dep);
                unit.AddEmployee(emp);
                emp.Department = dep;

                unit.RemoveDepartment(dep);
                Assert.IsFalse(ctx.Departments.Contains(dep), "Department was not removed from underlying context.");
                Assert.IsNull(emp.Department, "Employee.Department property has not been nulled when deleting department.");
                Assert.IsNull(emp.DepartmentId, "Employee.DepartmentId property has not been nulled when deleting department.");
                Assert.AreEqual(0, dep.Employees.Count, "Department.Employees collection was not cleared when deleting department.");
            }
        }

        /// <summary>
        /// Verify exception when removing department not in current underlying context
        /// </summary>
        [TestMethod]
        public void RemoveDepartmentOutsideUnitOfWork()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                try
                {
                    unit.RemoveDepartment(new Department());
                    Assert.Fail("Removing a Department that was not added to Unit of Work did not throw.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual("The supplied Department is not part of this Unit of Work.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Verify employee can be removed from underlying context
        /// </summary>
        [TestMethod]
        public void RemoveEmployee()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Employee emp = new Employee();
                unit.AddEmployee(emp);

                unit.RemoveEmployee(emp);
                Assert.IsFalse(ctx.Employees.Contains(emp), "Employee was not removed from underlying context.");
            }
        }

        /// <summary>
        /// Verify employee can be removed from underlying context
        /// And that employee gets un-assigned from manager
        /// </summary>
        [TestMethod]
        public void RemoveEmployeeWithManager()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Employee emp = new Employee();
                Employee man = new Employee();
                unit.AddEmployee(emp);
                unit.AddEmployee(man);
                emp.Manager = man;

                unit.RemoveEmployee(emp);
                Assert.IsFalse(ctx.Employees.Contains(emp), "Employee was not removed from underlying context.");
                Assert.AreEqual(0, man.Reports.Count, "Employee was not removed from managers reports.");
                Assert.IsNull(emp.Manager, "Manager property on Employee was not cleared.");
            }
        }

        /// <summary>
        /// Verify employee can be removed from underlying context
        /// And that any reports get un-assigned
        /// </summary>
        [TestMethod]
        public void RemoveEmployeeWithReports()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                Employee emp = new Employee();
                Employee man = new Employee();
                unit.AddEmployee(emp);
                unit.AddEmployee(man);
                emp.Manager = man;

                unit.RemoveEmployee(man);
                Assert.IsFalse(ctx.Employees.Contains(man), "Employee was not removed from underlying context.");
                Assert.AreEqual(0, man.Reports.Count, "Employee was not removed from managers reports.");
                Assert.IsNull(emp.Manager, "Manager property on Employee was not cleared.");
            }
        }

        /// <summary>
        /// Verify exception when removing employee not in current underlying context
        /// </summary>
        [TestMethod]
        public void RemoveEmployeeOutsideUnitOfWork()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                try
                {
                    unit.RemoveEmployee(new Employee());
                    Assert.Fail("Removing an Employee that was not added to Unit of Work did not throw.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual("The supplied Employee is not part of this Unit of Work.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Verify contact detail can be removed from underlying context
        /// </summary>
        [TestMethod]
        public void RemoveContactDetail()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);

                Employee emp = new Employee();
                ContactDetail detail = new Phone();
                unit.AddEmployee(emp);
                unit.AddContactDetail(emp, detail);

                unit.RemoveContactDetail(emp, detail);
                Assert.IsFalse(ctx.ContactDetails.Contains(detail), "ContactDetail was not removed from underlying context.");
                Assert.IsFalse(
                    emp.ContactDetails.Contains(detail),
                    "ContactDetail is still in collection on Employee after being removed via Unit of Work.");
            }
        }

        /// <summary>
        /// Verify exception when removing contact detail not in underlying context
        /// </summary>
        [TestMethod]
        public void RemoveContactDetailOutsideUnitOfWork()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                UnitOfWork unit = new UnitOfWork(ctx);
                try
                {
                    unit.RemoveContactDetail(new Employee(), new Address());
                    Assert.Fail("Removing a ContactDetail that was not added to Unit of Work did not throw.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual("The supplied Address is not part of this Unit of Work.", ex.Message);
                }
            }
        }
    }
}
