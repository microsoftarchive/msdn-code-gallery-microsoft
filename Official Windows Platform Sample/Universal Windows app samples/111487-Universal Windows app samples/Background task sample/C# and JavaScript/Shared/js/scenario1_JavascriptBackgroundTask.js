//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_JavascriptBackgroundTask.html", {
        ready: function (element, options) {
            document.getElementById("registerJavaScriptBackgroundTaskButton").addEventListener("click", registerJavaScriptBackgroundTask, false);
            document.getElementById("unregisterJavaScriptBackgroundTaskButton").addEventListener("click", unregisterJavaScriptBackgroundTask, false);
            JavaScriptBackgroundTask.updateUI();
        }
    });

    function registerJavaScriptBackgroundTask() {
        BackgroundTaskSample.registerBackgroundTask(BackgroundTaskSample.javaScriptBackgroundTaskEntryPoint,
                                                    BackgroundTaskSample.javaScriptBackgroundTaskName,
                                                    new Windows.ApplicationModel.Background.SystemTrigger(Windows.ApplicationModel.Background.SystemTriggerType.timeZoneChange, false),
                                                    null);
        JavaScriptBackgroundTask.updateUI();
    }

    function unregisterJavaScriptBackgroundTask() {
        BackgroundTaskSample.unregisterBackgroundTasks(BackgroundTaskSample.javaScriptBackgroundTaskName);
        JavaScriptBackgroundTask.updateUI();
    }
})();

var JavaScriptBackgroundTask = {
    "updateUI": function () {
        try {
            var registerButton = document.getElementById("registerJavaScriptBackgroundTaskButton");
            var unregisterButton = document.getElementById("unregisterJavaScriptBackgroundTaskButton");
            var taskProgress = document.getElementById("javaScriptBackgroundTaskProgress");
            var taskStatus = document.getElementById("javaScriptBackgroundTaskStatus");

            registerButton && (registerButton.disabled = BackgroundTaskSample.javaScriptBackgroundTaskRegistered);
            unregisterButton && (unregisterButton.disabled = !BackgroundTaskSample.javaScriptBackgroundTaskRegistered);
            taskProgress && (taskProgress.innerText = BackgroundTaskSample.javaScriptBackgroundTaskProgress);
            taskStatus && (taskStatus.innerText = BackgroundTaskSample.getBackgroundTaskStatus(BackgroundTaskSample.javaScriptBackgroundTaskName));
        } catch (ex) {

        }
    }
};
