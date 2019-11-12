//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/2_CustomFragmentLoad.html", {
        ready: function (element, options) {
            this.customFragmentLoadDiv = element.querySelector("#customFragmentLoadDiv");

            WinJS.Utilities.query("#customFragmentLoadButton", element)
                .listen("click", this.customFragmentLoad.bind(this));
        },

        customFragmentLoad: function () { 
            var that = this;

            this.resetOutput();

            // Read fragment from the HTML file and load it into the div.  Note
            // WinJS.UI.Fragments.renderCopy() returns a promise which a then() statement
            // is attached to in order to perform additional processing or handle errors
            // that may have occurred during the renderCopy() action.
            WinJS.UI.Fragments.renderCopy("/html/1_BasicFragmentLoad_fragment.html")
                .done(function (fragment) {
                    // Append the fragment to the div content
                    that.customFragmentLoadDiv.appendChild(fragment);
                    WinJS.log && WinJS.log("successfully loaded fragment", "sample", "status");

                    // Alternatively, you can do custom operations on the fragment such as
                    // retrieving data from a specific element:
                    //     var x = fragment.querySelector("basicFragmentLoad_test").innerHTML;
                },
                function (error) {
                    WinJS.log && WinJS.log("error loading fragment: " + error, "sample", "error");
                }
            );

        },

        resetOutput: function () {
            this.customFragmentLoadDiv.innerHTML = "";
        }
    });

    function doSomething1() {
        WinJS.log && WinJS.log("Error message here", "sample", "error");
    }

    function doSomething2() {
        WinJS.log && WinJS.log("Show status here", "sample", "status");
    }
})();
