' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.Fakes

    ''' <summary>
    ''' Tests the IObjectSet implementation in FakeObjectSet
    ''' </summary>
    <TestClass>
    Public Class FakeObjectSetTests
        ''' <summary>
        ''' Verify data passed in constructor is available in enumerator
        ''' </summary>
        <TestMethod>
        Public Sub InitializationWithTestData()
            Dim emp As New Employee()
            Dim [set] As New FakeObjectSet(Of Employee)(New Employee() { emp })
            Assert.IsTrue([set].Contains(emp), "Constructor did not add supplied Employees to public Enumerator.")
        End Sub

        ''' <summary>
        ''' Verify objects can be added to the set and that they are returned
        ''' </summary>
        <TestMethod>
        Public Sub AddObject()
            Dim emp As New Employee()
            Dim [set] As New FakeObjectSet(Of Employee)()
            [set].AddObject(emp)
            Assert.IsTrue([set].Contains(emp), "AddObject did not add supplied Employees to public Enumerator.")
        End Sub

        ''' <summary>
        ''' Verify objects can be attached to the set and that they are returned
        ''' </summary>
        <TestMethod>
        Public Sub Attach()
            Dim emp As New Employee()
            Dim [set] As New FakeObjectSet(Of Employee)()
            [set].Attach(emp)
            Assert.IsTrue([set].Contains(emp), "Attach did not add supplied Employees to public Enumerator.")
        End Sub

        ''' <summary>
        ''' Verify objects can be deleted from the set and that they are no longer returned
        ''' </summary>
        <TestMethod>
        Public Sub DeleteObject()
            Dim emp As New Employee()
            Dim [set] As New FakeObjectSet(Of Employee)(New Employee() { emp })
            [set].DeleteObject(emp)
            Assert.IsFalse([set].Contains(emp), "DeleteObject did not remove supplied Employees to public Enumerator.")
        End Sub

        ''' <summary>
        ''' Verify objects can be detached from the set and that they are no longer returned
        ''' </summary>
        <TestMethod>
        Public Sub Detach()
            Dim emp As New Employee()
            Dim [set] As New FakeObjectSet(Of Employee)(New Employee() { emp })
            [set].Detach(emp)
            Assert.IsFalse([set].Contains(emp), "Detach did not remove supplied Employees to public Enumerator.")
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub NullArgumentChecks()
            Dim ctor As Action = Sub()
                                     Dim x = New FakeObjectSet(Of Employee)(Nothing)
                                 End Sub

            Utilities.CheckNullArgumentException(ctor, "testData", "ctor")

            Dim objSet As New FakeObjectSet(Of Employee)()
            Utilities.CheckNullArgumentException(Sub() objSet.AddObject(Nothing), "entity", "AddObject")
            Utilities.CheckNullArgumentException(Sub() objSet.DeleteObject(Nothing), "entity", "DeleteObject")
            Utilities.CheckNullArgumentException(Sub() objSet.Attach(Nothing), "entity", "Attach")
            Utilities.CheckNullArgumentException(Sub() objSet.Detach(Nothing), "entity", "Detach")
        End Sub
    End Class
End Namespace
