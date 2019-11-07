//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_LaunchUri.html", {
        processed: function () {
            // Detect whether the platform is Windows Phone. If so, hide UI elements that are for scenarios not supported.
            // The platform detection is done by checking the desiredRemainingView property in options. 
            // It is not available on Windows Phone platform.
            var options = new Windows.System.LauncherOptions();
            if (!('desiredRemainingView' in options)) {
                launchUriWithWarningSection.hidden = true;
                launchUriOpenWithSection.hidden = true;
                launchUriSplitScreenSection.hidden = true;
                viewState.hidden = true;
            }
        },

        ready: function () {
            document.getElementById("launchUriButton").addEventListener("click", launchUri, false);
            document.getElementById("launchUriWithWarningButton").addEventListener("click", launchUriWithWarning, false);
            document.getElementById("launchUriOpenWithButton").addEventListener("click", launchUriOpenWith, false);
            document.getElementById("launchUriSplitScreenButton").addEventListener("click", launchUriSplitScreen, false);
        }
    });

    // Launch a URI.
    function launchUri() {
        // Create the URI to launch from a string.
        var uri = new Windows.Foundation.Uri(document.getElementById("uriToLaunch").value);

        // Launch the URI.
        Windows.System.Launcher.launchUriAsync(uri).done(
            function (success) {
                if (success) {
                    WinJS.log && WinJS.log("URI " + uri.absoluteUri + " launched.", "sample", "status");
                } else {

                    WinJS.log && WinJS.log("URI launch failed.", "sample", "error");
                }
            });
    }

    // Show warning dialog that the URI may be unsafe. Then launch the URI.
    function launchUriWithWarning() {
        // Create the URI to launch from a string.
        var uri = new Windows.Foundation.Uri(document.getElementById("uriToLaunch").value);

        var options = new Windows.System.LauncherOptions();
        // Set the show warning option.
        options.treatAsUntrusted = true;

        // Launch the URI.
        Windows.System.Launcher.launchUriAsync(uri, options).done(
            function (success) {
                if (success) {
                    WinJS.log && WinJS.log("URI " + uri.absoluteUri + " launched.", "sample", "status");
                } else {

                    WinJS.log && WinJS.log("URI launch failed.", "sample", "error");
                }
            });
    }

    // Show OpenWith dialog to let the user pick the applicaton to handle the URI. Then launch the URI.
    function launchUriOpenWith() {
        // Create the URI to launch from a string.
        var uri = new Windows.Foundation.Uri(document.getElementById("uriToLaunch").value);

        var options = new Windows.System.LauncherOptions();
        // Set the show picker option.
        options.displayApplicationPicker = true;

        // Position the Open With dialog so that it aligns with the button.
        // An alternative to using the rect is to set a point indicating where the dialog is supposed to be shown.
        options.ui.selectionRect = getSelectionRect(document.getElementById("launchUriOpenWithButton"));
        options.ui.preferredPlacement = Windows.UI.Popups.Placement.below;

        // Launch the URI.
        Windows.System.Launcher.launchUriAsync(uri, options).done(
            function (success) {
                if (success) {
                    WinJS.log && WinJS.log("URI " + uri.absoluteUri + " launched.", "sample", "status");
                } else {

                    WinJS.log && WinJS.log("URI launch failed.", "sample", "error");
                }
            });
    }

    // Launch the URI. Request that this app and the launched app share the screen.
    function launchUriSplitScreen() {
        // Create the URI to launch from a string.
        var uri = new Windows.Foundation.Uri(document.getElementById("uriToLaunch").value);

        var options = new Windows.System.LauncherOptions();
        // Request to share the screen.
        if (document.getElementById("viewState").selectedIndex === 0) {
            options.desiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.default;
        } else if (document.getElementById("viewState").selectedIndex === 1) {
            options.desiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.useLess;
        } else if (document.getElementById("viewState").selectedIndex === 2) {
            options.desiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.useHalf;
        } else if (document.getElementById("viewState").selectedIndex === 3) {
            options.desiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.useMore;
        } else if (document.getElementById("viewState").selectedIndex === 4) {
            options.desiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.useMinimum;
        } else if (document.getElementById("viewState").selectedIndex === 5) {
            options.desiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.useNone;
        }

        // Launch the URI.
        Windows.System.Launcher.launchUriAsync(uri, options).done(
            function (success) {
                if (success) {
                    WinJS.log && WinJS.log("URI " + uri.absoluteUri + " launched.", "sample", "status");
                } else {

                    WinJS.log && WinJS.log("URI launch failed.", "sample", "error");
                }
            });
    }

    function getSelectionRect(element) {
        var selectionRect = element.getBoundingClientRect();

        var rect = {
            x: getClientCoordinates(selectionRect.left),
            y: getClientCoordinates(selectionRect.top),
            width: getClientCoordinates(selectionRect.width),
            height: getClientCoordinates(selectionRect.height)
        };

        return rect;
    }

    function getClientCoordinates(cssUnits) {
        // Translate css coordinates to system coordinates.
        return cssUnits * (96 / window.screen.deviceXDPI);
    }
})();
