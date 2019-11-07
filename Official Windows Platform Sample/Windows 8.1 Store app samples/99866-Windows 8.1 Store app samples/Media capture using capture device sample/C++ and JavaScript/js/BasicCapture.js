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
            var systemMediaControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            systemMediaControls.addEventListener("propertychanged", mediaPropertyChangedHandler, false);
            scenarioInitialize();
        },

        unload: function (element, options) {
            var systemMediaControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            systemMediaControls.removeEventListener("propertychanged", mediaPropertyChangedHandler, false);
            releaseMediaCapture();
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

    // Convenience functions

    function recordLimitationExceededEventHandler() {
        if (recordState) {
            SdkSample.displayStatus("Stopping record as max record duration was exceeded");
            stopRecord();
        }
    }

    function id(elementId) {
        return document.getElementById(elementId);
    }

    // Initialize media capture settings
    function initCaptureSettings() {
        captureInitSettings = null;
        captureInitSettings =
            new Windows.Media.Capture.MediaCaptureInitializationSettings();
        captureInitSettings.audioDeviceId = "";
        captureInitSettings.videoDeviceId = "";
        captureInitSettings.photoCaptureSource =
            Windows.Media.Capture.PhotoCaptureSource.auto;
        captureInitSettings.streamingCaptureMode =
            Windows.Media.Capture.StreamingCaptureMode.audioAndVideo;
    }

    // Release all the resources associated with the media capture
    function releaseMediaCapture() {
        document.getElementById("previewVideo1").src = null;
        if (cameraControlSliders) {
            for (var i = 0; i < cameraControlSliders.length; i++) {
                cameraControlSliders[i].slider.removeEventListener("change",
                    cameraControlSliders[i].handler, false);
                cameraControlSliders[i].control = null;
                cameraControlSliders[i].handler = null;
            }
        }
        if (mediaCaptureMgr) {
            mediaCaptureMgr.close();
            mediaCaptureMgr = null;
        }
        SdkSample.displayStatus("");
    }

    // Initialize media capture with the current settings
    function startDevice() {
        SdkSample.displayStatus("Starting device");
        releaseMediaCapture();
        mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
        mediaCaptureMgr.initializeAsync(captureInitSettings).done(function (result) {

            // do we have a camera and a microphone present?
            if (mediaCaptureMgr.mediaCaptureSettings.videoDeviceId && mediaCaptureMgr.mediaCaptureSettings.audioDeviceId) {

                // Update the UI
                SdkSample.displayStatus("Device started");
                id("btnSnap" + scenarioId).disabled = false;
                id("btnRecord" + scenarioId).disabled = false;
                id("btnStartDevice" + scenarioId).disabled = true;
                id("btnStartPreview" + scenarioId).disabled = false;

                // Set up listeners
                mediaCaptureMgr.addEventListener("recordlimitationexceeded",
                    recordLimitationExceededEventHandler);
                mediaCaptureMgr.addEventListener("failed", function (e) {
                    // Something went very wrong. The user might have unplugged the camera while using it
                    releaseMediaCapture();
                    SdkSample.displayStatus("Fatal error: " + e.message);
                });


            } else {
                SdkSample.displayError("No capture device was found");
                id("btnStartDevice" + scenarioId).disabled = true;
            }

        }, errorHandler);
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

    // Set up brightness and contrast controls
    function getCameraSettings() {
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
        SdkSample.displayStatus("Starting preview");
        id("btnStartPreview" + scenarioId).disabled = true;

        try {
            var video = id("previewVideo" + scenarioId);
            video.src = URL.createObjectURL(mediaCaptureMgr, { oneTimeOnly: true });
            video.play();

        } catch (e) {
            SdkSample.displayStatus("Preview failed: " + e.message);
            return;
        }

        SdkSample.displayStatus("Preview started");
        getCameraSettings();

        // Initialize sliders.
        for (var i = 0; i < cameraControlSliders.length; i++) {
            cameraControlSliders[i].slider.disabled = false;
            initSlider(cameraControlSliders[i]);
        }
    }

    function capturePhoto() {

        // Get ready
        id("btnSnap" + scenarioId).disabled = true;
        SdkSample.displayStatus("Taking photo");

        if (!mediaCaptureMgr.mediaCaptureSettings.ConcurrentRecordAndPhotoSupported) {
            // If camera does not support record and Takephoto at the same time
            // disable Record button when taking photo
            id("btnRecord" + scenarioId).disabled = true;
        }

        // Create the file that will be used to store the picture
        Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(photoFile,
                Windows.Storage.CreationCollisionOption.generateUniqueName).then(function (newFile) {

                    photoStorage = newFile;
                    var photoProperties =
                        Windows.Media.MediaProperties.ImageEncodingProperties.createJpeg();

                    // Capture the photo
                    mediaCaptureMgr.capturePhotoToStorageFileAsync(photoProperties,
                            photoStorage).done(function (result) {

                                // Display result to the user
                                SdkSample.displayStatus("Photo Taken.  File " + photoStorage.path + "  ");
                                var url = URL.createObjectURL(photoStorage, { oneTimeOnly: true });
                                id("imageTag" + scenarioId).src = url;
                                id("btnSnap" + scenarioId).disabled = false;
                                id("btnRecord" + scenarioId).disabled = false;

                                // Error handling for photo capture
                            }, function capturePhotoError(error) {
                                id("btnSnap" + scenarioId).disabled = false;
                                SdkSample.displayError("An exception occurred trying to capturePhoto: " + error.message);
                            });

                    // Error handling for file creation
                }, function (error) {
                    id("btnSnap" + scenarioId).disabled = false;
                    SdkSample.displayError("Error saving photo: " + error.message);
                });
    }

    function startRecord() {
        var btnRecord;
        SdkSample.displayStatus("Starting record ");
        btnRecord = id("btnRecord" + scenarioId);
        btnRecord.disabled = true; // block button until recording begins

        // Stop captured video playback.
        var video = id("capturePlayback" + scenarioId);
        if (!video.paused) {
            video.pause();
        }

        // Create a new mp4 file to store the recording
        Windows.Storage.KnownFolders.videosLibrary.createFileAsync("cameraCapture.mp4",
                Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (newFile) {
                    storageFile = newFile;
                    encodingProfile = Windows.Media.MediaProperties.MediaEncodingProfile.createMp4(
                        Windows.Media.MediaProperties.VideoEncodingQuality.auto);

                    // Start the recording
                    mediaCaptureMgr.startRecordToStorageFileAsync(encodingProfile,
                            storageFile).done(function (result) {
                                recordState = true;
                                btnRecord.innerHTML = "Stop Record";
                                btnRecord.disabled = false; // we may re-enable the button now
                                SdkSample.displayStatus("Record Started");

                            }, errorHandler);

                }, errorHandler);
    }

    function stopRecord() {
        var btnRecord;
        SdkSample.displayStatus("Stopping record");
        btnRecord = id("btnRecord" + scenarioId);
        btnRecord.disabled = true; // block button until recording ends

        mediaCaptureMgr.stopRecordAsync().done(function (result) {
            recordState = false;
            btnRecord.innerHTML = "Start Record";
            btnRecord.disabled = false; // we may re-enable the button now
            SdkSample.displayStatus("Record Stopped.  File " + storageFile.path + "  ");

            // Playback the recorded video.
            try {
                var video = id("capturePlayback" + scenarioId);
                video.src = URL.createObjectURL(storageFile, { oneTimeOnly: true });
                video.controls = true;
                video.play();

                // Playback may fail. 
                // One possible situation happens when switching scenarios while recording.
            } catch (e) {
                SdkSample.displayError("Playback failed");
                return;
            }

        }, errorHandler);
    }

    // Flip-flop capture device recording
    function startStopRecord() {
        if (!recordState) {

            if (!mediaCaptureMgr.mediaCaptureSettings.ConcurrentRecordAndPhotoSupported) {
                // If camera does not support record and Takephoto at the same time
                // disable Photo button when recording
                id("btnSnap" + scenarioId).disabled = true;
            }
            startRecord();

        } else {
            stopRecord();
            id("btnSnap" + scenarioId).disabled = false;
        }
    }

    function errorHandler(err) {
        SdkSample.displayError(err.message);
    }


    function scenarioInitialize() {

        // Set up all the UI for this scenario
        SdkSample.displayStatus("");
        id("rngContrast").disabled = true;
        id("rngBrightness").disabled = true;
        id("btnSnap" + scenarioId).disabled = true;
        id("btnRecord" + scenarioId).disabled = true;
        id("btnStartDevice" + scenarioId).disabled = false;
        id("btnStartPreview" + scenarioId).disabled = true;
        id("btnSnap" + scenarioId).addEventListener("click", capturePhoto, false);
        id("btnRecord" + scenarioId).addEventListener("click", startStopRecord, false);
        id("btnStartDevice" + scenarioId).addEventListener("click", startDevice, false);
        id("btnStartPreview" + scenarioId).addEventListener("click", startPreview, false);

        // Disable playback controls if no captured media is present
        if (!id("capturePlayback" + scenarioId).src) {
            id("capturePlayback" + scenarioId).controls = false;
        }

        // initialize the capture device handlers
        initCaptureSettings();
    }
})();