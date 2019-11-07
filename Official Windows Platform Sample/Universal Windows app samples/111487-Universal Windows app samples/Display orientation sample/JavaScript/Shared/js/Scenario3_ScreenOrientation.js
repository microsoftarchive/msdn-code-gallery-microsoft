//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var displayInformation = Windows.Graphics.Display.DisplayInformation.getForCurrentView();
    var page = WinJS.UI.Pages.define("/html/Scenario3_ScreenOrientation.html", {
        ready: function (element, options) {
            addOrientationChangeListener();
            updateScreenForRotation(displayInformation.currentOrientation);
        },
        unload: function () {
            removeOrientationChangeListener();
        }
    });

    function addOrientationChangeListener() {
        displayInformation.addEventListener("orientationchanged", orientationChangeListener, false);
    }

    function removeOrientationChangeListener() {
        displayInformation.removeEventListener("orientationchanged", orientationChangeListener, false);
    }

    function orientationChangeListener(eventObject) {
        updateScreenForRotation(eventObject.target.currentOrientation);
    }

    var displayOrientations = Windows.Graphics.Display.DisplayOrientations;
    function updateScreenForRotation(orientation) {
        var textOut = document.getElementById("screenOrientation");

        switch (orientation) {
            case displayOrientations.landscape:
                textOut.innerText = "Landscape";
                break;
            case displayOrientations.portrait:
                textOut.innerText = "Portrait";
                break;
            case displayOrientations.landscapeFlipped:
                textOut.innerText = "Landscape (flipped)";
                break;
            case displayOrientations.portraitFlipped:
                textOut.innerText = "Portrait (flipped)";
                break;
            default:
                // This should never happen.
                textOut.innerText = "Unknown";
                break;
        }
    }

})();
