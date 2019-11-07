//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var photoKey = "capturedPhoto";
    var localSettings = Windows.Storage.ApplicationData.current.localSettings;
    
    var page = WinJS.UI.Pages.define("/html/capturephoto.html", {
        ready: function (element, options) {
            document.getElementById("captureButton").addEventListener("click", capturePhoto, false);
            document.getElementById("resetButton").addEventListener("click", reset, false);
            document.getElementById("resetButton").style.visibility = "hidden";

            if (localSettings.values[photoKey]) {
                document.getElementById("captureButton").disabled = true;
                reloadPhoto();
            }
        }
    });

    function capturePhoto() {
        WinJS.log && WinJS.log("", "sample", "status");

        // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
        var dialog = new Windows.Media.Capture.CameraCaptureUI();
        var aspectRatio = { width: 1, height: 1 };
        dialog.photoSettings.croppedAspectRatio = aspectRatio;
        dialog.captureFileAsync(Windows.Media.Capture.CameraCaptureUIMode.photo).done(function (file) {
            if (file) {
                var photoBlobUrl = URL.createObjectURL(file, { oneTimeOnly: true });
                document.getElementById("capturedPhoto").src = photoBlobUrl;
                document.getElementById("resetButton").style.visibility = "visible";
                localSettings.values[photoKey] = file.path;
            } else {
                WinJS.log && WinJS.log("No photo captured.", "sample", "status");
            }
        }, function (err) {
            WinJS.log && WinJS.log(err, "sample", "error");
        });
    }

    function reset() {
        WinJS.log && WinJS.log("", "sample", "status");
        document.getElementById("capturedPhoto").src = "/images/placeholder-sdk.png";
        document.getElementById("resetButton").style.visibility = "hidden";
        localSettings.values.remove(photoKey);
    }

    function reloadPhoto() {
        // Resume user's state
        Windows.Storage.StorageFile.getFileFromPathAsync(localSettings.values[photoKey]).done(function (file) {
            document.getElementById("capturedPhoto").src = URL.createObjectURL(file, { oneTimeOnly: true });
            document.getElementById("resetButton").style.visibility = "visible";
            document.getElementById("captureButton").disabled = false;
        }, function (err) {
            localSettings.values.remove(photoKey);
            document.getElementById("captureButton").disabled = false;
            WinJS.log && WinJS.log(err, "sample", "error");
        });
    }
})();
