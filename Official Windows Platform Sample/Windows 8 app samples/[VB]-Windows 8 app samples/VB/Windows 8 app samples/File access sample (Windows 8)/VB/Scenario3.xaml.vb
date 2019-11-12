'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate

Imports System
Imports System.IO
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler WriteBytesButton.Click, AddressOf WriteBytesButton_Click
        AddHandler ReadBytesButton.Click, AddressOf ReadBytesButton_Click
    End Sub

    Private Function GetBufferFromString(str As String) As IBuffer
        Using memoryStream As New InMemoryRandomAccessStream()
            Using dataWriter As New DataWriter(memoryStream)
                dataWriter.WriteString(str)
                Return dataWriter.DetachBuffer()
            End Using
        End Using
    End Function

    Private Async Sub WriteBytesButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                Dim userContent As String = InputTextBox.Text
                If Not String.IsNullOrEmpty(userContent) Then
                    Dim buffer As IBuffer = GetBufferFromString(userContent)
                    Await FileIO.WriteBufferAsync(file, buffer)
                    OutputTextBlock.Text = "The following " & buffer.Length & " bytes of text were written to '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & userContent
                Else
                    OutputTextBlock.Text = "The text box is empty, please write something and then click 'Write' again."
                End If
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub

    Private Async Sub ReadBytesButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                Dim buffer As IBuffer = Await FileIO.ReadBufferAsync(file)
                Using dataReader__1 As DataReader = DataReader.FromBuffer(buffer)
                    Dim fileContent As String = dataReader__1.ReadString(buffer.Length)
                    OutputTextBlock.Text = "The following " & buffer.Length & " bytes of text were read from '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & fileContent
                End Using
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub
End Class
