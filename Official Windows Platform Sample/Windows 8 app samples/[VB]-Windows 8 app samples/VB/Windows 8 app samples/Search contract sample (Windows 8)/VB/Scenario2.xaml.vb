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
Imports Windows.ApplicationModel.Search
Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private searchPane As SearchPane

    Private Shared ReadOnly suggestionList As String() = {"Shanghai", "Istanbul", "Karachi", "Delhi", "Mumbai", "Moscow", _
        "São Paulo", "Seoul", "Beijing", "Jakarta", "Tokyo", "Mexico City", _
        "Kinshasa", "New York City", "Lagos", "London", "Lima", "Bogota", _
        "Tehran", "Ho Chi Minh City", "Hong Kong", "Bangkok", "Dhaka", "Cairo", _
        "Hanoi", "Rio de Janeiro", "Lahore", "Chonquing", "Bengaluru", "Tianjin", _
        "Baghdad", "Riyadh", "Singapore", "Santiago", "Saint Petersburg", "Surat", _
        "Chennai", "Kolkata", "Yangon", "Guangzhou", "Alexandria", "Shenyang", _
        "Hyderabad", "Ahmedabad", "Ankara", "Johannesburg", "Wuhan", "Los Angeles", _
        "Yokohama", "Abidjan", "Busan", "Cape Town", "Durban", "Pune", _
        "Jeddah", "Berlin", "Pyongyang", "Kanpur", "Madrid", "Jaipur", _
        "Nairobi", "Chicago", "Houston", "Philadelphia", "Phoenix", "San Antonio", _
        "San Diego", "Dallas", "San Jose", "Jacksonville", "Indianapolis", "San Francisco", _
        "Austin", "Columbus", "Fort Worth", "Charlotte", "Detroit", "El Paso", _
        "Memphis", "Baltimore", "Boston", "Seattle Washington", "Nashville", "Denver", _
        "Louisville", "Milwaukee", "Portland", "Las Vegas", "Oklahoma City", "Albuquerque", _
        "Tucson", "Fresno", "Sacramento", "Long Beach", "Kansas City", "Mesa", _
        "Virginia Beach", "Atlanta", "Colorado Springs", "Omaha", "Raleigh", "Miami", _
        "Cleveland", "Tulsa", "Oakland", "Minneapolis", "Wichita", "Arlington", _
        " Bakersfield", "New Orleans", "Honolulu", "Anaheim", "Tampa", "Aurora", _
        "Santa Ana", "St. Louis", "Pittsburgh", "Corpus Christi", "Riverside", "Cincinnati", _
        "Lexington", "Anchorage", "Stockton", "Toledo", "St. Paul", "Newark", _
        "Greensboro", "Buffalo", "Plano", "Lincoln", "Henderson", "Fort Wayne", _
        "Jersey City", "St. Petersburg", "Chula Vista", "Norfolk", "Orlando", "Chandler", _
        "Laredo", "Madison", "Winston-Salem", "Lubbock", "Baton Rouge", "Durham", _
        "Garland", "Glendale", "Reno", "Hialeah", "Chesapeake", "Scottsdale", _
        "North Las Vegas", "Irving", "Fremont", "Irvine", "Birmingham", "Rochester", _
        "San Bernardino", "Spokane", "Toronto", "Montreal", "Vancouver", "Ottawa-Gatineau", _
        "Calgary", "Edmonton", "Quebec City", "Winnipeg", "Hamilton"}

    Public Sub New()
        Me.InitializeComponent()
        searchPane = searchPane.GetForCurrentView()
    End Sub

    Private Sub OnSearchPaneSuggestionsRequested(sender As SearchPane, e As SearchPaneSuggestionsRequestedEventArgs)
        Dim queryText = e.QueryText
        If String.IsNullOrEmpty(queryText) Then
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        Else
            Dim request = e.Request
            For Each suggestion As String In suggestionList
                If suggestion.StartsWith(queryText, StringComparison.CurrentCultureIgnoreCase) Then
                    ' Add suggestion to Search Pane
                    request.SearchSuggestionCollection.AppendQuerySuggestion(suggestion)

                    ' Break since the Search Pane can show at most 5 suggestions
                    If request.SearchSuggestionCollection.Size >= MainPage.SearchPaneMaxSuggestions Then
                        Exit For
                    End If
                End If
            Next

            If request.SearchSuggestionCollection.Size > 0 Then
                MainPage.Current.NotifyUser("Suggestions provided for query: " & queryText, NotifyType.StatusMessage)
            Else
                MainPage.Current.NotifyUser("No suggestions provided for query: " & queryText, NotifyType.StatusMessage)
            End If
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
