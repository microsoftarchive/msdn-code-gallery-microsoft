' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel of an individual Address
    ''' </summary>
    Public Class AddressViewModel
        Inherits ContactDetailViewModel
        ''' <summary>
        ''' The Address object backing this ViewModel
        ''' </summary>
        Private address As Address

        ''' <summary>
        ''' Initializes a new instance of the AddressViewModel class.
        ''' </summary>
        ''' <param name="detail">The underlying Address this ViewModel is to be based on</param>
        Public Sub New(ByVal detail As Address)
            If detail Is Nothing Then
                Throw New ArgumentNullException("detail")
            End If

            Me.address = detail
        End Sub

        ''' <summary>
        ''' The underlying Address this ViewModel is based on
        ''' </summary>
        Public Overrides ReadOnly Property Model() As ContactDetail
            Get
                Return Me.address
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the first address line
        ''' </summary>
        Public Property LineOne() As String
            Get
                Return Me.address.LineOne
            End Get

            Set(ByVal value As String)
                Me.address.LineOne = value
                Me.OnPropertyChanged("LineOne")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the second address line
        ''' </summary>
        Public Property LineTwo() As String
            Get
                Return Me.address.LineTwo
            End Get

            Set(ByVal value As String)
                Me.address.LineTwo = value
                Me.OnPropertyChanged("LineTwo")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the city of this address
        ''' </summary>
        Public Property City() As String
            Get
                Return Me.address.City
            End Get

            Set(ByVal value As String)
                Me.address.City = value
                Me.OnPropertyChanged("City")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the state of this address
        ''' </summary>
        Public Property State() As String
            Get
                Return Me.address.State
            End Get

            Set(ByVal value As String)
                Me.address.State = value
                Me.OnPropertyChanged("State")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the zip code of this address
        ''' </summary>
        Public Property ZipCode() As String
            Get
                Return Me.address.ZipCode
            End Get

            Set(ByVal value As String)
                Me.address.ZipCode = value
                Me.OnPropertyChanged("ZipCode")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the country of this address
        ''' </summary>
        Public Property Country() As String
            Get
                Return Me.address.Country
            End Get

            Set(ByVal value As String)
                Me.address.Country = value
                Me.OnPropertyChanged("Country")
            End Set
        End Property
    End Class
End Namespace
