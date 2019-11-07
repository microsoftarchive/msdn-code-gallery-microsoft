' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq
Imports EmployeeTracker.Common
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Tests

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for EmployeeWorkspaceViewModel
    ''' </summary>
    <TestClass>
    Public Class EmployeeWorkspaceViewModelTests
        ''' <summary>
        ''' Verify creation of a workspace with no data
        ''' </summary>
        <TestMethod>
        Public Sub InitializeWithEmptyData()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim departments As New ObservableCollection(Of DepartmentViewModel)()
                Dim employees As New ObservableCollection(Of EmployeeViewModel)()
                Dim vm As New EmployeeWorkspaceViewModel(employees, departments, unit)

                Assert.AreSame(employees, vm.AllEmployees, "ViewModel should expose the same instance of the collection so that changes outside the ViewModel are reflected.")
                Assert.IsNull(vm.CurrentEmployee, "Current employee should not be set if there are no department.")
                Assert.IsNotNull(vm.AddEmployeeCommand, "AddEmployeeCommand should be initialized")
                Assert.IsNotNull(vm.DeleteEmployeeCommand, "DeleteEmployeeCommand should be initialized")
            End Using
        End Sub

        ''' <summary>
        ''' Verify creation of a workspace with data
        ''' </summary>
        <TestMethod>
        Public Sub InitializeWithData()
            Dim e1 As New Employee()
            Dim e2 As New Employee()

            Dim d1 As New Department()
            Dim d2 As New Department()

            Using ctx As New FakeEmployeeContext(New Employee() { e1, e2 }, New Department() { d1, d2 })
                Dim unit As New UnitOfWork(ctx)
                Dim departments As New ObservableCollection(Of DepartmentViewModel)(ctx.Departments.Select(Function(d) New DepartmentViewModel(d)))
                Dim employees As New ObservableCollection(Of EmployeeViewModel)()
                For Each e In ctx.Employees
                    employees.Add(New EmployeeViewModel(e, employees, departments, unit))
                Next e

                Dim vm As New EmployeeWorkspaceViewModel(employees, departments, unit)

                Assert.IsNotNull(vm.CurrentEmployee, "Current employee should be set if there are departments.")
                Assert.AreSame(employees, vm.AllEmployees, "ViewModel should expose the same instance of the Employee collection so that changes outside the ViewModel are reflected.")
                Assert.AreSame(employees, vm.AllEmployees(0).ManagerLookup, "ViewModel should expose the same instance of the Employee collection to children so that changes outside the ViewModel are reflected.")
                Assert.AreSame(departments, vm.AllEmployees(0).DepartmentLookup, "ViewModel should expose the same instance of the Department collection to children so that changes outside the ViewModel are reflected.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub NullArgumentChecks()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim ctor As Action = Sub()
                                         Dim x = New EmployeeWorkspaceViewModel(Nothing, New ObservableCollection(Of DepartmentViewModel)(), unit)
                                     End Sub
                Utilities.CheckNullArgumentException(ctor, "employees", "ctor")

                ctor = Sub()
                           Dim x = New EmployeeWorkspaceViewModel(New ObservableCollection(Of EmployeeViewModel)(), Nothing, unit)
                       End Sub
                Utilities.CheckNullArgumentException(ctor, "departmentLookup", "ctor")

                ctor = Sub()
                           Dim x = New EmployeeWorkspaceViewModel(New ObservableCollection(Of EmployeeViewModel)(), New ObservableCollection(Of DepartmentViewModel)(), Nothing)
                       End Sub
                Utilities.CheckNullArgumentException(ctor, "unitOfWork", "ctor")
            End Using
        End Sub

        ''' <summary>
        ''' Verify current employee getter and setter
        ''' </summary>
        <TestMethod>
        Public Sub CurrentEmployee()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx)

                Dim lastProperty As String
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                lastProperty = Nothing
                vm.CurrentEmployee = Nothing
                Assert.IsNull(vm.CurrentEmployee, "CurrentEmployee should have been nulled.")
                Assert.AreEqual("CurrentEmployee", lastProperty, "CurrentEmployee should have raised a PropertyChanged when set to null.")

                lastProperty = Nothing
                Dim employee = vm.AllEmployees.First()
                vm.CurrentEmployee = employee
                Assert.AreSame(employee, vm.CurrentEmployee, "CurrentEmployee has not been set to specified value.")
                Assert.AreEqual("CurrentEmployee", lastProperty, "CurrentEmployee should have raised a PropertyChanged when set to a value.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify additions to employee collection are reflected
        ''' </summary>
        <TestMethod>
        Public Sub ExternalAddToEmployeeCollection()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim unit As New UnitOfWork(ctx)
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx, unit)

                Dim currentEmployee As EmployeeViewModel = vm.CurrentEmployee
                Dim newEmployee As New EmployeeViewModel(New Employee(), vm.AllEmployees, New ObservableCollection(Of DepartmentViewModel)(), unit)

                vm.AllEmployees.Add(newEmployee)
                Assert.IsTrue(vm.AllEmployees.Contains(newEmployee), "New employee should have been added to AllEmployees.")
                Assert.AreSame(currentEmployee, vm.CurrentEmployee, "CurrentEmployee should not have changed.")
                Assert.IsFalse(ctx.IsObjectTracked(newEmployee.Model), "ViewModel is not responsible for adding employees created externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from employee collection are reflected
        ''' When current employee is remains in collection
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveFromEmployeeCollection()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim unit As New UnitOfWork(ctx)
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx, unit)

                Dim current As EmployeeViewModel = vm.AllEmployees.First()
                Dim toDelete As EmployeeViewModel = vm.AllEmployees.Skip(1).First()
                vm.CurrentEmployee = current

                vm.AllEmployees.Remove(toDelete)
                Assert.IsFalse(vm.AllEmployees.Contains(toDelete), "Employee should have been removed from AllDepartments.")
                Assert.AreSame(current, vm.CurrentEmployee, "CurrentEmployee should not have changed.")
                Assert.IsTrue(ctx.IsObjectTracked(toDelete.Model), "ViewModel is not responsible for deleting employees removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from employee collection are reflected
        ''' When current employee is removed
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveSelectedEmployeeFromCollection()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim unit As New UnitOfWork(ctx)
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx, unit)
                Dim current As EmployeeViewModel = vm.CurrentEmployee

                Dim lastProperty As String = Nothing
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                vm.AllEmployees.Remove(current)
                Assert.IsFalse(vm.AllEmployees.Contains(current), "Employee should have been removed from AllEmployees.")
                Assert.IsNull(vm.CurrentEmployee, "CurrentEmployee should have been nulled as it was removed from the collection.")
                Assert.AreEqual("CurrentEmployee", lastProperty, "CurrentEmployee should have raised a PropertyChanged.")
                Assert.IsTrue(ctx.IsObjectTracked(current.Model), "ViewModel is not responsible for deleting employees removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify department delete command is only available when an employee is selected
        ''' </summary>
        <TestMethod>
        Public Sub DeleteCommandOnlyAvailableWhenEmployeeSelected()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx)

                vm.CurrentEmployee = Nothing
                Assert.IsFalse(vm.DeleteEmployeeCommand.CanExecute(Nothing), "Delete command should be disabled when no employee is selected.")

                vm.CurrentEmployee = vm.AllEmployees.First()
                Assert.IsTrue(vm.DeleteEmployeeCommand.CanExecute(Nothing), "Delete command should be enabled when employee is selected.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify employee can be deleted
        ''' </summary>
        <TestMethod>
        Public Sub DeleteEmployee()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx)

                Dim toDelete As EmployeeViewModel = vm.CurrentEmployee
                Dim originalCount As Integer = vm.AllEmployees.Count

                Dim lastProperty As String = Nothing
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                vm.DeleteEmployeeCommand.Execute(Nothing)

                Assert.AreEqual(originalCount - 1, vm.AllEmployees.Count, "One employee should have been removed from the AllEmployees property.")
                Assert.IsFalse(vm.AllEmployees.Contains(toDelete), "The selected employee should have been removed.")
                Assert.IsFalse(ctx.IsObjectTracked(toDelete.Model), "The selected employee has not been removed from the context.")
                Assert.IsNull(vm.CurrentEmployee, "No employee should be selected after deletion.")
                Assert.AreEqual("CurrentEmployee", lastProperty, "CurrentEmployee should have raised a PropertyChanged.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify a new employee can be added when another employee is selected
        ''' </summary>
        <TestMethod>
        Public Sub AddWhenEmployeeSelected()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx)

                vm.CurrentEmployee = vm.AllEmployees.First()
                TestAddEmployee(ctx, vm)
            End Using
        End Sub

        ''' <summary>
        ''' Verify a new employee can be added when no employee is selected
        ''' </summary>
        <TestMethod>
        Public Sub AddWhenEmployeeNotSelected()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeWorkspaceViewModel = BuildViewModel(ctx)

                vm.CurrentEmployee = Nothing
                TestAddEmployee(ctx, vm)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies addition of employee to workspace and unit of work
        ''' </summary>
        ''' <param name="unitOfWork">Context employee should get added to</param>
        ''' <param name="vm">Workspace to add employee to</param>
        Private Shared Sub TestAddEmployee(ByVal ctx As FakeEmployeeContext, ByVal vm As EmployeeWorkspaceViewModel)
            Dim originalEmployees As List(Of EmployeeViewModel) = vm.AllEmployees.ToList()

            Dim lastProperty As String = Nothing
            AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

            Assert.IsTrue(vm.AddEmployeeCommand.CanExecute(Nothing), "Add command should always be enabled.")
            vm.AddEmployeeCommand.Execute(Nothing)

            Assert.AreEqual(originalEmployees.Count + 1, vm.AllEmployees.Count, "One new employee should have been added to the AllEmployees property.")
            Assert.IsFalse(originalEmployees.Contains(vm.CurrentEmployee), "The new employee should be selected.")
            Assert.IsNotNull(vm.CurrentEmployee, "The new employee should be selected.")
            Assert.AreEqual("CurrentEmployee", lastProperty, "CurrentEmployee should have raised a PropertyChanged.")
            Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentEmployee.Model), "The new employee has not been added to the context.")
        End Sub

        ''' <summary>
        ''' Creates a fake context with seed data
        ''' </summary>
        ''' <returns>The fake context</returns>
        Private Shared Function BuildContextWithData() As FakeEmployeeContext
            Dim e1 As New Employee()
            Dim e2 As New Employee()

            Dim d1 As New Department()
            Dim d2 As New Department()

            Return New FakeEmployeeContext(New Employee() { e1, e2 }, New Department() { d1, d2 })
        End Function

        ''' <summary>
        ''' Creates a ViewModel based on a fake context
        ''' </summary>
        ''' <param name="ctx">Context to base view model on</param>
        ''' <returns>The new ViewModel</returns>
        Private Shared Function BuildViewModel(ByVal ctx As FakeEmployeeContext) As EmployeeWorkspaceViewModel
            Return BuildViewModel(ctx, New UnitOfWork(ctx))
        End Function

        ''' <summary>
        ''' Creates a ViewModel based on a fake context using an existing unit of work
        ''' </summary>
        ''' <param name="ctx">Context to base view model on</param>
        ''' <param name="unit">Current unit of work</param>
        ''' <returns>The new ViewModel</returns>
        Private Shared Function BuildViewModel(ByVal ctx As FakeEmployeeContext, ByVal unit As UnitOfWork) As EmployeeWorkspaceViewModel
            Dim departments As New ObservableCollection(Of DepartmentViewModel)(ctx.Departments.Select(Function(d) New DepartmentViewModel(d)))
            Dim employees As New ObservableCollection(Of EmployeeViewModel)()
            For Each e In ctx.Employees
                employees.Add(New EmployeeViewModel(e, employees, departments, unit))
            Next e

            Return New EmployeeWorkspaceViewModel(employees, departments, unit)
        End Function
    End Class
End Namespace
