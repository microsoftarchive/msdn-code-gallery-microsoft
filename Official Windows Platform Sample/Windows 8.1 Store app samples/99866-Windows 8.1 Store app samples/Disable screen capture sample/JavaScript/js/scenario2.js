//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {

            //Setting the ApplicationView property IsScreenCaptureEnabled to false will prevent screen capture
            Windows.UI.ViewManagement.ApplicationView.getForCurrentView().isScreenCaptureEnabled = false;
        }
    });
})();
