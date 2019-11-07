' Copyright (c) Microsoft Corporation. All rights reserved.
Imports Microsoft.Win32

''' <summary>
''' Reads and writes the top three high scores to the registry.
''' </summary>
''' <remarks></remarks>
Public Class HighScores

    ''' <summary>
    ''' Read scores from the registry.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetHighScores() As HighScore()
        Dim tops(2) As HighScore
        Dim scoreKey As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\VBSamples\Collapse\HighScores")
        For index As Integer = 0 To 2
            Dim key As String = "place" & index.ToString
            Dim score As New HighScore(CStr(scoreKey.GetValue(key)))
            tops(index) = score
        Next
        scoreKey.Close()
        Return tops
    End Function


    ''' <summary>
    ''' Update and write the high scores.
    ''' </summary>
    ''' <param name="score"></param>
    ''' <remarks></remarks>
    Public Shared Sub UpdateScores(ByVal score As Integer)
        Dim tops(3) As HighScore
        Dim scoreKey As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\VBSamples\Collapse\HighScores")
        tops(0) = New HighScore(scoreKey.GetValue("Place0").ToString)
        tops(1) = New HighScore(scoreKey.GetValue("Place1").ToString)
        tops(2) = New HighScore(scoreKey.GetValue("Place2").ToString)
        If score > tops(2).Score Then
            Dim name As String = InputBox("New high score of " & score & " for:")
            tops(3) = New HighScore(" :0")
            tops(3).Name = name
            tops(3).Score = score
            Array.Sort(tops)
            Array.Reverse(tops)
            scoreKey.SetValue("Place0", tops(0).ToString)
            scoreKey.SetValue("Place1", tops(1).ToString)
            scoreKey.SetValue("Place2", tops(2).ToString)
        End If
        scoreKey.Close()
    End Sub

    ''' <summary>
    ''' Set up the entries for new scores.
    ''' </summary>
    ''' <remarks></remarks>
    Shared Sub SetUpHighScores()
        Dim scoreKey As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\VBSamples\Collapse\HighScores")
        If scoreKey.GetValue("Place1") Is Nothing Then
            scoreKey.SetValue("Place0", " :0")
            scoreKey.SetValue("Place1", " :0")
            scoreKey.SetValue("Place2", " :0")
        End If
        scoreKey.Close()
    End Sub

    ''' <summary>
    ''' Reset scores.
    ''' </summary>
    ''' <remarks></remarks>
    Shared Sub ResetScores()
        Dim scoreKey As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\VBSamples\Collapse\HighScores")
        scoreKey.SetValue("Place0", " :0")
        scoreKey.SetValue("Place1", " :0")
        scoreKey.SetValue("Place2", " :0")
        scoreKey.Close()
    End Sub

End Class




