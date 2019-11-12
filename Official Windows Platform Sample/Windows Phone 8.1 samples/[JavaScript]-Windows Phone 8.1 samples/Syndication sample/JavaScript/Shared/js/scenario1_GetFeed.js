//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario1_GetFeed.html", {
        ready: function (element, options) {
            document.getElementById("getFeedButton").addEventListener("click", scenario1Start, false);
            document.getElementById("previousItemButton").addEventListener("click", previousClicked, false);
            document.getElementById("nextItemButton").addEventListener("click", nextClicked, false);
        }
    });

    var currentFeed = null;
    var currentItemIndex = 0;

    function onError(err) {
        WinJS.log && WinJS.log(err, "sample", "error");

        // Match error number with a SyndicationErrorStatus value. Use
        // Windows.Web.WebErrorStatus.getStatus() to retrieve HTTP error status codes.
        var errorStatus = Windows.Web.Syndication.SyndicationError.getStatus(err.number);
        if (errorStatus === Windows.Web.Syndication.SyndicationErrorStatus.invalidXml) {
            displayLog("An invalid XML exception was thrown. Please make sure to use a URI that points to a RSS or Atom feed.");
        }
    }

    // Retrieve and display feed at given feed address.
    function scenario1Start() {
        clearLog();

        // By default 'scenario1Input1' is disabled and URI validation is not required. When enabling the text box
        // validating the URI is required since it was received from an untrusted source (user input).
        // The URI is validated by catching exceptions thrown by the Uri constructor.
        // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require the
        // "Home or Work Networking" capability.
        var uri = null;
        try {
            uri = new Windows.Foundation.Uri(document.getElementById("scenario1Input1").value.trim());
        } catch (error) {
            WinJS.log && WinJS.log("Error: Invalid URI", "sample", "error");
            return;
        }

        WinJS.log && WinJS.log("Downloading feed...", "sample", "status");
        displayLog("Downloading feed: " + uri.absoluteUri);

        var client = new Windows.Web.Syndication.SyndicationClient();
        client.bypassCacheOnRetrieve = true;

        // Although most HTTP servers do not require User-Agent header, others will reject the request or return
        // a different response if this header is missing. Use setRequestHeader() to add custom headers.
        client.setRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

        client.retrieveFeedAsync(uri).done(function (feed) {
            currentFeed = feed;

            WinJS.log && WinJS.log("Feed download complete.", "sample", "status");

            var title = "(no title)";
            if (currentFeed.title) {
                title = currentFeed.title.text;
            }
            document.getElementById("scenario1FeedTitle").innerText = title;

            currentItemIndex = 0;
            if (currentFeed.items.size > 0) {
                displayCurrentItem();
            }

            // List the items.
            displayLog("Items: " + currentFeed.items.size);
        }, onError);
    }

    // Callback for previousItemButton click event.
    function previousClicked() {
        if (currentFeed && currentItemIndex > 0) {
            currentItemIndex--;
            displayCurrentItem();
        }
    }

    // Callback for nextItemButton click event.
    function nextClicked() {
        if (currentFeed && currentItemIndex < currentFeed.items.size - 1) {
            currentItemIndex++;
            displayCurrentItem();
        }
    }

    // Display current syndication item from feed on page.
    function displayCurrentItem() {
        var item = currentFeed.items[currentItemIndex];

        // Display item number.
        document.getElementById("scenario1Index").innerText = (currentItemIndex + 1) + " of " + currentFeed.items.size;

        // Display title.
        var title = "(no title)";
        if (item.title) {
            title = item.title.text;
        }
        document.getElementById("scenario1ItemTitle").innerText = title;

        // Display the main link.
        var link = "";
        if (item.links.size > 0) {
            link = item.links[0].uri.absoluteUri;
        }

        var scenario1Link = document.getElementById("scenario1Link");
        scenario1Link.innerText = link;
        scenario1Link.href = link;

        // Display the body as HTML.
        var content = "(no content)";
        if (item.content) {
            content = item.content.text;
        } else if (item.summary) {
            content = item.summary.text;
        }
        document.getElementById("scenario1WebView").innerHTML = window.toStaticHTML(content);

        // Display element extensions. The elementExtensions collection contains all the additional child elements within
        // the current element that do not belong to the Atom or RSS standards (e.g., Dublin Core extension elements).
        var bindableNodes = [];
        for (var i = 0; i < item.elementExtensions.size; i++) {
            var bindableNode = {
                nodeName: item.elementExtensions[i].nodeName,
                nodeNamespace: item.elementExtensions[i].nodeNamespace,
                nodeValue: item.elementExtensions[i].nodeValue,
            };
            bindableNodes.push(bindableNode);
        }
        var dataList = new WinJS.Binding.List(bindableNodes);
        var listView = document.getElementById("extensionsListView").winControl;
        WinJS.UI.setOptions(listView, {
            itemDataSource: dataList.dataSource
        });

    }

    function clearLog() {
        var outputField = document.getElementById("scenario1OutputField");
        outputField.innerHTML = "";
    }

    function displayLog(message) {
        var outputField = document.getElementById("scenario1OutputField");
        outputField.innerHTML += message + "<br/>";
    }
})();
