//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Device App For Printers JavaScript Sample";

    var scenarios = [
        { url: "/html/ink-level.html", title: "Ink Level" },
        { url: "/html/preferences.html", title: "Advanced Settings" },
    ];
    
    function activated(eventArgs) {
        // This function is the entry point for the DeviceAppForPrinters application.
        // The code below will determine which of 3 possible activation types was invoked,
        // and perform the appropriate action(s).

        if (!eventArgs) {
            WinJS.log && WinJS.log("eventArguments to activation is null", "sample", "error");
            return;
        }

        var url;
        var args;
        if (eventArgs.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {

            // Tile activation is the same as any other application - the user clicked on the tile.
            url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;

            // Toast activation happens when a user clicks on a Toast that has been created by this application.
            // Toast activation is similar to Tile activation above, except that whatever code
            // was responsible for showing the toast (in this application's case, it's the PrintNotificationHelper class)
            // can pass a context, which will be available here as the 'arguments' field of the event arguments object.

            // The notificationArgs argument contains the arguments of the toast.
            args = eventArgs.detail;

        } else if (eventArgs.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.printTaskSettings) {
            // 'printTaskSettings' activation happens when a user selects "more settings" from the print dialog flyout.
            // This type of activation receives context data about the printer that the user has selected.

            // The configuration argument contains the functionality for retrieving information
            // about the current printer context, as well as subscribing to the saverequested event.
            args = eventArgs.detail.configuration;

            url = WinJS.Application.sessionState.lastUrl || scenarios[1].url;

        } else {
            WinJS.log && WinJS.log("Unrecognized activation type: " + eventArgs.detail.kind, "sample", "error");
            return;
        }

        eventArgs.setPromise(WinJS.UI.processAll().then(function () {
            return WinJS.Navigation.navigate(url, args);
        }));
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        });
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
