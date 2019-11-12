//// Copyright (c) Microsoft Corporation. All rights reserved
(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1ScannerEnumeration.html", {
        ready: function (element, options) {
            document.getElementById("startEnumerationWatcher").addEventListener("click", startEnumerationWatcher, false);
            document.getElementById("stopEnumerationWatcher").addEventListener("click", stopEnumerationWatcher, false);
            updateButtons();
        }
    });

    function startEnumerationWatcher() {
        ScannerContext.startScannerWatcher();
        updateButtons();
        WinJS.log && WinJS.log("Scanner enumeration watcher started", "sample", "status");
    }

    function stopEnumerationWatcher() {
        ScannerContext.stopScannerWatcher();
        updateButtons();
        WinJS.log && WinJS.log("Scanner enumeration watcher stopped.", "sample", "status");
    }

    function updateButtons() {
        document.getElementById("startEnumerationWatcher").disabled = ScannerContext.watcherStarted;
        document.getElementById("stopEnumerationWatcher").disabled = !ScannerContext.watcherStarted;
    }
})();
