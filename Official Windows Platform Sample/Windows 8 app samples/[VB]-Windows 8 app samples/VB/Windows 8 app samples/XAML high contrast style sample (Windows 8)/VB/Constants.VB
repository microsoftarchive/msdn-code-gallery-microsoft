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

        Public Const FEATURE_NAME As String = "XAML high contrast style sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "High contrast via XAML", .ClassType = GetType(Global.HighContrast.Scenario1)},
                                                                  New Scenario With {.Title = "High contrast using system colors via XAML", .ClassType = GetType(Global.HighContrast.Scenario2)},
                                                                  New Scenario With {.Title = "High contrast using system colors via code", .ClassType = GetType(Global.HighContrast.Scenario3)}
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
