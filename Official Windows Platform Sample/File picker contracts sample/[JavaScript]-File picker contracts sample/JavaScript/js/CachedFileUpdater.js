//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var cachedFileUpdaterUI;
    var fileUpdateRequest;
    var fileUpdateRequestDeferral;

    var sampleTitle = "Cached File Updater app extension sample";

    var scenarios;

    function activated(eventObject) {
        //to identify if app is launched for cachedFileUpdater
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.cachedFileUpdater) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            cachedFileUpdaterUI = eventObject.detail.cachedFileUpdaterUI;

            cachedFileUpdaterUI.addEventListener("fileupdaterequested", onFileUpdateRequest);
            cachedFileUpdaterUI.addEventListener("uirequested", onUIRequested);

            switch (cachedFileUpdaterUI.updateTarget) {
                case Windows.Storage.Provider.CachedFileTarget.local:
                    scenarios = [{ url: "/html/cachedFileUpdaterScenario1.html", title: "Get latest version" }];
                    break;
                case Windows.Storage.Provider.CachedFileTarget.remote:
                    scenarios = [{ url: "/html/cachedFileUpdaterScenario2.html", title: "Remote file update" }];
                    break;
            }
            SdkSample.scenarios = scenarios;
        }
    }

    function onFileUpdateRequest(e) {
        fileUpdateRequest = e.request;
        fileUpdateRequestDeferral = fileUpdateRequest.getDeferral();

        switch (cachedFileUpdaterUI.uiStatus) {
            case Windows.Storage.Provider.UIStatus.hidden:
                fileUpdateRequest.status = Windows.Storage.Provider.FileUpdateStatus.userInputNeeded;
                fileUpdateRequestDeferral.complete();
                break;
            case Windows.Storage.Provider.UIStatus.visible:
                var url = scenarios[0].url;
                WinJS.Navigation.navigate(url, cachedFileUpdaterUI);
                break;
            case Windows.Storage.Provider.UIStatus.unavailable:
                fileUpdateRequest.status = Windows.Storage.Provider.FileUpdateStatus.failed;
                fileUpdateRequestDeferral.complete();
                break;
        }
    }

    function onUIRequested(e) {
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, { navigationState: eventObject.detail.state, cachedFileUpdaterUI: cachedFileUpdaterUI, fileUpdateRequest: fileUpdateRequest, fileUpdateRequestDeferral: fileUpdateRequestDeferral }).then(function () {
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
