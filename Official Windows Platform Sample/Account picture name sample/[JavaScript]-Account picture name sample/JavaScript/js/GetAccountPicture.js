//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/GetAccountPicture.html", {
        ready: function (element, options) {
            hideVisibleHolders();
            document.getElementById("getSmallImage").addEventListener("click", getSmallImage, false);
            document.getElementById("getLargeImage").addEventListener("click", getLargeImage, false);
            document.getElementById("getVideo").addEventListener("click", getVideo, false);
        }
    });

    function getSmallImage() {
        hideVisibleHolders();
        // The small picture returned by getAccountPicture() is 96x96 pixels in size.
        var image = Windows.System.UserProfile.UserInformation.getAccountPicture(Windows.System.UserProfile.AccountPictureKind.smallImage);
        if (image) {
            document.getElementById("smallImageHolder").style.visibility = "visible";
            document.getElementById("smallImageHolder").src = URL.createObjectURL(image, { oneTimeOnly: true });
            WinJS.log && WinJS.log("Small image path= " + image.path, "sample", "status");
        } else {
            WinJS.log && WinJS.log("Small account picture is not available.", "sample", "status");
        }
    }

    function getLargeImage() {
        hideVisibleHolders();
        // The large picture returned by getAccountPicture() is 448x448 pixels in size.
        var image = Windows.System.UserProfile.UserInformation.getAccountPicture(Windows.System.UserProfile.AccountPictureKind.largeImage);
        if (image) {
            document.getElementById("largeImageHolder").style.visibility = "visible";
            document.getElementById("largeImageHolder").src = URL.createObjectURL(image, { oneTimeOnly: true });
            WinJS.log && WinJS.log("Large image path= " + image.path, "sample", "status");
        } else {
            WinJS.log && WinJS.log("Large account picture is not available.", "sample", "status");
        }
    }

    function getVideo() {
        hideVisibleHolders();
        // The video returned from getAccountPicture is 448x448 pixels in size.
        var video = Windows.System.UserProfile.UserInformation.getAccountPicture(Windows.System.UserProfile.AccountPictureKind.video);
        if (video) {
            document.getElementById("videoHolder").style.visibility = "visible";
            document.getElementById("videoHolder").src = URL.createObjectURL(video, { oneTimeOnly: true });
            document.getElementById("videoHolder").play();
            WinJS.log && WinJS.log("Video path= " + video.path, "sample", "status");
        } else {
            WinJS.log && WinJS.log("Video is not available.", "sample", "status");
        }
    }

    function hideVisibleHolders() {
        document.getElementById("smallImageHolder").style.visibility = "hidden";
        document.getElementById("largeImageHolder").style.visibility = "hidden";
        document.getElementById("videoHolder").style.visibility = "hidden";
    }
})();
