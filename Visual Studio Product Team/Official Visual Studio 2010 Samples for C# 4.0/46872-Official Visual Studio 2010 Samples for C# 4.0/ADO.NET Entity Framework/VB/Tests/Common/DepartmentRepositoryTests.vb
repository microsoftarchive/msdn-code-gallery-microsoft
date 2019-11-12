' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports EmployeeTracker.Common
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting


Namespace Tests.EntityFramework

    ''' <summary>
    ''' Tests retrieval of data from a DepartmentObjectSetRepository
    ''' </summary>
    <TestClass()>
    Public Class DepartmentRepositoryTests
        ''' <summary>
        ''' Verify GetAllDepartments returns all data from base ObjectSet
        ''' </summary>
        <TestMethod()>
        Public Sub GetAllDepartments()
            Dim d1 As New Department()
            Dim d2 As New Department()
            Dim d3 As New Department()

            Using ctx As New FakeEmployeeContext(New Employee() {}, New Department() {d1, d2, d3})
                Dim rep As New DepartmentRepository(ctx)

                Dim result As IEnumerable(Of Department) = rep.GetAllDepartments()

                Assert.IsNotInstanceOfType(result, GetType(IQueryable), "Repositories should not return IQueryable as this allows modification of the query that gets sent to the store. ")

                Assert.AreEqual(3, result.Count())
                Assert.IsTrue(result.Contains(d1))
                Assert.IsTrue(result.Contains(d2))
                Assert.IsTrue(result.Contains(d3))
            End Using
        End Sub

        ''' <summary>
        ''' Verify GetAllDepartments returns an empty IEnumerable when no data is present
        ''' </summary>
        <TestMethod()>
        Public Sub GetAllDepartmentsEmpty()
            Using ctx As New FakeEmployeeContext()
                Dim rep As New DepartmentRepository(ctx)

                Dim result As IEnumerable(Of Department) = rep.GetAllDepartments()
                Assert.AreEqual(0, result.Count())
            End Using
        End Sub

        ''' <summary>
        ''' Verify ArgumentNullException when invalid null parameters are specified
        ''' </summary>
        <TestMethod()>
        Public Sub NullArgumentChecks()
            Dim ctor As Action = Sub()
                                     Dim x = New DepartmentRepository(Nothing)
                                 End Sub

            Utilities.CheckNullArgumentException(ctor, "context", "ctor")
        End Sub
    End Class
End Namespace
