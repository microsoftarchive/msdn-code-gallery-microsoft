
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

        Public Const FEATURE_NAME As String = "XAML user and custom controls sample"
        Public Const EXTENSION_SDK_LINK As String = "http://go.microsoft.com/fwlink/?LinkID=235409"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Using a custom UserControl", .ClassType = GetType(Global.UserAndCustomControls.Scenario1)},
                                                                   New Scenario With {.Title = "Creating a custom Control", .ClassType = GetType(Global.UserAndCustomControls.Scenario2)},
                                                                   New Scenario With {.Title = "DependencyProperty", .ClassType = GetType(Global.UserAndCustomControls.Scenario3)},
                                                                   New Scenario With {.Title = "Distributing your control", .ClassType = GetType(Global.UserAndCustomControls.Scenario4)}
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
