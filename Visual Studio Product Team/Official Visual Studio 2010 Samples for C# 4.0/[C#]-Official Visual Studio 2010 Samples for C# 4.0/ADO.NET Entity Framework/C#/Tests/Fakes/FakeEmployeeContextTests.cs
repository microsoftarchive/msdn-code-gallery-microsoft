// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.Fakes
{
    using System.Collections.Generic;
    using System.Linq;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the funtionality of FakeEmployeeContextTests
    /// </summary>
    [TestClass]
    public class FakeEmployeeContextTests
    {
        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void NullArgumentChecks()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                Utilities.CheckNullArgumentException(() => { var c = new FakeEmployeeContext(null, new Department[0]); c.Dispose(); }, "employees", "ctor");
                Utilities.CheckNullArgumentException(() => { var c = new FakeEmployeeContext(new Employee[0], null); c.Dispose(); }, "departments", "ctor");
                Utilities.CheckNullArgumentException(() => { ctx.IsObjectTracked(null); }, "entity", "IsObjectTracked");
            }
        }

        /// <summary>
        /// Verify default constructor initializes ObjectSets
        /// </summary>
        [TestMethod]
        public void Initialization()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                Assert.IsNotNull(ctx.Employees, "Constructor did not not initialize Employees ObjectSet.");
                Assert.IsNotNull(ctx.Departments, "Constructor did not initialize Departments ObjectSet.");
                Assert.IsNotNull(ctx.ContactDetails, "Constructor did not initialize ContactDetails ObjectSet.");
            }
        }

        /// <summary>
        /// Verify data supplied to constructor is available in ObjectSets
        /// </summary>
        [TestMethod]
        public void InitializationWithSuppliedCollections()
        {
            Department dep = new Department();
            ContactDetail det = new Phone();
            Employee emp = new Employee { ContactDetails = new List<ContactDetail> { det } };

            using (FakeEmployeeContext ctx = new FakeEmployeeContext(new Employee[] { emp }, new Department[] { dep }))
            {
                Assert.IsTrue(ctx.Employees.Contains(emp), "Constructor did not add supplied Employees to public ObjectSet.");
                Assert.IsTrue(ctx.Departments.Contains(dep), "Constructor did not add supplied Departments to public ObjectSet.");
                Assert.IsTrue(ctx.ContactDetails.Contains(det), "Constructor did not add supplied ContactDetails to public ObjectSet.");
            }
        }

        /// <summary>
        /// Verify CreateObject returns a new instance of the type
        /// </summary>
        [TestMethod]
        public void CreateObject()
        {
            // Fake context should create the actual base type and not a type derived from it
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                object entity = ctx.CreateObject<Department>();
                Assert.AreEqual(typeof(Department), entity.GetType(), "Department did not get created.");

                entity = ctx.CreateObject<Employee>();
                Assert.AreEqual(typeof(Employee), entity.GetType(), "Employee did not get created.");

                entity = ctx.CreateObject<Email>();
                Assert.AreEqual(typeof(Email), entity.GetType(), "Email did not get created.");

                entity = ctx.CreateObject<Phone>();
                Assert.AreEqual(typeof(Phone), entity.GetType(), "Phone did not get created.");

                entity = ctx.CreateObject<Address>();
                Assert.AreEqual(typeof(Address), entity.GetType(), "Address did not get created.");
            }
        }

        /// <summary>
        /// Verify SaveCalled event gets raised when context is saved
        /// </summary>
        [TestMethod]
        public void SaveCalled()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                bool called = false;
                ctx.SaveCalled += (sender, e) => { called = true; };
                ctx.Save();
                Assert.IsTrue(called, "Save did not raise SaveCalled event.");
            }
        }

        /// <summary>
        /// Verify DisposedCalled event gets raised when context is disposed
        /// </summary>
        [TestMethod]
        public void DisposeCalled()
        {
            bool called = false;
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                ctx.DisposeCalled += (sender, e) => { called = true; };
            }

            Assert.IsTrue(called, "Dispose did not raise DisposeCalled event.");
        }

        /// <summary>
        /// Verify IsObjectTracked for all entity types
        /// </summary>
        [TestMethod]
        public void IsObjectTracked()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                Employee e = new Employee();
                Assert.IsFalse(ctx.IsObjectTracked(e), "IsObjectTracked should be false when entity is not in added.");
                ctx.Employees.AddObject(e);
                Assert.IsTrue(ctx.IsObjectTracked(e), "IsObjectTracked should be true when entity is added.");

                Department d = new Department();
                Assert.IsFalse(ctx.IsObjectTracked(d), "IsObjectTracked should be false when entity is not in added.");
                ctx.Departments.AddObject(d);
                Assert.IsTrue(ctx.IsObjectTracked(d), "IsObjectTracked should be true when entity is added.");

                ContactDetail c = new Phone();
                Assert.IsFalse(ctx.IsObjectTracked(c), "IsObjectTracked should be false when entity is not in added.");
                ctx.ContactDetails.AddObject(c);
                Assert.IsTrue(ctx.IsObjectTracked(c), "IsObjectTracked should be true when entity is added.");
            }
        }
    }
}
