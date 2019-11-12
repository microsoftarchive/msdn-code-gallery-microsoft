' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing.Drawing2D

Public Class MainForm
    Inherits System.Windows.Forms.Form

#Region "Block Text"
    ' This Sub creates the Sample Text with an Embossed look.
    ' To create effect, the Sample Text is drawn twice.  The first
    ' time it is in Black and offset, then drawn in aquamarine.
    ' This gives the impression of the text being raised.
    ' To give the imporession of being engraved, simply use the
    ' negative of the Offset.
    Private Sub DrawBlockText()
        Dim textSize As SizeF
        Dim g As Graphics
        Dim myBackBrush As Brush = Brushes.Black
        Dim myForeBrush As Brush = Brushes.Aquamarine
        Dim myFont As New Font("Times New Roman", Me.nudFontSize.Value, FontStyle.Regular)
        Dim xLocation, yLocation As Single ' Used for the location
        Dim i As Integer

        ' Create a Graphics object from the picture box & clear it
        g = picDemoArea.CreateGraphics()
        g.Clear(Color.White)

        ' Find the Size required to draw the Sample Text
        textSize = g.MeasureString(Me.txtShortText.Text, myFont)

        ' Get the locations once to eliminate redundant calculations
        xLocation = (picDemoArea.Width - textSize.Width) / 2
        yLocation = (picDemoArea.Height - textSize.Height) / 2

        ' Draw the Black background first
        '   To get the effect, the text must be drawn repeatedly
        '   from the offset right up to where the main text
        '   will be drawn.
        ' Since people tend to think of light coming from the 
        '   upper right, it often makes more sense to subtract
        '   the offset depth from the X dimension, instead of 
        '   adding it. Otherwise, it looks more like a shadow.
        For i = CInt(effectDepth.Value) To 0 Step -1
            g.DrawString(txtShortText.Text, myFont, myBackBrush, _
                    xLocation - i, yLocation + i)
        Next

        ' Draw the aquamarine main text over the black text
        g.DrawString(txtShortText.Text, myFont, myForeBrush, xLocation, yLocation)
    End Sub
#End Region

#Region "Brush Text"
    ' This Sub creates the Sample Text with a brush (Hatch or Gradient).
    Private Sub DrawBrushText()
        Dim textSize As SizeF
        Dim g As Graphics
        Dim myBrush As Brush
        Dim gradientRectangle As RectangleF
        Dim myFont As New Font("Times New Roman", Me.nudFontSize.Value, FontStyle.Regular)

        ' Create a Graphics object from the picture box & clear it.
        g = picDemoArea.CreateGraphics()
        g.Clear(Color.White)

        ' Find the Size required to draw the Sample Text.
        textSize = g.MeasureString(Me.txtShortText.Text, myFont)

        ' Create the required brush.
        If Me.optHatch.Checked Then
            ' Create a Diagonal Brick HatchBrush.
            myBrush = New HatchBrush(HatchStyle.DiagonalBrick, _
                Color.Blue, Color.Yellow)
        Else
            ' Create a Diagonal Gradient LinearGradientBrush. 
            gradientRectangle = New RectangleF(New PointF(0, 0), textSize)
            myBrush = New LinearGradientBrush(gradientRectangle, Color.Blue, _
                Color.Yellow, LinearGradientMode.ForwardDiagonal)
        End If

        ' Draw the text.
        g.DrawString(txtShortText.Text, myFont, myBrush, _
                (picDemoArea.Width - textSize.Width) / 2, _
                (picDemoArea.Height - textSize.Height) / 2)
    End Sub
#End Region

