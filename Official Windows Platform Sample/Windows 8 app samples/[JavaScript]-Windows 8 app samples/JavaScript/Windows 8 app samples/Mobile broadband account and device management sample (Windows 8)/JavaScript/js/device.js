//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // MB Device variables
    var MobileEquipmentID = '',
        SimICCID = '',
        SubscriberID = '',
        MobileNumber = '',
        Manufacturer = '',
        FirmwareInfo = '',
        CustomDataClass = '',
        DataClassString = '',
        CellularClasses = '',
        DeviceModel = '';
    var DeviceNetworkStatus = 0;
    var deviceSelected = 1;
    var deviceAccountID = []; // array used to track GUIDs for each of the Mobile Broadband device
    var numberDevices = 0;

    var page = WinJS.UI.Pages.define("/html/device.html", {
        ready: function (element, options) {
            runDeviceScenario();
        }
    });

    // Mobile Device type and value enumeration used to look up data class information from HEX return value

    var dataClassMap = ["None", "GPRS", "EDGE", "UMTS", "HSDPA", "HSUPA", "LTE", "1xRTT", "1xEVDO", "1xEVDO-A", "1xEVDV",  "3xRTT", "1xEVDO-B", "UMB", "Custom"];
    dataClassMap.bitMap = [0x0, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x10000, 0x20000, 0x40000, 0x80000, 0x100000, 0x200000, 0x400000, 0x80000000];

    var mobileBroadbandDevices = 0; // stores the number of mobile broadband devices that have been installed

    // Simplifies DOM text input.
    function id(elementId) {
        return document.getElementById(elementId);
    }

    // Looks for the presence of Mobile Broadband devices; if installed,
    // program runs and gets device IDs. However, if there are errors,
    // the buttons are disabled and program doesn't do anything.
    function runDeviceScenario() {
        try {
            // See if we have any devices with installed applications.

            mobileBroadbandDevices = Windows.Networking.NetworkOperators.MobileBroadbandAccount.availableNetworkAccountIds;

            if (mobileBroadbandDevices.size !== 0) { // Device is or was installed AND access enabled
                WinJS.log && WinJS.log("Mobile Broadband Device(s) have been installed that grant access to this application", "sample", "status");

                // We have at least one device. Get the device ID for
                // each.
                for (var i = 0; i < mobileBroadbandDevices.size; i++) {
                    deviceAccountID[i] = mobileBroadbandDevices[i];
                }
                // have Button ready to display value for first device

                numberDevices = mobileBroadbandDevices.size;
                id("numDevices").innerHTML = "There are " + /*@static_cast(String)*/numberDevices + " account(s) installed.        ";
                id("updateDataButton").innerText = "Get Information for Device #" + /*@static_cast(String)*/deviceSelected;
                id("updateDataButton").addEventListener("click", updateDataButton, false);
            }
            else {
                id("updateDataButton").innerText = "No available accounts detected";
            }

        } catch (err) { // try catch for testing purposes.  Not required for SDK sample
            WinJS.log && WinJS.log("MBAE Error: " + err.description, "sample", "error");
        }
    }

    // Gets a device that is properly connected to computer and prints out
    // information about the device.
    function getCurrentDeviceInfo(deviceID) {
        try {
            // Get the device from provided account ID.
            var mobileBroadbandAccount = Windows.Networking.NetworkOperators.MobileBroadbandAccount.createFromNetworkAccountId(deviceID);

            // Get account properties.
            var NetworkAccountId = mobileBroadbandAccount.networkAccountId;
            id("networkAccountId").innerHTML = '' + NetworkAccountId;
            var ProviderGuid = mobileBroadbandAccount.serviceProviderGuid;
            id("providerGuid").innerHTML = '' + ProviderGuid;
            var ProviderName = mobileBroadbandAccount.serviceProviderName;
            id("providerName").innerHTML = '' + ProviderName;

            // Get basic hardware information and display..
            var hardwareInfo = mobileBroadbandAccount.currentDeviceInformation;

            // Get the network adapter information
            var myNetworkAdapter = mobileBroadbandAccount.currentNetwork.networkAdapter;
            // retrieving the adapter ID.
            var NetworkAdapterID = myNetworkAdapter.networkAdapterId;
            id("networkAdapterID").innerHTML = '' + NetworkAdapterID;
            // Retrieve the network type (none, Internet, Private Network)
            var interfaceType = myNetworkAdapter.networkItem;
            if (interfaceType) {
                var networkType = interfaceType.getNetworkTypes();
                switch (networkType) {
                    case (Windows.Networking.Connectivity.NetworkTypes.none):
                        id("networktype").innerHTML = "None";
                        break;
                    case (Windows.Networking.Connectivity.NetworkTypes.internet):
                        id("networktype").innerHTML = "Internet";
                        break;
                    case (Windows.Networking.Connectivity.NetworkTypes.privateNetwork):
                        id("networktype").innerHTML = "Private Network";
                        break;
                    case (Windows.Networking.Connectivity.NetworkTypes.internet | Windows.Networking.Connectivity.NetworkTypes.privateNetwork):
                        id("networktype").innerHTML = "Internet, Private Network";
                        break;
                }
            }
            else {
                id("networktype").innerHTML = "Not connected";
            }

            SimICCID = hardwareInfo.simIccId;
            id("simID").innerHTML = SimICCID;

            FirmwareInfo = hardwareInfo.firmwareInformation;
            id("firmwareVer").innerHTML = FirmwareInfo;

            SubscriberID = hardwareInfo.subscriberId;
            id("subID").innerHTML = SubscriberID;

            Manufacturer = hardwareInfo.manufacturer;
            id("manufacture").innerHTML = Manufacturer;

            DeviceModel = hardwareInfo.model;
            id("deviceModel").innerHTML = DeviceModel;

            // Convert the device status values to a string value.
            id("networkDeviceStatus").innerHTML = networkDeviceStatusToString(hardwareInfo.networkDeviceStatus);

            DeviceNetworkStatus = hardwareInfo.deviceType;
            // Convert the device type values to a string value.
            switch (DeviceNetworkStatus) {

                case 0:
                    id("deviceType").innerHTML = "Unknown";
                    break;
                case 1:
                    id("deviceType").innerHTML = "Embedded";
                    break;
                case 2:
                    id("deviceType").innerHTML = "Removable";
                    break;
                case 3:
                    id("deviceType").innerHTML = "Remote";
                    break;
            }

            id("deviceId").innerHTML = '' + hardwareInfo.deviceId;

            MobileEquipmentID = hardwareInfo.mobileEquipmentId;
            id("devID").innerHTML = MobileEquipmentID;

            MobileNumber = hardwareInfo.telephoneNumbers;
            var phoneNumbers = '';
            var i;
            if (MobileNumber.size !== 0) { // At least one number is available.
                for (i = 0; i < MobileNumber.size; i++) {
                    phoneNumbers = "#" + /*@static_cast(String)*/i + " " + MobileNumber[i] + " ";
                }
            } else { // No number is displayed by device. The device may still have a number though.
                phoneNumbers = "No Mobile Number Provided.";
            }
            id("mobileNumber").innerHTML = phoneNumbers;

            var cellClassType = hardwareInfo.cellularClass;

            // Convert the cellular class values to correct class and ID
            // strings.
            switch (cellClassType) {
                case 0:
                    CellularClasses = "None";
                    id("devIdLabel").innerHTML = '&bull; None:';
                    id("subIdLabel").innerHTML = '&bull; None:';
                    break;

                case 1:
                    CellularClasses = "GSM";
                    id("devIdLabel").innerHTML = '&bull; IMEI:';
                    id("subIdLabel").innerHTML = '&bull; IMSI:';
                    break;

                case 2:
                    CellularClasses = "CDMA";
                    id("devIdLabel").innerHTML = '&bull; ESN/MEID:';
                    id("subIdLabel").innerHTML = '&bull; MIN/IRM:';
                    break;
            }
            id("cellClasses").innerHTML = CellularClasses;

            var DataClasses = hardwareInfo.dataClasses;
            var len = dataClassMap.length;
            DataClassString = '';
            if (DataClasses === Windows.Networking.NetworkOperators.DataClasses.none) {
                DataClassString = "None";
            } else {
                // Do a bit map compare and if match add value to string to
                // be displayed.
                for (i = 0; i < len; i++) {
                    if ((DataClasses & dataClassMap.bitMap[i]) !== 0) {
                        if (DataClassString !== '') {
                            DataClassString += "/";
                        }
                        DataClassString += dataClassMap[i];
                    }
                }
            }
            id("dataClasses").innerHTML = DataClassString;

            // Get the state the MB device is operating under.
            try {
                var regState = mobileBroadbandAccount.currentNetwork.networkRegistrationState;
                id("netRegister").innerHTML = networkRegistrationStateToString(regState);
            } catch(err) { // no value returned
                id("netRegister").innerHTML = 'Value Not Returned';
            }

            var regError = mobileBroadbandAccount.currentNetwork.registrationNetworkError;
            if (regError === 0) { // No error
                id("netRegError").innerHTML = "none";
            } else { // Found error
                id("netRegError").innerHTML = regError.networkError;
            }

            var packetError = mobileBroadbandAccount.currentNetwork.packetAttachNetworkError;
            if (packetError === 0) { // No error
                id("packetAttachError").innerHTML = "none";
            } else { // Found error
                id("packetAttachError").innerHTML = packetError.networkError;
            }

            var netActivateError = mobileBroadbandAccount.currentNetwork.activationNetworkError;
            if (netActivateError === 0) { // No error
                id("activateError").innerHTML = "none";
            } else { // Found error
                id("activateError").innerHTML = netActivateError.networkError;
            }

            var accessPointName = mobileBroadbandAccount.currentNetwork.accessPointName;
            if (accessPointName.length > 0) {
                id("accessPointInfo").innerHTML = accessPointName;
            } else {
                id("accessPointInfo").innerHTML = "Device not connected";
            }


            var registeredProviderName = mobileBroadbandAccount.currentNetwork.registeredProviderName;
            id("registeredProviderName").innerHTML = '' + registeredProviderName;
            var registeredProviderId = mobileBroadbandAccount.currentNetwork.registeredProviderId;
            id("registeredProviderId").innerHTML = '' + registeredProviderId;

            var registeredDataClass = mobileBroadbandAccount.currentNetwork.registeredDataClass;
            var registeredDataClassLen = dataClassMap.length;
            var registeredDataClassString = '';
            // Do a bit map compare and if match add value to string to
            // be displayed.
            if (registeredDataClass === Windows.Networking.NetworkOperators.DataClasses.none) {
                registeredDataClassString = "None";
            } else {
                for (i = 0; i < len; i++) {
                    if ((registeredDataClass & dataClassMap.bitMap[i]) !== 0) {
                        if (registeredDataClassString !== '') {
                            registeredDataClassString += "/";
                        }
                        registeredDataClassString += dataClassMap[i];
                    }
                }
            }
            id("registeredDataClass").innerHTML = registeredDataClassString;

            WinJS.log && WinJS.log("MB Device #" + /*@static_cast(String)*/deviceSelected + " information read.", "sample", "status");
        } catch (err) {
            WinJS.log && WinJS.log("MB Device Hardware Failure Error: " + err.description, "sample", "error");
            id("devID").innerHTML = '';
            id("simID").innerHTML = '';
            id("subID").innerHTML = '';
            id("mobileNumber").innerHTML = '';
            id("manufacture").innerHTML = '';
            id("firmwareVer").innerHTML = '';
            id("dataClasses").innerHTML = '';
            id("cellClasses").innerHTML = '';
            id("deviceModel").innerHTML = '';
            id("netRegister").innerHTML = '';
            id("netRegError").innerHTML = '';
            id("packetAttachError").innerHTML = '';
            id("activateError").innerHTML = '';
            id("accessPointInfo").innerHTML = '';
            id("registeredProviderName").innerHTML = '';
            id("registeredProviderId").innerHTML = '';
            id("registeredDataClass").innerHTML = '';
            id("networkAccountId").innerHTML = '';
            id("providerGuid").innerHTML = '';
            id("providerName").innerHTML = '';
            id("networkAdapterID").innerHTML = '';
            id("networktype").innerHTML = '';
        }
    }

    function updateDataButton() {
        // Array starts at zero while number of devices assumes stating at 1.
        var currentDevice = deviceSelected - 1;
        getCurrentDeviceInfo(deviceAccountID[currentDevice]);
        // Increment device count until the max number of devices is
        // reached and then start over.
        deviceSelected = deviceSelected + 1;
        if (deviceSelected > numberDevices) { deviceSelected = 1; }
        // Update button with next device.
        id("updateDataButton").innerText = "Get Information for Device #" + /*@static_cast(String)*/deviceSelected;
    }

    function runScenario2Sample() {
        WinJS.log && WinJS.log("No output available.", "sample", "status");
    }

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
})();
