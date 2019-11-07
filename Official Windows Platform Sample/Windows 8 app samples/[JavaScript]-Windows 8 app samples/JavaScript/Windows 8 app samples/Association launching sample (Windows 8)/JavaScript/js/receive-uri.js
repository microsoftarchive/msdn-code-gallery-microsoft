//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/receive-uri.html", {
        processed: function (element, uri) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Uri is the parameter passed to navigation in the activated event handler.
            if (uri) {
                WinJS.log && WinJS.log("Protocol activation received. The received URI is " + uri.rawUri + ".", "sample", "status");
            }
        },

        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.
        }
    });
})();
