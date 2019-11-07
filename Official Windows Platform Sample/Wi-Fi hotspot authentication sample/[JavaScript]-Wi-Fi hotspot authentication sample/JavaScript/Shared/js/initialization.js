//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/initialization.html", {
        ready: function (element, options) {
            // Configure background task handler to perform authentication
            configuration.setAuthenticateThroughBackgroundTask(true);

            // Register button handlers
            document.getElementById("provisionButton").addEventListener("click", onButtonProvision, false);
            document.getElementById("registerBackgroundTaskButton").addEventListener("click", onButtonRegisterBackgroundTask, false);
            document.getElementById("unregisterBackgroundTaskButton").addEventListener("click", onButtonUnregisterBackgroundTask, false);

            // Register background task completion handler
            var isTaskRegistered = common.registerBackgroundTaskEventHandler();

            // Initialize button state
            updateButtonState(isTaskRegistered);
        }
    });

    // Update button state
    function updateButtonState(isTaskRegistered) {
        if (isTaskRegistered) {
            document.getElementById("registerBackgroundTaskButton").disabled = true;
            document.getElementById("unregisterBackgroundTaskButton").disabled = false;

        } else {
            document.getElementById("registerBackgroundTaskButton").disabled = false;
            document.getElementById("unregisterBackgroundTaskButton").disabled = true;
        }
    }

    // This is the click handler for the 'Provision' button to provision the embedded XML file
    function onButtonProvision() {

        // Error handler
        function provisionErrorHandler(error) {
            WinJS.log && WinJS.log(error.description, "sample", "error");
            document.getElementById("provisionButton").disabled = false;
        }

        document.getElementById("provisionButton").disabled = true;

        // Access provisioning file from installation folder
        Windows.ApplicationModel.Package.current.installedLocation.getFileAsync("ProvisioningData.xml").then(function (file) {
            // Load with XML parser
            Windows.Data.Xml.Dom.XmlDocument.loadFromFileAsync(file).then(function (xmlDocument) {
                // Get raw XML
                var provXml = xmlDocument.getXml();
                var provAgent = new Windows.Networking.NetworkOperators.ProvisioningAgent();
                // Apply provisioning data
                provAgent.provisionFromXmlDocumentAsync(provXml).done(function (results) {
                    if (results.allElementsProvisioned) {
                        // Provisioning is done successfully
                        WinJS.log && WinJS.log("Provisioning was successful", "sample", "status");
                    }
                    else {
                        // Error has occured during provisioning
                        WinJS.log && WinJS.log("Provisioning result: " + results.provisionResultsXml, "sample", "status");
                    }
                    document.getElementById("provisionButton").disabled = false;
                }, function (error) {
                    provisionErrorHandler(error);
                });
            }, function (error) {
                provisionErrorHandler(error);
            });
        }, function (error) {
            provisionErrorHandler(error);
        });
    }

    // This is the click handler for the 'Register' button to registers a background task for
    // the NetworkOperatorHotspotAuthentication event
    function onButtonRegisterBackgroundTask() {
        // Prepare to create the background task.
        try {
                        
            // For windows phone, we need to call RequestAccessAsync always to enable Background Task to be launched even when 
            // screen is locked
            if (platform.HotspotAuthenticationSamplePlatformSpecific.taskRequiresBackgroundAccess()) {
                var requestAccessAsyncSuccessful = true;
                Windows.ApplicationModel.Background.BackgroundExecutionManager.requestAccessAsync().done(function (accessStatus) {
                    if (Windows.ApplicationModel.Background.BackgroundAccessStatus.accessDenied === accessStatus) {
                        WinJS.log && WinJS.log("Access denied while Requesting Async Access", "sample", "error");
                        requestAccessAsyncSuccessful = false;
                    }
                });
                if (!requestAccessAsyncSuccessful) {
                    return;
                }
            }

            // Create a new background task builder.
            var myTaskBuilder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

            // Create a new networkOperatorHotspotAuthentication trigger.
            var myTrigger = new Windows.ApplicationModel.Background.NetworkOperatorHotspotAuthenticationTrigger();

            // Associate the networkOperatorHotspotAuthentication trigger with the background task builder.
            myTaskBuilder.setTrigger(myTrigger);

            // Specify the background task to run when the trigger fires.
            myTaskBuilder.taskEntryPoint = common.backgroundTaskEntryPoint;

            // Name the background task.
            myTaskBuilder.name = common.backgroundTaskName;

            // Register the background task.
            var myTask = myTaskBuilder.register();

            // Associate progress and completed event handlers with the new background task.
            myTask.addEventListener("completed", new common.completeHandler(myTask).onCompleted);

            updateButtonState(true);

        } catch (ex) {
            WinJS.log && WinJS.log(ex, "sample", "error");

            // Clean up any tasks that were potentially created.
            unregisterBackgroundTask();
        }
    }

    // This is the click handler for the 'Unregister' button
    function onButtonUnregisterBackgroundTask() {
        unregisterBackgroundTask();
        updateButtonState(false);
    }

    // Unregister background task
    function unregisterBackgroundTask() {
        // Loop through all background tasks and unregister any.
        var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
        var hascur = iter.hasCurrent;
        while (hascur) {
            var cur = iter.current.value;
            if (cur.name === common.backgroundTaskName) {
                cur.unregister(true);
            }
            hascur = iter.moveNext();
        }
    }
})();
