//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_Settings.html", {
        ready: function (element, options) {
            document.getElementById("settingsWriteSetting").addEventListener("click", settingsWriteSetting, false);
            document.getElementById("settingsDeleteSetting").addEventListener("click", settingsDeleteSetting, false);
            settingsDisplayOutput();
        }
    });

    var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;
    var settingName = "exampleSetting";
    var settingValue = "Hello World";

    // Guidance for Settings.
    //
    // Settings are a convenient way of storing small pieces of configuration data
    // for your application.
    //
    // Settings can be either Local or Roaming.
    //
    // Roaming settings will be synchronized across machines on which the user has
    // signed in with a Microsoft Account.  Roaming of settings is not instant; the
    // system weighs several factors when determining when to send the data.  Usage
    // of roaming data should be kept below the quota (available via the 
    // RoamingStorageQuota property), or else roaming of data will be suspended.
    //
    // User preferences for your application are a great match for roaming settings.
    // User preferences are usually fixed in number and small in size.  Users will
    // appreciated that your application is customized the way they prefer across
    // all of their machines.
    //
    // Local settings are not synchronized and remain on the machine on which they
    // were originally written.
    //
    // Care should be taken to guard against an excessive volume of data being
    // stored in settings.  Settings are not intended to be used as a database.
    // Large data sets will take longer to load from disk during your application's
    // launch.

    // This sample illustrates reading and writing from a roaming setting, though a
    // local setting could be used just as easily.

    function settingsWriteSetting() {
        roamingSettings.values[settingName] = settingValue;

        settingsDisplayOutput();
    }

    function settingsDeleteSetting() {
        roamingSettings.values.remove(settingName);

        settingsDisplayOutput();
    }

    function settingsDisplayOutput() {
        var value = roamingSettings.values[settingName];
        
        if (!value) {
            document.getElementById("settingsOutput").innerText = "Setting: <empty>";
        } else {
            document.getElementById("settingsOutput").innerText = "Setting: \"" + value + "\"";
        }
    }
})();
