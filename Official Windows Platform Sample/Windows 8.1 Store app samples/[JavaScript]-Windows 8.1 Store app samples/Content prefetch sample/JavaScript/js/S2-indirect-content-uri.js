//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S2-indirect-content-uri.html", {
        ready: function (element, options) {
            document.getElementById('setIndirectContentUri').addEventListener('click', setIndirectContentUri, false);
            document.getElementById('clearIndirectContentUri').addEventListener('click', clearIndirectContentUri, false);
            populateIndirectContentUri();
        }
    });

    function populateIndirectContentUri() {
        var uriInputText = document.getElementById('indirectContentUri');

        // There can only be one indirect content uri for the ContentPrefetcher. The expectation of the indirect content uri is that
        // it points to a XML file that contains a collection of URIs. The MSDN page contains the documentation of the schema that is
        // expected of the XML file.
        var indirectUri = Windows.Networking.BackgroundTransfer.ContentPrefetcher.indirectContentUri;

        if (indirectUri) {
            uriInputText.innerText = "The indirect content uri is " + indirectUri.absoluteCanonicalUri;
        } else {
            uriInputText.innerText = "There is no indirect content uri set";
        }
    }

    function setIndirectContentUri() {
        var indirectUri = document.getElementById('indirectContentUriField').value;

        WinJS.log && WinJS.log("", "ContentPrefetcherSample", "error");

        if (indirectUri) {
            var uri;
            try {
                uri = new Windows.Foundation.Uri(indirectUri);
            } catch (error) {
                // The URI that was provided by the application user may be incorrectly formed (e.g., not including http:// as a prefix).
                // We use the constructor of Windows.Foundation.Uri to determine if there was an error (as it throws if the URI string is
                // malformed).
                WinJS.log && WinJS.log("A URI must be provided that has the form scheme://address", "ContentPrefetcherSample", "error");
            }

            if (uri) {
                Windows.Networking.BackgroundTransfer.ContentPrefetcher.indirectContentUri = uri;
                document.getElementById('indirectContentUriField').value = "";
                populateIndirectContentUri();
            }
        }
    }

    function clearIndirectContentUri() {
        Windows.Networking.BackgroundTransfer.ContentPrefetcher.indirectContentUri = null;
        populateIndirectContentUri();
    }

})();
