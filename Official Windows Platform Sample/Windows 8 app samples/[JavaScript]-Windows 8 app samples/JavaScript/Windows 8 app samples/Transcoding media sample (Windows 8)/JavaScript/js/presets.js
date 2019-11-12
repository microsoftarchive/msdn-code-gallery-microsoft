//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var g_useMp4 = true,
        g_outputFileName = "TranscodeSampleOutput.mp4";

    var g_inputFile = null,
        g_outputFile = null;

    var g_profile,
        g_transcodeOp;

    var g_openPickerOp = null;

    var page = WinJS.UI.Pages.define("/html/presets.html", {
        ready: function (element, options) {
            id("pickFileButton").addEventListener("click", pickFile, false);
            id("targetFormat").addEventListener("change", onTargetFormatChanged, false);
            id("transcode").addEventListener("click", onTranscode, false);
            id("cancel").addEventListener("click", transcodeCancel, false);
        }
    });

    function onTranscode() {
        stopPlayers();
        disableButtons();

        // Create transcode object.
        var transcoder = null;
        transcoder = new Windows.Media.Transcoding.MediaTranscoder();

        // Get transcode profile.
        getPresetProfile(id("profileSelect"));

        // Clear messages.
        WinJS.log && WinJS.log("", "sample", "status");

        // Create output file and transcode.
        var videoLib = Windows.Storage.KnownFolders.videosLibrary;
        var createFileOp = videoLib.createFileAsync(g_outputFileName,
            Windows.Storage.CreationCollisionOption.generateUniqueName);
        createFileOp.done(function (ofile) {
            g_outputFile = ofile;
            g_transcodeOp = null;
            var prepareOp = transcoder.prepareFileTranscodeAsync(g_inputFile, g_outputFile, g_profile);
            prepareOp.done(function (result) {
                if (result.canTranscode) {
                    g_transcodeOp = result.transcodeAsync();
                    g_transcodeOp.done(transcodeComplete, transcoderErrorHandler, transcodeProgress);
                } else {
                    transcodeFailure(result.failureReason);
                }
            }); // prepareOp.done
            id("cancel").disabled = false;
        }); // createFileOp.done
    }


    function getPresetProfile(profileSelect) {
        g_profile = null;
        var mediaProperties = Windows.Media.MediaProperties;
        var videoEncodingProfile;
        switch (profileSelect.selectedIndex) {
            case 0:
                videoEncodingProfile = mediaProperties.VideoEncodingQuality.hd1080p;
                break;
            case 1:
                videoEncodingProfile = mediaProperties.VideoEncodingQuality.hd720p;
                break;
            case 2:
                videoEncodingProfile = mediaProperties.VideoEncodingQuality.wvga;
                break;
            case 3:
                videoEncodingProfile = mediaProperties.VideoEncodingQuality.ntsc;
                break;
            case 4:
                videoEncodingProfile = mediaProperties.VideoEncodingQuality.pal;
                break;
            case 5:
                videoEncodingProfile = mediaProperties.VideoEncodingQuality.vga;
                break;
            case 6:
                videoEncodingProfile = mediaProperties.VideoEncodingQuality.qvga;
                break;
        }
        if (g_useMp4) {
            g_profile = mediaProperties.MediaEncodingProfile.createMp4(videoEncodingProfile);
        } else {
            g_profile = mediaProperties.MediaEncodingProfile.createWmv(videoEncodingProfile);
        }
    }

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function stopPlayers() {
        var video = id("inputVideo");
        if (!video.paused) {
            video.pause();
        }
        video = id("outputVideo");
        if (!video.paused) {
            video.pause();
        }
    }

    function transcoderErrorHandler(error) {
        WinJS.log && WinJS.log(error.message, "sample", "error");
        enableButtons();
        id("cancel").disabled = true;
    }

    function transcodeProgress(percent) {
        WinJS.log && WinJS.log("Progress: " + percent.toString().split(".")[0] + "%", "sample", "status");
    }

    function transcodeComplete(result) {
        WinJS.log && WinJS.log("Transcode completed.", "sample", "status");
        enableButtons();
        id("cancel").disabled = true;
        try {
            id("outputPath").innerHTML = "Output (" + g_outputFile.path + ")";
            var video = id("outputVideo");
            video.src = URL.createObjectURL(g_outputFile, { oneTimeOnly: true });
            video.play();
        }
        catch (e) {
            WinJS.log && WinJS.log(e.message, "sample", "error");
        }
    }

    function transcodeFailure(failureReason) {
        if (g_outputFile !== null) {
            g_outputFile.deleteAsync();
        }

        switch (failureReason) {
            case Windows.Media.Transcoding.TranscodeFailureReason.codecNotFound:
                transcoderErrorHandler(new Error("Codec not found."));
                break;
            case Windows.Media.Transcoding.TranscodeFailureReason.invalidProfile:
                transcoderErrorHandler(new Error("Invalid profile."));
                break;
            case Windows.Media.Transcoding.TranscodeFailureReason.unknown:
                transcoderErrorHandler(new Error("Unknown failure."));
                break;
        }
    }

    function transcodeCancel() {
        if (g_transcodeOp) {
            g_transcodeOp.cancel();
            id("cancel").disabled = true;
            if (g_outputFile !== null) {
                g_outputFile.deleteAsync();
            }
        }
    }

    function enableButtons() {
        id("pickFileButton").disabled = false;
        id("targetFormat").disabled = false;
        id("transcode").disabled = false;
    }

    function disableButtons() {
        id("pickFileButton").disabled = true;
        id("targetFormat").disabled = true;
        id("transcode").disabled = true;
    }

    function pickFile() {
        // Verify that we are currently not snapped, or that we can unsnap to open the picker
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        if (currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped && !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
            WinJS.log && WinJS.log("Cannot pick files while application is in snapped view", "sample", "error");
        }
        else {
            g_openPickerOp = null;
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.videosLibrary;
            openPicker.fileTypeFilter.replaceAll([".mp4", ".wmv"]);
            g_openPickerOp = openPicker.pickSingleFileAsync();
            g_openPickerOp.done(function (file) {
                try {
                    if (file) {
                        g_inputFile = file;
                        // Load video on video tag.
                        var video = id("inputVideo");
                        video.src = URL.createObjectURL(file, { oneTimeOnly: true });
                        video.play();
                        // Enable buttons.
                        enableButtons();
                        g_openPickerOp = null;
                    }
                } catch (e) {
                    WinJS.log && WinJS.log(e.message, "sample", "error");
                }
            });
        }
    }

    function onTargetFormatChanged() {
        if (id("targetFormat").selectedIndex === 0) {
            g_outputFileName = "TranscodeSampleOutput.mp4";
            g_useMp4 = true;
        } else {
            g_outputFileName = "TranscodeSampleOutput.wmv";
            g_useMp4 = false;
        }
    }
})();
