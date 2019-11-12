' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Data.Objects
Imports System.Data.Objects.DataClasses
Imports System.Linq
Imports EmployeeTracker.Model
Imports EmployeeTracker.Model.Interfaces

Namespace EmployeeTracker.Common

    ''' <summary>
    ''' Repository for retrieving employee data from an ObjectSet
    ''' </summary>
    Public Class EmployeeRepository
        Implements IEmployeeRepository
        ''' <summary>
        ''' Underlying ObjectSet to retrieve data from
        ''' </summary>
        Private objectSet As IObjectSet(Of Employee)

        ''' <summary>
        ''' Initializes a new instance of the EmployeeRepository class.
        ''' </summary>
        ''' <param name="context">Context to retrieve data from</param>
        Public Sub New(ByVal context As IEmployeeContext)
            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            Me.objectSet = context.Employees
        End Sub

        ''' <summary>
        ''' All employees for the company
        ''' </summary>
        ''' <returns>Enumerable of all employees</returns>  
        Public Function GetAllEmployees() As IEnumerable(Of Employee) Implements IEmployeeRepository.GetAllEmployees
            ' NOTE: Some points considered during implementation of data access methods:
            '    -  ToList is used to ensure any data access related exceptions are thrown
            '       during execution of this method rather than when the data is enumerated.
            '    -  Returning IEnumerable rather than IQueryable ensures the repository has full control
            '       over how data is retrieved from the store, returning IQueryable would allow consumers
            '       to add additional operators and affect the query sent to the store.
            Return Me.objectSet.ToList()
        End Function

        ''' <summary>
        ''' Gets the longest serving employees
        ''' </summary>
        ''' <param name="quantity">The number of employees to return</param>
        ''' <returns>Enumerable of the longest serving employees</returns>
        Public Function GetLongestServingEmployees(ByVal quantity As Integer) As IEnumerable(Of Employee) Implements IEmployeeRepository.GetLongestServingEmployees
            ' NOTE: When running against a fake object set the sort on tenure will happen in memory
            '       When running against EF the Model Defined Function declared in EmployeeModel.edmx
            '       will be used and the sort will be processed in the store
            Return Me.objectSet.Where(Function(e) e.TerminationDate Is Nothing).OrderByDescending(Function(e) GetTenure(e)).Take(quantity).ToList()
        End Function

        ''' <summary>
        ''' Calculates the duration of employment of an employee at the comapny
        ''' </summary>
        ''' <param name="employee">The employee to calculate tenure for</param>
        ''' <returns>Tenure expressed in years</returns>
        <EdmFunction("EmployeeTracker.EntityFramework", "GetTenure")>
        Private Shared Function GetTenure(ByVal employee As Employee) As Integer
            ' NOTE: The body for this method is included to facilitate running against in-memory fakes
            '       EF does not require an implementation, see notes in GetLongestServingEmployees()
            Dim endDate As DateTime = If(employee.TerminationDate, DateTime.Today)
            Return CInt(endDate.Subtract(employee.HireDate).Days / 365)
        End Function
    End Class
End Namespace
