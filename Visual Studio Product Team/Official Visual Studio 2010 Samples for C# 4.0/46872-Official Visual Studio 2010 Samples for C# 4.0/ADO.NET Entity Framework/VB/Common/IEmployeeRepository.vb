' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports EmployeeTracker.Model

Namespace EmployeeTracker.Model.Interfaces

    ''' <summary>
    ''' Repository for retrieving employee data
    ''' </summary>
    Public Interface IEmployeeRepository
        ''' <summary>
        ''' All employees for the company
        ''' </summary>
        ''' <returns>Enumerable of all employees</returns>  
        Function GetAllEmployees() As IEnumerable(Of Employee)

        ''' <summary>
        ''' Gets the longest serving employees
        ''' </summary>
        ''' <param name="quantity">The number of employees to return</param>
        ''' <returns>Enumerable of the longest serving employees</returns>
        Function GetLongestServingEmployees(ByVal quantity As Integer) As IEnumerable(Of Employee)
    End Interface
End Namespace
