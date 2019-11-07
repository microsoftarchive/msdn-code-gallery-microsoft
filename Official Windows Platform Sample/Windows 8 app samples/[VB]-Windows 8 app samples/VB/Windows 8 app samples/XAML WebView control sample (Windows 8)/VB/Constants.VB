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
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML WebView control sample"

        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.


        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Navigate to a URL", .ClassType = GetType(Global.Controls_WebView.Scenario1)},
                                                             New Scenario With {.Title = "Load HTML", .ClassType = GetType(Global.Controls_WebView.Scenario2)},
                                                             New Scenario With {.Title = "Interact with script", .ClassType = GetType(Global.Controls_WebView.Scenario3)},
                                                             New Scenario With {.Title = "Using ScriptNotify", .ClassType = GetType(Global.Controls_WebView.Scenario4)},
                                                             New Scenario With {.Title = "Accessing the DOM", .ClassType = GetType(Global.Controls_WebView.Scenario5)},
                                                             New Scenario With {.Title = "Using WebViewBrush", .ClassType = GetType(Global.Controls_WebView.Scenario6)},
                                                             New Scenario With {.Title = "Supporting the Share contract", .ClassType = GetType(Global.Controls_WebView.Scenario7)},
                                                             New Scenario With {.Title = "Co-existing with the AppBar", .ClassType = GetType(Global.Controls_WebView.Scenario8)}
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

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

End Namespace
