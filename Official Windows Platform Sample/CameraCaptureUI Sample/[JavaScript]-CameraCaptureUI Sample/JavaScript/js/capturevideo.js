//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var videoKey = "capturedVideo";
    var localSettings = Windows.Storage.ApplicationData.current.localSettings;

    var page = WinJS.UI.Pages.define("/html/capturevideo.html", {
        ready: function (element, options) {
            document.getElementById("captureButton").addEventListener("click", captureVideo, false);
            document.getElementById("resetButton").addEventListener("click", reset, false);
            document.getElementById("resetButton").style.visibility = "hidden";

            if (localSettings.values[videoKey]) {
                document.getElementById("captureButton").disabled = true;
                reloadVideo();
            }
        }
    });

    function captureVideo() {
        WinJS.log && WinJS.log("", "sample", "status");

        // Using Windows.Media.Capture.CameraCaptureUI API to capture a video
        var dialog = new Windows.Media.Capture.CameraCaptureUI();
        dialog.videoSettings.format = Windows.Media.Capture.CameraCaptureUIVideoFormat.mp4;
        dialog.captureFileAsync(Windows.Media.Capture.CameraCaptureUIMode.video).done(function (file) {
            if (file) {
                var videoBlobUrl = URL.createObjectURL(file, {oneTimeOnly: true});
                document.getElementById("capturedVideo").src = videoBlobUrl;
                document.getElementById("resetButton").style.visibility = "visible";
                localSettings.values[videoKey] = file.path;
            } else {
                WinJS.log && WinJS.log("No video captured.", "sample", "status");
            }
        }, function (err) {
            WinJS.log && WinJS.log(err, "sample", "error");
        });
    }

    function reset() {
        WinJS.log && WinJS.log("", "sample", "status");
        document.getElementById("capturedVideo").src = "";
        document.getElementById("resetButton").style.visibility = "hidden";
        localSettings.values.remove(videoKey);
    }

    function reloadVideo() {
        // Resume user's state
        Windows.Storage.StorageFile.getFileFromPathAsync(localSettings.values[videoKey]).done(function (file) {
            document.getElementById("capturedVideo").src = URL.createObjectURL(file, { oneTimeOnly: true });
            document.getElementById("resetButton").style.visibility = "visible";
            document.getElementById("captureButton").disabled = false;            
        }, function (err) {
            localSettings.values.remove(videoKey);
            document.getElementById("captureButton").disabled = false;
            WinJS.log && WinJS.log(err, "sample", "error");
        });
    }
})();
