' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Deployment

Public Class MainForm

    Private WithEvents deployment As System.Deployment.Application.ApplicationDeployment

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If My.Application.IsNetworkDeployed Then
            AddHandler My.Application.Deployment.CheckForUpdateCompleted, AddressOf Me.CheckForUpdateCompleted
            AddHandler My.Application.Deployment.DownloadFileGroupCompleted, AddressOf Me.DownloadFilesCompleted
            AddHandler My.Application.Deployment.UpdateProgressChanged, AddressOf Me.ProgressChanged
            AddHandler My.Application.Deployment.UpdateCompleted, AddressOf Me.UpdateCompleted
        Else
            AppendContentLine(My.Resources.IsNotNetworkDeployed)
        End If
    End Sub

    Private Property Content() As String
        Get
            Return Me.ContentTextbox.Text
        End Get
        Set(ByVal value As String)
            Me.ContentTextbox.Text = value
        End Set
    End Property


    Private Sub CurrentDeploymentInfoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurrentDeploymentInfoButton.Click
        Content = String.Empty
        If My.Application.IsNetworkDeployed Then
            With My.Application.Deployment
                AppendContentLine("Application full name: " & .UpdatedApplicationFullName)
                AppendContentLine("Current version: " & .CurrentVersion.ToString())
                AppendContentLine("Update location: " & .UpdateLocation.ToString())
                AppendContentLine("Last update check: " & .TimeOfLastUpdateCheck.ToString())
            End With
        Else
            AppendContentLine(My.Resources.IsNotNetworkDeployed)
        End If
    End Sub


    Private Sub UpdateInfoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateInfoButton.Click
        Content = String.Empty
        If (My.Application.IsNetworkDeployed) Then
            With My.Application.Deployment
                Dim uci As System.Deployment.Application.UpdateCheckInfo = .CheckForDetailedUpdate()

                AppendContentLine("Update available: " & uci.UpdateAvailable)

                If (Not (uci.UpdateAvailable)) Then
                    Exit Sub
                End If

                AppendContentLine("Available version: " & uci.AvailableVersion.ToString())
                AppendContentLine("Update required: " & uci.IsUpdateRequired)
                AppendContentLine("Minimum required version: " & uci.MinimumRequiredVersion.ToString())
                AppendContentLine("Update size: " & uci.UpdateSizeBytes.ToString())
            End With
        Else
            AppendContentLine(My.Resources.IsNotNetworkDeployed)
        End If
    End Sub

    Private Sub DownloadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadButton.Click
        Content = String.Empty
        If My.Application.IsNetworkDeployed Then
            My.Application.Deployment.DownloadFileGroupAsync("Media")
            AppendContentLine("Downloading files...")
        Else
            AppendContentLine(My.Resources.IsNotNetworkDeployed)
        End If
    End Sub

    Private Sub UpdateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateButton.Click
        Content = String.Empty
        If My.Application.IsNetworkDeployed Then
            AppendContentLine("Updating application...")
            My.Application.Deployment.UpdateAsync()
        Else
            AppendContentLine(My.Resources.IsNotNetworkDeployed)
        End If
    End Sub

    Private Sub CheckForUpdateCompleted(ByVal sender As Object, ByVal e As System.Deployment.Application.CheckForUpdateCompletedEventArgs)
        If e.UpdateAvailable Then
            AppendContentLine("Update available.")
        Else
            AppendContentLine("No update available.")
        End If
    End Sub
    Private Sub DownloadFilesCompleted(ByVal sender As Object, ByVal e As System.Deployment.Application.DownloadFileGroupCompletedEventArgs)
        AppendContentLine("Download files complete.")
        My.Forms.PictureForm.ShowDialog()
    End Sub


    Private Sub UpdateCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        AppendContentLine("Update complete. Restart the application to run the new version.")
    End Sub


    Private Sub ProgressChanged(ByVal sender As Object, ByVal e As System.Deployment.Application.DeploymentProgressChangedEventArgs)
        AppendContentLine(String.Format("({0}%) {1} of {2} bytes downloaded.", e.ProgressPercentage, e.BytesCompleted, e.BytesTotal))
    End Sub


    Private Sub AppendContentLine(ByVal str As String)
        Content += str & vbNewLine & vbNewLine
    End Sub

    Private Sub ViewSourceCodeToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewSourceCodeToolStripButton.Click
        My.Forms.ViewSourceForm.ShowDialog()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class