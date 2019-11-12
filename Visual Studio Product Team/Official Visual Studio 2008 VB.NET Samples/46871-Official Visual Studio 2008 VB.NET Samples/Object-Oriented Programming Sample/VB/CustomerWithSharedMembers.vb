' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class CustomerWithSharedMembers

    Private custAccount As String
    Private custFirstName As String
    Private custLastName As String

    ' The following variable (CompanyName) is declared as
    ' Shared. All instances of this class will use the same
    ' instance of this variable. If it is changed in any
    ' instance of this class, it will be changed in all
    ' instances.
    Public Shared CompanyName As String

    Public Shared Function LastOrderDate() As Date

        ' This is a shared function. Refer to the calling
        ' code in frmMain for an explanation of how to 
        ' invoke a procedure declared as Shared.

        Dim orderDate As Date

        ' Normally you would look up in a database or
        ' collection the date of the last order here.
        ' For brevity we'll return a dummy date.

        orderDate = Now
        Return orderDate

    End Function

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
