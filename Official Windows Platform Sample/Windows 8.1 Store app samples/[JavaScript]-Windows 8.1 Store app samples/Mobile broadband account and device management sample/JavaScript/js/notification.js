//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var operatorNotifcationTaskEntryPoint = "backgroundtask.js",
        operatorNotificationTaskName = "MobileOperatorNotificationHandler";

    var page = WinJS.UI.Pages.define("/html/notification.html", {
        ready: function (element, options) {
            initializeOperatorMessage();
        }
    });

    function initializeOperatorMessage() {
        try {
            addCompletionEvents();
            var mobileBroadbandDevices = Windows.Networking.NetworkOperators.MobileBroadbandAccount.availableNetworkAccountIds;
            if (mobileBroadbandDevices.size !== 0) {
                WinJS.log && WinJS.log("Mobile broadband account found", "sample", "status");
            }
            else
            {
                WinJS.log && WinJS.log("No mobile broadband accounts found", "sample", "status");
            }
        } catch (ex) {
            WinJS.log && WinJS.log("Error finding background task: " + ex.description, "sample", "error");
        }
    }

    // Handle background task completion.
    function CompleteHandler(task) {
        this.onCompleted = function (args) {
            try {
                args.checkResult();
                WinJS.log && WinJS.log("Operator notification background task completed.", "sample", "status");
            } catch (ex) {
                WinJS.log && WinJS.log("Operator notification background task failed.", "sample", "error");
            }
        };
    }

    // Operator messages are pre-registered at install time with a
    // DeviceNotificationHandler element in the metadata package. In
    // order to handle completion events, find the already registered
    // background task.
    function addCompletionEvents()
    {
        var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
        var hascur = iter.hasCurrent;
        while (hascur) {
            var cur = iter.current.value;

            if (cur.name === operatorNotificationTaskName) {
                cur.addEventListener("completed", new CompleteHandler(cur).onCompleted);
            }
            hascur = iter.moveNext();
        }
    }
})();
