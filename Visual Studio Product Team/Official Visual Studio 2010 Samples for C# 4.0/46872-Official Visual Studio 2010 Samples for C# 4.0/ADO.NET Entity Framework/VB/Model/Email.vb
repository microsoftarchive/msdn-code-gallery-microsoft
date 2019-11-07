' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Namespace EmployeeTracker.Model

    ''' <summary>
    ''' Represents an Employees email address
    ''' </summary>
    Public Class Email
        Inherits ContactDetail
        ''' <summary>
        ''' Usage values that are valid for email addresses
        ''' </summary>
        Private Shared validUsageValues_Renamed() As String = { "Business", "Personal" }

        ''' <summary>
        ''' Gets a list of usage values that are valid for email addresses
        ''' </summary>
        Public Overrides ReadOnly Property ValidUsageValues() As IEnumerable(Of String)
            Get
                Return validUsageValues_Renamed
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the actual email address
        ''' </summary>
        Public Overridable Property Address() As String
    End Class
End Namespace
