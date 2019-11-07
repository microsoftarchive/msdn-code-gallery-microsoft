//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="..\base-sdk.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Playback.html", {
        ready: function (element, options) {
            WinJS.Utilities.query("#playMP4").listen("click", getStartVideoFuntion("http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4"));
            WinJS.Utilities.query("#playMP3").listen("click", getStartVideoFuntion("http://testdriveie9.wise.glbdns.microsoft.com/ietestdrivecontent/Musopen.Com Symphony No. 5 in C Minor, Op. 67 - I. Allegro con brio.mp3"));
            WinJS.Utilities.query("#playFromFile").listen("click", playFromFile);
            WinJS.Utilities.query("#playbackStop").listen("click", playbackStop);
            WinJS.Utilities.query("#playbackVideo").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            playbackStop();
        }
    });

    function getStartVideoFuntion(src) {
        return function () {
            // Set video element's source
            var vid = WinJS.Utilities.query("#playbackVideo")[0];
            SdkSample.displayError("");
            vid.src = src;
            vid.play();
        };
    }

    function playFromFile() {
        var vid = WinJS.Utilities.query("#playbackVideo")[0],
            Pickers = Windows.Storage.Pickers,
            picker = new Pickers.FileOpenPicker();
        picker.fileTypeFilter.append("*");
        picker.suggestedStartLocation = Pickers.PickerLocationId.videosLibrary;
        picker.pickSingleFileAsync().done(
            function (/*@override*/item) {
                if (item !== null) {
                    SdkSample.displayError("");
                    vid.src = URL.createObjectURL(item, { oneTimeOnly: true });
                    vid.play();
                }
            }
        );
    }

    function playbackStop() {
        var vid = WinJS.Utilities.query("#playbackVideo")[0];
        if (!vid.paused) {
            vid.pause();
        }
    }
})();
