//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var tetheringManager = null;

(function () {
    "use strict";

    var networkOperators = Windows.Networking.NetworkOperators;

    var deviceAccountID = []; // array used to track GUIDs for each of the Mobile Broadband device

    var page = WinJS.UI.Pages.define("/html/tethering.html", {
        ready: function (element, options) {
            prepareScenario();
        }
    });

    // Simplifies DOM text input.
    function id(elementId) {
        return document.getElementById(elementId);
    }

    // Looks for the presence of Mobile Broadband devices; if installed,
    // program runs and gets device IDs. However, if there are errors,
    // the buttons are disabled and program doesn't do anything.
    function prepareScenario() {
        try {

            var iter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();
            var hascur = iter.hasCurrent;
            while (hascur) {
                var cur = iter.current.value;
                if (cur.name === "MobileOperatorNotificationHandler") {
                    cur.addEventListener("completed", new CompleteHandler(cur).onCompleted);
                }
                hascur = iter.moveNext();
            }

            id("startTethering").disabled = false;
            id("stopTethering").disabled = false;
            id("applyConfiguration").disabled = false;
            id("startTethering").addEventListener("click", startTethering_Click, false);
            id("stopTethering").addEventListener("click", stopTethering_Click, false);
            id("applyConfiguration").addEventListener("click", apply_Click, false);
            makeTetheringManagerAvailable();
            updateUI();

        } catch (err) { // try catch for testing purposes.  Not required for SDK sample
            WinJS.log && WinJS.log("MBAE Error: " + err.description, "sample", "error");
        }
    }

    // Make sure tethering manager is available. Show proper error message when it's unavailable
    function makeTetheringManagerAvailable()
    {
        try {
            if (tetheringManager === null) {
                // See if we have any devices with installed applications.
                var mobileBroadbandDevices = Windows.Networking.NetworkOperators.MobileBroadbandAccount.availableNetworkAccountIds;

                if (mobileBroadbandDevices.size !== 0) { // Device is or was installed AND access enabled

                    // We have at least one device. Get the device ID for
                    // each.
                    for (var i = 0; i < mobileBroadbandDevices.size; i++) {
                        deviceAccountID[i] = mobileBroadbandDevices[i];
                    }

                    // verify tethering capabilities
                    var capabilities = networkOperators.NetworkOperatorTetheringManager.getTetheringCapability(deviceAccountID[0]);
                    if (capabilities !== networkOperators.TetheringCapability.enabled) {
                        onCapabilityError(capabilities);
                    } else {
                        tetheringManager = Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager.createFromNetworkAccountId(deviceAccountID[0]);
                    }
                }
            }
        } catch (err) { // try catch for testing purposes.  Not required for SDK sample
            WinJS.log && WinJS.log("MBAE Error: " + err.description, "sample", "error");
        }
    }

    // Update UI components according to tethering state
    function updateUI() {
        if (tetheringManager !== null) {
            var accessPointConfiguration = tetheringManager.getCurrentAccessPointConfiguration();
            id("ssid").value = accessPointConfiguration.ssid;
            id("passphrase").value = accessPointConfiguration.passphrase;
            var connectedClients = tetheringManager.clientCount;
            if (tetheringManager.tetheringOperationalState === Windows.Networking.NetworkOperators.TetheringOperationalState.on) {
                WinJS.log(connectedClients +
                                        " of " +
                                        tetheringManager.maxClientCount +
                                        " are connected to your tethering network", "sample", "status");
                id("startTethering").disabled = true;
            }
            else
            {
                id("stopTethering").disabled = true;
            }
        }
        else
        {
            WinJS.log("Tethering is not available.", "sample", "error");
            id("startTethering").disabled = true;
            id("stopTethering").disabled = true;
            id("applyConfiguration").disabled = true;
        }
    }

    function apply_Click() {
        if (tetheringManager !== null)
        {
            var newConfig = new Windows.Networking.NetworkOperators.NetworkOperatorTetheringAccessPointConfiguration();
            
            newConfig.passphrase = id('passphrase').value;
            newConfig.ssid = id('ssid').value;
            tetheringManager.configureAccessPointAsync(newConfig).then(onConfigureDone, onConfigureError, null);
        }
    }

    // Updating tethering AP configuration completed
    function onConfigureDone()
    {
        WinJS.log("configuration applied ", "sample", "status");
    }

    // Updating tethering AP configuration failed
    function onConfigureError(error)
    {
        updateUI();
        WinJS.log("failed to appliy tethering configuration. " + error, "sample", "error");
    }

    function startTethering_Click() {
        try {
            tetheringManager.startTetheringAsync().then(onTetheringStartComplete, onTetheringStartComplete,null);
            id("startTethering").disabled = true;
            id("stopTethering").disabled = true;
        } catch (err) {
            WinJS.log && WinJS.log("MBAE Error: " + err.description, "sample", "error");
        }
    }

    function stopTethering_Click() {
        try {
            tetheringManager.stopTetheringAsync().done(onTetheringStopComplete);
            id("startTethering").disabled = true;
            id("stopTethering").disabled = true;
        } catch (err) {
            WinJS.log && WinJS.log("MBAE Error: " + err.description, "sample", "error");
        }
    }

    //
    //  startTetheringComplete
    function onTetheringStartComplete(result) {
        var errorString = "";
        if (result.hasErrorAccurred === true) {

            if (result.additionalErrorMessage !== "") {
                errorString = result.additionalErrorMessage;
            } else {
                errorString = getTetheringErrorString(result.Status);
            }
            WinJS.log && WinJS.log("StartTethering failed: " + errorString, "sample", "error");
        }
        else {
            WinJS.log && WinJS.log("StartTethering succeeded", "sample", "status");
        }
        id("stopTethering").disabled = false;
    }

    //
    //  stopTetheringComplete
    function onTetheringStopComplete(result) {
        var errorString = "";
        if (result.hasErrorAccurred === true) {

            if (result.additionalErrorMessage !== "") {
                errorString = result.additionalErrorMessage;
            } else{
                errorString = getTetheringErrorString(result.Status);
            }
            WinJS.log && WinJS.log("StartTethering failed: " + errorString, "sample", "error");
        }
        else {
            WinJS.log && WinJS.log("StopTethering succeeded", "sample", "status");
        }
        id("startTethering").disabled = false;
    }


    //
    //  map tethering errors to strings
    function getTetheringErrorString(error){
        var errorString;
        switch(error)
        {
            case Windows.Networking.NetworkOperators.TetheringOperationStatus.success:
                errorString = "No error";
                break;
            case Windows.Networking.NetworkOperators.TetheringOperationStatus.unknown:
                errorString = "Unknown error has occurred.";
                break;
            case Windows.Networking.NetworkOperators.TetheringOperationStatus.mobileBroadbandDeviceOff:
                errorString = "Please make sure your MB device is turned on.";
                break;
            case Windows.Networking.NetworkOperators.TetheringOperationStatus.wiFiDeviceOff:
                errorString = "Please make sure your WiFi device is turned on.";
                break;
            case Windows.Networking.NetworkOperators.TetheringOperationStatus.entitlementCheckTimeout:
                errorString = "We coudn't contact your Mobile Broadband operator to verify your ability to enable tethering, please contact your Mobile Operator.";
                break;
            case Windows.Networking.NetworkOperators.TetheringOperationStatus.entitlementCheckFailure:
                errorString = "You Mobile Broadband operator does not allow tethering on this device.";
                break;
            case Windows.Networking.NetworkOperators.TetheringOperationStatus.operationInProgress:
                errorString = "The system is busy, please try again later.";
                break;
        }
        return errorString;
    }


    //
    //  show messages when tethering is disabled on this device
    function onCapabilityError(error) {
        var errorString = "";

        switch (error) {
            case networkOperators.TetheringCapability.disabledByGroupPolicy:
                errorString = 
                    "Your network administrator has disabled tethering on your machine.";
                break;
            case networkOperators.TetheringCapability.disabledByHardwareLimitation:
                errorString = 
                    "Your device hardware doesn't support tethering.";
                break;
            case networkOperators.TetheringCapability.disabledByOperator:
                errorString = 
                    "Your Mobile Broadband Operator has disabled tethering on your device.";
                break;
            case networkOperators.TetheringCapability.disabledBySku:
                errorString = 
                    "This version of Windows doesn't support tethering.";
                break;
            case networkOperators.TetheringCapability.disabledByRequiredAppNotInstalled:
                errorString = 
                    "Required  app is not installed.";
                break;
            case networkOperators.TetheringCapability.disabledDueToUnknownCause:
                errorString = 
                    "Unknown issue.";
                break;
        }

        WinJS.log && WinJS.log(errorString, "sample", "error");
    }

    //
    // Background task completion handler
    function CompleteHandler(task) {
        this.onCompleted = function (args) {
            try {
                args.checkResult();
                // extract notification type
                var key = task.taskId + "_type";
                var settings = Windows.Storage.ApplicationData.current.localSettings;
                var opMsgType = settings.values.lookup(key);
                if ((opMsgType === Windows.Networking.NetworkOperators.NetworkOperatorEventMessageType.tetheringOperationalStateChanged) ||
                    (opMsgType === Windows.Networking.NetworkOperators.NetworkOperatorEventMessageType.tetheringNumberOfClientsChanged)) {
                    updateUI();
                }
            } catch (ex) {
                WinJS.log && WinJS.log("Operator notification background task failed." + ex, "sample", "error");
            }
        };
    }

})();
