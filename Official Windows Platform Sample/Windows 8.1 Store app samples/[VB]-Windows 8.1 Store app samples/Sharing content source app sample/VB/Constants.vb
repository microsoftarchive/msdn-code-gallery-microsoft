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
Imports Windows.UI.ViewManagement
Imports SDKTemplate.Common
Imports ShareSource

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Share Source VB"
        Public Const MissingTitleError As String = "Enter a title for what you are sharing and try again."

        Public Shared scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Share text", .ClassType = GetType(ShareText)}, _
            New Scenario() With {.Title = "Share web link", .ClassType = GetType(ShareWebLink)}, _
            New Scenario() With {.Title = "Share application link", .ClassType = GetType(ShareApplicationLink)}, _
            New Scenario() With {.Title = "Share image", .ClassType = GetType(ShareImage)}, _
            New Scenario() With {.Title = "Share files", .ClassType = GetType(ShareFiles)}, _
            New Scenario() With {.Title = "Share delay rendered files", .ClassType = GetType(ShareDelayRenderedFiles)}, _
            New Scenario() With {.Title = "Share HTML content", .ClassType = GetType(ShareHtml)}, _
            New Scenario() With {.Title = "Share custom data", .ClassType = GetType(ShareCustomData)}, _
            New Scenario() With {.Title = "Fail with display text", .ClassType = GetType(SetErrorMessage)} _
        }
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public ReadOnly Property ApplicationLink() As Uri
            Get
                Return SharePage.GetApplicationLink(ClassType.Name)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
