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
Imports SDKTemplate
Imports Windows.ApplicationModel.DataTransfer

Partial Public NotInheritable Class ShareWebLink
    Inherits SDKTemplate.Common.SharePage

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        ' The URI used in this sample is provided by the user so we need to ensure it's a well formatted absolute URI
        ' before we try to share it.
        rootPage.NotifyUser("", NotifyType.StatusMessage)
        Dim dataPackageUri As Uri = ValidateAndGetUri(UriToShare.Text)
        If dataPackageUri IsNot Nothing Then
            Dim requestData As DataPackage = request.Data
            requestData.Properties.Title = TitleInputBox.Text
            requestData.Properties.Description = DescriptionInputBox.Text ' The description is optional.
            requestData.Properties.ContentSourceApplicationLink = ApplicationLink
            requestData.SetWebLink(dataPackageUri)
            succeeded = True
        Else
            request.FailWithDisplayText("Enter the web link you would like to share and try again.")
        End If
        Return succeeded
    End Function

    Private Function ValidateAndGetUri(ByVal uriString As String) As Uri
        Dim uri As Uri = Nothing
        Try
            uri = New Uri(uriString)
        Catch e1 As FormatException
            Me.rootPage.NotifyUser("The web link provided is not a valid absolute URI.", NotifyType.ErrorMessage)
        End Try
        Return uri
    End Function
End Class
