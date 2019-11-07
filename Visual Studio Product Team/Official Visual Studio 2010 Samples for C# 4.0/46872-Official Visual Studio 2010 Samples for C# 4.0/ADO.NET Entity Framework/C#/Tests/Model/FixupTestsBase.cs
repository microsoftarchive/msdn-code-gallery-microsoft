// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.Model
{
    using EmployeeTracker.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class for tests that verify fixup behavior between objects
    /// This used to test the Pure POCO and Entity Framework proxy objects
    /// to make sure they behave in the same manor.
    /// </summary>
    [TestClass]
    public abstract class FixupTestsBase
    {
        #region Department - Employees

        /// <summary>
        /// Add employee without department to a new departments employees collection
        /// </summary>
        [TestMethod]
        public void AddUnassignedEmployeeToDepartment()
        {
            Department dep = CreateObject<Department>();
            Employee emp = CreateObject<Employee>();

            dep.Employees.Add(emp);
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee was not added to collection on department.");
            Assert.AreSame(dep, emp.Department, "Department was not set on employee.");
        }

        /// <summary>
        /// Add employee with an existing department to a new departments employees collection
        /// </summary>
        [TestMethod]
        public void AddAssignedEmployeeToDepartment()
        {
            Employee emp = CreateObject<Employee>();
            Department depOriginal = CreateObject<Department>(); 
            Department depNew = CreateObject<Department>();
            depOriginal.Employees.Add(emp);

            depNew.Employees.Add(emp);
            Assert.IsFalse(depOriginal.Employees.Contains(emp), "Employee was not removed from collection on old department.");
            Assert.IsTrue(depNew.Employees.Contains(emp), "Employee was not added to collection on CreateObject<department.");
            Assert.AreSame(depNew, emp.Department, "Department was not set on employee.");
        }

        /// <summary>
        /// Remove an employee from a departments employees collection
        /// </summary>
        [TestMethod]
        public void RemoveEmployeeFromDepartment()
        {
            Department dep = CreateObject<Department>(); 
            Employee emp = CreateObject<Employee>();
            dep.Employees.Add(emp);

            dep.Employees.Remove(emp);
            Assert.IsFalse(dep.Employees.Contains(emp), "Employee was not removed from collection on department.");
            Assert.IsNull(emp.Department, "Department was not nulled on employee.");
        }

        /// <summary>
        /// Add an employee to a department they already belong to
        /// </summary>
        [TestMethod]
        public void AddEmployeeToSameDepartment()
        {
            Department dep = CreateObject<Department>(); 
            Employee emp = CreateObject<Employee>();
            dep.Employees.Add(emp);

            dep.Employees.Add(emp);
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee is not in collection on department.");
            Assert.AreSame(dep, emp.Department, "Department is not set on employee.");
        }

        /// <summary>
        /// Set the department on an employee that doesn't have a department assigned
        /// </summary>
        [TestMethod]
        public void SetDepartmentOnUnassignedEmployee()
        {
            Department dep = CreateObject<Department>(); 
            Employee emp = CreateObject<Employee>();

            emp.Department = dep;
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee was not added to collection on department.");
            Assert.AreSame(dep, emp.Department, "Department was not set on employee.");
        }

        /// <summary>
        /// Set the department on an employee that is assigned to a different department
        /// </summary>
        [TestMethod]
        public void SetDepartmentOnAssignedEmployee()
        {
            Employee emp = CreateObject<Employee>();
            Department depOriginal = CreateObject<Department>();
            Department depNew = CreateObject<Department>();
            emp.Department = depOriginal;

            emp.Department = depNew;
            Assert.IsFalse(depOriginal.Employees.Contains(emp), "Employee was not removed from collection on old department.");
            Assert.IsTrue(depNew.Employees.Contains(emp), "Employee was not added to collection on CreateObject<department.");
            Assert.AreSame(depNew, emp.Department, "Department was not set on employee.");
        }

        /// <summary>
        /// Clear the department on an employee
        /// </summary>
        [TestMethod]
        public void NullDepartmentOnAssignedEmployee()
        {
            Department dep = CreateObject<Department>();
            Employee emp = CreateObject<Employee>();
            emp.Department = dep;

            emp.Department = null;
            Assert.IsFalse(dep.Employees.Contains(emp), "Employee was not removed from collection on department.");
            Assert.IsNull(emp.Department, "Department was not nulled on employee.");
        }

        /// <summary>
        /// Set the department property on an employee to the same department
        /// </summary>
        [TestMethod]
        public void SetSameDepartmentOnEmployee()
        {
            Department dep = CreateObject<Department>();
            Employee emp = CreateObject<Employee>();
            emp.Department = dep;

            emp.Department = dep;
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee is not in collection on department.");
            Assert.AreEqual(1, dep.Employees.Count, "Employee has been added again to collection on department.");
            Assert.AreSame(dep, emp.Department, "Department is not set on employee.");
        }

        /// <summary>
        /// Set the department to null when it is already null
        /// </summary>
        [TestMethod]
        public void NullDepartmentOnUnassignedEmployee()
        {
            Employee emp = CreateObject<Employee>();

            emp.Department = null;
            Assert.IsNull(emp.Department, "Department is not null on employee.");
        }

        #endregion

        #region Manager - Reports

        /// <summary>
        /// Add employee without manager to a new manager reports collection
        /// </summary>
        [TestMethod]
        public void AddUnassignedEmployeeToManager()
        {
            Employee man = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();

            man.Reports.Add(emp);
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.");
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.");
        }

        /// <summary>
        /// Add employee with an existing manager to a new manager reports collection
        /// </summary>
        [TestMethod]
        public void AddAssignedEmployeeToManager()
        {
            Employee man = CreateObject<Employee>();
            Employee manOrig = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();
            manOrig.Reports.Add(emp);

            man.Reports.Add(emp);
            Assert.IsFalse(manOrig.Reports.Contains(emp), "Employee was not removed from collection on old manager.");
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.");
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.");
        }

        /// <summary>
        /// Remove an employee from a manager reports collection
        /// </summary>
        [TestMethod]
        public void RemoveEmployeeFromManager()
        {
            Employee man = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();
            man.Reports.Add(emp);

            man.Reports.Remove(emp);
            Assert.IsFalse(man.Reports.Contains(emp), "Employee was not removed from collection on old manager.");
            Assert.IsNull(emp.Manager, "Manager was not nulled on employee.");
        }

        /// <summary>
        /// Add an employee to a manager they already report to
        /// </summary>
        [TestMethod]
        public void AddEmployeeToSameManager()
        {
            Employee man = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();
            man.Reports.Add(emp);

            man.Reports.Add(emp);
            Assert.IsTrue(man.Reports.Contains(emp), "Employee is not in collection on manager.");
            Assert.AreSame(man, emp.Manager, "Manager is not set on employee.");
        }

        /// <summary>
        /// Set the manager on an employee that doesn't have a manager assigned
        /// </summary>
        [TestMethod]
        public void SetManagerOnUnassignedEmployee()
        {
            Employee man = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();

            emp.Manager = man;
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.");
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.");
        }

        /// <summary>
        /// Set the manager on an employee that is assigned to a different manager
        /// </summary>
        [TestMethod]
        public void SetManagerOnAssignedEmployee()
        {
            Employee man = CreateObject<Employee>();
            Employee manOrig = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();
            emp.Manager = manOrig;

            emp.Manager = man;
            Assert.IsFalse(manOrig.Reports.Contains(emp), "Employee was not removed from collection on old manager.");
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.");
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.");
        }

        /// <summary>
        /// Clear the manager on an employee
        /// </summary>
        [TestMethod]
        public void NullManagerOnAssignedEmployee()
        {
            Employee man = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();
            emp.Manager = man;

            emp.Manager = null;
            Assert.IsFalse(man.Reports.Contains(emp), "Employee was not removed from collection on manager.");
            Assert.IsNull(emp.Manager, "Manager was not nulled on employee.");
        }

        /// <summary>
        /// Set the manager property on an employee to the same manager
        /// </summary>
        [TestMethod]
        public void SetSameManagerOnEmployee()
        {
            Employee man = CreateObject<Employee>();
            Employee emp = CreateObject<Employee>();
            emp.Manager = man;

            emp.Manager = man;
            Assert.IsTrue(man.Reports.Contains(emp), "Employee is not in collection on manager.");
            Assert.AreEqual(1, man.Reports.Count, "Employee has been added again to collection on manager.");
            Assert.AreSame(man, emp.Manager, "Manager is not set on employee.");
        }

        /// <summary>
        /// Set the manager to null when it is already null
        /// </summary>
        [TestMethod]
        public void NullManagerOnUnassignedEmployee()
        {
            Employee emp = CreateObject<Employee>();

            emp.Manager = null;
            Assert.IsNull(emp.Manager, "Manager is not null on employee.");
        }

        #endregion

        /// <summary>
        /// Create an object of type T for tests to be run against
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <returns>An instance of T or a type derived from T</returns>
        protected abstract T CreateObject<T>() where T : class;
    }
}
