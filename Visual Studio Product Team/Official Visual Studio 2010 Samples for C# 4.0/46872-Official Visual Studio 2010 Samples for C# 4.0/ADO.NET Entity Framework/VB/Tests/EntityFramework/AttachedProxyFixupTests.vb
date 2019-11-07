' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports EmployeeTracker.EntityFramework
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Tests.Model

Namespace Tests.EntityFramework

    ''' <summary>
    ''' Tests the fixup behavior of Proxied versions of objects in the model that are attached to an ObjectContext
    ''' </summary>
    <TestClass, SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification := "Context is disposed in test cleanup.")>
    Public Class AttachedProxyFixupTests
        Inherits FixupTestsBase
        ''' <summary>
        ''' Context to use for proxy creation
        ''' </summary>
        Private context As EmployeeEntities

        ''' <summary>
        ''' Creates the resources needed for this test
        ''' </summary>
        <TestInitialize>
        Public Sub Setup()
            Me.context = New EmployeeEntities()

            ' Unit tests run without a database so we need to switch off LazyLoading
            Me.context.ContextOptions.LazyLoadingEnabled = False
        End Sub

        ''' <summary>
        ''' Releases any resourced used for this test run
        ''' </summary>
        <TestCleanup>
        Public Sub Teardown()
            Me.context.Dispose()
        End Sub

        ''' <summary>
        ''' Returns a change tracking proxy derived from T and attached to an ObjectContext
        ''' </summary>
        ''' <typeparam name="T">The type to be created</typeparam>
        ''' <returns>A new instance of type T</returns>
        Protected Overrides Function CreateObject(Of T As Class)() As T
            Dim obj As T = Me.context.CreateObject(Of T)()

            Dim e As Employee = TryCast(obj, Employee)
            If e IsNot Nothing Then
                Me.context.Employees.AddObject(e)
                Return obj
            End If

            Dim d As Department = TryCast(obj, Department)
            If d IsNot Nothing Then
                Me.context.Departments.AddObject(d)
                Return obj
            End If

            Dim c As ContactDetail = TryCast(obj, ContactDetail)
            If c IsNot Nothing Then
                Me.context.ContactDetails.AddObject(c)
                Return obj
            End If

            Assert.Fail(String.Format(CultureInfo.InvariantCulture, "Need to update AttachedProxyFixupTests.CreateObject to be able to attach objects of type {0}.", obj.GetType().Name))
            Return Nothing
        End Function
    End Class
End Namespace
