//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/2_NavigationEvents.html", {
        init: function (element, options) {
            // Hold onto bound copies of our navigation callback
            // functions so that we can clean it up when navigated
            // away from.
            this.beforenavigate = this.beforenavigate.bind(this);
            this.navigating = this.navigating.bind(this);
            this.navigated = this.navigated.bind(this);
        },

        ready: function (element, options) {
            WinJS.Utilities.query("a", this.element)
                .listen("click", this.navigationClicked.bind(this));

            // Hook up to our navigation handlers
            WinJS.Navigation.addEventListener("beforenavigate",
                this.beforenavigate);

            WinJS.Navigation.addEventListener("navigating",
                this.navigating);

            WinJS.Navigation.addEventListener("navigated",
                this.navigated);
        },

        navigationClicked: function (eventObject) {
            // Event handler for when a link is clicked in this page. Route
            // the call through to WinJS.Navigation.navigate instead of
            //using normal browser navigation.

            var destUrl = this.getRelativeUrl(eventObject.target.href);

            // Prevent the default browser navigation
            eventObject.preventDefault();

            // Kick off a navigation
            WinJS.log && WinJS.log("Navigating to " + destUrl, "sample", "status");
            WinJS.Navigation.navigate(destUrl, this.getNavState());
        },

        getRelativeUrl: function (absUrl) {
            // Strip the protocol and appx name off the url retrieved from the
            // a tag. As used in this sample, the code is expecting relative
            // urls. This is only a requirement of the sample, in practice
            // the navigation framework location can be any arbitrary string.

            var parts = absUrl.split("/");
            return "/" + parts.slice(3).join("/");
        },

        //
        // Functions that return the status of fields on our UI
        //
        shouldPreventNavigation: {
            get: function () {
                var checkbox = this.element.querySelector("#navigationEventsPreventNavigation");
                return checkbox.checked;
            }
        },

        doAsyncBeforeNavigating: {
            get: function () {
                var checkbox = this.element.querySelector("#navigationEventsDoAsyncWork");
                return checkbox.checked;
            }
        },

        getNavState: function () {
            var stateObject = { data: "" };

            var input = this.element.querySelector("#navigationEventsNavigationState");
            if (input.value) {
                stateObject.data = toStaticHTML(input.value);
                return stateObject;
            }
            return null;
        },

        //
        // Our navigation event handlers
        //

        beforenavigate: function (eventObject) {
            // This function gives you a chance to veto navigation. This demonstrates that capability
            if (this.shouldPreventNavigation) {
                WinJS.log && WinJS.log("Navigation to " + eventObject.detail.location + " was prevented", "sample", "status");
                eventObject.preventDefault();
            }
        },

        navigating: function (eventObject) {
            // This function gives you the ability to prepare for navigating away from your page.
            // One of the things you can do is complete some async work before letting navigation
            // continue. This function demonstrates this capability.

            if (this.doAsyncBeforeNavigating) {
                WinJS.log && WinJS.log("Completing 5 second async operation before navigating to " + eventObject.detail.location,
                    "sample", "status");
                eventObject.detail.setPromise(
                    // Pass a promise object to the navigation toolkit. The navigation operation will
                    // not complete until this promise does.
                    WinJS.Promise.timeout(5000)
                    );
            }
        },

        navigated: function (eventObject) {
            // This event handler is called after navigation is complete.
            //
            // Due to the use of the navigation framework by the sample itself, this implementation doesn't
            // actually change the screen contents; it's included here simply to demonstrate registration.
            // See the event handler in default.js for an example of how to swap out the contents of your
            // screen as a result of navigation.
            WinJS.log && WinJS.log("Navigation to " + eventObject.detail.location + " completed",
                "sample", "status");
        },

        unload: function () {
            // Called by sample framework when navigating away from a scenario. It's very important to
            // disconnect event handlers to global objects like the navigation framework when no longer
            // needed. If you don't, you end up leaking memory.

            WinJS.Navigation.removeEventListener("beforenavigate", this.beforenavigate);
            WinJS.Navigation.removeEventListener("navigating", this.navigating);
            WinJS.Navigation.removeEventListener("navigated", this.navigated);
        }
    });

})();
