' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports EmployeeTracker.Common
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel.Helpers

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel for managing Employees within the company
    ''' </summary>
    Public Class EmployeeWorkspaceViewModel
        Inherits ViewModelBase
        ''' <summary>
        ''' The employee currently selected in this workspace
        ''' </summary>
        Private privateCurrentEmployee As EmployeeViewModel

        ''' <summary>
        ''' UnitOfWork for managing changes
        ''' </summary>
        Private unitOfWork As IUnitOfWork

        ''' <summary>
        ''' Departments to be used for lookups
        ''' </summary>
        Private departmentLookup As ObservableCollection(Of DepartmentViewModel)

        ''' <summary>
        ''' Initializes a new instance of the EmployeeWorkspaceViewModel class.
        ''' </summary>
        ''' <param name="employees">Employees to be managed</param>
        ''' <param name="departmentLookup">The departments to be used for lookups</param>
        ''' <param name="unitOfWork">UnitOfWork for managing changes</param>
        Public Sub New(ByVal employees As ObservableCollection(Of EmployeeViewModel), ByVal departmentLookup As ObservableCollection(Of DepartmentViewModel), ByVal unitOfWork As IUnitOfWork)
            If employees Is Nothing Then
                Throw New ArgumentNullException("employees")
            End If

            If departmentLookup Is Nothing Then
                Throw New ArgumentNullException("departmentLookup")
            End If

            If unitOfWork Is Nothing Then
                Throw New ArgumentNullException("unitOfWork")
            End If

            Me.unitOfWork = unitOfWork
            Me.AllEmployees = employees
            Me.departmentLookup = departmentLookup
            Me.CurrentEmployee = If(employees.Count > 0, employees(0), Nothing)

            ' Re-act to any changes from outside this ViewModel
            AddHandler Me.AllEmployees.CollectionChanged, Sub(sender, e)
                                                              If e.OldItems IsNot Nothing AndAlso e.OldItems.Contains(Me.CurrentEmployee) Then
                                                                  Me.CurrentEmployee = Nothing
                                                              End If
                                                          End Sub

            Me.AddEmployeeCommand = New DelegateCommand(Sub(o) Me.AddEmployee())
            Me.DeleteEmployeeCommand = New DelegateCommand(Sub(o) Me.DeleteCurrentEmployee(), Function(o) Me.CurrentEmployee IsNot Nothing)
        End Sub

        ''' <summary>
        ''' Gets the command for adding a new employee
        ''' </summary>
        Private privateAddEmployeeCommand As ICommand
        Public Property AddEmployeeCommand() As ICommand
            Get
                Return privateAddEmployeeCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateAddEmployeeCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the command for deleting the current employee
        ''' </summary>
        Private privateDeleteEmployeeCommand As ICommand
        Public Property DeleteEmployeeCommand() As ICommand
            Get
                Return privateDeleteEmployeeCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateDeleteEmployeeCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets all employees whithin the company
        ''' </summary>
        Private privateAllEmployees As ObservableCollection(Of EmployeeViewModel)
        Public Property AllEmployees() As ObservableCollection(Of EmployeeViewModel)
            Get
                Return privateAllEmployees
            End Get
            Private Set(ByVal value As ObservableCollection(Of EmployeeViewModel))
                privateAllEmployees = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the employee currently selected in this workspace
        ''' </summary>
        Public Property CurrentEmployee() As EmployeeViewModel
            Get
                Return Me.privateCurrentEmployee
            End Get

            Set(ByVal value As EmployeeViewModel)
                Me.privateCurrentEmployee = value
                Me.OnPropertyChanged("CurrentEmployee")
            End Set
        End Property

        ''' <summary>
        ''' Handles addition a new employee to the workspace and model
        ''' </summary>
        Private Sub AddEmployee()
            Dim emp As Employee = Me.unitOfWork.CreateObject(Of Employee)()
            Me.unitOfWork.AddEmployee(emp)

            Dim vm As New EmployeeViewModel(emp, Me.AllEmployees, Me.departmentLookup, Me.unitOfWork)
            Me.AllEmployees.Add(vm)
            Me.CurrentEmployee = vm
        End Sub

        ''' <summary>
        ''' Handles deletion of the current employee
        ''' </summary>
        Private Sub DeleteCurrentEmployee()
            Me.unitOfWork.RemoveEmployee(Me.CurrentEmployee.Model)
            Me.AllEmployees.Remove(Me.CurrentEmployee)
            Me.CurrentEmployee = Nothing
        End Sub
    End Class
End Namespace
