//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s2_listRingtones.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", scenario2_showRingtones, false);
        }
    });

    function scenario2_showRingtones() {
        // Find all devices that support the ringtones service
        document.getElementById("scenarioOutput").innerHTML = "";
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            Windows.Devices.Portable.ServiceDevice.getDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.ringtonesService),
            null)
                .done(
                    function (ringtonesServices) {
                        // Display ringtones for the selected device
                        if (ringtonesServices.length) {
                            SdkSample.showItemSelector(ringtonesServices, showRingtones);
                        } else {
                            WinJS.log && WinJS.log("No ringtone services found. Please connect a device that supports the Ringtones Service to the system", "sample", "status");
                        }
                    },
                    function (e) {
                        WinJS.log && WinJS.log("Failed to find all ringtones services, error: " + e.message, "sample", "error");
                    });
    }

    function showRingtones(deviceInfoElement) {
        var deviceName = deviceInfoElement.name;
        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");
        deviceFactory.getDeviceFromIdAsync(deviceInfoElement.id,
            function (device) {
                // Get the first ringtone service on that device
                var ringtoneService = device.services[0];

                // Find all mp3 ringtones on the service
                ringtoneService.onGetChildrenByFormatComplete = function (errorCode, ringtones) {
                    if (errorCode === 0) {
                        var scenarioOutput = document.getElementById("scenarioOutput");
                        var numRingtones = ringtones.count;
                        scenarioOutput.innerHTML = "<h3>" + deviceName + " ringtones (" + numRingtones + " found)<h3/>";
                        for (var j = 0; j < numRingtones; j++) {
                            var ringtone = ringtones[j];
                            scenarioOutput.innerHTML += ringtone.objectName + " (" + ringtone.objectSize + " bytes)<br />";
                        }
                    } else {
                        WinJS.log && WinJS.log("Failed to list ringtones on " + deviceName + ", error: " + errorCode.toString(16), "sample", "error");
                    }
                };
                ringtoneService.getChildrenByFormat("mp3");
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
