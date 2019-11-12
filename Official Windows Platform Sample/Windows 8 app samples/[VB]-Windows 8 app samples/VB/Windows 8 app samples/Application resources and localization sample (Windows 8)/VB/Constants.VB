
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

        Public Const FEATURE_NAME As String = "Application Resources Sample "

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Strings Resources In XAML", .ClassType = GetType(Global.ApplicationResources.Scenario1)},
                                                                   New Scenario With {.Title = "File Resources In XAML", .ClassType = GetType(Global.ApplicationResources.Scenario2)},
                                                                   New Scenario With {.Title = "Strings Resources In Code", .ClassType = GetType(Global.ApplicationResources.Scenario3)},
                                                                   New Scenario With {.Title = "Resources in AppX manifest", .ClassType = GetType(Global.ApplicationResources.Scenario4)},
                                                                   New Scenario With {.Title = "Additional Resource Files", .ClassType = GetType(Global.ApplicationResources.Scenario5)},
                                                                   New Scenario With {.Title = "Class Library Resources", .ClassType = GetType(Global.ApplicationResources.Scenario6)},
                                                                   New Scenario With {.Title = "Runtime Changes/Events", .ClassType = GetType(Global.ApplicationResources.Scenario7)},
                                                                   New Scenario With {.Title = "Application Languages", .ClassType = GetType(Global.ApplicationResources.Scenario8)},
                                                                   New Scenario With {.Title = "Override Languages", .ClassType = GetType(Global.ApplicationResources.Scenario9)},
                                                                   New Scenario With {.Title = "Multi-dimensional fallback", .ClassType = GetType(Global.ApplicationResources.Scenario10)},
                                                                   New Scenario With {.Title = "Working with webservices", .ClassType = GetType(Global.ApplicationResources.Scenario11)}
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
