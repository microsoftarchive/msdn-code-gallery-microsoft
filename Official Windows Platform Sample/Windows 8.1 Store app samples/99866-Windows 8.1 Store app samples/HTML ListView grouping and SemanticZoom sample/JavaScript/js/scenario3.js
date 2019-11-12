//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            var listView = element.querySelector('#listView').winControl;

            // Group Header invoked handler
            function groupHeaderInvokedHandler(eventObject) {
                eventObject.detail.groupHeaderPromise.done(function (invokedGroupHeader) {

                    // Access item data from the groupHeaderPromise
                    WinJS.log && WinJS.log("The group header at index " + invokedGroupHeader.index + " is "
                        + invokedGroupHeader.data.groupTitle, "sample", "status");
                });
            }

            // Item invoked handler
            function itemInvokedHandler(eventObject) {
                eventObject.detail.itemPromise.done(function (invokedItem) {

                    // Access item data from the itemPromise
                    WinJS.log && WinJS.log("The item at index " + invokedItem.index + " is "
                        + invokedItem.data.title + " with a text value of "
                        + invokedItem.data.text, "sample", "status");
                });
            }

            listView.addEventListener("groupheaderinvoked", groupHeaderInvokedHandler, false);
            listView.addEventListener("iteminvoked", itemInvokedHandler, false);
        }
    });
})();