#Region "Emboss Text"
    ' This Sub creates the Sample Text with an Embossed look.
    ' To create effect, the Sample Text is drawn twice.  The first
    ' time it is in Black and offset, then drawn in white, the 
    ' current background color.
    ' This gives the impression of the text being raised.
    ' To give the imporession of being engraved, simply use the
    ' negative of the Offset.
    Private Sub DrawEmbossedText()
        Dim textSize As SizeF
        Dim g As Graphics
        Dim myBackBrush As Brush = Brushes.Black
        Dim myForeBrush As Brush = Brushes.White
        Dim myFont As New Font("Times New Roman", Me.nudFontSize.Value, FontStyle.Regular)
        Dim xLocation, yLocation As Single

        ' Create a Graphics object from the picture box & clear it.
        g = picDemoArea.CreateGraphics()
        g.Clear(Color.White)

        ' Find the Size required to draw the Sample Text.
        textSize = g.MeasureString(Me.txtShortText.Text, myFont)

        ' Get the locations once to eliminate redundant calculations.
        xLocation = (picDemoArea.Width - textSize.Width) / 2
        yLocation = (picDemoArea.Height - textSize.Height) / 2

        ' Draw the Black background first.
        ' (Note: if you instead subtract the mudEmbossDepth, you will get
        ' an Engrave effect.)
        g.DrawString(txtShortText.Text, myFont, myBackBrush, _
                xLocation + Me.effectDepth.Value, _
                yLocation + Me.effectDepth.Value)

        ' Draw the white main text over the black text
        g.DrawString(txtShortText.Text, myFont, myForeBrush, xLocation, yLocation)
    End Sub
#End Region

#Region "Reflect Text"
    ' This sub reflects text around the baseline of the characters.
    '   This is the first example that requires careful measurement of 
    '   the text, and is more advanced than most of the other examples.
    Private Sub DrawReflectText()
        Dim textSize As SizeF
        Dim g As Graphics
        Dim myBackBrush As Brush = Brushes.Gray
        Dim myForeBrush As Brush = Brushes.Black
        Dim myFont As New Font("Times New Roman", Me.nudFontSize.Value, FontStyle.Regular)
        Dim myState As GraphicsState ' Used to store current state of Graphics
        Dim xLocation, yLocation As Single ' Used for the location
        Dim textHeight As Single

        ' Create a Graphics object from the picture box & clear it
        g = picDemoArea.CreateGraphics()
        g.Clear(Color.White)

        ' Find the Size required to draw the Sample Text
        textSize = g.MeasureString(Me.txtShortText.Text, myFont)

        ' Get the locations once to eliminate redundant calculations
        xLocation = (picDemoArea.Width - textSize.Width) / 2
        yLocation = (picDemoArea.Height - textSize.Height) / 2

        ' Because we will be scaling, and scaling effects the ENTIRE
        '   graphics object, not just the text, we need to reposition
        '   the Origin of the Graphics object (0,0) to the (xLocation,
        '   yLocation) point. If we don't, when we attempt to flip 
        '   the text with a scaling transform, it will merely draw the
        '   reflected text at (xLocation, -yLocation) which is outside
        '   the viewable area.
        g.TranslateTransform(xLocation, yLocation)

        ' Reflecting around the Origin still poses problems. The
        '   origin represents the upper left corner of the rectangle.
        '   This means the reflection will occur at the TOP of the 
        '   original drawing. This is not how people are used to 
        '   seing reflected text. Thus, we need to determine where to
        '   draw the text. This can be done only when we have calculated
        '   the height required by the Drawing.
        ' This is not as simple as it may seem. The Height returned 
        '   from the MeasureString method includes some extra spacing 
        '   for descenders and whitespace. We want ONLY the height from
        '   the BASELINE (which is the line which all caps sit). Any
        '   characters with descenders drop below the baseline. To 
        '   calculate the height above the baseline, use the 
        '   GetCellAscent method. Since GetCellAscent returns a Design
        '   Metric value it must be converted to pixels, and scaled for
        '   the font size.
        ' Note: this looks best with characters that can be reflected
        '   over the baseline nicely -- like caps. Characters with descenders
        '   look odd. To fix that uncomment the two lines below, which
        '   then reflect across the lowest descender height.

        Dim lineAscent As Integer
        Dim lineSpacing As Integer
        Dim lineHeight As Single

        lineAscent = myFont.FontFamily.GetCellAscent(myFont.Style)
        lineSpacing = myFont.FontFamily.GetLineSpacing(myFont.Style)
        lineHeight = myFont.GetHeight(g)
        textHeight = lineHeight * lineAscent / lineSpacing

        '' Uncomment these lines to reflect over lowest portion
        ''   of the characters.
        'Dim lineDescent As Integer ' used for reflecting descending characters
        'lineDescent = myFont.FontFamily.GetCellDescent(myFont.Style)
        'textHeight = lineHeight * (lineAscent + lineDescent) / lineSpacing


        ' Draw the reflected one first. The only reason to draw the
        '   Reflected one first is to demonstrate the use of the
        '   GraphicsState object. 
        ' A GraphicsState object maintains the state of the Graphics
        '   object as it currently stands. You can then scale, resize and
        '   otherwise transform the Graphics object. You can 
        '   immediately go back to a previous state using the Restore
        '   method of the Graphics object.
        ' Had we drawn the main one first, we would not have needed the 
        '   Restore method or the GraphicsState object.

        ' First Save the graphics state
        myState = g.Save()

        ' To draw the reflection, use the ScaleTransform with a negative
        '   value. Using -1 will reflect the Text with no distortion.
        ' Remember to account for the fact that the origin has been reset.
        g.ScaleTransform(1, -1.0F) ' Only reflecting in the Y direction
        g.DrawString(txtShortText.Text, myFont, myBackBrush, 0, -textHeight)

        ' Reset the graphics state to before the transform
        g.Restore(myState)

        ' Draw the main text
        g.DrawString(txtShortText.Text, myFont, myForeBrush, 0, -textHeight)

    End Sub
