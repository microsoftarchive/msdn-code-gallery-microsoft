' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Linq
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.Fakes

    ''' <summary>
    ''' Tests the funtionality of FakeEmployeeContextTests
    ''' </summary>
    <TestClass>
    Public Class FakeEmployeeContextTests
        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub NullArgumentChecks()
            Utilities.CheckNullArgumentException(Sub()
                                                     Dim c = New FakeEmployeeContext(Nothing, New Department() {})
                                                     c.Dispose()
                                                 End Sub, "employees", "ctor")

            Utilities.CheckNullArgumentException(Sub()
                                                     Dim c = New FakeEmployeeContext(New Employee() {}, Nothing)
                                                     c.Dispose()
                                                 End Sub, "departments", "ctor")


            Utilities.CheckNullArgumentException(Sub()
                                                     Using ctx As New FakeEmployeeContext()
                                                         ctx.IsObjectTracked(Nothing)
                                                     End Using
                                                 End Sub, "entity", "IsObjectTracked")
        End Sub

        ''' <summary>
        ''' Verify default constructor initializes ObjectSets
        ''' </summary>
        <TestMethod>
        Public Sub Initialization()
            Using ctx As New FakeEmployeeContext()
                Assert.IsNotNull(ctx.Employees, "Constructor did not not initialize Employees ObjectSet.")
                Assert.IsNotNull(ctx.Departments, "Constructor did not initialize Departments ObjectSet.")
                Assert.IsNotNull(ctx.ContactDetails, "Constructor did not initialize ContactDetails ObjectSet.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify data supplied to constructor is available in ObjectSets
        ''' </summary>
        <TestMethod>
        Public Sub InitializationWithSuppliedCollections()
            Dim dep As New Department()
            Dim det As ContactDetail = New Phone()
            Dim emp As Employee = New Employee With {.ContactDetails = New List(Of ContactDetail) From {det}}

            Using ctx As New FakeEmployeeContext(New Employee() { emp }, New Department() { dep })
                Assert.IsTrue(ctx.Employees.Contains(emp), "Constructor did not add supplied Employees to public ObjectSet.")
                Assert.IsTrue(ctx.Departments.Contains(dep), "Constructor did not add supplied Departments to public ObjectSet.")
                Assert.IsTrue(ctx.ContactDetails.Contains(det), "Constructor did not add supplied ContactDetails to public ObjectSet.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify CreateObject returns a new instance of the type
        ''' </summary>
        <TestMethod>
        Public Sub CreateObject()
            ' Fake context should create the actual base type and not a type derived from it
            Using ctx As New FakeEmployeeContext()
                Dim entity As Object = ctx.CreateObject(Of Department)()
                Assert.AreEqual(GetType(Department), entity.GetType(), "Department did not get created.")

                entity = ctx.CreateObject(Of Employee)()
                Assert.AreEqual(GetType(Employee), entity.GetType(), "Employee did not get created.")

                entity = ctx.CreateObject(Of Email)()
                Assert.AreEqual(GetType(Email), entity.GetType(), "Email did not get created.")

                entity = ctx.CreateObject(Of Phone)()
                Assert.AreEqual(GetType(Phone), entity.GetType(), "Phone did not get created.")

                entity = ctx.CreateObject(Of Address)()
                Assert.AreEqual(GetType(Address), entity.GetType(), "Address did not get created.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify SaveCalled event gets raised when context is saved
        ''' </summary>
        <TestMethod>
        Public Sub SaveCalled()
            Using ctx As New FakeEmployeeContext()
                Dim called As Boolean = False
                AddHandler ctx.SaveCalled, Sub(sender, e) called = True
                ctx.Save()
                Assert.IsTrue(called, "Save did not raise SaveCalled event.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify DisposedCalled event gets raised when context is disposed
        ''' </summary>
        <TestMethod>
        Public Sub DisposeCalled()
            Dim called As Boolean = False
            Using ctx As New FakeEmployeeContext()
                AddHandler ctx.DisposeCalled, Sub(sender, e) called = True
            End Using

            Assert.IsTrue(called, "Dispose did not raise DisposeCalled event.")
        End Sub

        ''' <summary>
        ''' Verify IsObjectTracked for all entity types
        ''' </summary>
        <TestMethod>
        Public Sub IsObjectTracked()
            Using ctx As New FakeEmployeeContext()
                Dim e As New Employee()
                Assert.IsFalse(ctx.IsObjectTracked(e), "IsObjectTracked should be false when entity is not in added.")
                ctx.Employees.AddObject(e)
                Assert.IsTrue(ctx.IsObjectTracked(e), "IsObjectTracked should be true when entity is added.")

                Dim d As New Department()
                Assert.IsFalse(ctx.IsObjectTracked(d), "IsObjectTracked should be false when entity is not in added.")
                ctx.Departments.AddObject(d)
                Assert.IsTrue(ctx.IsObjectTracked(d), "IsObjectTracked should be true when entity is added.")

                Dim c As ContactDetail = New Phone()
                Assert.IsFalse(ctx.IsObjectTracked(c), "IsObjectTracked should be false when entity is not in added.")
                ctx.ContactDetails.AddObject(c)
                Assert.IsTrue(ctx.IsObjectTracked(c), "IsObjectTracked should be true when entity is added.")
            End Using
        End Sub
    End Class
End Namespace
