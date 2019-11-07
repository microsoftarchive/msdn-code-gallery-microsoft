' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports EmployeeTracker.Model

Namespace EmployeeTracker.Model.Interfaces

    ''' <summary>
    ''' Repository for retrieving department data
    ''' </summary>
    Public Interface IDepartmentRepository
        ''' <summary>
        ''' All departments for the company
        ''' </summary>
        ''' <returns>Enumerable of all departments</returns>
        Function GetAllDepartments() As IEnumerable(Of Department)
    End Interface
End Namespace
