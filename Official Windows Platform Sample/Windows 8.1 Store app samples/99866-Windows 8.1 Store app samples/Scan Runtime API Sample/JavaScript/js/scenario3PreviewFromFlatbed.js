//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario3PreviewFromFlatbed.html", {
        ready: function (element, options) {
            // Start scanner watcher if it was not previously started.
            if (!ScannerContext.watcherStarted) {
                ScannerContext.startScannerWatcher();
            }
            // Registering event handlers for start scenario button.
            Utils.id("startSceanrio").addEventListener("click", startScenario, false);
            // Registering event handler for current scanner id change event.
            ScannerContext.events.addEventListener("currentScannerId", updateStartScenarioButton, false);

            resetDisplay();
        },
        unload: function () {
            // Unregister event handlers
            ScannerContext.events.removeEventListener("currentScannerId", updateStartScenarioButton, false);
            Utils.id("scannerList").winControl.unLoad();
        }
    });

    /// <summary>
    /// Even Handler for click on Start Scenario button. Starts the scenario of getting the preview from Faltbed
    /// </summary>
    function startScenario() {
        WinJS.log && WinJS.log("Starting scenario of preview scanning from Flatbed.", "sample", "status");
        resetDisplay();
        var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
        scanToPreview(ScannerContext.currentScannerId, stream);
    }

    /// <summary>
    /// Previews the image from the scanner with given device id
    /// The preview is allowed only if the selected scanner is equipped with a Flatbed source and supports preview.
    /// </summary>
    /// <param name="deviceId">scanner device id</param>
    /// <param name="stream">RandomAccessStream in which preview given the by the scan runtime API is kept</param>
    function scanToPreview(deviceId, stream) {
        // Get the scanner object for this device id
        Windows.Devices.Scanners.ImageScanner.fromIdAsync(deviceId).then(function (myScanner) {
            if (myScanner.isScanSourceSupported(Windows.Devices.Scanners.ImageScannerScanSource.flatbed)) {
                if (myScanner.isPreviewSupported(Windows.Devices.Scanners.ImageScannerScanSource.flatbed)) {
                    WinJS.log && WinJS.log("Scanning.", "sample", "status");
                    // Scan API call to get preview from the flatbed
                    return myScanner.scanPreviewToStreamAsync(Windows.Devices.Scanners.ImageScannerScanSource.flatbed, stream);
                } else {
                    WinJS.log && WinJS.log("The selected scanner does not support preview from Flatbed.", "sample", "status");
                    return WinJS.Promise.then(null);
                }
            } else {
                WinJS.log && WinJS.log("The selected scanner does not report to be equipped with a Flatbed.", "sample", "status");
                return WinJS.Promise.then(null);
            }
           
        }).done(function (result) {
            // Check for result to prevent showing results in case of preview not supported scenarios.
            if (result) {
                if (result.succeeded) {
                    displayResult(stream);                    
                } else {
                    WinJS.log && WinJS.log("Failed to get preview from Flatbed." + scannedFileName, "sample", "error");
                }
            }
            
        }, function (error) {
            resetDisplay();
            Utils.displayErrorMessage(error);
        });
    }

    /// <summary>
    /// Enables or disables start and end scenario buttons depending on the state of the scenario running and scanner available
    /// </summary>
    function updateStartScenarioButton() {
        if (ScannerContext.currentScannerId) {
            Utils.id("startSceanrio").disabled = false;
        } else {
            Utils.id("startSceanrio").disabled = true;
        }
    }

    /// <summary>
    /// Displays image preview 
    /// </summary>
    function displayResult(stream) {
        var image = document.getElementById("displayImage");
        var blob = window.MSApp.createBlobFromRandomAccessStream("image/bmp", stream);
        var url = window.URL.createObjectURL(blob);
        WinJS.log && WinJS.log("Preview scanning is complete. Below is the preview image.", "sample", "status");
        image.src = url;
    }

    /// <summary>
    /// Set image and other display items to default values
    /// </summary>
    function resetDisplay() {
        var image = document.getElementById("displayImage");
        image.src = "/images/placeholder-sdk.png";
        updateStartScenarioButton();
    }
})();