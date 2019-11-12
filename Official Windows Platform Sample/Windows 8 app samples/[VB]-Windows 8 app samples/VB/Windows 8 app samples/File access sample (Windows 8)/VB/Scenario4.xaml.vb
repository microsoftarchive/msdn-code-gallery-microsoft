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
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler WriteToStreamButton.Click, AddressOf WriteToStreamButton_Click
        AddHandler ReadFromStreamButton.Click, AddressOf ReadFromStreamButton_Click
    End Sub

    Private Async Sub WriteToStreamButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                Dim userContent As String = InputTextBox.Text
                If Not String.IsNullOrEmpty(userContent) Then
                    Using transaction = CType(Await file.OpenTransactedWriteAsync, StorageStreamTransaction)
                        Using dataWriter As New DataWriter(transaction.Stream)
                            dataWriter.WriteString(userContent)
                            transaction.Stream.Size = Await dataWriter.StoreAsync() ' reset stream size to override the file
                            Await transaction.CommitAsync()
                            OutputTextBlock.Text = "The following text was written to '" & file.Name & "' using a stream:" & Environment.NewLine & Environment.NewLine & userContent
                        End Using
                    End Using
                Else
                    OutputTextBlock.Text = "The text box is empty, please write something and then click 'Write' again."
                End If
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub

    Private Async Sub ReadFromStreamButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                Using readStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
                    Using dataReader As New DataReader(readStream)
                        Dim size As UInt64 = readStream.Size
                        If size <= UInt32.MaxValue Then
                            Dim numBytesLoaded As UInt32 = Await dataReader.LoadAsync(CType(size, UInt32))
                            Dim fileContent As String = dataReader.ReadString(numBytesLoaded)
                            OutputTextBlock.Text = "The following text was read from '" & file.Name & "' using a stream:" & Environment.NewLine & Environment.NewLine & fileContent
                        Else
                            OutputTextBlock.Text = "File " & file.Name & " is too big for LoadAsync to load in a single chunk. Files larger than 4GB need to be broken into multiple chunks to be loaded by LoadAsync."
                        End If
                    End Using
                End Using
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub

End Class
