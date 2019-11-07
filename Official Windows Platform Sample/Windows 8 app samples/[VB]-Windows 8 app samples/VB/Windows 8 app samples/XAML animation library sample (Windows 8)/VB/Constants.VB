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

        Public Const FEATURE_NAME As String = "XAML Animations Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Animation overview", .ClassType = GetType(Global.Animation.Scenario1)},
                                                                  New Scenario With {.Title = "Types of animations", .ClassType = GetType(Global.Animation.Scenario2)},
                                                                  New Scenario With {.Title = "Easing functions", .ClassType = GetType(Global.Animation.Scenario3)}
                                                                 }

        Public Event MainPageResized(sender As Object, e As MainPageSizeChangedEventArgs)
        Public AutoSizeInputSectionWhenSnapped = True


        Public Class MainPageSizeChangedEventArgs
            Inherits EventArgs

            Public Property ViewState As ApplicationViewState

        End Class
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
