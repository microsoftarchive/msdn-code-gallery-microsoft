'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.SDKTemplate
    Public Enum NotifyType
        StatusMessage
        ErrorMessage
    End Enum

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

    Friend Class Status
        Friend Shared ReadOnly FileAdded As String = "File added to the basket."
        Friend Shared ReadOnly FileRemoved As String = "File removed from the basket."
        Friend Shared ReadOnly FileAddFailed As String = "Couldn't add file to the basket."
    End Class
End Namespace
