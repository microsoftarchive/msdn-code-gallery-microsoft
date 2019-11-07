Imports Microsoft.VisualBasic.PowerPacks

' This sample application demonstrates the use of Line and Shape controls to create a 
' simple user inteface for a math quiz game. The concept and user interface were designed
' by the 8 year-old daughter of one of our developers, and the code written with a little
' help from her father.
Public Class JCWanForm1
    Dim score As Integer = 0
    Dim number1 As Integer = 0
    Dim number2 As Integer = 0
    Dim rnd As New Random(Now.Second)
    Dim currentQShape As OvalShape



    Private Sub SetupQuestion()
        ' This method creates random math questions when the user clicks a shape.
        ' Create two random numbers.
        number1 = rnd.Next(10, 99)
        number2 = rnd.Next(5, 12)
        ' Present the math question via the two labels.
        Num1Label.Text = number1.ToString("00")
        Num2Label.Text = "X " + number2.ToString("00")
        ' Clear the previous answer from the text box.
        Me.AnswerTextBox.Text = ""
        ' Enable the button.
        Me.AnswerButton.Enabled = True
    End Sub



    Private Sub Q1Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q1Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q1Shape
    End Sub

    Private Sub Q2Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q2Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q2Shape
    End Sub

    Private Sub Q3Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q3Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q3Shape
    End Sub

    Private Sub Q4Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q4Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q4Shape
    End Sub

    Private Sub Q5Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q5Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q5Shape
    End Sub

    Private Sub Q6Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q6Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q6Shape
    End Sub

    Private Sub Q7Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q7Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q7Shape
    End Sub

    Private Sub Q8Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q8Shape.Click
        ' Bonus shape - no math question displayed.
        ' Fill in the shape.
        Q8Shape.FillColor = Color.Pink
        Q8Shape.FillStyle = FillStyle.Solid
        Q8Shape.Refresh()
        ' Display a message to the user.
        MessageBox.Show("Congratulations! You got a free Easter Egg!")
        ' Call a method to update the score.
        CorrectAnswer()
    End Sub

    Private Sub Q9Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q9Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q9Shape
    End Sub

    Private Sub Q10Shape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Q10Shape.Click
        ' Call the method that sets up a math question.
        SetupQuestion()
        currentQShape = Q10Shape
    End Sub

    Private Sub AnswerButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnswerButton.Click
        Dim ans As Integer = 0
        ' Fill in the shape that was clicked.
        currentQShape.FillStyle = FillStyle.Solid
        ' Retrieve the answer from the textbox.
        Integer.TryParse(AnswerTextBox.Text, ans)
        If (ans = number1 * number2) Then
            ' If the answer is correct, disable the shape.
            currentQShape.Enabled = False
            ' and call a method to update the score.
            CorrectAnswer()
        Else
            ' If incorrect, hide the shape.
            currentQShape.Visible = False
        End If
        currentQShape.Refresh()
    End Sub
    Private Sub CorrectAnswer()
        ' Add 1 to the score.
        score += 1
        ' Update the score label.
        ScoreLabel.Text = score.ToString()
        If (score >= 4) Then
            ' If 4 or more questions are correct, display the form.
            JCWanForm2.ShowDialog()
        End If

    End Sub


    Private Sub JCWanForm1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Disable the answer button until the user has selected a question.
        AnswerButton.Enabled = False
    End Sub
End Class
