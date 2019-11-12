//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/progress-intro.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });

})();

function completeImageDownload() {
    var progress = document.getElementById("imageDownloadProgress");

    if (!progress) {
        return;
    }

    // Because the CSS animation keeps running and consuming CPU/GPU when the element is hidden,
    // remove the control, rather than using display:none.
    // The Progress control stops its internal CSS animation when it is removed from DOM tree.
    progress.parentNode.removeChild(progress);

    WinJS.Utilities.addClass(document.getElementById("imagePlaceholder"), "completed");


    document.getElementById("btnCompleteImageDownload").disabled = true;
    document.getElementById("btnResetImageDownload").disabled = false;
}

function resetImageDownload() {
    var container = document.getElementById("imagePlaceholder");
    WinJS.Utilities.removeClass(container, "completed");

    // re-create the control
    var newProgress = document.createElement("progress");
    newProgress.id = "imageDownloadProgress";
    newProgress.setAttribute("class", "win-ring");
    container.appendChild(newProgress);

    document.getElementById("btnCompleteImageDownload").disabled = false;
    document.getElementById("btnResetImageDownload").disabled = true;
}

function updateGradientProgress() {
    var progress = document.getElementById("myGradientProgress");
    if (progress) {
        progress.value = 1;
    }

    document.getElementById("btnUpdateGradientProgress").disabled = true;
    document.getElementById("btnResetGradientProgress").disabled = false;
}

function resetGradientProgress() {
    // re-create the control with value 0
    var container = document.getElementById("gradientProgressContainer");
    while (container.childNodes.length >= 1) {
        container.removeChild(container.firstChild);
    }

    var newProgress = document.createElement("progress");
    newProgress.id = "myGradientProgress";
    newProgress.setAttribute("value", "0");
    newProgress.setAttribute("class", "gradientProgress");
    container.appendChild(newProgress);

    document.getElementById("btnUpdateGradientProgress").disabled = false;
    document.getElementById("btnResetGradientProgress").disabled = true;
}
