//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s4_autoplay.html", {

        processed: function (element, arg) {
            // During an initial activation, this event is called before the system splash screen is torn down.
            // The arg parameter will contain either the Autoplay drive or device id if either is passed to navigation
            // in the activated event handler. Otherwise, arg will be null.
            if (arg) {
                autoplay_getFirstImageOnStorage(arg);
            } else {
                WinJS.log && WinJS.log("This scenario will only run if launched from Autoplay.", "sample", "error");
            }
        },

        ready: function (element, options) {
            // During an initial activation, this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.
        }
    });

    function autoplay_getFirstImageOnStorage(arg) {
        // Autoplay will provide either a drive or a device id during activation
        var storage = (typeof arg === "string" ?
                      Windows.Devices.Portable.StorageDevice.fromId(arg) : arg);
        if (storage) {
            var storageName = storage.name;

            // Construct the query for image files
            var queryOptions = new Windows.Storage.Search.QueryOptions(
                Windows.Storage.Search.CommonFileQuery.orderByName, [".jpg", ".png", ".gif"]);
            var imageFileQuery = storage.createFileQueryWithOptions(queryOptions);

            // Run the query for image files
            WinJS.log && WinJS.log("[Launched by Autoplay] Looking for images on " + storageName + " ...", "sample", "status");
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
                                            WinJS.log && WinJS.log("[Launched by Autoplay] Displaying: " + imageName + ", date modified: " + imageProperties.dateModified + ", size: " + imageProperties.size + " bytes", "sample", "status");
                                            document.getElementById("scenarioImageHolder").src = window.URL.createObjectURL(imageFile, { oneTimeOnly: true });
                                        } else {
                                            WinJS.log && WinJS.log("[Launched by Autoplay] Cannot display " + imageName + " because its size is 0", "sample", "error");
                                        }
                                    },
                                    function (e) {
                                        WinJS.log && WinJS.log("[Launched by Autoplay] Error getting basic properties for " + imageFile.name + ", error: " + e.message, "sample", "error");
                                    });
                        } else {
                            WinJS.log && WinJS.log("[Launched by Autoplay] No images were found on " + storageName + ". You can use scenario 2 to transfer an image to it", "sample", "status");
                        }
                    },
                    function (e) {
                        WinJS.log && WinJS.log("[Launched by Autoplay] Error when looking for images on " + storageName + ", error: " + e.message, "sample", "error");
                    });
        } else {
            WinJS.log && WinJS.log("[Launched by Autoplay] Failed to acquire a storage from Autoplay!", "sample", "error");
        }
    }

})();
