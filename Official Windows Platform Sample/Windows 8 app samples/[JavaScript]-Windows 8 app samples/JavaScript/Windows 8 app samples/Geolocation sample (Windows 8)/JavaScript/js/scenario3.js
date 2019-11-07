//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var Background = Windows.ApplicationModel.Background;

    var geolocTask;
    var promise;
    var pageLoaded = false;
    var sampleBackgroundTaskName = "SampleLocationBackgroundTask";
    var sampleBackgroundTaskEntryPoint = "js\\backgroundtask.js";

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("registerBackgroundTaskButton").addEventListener("click", registerBackgroundTask, false);
            document.getElementById("unregisterBackgroundTaskButton").addEventListener("click", unregisterBackgroundTask, false);

            // Loop through all background tasks to see if our task is already registered
            var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
            var hascur = iter.hasCurrent;
            while (hascur) {
                var cur = iter.current.value;
                if (cur.name === sampleBackgroundTaskName) {
                    geolocTask = cur;
                    break;
                }
                hascur = iter.moveNext();
            }

            if (geolocTask) {
                // Associate an event handler to the existing background task
                geolocTask.addEventListener("completed", onCompleted);

                var backgroundAccessStatus = Background.BackgroundExecutionManager.getAccessStatus();
                switch (backgroundAccessStatus)
                {
                case Background.BackgroundAccessStatus.unspecified:
                case Background.BackgroundAccessStatus.denied:
                    WinJS.log && WinJS.log("This application must be added to the lock screen before the background task will run.", "sample", "status");
                    break;

                default:
                    WinJS.log && WinJS.log("Background task is already registered. Waiting for next update...", "sample", "status");
                    break;
                }

                document.getElementById("registerBackgroundTaskButton").disabled = true;
                document.getElementById("unregisterBackgroundTaskButton").disabled = false;
            } else {
                document.getElementById("registerBackgroundTaskButton").disabled = false;
                document.getElementById("unregisterBackgroundTaskButton").disabled = true;
            }

            pageLoaded = true;
        },
        unload: function () {
            pageLoaded = false;
            
            if (promise) {
                promise.operation.cancel();
            }

            if (geolocTask) {
                geolocTask.removeEventListener("completed", onCompleted);
            }
        }
    });

    // Handle background task completion
    function onCompleted() {
        try {
            var settings = Windows.Storage.ApplicationData.current.localSettings;

            WinJS.log && WinJS.log(settings.values["Status"], "sample", "status");

            document.getElementById("latitude").innerText = settings.values["Latitude"];
            document.getElementById("longitude").innerText = settings.values["Longitude"];
            document.getElementById("accuracy").innerText = settings.values["Accuracy"];
        } catch (ex) {
            WinJS.log && WinJS.log(ex.toString(), "sample", "error");
        }
    }

    function registerBackgroundTask() {
        // Request lockscreen access
        Background.BackgroundExecutionManager.requestAccessAsync().done(
            function (backgroundAccessStatus) {
                var builder =  new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

                // Register the background task
                builder.name = sampleBackgroundTaskName;
                builder.taskEntryPoint = sampleBackgroundTaskEntryPoint;
                builder.setTrigger(new Windows.ApplicationModel.Background.TimeTrigger(15, false));

                geolocTask = builder.register();

                geolocTask.addEventListener("completed", onCompleted);

                document.getElementById("registerBackgroundTaskButton").disabled = true;
                document.getElementById("unregisterBackgroundTaskButton").disabled = false;

                switch (backgroundAccessStatus) {
                    case Background.BackgroundAccessStatus.unspecified:
                    case Background.BackgroundAccessStatus.denied:
                        WinJS.log && WinJS.log("This application must be added to the lock screen before the background task will run.", "sample", "status");
                        break;

                    default:
                        // Finish by getting an initial position. This will present the location consent
                        // dialog if it's the first attempt for this application to access location.
                        getGeopositionAsync();
                        break;
                }
            },
            function (e) {
                // Did you forget to do the background task declaration in the package manifest?
                WinJS.log && WinJS.log(e.toString(), "sample", "error");
            }
        );
    }

    function unregisterBackgroundTask() {
        // Remove the application from the lock screen
        Background.BackgroundExecutionManager.removeAccess();

        // Unregister the background task
        geolocTask.unregister(true);
        geolocTask = null;

        WinJS.log && WinJS.log("Background task unregistered", "sample", "status");

        document.getElementById("latitude").innerHTML = "No data";
        document.getElementById("longitude").innerHTML = "No data";
        document.getElementById("accuracy").innerHTML = "No data";

        document.getElementById("registerBackgroundTaskButton").disabled = false;
        document.getElementById("unregisterBackgroundTaskButton").disabled = true;
    }

    function getGeopositionAsync() {
        var geolocator = new Windows.Devices.Geolocation.Geolocator();

        WinJS.log && WinJS.log("Getting initial position...", "sample", "status");

        promise = geolocator.getGeopositionAsync();
        promise.done(
            function (pos) {
                var coord = pos.coordinate;

                WinJS.log && WinJS.log("Initial position. Waiting for update...", "sample", "status");

                document.getElementById("latitude").innerHTML = coord.latitude;
                document.getElementById("longitude").innerHTML = coord.longitude;
                document.getElementById("accuracy").innerHTML = coord.accuracy;
            },
            function (err) {
                if (pageLoaded) {
                    WinJS.log && WinJS.log(err.message, "sample", "error");

                    document.getElementById("latitude").innerHTML = "No data";
                    document.getElementById("longitude").innerHTML = "No data";
                    document.getElementById("accuracy").innerHTML = "No data";
                }
            }
        );
    }
})();
