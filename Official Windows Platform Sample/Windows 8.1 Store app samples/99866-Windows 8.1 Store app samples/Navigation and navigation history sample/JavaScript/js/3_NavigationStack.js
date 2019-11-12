//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/3_NavigationStack.html", {
        ready: function (element, options) {
            WinJS.Utilities.query(".saveState", element)
                .listen("click", this.saveState.bind(this));

            WinJS.Utilities.query(".restoreState", element)
                .listen("click", this.restoreState.bind(this));
        },

        saveState: function () {
            // Clone the history so that we have a snapshot of the history as it is
            // now, not a reference to the live history object. This is only for
            // the sample, in regular use you'd save into session state directly.
            var navState = {
                backStack: WinJS.Navigation.history.backStack.slice(0),
                forwardStack: WinJS.Navigation.history.forwardStack.slice(0),
                current: WinJS.Navigation.history.current
            };

            // Save history to state - simple!
            WinJS.Application.sessionState.navigationHistory = navState;

            WinJS.log && WinJS.log("Navigation state has been saved", "sample", "status");
        },

        restoreState: function () {
            // Reset history from session state

            var savedState = WinJS.Application.sessionState.navigationHistory;

            if (savedState) {
                WinJS.Navigation.history = savedState;

                // You will notice - this does not trigger navigation itself, all it does
                // is reset the data structure. If you want to trigger navigation, you'll
                // need to add code to do that to your application.

                WinJS.log && WinJS.log("History reloaded from session", "sample", "status");
            } else {
                WinJS.log && WinJS.log("No saved history to reload", "sample", "status");
            }
        }
    });
})();
