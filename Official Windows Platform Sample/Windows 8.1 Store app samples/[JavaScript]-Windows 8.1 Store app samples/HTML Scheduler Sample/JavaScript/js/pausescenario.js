//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var pageLocation = "/html/pausescenario.html";

    WinJS.UI.Pages.define(pageLocation, {
        ready: function (element, options) {
            document.getElementById("pauseButton").addEventListener("click", pauseTask, false);
            WinJS.Navigation.addEventListener("beforenavigate", beforeNavigate);
        }
    });

    var S = WinJS.Utilities.Scheduler;
    var taskPaused = false;
    var job1;

    function beforeNavigate(eventInfo) {
        // if we're navigating away from this page to another page, resume the paused task
        if (WinJS.Navigation.location === pageLocation) {
            if (taskPaused) {
                pauseTask();
            }
        }
    }

    function pauseTask() {
        if (!taskPaused) {
            window.output("\nScheduling Jobs...");

            // schedule some work
            job1 = S.schedule(function () {
                window.output("Running job1");
            }, S.Priority.normal);
            window.output("Scheduled job1");

            var job2 = S.schedule(function () {
                window.output("Running job2");
            }, S.Priority.normal);
            window.output("Scheduled job2");

            var job3 = S.schedule(function () {
                window.output("Running job3");
            }, S.Priority.normal);
            window.output("Scheduled job3");

            window.output("\nPausing job1...");
            job1.pause();

            taskPaused = true;
            document.getElementById("pauseButton").innerText = "Resume job";
        }
        else {
            window.output("\Resuming Job1...");
            if (job1) {
                job1.resume();
            }
            taskPaused = false;
            document.getElementById("pauseButton").innerText = "Create and pause a job";
        }
    }
})();
