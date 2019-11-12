'***************************** Module Header ******************************\
' Module Name:	Default.aspx.vb
' Project:		VBAzureMarketPlaceBingSearch
' Copyright (c) Microsoft Corporation.
' 
' 
' Default page.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Net

Public Class _Default
    Inherits System.Web.UI.Page

    ' Create a Bing container.
    Private Const rootUrl As String = "https://api.datamarket.azure.com/Bing/Search"

    'TODO:Change this account key to yours.
    'Example:
    'AgiyQkKH0B/1OTwW/zXu3hGNc2mU2OGintltk1IqajY=
    Private Const AccountKey As String = "[Account key]"

    Private market As String = "en-us"


    ''' <summary>
    ''' Search for web only.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnWebSearch_Click(sender As Object, e As EventArgs) Handles btnWebSearch.Click
        Dim rptResult As New Repeater()

        ' This is the query expression.
        Dim query As String = tbQueryString.Text
        Dim bingContainer = New Bing.BingSearchContainer(New Uri(rootUrl))

        ' Configure bingContainer to use your credentials.
        bingContainer.Credentials = New NetworkCredential(AccountKey, AccountKey)

        ' Build the query, limiting to 10 results.
        Dim webQuery = bingContainer.Web(query, Nothing, Nothing, market, Nothing, Nothing, _
         Nothing, Nothing)
        webQuery = webQuery.AddQueryOption("$top", 10)

        ' Run the query and display the results.
        Dim webResults = webQuery.Execute()
        Dim lblResults As New Label()
        Dim searchResult As New StringBuilder()

        For Each wResult As Bing.WebResult In webResults

            searchResult.Append(String.Format("<a href={2}>{0}</a><br /> {1}<br /> {2}<br /><br />", wResult.Title, wResult.Url, wResult.Description))
        Next
        lblResults.Text = searchResult.ToString()
        Panel1.Controls.Add(lblResults)

    End Sub

    ''' <summary>
    ''' Search for image only.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnImageSearch_Click(sender As Object, e As EventArgs) Handles btnImageSearch.Click
        Dim rptResult As New Repeater()
        Dim query As String = tbQueryString.Text

        ' Create a Bing container.
        Dim rootUrl As String = "https://api.datamarket.azure.com/Bing/Search"
        Dim bingContainer = New Bing.BingSearchContainer(New Uri(rootUrl))

        ' Configure bingContainer to use your credentials.
        bingContainer.Credentials = New NetworkCredential(AccountKey, AccountKey)

        ' Build the query, limiting to 10 results.
        Dim imageQuery = bingContainer.Image(query, Nothing, market, Nothing, Nothing, Nothing, _
         Nothing)
        imageQuery = imageQuery.AddQueryOption("$top", 50)
        ' Run the query and display the results.
        Dim imageResults = imageQuery.Execute()
        Dim searchResult As New StringBuilder()
        Dim lblResults As New Label()

        For Each iResult As Bing.ImageResult In imageResults
            searchResult.Append(String.Format("Image Title: <a href={1}>{0}</a><br />Image Url: {1}<br /><br />", iResult.Title, iResult.MediaUrl))
        Next
        lblResults.Text = searchResult.ToString()
        Panel1.Controls.Add(lblResults)
    End Sub

    ''' <summary>
    ''' Search for video only.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnVideosSearch_Click(sender As Object, e As EventArgs) Handles btnVideosSearch.Click
        Dim rptResult As New Repeater()
        Dim query As String = tbQueryString.Text

        ' Create a Bing container.
        Dim rootUrl As String = "https://api.datamarket.azure.com/Bing/Search"
        Dim bingContainer = New Bing.BingSearchContainer(New Uri(rootUrl))

        ' Configure bingContainer to use your credentials.
        bingContainer.Credentials = New NetworkCredential(AccountKey, AccountKey)

        ' Build the query, limiting to 10 results.
        Dim mediaQuery = bingContainer.Video(query, Nothing, market, Nothing, Nothing, Nothing, _
         Nothing, Nothing)
        mediaQuery = mediaQuery.AddQueryOption("$top", 50)

        ' Run the query and display the results.
        Dim mediaResults = mediaQuery.Execute()
        Dim lblResults As New Label()
        Dim searchResult As New StringBuilder()

        For Each vResult As Bing.VideoResult In mediaResults
            searchResult.Append(String.Format("Video Tile: <a href={1}>{0}</a><br />Video URL: {1}<br />", vResult.Title, vResult.MediaUrl))
        Next
        lblResults.Text = searchResult.ToString()
        Panel1.Controls.Add(lblResults)
    End Sub

    ''' <summary>
    ''' Search for news only.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnNewsSearch_Click(sender As Object, e As EventArgs) Handles btnNewsSearch.Click
        Dim rptResult As New Repeater()

        Dim query As String = tbQueryString.Text
        ' Create a Bing container.
        Dim rootUrl As String = "https://api.datamarket.azure.com/Bing/Search"
        Dim bingContainer = New Bing.BingSearchContainer(New Uri(rootUrl))

        ' Get news for science and technology.
        Dim newsCat As String = "rt_ScienceAndTechnology"

        ' Configure bingContainer to use your credentials.
        bingContainer.Credentials = New NetworkCredential(AccountKey, AccountKey)

        ' Build the query, limiting to 10 results.
        Dim newsQuery = bingContainer.News(query, Nothing, market, Nothing, Nothing, Nothing, _
         Nothing, newsCat, Nothing)
        newsQuery = newsQuery.AddQueryOption("$top", 10)

        ' Run the query and display the results.
        Dim newsResults = newsQuery.Execute()

        Dim searchResult As New StringBuilder()
        Dim lblResults As New Label()

        For Each nResult As Bing.NewsResult In newsResults
            searchResult.Append(String.Format("<a href={0}>{1}</a><br /> {2}<br /> {3}&nbsp;{4}<br /><br />", nResult.Url, nResult.Title, nResult.Description, nResult.Source, nResult.[Date]))
        Next
        lblResults.Text = searchResult.ToString()

        Panel1.Controls.Add(lblResults)
    End Sub

    ''' <summary>
    ''' Search with spelling suggestion.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnSpellingSuggestionSearch_Click(sender As Object, e As EventArgs) Handles btnSpellingSuggestionSearch.Click
        Dim query As String = tbQueryString.Text
        ' Create a Bing container.
        Dim rootUrl As String = "https://api.datamarket.azure.com/Bing/Search"
        Dim bingContainer = New Bing.BingSearchContainer(New Uri(rootUrl))

        ' Configure bingContainer to use your credentials.
        bingContainer.Credentials = New NetworkCredential(AccountKey, AccountKey)

        ' Build the query.
        Dim spellQuery = bingContainer.SpellingSuggestions(query, Nothing, market, Nothing, Nothing, Nothing)

        ' Run the query and display the results.
        Dim spellResults = spellQuery.Execute()

        Dim spellResultList As New List(Of Bing.SpellResult)()

        For Each result In spellResults
            spellResultList.Add(result)
        Next

        Dim lblResults As New Label()
        If spellResultList.Count > 0 Then
            lblResults.Text = String.Format("Spelling suggestion is <strong>{0}</strong>", spellResultList(0).Value)
        Else
            lblResults.Text = "No spelling suggestion. Type some typo keywords for suggestion for example ""xbx gamess"""
        End If
        Panel1.Controls.Add(lblResults)

    End Sub

    ''' <summary>
    ''' Related search.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnRelatedSearch_Click(sender As Object, e As EventArgs) Handles btnRelatedSearch.Click
        Dim query As String = tbQueryString.Text
        ' Create a Bing container.
        Dim rootUrl As String = "https://api.datamarket.azure.com/Bing/Search"
        Dim bingContainer = New Bing.BingSearchContainer(New Uri(rootUrl))

        ' Configure bingContainer to use your credentials.
        bingContainer.Credentials = New NetworkCredential(AccountKey, AccountKey)

        ' Build the query, limiting to 10 results.
        Dim relatedQuery = bingContainer.RelatedSearch(query, Nothing, market, Nothing, Nothing, Nothing)
        relatedQuery = relatedQuery.AddQueryOption("$top", 10)

        ' Run the query and display the results.
        Dim relatedResults = relatedQuery.Execute()

        Dim relatedSearchResultList As New List(Of Bing.RelatedSearchResult)()
        Dim lblResults As New Label()
        Dim searchResults As New StringBuilder()
        For Each rResult As Bing.RelatedSearchResult In relatedResults
            searchResults.Append(String.Format("<a href={1}>{0}</a><br /> {1}<br />", rResult.Title, rResult.BingUrl))
        Next
        lblResults.Text = searchResults.ToString()
        Panel1.Controls.Add(lblResults)
    End Sub

    ''' <summary>
    ''' Composite search.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnCompositeSearch_Click(sender As Object, e As EventArgs) Handles btnCompositeSearch.Click
        Dim query As String = tbQueryString.Text
        ' Create a Bing container.
        Dim rootUrl As String = "https://api.datamarket.azure.com/Bing/Search"
        Dim bingContainer = New Bing.BingSearchContainer(New Uri(rootUrl))

        ' The composite operations to use.
        Dim operations As String = "web+news"

        ' Configure bingContainer to use your credentials.
        bingContainer.Credentials = New NetworkCredential(AccountKey, AccountKey)

        ' Build the query, limiting to 5 results (per service operation).
        Dim compositeQuery = bingContainer.Composite(operations, query, Nothing, Nothing, market, Nothing, _
         Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, _
         Nothing, Nothing, Nothing)
        compositeQuery = compositeQuery.AddQueryOption("$top", 5)

        ' Run the query and display the results.
        Dim compositeResults = compositeQuery.Execute()

        Dim searchResults As New StringBuilder()

        For Each cResult In compositeResults
            searchResults.Append("<h3>Web Result</h3>")
            ' Display web results.
            For Each result In cResult.Web
                searchResults.Append(String.Format("<a href={2}>{0}</a><br /> {1}<br /> {2}<br /><br />", result.Title, result.Url, result.Description))
            Next

            searchResults.Append("<h3>News Result</h3>")
            ' Display news results.
            For Each result In cResult.News
                searchResults.Append(String.Format("<a href={0}>{1}</a><br /> {2}<br /> {3}&nbsp;{4}<br /><br />", result.Url, result.Title, result.Description, result.Source, result.[Date]))
            Next
        Next

        Dim lblResults As New Label()

        lblResults.Text = searchResults.ToString()
        Panel1.Controls.Add(lblResults)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


    End Sub











End Class