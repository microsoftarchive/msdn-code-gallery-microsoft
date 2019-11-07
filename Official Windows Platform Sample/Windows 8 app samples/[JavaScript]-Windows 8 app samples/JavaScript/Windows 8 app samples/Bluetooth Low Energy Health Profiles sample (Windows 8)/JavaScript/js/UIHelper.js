//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    'use strict';

    function refreshUI(deviceShortName, deviceIndex) {
        /// <summary>
        /// The function will repopulate the UI with the data previously gathered from the device,
        /// when the user selects this particular scenario and the page is reloaded.
        /// </summary>
        /// <param name="deviceShortName" type="String">
        /// A string identifier used to uniquely identify the HTML elements used for this particular
        /// device scenario.
        /// </param>
        /// <param name="deviceIndex" type="Number">
        /// A numerical identifier used as a numerical device ID for indexing relevant data structures.
        /// </param>
        var devices = DeviceData.getDevices();

        var dataChart = new Chart.renderer();

        if (devices[deviceIndex]) {

            // Update the device service name
            var deviceServiceName = document.getElementById(Constants.deviceServiceName + deviceShortName);
            deviceServiceName.innerText = devices[deviceIndex].name;

            // If data has been received from the device
            if (devices[deviceIndex].data.length > 0) {

                // Update the latest reading received from the device
                var latestReadingElement = document.getElementById(Constants.lastMeasurementElement + deviceShortName);
                latestReadingElement.innerText = devices[deviceIndex].data[devices[deviceIndex].data.length - 1].toString();

                // Update the reading history of the device
                var valueHistoryElement = document.getElementById(Constants.dataPointContent + deviceShortName);

                // Populate the history list with the existing data
                for (var i in devices[deviceIndex].data) {
                    // Create new list item
                    var newItem = document.createElement('option');
                    newItem.innerText = devices[deviceIndex].data[i].toString();

                    valueHistoryElement.appendChild(newItem);
                }
            } else {
                var lastMeasurementValue = document.getElementById(Constants.lastMeasurementElement + deviceShortName);
                lastMeasurementValue.innerText = "No Data has been received from the device";
            }

            // Render the chart

            // Update the chart of the device
            // (if no data has been received yet this will still update the chart background)
            if (deviceIndex === Constants.bloodPressureDeviceIndex) {
                dataChart.plotBP(Constants.chartId + deviceShortName, devices[deviceIndex].data);
            } else {
                dataChart.plot(Constants.chartId + deviceShortName, devices[deviceIndex].data);
            }
        } else {
            dataChart.plot(Constants.chartId + deviceShortName, null);
        }
    }

    function updateUI(deviceShortName, deviceIndex, newValue) {
        /// <summary>
        /// The function will incrementally update the UI with the latest data received from the device.
        /// This function will only get called while the scenario page is loaded, thus avoiding updating
        /// the UI elements while the page is not visible.
        /// </summary>
        /// <param name="deviceShortName" type="String">
        /// A string identifier used to uniquely identify the HTML elements used for this particular
        /// device scenario.
        /// </param>
        /// <param name="deviceIndex" type="Number">
        /// A numerical identifier used as a numerical device ID for indexing relevant data structures.
        /// </param>
        /// <param name="newValue" type="Object">
        /// The data received from the device, encapsulated in a JavaScript object.
        /// </param>
        var valueHistoryElement = document.getElementById(Constants.dataPointContent + deviceShortName);

        // Create new list item
        var newItem = document.createElement('option');
        newItem.innerText = newValue.toString();

        // Add the new reading to the history list
        valueHistoryElement.appendChild(newItem);

        var devices = DeviceData.getDevices();

        var dataChart = new Chart.renderer();

        // If the device has been initialized
        if (devices[deviceIndex]) {
            // If data has been received from the device
            var lastMeasurementValue = null;

            if (devices[deviceIndex].data.length > 0) {

                // Update the UI
                var readingStr = devices[deviceIndex].data[devices[deviceIndex].data.length - 1].toString();

                // Update the latest reading element for the device
                lastMeasurementValue = document.getElementById(Constants.lastMeasurementElement + deviceShortName);
                lastMeasurementValue.innerText = readingStr;

            } else {
                lastMeasurementValue = document.getElementById(Constants.lastMeasurementElement + deviceShortName);
                lastMeasurementValue.innerText = "No Data has been received from the device";
            }

            // Update the chart of the device
            // (if no data has been received yet this will still update the chart background)
            if (deviceIndex === Constants.bloodPressureDeviceIndex) {
                dataChart.plotBP(Constants.chartId + deviceShortName, devices[deviceIndex].data);
            } else {
                dataChart.plot(Constants.chartId + deviceShortName, devices[deviceIndex].data);
            }
        } else {
            // Just refresh the chart background
            dataChart.plot(Constants.chartId + deviceShortName, null);
        }
    }

    function reportScenarioFailure(deviceShortName, scenarioDeviceName) {
        // Remove the UI elements for normal device operation
        var hrmOutput = document.getElementById(Constants.deviceContent + deviceShortName);
        while (hrmOutput.childNodes.length > 0) {
            hrmOutput.removeChild(hrmOutput.childNodes[0]);
        }

        // Report device status to the user
        WinJS.log && WinJS.log("No " + scenarioDeviceName + " Devices Found !", "sample", "status");
    }

    WinJS.Namespace.define('UIHelper', {
        refreshUI: refreshUI,
        updateUI: updateUI,
        reportScenarioFailure: reportScenarioFailure
    });
})();
