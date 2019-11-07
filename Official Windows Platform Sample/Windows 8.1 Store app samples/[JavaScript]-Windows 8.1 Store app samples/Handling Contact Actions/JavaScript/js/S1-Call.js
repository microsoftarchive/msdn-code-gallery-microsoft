//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S1-Call.html", {
        processed: function (element, callArgs) {
            // callArgs is the parameter passed to navigation in the activated event handler.
            if (callArgs) {
                if (callArgs.serviceId) {
                    if (callArgs.serviceId === "telephone") {
                        WinJS.log && WinJS.log("Call activation was received. The phone number to call is " + callArgs.serviceUserId + ".", "sample", "status");
                    } else {
                        WinJS.log && WinJS.log("This app doesn't support calling by using the " + callArgs.serviceId + " service.", "sample", "error");
                    }
                } else if (callArgs.uri) {
                    if (callArgs.uri.schemeName === "tel") {
                        WinJS.log && WinJS.log("Tel: activation was received. The phone number to call is " + callArgs.uri.path + ".", "sample", "status");
                    } else {
                        WinJS.log && WinJS.log("This app doesn't support the " + callArgs.uri.schemeName + " protocol.", "sample", "error");
                    }
                }
            }
        },
        ready: function (element, options) {
        }
    });
})();
