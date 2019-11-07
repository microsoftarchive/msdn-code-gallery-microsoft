//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1_NetworkStatusWithInternetPresent.html", {
        ready: function (element, options) {
            document.getElementById("registerSampleBackgroundTaskWithConditionButton").addEventListener("click", registerSampleBackgroundTaskWithCondition, false);
            document.getElementById("unregisterSampleBackgroundTaskWithConditionButton").addEventListener("click", unregisterSampleBackgroundTaskWithCondition, false);
            SampleBackgroundTaskWithCondition.updateUI();
        }
    });

    function registerSampleBackgroundTaskWithCondition() {
        try {
            //Before registering for network status change, save current internet profile and network adapter ID globally
            var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.getInternetConnectionProfile();
            if (connectionProfile !== null) {
                BackgroundTaskSample.internetProfile = connectionProfile.profileName;
                var networkAdapterInfo = connectionProfile.networkAdapter;
                if (networkAdapterInfo !== null) {
                    BackgroundTaskSample.networkAdapter = networkAdapterInfo.networkAdapterId.toString();
                }
                else {
                    BackgroundTaskSample.networkAdapter = "Not connected to Internet";
                }
            }
            else {
                BackgroundTaskSample.internetProfile = "Not connected to Internet";
                BackgroundTaskSample.networkAdapter = "Not connected to Internet";
            }
        }
        catch (ex) {
            WinJS.log && WinJS.log(ex, "sample", "error");
        }

        BackgroundTaskSample.registerBackgroundTask(BackgroundTaskSample.sampleBackgroundTaskEntryPoint,
                                                    BackgroundTaskSample.sampleBackgroundTaskWithConditionName,
                                                    new Windows.ApplicationModel.Background.SystemTrigger(Windows.ApplicationModel.Background.SystemTriggerType.networkStateChange, false),
                                                    new Windows.ApplicationModel.Background.SystemCondition(Windows.ApplicationModel.Background.SystemConditionType.internetAvailable));
        SampleBackgroundTaskWithCondition.updateUI();
        WinJS.log && WinJS.log("Registered", "sample", "status");
    }

    function unregisterSampleBackgroundTaskWithCondition() {
        BackgroundTaskSample.unregisterBackgroundTasks(BackgroundTaskSample.sampleBackgroundTaskWithConditionName);
        SampleBackgroundTaskWithCondition.updateUI();
        WinJS.log && WinJS.log("Unregistered", "sample", "status");
    }
})();

(function () {
    "use strict";

    window.SampleBackgroundTaskWithCondition = {
        updateUI: function () {
            try {
                var registerButton = document.getElementById("registerSampleBackgroundTaskWithConditionButton");
                var unregisterButton = document.getElementById("unregisterSampleBackgroundTaskWithConditionButton");

                registerButton && (registerButton.disabled = BackgroundTaskSample.sampleBackgroundTaskWithConditionRegistered);
                unregisterButton && (unregisterButton.disabled = !BackgroundTaskSample.sampleBackgroundTaskWithConditionRegistered);
            }
            catch (ex) {
                WinJS.log && WinJS.log(ex, "sample", "error");
            }
        }
    };
})();
