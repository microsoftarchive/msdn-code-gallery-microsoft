//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario9_imageProtocols.html", {
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
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.replaceAll([".jpg", ".jpeg", ".png", ".gif"]);
        picker.commitButtonText = "Copy";
        picker.pickSingleFileAsync().then(function (file) {
            return file.copyAsync(Windows.Storage.ApplicationData.current.localFolder, file.name, Windows.Storage.NameCollisionOption.generateUniqueName);
        }).done(function (newFile) {
            var imageAbsolutePath = newFile.path;
            //change image to relative path
            imageRelativePath = imageAbsolutePath.substring(imageAbsolutePath.lastIndexOf("\\") + 1);
            document.getElementById("notificationXmlContent").innerText = "";
            WinJS.log && WinJS.log("Image copied to application data local storage: " + newFile.path, "sample", "status");
        }, function (e) {
            document.getElementById("notificationXmlContent").innerText = "";
            WinJS.log && WinJS.log(e, "sample", "error");
        });
    }

    function sendTileNotification() {
        var protocol = document.getElementById("imageProtocolSelector").selectedIndex;
        var wide310x150TileContent;

        if (protocol === 0) { //using the ms-appx: protocol

            wide310x150TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150ImageAndText01();
            wide310x150TileContent.textCaptionWrap.text = "The image is in the appx package";
            wide310x150TileContent.image.src = "ms-appx:///images/redWide310x150.png";

        } else if (protocol === 1) { //using the ms-appdata:/// protocol

            wide310x150TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Image();
            wide310x150TileContent.image.src = "ms-appdata:///local/" + imageRelativePath; // make sure you are providing a relative path!

        } else if (protocol === 2) { //using http:// protocol
            // Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
            wide310x150TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150PeekImageCollection04();
            wide310x150TileContent.textBodyWrap.text = "The baseUri is " + document.getElementById("baseUri").value;

            wide310x150TileContent.imageMain.src = document.getElementById("image" + 0).value;
            wide310x150TileContent.imageSmallColumn1Row1.src = document.getElementById("image" + 1).value;
            wide310x150TileContent.imageSmallColumn1Row2.src = document.getElementById("image" + 2).value;
            wide310x150TileContent.imageSmallColumn2Row1.src = document.getElementById("image" + 3).value;
            wide310x150TileContent.imageSmallColumn2Row2.src = document.getElementById("image" + 4).value;

            // set the baseUri
            try {
                wide310x150TileContent.baseUri = document.getElementById("baseUri").value;
            } catch (e) {
                document.getElementById("notificationXmlContent").innerText = "";
                WinJS.log && WinJS.log(e.message, "sample", "error");
                return;
            }
        }
        wide310x150TileContent.requireSquare150x150Content = false;
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(wide310x150TileContent.createNotification());

        document.getElementById("notificationXmlContent").innerText = wide310x150TileContent.getContent();
        WinJS.log && WinJS.log("Tile notification sent", "sample", "status");
    }
})();