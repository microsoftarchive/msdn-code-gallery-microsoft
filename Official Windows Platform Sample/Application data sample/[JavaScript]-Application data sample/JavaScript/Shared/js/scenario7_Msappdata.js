//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario7_Msappdata.html", {
        ready: function (element, options) {
            copyImageToAppDataFolder("appDataLocal.png", Windows.Storage.ApplicationData.current.localFolder).done(function() {
                appendImage("Local");
            }, imagesAsyncError);

            copyImageToAppDataFolder("appDataTemp.png", Windows.Storage.ApplicationData.current.temporaryFolder).done(function () {
                appendImage("Temp");
            }, imagesAsyncError);

            copyImageToAppDataFolder("appDataRoaming.png", Windows.Storage.ApplicationData.current.roamingFolder).done(function () {
                appendImage("Roaming");
            }, imagesAsyncError);
        }
    });

    function imagesAsyncError(error) {
        sdkSample.displayError(error);
    }

    function copyImageToAppDataFolder(imageName, folder) {
        // Copy the image from the package to the AppData folder to demonstrate the ability to reference content
        // using the ms-appdata:// protocol
        return Windows.Storage.StorageFile.getFileFromApplicationUriAsync(Windows.Foundation.Uri("ms-appx:///images/" + imageName))
            .then(function (file) {
                return file.copyAsync(folder, imageName, Windows.Storage.NameCollisionOption.replaceExisting);
            });
    }

    function appendImage(folder) {
        var image = document.createElement("img");
        image.src = "ms-appdata:///" + folder + "/appData" + folder + ".png";
        image.alt = folder;
        document.getElementById("images" + folder).appendChild(image);
    }
})();
