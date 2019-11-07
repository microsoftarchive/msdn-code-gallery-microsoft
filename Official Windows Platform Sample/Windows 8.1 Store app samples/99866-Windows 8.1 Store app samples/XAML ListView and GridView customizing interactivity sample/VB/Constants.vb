'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System.Collections.Generic
Imports System
Imports ListViewInteraction

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Common.LayoutAwarePage

        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML ListView and GridView customizing interactivity"

        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        Private scenarios As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Multi-select storefront", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Creating a master-detail ListView", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "Creating a static ListView", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Creating a 'picker' GridView", .ClassType = GetType(Scenario4)} _
        }
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
