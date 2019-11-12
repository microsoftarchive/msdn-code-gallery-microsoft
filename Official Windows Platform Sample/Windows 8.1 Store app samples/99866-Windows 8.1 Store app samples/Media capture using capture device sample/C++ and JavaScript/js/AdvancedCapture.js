//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var cameraList = null;
    var microphoneList = null;
    var mediaCaptureMgr = null;
    var photoFile = "photo.jpg";
    var tempPhotoFile = "photoTmp.jpg";
    var lowLagPhoto = null;
    var lowLagRecord = null;
    var recordState = null;
    var captureInitSettings = null;
    var encodingProfile = null;
    var recordFile = null;
    var cameraControlSliders = null;
    var scenarioId = "2"; // this is used to properly identify elements in the layout
    var selectedEffect = "GrayscaleTransform.GrayscaleEffect";
    var rotateVideoOnOrientationChange = true;
    var reverseVideoRotation = false;    
    var rotHeight;
    var rotWidth;
    
    var page = WinJS.UI.Pages.define("/html/AdvancedCapture.html", {

        ready: function (element, options) {
            var systemMediaControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            systemMediaControls.addEventListener("propertychanged", mediaPropertyChangedHandler, false);
            scenarioInitialize();

            Windows.Graphics.Display.DisplayInformation.getForCurrentView().addEventListener("orientationchanged", updatePreviewForRotation, false);
        },

        unload: function (element, options) {
            var systemMediaControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            systemMediaControls.removeEventListener("propertychanged", mediaPropertyChangedHandler, false);

            Windows.Graphics.Display.DisplayInformation.getForCurrentView().removeEventListener("orientationchanged", updatePreviewForRotation, false);


            // release resources
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

    function initCameraSettings() {

        // Initialize capture settings
        captureInitSettings.photoCaptureSource = Windows.Media.Capture.PhotoCaptureSource.auto;
        captureInitSettings.streamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.audioAndVideo;

        // If the user chose another capture device, use it by default
        var selectedIndex = id("cameraSelect").selectedIndex;
        var deviceInfo = cameraList[selectedIndex];
        captureInitSettings.videoDeviceId = deviceInfo.id;

        var cameraLocation = null;
        if (deviceInfo.enclosureLocation) {
            cameraLocation = deviceInfo.enclosureLocation.panel;
        }

        if (cameraLocation !== null && cameraLocation === Windows.Devices.Enumeration.Panel.back){
            rotateVideoOnOrientationChange = true;
            reverseVideoRotation = false;

        }
        else if (cameraLocation !== null && cameraLocation === Windows.Devices.Enumeration.Panel.front) {
            rotateVideoOnOrientationChange = true;
            reverseVideoRotation = true;
        }
        else {
            rotateVideoOnOrientationChange = false;
        }
    }

    function initMicrophoneSettings() {

        // Initialize capture settings
        captureInitSettings.audioDeviceId = "";

        // if the user chose another microphone, use it by default
        var selectedIndex = id("microphoneSelect").selectedIndex;
        var microphoneDeviceInfo = microphoneList[selectedIndex];
        captureInitSettings.audioDeviceId = microphoneDeviceInfo.id;
    }

    function initializeSceneMode() {
        var sceneSelect = id("sceneModeSelect");

        // clear the previous list of scene modes if any
        while (sceneSelect.length > 0) {
            sceneSelect.remove(0);
        }

        // if no scene modes are supported, disable the option
        var supportedModes = mediaCaptureMgr.videoDeviceController.sceneModeControl.supportedModes;
        if (supportedModes.length === 0) {
            var option = document.createElement("option");
            option.innerText = "Scene mode not supported";
            sceneSelect.options.add(option);
            sceneSelect.disabled = true;
            return;
        }

        // update the UI to enable scene mode
        sceneSelect.disabled = false;

        // add listener to apply changes to the scene mode selection
        sceneSelect.addEventListener("change", onSceneModeChanged, false);

        // add entry for each of the scene modes
        supportedModes.forEach(function (mode) {
            var modeName = null;

            switch (mode) {
                case Windows.Media.Devices.CaptureSceneMode.manual:
                    modeName = "Manual";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.auto:
                    modeName = "Auto";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.macro:
                    modeName = "Macro";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.portrait:
                    modeName = "Portrait";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.sport:
                    modeName = "Sport";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.snow:
                    modeName = "Snow";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.night:
                    modeName = "Night";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.beach:
                    modeName = "Beach";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.sunset:
                    modeName = "Sunset";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.candlelight:
                    modeName = "Candlelight";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.landscape:
                    modeName = "Landscape";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.nightPortrait:
                    modeName = "NightPortrait";
                    break;
                case Windows.Media.Devices.CaptureSceneMode.backlight:
                    modeName = "Backlight";
                    break;
            }

            if (modeName !== null) {
                var optionMode = document.createElement("option");
                optionMode.value = mode;
                optionMode.innerText = modeName;
                sceneSelect.options.add(optionMode);
            }
        });
    }

    // handler used to apply changes when a different scene mode is selected
    function onSceneModeChanged(e) {
        var mode = e.target.options[e.target.selectedIndex].value;
        mediaCaptureMgr.videoDeviceController.sceneModeControl.setValueAsync(mode).done();
    }

    // this function releases the resources associated with low latency recorder
    function releaselowLagRecord(callback) {
        if (lowLagRecord) {
            lowLagRecord.finishAsync().done(function () {
                lowLagRecord = null;
                if (callback) {
                    callback();
                }
            });
        } else {
            if (callback) {
                callback();
            }
        }
    }

    // this function releases the resources associated with the low latency photo taking
    function releaseLowLagPhoto(callback) {
        if (lowLagPhoto) {
            lowLagPhoto.finishAsync().done(function () {
                lowLagPhoto = null;
                if (callback) {
                    callback();
                }
            });
        } else {
            if (callback) {
                callback();
            }
        }
    }

    // this function takes care of releasing the resources associated with media capturing
    function releaseMediaCapture() {

        // make sure that the low lag recorder function is disabled
        releaselowLagRecord(function () {

            // make sure that the low lag photo taking function is disabled
            releaseLowLagPhoto(function () {

                // release media capture manager resources
                if (id("previewVideo2")) {
                    id("previewVideo2").src = null;
                }
                if (mediaCaptureMgr) {
                    mediaCaptureMgr.close();
                    mediaCaptureMgr = null;
                }
            });
        });
    }

    //Initialize media capture with the current settings
    function startDevice() {
        displayStatus("Starting device");
        releaseMediaCapture();
        initCameraSettings();
        initMicrophoneSettings();
        mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
        mediaCaptureMgr.initializeAsync(captureInitSettings).done(function (result) {

            // do we have a camera and microphone?
            if (captureInitSettings.audioDeviceId && captureInitSettings.videoDeviceId) {

                // look for scene modes
                initializeSceneMode();

                // Update the UI
                id("videoEffect").disabled = false;
                id("btnStartPreview" + scenarioId).disabled = false;
                id("btnStartDevice" + scenarioId).disabled = true;
                displayStatus("Device started");

                // Set up listeners
                mediaCaptureMgr.addEventListener("recordlimitationexceeded", recordLimitationExceededEventHandler);
                mediaCaptureMgr.addEventListener("failed", function (e) {
                    // Something went very wrong. The user might have unplugged the camera while using it.
                    // Display error message
                    releaseMediaCapture();
                    displayError("Fatal error occurred: " + e.message);
                    id("btnSnap" + scenarioId).disabled = true;
                    id("btnRecord" + scenarioId).disabled = true;
                    id("btnStartPreview" + scenarioId).disabled = true;
                    id("btnStartDevice" + scenarioId).disabled = true;
                });

                // check for concurrency support in capture device
                if (mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {

                    // prepare the low latency advanced functionality
                    prepareLowLagRecord(function () {
                        prepareLowLagPhoto();
                    });

                    // update the UI
                    id("photoModeSelect").disabled = true;
                    id("recordModeSelect").disabled = true;

                } else {

                    // default to prepare low lag photo
                    prepareLowLagPhoto(function () {

                        // update the UI
                        id("photoModeSelect").checked = true;
                        id("photoModeSelect").disabled = false;
                        id("recordModeSelect").disabled = false;
                    });


                }

            } else {
                SdkSample.displayError("No capture device was found");
                id("btnStartDevice" + scenarioId).disabled = true;
            }

        }, errorHandler);
    }

    // if concurrency is not supported, this method will warm start the recording stream so we
    // may use the low lag functionality when recording
    function setRecordMode() {

        // if low lag photo is prepared and concurrency is not supported by this device, we should disable it first
        if (lowLagPhoto && !mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
            lowLagPhoto.finishAsync().done(function () {

                // because we can only enable the low lag record synchronously, do it within .done()
                lowLagPhoto = false;
                prepareLowLagRecord();

                // update the UI
                id("btnSnap" + scenarioId).disabled = true;
            });

            // otherwise we can enable low lag record as long as we make sure that low lag photo is disabled
        } else if (!lowLagPhoto && !mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
            prepareLowLagRecord();

            // if concurrency is supported we should not be calling this method in the first place
        } else if (mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
            displayError("Can't set mode to record because photo and record can be done concurrently");
        }
    }

    // if concurrency is not supported, this method will warm start the photo stream so we
    // may use the low lag functionality when taking a picture
    function setPhotoMode() {

        // if low lag record is prepared and concurrency is not supported by this device, we should disable it first
        if (lowLagRecord && !mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
            lowLagRecord.finishAsync().done(function () {

                // because we can only enable the low lag record synchronously, do it within .done()
                lowLagRecord = false;
                prepareLowLagPhoto();

                // update the UI
                id("btnRecord" + scenarioId).disabled = true;
            });

            // otherwise we can enable low lag photo as long as we make sure that low lag record is disabled
        } else if (!lowLagRecord && !mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
            prepareLowLagPhoto();

            // if concurrency is supported we shouldn not be calling this method in the first place
        } else if (mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
            displayError("Can't set mode to record because photo and record can be done concurrently");
        }
    }

    // This function gets the device ready for the advanced feature used to take a photo with low lag
    function prepareLowLagPhoto(callback) {

        // if we are already prepared then there is no need to do anything
        if (!lowLagPhoto) {

            // if low lag record is prepared and concurrency is not supported by this device, we should disable it first
            if (lowLagRecord && !mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
                displayError("Concurrent modes are not supported in this device");

            } else {

                // use advanced low lag photo taking function
                var photoFormat = Windows.Media.MediaProperties.ImageEncodingProperties.createJpeg();
                mediaCaptureMgr.prepareLowLagPhotoCaptureAsync(photoFormat).done(function (result) {

                    // retrieve the object that will allow us to perform low lag photo taking
                    lowLagPhoto = result;

                    // update UI
                    id("btnSnap" + scenarioId).disabled = false;

                    // perform callback
                    if (callback) {
                        callback();
                    }

                });
            }
        }
    }

    function prepareLowLagRecord(callback) {

        // if we are already prepared then there is no need to do anything
        if (!lowLagRecord) {

            // if low lag photo is prepared and concurrency is not supported by this device, we should disable it first
            if (lowLagPhoto && !mediaCaptureMgr.mediaCaptureSettings.concurrentRecordAndPhotoSupported) {
                displayError("Concurrent modes are not supported in this device");
            }

            // set up the recording profile and create a file to store the recording
            var recordFormat = Windows.Media.MediaProperties.MediaEncodingProfile.createMp4(Windows.Media.MediaProperties.VideoEncodingQuality.auto);
            Windows.Storage.KnownFolders.videosLibrary.createFileAsync("cameraCapture.mp4", Windows.Storage.CreationCollisionOption.generateUniqueName).then(function (newFile) {

                // keep track of the file created
                recordFile = newFile;

                // use advanced low lag recording function
                mediaCaptureMgr.prepareLowLagRecordToStorageFileAsync(recordFormat, recordFile).done(function (result) {

                    // retrieve the object that will allow us to perform low lag recording
                    lowLagRecord = result;

                    // update UI
                    id("btnRecord" + scenarioId).disabled = false;

                    // perform callback
                    if (callback) {
                        callback();
                    }

                    // low lag recorder hook to file error handler
                }, errorHandler);

                // file creation error handler
            }, errorHandler);
        }
    }


    function startPreview() {
        displayStatus("Starting preview");
        id("btnStartPreview" + scenarioId).disabled = true;
        var video = id("previewVideo" + scenarioId);
        video.src = URL.createObjectURL(mediaCaptureMgr, { oneTimeOnly: true });
        
        video.onloadeddata = function () {
            updatePreviewForRotation();
        };

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
                    Windows.Foundation.PropertyType.uint16));
            return encoder.bitmapProperties.setPropertiesAsync(properties);

        }).then(function () {
            return encoder.flushAsync();

        }).then(function () {
            inputStream.close();
            outputStream.close();
            return photoStorage;
        }, errorHandler);
    }

    function capturePhoto() {

        // Get ready
        var currentRotation = getCurrentPhotoRotation();
        var tempPhotoStorage = null;
        id("btnSnap" + scenarioId).disabled = true;
        displayStatus("Taking photo");

        // Create the file that will be used to store the picture
        Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(tempPhotoFile, Windows.Storage.CreationCollisionOption.generateUniqueName).done(
            function (newFile) {
                // open the newly created file as a stream

                tempPhotoStorage = newFile;
                tempPhotoStorage.openAsync(Windows.Storage.FileAccessMode.readWrite).done(
                    function (stream) {
                        // capture the photo using advance low lag function
                        lowLagPhoto.captureAsync().done(
                            function (photoTaken) {
                                // copy the photo taken into the new file stream
                                var contentStream = photoTaken.frame.cloneStream();
                                Windows.Storage.Streams.RandomAccessStream.copyAndCloseAsync(contentStream.getInputStreamAt(0), stream.getOutputStreamAt(0)).done(
                                    function () {

                                        // close the streams
                                        stream.close();
                                        contentStream.close();

                                        // reencode the photo to adjust for rotation changes, etc.
                                        reencodePhotoAsync(tempPhotoStorage, currentRotation).done(
                                            function (photo) {
                                                // update the UI

                                                displayStatus("Photo Taken.");
                                                var url = URL.createObjectURL(photo, { oneTimeOnly: true });
                                                id("btnSnap" + scenarioId).disabled = false;
                                                id("imageTag" + scenarioId).src = url;
                                                tempPhotoStorage.deleteAsync();
                                            });
                                    });
                            });
                    });
            });
    }

    // This function gets the device ready for the advanced feature used to start recording with low lag
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

            updateRecordForRotation();
            // use the low lag recorder that should be prepared at this point
            lowLagRecord.startAsync().done(function (result) {
                recordState = true;
                btnRecord.innerHTML = "Stop Record";
                btnRecord.disabled = false;
                displayStatus("Record Started");

            }, errorHandler);
        }
    }

    function stopRecord() {
        var btnRecord;
        displayStatus("Stopping record");
        btnRecord = id("btnRecord" + scenarioId);
        lowLagRecord.stopAsync().done(function () {            
            recordState = false;
            btnRecord.innerHTML = "Start Record";
            btnRecord.disabled = false;
            displayStatus("Record Stopped.  File " + recordFile.path + "  ");

            // Playback the recorded video.
            try {
                var video = id("capturePlayback" + scenarioId);
                video.src = URL.createObjectURL(recordFile, { oneTimeOnly: true });
                video.controls = true;
                video.play();

                // Playback may fail. 
                // One possible situation happens when switching scenarios while recording.
            } catch (e) {
                displayError("Playback failed:" + e.message);
                return;
            }

            // We must re-instantiate the low lag record functionality to create a new file for recording
            releaselowLagRecord(function () {
                prepareLowLagRecord();
            });

        }, errorHandler);
    }

    function startStopRecord() {
        if (!recordState) {
            startRecord();
        } else {
            stopRecord();
        }
    }

    function setEffectToStream(stream, effect, callback) {

        // if effect is falsy then we need to clear the effect
        if (!effect) {
            mediaCaptureMgr.clearEffectsAsync(stream, null).done(function () {
                if (callback) {
                    callback();
                }
            });

        // code below handles special case for image type in photo stream
        } else if (stream === Windows.Media.Capture.MediaStreamType.photo
        && mediaCaptureMgr.videoDeviceController.getMediaStreamProperties(Windows.Media.Capture.MediaStreamType.photo).type === "Image") { //Cant add an effect to an image type
            if (callback) {
                callback();
            }

            // code below handles special case for H264 video
        } else if (stream === Windows.Media.Capture.MediaStreamType.videoRecord
        && mediaCaptureMgr.videoDeviceController.getMediaStreamProperties(Windows.Media.Capture.MediaStreamType.videoRecord).subtype === "H264") {
            if (callback) {
                callback();
            }

            // otherwise, we can apply the effects to the stream normally
        } else {
            mediaCaptureMgr.addEffectAsync(stream, selectedEffect, null).done(function () {
                if (callback) {
                    callback();
                }
            }, errorHandler);
        }
    }

    // Flip-flop gray scale effect
    function addRemoveEffect() {

        var effect;
        if (id("videoEffect").checked) {
            effect = selectedEffect;
            displayStatus("Adding effect to streams");
        } else {
            effect = null;
            displayStatus("Removing effect from streams");
        }

        // We may have one or more streams depending on the hardware, so we need to apply the
        // effects independently to each of those streams.
        switch (mediaCaptureMgr.mediaCaptureSettings.videoDeviceCharacteristic) {

            // if all the streams are identical, applying to one will affect all
            case Windows.Media.Capture.VideoDeviceCharacteristic.allStreamsIdentical:
                setEffectToStream(Windows.Media.Capture.MediaStreamType.videoPreview, effect);
                break;

                // if two streams are identical, apply affect to only one of them and then to the independent stream
            case Windows.Media.Capture.VideoDeviceCharacteristic.previewRecordStreamsIdentical:
                setEffectToStream(Windows.Media.Capture.MediaStreamType.videoPreview, effect, function () {
                    setEffectToStream(Windows.Media.Capture.MediaStreamType.photo, effect);
                });
                break;

                // if two streams are identical, apply affect to only one of them and then to the independent stream
            case Windows.Media.Capture.VideoDeviceCharacteristic.previewPhotoStreamsIdentical:
                setEffectToStream(Windows.Media.Capture.MediaStreamType.videoPreview, effect, function () {
                    setEffectToStream(Windows.Media.Capture.MediaStreamType.videoRecord, effect);
                });
                break;

                // if two streams are identical, apply affect to only one of them and then to the independent stream
            case Windows.Media.Capture.VideoDeviceCharacteristic.recordPhotoStreamsIdentical:
                setEffectToStream(Windows.Media.Capture.MediaStreamType.videoPreview, effect, function () {
                    setEffectToStream(Windows.Media.Capture.MediaStreamType.photo, effect);
                });
                break;

                // if all streams are independent, apply effect to each one of them
            case Windows.Media.Capture.VideoDeviceCharacteristic.allStreamsIndependent:
                setEffectToStream(Windows.Media.Capture.MediaStreamType.videoPreview, effect, function () {
                    setEffectToStream(Windows.Media.Capture.MediaStreamType.videoRecord, effect, function () {
                        setEffectToStream(Windows.Media.Capture.MediaStreamType.photo, effect);
                    });
                });
                break;
        }
    }

    function enumerateCameras() {
        displayStatus("Enumerating capture devices");
        var cameraSelect = id("cameraSelect");
        cameraList = null;
        cameraList = new Array();

        // Clear the previous list of capture devices if any
        while (cameraSelect.length > 0) {
            cameraSelect.remove(0);
        }

        // Enumerate cameras and add them to the list
        var deviceInfo = Windows.Devices.Enumeration.DeviceInformation;
        deviceInfo.findAllAsync(Windows.Devices.Enumeration.DeviceClass.videoCapture).done(function (cameras) {
            if (cameras.length === 0) {
                cameraSelect.disabled = true;
                displayError("No camera was found");
                id("btnStartDevice" + scenarioId).disabled = true;
                cameraSelect.add(new Option("No cameras available"));

            } else {
                cameras.forEach(function (camera) {
                
                    // Make use of the camera's location if it is available to the description
                    var camLocation = camera.enclosureLocation;
                    if (camLocation !== null) {
                        if (camLocation.panel === Windows.Devices.Enumeration.Panel.front) {
                            cameraList.push(camera);
                            cameraSelect.add(new Option(camera.name + " - Front"));
                        } else if (camLocation.panel === Windows.Devices.Enumeration.Panel.back) {
                            cameraList.push(camera);
                            cameraSelect.add(new Option(camera.name + " - Back"));
                        } else if (camLocation.panel === Windows.Devices.Enumeration.Panel.bottom) {
                            cameraList.push(camera);
                            cameraSelect.add(new Option(camera.name + " - Bottom"));
                        } else if (camLocation.panel === Windows.Devices.Enumeration.Panel.left) {
                            cameraList.push(camera);
                            cameraSelect.add(new Option(camera.name + " - Left"));
                        } else if (camLocation.panel === Windows.Devices.Enumeration.Panel.right) {
                            cameraList.push(camera);
                            cameraSelect.add(new Option(camera.name + " - Right"));
                        } else if (camLocation.panel === Windows.Devices.Enumeration.Panel.top) {
                            cameraList.push(camera);
                            cameraSelect.add(new Option(camera.name + " - Top"));
                        } else {
                            cameraList.push(camera);
                            cameraSelect.add(new Option(camera.name + " - Location Unknown"));
                        }
                
                        // If camera location is unavailable, simply add the microphone to the list
                    } else {
                        cameraList.push(camera);
                        cameraSelect.add(new Option(camera.name + " - Location Unavailable"));
                    }
                });
            }
        }, errorHandler);                      
    }

    function enumerateMicrophones() {
        displayStatus("Enumerating Microphones");
        var microphoneSelect = id("microphoneSelect");
        var microphoneDeviceId = 0;
        microphoneList = new Array();

        // Clear the previous list of microphones if any
        while (microphoneSelect.length > 0) {
            microphoneSelect.remove(0);
        }

        // Enumerate microphones and add them to the list
        var microphoneDeviceInfo = Windows.Devices.Enumeration.DeviceInformation;
        microphoneDeviceInfo.findAllAsync(Windows.Media.Devices.MediaDevice.getAudioCaptureSelector(), null).done(function (deviceInformation) {
            if (deviceInformation.length > 0) {
                for (var i = 0; i < deviceInformation.length; i++) {

                    // Make use of the microphone's location if it is available to the description
                    var micLocation = deviceInformation[i].enclosureLocation;
                    if (micLocation !== null) {
                        if (micLocation.panel === Windows.Devices.Enumeration.Panel.front) {
                            microphoneList.push(deviceInformation[i]);
                            microphoneSelect.add(new Option(microphoneList[i].name + " - Front", i));
                        } else if (micLocation.panel === Windows.Devices.Enumeration.Panel.back) {
                            microphoneList.push(deviceInformation[i]);
                            microphoneSelect.add(new Option(microphoneList[i].name + " - Back", i));
                        } else if (micLocation.panel === Windows.Devices.Enumeration.Panel.bottom) {
                            microphoneList.push(deviceInformation[i]);
                            microphoneSelect.add(new Option(microphoneList[i].name + " - Bottom", i));
                        } else if (micLocation.panel === Windows.Devices.Enumeration.Panel.left) {
                            microphoneList.push(deviceInformation[i]);
                            microphoneSelect.add(new Option(microphoneList[i].name + " - Left", i));
                        } else if (micLocation.panel === Windows.Devices.Enumeration.Panel.right) {
                            microphoneList.push(deviceInformation[i]);
                            microphoneSelect.add(new Option(microphoneList[i].name + " - Right", i));
                        } else if (micLocation.panel === Windows.Devices.Enumeration.Panel.top) {
                            microphoneList.push(deviceInformation[i]);
                            microphoneSelect.add(new Option(microphoneList[i].name + " - Top", i));
                        } else {
                            microphoneList.push(deviceInformation[i]);
                            microphoneSelect.add(new Option(microphoneList[i].name + " - Location Unknown", i));
                        }

                        // If microphone location is unavailable, simply add the microphone to the list
                    } else {
                        microphoneList.push(deviceInformation[i]);
                        microphoneSelect.add(new Option(microphoneList[i].name + " - Location Unavailable", i));
                    }
                }

                // Select the first microphone from the list
                microphoneSelect.selectedIndex = 0;
                initMicrophoneSettings();

            } else {
                microphoneSelect.disabled = true;
                displayError("No Microphones found");
                id("btnStartDevice" + scenarioId).disabled = true;
                microphoneSelect.add(new Option("No microphones available"));
            }
        }, errorHandler);
    }

    function onDeviceChange() {
        releaseMediaCapture();
        id("btnStartDevice" + scenarioId).disabled = false;
        id("btnStartPreview" + scenarioId).disabled = true;
        id("btnRecord" + scenarioId).disabled = true;
        id("btnSnap" + scenarioId).disabled = true;
        id("recordModeSelect").disabled = true;
        id("photoModeSelect").disabled = true;
        id("videoEffect").disabled = true;
        id("videoEffect").checked = false;
        displayStatus("");
        initCameraSettings();
        initMicrophoneSettings();
    }

    function updatePreviewForRotation() {
        if (!mediaCaptureMgr) {
            return;
        }
        var rotDegree;
        var videoEncodingProperties = mediaCaptureMgr.videoDeviceController.getMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoPreview);        

        var previewMirroring = mediaCaptureMgr.getPreviewMirroring();
        var counterclockwiseRotation = (previewMirroring && !reverseVideoRotation) ||
           (!previewMirroring && reverseVideoRotation);                

        //Set the video subtype
        var rotGUID = "{0xC380465D, 0x2271, 0x428C, {0x9B, 0x83, 0xEC, 0xEA, 0x3B, 0x4A, 0x85, 0xC1}}";
        

        if (rotateVideoOnOrientationChange) {
            // Lookup up the rotation degrees.  
            rotDegree = videoPreviewRotationLookup(Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation, counterclockwiseRotation);

            // rotate the preview video
            videoEncodingProperties.properties.insert(rotGUID, rotDegree);
            mediaCaptureMgr.setEncodingPropertiesAsync(Windows.Media.Capture.MediaStreamType.VideoPreview, videoEncodingProperties, null).done(
                function () {
                    if (rotDegree === 90 || rotDegree === 270) {
                        previewVideo2.Height = rotWidth;
                        previewVideo2.Width = rotHeight;
                    } else {
                        previewVideo2.height = rotHeight;
                        previewVideo2.width = rotWidth;
                    }
                });             
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
            mediaCaptureMgr.setRecordRotation(videoRotationLookup(Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation, counterclockwiseRotation));
        } else {
            mediaCaptureMgr.setRecordRotation(Windows.Media.Capture.VideoRotation.none);
        }
    }

    function getCurrentPhotoRotation() {
        var counterclockwiseRotation = reverseVideoRotation;

        if (rotateVideoOnOrientationChange) {
            return photoRotationLookup(Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation, counterclockwiseRotation);
        } else {
            return Windows.Storage.FileProperties.PhotoOrientation.normal;
        }
    }

    function photoRotationLookup(displayOrientation, counterclockwise) {
        switch (displayOrientation) {
            case Windows.Graphics.Display.DisplayOrientations.landscape:
                return Windows.Storage.FileProperties.PhotoOrientation.normal;

            case Windows.Graphics.Display.DisplayOrientations.portrait:
                return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.rotate90 :
                    Windows.Storage.FileProperties.PhotoOrientation.rotate270;

            case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
                return Windows.Storage.FileProperties.PhotoOrientation.rotate180;

            case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
                return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.rotate270 :
                    Windows.Storage.FileProperties.PhotoOrientation.rotate90;

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

    function videoPreviewRotationLookup(displayOrientation, counterclockwise) {
        var degreesToRotate;

        switch (displayOrientation) {
            case Windows.Graphics.Display.DisplayOrientations.landscape: 
                degreesToRotate = 0;
                break;           
            case Windows.Graphics.Display.DisplayOrientations.portrait:
                if (counterclockwise) {
                    degreesToRotate = 270;
                } else {
                    degreesToRotate = 90;
                }
                break;
            case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped: 
                degreesToRotate = 180;
                break;
            case Windows.Graphics.Display.DisplayOrientations.portraitFlipped: 
                if (counterclockwise) {
                    degreesToRotate = 90;
                } else {
                    degreesToRotate = 270;
                }
                break;
            default:
                degreesToRotate = 0;
                break;
        }        

        return degreesToRotate;
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

        // Initialize the UI elements
        id("btnStartDevice" + scenarioId).disabled = false;
        id("btnStartDevice" + scenarioId).addEventListener("click", startDevice, false);
        id("btnStartPreview" + scenarioId).disabled = true;
        id("btnStartPreview" + scenarioId).addEventListener("click", startPreview, false);
        id("btnRecord" + scenarioId).disabled = true;
        id("btnRecord" + scenarioId).addEventListener("click", startStopRecord, false);
        id("btnSnap" + scenarioId).disabled = true;
        id("btnSnap" + scenarioId).addEventListener("click", capturePhoto, false);
        id("cameraSelect").addEventListener("change", onDeviceChange, false);
        id("microphoneSelect").addEventListener("change", onDeviceChange, false);
        id("videoEffect").addEventListener("click", addRemoveEffect, false);
        id("videoEffect").checked = false;
        id("videoEffect").disabled = true;
        id("photoModeSelect").disabled = true;
        id("sceneModeSelect").disabled = true;
        id("recordModeSelect").disabled = true;
        id("photoModeSelect").addEventListener("click", setPhotoMode, false);
        id("recordModeSelect").addEventListener("click", setRecordMode, false);

        // Update the list of capture devices and microphones available
        captureInitSettings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
        enumerateCameras();
        enumerateMicrophones();

        // Disable playback controls if no captured media is present
        if (!id("capturePlayback" + scenarioId).src) {
            id("capturePlayback" + scenarioId).controls = false;
        }

        rotHeight = previewVideo2.height;
        rotWidth = previewVideo2.width;

        // initialize internal flags
        recordState = false;
    }
})();
