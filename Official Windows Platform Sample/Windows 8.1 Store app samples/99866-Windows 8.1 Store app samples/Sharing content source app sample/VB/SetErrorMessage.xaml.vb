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

Partial Public NotInheritable Class SetErrorMessage
    Inherits SharePage

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim errorMessage As String = CustomErrorText.Text
        If String.IsNullOrEmpty(errorMessage) Then
            errorMessage = "Enter a failure display text and try again."
        End If
        request.FailWithDisplayText(errorMessage)
        Return False
    End Function
End Class
