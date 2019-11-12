//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var xhrRequest;

    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {
            WinJS.log && WinJS.log("Use the search pane to submit a query", "sample", "status");

            // Provide suggestions using an URL that supports the XML Search suggestion format.
            // Scenarios 2-6 introduce different methods of providing suggestions. The registration for the onsuggestionsrequested
            // event is added in a local scope for this sample's purpose, but in the common case, you should place this code in the
            // global scope, e.g. default.js, to run as soon as your app is launched. This way your app can provide suggestions
            // anytime the user brings up the search pane.
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = function (eventObject) {
                var queryText = eventObject.queryText, language = eventObject.language, suggestionRequest = eventObject.request;

                // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
                // Indicate that we'll do this asynchronously:
                var deferral = suggestionRequest.getDeferral();

                // Use the web service Uri entered in the urlInput that supports this standard in order to see suggestions come from the web service.
                // See http://msdn.microsoft.com/en-us/library/cc848863(v=vs.85).aspx for details on XML Search Suggestions format.
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
                        if (request.responseXML) {
                            generateSuggestions(request.responseXML, suggestionRequest.searchSuggestionCollection, queryText);
                        }

                        deferral.complete(); // Indicate we're done supplying suggestions.
                    },
                    function (error) {
                        WinJS.log && WinJS.log("Error retrieving suggestions for query: " + queryText, "sample", "status");

                        // Call complete on the deferral when there is an error.
                        deferral.complete();
                    });
            };

            // Generate suggestions from xml response by server.
            function generateSuggestions(responseXML, suggestions, queryText) {
                var section = responseXML.getElementsByTagName("Section");
                if (section && section.length > 0) {
                    var suggestionsXML = section[0];
                    for (var i = 0, len = suggestionsXML.childNodes.length; i < len; i++) {
                        var node = suggestionsXML.childNodes[i];
                        if (node.nodeName === "Separator") {
                            var separatorName = null;
                            var titleAttr = node.attributes.getNamedItem("title");
                            if (titleAttr) {
                                separatorName = titleAttr.value;
                            }
                            if (!separatorName) {
                                separatorName = "Suggestions";
                            }
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

                return;
            }

            // Generate Search Pane suggestion from XML mark up for a single suggestion.
            function addSuggestionFromNode(node, suggestions) {
                if (node.nodeName !== "Item") {
                    return null;
                }

                var text, description, url, imageUrl, imageAlt;

                // Obtain suggestion values
                for (var i = 0; i < node.childNodes.length; i++) {
                    switch (node.childNodes[i].nodeName) {
                        case "Text":
                            text = node.childNodes[i].textContent;
                            break;
                        case "Description":
                            description = node.childNodes[i].textContent;
                            break;
                        case "Url":
                            url = node.childNodes[i].textContent;
                            break;
                        case "Image":
                            if (node.childNodes[i].attributes.getNamedItem("source")) {
                                imageUrl = node.childNodes[i].attributes.getNamedItem("source").value;
                            }
                            if (node.childNodes[i].attributes.getNamedItem("alt")) {
                                imageAlt = node.childNodes[i].attributes.getNamedItem("alt").value;
                            }
                            break;
                    }
                }

                if (!description) {
                    description = "";
                }

                if (!text || text === "") {
                    // No proper suggestion item exists
                } else if (!url) {
                    suggestions.appendQuerySuggestion(text);
                } else {
                    var imageUri;
                    if (imageUrl || imageUrl === "") {
                        // We require an image for result suggestions.
                        imageUri = new Windows.Foundation.Uri(imageUrl);
                    } else {
                        // The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content.
                        imageUri = new Windows.Foundation.Uri("ms-appx:///Images/SDK_ResultSuggestionImage.png");
                    }
                    if (imageUri) {
                        var imageSource = Windows.Storage.Streams.RandomAccessStreamReference.createFromUri(imageUri);
                        suggestions.appendResultSuggestion(text, description, url, imageSource, imageAlt);
                    }
                }
                return;
            }

            // Handle the selection of a result suggestion since the XML Suggestion Format can return these.
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onresultsuggestionchosen = function (eventObject) {
                WinJS.log && WinJS.log("Result Suggestion Selected with Tag: " + eventObject.tag, "sample", "status");
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
