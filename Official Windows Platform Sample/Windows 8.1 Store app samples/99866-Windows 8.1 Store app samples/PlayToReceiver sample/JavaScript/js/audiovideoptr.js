//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var g_receiver = null;
    var g_displayRequest = null;
    var g_receiverStarted = false;
    var g_elementHandler = makeElementHandler();
    var g_receiverHandler = makeReceiverHandler();
    var g_stopping = false;
    var g_pause = true;
    var g_hasVideoSource = false;
    var g_imageURL = null;

    var page = WinJS.UI.Pages.define("/html/audiovideoptr.html", {
        ready: function (element, options) {
            id("dmrImage").style.display = "none";
            id("dmrVideo").style.display = "none";
            document.getElementById("startDMRButton").addEventListener("click", startPlayToReceiver, false);
            document.getElementById("stopDMRButton").addEventListener("click", stopPlayToReceiver, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function showVideo(){
        id("dmrImage").style.display = "none";
        id("dmrVideo").style.display = "";
    }

    function showImage(){
        id("dmrVideo").style.display = "none";
        id("dmrImage").style.display = "";
    }

    function onLoadImage(){
        var image = id("dmrImage");
        var dblZoomRatio = 1.0;
        if(image.naturalWidth !== 640 && image.naturalHeight !== 480 && 
            image.naturalWidth !== 0 && image.naturalHeight !== 0){
            var dblWidthZoom  = 640/image.naturalWidth;
            var dblHeightZoom = 480/image.naturalHeight;
            dblZoomRatio = Math.min(dblHeightZoom,dblWidthZoom);
        }
        image.style.width = (image.naturalWidth * dblZoomRatio) + "px";
        image.style.height = (image.naturalHeight * dblZoomRatio) + "px";
        showImage();
    }

    function makeElementHandler() {
        return {
            volumechange: function () { g_receiver.notifyVolumeChange(id("dmrVideo").volume, id("dmrVideo").muted); },
            ratechange: function () { g_receiver.notifyRateChange(id("dmrVideo").playbackRate); },
            loadedmetadata: function () { g_receiver.notifyLoadedMetadata(); },
            durationchange: function () { 
                if (id("dmrVideo").duration !== Infinity) {  // for realtime content, the duration received from the DMC is always zero, which translates to Infinity for Video tag Duration field
                    g_receiver.notifyDurationChange(id("dmrVideo").duration * 1000);
                }
	    },
            seeking: function () {
                if (!g_stopping) {  //per DLNA guidelines we need to suppress seek(0) notification when stopping
                    g_receiver.notifySeeking();
                }
            },
            seeked: function () {
                if (g_stopping) {   //per DLNA guidelines we need to suppress seek(0) notification when stopping
                    g_stopping = false;
                }
                else {
                    g_receiver.notifySeeked();
                }
            },
            playing: function () { g_receiver.notifyPlaying(); },
            pause: function () {
                if (g_stopping || (!g_pause && id("dmrVideo").currentTime === 0)) {
                    g_receiver.notifyStopped();
                }
                else{
                    g_receiver.notifyPaused();
                    g_pause = false;
                }
            },
            ended: function () { g_receiver.notifyEnded(); },
            error: function () {
                if (g_hasVideoSource) {
                    g_receiver.notifyError();
                    g_receiver.notifyStopped();
                }
            }
        };
    }

    function makeReceiverHandler() {
        return {
            playrequested: function () { 
                if(g_imageURL === null){
                    id("dmrVideo").play(); 
                }
                else{
                    id("dmrImage").src = g_imageURL;
                    g_receiver.notifyPlaying();
                }
            },
            pauserequested: function () { 
                if(!g_imageURL){
                    g_pause = true;
                    g_stopping = false;
                    id("dmrVideo").pause(); 
                }
            },
            playbackratechangerequested: function (eventIn) {
                if (!g_imageURL) {
                    id("dmrVideo").playbackRate = eventIn.rate;
                }
            },
            mutedchangerequested: function (eventIn) { id("dmrVideo").muted = eventIn.mute; },
            volumechangerequested: function (eventIn) { id("dmrVideo").volume = eventIn.volume; },
            currenttimechangerequested: function (eventIn) {
                if (!g_imageURL) {
                    if ((id("dmrVideo").currentTime !== 0) || (eventIn.time !== 0)) {
                        id("dmrVideo").currentTime = eventIn.time / 1000;
                    }
                }
            },
            sourcechangerequested: function (eventIn) {
                g_imageURL = null;
                g_pause = false;
                g_stopping = false;
                if (!eventIn.stream) {
                    g_hasVideoSource = false;
                    id("dmrVideo").removeAttribute("src");
                    id("dmrImage").style.display = "none";
                    id("dmrVideo").style.display = "none";
                } else {
                    var blob = MSApp.createBlobFromRandomAccessStream(eventIn.stream.contentType, eventIn.stream);
                    if(eventIn.stream.contentType.substring(0, 5) !== "image") {
                        showVideo();
                        g_hasVideoSource = true;
                        id("dmrVideo").src = URL.createObjectURL(blob, {oneTimeOnly: true});
                        g_imageURL = null;
                    }
                    else{
                        g_imageURL = URL.createObjectURL(blob, {oneTimeOnly: true});
                        g_hasVideoSource = false;
                        id("dmrVideo").removeAttribute("src");
                        g_receiver.notifyDurationChange(0);
                        g_receiver.notifyLoadedMetadata();
                    }
                }
            },
            timeupdaterequested: function (eventIn) {
                if (!g_imageURL) {
                    g_receiver.notifyTimeUpdate(id("dmrVideo").currentTime * 1000);
                } else {
                    g_receiver.notifyTimeUpdate(0);
                }
            },
            stoprequested: function (eventIn) {
                if(!g_imageURL){
                    if (id("dmrVideo").readyState !== 0) {
                        g_pause = false;
                        g_stopping = true;

                        if(id("dmrVideo").paused){
                            if(id("dmrVideo").currentTime !== 0){
                                id("dmrVideo").currentTime = 0;
                            }
                            g_receiver.notifyStopped();
                        }
                        else{
                            id("dmrVideo").pause();
                            id("dmrVideo").currentTime = 0;
                        }
                    }
                    else {
                        g_receiver.notifyError();
                        g_receiver.notifyStopped();
                    }
                }
                else {
                    id("dmrImage").style.display = "none";
                    g_receiver.notifyStopped();
                }
            }
        };
    }

    function startPlayToReceiver() {
        try {
            if (!g_receiver) {
                g_receiver = new Windows.Media.PlayTo.PlayToReceiver();
            }
            if (g_receiverStarted) {
                WinJS.log && WinJS.log("Receiver already started", "Receiver", "status");
                return;
            }

            //
            // Connect: element -> PlayToReceiver
            //
            var dmrVideo = id("dmrVideo");
            dmrVideo.addEventListener("volumechange", g_elementHandler.volumechange, false);
            dmrVideo.addEventListener("ratechange", g_elementHandler.ratechange, false);
            dmrVideo.addEventListener("loadedmetadata", g_elementHandler.loadedmetadata, false);
            dmrVideo.addEventListener("durationchange", g_elementHandler.durationchange, false);
            dmrVideo.addEventListener("seeking", g_elementHandler.seeking, false);
            dmrVideo.addEventListener("seeked", g_elementHandler.seeked, false);
            dmrVideo.addEventListener("playing", g_elementHandler.playing, false);
            dmrVideo.addEventListener("pause", g_elementHandler.pause, false);
            dmrVideo.addEventListener("ended", g_elementHandler.ended, false);
            dmrVideo.addEventListener("error", g_elementHandler.error, false);
            var dmrImage = id("dmrImage");
            dmrImage.addEventListener("load", onLoadImage);

            //
            // Connect: PlayToReceiver -> element
            //
            g_receiver.addEventListener("playrequested", g_receiverHandler.playrequested, false);
            g_receiver.addEventListener("pauserequested", g_receiverHandler.pauserequested, false);
            g_receiver.addEventListener("sourcechangerequested", g_receiverHandler.sourcechangerequested, false);
            g_receiver.addEventListener("playbackratechangerequested", g_receiverHandler.playbackratechangerequested, false);
            g_receiver.addEventListener("currenttimechangerequested", g_receiverHandler.currenttimechangerequested, false);
            g_receiver.addEventListener("mutechangerequested", g_receiverHandler.mutedchangerequested, false);
            g_receiver.addEventListener("volumechangerequested", g_receiverHandler.volumechangerequested, false);
            g_receiver.addEventListener("timeupdaterequested", g_receiverHandler.timeupdaterequested, false);
            g_receiver.addEventListener("stoprequested", g_receiverHandler.stoprequested, false);
            g_receiver.supportsVideo = true;
            g_receiver.supportsAudio = true;
            g_receiver.supportsImage = true;
            g_receiver.friendlyName = 'SDK JS Sample PlayToReceiver';

            //
            // Advertise the receiver on the local network and start receiving commands
            //
            g_receiver.startAsync().then(function () {
                g_receiverStarted = true;
                startDMRButton.disabled = true;
                stopDMRButton.disabled = false;
                g_receiver.notifyVolumeChange(id("dmrVideo").volume, id("dmrVideo").muted);
                //
                // Prevent the screen from locking
                //
                if (!g_displayRequest) {
                    g_displayRequest = new Windows.System.Display.DisplayRequest();
                }
                g_displayRequest.requestActive();

                WinJS.log && WinJS.log("Receiver \"" + g_receiver.friendlyName + "\" started.", "receiver", "status");
            }, function (e) {
                WinJS.log && WinJS.log("Receiver \"" + g_receiver.friendlyName + "\" returned an error (" + e.message + ")", "receiver", "error");
                removeVideoEventListeners();
                removeDMREventListeners();
            });
        }
        catch (err) {
            WinJS.log && WinJS.log("Error : " + err.message, "receiver", "error");
            g_receiver = null;
            g_receiverStarted = false;
        }
    }

    function removeVideoEventListeners() {
        var video = id("dmrVideo");
        if (!video.paused) {
            video.pause();
        }

        video.removeEventListener("volumechange", g_elementHandler.volumechange, false);
        video.removeEventListener("ratechange", g_elementHandler.ratechange, false);
        video.removeEventListener("loadedmetadata", g_elementHandler.loadedmetadata, false);
        video.removeEventListener("durationchange", g_elementHandler.durationchange, false);
        video.removeEventListener("seeking", g_elementHandler.seeking, false);
        video.removeEventListener("seeked", g_elementHandler.seeked, false);
        video.removeEventListener("playing", g_elementHandler.playing, false);
        video.removeEventListener("pause", g_elementHandler.pause, false);
        video.removeEventListener("ended", g_elementHandler.ended, false);
        video.removeEventListener("error", g_elementHandler.error, false);
    }

    function removeDMREventListeners() {
        g_receiver.removeEventListener("playrequested", g_receiverHandler.playrequested, false);
        g_receiver.removeEventListener("pauserequested", g_receiverHandler.pauserequested, false);
        g_receiver.removeEventListener("sourcechangerequested", g_receiverHandler.sourcechangerequested, false);
        g_receiver.removeEventListener("playbackratechangerequested", g_receiverHandler.playbackratechangerequested, false);
        g_receiver.removeEventListener("currenttimechangerequested", g_receiverHandler.currenttimechangerequested, false);
        g_receiver.removeEventListener("mutedchangerequested", g_receiverHandler.mutedchangerequested, false);
        g_receiver.removeEventListener("volumechangerequested", g_receiverHandler.volumechangerequested, false);
        g_receiver.removeEventListener("timeupdaterequested", g_receiverHandler.timeupdaterequested, false);
        g_receiver.removeEventListener("stoprequested", g_receiverHandler.stoprequested, false);
    }

    function stopPlayToReceiver() {
        if (g_receiver && g_receiverStarted) {

            removeVideoEventListeners();
            g_receiver.stopAsync().then(function () {
                removeDMREventListeners();

                g_displayRequest.requestRelease();

                g_receiverStarted = false;
                startDMRButton.disabled = false;
                stopDMRButton.disabled = true;

                WinJS.log && WinJS.log("Receiver stopped.", "receiver", "status");
            },
            function (e) {
                WinJS.log && WinJS.log("Receiver could not stop. Error = " + e.message, "receiver", "error");
            });
        } else {
            WinJS.log && WinJS.log("Receiver not started.", "receiver", "status");
        }
    }

})();
