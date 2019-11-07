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
Imports System.Collections.Generic
Imports System.Text
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler AddToListButton.Click, AddressOf AddToListButton_Click
        AddHandler ShowListButton.Click, AddressOf ShowListButton_Click
        AddHandler OpenFromListButton.Click, AddressOf OpenFromListButton_Click
    End Sub

    Private Sub AddToListButton_Click(sender As Object, e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            If MRURadioButton.IsChecked.Value Then
                rootPage.mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Name)
                OutputTextBlock.Text = "The file '" & file.Name & "' was added to the MRU list and a token was stored."
            ElseIf FALRadioButton.IsChecked.Value Then
                rootPage.falToken = StorageApplicationPermissions.FutureAccessList.Add(file, file.Name)
                OutputTextBlock.Text = "The file '" & file.Name & "' was added to the FAL list and a token was stored."
            End If
        End If
    End Sub

    Private Sub ShowListButton_Click(sender As Object, e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            If MRURadioButton.IsChecked.Value Then
                Dim entries As AccessListEntryView = StorageApplicationPermissions.MostRecentlyUsedList.Entries
                If entries.Count > 0 Then
                    Dim outputText As New StringBuilder("The MRU list contains the following item(s):" & Environment.NewLine & Environment.NewLine)
                    For Each entry As AccessListEntry In entries
                        ' Application previously chose to store file.Name in this field
                        outputText.AppendLine(entry.Metadata)
                    Next

                    OutputTextBlock.Text = outputText.ToString
                Else
                    OutputTextBlock.Text = "The MRU list is empty, please select 'Most Recently Used' list and click 'Add to List' to add a file to the MRU list."
                End If
            ElseIf FALRadioButton.IsChecked.Value Then
                Dim entries As AccessListEntryView = StorageApplicationPermissions.FutureAccessList.Entries
                If entries.Count > 0 Then
                    Dim outputText As New StringBuilder("The FAL list contains the following item(s):" & Environment.NewLine & Environment.NewLine)
                    For Each entry As AccessListEntry In entries
                        ' Application previously chose to store file.Name in this field
                        outputText.AppendLine(entry.Metadata)
                    Next

                    OutputTextBlock.Text = outputText.ToString
                Else
                    OutputTextBlock.Text = "The FAL list is empty, please select 'Future Access List' list and click 'Add to List' to add a file to the FAL list."
                End If
            End If
        End If
    End Sub

    Private Async Sub OpenFromListButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            If rootPage.sampleFile IsNot Nothing Then
                If MRURadioButton.IsChecked.Value Then
                    If rootPage.mruToken IsNot Nothing Then
                        ' Open the file via the token that was stored when adding this file into the MRU list
                        Dim file As StorageFile = Await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(rootPage.mruToken)

                        ' Read the file
                        Dim fileContent As String = Await FileIO.ReadTextAsync(file)
                        OutputTextBlock.Text = "The file '" & file.Name & "' was opened by a stored token from the MRU list, it contains the following text:" & Environment.NewLine & Environment.NewLine & fileContent
                    Else
                        OutputTextBlock.Text = "The MRU list is empty, please select 'Most Recently Used' list and click 'Add to List' to add a file to the MRU list."
                    End If
                ElseIf FALRadioButton.IsChecked.Value Then
                    If rootPage.falToken IsNot Nothing Then
                        ' Open the file via the token that was stored when adding this file into the FAL list
                        Dim file As StorageFile = Await StorageApplicationPermissions.FutureAccessList.GetFileAsync(rootPage.falToken)

                        ' Read the file
                        Dim fileContent As String = Await FileIO.ReadTextAsync(file)
                        OutputTextBlock.Text = "The file '" & file.Name & "' was opened by a stored token from the FAL list, it contains the following text:" & Environment.NewLine & Environment.NewLine & fileContent
                    Else
                        OutputTextBlock.Text = "The FAL list is empty, please select 'Future Access List' list and click 'Add to List' to add a file to the FAL list."
                    End If
                End If
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub
End Class
