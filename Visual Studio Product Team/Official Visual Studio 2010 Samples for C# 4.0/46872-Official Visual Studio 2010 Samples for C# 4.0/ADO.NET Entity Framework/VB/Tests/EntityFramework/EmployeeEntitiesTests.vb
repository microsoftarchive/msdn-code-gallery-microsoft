' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System.Data.Objects.DataClasses
Imports EmployeeTracker.EntityFramework
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.EntityFramework

    ''' <summary>
    ''' Tests the features that have been added to the EF Context
    ''' </summary>
    <TestClass>
    Public Class EmployeeEntitiesTests
        ''' <summary>
        ''' Verify that all classes in the model can be proxied by EF
        ''' </summary>
        <TestMethod>
        Public Sub AllEntitiesBecomeChangeTrackingProxies()
            Using ctx As New EmployeeEntities()
                Dim entity As Object = ctx.CreateObject(Of Department)()
                Assert.IsInstanceOfType(entity, GetType(IEntityWithChangeTracker), "Department did not get proxied.")

                entity = ctx.CreateObject(Of Employee)()
                Assert.IsInstanceOfType(entity, GetType(IEntityWithChangeTracker), "Employee did not get proxied.")

                entity = ctx.CreateObject(Of Email)()
                Assert.IsInstanceOfType(entity, GetType(IEntityWithChangeTracker), "Email did not get proxied.")

                entity = ctx.CreateObject(Of Phone)()
                Assert.IsInstanceOfType(entity, GetType(IEntityWithChangeTracker), "Phone did not get proxied.")

                entity = ctx.CreateObject(Of Address)()
                Assert.IsInstanceOfType(entity, GetType(IEntityWithChangeTracker), "Address did not get proxied.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify IsObjectTracked for all entity types
        ''' </summary>
        <TestMethod>
        Public Sub IsObjectTracked()
            Using ctx As New EmployeeEntities()
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
