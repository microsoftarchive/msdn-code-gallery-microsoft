// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the IObjectSet implementation in FakeObjectSet
    /// </summary>
    [TestClass]
    public class FakeObjectSetTests
    {
        /// <summary>
        /// Verify data passed in constructor is available in enumerator
        /// </summary>
        [TestMethod]
        public void InitializationWithTestData()
        {
            Employee emp = new Employee();
            FakeObjectSet<Employee> set = new FakeObjectSet<Employee>(new Employee[] { emp });
            Assert.IsTrue(set.Contains(emp), "Constructor did not add supplied Employees to public Enumerator.");
        }

        /// <summary>
        /// Verify objects can be added to the set and that they are returned
        /// </summary>
        [TestMethod]
        public void AddObject()
        {
            Employee emp = new Employee();
            FakeObjectSet<Employee> set = new FakeObjectSet<Employee>();
            set.AddObject(emp);
            Assert.IsTrue(set.Contains(emp), "AddObject did not add supplied Employees to public Enumerator.");
        }

        /// <summary>
        /// Verify objects can be attached to the set and that they are returned
        /// </summary>
        [TestMethod]
        public void Attach()
        {
            Employee emp = new Employee();
            FakeObjectSet<Employee> set = new FakeObjectSet<Employee>();
            set.Attach(emp);
            Assert.IsTrue(set.Contains(emp), "Attach did not add supplied Employees to public Enumerator.");
        }

        /// <summary>
        /// Verify objects can be deleted from the set and that they are no longer returned
        /// </summary>
        [TestMethod]
        public void DeleteObject()
        {
            Employee emp = new Employee();
            FakeObjectSet<Employee> set = new FakeObjectSet<Employee>(new Employee[] { emp });
            set.DeleteObject(emp);
            Assert.IsFalse(set.Contains(emp), "DeleteObject did not remove supplied Employees to public Enumerator.");
        }

        /// <summary>
        /// Verify objects can be detached from the set and that they are no longer returned
        /// </summary>
        [TestMethod]
        public void Detach()
        {
            Employee emp = new Employee();
            FakeObjectSet<Employee> set = new FakeObjectSet<Employee>(new Employee[] { emp });
            set.Detach(emp);
            Assert.IsFalse(set.Contains(emp), "Detach did not remove supplied Employees to public Enumerator.");
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void NullArgumentChecks()
        {
            Utilities.CheckNullArgumentException(() => { new FakeObjectSet<Employee>(null); }, "testData", "ctor");

            FakeObjectSet<Employee> set = new FakeObjectSet<Employee>();
            Utilities.CheckNullArgumentException(() => { set.AddObject(null); }, "entity", "AddObject");
            Utilities.CheckNullArgumentException(() => { set.DeleteObject(null); }, "entity", "DeleteObject");
            Utilities.CheckNullArgumentException(() => { set.Attach(null); }, "entity", "Attach");
            Utilities.CheckNullArgumentException(() => { set.Detach(null); }, "entity", "Detach");
        }
    }
}
