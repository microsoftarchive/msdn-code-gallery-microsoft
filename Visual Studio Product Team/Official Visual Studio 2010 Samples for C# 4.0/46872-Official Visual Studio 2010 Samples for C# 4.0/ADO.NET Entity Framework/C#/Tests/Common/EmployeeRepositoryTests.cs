// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EmployeeTracker.Common;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests retrieval of data from an EmployeeObjectSetRepository
    /// </summary>
    [TestClass]
    public class EmployeeRepositoryTests
    {
        /// <summary>
        /// Verify GetAllEmployees returns all data from base ObjectSet
        /// </summary>
        [TestMethod]
        public void GetAllEmployees()
        {
            Employee e1 = new Employee();
            Employee e2 = new Employee();
            Employee e3 = new Employee();

            using (FakeEmployeeContext ctx = new FakeEmployeeContext(new Employee[] { e1, e2, e3 }, new Department[] { }))
            {
                EmployeeRepository rep = new EmployeeRepository(ctx);

                IEnumerable<Employee> result = rep.GetAllEmployees();

                Assert.IsNotInstanceOfType(
                    result,
                    typeof(IQueryable),
                    "Repositories should not return IQueryable as this allows modification of the query that gets sent to the store. ");

                Assert.AreEqual(3, result.Count());
                Assert.IsTrue(result.Contains(e1));
                Assert.IsTrue(result.Contains(e2));
                Assert.IsTrue(result.Contains(e3));
            }
        }

        /// <summary>
        /// Verify GetAllEmployees returns an empty IEnumerable when no data is present
        /// </summary>
        [TestMethod]
        public void GetAllEmployeesEmpty()
        {
            using (FakeEmployeeContext ctx = new FakeEmployeeContext())
            {
                EmployeeRepository rep = new EmployeeRepository(ctx);

                IEnumerable<Employee> result = rep.GetAllEmployees();
                Assert.AreEqual(0, result.Count());
            }
        }

        /// <summary>
        /// Verify GetLongestServingEmployees dorrectly filters and sorts data
        /// </summary>
        [TestMethod]
        public void GetLongestServingEmployees()
        {
            Employee e1 = new Employee { HireDate = new DateTime(2003, 1, 1) };
            Employee e2 = new Employee { HireDate = new DateTime(2001, 1, 1) };
            Employee e3 = new Employee { HireDate = new DateTime(2000, 1, 1) };
            Employee e4 = new Employee { HireDate = new DateTime(2002, 1, 1) };

            // The following employee verifies GetLongestServingEmployees does not return terminated employees
            Employee e5 = new Employee { HireDate = new DateTime(1999, 1, 1), TerminationDate = DateTime.Today };

            using (FakeEmployeeContext ctx = new FakeEmployeeContext(new Employee[] { e1, e2, e3, e4, e5 }, new Department[] { }))
            {
                EmployeeRepository rep = new EmployeeRepository(ctx);

                // Select a subset
                List<Employee> result = rep.GetLongestServingEmployees(2).ToList();
                Assert.AreEqual(2, result.Count, "Expected two items in result.");
                Assert.AreSame(e3, result[0], "Incorrect item at position 0.");
                Assert.AreSame(e2, result[1], "Incorrect item at position 1.");

                // Select more than are present
                result = rep.GetLongestServingEmployees(50).ToList();
                Assert.AreEqual(4, result.Count, "Expected four items in result.");
            }
        }

        /// <summary>
        /// Verify ArgumentNullException when invalid null parameters are specified
        /// </summary>
        [TestMethod]
        public void NullArgumentChecks()
        {
            Utilities.CheckNullArgumentException(() => { new EmployeeRepository(null); }, "context", "ctor");
        }
    }
}
