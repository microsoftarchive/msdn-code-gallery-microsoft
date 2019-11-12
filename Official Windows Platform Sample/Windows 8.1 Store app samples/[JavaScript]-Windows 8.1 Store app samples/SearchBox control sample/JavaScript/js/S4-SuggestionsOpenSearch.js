//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S4-SuggestionsOpenSearch.html", {
        ready: function (element, options) {
            var searchBox = document.getElementById("searchBoxId");
            searchBox.addEventListener("suggestionsrequested", suggestionsRequestedHandler);
            searchBox.addEventListener("querysubmitted", querySubmittedHandler);
        }
    });

    var xhrRequest;

    function suggestionsRequestedHandler(eventObject) {
        var queryText = eventObject.detail.queryText,
            suggestionCollection = eventObject.detail.searchSuggestionCollection;

        // Use the web service URI entered in the urlInput that supports this standard in order to see suggestions come from the web service.
        // See http://www.opensearch.org/Specifications/OpenSearch/Extensions/Suggestions/1.0 for details on OpenSearch Suggestions format.
        // And replace "{searchTerms}" with the query string, which should be encoded into the URI.
        var suggestionUri = document.getElementById("urlInput").innerText.replace("{searchTerms}", encodeURIComponent(queryText));

        // Cancel the previous suggestion request if it is not finished.
        if (xhrRequest && xhrRequest.cancel) {
            xhrRequest.cancel();
        }

        // Create request to obtain suggestions from service and supply them to the Search Pane.
        xhrRequest = WinJS.xhr({ url: suggestionUri });
        eventObject.detail.setPromise(xhrRequest.then(
            function (request) {
                if (request.responseText) {
                    var parsedResponse = JSON.parse(request.responseText);
                    if (parsedResponse && parsedResponse instanceof Array && parsedResponse.length >= 2) {
                        var suggestions = parsedResponse[1];
                        if (suggestions) {
                            suggestionCollection.appendQuerySuggestions(suggestions);
                            WinJS.log && WinJS.log("Suggestions provided for query: " + queryText, "sample", "status");
                        } else {
                            WinJS.log && WinJS.log("No suggestions provided for query: " + queryText, "sample", "status");
                        }
                    }
                }
            },
            function (error) {
                WinJS.log && WinJS.log("Error retrieving suggestions for query: " + queryText, "sample", "status");
            }));
    }

    function querySubmittedHandler(eventObject) {
        var queryText = eventObject.detail.queryText;
        WinJS.log && WinJS.log(queryText, "sample", "status");
    }
})();
