' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq
Imports EmployeeTracker.Common
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for EmployeeViewModel
    ''' </summary>
    <TestClass>
    Public Class EmployeeViewModelTests
        ''' <summary>
        ''' Verify getters and setters on ViewModel affect underlying data and notify changes
        ''' </summary>
        <TestMethod>
        Public Sub Initialization()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)

                Assert.IsNotNull(vm.ManagerLookup, "Manager lookup should be initialized.")
                Assert.IsTrue(vm.ManagerLookup.Contains(vm), "ViewModel must be capable of containing itself in the manager lookup.")

                Assert.IsNotNull(vm.DepartmentLookup, "Department lookup should be initialized.")
                Assert.IsNotNull(vm.AddAddressCommand, "AddAddressCommand should be initialized.")
                Assert.IsNotNull(vm.AddEmailAddressCommand, "AddEmailAddressCommand should be initialized.")
                Assert.IsNotNull(vm.AddPhoneNumberCommand, "AddPhoneNumberCommand should be initialized.")
                Assert.IsNotNull(vm.DeleteContactDetailCommand, "DeleteContactDetailCommand should be initialized.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify getters reflect changes in model
        ''' </summary>
        <TestMethod>
        Public Sub ReferencesGetAndSet()
            ' Scalar properties are inherited from BasicEmployeeViewModel and are already tested
            Dim d1 As New Department()
            Dim d2 As New Department()

            Dim e1 As New Employee()
            Dim e2 As New Employee()
            Dim employee As Employee = New Employee With {.Department = d1, .Manager = e1}
            employee.ContactDetails.Add(New Phone())
            employee.ContactDetails.Add(New Email())

            Using ctx As New FakeEmployeeContext(New Employee() { e1, e2, employee }, New Department() { d1, d2 })
                Dim unit As New UnitOfWork(ctx)

                Dim dvm1 As New DepartmentViewModel(d1)
                Dim dvm2 As New DepartmentViewModel(d2)
                Dim departments As New ObservableCollection(Of DepartmentViewModel)() From {dvm1, dvm2}

                Dim employees As New ObservableCollection(Of EmployeeViewModel)()
                Dim evm1 As New EmployeeViewModel(e1, employees, departments, unit)
                Dim evm2 As New EmployeeViewModel(e2, employees, departments, unit)
                Dim employeeViewModel As New EmployeeViewModel(employee, employees, departments, unit)
                employees.Add(evm1)
                employees.Add(evm2)
                employees.Add(employeeViewModel)

                ' Test initial references are surfaced in ViewModel
                Assert.AreEqual(evm1, employeeViewModel.Manager, "ViewModel did not return ViewModel representing current manager.")
                Assert.AreEqual(e1, employeeViewModel.Manager.Model, "ViewModel did not return ViewModel representing current manager.")
                Assert.AreEqual(dvm1, employeeViewModel.Department, "ViewModel did not return ViewModel representing current department.")
                Assert.AreEqual(d1, employeeViewModel.Department.Model, "ViewModel did not return ViewModel representing current department.")
                Assert.AreEqual(2, employeeViewModel.ContactDetails.Count, "Contact details have not been populated on ViewModel.")

                ' Test changing properties updates Model and raises PropertyChanged
                Dim lastProperty As String
                AddHandler employeeViewModel.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                lastProperty = Nothing
                employeeViewModel.Department = dvm2
                Assert.AreEqual("Department", lastProperty, "Setting Department property did not raise correct PropertyChanged event.")
                Assert.AreEqual(d2, employee.Department, "Setting Department property in ViewModel is not reflected in Model.")

                lastProperty = Nothing
                employeeViewModel.Manager = evm2
                Assert.AreEqual("Manager", lastProperty, "Setting Manager property did not raise correct PropertyChanged event.")
                Assert.AreEqual(e2, employee.Manager, "Setting Manager property in ViewModel is not reflected in Model.")

                ' Test ViewModel returns current value from model
                employee.Manager = e1
                Assert.AreEqual(evm1, employeeViewModel.Manager, "ViewModel did not return correct manager when model was updated outside of ViewModel.")
                employee.Department = d1
                Assert.AreEqual(dvm1, employeeViewModel.Department, "ViewModel did not return correct department when model was updated outside of ViewModel.")

                ' Test ViewModel returns current value from model when set to null
                employee.Manager = Nothing
                Assert.AreEqual(Nothing, employeeViewModel.Manager, "ViewModel did not return correct manager when model was updated outside of ViewModel.")
                employee.Department = Nothing
                Assert.AreEqual(Nothing, employeeViewModel.Department, "ViewModel did not return correct department when model was updated outside of ViewModel.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify a new email address can be added
        ''' </summary>
        <TestMethod>
        Public Sub AddEmail()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)
                Dim originalDetails As List(Of ContactDetailViewModel) = vm.ContactDetails.ToList()

                Assert.IsTrue(vm.AddEmailAddressCommand.CanExecute(Nothing), "AddEmailAddressCommand should always be enabled.")
                vm.AddEmailAddressCommand.Execute(Nothing)

                Assert.IsNotNull(vm.CurrentContactDetail, "New email should be selected.")
                Assert.IsFalse(originalDetails.Contains(vm.CurrentContactDetail), "New email should be selected.")
                Assert.IsInstanceOfType(vm.CurrentContactDetail, GetType(EmailViewModel), "New contact should be an email.")
                Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentContactDetail.Model), "New email should have been added to context.")
                Assert.AreEqual(originalDetails.Count + 1, vm.ContactDetails.Count, "New email should have been added to AllContactDetails property.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify a new phone number can be added
        ''' </summary>
        <TestMethod>
        Public Sub AddPhone()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)
                Dim originalDetails As List(Of ContactDetailViewModel) = vm.ContactDetails.ToList()

                Assert.IsTrue(vm.AddPhoneNumberCommand.CanExecute(Nothing), "AddPhoneNumberCommand should always be enabled.")
                vm.AddPhoneNumberCommand.Execute(Nothing)

                Assert.IsNotNull(vm.CurrentContactDetail, "New phone should be selected.")
                Assert.IsFalse(originalDetails.Contains(vm.CurrentContactDetail), "New phone should be selected.")
                Assert.IsInstanceOfType(vm.CurrentContactDetail, GetType(PhoneViewModel), "New contact should be a phone.")
                Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentContactDetail.Model), "New phone should have been added to context.")
                Assert.AreEqual(originalDetails.Count + 1, vm.ContactDetails.Count, "New phone should have been added to AllContactDetails property.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify a new address can be added
        ''' </summary>
        <TestMethod>
        Public Sub AddAddress()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)
                Dim originalDetails As List(Of ContactDetailViewModel) = vm.ContactDetails.ToList()

                Assert.IsTrue(vm.AddAddressCommand.CanExecute(Nothing), "AddAddressCommand should always be enabled.")
                vm.AddAddressCommand.Execute(Nothing)

                Assert.IsNotNull(vm.CurrentContactDetail, "New address should be selected.")
                Assert.IsFalse(originalDetails.Contains(vm.CurrentContactDetail), "New address should be selected.")
                Assert.IsInstanceOfType(vm.CurrentContactDetail, GetType(AddressViewModel), "New contact should be an address.")
                Assert.IsTrue(ctx.IsObjectTracked(vm.CurrentContactDetail.Model), "New address should have been added to context.")
                Assert.AreEqual(originalDetails.Count + 1, vm.ContactDetails.Count, "New address should have been added to AllContactDetails property.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify delete is only available when a contact is selected
        ''' </summary>
        <TestMethod>
        Public Sub DeleteContactDetailOnlyAvailableWhenDetailIsSelected()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)
                Dim originalDetails As List(Of ContactDetailViewModel) = vm.ContactDetails.ToList()


                vm.CurrentContactDetail = Nothing
                Assert.IsFalse(vm.DeleteContactDetailCommand.CanExecute(Nothing), "DeleteContactDetailCommand should be disabled when no detail is selected.")

                vm.CurrentContactDetail = vm.ContactDetails.First()
                Assert.IsTrue(vm.DeleteContactDetailCommand.CanExecute(Nothing), "DeleteContactDetailCommand should be enabled when a detail is selected.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify contact detail can be deleted
        ''' </summary>
        <TestMethod>
        Public Sub DeleteContactDetail()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)
                Dim originalDetails As List(Of ContactDetailViewModel) = vm.ContactDetails.ToList()


                Dim toDelete As ContactDetailViewModel = vm.ContactDetails.First()
                vm.CurrentContactDetail = toDelete
                vm.DeleteContactDetailCommand.Execute(Nothing)

                Assert.IsNull(vm.CurrentContactDetail, "No detail should be selected after deleting.")
                Assert.IsFalse(vm.ContactDetails.Contains(toDelete), "Detail should be removed from ContactDetails property.")
                Assert.IsFalse(ctx.IsObjectTracked(toDelete), "Detail should be deleted from context.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify additions to department collection are reflected
        ''' </summary>
        <TestMethod>
        Public Sub ExternalAddToDepartmentLookup()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)

                Dim currentDepartment As DepartmentViewModel = vm.Department
                Dim newDepartment As New DepartmentViewModel(New Department())

                vm.DepartmentLookup.Add(newDepartment)
                Assert.IsTrue(vm.DepartmentLookup.Contains(newDepartment), "New department should have been added to DepartmentLookup.")
                Assert.AreSame(currentDepartment, vm.Department, "Assigned Department should not have changed.")
                Assert.IsFalse(ctx.IsObjectTracked(newDepartment.Model), "ViewModel is not responsible for adding departments created externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from department collection are reflected
        ''' When current department is remains in collection
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveDepartmentLookup()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)

                Dim currentDepartment As DepartmentViewModel = vm.DepartmentLookup.First()
                Dim toDelete As DepartmentViewModel = vm.DepartmentLookup.Skip(1).First()
                vm.Department = currentDepartment

                vm.DepartmentLookup.Remove(toDelete)
                Assert.IsFalse(vm.DepartmentLookup.Contains(toDelete), "Department should have been removed from DepartmentLookup.")
                Assert.AreSame(currentDepartment, vm.Department, "Assigned Department should not have changed.")
                Assert.IsTrue(ctx.IsObjectTracked(toDelete.Model), "ViewModel is not responsible for deleting departments removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from department collection are reflected
        ''' When current department is removed
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveDepartmentLookupSelectedDepartment()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)

                Dim currentDepartment As DepartmentViewModel = vm.Department

                Dim lastProperty As String = Nothing
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                vm.DepartmentLookup.Remove(currentDepartment)
                Assert.IsFalse(vm.DepartmentLookup.Contains(currentDepartment), "Department should have been removed from DepartmentLookup.")
                Assert.IsNull(vm.Department, "Assigned Department should have been nulled as it was removed from the collection.")
                Assert.AreEqual("Department", lastProperty, "Department should have raised a PropertyChanged.")
                Assert.IsTrue(ctx.IsObjectTracked(currentDepartment.Model), "ViewModel is not responsible for deleting departments removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify additions to employee collection are reflected
        ''' </summary>
        <TestMethod>
        Public Sub ExternalAddToManagerLookup()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim unit As New UnitOfWork(ctx)
                Dim vm As EmployeeViewModel = BuildViewModel(ctx, unit)

                Dim currentManager As EmployeeViewModel = vm.Manager
                Dim newManager As New EmployeeViewModel(New Employee(), vm.ManagerLookup, vm.DepartmentLookup, unit)

                vm.ManagerLookup.Add(newManager)
                Assert.IsTrue(vm.ManagerLookup.Contains(newManager), "New department should have been added to ManagerLookup.")
                Assert.AreSame(currentManager, vm.Manager, "Assigned Manager should not have changed.")
                Assert.IsFalse(ctx.IsObjectTracked(newManager.Model), "ViewModel is not responsible for adding Employees created externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from employee collection are reflected
        ''' When current manager is remains in collection
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveManagerLookup()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)

                Dim currentManager As EmployeeViewModel = vm.ManagerLookup.First()
                Dim toDelete As EmployeeViewModel = vm.ManagerLookup.Skip(1).First()
                vm.Manager = currentManager

                vm.ManagerLookup.Remove(toDelete)
                Assert.IsFalse(vm.ManagerLookup.Contains(toDelete), "Employee should have been removed from ManagerLookup.")
                Assert.AreSame(currentManager, vm.Manager, "Assigned Manager should not have changed.")
                Assert.IsTrue(ctx.IsObjectTracked(toDelete.Model), "ViewModel is not responsible for deleting Employees removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify removals from employee collection are reflected
        ''' When current manager is removed
        ''' </summary>
        <TestMethod>
        Public Sub ExternalRemoveManagerLookupSelectedManager()
            Using ctx As FakeEmployeeContext = BuildContextWithData()
                Dim vm As EmployeeViewModel = BuildViewModel(ctx)

                Dim currentManager As EmployeeViewModel = vm.Manager

                Dim lastProperty As String = Nothing
                AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

                vm.ManagerLookup.Remove(currentManager)
                Assert.IsFalse(vm.ManagerLookup.Contains(currentManager), "Employee should have been removed from ManagerLookup.")
                Assert.IsNull(vm.Manager, "Assigned Manager should have been nulled as it was removed from the collection.")
                Assert.AreEqual("Manager", lastProperty, "Manager should have raised a PropertyChanged.")
                Assert.IsTrue(ctx.IsObjectTracked(currentManager.Model), "ViewModel is not responsible for deleting Employees removed externally.")
            End Using
        End Sub

        ''' <summary>
        ''' Creates a fake context with seed data
        ''' </summary>
        ''' <returns>The fake context</returns>
        Private Shared Function BuildContextWithData() As FakeEmployeeContext
            Dim d1 As New Department()
            Dim d2 As New Department()

            Dim e1 As Employee = New Employee With {.Department = d1}
            Dim e2 As Employee = New Employee With {.Department = d1}

            e1.Manager = e2

            e1.ContactDetails.Add(New Phone())
            e1.ContactDetails.Add(New Email())
            e1.ContactDetails.Add(New Address())
            e2.ContactDetails.Add(New Phone())

            Return New FakeEmployeeContext(New Employee() { e1, e2 }, New Department() { d1, d2 })
        End Function

        ''' <summary>
        ''' Creates a ViewModel based on a fake context
        ''' </summary>
        ''' <param name="ctx">Context to base view model on</param>
        ''' <returns>The new ViewModel</returns>
        Private Shared Function BuildViewModel(ByVal ctx As FakeEmployeeContext) As EmployeeViewModel
            Return BuildViewModel(ctx, New UnitOfWork(ctx))
        End Function

        ''' <summary>
        ''' Creates a ViewModel based on a fake context using an existing unit of work
        ''' </summary>
        ''' <param name="ctx">Context to base view model on</param>
        ''' <param name="unit">Current unit of work</param>
        ''' <returns>The new ViewModel</returns>
        Private Shared Function BuildViewModel(ByVal ctx As FakeEmployeeContext, ByVal unit As UnitOfWork) As EmployeeViewModel
            Dim departments As New ObservableCollection(Of DepartmentViewModel)(ctx.Departments.Select(Function(d) New DepartmentViewModel(d)))
            Dim employees As New ObservableCollection(Of EmployeeViewModel)()
            For Each e In ctx.Employees
                employees.Add(New EmployeeViewModel(e, employees, departments, unit))
            Next e

            Return employees(0)
        End Function
    End Class
End Namespace
