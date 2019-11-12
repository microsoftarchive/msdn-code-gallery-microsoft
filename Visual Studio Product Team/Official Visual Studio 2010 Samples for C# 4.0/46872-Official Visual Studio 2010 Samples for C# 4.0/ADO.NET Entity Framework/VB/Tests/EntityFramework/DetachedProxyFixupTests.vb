' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System.Diagnostics.CodeAnalysis
Imports EmployeeTracker.EntityFramework
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Tests.Model

Namespace Tests.EntityFramework

    ''' <summary>
    ''' Tests the fixup behavior of Proxied versions of objects in the model when not attached to an ObjectContext
    ''' </summary>
    <TestClass, SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification := "Context is disposed in test cleanup.")>
    Public Class DetachedProxyFixupTests
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
        ''' Returns a change tracking proxy derived from T
        ''' </summary>
        ''' <typeparam name="T">The type to be created</typeparam>
        ''' <returns>A new instance of type T</returns>
        Protected Overrides Function CreateObject(Of T As Class)() As T
            Return Me.context.CreateObject(Of T)()
        End Function
    End Class
End Namespace
