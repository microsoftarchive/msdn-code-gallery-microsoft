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
Imports Windows.Storage.FileProperties
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current

    Private dateAccessedProperty As String = "System.DateAccessed"
    Private fileOwnerProperty As String = "System.FileOwner"


    Public Sub New()
        Me.InitializeComponent()
        AddHandler ShowPropertiesButton.Click, AddressOf ShowPropertiesButton_Click
    End Sub

    Private Async Sub ShowPropertiesButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            rootPage.ResetScenarioOutput(OutputTextBlock)
            Dim file As StorageFile = rootPage.sampleFile
            If file IsNot Nothing Then
                ' Get top level file properties
                Dim outputText As New StringBuilder()
                outputText.AppendLine("File name: " & file.Name)
                outputText.AppendLine("File type: " & file.FileType)

                ' Get basic properties
                Dim basicProperties As BasicProperties = Await file.GetBasicPropertiesAsync()
                outputText.AppendLine("File size: " & basicProperties.Size & " bytes")
                outputText.AppendLine("Date modified: " & basicProperties.DateModified.ToString)

                ' Get extra properties
                Dim propertiesName As New List(Of String)()
                propertiesName.Add(dateAccessedProperty)
                propertiesName.Add(fileOwnerProperty)
                Dim extraProperties As IDictionary(Of String, Object) = Await file.Properties.RetrievePropertiesAsync(propertiesName)
                Dim propValue = extraProperties(dateAccessedProperty)
                If propValue IsNot Nothing Then
                    outputText.AppendLine("Date accessed: " & propValue.ToString)
                End If
                propValue = extraProperties(fileOwnerProperty)
                If propValue IsNot Nothing Then
                    outputText.AppendLine("File owner: " & propValue.ToString)
                End If

                OutputTextBlock.Text = outputText.ToString()
            End If
        Catch ex As FileNotFoundException
            rootPage.NotifyUserFileNotExist()
        End Try
    End Sub
End Class
