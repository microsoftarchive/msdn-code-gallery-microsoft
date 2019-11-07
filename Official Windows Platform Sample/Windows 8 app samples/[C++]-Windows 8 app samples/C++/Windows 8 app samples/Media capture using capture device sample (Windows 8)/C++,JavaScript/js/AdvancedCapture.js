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
    var tempPhotoFile = "photoTmp.jpg";
    var deviceList = null;
    var recordState = null;
    var captureInitSettings = null;
    var encodingProfile = null;
    var storageFile = null;
    var cameraControlSliders = null;
    var scenarioId = "2";
    var effectAdded = false;
    var effectAddedToRecord = false;
    var effectAddedToPhoto = false;
    var selectedEffect = "GrayscaleTransform.GrayscaleEffect";
    var rotateVideoOnOrientationChange = true;
    var reverseVideoRotation = false;

    var page = WinJS.UI.Pages.define("/html/AdvancedCapture.html", {
        ready: function (element, options) {
            Windows.Media.MediaControl.addEventListener("soundlevelchanged", soundLevelChangedHandler, false);
            scenarioInitialize();

            var dispProp = Windows.Graphics.Display.DisplayProperties;
            dispProp.addEventListener("orientationchanged", updatePreviewForRotation, false);
        },
        unload: function (element, options) {
            Windows.Media.MediaControl.removeEventListener("soundlevelchanged", soundLevelChangedHandler, false);

            var dispProp = Windows.Graphics.Display.DisplayProperties;
            dispProp.removeEventListener("orientationchanged", updatePreviewForRotation, false);
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

        //If the user chosen another webcam, use that webcam by default
        var selectedIndex = id("cameraSelect").selectedIndex;
        var deviceInfo = deviceList[selectedIndex];
        captureInitSettings.videoDeviceId = deviceInfo.id;

        var cameraLocation = null;

        if (deviceInfo.enclosureLocation) {
            cameraLocation = deviceInfo.enclosureLocation.panel;
        }

        if (cameraLocation === Windows.Devices.Enumeration.Panel.back) {
            rotateVideoOnOrientationChange = true;
            reverseVideoRotation = false;
        } else if (cameraLocation === Windows.Devices.Enumeration.Panel.front) {
            rotateVideoOnOrientationChange = true;
            reverseVideoRotation = true;
        } else {
            rotateVideoOnOrientationChange = false;
        }
    }

    function releaseMediaCapture() {
        document.getElementById("previewVideo2").src = null;
        mediaCaptureMgr = null;
    }

    function startDevice() {
        //Initialize media capture with the settings
        displayStatus("Starting device");
        releaseMediaCapture();
        initCaptureSettings();
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
        }, errorHandler);
    }

    function startPreview() {
        displayStatus("Starting preview");
        id("btnStartPreview" + scenarioId).disabled = true;
        updatePreviewForRotation();
        var video = id("previewVideo" + scenarioId);
        video.src = URL.createObjectURL(mediaCaptureMgr, { oneTimeOnly: true });
        video.play();
        displayStatus("Preview started");
    }

    function reencodePhotoAsync(tempStorageFile, currentRotation) {
        var inputStream = null;
        var outputStream = null;
        var decoder = null;
        var encoder = null;
        var photoStorage = null;

        return tempStorageFile.openAsync(Windows.Storage.FileAccessMode.read).then(function (stream) {
            inputStream = stream;
            return Windows.Graphics.Imaging.BitmapDecoder.createAsync(inputStream);
        }).then(function (_decoder) {
            decoder = _decoder;
            return Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(photoFile,
                Windows.Storage.CreationCollisionOption.generateUniqueName);
        }).then(function (file) {
            photoStorage = file;
            return photoStorage.openAsync(Windows.Storage.FileAccessMode.readWrite);
        }).then(function (stream) {
            outputStream = stream;
            outputStream.size = 0;
            return Windows.Graphics.Imaging.BitmapEncoder.createForTranscodingAsync(outputStream, decoder);
        }).then(function (_encoder) {
            encoder = _encoder;
            var properties = new Windows.Graphics.Imaging.BitmapPropertySet();
            properties.insert("System.Photo.Orientation",
                new Windows.Graphics.Imaging.BitmapTypedValue(
                    currentRotation,
                    Windows.Foundation.PropertyType.uint16
                    ));
            return encoder.bitmapProperties.setPropertiesAsync(properties);
        }).then(function () {
            return encoder.flushAsync();
        }).then(function () {
            inputStream.close();
            outputStream.close();
            return tempStorageFile.deleteAsync(Windows.Storage.StorageDeleteOption.permanentDelete);
        }).then(function() {
            return photoStorage;
        });
    }

    function capturePhoto() {
        var currentRotation = getCurrentPhotoRotation();
        var tempPhotoStorage = null;
        id("btnSnap" + scenarioId).disabled = true;
        displayStatus("Taking photo");
        
        return Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(tempPhotoFile,
            Windows.Storage.CreationCollisionOption.generateUniqueName).then(function (newFile) {
        tempPhotoStorage = newFile;
        var photoProperties = Windows.Media.MediaProperties.ImageEncodingProperties.createJpeg();

        mediaCaptureMgr.capturePhotoToStorageFileAsync(photoProperties, tempPhotoStorage).then(function (result) {
            return reencodePhotoAsync(tempPhotoStorage, currentRotation);
        }).then(function(photoStorage) {
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
                updateRecordForRotation();
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
    function addEffectToImageStream() {
        if ((mediaCaptureMgr.mediaCaptureSettings.videoDeviceCharacteristic !== Windows.Media.Capture.VideoDeviceCharacteristic.previewPhotoStreamsIdentical) &&
        (mediaCaptureMgr.mediaCaptureSettings.videoDeviceCharacteristic !== Windows.Media.Capture.VideoDeviceCharacteristic.allStreamsIdentical) &&
        (mediaCaptureMgr.mediaCaptureSettings.videoDeviceCharacteristic !== Windows.Media.Capture.VideoDeviceCharacteristic.recordPhotoStreamsIndentical)) {
            var iMediaEncodingProperties = mediaCaptureMgr.videoDeviceController.getMediaStreamProperties(Windows.Media.Capture.MediaStreamType.photo);
            if (iMediaEncodingProperties.type === "Image") { //Cant add an effect to an image type
                var vectorView = mediaCaptureMgr.videoDeviceController.getAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.photo);
                {
                    var i = 0;
                    while (i < vectorView.size) {
                        var props = vectorView[i];
                        if (props.type === "Video") {
                            mediaCaptureMgr.videoDeviceController.setMediaStreamPropertiesAsync(Windows.Media.Capture.MediaStreamType.photo, props).done(
                                function (result) {
                                    displayStatus("Changed type on image stream");
                                    mediaCaptureMgr.addEffectAsync(Windows.Media.Capture.MediaStreamType.photo, selectedEffect, null).done(
                                       function (result2) {
                                           displayStatus("Effect added to image stream");
                                           effectAddedToPhoto = true;

                                       }, errorHandler);
                                }
                                , errorHandler);
                            break;
                        }
                    }
                }
            }
            else {
                mediaCaptureMgr.addEffectAsync(Windows.Media.Capture.MediaStreamType.photo, selectedEffect, null).done(
                function (result) {
                    displayStatus("Effect added to image stream");
                    effectAddedToPhoto = true;
                }, errorHandler);

            }
        }
    }

    function addRemoveEffect() {
        //Add gray scale effect

        if (id("videoEffect").checked) {
            displayStatus("Adding effect");
            mediaCaptureMgr.addEffectAsync(Windows.Media.Capture.MediaStreamType.videoPreview, selectedEffect, null).done(
                        function (result) {
                            displayStatus("Effect added to preview");
                            effectAdded = true;
                            if (mediaCaptureMgr.mediaCaptureSettings.videoDeviceCharacteristic !== Windows.Media.Capture.VideoDeviceCharacteristic.previewRecordStreamsIdentical &&
                                mediaCaptureMgr.mediaCaptureSettings.videoDeviceCharacteristic !== Windows.Media.Capture.VideoDeviceCharacteristic.allStreamsIdentical
                                 ) {
                                var iMediaEncodingProperties = mediaCaptureMgr.videoDeviceController.getMediaStreamProperties(Windows.Media.Capture.MediaStreamType.videoRecord);
                                if (iMediaEncodingProperties.subtype !== "H264") { //Cant add an effect to an H264 webcam
                                    mediaCaptureMgr.addEffectAsync(Windows.Media.Capture.MediaStreamType.videoRecord, selectedEffect, null).done(
                                        function (result2) {
                                            displayStatus("Effect added to record");
                                            effectAddedToRecord = true;
                                            addEffectToImageStream();
                                        }, errorHandler);
                                }
                                else {
                                    addEffectToImageStream();
                                }
                            }
                            else {
                                addEffectToImageStream();
                            }
                        }, errorHandler);
        }

        else {
            displayStatus("Clearing effect");
            mediaCaptureMgr.clearEffectsAsync(Windows.Media.Capture.MediaStreamType.videoPreview, null).done(
                        function (result) {
                            displayStatus("Effect cleared");
                            if (effectAddedToRecord) {
                                mediaCaptureMgr.clearEffectsAsync(Windows.Media.Capture.MediaStreamType.videoRecord, null).done(
                                function (result2) {
                                    effectAddedToRecord = false;
                                    displayStatus("Effect cleared");
                                    if (effectAddedToPhoto) {
                                        mediaCaptureMgr.clearEffectsAsync(Windows.Media.Capture.MediaStreamType.photo, null).done(
                                        function (result3) {
                                            effectAddedToPhoto = false;
                                            displayStatus("Effect cleared");
                                        }, errorHandler);
                                    }
                                },
                    errorHandler);
                            }
                            else if (effectAddedToPhoto) {
                                mediaCaptureMgr.clearEffectsAsync(Windows.Media.Capture.MediaStreamType.photo, null).done(
                                       function (result4) {
                                           effectAddedToPhoto = false;
                                           displayStatus("Effect cleared");
                                       }, errorHandler);
                            }
                        }, errorHandler);
        }
    }

    function enumerateCameras() {
        displayStatus("Enumerating webcams");
        var cameraSelect = id("cameraSelect");
        deviceList = null;
        deviceList = new Array();
        while (cameraSelect.length > 0) {
            cameraSelect.remove(0);
        }
        //Enumerate webcams and add them to the list
        var deviceInfo = Windows.Devices.Enumeration.DeviceInformation;
        deviceInfo.findAllAsync(Windows.Devices.Enumeration.DeviceClass.videoCapture).done(function (devices) {
            // Add the devices to deviceList
            displayStatus("Webcams enumerated");
            if (devices.length > 0) {
                for (var i = 0; i < devices.length; i++) {
                    deviceList.push(devices[i]);
                    cameraSelect.add(new Option(deviceList[i].name), i);
                }
                //Select the first webcam
                cameraSelect.selectedIndex = 0;
                initCaptureSettings();

            } else {
                displayError("No camera device is found ");
                // disable buttons.
            }
        }, errorHandler);
    }

    function onCameraChange() {
        releaseMediaCapture();
        id("btnStartDevice" + scenarioId).disabled = false;
        id("btnStartPreview" + scenarioId).disabled = true;
        id("btnRecord" + scenarioId).disabled = true;
        id("btnSnap" + scenarioId).disabled = true;
        displayStatus("");
        initCaptureSettings();
    }

    function updatePreviewForRotation() {
        if (!mediaCaptureMgr) {
            return;
        }
        var previewMirroring = mediaCaptureMgr.getPreviewMirroring();
        var counterclockwiseRotation = (previewMirroring && !reverseVideoRotation) ||
            (!previewMirroring && reverseVideoRotation);

        if (rotateVideoOnOrientationChange) {
            mediaCaptureMgr.setPreviewRotation(videoRotationLookup(Windows.Graphics.Display.DisplayProperties.currentOrientation, counterclockwiseRotation));
        } else {
            mediaCaptureMgr.setPreviewRotation(Windows.Media.Capture.VideoRotation.none);
        }
    }

    function updateRecordForRotation() {
        if (!mediaCaptureMgr) {
            return;
        }
        var counterclockwiseRotation = reverseVideoRotation;

        if (rotateVideoOnOrientationChange) {
            mediaCaptureMgr.setRecordRotation(videoRotationLookup(Windows.Graphics.Display.DisplayProperties.currentOrientation, counterclockwiseRotation));
        } else {
            mediaCaptureMgr.setRecordRotation(Windows.Media.Capture.VideoRotation.none);
        }
    }

    function getCurrentPhotoRotation() {
        var counterclockwiseRotation = reverseVideoRotation;

        if (rotateVideoOnOrientationChange) {
            return photoRotationLookup(Windows.Graphics.Display.DisplayProperties.currentOrientation, counterclockwiseRotation);
        } else {
            return Windows.Storage.FileProperties.PhotoOrientation.normal;
        }
    }

    function photoRotationLookup(displayOrientation, counterclockwise) {
        switch (displayOrientation) {
            case Windows.Graphics.Display.DisplayOrientations.landscape:
                return Windows.Storage.FileProperties.PhotoOrientation.normal;

            case Windows.Graphics.Display.DisplayOrientations.portrait:
                return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.rotate270:
                    Windows.Storage.FileProperties.PhotoOrientation.rotate90;

            case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
                return Windows.Storage.FileProperties.PhotoOrientation.rotate180;

            case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
                return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.rotate90 :
                    Windows.Storage.FileProperties.PhotoOrientation.rotate270;

            default:
                return Windows.Storage.FileProperties.PhotoOrientation.unspecified;
        }
    }

    function videoRotationLookup(displayOrientation, counterclockwise) {
        switch (displayOrientation) {
            case Windows.Graphics.Display.DisplayOrientations.landscape:
                return Windows.Media.Capture.VideoRotation.none;

            case Windows.Graphics.Display.DisplayOrientations.portrait:
                return (counterclockwise) ? Windows.Media.Capture.VideoRotation.clockwise270Degrees :
                    Windows.Media.Capture.VideoRotation.clockwise90Degrees;

            case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
                return Windows.Media.Capture.VideoRotation.clockwise180Degrees;

            case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
                return (counterclockwise) ? Windows.Media.Capture.VideoRotation.clockwise90Degrees :
                    Windows.Media.Capture.VideoRotation.clockwise270Degrees;

            default:
                return Windows.Media.Capture.VideoRotation.none;
        }
    }

    function suspendingHandler(suspendArg) {
        displayStatus("Suspended");
        if (recordState) {
            stopRecord();
        }
        releaseMediaCapture();
    }

    function resumingHandler(resumeArg) {
        displayStatus("Resumed");
        scenarioInitialize();
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
        id("cameraSelect").addEventListener("change", onCameraChange, false);
        id("videoEffect").addEventListener("click", addRemoveEffect, false);
        id("videoEffect").checked = false;
        enumerateCameras();
    }

})();
