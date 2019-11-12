Imports Microsoft.VisualBasic.PowerPacks

' This application demonstrates a simple game utilizing Line and Shape controls.
Public Class DartForm

    Private state As Integer
    Private score As Long
    Private s(6) As Integer
    Private v(3) As Integer
    Private t As Integer
    Private y As Integer = 0


    Private Sub Add_Score(ByVal x As Integer, ByVal y As Integer)

        Me.RectangleShape1.SendToBack()

        Dim pt As New Point(x, y)
        Dim shape As Shape
        ' Call GetChildAtPoint to determine which shape the dart is over.
        shape = Me.ShapeContainer1.GetChildAtPoint(pt)

        ' If not over a shape or if the method returns the dart's
        ' shape then do nothing.
        If shape Is Nothing Or shape.Name = "RectangleShape1" Then
            Return
        End If

        ' Add the value of the shape's Tag to the score.
        score += s(shape.Tag + 1)

        ' Display the updated score.
        Me.Label2.Text = score.ToString

        ' Bring the shape that contains the dart image to the foreground.
        Me.RectangleShape1.BringToFront()

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ' Toggle the state of the button.
        state = (state + 1) Mod 3
        If state = 0 Then
            ' The dart has been fired; update the score.
            Me.Add_Score(Me.RectangleShape1.Left, Me.RectangleShape1.Top)
            ' Disable the timers and change the button text.
            Me.Timer1.Enabled = False
            Me.Timer2.Enabled = False
            Me.Button1.Text = "Launch"
        ElseIf state = 1 Then
            ' The dart has been launched and moves to the left.
            Me.RectangleShape1.Left = 320
            Me.RectangleShape1.Top = 305
            ' Start the timers.
            Me.Timer1.Enabled = True
            Me.Timer2.Enabled = False
            ' Update the button text.
            Me.Button1.Text = "Up"
            ' Bring the shape that contains the dart image to the foreground.
            Me.RectangleShape1.BringToFront()
        Else
            ' The dart has changed direction and is moving up.
            ' Disable the first timer.
            Me.Timer1.Enabled = False
            Me.Timer2.Enabled = True
            ' Update the button text.
            Me.Button1.Text = "Fire"
            ' Bring the shape that contains the dart image to the foreground.
            Me.RectangleShape1.BringToFront()
        End If
    End Sub

    

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If Me.RectangleShape1.Left < 5 Then
            ' If the dart is less than 5 pixels from the left
            ' edge of the form, stop the timer.
            Me.Timer1.Enabled = False
            Return
        End If
        ' Move the dart 5 pixels to the left.
        Me.RectangleShape1.Left -= 5
        

    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Initialize the score.
        score = 0
        ' Set up the scoring values.
        s(1) = 100
        s(2) = 80
        s(3) = 60
        s(4) = 40
        s(5) = 20
        s(6) = 10
        ' Set up the speed variables.
        v(1) = 50
        v(2) = 20
        v(3) = 10

        ' Set the difficulty level.
        Me.ComboBox1.Text = "Easy"
        y = 1

        ' Set up the shape that contains the dart.
        Dim g As New System.Drawing.Bitmap(RectangleShape1.BackgroundImage)
        ' Make the white areas in the dart bitmap transparent.
        g.MakeTransparent(System.Drawing.Color.White)
        ' Assign the background image.
        RectangleShape1.BackgroundImage = g
        ' Bring the shape that contains the dart image to the foreground.
        Me.RectangleShape1.BringToFront()
        
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        If Me.RectangleShape1.Top < 5 Then
            ' If the dart is less than 5 pixels from the top
            ' edge of the form, stop the timer.
            Me.Timer2.Enabled = False
            Return
        End If
        ' Move the dart up 5 pixels.
        Me.RectangleShape1.Top -= 5

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        ' Clear the score.
        score = 0
        Me.Label2.Text = score.ToString
        ' Set the focus back to the launch button.
        If Button1.Focused = False Then
            Button1.Focus()
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        ' If not loading the form, display a warning message.
        If (y <> 0) Then
            MessageBox.Show("Your score will be cleared!")
        End If

        ' Change the timer intervals based on the dificulty level.
        t = v(Me.ComboBox1.SelectedIndex + 1)
        Me.Timer1.Interval = t
        Me.Timer2.Interval = t

        ' Clear the score.
        score = 0
        Me.Label2.Text = score.ToString
        ' Set the focus back to the launch button.
        If Button1.Focused = False Then
            Button1.Focus()
        End If
    End Sub

    
End Class