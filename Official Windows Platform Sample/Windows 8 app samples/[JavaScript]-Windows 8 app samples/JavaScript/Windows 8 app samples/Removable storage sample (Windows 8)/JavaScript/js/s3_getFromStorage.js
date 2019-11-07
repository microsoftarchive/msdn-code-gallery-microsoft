//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s3_getFromStorage.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", scenario3_getFirstImageOnStorage, false);
        }
    });

    function scenario3_getFirstImageOnStorage() {
        // Find all storage devices using Windows.Devices.Enumeration
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            Windows.Devices.Portable.StorageDevice.getDeviceSelector(),
            null)
                .done(
                    function (storageDevices) {
                        // Get file from the selected storage
                        if (storageDevices.length) {
                            SdkSample.showItemSelector(storageDevices, getFirstImageOnStorage);
                        } else {
                            WinJS.log && WinJS.log("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", "sample", "status");
                        }
                    },
                    function (e) {
                        WinJS.log && WinJS.log("Failed to find all storage devices, error: " + e.message, "sample", "error");
                    });
    }

    function getFirstImageOnStorage(deviceInfoElement) {
        // Convert the selected device information element to a StorageFolder
        var storage = Windows.Devices.Portable.StorageDevice.fromId(deviceInfoElement.id);
        var storageName = deviceInfoElement.name;

        // Construct the query for image files
        var queryOptions = new Windows.Storage.Search.QueryOptions(
            Windows.Storage.Search.CommonFileQuery.orderByName, [".jpg", ".png", ".gif"]);
        var imageFileQuery = storage.createFileQueryWithOptions(queryOptions);

        // Run the query for image files
        WinJS.log && WinJS.log("Looking for images on " + storageName + " ...", "sample", "status");
        imageFileQuery.getFilesAsync()
            .done(
                function (imageFiles) {
                    if (imageFiles.length > 0) {
                        var imageFile = imageFiles[0];
                        var imageName = imageFile.name;
                        WinJS.log && WinJS.log("Found " + imageName + " on " + storageName, "sample", "status");
                        imageFile.getBasicPropertiesAsync()
                            .done(
                                function (imageProperties) {
                                    if (imageProperties.size > 0) {
                                        WinJS.log && WinJS.log("Displaying: " + imageName + ", date modified: " + imageProperties.dateModified + ", size: " + imageProperties.size + " bytes", "sample", "status");
                                        document.getElementById("scenarioImageHolder").src = window.URL.createObjectURL(imageFile, { oneTimeOnly: true });
                                    } else {
                                        WinJS.log && WinJS.log("Cannot display " + imageName + " because its size is 0", "sample", "error");
                                    }
                                },
                                function (e) {
                                    WinJS.log && WinJS.log("Error getting basic properties for " + imageFile.name + ", error: " + e.message, "sample", "error");
                                });
                    } else {
                        WinJS.log && WinJS.log("No images were found on " + storageName + ". You can use scenario 2 to transfer an image to it", "sample", "status");
                    }
                },
                function (e) {
                    WinJS.log && WinJS.log("Error when looking for images on " + storageName + ", error: " + e.message, "sample", "error");
                });

    }

})();
