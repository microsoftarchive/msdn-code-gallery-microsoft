//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4_CompositeSettings.html", {
        ready: function (element, options) {
            document.getElementById("compositeSettingsWriteCompositeSetting").addEventListener("click", compositeSettingsWriteCompositeSetting, false);
            document.getElementById("compositeSettingsDeleteCompositeSetting").addEventListener("click", compositeSettingsDeleteCompositeSetting, false);
            compositeSettingsDisplayOutput();
        }
    });

    var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;
    var settingName = "exampleCompositeSetting";
    var settingName1 = "one";
    var settingName2 = "hello";

    function compositeSettingsWriteCompositeSetting() {
        var composite = new Windows.Storage.ApplicationDataCompositeValue();
        composite[settingName1] = 1; // example value
        composite[settingName2] = "world"; // example value
        roamingSettings.values[settingName] = composite;

        compositeSettingsDisplayOutput();
    }

    function compositeSettingsDeleteCompositeSetting() {
        roamingSettings.values.remove(settingName);

        compositeSettingsDisplayOutput();
    }

    function compositeSettingsDisplayOutput() {
        var composite = roamingSettings.values[settingName];

        if (!composite) {
            document.getElementById("compositeSettingsOutput").innerText = "Composite Setting: <empty>";
        } else {
            document.getElementById("compositeSettingsOutput").innerText = "Composite Setting: {" 
                + settingName1 
                + " = " 
                + composite[settingName1] 
                + ", "
                + settingName2
                +" = \"" 
                + composite[settingName2] 
                + "\"}";
        }
    }
})();
