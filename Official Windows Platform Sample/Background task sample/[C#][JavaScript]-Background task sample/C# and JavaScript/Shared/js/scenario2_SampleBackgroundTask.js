//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_SampleBackgroundTask.html", {
        ready: function (element, options) {
            document.getElementById("registerSampleBackgroundTaskButton").addEventListener("click", registerSampleBackgroundTask, false);
            document.getElementById("unregisterSampleBackgroundTaskButton").addEventListener("click", unregisterSampleBackgroundTask, false);
            SampleBackgroundTask.updateUI();
        }
    });

    function registerSampleBackgroundTask() {
        BackgroundTaskSample.registerBackgroundTask(BackgroundTaskSample.sampleBackgroundTaskEntryPoint,
                                                    BackgroundTaskSample.sampleBackgroundTaskName,
                                                    new Windows.ApplicationModel.Background.SystemTrigger(Windows.ApplicationModel.Background.SystemTriggerType.timeZoneChange, false),
                                                    null);
        SampleBackgroundTask.updateUI();
    }

    function unregisterSampleBackgroundTask() {
        BackgroundTaskSample.unregisterBackgroundTasks(BackgroundTaskSample.sampleBackgroundTaskName);
        SampleBackgroundTask.updateUI();
    }
})();

var SampleBackgroundTask = {
    "updateUI": function () {
        try {
            var registerButton = document.getElementById("registerSampleBackgroundTaskButton");
            var unregisterButton = document.getElementById("unregisterSampleBackgroundTaskButton");
            var taskProgress = document.getElementById("sampleBackgroundTaskProgress");
            var taskStatus = document.getElementById("sampleBackgroundTaskStatus");

            registerButton && (registerButton.disabled = BackgroundTaskSample.sampleBackgroundTaskRegistered);
            unregisterButton && (unregisterButton.disabled = !BackgroundTaskSample.sampleBackgroundTaskRegistered);
            taskProgress && (taskProgress.innerText = BackgroundTaskSample.sampleBackgroundTaskProgress);
            taskStatus && (taskStatus.innerText = BackgroundTaskSample.getBackgroundTaskStatus(BackgroundTaskSample.sampleBackgroundTaskName));
        } catch (ex) {

        }
    }
};
