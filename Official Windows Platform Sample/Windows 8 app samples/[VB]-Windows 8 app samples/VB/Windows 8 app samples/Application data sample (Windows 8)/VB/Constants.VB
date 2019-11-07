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

        Public Const FEATURE_NAME As String = "Application Data Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Files", .ClassType = GetType(Global.ApplicationDataSample.Files)},
                                                               New Scenario With {.Title = "Settings", .ClassType = GetType(Global.ApplicationDataSample.Settings)},
                                                               New Scenario With {.Title = "Setting Containers", .ClassType = GetType(Global.ApplicationDataSample.SettingContainer)},
                                                               New Scenario With {.Title = "Composite Settings", .ClassType = GetType(Global.ApplicationDataSample.CompositeSettings)},
                                                               New Scenario With {.Title = "DataChanged Event", .ClassType = GetType(Global.ApplicationDataSample.DataChangedEvent)},
                                                               New Scenario With {.Title = "Roaming: HighPriority", .ClassType = GetType(Global.ApplicationDataSample.HighPriority)},
                                                               New Scenario With {.Title = "ms-appdata:// Protocol", .ClassType = GetType(Global.ApplicationDataSample.Msappdata)},
                                                               New Scenario With {.Title = "Clear", .ClassType = GetType(Global.ApplicationDataSample.ClearScenario)},
                                                               New Scenario With {.Title = "SetVersion", .ClassType = GetType(Global.ApplicationDataSample.SetVersion)}
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

