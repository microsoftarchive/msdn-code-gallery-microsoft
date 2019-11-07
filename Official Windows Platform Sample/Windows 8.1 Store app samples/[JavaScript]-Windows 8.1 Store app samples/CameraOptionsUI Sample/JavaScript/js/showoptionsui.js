//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var mediaCaptureMgr = null;
    var startPreviewButton = null;
    var showSettingsButton = null;
    var systemMediaControls = null;
    var previewStarted = false;

    var page = WinJS.UI.Pages.define("/html/showoptionsui.html", {
        ready: function (element, options) {
            startPreviewButton = document.getElementById("startPreviewButton");
            startPreviewButton.addEventListener("click", initializeMediaCapture, false);
            startPreviewButton.disabled = false;

            showSettingsButton = document.getElementById("showSettingsButton");
            showSettingsButton.addEventListener("click", showSettings, false);
            showSettingsButton.style.visibility = "hidden";

            // Using the soundlevelchanged event to determine when the app can stream from the webcam
            systemMediaControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            systemMediaControls.addEventListener("propertychanged", systemMediaControlsPropertyChanged, false);
        },

        unload: function () {
            if (systemMediaControls) {
                systemMediaControls.removeEventListener("propertychanged", mediaPropertyChanged, false);
                systemMediaControls = null;
            }
        }
    });

    function initializeMediaCapture() {
        // Check if the machine has a webcam
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(Windows.Devices.Enumeration.DeviceClass.videoCapture)
            .done(function (devices) {
                if (devices.length > 0) {
                    WinJS.log && WinJS.log("", "sample", "error");

                    // Using Windows.Media.Capture.MediaCapture APIs to stream from webcam
                    mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                    mediaCaptureMgr.initializeAsync().done(initializeComplete, initializeError);
                } else {
                    WinJS.log && WinJS.log("A webcam is required to run this sample.", "sample", "error");
                }
            });
    }

    function initializeComplete(op) {
        startPreview();
    }

    function initializeError(op) {
        WinJS.log && WinJS.log(op.message, "sample", "error");
    }

    function startPreview() {
        document.getElementById("previewTag").src = URL.createObjectURL(mediaCaptureMgr);
        document.getElementById("previewTag").play();
        startPreviewButton.disabled = true;
        showSettingsButton.style.visibility = "visible";
        previewStarted = true;
    }

    function showSettings() {
        if (mediaCaptureMgr) {
            // Using Windows.Windows.Media.Capture.CameraOptionsUI API to show webcam settings
            Windows.Media.Capture.CameraOptionsUI.show(mediaCaptureMgr);
        }
    }

    function systemMediaControlsPropertyChanged(e) {
        if (e.property === Windows.Media.SystemMediaTransportControlsProperty.soundLevel) {
            if (e.target.soundLevel === Windows.Media.SoundLevel.muted) {
                if (previewStarted) {
                    mediaCaptureMgr = null;
                    document.getElementById("previewTag").src = null;
                    previewStarted = false;
                }
            }
            else {
                if (!previewStarted) {
                    startPreviewButton.disabled = false;
                    showSettingsButton.style.visibility = "hidden";
                }
            }
        }
    }
})();
