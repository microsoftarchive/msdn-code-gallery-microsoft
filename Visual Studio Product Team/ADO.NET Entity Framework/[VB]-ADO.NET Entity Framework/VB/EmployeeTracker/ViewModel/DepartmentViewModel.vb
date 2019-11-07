' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel.Helpers

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel of an individual Department
    ''' </summary>
    Public Class DepartmentViewModel
        Inherits ViewModelBase
        ''' <summary>
        ''' Initializes a new instance of the DepartmentViewModel class.
        ''' </summary>
        ''' <param name="department">The underlying Department this ViewModel is to be based on</param>
        Public Sub New(ByVal department As Department)
            If department Is Nothing Then
                Throw New ArgumentNullException("department")
            End If

            Me.Model = department
        End Sub

        ''' <summary>
        ''' Gets the underlying Department this ViewModel is based on
        ''' </summary>
        Private privateModel As Department
        Public Property Model() As Department
            Get
                Return privateModel
            End Get
            Private Set(ByVal value As Department)
                privateModel = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of this department
        ''' </summary>
        Public Property DepartmentName() As String
            Get
                Return Me.Model.DepartmentName
            End Get

            Set(ByVal value As String)
                Me.Model.DepartmentName = value
                Me.OnPropertyChanged("DepartmentName")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the code of this department
        ''' </summary>
        Public Property DepartmentCode() As String
            Get
                Return Me.Model.DepartmentCode
            End Get

            Set(ByVal value As String)
                Me.Model.DepartmentCode = value
                Me.OnPropertyChanged("DepartmentCode")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the date this department was last audited on
        ''' </summary>
        Public Property LastAudited() As DateTime?
            Get
                Return Me.Model.LastAudited
            End Get

            Set(ByVal value? As DateTime)
                Me.Model.LastAudited = value
                Me.OnPropertyChanged("LastAudited")
            End Set
        End Property
    End Class
End Namespace
