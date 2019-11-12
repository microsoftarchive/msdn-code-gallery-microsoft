' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Private Sub btnValidate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidate.Click
        ' Used to loop through all the controls on the form.
        Dim existingControl As Control

        ' Holds the message that will be displayed to the user indicating
        ' whether the controls contain valid input.
        Dim validationMessage As String = String.Empty

        ' Loop through all the controls on the form.
        For Each existingControl In Controls
            ' If the current control inherits RegExTextBox
            If TypeOf existingControl Is RegExTextBox Then
                ' Cast it from an existingControl type to a RegExTextBox type.
                ' This will allow you to access the IsValid property.
                Dim regexControl As RegExTextBox = CType(existingControl, RegExTextBox)

                ' If the text in the control isn't correct, then add this control
                ' to the list of invalid controls.
                If Not regexControl.IsValid Then
                    validationMessage &= regexControl.Name & ":" & _
                      regexControl.ErrorMessage & vbCrLf
                End If
            End If
        Next

        ' Are there controls that contain invalid text?
        If validationMessage <> "" Then
            ' List those controls in the textbox.
            txtInvalidControls.Text = "The following controls have invalid values: " _
                & vbCrLf & validationMessage
        Else
            ' Otherwise indicate that everything is ok.
            txtInvalidControls.Text = "All controls contain valid input."
        End If
    End Sub

    Private Sub mskIPAddress_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mskIPAddress.Leave
        lblIPAddress.Text = mskIPAddress.Text
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
