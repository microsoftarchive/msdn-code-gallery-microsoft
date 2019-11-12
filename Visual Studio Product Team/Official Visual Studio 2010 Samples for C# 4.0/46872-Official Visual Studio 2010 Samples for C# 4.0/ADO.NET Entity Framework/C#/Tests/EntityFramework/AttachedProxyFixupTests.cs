// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.EntityFramework
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using EmployeeTracker.EntityFramework;
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tests.Model;

    /// <summary>
    /// Tests the fixup behavior of Proxied versions of objects in the model that are attached to an ObjectContext
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Context is disposed in test cleanup.")]
    public class AttachedProxyFixupTests : FixupTestsBase
    {
        /// <summary>
        /// Context to use for proxy creation
        /// </summary>
        private EmployeeEntities context;

        /// <summary>
        /// Creates the resources needed for this test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            this.context = new EmployeeEntities();

            // Unit tests run without a database so we need to switch off LazyLoading
            this.context.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Releases any resourced used for this test run
        /// </summary>
        [TestCleanup]
        public void Teardown()
        {
            this.context.Dispose();
        }

        /// <summary>
        /// Returns a change tracking proxy derived from T and attached to an ObjectContext
        /// </summary>
        /// <typeparam name="T">The type to be created</typeparam>
        /// <returns>A new instance of type T</returns>
        protected override T CreateObject<T>()
        {
            T obj = this.context.CreateObject<T>();

            Employee e = obj as Employee;
            if (e != null)
            {
                this.context.Employees.AddObject(e);
                return obj;
            }

            Department d = obj as Department;
            if (d != null)
            {
                this.context.Departments.AddObject(d);
                return obj;
            }

            ContactDetail c = obj as ContactDetail;
            if (c != null)
            {
                this.context.ContactDetails.AddObject(c);
                return obj;
            }
           
            Assert.Fail(string.Format(CultureInfo.InvariantCulture, "Need to update AttachedProxyFixupTests.CreateObject to be able to attach objects of type {0}.", obj.GetType().Name));
            return null;
        }
    }
}
