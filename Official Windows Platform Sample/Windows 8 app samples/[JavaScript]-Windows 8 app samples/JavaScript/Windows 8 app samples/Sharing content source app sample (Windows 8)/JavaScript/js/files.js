//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var selectedFiles;

    var page = WinJS.UI.Pages.define("/html/files.html", {
        ready: function (element, options) {
            selectedFiles = null;
            var dataTransferManager = Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView();
            dataTransferManager.addEventListener("datarequested", dataRequested);
            document.getElementById("chooseFilesButton").addEventListener("click", chooseFiles, false);
            document.getElementById("shareButton").addEventListener("click", showShareUI, false);
        },
        unload: function () {
            var dataTransferManager = Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView();
            dataTransferManager.removeEventListener("datarequested", dataRequested);
        }
    });

    function chooseFiles() {
        // Verify that we are currently not snapped, or that we can unsnap to open the picker
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        if ((currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped) && !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
            // fail gracefully if we can't unsnap
            WinJS.log && WinJS.log("Cannot unsnap the sample application.", "sample", "status");
            return;
        }

        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.append("*");
        picker.viewMode = Windows.Storage.Pickers.PickerViewMode.list;
        picker.pickMultipleFilesAsync().done(function (files) {
            if (files.size > 0) {
                selectedFiles = files;
                document.getElementById("scenarioOutput").className = "unhidden";
                document.getElementById("selectedFiles").innerText = "";
                for (var i = 0, len = files.size; i < len; i++) {
                    document.getElementById("selectedFiles").innerText += files[i].name;
                    if (i !== files.size - 1) {
                        document.getElementById("selectedFiles").innerText += ", ";
                    }
                }
                WinJS.log && WinJS.log("", "sample", "error");
            } else {
                selectedFiles = null;
                document.getElementById("scenarioOutput").className = "hidden";
                document.getElementById("selectedFiles").innerText = "No files selected.";
                WinJS.log && WinJS.log("No file was selected.", "sample", "error");
            }
        });
    }

    function dataRequested(e) {
        var request = e.request;

        // Title is required
        var dataPackageTitle = document.getElementById("titleInputBox").value;
        if ((typeof dataPackageTitle === "string") && (dataPackageTitle !== "")) {
            if (selectedFiles) {
                request.data.properties.title = dataPackageTitle;

                // The description is optional.
                var dataPackageDescription = document.getElementById("descriptionInputBox").value;
                if ((typeof dataPackageDescription === "string") && (dataPackageDescription !== "")) {
                    request.data.properties.description = dataPackageDescription;
                }
                request.data.setStorageItems(selectedFiles);
            } else {
                request.failWithDisplayText("Select the files you would like to share and try again.");
            }
        } else {
            request.failWithDisplayText(SdkSample.missingTitleError);
        }
    }

    function showShareUI() {
        Windows.ApplicationModel.DataTransfer.DataTransferManager.showShareUI();
    }
})();
