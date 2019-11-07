' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class EMailTextBox
    Inherits RegExTextBox

    Private Sub SetValidation()
        ' Set a default value for the ValidationExpression so
        ' that this control will validate that the contents of
        ' the TextBox look like an e-mail address.
        Me.ValidationExpression = "^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"
        Me.ErrorMessage = "The e-mail address must be in the form of abc@microsoft.com"
    End Sub

End Class
