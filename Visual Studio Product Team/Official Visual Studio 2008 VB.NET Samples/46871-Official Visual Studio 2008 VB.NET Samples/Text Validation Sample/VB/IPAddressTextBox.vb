' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class IPAddressTextBox
    Inherits RegExTextBox

    Private Sub SetValidation()
        ' Set a default value for the ValidationExpression so
        ' that this control will validate that the contents of
        ' the TextBox look like an IP address.
        Me.ValidationExpression = "^((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])$"
        Me.ErrorMessage = "The IP address must be in the form of 111.111.111.111"
    End Sub

End Class
