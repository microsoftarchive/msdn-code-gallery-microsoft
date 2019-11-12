//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    'use strict';

    WinJS.Namespace.define('Constants',
    {
        title:                          'Bluetooth Low Energy Health Profiles Sample',

        chartId:                        'chartCanvas',
        dataPointContent:               'dataPointContent',
        deviceServiceName:              'deviceServiceName',
        deviceContent:                  'deviceContent',
        lastMeasurementElement:         'lastMeasuredValue',

        // Arbitrary device indexes used for synchronizing the device events with their data and views
        heartRateDeviceIndex:           0,
        thermometerDeviceIndex:         1,
        bloodPressureDeviceIndex:       2,

        // Arbitrary device monikers used to identify device specific html elements
        heartRateDeviceShortName:       'HRM',
        thermometerDeviceShortName:     'Temp',
        bloodPressureDeviceShortName:   'BP',

        // Universally Unique Identifiers specific to each type of device as defined in the Bluetooth Low Energy Specification
        heartRateUUID:                  '{0000180D-0000-1000-8000-00805f9b34fb}',
        thermometerUUID:                '{00001809-0000-1000-8000-00805f9b34fb}',
        bloodPressureUUID:              '{00001810-0000-1000-8000-00805f9b34fb}'
    });
})();
