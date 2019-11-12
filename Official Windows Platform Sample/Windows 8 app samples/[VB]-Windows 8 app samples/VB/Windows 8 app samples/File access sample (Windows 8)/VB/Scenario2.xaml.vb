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
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler WriteTextButton.Click, AddressOf WriteTextButton_Click
        AddHandler ReadTextButton.Click, AddressOf ReadTextButton_Click
    End Sub

    Private Async Sub WriteTextButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                Dim userContent As String = InputTextBox.Text
                If Not String.IsNullOrEmpty(userContent) Then
                    Await FileIO.WriteTextAsync(file, userContent)
                    OutputTextBlock.Text = "The following text was written to '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & userContent
                Else
                    OutputTextBlock.Text = "The text box is empty, please write something and then click 'Write' again."
                End If
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub

    Private Async Sub ReadTextButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                Dim fileContent As String = Await FileIO.ReadTextAsync(file)
                OutputTextBlock.Text = "The following text was read from '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & fileContent
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub
End Class
