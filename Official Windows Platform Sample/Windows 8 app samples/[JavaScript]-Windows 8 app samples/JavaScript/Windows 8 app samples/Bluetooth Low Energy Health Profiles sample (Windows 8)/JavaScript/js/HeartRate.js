//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var hrmService = null;
    var uiVisible = false;
    var hrmInitialized = false;

    var page = WinJS.UI.Pages.define("/html/HeartRate.html", {

        // This function gets called every time the user selects this particular scenario
        // so we should make sure that the initialization code gets called only once, while
        // the chart refresh code can get called multiple times
        ready: function (element, options) {
            // Initialize the HeartRate device scenario

            uiVisible = true;

            if (!hrmInitialized) {
                // initializeHeartRateDevices will set the value of hrmInitialized depending 
                // on whether devices were successfully initialized or not.
                initializeHeartRateDevicesAsync();
            }

            // Populate the UI with existing data
            UIHelper.refreshUI(Constants.heartRateDeviceShortName, Constants.heartRateDeviceIndex);
        },

        // This function gets called just before the current scenario gets unloaded
        // because the user chose a different scenario
        unload: function () {
            // Set the flag to not update the UI while this scenario is not visible
            uiVisible = false;
        }
    });

    function getDeviceReadingsAsync() {
        try {
            // The way the Async pattern is implemented is that when ReadHeartRateMeasurement
            // function returns, the Wpd Automation platform will invoke the onReadHeartRateMeasurementComplete
            // handler, set to retrievedReading below
            hrmService.ReadHeartRateMeasurement();
        } catch (ex) {
            onError(ex.toString());
            WinJS.log && WinJS.log(ex.toString(), "sample", "error");
        }
    }

    function retrievedReading(result) {
        try {
            var measuredValue = {
                // Create a Date object from the numerical timestamp provided to us by the driver
                timestamp: new Date(result.TimeStamp * 1000),
                value: result.Rate,

                // Override the default toString function
                toString: function () {
                    return this.value + ' bpm @ ' + this.timestamp;
                }
            };

            // Dispatch the retrieved measurement to update the application data and the associated view
            DeviceData.addValue(
                Constants.heartRateDeviceIndex,
                measuredValue);

            if (uiVisible) {
                UIHelper.updateUI(Constants.heartRateDeviceShortName, Constants.heartRateDeviceIndex, measuredValue);
            }

            // Query the driver for more data
            getDeviceReadingsAsync();
        } catch (exception) {
            WinJS.log && WinJS.log(exception.toString(), "sample", "error");
        }
    }

    function initializeHeartRateDevicesAsync() {
        // Initialize Heart Rate Devices
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync("System.Devices.InterfaceClassGuid:=\"" + Constants.heartRateUUID + "\"", null).
            done(function (devices) {
                // If devices were found, proceed with initialization
                if (devices.length > 0) {
                    try {
                        // Use WPD Automation to initialize the device objects
                        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");

                        var devServiceName = document.getElementById(Constants.deviceServiceName + Constants.heartRateDeviceShortName);
                        devServiceName.innerText = devices[0].name;

                        // For the purpose of this sample we will initialize the first device
                        deviceFactory.getDeviceFromIdAsync(devices[0].id, function (device) {

                            // Initialize the Heart Rate Monitor service
                            hrmService = device.services[0];

                            var devs = DeviceData.getDevices();
                            var devId = Constants.heartRateDeviceIndex;
                            devs[devId] = {
                                devId: devId,
                                name: devices[0].name,
                                description: devices[0].id,
                                data: []
                            };

                            // Rather than setting the handler for the complete method every time
                            // by using the traditional Promise based Async pattern
                            // we use a Wpd Automation feature to set the complete function only once
                            hrmService.onReadHeartRateMeasurementComplete = retrievedReading;
                            hrmInitialized = true;
                            getDeviceReadingsAsync();

                        }, function (errorCode) {
                            WinJS.log && WinJS.log("Getting the device failed with error: " + errorCode.toString(16), "sample", "error");
                        });

                    } catch (exception) {
                        WinJS.log && WinJS.log(exception.toString(), "sample", "error");
                    }
                } else {
                    // Update the UI to signal the scenario failure because of lack of connected devices
                    UIHelper.reportScenarioFailure(Constants.heartRateDeviceShortName, "Heart Rate Monitor");
                }
            });
    }

    WinJS.Namespace.define('HeartRateDevices',
    {
        initializaHeartRateDevicesAsync: initializeHeartRateDevicesAsync,
    });
})();
