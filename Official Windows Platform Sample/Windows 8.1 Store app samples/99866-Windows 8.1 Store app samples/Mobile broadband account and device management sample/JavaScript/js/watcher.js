//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var networkAccountWatcher = Windows.Networking.NetworkOperators.MobileBroadbandAccountWatcher();

    var page = WinJS.UI.Pages.define("/html/watcher.html", {
        ready: function (element, options) {
            prepareWatcher();
            document.getElementById("startMonitoring").addEventListener("click", watcherStart, false);
            document.getElementById("stopMonitoring").addEventListener("click", watcherStop, false);
        },
        unload: function (eventArgument) {
            unloadScenario();
        }
    });

    function networkDeviceStatusToString(deviceStatus) {
        switch (deviceStatus) {
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.deviceNotReady:
                return "Device Not Ready";
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.deviceReady:
                return "Device Ready";
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.simNotInserted:
                return "Sim Not Inserted";
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.badSim:
                return "Bad Sim";
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.deviceHardwareFailure:
                return "Device Hardware Failure";
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.accountNotActivated:
                return "Account Not Activated";
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.deviceLocked:
                return "Device Locked";
            case Windows.Networking.NetworkOperators.NetworkDeviceStatus.deviceBlocked:
                return "Device Blocked";
            default:
                return "Unknown";
        }
    }

    function mobileBroadbandRadioStateToString(radioState) {
        switch (radioState) {
            case Windows.Networking.NetworkOperators.MobileBroadbandRadioState.off:
                return "Off";
            case Windows.Networking.NetworkOperators.MobileBroadbandRadioState.on:
                return "On";
            default:
                return "Unknown";
        }
    }

    function networkRegistrationStateToString(state) {
        switch (state) {
            case Windows.Networking.NetworkOperators.NetworkRegistrationState.none:
                return "None";
            case Windows.Networking.NetworkOperators.NetworkRegistrationState.deregistered:
                return "Deregistered";
            case Windows.Networking.NetworkOperators.NetworkRegistrationState.searching:
                return "Searching";
            case Windows.Networking.NetworkOperators.NetworkRegistrationState.home:
                return "Home";
            case Windows.Networking.NetworkOperators.NetworkRegistrationState.roaming:
                return "Roaming";
            case Windows.Networking.NetworkOperators.NetworkRegistrationState.partner:
                return "Partner";
            case Windows.Networking.NetworkOperators.NetworkRegistrationState.denied:
                return "Denied";
            default:
                return "Unknown";
        }
    }

    function dumpAccountDeviceInformation(deviceInformation) {
        var message = "";

        message += "\nNetworkDeviceStatus: " + networkDeviceStatusToString(deviceInformation.networkDeviceStatus);
        message += "\nDeviceId: " + deviceInformation.deviceId;
        message += "\nSubscriberId: " + deviceInformation.subscriberId;
        message += "\nSimIccId: " + deviceInformation.simIccId;
        message += "\nRadioState: " + mobileBroadbandRadioStateToString(deviceInformation.currentRadioState);

        return message;
    }

    function dumpAccountNetwork(network) {
        var message = "";

        message += "\nNetworkRegistrationState: " + networkRegistrationStateToString(network.networkRegistrationState);
        message += "\nRegistrationNetworkError: " + network.registrationNetworkError;
        message += "\nPacketAttachNetworkError: " + network.packetAttachNetworkError;
        message += "\nActivationNetworkError: " + network.activationNetworkError;
        message += "\nAccessPointName: " + network.accessPointName;

        return message;
    }

    function dumpPropertyData(networkAccountId, hasDeviceInformationChanged, hasNetworkChanged) {
        var message = "";

        var account = Windows.Networking.NetworkOperators.MobileBroadbandAccount.createFromNetworkAccountId(networkAccountId);

        if (hasDeviceInformationChanged) {
            message += dumpAccountDeviceInformation(account.currentDeviceInformation);
        }

        if (hasNetworkChanged) {
            message += dumpAccountNetwork(account.currentNetwork);
        }

        return message;
    }

    var isListenerAdded = false;
    function prepareWatcher() {
        if (!isListenerAdded) {
            isListenerAdded = true;

            // Add event listener for account added.
            networkAccountWatcher.addEventListener(
                "accountadded",
                function (args) {
                    var message = "\n[accountadded] " + args.networkAccountId;
                    var account = Windows.Networking.NetworkOperators.MobileBroadbandAccount.createFromNetworkAccountId(args.networkAccountId);
                    message += ", service provider name: " + account.serviceProviderName;
                    document.getElementById("eventLogger").innerText += message;
                },
                false);

            // Add event listener for account updated.
            networkAccountWatcher.addEventListener(
                "accountupdated",
                function (args) {
                    var message = "\n[accountupdated] " + args.networkAccountId + ", (network = " + args.hasNetworkChanged + " deviceinformation = " + args.hasDeviceInformationChanged + ")";
                    message += dumpPropertyData(args.networkAccountId, args.hasDeviceInformationChanged, args.hasNetworkChanged);
                    document.getElementById("eventLogger").innerText += message;
                },
                false);

            // Add event listener for account removed.
            networkAccountWatcher.addEventListener(
                "accountremoved",
                function (args) {
                    var message = "\n[accountremoved] " + args.networkAccountId;
                    document.getElementById("eventLogger").innerText += message;
                },
                false);

            // Add event listener for enumeration completed.
            networkAccountWatcher.addEventListener(
                "enumerationcompleted",
                function () {
                    document.getElementById("eventLogger").innerText += "\n[enumerationcompleted]";
                },
                false);

            // Add event listener for stop.
            networkAccountWatcher.addEventListener(
                "stopped",
                function () {
                    document.getElementById("eventLogger").innerText += "\n[stopped] Watcher is stopped successfully";
                },
                false);
        }
    }

    function watcherStart() {
        if ((networkAccountWatcher.status === Windows.Networking.NetworkOperators.MobileBroadbandAccountWatcherStatus.started) ||
    (networkAccountWatcher.status === Windows.Networking.NetworkOperators.MobileBroadbandAccountWatcherStatus.enumerationCompleted)) {
            WinJS.log && WinJS.log("Watcher is already started", "sample", "status");
        } else {
            networkAccountWatcher.start();
            WinJS.log && WinJS.log("Watcher is started successfully", "sample", "status");
        }
    }

    function watcherStop() {
        if ((networkAccountWatcher.status === Windows.Networking.NetworkOperators.MobileBroadbandAccountWatcherStatus.started) ||
            (networkAccountWatcher.status === Windows.Networking.NetworkOperators.MobileBroadbandAccountWatcherStatus.enumerationCompleted)) {
            WinJS.log && WinJS.log("Watcher is stopping ...", "sample", "status");
            networkAccountWatcher.stop();
        } else {
            WinJS.log && WinJS.log("Watcher is already stopped", "sample", "status");
        }
    }

    function unloadScenario() {
        if ((networkAccountWatcher.status === Windows.Networking.NetworkOperators.MobileBroadbandAccountWatcherStatus.started) ||
            (networkAccountWatcher.status === Windows.Networking.NetworkOperators.MobileBroadbandAccountWatcherStatus.enumerationCompleted)) {
            networkAccountWatcher.stop();
        }
    }
})();
