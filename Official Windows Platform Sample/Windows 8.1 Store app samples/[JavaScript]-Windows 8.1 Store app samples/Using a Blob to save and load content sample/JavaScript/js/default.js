//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="Windows.Storage.js"/>
/// <reference path="base-sdk.js"/>
/// <reference path="jstoolkit-sdk.js"/>
/// <reference path="Blob.js"/>
/// <reference path="XMLhttpRequest.js"/>

(function () {
    "use strict";

    var sampleTitle = "Blob Sample";
    
    WinJS.Namespace.define("BlobSample", {
        asyncError: function (error) {
            WinJS.log && WinJS.log("Async failure", "sample", "error");
        }
    });

    var scenarios = [
        { url: "/html/FileToBlobUrl.html", title: "FilePicker -> URL -> Image, Video, or Audio" },
        { url: "/html/MultiVsSingleUseBlobUrl.html", title: "File Picker -> URL -> Multi use VS Single use Blob Urls" },
        { url: "/html/StorageFileReadText.html", title: "File Picker -> StorageFile -> text of file" },
        { url: "/html/FileReadText.html", title: "input type='file' -> File -> text of file" },
        { url: "/html/CanvasBlob-Image.html", title: "Canvas -> Blob -> Image" },
        { url: "/html/XhrBlobSaveToDisk.html", title: "XHR Blob Download -> Save to Disk" },
        { url: "/html/BlobBuilder.html", title: "BlobBuilder" },
        { url: "/html/PostMessageBlob.html", title: "Postmessage Blob to Web Context" },
        { url: "/html/ThumbnailBlob-Image.html", title: "StorageFile Thumbnail -> Blob -> Image tag" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
