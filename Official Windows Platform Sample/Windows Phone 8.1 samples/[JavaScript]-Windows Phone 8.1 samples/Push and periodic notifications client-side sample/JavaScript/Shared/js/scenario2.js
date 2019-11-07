//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
  
    var background = Windows.ApplicationModel.Background;
    var pushNotificationsTaskName = "UpdateChannels";
    var maintenanceInterval = 10 * 24 * 60; // Check for channels that need to be updated every 10 days

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("renewChannels").addEventListener("click", renewAllChannels, false);
            document.getElementById("registerBackgroundTask").addEventListener("click", registerTask, false);
            document.getElementById("unregisterBackgroundTask").addEventListener("click", unregisterTask, false);

            if (!SampleNotifications.notifier) {
                SampleNotifications.notifier = new SampleNotifications.Notifier();
            }
        }
    });

    function renewAllChannels() {
        SampleNotifications.notifier.renewAllAsync(true).done(function () {
            WinJS.log && WinJS.log("Channels renewed successfully", "sample", "status");
        }, function (error) {
            WinJS.log && WinJS.log("Channels renewal failed: " + error.message, "sample", "error");
        });
    }

    // When the application starts for the very first time, it should register the background task
    // to renew all the push notification channels when the device receives AC power
    function registerTask() {
        if (!getRegisteredTask()) {
            var taskBuilder = new background.BackgroundTaskBuilder();
            var trigger = new background.MaintenanceTrigger(maintenanceInterval, false);
            taskBuilder.setTrigger(trigger);
            taskBuilder.taskEntryPoint = "js\\backgroundTask.js";
            taskBuilder.name = pushNotificationsTaskName;

            var internetCondition = new background.SystemCondition(background.SystemConditionType.internetAvailable);
            taskBuilder.addCondition(internetCondition);

            try {
                taskBuilder.register();
                WinJS.log && WinJS.log("Task registered", "sample", "status");
            } catch (e) {
                WinJS.log && WinJS.log("Error registering task: " + e.message, "sample", "error");
            }
        } else {
            WinJS.log && WinJS.log("Task already registered", "sample", "error");
        }
    }

    function unregisterTask() {
        var task = getRegisteredTask();
        if (task) {
            task.unregister(true);
            WinJS.log && WinJS.log("Task unregistered", "sample", "error");
        } else {
            WinJS.log && WinJS.log("Task not registered", "sample", "error");
        }
    }

    function getRegisteredTask() {
        var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
        var hascur = iter.hasCurrent;
        while (hascur) {
            var cur = iter.current.value;
            if (cur.name === pushNotificationsTaskName) {
                return cur;
            }
            hascur = iter.moveNext();
        }
        return null;
    }

})();
