
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

        Public Const FEATURE_NAME As String = "Rotation Sample "

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Clear Preferences", .ClassType = GetType(Global.Rotation.None)},
                                                                   New Scenario With {.Title = "Landscape Only", .ClassType = GetType(Global.Rotation.Landscape)},
                                                                   New Scenario With {.Title = "Portrait Only", .ClassType = GetType(Global.Rotation.Portrait)},
                                                                   New Scenario With {.Title = "Landscape Flipped Only", .ClassType = GetType(Global.Rotation.LandscapeFlipped)},
                                                                   New Scenario With {.Title = "Portrait Flipped Only", .ClassType = GetType(Global.Rotation.PortraitFlipped)}}

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
