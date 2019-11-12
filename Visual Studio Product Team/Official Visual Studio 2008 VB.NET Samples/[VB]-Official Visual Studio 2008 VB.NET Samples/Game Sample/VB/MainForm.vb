' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Dim matrix As Grid
    Dim score As Integer = 0
    Private mouseOffset As Point
    Private paused As Boolean = False
    Private isSoundOn As Boolean = True

    'If there is no title bar, you can move the form just by dragging it.
    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        mouseOffset = New Point(-e.X, -e.Y)
    End Sub

    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Dim mousePos As Point = Control.MousePosition
            mousePos.Offset(mouseOffset.X, mouseOffset.Y)
            Location = mousePos
        End If
    End Sub

    Private Sub BlockClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        ' Play the sound.
        If isSoundOn Then
            My.Computer.Audio.Play(My.Resources.Balloon, AudioPlayMode.Background)
        End If

        ' Update the matrix and compute the new score.
        Dim count As Integer = matrix.Click(New Point(e.X, e.Y))
        score += 10 * count

        ' Draw the new grid.
        matrix.Draw(Me.PictureBox1.CreateGraphics(), Me.PictureBox1.BackColor)

        ' Write the score on the screen.
        Dim images() As PictureBox = { _
            Me.tenthousands, Me.thousands, Me.hundreds, Me.tens, Me.ones}

        Dim scoreString As String = score.ToString().PadLeft(5)
        Dim digits() As String = { _
            scoreString.Chars(0), _
            scoreString.Chars(1), _
            scoreString.Chars(2), _
            scoreString.Chars(3), _
            scoreString.Chars(4)}

        For index As Integer = 0 To 4
            If digits(index) <> " " Then
                images(index).Image = numbers.Images(CInt(digits(index)))
            Else
                images(index).Image = Nothing
            End If
        Next
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        ' Add another row to the grid and update the screen.
        matrix.AddRow()
        matrix.Draw(Me.PictureBox1.CreateGraphics(), Me.PictureBox1.BackColor)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PointTranslator.Graphics = Me.PictureBox1.CreateGraphics()
        Me.PictureBox1.Width = Block.BlockSize * 12
        Me.PictureBox1.Height = Block.BlockSize * 15
        HighScores.SetUpHighScores()

        ' Setup the background color and the starting score.
        Me.BackColor = Color.FromArgb(0, 255, 255)
        Me.ones.Image = Me.numbers.Images(0)
        Me.tens.Image = Me.numbers.Images(0)
        Me.hundreds.Image = Me.numbers.Images(0)

        ' Make a borderless form.
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        Me.Menu = Nothing
    End Sub

    Private Sub StartNewGame()
        ' If a game is already running, check for a new high score.
        If Not matrix Is Nothing Then
            Me.Timer1.Enabled = False
            HighScores.UpdateScores(score)
        End If

        Timer1.Enabled = False
        matrix = New Grid(6)
        score = 0
        matrix.Draw(Me.PictureBox1.CreateGraphics(), Me.PictureBox1.BackColor)
        Timer1.Enabled = True
        AddHandler PictureBox1.MouseDown, AddressOf BlockClick
    End Sub


    Private Sub newGameMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles newGameMenu.Click
        StartNewGame()
    End Sub

    Private Sub newGame_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles newGame.Click
        StartNewGame()
    End Sub


    ' To pause the game, turn off the timer.
    Private Sub Pause()
        Timer1.Enabled = False
        Me.pauseMenu.Visible = False
        Me.restartMenu.Visible = True
        RemoveHandler PictureBox1.MouseDown, AddressOf BlockClick
        paused = True
    End Sub

    Private Sub pauseMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pauseMenu.Click
        Me.Pause()
    End Sub

    Private Sub Restart()
        Timer1.Enabled = True
        Me.pauseMenu.Visible = True
        Me.restartMenu.Visible = False
        AddHandler PictureBox1.MouseDown, AddressOf BlockClick
        paused = False
    End Sub

    Private Sub restartMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles restartMenu.Click
        Restart()
    End Sub


    Private Sub exitMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitMenu.Click
        Me.EndGame()
    End Sub

    Private Sub EndGame()
        ' Get top scores so far.
        Me.Timer1.Enabled = False
        HighScores.UpdateScores(score)
        Me.Close()
    End Sub

    Private Sub exitGame_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitGame.Click
        EndGame()
    End Sub

    Private Sub options_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles options.Click
        ShowOptions()
    End Sub

    Private Sub ShowOptions()
        Dim optionsForm As New Options
        optionsForm.SoundOn = isSoundOn
        optionsForm.ShowDialog()
        isSoundOn = optionsForm.SoundOn
        optionsForm.Dispose()
    End Sub

    Private Sub Form1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        Select Case e.KeyChar
            Case "p"c, "P"c
                If paused Then
                    Restart()
                Else
                    Pause()
                End If
            Case "m"c, "M"c
                If Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D Then
                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
                    Me.Menu = Nothing
                Else
                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D
                    Me.Menu = Me.MainMenu1
                End If
            Case Else
                ' Do nothing.
        End Select
    End Sub

    Private Sub optionsMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optionsMenu.Click
        Dim optionsForm As New Options
        optionsForm.ShowDialog()
    End Sub
End Class
