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
    ''' Represents a person employeed by the company
    ''' </summary>
    Public Class Employee
        ''' <summary>
        ''' Contact details belonging to this Employee
        ''' </summary>
        Private details As ICollection(Of ContactDetail)

        ''' <summary>
        ''' The Employees that report to this Employee
        ''' </summary>
        Private privateReports As ICollection(Of Employee)

        ''' <summary>
        ''' The Department this Employee belongs to
        ''' </summary>
        Private privateDepartment As Department

        ''' <summary>
        ''' The manager of this Employee
        ''' </summary>
        Private privateManager As Employee

        ''' <summary>
        ''' Initializes a new instance of the Employee class.
        ''' </summary>
        Public Sub New()
            ' NOTE: No fixup is required as this is a uni-directional navigation
            Me.details = New ObservableCollection(Of ContactDetail)()

            ' Wire up the reports collection to sync references
            ' NOTE: When running against Entity Framework with change tracking proxies this logic will not get executed
            '       because the Reports property will get over-ridden and replaced with an EntityCollection<Employee>.
            '       The EntityCollection will perform this fixup instead.
            Dim reps As New ObservableCollection(Of Employee)()
            Me.privateReports = reps
            AddHandler reps.CollectionChanged, Sub(sender, e)
                                                   ' Set the reference on any employees being added to this manager
                                                   If e.NewItems IsNot Nothing Then
                                                       For Each item As Employee In e.NewItems
                                                           If item.Manager IsNot Me Then
                                                               item.Manager = Me
                                                           End If
                                                       Next item
                                                   End If

                                                   ' Clear the reference on any employees being removed that still points to this manager
                                                   If e.OldItems IsNot Nothing Then
                                                       For Each item As Employee In e.OldItems
                                                           If item.Manager Is Me Then
                                                               item.Manager = Nothing
                                                           End If
                                                       Next item
                                                   End If
                                               End Sub
        End Sub

        ''' <summary>
        ''' Gets or sets the Id of this Employee
        ''' </summary>
        Public Overridable Property EmployeeId() As Integer

        ''' <summary>
        ''' Gets or sets this Employees title
        ''' </summary>
        Public Overridable Property Title() As String

        ''' <summary>
        ''' Gets or sets this Employees first name
        ''' </summary>
        Public Overridable Property FirstName() As String

        ''' <summary>
        ''' Gets or sets this Employees last name
        ''' </summary>
        Public Overridable Property LastName() As String

        ''' <summary>
        ''' Gets or sets this Employees position
        ''' </summary>
        Public Overridable Property Position() As String

        ''' <summary>
        ''' Gets or sets the date this Employees was hired
        ''' </summary>
        Public Overridable Property HireDate() As DateTime

        ''' <summary>
        ''' Gets or sets the date this Employees left the company
        ''' Returns null if they are a current employee
        ''' </summary>
        Public Overridable Property TerminationDate() As DateTime?

        ''' <summary>
        ''' Gets or sets this Employees date of birth
        ''' </summary>
        Public Overridable Property BirthDate() As DateTime

        ''' <summary>
        ''' Gets or sets the Id of the Department this Employees belongs to
        ''' </summary>
        Public Overridable Property DepartmentId() As Integer?

        ''' <summary>
        ''' Gets or sets the Id of this Employees manager
        ''' </summary>
        Public Overridable Property ManagerId() As Integer?

        ''' <summary>
        ''' Gets or sets the contact details of this Employee
        ''' No fixup is performed as this is a uni-directional navigation
        ''' </summary>
        Public Overridable Property ContactDetails() As ICollection(Of ContactDetail)
            Get
                Return Me.details
            End Get
            Set(ByVal value As ICollection(Of ContactDetail))
                Me.details = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the employees that report to this Employee
        ''' Adding or removing will fixup the manager property on the affected employee
        ''' </summary>
        Public Overridable Property Reports() As ICollection(Of Employee)
            Get
                Return Me.privateReports
            End Get
            Set(ByVal value As ICollection(Of Employee))
                Me.privateReports = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Department this Employees belongs to
        ''' Setting this property will fixup the collection on the original and new department
        ''' </summary>
        Public Overridable Property Department() As Department
            Get
                Return Me.privateDepartment
            End Get

            Set(ByVal value As Department)
                If value IsNot Me.privateDepartment Then
                    Dim original As Department = Me.privateDepartment
                    Me.privateDepartment = value

                    ' Remove from old collection
                    If original IsNot Nothing AndAlso original.Employees.Contains(Me) Then
                        original.Employees.Remove(Me)
                    End If

                    ' Add to new collection
                    If value IsNot Nothing AndAlso (Not value.Employees.Contains(Me)) Then
                        value.Employees.Add(Me)
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets this Employees manager
        ''' Setting this property will fixup the collection on the original and new manager
        ''' </summary>
        Public Overridable Property Manager() As Employee
            Get
                Return Me.privateManager
            End Get

            Set(ByVal value As Employee)
                If value IsNot Me.privateManager Then
                    Dim original As Employee = Me.privateManager
                    Me.privateManager = value

                    ' Remove from old collection
                    If original IsNot Nothing AndAlso original.Reports.Contains(Me) Then
                        original.Reports.Remove(Me)
                    End If

                    ' Add to new collection
                    If value IsNot Nothing AndAlso (Not value.Reports.Contains(Me)) Then
                        value.Reports.Add(Me)
                    End If
                End If
            End Set
        End Property
    End Class
End Namespace
