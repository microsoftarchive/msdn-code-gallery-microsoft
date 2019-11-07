//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var Animation = WinJS.UI.Animation;
    var ViewManagement = Windows.UI.ViewManagement;

    var timeoutLength = 500;
    var lastFadeOutTime = 0;

    // Set up the UI for this page
    var viewSelect;

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("animatedSwitchButton").addEventListener("click", doAnimatedSwitch, false);
            document.addEventListener("visibilitychange", onVisibilityChange, false);
            WinJS.Navigation.addEventListener("navigated", onNavigated, false);
            viewSelect = document.querySelector(".view-select").winControl;
            viewSelect.itemDataSource = MultipleViews.manager.secondaryViews.dataSource;
        }
    });

    // Release listeners to avoid leaks
    function onNavigated(e) {
        if (e.detail.location !== "/html/scenario3.html") {
            document.removeEventListener("visibilitychange", onVisibilityChange, false);
            WinJS.Navigation.removeEventListener("navigated", onNavigated, false);
        }
    }

    function onVisibilityChange(e) {
        // Time out the animation if the secondary window fails to respond in 500
        // ms. Since this animation clears out the main view of the app, it's not desirable
        // to leave it unusable.
        var now = new Date();
        if (!document.hidden || now.getTime() - lastFadeOutTime > timeoutLength) {
            document.body.style.opacity = 1.0;
        }
    }

    function doAnimatedSwitch() {
        // The sample demonstrates a general strategy for doing custom animations when switching view
        // It's technically only possible to animate the contents of a particular view. But, it is possible
        // to animate the outgoing view to some common visual (like a blank background), have the incoming
        // view draw that same visual, switch in the incoming window (which will be imperceptible to the
        // user since both windows will be showing the same thing), then animate the contents of the incoming
        // view in from the common visual.
        var currentId = ViewManagement.ApplicationView.getForCurrentView().id;
        var shouldAnimate = false;
        var view = null;
        viewSelect.selection.getItems().then(function (items) {
            if (items.length > 0) {
                view = items[0].data;

                // Prevent the view from being closed while switching to it.
                view.startViewInUse();

                // Signal that the window is about to be switched to
                // If the view is already shown to the user, then the app
                // shouldn't run any extra animations.
                return ViewManagement.ApplicationViewSwitcher.prepareForCustomAnimatedSwitchAsync(
                    view.viewId,
                    currentId,
                    ViewManagement.ApplicationViewSwitchingOptions.skipAnimation);
            } else {
                WinJS.log && WinJS.log("Please choose a view to show", "sample", "error");
            }
            return WinJS.Promise.wrap();
        }).then(function (isViewVisible) {
            shouldAnimate = !!view && !isViewVisible;

            // The view may already be on screen, in which case there's no need to animate its
            // contents (or animate out the contents of the current window.)
            if (shouldAnimate) {
                // The view isn't visible, so animate it on screen.
                var now = new Date();
                lastFadeOutTime = now.getTime();

                // Make the current view totally blank. The incoming view is also
                // going to be totally blank when the two windows switch.
                return Animation.fadeOut(document.body);
            }
            return WinJS.Promise.wrap();
        }).done(function () {
            if (view) {
                if (shouldAnimate) {

                    // Once the current view is blank, switch in the other window.
                    view.appView.postMessage({
                        doAnimateAndSwitch: true
                    }, SecondaryViewsHelper.thisDomain);
                }
                view.stopViewInUse();
            }
        });
    }
})();
