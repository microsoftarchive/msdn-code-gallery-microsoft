//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1_DeviceEvents.html", {
        ready: function (element, options) {
            devicesList.onchange = onDevicesListSelect;
            runButton.addEventListener("click", onRunButtonClick, false);
            document.addEventListener("onValueChanged", onValueChanged, false);
            document.addEventListener("onDeviceConnectionUpdated", onDeviceConnectionUpdated, false);

            if (HeartRateService.isServiceInitialized()) {
                runButton.disabled = true;

                loadExistingData();
            }
        },
        unload: function (element, options) {
            document.removeEventListener("onValueChanged", onValueChanged, false);
        }
    });

    function loadExistingData() {
        outputData.style.display = "";
        var data = HeartRateService.getData();
        dataChart.makeVisible("outputDataChart");
        dataChart.plot(data);
        for (var i = 0; i < data.length; i++) {
            var measurementElement = document.createElement("option");
            measurementElement.innerText = data[i].toString();
            outputSelect.appendChild(measurementElement);
        }
    }

    var gatt = Windows.Devices.Bluetooth.GenericAttributeProfile;
    var pnp = Windows.Devices.Enumeration.Pnp;
    var dataChart = new Chart.renderer();
    var gattDevices;

    function onRunButtonClick() {
        runButton.disabled = true;

        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            gatt.GattDeviceService.getDeviceSelectorFromUuid(gatt.GattServiceUuids.heartRate),
            ["System.Devices.ContainerId"]).done(
                function (devices) {
                    if (devices.length > 0) {
                        gattDevices = devices;

                        while (devicesList.firstChild) {
                            devicesList.removeChild(devicesList.firstChild);
                        }

                        for (var i = 0; i < devices.length; i++) {
                            var device = devices[i];
                            var deviceElement = document.createElement("option");
                            deviceElement.innerText = device.name;
                            devicesList.appendChild(deviceElement);
                        }

                        devicesList.selectedIndex = -1;

                        // Make the device selector visible
                        deviceSelector.style.display = "";
                    } else {
                        WinJS.log && WinJS.log("Could not find any Heart Rate devices. Please make sure your device " +
                            "is paired and powered on!", "sample", "status");
                    }
                }, function (error) {
                    WinJS.log && WinJS.log("Finding Heart Rate devices failed with error :" + error, "sample", "error");
                });

        runButton.disabled = false;
    }

    function onDevicesListSelect() {
        runButton.disabled = true;
        deviceSelector.style.display = "none";

        statusLabel.textContent = "Initializing device...";
        var device = gattDevices[devicesList.selectedIndex];
        HeartRateService.initializeHeartRateServiceAsync(device).done(
            function () {
                if (HeartRateService.isServiceInitialized()) {
                    var output = document.getElementById("outputData");
                    output && (output.style.display = "");
                    dataChart.makeVisible("outputDataChart");
                    var deviceContainerId = device.properties["System.Devices.ContainerId"].toString();
                    pnp.PnpObject.createFromIdAsync(pnp.PnpObjectType.deviceContainer, deviceContainerId,
                        ["System.Devices.Connected"]).done(
                        function (deviceObject) {
                            var isConnected = deviceObject.properties["System.Devices.Connected"];
                            onDeviceConnectionUpdated({ detail: { isConnected: isConnected } });
                        });
                }
            }, function (error) {
                WinJS.log && WinJS.log("Failed to initialize the GattDeviceService :" + error, "sample", "error");
            });
    }

    function onValueChanged(args) {
        var heartRateMeasurement = args.detail.value.heartRateValue;

        var data = HeartRateService.getData();

        dataChart.plot(data);

        var measurementElement = document.createElement("option");
        measurementElement.innerText =  data[data.length - 1].toString();
        outputSelect.appendChild(measurementElement);

        document.getElementById("statusLabel").innerText = 
            "Latest received heart rate measurement: " + heartRateMeasurement.toString();
    }

    function onDeviceConnectionUpdated(args) {
        var isConnected = args.detail.isConnected;
        var statusElement = document.getElementById("statusLabel");
        if (isConnected) {
            statusElement && (statusElement.textContent = "Waiting for device to send data...");
        } else {
            statusElement && (statusElement.textContent = "Waiting for device to connect...");
        }
    }
})();
