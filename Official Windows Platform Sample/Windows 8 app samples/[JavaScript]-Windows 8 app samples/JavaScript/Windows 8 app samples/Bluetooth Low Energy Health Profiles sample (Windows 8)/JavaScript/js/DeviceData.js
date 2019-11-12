//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    'use strict';

    // The data format for the devices array will be : {name: , description: , datapoints: [] }
    var devices = [];

    function getDevices() {
        return devices;
    }

    function addValue(deviceIndex, newValue) {
        devices[deviceIndex].data[devices[deviceIndex].data.length] = newValue;
    }

    WinJS.Namespace.define('DeviceData', {
        getDevices: getDevices,
        addValue: addValue
    });
})();
