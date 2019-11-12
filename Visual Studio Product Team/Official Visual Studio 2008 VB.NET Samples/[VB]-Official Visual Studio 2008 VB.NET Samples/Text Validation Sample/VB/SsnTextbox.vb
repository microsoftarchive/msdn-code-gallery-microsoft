' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class SsnTextbox
    Inherits RegExTextBox

    Private Sub SetValidation()
        ' Set a default value for the ValidationExpression so
        ' that this control will validate that the contents of
        ' the TextBox look like a social security number.
        Me.ValidationExpression = "^\d{3}-\d{2}-\d{4}$"
        Me.ErrorMessage = "The social security number must be in the form of 555-55-5555"
    End Sub

End Class
