//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var fileOpenPickerUI;
    var localFileId = "MyCachedFile"; //ID representing the file added from the application's package

    var page = WinJS.UI.Pages.define("/html/fileOpenPickerScenario3.html", {
        processed: function (element, options) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Do any initialization work that is required to set up the initial UI.
        },
        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.

            fileOpenPickerUI = options.fileOpenPickerUI;
            // To add the event listener to fileOpenPickerUI when the page is loaded
            fileOpenPickerUI.addEventListener("fileremoved", onFileRemovedFromBasket, false);

            document.getElementById("addFileButton").addEventListener("click", onAddFile, false);
            document.getElementById("removeFileButton").addEventListener("click", onRemoveFile, false);
        },
        unload: function () {
            // To remove the event listener from fileOpenPickerUI when the page is unloaded
            fileOpenPickerUI.removeEventListener("fileremoved", onFileRemovedFromBasket, false);
        }
    });

    function onAddFile() {
        // Respond to the "Add" button being clicked

        Windows.Storage.ApplicationData.current.localFolder.createFileAsync("CachedFile.txt", Windows.Storage.CreationCollisionOption.replaceExisting).then(function (file) {
            Windows.Storage.FileIO.writeTextAsync(file, "Cached file created...").then(function () {
                Windows.Storage.Provider.CachedFileUpdater.setUpdateInformation(file, "CachedFile", Windows.Storage.Provider.ReadActivationMode.beforeAccess, Windows.Storage.Provider.WriteActivationMode.notNeeded, Windows.Storage.Provider.CachedFileOptions.requireUpdateOnAccess);
                addFileToBasket(localFileId, file);
            }, onError);
        }, onError);
    };

    function onRemoveFile() {
        // Respond to the "Remove" button being clicked
        removeFileFromBasket(localFileId);
    };

    function onFileRemovedFromBasket(e) {
        // Add any code to be called when an item is removed from the basket by the user
        WinJS.log && WinJS.log(SdkSample.fileRemoved, "sample", "status");

        // Adjust the add/remove buttons based on removal
        updateSelectionState(e.id, false);
    };

    function addFileToBasket(fileId, file) {
        // Programmatically add the file to the basket

        var inBasket;
        switch (fileOpenPickerUI.addFile(fileId, file)) {
            case Windows.Storage.Pickers.Provider.AddFileResult.added:
                // notify user that the file was added to the basket
                WinJS.log && WinJS.log(SdkSample.fileAdded, "sample", "status");
                // Fallthrough is intentional here.
            case Windows.Storage.Pickers.Provider.AddFileResult.alreadyAdded:
                // Optionally notify the user that the file is already in the basket
                inBasket = true;
                break;
            case Windows.Storage.Pickers.Provider.AddFileResult.notAllowed:
                // Optionally notify the user that the file is not allowed in the basket
            case Windows.Storage.Pickers.Provider.AddFileResult.unavailable:
                // Optionally notify the user that the basket is not currently available
            default:
                inBasket = false;
                WinJS.log && WinJS.log(SdkSample.fileAddFailed, "sample", "status");
                break;
        }

        // Adjust the add/remove buttons based on addition
        updateSelectionState(fileId, inBasket);
    };

    function removeFileFromBasket(fileId) {
        // Programmatically remove the file from the picker basket if the file is in basket.
        if (fileOpenPickerUI.containsFile(fileId)) {
            fileOpenPickerUI.removeFile(fileId);
            WinJS.log && WinJS.log(SdkSample.fileRemoved, "sample", "status");
        }

        // Adjust the add/remove buttons based on removal
        updateSelectionState(fileId, false);
    };

    function updateSelectionState(fileId, selected) {
        // Update the add/remove buttons as selection changes

        if (fileId === localFileId) {
            document.getElementById("addFileButton").disabled = selected;
            document.getElementById("removeFileButton").disabled = !selected;
        }
    };

    function onError(error) {
        var errorBox = document.getElementById("error");
        errorBox.innerHTML = error;
    }
})();
