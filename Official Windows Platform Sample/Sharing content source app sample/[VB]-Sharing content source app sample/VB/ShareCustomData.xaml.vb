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
Imports Microsoft.VisualBasic

Partial Public NotInheritable Class ShareCustomData
    Inherits SDKTemplate.Common.SharePage

    Public Sub New()
        Me.InitializeComponent()
        CustomDataTextBox.Text = "{" & vbCrLf & "               ""type"" : ""http://schema.org/Book""," & vbCrLf & "               ""properties"" :" & vbCrLf & "               {" & vbCrLf & "                ""image"" : ""http://sourceurl.com/catcher-in-the-rye-book-cover.jpg""," & vbCrLf & "                ""name"" : ""The Catcher in the Rye""," & vbCrLf & "                ""bookFormat"" : ""http://schema.org/Paperback""," & vbCrLf & "                ""author"" : ""http://sourceurl.com/author/jd_salinger.html""," & vbCrLf & "                ""numberOfPages"" : 224," & vbCrLf & "                ""publisher"" : ""Little, Brown, and Company""," & vbCrLf & "                ""datePublished"" : ""1991-05-01""," & vbCrLf & "                ""inLanguage"" : ""English""," & vbCrLf & "                ""isbn"" : ""0316769487""" & vbCrLf & "                }" & vbCrLf & "            }"
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        Dim dataPackageFormat As String = DataFormatInputBox.Text
        If Not String.IsNullOrEmpty(dataPackageFormat) Then
            Dim dataPackageText As String = CustomDataTextBox.Text
            If Not String.IsNullOrEmpty(dataPackageText) Then
                Dim requestData As DataPackage = request.Data
                requestData.Properties.Title = TitleInputBox.Text
                requestData.Properties.Description = DescriptionInputBox.Text ' The description is optional.
                requestData.Properties.ContentSourceApplicationLink = ApplicationLink
                requestData.SetData(dataPackageFormat, dataPackageText)
                succeeded = True
            Else
                request.FailWithDisplayText("Enter the custom data you would like to share and try again.")
            End If
        Else
            request.FailWithDisplayText("Enter a custom data format and try again.")
        End If
        Return succeeded
    End Function
End Class
