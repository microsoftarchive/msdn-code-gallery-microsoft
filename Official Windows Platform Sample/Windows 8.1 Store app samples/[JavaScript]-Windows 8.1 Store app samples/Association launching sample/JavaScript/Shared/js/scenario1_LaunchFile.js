//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_LaunchFile.html", {
        processed: function () {
            // Detect whether the platform is Windows Phone. If so, hide UI elements that are for scenarios not supported.
            // The platform detection is done by checking the desiredRemainingView property in options. 
            // It is not available on Windows Phone platform.
            var options = new Windows.System.LauncherOptions();
            if (!('desiredRemainingView' in options)) {
                launchFileWithWarningSection.hidden = true;
                launchFileOpenWithSection.hidden = true;
                launchFileSplitScreenSection.hidden = true;
                viewState.hidden = true;
            }
        },

        ready: function (element, options) {
            document.getElementById("launchFileButton").addEventListener("click", launchFile, false);
            document.getElementById("launchFileWithWarningButton").addEventListener("click", launchFileWithWarning, false);
            document.getElementById("launchFileOpenWithButton").addEventListener("click", launchFileOpenWith, false);
            document.getElementById("launchFileSplitScreenButton").addEventListener("click", launchFileSplitScreen, false);
            document.getElementById("pickAndLaunchFileButton").addEventListener("click", pickAndLaunchFile, false);

            if (options && options.activationKind === Windows.ApplicationModel.Activation.ActivationKind.pickFileContinuation) {
                continueFileOpenPicker(options.activatedEventArgs);
            }
        }
    });

    // Path to the file to launch, relative to the package's install directory.
    var fileToLaunch = "images\\microsoft-sdk.png";

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
                var options = new Windows.System.LauncherOptions();
                // Set the show warning option.
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
                var options = new Windows.System.LauncherOptions();
                // Set the show picker option.                    
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

    // Launch the file. Request that this app and the launched app share the screen.
    function launchFileSplitScreen() {
        // First, get a file via the picker.
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.fileTypeFilter.replaceAll(["*"]);

        openPicker.pickSingleFileAsync().done(
            function (file) {
                if (file) {
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

                    // Now launch the picked file.
                    Windows.System.Launcher.launchFileAsync(file, options).done(
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

    // Have the user pick a file, then launch it.
    function pickAndLaunchFile() {
        // First, get a file via the picker.
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.fileTypeFilter.replaceAll(["*"]);

        if (!('pickSingleFileAndContinue' in openPicker)) {
            openPicker.pickSingleFileAsync().done(
                function (file) {
                    if (file) {
                        // Request to share the screen.
                        var options = new Windows.System.LauncherOptions();

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

                        // Now launch the picked file.
                        Windows.System.Launcher.launchFileAsync(file, options).done(
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
        else {
            // Windows phone platform
            openPicker.pickSingleFileAndContinue();
        }
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

    function continueFileOpenPicker(eventObject) {
        var files = eventObject[0].files;
        var filePicked = files.size > 0 ? files[0] : null;
        if (filePicked !== null) {
            // Now launch the picked file.
            Windows.System.Launcher.launchFileAsync(filePicked).done(
                function (success) {
                    if (success) {
                        WinJS.log && WinJS.log("File " + filePicked.name + " launched.", "sample", "status");
                    } else {
                        WinJS.log && WinJS.log("File launch failed.", "sample", "error");
                    }
                });
        } else {
            // The picker was dismissed with no selected file
            WinJS.log && WinJS.log("No file was picked.", "sample", "error");
        }
    }
})();
