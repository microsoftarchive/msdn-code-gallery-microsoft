//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S5-SuggestionsXml.html", {
        ready: function (element, options) {
            var searchBox = document.getElementById("searchBoxId");
            searchBox.addEventListener("suggestionsrequested", suggestionsRequestedHandler);
            searchBox.addEventListener("querysubmitted", querySubmittedHandler);
            searchBox.addEventListener("resultsuggestionchosen", resultSuggestionChosenHandler);
        }
    });

    var xhrRequest;

    function suggestionsRequestedHandler(eventObject) {
        var queryText = eventObject.detail.queryText,
            suggestionCollection = eventObject.detail.searchSuggestionCollection;

        // Use the web service URI entered in the urlInput that supports this standard in order to see suggestions come from the web service.
        // See http://msdn.microsoft.com/en-us/library/cc848863(v=vs.85).aspx for details on XML Search Suggestions format.
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
                if (request.responseXML) {
                    generateSuggestions(request.responseXML, suggestionCollection, queryText);
                }
            },
            function (error) {
                WinJS.log && WinJS.log("Error retrieving suggestions for query: " + queryText, "sample", "status");
            }));
    }

    // Generate suggestions from xml response by server.
    function generateSuggestions(responseXML, suggestions, queryText) {
        var section = responseXML.getElementsByTagName("Section");
        if (section && section.length > 0) {
            var suggestionsXML = section[0];
            for (var i = 0, len = suggestionsXML.childNodes.length; i < len; i++) {
                var node = suggestionsXML.childNodes[i];
                if (node.nodeName === "Separator") {
                    var titleAttr = node.attributes.getNamedItem("title");
                    var separatorName = (titleAttr ? titleAttr.value : "Suggestions");
                    suggestions.appendSearchSeparator(separatorName);
                } else {
                    addSuggestionFromNode(node, suggestions);
                }
            }

            if (suggestions.size > 0) {
                WinJS.log && WinJS.log("Suggestions provided for query: " + queryText, "sample", "status");
            } else {
                WinJS.log && WinJS.log("No suggestions provided for query: " + queryText, "sample", "status");
            }
        }
    }

    // Generate Search Pane suggestion from XML mark up for a single suggestion.
    function addSuggestionFromNode(node, suggestions) {
        if (node.nodeName === "Item") {
            var text,
                description,
                url,
                imageSource,
                imageAlt;

            // Obtain suggestion values
            for (var i = 0, len = node.childNodes.length; i < len; i++) {
                var child = node.childNodes[i];
                switch (child.nodeName) {
                    case "Text":
                        text = child.textContent;
                        break;
                    case "Description":
                        description = child.textContent;
                        break;
                    case "Url":
                        url = child.textContent;
                        break;
                    case "Image":
                        if (child.attributes.getNamedItem("source")) {
                            imageSource = child.attributes.getNamedItem("source").value;
                        }
                        if (child.attributes.getNamedItem("alt")) {
                            imageAlt = child.attributes.getNamedItem("alt").value;
                        }
                        break;
                }
            }

            if (!description) {
                description = "";
            }

            // Text is required because it is used as the query suggestion or result name
            if (text) {
                // If a URL is supplied, then this is a result suggestion, otherwise it's a query suggestion
                if (url) {
                    // Either use the supplied image or fallback to a default one
                    var imageUri;
                    if (imageSource) {
                        // We require an image for result suggestions.
                        imageUri = new Windows.Foundation.Uri(imageSource);
                    } else {
                        // The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content.
                        imageUri = new Windows.Foundation.Uri("ms-appx:///Images/SDK_ResultSuggestionImage.png");
                    }
                    if (imageUri) {
                        var imageStream = Windows.Storage.Streams.RandomAccessStreamReference.createFromUri(imageUri);
                        suggestions.appendResultSuggestion(text, description, url, imageStream, imageAlt);
                    }
                } else {
                    suggestions.appendQuerySuggestion(text);
                }
            }
        }
    }

    function querySubmittedHandler(eventObject) {
        var queryText = eventObject.detail.queryText;
        WinJS.log && WinJS.log(queryText, "sample", "status");
    }

    function resultSuggestionChosenHandler(eventObject) {
        WinJS.log && WinJS.log("Result chosen: " + eventObject.detail.tag, "sample", "status");
    }
})();
