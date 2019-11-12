' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Diagnostics.CodeAnalysis

Namespace EmployeeTracker.Model

    ''' <summary>
    ''' Represents a Department within the company
    ''' </summary>
    Public Class Department
        ''' <summary>
        ''' The Employees that belong to this Department
        ''' </summary>
        Private privateEmployees As ICollection(Of Employee)

        ''' <summary>
        ''' Initializes a new instance of the Department class.
        ''' </summary>
        Public Sub New()
            ' Wire up the employees collection to sync references
            ' NOTE: When running against Entity Framework with change tracking proxies this logic will not get executed
            '       because the Employees property will get over-ridden and replaced with an EntityCollection<Employee>.
            '       The EntityCollection will perform this fixup instead.
            Dim emps As New ObservableCollection(Of Employee)()
            Me.privateEmployees = emps
            AddHandler emps.CollectionChanged, Sub(sender, e)
                                                   ' Set the reference on any employees being added to this department
                                                   If e.NewItems IsNot Nothing Then
                                                       For Each item As Employee In e.NewItems
                                                           If item.Department IsNot Me Then
                                                               item.Department = Me
                                                           End If
                                                       Next item
                                                   End If

                                                   ' Clear the reference on any employees being removed that still points to this department
                                                   If e.OldItems IsNot Nothing Then
                                                       For Each item As Employee In e.OldItems
                                                           If item.Department Is Me Then
                                                               item.Department = Nothing
                                                           End If
                                                       Next item
                                                   End If
                                               End Sub
        End Sub

        ''' <summary>
        ''' Gets or sets the Id of this Department
        ''' </summary>
        Public Overridable Property DepartmentId() As Integer

        ''' <summary>
        ''' Gets or sets the Name of this Department
        ''' </summary>
        Public Overridable Property DepartmentName() As String

        ''' <summary>
        ''' Gets or sets the Code of this Department
        ''' </summary>
        Public Overridable Property DepartmentCode() As String

        ''' <summary>
        ''' Gets or sets the date this Department was last audited
        ''' </summary>
        Public Overridable Property LastAudited() As DateTime?

        ''' <summary>
        ''' Gets or sets the employees that belong to this Department
        ''' Adding or removing will fixup the department property on the affected employee
        ''' </summary>
        Public Overridable Property Employees() As ICollection(Of Employee)
            Get
                Return Me.privateEmployees
            End Get
            Set(ByVal value As ICollection(Of Employee))
                Me.privateEmployees = value
            End Set
        End Property
    End Class
End Namespace
