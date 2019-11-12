'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports BasicMediaPlayback

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public SystemMediaTransportControlsInitialized As Boolean = False

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML Media Playback Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Playing a media file", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Play To devices", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "Using markers", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Custom transport controls", .ClassType = GetType(Scenario4)}, _
            New Scenario() With {.Title = "Playing multiple files", .ClassType = GetType(Scenario5)}, _
            New Scenario() With {.Title = "Playing background media", .ClassType = GetType(Scenario6)} _
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
