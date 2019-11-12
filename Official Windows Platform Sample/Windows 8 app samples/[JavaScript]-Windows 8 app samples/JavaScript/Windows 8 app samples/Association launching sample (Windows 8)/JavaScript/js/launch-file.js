//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/launch-file.html", {
        ready: function (element, options) {
            document.getElementById("launchFileButton").addEventListener("click", launchFile, false);
            document.getElementById("launchFileWithWarningButton").addEventListener("click", launchFileWithWarning, false);
            document.getElementById("launchFileOpenWithButton").addEventListener("click", launchFileOpenWith, false);
            document.getElementById("pickAndLaunchFileButton").addEventListener("click", pickAndLaunchFile, false);
        }
    });

    // Path to the file to launch, relative to the package's install directory.
    var fileToLaunch = "images\\Icon.targetsize-256.png";

    // Launch a .png file that came with the package.
    function launchFile() {
        // First, get the image file from the package's image directory.
        Windows.ApplicationModel.Package.current.installedLocation.getFileAsync(fileToLaunch).done(
            function (file) {
                // Now launch the retrieved file.
                Windows.System.Launcher.launchFileAsync(file).done(
                    function (success) {
                        if (success) {
                            WinJS.log && WinJS.log("File " + file.name + " launched.", "sample", "status");
                        } else {
                            WinJS.log && WinJS.log("File launch failed.", "sample", "error");
                        }
                    });
            });
    }

    // Show warning dialog that the file may be unsafe. Then launch the file.
    function launchFileWithWarning() {
        // First, get the image file from the package's image directory.
        Windows.ApplicationModel.Package.current.installedLocation.getFileAsync(fileToLaunch).done(
            function (file) {
                // Set the show warning option.
                var options = new Windows.System.LauncherOptions();
                options.treatAsUntrusted = true;

                // Now launch the retrieved file.
                Windows.System.Launcher.launchFileAsync(file, options).done(
                    function (success) {
                        if (success) {
                            WinJS.log && WinJS.log("File " + file.name + " launched.", "sample", "status");
                        } else {
                            WinJS.log && WinJS.log("File launch failed.", "sample", "error");
                        }
                    });
            });
    }

    // Show OpenWith dialog to let the user pick the applicaton to handle the .png file. Then launch the file.
    function launchFileOpenWith() {
        // First, get the image file from the package's image directory.
        Windows.ApplicationModel.Package.current.installedLocation.getFileAsync(fileToLaunch).done(
            function (file) {
                // Set the show picker option.
                var options = new Windows.System.LauncherOptions();
                options.displayApplicationPicker = true;

                // Position the Open With dialog so that it aligns with the button.
                // An alternative to using the rect is to set a point indicating where the dialog is supposed to be shown.
                options.ui.selectionRect = getSelectionRect(document.getElementById("launchFileOpenWithButton"));
                options.ui.preferredPlacement = Windows.UI.Popups.Placement.below;

                // Now launch the retrieved file.
                Windows.System.Launcher.launchFileAsync(file, options).done(
                    function (success) {
                        if (success) {
                            WinJS.log && WinJS.log("File " + file.name + " launched.", "sample", "status");
                        } else {
                            WinJS.log && WinJS.log("File launch failed.", "sample", "error");
                        }
                    });
            });
    }

    // Have the user pick a file, then launch it.
    function pickAndLaunchFile() {
        // First, get a file via the picker.
        // To use the picker, the sample must not be snapped.
        if (Windows.UI.ViewManagement.ApplicationView.value === Windows.UI.ViewManagement.ApplicationViewState.snapped) {
            if (!Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
                WinJS.log && WinJS.log("Unable to unsnap the sample.", "sample", "error");
                return;
            }
        }

        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.fileTypeFilter.replaceAll(["*"]);

        openPicker.pickSingleFileAsync().done(
            function (file) {
                if (file) {
                    // Now launch the picked file.
                    Windows.System.Launcher.launchFileAsync(file).done(
                        function (success) {
                            if (success) {
                                WinJS.log && WinJS.log("File " + file.name + " launched.", "sample", "status");
                            } else {
                                WinJS.log && WinJS.log("File launch failed.", "sample", "error");
                            }
                        });
                }
                else {
                    WinJS.log && WinJS.log("No file was picked.", "sample", "error");
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
