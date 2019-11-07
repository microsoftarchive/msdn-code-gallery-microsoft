//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/settings.html", {
        ready: function (element, options) {
            document.getElementById("showSetting").addEventListener("click", showSettingsFlyout, false);

            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });

    function showSettingsFlyout() {
            WinJS.UI.SettingsFlyout.showSettings("Downloads");
    }

   WinJS.Application.onsettings = function (e) {
       e.detail.applicationcommands = { "Downloads": { title: "Downloads"} };
       WinJS.UI.SettingsFlyout.populateSettings(e);
   };

})();

