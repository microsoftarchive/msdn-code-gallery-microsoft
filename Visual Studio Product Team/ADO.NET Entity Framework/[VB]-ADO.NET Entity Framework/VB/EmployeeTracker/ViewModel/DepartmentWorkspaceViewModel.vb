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
    ''' ViewModel for managing Departments within the company
    ''' </summary>
    Public Class DepartmentWorkspaceViewModel
        Inherits ViewModelBase
        ''' <summary>
        ''' The deprtment currently selected in the workspace
        ''' </summary>
        Private privateCurrentDepartment As DepartmentViewModel

        ''' <summary>
        ''' UnitOfWork for managing changes
        ''' </summary>
        Private unitOfWork As IUnitOfWork

        ''' <summary>
        ''' Initializes a new instance of the DepartmentWorkspaceViewModel class.
        ''' </summary>
        ''' <param name="departments">The departments to be managed</param>
        ''' <param name="unitOfWork">UnitOfWork for managing changes</param>
        Public Sub New(ByVal departments As ObservableCollection(Of DepartmentViewModel), ByVal unitOfWork As IUnitOfWork)
            If departments Is Nothing Then
                Throw New ArgumentNullException("departments")
            End If

            If unitOfWork Is Nothing Then
                Throw New ArgumentNullException("unitOfWork")
            End If

            Me.unitOfWork = unitOfWork
            Me.AllDepartments = departments
            Me.CurrentDepartment = If(Me.AllDepartments.Count > 0, Me.AllDepartments(0), Nothing)

            ' Re-act to any changes from outside this ViewModel
            AddHandler Me.AllDepartments.CollectionChanged, Sub(sender, e)
                                                                If e.OldItems IsNot Nothing AndAlso e.OldItems.Contains(Me.CurrentDepartment) Then
                                                                    Me.CurrentDepartment = Nothing
                                                                End If
                                                            End Sub

            Me.AddDepartmentCommand = New DelegateCommand(Sub(o) Me.AddDepartment())
            Me.DeleteDepartmentCommand = New DelegateCommand(Sub(o) Me.DeleteCurrentDepartment(), Function(o) Me.CurrentDepartment IsNot Nothing)
        End Sub

        ''' <summary>
        ''' Gets the command for adding a new department
        ''' </summary>
        Private privateAddDepartmentCommand As ICommand
        Public Property AddDepartmentCommand() As ICommand
            Get
                Return privateAddDepartmentCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateAddDepartmentCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the command for deleting the current department
        ''' </summary>
        Private privateDeleteDepartmentCommand As ICommand
        Public Property DeleteDepartmentCommand() As ICommand
            Get
                Return privateDeleteDepartmentCommand
            End Get
            Private Set(ByVal value As ICommand)
                privateDeleteDepartmentCommand = value
            End Set
        End Property

        ''' <summary>
        ''' Gets all departments whithin the company
        ''' </summary>
        Private privateAllDepartments As ObservableCollection(Of DepartmentViewModel)
        Public Property AllDepartments() As ObservableCollection(Of DepartmentViewModel)
            Get
                Return privateAllDepartments
            End Get
            Private Set(ByVal value As ObservableCollection(Of DepartmentViewModel))
                privateAllDepartments = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the deprtment currently selected in the workspace
        ''' </summary>
        Public Property CurrentDepartment() As DepartmentViewModel
            Get
                Return Me.privateCurrentDepartment
            End Get

            Set(ByVal value As DepartmentViewModel)
                Me.privateCurrentDepartment = value
                Me.OnPropertyChanged("CurrentDepartment")
            End Set
        End Property

        ''' <summary>
        ''' Handles addition a new department to the workspace and model
        ''' </summary>
        Private Sub AddDepartment()
            Dim dep As Department = Me.unitOfWork.CreateObject(Of Department)()
            Me.unitOfWork.AddDepartment(dep)

            Dim vm As New DepartmentViewModel(dep)
            Me.AllDepartments.Add(vm)
            Me.CurrentDepartment = vm
        End Sub

        ''' <summary>
        ''' Handles deletion of the current department
        ''' </summary>
        Private Sub DeleteCurrentDepartment()
            Me.unitOfWork.RemoveDepartment(Me.CurrentDepartment.Model)
            Me.AllDepartments.Remove(Me.CurrentDepartment)
            Me.CurrentDepartment = Nothing
        End Sub
    End Class
End Namespace
