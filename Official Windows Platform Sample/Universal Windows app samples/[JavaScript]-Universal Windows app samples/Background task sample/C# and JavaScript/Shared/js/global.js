//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var BackgroundTaskSample = {

    "sampleBackgroundTaskEntryPoint": "Tasks.SampleBackgroundTask",
    "sampleBackgroundTaskName": "SampleBackgroundTask",
    "sampleBackgroundTaskProgress": "",
    "sampleBackgroundTaskRegistered": false,

    "sampleBackgroundTaskWithConditionEntryPoint": "Tasks.SampleBackgroundTask",
    "sampleBackgroundTaskWithConditionName": "SampleBackgroundTaskWithCondition",
    "sampleBackgroundTaskWithConditionProgress": "",
    "sampleBackgroundTaskWithConditionRegistered": false,

    "servicingCompleteTaskEntryPoint": "Tasks.ServicingComplete",
    "servicingCompleteTaskName": "ServicingCompleteTask",
    "servicingCompleteTaskProgress": "",
    "servicingCompleteTaskRegistered": false,

    "javaScriptBackgroundTaskEntryPoint": "js\\backgroundtask.js",
    "javaScriptBackgroundTaskName": "SampleJavaScriptBackgroundTask",
    "javaScriptBackgroundTaskProgress": "",
    "javaScriptBackgroundTaskRegistered": false,

    "timeTriggerTaskEntryPoint": "Tasks.SampleBackgroundTask",
    "timeTriggerTaskName": "TimeTriggerTask",
    "timeTriggerTaskProgress": "",
    "timeTriggerTaskRegistered": false,

    //
    // Register a background task with the specified taskEntryPoint, taskName, trigger,
    // and condition (optional).
    //
    "registerBackgroundTask": function (taskEntryPoint, taskName, trigger, condition) {
        if (BackgroundTaskSamplePlatformSpecific.taskRequiresBackgroundAccess(taskName)) {
            Windows.ApplicationModel.Background.BackgroundExecutionManager.requestAccessAsync();
        }

        var builder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

        builder.name = taskName;
        builder.taskEntryPoint = taskEntryPoint;
        builder.setTrigger(trigger);

        if (condition !== null) {
            builder.addCondition(condition);

            //
            // If the condition changes while the background task is executing then it will
            // be canceled.
            //
            builder.cancelOnConditionLoss = true;
        }

        var task = builder.register();

        BackgroundTaskSample.attachProgressAndCompletedHandlers(task);

        BackgroundTaskSample.updateBackgroundTaskStatus(taskName, true);

        //
        // Remove previous completion status from local settings.
        //
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        settings.values.remove(taskName);
    },

    //
    // Unregister all background tasks with given name.
    //
    "unregisterBackgroundTasks": function (taskName) {
        //
        // Loop through all background tasks and unregister any with SampleBackgroundTaskName or
        // SampleBackgroundTaskWithConditionName or timeTriggerTaskName.
        //
        var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
        var hascur = iter.hasCurrent;
        while (hascur) {
            var cur = iter.current.value;
            if (cur.name === taskName) {
                cur.unregister(true);
                BackgroundTaskSample.updateBackgroundTaskStatus(taskName, false);
            }
            hascur = iter.moveNext();
        }
    },

    //
    // Associate progress and completed event handlers with the  background task.
    //
    "attachProgressAndCompletedHandlers": function (task) {
        task.addEventListener("progress", new BackgroundTaskSample.progressHandler(task).onProgress);
        task.addEventListener("completed", new BackgroundTaskSample.completeHandler(task).onCompleted);
    },

    //
    // Keep track of which background tasks are registered in a global variable.
    //
    "updateBackgroundTaskStatus": function (taskName, registered) {

        switch (taskName) {
            case BackgroundTaskSample.sampleBackgroundTaskName:
                BackgroundTaskSample.sampleBackgroundTaskRegistered = registered;
                break;
            case BackgroundTaskSample.sampleBackgroundTaskWithConditionName:
                BackgroundTaskSample.sampleBackgroundTaskWithConditionRegistered = registered;
                break;
            case BackgroundTaskSample.servicingCompleteTaskName:
                BackgroundTaskSample.servicingCompleteTaskRegistered = registered;
                break;
            case BackgroundTaskSample.javaScriptBackgroundTaskName:
                BackgroundTaskSample.javaScriptBackgroundTaskRegistered = registered;
                break;
            case BackgroundTaskSample.timeTriggerTaskName:
                BackgroundTaskSample.timeTriggerTaskRegistered = registered;
                break;
        }
    },

    "getBackgroundTaskStatus": function (taskName) {

        var registered = false;
        switch (taskName) {
            case BackgroundTaskSample.sampleBackgroundTaskName:
                registered = BackgroundTaskSample.sampleBackgroundTaskRegistered;
                break;
            case BackgroundTaskSample.sampleBackgroundTaskWithConditionName:
                registered = BackgroundTaskSample.sampleBackgroundTaskWithConditionRegistered;
                break;
            case BackgroundTaskSample.servicingCompleteTaskName:
                registered = BackgroundTaskSample.servicingCompleteTaskRegistered;
                break;
            case BackgroundTaskSample.javaScriptBackgroundTaskName:
                registered = BackgroundTaskSample.javaScriptBackgroundTaskRegistered;
                break;
            case BackgroundTaskSample.timeTriggerTaskName:
                registered = BackgroundTaskSample.timeTriggerTaskRegistered;
                break;
        }

        var backgroundTaskStatus = registered ? "Registered" : "Unregistered";
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        if (settings.values.hasKey(taskName)) {
            backgroundTaskStatus += " - " + settings.values[taskName];
        }

        return backgroundTaskStatus;
    },

    "initializeBackgroundTaskState": function () {
        //
        // Associate JS event handlers with background task progress and completion events.
        //
        var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
        var hascur = iter.hasCurrent;
        while (hascur) {
            var cur = iter.current.value;
            BackgroundTaskSample.attachProgressAndCompletedHandlers(cur);
            BackgroundTaskSample.updateBackgroundTaskStatus(cur.name, true);
            hascur = iter.moveNext();
        }
    },

    //
    // Handle background task progress.
    //
    "progressHandler": function (task) {
        this.onProgress = function (args) {
            try {
                var settings = Windows.Storage.ApplicationData.current.localSettings;
                var progress = "Progress: " + args.progress + "%";

                switch (task.name) {
                    case BackgroundTaskSample.sampleBackgroundTaskName:
                        BackgroundTaskSample.sampleBackgroundTaskProgress = progress;
                        SampleBackgroundTask.updateUI();
                        break;
                    case BackgroundTaskSample.sampleBackgroundTaskWithConditionName:
                        BackgroundTaskSample.sampleBackgroundTaskWithConditionProgress = progress;
                        SampleBackgroundTaskWithCondition.updateUI();
                        break;
                    case BackgroundTaskSample.servicingCompleteTaskName:
                        BackgroundTaskSample.servicingCompleteTaskProgress = progress;
                        ServicingComplete.updateUI();
                        break;
                    case BackgroundTaskSample.javaScriptBackgroundTaskName:
                        BackgroundTaskSample.javaScriptBackgroundTaskProgress = progress;
                        JavaScriptBackgroundTask.updateUI();
                        break;
                    case BackgroundTaskSample.timeTriggerTaskName:
                        BackgroundTaskSample.timeTriggerTaskProgress = progress;
                        TimeTriggerBackgroundTask.updateUI();
                        break;
                }
            } catch (ex) {
            }
        };
    },

    //
    // Handle background task completion.
    //
    "completeHandler": function (task) {
        this.onCompleted = function (args) {
            try {
                switch (task.name) {
                    case BackgroundTaskSample.sampleBackgroundTaskName:
                        SampleBackgroundTask.updateUI();
                        break;
                    case BackgroundTaskSample.sampleBackgroundTaskWithConditionName:
                        SampleBackgroundTaskWithCondition.updateUI();
                        break;
                    case BackgroundTaskSample.servicingCompleteTaskName:
                        ServicingComplete.updateUI();
                        break;
                    case BackgroundTaskSample.javaScriptBackgroundTaskName:
                        JavaScriptBackgroundTask.updateUI();
                        break;
                    case BackgroundTaskSample.timeTriggerTaskName:
                        TimeTriggerBackgroundTask.updateUI();
                        break;
                }
            } catch (ex) {
            }
        };
    }
};
