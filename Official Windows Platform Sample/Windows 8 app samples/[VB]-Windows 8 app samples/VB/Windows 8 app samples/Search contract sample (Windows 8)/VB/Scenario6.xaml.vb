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
Imports System.Threading.Tasks
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Search
Imports Windows.Foundation
Imports Windows.Data.Xml.Dom
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private searchPane As SearchPane

    Private Shared ReadOnly exampleResponse As String = "xmlSuggestionService\exampleXmlResponse.xml"

    Public Sub New()
        Me.InitializeComponent()
        searchPane = searchPane.GetForCurrentView()
    End Sub

    Private Sub AddSuggestionFromNode(node As IXmlNode, suggestions As SearchSuggestionCollection)
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
        Next

        ' No proper suggestion item exists
        If String.IsNullOrWhiteSpace(text) Then
        ElseIf String.IsNullOrWhiteSpace(url) Then
            suggestions.AppendQuerySuggestion(text)
        Else
            ' The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content
            Dim uri As Uri = If(String.IsNullOrWhiteSpace(imageUrl), New Uri("ms-appx:///Assets/SDK_ResultSuggestionImage.png"), New Uri(imageUrl))
            Dim imageSource As RandomAccessStreamReference = RandomAccessStreamReference.CreateFromUri(uri)
            suggestions.AppendResultSuggestion(text, description, url, imageSource, imageAlt)
        End If
    End Sub

    Private Async Function GenerateSuggestions(file As StorageFile, suggestions As SearchSuggestionCollection) As Task
        Dim doc As New XmlDocument()
        doc.LoadXml(Await FileIO.ReadTextAsync(file))
        Dim nodes As XmlNodeList = doc.GetElementsByTagName("Section")
        If nodes.Count > 0 Then
            Dim section As IXmlNode = nodes.ElementAt(0)
            For Each node As IXmlNode In section.ChildNodes
                If node.NodeType <> NodeType.ElementNode Then
                    Continue For
                End If
                If node.NodeName.Equals("Separator", StringComparison.CurrentCultureIgnoreCase) Then
                    Dim title As String = node.Attributes.GetNamedItem("title").NodeValue.Tostring
                    If String.IsNullOrWhiteSpace(title) Then
                        suggestions.AppendSearchSeparator(title)
                    End If
                Else
                    AddSuggestionFromNode(node, suggestions)
                End If
            Next
        End If
    End Function

    Private Async Sub OnSearchPaneSuggestionsRequested(sender As SearchPane, e As SearchPaneSuggestionsRequestedEventArgs)
        Dim queryText = e.QueryText
        If String.IsNullOrEmpty(queryText) Then
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        Else
            Dim request = e.Request
            Dim deferral = request.GetDeferral()

            Try
                Dim file As StorageFile = Await Package.Current.InstalledLocation.GetFileAsync(exampleResponse)
                Await Me.GenerateSuggestions(file, request.SearchSuggestionCollection)

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
