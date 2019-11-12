//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/uploadFile.html", {
        ready: function (element, options) {
            // Assign event listeners for each button on click.
            id("StartUpload").addEventListener("click", uploadFile, false);
            id("StartUploads").addEventListener("click", uploadFiles, false);
            id("CancelAll").addEventListener("click", cancelAll, false);

            // On load check if there are uploads in progress from a previous activation.
            printLog("Loading uploads ... ");
            // Enumerate outstanding uploads.
            Windows.Networking.BackgroundTransfer.BackgroundUploader.getCurrentUploadsAsync().done(function (uploads) {
                printLog("done.<br/>");
                // If uploads from previous application state exist, reassign callbacks and persist to global array.
                for (var i = 0; i < uploads.size; i++) {
                    var upload = new UploadOperation();
                    upload.load(uploads[i]);
                    uploadOperations.push(upload);
                }
            });
        }
    });

    // Global array used to persist operations.
    var uploadOperations = [];

    // Class associated with each upload.
    function UploadOperation() {
        var upload = null;
        var promise = null;

        this.start = function (uri, file) {
            printLog("Using URI: " + uri.absoluteUri + "<br/>");

            var uploader = new Windows.Networking.BackgroundTransfer.BackgroundUploader();

            // Set a header, so the server can save the file (this is specific to the sample server).
            uploader.setRequestHeader("Filename", file.name);

            // Create a new upload operation.
            upload = uploader.createUpload(uri, file);

            // Start the upload and persist the promise to be able to cancel the upload.
            promise = upload.startAsync().then(complete, error, progress);
        };

        this.startMultipart = function (uri, files) {
            printLog("Using URI: " + uri.absoluteUri + "<br/>");

            var uploader = new Windows.Networking.BackgroundTransfer.BackgroundUploader();
            var contentParts = [];
            files.forEach(function (file, index) {
                var part = new Windows.Networking.BackgroundTransfer.BackgroundTransferContentPart("File" + index, file.name);
                part.setFile(file);
                contentParts.push(part);
            });

            // Create a new upload operation.
            uploader.createUploadAsync(uri, contentParts).then(function (uploadOperation) {
                // Start the upload and persist the promise to be able to cancel the upload.
                upload = uploadOperation;
                promise = uploadOperation.startAsync().then(complete, error, progress);
            });
        };

        // On application activation, reassign callbacks for a upload
        // operation persisted from previous application state.
        this.load = function (loadedUpload) {
            upload = loadedUpload;
            printLog("Found upload: " + upload.guid + " from previous application run.<br\>");
            promise = upload.attachAsync().then(complete, error, progress);
        };

        // Cancel upload.
        this.cancel = function () {
            if (promise) {
                promise.cancel();
                promise = null;
                printLog("Canceling upload: " + upload.guid + "<br\>");
            }
            else {
                printLog("Upload " + upload.guid + " already canceled.<br\>");
            }
        };

        // Returns true if this is the upload identified by the guid.
        this.hasGuid = function (guid) {
            return upload.guid === guid;
        };

        // Removes upload operation from global array.
        function removeUpload(guid) {
            uploadOperations.forEach(function (operation, index) {
                if (operation.hasGuid(guid)) {
                    uploadOperations.splice(index, 1);
                }
            });
        }

        // Progress callback.
        function progress() {
            // Output all attributes of the progress parameter.
            printLog(upload.guid + " - Progress: ");
            var currentProgress = upload.progress;
            for (var att in currentProgress) {
                printLog(att + ": " + currentProgress[att] + ", ");
            }
            printLog("<br/>");

            // Handle various pause status conditions. This will never happen when using POST verb (the default)
            // but may when using PUT. Application can change verb used by using method property of BackgroundUploader class.
            if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedCostedNetwork) {
                printLog("Upload " + upload.guid + " paused because of costed network <br\>");
            } else if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedNoNetwork) {
                printLog("Upload " + upload.guid + " paused because network is unavailable.<br\>");
            }
        }

        // Completion callback.
        function complete() {
            removeUpload(upload.guid);

            printLog(upload.guid + " - upload complete. Status code: " + upload.getResponseInformation().statusCode + "<br/>");
            displayStatus(upload.guid + " - upload complete.");
        }

        // Error callback.
        function error(err) {
            if (upload) {
                removeUpload(upload.guid);
                printLog(upload.guid + " - upload completed with error.<br/>");
            }
            displayException(err);
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

    function uploadFile() {
        // Verify that we are currently not snapped, or that we can unsnap to open the picker.
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        if (currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped && !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
            displayError("Error: File picker cannot be opened in snapped mode. Please unsnap first.");
        }

        var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
        filePicker.fileTypeFilter.replaceAll(["*"]);
        filePicker.pickSingleFileAsync().then(function (file) {
            if (!file) {
                displayError("Error: No file selected.");
                return;
            }

            // By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
            // box validating the URI is required since it was received from an untrusted source (user input).
            // The URI is validated by catching exceptions thrown by the Uri constructor.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            var uri = null;
            try {
                uri = new Windows.Foundation.Uri(document.getElementById("serverAddressField").value);
            } catch (error) {
                displayError("Error: Invalid URI. " + error.message);
                return;
            }

            var upload = new UploadOperation();
            upload.start(uri, file);

            // Persist the upload operation in the global array.
            uploadOperations.push(upload);
        });
    }

    function uploadFiles() {
        // Verify that we are currently not snapped, or that we can unsnap to open the picker.
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        if (currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped && !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
            displayError("Error: File picker cannot be opened in snapped mode. Please unsnap first.");
        }

        var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
        filePicker.fileTypeFilter.replaceAll(["*"]);
        filePicker.pickMultipleFilesAsync().then(function (files) {
            if (files === 0) {
                displayError("Error: No file selected.");
                return;
            }

            // By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
            // box validating the URI is required since it was received from an untrusted source (user input).
            // The URI is validated by catching exceptions thrown by the Uri constructor.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            var uri = null;
            try {
                uri = new Windows.Foundation.Uri(document.getElementById("serverAddressField").value);
            } catch (error) {
                displayError("Error: Invalid URI. " + error.message);
                return;
            }

            var upload = new UploadOperation();
            upload.startMultipart(uri, files);

            // Persist the upload operation in the global array.
            uploadOperations.push(upload);
        });
    }

    // Cancel all uploads.
    function cancelAll() {
        for (var i = 0; i < uploadOperations.length; i++) {
            uploadOperations[i].cancel();
        }
    }

})();
