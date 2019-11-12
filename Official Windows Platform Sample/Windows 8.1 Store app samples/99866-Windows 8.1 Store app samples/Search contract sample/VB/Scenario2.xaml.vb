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
Imports Windows.ApplicationModel.Search

Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private searchPane As SearchPane

    Private Shared ReadOnly suggestionList() As String = {"Shanghai", "Istanbul", "Karachi", "Delhi", "Mumbai", "Moscow", "São Paulo", "Seoul", "Beijing", "Jakarta", "Tokyo", "Mexico City", "Kinshasa", "New York City", "Lagos", "London", "Lima", "Bogota", "Tehran", "Ho Chi Minh City", "Hong Kong", "Bangkok", "Dhaka", "Cairo", "Hanoi", "Rio de Janeiro", "Lahore", "Chonquing", "Bengaluru", "Tianjin", "Baghdad", "Riyadh", "Singapore", "Santiago", "Saint Petersburg", "Surat", "Chennai", "Kolkata", "Yangon", "Guangzhou", "Alexandria", "Shenyang", "Hyderabad", "Ahmedabad", "Ankara", "Johannesburg", "Wuhan", "Los Angeles", "Yokohama", "Abidjan", "Busan", "Cape Town", "Durban", "Pune", "Jeddah", "Berlin", "Pyongyang", "Kanpur", "Madrid", "Jaipur", "Nairobi", "Chicago", "Houston", "Philadelphia", "Phoenix", "San Antonio", "San Diego", "Dallas", "San Jose", "Jacksonville", "Indianapolis", "San Francisco", "Austin", "Columbus", "Fort Worth", "Charlotte", "Detroit", "El Paso", "Memphis", "Baltimore", "Boston", "Seattle Washington", "Nashville", "Denver", "Louisville", "Milwaukee", "Portland", "Las Vegas", "Oklahoma City", "Albuquerque", "Tucson", "Fresno", "Sacramento", "Long Beach", "Kansas City", "Mesa", "Virginia Beach", "Atlanta", "Colorado Springs", "Omaha", "Raleigh", "Miami", "Cleveland", "Tulsa", "Oakland", "Minneapolis", "Wichita", "Arlington", " Bakersfield", "New Orleans", "Honolulu", "Anaheim", "Tampa", "Aurora", "Santa Ana", "St. Louis", "Pittsburgh", "Corpus Christi", "Riverside", "Cincinnati", "Lexington", "Anchorage", "Stockton", "Toledo", "St. Paul", "Newark", "Greensboro", "Buffalo", "Plano", "Lincoln", "Henderson", "Fort Wayne", "Jersey City", "St. Petersburg", "Chula Vista", "Norfolk", "Orlando", "Chandler", "Laredo", "Madison", "Winston-Salem", "Lubbock", "Baton Rouge", "Durham", "Garland", "Glendale", "Reno", "Hialeah", "Chesapeake", "Scottsdale", "North Las Vegas", "Irving", "Fremont", "Irvine", "Birmingham", "Rochester", "San Bernardino", "Spokane", "Toronto", "Montreal", "Vancouver", "Ottawa-Gatineau", "Calgary", "Edmonton", "Quebec City", "Winnipeg", "Hamilton"}

    Public Sub New()
        Me.InitializeComponent()
        searchPane = searchPane.GetForCurrentView()
    End Sub

    Private Sub OnSearchPaneSuggestionsRequested(ByVal sender As SearchPane, ByVal e As SearchPaneSuggestionsRequestedEventArgs)
        Dim queryText = e.QueryText
        If String.IsNullOrEmpty(queryText) Then
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        Else
            Dim request = e.Request
            For Each suggestion As String In suggestionList
                If suggestion.StartsWith(queryText, StringComparison.CurrentCultureIgnoreCase) Then
                    ' Add suggestion to Search Pane
                    request.SearchSuggestionCollection.AppendQuerySuggestion(suggestion)

                    ' Break since the Search Pane can show at most 25 suggestions
                    If request.SearchSuggestionCollection.Size >= MainPage.SearchPaneMaxSuggestions Then
                        Exit For
                    End If
                End If
            Next suggestion

            If request.SearchSuggestionCollection.Size > 0 Then
                MainPage.Current.NotifyUser("Suggestions provided for query: " & queryText, NotifyType.StatusMessage)
            Else
                MainPage.Current.NotifyUser("No suggestions provided for query: " & queryText, NotifyType.StatusMessage)
            End If
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
