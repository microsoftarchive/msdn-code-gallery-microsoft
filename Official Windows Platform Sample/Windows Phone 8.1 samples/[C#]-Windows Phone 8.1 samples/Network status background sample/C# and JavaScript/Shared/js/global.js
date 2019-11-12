//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var BackgroundTaskSample = {

    "sampleBackgroundTaskEntryPoint": "NetworkStatusTask.NetworkStatusBackgroundTask",
    "sampleBackgroundTaskWithConditionName": "SampleBackgroundTaskWithCondition",
    "sampleBackgroundTaskWithConditionRegistered": false,
    "internetProfile": "Not connected to Internet",
    "networkAdapter": "Not connected to Internet",
    
    //
    // Register a background task with the specified taskEntryPoint, taskName, trigger,
    // and condition (optional).
    //
    "registerBackgroundTask": function (taskEntryPoint, taskName, trigger, condition) {
        var builder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

        builder.name = taskName;
        builder.taskEntryPoint = taskEntryPoint;
        builder.setTrigger(trigger);

        if (condition !== null) {
            builder.addCondition(condition);
        }

        var task = builder.register();

        BackgroundTaskSample.attachCompletedHandlers(task);

        BackgroundTaskSample.updateBackgroundTaskStatus(taskName, true);
    },

    //
    // Unregister all background tasks with given name.
    //
    "unregisterBackgroundTasks": function (taskName) {
        //
        // Loop through all background tasks and unregister any with 
        // SampleBackgroundTaskWithConditionName.
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
    // Associate completed event handler with the  background task.
    //
    "attachCompletedHandlers": function (task) {
        task.addEventListener("completed", new BackgroundTaskSample.completeHandler(task).onCompleted);
    },

    //
    // Keep track of which background tasks are registered in a global variable.
    //
    "updateBackgroundTaskStatus": function (taskName, registered) {
        switch (taskName) {
            case BackgroundTaskSample.sampleBackgroundTaskWithConditionName:
                BackgroundTaskSample.sampleBackgroundTaskWithConditionRegistered = registered;
                break;
        }
    },

    "initializeBackgroundTaskState": function () {
        //
        // Associate JS event handlers with background task progress and completion events.
        //
        var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
        var hascur = iter.hasCurrent;
        while (hascur) {
            var cur = iter.current.value;
            BackgroundTaskSample.attachCompletedHandlers(cur);
            BackgroundTaskSample.updateBackgroundTaskStatus(cur.name, true);
            hascur = iter.moveNext();
        }
    },


    //
    // Handle background task completion.
    //
    "completeHandler": function (task) {
        this.onCompleted = function (args) {
            try {
                var localSettings = Windows.Storage.ApplicationData.current.localSettings;
                var profile = localSettings.values["InternetProfile"];
                var adapter = localSettings.values["NetworkAdapterId"];

                var hasNewConnectionCost = localSettings.values["HasNewConnectionCost"];
                var hasNewDomainConnectivityLevel = localSettings.values["HasNewDomainConnectivityLevel"];
                var hasNewHostNameList = localSettings.values["HasNewHostNameList"];
                var hasNewInternetConnectionProfile = localSettings.values["HasNewInternetConnectionProfile"];
                var hasNewNetworkConnectivityLevel = localSettings.values["HasNewNetworkConnectivityLevel"];

                if ((profile !== null) && (adapter !== null))
                {
                    //If internet profile has changed, display the new internet profile
                    if ((profile !== BackgroundTaskSample.internetProfile) || (adapter !== BackgroundTaskSample.networkAdapter)) {
                        BackgroundTaskSample.internetProfile = profile.toString();
                        BackgroundTaskSample.networkAdapter = adapter.toString();

                        var outputString = "Internet Profile changed\n" + "=================\n" + "Current Internet Profile : " + BackgroundTaskSample.internetProfile + "\n\n";

                        if (hasNewConnectionCost !== undefined) {
                            outputString += hasNewConnectionCost.toString() + "\n";
                        }
                        if (hasNewDomainConnectivityLevel !== undefined) {
                            outputString += hasNewDomainConnectivityLevel.toString() + "\n";
                        }
                        if (hasNewHostNameList !== undefined) {
                            outputString += hasNewHostNameList.toString() + "\n";
                        }
                        if (hasNewInternetConnectionProfile !== undefined) {
                            outputString += hasNewInternetConnectionProfile.toString() + "\n";
                        }
                        if (hasNewNetworkConnectivityLevel !== undefined) {
                            outputString += hasNewNetworkConnectivityLevel.toString() + "\n";
                        }

                        WinJS.log && WinJS.log(outputString, "sample", "status");
                    }
                }
            }
            catch (ex) {
                WinJS.log && WinJS.log(ex, "sample", "error");
            }
        };
    }
};
