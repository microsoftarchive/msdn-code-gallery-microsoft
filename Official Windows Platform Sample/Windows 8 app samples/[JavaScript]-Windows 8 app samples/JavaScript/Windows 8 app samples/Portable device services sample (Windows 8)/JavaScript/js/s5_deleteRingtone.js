//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s5_deleteRingtone.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", findAndDisplayRingtonesFromDevice, false);
        }
    });

    function findAndDisplayRingtonesFromDevice() {
        // Find all devices that support the ringtones service
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            Windows.Devices.Portable.ServiceDevice.getDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.ringtonesService),
            null)
                .done(
                    function (ringtonesServices) {
                        // Send a ringtone to the selected device
                        if (ringtonesServices.length) {
                            SdkSample.showItemSelector(ringtonesServices, displayRingtoneSelections);
                        } else {
                            WinJS.log && WinJS.log("No ringtone services found. Please connect a device that supports the Ringtones Service to the system", "sample", "status");
                        }
                    },
                    function (e) {
                        WinJS.log && WinJS.log("Failed to find all ringtones services, error: " + e.message, "sample", "error");
                    });
    }

    function displayRingtoneSelections(deviceInfoElement) {
        var deviceName = deviceInfoElement.name;
        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");
        deviceFactory.getDeviceFromIdAsync(deviceInfoElement.id,
            function (device) {
                // Get the first ringtone service on that device
                var ringtoneService = device.services[0];

                // Find all mp3 ringtones on the service
                ringtoneService.onGetChildrenByFormatComplete = function (errorCode, ringtones) {
                    if (errorCode === 0) {
                        if (ringtones.count) {
                            SdkSample.showItemSelector(ringtones, deleteRingtone);
                        } else {
                            WinJS.log && WinJS.log("No ringtones found on " + deviceName + ". Please send a ringtone using scenario 3", "sample", "status");
                        }
                    } else {
                        WinJS.log && WinJS.log("Failed to find ringtones on " + deviceName + ", error: " + errorCode.toString(16), "sample", "error");
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

    function deleteRingtone(ringtone) {
        var ringtoneName = ringtone.objectName;
        ringtone.parent.removeChild(ringtone);
        WinJS.log && WinJS.log("Deleted '" + ringtoneName + "' from device", "sample", "status");
    }

})();
