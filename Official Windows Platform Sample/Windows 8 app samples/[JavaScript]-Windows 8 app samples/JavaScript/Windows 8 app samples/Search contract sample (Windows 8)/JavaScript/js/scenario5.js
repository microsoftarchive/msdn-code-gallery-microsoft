//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var xhrRequest;

    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {
            WinJS.log && WinJS.log("Use the search pane to submit a query", "sample", "status");

            // Provide suggestions using an URL that supports the OpenSearch suggestion format.
            // Scenarios 2-6 introduce different methods of providing suggestions. The registration for the onsuggestionsrequested
            // event is added in a local scope for this sample's purpose, but in the common case, you should place this code in the
            // global scope, e.g. default.js, to run as soon as your app is launched. This way your app can provide suggestions
            // anytime the user brings up the search pane.
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = function (eventObject) {
                var queryText = eventObject.queryText, language = eventObject.language, suggestionRequest = eventObject.request;

                // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
                // Indicate that we'll obtain suggestions asynchronously:
                var deferral = suggestionRequest.getDeferral();

                // Use the web service Uri entered in the urlInput that supports this standard in order to see suggestions come from the web service.
                // See http://www.opensearch.org/Specifications/OpenSearch/Extensions/Suggestions/1.0 for details on OpenSearch Suggestions format.
                // And replace "{searchTerms}" with the query string, which should be encoded into the URI.
                var suggestionUri = document.getElementById("urlInput").innerText.replace("{searchTerms}", encodeURIComponent(queryText));

                // Cancel the previous suggestion request if it is not finished.
                if (xhrRequest && xhrRequest.cancel) {
                    xhrRequest.cancel();
                }

                // Create request to obtain suggestions from service and supply them to the Search Pane.
                xhrRequest = WinJS.xhr({ url: suggestionUri });
                xhrRequest.done(
                    function (request) {
                        if (request.responseText) {
                            var parsedResponse = JSON.parse(request.responseText);
                            if (parsedResponse && parsedResponse instanceof Array) {
                                var suggestions = parsedResponse[1];
                                if (suggestions) {
                                    suggestionRequest.searchSuggestionCollection.appendQuerySuggestions(suggestions);
                                    WinJS.log && WinJS.log("Suggestions provided for query: " + queryText, "sample", "status");
                                } else {
                                    WinJS.log && WinJS.log("No suggestions provided for query: " + queryText, "sample", "status");
                                }
                            }
                        }

                        deferral.complete(); // Indicate we're done supplying suggestions.
                    },
                    function (error) {
                        WinJS.log && WinJS.log("Error retrieving suggestions for query: " + queryText, "sample", "status");
                        // Call complete on the deferral when there is an error.
                        deferral.complete();
                    });
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
