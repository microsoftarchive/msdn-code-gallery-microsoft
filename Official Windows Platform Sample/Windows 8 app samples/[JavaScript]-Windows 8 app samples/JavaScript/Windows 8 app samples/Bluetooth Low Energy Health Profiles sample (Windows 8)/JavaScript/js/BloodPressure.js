//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var bpService = null;
    var uiVisible = false;
    var bpInitialized = false;

    var page = WinJS.UI.Pages.define("/html/BloodPressure.html", {
        ready: function (element, options) {
            // Initialize the BloodPressure device scenario

            uiVisible = true;

            if (!bpInitialized) {
                // initializeBloodPressureDevices will set the value of bpInitialized depending
                // on whether devices were successfully initialized or not.
                initializeBloodPressureDevicesAsync();
            }

            // Populate the UI with existing data
            UIHelper.refreshUI(Constants.bloodPressureDeviceShortName, Constants.bloodPressureDeviceIndex);
        },

        // This function gets called just before the current scenario gets unloaded
        // because the user chose a different scenario
        unload: function () {
            // Set the flag to not update the UI while this scenario is not visible
            uiVisible = false;
        }
    });

    function initializeBloodPressureDevicesAsync() {

        // Setup event handlers for application lifecycle events
        Windows.UI.WebUI.WebUIApplication.addEventListener('suspending', applicationSuspended);
        Windows.UI.WebUI.WebUIApplication.addEventListener('resuming', applicationActivated);

        // Initialize Blood Pressure Devices
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync("System.Devices.InterfaceClassGuid:=\"" + Constants.bloodPressureUUID + "\"", null).
            done(function (devices) {
                // If devices were found, proceed with initialization
                if (devices.length > 0) {
                    try {

                        // Use WPD Automation to initialize the device objects
                        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");

                        var devServiceName = document.getElementById('deviceServiceName' + Constants.bloodPressureDeviceShortName);
                        devServiceName.innerText = devices[0].name;


                        // For the purpose of this sample we will initialize the first device
                        deviceFactory.getDeviceFromIdAsync(devices[0].id, function (device) {

                            // Initialize the temperature service
                            bpService = device.services[0];

                            // Set up ApplicationActivated to be called asynchronously (fire-and-forget mode)
                            bpService.onApplicationActivatedComplete = function() {};

                            // Set up ApplicationSuspended to be called asynchronously (fire-and-forget mode)
                            bpService.onApplicationSuspendedComplete = function() {};

                            var devs = DeviceData.getDevices();
                            var devId = Constants.bloodPressureDeviceIndex;
                            devs[devId] = {
                                devId: devId,
                                name: devices[0].name,
                                description: devices[0].id,
                                data: [],
                                handler: function (timestamp, typeStr, systolic, diastolic, meanArterialPressure) {
                                    try {
                                        // Because the measurement is a multiple value measurement we simply wrap the values into an object for easier handling
                                        var measuredValue = {
                                            timestamp: new Date(timestamp * 1000),
                                            unitType: typeStr,
                                            systolic: systolic,
                                            diastolic: diastolic,
                                            meanArterialPressure: meanArterialPressure,
                                            toString: function () {
                                                return 'Systolic: ' + this.systolic + this.unitType + ', ' +
                                                    'Diastolic: ' + this.diastolic + this.unitType + ', ' +
                                                    'Mean Arterial Pressure: ' + this.meanArterialPressure + this.unitType +
                                                    ' @ ' + this.timestamp;
                                            },
                                        };

                                        // The measurement object is uniformly added to the datasets just like any other meaurement
                                        // Some specialized processing to update the UI is still required though because this
                                        // measurement is comprised of multiple numeric values
                                        DeviceData.addValue(
                                            Constants.bloodPressureDeviceIndex,
                                            measuredValue);

                                        if (uiVisible) {
                                            UIHelper.updateUI(Constants.bloodPressureDeviceShortName, Constants.bloodPressureDeviceIndex, measuredValue);
                                        }
                                    } catch (exception) {
                                        WinJS.log && WinJS.log(exception.toString(), "sample", "error");
                                    }
                                }
                            };

                            bpService.onBloodPressureMeasurement = devs[devId].handler;
                            bpInitialized = true;
                            bpService.ApplicationActivated();

                        }, function (errorCode) {
                            WinJS.log && WinJS.log("Getting the device failed with error: " + errorCode.toString(16), "sample", "error");
                        });

                    } catch (exception) {
                        WinJS.log && WinJS.log(exception.toString(), "sample", "error");
                    }
                } else {
                    // Update the UI to signal the scenario failure because of lack of connected devices
                    UIHelper.reportScenarioFailure(Constants.bloodPressureDeviceShortName, "Blood Pressure Monitor");
                }
            });
    }

    function applicationActivated() {
        try {
            // Invoking a Wpd Service Method to let the driver know that
            // the application has foreground and can receive notifications
            if (bpService !== null) {
                bpService.ApplicationActivated();
            }
        } catch (exception) {
            WinJS.log && WinJS.log(exception.toString(), "sample", "error");
        }
    }

    function applicationSuspended() {
        try {
            // Invoking a Wpd Service Method to let the driver know that
            // the application has been suspended and won't receive notifications
            if (bpService !== null) {
                bpService.ApplicationSuspended();
            }
        } catch (exception) {
            WinJS.log && WinJS.log(exception.toString(), "sample", "error");
        }
    }

    WinJS.Namespace.define('BloodPressureDevices',
    {
        initializeBloodPressureDevicesAsync: initializeBloodPressureDevicesAsync,
        applicationActivated: applicationActivated,
        applicationSuspended: applicationSuspended
    });
})();
