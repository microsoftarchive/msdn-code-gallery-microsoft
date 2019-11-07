Imports Windows.ApplicationModel.Search

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

        Public Const FEATURE_NAME As String = "Search Contract Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Using the Search contract", .ClassType = GetType(SearchContract.Scenario1)},
                                                                    New Scenario With {.Title = "Suggestions from an app-defined list", .ClassType = GetType(SearchContract.Scenario2)},
                                                                    New Scenario With {.Title = "Suggestions in East Asian languages", .ClassType = GetType(SearchContract.Scenario3)},
                                                                    New Scenario With {.Title = "Suggestions provided by Windows", .ClassType = GetType(SearchContract.Scenario4)},
                                                                    New Scenario With {.Title = "Suggestions from Open Search", .ClassType = GetType(SearchContract.Scenario5)},
                                                                    New Scenario With {.Title = "Suggestions from a service returning XML", .ClassType = GetType(SearchContract.Scenario6)}
                                                                   }

        Friend Const SearchPaneMaxSuggestions As Integer = 5

        Friend Sub ProcessQueryText(queryText As String)
            NotifyUser("Query submitted: " & queryText, NotifyType.StatusMessage)
        End Sub

        Friend Sub OnQuerySubmitted(sender As Object, args As SearchPaneQuerySubmittedEventArgs)
            ProcessQueryText(args.QueryText)
        End Sub
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

        Public Overrides Function Tostring() As String
            Return Title
        End Function
    End Class
End Namespace

