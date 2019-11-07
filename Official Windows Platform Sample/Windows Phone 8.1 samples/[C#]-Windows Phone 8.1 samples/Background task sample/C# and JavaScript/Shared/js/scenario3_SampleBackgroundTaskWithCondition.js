//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3_SampleBackgroundTaskWithCondition.html", {
        ready: function (element, options) {
            document.getElementById("registerSampleBackgroundTaskWithConditionButton").addEventListener("click", registerSampleBackgroundTaskWithCondition, false);
            document.getElementById("unregisterSampleBackgroundTaskWithConditionButton").addEventListener("click", unregisterSampleBackgroundTaskWithCondition, false);
            SampleBackgroundTaskWithCondition.updateUI();
        }
    });

    function registerSampleBackgroundTaskWithCondition() {
        BackgroundTaskSample.registerBackgroundTask(BackgroundTaskSample.sampleBackgroundTaskEntryPoint,
                                                    BackgroundTaskSample.sampleBackgroundTaskWithConditionName,
                                                    new Windows.ApplicationModel.Background.SystemTrigger(Windows.ApplicationModel.Background.SystemTriggerType.timeZoneChange, false),
                                                    new Windows.ApplicationModel.Background.SystemCondition(Windows.ApplicationModel.Background.SystemConditionType.internetAvailable));
        SampleBackgroundTaskWithCondition.updateUI();
    }

    function unregisterSampleBackgroundTaskWithCondition() {
        BackgroundTaskSample.unregisterBackgroundTasks(BackgroundTaskSample.sampleBackgroundTaskWithConditionName);
        SampleBackgroundTaskWithCondition.updateUI();
    }
})();

var SampleBackgroundTaskWithCondition = {
    "updateUI": function () {
        try {
            var registerButton = document.getElementById("registerSampleBackgroundTaskWithConditionButton");
            var unregisterButton = document.getElementById("unregisterSampleBackgroundTaskWithConditionButton");
            var taskProgress = document.getElementById("sampleBackgroundTaskWithConditionProgress");
            var taskStatus = document.getElementById("sampleBackgroundTaskWithConditionStatus");

            registerButton && (registerButton.disabled = BackgroundTaskSample.sampleBackgroundTaskWithConditionRegistered);
            unregisterButton && (unregisterButton.disabled = !BackgroundTaskSample.sampleBackgroundTaskWithConditionRegistered);
            taskProgress && (taskProgress.innerText = BackgroundTaskSample.sampleBackgroundTaskWithConditionProgress);
            taskStatus && (taskStatus.innerText = BackgroundTaskSample.getBackgroundTaskStatus(BackgroundTaskSample.sampleBackgroundTaskWithConditionName));
        } catch (ex) {

        }
    }
};
