''' <summary>
''' Exception that is raised when an error is raised when a customer is deleted.
''' </summary>
Public Class CustomerNotDeletedException
    Inherits CustomerException

    Private userIdValue As String

    Public Sub New(ByVal Message As String, ByVal ReqCustomer As Customer, ByVal UserId As String)
        MyBase.New(Message, ReqCustomer)
        Me.userIdValue = UserId
    End Sub

    Public ReadOnly Property UserId() As String
        Get
            Return userIdValue
        End Get
    End Property
End Class