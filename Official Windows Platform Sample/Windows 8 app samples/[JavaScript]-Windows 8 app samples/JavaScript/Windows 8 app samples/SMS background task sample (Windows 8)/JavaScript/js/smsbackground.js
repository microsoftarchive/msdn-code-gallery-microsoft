//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/smsbackground.html", {
        ready: function (element, options) {
            try {

            document.getElementById("registerSampleJavaScriptBackgroundTask").addEventListener("click", registerSampleJavaScriptBackgroundTask, false);
            document.getElementById("unregisterSampleJavaScriptBackgroundTask").addEventListener("click", unregisterSampleJavaScriptBackgroundTask, false);

            //
            // Initialize UI elements based on currently registered background tasks.
            //
            updateSampleJavaScriptBackgroundTaskUIState(false);

            //
            // Associate JS event handlers with background task progress and completion events.
            //
            var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
            var hascur = iter.hasCurrent;
            while (hascur) {
                var cur = iter.current.value;

                if (cur.name === sampleJavaScriptBackgroundTaskName) {
                    updateSampleJavaScriptBackgroundTaskUIState(true);
                    cur.addEventListener("progress", new ProgressHandler(cur).onProgress);
                    cur.addEventListener("completed", new CompleteHandler(cur).onCompleted);
                }
                hascur = iter.moveNext();
            }

        } catch (ex) {
            WinJS.log && WinJS.log(ex.name + " : " + ex.description, "sample", "error");
        }
        }
    });

    var hasDeviceAccess = false;
    var sampleJavaScriptBackgroundTaskEntryPoint = "js\\backgroundtask.js";
    var sampleJavaScriptBackgroundTaskName = "SampleSMSJavaScriptBackgroundTask";

    //
    // Handle background task progress.
    //
    function ProgressHandler(task) {
        this.onProgress = function (args) {
            try {
                var progress = "Progress: " + args.progress + "%";
                document.getElementById("sampleJavaScriptBackgroundTaskProgress").innerText = progress;
            } catch (ex) {
                WinJS.log && WinJS.log(ex.name + " : " + ex.description, "sample", "error");
            }
        };
    }

    //
    // Handle background task completion.
    //
    function CompleteHandler(task) {
        this.onCompleted = function (args) {
            try {
                var key = task.taskId;
                var settings = Windows.Storage.ApplicationData.current.localSettings;
                document.getElementById("sampleJavaScriptBackgroundTaskStatus").innerText = settings.values[key];
            } catch (ex) {
                WinJS.log && WinJS.log(ex.name + " : " + ex.description, "sample", "error");
            }
        };
    }

    //
    // Register a JavaScript background task for a SMS text message received event.
    //
    function registerBackgroundTask()
    {
        //
        // Prepare to create the background task.
        //
        try {

            //
            // Create a new background task builder.
            //
            var myTaskBuilder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

            //
            // Create a new text message received trigger.
            //
            var myTrigger = new Windows.ApplicationModel.Background.SystemTrigger(Windows.ApplicationModel.Background.SystemTriggerType.smsReceived, false);

            //
            // Associate the text message received trigger with the background task builder.
            //
            myTaskBuilder.setTrigger(myTrigger);

            //
            // Specify the background task to run when the trigger fires.
            //
            myTaskBuilder.taskEntryPoint = sampleJavaScriptBackgroundTaskEntryPoint;

            //
            // Name the background task.
            //
            myTaskBuilder.name = sampleJavaScriptBackgroundTaskName;

            //
            // Register the background task.
            //
            var myTask = myTaskBuilder.register();

            //
            // Associate progress and completed event handlers with the new background task.
            //
            myTask.addEventListener("progress", new ProgressHandler(myTask).onProgress);
            myTask.addEventListener("completed", new CompleteHandler(myTask).onCompleted);

            updateSampleJavaScriptBackgroundTaskUIState(true);

        } catch (ex) {
            WinJS.log && WinJS.log(ex.name + " : " + ex.description, "sample", "error");

            //
            // Clean up any tasks that were potentially created.
            //
            unregisterSampleJavaScriptBackgroundTask();
        }
    }

    //
    // Device access error callback
    //
    function smsDeviceErrorCallback(error) {
        WinJS.log && WinJS.log("Failed to find SMS device\n" + error.name + " : " + error.description, "sample", "error");
    }

    //
    // Device access callback function
    //
    function smsDeviceReceived(smsDeviceResult) {
        WinJS.log && WinJS.log("Successfully connected to SMS device with account number: " + smsDeviceResult.accountPhoneNumber, "sample", "status");
        hasDeviceAccess = true;

        // Since device is accessible go ahead with the background task registration.
        registerBackgroundTask();
    }

    //
    // Handle Register button click.
    //
    function registerSampleJavaScriptBackgroundTask() {

        //
        // Before registering a background task for the first time, attempt to access a SMS device.
        // This will prompt for user consent for this app to use SMS, if it has not already been granted.
        // If user declines, the app will not be able to call a GetMessageAsync to read the SMS message.
        //
        if (!hasDeviceAccess) {
            var smsDeviceOperation = Windows.Devices.Sms.SmsDevice.getDefaultAsync();
            smsDeviceOperation.done(smsDeviceReceived, smsDeviceErrorCallback);
        }
        else {
            registerBackgroundTask();
        }
    }

    //
    // Handle Unregister button click.
    //
    function unregisterSampleJavaScriptBackgroundTask() {

        var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
        var hascur = iter.hasCurrent;
        while (hascur) {
            var cur = iter.current.value;
            if (cur.name === sampleJavaScriptBackgroundTaskName) {
                cur.unregister(true);
            }
            hascur = iter.moveNext();
        }
        updateSampleJavaScriptBackgroundTaskUIState(false);
    }

    //
    // Updates button text in the sample application UI.
    //
    function updateSampleJavaScriptBackgroundTaskUIState(registered) {
        if (registered) {
            document.getElementById("sampleJavaScriptBackgroundTaskStatus").innerText = "Registered for SMS text message background event";
            document.getElementById("registerSampleJavaScriptBackgroundTask").disabled = true;
            document.getElementById("unregisterSampleJavaScriptBackgroundTask").disabled = false;

        } else {
            document.getElementById("sampleJavaScriptBackgroundTaskStatus").innerText = "Unregistered for SMS text message background event";
            document.getElementById("registerSampleJavaScriptBackgroundTask").disabled = false;
            document.getElementById("unregisterSampleJavaScriptBackgroundTask").disabled = true;
        }
    }
})();
