//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Scenarios 2-6 introduce different methods of providing suggestions. This suggestion list is added in a local scope for this
    // sample's purpose, but in the common case, you should place this code in the global scope, e.g. default.js, to run as
    // soon as your app is launched. This way your app can provide suggestions anytime the user brings up the search pane.
    var suggestionList = ["Shanghai", "Istanbul", "Karachi", "Delhi", "Mumbai", "Moscow", "SÃo Paulo", "Seoul", "Beijing", "Jakarta",
                    "Tokyo", "Mexico City", "Kinshasa", "New York City", "Lagos", "London", "Lima", "Bogota", "Tehran", "Ho Chi Minh City", "Hong Kong",
                    "Bangkok", "Dhaka", "Cairo", "Hanoi", "Rio de Janeiro", "Lahore", "Chonquing", "Bengaluru", "Tianjin", "Baghdad", "Riyadh", "Singapore",
                    "Santiago", "Saint Petersburg", "Surat", "Chennai", "Kolkata", "Yangon", "Guangzhou", "Alexandria", "Shenyang", "Hyderabad", "Ahmedabad",
                    "Ankara", "Johannesburg", "Wuhan", "Los Angeles", "Yokohama", "Abidjan", "Busan", "Cape Town", "Durban", "Pune", "Jeddah", "Berlin",
                    "Pyongyang", "Kanpur", "Madrid", "Jaipur", "Nairobi", "Chicago", "Houston", "Philadelphia", "Phoenix", "San Antonio", "San Diego",
                    "Dallas", "San Jose", "Jacksonville", "Indianapolis", "San Francisco", "Austin", "Columbus", "Fort Worth", "Charlotte", "Detroit",
                    "El Paso", "Memphis", "Baltimore", "Boston", "Seattle Washington", "Nashville", "Denver", "Louisville", "Milwaukee", "Portland",
                    "Las Vegas", "Oklahoma City", "Albuquerque", "Tucson", "Fresno", "Sacramento", "Long Beach", "Kansas City", "Mesa", "Virginia Beach",
                    "Atlanta", "Colorado Springs", "Omaha", "Raleigh", "Miami", "Cleveland", "Tulsa", "Oakland", "Minneapolis", "Wichita", "Arlington",
                    "Bakersfield", "New Orleans", "Honolulu", "Anaheim", "Tampa", "Aurora", "Santa Ana", "St. Louis", "Pittsburgh", "Corpus Christi",
                    "Riverside", "Cincinnati", "Lexington", "Anchorage", "Stockton", "Toledo", "St. Paul", "Newark", "Greensboro", "Buffalo", "Plano",
                    "Lincoln", "Henderson", "Fort Wayne", "Jersey City", "St. Petersburg", "Chula Vista", "Norfolk", "Orlando", "Chandler", "Laredo", "Madison",
                    "Winston-Salem", "Lubbock", "Baton Rouge", "Durham", "Garland", "Glendale", "Reno", "Hialeah", "Chesapeake", "Scottsdale", "North Las Vegas",
                    "Irving", "Fremont", "Irvine", "Birmingham", "Rochester", "San Bernardino", "Spokane", "Toronto", "Montreal", "Vancouver", "Ottawa-Gatineau",
                    "Calgary", "Edmonton", "Quebec City", "Winnipeg", "Hamilton"];

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            WinJS.log && WinJS.log("Use the search pane to submit a query", "sample", "status");

            // Supply suggestions that correspond to the query the suggestion request is for using an app defined list.
            // Register the onsuggestionsrequested event in your apps global scope, for example default.js, so that it is registered as soon as your app is launched.
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = function (eventObject) {
                var queryText = eventObject.queryText, suggestionRequest = eventObject.request;
                var query = queryText.toLowerCase();
                var maxNumberOfSuggestions = 25;
                for (var i = 0, len = suggestionList.length; i < len; i++) {
                    if (suggestionList[i].substr(0, query.length).toLowerCase() === query) {
                        suggestionRequest.searchSuggestionCollection.appendQuerySuggestion(suggestionList[i]);
                        if (suggestionRequest.searchSuggestionCollection.size === maxNumberOfSuggestions) {
                            break;
                        }
                    }
                }

                if (suggestionRequest.searchSuggestionCollection.size > 0) {
                    WinJS.log && WinJS.log("Suggestions provided for query: " + queryText, "sample", "status");
                } else {
                    WinJS.log && WinJS.log("No suggestions provided for query: " + queryText, "sample", "status");
                }
            };
        },
        unload: function () {
            // Scenarios 2-6 introduce different methods of providing suggestions. For the purposes of this sample,
            // remove suggestion handling when unloading this page so that it does not conflict with other scenarios.
            // This should not be added to your app.
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = null;
        }
    });
})();
