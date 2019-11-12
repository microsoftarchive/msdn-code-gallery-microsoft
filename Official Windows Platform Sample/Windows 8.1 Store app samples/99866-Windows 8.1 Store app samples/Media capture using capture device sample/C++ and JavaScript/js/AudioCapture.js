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
    var userRequestedRaw = false;
    var rawAudioSupported = false;


    var page = WinJS.UI.Pages.define("/html/AudioCapture.html", {
        ready: function (element, options) {
            var systemMediaControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            systemMediaControls.addEventListener("propertychanged", mediaPropertyChangedHandler, false);
            scenarioInitialize();
        },

        unload: function (element, options) {
            var systemMediaControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            systemMediaControls.removeEventListener("propertychanged", mediaPropertyChangedHandler, false);
        }
    });


    // The most reliable way to have a listener that fires an event when media applications are no longer visible
    // is to use the sound level events. When an application becomes invisible to the user, its playback sound will be muted.
    function mediaPropertyChangedHandler(e) {
        if (e.property === Windows.Media.SystemMediaTransportControlsProperty.soundLevel) {
            if (e.target.soundLevel === Windows.Media.SoundLevel.muted) {
                if (recordState) {
                    stopRecord();
                }
                releaseMediaCapture();
            }
            else {
                scenarioInitialize();
            }
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

    function startAudioCapture() {
        mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
        mediaCaptureMgr.addEventListener("failed", errorHandler, false);

        captureInitSettings.mediaCategory = Windows.Media.Capture.MediaCategory.other;
        captureInitSettings.audioProcessing = (rawAudioSupported && userRequestedRaw) ? Windows.Media.AudioProcessing.raw : Windows.Media.AudioProcessing.default;

        mediaCaptureMgr.initializeAsync(captureInitSettings).done(function (result) {
            id("btnStartDevice" + scenarioId).disabled = true;
            id("btnRecord" + scenarioId).disabled = false;
            displayStatus("Device started");
            mediaCaptureMgr.addEventListener("recordlimitationexceeded", recordLimitationExceededEventHandler);
            mediaCaptureMgr.addEventListener("failed", failedEventHandler);
        },
errorHandler);
    }

    function startDevice() {
        //Initialize media capture with the settings
        userRequestedRaw = id("recordRawAudio").checked.valueOf() ? true : false;
        id("recordRawAudio").disabled = true;
        displayStatus("Starting device");
        releaseMediaCapture();
        startAudioCapture();
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
        rawAudioSupported = false;
        userRequestedRaw = false;
        initCaptureSettings();

        // Read system's raw audio stream support
        var propertiesToRetrieve = new Array();
        // Property stating whether the device supports RAW mode
        propertiesToRetrieve.push("System.Devices.AudioDevice.RawProcessingSupported");
        var DevInfo = Windows.Devices.Enumeration.DeviceInformation;
        var WMDM = Windows.Media.Devices.MediaDevice;
        if (DevInfo) {
            try {
                DevInfo.createFromIdAsync(
                    WMDM.getDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Communications),
                    propertiesToRetrieve).then(function (device) {
                        var prop = device.properties;
                        if (prop["System.Devices.AudioDevice.RawProcessingSupported"]) {
                            rawAudioSupported = true;
                            id("recordRawAudio").disabled = false;
                            displayStatus("Raw audio recording is supported");
                        }
                        else {
                            rawAudioSupported = false;
                            displayStatus("Raw audio recording is not supported");
                        }
                    });
            }
            catch (e) {
                displayStatus(e.message);
            }
        }
        else {
            displayStatus("Cannot get device information on raw audio streaming support");
        }
    }


})();
