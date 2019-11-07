' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Data.Objects
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Linq
Imports EmployeeTracker.Common
Imports EmployeeTracker.Model
Imports EmployeeTracker.Model.Interfaces

Namespace EmployeeTracker.Fakes

    ''' <summary>
    ''' In-memory implementation of IEmployeeContext
    ''' </summary>
    Public NotInheritable Class FakeEmployeeContext
        Implements IEmployeeContext
        ''' <summary>
        ''' Initializes a new instance of the FakeEmployeeContext class.
        ''' The context contains empty initial data.
        ''' </summary>
        Public Sub New()
            Me.privateEmployees = New FakeObjectSet(Of Employee)()
            Me.privateDepartments = New FakeObjectSet(Of Department)()
            Me.privateContactDetails = New FakeObjectSet(Of ContactDetail)()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the FakeEmployeeContext class.
        ''' The context contains the supplied initial data.
        ''' </summary>
        ''' <param name="employees">Employees to include in the context</param>
        ''' <param name="departments">Departments to include in the context</param>
        Public Sub New(ByVal employees As IEnumerable(Of Employee), ByVal departments As IEnumerable(Of Department))
            If employees Is Nothing Then
                Throw New ArgumentNullException("employees")
            End If

            If departments Is Nothing Then
                Throw New ArgumentNullException("departments")
            End If

            Me.privateEmployees = New FakeObjectSet(Of Employee)(employees)
            Me.privateDepartments = New FakeObjectSet(Of Department)(departments)

            ' Derive contact detail from supplied employee data
            Me.privateContactDetails = New FakeObjectSet(Of ContactDetail)()
            For Each emp In employees
                For Each det In emp.ContactDetails
                    Me.privateContactDetails.AddObject(det)
                Next det
            Next emp
        End Sub

        ''' <summary>
        ''' Raised whenever Save() is called
        ''' </summary>
        Public Event SaveCalled As EventHandler(Of EventArgs)

        ''' <summary>
        ''' Raised whenever Dispose() is called
        ''' </summary>
        Public Event DisposeCalled As EventHandler(Of EventArgs)

        ''' <summary>
        ''' Gets all employees tracked by this context
        ''' </summary>
        Private privateEmployees As IObjectSet(Of Employee)
        Public ReadOnly Property Employees() As IObjectSet(Of Employee) Implements IEmployeeContext.Employees
            Get
                Return privateEmployees
            End Get
        End Property

        ''' <summary>
        ''' Gets all departments tracked by this context
        ''' </summary>
        Private privateDepartments As IObjectSet(Of Department)
        Public ReadOnly Property Departments() As IObjectSet(Of Department) Implements IEmployeeContext.Departments
            Get
                Return privateDepartments
            End Get
        End Property

        ''' <summary>
        ''' Gets all contact details tracked by this context
        ''' </summary>
        Private privateContactDetails As IObjectSet(Of ContactDetail)
        Public ReadOnly Property ContactDetails() As IObjectSet(Of ContactDetail) Implements IEmployeeContext.ContactDetails
            Get
                Return privateContactDetails
            End Get
        End Property

        ''' <summary>
        ''' Save all pending changes in this context
        ''' </summary>
        Public Sub Save() Implements IEmployeeContext.Save
            Me.OnSaveCalled(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Release all resources used by this context
        ''' </summary>
        Public Sub Dispose() Implements IEmployeeContext.Dispose
            Me.OnDisposeCalled(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Creates a new instance of the specified object type
        ''' NOTE: This pattern is used to allow the use of change tracking proxies
        '''       when running against the Entity Framework.
        ''' </summary>
        ''' <typeparam name="T">The type to create</typeparam>
        ''' <returns>The newly created object</returns>
        Public Function CreateObject(Of T As Class)() As T Implements IEmployeeContext.CreateObject
            Return Activator.CreateInstance(Of T)()
        End Function

        ''' <summary>
        ''' Checks if the specified object is tracked by this context
        ''' </summary>
        ''' <param name="entity">The object to search for</param>
        ''' <returns>True if the object is tracked, false otherwise</returns>
        Public Function IsObjectTracked(ByVal entity As Object) As Boolean Implements IEmployeeContext.IsObjectTracked
            If entity Is Nothing Then
                Throw New ArgumentNullException("entity")
            End If

            Dim contained As Boolean = False

            If TypeOf entity Is Employee Then
                contained = Me.Employees.Contains(DirectCast(entity, Employee))
            ElseIf TypeOf entity Is Department Then
                contained = Me.Departments.Contains(DirectCast(entity, Department))
            ElseIf TypeOf entity Is ContactDetail Then
                contained = Me.ContactDetails.Contains(DirectCast(entity, ContactDetail))
            End If

            Return contained
        End Function

        ''' <summary>
        ''' Raises the SaveCalled event
        ''' </summary>
        ''' <param name="e">Arguments for the event</param>
        Private Sub OnSaveCalled(ByVal e As EventArgs)
            Dim handler = Me.SaveCalledEvent
            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        ''' Raises the DisposeCalled event
        ''' </summary>
        ''' <param name="e">Arguments for the event</param>
        Private Sub OnDisposeCalled(ByVal e As EventArgs)
            Dim handler = Me.DisposeCalledEvent
            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub
    End Class
End Namespace
