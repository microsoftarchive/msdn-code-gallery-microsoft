// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.EntityFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using EmployeeTracker.Common;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests retrieval of data from a DepartmentObjectSetRepository
    /// </summary>
    [TestClass]
    public class DepartmentRepositoryTests
    {
        /// <summary>
        /// Verify GetAllDepartments returns all data from base ObjectSet
        /// </summary>
        [TestMethod]
        public void GetAllDepartments()
        {
            Department d1 = new Department();
            Department d2 = new Department();
            Department d3 = new Department();

            using (FakeEmployeeContext ctx = new FakeEmployeeContext(new Employee[] { }, new Department[] { d1, d2, d3 }))
            {
                DepartmentRepository rep = new DepartmentRepository(ctx);

                IEnumerable<Department> result = rep.GetAllDepartments();

                Assert.IsNotInstanceOfType(
                    result,
                    typeof(IQueryable),
                    "Repositories should not return IQueryable as this allows modification of the query that gets sent to the store. ");

                Assert.AreEqual(3, result.Count());
                Assert.IsTrue(result.Contains(d1));
                Assert.IsTrue(result.Contains(d2));
                Assert.IsTrue(result.Contains(d3));
            }
        }

        /// <summary>
        /// Verify GetAllDepartments returns an empty IEnumerable when no data is present
        /// </summary>
        [TestMethod]
        public void GetAllDepartmentsEmpty()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                DepartmentRepository rep = new DepartmentRepository(ctx);

                IEnumerable<Department> result = rep.GetAllDepartments();
                Assert.AreEqual(0, result.Count());
            }
        }

        /// <summary>
        /// Verify ArgumentNullException when invalid null parameters are specified
        /// </summary>
        [TestMethod]
        public void NullArgumentChecks()
        {
            Utilities.CheckNullArgumentException(() => { new DepartmentRepository(null); }, "context", "ctor");
        }
    }
}
