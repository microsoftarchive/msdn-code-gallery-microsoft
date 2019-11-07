'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports ApplicationResources

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Application Resources VB sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "String Resources In XAML", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "File Resources In XAML", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "String Resources In Code", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Resources in the AppX manifest", .ClassType = GetType(Scenario4)}, _
            New Scenario() With {.Title = "Additional Resource Files", .ClassType = GetType(Scenario5)}, _
            New Scenario() With {.Title = "Class Library Resources", .ClassType = GetType(Scenario6)}, _
            New Scenario() With {.Title = "Runtime Changes/Events", .ClassType = GetType(Scenario7)}, _
            New Scenario() With {.Title = "Application Languages", .ClassType = GetType(Scenario8)}, _
            New Scenario() With {.Title = "Override Languages", .ClassType = GetType(Scenario9)}, _
            New Scenario() With {.Title = "Multi-dimensional fallback", .ClassType = GetType(Scenario10)}, _
            New Scenario() With {.Title = "Working with webservices", .ClassType = GetType(Scenario11)}, _
            New Scenario() With {.Title = "Retrieving resources in non-UI threads", .ClassType = GetType(Scenario12)}, _
            New Scenario() With {.Title = "File resources in code", .ClassType = GetType(Scenario13)} _
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
