'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML vector-based drawing sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Shapes", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Strokes and fills", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "Gradients", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Clipping", .ClassType = GetType(Scenario4)}, _
            New Scenario() With {.Title = "Paths", .ClassType = GetType(Scenario5)} _
        }

        ' Converts between ComboBox indices and colors
        Public Function ConvertIndexToColor(ByVal index As Integer) As Color
            Select Case index
                Case 0
                    Return Colors.Red
                Case 1
                    Return Colors.Green
                Case 2
                    Return Colors.Blue
                Case 3
                    Return Colors.Gray
                Case Else
                    Return Colors.White
            End Select
        End Function
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
