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
Imports Windows.ApplicationModel.DataTransfer

Partial Public NotInheritable Class ShareText
    Inherits SDKTemplate.Common.SharePage

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        Dim dataPackageText As String = TextToShare.Text
        If Not String.IsNullOrEmpty(dataPackageText) Then
            Dim requestData As DataPackage = request.Data
            requestData.Properties.Title = TitleInputBox.Text
            requestData.Properties.Description = DescriptionInputBox.Text ' The description is optional.
            requestData.Properties.ContentSourceApplicationLink = ApplicationLink
            requestData.SetText(dataPackageText)
            succeeded = True
        Else
            request.FailWithDisplayText("Enter the text you would like to share and try again.")
        End If
        Return succeeded
    End Function
End Class

