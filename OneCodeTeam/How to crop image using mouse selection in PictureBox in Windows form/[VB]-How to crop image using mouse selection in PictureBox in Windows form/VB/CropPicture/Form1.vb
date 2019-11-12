Imports System.Drawing.Drawing2D

Public Class Form1
    Dim img As Image
    Dim mouseClicked As Boolean
    Dim startPoint As New Point()
    Dim endPoint As New Point()
    Dim rectCropArea As Rectangle

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Overloads Sub OnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loadPrimaryImage()
    End Sub

    Private Sub loadPrimaryImage()
        img = Image.FromFile("..\..\images.jpg")
        PictureBox1.Image = img
    End Sub

    Private Sub PicBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        mouseClicked = False
        If (endPoint.X <> -1) Then
            Dim currentPoint As New Point(e.X, e.Y)
            Y1.Text = e.X.ToString()
            Y2.Text = e.Y.ToString()
        End If

        endPoint.X = -1
        endPoint.Y = -1
        startPoint.X = -1
        startPoint.Y = -1
    End Sub

    Private Sub PicBox_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        mouseClicked = True
        startPoint.X = e.X
        startPoint.Y = e.Y
        'Display coordinates
        X1.Text = startPoint.X.ToString()
        Y1.Text = startPoint.Y.ToString()

        endPoint.X = -1
        endPoint.Y = -1

        rectCropArea = New Rectangle(New Point(e.X, e.Y), New Size())
    End Sub

    Private Sub PicBox_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        Dim ptCurrent As New Point(e.X, e.Y)

        If (mouseClicked) Then

            If (endPoint.X <> -1) Then
                'Display Coordinates
                X1.Text = startPoint.X.ToString()
                Y1.Text = startPoint.Y.ToString()
                X2.Text = e.X.ToString()
                Y2.Text = e.Y.ToString()
            End If

            endPoint = ptCurrent

            If (e.X > startPoint.X And e.Y > startPoint.Y) Then
                rectCropArea.Width = e.X - startPoint.X
                rectCropArea.Height = e.Y - startPoint.Y


            ElseIf (e.X < startPoint.X And e.Y > startPoint.Y) Then
                rectCropArea.Width = startPoint.X - e.X
                rectCropArea.Height = e.Y - startPoint.Y
                rectCropArea.X = e.X
                rectCropArea.Y = startPoint.Y

            ElseIf (e.X > startPoint.X And e.Y < startPoint.Y) Then
                rectCropArea.Width = e.X - startPoint.X
                rectCropArea.Height = startPoint.Y - e.Y
                rectCropArea.X = startPoint.X
                rectCropArea.Y = e.Y

            Else
                rectCropArea.Width = startPoint.X - e.X
                rectCropArea.Height = startPoint.Y - e.Y
                rectCropArea.X = e.X
                rectCropArea.Y = e.Y
            End If

            PictureBox1.Refresh()

        End If

    End Sub

    Private Sub PicBox_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint
        Dim drawLine As New Pen(Color.Red)
        drawLine.DashStyle = DashStyle.Dash
        e.Graphics.DrawRectangle(drawLine, rectCropArea)
    End Sub

    Private Sub btnCrop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        PictureBox2.Refresh()

        Dim sourceBitmap As New Bitmap(PictureBox1.Image, PictureBox1.Width, PictureBox1.Height)
        Dim g As Graphics = PictureBox2.CreateGraphics()

        If Not (CheckBox1.Checked) Then
            g.DrawImage(sourceBitmap, New Rectangle(0, 0, PictureBox2.Width, PictureBox2.Height), rectCropArea, GraphicsUnit.Pixel)
            sourceBitmap.Dispose()

        Else
            Dim x1, x2, y1, y2 As Integer
            Try
                x1 = Convert.ToInt32(CX1.Text)
                x2 = Convert.ToInt32(CX2.Text)
                y1 = Convert.ToInt32(CY1.Text)
                y2 = Convert.ToInt32(CY2.Text)
            Catch ex As Exception
                MessageBox.Show("Enter valid Coordinates (only Integer values)")
            End Try

            If ((x1 < x2 And y1 < y2)) Then
                rectCropArea = New Rectangle(x1, y1, x2 - x1, y2 - y1)
            ElseIf (x2 < x1 And y2 > y1) Then
                rectCropArea = New Rectangle(x2, y1, x1 - x2, y2 - y1)
            ElseIf (x2 > x1 And y2 < y1) Then
                rectCropArea = New Rectangle(x1, y2, x2 - x1, y1 - y2)
            Else
                rectCropArea = New Rectangle(x2, y2, x1 - x2, y1 - y2)
            End If

            PictureBox1.Refresh() 'This repositions the dashed box to new location as per coordinates entered.

            g.DrawImage(sourceBitmap, New Rectangle(0, 0, PictureBox2.Width, PictureBox2.Height), rectCropArea, GraphicsUnit.Pixel)
            sourceBitmap.Dispose()
        End If
    End Sub

    Private Sub pictureBox1_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseClick
        PictureBox1.Refresh()
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If (CheckBox1.Checked) Then
            CX1.Visible = True
            Label10.Visible = True
            CY1.Visible = True
            Label9.Visible = True
            CX2.Visible = True
            Label8.Visible = True
            CY2.Visible = True
            Label7.Visible = True

            X1.Text = "0"
            X2.Text = "0"
            Y1.Text = "0"
            Y2.Text = "0"

        Else
            CX1.Visible = False
            Label10.Visible = False
            CY1.Visible = False
            Label9.Visible = False
            CX2.Visible = False
            Label8.Visible = False
            CY2.Visible = False
            Label7.Visible = False
        End If

    End Sub
End Class
