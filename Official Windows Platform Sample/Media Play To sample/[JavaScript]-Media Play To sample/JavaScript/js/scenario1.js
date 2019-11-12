//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var g_playToManager = null;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            id("VideoSource").msPlayToSource.connection.addEventListener("statechanged", playToSrcStateChangeHandler);
            id("VideoSource").msPlayToSource.connection.addEventListener("error", playToSrcErrorHandler);
            id("playWebContentButton").addEventListener("click", playWebContent, false);
            id("pickFileButton").addEventListener("click", pickFile, false);
            id("playToButton").addEventListener("click", showPlayTo, false);
            g_playToManager = Windows.Media.PlayTo.PlayToManager.getForCurrentView();
            g_playToManager.addEventListener("sourcerequested", playToSrcRequestHandler, false);
        },
        unload: function (element, options) {
            g_playToManager.removeEventListener("sourcerequested", playToSrcRequestHandler, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function playToSrcStateChangeHandler(eventIn) {
        var states = Windows.Media.PlayTo.PlayToConnectionState;
        if (eventIn.currentState === states.disconnected) {
            WinJS.log && WinJS.log("PlayTo connection state: Disconnected", "sample", "status");
        } else if (eventIn.currentState === states.connected) {
            WinJS.log && WinJS.log("PlayTo connection state: Connected", "sample", "status");
        } else if (eventIn.currentState === states.rendering) {
            WinJS.log && WinJS.log("PlayTo connection state: Rendering", "sample", "status");
        }
    }

    function playToSrcRequestHandler(eventIn) {
        if (id("VideoSource") !== null) {
            eventIn.sourceRequest.setSource(id("VideoSource").msPlayToSource);
        }
    }

    function playToSrcErrorHandler(eventIn) {
        WinJS.log && WinJS.log("PlayTo connection error: " + eventIn.message, "sample", "error");
    }

    function pickFile() {
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.videosLibrary;
        openPicker.fileTypeFilter.replaceAll([".mp4", ".wmv"]);
        openPicker.pickSingleFileAsync().then(function (file) {
            if (file) {
                var localVideo = id("VideoSource");
                localVideo.src = URL.createObjectURL(file, { oneTimeOnly: true });
                localVideo.play();
            }
        },
        handleError);
    }

    function handleError(error) {
        WinJS.log && WinJS.log("Error: " + error.message, "sample", "error");
    }

    function playWebContent() {
        var localVideo = id("VideoSource");
        localVideo.src = "http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4";
        localVideo.play();
    }

    function showPlayTo() {
        Windows.Media.PlayTo.PlayToManager.showPlayToUI();
    }

})();
