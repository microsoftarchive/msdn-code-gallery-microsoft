//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    "use strict";
    var mediaCaptureMgr         = null;
    var lowLagPhoto             = null;    
    var photoSequencePrepared   = null;
    var pastFrames              = 5;
    var futureFrames = 5;
    var photosPerSecond = 10;
    var framesCaptured 		= null;
    var frameSelected 		= null;
    var savedFrames             = new Array();
    var captureInitSettings     = null;
    var encodingProfile 	= null;
    var previousFrameTimeStamp 	= null;
    var photoStorage 		= null;
    var photoFile 		= "photo.jpg";
    var returnCellToBlue 	= false;

    var scenarioId              = "4";


    var page = WinJS.UI.Pages.define("/html/PhotoSequence.html", {
        ready: function (element, options) {
            Windows.Media.MediaControl.addEventListener("soundlevelchanged", soundLevelChangedHandler, false);
            scenarioInitialize();

        },
        unload: function (element, options) {
            Windows.Media.MediaControl.removeEventListener("soundlevelchanged", soundLevelChangedHandler, false);
            releaseMediaCapture();
        }
    });

    function soundLevelChangedHandler() {
        if (Windows.Media.MediaControl.soundLevel === Windows.Media.SoundLevel.muted) {            
            releaseMediaCapture();
        }
        else {
            scenarioInitialize();
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
        captureInitSettings.photoCaptureSource = Windows.Media.Capture.PhotoCaptureSource.auto;

        // CR: Do I need to set this for photosequence?
        captureInitSettings.streamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.video;
    }


    function releaseMediaCapture() {        
        document.getElementById("previewVideo" + scenarioId).pause();
        document.getElementById("previewVideo" + scenarioId).src = null;

	photoSequencePrepared = false;
        id("btnSnap" + scenarioId).innerHTML = "Prepare Photo Sequence"; 
        if (mediaCaptureMgr !== null) {
            mediaCaptureMgr.close();
            mediaCaptureMgr = null;
        }

        displayStatus("");
    }

    function startDevice() {
        //Initialize media capture with the settings
        displayStatus("Starting device");
        releaseMediaCapture();
        mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
        mediaCaptureMgr.addEventListener("failed", errorHandler, false);
        
        // Setup the media capture manager and enable the buttons to proceed in the scenario        
        mediaCaptureMgr.initializeAsync(captureInitSettings).done(function (result) {
            id("btnStartPreview" + scenarioId).disabled = false;
            id("btnStartDevice" + scenarioId).disabled = true;
            id("btnSnap" + scenarioId).disabled = false;
        },errorHandler);
    }

    function startPreview() {
        displayStatus("Starting preview");
        id("btnStartPreview" + scenarioId).disabled = true;
        var video = id("previewVideo" + scenarioId);
        video.src = URL.createObjectURL(mediaCaptureMgr, { oneTimeOnly: true });
        video.play();
        displayStatus("Preview started");        
    }

    function preparePhotoSequence() {
        // Check to see if the camerea supports photosequence
        if (!mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.supported) {
            displayStatus("Photo Sequence not supported");
        } else {
            var photoFormat = Windows.Media.MediaProperties.ImageEncodingProperties.createJpeg();            
            mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.thumbnailRequestedSize = 300;
            mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.thumbnailEnabled = true;
                        

            // Past photos will set the number of photos to keep in the buffer for a photosequence
            if (mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.maxPastPhotos > pastFrames) {
                mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.PastPhotoLimit = pastFrames;
            } else {
                mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.PastPhotoLimit = mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.maxPastPhotos;
            }

            // This will set how fast the photos will be taken.
            if (mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.maximumPhotosPerSecond > photosPerSecond) {
                mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.photosPerSecondLimit = photosPerSecond;
            } else {
                mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.photosPerSecondLimit = mediaCaptureMgr.videoDeviceController.lowLagPhotoSequence.maximumPhotosPerSecond;
            }

            // Start up the photosequence.  The camera will now be taking photos in the background after this completes
            mediaCaptureMgr.prepareLowLagPhotoSequenceCaptureAsync(photoFormat).done(function (result) {
                lowLagPhoto = result;                
                id("btnSnap" + scenarioId).innerHTML = "Take Photo Sequence";
                photoSequencePrepared = true;
                displayStatus("Photo sequence ready");                                              
            }, errorHandler);
        }
    }

    // Taking the "photosquence" will actively start collecting the photos.  The first photos will be from the buffer
    // setup when you prepare your photosequence
    function takePhotoSequence() {
        if (photoSequencePrepared) {            
            id("btnSnap" + scenarioId).disabled = true;

            // Clear the preview table
            clearPreviewTable();
            
            lowLagPhoto.startAsync().done(function () {
                framesCaptured = 0;

                // This listener will be called for each photo captured
                lowLagPhoto.addEventListener("photocaptured", photoCaptured);

            });
        }

    }


    function photoCaptured(photo) {
        // We are going to collect a set amount of frames
	var cell = null;
        if (framesCaptured < (pastFrames +futureFrames)) {
            if (framesCaptured === 0) {
                previousFrameTimeStamp = null;
            }

            savedFrames[framesCaptured] = photo.frame.cloneStream();
            var thumbnailPreviewTable = document.getElementById("thumbnailPreview");
            if (thumbnailPreview.rows.length > 0) {
                cell = thumbnailPreviewTable.rows[0].insertCell(-1);
            } else {
                thumbnailPreviewTable.insertRow(-1);
                cell = thumbnailPreviewTable.rows[0].insertCell(-1);
            }

            var thumbnailImage = document.createElement("img");
            thumbnailImage.width = 160;
            thumbnailImage.height = 120;            
            thumbnailImage.src = URL.createObjectURL(photo.thumbnail, { oneTimeOnly: true });

            cell.appendChild(thumbnailImage);

            // this is when we transistion from frames in the past to frames taken after the request
            if ((previousFrameTimeStamp < 0) && (photo.captureTimeOffset >= 0)) {
                highlightCell(cell, "blue");
            }

            cell.onclick = function () {
                displayStatus("Cell " + cell.cellIndex);
                var imageTag = document.getElementById("imageTag" + scenarioId);
                imageTag.src = URL.createObjectURL(savedFrames[cell.cellIndex], { oneTimeOnly: true });

                highlightCell(cell, "yellow");
            };

            displayStatus("Photo captured");
            framesCaptured++;
            previousFrameTimeStamp = photo.captureTimeOffset;

        } else {
            lowLagPhoto.removeEventListener("photocaptured", photoCaptured);
            lowLagPhoto.stopAsync();
            displayStatus("Capture Complete");
            id("btnSnap" + scenarioId).disabled = false;
            
        }
    }

    function highlightCell(selectedCell, color) {
        var previewTable = document.getElementById("thumbnailPreview");        
        
        // Clear the previously selected 
        for (var j = 0, cell; typeof(previewTable.cells[j]) !== 'undefined'; j++) {
	    cell = previewTable.cells[j];
            if (cell.style.borderColor === color){
                cell.style.borderStyle = "none";
                if (returnCellToBlue === true) {
                    cell.style.borderStyle = "solid";
                    cell.style.borderColor = "blue";
                    returnCellToBlue = false;
                }
            }
        }

        frameSelected = selectedCell.cellIndex;
        displayStatus("selected cell " + frameSelected);

        if (selectedCell.style.borderColor === "blue") {
            returnCellToBlue = true;
        }

        selectedCell.style.borderStyle = "solid";
        selectedCell.style.borderColor = color;

        document.getElementById("btnSavePhoto4").disabled = false;

    }

    function saveSelected() {
        Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(photoFile, Windows.Storage.CreationCollisionOption.generateUniqueName).done(
            function (newFile) {
                photoStorage = newFile;
                photoStorage.openAsync(Windows.Storage.FileAccessMode.readWrite).done(
                    function (stream) {
                        var contentStream = savedFrames[frameSelected].cloneStream();
                        Windows.Storage.Streams.RandomAccessStream.copyAndCloseAsync(contentStream.getInputStreamAt(0), stream.getOutputStreamAt(0)).done( function() {
                            displayStatus("PhotoPath " + photoStorage.path);
                        });

                    });
            });
    }

    function clearPreviewTable() {
        var previewTable = document.getElementById("thumbnailPreview");
        if (previewTable.rows.length > 0) {
            previewTable.deleteRow(0);
        }
    }

    function cellSelected() {
        displayStatus("Frame " + framesCaptured + " selected");
    }

    function startStopPhotoSequence() {
        if (photoSequencePrepared) {
            takePhotoSequence();
        } else {
            preparePhotoSequence();
        }

    }

    function errorHandler(err) {
        displayError(err.message);
    }


    function scenarioInitialize() {
        id("btnStartDevice" + scenarioId).disabled = false;
        id("btnStartDevice" + scenarioId).addEventListener("click", startDevice, false);
        id("btnStartPreview" + scenarioId).disabled = true;
        id("btnStartPreview" + scenarioId).addEventListener("click", startPreview, false);
        id("btnSnap" + scenarioId).disabled = true;
        id("btnSnap" + scenarioId).addEventListener("click", startStopPhotoSequence, false);
        id("btnSavePhoto" + scenarioId).disabled = true;
        id("btnSavePhoto" + scenarioId).addEventListener("click", saveSelected, false);
        initCaptureSettings();
	clearPreviewTable();
        displayStatus("");
    }

})();
