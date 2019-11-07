Public Class InvalidQueryException
    Inherits Exception

    Private _message As String

    Public Sub New(ByVal message As String)
        Me._message = message & " "
    End Sub

    Public Overrides ReadOnly Property Message() As String
        Get
            Return "The client query is invalid: " & _message
        End Get
    End Property
End Class
