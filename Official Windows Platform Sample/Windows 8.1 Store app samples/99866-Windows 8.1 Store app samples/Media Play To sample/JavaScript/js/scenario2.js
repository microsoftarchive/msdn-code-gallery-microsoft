//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var g_playlist = new Array("Chrysanthemum.jpg", "Desert.jpg", "Hydrangeas.jpg", "Jellyfish.jpg", "Koala.jpg");
    var g_playlistCurrIndex = 0;
    var g_playlistImageTimerID = null;
    var g_playlistAutoPlay = false;
    var g_connectionState;
    var g_playToManager = null;

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            id("playlistPlayButton").addEventListener("click", playlistPlay, false);
            id("playlistPauseButton").addEventListener("click", playlistPause, false);
            id("playlistPreviousButton").addEventListener("click", function () { playlistPlayNext(-1); }, false);
            id("playlistNextButton").addEventListener("click", function () { playlistPlayNext(1); }, false);
            id("playlistSelect").addEventListener("change", onPlaylistChange, false);
            id("ImageSource").msPlayToSource.connection.addEventListener("statechanged", playToSrcStateChangeHandler);
            id("ImageSource").msPlayToSource.connection.addEventListener("error", playToSrcErrorHandler);
            g_playToManager = Windows.Media.PlayTo.PlayToManager.getForCurrentView();
            g_playToManager.addEventListener("sourcerequested", playToSrcRequestHandler, false);
            playlistInit();
            playlistPlayNext(0);
        },
        unload: function (element, options) {
            g_playToManager.removeEventListener("sourcerequested", playToSrcRequestHandler, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function playToSrcStateChangeHandler(eventIn) {
        g_connectionState = eventIn.currentState;
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
        if (id("ImageSource") !== null) {
            eventIn.sourceRequest.setSource(id("ImageSource").msPlayToSource);
        }
    }

    function playToSrcErrorHandler(eventIn) {
        WinJS.log && WinJS.log("PlayTo connection error: " + eventIn.message, "sample", "error");
    }

    function playlistInit() {
        var playlistSelect = id("playlistSelect");
        for (var i = 0; i < g_playlist.length; i++) {
            var option = document.createElement('OPTION');
            option.value = i;
            option.innerText = g_playlist[i];
            playlistSelect.options.add(option);
        }
    }

    function playlistPlayNext(offset) {
        if (!g_playlist) {
            return;
        }

        var selectedIndex = g_playlistCurrIndex + offset;
        if (selectedIndex >= g_playlist.length) {
            selectedIndex = 0;
        } else if (selectedIndex < 0) {
            selectedIndex = g_playlist.length - 1;
        }

        var playlistSelect = id("playlistSelect");
        var localImage = id("ImageSource");

        if (!playlistSelect) {
            return;
        }

        if ((localImage.msPlayToSource.connection.state !== Windows.Media.PlayTo.PlayToConnectionState.disconnected) &&
            (g_connectionState !== Windows.Media.PlayTo.PlayToConnectionState.rendering)) {
            return;
        }

        if (selectedIndex < playlistSelect.options.length) {
            // Cancel previous timer.
            if (g_playlistImageTimerID) {
                clearTimeout(g_playlistImageTimerID);
            }

            // Highlight the new item in playlist.
            id("playlistSelect").selectedIndex = selectedIndex;
            g_playlistCurrIndex = selectedIndex;

            // Play it.
            var player = id("ImageSource");
            player.src = "images\\" + g_playlist[selectedIndex];
            if (g_playlistAutoPlay) {
                g_playlistImageTimerID = setTimeout(function () {
                    playlistPlayNext(1);
                }, 10000);
            }
        }
    }

    function playlistPlay() {
        g_playlistAutoPlay = true;
        playlistPlayNext(0);
    }

    function playlistPause() {
        stopPlayback();
        g_playlistAutoPlay = false;
    }

    function stopPlayback() {
        if (g_playlistImageTimerID) {
            clearTimeout(g_playlistImageTimerID);
        }
    }

    function onPlaylistChange() {
        g_playlistCurrIndex = id("playlistSelect").selectedIndex;
        playlistPlayNext(0);
    }

})();
