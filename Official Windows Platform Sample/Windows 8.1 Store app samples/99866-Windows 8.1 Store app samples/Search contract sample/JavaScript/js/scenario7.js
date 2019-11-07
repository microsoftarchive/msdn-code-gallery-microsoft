//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario7.html", {
        processed: function (element, options) {
            WinJS.log && WinJS.log("Use the search pane to submit a query.", "sample", "status");

            // Code for participating in the Search contract and handling the user's search queries can be found in default.js.
            // Code must be placed in the global scope so that it can receive user queries at any time.
            // You also need to add the Search declaration in the package.appxmanifest to support the Search contract.
        },

        ready: function (element, options) {
            // Turn on type to search.
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().showOnKeyboardInput = true;
        },

        unload: function () {
            // Turn off type to search.
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().showOnKeyboardInput = false;
    }
});
})();
