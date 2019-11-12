''' <summary>
''' Generic container for a pair of values
''' </summary>
''' <typeparam name="T">First value type</typeparam>
''' <typeparam name="V">Second value type</typeparam>
''' <remarks>Useful to pass a pair of values; 
''' Types T and V are parameters; 
''' Use Matches function to compare Pair objects.
''' </remarks>
Public Class Pair(Of T, V)

    ' Internal variables
    ' Note how types T and V are variable.
    Private fVal As T = Nothing
    Private sVal As V = Nothing

    ' Tests to see if types and values match for Pair first and second values.
    Public Function Matches(ByVal value As Pair(Of T, V)) As Boolean
        If value IsNot Me Then
            If value.FirstValue.Equals(Me.FirstValue) Then
                If value.SecondValue.Equals(Me.SecondValue) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End If
    End Function


    Public Property FirstValue() As T
        Get
            Return fVal
        End Get
        Set(ByVal value As T)
            fVal = value
        End Set
    End Property

    Public Property SecondValue() As V
        Get
            Return sVal
        End Get
        Set(ByVal value As V)
            sVal = value
        End Set
    End Property

End Class