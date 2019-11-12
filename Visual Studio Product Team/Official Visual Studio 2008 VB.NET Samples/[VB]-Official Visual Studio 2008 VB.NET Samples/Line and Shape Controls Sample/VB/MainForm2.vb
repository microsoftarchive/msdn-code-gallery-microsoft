Public Class MainForm2
    ' This form is used to launch the four Line and Shape sample applications.
    Private Sub RectangleShape1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RectangleShape1.Click
        ' Launch the Five Stones application.
        Dim f1 As New FiveStoneForm
        f1.ShowDialog()
    End Sub

    Private Sub RectangleShape2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RectangleShape2.Click
        ' Launch the Darts application.
        Dim f1 As New DartForm
        f1.ShowDialog()
    End Sub

    Private Sub RectangleShape3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RectangleShape3.Click
        ' Launch the Picture application.
        Dim f1 As New PicForm
        f1.ShowDialog()
    End Sub
    Private Sub RectangleShape4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RectangleShape4.Click
        ' Launch the Math application.
        Dim f1 As New JCWanForm1
        f1.ShowDialog()
    End Sub

    Private Sub RectangleShape1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles RectangleShape1.MouseEnter
        ' Display a message in the status bar when the mouse is over the shape.
        ToolStripStatusLabel1.Text = "Click here to play the 5 Stones game."
    End Sub

    Private Sub RectangleShape2_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles RectangleShape2.MouseEnter
        ' Display a message in the status bar when the mouse is over the shape.
        ToolStripStatusLabel1.Text = "Click here to play a game of Darts."
    End Sub

    Private Sub RectangleShape3_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles RectangleShape3.MouseEnter
        ' Display a message in the status bar when the mouse is over the shape.
        ToolStripStatusLabel1.Text = "Click here to see a picture drawn with Line and Shape controls."
    End Sub
    Private Sub RectangleShape4_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles RectangleShape4.MouseEnter
        ' Display a message in the status bar when the mouse is over the shape.
        ToolStripStatusLabel1.Text = "Click here to play a math game."
    End Sub


    Private Sub RectangleShape_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles RectangleShape1.MouseLeave, RectangleShape2.MouseLeave, RectangleShape3.MouseLeave, RectangleShape4.MouseLeave
        ' Clear the status bar message.
        ToolStripStatusLabel1.Text = ""
    End Sub


    Private Sub MainForm2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub


End Class