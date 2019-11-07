//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Activation and Lifetime";
    var activationDeferral = null;
    var scenarios = [
        { url: "/html/lifetime.html", title: "Activation, Suspending and Resuming of a JS App" },
        { url: "/html/protocol.html", title: "Protocol activation" },
    ];

    function completeDeferral() {
        // This function is used to complete deferral taken during activation.
        // After the asynchronous operation is done the app must call complete on the deferral object
        // as follows else the app would get terminated. Make sure the app calls complete on the deferral
        // object even when there's an exception else the app would get terminated.
        if (activationDeferral) {
            activationDeferral.complete();
            activationDeferral = null;
        }
    }

    function domContentLoaded() {
        // The DOMContentLoaded event is raised after the app loads all its JavaScript and CSS files in the top level document.
        // This happens before the activation event is fired. All the referenced files are ready to be accessed
        // (including images, although they are not guaranteed to be loaded). At this point, you can begin
        // initialization for your top level document (app initialization doesn't usually require loaded images).
    }

    function activated(eventObject) {
        // The activated event is raised when the app is activated by the system. During an initial launch
        // it fires after DOMContentLoaded, but before window.onload. It tells the app whether it was
        // activated because the user launched it or it was launched by some other means. Use the
        // activated event handler to check the type of activation and respond appropriately to it,
        // and to load any state needed for the activation.

        var url = null;

        if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Navigate to either the first scenario or to the last running scenario
            // before suspension or termination.
            url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
        } else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.protocol) {
            // This is a protocol activation, let's navigate to the protocol.html fragment.
            // Protocol format currently supported in this sample is: sdksampleprotocol:domain?src=[some url]
            url = scenarios[1].url;
        }
        //else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.search) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.shareTarget) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.sendTarget) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.file) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.fileOpenPicker) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.fileSavePicker) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.cachedFileUpdater) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.contactPicker) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.device) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.printTaskSettings) {
        //    // noop
        //} else if (eventObject.kind === Windows.ApplicationModel.Activation.ActivationKind.cameraSettings) {
        //    // noop
        //}

        if (url) {
            // If there is going to be some asynchronous operation done during activation then
            // the app can signal the need to handle activation of the application asynchronously.
            // To do so the app can use the getDeferral method.
            activationDeferral = eventObject.activatedOperation.getDeferral();

            WinJS.UI.processAll().then(function () {
                WinJS.Navigation.navigate(url, eventObject);
            });
        }
    }

    function load() {
        // After the DOMContentLoaded and activated events are raised and after all the images referenced by the page are loaded,
        // the onload event is raised. If you need to process images in your top level document, this is where you do it.
    }

    function suspending(eventObject) {
        // An app can be suspended when the user moves it to the background or when the system enters a low power state.
        // When the app is being suspended, it raises the suspending event and has a limited amount of time to save its data. If
        // the app's suspending event handler doesn't complete within that limited time the system assumes the app has stopped
        // responding and terminates it.

        var host = document.getElementById("contentHost");
        // Call suspending method on current scenario, if there is one
        host.winControl && host.winControl.suspending && host.winControl.suspending(eventObject);
    }

    function resuming() {
        // Most apps don't need a resuming event handler. When your app is resumed, variables and objects have
        // the exact same state they had when the app was suspended. Register a resuming handler only if you
        // need to update data or objects that might have changed between the time your app was suspended and
        // when it was resumed, such as content or network connections that may have gone stale, or if you need
        // to reacquire access to a device, such as a webcam.
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
            completeDeferral();
        });
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    document.addEventListener("DOMContentLoaded", domContentLoaded, false);
    window.addEventListener("load", load, false);
    Windows.UI.WebUI.WebUIApplication.addEventListener("activated", activated, false);
    Windows.UI.WebUI.WebUIApplication.addEventListener("suspending", suspending, false);
    Windows.UI.WebUI.WebUIApplication.addEventListener("resuming", resuming, false);
    WinJS.Application.start();
})();
