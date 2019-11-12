//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/servicing-complete.html", {
        ready: function (element, options) {
            ServicingComplete.updateUI();
        }
    });
})();

var ServicingComplete = {
    "updateUI": function () {
        try {
            var taskProgress = document.getElementById("servicingCompleteProgress");
            var taskStatus = document.getElementById("servicingCompleteStatus");

            taskProgress && (taskProgress.innerText = BackgroundTaskSample.servicingCompleteTaskProgress);
            taskStatus && (taskStatus.innerText = BackgroundTaskSample.getBackgroundTaskStatus(BackgroundTaskSample.servicingCompleteTaskName));
        } catch (ex) {

        }
    }
};
