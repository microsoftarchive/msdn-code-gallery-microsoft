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
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Message Dialog VB Sample"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Use custom commands", .ClassType = GetType(MessageDialogSample.CustomCommand)}, _
            New Scenario() With {.Title = "Use default close command", .ClassType = GetType(MessageDialogSample.DefaultCloseCommand)}, _
            New Scenario() With {.Title = "Use completed callback", .ClassType = GetType(MessageDialogSample.CompletedCallback)}, _
            New Scenario() With {.Title = "Use cancel command", .ClassType = GetType(MessageDialogSample.CancelCommand)} _
        }
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
