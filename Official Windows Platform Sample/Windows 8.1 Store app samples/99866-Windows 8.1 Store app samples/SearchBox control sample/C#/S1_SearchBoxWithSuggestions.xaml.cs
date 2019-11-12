//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************
using SDKTemplate;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SearchBox
{
    /// <summary>
    /// Scenario provides suggestions from the app content
    /// </summary>
    public sealed partial class S1_SearchBoxWithSuggestions : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public S1_SearchBoxWithSuggestions()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Populates SearchBox with Suggestions when user enters text
        /// </summary>
        /// <param name="sender">The Xaml SearchBox</param>
        /// <param name="e">Event when user changes text in SearchBox</param>
        private void SearchBoxEventsSuggestionsRequested(object sender, SearchBoxSuggestionsRequestedEventArgs e)
        {
            string queryText = e.QueryText;
            if (!string.IsNullOrEmpty(queryText))
            {
                Windows.ApplicationModel.Search.SearchSuggestionCollection suggestionCollection = e.Request.SearchSuggestionCollection;
                foreach (string suggestion in suggestionList)
                {
                    if (suggestion.StartsWith(queryText, StringComparison.CurrentCultureIgnoreCase))
                    {
                        suggestionCollection.AppendQuerySuggestion(suggestion);
                    }
                }
            }

            if (e.Request.SearchSuggestionCollection.Size > 0)
            {
                MainPage.Current.NotifyUser("Suggestions provided for query: " + queryText, NotifyType.StatusMessage);
            }
            else
            {
                MainPage.Current.NotifyUser("No suggestions provided for query: " + queryText, NotifyType.StatusMessage);
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

        /// <summary>
        /// App provided suggestions list
        /// </summary>
        private static readonly string[] suggestionList =
            {
                "Shanghai", "Istanbul", "Karachi", "Delhi", "Mumbai", "Moscow", "São Paulo", "Seoul", "Beijing", "Jakarta",
                "Tokyo", "Mexico City", "Kinshasa", "New York City", "Lagos", "London", "Lima", "Bogota", "Tehran", "Ho Chi Minh City",
                "Hong Kong", "Bangkok", "Dhaka", "Cairo", "Hanoi", "Rio de Janeiro", "Lahore", "Chonquing", "Bengaluru", "Tianjin",
                "Baghdad", "Riyadh", "Singapore", "Santiago", "Saint Petersburg", "Surat", "Chennai", "Kolkata", "Yangon", "Guangzhou",
                "Alexandria", "Shenyang", "Hyderabad", "Ahmedabad", "Ankara", "Johannesburg", "Wuhan", "Los Angeles", "Yokohama",
                "Abidjan", "Busan", "Cape Town", "Durban", "Pune", "Jeddah", "Berlin", "Pyongyang", "Kanpur", "Madrid", "Jaipur",
                "Nairobi", "Chicago", "Houston", "Philadelphia", "Phoenix", "San Antonio", "San Diego", "Dallas", "San Jose",
                "Jacksonville", "Indianapolis", "San Francisco", "Austin", "Columbus", "Fort Worth", "Charlotte", "Detroit",
                "El Paso", "Memphis", "Baltimore", "Boston", "Seattle Washington", "Nashville", "Denver", "Louisville", "Milwaukee",
                "Portland", "Las Vegas", "Oklahoma City", "Albuquerque", "Tucson", "Fresno", "Sacramento", "Long Beach", "Kansas City",
                "Mesa", "Virginia Beach", "Atlanta", "Colorado Springs", "Omaha", "Raleigh", "Miami", "Cleveland", "Tulsa", "Oakland",
                "Minneapolis", "Wichita", "Arlington", "Bakersfield", "New Orleans", "Honolulu", "Anaheim", "Tampa", "Aurora",
                "Santa Ana", "St. Louis", "Pittsburgh", "Corpus Christi", "Riverside", "Cincinnati", "Lexington", "Anchorage",
                "Stockton", "Toledo", "St. Paul", "Newark", "Greensboro", "Buffalo", "Plano", "Lincoln", "Henderson", "Fort Wayne",
                "Jersey City", "St. Petersburg", "Chula Vista", "Norfolk", "Orlando", "Chandler", "Laredo", "Madison", "Winston-Salem",
                "Lubbock", "Baton Rouge", "Durham", "Garland", "Glendale", "Reno", "Hialeah", "Chesapeake", "Scottsdale",
                "North Las Vegas", "Irving", "Fremont", "Irvine", "Birmingham", "Rochester", "San Bernardino", "Spokane",
                "Toronto", "Montreal", "Vancouver", "Ottawa-Gatineau", "Calgary", "Edmonton", "Quebec City", "Winnipeg", "Hamilton"
            };
    }
}
