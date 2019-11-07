'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.Generic
Imports System

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage
        Public Const FEATURE_NAME As String = "Clipboard Sample"

        Public isClipboardContentChangeChecked As Boolean = False
        Public needToPrintClipboardFormat As Boolean = False

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Copy and paste text", .ClassType = GetType(Global.Clipboard.CopyText)},
                                                         New Scenario With {.Title = "Copy and paste an image", .ClassType = GetType(Global.Clipboard.CopyImage)},
                                                         New Scenario With {.Title = "Copy and paste files", .ClassType = GetType(Global.Clipboard.CopyFile)},
                                                         New Scenario With {.Title = "Other Clipboard operations", .ClassType = GetType(Global.Clipboard.OtherScenarios)}}
    End Class

    Public Class Scenario
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType() As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

