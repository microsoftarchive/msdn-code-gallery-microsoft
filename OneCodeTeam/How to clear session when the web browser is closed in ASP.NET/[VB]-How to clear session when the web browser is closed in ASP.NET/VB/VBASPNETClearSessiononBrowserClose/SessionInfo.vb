Public Class SessionInfo
    ' Dummy session variable value
    Private _sessionValue As String

    ' Closed time of the browser
    Private _closedTime As DateTime

    Public Property SessionValue() As String
        Get
            Return _sessionValue
        End Get
        Set(value As String)
            _sessionValue = value
        End Set
    End Property


    Public Property BrowserClosedTime() As DateTime
        Get
            Return _closedTime
        End Get
        Set(value As DateTime)
            _closedTime = value
        End Set
    End Property

End Class
