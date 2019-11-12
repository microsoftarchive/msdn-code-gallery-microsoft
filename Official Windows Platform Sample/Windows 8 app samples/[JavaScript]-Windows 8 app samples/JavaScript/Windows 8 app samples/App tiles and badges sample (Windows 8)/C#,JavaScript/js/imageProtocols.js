//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/imageProtocols.html", {
        ready: function (element, options) {
            document.getElementById("imageProtocolSelector").addEventListener("change", imageProtocolSelector, false);
            document.getElementById("openPicker").addEventListener("click", openPicker, false);
            document.getElementById("sendTileNotification").addEventListener("click", sendTileNotification, false);
            document.getElementById("imageProtocolSelector").selectedIndex = 0;
        }
    });

    function imageProtocolSelector() {
        var protocol = document.getElementById("imageProtocolSelector").selectedIndex;
        if (protocol === 0) {
            document.getElementById("appdataURLDiv").style.display = "none";
            document.getElementById("httpURLDiv").style.display = "none";
        } else if (protocol === 1) {
            document.getElementById("appdataURLDiv").style.display = "block";
            document.getElementById("httpURLDiv").style.display = "none";
        } else if (protocol === 2) {
            document.getElementById("appdataURLDiv").style.display = "none";
            document.getElementById("httpURLDiv").style.display = "block";
        }
    }

    var imageRelativePath;
    function openPicker() {
        if (SdkSample.ensureUnsnapped()) {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.fileTypeFilter.replaceAll([".jpg", ".jpeg", ".png", ".gif"]);
            picker.commitButtonText = "Copy";
            picker.pickSingleFileAsync().then(function (file) {
                return file.copyAsync(Windows.Storage.ApplicationData.current.localFolder, file.name, Windows.Storage.NameCollisionOption.generateUniqueName);
            }).done(function (newFile) {
                var imageAbsolutePath = newFile.path;
                //change image to relative path
                imageRelativePath = imageAbsolutePath.substring(imageAbsolutePath.lastIndexOf("\\") + 1);
                WinJS.log && WinJS.log("Image copied to application data local storage: " + newFile.path, "sample", "status");
            }, function (e) {
                WinJS.log && WinJS.log(e, "sample", "error");
            });
        } else {
            WinJS.log && WinJS.log("Cannot unsnap the sample application.", "sample", "status");
        }
    }

    function sendTileNotification() {
        var protocol = document.getElementById("imageProtocolSelector").selectedIndex;
        var tileContent;

        if (protocol === 0) { //using the ms-appx: protocol

            tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideImageAndText01();
            tileContent.textCaptionWrap.text = "The image is in the appx package";
            tileContent.image.src = "ms-appx:///images/redWide.png";

        } else if (protocol === 1) { //using the ms-appdata:/// protocol

            tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideImage();
            tileContent.image.src = "ms-appdata:///local/" + imageRelativePath; // make sure you are providing a relative path!

        } else if (protocol === 2) { //using http:// protocol
            // Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
            tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWidePeekImageCollection04();
            tileContent.textBodyWrap.text = "The baseUri is " + document.getElementById("baseUri").value;

            tileContent.imageMain.src = document.getElementById("image" + 0).value;
            tileContent.imageSmallColumn1Row1.src = document.getElementById("image" + 1).value;
            tileContent.imageSmallColumn1Row2.src = document.getElementById("image" + 2).value;
            tileContent.imageSmallColumn2Row1.src = document.getElementById("image" + 3).value;
            tileContent.imageSmallColumn2Row2.src = document.getElementById("image" + 4).value;

            // set the baseUri
            try {
                tileContent.baseUri = document.getElementById("baseUri").value;
            } catch (e) {
                WinJS.log && WinJS.log(e.message, "sample", "error");
                return;
            }
        }
        tileContent.requireSquareContent = false;
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        WinJS.log && WinJS.log(tileContent.getContent(), "sample", "status");
    }
})();
