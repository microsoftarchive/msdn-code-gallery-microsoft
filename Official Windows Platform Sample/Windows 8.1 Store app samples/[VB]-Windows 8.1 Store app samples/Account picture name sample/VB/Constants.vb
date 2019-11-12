'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports AccountPictureName
Imports System
Imports System.Collections.Generic
Imports Windows.UI.ViewManagement

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Account Picture Name VB"

        Private scenarios As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Get user's display name", .ClassType = GetType(GetUserDisplayName)}, _
            New Scenario() With {.Title = "Get user's first and last name", .ClassType = GetType(GetUserFirstLastName)}, _
            New Scenario() With {.Title = "Get user's Account Picture", .ClassType = GetType(GetAccountPicture)}, _
            New Scenario() With {.Title = "Set user's Account Picture and Listen for changes", .ClassType = GetType(SetAccountPictureAndListen)} _
        }


        ' Navigates to the Scenario "Set Account Picture and listen"
        Public Sub NavigateToSetAccountPictureAndListen()
            Dim index As Integer = -1
            ' Populate the ListBox with the list of scenarios as defined in Constants.vb.
            For Each s As Scenario In scenarios
                index += 1
                If s.ClassType Is GetType(SetAccountPictureAndListen) Then
                    Exit For
                End If
            Next s
            SuspensionManager.SessionState("SelectedScenario") = index
            ScenariosList.SelectedIndex = index
            LoadScenario(scenarios(index).ClassType)
            InvalidateSize()
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
