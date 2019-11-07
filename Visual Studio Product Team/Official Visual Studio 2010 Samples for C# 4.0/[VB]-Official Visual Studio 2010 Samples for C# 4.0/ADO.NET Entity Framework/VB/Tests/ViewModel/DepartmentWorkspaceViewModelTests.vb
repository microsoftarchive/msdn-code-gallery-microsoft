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
    ''' Unit tests for DepartmentWorkspaceViewModel
    ''' </summary>
    <TestClass>
    Public Class DepartmentWorkspaceViewModelTests
        ''' <summary>
        ''' Verify creation of a workspace with no data
        ''' </summary>
        <TestMethod>
        Public Sub InitializeWithEmptyData()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim departments As New ObservableCollection(Of DepartmentViewModel)()
                Dim vm As New DepartmentWorkspaceViewModel(departments, unit)

                Assert.IsNull(vm.CurrentDepartment, "Current department should not be set if there are no department.")
                Assert.AreSame(departments, vm.AllDepartments, "ViewModel should expose the same instance of the collection so that changes outside the ViewModel are reflected.")
                Assert.IsNotNull(vm.AddDepartmentCommand, "AddDepartmentCommand should be initialized")
                Assert.IsNotNull(vm.DeleteDepartmentCommand, "DeleteDepartmentCommand should be initialized")
            End Using
        End Sub

        ''' <summary>
        ''' Verify creation of a workspace with data
        ''' </summary>
        <TestMethod>
        Public Sub InitializeWithData()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim unit As New UnitOfWork(ctx)
                Dim departments As New ObservableCollection(Of DepartmentViewModel)(ctx.Departments.Select(Function(d) New DepartmentViewModel(d)))
                Dim vm As New DepartmentWorkspaceViewModel(departments, unit)

                Assert.IsNotNull(vm.CurrentDepartment, "Current department should be set if there are departments.")
                Assert.AreSame(departments, vm.AllDepartments, "ViewModel should expose the same instance of the collection so that changes outside the ViewModel are reflected.")
                Assert.IsNotNull(vm.AddDepartmentCommand, "AddDepartmentCommand should be initialized")
                Assert.IsNotNull(vm.DeleteDepartmentCommand, "DeleteDepartmentCommand should be initialized")
            End Using
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub CheckNullArgumentExceptions()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim ctor As Action = Sub()
                                         Dim x = New DepartmentWorkspaceViewModel(Nothing, unit)
                                     End Sub

                Utilities.CheckNullArgumentException(ctor, "departments", "ctor")

                ctor = Sub()
                           Dim x = New DepartmentWorkspaceViewModel(New ObservableCollection(Of DepartmentViewModel)(), Nothing)
                       End Sub

                Utilities.CheckNullArgumentException(ctor, "unitOfWork", "ctor")
            End Using
        End Sub

        ''' <summary>
        ''' Verify current department getter and setter
        ''' </summary>
        <TestMethod>
        Public Sub CurrentDepartment()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                Dim lastProperty As String
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                lastProperty = Nothing
                vm.CurrentDepartment = Nothing
                Assert.IsNull(vm.CurrentDepartment, "CurrentDepartment should have been nulled.")
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged when set to null.")

                lastProperty = Nothing
                Dim department = vm.AllDepartments.First()
                vm.CurrentDepartment = department
                Assert.AreSame(department, vm.CurrentDepartment, "CurrentDepartment has not been set to specified value.")
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged when set to a value.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify additions to department collection are reflected
        ''' </summary>
        <TestMethod>
        Public Sub ExternalAddToDepartmentCollection()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                Dim currentDepartment As DepartmentViewModel = vm.CurrentDepartment
                Dim newDepartment As New DepartmentViewModel(New Department())

                vm.AllDepartments.Add(newDepartment)
                Assert.IsTrue(vm.AllDepartments.Contains(newDepartment), "New department should have been added to AllDepartments.")
                Assert.AreSame(currentDepartment, vm.CurrentDepartment, "CurrentDepartment should not have changed.")
                Assert.IsFalse(ctx.IsObjectTracked(newDepartment.Model), "ViewModel is not responsible for adding departments created externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from department collection are reflected
        ''' When current department is remains in collection
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveDepartmentFromCollection()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                Dim currentDepartment As DepartmentViewModel = vm.AllDepartments.First()
                Dim toDelete As DepartmentViewModel = vm.AllDepartments.Skip(1).First()
                vm.CurrentDepartment = currentDepartment

                vm.AllDepartments.Remove(toDelete)
                Assert.IsFalse(vm.AllDepartments.Contains(toDelete), "Department should have been removed from AllDepartments.")
                Assert.AreSame(currentDepartment, vm.CurrentDepartment, "CurrentDepartment should not have changed.")
                Assert.IsTrue(ctx.IsObjectTracked(toDelete.Model), "ViewModel is not responsible for deleting departments removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from department collection are reflected
        ''' When current department is removed
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveSelectedDepartmentFromCollection()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                Dim currentDepartment As DepartmentViewModel = vm.CurrentDepartment

                Dim lastProperty As String = Nothing
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                vm.AllDepartments.Remove(currentDepartment)
                Assert.IsFalse(vm.AllDepartments.Contains(currentDepartment), "Department should have been removed from AllDepartments.")
                Assert.IsNull(vm.CurrentDepartment, "CurrentDepartment should have been nulled as it was removed from the collection.")
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged.")
                Assert.IsTrue(ctx.IsObjectTracked(currentDepartment.Model), "ViewModel is not responsible for deleting departments removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify department delete command is only available when a department is selected
        ''' </summary>
        <TestMethod>
        Public Sub DeleteCommandOnlyAvailableWhenDepartmentSelected()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                vm.CurrentDepartment = Nothing
                Assert.IsFalse(vm.DeleteDepartmentCommand.CanExecute(Nothing), "Delete command should be disabled when no department is selected.")

                vm.CurrentDepartment = vm.AllDepartments.First()
                Assert.IsTrue(vm.DeleteDepartmentCommand.CanExecute(Nothing), "Delete command should be enabled when department is selected.")
            End Using
        End Sub

        ''' <summary>
        ''' Cerify department can be deleted
        ''' </summary>
        <TestMethod>
        Public Sub DeleteDepartment()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                vm.CurrentDepartment = vm.AllDepartments.First()
                Dim toDelete As DepartmentViewModel = vm.CurrentDepartment
                Dim originalCount As Integer = vm.AllDepartments.Count

                Dim lastProperty As String = Nothing
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                vm.DeleteDepartmentCommand.Execute(Nothing)

                Assert.AreEqual(originalCount - 1, vm.AllDepartments.Count, "One department should have been removed from the AllDepartments property.")
                Assert.IsFalse(vm.AllDepartments.Contains(toDelete), "The selected department should have been removed.")
                Assert.IsFalse(ctx.IsObjectTracked(toDelete.Model), "The selected department has not been removed from the UnitOfWork.")
                Assert.IsNull(vm.CurrentDepartment, "No department should be selected after deletion.")
                Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify a new department can be added when another department is selected
        ''' </summary>
        <TestMethod>
        Public Sub AddWhenDepartmentSelected()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                vm.CurrentDepartment = vm.AllDepartments.First()
                TestAddDepartment(ctx, vm)
            End Using
        End Sub

        ''' <summary>
        ''' Verify a new department can be added when no department is selected
        ''' </summary>
        <TestMethod>
        Public Sub AddWhenDepartmentNotSelected()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As DepartmentWorkspaceViewModel = BuildViewModel(ctx)

                vm.CurrentDepartment = Nothing
                TestAddDepartment(ctx, vm)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies addition of department to workspace and unit of work
        ''' </summary>
        ''' <param name="ctx">Context the department should get added to</param>
        ''' <param name="vm">Workspace to add department to</param>
        Private Shared Sub TestAddDepartment(ByVal ctx As FakeEmployeeContext, ByVal vm As DepartmentWorkspaceViewModel)
            Dim originalDepartments As List(Of DepartmentViewModel) = vm.AllDepartments.ToList()

            Dim lastProperty As String = Nothing
            AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

            Assert.IsTrue(vm.AddDepartmentCommand.CanExecute(Nothing), "Add command should always be enabled.")
            vm.AddDepartmentCommand.Execute(Nothing)

            Assert.AreEqual(originalDepartments.Count + 1, vm.AllDepartments.Count, "One new department should have been added to the AllDepartments property.")
            Assert.IsFalse(originalDepartments.Contains(vm.CurrentDepartment), "The new department should be selected.")
            Assert.IsNotNull(vm.CurrentDepartment, "The new department should be selected.")
            Assert.AreEqual("CurrentDepartment", lastProperty, "CurrentDepartment should have raised a PropertyChanged.")
            Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentDepartment.Model), "The new department has not been added to the context.")
        End Sub

        ''' <summary>
        ''' Build a fake context with seeded data
        ''' </summary>
        ''' <returns>The populated context</returns>
        Private Shared Function BuildContextWithData() As FakeEmployeeContext
            Dim d1 As New Department()
            Dim d2 As New Department()

            Dim dvm1 As New DepartmentViewModel(d1)
            Dim dvm2 As New DepartmentViewModel(d2)

            Return New FakeEmployeeContext(New Employee() { }, New Department() { d1, d2 })
        End Function

        ''' <summary>
        ''' Creates a ViewModel based on a fake context
        ''' </summary>
        ''' <param name="ctx">Context to base view model on</param>
        ''' <returns>The new ViewModel</returns>
        Private Shared Function BuildViewModel(ByVal ctx As FakeEmployeeContext) As DepartmentWorkspaceViewModel
            Dim unit As New UnitOfWork(ctx)
            Dim departments As New ObservableCollection(Of DepartmentViewModel)(ctx.Departments.Select(Function(d) New DepartmentViewModel(d)))
            Dim vm As New DepartmentWorkspaceViewModel(departments, unit)
            Return vm
        End Function
    End Class
End Namespace
