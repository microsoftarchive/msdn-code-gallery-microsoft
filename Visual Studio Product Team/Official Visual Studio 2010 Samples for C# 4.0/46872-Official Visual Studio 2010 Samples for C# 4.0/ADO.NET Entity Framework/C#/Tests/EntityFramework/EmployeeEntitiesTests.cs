// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.EntityFramework
{
    using System.Data.Objects.DataClasses;
    using EmployeeTracker.EntityFramework;
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the features that have been added to the EF Context
    /// </summary>
    [TestClass]
    public class EmployeeEntitiesTests
    {
        /// <summary>
        /// Verify that all classes in the model can be proxied by EF
        /// </summary>
        [TestMethod]
        public void AllEntitiesBecomeChangeTrackingProxies()
        {
            using (EmployeeEntities ctx = new EmployeeEntities())
            {
                object entity = ctx.CreateObject<Department>();
                Assert.IsInstanceOfType(entity, typeof(IEntityWithChangeTracker), "Department did not get proxied.");

                entity = ctx.CreateObject<Employee>();
                Assert.IsInstanceOfType(entity, typeof(IEntityWithChangeTracker), "Employee did not get proxied.");

                entity = ctx.CreateObject<Email>();
                Assert.IsInstanceOfType(entity, typeof(IEntityWithChangeTracker), "Email did not get proxied.");

                entity = ctx.CreateObject<Phone>();
                Assert.IsInstanceOfType(entity, typeof(IEntityWithChangeTracker), "Phone did not get proxied.");

                entity = ctx.CreateObject<Address>();
                Assert.IsInstanceOfType(entity, typeof(IEntityWithChangeTracker), "Address did not get proxied.");
            }
        }

        /// <summary>
        /// Verify IsObjectTracked for all entity types
        /// </summary>
        [TestMethod]
        public void IsObjectTracked()
        {
            using (EmployeeEntities ctx = new EmployeeEntities())
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
