' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel of an individual Phone
    ''' </summary>
    Public Class PhoneViewModel
        Inherits ContactDetailViewModel
        ''' <summary>
        ''' The Phone object backing this ViewModel
        ''' </summary>
        Private phone As Phone

        ''' <summary>
        ''' Initializes a new instance of the PhoneViewModel class.
        ''' </summary>
        ''' <param name="detail">The underlying Phone this ViewModel is to be based on</param>
        Public Sub New(ByVal detail As Phone)
            If detail Is Nothing Then
                Throw New ArgumentNullException("detail")
            End If

            Me.phone = detail
        End Sub

        ''' <summary>
        ''' The underlying Phone this ViewModel is based on
        ''' </summary>
        Public Overrides ReadOnly Property Model() As ContactDetail
            Get
                Return Me.phone
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the actual number
        ''' </summary>
        Public Property Number() As String
            Get
                Return Me.phone.Number
            End Get

            Set(ByVal value As String)
                Me.phone.Number = value
                Me.OnPropertyChanged("Number")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the extension to be used with this phone number
        ''' </summary>
        Public Property Extension() As String
            Get
                Return Me.phone.Extension
            End Get

            Set(ByVal value As String)
                Me.phone.Extension = value
                Me.OnPropertyChanged("Extension")
            End Set
        End Property
    End Class
End Namespace
