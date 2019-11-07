//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var isPaused = true;
    var page = WinJS.UI.Pages.define("/html/custom-icons.html", {
        ready: function (element, options) {
            document.getElementById("cmdAudio").addEventListener("click", doClickAudio, false);
            document.getElementById("cmdPlay").addEventListener("click", doClickPlay, false);
            document.getElementById("cmdAccept").addEventListener("click", doClickAccept, false);
            WinJS.log && WinJS.log("To show the bar, swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, tap in the application, swipe, right-click, or press Windows Logo + z again.", "sample", "status");
        },
        unload: function () {
            AppBarSampleUtils.removeAppBars();
        }
    });

    // Command button functions
    function doClickAudio() {
        var cmd = document.getElementById('cmdAudio');
        WinJS.log && WinJS.log("Song details button pressed", "sample", "status");
    }
    function doClickPlay() {
        var cmd = document.getElementById('cmdPlay');
        // Still need to change icon and label when selected - if selected, set label, etc.
        if (!isPaused) {
            isPaused = true; // paused
            cmd.winControl.icon = 'play';
            cmd.winControl.label = 'Play';
            cmd.winControl.tooltip = 'Play this song';
            WinJS.log && WinJS.log("Play button pressed.", "sample", "status");
        } else {
            isPaused = false; // playing
            cmd.winControl.icon = 'pause';
            cmd.winControl.label = 'Pause';
            cmd.winControl.tooltip = 'Pause this song';
            WinJS.log && WinJS.log("Pause button pressed.", "sample", "status");
        }
    }
    function doClickAccept() {
        WinJS.log && WinJS.log("Accept button pressed", "sample", "status");
    }

})();
