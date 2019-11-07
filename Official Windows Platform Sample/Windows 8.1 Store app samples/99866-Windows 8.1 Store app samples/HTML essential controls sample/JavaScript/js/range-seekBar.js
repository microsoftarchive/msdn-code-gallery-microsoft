//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/range-seekBar.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();

            setupPlayer();          // initialize the video seek bar example
        }
    });

})();

var player = null;
var seekbar = null;
var popupDiv = null;
var videoUrl = "";

// when the user is interacting with the seek bar, pause the video, seek it, and don't look at position change events from the video.
var interactingWithSeekBar = false;

function playVideo(videoName, autoplay) {
    player.pause();
    player.src = videoName;
    player.autoplay = autoplay;
    player.load();

    // reset the interaction status
    interactingWithSeekBar = false;
}

// Switch the CSS class that contains the font glyph. Also change the title tooltip.
function switchPlayButton(playing) {
    var btn = document.getElementById("playbutton");
    if (btn) {
        if (playing) {
            btn.setAttribute("class", "playing");
            btn.setAttribute("title", "Pause");
        } else {
            btn.setAttribute("class", "paused");
            btn.setAttribute("title", "Play");
        }
    }
}

function startInteractingWithSeekBar() {
    interactingWithSeekBar = true;

    // pause the player
    if ((player.readyState >= 2) && (!player.paused)) { 
        player.pause();
    }
}

function stopInteractingWithSeekBar() {
    interactingWithSeekBar = false;

    // resume the player
    if (player.paused) {
        player.play();
    }
}

function setupPlayer() {
    seekbar = document.getElementById("seekbar");
    seekbar.value = 0;
    seekbar.addEventListener("change", seekVideo, false);

    // the user started interacting with the seek bar
    seekbar.addEventListener("keydown", startInteractingWithSeekBar, false);
    seekbar.addEventListener("pointerdown", startInteractingWithSeekBar, false);

    // the user finished interacting with the seek bar
    seekbar.addEventListener("keyup", stopInteractingWithSeekBar, false);
    seekbar.addEventListener("pointerup", stopInteractingWithSeekBar, false);
    seekbar.addEventListener("pointercancel", stopInteractingWithSeekBar, false);

    player = document.getElementById("player");
    player.addEventListener("durationchange", setupSeekbar, false);
    player.addEventListener("timeupdate", updateUI, false);
    player.addEventListener("ended", ended, false);
    player.addEventListener("error", error, false);
    player.addEventListener("play", function () { switchPlayButton(true); }, false);
    player.addEventListener("pause", function () { switchPlayButton(false); }, false);

    document.getElementById("playbutton").setAttribute("class", "paused");
    document.getElementById("playbutton").addEventListener("click", playPause, false);
    document.getElementById("player").addEventListener("click", playPause, false);   // click anywhere except the seek bar will play/pause

    if (videoUrl) {
        playVideo(videoUrl, false);
    }
}

function error(e) {
    document.getElementById("videoPlaybackError").innerText = "Error: can't play the video. Please make sure the file path and file type are correct.";
}

function setupSeekbar() {
    seekbar.min = 0;
    seekbar.max = player.duration;
    seekbar.value = 0;
}

function updateUI() {
    if ((!interactingWithSeekBar) && (!player.paused) && (!player.seeking)) {
        // the user is not interacting with the seek bar and the video is playing, thus the change in video playback position should be reflected to the seek bar position.
        seekbar.value = player.currentTime;
    }
}

function ended() {
    seekbar.value = 0;
    player.pause();
    player.currentTime = 0;
}

function seekVideo(e) {
    if (player.readyState < 2) {
        e.target.value = "0";
        return;
    }

    // seek using slider value
    player.currentTime = e.target.valueAsNumber;
}

function playPause(e) {
    if ("" === videoUrl) {
        loadVideo();
        return;
    }

    if (player.readyState < 2) { return; }

    if (player.paused) {
        player.play();

    } else {
        player.pause();
    }
}

function loadVideo() {
    if (!window.Windows) {
        document.getElementById("videoPlaybackError").innerText = "Error: the file picker only works when running as a Windows Store app.";
        return;
    }

    document.getElementById("videoPlaybackError").innerText = "";

    // Open the WinRT file picker
    var picker = new Windows.Storage.Pickers.FileOpenPicker();

    picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.videosLibrary;

    picker.fileTypeFilter.replaceAll(["*"]);

    picker.pickSingleFileAsync().then(function (file) {
        if (file) {
            videoUrl = URL.createObjectURL(file, { oneTimeOnly: true });
            if (videoUrl) {
                playVideo(videoUrl, true);
            } else {
                document.getElementById("videoPlaybackError").innerText = "Error: can't load the file.";
            }
        }
    });
}
