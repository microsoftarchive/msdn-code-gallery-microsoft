'******************************** Module Header ***********************************'
' Module Name:  MainForm.cs
' Project:      VBExtractAudioFile
' Copyright (c) Microsoft Corporation.
'
' The sample demonstrates how to extract and convert audio file formats, which 
' include wav, mp3 and mp4 files.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************'

Imports System.IO
Imports System.Security.Permissions



Partial Friend Class MainForm
    Inherits Form
    Public Sub New()
        InitializeComponent()

        Me.cmbOutputAudioType.DataSource = System.Enum.GetValues(GetType(OutputAudioType))
        Me.tbOutputDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
    End Sub

    ''' <summary>
    ''' Open the music file using the OpenFileDialog object.
    ''' </summary>
    Private Sub btnChooseSourceFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnChooseSourceFile.Click
        If Me.openAudioFileDialog.ShowDialog() = DialogResult.OK Then
            Me.player.URL = openAudioFileDialog.FileName
            Me.player.Ctlcontrols.play()

            Me.btnChooseSourceFile.Text = openAudioFileDialog.FileName
        End If
    End Sub

    ''' <summary>
    ''' Set the startpoint and the endpoint of the video clip.
    ''' </summary>
    Private Sub btnSetBeginEndPoints_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSetBeginEndPoints.Click
        If Not String.IsNullOrEmpty(Me.player.URL) Then
            If btnSetBeginEndPoints.Tag.Equals("SetStartPoint") Then
                ' Save the startpoint.
                ' player.Ctlcontrols.currentPosition contains the current 
                ' position in the media item in seconds from the beginning.
                Me.tbStartpoint.Text = (player.Ctlcontrols.currentPosition * 1000).ToString("0")
                Me.tbEndpoint.Text = ""

                Me.btnSetBeginEndPoints.Text = "Set End Point"
                Me.btnSetBeginEndPoints.Tag = "SetEndPoint"
            ElseIf btnSetBeginEndPoints.Tag.Equals("SetEndPoint") Then
                ' Check if the startpoint is in front of the endpoint.
                Dim endPoint As Integer = CInt(Fix(player.Ctlcontrols.currentPosition * 1000))
                If endPoint <= Integer.Parse(Me.tbStartpoint.Text) Then
                    MessageBox.Show("Audio endpoint is overlapped. Please reset the endpoint.")
                Else
                    ' Save the endpoint.
                    Me.tbEndpoint.Text = endPoint.ToString()

                    Me.btnSetBeginEndPoints.Text = "Set Start Point"
                    Me.btnSetBeginEndPoints.Tag = "SetStartPoint"
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Extract the video clip.
    ''' </summary>
    Private Sub btnExtract_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExtract.Click
        Try
            ' Check parameters ...
            Dim sourceAudioFile As String = Me.player.URL
            If String.IsNullOrEmpty(sourceAudioFile) Then
                Throw New ArgumentException("Please choose the source audio file")
            End If
            Dim outputDirectory As String = Me.tbOutputDirectory.Text
            If String.IsNullOrEmpty(outputDirectory) Then
                Throw New ArgumentException("Please specify the output directory")
            End If
            Dim startpoint As String = Me.tbStartpoint.Text
            If String.IsNullOrEmpty(tbStartpoint.Text) Then
                Throw New ArgumentException("Please specify the startpoint of the audio clip")
            End If
            Dim endpoint As String = Me.tbEndpoint.Text
            If String.IsNullOrEmpty(endpoint) Then
                Throw New ArgumentException("Please specify the endpoint of the audio clip")
            End If

            ' Extract the audio file.
            Dim outputType As OutputAudioType = CType(Me.cmbOutputAudioType.SelectedValue, OutputAudioType)
            Dim outputFileName As String = ExpressionEncoderWrapper.ExtractAudio(sourceAudioFile, outputDirectory, outputType, Double.Parse(startpoint), Double.Parse(endpoint))

            MessageBox.Show("Audio file is successfully extracted: " & outputFileName)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error")
        End Try
    End Sub

    ''' <summary>
    ''' Select the output directory
    ''' </summary>
    Private Sub btnChooseOutputDirectory_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnChooseOutputDirectory.Click
        If Me.outputFolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Me.tbOutputDirectory.Text = outputFolderBrowserDialog.SelectedPath
        End If
    End Sub

    Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Releases Windows Media Player resources.
        Me.player.close()
    End Sub
End Class
