//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/1_BasicFragmentLoad.html", {
        ready: function (element, options) {
            this.basicFragmentLoadDiv = element.querySelector("#basicFragmentLoadDiv");

            WinJS.Utilities.query("#basicFragmentLoadButton", element)
                .listen("click", this.basicFragmentLoad.bind(this));
        },

        basicFragmentLoad: function () {
            this.resetOutput();

            // Read fragment from the HTML file and load it into the div.  Note
            // Fragments.renderCopy() returns a promise which a done() statement
            // is attached to in order to perform additional processing or handle errors
            // that may have occurred during the cloneTo() action.
            WinJS.UI.Fragments.renderCopy("/html/1_BasicFragmentLoad_Fragment.html",
                this.basicFragmentLoadDiv)
                .done(
                    function () {
                        WinJS.log && WinJS.log("successfully loaded fragment", "sample", "status");
                    },
                    function (error) {
                        WInJS.log && WinJS.log("error loading fragment: " + error, "sample", "error");
                    }
                );
        },

        resetOutput: function () {
            this.basicFragmentLoadDiv.innerHTML = "";
        }

    });

})();
