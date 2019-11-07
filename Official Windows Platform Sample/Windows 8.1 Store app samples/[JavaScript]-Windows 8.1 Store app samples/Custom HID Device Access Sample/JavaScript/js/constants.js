//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    WinJS.Namespace.define("SdkSample.Constants", {
        sampleNamespace: "SdkSample.CustomHidDeviceAccess",
        maxUint32Value: 0xFFFFFFFF,
        deviceType: {
            superMutt: 1,
            all: 2,
            none: 3
        },
        deviceProperties: {
            deviceInstanceId: "System.Devices.DeviceInstanceId",
        },
        /// <summary>
        /// This is the report descriptor for the support SuperMutt device
        ///
        /// 0x06, 0xAA, 0xFF,             // USAGE_PAGE (Vendor Defined Page 1)
        /// 0x09, 0x01,                   // USAGE (Vendor Usage  1)
        /// 0xa1, 0x01,                   // COLLECTION (Application)
        /// 0x85, 0x01,                   //     REPORT_ID (1)
        /// 0xa1, 0x00,                   //  COLLECTION (Physical)
        /// 0x05, 0x09,                   //     USAGE_PAGE (Button)
        /// 0x19, 0x01,                   //     USAGE_MINIMUM (Button 1)
        /// 0x29, 0x04,                   //     USAGE_MAXIMUM (Button 4)
        /// 0x15, 0x00,                   //     LOGICAL_MINIMUM (0)
        /// 0x25, 0x01,                   //     LOGICAL_MAXIMUM (1)
        /// 0x75, 0x01,                   //     REPORT_SIZE (1)
        /// 0x95, 0x04,                   //     REPORT_COUNT (4)
        /// 0x81, 0x02,                   //     INPUT (Data,Var,Abs)
        /// 0x05, 0x0c,                   //     USAGE_PAGE (Consumer Devices)
        /// 0x09, 0xe0,                   //     USAGE (Volume)
        /// 0x15, 0x00,                   //     LOGICAL_MINIMUM (0)
        /// 0x25, 0x0a,                   //     LOGICAL_MAXIMUM (10)
        /// 0x35, 0x00,                   //     PHYSICAL_MINIMUM (0)
        /// 0x45, 0x64,                   //     PHYSICAL_MAXIMUM (100)
        /// 0x75, 0x04,                   //     REPORT_SIZE (4)
        /// 0x95, 0x01,                   //     REPORT_COUNT (1)
        /// 0x81, 0x02,                   //     INPUT (Data,Var,Abs)
        /// 0xc0,                         //  END_COLLECTION
        /// 0xa1, 0x00,                   //  COLLECTION (Physical)
        /// 0x05, 0x09,                   //     USAGE_PAGE (Button)
        /// 0x19, 0x01,                   //     USAGE_MINIMUM (Button 1)
        /// 0x29, 0x04,                   //     USAGE_MAXIMUM (Button 4)
        /// 0x15, 0x00,                   //     LOGICAL_MINIMUM (0)
        /// 0x25, 0x01,                   //     LOGICAL_MAXIMUM (1)
        /// 0x75, 0x01,                   //     REPORT_SIZE (1)
        /// 0x95, 0x04,                   //     REPORT_COUNT (4)
        /// 0x91, 0x02,                   //     OUTPUT (Data,Var,Abs)
        /// 0x05, 0x0c,                   //     USAGE_PAGE (Consumer Devices)
        /// 0x09, 0xe0,                   //     USAGE (Volume)
        /// 0x15, 0x00,                   //     LOGICAL_MINIMUM (0)
        /// 0x25, 0x0a,                   //     LOGICAL_MAXIMUM (10)
        /// 0x35, 0x00,                   //     PHYSICAL_MINIMUM (0)
        /// 0x45, 0x64,                   //     PHYSICAL_MAXIMUM (100)
        /// 0x75, 0x04,                   //     REPORT_SIZE (4)
        /// 0x95, 0x01,                   //     REPORT_COUNT (1)
        /// 0x91, 0x02,                   //     OUTPUT (Data,Var,Abs)
        /// 0xc0,                         //  END_COLLECTION
        /// 0xa1, 0x02,                   //  COLLECTION (Logical)
        /// 0x85, 0x03,                   //     REPORT_ID (3)
        /// 0x06, 0xAA, 0xff,             //     USAGE_PAGE (Vendor Defined Page 1)
        /// 0x09, 0x01,                   //     USAGE (Vendor Usage  1)
        /// 0x15, 0x00,                   //     LOGICAL_MINIMUM (0)
        /// 0x25, 0x01,                   //     LOGICAL_MAXIMUM (1)
        /// 0x75, 0x01,                   //     REPORT_SIZE (1)
        /// 0x95, 0x01,                   //     REPORT_COUNT (1)
        /// 0x91, 0x02,                   //     OUTPUT (Data,Var,Abs)
        /// 0x95, 0x07,                   //     REPORT_COUNT (7)
        /// 0x91, 0x07,                   //     OUTPUT (Cnst,Var,Rel)
        /// 0xc0,                         //    END_COLLECTION
        /// 0xa1, 0x02,                   //  COLLECTION (Logical)
        /// 0x85, 0x04,                   //     REPORT_ID (4)
        /// 0x06, 0xAA, 0xff,             //     USAGE_PAGE (Vendor Defined Page 1)
        /// 0x09, 0x01,                   //     USAGE (Vendor Usage  1)
        /// 0x15, 0x00,                   //     LOGICAL_MINIMUM (0)
        /// 0x26, 0xff, 0x00,             //     LOGICAL_MAXIMUM (255)
        /// 0x75, 0x08,                   //     REPORT_SIZE (8)
        /// 0x95, 0x01,                   //     REPORT_COUNT (1)
        /// 0xb1, 0x02,                   //     FEATURE (Data,Var,Abs)
        /// 0xc0,                         //    END_COLLECTION
        /// 0x06, 0xDD, 0xff,             //   USAGE_PAGE (Vendor Defined Page 1)
        /// 0x09, 0x01,                   //   USAGE (Vendor Usage  1)
        /// 0xa1, 0x02,                   //  COLLECTION (Logical)
        /// 0x85, 0xff,                   //   REPORT_ID (255)
        /// 0x09, 0x01,                   //   USAGE (Vendor Usage  1)
        /// 0x15, 0x00,                   //   LOGICAL_MINIMUM (0)
        /// 0x26, 0xff, 0x00,             //   LOGICAL_MAXIMUM (255)
        /// 0x75, 0x08,                   //   REPORT_SIZE (8)
        /// 0x95, 0x01,                   //   REPORT_COUNT (1)
        /// 0xb1, 0x02,                   //   FEATURE (Data,Var,Abs)
        /// 0xc0,                          //  END_COLLECTION
        /// 0xc0                          // END_COLLECTION
        /// </summary>
        superMutt: {
            device: {
                vid: 0x045E,
                pid: 0x0610,
                usagePage: 0xFFAA,
                usageId: 0x01,
            },
            /// <summary>
            /// Only available for Feature Reports
            /// </summary>
            ledPattern: {
                reportId: 0x04,
                usagePage: 0xFF00,
                usageId: 0x01,
            },
            /// <summary>
            /// Only available for Input/Output Reports
            /// </summary>
            readWriteBufferControlInformation: {
                reportId: 0x01,
                volumeUsagePage: 0x0C,
                volumeUsageId: 0xE0,
                volumeDataStartBit: 4,  // Numeric data starts on bit 4
                buttonUsagePage: 0x09,
                /// <summary>
                /// Usage id corresponding to each button
                /// 0x19, 0x01,                   //     USAGE_MINIMUM (Button 1)
                /// 0x29, 0x04,                   //     USAGE_MAXIMUM (Button 4)
                /// </summary>
                buttonUsageId: [1, 2, 3, 4],
                buttonDataMask: [
                    0x01, // Button 1
                    0x02, // Button 2
                    0x04, // Button 3
                    0x08, // Button 4
                ]
            },
            /// <summary>
            /// Only available for output reports. The report Id is used to start and stop
            /// device input reports.
            /// Data:   0x01 will enable input reports
            ///          0x00 will disable input reports
            /// </summary>
            deviceInputReportControlInformation: {
                 reportId: 0x03,
                 usagePage: 0xFF00,
                 usageId: 0x01,

                /// <summary>
                /// Values used to turn on/off the device interrupts
                /// </summary>
                dataTurnOn: 0x01,
                dataTurnOff: 0x00,
            }
        }
    });
})();
