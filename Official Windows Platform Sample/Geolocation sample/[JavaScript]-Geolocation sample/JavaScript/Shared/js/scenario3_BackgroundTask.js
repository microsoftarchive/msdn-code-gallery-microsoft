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
    var getGeopositionPromise;
    var sampleBackgroundTaskName = "SampleLocationBackgroundTask";
    var sampleBackgroundTaskEntryPoint = "js\\backgroundtask.js";
    var disposed;

    var page = WinJS.UI.Pages.define("/html/scenario3_BackgroundTask.html", {
        ready: function (element, options) {
            disposed = false;
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
                try {
                    var backgroundAccessStatus = Background.BackgroundExecutionManager.getAccessStatus();
                    switch (backgroundAccessStatus) {
                        case Background.BackgroundAccessStatus.unspecified:
                        case Background.BackgroundAccessStatus.denied:
                            WinJS.log && WinJS.log("This application must be added to the lock screen before the background task will run.", "sample", "status");
                            break;

                        default:
                            WinJS.log && WinJS.log("Background task is already registered. Waiting for next update...", "sample", "status");
                            break;
                    }
                } catch (ex) {

                    // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED) === -2147024846
                    if (ex.number === -2147024846) {
                        WinJS.log && WinJS.log("Location Simulator not supported.  Could not determine lock screen status, be sure that the application is added to the lock screen.", "sample", "status");
                    } else {
                        WinJS.log && WinJS.log(ex.toString(), "sample", "error");
                    }
                }

                TimerTriggerBackgroundTask.updateButtonStates(/*registered:*/ true);
            } else {
                TimerTriggerBackgroundTask.updateButtonStates(/*registered:*/ false);
            }
        },
        dispose: function () {
            if (!disposed) {
                disposed = true;

                if (getGeopositionPromise) {
                    getGeopositionPromise.operation.cancel();
                }

                if (geolocTask) {
                    geolocTask.removeEventListener("completed", onCompleted);
                }
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
        try {
            // Request lockscreen access
            Background.BackgroundExecutionManager.requestAccessAsync().done(
                function (backgroundAccessStatus) {
                    var builder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

                    // Register the background task
                    builder.name = sampleBackgroundTaskName;
                    builder.taskEntryPoint = sampleBackgroundTaskEntryPoint;
                    builder.setTrigger(new Windows.ApplicationModel.Background.TimeTrigger(15, false));

                    geolocTask = builder.register();

                    geolocTask.addEventListener("completed", onCompleted);

                    TimerTriggerBackgroundTask.updateButtonStates(/*registered:*/ true);

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
        } catch (ex) {
            // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED) === -2147024846

            if (ex.number === -2147024846) {
                WinJS.log && WinJS.log("Location Simulator not supported.  Could not get permission to add application to the lock screen, this application must be added to the lock screen before the background task will run.", "sample", "status");
            } else {
                WinJS.log && WinJS.log(ex.toString(), "sample", "error");
            }
        }
    }

    function unregisterBackgroundTask() {
        // Unregister the background task
        if (geolocTask) {
            geolocTask.unregister(true);
            geolocTask = null;
        }

        WinJS.log && WinJS.log("Background task unregistered", "sample", "status");

        document.getElementById("latitude").innerHTML = "No data";
        document.getElementById("longitude").innerHTML = "No data";
        document.getElementById("accuracy").innerHTML = "No data";

        TimerTriggerBackgroundTask.updateButtonStates(/*registered:*/ false);
    }

    function getGeopositionAsync() {
        var geolocator = new Windows.Devices.Geolocation.Geolocator();

        WinJS.log && WinJS.log("Getting initial position...", "sample", "status");

        getGeopositionPromise = geolocator.getGeopositionAsync();
        getGeopositionPromise.done(
            function (pos) {
                var coord = pos.coordinate;

                WinJS.log && WinJS.log("Initial position. Waiting for update...", "sample", "status");

                document.getElementById("latitude").innerHTML = coord.point.position.latitude;
                document.getElementById("longitude").innerHTML = coord.point.position.longitude;
                document.getElementById("accuracy").innerHTML = coord.accuracy;
            },
            function (err) {
                if (!disposed) {
                    WinJS.log && WinJS.log(err.message, "sample", "error");

                    document.getElementById("latitude").innerHTML = "No data";
                    document.getElementById("longitude").innerHTML = "No data";
                    document.getElementById("accuracy").innerHTML = "No data";
                }
            }
        );
    }
})();

// Update the disabled state of the register/unregister buttons.
var TimerTriggerBackgroundTask = {
    "updateButtonStates": function (registered) {
        var registerButton = document.getElementById("registerBackgroundTaskButton");
        var unregisterButton = document.getElementById("unregisterBackgroundTaskButton");

        registerButton && (registerButton.disabled = registered);
        unregisterButton && (unregisterButton.disabled = !registered);
    }
};