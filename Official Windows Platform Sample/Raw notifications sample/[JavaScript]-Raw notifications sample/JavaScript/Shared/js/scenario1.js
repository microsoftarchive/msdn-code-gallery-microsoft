//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var background = Windows.ApplicationModel.Background;
    var pushNotifications = Windows.Networking.PushNotifications;
    var notifications = Windows.UI.Notifications;
    var sampleTaskName = "SampleJavascriptBackgroundTask";
    var sampleTaskEntryPoint = "js\\backgroundTask.js";
    var outputBox;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("scenario1Open").addEventListener("click", scenario1ReopenAndRegister, false);
            document.getElementById("scenario1UnregisterTask").addEventListener("click", scenario1UnregisterTask, false);
            if (SdkSample.channel) {
                document.getElementById("scenario1ChannelOutput").value = SdkSample.channel.uri;
            }
        }
    });

    function scenario1ReopenAndRegister() {
        background.BackgroundExecutionManager.requestAccessAsync().done(function (result) {
            // Make sure the app can even receive raw notifications
            if (result !== background.BackgroundAccessStatus.denied && result !== background.BackgroundAccessStatus.unspecified) {

                // Clean up the registered task just for the purpose of the sample
                unregisterBackgroundTask();

                // Only open a new channel if you haven't already done so
                if (!SdkSample.channel) {
                    openNotificationsChannel().done(registerBackgroundTask);
                } else {
                    registerBackgroundTask();
                }
            } else {
                WinJS.log && WinJS.log("Lock screen access is denied", "sample", "status");
            }
        }, function (e) {
            WinJS.log && WinJS.log("An error occurred while requesting lock screen access.", "sample", "error");
        });
    }

    function scenario1UnregisterTask() {
        if (unregisterBackgroundTask()) {
            WinJS.log && WinJS.log("Task unregistered", "sample", "status");
        } else {
            WinJS.log && WinJS.log("No task is registered", "sample", "error");
        }
    }

    function openNotificationsChannel() {
        // Open the channel. See the "Push and Polling Notifications" sample for more detail
        var channelOperation = pushNotifications.PushNotificationChannelManager.createPushNotificationChannelForApplicationAsync();
        WinJS.log && WinJS.log("Opening a channel...", "sample", "status");
        return channelOperation.then(function (newChannel) {
            WinJS.log && WinJS.log("Channel request succeeded!", "sample", "status");
            document.getElementById("scenario1ChannelOutput").value = newChannel.uri;
            SdkSample.channel = newChannel;
        },
            function (error) {
                WinJS.log && WinJS.log("Could not create a channel (error number: " + error.number + ")", "sample", "error");
            }
        );
    }

    function registerBackgroundTask() {
        // Register the background task for raw notifications
        var taskBuilder = new background.BackgroundTaskBuilder();
        var trigger = new background.PushNotificationTrigger();
        taskBuilder.setTrigger(trigger);
        taskBuilder.taskEntryPoint = sampleTaskEntryPoint;
        taskBuilder.name = sampleTaskName;

        try {
            var task = taskBuilder.register();
            task.addEventListener("completed", backgroundTaskComplete);
            WinJS.log && WinJS.log("Background task registered", "sample", "status");
        } catch (e) {
            WinJS.log && WinJS.log("Registration error: " + e.message, "sample", "error");
            unregisterBackgroundTask();
        }
    }

    function unregisterBackgroundTask() {
        var iter = background.BackgroundTaskRegistration.allTasks.first();
        while (iter.hasCurrent) {
            var task = iter.current.value;
            if (task.name === sampleTaskName) {
                task.unregister(true);
                return true;
            }
            iter.moveNext();
        }
        return false;
    }

    function backgroundTaskComplete() {
        // Retrieve state that is set when a raw notification is received
        try {
            // This sample assumes the payload is a string, but it can be of any type.
            var settings = Windows.Storage.ApplicationData.current.localSettings;
            WinJS.log && WinJS.log("Background task triggered by raw notification with payload = " + settings.values[sampleTaskName] + " has completed!", "sample", "status");
        } catch (e) {
            WinJS.log && WinJS.log("Error while processing background task: " + e.message, "sample", "error");
        }
    }
})();
