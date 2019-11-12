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
            //ui-light by default
            if (document.styleSheets[3].disabled === true) {
                document.getElementById("darkStyle").checked = false;
                document.getElementById("lightStyle").checked = true;
            } else if (document.styleSheets[0].disabled === true) {
                document.getElementById("darkStyle").checked = true;
                document.getElementById("lightStyle").checked = false;
            }
        }
    });

    function showSettingsFlyout() {

        //Show the setting flyout if not in the snapped view
        if (Windows.UI.ViewManagement.ApplicationView.value !== Windows.UI.ViewManagement.ApplicationViewState.snapped) {
            WinJS.UI.SettingsFlyout.showSettings("Downloads");
        }

    }

   WinJS.Application.onsettings = function (e) {
       e.detail.applicationcommands = { "Downloads": { title: "Downloads"} };
       WinJS.UI.SettingsFlyout.populateSettings(e);
   };

})();

