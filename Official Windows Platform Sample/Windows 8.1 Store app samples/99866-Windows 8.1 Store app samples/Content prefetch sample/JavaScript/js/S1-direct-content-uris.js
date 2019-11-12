//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S1-direct-content-uris.html", {
        ready: function (element, options) {
            document.getElementById("addDirectUri").addEventListener("click", addDirectUri, false);
            document.getElementById("clearDirectUris").addEventListener("click", clearDirectUris, false);

            initializeUriInputField("http://www.microsoft.com/favicon.ico");
            populateUriTable();
        }
    });

    function addDirectUri() {
        var directUriField = document.getElementById('directUri');
        var uriToAdd = directUriField.value;

        if (uriToAdd) {
            var uri;
            try {
                uri = new Windows.Foundation.Uri(uriToAdd);
            } catch (error) {
                // The URI that was provided by the application user may be incorrectly formed (e.g., not including http:// as a prefix).
                // We use the constructor of Windows.Foundation.Uri to determine if there was an error (as it throws if the URI string is
                // malformed).
                WinJS.log && WinJS.log("A URI must be provided that has the form scheme://address", "ContentPrefetcherSample", "error");
            }

            if (uri) {
                // This is the interesting function call for the ContentPrefetcher API.
                // The ContentPrefetcher's contentUris is a vector, so it supports all the common operations that a
                // vector should. There is nothing else that is required to make the ContentPrefetcher aware of URIs
                // that should be prefetched.
                Windows.Networking.BackgroundTransfer.ContentPrefetcher.contentUris.append(uri);

                populateUriTable();
                initializeUriInputField("");
            }
        }
    }

    function clearDirectUris() {
        // The ContentPrefetcher's contentUris vector is persistent over multiple app runs. Clearing the vector
        // is important if attempting to reset the URI list, as "old" URIs will continue to be preserved until
        // removed. When the app starts up, the ContentPrefetcher.contentUris will contain all the URIs that have
        // been set in previous application runs.
        Windows.Networking.BackgroundTransfer.ContentPrefetcher.contentUris.clear();
        populateUriTable();
    }

    function populateUriTable() {
        var uriTable = document.getElementById('directUriTable');
        clearUriTable(uriTable);
        var uris = Windows.Networking.BackgroundTransfer.ContentPrefetcher.contentUris.getView();

        uris.forEach(function (uri) {
            var tableRow = uriTable.insertRow(-1);
            tableRow.insertCell(0).innerText = uri.absoluteCanonicalUri;

            var cachedTableCell = tableRow.insertCell(1);
            updateCachedTableCell(uri, cachedTableCell);
        });
    }

    function clearUriTable(uriTable) {
        var rowCount = uriTable.rows.length;
        for (var i = rowCount - 1; i > 0; --i) {
            uriTable.deleteRow(i);
        }
    }

    function initializeUriInputField(uriString) {
        var uriInputField = document.getElementById('directUri');
        uriInputField.innerText = uriString;
    }

    function updateCachedTableCell(uri, tableCell) {
        // Our mechanism for determining if something in the cache is to use the HttpClient to do the
        // heavy lifting for us. We tell the APIs that we only want to fetch URIs in an "only from cache" mode.
        // The APIs will throw an exception if the URI is not available in the cache. We catch the exception and
        // react appropriately.
        var filter = Windows.Web.Http.Filters.HttpBaseProtocolFilter();
        filter.cacheControl.readBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.onlyFromCache;

        var httpClient = new Windows.Web.Http.HttpClient(filter);
        var request = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.get, uri);

        httpClient.sendRequestAsync(request).then(function (response) {
            tableCell.className = "cached";
            tableCell.innerText = "Yes";
        }, function (error) {
            tableCell.className = "uncached";
            tableCell.innerText = "No";
        });
    }
})();
