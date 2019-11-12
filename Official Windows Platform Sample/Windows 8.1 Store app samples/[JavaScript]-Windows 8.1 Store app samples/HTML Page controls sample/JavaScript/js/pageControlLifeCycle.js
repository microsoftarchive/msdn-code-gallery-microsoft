//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.0.6/js/base.js" />
/// <reference path="//Microsoft.WinJS.0.6/js/ui.js" />

(function () {
    "use strict";
    WinJS.UI.Pages.define("/html/pageControlLifecycle.html", {
        ready: function (element, options) {
            var that = this;
            WinJS.Utilities.query("#normal", element).listen("click",
                function () { that._createControl(); });
        },

        _createControl: function () {
            var that = this;

            // Disable both our buttons while our controls are being created
            this._enableButtons(false);

            // We're going to swap out the contents of our control host div.
            // First, we'll check for the existence of an "unload" method
            // on the control, and call it if defined. unload isn't part of
            // the page control system itself, but you can add arbitrary methods
            // to a page control and call them however you'd like

            var host = this.element.querySelector("#controlHost");
            if (host.winControl && host.winControl.unloadExample) {
                host.winControl.unloadExample();
            }

            // Clear out the previous DOM from our host...
            WinJS.Utilities.empty(host);

            // And render the new one
            WinJS.UI.Pages.render("/pages/advancedControl.html", host)
                .then(
                    function () { that._enableButtons(true); },
                    function () { that._enableButtons(true); }
                );
        },

        // Helper function - enable or disable the buttons
        _enableButtons: function (enabled) {
            WinJS.Utilities.query("button", this.element).forEach(function (b) { b.disabled = !enabled; });
        }

    });

})();
