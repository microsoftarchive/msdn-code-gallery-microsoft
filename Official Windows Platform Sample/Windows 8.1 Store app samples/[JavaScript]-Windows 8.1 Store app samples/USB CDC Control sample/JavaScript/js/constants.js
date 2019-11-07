//// Copyright (c) Microsoft Corporation. All rights reserved

/// <dictionary target='member'>dte</dictionary>

(function () {
    "use strict";
    WinJS.Namespace.define("SdkSample.Constants", {
        infiniteTimeout: -1,
        enoughLongTimeout: 3600 * 1000,
        dteRateTestValue: 4800,
        dataBitsTestValue: 8,
        expectedResultSetLineCoding: 7,
        expectedResultGetLineCoding: 7,
        interfaceClass: {
            cdcControl: 2,
            cdcData: 10,
            vendorSpecific: 255
        },
        requestType: {
            set: 0x21,
            get: 0xA1
        },
        requestCode: {
            setLineCoding: 0x20,
            getLineCoding: 0x21,
            setControlLineState: 0x22,
            sendBreak: 0x23
        },
        parity: {
            none: 0,
            odd: 1,
            even: 2,
            mark: 3,
            space: 4
        },
        supportedDevices: [
            {
                vid: 0x056e,
                pid: 0x5003
            },
            {
                vid: 0x056e,
                pid: 0x5004
            }
        ]
    });
})();
