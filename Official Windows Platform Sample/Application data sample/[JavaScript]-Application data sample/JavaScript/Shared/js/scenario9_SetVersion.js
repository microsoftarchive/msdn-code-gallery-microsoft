//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario9_SetVersion.html", {
        ready: function (element, options) {
            document.getElementById("setVersion0").addEventListener("click", setVersion0, false);
            document.getElementById("setVersion1").addEventListener("click", setVersion1, false);
            setVersionDisplayOutput();
        }
    });

    var appData = Windows.Storage.ApplicationData.current;
    var localSettings = appData.localSettings;

    var settingName = "setVersionSetting";
    var settingValueV0 = "Data.v0";
    var settingValueV1 = "Data.v1";

    function setVersion0Callback(setVersionRequest) {
        var setVersionDeferral = setVersionRequest.getDeferral();
        var version = appData.version;
        if (version === 0) {
            // Version is already 0.  Nothing to do.
        } else if (version === 1) {
            // Need to convert data from v1 to v0.
            // This sample simulates that conversion by writing a version-specific value.
            localSettings.values[settingName] = settingValueV0;
        } else {
            // Unexpected version!
            throw "Unexpected ApplicationData Version: " + version;
        }

        setVersionDeferral.complete();
    }

    function setVersion1Callback(setVersionRequest) {
        var setVersionDeferral = setVersionRequest.getDeferral();
        var version = appData.version;
        if (version === 1) {
            // Version is already 1.  Nothing to do.
        } else if (version === 0) {
            // Need to convert data from v0 to v1.

            // This sample simulates that conversion by writing a version-specific value.
            localSettings.values[settingName] = settingValueV1;
        } else {
            // Unexpected version!
            throw "Unexpected ApplicationData Version: " + version;
        }
        setVersionDeferral.complete();
    }

    function setVersion0() {
        appData.setVersionAsync(0, setVersion0Callback).then(setVersionDisplayOutput);
    }

    function setVersion1() {
        appData.setVersionAsync(1, setVersion1Callback).then(setVersionDisplayOutput);
    }

    function setVersionDisplayOutput() {
        var output = "Version: " + appData.version;

        document.getElementById("setVersionOutput").innerText = output;
    }
})();
