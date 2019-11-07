// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=392286
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    var NotifyType = {};
    NotifyType.StatusMessage = 0;
    NotifyType.ErrorMessage = 1;


    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            
            args.setPromise(WinJS.UI.processAll().then(function ready() {

                window.addEventListener("resize", handleResize, false);
                getid("startDownloadButton").addEventListener("click", startDownload, false);
                getid("pauseAllButton").addEventListener("click", pauseAll, false);
                getid("resumeAllButton").addEventListener("click", resumeAll, false);
                getid("cancelAllButton").addEventListener("click", cancelAll, false);

                // Enumerate outstanding downloads.
                Windows.Networking.BackgroundTransfer.BackgroundDownloader.getCurrentDownloadsAsync().done(function (downloads) {
                    // If downloads from previous application state exist, reassign callbacks and persist to global array.
                    if (downloads) {
                        Log("Loading background downloads: " + downloads.size);
                        for (var i = 0; i < downloads.size; i++) {
                            var download = new DownloadOperation();
                            download.load(downloads[i]);
                            downloadOperations.push(download);
                        }
                    }
                });
            }));
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };


    function startDownload() {
        downloadZipFile(Windows.Networking.BackgroundTransfer.BackgroundTransferPriority.default, false);
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

    // Cancel all downloads.
    function cancelAll() {
        for (var i = 0; i < downloadOperations.length; i++) {
            downloadOperations[i].cancel();
        }
    }

    var uri = null;

    // download Zip File
    function downloadZipFile(priority, requestUnconstrainedTransfer) {
        // Instantiate downloads.
        var newDownload = new DownloadOperation();

        // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
        // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
        // the "Home or Work Networking" capability.
        try {
            uri = new Windows.Foundation.Uri(getid("zipAddressUrl").value);
        } catch (error) {
            displayError("Invalid URI.", NotifyType.ErrorMessage);
            return;
        }

        var openPicker = new Windows.Storage.Pickers.FolderPicker();
        openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.desktop;
        openPicker.fileTypeFilter.replaceAll([".zip"]);

        // Open the picker for the user to pick a file
        openPicker.pickSingleFolderAsync().then(function (destinationFolder) {
            if (destinationFolder) {
                // Application now has read/write access to all contents in the picked folder 
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.addOrReplace("PickedFolderToken", destinationFolder);
                Log("Picked folder: " + destinationFolder.name);
            } else {
                // The picker was dismissed with no selected file
                Log("Operation cancelled.");
                return;
            }

            var localFileName = getid("localfile").value.trim();

            newDownload.start(uri, destinationFolder, localFileName, priority, requestUnconstrainedTransfer);

            downloadOperations.push(newDownload);
        });
    }

    // Global array used to persist operations.
    var downloadOperations = [];

    // Class associated with each download.
    function DownloadOperation() {
        var download = null;
        var promise = null;

        this.start = function (uri, destFolder, fileName, priority, requestUnconstrainedDownload) {
            var ext = fileName.substr(fileName.lastIndexOf('.'));
            if (ext.localeCompare(".zip") != 0) {
                displayError("Invalid file type. Please make sure the file type is zip.", NotifyType.ErrorMessage);
                return;
            }

            // Asynchronously create the file in the pictures folder.
            destFolder.createFileAsync(fileName, Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (localFile) {
                var downloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();

                // Create a new download operation.
                download = downloader.createDownload(uri, localFile);

                Log("Downloading " + uri.absoluteUri.toString() + "to " + destFolder.name + "with " + priority + " priority, " + download.guid);
                download.priority = priority;

                // In this sample, we do not show how to request unconstrained download.
                // For more information about background transfer, please refer to the SDK Background transfer sample:
                // http://code.msdn.microsoft.com/windowsapps/Background-Transfer-Sample-d7833f61
                if (!requestUnconstrainedDownload) {
                    // Start the download and persist the promise to be able to cancel the download.
                    promise = download.startAsync().then(complete, error, progress);

                    promise.done(function () {
                        if (download.progress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.canceled) {
                            LogStatus("Canceled: " + download.Guid, NotifyType.StatusMessage);
                            return;
                        }

                        var zipFileName = fileName.substr(0, fileName.lastIndexOf('.'));
                        destFolder.createFolderAsync(zipFileName, Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (unzipFolder) {
                            // Call C# compent to unzip the specified zipfile to a folder
                            upZipFile(localFile, unzipFolder);
                        });
                    });

                    return;
                }
            }, error);
        };

        // On application activation, reassign callbacks for a download
        // operation persisted from previous application state.
        this.load = function (loadedDownload) {
            download = loadedDownload;
            promise = download.attachAsync().then(complete, error, progress);
        };

        // Cancel download.
        this.cancel = function () {
            if (promise) {
                promise.cancel();
                promise = null;
                Log("Canceling Downloads: " + downloadOperations.length);
            }
        };

        // Resume download - download will restart if server does not allow range-requests.
        this.resume = function () {
            Log("Downloads: " + downloadOperations.length);
            if (download) {
                if (download.progress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedByApplication) {
                    download.getResultStreamAt(download.progress.bytesReceived);
                    download.resume();
                    Log("Resumed: " + download.guid);
                }
                else {
                    Log("Skipped: " + download.guid +
                        " Status:" + download.progress.status);
                }
            }
        };

        // Pause download.
        this.pause = function () {
            Log("Downloads: " + downloadOperations.length);

            if (download) {
                if (download.progress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.running) {
                    download.pause();
                    Log("Paused: " + download.guid);
                }
                else {
                    Log("Skipped: " + download.guid +
                        " Status:" + download.progress.status);
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

            LogStatus("Running: " + download.guid, NotifyType.StatusMessage);

            // Output all attributes of the progress parameter.
            Log("Progress: " + download.guid, "Status: " + download.progress.status);

            var percent = 100;
            var currentProgress = download.progress;
            if (currentProgress.totalBytesToReceive > 0) {
                percent = currentProgress.bytesReceived * 100 / currentProgress.totalBytesToReceive;
            }

            Log("- Transfered bytes: " + currentProgress.bytesReceived + " of " + currentProgress.totalBytesToReceive + ", " + parseInt(percent) + "%");

            if (currentProgress.hasRestarted) {
                Log("- Download restarted");
            }

            if (currentProgress.hasResponseChanged) {

                // We've received new response headers from the server.
                Log("-Response updated; Header count: " + download.getResponseInformation().headers.size);

                // If you want to stream the response data this is a good time to start.
                // download.GetResultStreamAt(0);    
            }
        }

        // Completion callback.
        function complete() {
            removeDownload(download.guid);

            try {
                var responseInfo = download.getResponseInformation();
                LogStatus("Completed: " + download.guid + ", Status code: " + responseInfo.statusCode, NotifyType.StatusMessage);

            } catch (err) {
                displayException(err);
            }
        }

        // Error callback.
        function error(err) {
            if (download) {
                removeDownload(download.guid);
            }
            displayException(err);
        }
    }

    // handle window resize event
    function handleResize() {
        // Get window size
        var screenWidth = window.outerWidth;
        var screenHeight = window.outerHeight;

        if (screenWidth <= 500) {
            getid("title").style.display = "none";
            getid("link").style.display = "none";
            getid("outputField").style.width = "380px";
        }
        else if (screenWidth < screenHeight) {
            getid("rootGrid").style.msGridColumns = "20px 1fr 20px";
            getid("outputField").style.width = "450px";
        }
        else {
            getid("rootGrid").style.msGridColumns = "100px 1fr 100px";
            getid("title").style.removeAttribute("display");
            getid("link").style.removeAttribute("display");
            getid("outputField").style.width = "460px";
        }
    }

    function displayException(err) {
        var message;
        if (err.stack) {
            message = err.stack;
        }
        else {
            message = err.message;
        }

        var errorStatus = Windows.Networking.BackgroundTransfer.BackgroundTransferError.getStatus(err.number);
        if (errorStatus === Windows.Web.WebErrorStatus.cannotConnect) {
            message = "App cannot connect. Network may be down, connection was refused or the host is unreachable.";
        }

        displayError(message, NotifyType.ErrorMessage);
    }

    // Print error message
    function displayError(text, type) {
        switch (type) {
            case NotifyType.StatusMessage:
                getid("error").style.color = "green";
                break;
            case NotifyType.ErrorMessage:
                getid("error").style.color = "blue";
                break;
        }
        document.getElementById("error").innerText = text;
        if (getid("error").innerText != "") {
            getid("error").style.visibility = "visible";
        }
        else {
            getid("error").style.visibility = "collapse";
        }
    }

    function LogStatus(message, type) {
        displayError(message, type);
        Log(message);
    }

    function Log(message) {
        var textare = getid("outputField");
        textare.innerHTML += message + "\r\n";
        textare.style.height = textare.scrollHeight + 'px';
    }

    function getid(elementId) {
        return document.getElementById(elementId);
    }

    // Unzips the specified zipfile to a folder.
    function upZipFile(zipFile, unzipFolder) {
        LogStatus("Upziping file: " + zipFile.displayName + "...", NotifyType.StatusMessage);
        ZipHelperWinRT.ZipHelper.unZipFileAsync(zipFile, unzipFolder).then(function () {
            LogStatus("Unzip file '" + zipFile.displayName + "' successfully", NotifyType.StatusMessage);
        }, function (err) {
            displayError("Failed to unzip file ..." + err.message.substr(0, err.message.indexOf('.') + 1), NotifyType.ErrorMessage);
        });
    }

    app.start();
})();