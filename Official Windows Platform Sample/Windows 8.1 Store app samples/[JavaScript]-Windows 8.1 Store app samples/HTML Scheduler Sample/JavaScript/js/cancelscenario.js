//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/cancelscenario.html", {
        ready: function (element, options) {
            document.getElementById("cancelButton").addEventListener("click", cancelJobs, false);
            document.getElementById("cancelByOwnerButton").addEventListener("click", cancelJobsByOwner, false);
        }
    });

    var S = WinJS.Utilities.Scheduler;

    function cancelJobs() {
        window.output("\nScheduling Jobs...");

        // schedule some work
        var job1 = S.schedule(function () {
            window.output("Running job1");
        }, S.Priority.normal);
        window.output("Scheduled job1");

        // schedule some work
        var job2 = S.schedule(function () {
            window.output("Running job2");
        }, S.Priority.normal);
        window.output("Scheduled job2");

        window.output("Canceling job1");
        job1.cancel();
    }

    function cancelJobsByOwner() {
        window.output("\nScheduling Jobs...");

        var ownerObject1 = S.createOwnerToken();
        var ownerObject2 = S.createOwnerToken();
        var ownerObject3 = S.createOwnerToken();

        // schedule some work
        var job1 = S.schedule(function () {
            window.output("Running job1 with owner1");
        }, S.Priority.normal);
        job1.owner = ownerObject1;
        window.output("Scheduled job1 with owner1");

        // schedule some work
        var job2 = S.schedule(function () {
            window.output("Running job2 with owner2");
        }, S.Priority.normal);
        job2.owner = ownerObject2;
        window.output("Scheduled job2 with owner2");

        // schedule some work
        var job3 = S.schedule(function () {
            window.output("Running job3 with owner1");
        }, S.Priority.normal);
        job3.owner = ownerObject1;
        window.output("Scheduled job3 with owner1");

        // schedule some work
        var job4 = S.schedule(function () {
            window.output("Running job4 with owner3");
        }, S.Priority.normal);
        job4.owner = ownerObject3;
        window.output("Scheduled job4 with owner3");

        window.output("Canceling jobs with owner1");
        ownerObject1.cancelAll();
    }
})();
