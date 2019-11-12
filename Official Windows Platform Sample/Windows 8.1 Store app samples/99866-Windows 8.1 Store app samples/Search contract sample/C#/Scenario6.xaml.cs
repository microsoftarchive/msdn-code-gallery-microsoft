//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;

using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SearchContract
{
    public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
    {
        private SearchPane searchPane;
        private IAsyncOperation<XmlDocument> currentXmlRequestOp = null;

        public Scenario6()
        {
            this.InitializeComponent();
            searchPane = SearchPane.GetForCurrentView();
        }

        private void AddSuggestionFromNode(IXmlNode node, SearchSuggestionCollection suggestions)
        {
            string text = "";
            string description = "";
            string url = "";
            string imageUrl = "";
            string imageAlt = "";

            foreach (IXmlNode subNode in node.ChildNodes)
            {
                if (subNode.NodeType != NodeType.ElementNode)
                {
                    continue;
                }
                if (subNode.NodeName.Equals("Text", StringComparison.CurrentCultureIgnoreCase))
                {
                    text = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Description", StringComparison.CurrentCultureIgnoreCase))
                {
                    description = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Url", StringComparison.CurrentCultureIgnoreCase))
                {
                    url = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Image", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (subNode.Attributes.GetNamedItem("source") != null)
                    {
                        imageUrl = subNode.Attributes.GetNamedItem("source").InnerText;
                    }
                    if (subNode.Attributes.GetNamedItem("alt") != null)
                    {
                        imageAlt = subNode.Attributes.GetNamedItem("alt").InnerText;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                // No proper suggestion item exists
            }
            else if (string.IsNullOrWhiteSpace(url))
            {
                suggestions.AppendQuerySuggestion(text);
            }
            else
            {
                // The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content
                Uri uri = string.IsNullOrWhiteSpace(imageUrl) ? new Uri("ms-appx:///Assets/SDK_ResultSuggestionImage.png") : new Uri(imageUrl);
                RandomAccessStreamReference imageSource = RandomAccessStreamReference.CreateFromUri(uri);
                suggestions.AppendResultSuggestion(text, description, url, imageSource, imageAlt);
            }
        }

        private async Task GetSuggestionsAsync(string str, SearchSuggestionCollection suggestions)
        {
            // Cancel the previous suggestion request if it is not finished.
            if (currentXmlRequestOp != null)
            {
                currentXmlRequestOp.Cancel();
            }

            // Get the suggestion from a web service.
            currentXmlRequestOp = XmlDocument.LoadFromUriAsync(new Uri(str));
            XmlDocument doc = await currentXmlRequestOp;
            currentXmlRequestOp = null;
            XmlNodeList nodes = doc.GetElementsByTagName("Section");
            if (nodes.Count > 0)
            {
                IXmlNode section = nodes[0];
                foreach (IXmlNode node in section.ChildNodes)
                {
                    if (node.NodeType != NodeType.ElementNode)
                    {
                        continue;
                    }
                    if (node.NodeName.Equals("Separator", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string title = null;
                        IXmlNode titleAttr = node.Attributes.GetNamedItem("title");
                        if (titleAttr != null)
                        {
                            title = titleAttr.NodeValue.ToString();
                        }
                        suggestions.AppendSearchSeparator(string.IsNullOrWhiteSpace(title) ? "Suggestions" : title);
                    }
                    else
                    {
                        AddSuggestionFromNode(node, suggestions);
                    }
                }
            }
        }

        private async void OnSearchPaneSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs e)
        {
            var queryText = e.QueryText;
            if (string.IsNullOrEmpty(queryText))
            {
                MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage);
            }
            else if (string.IsNullOrEmpty(UrlTextBox.Text))
            {
                MainPage.Current.NotifyUser("Please enter the web service URL", NotifyType.StatusMessage);
            }
            else
            {
                // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
                var request = e.Request;
                var deferral = request.GetDeferral();

                try
                {
                    // Use the web service Url entered in the UrlTextBox that supports XML Search Suggestions in order to see suggestions come from the web service.
                    // See http://msdn.microsoft.com/en-us/library/cc848863(v=vs.85).aspx for details on XML Search Suggestions format.
                    // Replace "{searchTerms}" of the Url with the query string.
                    Task task = GetSuggestionsAsync(Regex.Replace(UrlTextBox.Text, "{searchTerms}", Uri.EscapeDataString(queryText)), request.SearchSuggestionCollection);
                    await task;
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        if (request.SearchSuggestionCollection.Size > 0)
                        {
                            MainPage.Current.NotifyUser("Suggestions provided for query: " + queryText, NotifyType.StatusMessage);
                        }
                        else
                        {
                            MainPage.Current.NotifyUser("No suggestions provided for query: " + queryText, NotifyType.StatusMessage);
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    // Previous suggestion request was canceled.
                }
                catch (FormatException)
                {
                    MainPage.Current.NotifyUser("Suggestions could not be retrieved, please verify that the URL points to a valid service (for example http://contoso.com?q={searchTerms})", NotifyType.ErrorMessage);
                }
                catch (Exception)
                {
                    MainPage.Current.NotifyUser("Suggestions could not be displayed, please verify that the service provides valid XML Search Suggestions.", NotifyType.ErrorMessage);
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private void OnResultSuggestionChosen(SearchPane sender, SearchPaneResultSuggestionChosenEventArgs e)
        {
            // Handle the selection of a result suggestion since the XML Suggestion Format can return these.
            MainPage.Current.NotifyUser("Result suggestion selected with tag: " + e.Tag, NotifyType.StatusMessage);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage);
            // These events should be registered when your app first creates its main window after receiving an activated event, like OnLaunched, OnSearchActivated.
            // Typically this occurs in App.xaml.cs.
            searchPane.SuggestionsRequested += new TypedEventHandler<SearchPane, SearchPaneSuggestionsRequestedEventArgs>(OnSearchPaneSuggestionsRequested);
            searchPane.ResultSuggestionChosen += new TypedEventHandler<SearchPane, SearchPaneResultSuggestionChosenEventArgs>(OnResultSuggestionChosen);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            searchPane.SuggestionsRequested -= new TypedEventHandler<SearchPane, SearchPaneSuggestionsRequestedEventArgs>(OnSearchPaneSuggestionsRequested);
            searchPane.ResultSuggestionChosen -= new TypedEventHandler<SearchPane, SearchPaneResultSuggestionChosenEventArgs>(OnResultSuggestionChosen);
        }
    }
}
