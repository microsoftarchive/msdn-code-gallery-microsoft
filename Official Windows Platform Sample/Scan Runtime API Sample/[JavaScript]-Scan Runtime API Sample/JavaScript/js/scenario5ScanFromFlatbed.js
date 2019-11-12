//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var scanningPromise;
    var scenarioRunning = false;

    var page = WinJS.UI.Pages.define("/html/scenario5ScanFromFlatbed.html", {
        ready: function (element, options) {
            // Start scanner watcher if it was not previously started.
            if (!ScannerContext.watcherStarted) {
                ScannerContext.startScannerWatcher();
            }
            // Registering event handlers for start and cancel scenario buttons.
            Utils.id("startSceanrio").addEventListener("click", startScenario, false);
            Utils.id("endSceanrio").addEventListener("click", endScenario, false);
            // Registering event handler for current scanner id change event.
            ScannerContext.events.addEventListener("currentScannerId", updateStartAndEndScenarioButtons, false);

            resetDisplay();
        },
        unload: function () {
            // Unregister event handlers
            ScannerContext.events.removeEventListener("currentScannerId", updateStartAndEndScenarioButtons, false);
            Utils.id("scannerList").winControl.unLoad();
            //If a scan job is in progress, cancel it now
            endScenario();
        }
    });

    /// <summary>
    /// Event Handler for click on start scenario button. Starts the scenario for scanning from the Faltbed
    /// </summary>
    function startScenario() {
        WinJS.log && WinJS.log("Starting scenario of scanning from Flatbed.", "sample", "status");
        resetDisplay();
        scenarioRunning = true;
        updateStartAndEndScenarioButtons();
        scanToFolder(ScannerContext.currentScannerId, Windows.Storage.KnownFolders.picturesLibrary);
    }

    /// <summary>
    /// Scans image from the Flatbed source of the scanner
    /// The scanning is allowed only if the selected scanner is equipped with a Flatbed source
    /// </summary>
    /// <param name="deviceId">scanner device id</param>
    /// <param name="folder">the folder that receives the scanned files</param>
    function scanToFolder(deviceId, folder) {
        // Get the scanner object for this device id
        Windows.Devices.Scanners.ImageScanner.fromIdAsync(deviceId).then(function (myScanner) {
            // Check to see if the user has already canceled the scenario
            if (scenarioRunning) {
                if (myScanner.isScanSourceSupported(Windows.Devices.Scanners.ImageScannerScanSource.flatbed)) {
                    // Set the scan file format to Device Independent Bitmap (DIB)
                    myScanner.flatbedConfiguration.format = Windows.Devices.Scanners.ImageScannerFormat.deviceIndependentBitmap;                    
                    WinJS.log && WinJS.log("Scanning.", "sample", "status");

                    // Scan API call to start scanning from the Flatbed source of the scanner.
                    return scanningPromise = myScanner.scanFilesToFolderAsync(Windows.Devices.Scanners.ImageScannerScanSource.flatbed, folder);
                } else {
                    WinJS.log && WinJS.log("The selected scanner does not report to be equipped with a Flatbed.", "sample", "error");
                    return WinJS.Promise.then(null);
                }
            } else {
                // Scenario has already been canceled; return nullptr so no further action is possible 
                return WinJS.Promise.then(null);
            }
        }).done(function (result) {
            scenarioRunning = false;
            updateStartAndEndScenarioButtons();
            // Check for result to prevent showing results in case of cancellation and scanner not equipped with flatbed source.
            if (result) {
                if (result.scannedFiles.size > 0) {
                    displayResults(result.scannedFiles);
                } else {
                    WinJS.log && WinJS.log("There are no files scanned from the Flatbed." + scannedFileName, "sample", "error");
                }
            }
        }, function (error) {
            if (error.name === "Canceled") {
                Utils.displayScanCancelationMessage();
            } else {
                resetDisplay();
                Utils.displayErrorMessage(error);
            }
        });
    }

    /// <summary>
    /// Event handler for click on  Cancel Scenario button.
    /// </summary>
    function endScenario() {
        if (scenarioRunning) {
            scanningPromise.cancel();
            resetDisplay();
        }
    }

    /// <summary>
    /// Enables or disables start and end scenario buttons depending on the state of the scenario running and scanner available
    /// </summary>
    function updateStartAndEndScenarioButtons() {
        var startButton = Utils.id("startSceanrio");
        var cancelButton = Utils.id("endSceanrio");
        Utils.updateStartAndEndScenarioButtons(startButton, cancelButton, ScannerContext.currentScannerId, scenarioRunning);
    }

    /// <summary>
    /// Displays the scanned image
    /// </summary>
    function displayResults(scannedFiles) {
        var image = document.getElementById("displayImage");
        Utils.displayImageAndScanCompleteMessage(scannedFiles, image);
    }

    /// <summary>
    /// Set image and other display items to default values
    /// </summary>
    function resetDisplay() {
        var image = document.getElementById("displayImage");
        image.src = "/images/placeholder-sdk.png";
        scenarioRunning = false;
        updateStartAndEndScenarioButtons();
    }
})();