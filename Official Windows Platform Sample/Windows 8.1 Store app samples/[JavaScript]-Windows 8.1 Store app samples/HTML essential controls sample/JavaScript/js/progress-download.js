//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/progress-download.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();

            initializeProgress();   // initialize the file download example
        }
    });

})();

var downloadStatus = null;
var downloadProgress = null;
var completeTimer = null;

function initializeProgress() {
    downloadProgress = document.getElementById("downloadProgress");
    downloadStatus = document.getElementById("downloadStatus");
}

function progressPauseResume() {
    var btn = document.getElementById("downloadPauseButton");
    var container = document.getElementById("downloadContainer");

    // Position -1 means the progress control is indeterminate. When determinate, position equals value/max.
    if (("hidden" === btn.style.visibility) || (-1 === downloadProgress.position)) {
        // still during connecting. can't pause/resume.
        return;
    }

    if (WinJS.Utilities.hasClass(btn, "paused")) {
        // the download was paused, therefore now we resume it.

        // Add your code here to resume download

        WinJS.Utilities.removeClass(downloadProgress, "win-paused");
        downloadStatus.innerText = "Downloading";

        WinJS.Utilities.removeClass(btn, "paused");
        WinJS.Utilities.addClass(btn, "downloading");

        // add a tooltip to the container to tell what will happen if the user clicks again.
        container.title = "Pause";

        document.getElementById("increaseValue").disabled = false;
    }
    else if (WinJS.Utilities.hasClass(btn, "downloading")) {
        // the download was running, therefore now we pause it.

        // Add your code here to pause download

        WinJS.Utilities.addClass(downloadProgress, "win-paused");
        downloadStatus.innerText = "Paused";

        WinJS.Utilities.removeClass(btn, "downloading");
        WinJS.Utilities.addClass(btn, "paused");

        // add a tooltip to the container to tell what will happen if the user clicks again.
        container.title = "Resume";

        document.getElementById("increaseValue").disabled = true;
    }
}

function progressSwitchDeterminate() {
    // Position -1 means the progress control is indeterminate.
    if (-1 === downloadProgress.position) {
        // Add your code here to finish the initialization/estimation and start the download

        // Do a fade-in animation whenever switching from indeterminate progress bar to determinate progress bar
        WinJS.Utilities.removeClass(downloadProgress, "switchDeterminate");
        WinJS.Utilities.addClass(downloadProgress, "switchDeterminate");

        downloadProgress.setAttribute("value", "0");

        downloadStatus.innerText = "Downloading";
        document.getElementById("downloadPauseButton").style.visibility = "visible";    // show the pause/resume button
        document.getElementById("downloadContainer").title = "Pause";                   // can be paused now
        document.getElementById("increaseValue").disabled = false;                      // allow increasing the value of progress
        document.getElementById("switchDeterminate").disabled = true;
    }
}

function progressIncrease() {
    downloadProgress.value = downloadProgress.value + 0.2;  // default max is 1.0

    if (parseInt(downloadProgress.value) >= 1) {
        // Since the progress fill may need more time to animate to 100%, please wait for 1s here before hiding the download UI.
        completeTimer = setTimeout("downloadComplete();", 1000);
    }
}

function downloadComplete() {
    document.getElementById("downloadPauseButton").style.visibility = "hidden";
    document.getElementById("downloadControl").style.visibility = "hidden";
    downloadStatus.innerText = "Completed";
    document.getElementById("increaseValue").disabled = true;
}

function progressReset() {
    // avoid the completion timer changing status after this is called
    clearTimeout(completeTimer);

    downloadProgress.setAttribute("value", "0");
    downloadProgress.removeAttribute("value");

    var btn = document.getElementById("downloadPauseButton");
    WinJS.Utilities.removeClass(btn, "paused");
    WinJS.Utilities.addClass(btn, "downloading");
    btn.style.visibility = "hidden"; // because the indeterminate activity of connecting to the server can't be paused/resumed.

    document.getElementById("downloadControl").style.visibility = "visible";

    downloadStatus.innerText = "Connecting";

    WinJS.Utilities.removeClass(downloadProgress, "win-paused");

    document.getElementById("downloadContainer").title = "";    // remove the "pause"/"resume" tooltip since it's not allowed in this state.

    document.getElementById("increaseValue").disabled = true; // should switch to determinate first.
    document.getElementById("switchDeterminate").disabled = false;
}
