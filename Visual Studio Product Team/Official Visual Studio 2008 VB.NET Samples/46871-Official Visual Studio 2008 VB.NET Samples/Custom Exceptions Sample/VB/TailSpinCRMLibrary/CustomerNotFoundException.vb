''' <summary>
''' Exception denoting that the customer could not be found
''' </summary>
Public Class CustomerNotFoundException
    Inherits CustomerException

    Public Sub New(ByVal Message As String)
        ' We pass Nothing for the Customer
        ' since we couldn't find them.
        MyBase.New(Message, Nothing)
    End Sub
End Class

