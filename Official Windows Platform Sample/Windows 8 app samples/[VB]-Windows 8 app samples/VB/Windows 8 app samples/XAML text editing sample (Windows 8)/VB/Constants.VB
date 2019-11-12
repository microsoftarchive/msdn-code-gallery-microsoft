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

        Public Const FEATURE_NAME As String = "XAML text editing sample"


        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Windows 8 text selection", .ClassType = GetType(Global.TextEditing.Scenario1)},
                                                                   New Scenario With {.Title = "Accessing text selection", .ClassType = GetType(Global.TextEditing.Scenario2)},
                                                                   New Scenario With {.Title = "Spell checking", .ClassType = GetType(Global.TextEditing.Scenario3)},
                                                                   New Scenario With {.Title = "Text prediction", .ClassType = GetType(Global.TextEditing.Scenario4)},
                                                                   New Scenario With {.Title = "Input scopes", .ClassType = GetType(Global.TextEditing.Scenario5)},
                                                                   New Scenario With {.Title = "RichEditBox and WinRT TOM", .ClassType = GetType(Global.TextEditing.Scenario6)},
                                                                   New Scenario With {.Title = "PasswordBox", .ClassType = GetType(Global.TextEditing.Scenario7)}}

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
