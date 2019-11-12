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

Partial Public NotInheritable Class ShareHtml
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private rootPage As MainPage = MainPage.Current
    Private dataTransferManager As DataTransferManager
    Private Const htmlContent As String = "<html><body><div id=""htmlFragment"" style=""width: 500px;background-color:#f2f2f2;padding: 1px 15px 20px 15px;margin-bottom:40px; font-family:'Segoe UI Semilight'"">" & vbCr & vbLf & "                            <h2 style=""margin-bottom:10px""><img id=""htmlFragmentImage"" src=""http://build.blob.core.windows.net/media/Default/home/dev-center_branding.png"" alt=""Windows Dev Center"" /></h2>" & vbCr & vbLf & "                            <p style=""margin-top:0"">Go to the new <a href=""http://msdn.microsoft.com/en-us/windows/home/"">Windows Dev Center</a> to get the Windows 8 Developer Preview, dev tools, samples, forums, docs and other resources to start building on Windows 8 now.</p>" & vbCr & vbLf & vbCr & vbLf & "                            <a href=""http://msdn.microsoft.com/en-us/windows/apps/br229516"" style=""padding-left:20px;""><span style=""font-size:14px"">Downloads</span></a>" & vbCr & vbLf & "                            <span style=""padding:0 20px;color:#585858;font-size:14px"">|</span>" & vbCr & vbLf & "                            <a href=""http://msdn.microsoft.com/library/windows/apps/br211386""><span style=""font-size:14px"">Getting started</span></a>" & vbCr & vbLf & "                            <span style=""padding:0 20px;color:#585858;font-size:14px"">|</span>" & vbCr & vbLf & "                            <a href=""http://code.msdn.microsoft.com/en-us/windowsapps""><span style=""font-size:14px"">App samples</span></a>" & vbCr & vbLf & "                            <span style=""padding:0 20px;color:#585858;font-size:14px"">|</span>" & vbCr & vbLf & "                            <a href=""http://social.msdn.microsoft.com/Forums/en-us/category/windowsapps""><span style=""font-size:14px"">Forums</span></a>" & vbCr & vbLf & "                        </div></body></html>"

    Public Sub New()
        Me.InitializeComponent()
        ShareWebView.NavigateToString(htmlContent)
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
        ' Get the user's selection from the WebView.
        Dim requestData As DataPackage = ShareWebView.DataTransferPackage
        Dim dataPackageView As DataPackageView = requestData.GetView()

        If (dataPackageView IsNot Nothing) Then
            If (dataPackageView.AvailableFormats.Count > 0) Then
                Dim dataPackageTitle As String = TitleInputBox.Text

                ' The title is required.
                If Not String.IsNullOrEmpty(dataPackageTitle) Then
                    requestData.Properties.Title = dataPackageTitle

                    ' The description is optional.
                    Dim dataPackageDescription As String = DescriptionInputBox.Text
                    If dataPackageDescription IsNot Nothing Then
                        requestData.Properties.Description = dataPackageDescription
                    End If
                    e.Request.Data = requestData
                Else
                    e.Request.FailWithDisplayText(MainPage.MissingTitleError)
                End If
            End If

        Else
            e.Request.FailWithDisplayText("Make a selection in the HTML fragment and try again.")
        End If
    End Sub

    Private Sub ShowUIButton_Click(sender As Object, e As RoutedEventArgs)
        dataTransferManager.ShowShareUI()
    End Sub
End Class
