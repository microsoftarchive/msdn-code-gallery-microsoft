//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s3_sendRingtone.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", scenario3_sendRingtoneToDevice, false);
        }
    });

    function scenario3_sendRingtoneToDevice() {
        // Find all devices that support the ringtones service
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            Windows.Devices.Portable.ServiceDevice.getDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.ringtonesService),
            null)
                .done(
                    function (ringtonesServices) {
                        // Send a ringtone to the selected device
                        if (ringtonesServices.length) {
                            SdkSample.showItemSelector(ringtonesServices, sendRingtoneToDevice);
                        } else {
                            WinJS.log && WinJS.log("No ringtone services found. Please connect a device that supports the Ringtones Service to the system", "sample", "status");
                        }
                    },
                    function (e) {
                        WinJS.log && WinJS.log("Failed to find all ringtones services, error: " + e.message, "sample", "error");
                    });
    }

    function sendRingtoneToDevice(deviceInfoElement) {
        var deviceName = deviceInfoElement.name;
        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");
        deviceFactory.getDeviceFromIdAsync(deviceInfoElement.id,
            function (device) {
                // Get the first ringtone service on that device
                var ringtoneService = device.services[0];

                // Launch the file open picker to select a ringtone file to transfer to the device
                var sourceFileName;
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.fileTypeFilter.replaceAll([".mp3"]);
                picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
                picker.pickSingleFileAsync()
                    .then(
                // Open the selected file to get a read-only stream
                        function (sourceFile) {
                            // SourceFile is null if the user clicks cancel in the picker
                            if (sourceFile) {
                                sourceFileName = sourceFile.displayName;
                                return sourceFile.openAsync(Windows.Storage.FileAccessMode.read);
                            }
                            return null;
                        },
                        function (e) {
                            WinJS.log && WinJS.log("Failed to select a music file, error: " + e.message, "sample", "error");
                        })
                    .done(
                // Create a new object on the ringtone service, passing it data from the source stream
                        function (sourceStream) {
                            if (sourceStream) {
                                var container = ringtoneService.createNewObject("mp3");
                                container.data = sourceStream;
                                container.name = sourceFileName;
                                container.onTransferProgress = function (numBytesTransferred, numBytesTotal) {
                                    WinJS.log && WinJS.log("Copying '" + container.name + "' to " + deviceName + " (" + numBytesTransferred + " bytes of " + numBytesTotal + " bytes)", "sample", "status");
                                };
                                ringtoneService.onAddChildComplete = function (errorCode, child) {
                                    if (errorCode === 0) {
                                        WinJS.log && WinJS.log("Transferred '" + child.name + "' (" + child.objectSize + " bytes) to " + deviceName, "sample", "status");
                                    } else {
                                        WinJS.log && WinJS.log("Failed to send a ringtone to " + deviceName + ", error: " + errorCode.toString(16), "sample", "error");
                                    }
                                };
                                ringtoneService.addChild(container);
                            } else {
                                WinJS.log && WinJS.log("No music file selected", "sample", "status");
                            }
                        },
                        function (e) {
                            WinJS.log && WinJS.log("Failed to open the selected music file, error: " + e.message, "sample", "error");
                        });
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
