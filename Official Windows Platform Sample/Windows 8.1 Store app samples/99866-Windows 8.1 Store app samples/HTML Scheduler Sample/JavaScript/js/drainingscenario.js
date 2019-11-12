//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/drainingscenario.html", {
        ready: function (element, options) {
            document.getElementById("drainHigh").addEventListener("click", drainHigh, false);
            document.getElementById("drainBelowNormal").addEventListener("click", drainBelowNormal, false);
        }
    });

    var S = WinJS.Utilities.Scheduler;

    function drainHigh() {
        drainToPriority(S.Priority.high, "high");
    }

    function drainBelowNormal() {
        drainToPriority(S.Priority.belowNormal, "belowNormal");
    }

    function drainToPriority(priority, priorityName) {
        window.output("\nScheduling Jobs...");

        // schedule some work
        S.schedule(function () {
            window.output("Running job1 at normal priority");
        }, S.Priority.normal);
        window.output("Scheduled job1 at normal priority");

        S.schedule(function () {
            window.output("Running job2 at high priority");
        }, S.Priority.high);
        window.output("Scheduled job2 at high priority");

        S.schedule(function () {
            window.output("Running job3 at idle priority");
        }, S.Priority.idle);
        window.output("Scheduled job3 at idle priority");

        S.schedule(function () {
            window.output("Running job4 at belowNormal priority");
        }, S.Priority.belowNormal);
        window.output("Scheduled job4 at belowNormal priority");

        S.schedule(function () {
            window.output("Running job5 at normal priority");
        }, S.Priority.normal);
        window.output("Scheduled job5 at normal priority");


        window.output("Draining scheduler to " + priorityName + " priority");
        S.requestDrain(priority).done(function () {
            window.output("Done draining");
        }
        );
    }
})();
