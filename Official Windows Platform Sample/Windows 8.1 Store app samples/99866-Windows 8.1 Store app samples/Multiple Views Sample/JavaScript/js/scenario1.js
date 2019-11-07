//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ViewManagement = Windows.UI.ViewManagement;

    // Set up the UI for this page
    var indexToViewSizeMap =
        [
            ViewManagement.ViewSizePreference.default,
            ViewManagement.ViewSizePreference.useHalf,
            ViewManagement.ViewSizePreference.useLess,
            ViewManagement.ViewSizePreference.useMinimum,
            ViewManagement.ViewSizePreference.useMore,
            ViewManagement.ViewSizePreference.useNone
        ];

    var defaultTitle = "New window";

    var viewSelect;
    var sizePreferenceChooser, anchorSizePreferenceChooser;
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("createViewButton").addEventListener("click", createView, false);
            document.getElementById("showAsStandaloneButton").addEventListener("click", showAsStandalone, false);
            document.getElementById("closeViewButton").addEventListener("click", closeView, false);

            sizePreferenceChooser = document.getElementById("sizePreferenceChooser");
            anchorSizePreferenceChooser = document.getElementById("anchorSizePreferenceChooser");
            sizePreferenceChooser.selectedIndex = 0;
            anchorSizePreferenceChooser.selectedIndex = 0;

            viewSelect = document.querySelector(".view-select").winControl;
        }
    });

    function createView() {
        // Set up the secondary view, but don't show it yet.
        // The actual creation of the view is done in viewLifetimeControl.js.
        MultipleViews.manager.createNewView("ms-appx:///html/secondaryView.html", { title: defaultTitle });
    }

    function showAsStandalone() {
        var view;
        viewSelect.selection.getItems().then(function (items) {
            var sizePreference = indexToViewSizeMap[sizePreferenceChooser.selectedIndex];
            var anchorSizePreference = indexToViewSizeMap[anchorSizePreferenceChooser.selectedIndex];

            // Show the selected view. If the user selected more than one, show the first one.
            if (items.length > 0) {
                view = items[0].data;

                // Prevent the view from closing while switching to it. See ViewLifetimeControl for details
                view.startViewInUse();

                // Show the previously created secondary view, using the size
                // preferences the user specified. In your app, you should
                // choose a size that's best for your scenario and code it,
                // instead of requiring the user to decide.
                return ViewManagement.ApplicationViewSwitcher.tryShowAsStandaloneAsync(
                    view.viewId,
                    sizePreference,
                    ViewManagement.ApplicationView.getForCurrentView().id,
                    anchorSizePreference
                    );
            }
            return WinJS.Promise.wrap();
        }).done(function (shown) {
            if (view) {
                view.stopViewInUse();
            }
            if (!shown) {
                WinJS.log && WinJS.log("Please choose a view to show, a size preference for each view", "sample", "error");
            }
        });
    }

    function closeView() {
        // Callback when the async tasks below complete
        function closeCompletion() {
            if (view) {
                view.stopViewInUse();
            } else {
                WinJS.log && WinJS.log("Please choose a view to close", "sample", "error");
            }
        }

        var view;
        viewSelect.selection.getItems().then(function (items) {
            // Close the selected view. If the user selected more than one, show the first one.
            if (items.length > 0) {
                view = items[0].data;
                view.startViewInUse();

                // A view may be on screen at the time you try to close it. To make sure the user
                // isn't left with a blank screen, you should bring up another view in its place.
                // Passing in the "consolidateViews" parameter effectively removes the view being
                // closed from the user's control. 
                return ViewManagement.ApplicationViewSwitcher.switchAsync(
                    ViewManagement.ApplicationView.getForCurrentView().id,
                    view.viewId,
                    ViewManagement.ApplicationViewSwitchingOptions.consolidateViews
                    );
            }
            return WinJS.Promise.wrap();
        }).done(closeCompletion, closeCompletion);
    }
})();
