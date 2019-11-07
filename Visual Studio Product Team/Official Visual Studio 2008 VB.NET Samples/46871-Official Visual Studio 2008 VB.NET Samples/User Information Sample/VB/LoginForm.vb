' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class LoginForm

    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        Dim samplePrincipal As New SampleIPrincipal(Me.UsernameTextBox.Text, Me.PasswordTextBox.Text)
        My.User.CurrentPrincipal = samplePrincipal

        If (Not My.User.IsAuthenticated) Then
            'Check to see if the username and password are correct using My.User
            Me.UsernameTextBox.Text = String.Empty
            Me.PasswordTextBox.Text = String.Empty
            MessageBox.Show("Username or password are incorrect")
        Else
            ' The username and password are correct, so display the main form.
            Me.Visible = False
            My.Forms.MainForm.Show()
        End If
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub

    Private Sub LoginForm1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' If the current user is not authenticated, then prompt for a username and password and use a custom 
        ' IPrincipal/IIdentity pair to perfom custom authentication and authorization. A sample, insecure 
        ' IPrincipal/IIdentity implementation has been provided in the sample.
        '
        ' See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnnetsec/html/SecNetHT06.asp for 
        ' more information on the IPrincipal and IIdentity interfaces.
        If (My.User.IsAuthenticated) Then
            ' The user is already authenticated, so just display the main form.
            ' The user has not been authenticated, so enable custom authentication and authorization
            Me.Close()
            My.Forms.MainForm.Show()
        End If
    End Sub
End Class
