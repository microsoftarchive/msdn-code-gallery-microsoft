//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var scanningPromise;
    var asyncOperation;
    var fileNameList = new WinJS.Binding.List();
    var scenarioRunning = false;

    var page = WinJS.UI.Pages.define("/html/scenario7MultipleResultsWithProgress.html", {
        ready: function (element, options) {
            // Data bind file names to list view
            var listView = Utils.id("fileNameList");
            listView.winControl.itemDataSource = fileNameList.dataSource;
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
    /// Even Handler for click on Start Scenario button. Starts the scenario for scanning from the Feeder and getting multiple results with progress
    /// </summary>
    function startScenario() {
        WinJS.log && WinJS.log("Starting scenario of scanning from Feeder and getting multiple results with progress.", "sample", "status");
        resetDisplay();
        scenarioRunning = true;
        updateStartAndEndScenarioButtons();
        scanToFolder(ScannerContext.currentScannerId, Windows.Storage.KnownFolders.picturesLibrary);
    }

    /// <summary>
    /// Scans all the images from Feeder source of the scanner
    /// The scanning is allowed only if the selected scanner is equipped with a feeder
    /// </summary>
    /// <param name="deviceId">scanner device id</param>
    /// <param name="folder">the folder that receives the scanned files</param>
    function scanToFolder(deviceId, folder) {
        // Get the scanner object for this device id
        Windows.Devices.Scanners.ImageScanner.fromIdAsync(deviceId).then(function (myScanner) {
            // Check to see if the user has already canceled the scenario
            if (scenarioRunning) {
                if (myScanner.isScanSourceSupported(Windows.Devices.Scanners.ImageScannerScanSource.feeder)) {                    
                    // Set MaxNumberOfPages to zero to scan all the pages that are present in the feeder
                    myScanner.feederConfiguration.maxNumberOfPages = 0;

                    WinJS.log && WinJS.log("Scanning.", "sample", "status");
                    // Scan API call to start scanning from the Feeder source of the scanner.
                    scanningPromise = myScanner.scanFilesToFolderAsync(Windows.Devices.Scanners.ImageScannerScanSource.feeder, folder);

                    return scanningPromise;
                } else {
                    WinJS.log && WinJS.log("The selected scanner does not report to be equipped with a Feeder.", "sample", "error");
                    return WinJS.Promise.then(null);
                }
            } else {
                // Scenario has already been canceled; return nullptr so no further action is possible 
                return WinJS.Promise.then(null);
            }
        }).done(function (result) {
            scenarioRunning = false;
            updateStartAndEndScenarioButtons();
            // Check for result to prevent showing results in case of cancellation and scanner not equipped with feeder source.
            if (result) {

                // Number of scanned files should be zero here since we already processed during scan progress notifications all the files that have been scanned
                WinJS.log && WinJS.log("Scanning is complete.", "sample", "status");
            }
        }, function (error) {
            if (error.name === "Canceled") {
                Utils.displayScanCancelationMessage();
            } else {
                resetDisplay();
                Utils.displayErrorMessage(error);
            }
        },
        function (progress) {
            var noOfFilesScanned = progress;
            var result = null;
            try {
                // Displays the image of the scanned file
                result = scanningPromise.operation.getResults();
            } catch (exception) {
                // The try catch is placed here for scenarios in which operation has already been cancelled when progress call is made
                Utils.displayScanCancelationMessage();
            }
            if (result && result.scannedFiles.size > 0) {
                displayProgressResults(result.scannedFiles, noOfFilesScanned);
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
    /// Displays the scanned image and list files that are scanned for far
    /// </summary>
    function displayProgressResults(scannedFiles, noOfFilesScanned) {
        var image = document.getElementById("displayImage");
        var file = scannedFiles.getAt(0);

        image.src = window.URL.createObjectURL(file, { oneTimeOnly: true });
        image.alt = file.name;

        WinJS.log && WinJS.log("Scanning is in progress. The number of files scanned so far: " + noOfFilesScanned + ". Below is the latest scanned image. \n" +
                        "All the files that have been scanned are saved to local My Pictures folder.", "sample", "status");
        
        Utils.updateFileListData(scannedFiles, fileNameList);
        Utils.id("fileListDiv").style.visibility = "visible";
    }

    /// <summary>
    /// Displays first of the scanned image and list files that are scanned
    /// </summary>
    function displayResults(scannedFiles) {
        var image = document.getElementById("displayImage");
        Utils.displayImageAndScanCompleteMessage(scannedFiles, image);
        Utils.updateFileListData(scannedFiles, fileNameList);
        Utils.id("fileListDiv").style.visibility = "visible";
    }

    /// <summary>
    /// Set image and other display items to default values
    /// </summary>
    function resetDisplay() {
        scenarioRunning = false;
        var image = document.getElementById("displayImage");
        image.src = "/images/placeholder-sdk.png";
        Utils.id("fileListDiv").style.visibility = "hidden";
        fileNameList.splice(0, fileNameList.length);
        updateStartAndEndScenarioButtons();
    }
})();