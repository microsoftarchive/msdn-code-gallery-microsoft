' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Customer

    Private custAccount As String
    Private custFirstName As String
    Private custLastName As String

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
