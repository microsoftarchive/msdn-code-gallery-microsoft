' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports EmployeeTracker.Common
Imports EmployeeTracker.Model.Interfaces
Imports EmployeeTracker.ViewModel.Helpers

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel for accessing all data for the company
    ''' </summary>
    Public Class MainViewModel
        Inherits ViewModelBase
        ''' <summary>
        ''' UnitOfWork for co-ordinating changes
        ''' </summary>
        Private unitOfWork As IUnitOfWork

        ''' <summary>
        ''' Initializes a new instance of the MainViewModel class.
        ''' </summary>
        ''' <param name="unitOfWork">UnitOfWork for co-ordinating changes</param>
        ''' <param name="departmentRepository">Repository for querying department data</param>
        ''' <param name="employeeRepository">Repository for querying employee data</param>
        Public Sub New(ByVal unitOfWork As IUnitOfWork, ByVal departmentRepository As IDepartmentRepository, ByVal employeeRepository As IEmployeeRepository)
            If unitOfWork Is Nothing Then
                Throw New ArgumentNullException("unitOfWork")
            End If

            If departmentRepository Is Nothing Then
                Throw New ArgumentNullException("departmentRepository")
            End If

            If employeeRepository Is Nothing Then
                Throw New ArgumentNullException("employeeRepository")
            End If

            Me.unitOfWork = unitOfWork

            ' Build data structures to populate areas of the application surface
            Dim allEmployees As New ObservableCollection(Of EmployeeViewModel)()
            Dim allDepartments As New ObservableCollection(Of DepartmentViewModel)()

            For Each dep In departmentRepository.GetAllDepartments()
                allDepartments.Add(New DepartmentViewModel(dep))
            Next dep

            For Each emp In employeeRepository.GetAllEmployees()
                allEmployees.Add(New EmployeeViewModel(emp, allEmployees, allDepartments, Me.unitOfWork))
            Next emp

            Me.DepartmentWorkspace = New DepartmentWorkspaceViewModel(allDepartments, unitOfWork)
            Me.EmployeeWorkspace = New EmployeeWorkspaceViewModel(allEmployees, allDepartments, unitOfWork)

            ' Build non-interactive list of long serving employees
            Dim longServingEmployees As New List(Of BasicEmployeeViewModel)()
            For Each emp In employeeRepository.GetLongestServingEmployees(5)
                longServingEmployees.Add(New BasicEmployeeViewModel(emp))
            Next emp

            Me.LongServingEmployees = longServingEmployees

            Me.SaveCommand = New DelegateCommand(Sub(o) Me.Save())
        End Sub

        ''' <summary>
        ''' Gets the command to save all changes made in the current sessions UnitOfWork
        ''' </summary>
        Private privateSaveCommand As ICommand
        Public Property SaveCommand() As ICommand
            Get
                Return privateSaveCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateSaveCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the workspace for managing employees of the company
        ''' </summary>
        Private privateEmployeeWorkspace As EmployeeWorkspaceViewModel
        Public Property EmployeeWorkspace() As EmployeeWorkspaceViewModel
            Get
                Return privateEmployeeWorkspace
            End Get
            Private Set(ByVal value As EmployeeWorkspaceViewModel)
                privateEmployeeWorkspace = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the workspace for managing departments of the company
        ''' </summary>
        Private privateDepartmentWorkspace As DepartmentWorkspaceViewModel
        Public Property DepartmentWorkspace() As DepartmentWorkspaceViewModel
            Get
                Return privateDepartmentWorkspace
            End Get
            Private Set(ByVal value As DepartmentWorkspaceViewModel)
                privateDepartmentWorkspace = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the list of employees for the Loyalty Board
        ''' </summary>
        Private privateLongServingEmployees As IEnumerable(Of BasicEmployeeViewModel)
        Public Property LongServingEmployees() As IEnumerable(Of BasicEmployeeViewModel)
            Get
                Return privateLongServingEmployees
            End Get
            Private Set(ByVal value As IEnumerable(Of BasicEmployeeViewModel))
                privateLongServingEmployees = value
            End Set
        End Property

        ''' <summary>
        ''' Saves all changes made in the current sessions UnitOfWork
        ''' </summary>
        Private Sub Save()
            Me.unitOfWork.Save()
        End Sub
    End Class
End Namespace
