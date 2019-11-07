//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;
using Windows.Data.Json;
using System.Text.RegularExpressions;

namespace SearchBox
{
    /// <summary>
    /// Provides suggestions from the Open Search Protocol
    /// </summary>
    public sealed partial class S4_SuggestionsOpenSearch : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
       
        private HttpClient httpClient;
        private Task<string> currentHttpTask;
        private CultureInfo cultureInfo;

        public S4_SuggestionsOpenSearch()
        {
            this.InitializeComponent();
            httpClient = new HttpClient();
            cultureInfo = new CultureInfo("en-us");
        }

        ~S4_SuggestionsOpenSearch()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }
        }

        /// <summary>
        /// Completes the retreval of suggestions from the web service
        /// </summary>
        /// <param name="str">User query string</param>
        /// <param name="suggestions">Suggestions list to append new suggestions</param>
        /// <returns></returns>
        private async Task GetSuggestionsAsync(string str, SearchSuggestionCollection suggestions)
        {
            // Cancel the previous suggestion request if it is not finished.
            if (currentHttpTask != null)
            {
                currentHttpTask.AsAsyncOperation<string>().Cancel();
            }

            // Get the suggestions from an open search service.
            currentHttpTask = httpClient.GetStringAsync(str);
            string response = await currentHttpTask;
            JsonArray parsedResponse = JsonArray.Parse(response);
            if (parsedResponse.Count > 1)
            {
                foreach (JsonValue value in parsedResponse[1].GetArray())
                {
                    suggestions.AppendQuerySuggestion(value.GetString());
                }
            }
        }

        /// <summary>
        /// Populates SearchBox with Suggestions when user enters text
        /// </summary>
        /// <param name="sender">Xaml SearchBox</param>
        /// <param name="e">Event when user changes text in SearchBox</param>
        private async void SearchBoxEventsSuggestionsRequested(Object sender, SearchBoxSuggestionsRequestedEventArgs e)
        {
            var queryText = e.QueryText;
            if (string.IsNullOrEmpty(queryText))
            {
                MainPage.Current.NotifyUser("Use the search control to submit a query", NotifyType.StatusMessage);
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
                    // Use the web service Url entered in the UrlTextBox that supports OpenSearch Suggestions in order to see suggestions come from the web service.
                    // See http://www.opensearch.org/Specifications/OpenSearch/Extensions/Suggestions/1.0 for details on OpenSearch Suggestions format.
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
                    // We have canceled the task.
                }
                catch (FormatException)
                {
                    MainPage.Current.NotifyUser(@"Suggestions could not be retrieved -- please verify that the URL points to a valid service (for example http://contoso.com?q={searchTerms}", NotifyType.ErrorMessage);
                }
                catch (Exception)
                {
                    MainPage.Current.NotifyUser("Suggestions could not be displayed -- please verify that the service provides valid OpenSearch suggestions", NotifyType.ErrorMessage);
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        /// <summary>
        /// Called when query submitted in SearchBox
        /// </summary>
        /// <param name="sender">The Xaml SearchBox</param>
        /// <param name="e">Event when user submits query</param>
        private void SearchBoxEventsQuerySubmitted(object sender, SearchBoxQuerySubmittedEventArgs e)
        {
            MainPage.Current.NotifyUser(e.QueryText, NotifyType.StatusMessage);
        }
    }
}
