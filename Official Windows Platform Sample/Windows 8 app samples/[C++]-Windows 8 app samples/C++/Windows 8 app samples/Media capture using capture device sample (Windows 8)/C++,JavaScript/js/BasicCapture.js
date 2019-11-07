//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    "use strict";
    var mediaCaptureMgr = null;
    var photoFile = "photo.jpg";
    var recordState = null;
    var captureInitSettings = null;
    var encodingProfile = null;
    var storageFile = null;
    var photoStorage = null;
    var cameraControlSliders = null;
    var scenarioId = "1";
    var brightness;
    var contrast;

    var page = WinJS.UI.Pages.define("/html/BasicCapture.html", {
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
        captureInitSettings.photoCaptureSource = Windows.Media.Capture.PhotoCaptureSource.videoPreview;
        captureInitSettings.streamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.audioAndVideo;
    }


    function releaseMediaCapture() {
        document.getElementById("previewVideo1").src = null;
        if (cameraControlSliders) {
            for (var i = 0; i < cameraControlSliders.length; i++) {
                cameraControlSliders[i].slider.removeEventListener("change", cameraControlSliders[i].handler, false);
                cameraControlSliders[i].control = null;
                cameraControlSliders[i].handler = null;
            }
        }
        mediaCaptureMgr = null;
        displayStatus("");
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

            id("btnSnap" + scenarioId).disabled = false;
            id("btnStartPreview" + scenarioId).disabled = false;            
            

            displayStatus("Device started");
            mediaCaptureMgr.addEventListener("recordlimitationexceeded", recordLimitationExceededEventHandler);
            mediaCaptureMgr.addEventListener("failed", failedEventHandler);
        },
errorHandler);
    }

    function initSlider(controlSlider) {
        var slider = controlSlider.slider;
        var control = controlSlider.control;
        var val = control.tryGetValue();
        slider.disabled = true;
        if (controlSlider.control) {
            if (control.capabilities.supported) {
                slider.min = control.capabilities.min;
                slider.max = control.capabilities.max;
                var valObject = control.tryGetValue();
                var v = control.tryGetAuto();
                if (valObject.succeeded) {
                    slider.value = valObject.value;
                    slider.addEventListener("change", controlSlider.handler, false);
                    slider.disabled = false;
                }
            }
        }
        control = null;
        slider = null;
    }

    function getCameraSettings() {
        //Set up brightness and contrast controls
        var controller = mediaCaptureMgr.videoDeviceController;
        
        
        cameraControlSliders = new Array(
        {
            control: controller.brightness, slider: id("rngBrightness")
        },
        {
            control: controller.contrast, slider: id("rngContrast")
        });
        for (var i = 0; i < cameraControlSliders.length; i++) {
            cameraControlSliders[i].handler = (function (_control, _slider) {
                return function () {
                    _control.trySetValue(parseInt(_slider.value));
                };
            })(cameraControlSliders[i].control, cameraControlSliders[i].slider);
        }
        controller = null;
    }

    function startPreview() {
        displayStatus("Starting preview");
        id("btnStartPreview" + scenarioId).disabled = true;
        var video = id("previewVideo" + scenarioId);
        video.src = URL.createObjectURL(mediaCaptureMgr, { oneTimeOnly: true });
        video.play();
        displayStatus("Preview started");
        getCameraSettings();
        // Initialize sliders.
        for (var i = 0; i < cameraControlSliders.length; i++) {
            cameraControlSliders[i].slider.disabled = false;
            initSlider(cameraControlSliders[i]);
        }
    }


    function capturePhoto() {
        id("btnSnap" + scenarioId).disabled = true;
        displayStatus("Taking photo");
        return Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(photoFile, Windows.Storage.CreationCollisionOption.generateUniqueName).then(
    function (newFile) {
        photoStorage = newFile;
        var photoProperties = Windows.Media.MediaProperties.ImageEncodingProperties.createJpeg();
        mediaCaptureMgr.capturePhotoToStorageFileAsync(photoProperties, photoStorage).done(
        function (result) {
            displayStatus("Photo Taken.  File " + photoStorage.path + "  ");
            var url = URL.createObjectURL(photoStorage, { oneTimeOnly: true });
            id("imageTag" + scenarioId).src = url;
            id("btnSnap" + scenarioId).disabled = false;
        },
        function capturePhotoError(error) {
            id("btnSnap" + scenarioId).disabled = false;
            displayStatus("An exception occurred trying to capturePhoto: " + error.message);
        });
    },
    function (error) {
        id("btnSnap" + scenarioId).disabled = false;
        displayStatus("capturePhoto async exception " + error.message);
    });
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
            Windows.Storage.KnownFolders.videosLibrary.createFileAsync("cameraCapture.mp4", Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (newFile) {
                storageFile = newFile;
                encodingProfile = Windows.Media.MediaProperties.MediaEncodingProfile.createMp4(Windows.Media.MediaProperties.VideoEncodingQuality.auto);
                mediaCaptureMgr.startRecordToStorageFileAsync(encodingProfile, storageFile).done(function (result) {
                    recordState = true;
                    btnRecord.innerHTML = "Stop Record";
                    btnRecord.disabled = false;
                    try {
                        displayStatus("Record Started");
                    }
                    catch (e) {
                        var test = 0;
                    }
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
        id("btnStartDevice" + scenarioId).addEventListener("click", startDevice, false);
        id("btnStartPreview" + scenarioId).disabled = true;
        id("btnStartPreview" + scenarioId).addEventListener("click", startPreview, false);
        id("btnRecord" + scenarioId).disabled = true;
        id("btnRecord" + scenarioId).addEventListener("click", startStopRecord, false);
        id("btnSnap" + scenarioId).disabled = true;
        id("btnSnap" + scenarioId).addEventListener("click", capturePhoto, false);
        id("rngContrast").disabled = true;
        id("rngBrightness").disabled = true;
        initCaptureSettings();
        displayStatus("");
    }

})();
