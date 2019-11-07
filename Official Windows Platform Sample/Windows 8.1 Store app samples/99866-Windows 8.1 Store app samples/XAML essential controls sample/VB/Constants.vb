'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports BasicControls

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Basic controls sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Slider - Introduction", .ClassType = GetType(SliderIntro)}, _
            New Scenario() With {.Title = "Progress - Introduction", .ClassType = GetType(ProgressIntro)}, _
            New Scenario() With {.Title = "Buttons", .ClassType = GetType(Buttons)}, _
            New Scenario() With {.Title = "Text input", .ClassType = GetType(TextInput)}, _
            New Scenario() With {.Title = "Combo/List boxes", .ClassType = GetType(ComboboxIntro)}, _
            New Scenario() With {.Title = "Miscellaneous controls", .ClassType = GetType(MiscControls)}, _
            New Scenario() With {.Title = "Styling a control", .ClassType = GetType(StylingIntro)}, _
            New Scenario() With {.Title = "Templating a control", .ClassType = GetType(TemplatingIntro)}, _
            New Scenario() With {.Title = "Visual State Manager", .ClassType = GetType(VisualStateManagerIntro)} _
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
