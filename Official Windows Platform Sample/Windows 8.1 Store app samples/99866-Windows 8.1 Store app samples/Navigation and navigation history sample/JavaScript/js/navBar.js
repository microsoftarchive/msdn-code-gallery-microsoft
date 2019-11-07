//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    // This page control manages the back and forward buttons that appear in the output
    // section of each scenario page.

    var page = WinJS.UI.Pages.define("/html/navBar.html", {
        ready: function (element, options) {
            this.backButton = element.querySelector(".back");
            this.forwardButton = element.querySelector(".forward");

            this.setCanNavigateYesNo();
            this.enableForwardButton();
        },

        setCanNavigateYesNo: function () {
            // This method checks the current status of the navigation stack
            // to see if there's any place to navigate forward or back on it.
            // If so, sets the "yes/no" text on the screen to match the
            // state for that flag.

            this.element.querySelector(".canGoBackYesNoText").textContent =
                WinJS.Navigation.canGoBack ? "Yes" : "No";

            this.element.querySelector(".canGoForwardYesNoText").textContent =
                WinJS.Navigation.canGoForward ? "Yes" : "No";
        },

        enableForwardButton: function () {
            // This function checks the current forward state of the
            // navigation state. If the direction can be used, it will enable
            // the corresponding button and hook up an event listener to the
            // button to do the navigation;

            if (WinJS.Navigation.canGoForward) {
                WinJS.Utilities.query(".forward", this.element)
                    .listen("click", this.onForward.bind(this))
                    .forEach(function (e) {
                        e.removeAttribute("disabled");
                    });
            }
        },

        onForward: function () {
            // This function is called when the forward button is pressed.
            // Tell the navigation framework to go forward;

            WinJS.Navigation.forward();
        }
    });

    WinJS.Namespace.define("NavigationSample", {
        NavBar: page
    });
})();
