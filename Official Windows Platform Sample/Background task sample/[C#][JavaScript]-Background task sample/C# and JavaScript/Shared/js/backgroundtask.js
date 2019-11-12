// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// A JavaScript background task runs a specified JavaScript file.
//
(function () {
    "use strict";

    //
    // The background task instance's activation parameters are available via Windows.UI.WebUI.WebUIBackgroundTaskInstance.current
    //
    var cancel = false,
        progress = 0,
        backgroundTaskInstance = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current,
        cancelReason = "";

    console.log("Background " + backgroundTaskInstance.task.name + " Starting...");

    //
    // Query BackgroundWorkCost
    // Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
    // of work in the background task and return immediately.
    //
    var cost = Windows.ApplicationModel.Background.BackgroundWorkCost.currentBackgroundWorkCost;
    Windows.Storage.ApplicationData.current.localSettings.values["BackgroundWorkCost"] = cost.toString();

    //
    // Associate a cancellation handler with the background task.
    //
    function onCanceled(cancelEventArg) {
        cancel = true;
        cancelReason = cancelEventArg;
    }
    backgroundTaskInstance.addEventListener("canceled", onCanceled);

    //
    // This function is set to run every 1000 milliseconds ten times and perform background task activity.
    //
    function onTimer() {
        var key = null,
            settings = Windows.Storage.ApplicationData.current.localSettings,
            value = null;

        if ((!cancel) && (progress < 100)) {
            //
            // Simulate work being done.
            //
            setTimeout(onTimer, 1000);

            //
            // Indicate progress to the foreground application.
            //
            progress += 10;
            backgroundTaskInstance.progress = progress;

        } else {
            //
            // Use the succeeded property to indicate if this background task completed successfully.
            //
            backgroundTaskInstance.succeeded = (progress === 100);
            value = backgroundTaskInstance.succeeded ? "Completed" : "Canceled with reason: " + cancelReason ;
            console.log("Background " + backgroundTaskInstance.task.name + value);

            //
            // Write to localSettings to indicate that this background task completed.
            //
            key = backgroundTaskInstance.task.name;
            settings.values[key] = value;

            //
            // A JavaScript background task must call close when it is done.
            //
            close();
        }
    }

    //
    // Start the timer function to simulate background task work.
    //
    setTimeout(onTimer, 1000);
})();

