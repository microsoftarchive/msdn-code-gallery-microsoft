' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.Model

    ''' <summary>
    ''' Base class for tests that verify fixup behavior between objects
    ''' This used to test the Pure POCO and Entity Framework proxy objects
    ''' to make sure they behave in the same manor.
    ''' </summary>
    <TestClass>
    Public MustInherit Class FixupTestsBase
        #Region "Department - Employees"

        ''' <summary>
        ''' Add employee without department to a new departments employees collection
        ''' </summary>
        <TestMethod>
        Public Sub AddUnassignedEmployeeToDepartment()
            Dim dep As Department = CreateObject(Of Department)()
            Dim emp As Employee = CreateObject(Of Employee)()

            dep.Employees.Add(emp)
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee was not added to collection on department.")
            Assert.AreSame(dep, emp.Department, "Department was not set on employee.")
        End Sub

        ''' <summary>
        ''' Add employee with an existing department to a new departments employees collection
        ''' </summary>
        <TestMethod>
        Public Sub AddAssignedEmployeeToDepartment()
            Dim emp As Employee = CreateObject(Of Employee)()
            Dim depOriginal As Department = CreateObject(Of Department)()
            Dim depNew As Department = CreateObject(Of Department)()
            depOriginal.Employees.Add(emp)

            depNew.Employees.Add(emp)
            Assert.IsFalse(depOriginal.Employees.Contains(emp), "Employee was not removed from collection on old department.")
            Assert.IsTrue(depNew.Employees.Contains(emp), "Employee was not added to collection on CreateObject<department.")
            Assert.AreSame(depNew, emp.Department, "Department was not set on employee.")
        End Sub

        ''' <summary>
        ''' Remove an employee from a departments employees collection
        ''' </summary>
        <TestMethod>
        Public Sub RemoveEmployeeFromDepartment()
            Dim dep As Department = CreateObject(Of Department)()
            Dim emp As Employee = CreateObject(Of Employee)()
            dep.Employees.Add(emp)

            dep.Employees.Remove(emp)
            Assert.IsFalse(dep.Employees.Contains(emp), "Employee was not removed from collection on department.")
            Assert.IsNull(emp.Department, "Department was not nulled on employee.")
        End Sub

        ''' <summary>
        ''' Add an employee to a department they already belong to
        ''' </summary>
        <TestMethod>
        Public Sub AddEmployeeToSameDepartment()
            Dim dep As Department = CreateObject(Of Department)()
            Dim emp As Employee = CreateObject(Of Employee)()
            dep.Employees.Add(emp)

            dep.Employees.Add(emp)
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee is not in collection on department.")
            Assert.AreSame(dep, emp.Department, "Department is not set on employee.")
        End Sub

        ''' <summary>
        ''' Set the department on an employee that doesn't have a department assigned
        ''' </summary>
        <TestMethod>
        Public Sub SetDepartmentOnUnassignedEmployee()
            Dim dep As Department = CreateObject(Of Department)()
            Dim emp As Employee = CreateObject(Of Employee)()

            emp.Department = dep
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee was not added to collection on department.")
            Assert.AreSame(dep, emp.Department, "Department was not set on employee.")
        End Sub

        ''' <summary>
        ''' Set the department on an employee that is assigned to a different department
        ''' </summary>
        <TestMethod>
        Public Sub SetDepartmentOnAssignedEmployee()
            Dim emp As Employee = CreateObject(Of Employee)()
            Dim depOriginal As Department = CreateObject(Of Department)()
            Dim depNew As Department = CreateObject(Of Department)()
            emp.Department = depOriginal

            emp.Department = depNew
            Assert.IsFalse(depOriginal.Employees.Contains(emp), "Employee was not removed from collection on old department.")
            Assert.IsTrue(depNew.Employees.Contains(emp), "Employee was not added to collection on CreateObject<department.")
            Assert.AreSame(depNew, emp.Department, "Department was not set on employee.")
        End Sub

        ''' <summary>
        ''' Clear the department on an employee
        ''' </summary>
        <TestMethod>
        Public Sub NullDepartmentOnAssignedEmployee()
            Dim dep As Department = CreateObject(Of Department)()
            Dim emp As Employee = CreateObject(Of Employee)()
            emp.Department = dep

            emp.Department = Nothing
            Assert.IsFalse(dep.Employees.Contains(emp), "Employee was not removed from collection on department.")
            Assert.IsNull(emp.Department, "Department was not nulled on employee.")
        End Sub

        ''' <summary>
        ''' Set the department property on an employee to the same department
        ''' </summary>
        <TestMethod>
        Public Sub SetSameDepartmentOnEmployee()
            Dim dep As Department = CreateObject(Of Department)()
            Dim emp As Employee = CreateObject(Of Employee)()
            emp.Department = dep

            emp.Department = dep
            Assert.IsTrue(dep.Employees.Contains(emp), "Employee is not in collection on department.")
            Assert.AreEqual(1, dep.Employees.Count, "Employee has been added again to collection on department.")
            Assert.AreSame(dep, emp.Department, "Department is not set on employee.")
        End Sub

        ''' <summary>
        ''' Set the department to null when it is already null
        ''' </summary>
        <TestMethod>
        Public Sub NullDepartmentOnUnassignedEmployee()
            Dim emp As Employee = CreateObject(Of Employee)()

            emp.Department = Nothing
            Assert.IsNull(emp.Department, "Department is not null on employee.")
        End Sub

        #End Region

        #Region "Manager - Reports"

        ''' <summary>
        ''' Add employee without manager to a new manager reports collection
        ''' </summary>
        <TestMethod>
        Public Sub AddUnassignedEmployeeToManager()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()

            man.Reports.Add(emp)
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.")
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.")
        End Sub

        ''' <summary>
        ''' Add employee with an existing manager to a new manager reports collection
        ''' </summary>
        <TestMethod>
        Public Sub AddAssignedEmployeeToManager()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim manOrig As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()
            manOrig.Reports.Add(emp)

            man.Reports.Add(emp)
            Assert.IsFalse(manOrig.Reports.Contains(emp), "Employee was not removed from collection on old manager.")
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.")
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.")
        End Sub

        ''' <summary>
        ''' Remove an employee from a manager reports collection
        ''' </summary>
        <TestMethod>
        Public Sub RemoveEmployeeFromManager()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()
            man.Reports.Add(emp)

            man.Reports.Remove(emp)
            Assert.IsFalse(man.Reports.Contains(emp), "Employee was not removed from collection on old manager.")
            Assert.IsNull(emp.Manager, "Manager was not nulled on employee.")
        End Sub

        ''' <summary>
        ''' Add an employee to a manager they already report to
        ''' </summary>
        <TestMethod>
        Public Sub AddEmployeeToSameManager()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()
            man.Reports.Add(emp)

            man.Reports.Add(emp)
            Assert.IsTrue(man.Reports.Contains(emp), "Employee is not in collection on manager.")
            Assert.AreSame(man, emp.Manager, "Manager is not set on employee.")
        End Sub

        ''' <summary>
        ''' Set the manager on an employee that doesn't have a manager assigned
        ''' </summary>
        <TestMethod>
        Public Sub SetManagerOnUnassignedEmployee()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()

            emp.Manager = man
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.")
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.")
        End Sub

        ''' <summary>
        ''' Set the manager on an employee that is assigned to a different manager
        ''' </summary>
        <TestMethod>
        Public Sub SetManagerOnAssignedEmployee()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim manOrig As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()
            emp.Manager = manOrig

            emp.Manager = man
            Assert.IsFalse(manOrig.Reports.Contains(emp), "Employee was not removed from collection on old manager.")
            Assert.IsTrue(man.Reports.Contains(emp), "Employee was not added to collection on manager.")
            Assert.AreSame(man, emp.Manager, "Manager was not set on employee.")
        End Sub

        ''' <summary>
        ''' Clear the manager on an employee
        ''' </summary>
        <TestMethod>
        Public Sub NullManagerOnAssignedEmployee()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()
            emp.Manager = man

            emp.Manager = Nothing
            Assert.IsFalse(man.Reports.Contains(emp), "Employee was not removed from collection on manager.")
            Assert.IsNull(emp.Manager, "Manager was not nulled on employee.")
        End Sub

        ''' <summary>
        ''' Set the manager property on an employee to the same manager
        ''' </summary>
        <TestMethod>
        Public Sub SetSameManagerOnEmployee()
            Dim man As Employee = CreateObject(Of Employee)()
            Dim emp As Employee = CreateObject(Of Employee)()
            emp.Manager = man

            emp.Manager = man
            Assert.IsTrue(man.Reports.Contains(emp), "Employee is not in collection on manager.")
            Assert.AreEqual(1, man.Reports.Count, "Employee has been added again to collection on manager.")
            Assert.AreSame(man, emp.Manager, "Manager is not set on employee.")
        End Sub

        ''' <summary>
        ''' Set the manager to null when it is already null
        ''' </summary>
        <TestMethod>
        Public Sub NullManagerOnUnassignedEmployee()
            Dim emp As Employee = CreateObject(Of Employee)()

            emp.Manager = Nothing
            Assert.IsNull(emp.Manager, "Manager is not null on employee.")
        End Sub

        #End Region

        ''' <summary>
        ''' Create an object of type T for tests to be run against
        ''' </summary>
        ''' <typeparam name="T">The type of object to create</typeparam>
        ''' <returns>An instance of T or a type derived from T</returns>
        Protected MustOverride Function CreateObject(Of T As Class)() As T
    End Class
End Namespace
