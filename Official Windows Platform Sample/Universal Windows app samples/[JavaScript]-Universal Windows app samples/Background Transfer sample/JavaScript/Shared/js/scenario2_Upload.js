//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_Upload.html", {
        ready: function (element, options) {
            // Assign event listeners for each button on click.
            id("startUploadButton").addEventListener("click", uploadFile, false);
            id("startMultipartUploadButton").addEventListener("click", uploadFiles, false);
            id("cancelAllButton").addEventListener("click", cancelAll, false);

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

            if (Windows.ApplicationModel.Activation.ActivationKind.pickFileContinuation !== undefined &&
                options !== undefined &&
                options.activationKind === Windows.ApplicationModel.Activation.ActivationKind.pickFileContinuation) {
                continueFileOpenPicker(options.activatedEventArgs);
            }
        }
    });

    function continueFileOpenPicker(args) {
        if (args.length > 0) {
            var uri = new Windows.Foundation.Uri(args[0].continuationData["uri"]);
            if (args[0].files.length === 1) {
                uploadSingleFileAsync(uri, args[0].files[0]).done(null, displayException);
            } else if (args[0].files.length > 1) {
                uploadMultipleFilesAsync(uri, args[0].files).done(null, displayException);
            }
        }
    }

    // Global array used to persist operations.
    var uploadOperations = [];

    var maxUploadFileSize = 100 * 1024 * 1024; // 100 MB (arbitrary limit chosen for this sample)

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
            } else {
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
            displayStatus("Completed: " + upload.guid + ", Status Code: " + upload.getResponseInformation().statusCode);
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

    function uploadFile() {
        // Validating the URI is required since it was received from an untrusted source (user input).
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

        var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
        filePicker.fileTypeFilter.replaceAll(["*"]);

        if (filePicker.pickSingleFileAndContinue !== undefined) {
            filePicker.continuationData["uri"] = uri.absoluteCanonicalUri;
            filePicker.pickSingleFileAndContinue();
        } else {
            filePicker.pickSingleFileAsync().then(function (file) {
                uploadSingleFileAsync(uri, file);
            }).done(null, displayException);
        }
    }

    function uploadSingleFileAsync(uri, file)
    {
        if (!file) {
            displayError("Error: No file selected.");
            return;
        }

        return file.getBasicPropertiesAsync().then(function (properties) {
            if (properties.size > maxUploadFileSize) {
                displayError("Selected file exceeds max. upload file size (" + (maxUploadFileSize / (1024 * 1024)) +
                    " MB).");
                return;
            }

            var upload = new UploadOperation();
            upload.start(uri, file);

            // Persist the upload operation in the global array.
            uploadOperations.push(upload);
        });
    }

    function uploadFiles() {
        // Validating the URI is required since it was received from an untrusted source (user input).
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

        var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
        filePicker.fileTypeFilter.replaceAll(["*"]);

        if (filePicker.pickMultipleFilesAndContinue !== undefined) {
            filePicker.continuationData["uri"] = uri.absoluteCanonicalUri;
            filePicker.pickMultipleFilesAndContinue();
        } else {
            filePicker.pickMultipleFilesAsync().then(function (files) {
                uploadMultipleFilesAsync(uri, files);
            }).done(null, displayException);
        }
    }

    function uploadMultipleFilesAsync(uri, files) {
        var promise = validateFilesAsync(files);

        if (!promise) {
            return;
        }

        return promise.then(function (validatedFiles) {
            if (!validatedFiles) {
                return;
            }

            var upload = new UploadOperation();
            upload.startMultipart(uri, validatedFiles);

            // Persist the upload operation in the global array.
            uploadOperations.push(upload);
        });
    }

    function validateFilesAsync(files) {
        if (files.size === 0) {
            displayError("Error: No file selected.");
            return;
        }

        var getPropertiesPromises = [];
        var totalFileSize = 0;

        // Get file size of all files. If the sum exceeds the maximum upload size, return null to indicate
        // invalid files.
        files.forEach(function (file, index) {
            getPropertiesPromises.push(file.getBasicPropertiesAsync().then(function (properties) {
                totalFileSize += properties.size;
            }));
        });

        return WinJS.Promise.join(getPropertiesPromises).then(function () {
            if (totalFileSize > maxUploadFileSize) {
                displayError("Size of selected files exceeds max. upload file size (" +
                    (maxUploadFileSize / (1024 * 1024)) + " MB).");
                return null;
            }

            return files;
        });
    }

    // Cancel all uploads.
    function cancelAll() {
        for (var i = 0; i < uploadOperations.length; i++) {
            uploadOperations[i].cancel();
        }
    }

})();
