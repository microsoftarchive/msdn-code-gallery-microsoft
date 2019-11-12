' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Namespace EmployeeTracker.Model

    ''' <summary>
    ''' Base class representing a contact detail of an Employee
    ''' </summary>
    Public MustInherit Class ContactDetail
        ''' <summary>
        ''' Gets values that are valid for the usage property
        ''' </summary>
        Public MustOverride ReadOnly Property ValidUsageValues() As IEnumerable(Of String)

        ''' <summary>
        ''' Gets or sets the Id of this ContactDetail
        ''' </summary>
        Public Overridable Property ContactDetailId() As Integer

        ''' <summary>
        ''' Gets or sets the Id of the Employee this ContactDetail belongs to
        ''' </summary>
        Public Overridable Property EmployeeId() As Integer

        ''' <summary>
        ''' Gets or sets how this contact detail is used i.e. Home/Business etc.
        ''' </summary>
        Public Overridable Property Usage() As String
    End Class
End Namespace
