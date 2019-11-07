//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/auth-by-backgroundtask.html", {
        ready: function (element, options) {
            // Configure background task handler to perform authentication
            configuration.setAuthenticateThroughBackgroundTask(true);

            // Register background task completion handler
            common.registerBackgroundTaskEventHandler();
        }
    });
})();
