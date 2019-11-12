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
            // Notify the ListView to calculate its layout
            var listView = element.querySelector("#listView").winControl;

            function swapLayout() {
                if (listView.layout instanceof WinJS.UI.GridLayout) {

                    // Create a new List Layout
                    listView.layout = new WinJS.UI.ListLayout();

                    // Apply an alternate CSS class to the ListView
                    WinJS.Utilities.toggleClass(listView.element, "listLayout");

                    // Update button and print output
                    element.querySelector("#swapLayoutButton").innerText = "Switch to grid";
                    WinJS.log && WinJS.log("You switched to List layout!", "sample", "status");

                } else {

                    // Create a new Grid Layout
                    listView.layout = new WinJS.UI.GridLayout();

                    // Apply an alternate CSS class to the ListView
                    WinJS.Utilities.toggleClass(listView.element, "listLayout");

                    // Update button and print output
                    element.querySelector("#swapLayoutButton").innerText = "Switch to list";
                    WinJS.log && WinJS.log("You switched to Grid layout!", "sample", "status");
                }
            }

            element.querySelector("#swapLayoutButton").addEventListener("click", swapLayout, false);
        }
    });
})();
