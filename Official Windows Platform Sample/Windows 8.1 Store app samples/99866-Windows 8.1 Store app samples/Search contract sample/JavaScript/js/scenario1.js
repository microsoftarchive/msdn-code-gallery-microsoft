//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        processed: function (element, options) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Options is the parameter passed to navigation in the activated event handler.

            if (options && options.searchDetails) {
                WinJS.log && WinJS.log("Activated for Search with the query: " + options.searchDetails.queryText, "sample", "status");
            } else {
                WinJS.log && WinJS.log("Use the search pane to submit a query.", "sample", "status");
            }

            // Code for participating in the Search Contract and handling the user's search queries can be found in default.js.
            // Code must be placed in the global scope so that it can receive user queries at any time.
            // You also need to add the Search declaration in the package.appxmanifest to support the Search Contract.
        },

        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.
        }
    });
})();
