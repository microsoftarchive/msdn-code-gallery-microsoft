Imports Microsoft.Ink

Public Class Form1
    Dim WithEvents myInkOverlay As InkOverlay
    Dim selectedMode As InkOverlayEditingMode

    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        myInkOverlay = New InkOverlay(Panel1)
        myInkOverlay.Enabled = True
    End Sub

    'Back of pen erase
    Sub CursorInRangeEventHandler(ByVal sender As Object, ByVal e As InkCollectorCursorInRangeEventArgs) Handles myInkOverlay.CursorInRange
        'If the pen is inverted, set mode to "delete"
        If (e.Cursor.Inverted) Then
            myInkOverlay.EditingMode = InkOverlayEditingMode.Delete
        Else
            'Pen is not inverted, so select whatever mode the user requested
            myInkOverlay.EditingMode = selectedMode
        End If
    End Sub

    'Ink mode radio button
    Sub InkRadioClickedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton1.CheckedChanged
        If (RadioButton1.Checked) Then
            selectedMode = InkOverlayEditingMode.Ink
            DoModeChange(InkOverlayEditingMode.Ink)
        End If
    End Sub

    'Select mode radio button
    Sub SelectRadioClickedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton2.CheckedChanged
        If (RadioButton2.Checked) Then
            selectedMode = InkOverlayEditingMode.Select
            DoModeChange(InkOverlayEditingMode.Select)
        End If
    End Sub

    'Delete mode radio button 
    Sub DeleteRadioClickedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton3.CheckedChanged
        If (RadioButton3.Checked) Then
            selectedMode = InkOverlayEditingMode.Delete
            DoModeChange(InkOverlayEditingMode.Delete)
        End If
    End Sub

    'Wire up delete mode buttons

    'Delete entire stroke
    Sub StrokeRadioClickedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton4.CheckedChanged
        'Can be called during Form construction, prior to myInkOverlay instantiation
        If (myInkOverlay Is Nothing) Then
            Return
        End If

        If (RadioButton4.Checked) Then
            myInkOverlay.EraserMode = InkOverlayEraserMode.StrokeErase
        End If
    End Sub

    'Delete point(s) at pen tip only 
    Sub PointRadioClickedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton5.CheckedChanged
        If (RadioButton5.Checked) Then
            myInkOverlay.EraserMode = InkOverlayEraserMode.PointErase
        End If
    End Sub

    Sub DoModeChange(ByVal NewMode As InkOverlayEditingMode)
        'Can be called during Form construction, prior to myInkOverlay instantiation
        If (myInkOverlay Is Nothing) Then
            Return
        End If

        'Switch the collection mode
        myInkOverlay.EditingMode = NewMode

        'Switch the radio buttons to reflect new mode
        Select Case NewMode
            Case InkOverlayEditingMode.Ink
                RadioButton1.Checked = True
                RadioButton2.Checked = False
                RadioButton3.Checked = False
            Case InkOverlayEditingMode.Select
                RadioButton1.Checked = False
                RadioButton2.Checked = True
                RadioButton3.Checked = False
            Case InkOverlayEditingMode.Delete
                RadioButton1.Checked = False
                RadioButton2.Checked = False
                RadioButton3.Checked = True
            Case Else
                Throw New ArgumentOutOfRangeException()
        End Select
    End Sub


End Class
