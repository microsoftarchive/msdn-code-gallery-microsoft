//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var FirmwareSectorClass = WinJS.Class.define(function (sectornumber, binaryForSector) {
        this.sector = sectornumber;
        this.binaryArray = binaryForSector;
    }, {
        /// <summary>
        /// Sector number of where the BinaryArray belongs to
        /// </summary>
        sector: null,
        /// <summary>
        /// Part of the firmware that belongs in this sector
        /// </summary>
        binaryArray: null
    }, null);

    var ControlTransferSetupPacketsFactory = WinJS.Class.define(null, null, {
        setupPacketPurpose: {
            setupDeviceForFirmwareUpdate: 0,
            writeSector: 1,
            resetDevice: 2
        },
        createSetupPacket: function (purposeOfSetupPacket) {
            var vendorControlOutRequestType = new Windows.Devices.Usb.UsbControlRequestType();
            vendorControlOutRequestType.controlTransferType = Windows.Devices.Usb.UsbControlTransferType.vendor;
            vendorControlOutRequestType.direction = Windows.Devices.Usb.UsbTransferDirection.out;
            vendorControlOutRequestType.recipient = Windows.Devices.Usb.UsbControlRecipient.device;

            var usbSetupPacket = new Windows.Devices.Usb.UsbSetupPacket();
            usbSetupPacket.requestType = vendorControlOutRequestType;

            switch (purposeOfSetupPacket) {
                case ControlTransferSetupPacketsFactory.setupPacketPurpose.setupDeviceForFirmwareUpdate:
                    usbSetupPacket.request = 0xC5;
                    usbSetupPacket.value = 0;
                    usbSetupPacket.index = 0;
                    usbSetupPacket.length = 0;

                    return usbSetupPacket;

                case ControlTransferSetupPacketsFactory.setupPacketPurpose.writeSector:
                    /// <summary>
                    /// Value, Index, Length must be filled in later with information specifically about the data being transfered
                    /// </summary>
                    usbSetupPacket.request = 0xC2;

                    return usbSetupPacket;

                case ControlTransferSetupPacketsFactory.setupPacketPurpose.resetDevice:
                    usbSetupPacket.request = 0xDA;
                    usbSetupPacket.value = 0;
                    usbSetupPacket.index = 0;
                    usbSetupPacket.length = 0;

                    return usbSetupPacket;
            }

            return null;
        }
    });

    WinJS.Namespace.define("SdkSample.Constants", {
        controlTransferSetupPacketsFactory: ControlTransferSetupPacketsFactory,
        localSettingKeys: {
            firmwareUpdateBackgroundTask: {
                taskStatus: "FirmwareUpdateBackgroundTaskStatus",
                newFirmwareVersion: "FirmwareUpdateBackgroundTaskNewFirmwareVersion"
            }
        },
        firmwareUpdateTaskInformation: {
            name: "FirmwareUpdateBackgroundTask",
            taskEntryPoint: "js\\updateFirmwareTask.js",
            taskCanceled: "Canceled",
            taskCompleted: "Completed",
            // Wait for 2 minutes
            // Convert minutes into units of milliseconds
            //(2 minutes * 60 seconds/minute * 1000 milliseconds/second
            approximateFirmwareUpdateTime: 2 * 60 * 1000
        },
        firmware: {
            sectors: [
                new FirmwareSectorClass(0x00000000, SdkSample.SectorImages.sectorZero),
                new FirmwareSectorClass(0x00090000, SdkSample.SectorImages.sectorNine),
                new FirmwareSectorClass(0x000A0000, SdkSample.SectorImages.sectorA),
                new FirmwareSectorClass(0x000B0000, SdkSample.SectorImages.sectorB),
                new FirmwareSectorClass(0x000C0000, SdkSample.SectorImages.sectorC),
                new FirmwareSectorClass(0x000D0000, SdkSample.SectorImages.sectorD),
                new FirmwareSectorClass(0x000E0000, SdkSample.SectorImages.sectorE),
                new FirmwareSectorClass(0x000F0000, SdkSample.SectorImages.sectorF),
                new FirmwareSectorClass(0x00100000, SdkSample.SectorImages.sectorOneZero),
                new FirmwareSectorClass(0x00080000, SdkSample.SectorImages.sectorEight)
            ],
            // Chipset limitations prevents us from writing more than 4k blocks
            maxBytesToWritePerControlTransfer: 0x1000
        },
        superMutt: {
            device: {
                vid: 0x045E,
                pid: 0x0611
            }
        },
    });
})();
