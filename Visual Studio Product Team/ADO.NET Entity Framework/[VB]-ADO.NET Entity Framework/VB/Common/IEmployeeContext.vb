' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System
Imports System.Data.Objects
Imports EmployeeTracker.Model

Namespace EmployeeTracker.Common

    ''' <summary>
    ''' Data context containing data for the EmployeeTracker model
    ''' </summary>
    Public Interface IEmployeeContext
        Inherits IDisposable
        ''' <summary>
        ''' Gets Employees in the data context
        ''' </summary>
        ReadOnly Property Employees() As IObjectSet(Of Employee)

        ''' <summary>
        ''' Gets Departments in the data context
        ''' </summary>
        ReadOnly Property Departments() As IObjectSet(Of Department)

        ''' <summary>
        ''' Gets ContactDetails in the data context
        ''' </summary>
        ReadOnly Property ContactDetails() As IObjectSet(Of ContactDetail)

        ''' <summary>
        ''' Save all pending changes to the data context
        ''' </summary>
        Sub Save()

        ''' <summary>
        ''' Creates a new instance of the specified object type
        ''' NOTE: This pattern is used to allow the use of change tracking proxies
        '''       when running against the Entity Framework.
        ''' </summary>
        ''' <typeparam name="T">The type to create</typeparam>
        ''' <returns>The newly created object</returns>
        Function CreateObject(Of T As Class)() As T

        ''' <summary>
        ''' Checks if the supplied object is tracked in this data context
        ''' </summary>
        ''' <param name="obj">The object to check for</param>
        ''' <returns>True if the object is tracked, false otherwise</returns>
        Function IsObjectTracked(ByVal obj As Object) As Boolean
    End Interface
End Namespace
