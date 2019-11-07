//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="..\base-sdk.js" />

(function () {
    "use strict";

    function videoStabilizationStart() {
        var vid = WinJS.Utilities.query("#videoStabilizationNormal")[0],
            vidStab = WinJS.Utilities.query("#videoStabilizationStabilized")[0];

        vidStab.msClearEffects();
        vidStab.muted = true;
        try {
            vidStab.msInsertVideoEffect(
                Windows.Media.VideoEffects.videoStabilization,
                true,
                null);
        } catch (err) {
            // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED)
            if (err.number === -2147024846) {
                WinJS.log("Video Stabilization not supported.", "", "error");
            } else {
                WinJS.log("Video Stabilization error.", "", "error");
            }
            return;
        }

        vidStab.oncanplaythrough = function () {
            // Start both videos at the same time
            vid.play();
            vidStab.play();
            vidStab.oncanplaythrough = null;
        };

        SdkSample.pickMediaFile([".mp4", ".wmv", ".avi"], [vid, vidStab], true /*oneTimeOnly*/);
    }

    function videoStabilizationStop() {
        var vid = WinJS.Utilities.query("#videoStabilizationNormal")[0];
        vid.removeAttribute("src");

        var vidStab = WinJS.Utilities.query("#videoStabilizationStabilized")[0];
        vidStab.removeAttribute("src");
    }

    var page = WinJS.UI.Pages.define("/html/scenario3_VideoStabilizationEffect.html", {
        extensions: null,

        ready: function (element, options) {
            WinJS.Utilities.query("#videoStabilizationStart").listen("click", videoStabilizationStart);
            WinJS.Utilities.query("#videoStabilizationStop").listen("click", videoStabilizationStop);
            WinJS.Utilities.query("#videoStabilizationNormal").listen("error", SdkSample.videoOnError);
            WinJS.Utilities.query("#videoStabilizationStabilized").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            videoStabilizationStop();
        }
    });
})();
