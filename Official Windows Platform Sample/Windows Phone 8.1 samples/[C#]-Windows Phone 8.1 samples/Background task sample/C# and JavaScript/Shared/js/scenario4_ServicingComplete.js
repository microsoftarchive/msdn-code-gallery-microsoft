//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4_ServicingComplete.html", {
        ready: function (element, options) {
            document.getElementById("registerServicingCompleteTaskButton").addEventListener("click", registerServicingCompleteTask, false);
            document.getElementById("unregisterServicingCompleteTaskButton").addEventListener("click", unregisterServicingCompleteTask, false);
            ServicingComplete.updateUI();
        }
    });

    function registerServicingCompleteTask() {
        BackgroundTaskSample.registerBackgroundTask(BackgroundTaskSample.servicingCompleteTaskEntryPoint,
                                                    BackgroundTaskSample.servicingCompleteTaskName,
                                                    new Windows.ApplicationModel.Background.SystemTrigger(Windows.ApplicationModel.Background.SystemTriggerType.servicingComplete, false),
                                                    null);
        ServicingComplete.updateUI();
    }

    function unregisterServicingCompleteTask() {
        BackgroundTaskSample.unregisterBackgroundTasks(BackgroundTaskSample.servicingCompleteTaskName);
        ServicingComplete.updateUI();
    }    
})();

var ServicingComplete = {
    "updateUI": function () {
        try {
            var registerButton = document.getElementById("registerServicingCompleteTaskButton");
            var unregisterButton = document.getElementById("unregisterServicingCompleteTaskButton");            
            var taskProgress = document.getElementById("servicingCompleteProgress");
            var taskStatus = document.getElementById("servicingCompleteStatus");

            registerButton && (registerButton.disabled = BackgroundTaskSample.servicingCompleteTaskRegistered);
            unregisterButton && (unregisterButton.disabled = !BackgroundTaskSample.servicingCompleteTaskRegistered);
            taskProgress && (taskProgress.innerText = BackgroundTaskSample.servicingCompleteTaskProgress);
            taskStatus && (taskStatus.innerText = BackgroundTaskSample.getBackgroundTaskStatus(BackgroundTaskSample.servicingCompleteTaskName));
        } catch (ex) {

        }
    }
};
