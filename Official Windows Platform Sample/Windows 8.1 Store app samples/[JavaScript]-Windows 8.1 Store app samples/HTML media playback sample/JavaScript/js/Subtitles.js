//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Subtitles.html", {
        ready: function (element, options) {
            WinJS.Utilities.query("#play").listen("click", play);
            WinJS.Utilities.query("#stop").listen("click", stopPlayback);
            WinJS.Utilities.query("#subtitleOn").listen("click", subtitleOn);
            WinJS.Utilities.query("#subtitleOff").listen("click", subtitleOff);
            WinJS.Utilities.query("#subtitleVideo").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            stopPlayback();
        }
    });

    function play() {
        WinJS.Utilities.query("#subtitleVideo")[0].play();
    }

    function stopPlayback() {
        var video = WinJS.Utilities.query("#subtitleVideo")[0];

        if (!video.paused) {
            video.pause();
        }
    }

    function subtitleOn() {
        var video = WinJS.Utilities.query("#subtitleVideo")[0];

        video.textTracks[0].mode = TextTrack.SHOWING;
    }

    function subtitleOff() {
        var video = WinJS.Utilities.query("#subtitleVideo")[0];

        video.textTracks[0].mode = TextTrack.OFF;
    }
})();
