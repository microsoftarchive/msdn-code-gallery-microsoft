//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S2-Send-Message.html", {
        processed: function (element, messageArgs) {
            // messageArgs is the parameter passed to navigation in the activated event handler.
            if (messageArgs) {
                WinJS.log && WinJS.log("Send message activation was received. The service to use is " + messageArgs.serviceId + ". The user ID to message is " + messageArgs.serviceUserId + ".", "sample", "status");
            }
        },
        ready: function (element, options) {
        }
    });
})();