#End Region

#Region "Shadow Text"
    ' This Sub creates the Sample Text with a solid brush and shadow
    '   To create the shadow, the Sample Text is drawn twice.  The first
    '   time it is in Grey and offset, then normally in Black.
    '   Other colors can, of course, be used.
    Private Sub DrawShadowText()
        Dim textSize As SizeF
        Dim g As Graphics
        Dim myShadowBrush As Brush = Brushes.Gray
        Dim myForeBrush As Brush = Brushes.Black
        Dim myFont As New Font("Times New Roman", Me.nudFontSize.Value, FontStyle.Regular)
        Dim xLocation, yLocation As Single

        ' Create a Graphics object from the picture box & clear it.
        g = picDemoArea.CreateGraphics()
        g.Clear(Color.White)

        ' Find the Size required to draw the Sample Text.
        textSize = g.MeasureString(Me.txtShortText.Text, myFont)

        ' Get the locations once to eliminate redundant calculations.
        xLocation = (picDemoArea.Width - textSize.Width) / 2
        yLocation = (picDemoArea.Height - textSize.Height) / 2

        ' Draw the Shadow first.
        g.DrawString(txtShortText.Text, myFont, myShadowBrush, _
                xLocation + Me.effectDepth.Value, _
                yLocation + Me.effectDepth.Value)

        ' Draw the Main text over the shadow.
        g.DrawString(txtShortText.Text, myFont, myForeBrush, xLocation, yLocation)
    End Sub
#End Region

