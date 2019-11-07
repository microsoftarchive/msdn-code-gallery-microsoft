Imports Microsoft.VisualBasic.PowerPacks

' This sample demonstrates the use of Line and Shape controls to create a classic
' board game with rich graphics.

Public Class FiveStoneForm

    Private c(9, 9) As OvalShape
    Private a(9, 9) As Integer
    Private d(4, 2) As Integer
    Private turn As Integer
    Private count As Integer
    Private WithEvents Surface As ShapeContainer


    Private Sub Surface_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Surface.Load
        ' This procedure creates the "stone" shapes.

        Dim i As Integer
        Dim j As Integer
        Dim x As Integer
        Dim y As Integer = -50
        Dim pt As Point

        ' Calculates the point for each intesection of lines.
        For i = 1 To 9
            x = -50
            y += 70
            For j = 1 To 9
                ' Create a new stone.
                c(i, j) = New OvalShape
                x += 70
                pt = New Point(x, y)
                ' Set a gradient fill.
                c(i, j).FillGradientStyle = FillGradientStyle.Central
                c(i, j).Width = 40
                c(i, j).Height = 40
                ' Hide the new shape.
                c(i, j).Visible = False
                c(i, j).Location = pt
                ' Add the new shape.
                Me.Surface.Shapes.Add(c(i, j))
            Next
        Next
    End Sub

    Private Function check(ByVal x As Integer, ByVal y As Integer) As Boolean
        ' This procedure checks to see if the current player
        ' has five stones in a row.
        Dim i As Integer
        Dim k As Integer
        Dim ii As Integer
        Dim jj As Integer

        For i = 1 To 4
            ii = x
            jj = y
            k = 0

            While a(ii, jj) = turn
                k += 1
                ii += d(i, 1)
                jj += d(i, 2)
                If ii < 1 Or ii > 9 Or jj < 1 Or jj > 9 Then
                    Exit While
                End If
            End While

            ii = x
            jj = y
            While a(ii, jj) = turn
                k += 1
                ii -= d(i, 1)
                jj -= d(i, 2)
                If ii < 1 Or ii > 9 Or jj < 1 Or jj > 9 Then
                    Exit While
                End If
            End While

            If k >= 6 Then
                Return True
            End If

        Next
        Return False
    End Function

    Private Sub clean()
        ' Clears the board for a new game.
        Dim i As Integer
        Dim j As Integer

        For i = 1 To 9
            For j = 1 To 9
                If a(i, j) = -1 Then
                    c(i, j).Visible = False
                    RemoveHandler c(i, j).Click, AddressOf Oval_Click
                End If
            Next
        Next
    End Sub

    Private Sub Oval_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' This procedure is used to make a stone visible when
        ' the player clicks on an intersection of lines.
        '
        ' Set the next player's turn.
        turn = 1 - turn
        Dim o As OvalShape = sender
        If turn = 0 Then
            ' White player's turn - change the color
            ' of the selected stone.
            o.BorderColor = Color.Silver
            o.FillColor = Color.Silver
            o.FillGradientColor = Color.White
            ' Change the labels to show the next turn.
            Me.Label4.Visible = True
            Me.Label3.Visible = False
        Else
            ' Black player's turn - change the color
            ' of the selected stone.
            o.BorderColor = Color.Black
            o.FillColor = Color.Black
            o.FillGradientColor = Color.Gray
            ' Change the labels to show the next turn.
            Me.Label4.Visible = False
            Me.Label3.Visible = True
        End If

        ' Set the fill style - this makes the stone visible.
        o.FillStyle = FillStyle.Solid
        Dim i As Integer = (o.Location.Y - 20) / 70 + 1
        Dim j As Integer = (o.Location.X - 20) / 70 + 1

        a(i, j) = turn
        count += 1
        ' Disable the Click event for the stone.
        RemoveHandler o.Click, AddressOf Oval_Click

        ' Call the check procedure to see if the player has won.
        If check(i, j) Then
            ' Clear the board and display a message.
            clean()
            If turn = 0 Then
                MessageBox.Show("White wins!")
            Else
                MessageBox.Show("Black wins!")
            End If
            Exit Sub
        End If

        ' If all stones are visible and there is no
        ' winner, it's a draw.
        If count = 81 Then
            MessageBox.Show("Draw!")
        End If
    End Sub

    Private Sub Form4_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' This procedure sets up the game board.
        d(1, 1) = 1
        d(3, 2) = 1
        d(2, 1) = 1
        d(2, 2) = 1
        d(1, 2) = 0
        d(3, 1) = 0
        d(4, 2) = 1
        d(4, 1) = -1
        Surface = New ShapeContainer
        Surface.Parent = ShapeContainer1
        StartNewGame()
    End Sub
    Private Sub DrawButton()
        ' This procedure uses Graphics methods to draw text on the
        ' Start button, which is actually a RectangleShape control.
        Dim drawFormat As New StringFormat()

        Using formGraphics As Graphics = Me.CreateGraphics(), _
            drawFont As New System.Drawing.Font("Arial", 10), _
            drawBrush As New SolidBrush(Color.Black)

            formGraphics.DrawString("Start a new game", drawFont, drawBrush, _
                RectangleShape3.Left + 8, RectangleShape3.Top + 15, drawFormat)
        End Using

    End Sub

    Private Sub RectangleShape3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RectangleShape3.Click
        ' Calls the StartNewGame procedure.
        StartNewGame()
    End Sub
    Private Sub StartNewGame()
        ' This procedure sets up the board for a new game.
        Dim i As Integer
        Dim j As Integer

        ' Loop through the collection of stones. 
        For i = 1 To 9
            For j = 1 To 9
                a(i, j) = -1
                ' Make the border and fill transparent.
                c(i, j).BorderStyle = Drawing2D.DashStyle.Custom
                c(i, j).FillStyle = FillStyle.Transparent
                ' Set Visible to true so that the stones can
                ' respond to the Click event.
                c(i, j).Visible = True
                ' Reset the Click event handlers.
                RemoveHandler c(i, j).Click, AddressOf Oval_Click
                AddHandler c(i, j).Click, AddressOf Oval_Click
            Next
        Next

        ' Reset the turn, count, and turn labels.
        turn = 0
        count = 0
        Me.Label4.Visible = True
        Me.Label3.Visible = False
    End Sub
    Private Sub RectangleShape3_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles RectangleShape3.Paint
        ' Call a method to redraw the fake button.
        DrawButton()
    End Sub
End Class