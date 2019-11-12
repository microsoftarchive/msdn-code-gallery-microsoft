//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var isPaused = true;
    var appBar;

    var page = WinJS.UI.Pages.define("/html/custom-icons.html", {
        ready: function (element, options) {
            appBar = document.getElementById("customIconsAppBar").winControl;
            appBar.getCommandById("cmdShuffle").addEventListener("click", doClickShuffle, false);
            appBar.getCommandById("cmdPlay").addEventListener("click", doClickPlay, false);
            appBar.getCommandById("cmdAccept").addEventListener("click", doClickAccept, false);
        }
    });

    // Command button functions
    function doClickShuffle() {
        var cmd = appBar.getCommandById('cmdShuffle');
        WinJS.log && WinJS.log("Shuffle button pressed", "sample", "status");
    }
    function doClickPlay() {
        var cmd = appBar.getCommandById('cmdPlay');
        // Still need to change icon and label when selected - if selected, set label, etc.
        if (!isPaused) {
            isPaused = true; // paused
            cmd.icon = 'play';
            cmd.label = 'Play';
            cmd.tooltip = 'Play this song';
            WinJS.log && WinJS.log("Pause button pressed.", "sample", "status");
        } else {
            isPaused = false; // playing
            cmd.icon = 'pause';
            cmd.label = 'Pause';
            cmd.tooltip = 'Pause this song';
            WinJS.log && WinJS.log("Play button pressed.", "sample", "status");
        }
    }
    function doClickAccept() {
        WinJS.log && WinJS.log("Accept button pressed", "sample", "status");
    }

})();
