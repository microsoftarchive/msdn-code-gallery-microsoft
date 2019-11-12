//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {

            //Setting the ApplicationView property IsScreenCaptureEnabled to true will allow screen capture.
            //This is the default setting for this property.
            Windows.UI.ViewManagement.ApplicationView.getForCurrentView().isScreenCaptureEnabled = true;
        }
    });
})();
