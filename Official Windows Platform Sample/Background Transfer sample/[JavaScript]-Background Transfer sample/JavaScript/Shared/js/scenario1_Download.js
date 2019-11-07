//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_Download.html", {
        ready: function (element, options) {
            // Assign event listeners for each button on click.
            id("startDownloadButton").addEventListener("click", startDownload, false);
            id("startUnconstrainedDownloadButton").addEventListener("click", startUnconstrainedDownload, false);
            id("startHighPriorityDownloadButton").addEventListener("click", startHighPriorityDownload, false);
            id("cancelAllButton").addEventListener("click", cancelAll, false);
            id("pauseAllButton").addEventListener("click", pauseAll, false);
            id("resumeAllButton").addEventListener("click", resumeAll, false);

            // On load check if there are downloads in progress from a previous activation.
            printLog("Loading downloads ... ");

            // Enumerate outstanding downloads.
            Windows.Networking.BackgroundTransfer.BackgroundDownloader.getCurrentDownloadsAsync().done(function (downloads) {
                printLog("done.<br/>");
                // If downloads from previous application state exist, reassign callbacks and persist to global array.
                for (var i = 0; i < downloads.size; i++) {
                    var download = new DownloadOperation();
                    download.load(downloads[i]);
                    downloadOperations.push(download);
                }
            });
        }
    });

    // Global array used to persist operations.
    var downloadOperations = [];

    // Class associated with each download.
    function DownloadOperation() {
        var download = null;
        var promise = null;
        var imageStream = null;

        this.start = function (uri, fileName, priority, requestUnconstrainedDownload) {
            // Asynchronously create the file in the pictures folder.
            Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(fileName, Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (newFile) {
                var downloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();
                printLog("Using URI: " + uri.absoluteUri + "<br/>");

                // Create a new download operation.
                download = downloader.createDownload(uri, newFile);

                printLog("Created download " + download.guid + " with priority " +
                    (priority === Windows.Networking.BackgroundTransfer.BackgroundTransferPriority.high ? "high" : "default") +
                    "<br/>");
                download.priority = priority;

                if (!requestUnconstrainedDownload) {
                    // Start the download and persist the promise to be able to cancel the download.
                    promise = download.startAsync().then(complete, error, progress);
                    return;
                }

                // Create a list of download operations: We'll request that operations in this list will run
                // unconstrained.
                var requestOperations = [];
                requestOperations.push(download);

                // If the app isn't actively being used, at some point the system may slow down or pause long running
                // downloads. The purpose of this behavior is to increase the device's battery life.
                // By requesting unconstrained downloads, the app can request the system to not suspend any of the
                // downloads in the list for power saving reasons.
                // Use this API with caution since it not only may reduce battery life, but it may show a prompt to
                // the user.
                var requestPromise;
                try {
                    requestPromise = Windows.Networking.BackgroundTransfer.BackgroundDownloader.requestUnconstrainedDownloadsAsync(requestOperations);
                } catch (error) {
                    var notImplementedException = -2147467263;
                    if (error.number === notImplementedException) {
                        displayError("BackgroundDownloader.requestUnconstrainedDownloadsAsync is not supported in Windows Phone.");
                        return;
                    }
                    throw error;
                }

                requestPromise.done(function (result) {
                    printLog("Request for unconstrained downloads has been " +
                        (result.isUnconstrained ? "granted" : "denied") + "<br/>");

                    promise = download.startAsync().then(complete, error, progress);
                }, error);

            }, error);
        };

        // On application activation, reassign callbacks for a download
        // operation persisted from previous application state.
        this.load = function (loadedDownload) {
            download = loadedDownload;
            printLog("Found download: " + download.guid + " from previous application run.<br\>");
            promise = download.attachAsync().then(complete, error, progress);
        };

        // Cancel download.
        this.cancel = function () {
            if (promise) {
                promise.cancel();
                promise = null;
                printLog("Canceling download: " + download.guid + "<br\>");
                if (imageStream) {
                    imageStream.close();
                    imageStream = null;
                }
            } else {
                printLog("Download " + download.guid + " already canceled.<br\>");
            }
        };

        // Resume download - download will restart if server does not allow range-requests.
        this.resume = function () {
            if (download) {
                if (download.progress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedByApplication) {
                    download.resume();
                    printLog("Resuming download: " + download.guid + "<br\>");
                } else {
                    printLog("Download " + download.guid +
                        " is not paused, it may be running, completed, canceled or in error.<br\>");
                }
            }
        };

        // Pause download.
        this.pause = function () {
            if (download) {
                if (download.progress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.running) {
                    download.pause();
                    printLog("Pausing download: " + download.guid + "<br\>");
                } else {
                    printLog("Download " + download.guid +
                        " is not running, it may be paused, completed, canceled or in error.<br\>");
                }
            }
        };

        // Returns true if this is the download identified by the guid.
        this.hasGuid = function (guid) {
            return download.guid === guid;
        };

        // Removes download operation from global array.
        function removeDownload(guid) {
            downloadOperations.forEach(function (operation, index) {
                if (operation.hasGuid(guid)) {
                    downloadOperations.splice(index, 1);
                }
            });
        }

        // Progress callback.
        function progress() {
            // Output all attributes of the progress parameter.
            printLog(download.guid + " - progress: ");
            var currentProgress = download.progress;
            for (var att in currentProgress) {
                printLog(att + ": " + currentProgress[att] + ", ");
            }
            printLog("<br/>");

            // Handle various pause status conditions.
            if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedByApplication) {
                printLog("Download " + download.guid + " paused by application <br\>");
            } else if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedCostedNetwork) {
                printLog("Download " + download.guid + " paused because of costed network <br\>");
            } else if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedNoNetwork) {
                printLog("Download " + download.guid + " paused because network is unavailable.<br\>");
            } else {
                // We need a response before assigning the result stream to the image: If we get a response from
                // the server (hasResponseChanged == true) and if we haven't assigned the stream yet
                // (imageStream == null), then assign the stream to the image.
                // There is a second scenario where we need to assign the stream to the image: If a download gets
                // interrupted and cannot be resumed, the request is restarted. In that case we need to re-assign
                // the stream to the image since the requested image may have changed.
                if ((currentProgress.hasResponseChanged && !imageStream) || (currentProgress.hasRestarted)) {
                    try {
                        // Get Content-Type response header.
                        var contentType = download.getResponseInformation().headers.lookup("Content-Type");

                        // Check the stream is an image. For an example, change the URI string of the 'serverAddressField'
                        // to 'http://localhost/BackgroundTransferSample/data/windows-sdk.png' and start a download.
                        if (contentType.indexOf("image/") === 0) {
                            // Get the stream starting from byte 0.
                            imageStream = download.getResultStreamAt(0);

                            // Convert the stream to MS-Stream.
                            var msStream = MSApp.createStreamFromInputStream(contentType, imageStream);
                            var imageUrl = URL.createObjectURL(msStream);

                            // Pass the stream URL to the HTML image tag.
                            id("imageHolder").src = imageUrl;

                            // Close the stream once the image is displayed.
                            id("imageHolder").onload = function () {
                                if (imageStream) {
                                    imageStream.close();
                                    imageStream = null;
                                }
                            };
                        }
                    } catch (err) {
                        printLog("<b>Error in outputting file:</b> " + err + "<br\>");
                    }
                }
            }
        }

        // Completion callback.
        function complete() {
            removeDownload(download.guid);

            try {
                var responseInfo = download.getResponseInformation();
                printLog(download.guid + " - download complete. Status code: " + responseInfo.statusCode + "<br/>");
                displayStatus("Completed: " + download.guid + ", Status Code: " + responseInfo.statusCode);
            } catch (err) {
                displayException(err);
            }
        }

        // Error callback.
        function error(err) {
            if (download) {
                removeDownload(download.guid);
                printLog(download.guid + " - download completed with error.<br/>");
            }
            displayException(err);
        }
    }

    function displayException(err) {
        var message;
        if (err.stack) {
            message = err.stack;
        } else {
            message = err.message;
        }

        var errorStatus = Windows.Networking.BackgroundTransfer.BackgroundTransferError.getStatus(err.number);
        if (errorStatus === Windows.Web.WebErrorStatus.cannotConnect) {
            message = "App cannot connect. Network may be down, connection was refused or the host is unreachable.";
        }

        displayError(message);
    }

    function displayError(/*@type(String)*/message) {
        WinJS.log && WinJS.log(message, "sample", "error");
    }

    function displayStatus(/*@type(String)*/message) {
        WinJS.log && WinJS.log(message, "sample", "status");
    }

    // Print helper function.
    function printLog(/*@type(String)*/txt) {
        var console = document.getElementById("outputConsole");
        console.innerHTML += txt;
    }

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function startDownload() {
        downloadFile(Windows.Networking.BackgroundTransfer.BackgroundTransferPriority.default, false);
    }

    function startUnconstrainedDownload() {
        downloadFile(Windows.Networking.BackgroundTransfer.BackgroundTransferPriority.default, true);
    }

    function startHighPriorityDownload() {
        downloadFile(Windows.Networking.BackgroundTransfer.BackgroundTransferPriority.high, false);
    }

    function downloadFile(priority, requestUnconstrainedTransfer) {
        // Instantiate downloads.
        var newDownload = new DownloadOperation();

        // Pass the uri and the file name to be stored on disk to start the download.
        var fileName = document.getElementById("fileNameField").value;
        if (fileName === "") {
            displayError("A local file name is required.");
            return;
        }

        // Validating the URI is required since it was received from an untrusted source (user input).
        // The URI is validated by catching exceptions thrown by the Uri constructor.
        // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require the
        // "Home or Work Networking" capability.
        var uri = null;
        try {
            uri = new Windows.Foundation.Uri(document.getElementById("serverAddressField").value);
        } catch (error) {
            displayError("Error: Invalid URI. " + error.message);
            return;
        }

        newDownload.start(uri, fileName, priority, requestUnconstrainedTransfer);

        // Persist the download operation in the global array.
        downloadOperations.push(newDownload);
    }

    // Cancel all downloads.
    function cancelAll() {
        for (var i = 0; i < downloadOperations.length; i++) {
            downloadOperations[i].cancel();
        }
    }

    // Pause all downloads.
    function pauseAll() {
        for (var i = 0; i < downloadOperations.length; i++) {
            downloadOperations[i].pause();
        }
    }

    // Resume all downloads.
    function resumeAll() {
        for (var i = 0; i < downloadOperations.length; i++) {
            downloadOperations[i].resume();
        }
    }

})();
