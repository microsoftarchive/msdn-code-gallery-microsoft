' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Public Class MainForm

    Const WinkTimerInterval As Integer = 150 ' In milliseconds
    Protected eyeImages(4) As Image
    Protected currentImage As Integer = 0
    Protected animationStep As Integer = 1

    Const BallTimerInterval As Integer = 25 ' In milliseconds
    Private ballSize As Integer = 16 ' As fraction of client area
    Private moveSize As Integer = 4 ' As fraction of ball size
    Private bitmap As Bitmap
    Private ballPositionX As Integer
    Private ballPositionY As Integer
    Private ballRadiusX As Integer
    Private ballRadiusY As Integer
    Private ballMoveX As Integer
    Private ballMoveY As Integer
    Private ballBitmapWidth As Integer
    Private ballBitmapHeight As Integer
    Private bitmapWidthMargin As Integer
    Private bitmapHeightMargin As Integer

    Const TextTimerInterval As Integer = 15 ' In milliseconds
    Protected currentGradientShift As Integer = 10
    Protected gradiantStep As Integer = 5

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Fills the image array for the Winking Eye example.
        eyeImages(0) = My.Resources.Eye1
        eyeImages(1) = My.Resources.Eye2
        eyeImages(2) = My.Resources.Eye3
        eyeImages(3) = My.Resources.Eye4
    End Sub

    Private Sub RadioButtons_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optWink.CheckedChanged, optBall.CheckedChanged
        If optWink.Checked Then
            tmrAnimation.Interval = WinkTimerInterval
        ElseIf optBall.Checked Then
            tmrAnimation.Interval = BallTimerInterval
        ElseIf optText.Checked Then
            tmrAnimation.Interval = TextTimerInterval
        End If

        OnResize(EventArgs.Empty)
    End Sub

    Protected Overridable Sub TimerOnTick(ByVal obj As Object, ByVal ea As EventArgs) Handles tmrAnimation.Tick
        If optWink.Checked Then

            ' Obtain the Graphics object exposed by the Form.
            Dim grfx As Graphics = CreateGraphics()

            ' Call DrawImage, using Overload #8, which takes the current image to be
            ' displayed, the X and Y coordinates (which, in this case centers the 
            ' image in the client area), and the image's width and height.
            grfx.DrawImage(eyeImages(currentImage), _
                CInt((ClientSize.Width - eyeImages(currentImage).Width) / 2), _
                CInt((ClientSize.Height - eyeImages(currentImage).Height) / 2), _
                eyeImages(currentImage).Width, _
                eyeImages(currentImage).Height)
            ' It is always a good idea to call Dispose for objects that expose this
            ' method instead of waiting for the Garbage Collector to do it for you.
            ' This almost always increases the application's performance.
            grfx.Dispose()

            ' Loop through the images.
            currentImage += animationStep
            If currentImage = 3 Then
                ' This is the last image of the four, so reverse the animation
                ' order so that the eye closes.
                animationStep = -1
            ElseIf currentImage = 0 Then
                ' This is the first image of the four, so reverse the animation
                ' order so that the eye opens again.
                animationStep = 1
            End If

        ElseIf optBall.Checked Then

            ' Obtain the Graphics object exposed by the Form.
            Dim grfx As Graphics = CreateGraphics()
            ' Draw the bitmap containing the ball on the Form.
            grfx.DrawImage(bitmap, _
                CInt(ballPositionX - ballBitmapWidth / 2), _
                CInt(ballPositionY - ballBitmapHeight / 2), _
                ballBitmapWidth, ballBitmapHeight)

            grfx.Dispose()

            ' Increment the ball position by the distance it has
            ' moved in both X and Y after being redrawn.
            ballPositionX += ballMoveX
            ballPositionY += ballMoveY

            ' Reverse the ball's direction when it hits a boundary.
            If ballPositionX + ballRadiusX >= ClientSize.Width _
                Or ballPositionX - ballRadiusX <= 0 Then
                ballMoveX = -ballMoveX
                Beep()
            End If
            ' Set the Y boundary at 80 instead of 0 so the ball does not bounce
            ' into controls on the Form.
            If ballPositionY + ballRadiusY >= ClientSize.Height _
                Or ballPositionY - ballRadiusY <= 80 Then
                ballMoveY = -ballMoveY
                Beep()
            End If

        ElseIf optText.Checked Then

            ' Obtain the Graphics object exposed by the Form.
            Dim grfx As Graphics = CreateGraphics()

            ' Set the font type, text, and determine its size.
            Dim font As New Font("Microsoft Sans Serif", 96, _
                FontStyle.Bold, GraphicsUnit.Point)
            Dim strText As String = "GDI+!"
            Dim sizfText As New SizeF(grfx.MeasureString(strText, font))

            ' Set the point at which the text will be drawn: centered
            ' in the client area.
            Dim ptfTextStart As New PointF( _
                CSng(ClientSize.Width - sizfText.Width) / 2, _
                CSng(ClientSize.Height - sizfText.Height) / 2)

            ' Set the gradient start and end point, the latter being adjusted
            ' by a changing value to give the animation affect.
            Dim ptfGradientStart As New PointF(0, 0)
            Dim ptfGradientEnd As New PointF(currentGradientShift, 200)

            ' Instantiate the brush used for drawing the text.
            Dim grBrush As New LinearGradientBrush(ptfGradientStart, _
                ptfGradientEnd, Color.Blue, BackColor)

            ' Draw the text centered on the client area.
            grfx.DrawString(strText, font, grBrush, ptfTextStart)

            grfx.Dispose()

            ' Shift the gradient, reversing it when it gets to a certain value.
            currentGradientShift += gradiantStep
            If currentGradientShift = 500 Then
                gradiantStep = -5
            ElseIf currentGradientShift = -50 Then
                gradiantStep = 5
            End If
        End If
    End Sub

    ' This method overrides the OnResize method in the base Control class. OnResize 
    ' raises the Resize event, which occurs when the control (in this case, the 
    ' Form) is resized.
    Protected Overrides Sub OnResize(ByVal ea As EventArgs)
        If optWink.Checked Then

            ' Obtain the Graphics object exposed by the Form and erase any drawings.
            Dim grfx As Graphics = CreateGraphics()
            ' You could also call grfx.Clear(BackColor) or Me.Invalidate() to clear
            ' off the screen.
            Me.Refresh()
            grfx.Dispose()

        ElseIf optBall.Checked Then

            ' Obtain the Graphics object exposed by the Form and erase any drawings.
            Dim grfx As Graphics = CreateGraphics()
            grfx.Clear(BackColor)

            ' Set the radius of the ball to a fraction of the width or height
            ' of the client area, whichever is less.
            Dim dblRadius As Double = Math.Min(ClientSize.Width / grfx.DpiX, _
                ClientSize.Height / grfx.DpiY) / ballSize

            ' Set the width and height of the ball as in most cases the DPI is
            ' identical in the X and Y axes.
            ballRadiusX = CInt(dblRadius * grfx.DpiX)
            ballRadiusY = CInt(dblRadius * grfx.DpiY)

            grfx.Dispose()

            ' Set the distance the ball moves to 1 pixel or a fraction of the
            ' ball's size, whichever is greater. This means that the distance the 
            ' ball moves each time it is drawn is proportional to its size, which 
            ' is, in turn, proportional to the size of the client area. Thus, when 
            ' the client area is shrunk the ball slows down, and when it is 
            ' increased, the ball speeds up. 
            ballMoveX = CInt(Math.Max(1, ballRadiusX / moveSize))
            ballMoveY = CInt(Math.Max(1, ballRadiusY / moveSize))

            ' Notice that the value of the ball's movement also serves as the
            ' margin around the ball, which determines the size of the actual 
            ' bitmap on which the ball is drawn. Thus, the distance the ball moves 
            ' is exactly equal to the size of the bitmap, which permits the previous 
            ' image of the ball to be erased before the next image is drawn, all 
            ' without an inordinate amount of flickering.
            bitmapWidthMargin = ballMoveX
            bitmapHeightMargin = ballMoveY

            ' Determine the actual size of the Bitmap on which the ball is drawn by
            ' adding the margins to the ball's dimensions.
            ballBitmapWidth = 2 * (ballRadiusX + bitmapWidthMargin)
            ballBitmapHeight = 2 * (ballRadiusY + bitmapHeightMargin)

            ' Create a new bitmap, passing in the width and height
            bitmap = New Bitmap(ballBitmapWidth, ballBitmapHeight)

            ' Obtain the Graphics object exposed by the Bitmap, clear the existing 
            ' ball, and draw the new ball.
            grfx = Graphics.FromImage(bitmap)
            With grfx
                .Clear(BackColor)
                .FillEllipse(Brushes.Red, New Rectangle(ballMoveX, _
                    ballMoveY, 2 * ballRadiusX, 2 * ballRadiusY))
                .Dispose()
            End With

            ' Reset the ball's position to the center of the client area.
            ballPositionX = CInt(ClientSize.Width / 2)
            ballPositionY = CInt(ClientSize.Height / 2)

        ElseIf optText.Checked Then
            ' Obtain the Graphics object exposed by the Form and erase any drawings.
            Dim grfx As Graphics = CreateGraphics()
            grfx.Clear(BackColor)
        End If
    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class