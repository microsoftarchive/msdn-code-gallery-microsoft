
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
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Share Source Sample "

        Public Const MissingTitleError As String = "Enter a title for what you are sharing and try again."

        Public Shared _scenarioList As New List(Of Scenario)() From {New Scenario() With {.Title = "Share text", .ClassType = GetType(Global.ShareSource.ShareText)},
                                                                     New Scenario() With {.Title = "Share link", .ClassType = GetType(Global.ShareSource.ShareLink)},
                                                                     New Scenario() With {.Title = "Share image", .ClassType = GetType(Global.ShareSource.ShareImage)},
                                                                     New Scenario() With {.Title = "Share files", .ClassType = GetType(Global.ShareSource.ShareFiles)},
                                                                     New Scenario() With {.Title = "Share delay rendered files", .ClassType = GetType(Global.ShareSource.ShareDelayRenderedFiles)},
                                                                     New Scenario() With {.Title = "Share HTML content", .ClassType = GetType(Global.ShareSource.ShareHtml)},
                                                                     New Scenario() With {.Title = "Share custom data", .ClassType = GetType(Global.ShareSource.ShareCustomData)},
                                                                     New Scenario() With {.Title = "Fail with display text", .ClassType = GetType(Global.ShareSource.SetErrorMessage)}
                                                                    }

        

    End Class

    Public Class Scenario
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType As Type
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
