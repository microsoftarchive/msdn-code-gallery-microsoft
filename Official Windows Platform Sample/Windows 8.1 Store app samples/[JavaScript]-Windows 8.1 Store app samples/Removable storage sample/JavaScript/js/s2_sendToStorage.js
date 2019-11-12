//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s2_sendToStorage.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", scenario2_sendImageFileToStorage, false);
        }
    });

    function scenario2_sendImageFileToStorage() {
        // Find all storage devices using Windows.Devices.Enumeration
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            Windows.Devices.Portable.StorageDevice.getDeviceSelector(),
            null)
                .done(
                    function (storageDevices) {
                        // Send file to the selected storage
                        if (storageDevices.length) {
                            SdkSample.showItemSelector(storageDevices, sendImageFileToStorage);
                        } else {
                            WinJS.log && WinJS.log("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", "sample", "status");
                        }
                    },
                    function (e) {
                        WinJS.log && WinJS.log("Failed to find all storage devices, error: " + e.message, "sample", "error");
                    });
    }

    function sendImageFileToStorage(deviceInfoElement) {
        // Launch the picker to select an image file
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.replaceAll([".jpg", ".png", ".gif"]);
        picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
        picker.pickSingleFileAsync()
            .done(
                function (sourceFile) {
                    if (sourceFile) {
                        // Convert the selected device information element to a StorageFolder
                        var storage = Windows.Devices.Portable.StorageDevice.fromId(deviceInfoElement.id);
                        var storageName = deviceInfoElement.name;

                        WinJS.log && WinJS.log("Copying image: " + sourceFile.name + " to " + storageName + " ...", "sample", "status");
                        copyFileToFolderOnStorage(sourceFile, storage, storageName);

                    } else {
                        WinJS.log && WinJS.log("No file was selected", "sample", "status");
                    }
                },
                function (e) {
                    WinJS.log && WinJS.log("Failed to select a file: " + e.message, "sample", "error");
                });
    }

    function copyFileToFolderOnStorage(sourceFile, storage, storageName) {
        // Construct a folder search to find sub-folders under the current storage.
        // The default (shallow) query should be sufficient in finding the first level of sub-folders.
        // If the first level of sub-folders are not writable, a deep query + recursive copy may be needed.
        storage.getFoldersAsync()
            .done(
                function (folders) {
                    if (folders.length > 0) {
                        var destinationFolder = folders[0];
                        var destinationFolderName = destinationFolder.name;

                        WinJS.log && WinJS.log("Trying the first sub-folder: " + destinationFolderName + "...", "sample", "status");
                        sourceFile.copyAsync(destinationFolder, sourceFile.name, Windows.Storage.NameCollisionOption.generateUniqueName)
                            .done(
                                function (newFile) {
                                    WinJS.log && WinJS.log("Image " + newFile.name + " created in folder: " + destinationFolderName + " on " + storageName, "sample", "status");
                                },
                                function (e) {
                                    WinJS.log && WinJS.log("Failed to copy image to the first sub-folder: " + destinationFolderName + ", " + storageName + " may not allow sending files to its top level folders", "sample", "error");
                                });
                    } else {
                        WinJS.log && WinJS.log("No sub-folders found on " + storageName + " to copy to", "sample", "status");
                    }
                },
                function (e) {
                    WinJS.log && WinJS.log("Failed to find any sub-folders on " + storageName + " to copy to: " + e.message, "sample", "error");
                });
    }

})();
