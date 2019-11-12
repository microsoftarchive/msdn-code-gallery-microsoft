//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    
    var page = WinJS.UI.Pages.define("/html/scenario2_ReadCharacteristicValue.html", {
        ready: function (element, options) {
            document.getElementById("readValueButton").addEventListener("click", onReadValueButtonClick, false);

            if (HeartRateService.isServiceInitialized()) {
                readValueButton.disabled = false;
            }
        }
    });

    function onReadValueButtonClick() {
        readValueButton.disabled = true;
        var gatt = Windows.Devices.Bluetooth.GenericAttributeProfile;
        var bodySensorLocationCharacteristics = HeartRateService.getHeartRateService().getCharacteristics(
            gatt.GattCharacteristicUuids.bodySensorLocation);

        if (bodySensorLocationCharacteristics.length > 0) {
            bodySensorLocationCharacteristics[0].readValueAsync().done(
                function (readResult) {
                    if (readResult.status === gatt.GattCommunicationStatus.success) {
                        var bodySensorLocationData = new Uint8Array(readResult.value.length);
                        Windows.Storage.Streams.DataReader.fromBuffer(readResult.value).readBytes(bodySensorLocationData);
                        var bodySensorLocation = HeartRateService.processBodySensorLocation(bodySensorLocationData);
                        var readStatus = document.getElementById("readStatusLabel");
                        if (bodySensorLocation !== "") {
                            readStatus && (readStatus.innerText = "The Body Sensor Location of your device is : " +
                                bodySensorLocation);
                        } else {
                            readStatus && (readStatus.innerText = "The Body Sensor Location cannot be interpreted");
                        }
                    }
                    var readButton = document.getElementById("readValueButton");
                    readButton && (readButton.disabled = false);
                }, function (error) {
                    WinJS.log && WinJS.log("Reading the Body Sensor Location characteristic value failed with error: " +
                        error.toString(), "sample", "error");
                    var readButton = document.getElementById("readValueButton");
                    readButton && (readButton.disabled = false);
                });
        }
    }
})();
