//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    "use strict";
    var mediaCaptureMgr = null;
    var recordState = null;
    var captureInitSettings = null;
    var encodingProfile = null;
    var storageFile = null;
    var scenarioId = "3";

    var page = WinJS.UI.Pages.define("/html/AudioCapture.html", {
        ready: function (element, options) {
            Windows.Media.MediaControl.addEventListener("soundlevelchanged", soundLevelChangedHandler, false);
            scenarioInitialize();
        },

        unload: function (element, options) {
            Windows.Media.MediaControl.removeEventListener("soundlevelchanged", soundLevelChangedHandler, false);
        }
    });


    function soundLevelChangedHandler() {
        if (Windows.Media.MediaControl.soundLevel === Windows.Media.SoundLevel.muted) {
            if (recordState) {
                stopRecord();
            }
            releaseMediaCapture();
        }
        else {
            scenarioInitialize();
        }
    }

    function recordLimitationExceededEventHandler() {
        if (recordState) {
            displayStatus("Stopping record as max record duration was exceeded");
            stopRecord();
        }
    }

    function failedEventHandler(e) {
        displayError("Fatal error", e.message);
    }

    function displayStatus(statusText) {
        SdkSample.displayStatus(statusText);
    }

    function displayError(error) {
        SdkSample.displayError(error);
    }

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function initCaptureSettings() {
        //Initialize capture settings

        captureInitSettings = null;
        captureInitSettings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
        captureInitSettings.audioDeviceId = "";
        captureInitSettings.videoDeviceId = "";
        captureInitSettings.streamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.audio;
    }


    function releaseMediaCapture() {
        mediaCaptureMgr = null;
    }

    function startDevice() {
        //Initialize media capture with the settings
        displayStatus("Starting device");
        releaseMediaCapture();
        mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
        mediaCaptureMgr.addEventListener("failed", errorHandler, false);

        mediaCaptureMgr.initializeAsync(captureInitSettings).done(function (result) {
            id("btnStartDevice" + scenarioId).disabled = true;
            id("btnRecord" + scenarioId).disabled = false;

            displayStatus("Device started");
            mediaCaptureMgr.addEventListener("recordlimitationexceeded", recordLimitationExceededEventHandler);
            mediaCaptureMgr.addEventListener("failed", failedEventHandler);
        },
errorHandler);
    }

    function startRecord() {
        var btnRecord;
        displayStatus("Starting record ");
        btnRecord = id("btnRecord" + scenarioId);
        btnRecord.disabled = true;
        if (!recordState) {
            // Stop captured video playback.
            var video = id("capturePlayback" + scenarioId);
            if (!video.paused) {
                video.pause();
            }
            // Start recording.
            Windows.Storage.KnownFolders.videosLibrary.createFileAsync("cameraCapture.m4a", Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (newFile) {
                storageFile = newFile;
                encodingProfile = Windows.Media.MediaProperties.MediaEncodingProfile.createM4a(Windows.Media.MediaProperties.AudioEncodingQuality.auto);
                mediaCaptureMgr.startRecordToStorageFileAsync(encodingProfile, storageFile).done(function (result) {
                    recordState = true;
                    btnRecord.innerHTML = "Stop Record";
                    btnRecord.disabled = false;
                    displayStatus("Record Started");
                },
                errorHandler);
            },
            errorHandler);
        }
    }

    function stopRecord() {
        var btnRecord;
        displayStatus("Stopping record");
        btnRecord = id("btnRecord" + scenarioId);
        mediaCaptureMgr.stopRecordAsync().done(function (result) {
            recordState = false;
            btnRecord.innerHTML = "Start Record";
            btnRecord.disabled = false;
            displayStatus("Record Stopped.  File " + storageFile.path + "  ");

            // Playback the recorded video.
            var video = id("capturePlayback" + scenarioId);
            video.src = URL.createObjectURL(storageFile, { oneTimeOnly: true });
            video.play();
        },
        errorHandler);
    }

    function startStopRecord() {
        if (!recordState) {
            startRecord();
        } else {
            stopRecord();
        }
    }

    function errorHandler(err) {
        displayError(err.message);
    }


    function scenarioInitialize() {
        id("btnStartDevice" + scenarioId).disabled = false;
        id("btnStartDevice" + scenarioId).addEventListener("click", startDevice, false);
        id("btnRecord" + scenarioId).disabled = true;
        id("btnRecord" + scenarioId).addEventListener("click", startStopRecord, false);
        initCaptureSettings();
    }


})();
