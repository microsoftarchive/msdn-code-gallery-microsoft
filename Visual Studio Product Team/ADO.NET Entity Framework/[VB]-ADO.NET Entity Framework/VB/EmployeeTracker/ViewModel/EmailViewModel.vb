' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel of an individual Email
    ''' </summary>
    Public Class EmailViewModel
        Inherits ContactDetailViewModel
        ''' <summary>
        ''' The Email object backing this ViewModel
        ''' </summary>
        Private email As Email

        ''' <summary>
        ''' Initializes a new instance of the EmailViewModel class.
        ''' </summary>
        ''' <param name="detail">The underlying Email this ViewModel is to be based on</param>
        Public Sub New(ByVal detail As Email)
            If detail Is Nothing Then
                Throw New ArgumentNullException("detail")
            End If

            Me.email = detail
        End Sub

        ''' <summary>
        ''' Gets the underlying Email this ViewModel is based on
        ''' </summary>
        Public Overrides ReadOnly Property Model() As ContactDetail
            Get
                Return Me.email
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the actual email address
        ''' </summary>
        Public Property Address() As String
            Get
                Return Me.email.Address
            End Get

            Set(ByVal value As String)
                Me.email.Address = value
                Me.OnPropertyChanged("Address")
            End Set
        End Property
    End Class
End Namespace
