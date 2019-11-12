//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var common = {};
var platform = {};

(function () {
    common.backgroundTaskEntryPoint = "js\\backgroundtask.js";
    common.backgroundTaskName = "AuthenticationBackgroundTask";
    common.authenticationContext = null;
    common.completeHandlerCallback = null;
    common.hasRegisteredBackgroundTaskHandler = false;

    // Handle background task completion.
    function completeHandler(task) {
        // Update the UI with the completion status of the background task
        // The Run method of the background task sets this status.
        this.onCompleted = function (args) {
            try {
                if (task.name === common.backgroundTaskName)
                {
                    WinJS.log && WinJS.log("Background task handler completed", "sample", "status");

                    // Signal event for foreground authentication
                    if (common.completeHandlerCallback !== null) {
                        common.completeHandlerCallback();
                    }
                }
            } catch (ex) {
                WinJS.log && WinJS.log(ex, "sample", "error");
            }
        };
    }
    common.completeHandler = completeHandler;

    // Background task completion handler. When authenticating through the foreground app,
    // this triggers the authentication flow if the app is currently running.
    function registerBackgroundTaskEventHandler() {
        if (!common.hasRegisteredBackgroundTaskHandler) {
            try {
                var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
                var hascur = iter.hasCurrent;
                while (hascur) {
                    var cur = iter.current.value;
                    if (cur.name === common.backgroundTaskName) {
                        cur.addEventListener("completed", new common.completeHandler(cur).onCompleted);
                        common.hasRegisteredBackgroundTaskHandler = true;
                        break;
                    }
                    hascur = iter.moveNext();
                }
            } catch (ex) {
                WinJS.log && WinJS.log(ex, "sample", "error");
            }
        }
        return common.hasRegisteredBackgroundTaskHandler;
    }
    common.registerBackgroundTaskEventHandler = registerBackgroundTaskEventHandler;

})();
