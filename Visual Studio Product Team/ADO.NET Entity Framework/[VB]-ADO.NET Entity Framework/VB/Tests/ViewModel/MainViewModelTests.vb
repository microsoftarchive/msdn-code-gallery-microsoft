' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System.Linq
Imports EmployeeTracker.Common
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model.Interfaces
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for MainViewModel
    ''' </summary>
    <TestClass>
    Public Class MainViewModelTests
        ''' <summary>
        ''' Test creation of a new entry point
        ''' </summary>
        <TestMethod>
        Public Sub Initialization()
            Using ctx As FakeEmployeeContext = Generation.BuildFakeSession()
                Dim unit As New UnitOfWork(ctx)
                Dim departmentRepository As IDepartmentRepository = New DepartmentRepository(ctx)
                Dim employeeRepository As IEmployeeRepository = New EmployeeRepository(ctx)

                Dim departmentCount As Integer = departmentRepository.GetAllDepartments().Count()
                Dim employeeCount As Integer = employeeRepository.GetAllEmployees().Count()

                Dim main As New MainViewModel(unit, departmentRepository, employeeRepository)

                Assert.IsNotNull(main.DepartmentWorkspace, "Department workspace should be initialized.")
                Assert.AreEqual(departmentCount, main.DepartmentWorkspace.AllDepartments.Count, "Department workspace should contain all departments from repository.")

                Assert.IsNotNull(main.EmployeeWorkspace, "Employee workspace should be initialized.")
                Assert.AreEqual(employeeCount, main.EmployeeWorkspace.AllEmployees.Count, "Employee workspace should contain all employees from repository.")

                Assert.IsNotNull(main.LongServingEmployees, "Long serving employee list should be initialized.")
                Assert.AreEqual(5, main.LongServingEmployees.Count(), "Long serving employee list should be restricted to five employees.")

                Assert.AreSame(main.DepartmentWorkspace.AllDepartments, main.EmployeeWorkspace.AllEmployees(0).DepartmentLookup, "A single instance of the department list should be used so that adds/removes flow throughout the application.")

                Assert.AreSame(main.EmployeeWorkspace.AllEmployees, main.EmployeeWorkspace.AllEmployees(0).ManagerLookup, "A single instance of the employee list should be used so that adds/removes flow throughout the application.")

                Assert.IsNotNull(main.SaveCommand, "SaveCommand should be initialized.")
            End Using
        End Sub

        ''' <summary>
        ''' Test save command causes a save on the backing unit of work
        ''' </summary>
        <TestMethod>
        Public Sub SaveCommand()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim departmentRepository As IDepartmentRepository = New DepartmentRepository(ctx)
                Dim employeeRepository As IEmployeeRepository = New EmployeeRepository(ctx)
                Dim main As New MainViewModel(unit, departmentRepository, employeeRepository)

                Dim called As Boolean = False
                AddHandler ctx.SaveCalled, Sub(sender, e) called = True
                main.SaveCommand.Execute(Nothing)
                Assert.IsTrue(called, "SaveCommand should result in save on underlying UnitOfWork.")
            End Using
        End Sub
    End Class
End Namespace
