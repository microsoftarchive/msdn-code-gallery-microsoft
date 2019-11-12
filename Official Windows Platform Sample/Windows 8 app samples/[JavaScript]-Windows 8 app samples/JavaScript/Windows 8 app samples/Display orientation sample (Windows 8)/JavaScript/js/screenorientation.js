//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//var scenarioOrientation = {};

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/screenorientation.html", {
        ready: function (element, options) {
            addOrientationChangeListener();
            updateScreenForRotation();
        },
        unload: function () {
            removeOrientationChangeListener();
        }
    });

    function addOrientationChangeListener() {
        // Register for Orientation change event
        var dispProp = Windows.Graphics.Display.DisplayProperties;
        dispProp.addEventListener("orientationchanged", updateScreenForRotation, false);
    }

    function removeOrientationChangeListener() {
        // Un-Register for Orientation change event
        var dispProp = Windows.Graphics.Display.DisplayProperties;
        dispProp.removeEventListener("orientationchanged", updateScreenForRotation, false);
    }

    function updateScreenForRotation() {
        var textOut;
        textOut = document.getElementById("screenOrientation");

        // do something based on the curent orientation
        switch (Windows.Graphics.Display.DisplayProperties.currentOrientation) {
            case Windows.Graphics.Display.DisplayOrientations.landscape:
                textOut.innerText = "Landscape";
                break;
            case Windows.Graphics.Display.DisplayOrientations.portrait:
                textOut.innerText = "Portrait";
                break;
            case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
                textOut.innerText = "Landscape (flipped)";
                break;
            case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
                textOut.innerText = "Portrait (flipped)";
                break;
            default:
                // this should never happen
                textOut.innerText = "Unknown";
                break;
        }
    }

})();
