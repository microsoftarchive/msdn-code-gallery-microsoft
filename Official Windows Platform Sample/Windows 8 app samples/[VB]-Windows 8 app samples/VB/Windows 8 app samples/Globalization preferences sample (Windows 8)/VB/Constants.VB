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
        Public Const FEATURE_NAME As String = "Globalization Preferences Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "User Preferences", .ClassType = GetType(Global.GlobalizationPreferencesSample.UserPreferences)},
                                                                   New Scenario With {.Title = "Language characteristics", .ClassType = GetType(Global.GlobalizationPreferencesSample.LanguageScenario)},
                                                                   New Scenario With {.Title = "Geographic Region characteristics", .ClassType = GetType(Global.GlobalizationPreferencesSample.GeographicRegionScenario)},
                                                                   New Scenario With {.Title = "Current input language", .ClassType = GetType(Global.GlobalizationPreferencesSample.CurrentInputLanguage)}
                                                                  }
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

        Public Overrides Function ToString As String
            Return Title
        End Function
    End Class
End Namespace

