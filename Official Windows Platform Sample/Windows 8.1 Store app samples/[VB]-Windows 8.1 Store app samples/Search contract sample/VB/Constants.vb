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

        Public Const FEATURE_NAME As String = "Search Contract Sample (VB)"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Using the Search contract", .ClassType = GetType(SearchContract.Scenario1)}, _
            New Scenario() With {.Title = "Suggestions from an app-defined list", .ClassType = GetType(SearchContract.Scenario2)}, _
            New Scenario() With {.Title = "Suggestions in East Asian languages", .ClassType = GetType(SearchContract.Scenario3)}, _
            New Scenario() With {.Title = "Suggestions provided by Windows", .ClassType = GetType(SearchContract.Scenario4)}, _
            New Scenario() With {.Title = "Suggestions from Open Search", .ClassType = GetType(SearchContract.Scenario5)}, _
            New Scenario() With {.Title = "Suggestions from a service returning XML", .ClassType = GetType(SearchContract.Scenario6)}, _
            New Scenario() With {.Title = "Open Search charm by typing", .ClassType = GetType(SearchContract.Scenario7)} _
        }

        Friend Const SearchPaneMaxSuggestions As Integer = 25

        Friend Sub ProcessQueryText(ByVal queryText As String)
            NotifyUser("Query submitted: " & queryText, NotifyType.StatusMessage)
        End Sub
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
