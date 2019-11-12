'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate
Imports System
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Search
Imports Windows.Data.Json
Imports Windows.Foundation
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports System.Collections.Generic

Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private searchPane As SearchPane

    Private Shared ReadOnly exampleResponse As String = "jsonSuggestionService\exampleJsonResponse.json"

    Public Sub New()
        Me.InitializeComponent()
        searchPane = searchPane.GetForCurrentView()
    End Sub

    Private Async Sub OnSearchPaneSuggestionsRequested(sender As SearchPane, e As SearchPaneSuggestionsRequestedEventArgs)
        Dim queryText = e.QueryText
        If String.IsNullOrEmpty(queryText) Then
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        Else
            Dim request = e.Request
            Dim deferral = request.GetDeferral()

            Try
                Dim file As StorageFile = Await Package.Current.InstalledLocation.GetFileAsync(exampleResponse)
                Dim response As String = Await FileIO.ReadTextAsync(file)
                Dim parsedResponse As JsonArray = JsonArray.Parse(response)
                If parsedResponse.Count > 1 Then
                    parsedResponse = JsonArray.Parse(parsedResponse(1).Stringify())
                    For Each value As JsonValue In parsedResponse
                        request.SearchSuggestionCollection.AppendQuerySuggestion(value.GetString())
                        If request.SearchSuggestionCollection.Size >= MainPage.SearchPaneMaxSuggestions Then
                            Exit For
                        End If
                    Next
                End If

                If request.SearchSuggestionCollection.Size > 0 Then
                    MainPage.Current.NotifyUser("Suggestions provided for query: " & queryText, NotifyType.StatusMessage)
                Else
                    MainPage.Current.NotifyUser("No suggestions provided for query: " & queryText, NotifyType.StatusMessage)
                End If
            Catch ex As Exception
                MainPage.Current.NotifyUser("Suggestions could not be displayed, please verify that the service provides valid OpenSearch suggestions", NotifyType.ErrorMessage)
            Finally
                deferral.Complete()
            End Try
        End If
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        AddHandler searchPane.SuggestionsRequested, AddressOf OnSearchPaneSuggestionsRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler searchPane.SuggestionsRequested, AddressOf OnSearchPaneSuggestionsRequested
    End Sub
End Class
