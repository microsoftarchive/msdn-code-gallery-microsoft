
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

        Public Const FEATURE_NAME As String = "XAML essential controls sample "

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Slider - Introduction", .ClassType = GetType(Global.BasicControls.SliderIntro)},
                                                                   New Scenario With {.Title = "Progress - Introduction", .ClassType = GetType(Global.BasicControls.ProgressIntro)},
                                                                   New Scenario With {.Title = "Buttons", .ClassType = GetType(Global.BasicControls.Buttons)},
                                                                   New Scenario With {.Title = "Text input", .ClassType = GetType(Global.BasicControls.TextInput)},
                                                                   New Scenario With {.Title = "Combo/List boxes", .ClassType = GetType(Global.BasicControls.ComboboxIntro)},
                                                                   New Scenario With {.Title = "Miscellaneous controls", .ClassType = GetType(Global.BasicControls.MiscControls)},
                                                                   New Scenario With {.Title = "Styling a control", .ClassType = GetType(Global.BasicControls.StylingIntro)},
                                                                   New Scenario With {.Title = "Templating a control", .ClassType = GetType(Global.BasicControls.TemplatingIntro)},
                                                                   New Scenario With {.Title = "Visual State Manager", .ClassType = GetType(Global.BasicControls.VisualStateManagerIntro)}
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
