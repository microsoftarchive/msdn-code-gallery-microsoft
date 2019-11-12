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
Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports Windows.ApplicationModel.Search
Imports Windows.Data.Json

Imports System.Collections.Generic

Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    Private searchPane As SearchPane
    Private httpClient As HttpClient
    Private currentHttpTask As Task(Of String) = Nothing

    Public Sub New()
        Me.InitializeComponent()
        searchPane = searchPane.GetForCurrentView()
        httpClient = New HttpClient()
    End Sub

    Protected Overrides Sub Finalize()
        Dispose()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If httpClient IsNot Nothing Then
            httpClient.Dispose()
            httpClient = Nothing
        End If
    End Sub

    Private Async Function GetSuggestionsAsync(ByVal str As String, ByVal suggestions As SearchSuggestionCollection) As Task
        ' Cancel the previous suggestion request if it is not finished.
        If currentHttpTask IsNot Nothing Then
            currentHttpTask.AsAsyncOperation().Cancel()
        End If

        ' Get the suggestions from an open search service.
        currentHttpTask = httpClient.GetStringAsync(str)
        Dim response As String = Await currentHttpTask
        Dim parsedResponse As JsonArray = JsonArray.Parse(response)
        If parsedResponse.Count > 1 Then
            For Each value As JsonValue In parsedResponse(1).GetArray()
                suggestions.AppendQuerySuggestion(value.GetString())
                If suggestions.Size >= MainPage.SearchPaneMaxSuggestions Then
                    Exit For
                End If
            Next value
        End If
    End Function

    Private Async Sub OnSearchPaneSuggestionsRequested(ByVal sender As SearchPane, ByVal e As SearchPaneSuggestionsRequestedEventArgs)
        Dim queryText = e.QueryText
        If String.IsNullOrEmpty(queryText) Then
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        ElseIf String.IsNullOrEmpty(UrlTextBox.Text) Then
            MainPage.Current.NotifyUser("Please enter the web service URL", NotifyType.StatusMessage)
        Else
            ' The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
            Dim request = e.Request
            Dim deferral = request.GetDeferral()

            Try
                ' Use the web service Url entered in the UrlTextBox that supports OpenSearch Suggestions in order to see suggestions come from the web service.
                ' See http://www.opensearch.org/Specifications/OpenSearch/Extensions/Suggestions/1.0 for details on OpenSearch Suggestions format.
                ' Replace "{searchTerms}" of the Url with the query string.
                Dim task As Task = GetSuggestionsAsync(Regex.Replace(UrlTextBox.Text, "{searchTerms}", Uri.EscapeDataString(queryText)), request.SearchSuggestionCollection)
                Await task

                If task.Status = TaskStatus.RanToCompletion Then
                    If request.SearchSuggestionCollection.Size > 0 Then
                        MainPage.Current.NotifyUser("Suggestions provided for query: " & queryText, NotifyType.StatusMessage)
                    Else
                        MainPage.Current.NotifyUser("No suggestions provided for query: " & queryText, NotifyType.StatusMessage)
                    End If
                End If
            Catch e1 As TaskCanceledException
                ' We have canceled the task.
            Catch e2 As FormatException
                MainPage.Current.NotifyUser("Suggestions could not be retrieved, please verify that the URL points to a valid service (for example http://contoso.com?q={searchTerms}", NotifyType.ErrorMessage)
            Catch e3 As Exception
                MainPage.Current.NotifyUser("Suggestions could not be displayed, please verify that the service provides valid OpenSearch suggestions", NotifyType.ErrorMessage)
            Finally
                deferral.Complete()
            End Try
        End If
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        ' This event should be registered when your app first creates its main window after receiving an activated event, like OnLaunched, OnSearchActivated.
        ' Typically this occurs in App.xaml.vb.
        AddHandler searchPane.SuggestionsRequested, AddressOf OnSearchPaneSuggestionsRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler searchPane.SuggestionsRequested, AddressOf OnSearchPaneSuggestionsRequested
    End Sub
End Class
