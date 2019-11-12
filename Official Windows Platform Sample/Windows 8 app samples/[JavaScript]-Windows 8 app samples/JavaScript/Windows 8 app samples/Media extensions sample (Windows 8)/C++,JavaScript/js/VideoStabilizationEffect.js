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
        vidStab.msInsertVideoEffect(
            Windows.Media.VideoEffects.videoStabilization,
            true,
            null);

        vidStab.oncanplaythrough = function () {
            // Start both videos at the same time
            vid.play();
            vidStab.play();
            vidStab.oncanplaythrough = null;
        };

        SdkSample.pickMediaFile([".mp4", ".wmv", ".avi"], [vid, vidStab]);
    }

    function videoStabilizationStop() {
        var vid = WinJS.Utilities.query("#videoStabilizationNormal")[0];
        vid.suppressErrors = true;
        vid.src = null;

        var vidStab = WinJS.Utilities.query("#videoStabilizationStabilized")[0];
        vidStab.suppressErrors = true;
        vidStab.src = null;
    }

    var page = WinJS.UI.Pages.define("/html/VideoStabilizationEffect.html", {
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
