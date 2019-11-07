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
Imports System.Text.RegularExpressions
Imports Windows.ApplicationModel.Search
Imports Windows.Data.Xml.Dom
Imports Windows.Storage.Streams

Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private searchPane As SearchPane
    Private currentXmlRequestOp As IAsyncOperation(Of XmlDocument) = Nothing

    Public Sub New()
        Me.InitializeComponent()
        searchPane = searchPane.GetForCurrentView()
    End Sub

    Private Sub AddSuggestionFromNode(ByVal node As IXmlNode, ByVal suggestions As SearchSuggestionCollection)
        Dim text As String = ""
        Dim description As String = ""
        Dim url As String = ""
        Dim imageUrl As String = ""
        Dim imageAlt As String = ""

        For Each subNode As IXmlNode In node.ChildNodes
            If subNode.NodeType <> NodeType.ElementNode Then
                Continue For
            End If
            If subNode.NodeName.Equals("Text", StringComparison.CurrentCultureIgnoreCase) Then
                text = subNode.InnerText
            ElseIf subNode.NodeName.Equals("Description", StringComparison.CurrentCultureIgnoreCase) Then
                description = subNode.InnerText
            ElseIf subNode.NodeName.Equals("Url", StringComparison.CurrentCultureIgnoreCase) Then
                url = subNode.InnerText
            ElseIf subNode.NodeName.Equals("Image", StringComparison.CurrentCultureIgnoreCase) Then
                If subNode.Attributes.GetNamedItem("source") IsNot Nothing Then
                    imageUrl = subNode.Attributes.GetNamedItem("source").InnerText
                End If
                If subNode.Attributes.GetNamedItem("alt") IsNot Nothing Then
                    imageAlt = subNode.Attributes.GetNamedItem("alt").InnerText
                End If
            End If
        Next subNode

        If String.IsNullOrWhiteSpace(text) Then
            ' No proper suggestion item exists
        ElseIf String.IsNullOrWhiteSpace(url) Then
            suggestions.AppendQuerySuggestion(text)
        Else
            ' The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content
            Dim uri As Uri = If(String.IsNullOrWhiteSpace(imageUrl), New Uri("ms-appx:///Assets/SDK_ResultSuggestionImage.png"), New Uri(imageUrl))
            Dim imageSource As RandomAccessStreamReference = RandomAccessStreamReference.CreateFromUri(uri)
            suggestions.AppendResultSuggestion(text, description, url, imageSource, imageAlt)
        End If
    End Sub

    Private Async Function GetSuggestionsAsync(ByVal str As String, ByVal suggestions As SearchSuggestionCollection) As Task
        ' Cancel the previous suggestion request if it is not finished.
        If currentXmlRequestOp IsNot Nothing Then
            currentXmlRequestOp.Cancel()
        End If

        ' Get the suggestion from a web service.
        currentXmlRequestOp = XmlDocument.LoadFromUriAsync(New Uri(str))
        Dim doc As XmlDocument = Await currentXmlRequestOp
        currentXmlRequestOp = Nothing
        Dim nodes As XmlNodeList = doc.GetElementsByTagName("Section")
        If nodes.Count > 0 Then
            Dim section As IXmlNode = nodes(0)
            For Each node As IXmlNode In section.ChildNodes
                If node.NodeType <> NodeType.ElementNode Then
                    Continue For
                End If
                If node.NodeName.Equals("Separator", StringComparison.CurrentCultureIgnoreCase) Then
                    Dim title As String = Nothing
                    Dim titleAttr As IXmlNode = node.Attributes.GetNamedItem("title")
                    If titleAttr IsNot Nothing Then
                        title = titleAttr.NodeValue.ToString()
                    End If
                    suggestions.AppendSearchSeparator(If(String.IsNullOrWhiteSpace(title), "Suggestions", title))
                Else
                    AddSuggestionFromNode(node, suggestions)
                End If
            Next node
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
                ' Use the web service Url entered in the UrlTextBox that supports XML Search Suggestions in order to see suggestions come from the web service.
                ' See http://msdn.microsoft.com/en-us/library/cc848863(v=vs.85).aspx for details on XML Search Suggestions format.
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
                ' Previous suggestion request was canceled.
            Catch e2 As FormatException
                MainPage.Current.NotifyUser("Suggestions could not be retrieved, please verify that the URL points to a valid service (for example http://contoso.com?q={searchTerms})", NotifyType.ErrorMessage)
            Catch e3 As Exception
                MainPage.Current.NotifyUser("Suggestions could not be displayed, please verify that the service provides valid XML Search Suggestions.", NotifyType.ErrorMessage)
            Finally
                deferral.Complete()
            End Try
        End If
    End Sub

    Private Sub OnResultSuggestionChosen(ByVal sender As SearchPane, ByVal e As SearchPaneResultSuggestionChosenEventArgs)
        ' Handle the selection of a result suggestion since the XML Suggestion Format can return these.
        MainPage.Current.NotifyUser("Result suggestion selected with tag: " & e.Tag, NotifyType.StatusMessage)
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        ' These events should be registered when your app first creates its main window after receiving an activated event, like OnLaunched, OnSearchActivated.
        ' Typically this occurs in App.xaml.vb.
        AddHandler searchPane.SuggestionsRequested, AddressOf OnSearchPaneSuggestionsRequested
        AddHandler searchPane.ResultSuggestionChosen, AddressOf OnResultSuggestionChosen
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler searchPane.SuggestionsRequested, AddressOf OnSearchPaneSuggestionsRequested
        RemoveHandler searchPane.ResultSuggestionChosen, AddressOf OnResultSuggestionChosen
    End Sub
End Class
