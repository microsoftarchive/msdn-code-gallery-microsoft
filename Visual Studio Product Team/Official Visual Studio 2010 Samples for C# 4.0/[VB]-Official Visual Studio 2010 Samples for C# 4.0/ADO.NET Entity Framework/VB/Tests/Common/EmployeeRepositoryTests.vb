' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports EmployeeTracker.Common
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.Common

    ''' <summary>
    ''' Tests retrieval of data from an EmployeeObjectSetRepository
    ''' </summary>
    <TestClass>
    Public Class EmployeeRepositoryTests
        ''' <summary>
        ''' Verify GetAllEmployees returns all data from base ObjectSet
        ''' </summary>
        <TestMethod>
        Public Sub GetAllEmployees()
            Dim e1 As New Employee()
            Dim e2 As New Employee()
            Dim e3 As New Employee()

            Using ctx As New FakeEmployeeContext(New Employee() { e1, e2, e3 }, New Department() { })
                Dim rep As New EmployeeRepository(ctx)

                Dim result As IEnumerable(Of Employee) = rep.GetAllEmployees()

                Assert.IsNotInstanceOfType(result, GetType(IQueryable), "Repositories should not return IQueryable as this allows modification of the query that gets sent to the store. ")

                Assert.AreEqual(3, result.Count())
                Assert.IsTrue(result.Contains(e1))
                Assert.IsTrue(result.Contains(e2))
                Assert.IsTrue(result.Contains(e3))
            End Using
        End Sub

        ''' <summary>
        ''' Verify GetAllEmployees returns an empty IEnumerable when no data is present
        ''' </summary>
        <TestMethod>
        Public Sub GetAllEmployeesEmpty()
            Using ctx As New FakeEmployeeContext()
                Dim rep As New EmployeeRepository(ctx)

                Dim result As IEnumerable(Of Employee) = rep.GetAllEmployees()
                Assert.AreEqual(0, result.Count())
            End Using
        End Sub

        ''' <summary>
        ''' Verify GetLongestServingEmployees dorrectly filters and sorts data
        ''' </summary>
        <TestMethod>
        Public Sub GetLongestServingEmployees()
            Dim e1 As Employee = New Employee With {.HireDate = New DateTime(2003, 1, 1)}
            Dim e2 As Employee = New Employee With {.HireDate = New DateTime(2001, 1, 1)}
            Dim e3 As Employee = New Employee With {.HireDate = New DateTime(2000, 1, 1)}
            Dim e4 As Employee = New Employee With {.HireDate = New DateTime(2002, 1, 1)}

            ' The following employee verifies GetLongestServingEmployees does not return terminated employees
            Dim e5 As Employee = New Employee With {.HireDate = New DateTime(1999, 1, 1), .TerminationDate = DateTime.Today}

            Using ctx As New FakeEmployeeContext(New Employee() { e1, e2, e3, e4, e5 }, New Department() { })
                Dim rep As New EmployeeRepository(ctx)

                ' Select a subset
                Dim result As List(Of Employee) = rep.GetLongestServingEmployees(2).ToList()
                Assert.AreEqual(2, result.Count, "Expected two items in result.")
                Assert.AreSame(e3, result(0), "Incorrect item at position 0.")
                Assert.AreSame(e2, result(1), "Incorrect item at position 1.")

                ' Select more than are present
                result = rep.GetLongestServingEmployees(50).ToList()
                Assert.AreEqual(4, result.Count, "Expected four items in result.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify ArgumentNullException when invalid null parameters are specified
        ''' </summary>
        <TestMethod>
        Public Sub NullArgumentChecks()
            Dim ctor As Action = Sub()
                                     Dim x = New EmployeeRepository(Nothing)
                                 End Sub

            Utilities.CheckNullArgumentException(ctor, "context", "ctor")
        End Sub
    End Class
End Namespace
