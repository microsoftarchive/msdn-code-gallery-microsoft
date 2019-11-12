// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    // Get the background task details
    var backgroundTask = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current;
    var settings = Windows.Storage.ApplicationData.current.localSettings;
    var taskName = backgroundTask.task.name;

    console.log("Background task \"" + taskName + "\" starting...");

    // Store the content received from the notification so it can be retrieved
    // from the UI.
    var notificationDetails = backgroundTask.triggerDetails;
    settings.values[taskName] = notificationDetails.content;

    console.log("Background \"" + taskName + "\" completed!");

    // Close the instance running the task
    close();
})();
