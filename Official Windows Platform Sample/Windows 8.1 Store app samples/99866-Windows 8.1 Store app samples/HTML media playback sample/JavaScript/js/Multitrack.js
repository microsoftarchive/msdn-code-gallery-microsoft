//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="..\base-sdk.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Multitrack.html", {
        ready: function (element, options) {
            WinJS.Utilities.query("#multitrackStart").listen("click", multitrackStart);
            WinJS.Utilities.query("#multitrackStartChannel1").listen("click", getMultitrackFunction("en"));
            WinJS.Utilities.query("#multitrackStartChannel2").listen("click", getMultitrackFunction("hi"));
            WinJS.Utilities.query("#multitrackStartChannel3").listen("click", getMultitrackFunction("zh"));
            WinJS.Utilities.query("#multitrackStartChannel4").listen("click", getMultitrackFunction("pl"));
            WinJS.Utilities.query("#multitrackStop").listen("click", multitrackStop);
            WinJS.Utilities.query("#multitrackVideo").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            multitrackStop();
        }
    });

    function multitrackStart() {
        SdkSample.displayError("");
        var vid = WinJS.Utilities.query("#multitrackVideo")[0];
        vid.play();
    }

    function getMultitrackFunction(channel) {
        return function () {
            SdkSample.displayError("");
            var vid = WinJS.Utilities.query("#multitrackVideo")[0];
            for (var ch = 0; ch < vid.audioTracks.length; ch++) {
                vid.audioTracks[ch].enabled = (vid.audioTracks[ch].language === channel);
            }
        };
    }

    function multitrackStop() {
        var vid = WinJS.Utilities.query("#multitrackVideo")[0];
        if (!vid.paused) {
            vid.pause();
        }
    }
})();
