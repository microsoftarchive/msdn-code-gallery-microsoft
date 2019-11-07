//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/3_FragmentsWithControls.html", {
        ready: function (element, options) {
            this.fragmentsWithControlsDiv = element.querySelector("#fragmentsWithControlsDiv");

            WinJS.Utilities.query("#fragmentsWithControlsButton", element)
                .listen("click", this.fragmentsWithControls.bind(this));
        },

        fragmentsWithControls: function () {
            var that = this;
            this.fragmentsWithControlsDiv.innerHTML = "";

           // Read fragment from the HTML file and load it into the div.  Note
            // WinJS.UI.Fragments.renderCopy() returns a promise which we attach a done() call
            // in order to perform additional processing or handle errors that may have
            // occurred during the renderCopy() action.
            // Passing the DOM element as the second argument will get renderCopy to parent
            // the fragment to that DOM element automatically.
            WinJS.UI.Fragments.renderCopy("/html/3_FragmentsWithControls_Fragment.html",
                this.fragmentsWithControlsDiv)
                .done(function() {
                    // After the fragment is loaded into the target element,
                    // WinJS.UI.processAll() needs to be called to activate the
                    // controls and process options records.
                    WinJS.UI.processAll(that.fragmentsWithControlsDiv);
                    WinJS.log && WinJS.log("successfully loaded fragment", "sample", "status");
                },

                function(error) {
                    WinJS.log && WinJS.log("error loading fragment: " + error, "sample", "error");
                }
            );
        }
    });
})();
