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
Imports System.Collections.Generic

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "SMS Send/Receive Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Sending a text message", .ClassType = GetType(SmsSendReceive.SendMessage)}, _
            New Scenario() With {.Title = "Receiving a text message", .ClassType = GetType(SmsSendReceive.ReceiveMessage)}, _
            New Scenario() With {.Title = "Sending a message in PDU format", .ClassType = GetType(SmsSendReceive.SendPduMessage)}, _
            New Scenario() With {.Title = "Reading a message from SIM", .ClassType = GetType(SmsSendReceive.ReadMessage)}, _
            New Scenario() With {.Title = "Deleting a message from SIM", .ClassType = GetType(SmsSendReceive.DeleteMessage)} _
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
