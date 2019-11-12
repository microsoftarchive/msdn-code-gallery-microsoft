' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing.Drawing2D

Public Class MainForm
    Inherits System.Windows.Forms.Form


    ''' <summary>
    ''' The necessary class variables are declared here.
    ''' </summary>
    ''' <remarks></remarks>
    Dim demoPen As New Pen(Color.Black)
    Dim penColor As Color = Color.BurlyWood


    ''' <summary>
    ''' This is used to turn on and off the timer that handles
    ''' the cycling of the dots and dashes.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCycle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCycle.Click
        timerCycle.Interval = 333
        timerCycle.Enabled = Not timerCycle.Enabled
        If timerCycle.Enabled Then
            btnCycle.Text = "Stop!"
        Else
            btnCycle.Text = "Animate"
        End If
    End Sub


    ''' <summary>
    ''' This Sub sets the Color based on the user selection from
    ''' a ColorDialog.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnSetColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetColor.Click
        Dim cdlg As New ColorDialog()

        If cdlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            penColor = cdlg.Color
            txtColor.Text = cdlg.Color.ToString()
            txtColor.BackColor = cdlg.Color
        End If
    End Sub


    ''' <summary>
    ''' When the Form1 is loaded, this Sub sets up defaults for the page
    ''' and builds the necessary combo boxes. Notice that the combo
    ''' boxes here hold various objects, instead of standard strings. This
    ''' way they can be used directly when assigning their values to 
    ''' other objects.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set up the StartCap and EndCap options.
        For Each cap As LineCap In System.Enum.GetValues(GetType(LineCap))
            comboStartCap.Items.Add(cap)
            comboEndCap.Items.Add(cap)
        Next

        ' Set up Dash Cap.
        For Each dash As DashCap In System.Enum.GetValues(GetType(DashCap))
            comboDashCap.Items.Add(dash)
        Next

        ' Set up Line Join
        For Each join As LineJoin In System.Enum.GetValues(GetType(LineJoin))
            comboLineJoin.Items.Add(join)
        Next

        ' Set up Line Join
        For Each style As DashStyle In System.Enum.GetValues(GetType(DashStyle))
            comboLineStyle.Items.Add(style)
        Next

        ' Set up Alignment
        For Each align As PenAlignment In System.Enum.GetValues(GetType(PenAlignment))
            comboAlignment.Items.Add(align)
        Next
    End Sub

    ''' <summary>
    ''' RadioCheckBox code
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub radioColor_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioColor.CheckedChanged
        txtColor.Enabled = radioColor.Checked
        btnSetColor.Enabled = radioColor.Checked
        comboBrush.Enabled = radioBrush.Checked
    End Sub

    ''' <summary>
    ''' RedrawPicture collects all the user defined information, and uses it
    ''' to create a Pen object, which is then used to draw one of three different
    ''' drawings. Notice that this Sub handles virtually all of events triggered
    ''' by the user interface.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RedrawPicture(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles MyBase.Activated, comboTransform.SelectedIndexChanged, updownMiterLimit.ValueChanged, comboLineStyle.SelectedIndexChanged, comboLineJoin.SelectedIndexChanged, comboEndCap.SelectedIndexChanged, comboStartCap.SelectedIndexChanged, updownWidth.ValueChanged, comboShape.SelectedIndexChanged, txtColor.TextChanged, comboAlignment.SelectedIndexChanged, comboDashCap.SelectedIndexChanged, comboBrush.SelectedIndexChanged

        Dim penBrush As Brush = New SolidBrush(Color.Black)
        Dim blackThinPen As New Pen(Color.Black)

        ' Reset the PictureBox
        pbLines.CreateGraphics().Clear(pbLines.BackColor)
        pbLines.Refresh()
        ' Get rid of any current transform on the Pen
        demoPen.ResetTransform()
        ' Set the DashPattern to use if Custom type of Dashed Line is selected
        demoPen.DashPattern = New Single() {0.5, 0.25, 0.75, 1.5}

        ' Since a Pen can have either a Color or a Brush assigned, but not both,
        ' this code determines which should be used. Note that assigning a Color
        ' is identical to assigning the pen a SolidBrush.
        If radioColor.Checked Then
            demoPen.Color = penColor
        Else
            Select Case comboBrush.Text
                Case "Solid"
                    ' The same as assigning the Pen a Color
                    penBrush = New SolidBrush(penColor)
                Case "Hatch"
                    ' Defines a HatchBrush to use, in this case a Plaid one.
                    penBrush = New HatchBrush(HatchStyle.Plaid, penColor)
                Case "Texture"
                    ' Assigns a bitmap image to be used as the Brush.
                    penBrush = New TextureBrush( _
                        My.Resources.WaterLilies, WrapMode.Tile)
                Case "Gradient"
                    ' Builds a LinearGradientBrush to use as the Brush. Other types
                    '   of gradient brushes could be used here as well.
                    penBrush = New LinearGradientBrush( _
                        New Point(0, 0), _
                        New Point(pbLines.Width, pbLines.Height), _
                        Color.AliceBlue, Color.DarkBlue)
            End Select

            demoPen.Brush = penBrush
        End If

        ' Set the user defined values for all the pen objects
        ' Width of the Pen is in pixels 
        demoPen.Width = updownWidth.Value
        ' DashStyle determines the look of the line.
        ' It can be dashes, dots and dashes, or even custom
        demoPen.DashStyle = CType(comboLineStyle.SelectedItem, DashStyle)
        ' MiterLimit is a float that determines when the Miter edge of
        ' two adjacent lines should be clipped. The default is 10.0
        demoPen.MiterLimit = updownMiterLimit.Value
        ' StartCap determines the cap that should be put on
        ' the start of a line drawn by the pen
        demoPen.StartCap = CType(comboStartCap.SelectedItem, LineCap)
        ' EndCap determines the cap that should be put on
        ' the end of a line drawn by the pen
        demoPen.EndCap = CType(comboEndCap.SelectedItem, LineCap)
        ' DashCap determines the cap that should be put on
        ' both ends of any dashes in a line drawn by the pen
        demoPen.DashCap = CType(comboDashCap.SelectedItem, DashCap)
        ' LineJoin determines how two adjacent lines should be joined.
        ' For instance, they can have a rounded join, or a mitered join.
        demoPen.LineJoin = CType(comboLineJoin.SelectedItem, LineJoin)
        ' Alignment determines which 'side' of the designated line, that
        ' the pen should draw on. For instance, Inset will cause the
        ' pen to draw on the inside of a circle.
        demoPen.Alignment = CType(comboAlignment.SelectedItem, PenAlignment)

        ' Transforms are used for some advanced features of pens. You can,
        ' for instance, create a caligraphic style pen.
        Select Case comboTransform.Text
            Case "None"
                ' ResetTransform resets the pen to having no transform
                demoPen.ResetTransform()
            Case "Scale"
                ' ScaleTransform  changes the appearance of the pen, by
                '   by changing the width and height of the pen. For instance,
                '   the transform below makes the width half as thin as the
                '   normal, and doubles the height.
                demoPen.ScaleTransform(0.5, 2)
            Case "Rotate"
                ' RotateTransform only is used if the underlying 
                '   Brush supports it. It rotates the brush by a number of 
                '   degrees.
                demoPen.RotateTransform(45)
            Case "Translate"
                ' TranslateTransform only is used if the underlying 
                '   Brush supports it. It translates the underlying brush.
                demoPen.TranslateTransform(2, 4)
        End Select



        ' Now that the Pen has been defined create a graphics object, and
        '   redraw the image on the PictureBox. Also draw a thin black line
        '   using the same command. This will allow the user to see where
        '   the line was intended to go, and aids in illuminating what
        '   various properties do.
        Dim graphic As Graphics = pbLines.CreateGraphics()

        If Me.comboShape.Text = "Lines" Then
            ' Draw 3 simple lines. 
            ' Draw using the user defined pen
            graphic.DrawLine(demoPen, 35, 35, pbLines.Width - 35, 35)
            graphic.DrawLine(demoPen, 35, 80, 35, pbLines.Height - 35)
            graphic.DrawLine(demoPen, 90, 90, pbLines.Width - 35, _
                pbLines.Height - 35)

            ' Draw using the thin black pen, to see effects
            graphic.DrawLine(blackThinPen, 35, 35, pbLines.Width - 35, 35)
            graphic.DrawLine(blackThinPen, 35, 80, 35, pbLines.Height - 35)
            graphic.DrawLine(blackThinPen, 90, 90, pbLines.Width - 35, _
                    pbLines.Height - 35)

        ElseIf Me.comboShape.Text = "Intersecting Lines" Then
            ' Draw a compound line

            ' Create a more complex shape using an array of Points
            ' To define a multisegment line. If several independent
            ' lines are used instead, even if they connect, then the 
            ' end and start caps would be placed on each independent line.
            ' Here they are only placed on the beginning and end of the 
            ' compound line.
            Dim ptArray(5) As PointF
            ptArray(0) = New PointF(35, 35)
            ptArray(1) = New PointF(70, pbLines.Height - 75)
            ptArray(2) = New PointF(100, 35)
            ptArray(3) = New PointF(pbLines.Width - 40, pbLines.Height \ 2)
            ptArray(4) = New PointF(pbLines.Width \ 2, pbLines.Height \ 2)
            ptArray(5) = New PointF(pbLines.Width - 25, 25)

            ' Draw the lines using the user defined Pen
            graphic.DrawLines(demoPen, ptArray)

            ' Draw the lines using the thin, black Pen
            graphic.DrawLines(blackThinPen, ptArray)

        ElseIf Me.comboShape.Text = "Circles and Curves" Then
            ' Draw a circle and a curve

            ' Draw the curves using the user defined Pen
            graphic.DrawEllipse(demoPen, 25, 25, 200, 200)
            graphic.DrawArc(demoPen, 55, 55, pbLines.Height - 55, _
                pbLines.Height - 55, 110, 150)

            ' Draw the curves using the thin, black Pen
            graphic.DrawEllipse(blackThinPen, 25, 25, 200, 200)
            graphic.DrawArc(blackThinPen, 55, 55, pbLines.Height - 55, _
                pbLines.Height - 55, 110, 150)

        End If

    End Sub

    ''' <summary>
    ''' When the timer fires, the DashOffset property is incremented and the dots
    ''' and dashes in a line apprear to animate.
    ''' </summary>
    Private Sub timerCycle_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerCycle.Tick
        demoPen.DashOffset = (demoPen.DashOffset + 0.5F) Mod 30
        RedrawPicture(Me, New System.EventArgs())
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
