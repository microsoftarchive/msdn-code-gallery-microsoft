//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var fileOpenPickerUI;
    var uriFileId = "MyUriFile"; //ID representing the file added from a URI

    var page = WinJS.UI.Pages.define("/html/fileOpenPickerScenario2.html", {
        ready: function (element, options) {
            fileOpenPickerUI = options.fileOpenPickerUI;
            //to add the event listener to fileOpenPickerUI when the page is loaded
            fileOpenPickerUI.addEventListener("fileremoved", onFileRemovedFromBasket, false);

            document.getElementById("addUriFileButton").addEventListener("click", onAddUriFile, false);
            document.getElementById("removeUriFileButton").addEventListener("click", onRemoveUriFile, false);
        },
        unload: function () {
            //to remove the event listener from fileOpenPickerUI when the page is unloaded
            fileOpenPickerUI.removeEventListener("fileremoved", onFileRemovedFromBasket, false);
        }
    });

    function onAddUriFile() {
        // Respond to the "Add" button being clicked
        var imageSrcInput = document.getElementById("imageSrcInput");
        var uri = null;
        try {
            uri = new Windows.Foundation.Uri(imageSrcInput.value);
        } catch (error) {
            WinJS.log && WinJS.log("Please enter a valid URI.", "sample", "error");
        }

        if (uri !== null) {
            var thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.createFromUri(uri);

            // Retrieve a file from a URI to be added to the picker basket
            Windows.Storage.StorageFile.createStreamedFileFromUriAsync("URI.png", uri, thumbnail).then(function (fileToAdd) {
                addFileToBasket(uriFileId, fileToAdd);
            },
            function (error) {
                var errorBox = document.getElementById("error");
                errorBox.innerHTML = error;
            });
        }
    };

    function onRemoveUriFile() {
        // Respond to the "Remove" button being clicked
        removeFileFromBasket(uriFileId);
    }

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

        if (fileId === uriFileId) {
            document.getElementById("addUriFileButton").disabled = selected;
            document.getElementById("removeUriFileButton").disabled = !selected;
        }
    };
})();
