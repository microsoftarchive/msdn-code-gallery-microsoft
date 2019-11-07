' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class ZipCodeTextBox
    Inherits RegExTextBox

    Private Sub SetValidation()
        ' Set a default value for the ValidationExpression so
        ' that this control will validate that the contents of
        ' the TextBox look like a 5 digit US zip code.
        Me.ValidationExpression = "^\d{5}$"
        Me.ErrorMessage = "The zip code must be in the form of 99999"
    End Sub

End Class
