//// Copyright (c) Microsoft Corporation. All rights reserved

// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/verticallayout.html", {
        processed: function (element, options) {
            this._hub = element.querySelector(".win-hub").winControl;
            this._listview = element.querySelector(".ZoomedOutListView").winControl;
            this._onHeaderInvokedBound = this.onHeaderInvoked.bind(this);
            this._onResizeBound = this.onResize.bind(this);
            window.addEventListener("headerinvoked", this._onHeaderInvokedBound);
            window.addEventListener("resize", this._onResizeBound);
            updateHubLayout(this._hub, this._listview);
        },

        // This function is called whenever a user navigates to this page. 
        ready: function (element, options) {

            // Reset the scroll position from the history state.
            var oldScrollPosition = WinJS.Navigation.state && WinJS.Navigation.state.oldScrollPosition;
            if (oldScrollPosition) {
                this._hub.scrollPosition = oldScrollPosition;
            }
        },

        //navigate to deeper levels by invoking interactive headers
        onHeaderInvoked: function (ev) {
            var index = ev.detail.index;
            var section = ev.detail.section;
            var hub = this._hub;

            // Store the scroll position so it can be retrieved if user comes "back" to this page.
            WinJS.Navigation.state = WinJS.Navigation.state || {};
            WinJS.Navigation.state.oldScrollPosition = hub.scrollPosition;

            //check that the correct section is invoked
            if (index === 2) {
                WinJS.Navigation.navigate("/pages/listview.html");
            }

            if (index === 3) {
                WinJS.Navigation.navigate("/pages/video.html");
            }
        },

        //go into vertical state when width is less than 500px
        //a developer can choose to go into vertical state when height of the app is greater than the width of the app
        onResize: function () {
            if (document.body.contains(this._hub.element)) {
                updateHubLayout(this._hub, this._listview);
                return this._hub.orientation;
            } else {
                return;
            }
        },

        unload: function () {
            // Respond to navigations away from this page.
            window.removeEventListener("headerinvoked", this._onHeaderInvokedBound);
            window.removeEventListener("resize", this._onResizeBound);
        }
    });

    function updateHubLayout(hub, listview) {
        if (document.body.clientWidth < 500) {
            if (hub.orientation !== WinJS.UI.Orientation.vertical) {
                hub.orientation = WinJS.UI.Orientation.vertical;
                listview.layout = new WinJS.UI.ListLayout();
            }
        }
        else {
            if (hub.orientation !== WinJS.UI.Orientation.horizontal) {
                hub.orientation = WinJS.UI.Orientation.horizontal;
                listview.layout = new WinJS.UI.GridLayout();
            }
        }
    }
})();

