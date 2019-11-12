//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Stereo3D.html", {
        ready: function (element, options) {
            WinJS.Utilities.query("#play").listen("click", play);
            WinJS.Utilities.query("#stop").listen("click", stopPlayback);
            WinJS.Utilities.query("#stereoOn").listen("click", stereoOn);
            WinJS.Utilities.query("#stereoOff").listen("click", stereoOff);
            WinJS.Utilities.query("#stereo3DVideo").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            stopPlayback();
        }
    });

    function play() {
        WinJS.Utilities.query("#stereo3DVideo")[0].play();
    }

    function stopPlayback() {
        var video = WinJS.Utilities.query("#stereo3DVideo")[0];

        if (!video.paused) {
            video.pause();
        }
    }

    function stereoOn() {
        var video = WinJS.Utilities.query("#stereo3DVideo")[0];
        if (Windows.Graphics.Display.DisplayInformation.getForCurrentView().stereoEnabled) {
            video.msStereo3DRenderMode = "stereo";
        } else {
            SdkSample.displayError("Your system currently doesn't support stereo display.");
        }
    }

    function stereoOff() {
        var video = WinJS.Utilities.query("#stereo3DVideo")[0];

        video.msStereo3DRenderMode = "mono";
    }
})();
