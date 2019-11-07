' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Data.Objects
Imports System.Linq
Imports EmployeeTracker.Model
Imports EmployeeTracker.Model.Interfaces

Namespace EmployeeTracker.Common

    ''' <summary>
    ''' Repository for retrieving department data from an IObjectSet
    ''' </summary>
    Public Class DepartmentRepository
        Implements IDepartmentRepository
        ''' <summary>
        ''' Underlying ObjectSet to retrieve data from
        ''' </summary>
        Private objectSet As IObjectSet(Of Department)

        ''' <summary>
        ''' Initializes a new instance of the DepartmentRepository class.
        ''' </summary>
        ''' <param name="context">Context to retrieve data from</param>
        Public Sub New(ByVal context As IEmployeeContext)
            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            Me.objectSet = context.Departments
        End Sub

        ''' <summary>
        ''' All departments for the company
        ''' </summary>
        ''' <returns>Enumerable of all departments</returns>
        Public Function GetAllDepartments() As IEnumerable(Of Department) Implements IDepartmentRepository.GetAllDepartments
            ' NOTE: Some points considered during implementation of data access methods:
            '    -  ToList is used to ensure any data access related exceptions are thrown
            '       during execution of this method rather than when the data is enumerated.
            '    -  Returning IEnumerable rather than IQueryable ensures the repository has full control
            '       over how data is retrieved from the store, returning IQueryable would allow consumers
            '       to add additional operators and affect the query sent to the store.
            Return Me.objectSet.ToList()
        End Function
    End Class
End Namespace
