''' <summary>
''' A Customer that purchases goods and services
''' </summary>
Public Class Customer
    Public Sub New()
        _id = -1
        _name = String.Empty
    End Sub
    Public Sub New(ByVal id As Integer, ByVal name As String)
        _id = id
        _name = name
    End Sub
    Private _id As Integer
    ''' <summary>
    ''' The ID of the Customer
    ''' </summary>
    ''' <value></value>
    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property
    Private _name As String
    ''' <summary>
    ''' The name of the customer
    ''' </summary>
    ''' <value></value>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property
End Class
