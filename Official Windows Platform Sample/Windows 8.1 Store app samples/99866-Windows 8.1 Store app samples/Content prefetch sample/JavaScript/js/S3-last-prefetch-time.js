//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S3-last-prefetch-time.html", {
        ready: function (element, options) {
            document.getElementById("getLastPrefetchTime").addEventListener("click", getLastPrefetchTime, false);
        }
    });

    function getLastPrefetchTime() {
        var label = document.getElementById("lastPrefetchTime");
        if (label) {
            var lastPrefetchTime = Windows.Networking.BackgroundTransfer.ContentPrefetcher.lastSuccessfulPrefetchTime;
            if (lastPrefetchTime) {
                label.textContent = "The last successful prefetch time was " + lastPrefetchTime.toString();
            } else {
                label.textContent = "There have been no successful prefetches yet";
            }
        }
    }
})();
