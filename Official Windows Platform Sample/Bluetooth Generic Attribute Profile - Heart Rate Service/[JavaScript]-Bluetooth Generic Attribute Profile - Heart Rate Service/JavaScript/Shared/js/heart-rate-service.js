//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    'use strict';

    // The format for the objects in the data array will be : 
    // {timestamp: , value: , expendedEnergy: , toString: }
    var data = [];

    var service;
    var characteristic;
    var watcher;
    var containerId;
    var serviceInitialized = false;
    var app = WinJS.Application;
    var gatt = Windows.Devices.Bluetooth.GenericAttributeProfile;
    var pnp = Windows.Devices.Enumeration.Pnp;

    app.addEventListener("oncheckpoint", function (args) {
        serviceInitialized = false;

        // This is an appropriate place to save to persistent storage any datapoint the application cares about.
        // For the purpose of this sample we just discard any values.
        data = [];

        // This application is about to be suspended, the application should release all resources at this point.
        if (service) {
            service.close();
            service = null;
        }

        if (characteristic) {
            characteristic = null;
        }

        if (watcher) {
            watcher.stop();
            watcher = null;
        }
    });

    function processBodySensorLocationData(bodySensorLocationData) {
        var bodySensorLocationValue = bodySensorLocationData[0];

        var retval;

        switch (bodySensorLocationValue) {
            case 0:
                retval = "Other";
                break;
            case 1:
                retval = "Chest";
                break;
            case 2:
                retval = "Wrist";
                break;
            case 3:
                retval = "Finger";
                break;
            case 4:
                retval = "Hand";
                break;
            case 5:
                retval = "Ear Lobe";
                break;
            case 6:
                retval = "Foot";
                break;
            default:
                retval = "";
                break;
        }
        return retval;
    }

    function processHeartRateMeasurementData(heartRateData) {
        var HEART_RATE_VALUE_FORMAT = 0x01;
        var ENERGY_EXPENDED_STATUS = 0x08;

        var currentOffset = 0;
        var flags = heartRateData[currentOffset];
        var isHeartRateValueSizeLong = ((flags & HEART_RATE_VALUE_FORMAT) !== 0);
        var hasEnergyExpended = ((flags & ENERGY_EXPENDED_STATUS) !== 0);

        currentOffset++;

        var heartRateMeasurementValue;

        if (isHeartRateValueSizeLong) {
            heartRateMeasurementValue = (heartRateData[currentOffset + 1] << 8) + heartRateData[currentOffset];
            currentOffset += 2;
        } else {
            heartRateMeasurementValue = heartRateData[currentOffset];
            currentOffset++;
        }

        var expendedEnergyValue;

        if (hasEnergyExpended) {
            expendedEnergyValue = (heartRateData[currentOffset + 1] << 8) + heartRateData[currentOffset];
            currentOffset += 2;
        }

        return {
            heartRateValue: heartRateMeasurementValue,
            energyExpended: expendedEnergyValue
        };
    }

    function onHeartRateMeasurementValueChanged(args) {
        var heartRateData = new Uint8Array(args.characteristicValue.length);

        Windows.Storage.Streams.DataReader.fromBuffer(args.characteristicValue).readBytes(heartRateData);

        // Interpret the Heart Rate measurement value according to the Heart Rate Bluetooth Profile
        var heartRateMeasurement = processHeartRateMeasurementData(heartRateData);

        data[data.length] = {
            timestamp: args.timestamp,
            value: heartRateMeasurement.heartRateValue,
            expendedEnergy: heartRateMeasurement.energyExpended,
            toString: function () {
                return this.value + ' bpm @ ' + this.timestamp;
            }
        };

        var evt = document.createEvent("CustomEvent");
        evt.initCustomEvent("onValueChanged", true, true, {
            value: heartRateMeasurement
        });
        document.dispatchEvent(evt);
    }

    function initializeHeartRateServiceAsync(device) {
        containerId = "{" + device.properties["System.Devices.ContainerId"] + "}";

        return gatt.GattDeviceService.fromIdAsync(device.id).then(
            function (deviceService) {
                if (deviceService) {
                    service = deviceService;
                    serviceInitialized = true;
                    return configureServiceForNotificationsAsync();
                } else {
                    WinJS.log && WinJS.log("Access to the device is denied, because the application was not " +
                        "granted access, or the device is currently in use by another application.", "sample",
                        "status");
                    return;
                }
        }, function (error) {
            WinJS.log && WinJS.log(error, "sample", "status");
        });
}

    function configureServiceForNotificationsAsync() {
        // The Heart Rate profile states that there must be one HeartRateMeasurement characteristic for the service
        characteristic = service.getCharacteristics(gatt.GattCharacteristicUuids.heartRateMeasurement)[0];

        // While encryption is not required by all devices, if encryption is supported by the device,
        // it can be enabled by setting the ProtectionLevel property of the Characteristic object.
        // All subsequent operations on the characteristic will work over an encrypted link.
        characteristic.protectionLevel = gatt.GattProtectionLevel.encryptionRequired;

        // Register the event handler for receiving device notification data
        characteristic.addEventListener("valuechanged", onHeartRateMeasurementValueChanged, false);

        // In order to avoid unnecessary communication with the device, determine if the device is already 
        // correctly configured to send notifications.
        // By default ReadClientCharacteristicConfigurationDescriptorAsync will attempt to get the current
        // value from the system cache and communication with the device is not typically required.
        return characteristic.readClientCharacteristicConfigurationDescriptorAsync().then(
            function (currentDescriptorValue) {
                if ((currentDescriptorValue.status !== gatt.GattCommunicationStatus.success) ||
                    (currentDescriptorValue.clientCharacteristicConfigurationDescriptor !==
                        gatt.GattClientCharacteristicConfigurationDescriptorValue.notify)) {

                    // Set the Client Characteristic Configuration Descriptor to enable the device to send
                    // notifications when the Characteristic value changes.
                    return characteristic.writeClientCharacteristicConfigurationDescriptorAsync(
                        gatt.GattClientCharacteristicConfigurationDescriptorValue.notify).then(
                            function (communicationStatus) {
                                if (communicationStatus === gatt.GattCommunicationStatus.unreachable) {
                                    // Register a PnpObjectWatcher to detect when a connection to the device is
                                    // established, such that the application can retry device configuration.
                                    startDeviceConnectionWatcher();
                                }
                            });
                }
                return;
            });
    }

    function startDeviceConnectionWatcher() {
        watcher = pnp.PnpObject.createWatcher(pnp.PnpObjectType.deviceContainer, ["System.Devices.Connected"], "");
        watcher.onupdated = onDeviceConnectionUpdated;
        watcher.start();
    }

    function onDeviceConnectionUpdated(e) {
        var isConnected = e.properties["System.Devices.Connected"];
        if ((e.id === containerId) && (isConnected === true)) {
            // Set the Client Characteristic Configuration descriptor on the device, 
            // registering for Characteristic Value Changed notifications.
            characteristic.writeClientCharacteristicConfigurationDescriptorAsync(
                gatt.GattClientCharacteristicConfigurationDescriptorValue.notify).done(
                    function (communicationStatus) {
                        if (communicationStatus === gatt.GattCommunicationStatus.success) {
                            serviceInitialized = true;

                            watcher.stop();
                            watcher = null;
                        }

                        var evt = document.createEvent("CustomEvent");
                        evt.initCustomEvent("onDeviceConnectionUpdated", true, true, {
                            isConnected: isConnected
                        });
                        document.dispatchEvent(evt);
                    }, function (error) {
                        WinJS.log && WinJS.log(error, "sample", "status");
                    });
        }
    }

    function isServiceInitialized() {
        return serviceInitialized;
    }

    function getHeartRateService() {
        return service;
    }

    function getData() {
        return data;
    }

    WinJS.Namespace.define('HeartRateService', {
        initializeHeartRateServiceAsync: initializeHeartRateServiceAsync,
        processBodySensorLocation: processBodySensorLocationData,
        isServiceInitialized: isServiceInitialized,
        getHeartRateService: getHeartRateService,
        getData: getData
    });
})();