#Region "Shear Text"
    ' This sub shears the text so that it appears angled. This requires
    ' the use of a Matrix which defines the shear. 
    Private Sub DrawShearText()
        Dim textSize As SizeF
        Dim g As Graphics
        Dim myForeBrush As Brush = Brushes.Black
        Dim myFont As New Font("Times New Roman", Me.nudFontSize.Value, FontStyle.Regular)
        Dim myTransform As Matrix
        Dim xLocation, yLocation As Single

        ' Create a Graphics object from the picture box & clear it.
        g = picDemoArea.CreateGraphics()
        g.Clear(Color.White)

        ' Find the Size required to draw the Sample Text.
        textSize = g.MeasureString(Me.txtShortText.Text, myFont)

        ' Get the locations once to eliminate redundant calculations.
        xLocation = (picDemoArea.Width - textSize.Width) / 2
        yLocation = (picDemoArea.Height - textSize.Height) / 2

        ' Because we will be scaling, and scaling effects the ENTIRE
        '   graphics object, not just the text, we need to reposition
        '   the Origin of the Graphics object (0,0) to the (xLocation,
        '   yLocation) point. 
        g.TranslateTransform(xLocation, yLocation)

        ' Get the transform for the current Graphics object, and
        ' Shear it by the specified amount.
        myTransform = g.Transform
        myTransform.Shear(nudSkew.Value, 0)
        g.Transform = myTransform

        ' Draw the main text.
        g.DrawString(txtShortText.Text, myFont, myForeBrush, 0, 0)
    End Sub
#End Region

#Region "Multiline Text"
    ' This sub simply takes the lines of text in the textbox and places them
    ' in the picDemoArea PictureBox.  It will word wrap as necessary, but will not scroll.
    Private Sub DrawMultiLine()
        Dim textSize As SizeF
        Dim g As Graphics
        Dim myForeBrush As Brush = Brushes.Black
        Dim myFont As New Font("Times New Roman", Me.multilineSize.Value, FontStyle.Regular)

        ' Create a Graphics object from the picture box & clear it.
        g = picMultiLine.CreateGraphics()
        g.Clear(Color.White)

        ' Find the Size required to draw the Sample Text.
        textSize = g.MeasureString(txtLongText.Text, myFont)

        ' Draw the main text.
        g.DrawString(txtLongText.Text, myFont, myForeBrush, _
            New RectangleF(0, 0, picDemoArea.Width, picDemoArea.Height))
    End Sub
#End Region


    Private Sub lstEffects_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstEffects.SelectedIndexChanged
        Me.optHatch.Enabled = False
        Me.optGradient.Enabled = False
        Me.effectDepth.Enabled = False
        Me.nudSkew.Enabled = False
        Select Case lstEffects.SelectedItem.ToString()
            Case "Brush"
                Me.optHatch.Enabled = True
                Me.optGradient.Enabled = True
            Case "Shadow", "Embossed", "Block"
                Me.effectDepth.Enabled = True
                Me.effectDepth.Enabled = True
                Me.effectDepth.Enabled = True
            Case "Shear"
                Me.nudSkew.Enabled = True
            Case "Reflect"
                ' Don't need to change anything.
        End Select

        Me.DrawText()
    End Sub

    Private Sub DrawText()
        If lstEffects.SelectedItem Is Nothing Then
            lstEffects.SelectedIndex = 0
        End If

        Select Case lstEffects.SelectedItem.ToString()
            Case "Brush"
                Me.DrawBrushText()
            Case "Shadow"
                Me.DrawShadowText()
            Case "Embossed"
                Me.DrawEmbossedText()
            Case "Block"
                Me.DrawBlockText()
            Case "Shear"
                Me.DrawShearText()
            Case "Reflect"
                Me.DrawReflectText()
        End Select
    End Sub

    Private Sub multiLineSize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles multilineSize.ValueChanged
        DrawMultiLine()
    End Sub


    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        DrawText()
        DrawMultiLine()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lstEffects.SelectedIndex = 0
    End Sub

    Private Sub UIChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles effectDepth.ValueChanged, nudSkew.ValueChanged, txtShortText.Leave, _
        nudFontSize.ValueChanged, multilineSize.ValueChanged, optGradient.CheckedChanged, _
        optHatch.CheckedChanged
        If Me.nudFontSize.Value = 0 Then
            Me.nudFontSize.Value = 50
        End If
        DrawText()
    End Sub

    Private Sub txtLongText_Leave(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtLongText.Leave
        DrawMultiLine()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

End Class
