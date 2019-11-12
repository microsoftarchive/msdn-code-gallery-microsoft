' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Namespace EmployeeTracker.Model

    ''' <summary>
    ''' Represents an Employees address
    ''' </summary>
    Public Class Address
        Inherits ContactDetail
        ''' <summary>
        ''' Usage values that are valid for addresses
        ''' </summary>
        Private Shared validUsageValues_Renamed() As String = { "Business", "Home", "Mailing" }

        ''' <summary>
        ''' Gets a list of usage values that are valid for addresses
        ''' </summary>
        Public Overrides ReadOnly Property ValidUsageValues() As IEnumerable(Of String)
            Get
                Return validUsageValues_Renamed
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the first line of this Address
        ''' </summary>
        Public Overridable Property LineOne() As String

        ''' <summary>
        ''' Gets or sets the second line of this Address
        ''' </summary>
        Public Overridable Property LineTwo() As String

        ''' <summary>
        ''' Gets or sets the city of this Address
        ''' </summary>
        Public Overridable Property City() As String

        ''' <summary>
        ''' Gets or sets the state of this Address
        ''' </summary>
        Public Overridable Property State() As String

        ''' <summary>
        ''' Gets or sets the zipc code of this Address
        ''' </summary>
        Public Overridable Property ZipCode() As String

        ''' <summary>
        ''' Gets or sets the country of this Address
        ''' </summary>
        Public Overridable Property Country() As String
    End Class
End Namespace
