//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "File access JS sample";

    var sampleFile = null;
    var mruToken = null;
    var falToken = null;

    var scenarios = [
        { url: "/html/scenario1.html", title: "Creating a file" },
        { url: "/html/scenario2.html", title: "Writing and reading text in a file" },
        { url: "/html/scenario3.html", title: "Writing and reading bytes in a file" },
        { url: "/html/scenario4.html", title: "Writing and reading using a stream" },
        { url: "/html/scenario5.html", title: "Displaying file properties" },
        { url: "/html/scenario6.html", title: "Persisting a storage item for future use" },
        { url: "/html/scenario7.html", title: "Copying a file" },
        { url: "/html/scenario8.html", title: "Deleting a file" }
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

    function validateFileExistence() {
        Windows.Storage.KnownFolders.documentsLibrary.getFileAsync("sample.dat").done(
            function (file) {
                // If file exists.
                SdkSample.sampleFile = file;
            },
            function (err) {
                // If file doesn't exist, indicate users to use scenario 1.
                SdkSample.sampleFile = null;
                WinJS.log && WinJS.log("The file 'sample.dat' does not exist. Use scenario one to create this file.", "sample", "error");
            }
        );
    };

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        validateFileExistence: validateFileExistence,
        sampleFile: sampleFile,
        mruToken: mruToken,
        falToken: falToken
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
