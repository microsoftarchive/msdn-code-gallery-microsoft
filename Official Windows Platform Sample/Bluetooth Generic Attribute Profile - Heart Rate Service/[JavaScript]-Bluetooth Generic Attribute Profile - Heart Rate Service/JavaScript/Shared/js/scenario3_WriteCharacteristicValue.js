//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var gatt = Windows.Devices.Bluetooth.GenericAttributeProfile;
    var characteristic;
    
    var page = WinJS.UI.Pages.define("/html/scenario3_WriteCharacteristicValue.html", {
        ready: function (element, options) {
            writeCharacteristicValueButton.addEventListener("click", onWriteCharacteristicValueButtonClick, false);
            document.addEventListener("onValueChanged", onValueChanged, false);

            if (HeartRateService.isServiceInitialized()) {
                var heartRateControlPointCharacteristics = HeartRateService.getHeartRateService().getCharacteristics(
                    gatt.GattCharacteristicUuids.heartRateControlPoint);

                if (heartRateControlPointCharacteristics.length > 0) {
                    characteristic = heartRateControlPointCharacteristics[0];
                    writeCharacteristicValueButton.disabled = false;
                } else {
                    WinJS.log && WinJS.log("Your device does not support the Expended Energy characteristic.",
                        "sample", "status");
                }
            }
        },
        unload: function (element, options) {
            document.removeEventListener("onValueChanged", onValueChanged, false);
        }
    });

    function onValueChanged(args) {
        var expendedEnergy = args.detail.value.energyExpended;
        var latestExpendedEnergyValue = document.getElementById("expendedEnergyValue");

        if (expendedEnergy) {
            latestExpendedEnergyValue.innerText = "Expended Energy: " + expendedEnergy;
        }
    }

    function onWriteCharacteristicValueButtonClick() {

        writeCharacteristicValueButton.disabled = true;

        var writer = new Windows.Storage.Streams.DataWriter();
        writer.writeByte(1);

        characteristic.writeValueAsync(writer.detachBuffer()).done(
            function (communicationStatus) {
                if (communicationStatus === gatt.GattCommunicationStatus.success) {
                    var expendedEnergyStatus = document.getElementById("expendedEnergyResetStatus");
                    expendedEnergyStatus && (expendedEnergyStatus.textContent = "Expended Energy successfully reset.");
                } else {
                    WinJS.log && WinJS.log("Your device is unreachable, most likely the device is out of range, " +
                        "or is running low on battery, please make sure your device is working and try again.",
                        "sample", "status");
                }
                var writeButton = document.getElementById("writeCharacteristicValueButton");
                writeButton && (writeButton.disabled = false);
            }, function (error) {
                WinJS.log && WinJS.log("Writing the Heart Rate Control Point characteristic value failed with error :" +
                    error.toString(), "sample", "error");
                var writeButton = document.getElementById("writeCharacteristicValueButton");
                writeButton && (writeButton.disabled = false);
            });
    }
})();
