//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Custom templating function
var GeofenceBackgroundEventsItemTemplate = WinJS.Utilities.markSupportedForProcessing(function GeofenceBackgroundEventsItemTemplate(itemPromise) {
    return itemPromise.then(function (currentItem) {

        // Build ListView Item Container div
        var result = document.createElement("div");
        result.className = "GeofenceBackgroundEventsListViewItemStyle";

        // Build content body
        var body = document.createElement("div");

        // Display title
        if (currentItem.data) {
            var title = document.createElement("h4");
            title.innerText = currentItem.data;
            body.appendChild(title);
        }

        // put the body into the ListView Item
        result.appendChild(body);

        return result;
    });
});

(function () {
    "use strict";
    var Background = Windows.ApplicationModel.Background;
    var geofenceTask;
    var geolocator = new Windows.Devices.Geolocation.Geolocator();
    var getGeopositionPromise;
    var sampleBackgroundTaskName = "SampleGeofencingBackgroundTask";
    var sampleBackgroundTaskEntryPoint = "js\\geofencebackgroundtask.js";
    var geofenceEventsData;
    var geofenceEventsListView;
    var disposed;
    var page = WinJS.UI.Pages.define("/html/scenario5_GeofenceBackgroundTask.html", {
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
                    geofenceTask = cur;
                    break;
                }
                hascur = iter.moveNext();
            }

            if (geofenceTask) {
                // Associate an event handler to the existing background task
                geofenceTask.addEventListener("completed", onCompleted);

                try {
                    var backgroundAccessStatus = Background.BackgroundExecutionManager.getAccessStatus();
                    switch (backgroundAccessStatus) {
                        case Background.BackgroundAccessStatus.unspecified:
                        case Background.BackgroundAccessStatus.denied:
                            WinJS.log && WinJS.log("This application must be a lock screen app for the background task to run.", "sample", "status");
                            break;

                        default:
                            WinJS.log && WinJS.log("Background task is already registered. Waiting for next update...", "sample", "status");
                            break;
                    }
                } catch (ex) {

                    // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED) === -2147024846
                    // This HRESULT is only captured in Windows
                    var notSupportedError = -2147024846;
                    if (ex.number === notSupportedError) {
                        WinJS.log && WinJS.log("Location Simulator not supported.  Could not determine lock screen status, be sure that the application is added to the lock screen.", "sample", "status");
                    } else {
                        WinJS.log && WinJS.log(ex.toString(), "sample", "error");
                    }
                }

                LocationTriggerBackgroundTask.updateButtonStates(/*registered:*/ true);
            } else {
                LocationTriggerBackgroundTask.updateButtonStates(/*registered:*/ false);
            }

            geofenceEventsData = new WinJS.Binding.List();
            geofenceEventsListView = element.querySelector('#GeofenceBackgroundEventsListView').winControl;
            geofenceEventsListView.itemDataSource = geofenceEventsData.dataSource;
            geofenceEventsListView.itemTemplate = GeofenceBackgroundEventsItemTemplate;
            fillEventListBoxWithExistingEvents();
        },
        unload: function () {
            if (!disposed) {
                disposed = true;

                if (getGeopositionPromise) {
                    getGeopositionPromise.operation.cancel();
                }
            }
        }
    });

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        if (eventObject.detail.location === "/html/scenario5_GeofenceBackgroundTask.html") {
            // add geofence event to listbox
            fillEventListBoxWithExistingEvents();
        }
    });

    // Handle background task completion
    function onCompleted() {
        try {
            // Update the UI with the completion status of the background task
            // The Run method of the background task sets the LocalSettings. 
            var settings = Windows.Storage.ApplicationData.current.localSettings;

            // get status
            if (settings.values.hasKey("Status")) {
                WinJS.log && WinJS.log(settings.values["Status"].toString(), "sample", "status");
            }

            // add geofence event to listbox
            fillEventListBoxWithExistingEvents();
        } catch (ex) {
            // The background task had an error
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
                    builder.setTrigger(new Windows.ApplicationModel.Background.LocationTrigger(Windows.ApplicationModel.Background.LocationTriggerType.geofence));

                    // If it is important that there is user presence and/or
                    // internet connection when OnCompleted is called
                    // the following could be called before calling Register()
                    // var condition = new SystemCondition(SystemConditionType.userPresent | SystemConditionType.internetAvailable);
                    // builder.addCondition(condition);

                    geofenceTask = builder.register();

                    geofenceTask.addEventListener("completed", onCompleted);

                    LocationTriggerBackgroundTask.updateButtonStates(/*registered:*/ true);

                    switch (backgroundAccessStatus) {
                        case Background.BackgroundAccessStatus.unspecified:
                        case Background.BackgroundAccessStatus.denied:
                            WinJS.log && WinJS.log("This application must be a lock screen app for the background task to run.", "sample", "status");
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
            var notSupportedError = -2147024846;
            if (ex.number === notSupportedError) {
                WinJS.log && WinJS.log("Location Simulator not supported.  Could not get permission to add application to the lock screen, this application must be added to the lock screen before the background task will run.", "sample", "status");
            } else {
                WinJS.log && WinJS.log(ex.toString(), "sample", "error");
            }
        }
    }

    function unregisterBackgroundTask() {
        // Unregister the background task
        if (geofenceTask) {
            geofenceTask.unregister(true);
            geofenceTask = null;
        }

        WinJS.log && WinJS.log("Background task unregistered", "sample", "status");

        LocationTriggerBackgroundTask.updateButtonStates(/*registered:*/ false);
    }

    function getGeopositionAsync() {
        WinJS.log && WinJS.log("Checking permissions...", "sample", "status");

        getGeopositionPromise = geolocator.getGeopositionAsync();
        getGeopositionPromise.done(
            function (pos) {
                if (!disposed) {
                    var coord = pos.coordinate;

                    // got permissions so clear the status string
                    WinJS.log && WinJS.log("", "sample", "status");
                }
            },
            function (err) {
                if (!disposed) {
                    WinJS.log && WinJS.log(err.message, "sample", "error");
                }
            }
        );
    }

    function fillEventListBoxWithExistingEvents() {
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        if (settings.values.hasKey("BackgroundGeofenceEventCollection")) {
            var geofenceEvent = settings.values["BackgroundGeofenceEventCollection"].toString();

            if (0 !== geofenceEvent.length) {
                // remove all entries and repopulate
                while (geofenceEventsData.length > 0) {
                    geofenceEventsData.pop();
                }

                var events = JSON.parse(geofenceEvent);

                // NOTE: the events are accessed in reverse order
                // because the events were added to JSON from
                // newer to older.  geofenceEventsData.unshift() adds
                // each new entry to the beginning of the collection.
                for (var pos = events.length - 1; pos >= 0; pos--) {
                    var element = events[pos].toString();
                    geofenceEventsData.unshift(element);
                }
            }
        }
    }
})();

// Update the enable state of the register/unregister buttons.
var LocationTriggerBackgroundTask = {
    "updateButtonStates": function (registered) {
        var registerButton = document.getElementById("registerBackgroundTaskButton");
        var unregisterButton = document.getElementById("unregisterBackgroundTaskButton");

        registerButton && (registerButton.disabled = registered);
        unregisterButton && (unregisterButton.disabled = !registered);
    }
};