//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var pageLocation = "/html/yieldingscenario.html";

    WinJS.UI.Pages.define(pageLocation, {
        ready: function (element, options) {
            document.getElementById("execute").addEventListener("click", executeYieldingTask, false);
            document.getElementById("addTask").addEventListener("click", addTask, false);
            document.getElementById("completeTask").addEventListener("click", completeTasks, false);
            WinJS.Navigation.addEventListener("beforenavigate", beforeNavigate);
        }
    });

    var S = WinJS.Utilities.Scheduler;
    var taskCompleted = false;
    var highPriorityTask;
    var normalPriorityTask;

    function beforeNavigate(eventInfo) {
        // if we're navigating away from this page to another page, cancel the pending task
        if (WinJS.Navigation.location === pageLocation) {
            completeTasks();
            return;
        }

        // if we're navigating to this page, reset taskCompleted
        if (eventInfo.detail.location === pageLocation) {
            taskCompleted = false;
            return;
        }
    }

    function completeTasks() {
        window.output("Completing yielding task.");
        taskCompleted = true;
        if (highPriorityTask) {
            highPriorityTask.cancel();
        }
        if (normalPriorityTask) {
            normalPriorityTask.cancel();
        }
    }

    function addTask() {
        window.output("\nScheduling higher priority jobs");

        // schedule some work
        highPriorityTask = S.schedule(function () {
            window.output("Running job at high priority");
        }, S.Priority.high);
        window.output("Scheduled job at high priority");

        // schedule some work
        normalPriorityTask = S.schedule(function () {
            window.output("Running job at normal priority");
        }, S.Priority.normal);
        window.output("Scheduled job at normal priority");
    }

    function executeYieldingTask() {
        document.getElementById("addTask").disabled = false;
        document.getElementById("completeTask").disabled = false;
        document.getElementById("execute").disabled = true;

        // schedule some work
        S.schedule(function worker(jobInfo) {
            while (!taskCompleted) {
                if (jobInfo.shouldYield) {
                    // not finished, run this function again
                    window.output("Yielding and putting idle job back on scheduler.");
                    jobInfo.setWork(worker);
                    break;
                }
                else {
                    window.output("Running idle yielding job...");
                    var start = performance.now();
                    while (performance.now() < (start + 2000)) {
                        // do nothing;
                    }
                }
            }

            if (taskCompleted) {
                window.output("Completed yielding task.");
                var addTaskButton = document.getElementById("addTask");
                if (addTaskButton) {
                    addTaskButton.disabled = true;
                }
                var completeTaskButton = document.getElementById("completeTask");
                if (completeTaskButton) {
                    completeTaskButton.disabled = true;
                }
                var executeButton = document.getElementById("execute");
                if (executeButton) {
                    executeButton.disabled = false;
                }
                taskCompleted = false;
            }
        }, S.Priority.idle);

        window.output("Scheduled yielding job at idle priority");        
    }
})();
