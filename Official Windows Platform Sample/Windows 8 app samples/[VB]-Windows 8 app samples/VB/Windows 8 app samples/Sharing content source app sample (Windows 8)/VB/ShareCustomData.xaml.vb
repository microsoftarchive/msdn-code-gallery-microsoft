'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.ApplicationModel.DataTransfer
Imports SDKTemplate

Partial Public NotInheritable Class ShareCustomData
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current
    Private dataTransferManager As DataTransferManager

    Public Sub New()
        Me.InitializeComponent()
        CustomDataTextBox.Text = "{" & vbCr & vbLf & "               ""type"" : ""http://schema.org/Book""," & vbCr & vbLf & "               ""properties"" :" & vbCr & vbLf & "               {" & vbCr & vbLf & "                ""image"" : ""http://sourceurl.com/catcher-in-the-rye-book-cover.jpg""," & vbCr & vbLf & "                ""name"" : ""The Catcher in the Rye""," & vbCr & vbLf & "                ""bookFormat"" : ""http://schema.org/Paperback""," & vbCr & vbLf & "                ""author"" : ""http://sourceurl.com/author/jd_salinger.html""," & vbCr & vbLf & "                ""numberOfPages"" : 224," & vbCr & vbLf & "                ""publisher"" : ""Little, Brown, and Company""," & vbCr & vbLf & "                ""datePublished"" : ""1991-05-01""," & vbCr & vbLf & "                ""inLanguage"" : ""English""," & vbCr & vbLf & "                ""isbn"" : ""0316769487""" & vbCr & vbLf & "                }" & vbCr & vbLf & "            }"
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Register this page as a share source.
        Me.dataTransferManager = dataTransferManager.GetForCurrentView()
        AddHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ' Unregister this page as a share source.
        RemoveHandler Me.dataTransferManager.DataRequested, AddressOf Me.OnDataRequested
    End Sub

    Private Sub OnDataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        Dim dataPackageTitle As String = TitleInputBox.Text

        ' The title is required.
        If Not String.IsNullOrEmpty(dataPackageTitle) Then
            Dim dataPackageFormat As String = DataFormatInputBox.Text
            If Not String.IsNullOrEmpty(dataPackageFormat) Then
                Dim dataPackageText As String = CustomDataTextBox.Text
                If Not String.IsNullOrEmpty(dataPackageText) Then
                    Dim requestData As DataPackage = e.Request.Data
                    requestData.Properties.Title = dataPackageTitle

                    ' The description is optional.
                    Dim dataPackageDescription As String = DescriptionInputBox.Text
                    If dataPackageDescription IsNot Nothing Then
                        requestData.Properties.Description = dataPackageDescription
                    End If
                    requestData.SetData(dataPackageFormat, dataPackageText)
                Else
                    e.Request.FailWithDisplayText("Enter the custom data you would like to share and try again.")
                End If
            Else
                e.Request.FailWithDisplayText("Enter a custom data format and try again.")
            End If
        Else
            e.Request.FailWithDisplayText(Global.SDKTemplate.MainPage.MissingTitleError)
        End If
    End Sub

    Private Sub ShowUIButton_Click(sender As Object, e As RoutedEventArgs)
        dataTransferManager.ShowShareUI()
    End Sub
End Class
