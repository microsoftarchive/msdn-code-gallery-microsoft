' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing.Drawing2D

Public Class MainForm

    ''' <summary>
    ''' All the class variables are defined.
    ''' Since Brush is a MustInherit (abstract) class, this brush will
    ''' actually be holding instances of the other 5 types of brushes.
    ''' </summary>
    ''' <remarks></remarks>
    Dim demoBrush As Brush              ' Demonstration brush    
    Dim color1 As Color = Color.Blue    ' Mainly acts as foreground color
    Dim color2 As Color = Color.White   ' Mainly acts as background color
    Dim demoPen As Pen                  ' Pen to use when drawing lines
    Dim brushSize As Rectangle          ' Rectangle used when tiling brushes
    Dim formGraphics As Graphics        ' Graphics object used to draw brushes

    ''' <summary>
    ''' This subroutine is used to set the color1 value.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnSetColor1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetColor1.Click
        Dim cdlg As New ColorDialog()

        If cdlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            color1 = cdlg.Color
            txtColor1.Text = cdlg.Color.ToString()
            txtColor1.BackColor = cdlg.Color
        End If
    End Sub

    ''' <summary>
    ''' This subroutine is used to set the color2 value.
    ''' </summary>
    Private Sub btnSetColor2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetColor2.Click
        Dim cdlg As New ColorDialog()

        If cdlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            color2 = cdlg.Color
            txtColor2.Text = cdlg.Color.ToString()
            txtColor2.BackColor = cdlg.Color
        End If

    End Sub


    ''' <summary>
    ''' This subrouting changes the size of the brush used to draw in 
    ''' the PictureBox by defining a new rectangle. These rectangles could be
    ''' defined.
    ''' </summary>
    Private Sub cboBrushSize_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboBrushSize.SelectedIndexChanged
        Select Case cboBrushSize.Text
            Case "Large"
                ' "Large" takes up all of picDemoArea
                brushSize = New Rectangle(0, 0, _
                    picDemoArea.Width, picDemoArea.Height)
            Case "Medium"
                ' "Medium" breaks up picDemoArea into 4 pieces
                brushSize = New Rectangle(0, 0, _
                    picDemoArea.Width \ 2, picDemoArea.Height \ 2)
            Case "Small"
                ' "Small" breaks up picDemoArea into 16 pieces
                brushSize = New Rectangle(0, 0, _
                    picDemoArea.Width \ 4, picDemoArea.Height \ 4)
        End Select

        ' Call RedrawPicture
        RedrawPicture(cboBrushSize, New EventArgs())
    End Sub


    ''' <summary>
    ''' This draws the picture with the default settings when the form loads.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated, MyBase.Activated
        brushSize = New Rectangle(0, 0, _
            picDemoArea.Width, picDemoArea.Height)
        ' Call RedrawPicture
        RedrawPicture(cboBrushSize, New EventArgs())
    End Sub


    ''' <summary>
    ''' This subroutine ensures that the User Interface is set up properly 
    ''' and sets some variables to their initial values.
    ''' </summary>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the brush size equal to the entire Picture Box area by defualt
        brushSize = New Rectangle(0, 0, picDemoArea.Width, picDemoArea.Height)

        ' Fill the cboWrapMode combo box with the various WrapMode enum values. 
        ' The Shared method Enum.GetValues returns an array of all the wrap modes.
        For Each mode As WrapMode In System.Enum.GetValues(GetType(WrapMode))
            cboWrapMode.Items.Add(mode)
        Next

        ' Fill the hatch style combo.  
        For Each style As HatchStyle In System.Enum.GetValues(GetType(HatchStyle))
            cboHatchStyle.Items.Add(style)
        Next

        ' Fill the available GradientStyles
        For Each gradient As LinearGradientMode In System.Enum.GetValues(GetType(LinearGradientMode))
            cboGradientMode.Items.Add(gradient)
        Next
    End Sub


    ''' <summary>
    ''' This routine includes the bulk of the demonstration. It creates one
    ''' of 5 types of brushes, and assigns the appropriate user defined parameters
    ''' to the brush. The brush is then assigned to demoBrush, which is used to 
    ''' draw one of three different shapes.  There is also code to ensure that 
    ''' the UI only displays the options that are appropriate for the type
    ''' of brush being used.
    ''' Please note that this Error Handler handles virtually all of the events
    ''' fired by the UI.
    ''' </summary>
    Private Sub RedrawPicture(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboBrushType.SelectedIndexChanged, cboDrawing.SelectedIndexChanged, txtColor1.TextChanged, cboWrapMode.SelectedIndexChanged, cboHatchStyle.SelectedIndexChanged, txtColor2.TextChanged, cboGradientMode.SelectedIndexChanged, nudRotation.ValueChanged, nudGradientBlend.ValueChanged

        ' Reset the PictureBox
        picDemoArea.CreateGraphics().Clear(Color.White)
        picDemoArea.Refresh()

        ' Reset the Status Bar
        Me.sbrDrawingStatus.Text = ""

        ' Construct the brush with the user selected properties. One of five
        ' different brushes will be created based on the user selection.
        ' The reason a brush of the specific type is created, and then assigned
        ' to demoBrush, is that Intellisense is available when working with 
        ' the specific brush objects.
        Select Case cboBrushType.Text

            Case "Solid" ' Use a SolidBrush

                ' Update the UI: deactivate and reactivate all the appropriate 
                ' controls for this brush
                Me.cboBrushSize.Enabled = False
                Me.cboHatchStyle.Enabled = False
                Me.cboWrapMode.Enabled = False
                Me.txtColor2.Enabled = False
                Me.btnSetColor2.Enabled = False
                Me.nudGradientBlend.Enabled = False
                Me.nudRotation.Enabled = False
                Me.cboGradientMode.Enabled = False

                ' Create a solid brush based on selected color
                Dim mySolidBrush As New SolidBrush(color1)

                ' Another good way to get a solid brush, if you know the color
                ' you want at design time is to use the Brushes class
                ' For instance, this line builds an AliceBlue brush
                ' demoBrush = Brushes.AliceBlue

                ' Set demoBrush equal to the newly created brush
                demoBrush = mySolidBrush

            Case "Hatch" ' Use a HatchBrush

                ' Update the UI: deactivate and reactivate all the appropriate 
                ' controls for this brush
                Me.cboBrushSize.Enabled = False
                Me.cboHatchStyle.Enabled = True
                Me.cboWrapMode.Enabled = False
                Me.txtColor2.Enabled = True
                Me.btnSetColor2.Enabled = True
                Me.nudGradientBlend.Enabled = False
                Me.nudRotation.Enabled = False
                Me.cboGradientMode.Enabled = False

                ' Create a new HatchBrush using the two colors for 
                ' foreground and background color settings.
                ' Since the HatchStyle property is Read-Only the HatchStyle
                ' must be set at the creation of the HatchBrush
                Dim myHatchBrush As New HatchBrush( _
                    CType(cboHatchStyle.SelectedItem, HatchStyle), _
                    color1, color2)

                ' Set demoBrush equal to the newly created brush
                demoBrush = myHatchBrush

            Case "Texture" ' Use a TextureBrush

                ' Update the UI: deactivate and reactivate all the appropriate 
                ' controls for this brush
                Me.cboBrushSize.Enabled = True
                Me.cboHatchStyle.Enabled = False
                Me.cboWrapMode.Enabled = True
                Me.txtColor2.Enabled = False
                Me.btnSetColor2.Enabled = False
                Me.nudGradientBlend.Enabled = False
                Me.nudRotation.Enabled = True
                Me.cboGradientMode.Enabled = False

                ' Create a new TextureBrush based on a bitmap. This bitmap can
                ' also be a pattern that you have created. 
                ' Be cautious here, since defining a Rectangle large that
                ' that the provided Bitmap will cause an OutOfMemory 
                ' exception to be thrown.
                Dim myTextureBrush As New TextureBrush( _
                    My.Resources.WaterLilies, brushSize)

                ' The WrapMode determines how the brush will be tiled if it
                ' is not spread over the entire graphic area.
                myTextureBrush.WrapMode = CType(cboWrapMode.SelectedItem, WrapMode)

                ' The RotateTransform method rotates the brush by the user
                ' specified amount
                myTextureBrush.RotateTransform(nudRotation.Value)

                ' You can also use a ScaleTransform to deform the brush
                ' The following cuts the width of brush in half, and
                ' doubles the height.
                'myTextureBrush.ScaleTransform(0.5F, 2.0F)

                ' Set demoBrush equal to the newly created brush
                demoBrush = myTextureBrush

            Case "LinearGradient" ' Use a LinearGradientBrush

                ' Update the UI: deactivate and reactivate all the appropriate 
                ' controls for this brush
                Me.cboBrushSize.Enabled = True
                Me.cboHatchStyle.Enabled = False
                Me.cboWrapMode.Enabled = True
                Me.txtColor2.Enabled = True
                Me.btnSetColor2.Enabled = True
                Me.nudGradientBlend.Enabled = True
                Me.nudRotation.Enabled = True
                Me.cboGradientMode.Enabled = True

                ' Create a new LinearGradientBrush.  The brush is based on 
                ' a size defined by a rectangle, in this case using the
                ' user defined brushSize. Two colors are used defining
                ' the start and end colors of the gradient. (More advanced 
                ' gradients can be built using the Blend property.)
                ' Finally, the LinearGradientMode is defined in the constructor.
                ' An angle can also be used, but for simplicity it is not here.
                Dim myLinearGradientBrush As New LinearGradientBrush( _
                    brushSize, color1, color2, _
                    CType(cboGradientMode.SelectedItem, LinearGradientMode))

                ' The WrapMode determines how the brush will be tiled if it
                ' is not spread over the entire graphic area.
                ' The LinearGradientBrush cannot use the Clamp value for WrapMode
                If CType(cboWrapMode.SelectedItem, WrapMode) <> WrapMode.Clamp Then
                    myLinearGradientBrush.WrapMode = _
                        CType(cboWrapMode.SelectedItem, WrapMode)
                Else
                    Me.sbrDrawingStatus.Text += _
                        "A Linear Gradient Brush cannot use the Clamp WrapMode."
                End If

                ' The RotateTransform method rotates the brush by the user
                ' specified amount
                myLinearGradientBrush.RotateTransform(nudRotation.Value)

                ' You can also use a ScaleTransform to deform the brush
                ' The following cuts the width of brush in half, and
                ' doubles the height.
                'myLinearGradientBrush.ScaleTransform(0.5F, 2.0F)

                ' Set the point where the blending will focus.  Any single 
                ' between 0 and 1 is allowed. The default is one.
                myLinearGradientBrush.SetBlendTriangularShape( _
                    nudGradientBlend.Value)

                ' For more advanced uses, you can use the SetSigmaBellShape
                ' method to set where the center of the gradient occurs.
                'myLinearGradientBrush.SetSigmaBellShape(0.2)

                ' Set demoBrush equal to the newly created brush
                demoBrush = myLinearGradientBrush

            Case "PathGradient" ' Use a PathGradientBrush

                ' Update the UI: deactivate and reactivate all the appropriate 
                ' controls for this brush
                Me.cboBrushSize.Enabled = True
                Me.cboHatchStyle.Enabled = False
                Me.cboWrapMode.Enabled = True
                Me.txtColor2.Enabled = True
                Me.btnSetColor2.Enabled = True
                Me.nudGradientBlend.Enabled = True
                Me.nudRotation.Enabled = True
                Me.cboGradientMode.Enabled = False

                ' Define a set of points to use for the path this gradient will
                ' follow. A GraphicsPath object could also be defined and used
                ' instead. In this case, we're using a simple triangle.
                Dim pathPoint() As Point = {New Point(0, brushSize.Height), _
                        New Point(brushSize.Width, brushSize.Height), _
                        New Point(brushSize.Width, 0)}

                ' Create a new PathGradientBrush based on the path just created.
                ' (Anything not bounded by the path will be transparent, 
                ' instead of containing coloring.)
                Dim myPathGradientBrush As New PathGradientBrush(pathPoint)

                ' Set the Colors for the PathGradient, this is done differently
                ' from other gradients, since differnt colors can be used for 
                ' each side. In this case, only one color is used, but a color
                ' could be assigned to each side of the path.
                ' The CenterColor is the color that the edges blend into.
                myPathGradientBrush.CenterColor = color1
                ' The SurroundColors is an array of colors that defines the
                ' colors around the edge.
                myPathGradientBrush.SurroundColors = New Color() {color2}

                ' For advanced uses, the CenterPoint property can be set 
                ' somewhere other that the center of the path (even outside 
                ' the rectangle bounding the path).
                'myPathGradientBrush.CenterPoint = New PointF(50, 50)

                ' The WrapMode determines how the brush will be tiled if it
                ' is not spread over the entire graphic area.
                myPathGradientBrush.WrapMode = _
                    CType(cboWrapMode.SelectedItem, WrapMode)

                ' The RotateTransform method rotates the brush by the user
                ' specified amount
                myPathGradientBrush.RotateTransform(nudRotation.Value)

                ' You can also use a ScaleTransform to deform the brush
                ' The following cuts the width of brush in half, and
                ' doubles the height.
                'myPathGradientBrush.ScaleTransform(0.5F, 2.0F)

                ' Set the blending
                myPathGradientBrush.SetBlendTriangularShape( _
                    nudGradientBlend.Value)

                ' For more advanced uses, you can use the SetSigmaBellShape
                ' method to set where the center of the gradient occurs.
                'myPathGradientBrush.SetSigmaBellShape(0.2)

                ' Set demoBrush equal to the newly created brush
                demoBrush = myPathGradientBrush
            Case Else
                demoBrush = New SolidBrush(color1)
        End Select

        ' Use the brush to draw the appropriate Drawing in the picDemoArea
        formGraphics = picDemoArea.CreateGraphics()

        ' Select the Type of drawing based on user input
        Select Case cboDrawing.Text
            Case "Fill"
                ' "Fill" fills the entire PictureBox
                formGraphics.FillRectangle(demoBrush, 0, 0, _
                    picDemoArea.Width, picDemoArea.Height)

            Case "Ellipses"
                ' "Ellipses" draws two intesecting ellipses
                formGraphics.FillEllipse(demoBrush, picDemoArea.Width \ 10, _
                    picDemoArea.Height \ 10, picDemoArea.Width \ 2, _
                    picDemoArea.Height \ 2)
                formGraphics.FillEllipse(demoBrush, picDemoArea.Width \ 3, _
                    picDemoArea.Height \ 3, picDemoArea.Width \ 2, _
                    picDemoArea.Height \ 2)
            Case "Lines"
                ' "Lines" draws a series of intersecting lines

                ' First build a Pen based on the brush
                Dim demoPen As New Pen(demoBrush, 40)

                ' Now do the drawing from each corner to all other corners
                formGraphics.DrawLine( _
                    demoPen, 0, 0, picDemoArea.Width, picDemoArea.Height)
                formGraphics.DrawLine(demoPen, 0, 0, 0, picDemoArea.Height)
                formGraphics.DrawLine(demoPen, 0, 0, picDemoArea.Width, 0)
                formGraphics.DrawLine(demoPen, picDemoArea.Width, 0, _
                    picDemoArea.Width, picDemoArea.Height)
                formGraphics.DrawLine(demoPen, 0, picDemoArea.Height, _
                    picDemoArea.Width, picDemoArea.Height)
                formGraphics.DrawLine( _
                    demoPen, picDemoArea.Width, 0, 0, picDemoArea.Height)

        End Select

        ' Set the Drawing Status to Success if there weren't any other problems
        If Me.sbrDrawingStatus.Text = "" Then
            Me.sbrDrawingStatus.Text = "Success!"
        End If

    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub
End Class
