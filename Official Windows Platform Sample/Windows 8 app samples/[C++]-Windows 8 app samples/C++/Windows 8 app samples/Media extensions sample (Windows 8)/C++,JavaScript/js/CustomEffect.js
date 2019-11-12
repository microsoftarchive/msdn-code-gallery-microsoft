//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="..\base-sdk.js" />

(function () {
    "use strict";

    function grayscaleEffectStart() {
        // Set video element's source
        var vid = WinJS.Utilities.query("#customEffectVideo")[0],
            src = null;

        if (vid.readyState !== 0) {
            src = vid.src;
        }
        vid.pause();
        vid.msClearEffects();
        // Installing video effect without any configuration
        vid.msInsertVideoEffect("GrayscaleTransform.GrayscaleEffect", true, null);
        if (src) {
            vid.src = src;
        } else {
            SdkSample.pickMediaFile([".mp4", ".wmv", ".avi"], [vid]);
        }
    }
    
    function invertEffectStart() {
        // Set video element's source
        var vid = WinJS.Utilities.query("#customEffectVideo")[0],
            src = null;

        if (vid.readyState !== 0) {
            src = vid.src;
        }
        vid.pause();
        vid.msClearEffects();
        // Installing video effect without any configuration
        vid.msInsertVideoEffect("InvertTransform.InvertEffect", true, null);
        if (src) {
            vid.src = src;
        } else {
            SdkSample.pickMediaFile([".mp4", ".wmv", ".avi"], [vid]);
        }
    }

    function getPolarEffectStartFunc(effectName) {
        return function () {
            // Set video element's source
            var vid = WinJS.Utilities.query("#customEffectVideo")[0],
                src = null;

            if (vid.readyState !== 0) {
                src = vid.src;
            }
            vid.pause();
            // Installing video effect with configuration in property set.
            var effect = new Windows.Foundation.Collections.PropertySet();
            effect["effect"] = effectName;
            vid.msClearEffects();
            vid.msInsertVideoEffect("PolarTransform.PolarEffect", true, effect);
            if (src) {
                vid.src = src;
            } else {
                SdkSample.pickMediaFile([".mp4", ".wmv", ".avi"], [vid]);
            }
        };
    }

    function customEffectStop() {
        var vid = WinJS.Utilities.query("#customEffectVideo")[0];
        vid.suppressErrors = true;
        vid.src = null;
    }

    var page = WinJS.UI.Pages.define("/html/CustomEffect.html", {
        ready: function (element, options) {
	    WinJS.Utilities.query("#invertEffectStart").listen("click", invertEffectStart);
            WinJS.Utilities.query("#grayscaleEffectStart").listen("click", grayscaleEffectStart);
            WinJS.Utilities.query("#fisheyeEffectStart").listen("click", getPolarEffectStartFunc("Fisheye"));
            WinJS.Utilities.query("#pinchEffectStart").listen("click", getPolarEffectStartFunc("Pinch"));
            WinJS.Utilities.query("#warpEffectStart").listen("click", getPolarEffectStartFunc("Warp"));
            WinJS.Utilities.query("#customEffectStop").listen("click", customEffectStop);
            WinJS.Utilities.query("#customEffectVideo").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            customEffectStop();
        }
    });
})();
