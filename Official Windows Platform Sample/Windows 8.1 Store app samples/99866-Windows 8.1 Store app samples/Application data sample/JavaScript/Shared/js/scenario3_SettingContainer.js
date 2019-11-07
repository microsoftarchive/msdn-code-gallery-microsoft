//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3_SettingContainer.html", {
        ready: function (element, options) {
            document.getElementById("containersCreateContainer").addEventListener("click", containersCreateContainer, false);
            document.getElementById("containersDeleteContainer").addEventListener("click", containersDeleteContainer, false);
            document.getElementById("containersWriteSetting").addEventListener("click", containersWriteSetting, false);
            document.getElementById("containersDeleteSetting").addEventListener("click", containersDeleteSetting, false);
            containersDisplayOutput();
        }
    });

    var localSettings = Windows.Storage.ApplicationData.current.localSettings;
    var containerName = "exampleContainer";
    var settingName = "exampleSetting";

    function containersCreateContainer() {
        var container = localSettings.createContainer(containerName, Windows.Storage.ApplicationDataCreateDisposition.always);

        containersDisplayOutput();
    }

    function containersDeleteContainer() {
        localSettings.deleteContainer(containerName);

        containersDisplayOutput();
    }

    function containersWriteSetting() {
        if (localSettings.containers.hasKey(containerName)) {
            localSettings.containers.lookup(containerName).values[settingName] = "Hello World"; // example value
        }

        containersDisplayOutput();
    }

    function containersDeleteSetting() {
        if (localSettings.containers.hasKey(containerName)) {
            localSettings.containers.lookup(containerName).values.remove(settingName);
        }

        containersDisplayOutput();
    }

    function containersDisplayOutput() {
        var hasContainer = localSettings.containers.hasKey(containerName);
        var hasSetting = hasContainer ? localSettings.containers.lookup(containerName).values.hasKey(settingName) : false;

        document.getElementById("containersOutput").innerText = "Container Exists: " + /*@static_cast(String)*/hasContainer + "\nSetting Exists: " + /*@static_cast(String)*/hasSetting;
    }
})();
