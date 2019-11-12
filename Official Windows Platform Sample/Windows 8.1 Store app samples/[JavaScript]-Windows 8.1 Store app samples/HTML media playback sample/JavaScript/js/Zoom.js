//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Zoom.html", {
        ready: function (element, options) {
            WinJS.Utilities.query("#zoomIn").listen("click", zoomIn);
            WinJS.Utilities.query("#zoomOut").listen("click", zoomOut);
            WinJS.Utilities.query("#zoomVideo1").listen("error", SdkSample.videoOnError);
            WinJS.Utilities.query("#zoomVideo2").listen("error", SdkSample.videoOnError);

            WinJS.Utilities.query("#zoomVideo1")[0].play();
            WinJS.Utilities.query("#zoomVideo2")[0].play();
        },

        unload: function () {
            stopPlayback();
        }
    });

    function zoomIn() {
        WinJS.Utilities.query("#zoomVideo1")[0].msZoom = true;
        WinJS.Utilities.query("#zoomVideo2")[0].msZoom = true;
    }

    function zoomOut() {
        WinJS.Utilities.query("#zoomVideo1")[0].msZoom = false;
        WinJS.Utilities.query("#zoomVideo2")[0].msZoom = false;
    }

    function stopPlayback() {
        var vid1 = WinJS.Utilities.query("#zoomVideo1")[0],
            vid2 = WinJS.Utilities.query("#zoomVideo2")[0];
        if (!vid1.paused) {
            vid1.pause();
        }
        if (!vid2.paused) {
            vid2.pause();
        }
    }
})();
