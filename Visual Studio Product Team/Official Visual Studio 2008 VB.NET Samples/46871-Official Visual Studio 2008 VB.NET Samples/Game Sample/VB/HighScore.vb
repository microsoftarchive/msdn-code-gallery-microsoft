''' <summary>
''' Represents one high score.
''' </summary>
''' <remarks></remarks>
Public Class HighScore
    Implements IComparable

    Public nameValue As String
    Public scoreValue As Integer

    Public Property Name() As String
        Get
            Return nameValue
        End Get
        Set(ByVal Value As String)
            nameValue = Value
        End Set
    End Property

    Public Property Score() As Integer
        Get
            Return scoreValue
        End Get
        Set(ByVal Value As Integer)
            scoreValue = Value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return Name & ":" & Score
    End Function

    Public Sub New(ByVal saved As String)
        Name = saved.Split(":".ToCharArray)(0)
        Score = CInt(saved.Split(":".ToCharArray)(1))
    End Sub

    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
        Dim other As HighScore
        other = CType(obj, HighScore)
        Return Me.Score - other.Score
    End Function
End Class