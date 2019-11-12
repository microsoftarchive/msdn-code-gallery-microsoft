//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Media Extensions";

    var scenarios = [
        { url: "/html/LocalDecoder.html", title: "Local Decoder" },
        { url: "/html/SchemaHandler.html", title: "Custom Schema Handler" },
        { url: "/html/VideoStabilizationEffect.html", title: "Video Stabilization Effect" },
        { url: "/html/CustomEffect.html", title: "Custom Effects" }
    ];

    function pickMediaFile(filters, videos) {
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        if (currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped &&
            !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
            // File picker cannot be used in the snapped state.
            SdkSample.displayStatus("File picker cannot be used when application is in the snapped state.");
            return;
        }

        var Pickers = Windows.Storage.Pickers,
            picker = new Pickers.FileOpenPicker();

        picker.suggestedStartLocation = Pickers.PickerLocationId.videosLibrary;
        picker.fileTypeFilter.replaceAll(filters);
        picker.pickSingleFileAsync().done(
            function (file) {
            if (file) {
                videos.forEach(function(video, i) {
                    video.src = URL.createObjectURL(file, {oneTimeOnly: true});
                });
            }
        },
            function (e) {
            SdkSample.displayError("Error while selecting a file: " + e.message);
        });
    }

    function videoOnError(error) {
        if (!error.currentTarget.suppressErrors) {
            switch (error.target.error.code) {
                case error.target.error.MEDIA_ERR_ABORTED:
                    SdkSample.displayError("You aborted the video playback.");
                    break;
                case error.target.error.MEDIA_ERR_NETWORK:
                    SdkSample.displayError("A network error caused the video download to fail part-way.");
                    break;
                case error.target.error.MEDIA_ERR_DECODE:
                    SdkSample.displayError("The video playback was aborted due to a corruption problem or because the video used features your browser did not support.");
                    break;
                case error.target.error.MEDIA_ERR_SRC_NOT_SUPPORTED:
                    SdkSample.displayError("The video could not be loaded, either because the server or network failed or because the format is not supported.");
                    break;
                default:
                    SdkSample.displayError("An unknown error occurred.");
                    break;
            }    
        }
        error.currentTarget.suppressErrors = false;
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        navigationDisabled: false,
        currentScenarioUrl: null,
        disabledOnIndex: 0,
        pickMediaFile: pickMediaFile,
        videoOnError: videoOnError,

        formatError: function (e, prefix) {
            if (e.number) {
                WinJS.log && WinJS.log(
                    prefix +
                    e.message +
                    ", Error code: " +
                    e.number.toString(16),
                    null,
                    "error"
                );
            } else {
                WinJS.log && WinJS.log(
                    prefix +
                    e.message,
                    null,
                    "error"
                );
            }
        },

        displayStatus: function (msg) {
            WinJS.log && WinJS.log(msg, null, "status");
        },

        displayError: function (msg) {
            WinJS.log && WinJS.log(msg, null, "error");
        },

        clearLastStatus: function () {
            WinJS.log && WinJS.log("", null, "status");
        },
    });

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

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
