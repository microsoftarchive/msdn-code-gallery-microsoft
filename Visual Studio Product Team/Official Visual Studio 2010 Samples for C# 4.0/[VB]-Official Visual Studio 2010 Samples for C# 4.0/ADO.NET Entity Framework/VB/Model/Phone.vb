' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Namespace EmployeeTracker.Model

    ''' <summary>
    ''' Represents an employees phone number
    ''' </summary>
    Public Class Phone
        Inherits ContactDetail
        ''' <summary>
        ''' Usage values that are valid for phone numbers
        ''' </summary>
        Private Shared validUsageValues_Renamed() As String = { "Business", "Home", "Cell" }

        ''' <summary>
        ''' Gets a list of usage values that are valid for phone numbers
        ''' </summary>
        Public Overrides ReadOnly Property ValidUsageValues() As IEnumerable(Of String)
            Get
                Return validUsageValues_Renamed
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the actual phone number
        ''' </summary>
        Public Overridable Property Number() As String

        ''' <summary>
        ''' Gets or sets the extension associated with this phone number
        ''' </summary>
        Public Overridable Property Extension() As String
    End Class
End Namespace
