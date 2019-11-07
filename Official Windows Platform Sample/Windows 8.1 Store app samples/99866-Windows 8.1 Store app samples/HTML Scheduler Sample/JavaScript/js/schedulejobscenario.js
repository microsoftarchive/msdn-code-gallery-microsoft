//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/schedulejobscenario.html", {
        ready: function (element, options) {
            document.getElementById("scheduleButton").addEventListener("click", scheduleJobs, false);
        }
    });

    function scheduleJobs() {
        window.output("\nScheduling Jobs...");
        var S = WinJS.Utilities.Scheduler;

        // schedule some work
        S.schedule(function () {
            window.output("Running job at aboveNormal priority");
        }, S.Priority.aboveNormal);
        window.output("Scheduled job at aboveNormal priority");

        S.schedule(function () {
            window.output("Running job at idle priority");
        }, S.Priority.idle, this);
        window.output("Scheduled job at idle priority");

        S.schedule(function () {
            window.output("Running job at belowNormal priority");
        }, S.Priority.belowNormal);
        window.output("Scheduled job at belowNormal priority");

        S.schedule(function () {
            window.output("Running job at normal priority");
        }, S.Priority.normal);
        window.output("Scheduled job at normal priority");

        S.schedule(function () {
            window.output("Running job at high priority");
        }, S.Priority.high);
        window.output("Scheduled job at high priority");

        window.output("Finished Scheduling Jobs\n");
    }
})();
