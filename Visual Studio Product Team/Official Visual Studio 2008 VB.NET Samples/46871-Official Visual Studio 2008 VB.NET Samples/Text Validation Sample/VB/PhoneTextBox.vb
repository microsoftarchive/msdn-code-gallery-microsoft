' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class PhoneTextBox
    Inherits RegExTextBox

    Private Sub SetValidation()
        ' Set a default value for the ValidationExpression so
        ' that this control will validate that the contents of
        ' the TextBox look like a phone number.
        Me.ValidationExpression = "^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$"
        Me.ErrorMessage = "The phone number must be in the form of (555) 555-1212 or 555-555-1212."
    End Sub
End Class
