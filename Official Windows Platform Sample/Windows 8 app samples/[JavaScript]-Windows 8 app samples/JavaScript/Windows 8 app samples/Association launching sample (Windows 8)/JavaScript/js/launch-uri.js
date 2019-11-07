//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/launch-uri.html", {
        ready: function (element, options) {
            document.getElementById("launchUriButton").addEventListener("click", launchUri, false);
            document.getElementById("launchUriWithWarningButton").addEventListener("click", launchUriWithWarning, false);
            document.getElementById("launchUriOpenWithButton").addEventListener("click", launchUriOpenWith, false);
        }
    });

    // URI to launch. The URI must contain the protocol scheme.
    var uriToLaunch = "http://www.bing.com";

    // Launch a URI.
    function launchUri() {
        // Create the URI to launch from a string.
        var uri = new Windows.Foundation.Uri(uriToLaunch);

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
        var uri = new Windows.Foundation.Uri(uriToLaunch);

        // Set the show warning option.
        var options = new Windows.System.LauncherOptions();
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
        var uri = new Windows.Foundation.Uri(uriToLaunch);

        // Set the show picker option.
        var options = new Windows.System.LauncherOptions();
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
