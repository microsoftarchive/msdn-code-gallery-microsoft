//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
    var page = WinJS.UI.Pages.define("/html/app-listing-uri.html", {
        ready: function (element, options) {
            document.getElementById("displayLink").addEventListener("click", displayLink, false);
            // Initialize the license proxy file
            loadAppListingUriProxyFile();
        },
        unload: function () {
            currentApp.licenseInformation.removeEventListener("licensechanged", appListingUriRefreshScenario);
        }
    });

    function loadAppListingUriProxyFile() {
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("data").done(
            function (folder) {
                folder.getFileAsync("app-listing-uri.xml").done(
                    function (file) {
                        currentApp.licenseInformation.addEventListener("licensechanged", appListingUriRefreshScenario);
                        Windows.ApplicationModel.Store.CurrentAppSimulator.reloadSimulatorAsync(file).done();
                    });
            });
    }

    function appListingUriRefreshScenario() {
    }

    function displayLink() {
        WinJS.log && WinJS.log(currentApp.linkUri.absoluteUri, "sample", "status");
    }
})();
