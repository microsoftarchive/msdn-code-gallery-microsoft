' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class CustomerWithConstructor

    Private custAccount As String
    Private custFirstName As String
    Private custLastName As String

    Sub New(ByVal AccountNumber As String, ByVal FirstName As String, ByVal LastName As String)

        ' This is the Constructor for this class.

        Me.AccountNumber = AccountNumber
        Me.FirstName = FirstName
        Me.LastName = LastName

    End Sub

#Region "Customer Properties"
    Public Property AccountNumber() As String
        Get
            Return custAccount
        End Get
        Set(ByVal Value As String)
            custAccount = Value
        End Set
    End Property

    Public Property FirstName() As String
        Get
            Return custFirstName
        End Get
        Set(ByVal Value As String)
            custFirstName = Value
        End Set
    End Property

    Public Property LastName() As String
        Get
            Return custLastName
        End Get
        Set(ByVal Value As String)
            custLastName = Value
        End Set
    End Property
#End Region


End Class
