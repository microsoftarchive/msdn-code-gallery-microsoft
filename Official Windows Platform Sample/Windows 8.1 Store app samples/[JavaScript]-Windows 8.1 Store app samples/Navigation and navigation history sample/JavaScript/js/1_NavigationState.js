//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    // This scenario has code that deals with the navigation history.
    // See also the navBar.js file for code for implementing forward
    // and back navigation.

    var page = WinJS.UI.Pages.define("/html/1_NavigationState.html", {
        ready: function (element, options) {
            this.showNavigationHistory();
        },

        showNavigationHistory: function () {
            // This function looks at the WinJS.Navigation.history member. This
            // contains the current navigation stack, along with any state
            // passed to the navigate function.

            var tableBody = this.element.querySelector(".navigationStateOutput tbody");
            var rowTemplate = this.element.querySelector(".rowTemplate").winControl;
            var rowData;
            var entry;
            var i;
            var len;

            // The navigation history contains three variables:
            // backStack: Array of previous locations in navigation history.
            // current: The current location
            // forwardStack: Array of locations you could go forward to. Erased on new navigation.

            // We start with the back stack. Walk through it, displaying the
            // contents from back to front.

            for (i = 0, len = WinJS.Navigation.history.backStack.length; i < len; ++i) {
                entry = WinJS.Navigation.history.backStack[i];
                rowData = {
                    rowClass: 'backStack',
                    index: len - i,
                    location: entry.location,
                    state: this.stateToString(entry.state)
                };

                rowTemplate.render(rowData, tableBody);
            }

            // Next, add a row for the current location
            rowData = {
                rowClass: 'current',
                index: 'current',
                location: WinJS.Navigation.history.current.location,
                state: this.stateToString(WinJS.Navigation.history.current.state)
            };
            rowTemplate.render(rowData, tableBody);

            // And finally, the forward stack. We traverse in the opposite order;
            // since the array is used as a stack (with the pop & push methods),
            // we need to traverse in the opposite direction, since the top of
            // the stack is actually at the end of the array.

            len = WinJS.Navigation.history.forwardStack.length;
            for (i = len - 1; i >= 0; --i) {
                entry = WinJS.Navigation.history.forwardStack[i];
                rowData = {
                    rowClass: 'forwardStack',
                    index: len - i,
                    location: entry.location,
                    state: this.stateToString(entry.state)
                };

                rowTemplate.render(rowData, tableBody);
            }
        },

        addRow: function (tableBody, rowData) {
            this.rowTemplate = this.rowTemplate ||
                this.element.querySelector(".rowTemplate").winControl;
            this.rowTemplate.render(rowData, tableBody);
        },

        stateToString: function (state) {
            if (state) {
                return JSON.stringify(state);
            }
            return "";
        }
    });
})();
