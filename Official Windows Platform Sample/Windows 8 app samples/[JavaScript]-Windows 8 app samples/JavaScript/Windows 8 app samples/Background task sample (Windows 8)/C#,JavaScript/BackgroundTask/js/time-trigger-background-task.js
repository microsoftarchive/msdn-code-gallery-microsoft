//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/time-trigger-background-task.html", {
        ready: function (element, options) {
            document.getElementById("registerTimeTriggerBackgroundTaskButton").addEventListener("click", registerTimeTriggerBackgroundTask, false);
            document.getElementById("unregisterTimeTriggerBackgroundTaskButton").addEventListener("click", unregisterTimeTriggerBackgroundTask, false);
            TimeTriggerBackgroundTask.updateUI();
        }
    });

    function registerTimeTriggerBackgroundTask() {

        //
        // Time triggered tasks can only run when the application is on the lock screen.
        // Time triggered tasks can be registered even if the application is not on the lockscreen.
        // 
        Windows.ApplicationModel.Background.BackgroundExecutionManager.requestAccessAsync();

        BackgroundTaskSample.registerBackgroundTask(BackgroundTaskSample.sampleBackgroundTaskEntryPoint,
                                                    BackgroundTaskSample.timeTriggerTaskName,
                                                    new Windows.ApplicationModel.Background.TimeTrigger(15, false),
                                                    null);
        TimeTriggerBackgroundTask.updateUI();
    }

    function unregisterTimeTriggerBackgroundTask() {
        BackgroundTaskSample.unregisterBackgroundTasks(BackgroundTaskSample.timeTriggerTaskName);
        TimeTriggerBackgroundTask.updateUI();
    }
})();

var TimeTriggerBackgroundTask = {
    "updateUI": function () {
        try {
            var registerButton = document.getElementById("registerTimeTriggerBackgroundTaskButton");
            var unregisterButton = document.getElementById("unregisterTimeTriggerBackgroundTaskButton");
            var taskProgress = document.getElementById("timeTriggerTaskProgress");
            var taskStatus = document.getElementById("timeTriggerTaskStatus");

            registerButton && (registerButton.disabled = BackgroundTaskSample.timeTriggerTaskRegistered);
            unregisterButton && (unregisterButton.disabled = !BackgroundTaskSample.timeTriggerTaskRegistered);
            taskProgress && (taskProgress.innerText = BackgroundTaskSample.timeTriggerTaskProgress);
            taskStatus && (taskStatus.innerText = BackgroundTaskSample.getBackgroundTaskStatus(BackgroundTaskSample.timeTriggerTaskName));
        } catch (ex) {

        }
    }
};
