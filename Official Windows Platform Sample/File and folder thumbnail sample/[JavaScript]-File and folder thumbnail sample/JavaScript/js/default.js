//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "File thumbnails JS sample";

    var errors = {
        noExif:             "No result (no EXIF thumbnail or cached thumbnail available for fast retrieval)",
        noAlbumArt:         "No result (no album art available for this song)",
        noIcon:             "No result (no icon available for this document type)",
        noThumbnail:        "No result (no thumbnail could be obtained from the selected file)",
        noImages:           "No result (no thumbnail could be obtained from the selected folder - make sure that the folder contains images)",
        emptyFilegroup:     "No result (unexpected error: retrieved file group was null)",
        filegroupLocation:  "File groups are only available for library locations, please select a folder from one of your libraries",
        fail:               "No result (unexpected error: no thumbnail could be retrieved)",
        cancel:             "No result (operation cancelled, no item selected)",
        invalidSize:        "Invalid size (specified size must be numerical and greater than zero)",
    };

    var scenarios = [
        { url: "/html/scenario1.html", title: "Display a thumbnail for a picture" },
        { url: "/html/scenario2.html", title: "Display album art for a song" },
        { url: "/html/scenario3.html", title: "Display an icon for a document" },
        { url: "/html/scenario4.html", title: "Display a thumbnail for a folder" },
        { url: "/html/scenario5.html", title: "Display a thumbnail for a file group" },
        { url: "/html/scenario6.html", title: "Display a scaled image" },
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
        scenarios: scenarios,
        errors: errors,
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
