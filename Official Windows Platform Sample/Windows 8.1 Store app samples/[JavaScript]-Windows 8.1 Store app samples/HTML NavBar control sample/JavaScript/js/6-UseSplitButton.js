//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var navcontainer;

    var page = WinJS.UI.Pages.define("/html/6-UseSplitButton.html", {
        ready: function (element, options) {
            document.body.querySelector('#useSplit').addEventListener('invoked', this.navbarInvoked.bind(this));
            document.body.querySelector('#contactNavBarContainer').addEventListener('invoked', this.navbarInvoked.bind(this));

            var navBarContainerEl = document.body.querySelector('#useSplit .globalNav');
            if (navBarContainerEl) {
                this.setupNavBarContainer();
            } else {
                var navBarEl = document.getElementById('useSplit');
                navBarEl.addEventListener('childrenprocessed', this.setupNavBarContainer.bind(this));
            }
        },

        navbarInvoked: function (ev) {
            var navbarCommand = ev.detail.navbarCommand;
            WinJS.log && WinJS.log(navbarCommand.label + " NavBarCommand invoked", "sample", "status");
            document.querySelector('select').focus();
        },

        setupNavBarContainer: function () {
            var navBarContainerEl = document.body.querySelector('#useSplit .globalNav');

            navBarContainerEl.addEventListener("splittoggle", function (e) {
                var flyout = document.getElementById("contactFlyout").winControl;
                var navbarCommand = e.detail.navbarCommand;
                if (e.detail.opened) {
                    flyout.show(navbarCommand.element);
                    var subNavBarContainer = flyout.element.querySelector('.win-navbarcontainer');
                    if (subNavBarContainer) {
                        // Switching the navbarcontainer from display none to display block requires forceLayout in case there was a pending measure.
                        subNavBarContainer.winControl.forceLayout();
                        // Reset back to the first item:
                        subNavBarContainer.currentIndex = 0;
                    }
                    flyout.addEventListener('beforehide', go);
                } else {
                    flyout.removeEventListener('beforehide', go);
                    flyout.hide();
                }
                function go() {
                    flyout.removeEventListener('beforehide', go);
                    navbarCommand.splitOpened = false;
                }
            });
        }
    });
})();
