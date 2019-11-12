Imports Microsoft.Ink

Public Class Form1
    Dim myInkOverlay As InkOverlay


    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'Place the various raster operations in the group box
        InitializeRasterOperations()
    End Sub

    'Handle the form loading event by wiring up ink collection
    Sub FormLoadHandler(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'Create and enable the ink collecting Control
        myInkOverlay = New InkOverlay(Panel1)
        myInkOverlay.Enabled = True

        'Set the attributes of the ink collector
        SetAttributes()
    End Sub


    'Creates controls for selecting raster operation
    Sub InitializeRasterOperations()
        'Add all the types of Raster Operations to the groupbox for possible selection
        For Each rasterOpName As String In System.Enum.GetNames(GetType(RasterOperation))
            'Create a new radio button
            Dim rasterButton As New RadioButton()
            rasterButton.Text = rasterOpName

            'Default raster op is "Copy Pen"
            If rasterOpName = "CopyPen" Then
                rasterButton.Checked = True
            End If

            'Add it to the flow layout panel
            FlowLayoutPanel1.Controls.Add(rasterButton)

            'Add a handler to it
            AddHandler rasterButton.CheckedChanged, AddressOf RasterOpChosenHandler
        Next
    End Sub

    'Handles selections of raster operation
    Sub RasterOpChosenHandler(ByVal sender As Object, ByVal e As EventArgs)
        'Who has been chosen?
        Dim chosenButton As RadioButton = CType(sender, RadioButton)
        'This will be called if checked or unchecked, so only assign on checked
        If chosenButton.Checked Then
            'What's the RasterOperation value of selected button?
            Dim rasterOp = System.Enum.Parse(GetType(RasterOperation), chosenButton.Text)
            'Set attribute
            myInkOverlay.DefaultDrawingAttributes.RasterOperation = rasterOp
        End If
    End Sub

    Sub SetAttributesHandler(ByVal sender As Object, ByVal e As EventArgs) Handles antiAliasCheckbox.CheckedChanged, pressureSensitiveCheckbox.CheckedChanged, penTipRectangle.CheckedChanged, penTipEllipse.CheckedChanged, widthUpDown.ValueChanged, transparencyUpDown.ValueChanged, heightUpDown.ValueChanged
        'Can be called prior to myInkOverlay being initialized, so check if that's the case
        If myInkOverlay Is Nothing Then
            Return
        End If
        SetAttributes()
    End Sub

    'Sets the major attributes of the ink overlay (minus color, raster op, and some rare properties
    Sub SetAttributes()
        'Anti-aliasing
        If antiAliasCheckbox.Checked = True Then
            myInkOverlay.DefaultDrawingAttributes.AntiAliased = True
        Else
            myInkOverlay.DefaultDrawingAttributes.AntiAliased = False
        End If

        'Pressure sensitivity
        If pressureSensitiveCheckbox.Checked = True Then
            myInkOverlay.DefaultDrawingAttributes.IgnorePressure = False
        Else
            myInkOverlay.DefaultDrawingAttributes.IgnorePressure = True
        End If

        'Pen Tip
        If penTipRectangle.Checked = True Then
            myInkOverlay.DefaultDrawingAttributes.PenTip = PenTip.Rectangle
        Else
            myInkOverlay.DefaultDrawingAttributes.PenTip = PenTip.Ball
        End If

        'Transparency
        myInkOverlay.DefaultDrawingAttributes.Transparency = transparencyUpDown.Value

        'Height
        myInkOverlay.DefaultDrawingAttributes.Height = heightUpDown.Value
        'Width
        myInkOverlay.DefaultDrawingAttributes.Width = widthUpDown.Value
    End Sub

    'Handles "color" button
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Create a standard dialog
        Dim colorDialog As New ColorDialog()

        'Set color to current color
        colorDialog.Color = myInkOverlay.DefaultDrawingAttributes.Color

        'Show it
        If colorDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            'Set color to chosen
            myInkOverlay.DefaultDrawingAttributes.Color = colorDialog.Color
        End If
    End Sub
End Class
