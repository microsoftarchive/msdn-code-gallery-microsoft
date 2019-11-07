//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s1_deviceStatus.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", scenario1_showDeviceStatus, false);
            WinJS.log && WinJS.log();
        }
    });

    function scenario1_showDeviceStatus() {
        // Find all devices that support the device status service
        document.getElementById("scenarioOutput").innerHTML = "";
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            Windows.Devices.Portable.ServiceDevice.getDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.deviceStatusService),
            null)
                .done(
                    function (statusServices) {
                        // Display device status for the selected device
                        if (statusServices.length) {
                            SdkSample.showItemSelector(statusServices, showDeviceStatus);
                        } else {
                            WinJS.log && WinJS.log("No device status services found. Please connect a device that supports the Device Status Service to the system", "sample", "status");
                        }
                    },
                    function (e) {
                        WinJS.log && WinJS.log("Failed to find all device status services, error: " + e.message, "sample", "error");
                    });
    }

    function showDeviceStatus(deviceInfoElement) {
        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");
        deviceFactory.getDeviceFromIdAsync(deviceInfoElement.id,
            function (device) {
                // There is typically 1 status service per device
                var statusService = device.services[0];

                var scenarioOutput = document.getElementById("scenarioOutput");
                scenarioOutput.innerHTML = "<h3>" + device.deviceFriendlyName + " status<h3/>";
                scenarioOutput.innerHTML += "Manufacturer: " + device.deviceManufacturer + "<br />";

                // Note: the names below need to match the Service Property Description
                scenarioOutput.innerHTML += "Missed calls: " + statusService.missedCalls + "<br />";
                scenarioOutput.innerHTML += "New text messages: " + statusService.textMessages + "<br />";
                scenarioOutput.innerHTML += "Voice mail: " + statusService.voiceMail + "<br />";
                scenarioOutput.innerHTML += "Network: " + statusService.networkName + "<br />";
                scenarioOutput.innerHTML += "Network type: " + statusService.networkType + "<br />";
                scenarioOutput.innerHTML += "Roaming: " + statusService.roaming + "<br />";
                scenarioOutput.innerHTML += "Storage capacity: " + statusService.storageCapacity + "<br />";
                scenarioOutput.innerHTML += "Battery life: " + statusService.batteryLife + "<br />";
                scenarioOutput.innerHTML += "Charging state: " + statusService.chargingState + "<br />";
            },
            function (errorCode) {
                var E_ACCESSDENIED = 2147942405; // 0x80070005
                if (errorCode === E_ACCESSDENIED) {
                    WinJS.log && WinJS.log("Access to " + deviceInfoElement.name + " is denied. Only a Privileged Application may access this device.", "sample", "error");
                } else {
                    WinJS.log && WinJS.log("Failed to get the selected device, error: " + errorCode.toString(16), "sample", "error");
                }
            });
    }

})();
