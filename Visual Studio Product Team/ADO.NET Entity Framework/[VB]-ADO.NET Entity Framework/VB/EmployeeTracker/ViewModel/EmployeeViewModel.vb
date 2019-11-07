' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows.Input
Imports EmployeeTracker.Common
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel.Helpers

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel of an individual <see cref="Employee"/>
    ''' </summary>
    Public Class EmployeeViewModel
        Inherits BasicEmployeeViewModel
        ''' <summary>
        ''' The department currently assigned to this Employee
        ''' </summary>
        Private privateDepartment As DepartmentViewModel

        ''' <summary>
        ''' The manager currently assigned to this Employee
        ''' </summary>
        Private privateManager As EmployeeViewModel

        ''' <summary>
        ''' The contact detail currently selected
        ''' </summary>
        Private privateCurrentContactDetail As ContactDetailViewModel

        ''' <summary>
        ''' UnitOfWork for managing changes
        ''' </summary>
        Private unitOfWork As IUnitOfWork

        ''' <summary>
        ''' Initializes a new instance of the EmployeeViewModel class.
        ''' </summary>
        ''' <param name="employee">The underlying Employee this ViewModel is to be based on</param>
        ''' <param name="managerLookup">Existing collection of employees to use as a manager lookup</param>
        ''' <param name="departmentLookup">Existing collection of departments to use as a department lookup</param>
        ''' <param name="unitOfWork">UnitOfWork for managing changes</param>
        Public Sub New(ByVal employee As Employee, ByVal managerLookup As ObservableCollection(Of EmployeeViewModel), ByVal departmentLookup As ObservableCollection(Of DepartmentViewModel), ByVal unitOfWork As IUnitOfWork)
            MyBase.New(employee)
            If employee Is Nothing Then
                Throw New ArgumentNullException("employee")
            End If

            Me.unitOfWork = unitOfWork
            Me.ManagerLookup = managerLookup
            Me.DepartmentLookup = departmentLookup

            ' Build data structures for contact details
            Me.ContactDetails = New ObservableCollection(Of ContactDetailViewModel)()
            For Each detail In employee.ContactDetails
                Dim vm As ContactDetailViewModel = ContactDetailViewModel.BuildViewModel(detail)
                If vm IsNot Nothing Then
                    Me.ContactDetails.Add(vm)
                End If
            Next detail

            ' Re-act to any changes from outside this ViewModel
            AddHandler Me.DepartmentLookup.CollectionChanged, Sub(sender, e)
                                                                  If e.OldItems IsNot Nothing AndAlso e.OldItems.Contains(Me.Department) Then
                                                                      Me.Department = Nothing
                                                                  End If
                                                              End Sub
            AddHandler Me.ManagerLookup.CollectionChanged, Sub(sender, e)
                                                               If e.OldItems IsNot Nothing AndAlso e.OldItems.Contains(Me.Manager) Then
                                                                   Me.Manager = Nothing
                                                               End If
                                                           End Sub

            Me.AddEmailAddressCommand = New DelegateCommand(Sub(o) Me.AddContactDetail(Of Email)())
            Me.AddPhoneNumberCommand = New DelegateCommand(Sub(o) Me.AddContactDetail(Of Phone)())
            Me.AddAddressCommand = New DelegateCommand(Sub(o) Me.AddContactDetail(Of Address)())
            Me.DeleteContactDetailCommand = New DelegateCommand(Sub(o) Me.DeleteCurrentContactDetail(), Function(o) Me.CurrentContactDetail IsNot Nothing)
        End Sub

        ''' <summary>
        ''' Gets the command for adding a new Email address
        ''' </summary>
        Private privateAddEmailAddressCommand As ICommand
        Public Property AddEmailAddressCommand() As ICommand
            Get
                Return privateAddEmailAddressCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateAddEmailAddressCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the command for adding a new phone number
        ''' </summary>
        Private privateAddPhoneNumberCommand As ICommand
        Public Property AddPhoneNumberCommand() As ICommand
            Get
                Return privateAddPhoneNumberCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateAddPhoneNumberCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the command for adding a new address
        ''' </summary>
        Private privateAddAddressCommand As ICommand
        Public Property AddAddressCommand() As ICommand
            Get
                Return privateAddAddressCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateAddAddressCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the command for deleting the current employee
        ''' </summary>
        Private privateDeleteContactDetailCommand As ICommand
        Public Property DeleteContactDetailCommand() As ICommand
            Get
                Return privateDeleteContactDetailCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateDeleteContactDetailCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the currently selected contact detail
        ''' </summary>
        Public Property CurrentContactDetail() As ContactDetailViewModel
            Get
                Return Me.privateCurrentContactDetail
            End Get

            Set(ByVal value As ContactDetailViewModel)
                Me.privateCurrentContactDetail = value
                Me.OnPropertyChanged("CurrentContactDetail")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the department currently assigned to this Employee
        ''' </summary>
        Public Property Department() As DepartmentViewModel
            Get
                ' We need to reflect any changes made in the model so we check the current value before returning
                If Me.Model.Department Is Nothing Then
                    Return Nothing
                ElseIf Me.privateDepartment Is Nothing OrElse Me.privateDepartment.Model IsNot Me.Model.Department Then
                    Me.privateDepartment = Me.DepartmentLookup.Where(Function(d) d.Model Is Me.Model.Department).SingleOrDefault()
                End If

                Return Me.privateDepartment
            End Get

            Set(ByVal value As DepartmentViewModel)
                Me.privateDepartment = value
                Me.Model.Department = If((value Is Nothing), Nothing, value.Model)
                Me.OnPropertyChanged("Department")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the manager currently assigned to this Employee
        ''' </summary>
        Public Property Manager() As EmployeeViewModel
            Get
                ' We need to reflect any changes made in the model so we check the current value before returning
                If Me.Model.Manager Is Nothing Then
                    Return Nothing
                ElseIf Me.privateManager Is Nothing OrElse Me.privateManager.Model IsNot Me.Model.Manager Then
                    Me.privateManager = Me.ManagerLookup.Where(Function(e) e.Model Is Me.Model.Manager).SingleOrDefault()
                End If

                Return Me.privateManager
            End Get

            Set(ByVal value As EmployeeViewModel)
                Me.privateManager = value
                Me.Model.Manager = If((value Is Nothing), Nothing, value.Model)
                Me.OnPropertyChanged("Manager")
            End Set
        End Property

        ''' <summary>
        ''' Gets a collection of departments this employee could be assigned to
        ''' </summary>
        Private privateDepartmentLookup As ObservableCollection(Of DepartmentViewModel)
        Public Property DepartmentLookup() As ObservableCollection(Of DepartmentViewModel)
            Get
                Return privateDepartmentLookup
            End Get
            Private Set(ByVal value As ObservableCollection(Of DepartmentViewModel))
                privateDepartmentLookup = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a collection of employees who could be this employee's manager
        ''' </summary>
        Private privateManagerLookup As ObservableCollection(Of EmployeeViewModel)
        Public Property ManagerLookup() As ObservableCollection(Of EmployeeViewModel)
            Get
                Return privateManagerLookup
            End Get
            Private Set(ByVal value As ObservableCollection(Of EmployeeViewModel))
                privateManagerLookup = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the contact details on file for this employee
        ''' </summary>
        Private privateContactDetails As ObservableCollection(Of ContactDetailViewModel)
        Public Property ContactDetails() As ObservableCollection(Of ContactDetailViewModel)
            Get
                Return privateContactDetails
            End Get
            Private Set(ByVal value As ObservableCollection(Of ContactDetailViewModel))
                privateContactDetails = value
            End Set
        End Property

        ''' <summary>
        ''' Handles addition a new contact detail to this employee
        ''' </summary>
        ''' <typeparam name="T">The type of contact detail to be added</typeparam>
        Private Sub AddContactDetail(Of T As ContactDetail)()
            Dim detail As ContactDetail = Me.unitOfWork.CreateObject(Of T)()
            Me.unitOfWork.AddContactDetail(Me.Model, detail)

            Dim vm As ContactDetailViewModel = ContactDetailViewModel.BuildViewModel(detail)
            Me.ContactDetails.Add(vm)
            Me.CurrentContactDetail = vm
        End Sub

        ''' <summary>
        ''' Handles deletion of the current employee
        ''' </summary>
        Private Sub DeleteCurrentContactDetail()
            Me.unitOfWork.RemoveContactDetail(Me.Model, Me.CurrentContactDetail.Model)
            Me.ContactDetails.Remove(Me.CurrentContactDetail)
            Me.CurrentContactDetail = Nothing
        End Sub
    End Class
End Namespace
