//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Usb;

namespace BackgroundTask
{
    class FirmwareSector
    {
        /// <summary>
        /// Sector number of where the BinaryArray belongs to
        /// </summary>
        public UInt32 Sector;

        /// <summary>
        /// Part of the firmware that belongs in this sector
        /// </summary>
        public Byte[] BinaryArray;

        public FirmwareSector(UInt32 sectorNumber, Byte[] binaryForSector)
        {
            Sector = sectorNumber;
            BinaryArray = binaryForSector;
        }

    }

    class ControlTransferSetupPacketsFactory
    {
        public enum SetupPacketPurpose
        {
            SetupDeviceForFirmwareUpdate,
            WriteSector,
            ResetDevice
        }

        public static UsbSetupPacket CreateSetupPacket(SetupPacketPurpose purpose)
        {
            var vendorControlOutRequestType = new UsbControlRequestType
                {
                    ControlTransferType = UsbControlTransferType.Vendor,
                    Direction = UsbTransferDirection.Out,
                    Recipient = UsbControlRecipient.Device
                };

            switch (purpose)
            {
                case SetupPacketPurpose.SetupDeviceForFirmwareUpdate:
                    return new UsbSetupPacket
                        {
                            RequestType = vendorControlOutRequestType,
                            Request = 0xC5,
                            Value = 0,
                            Index = 0,
                            Length = 0
                        };
                case SetupPacketPurpose.WriteSector:
                    /// <summary>
                    /// Value, Index, Length must be filled in later with information specifically about the data being transfered
                    /// </summary>
                    return new UsbSetupPacket
                        {
                            RequestType = vendorControlOutRequestType,
                            Request = 0xC2,
                        };
                case SetupPacketPurpose.ResetDevice:
                    return new UsbSetupPacket
                        {
                            RequestType = vendorControlOutRequestType,
                            Request = 0xDA,
                            Value = 0,
                            Index = 0,
                            Length = 0
                        };
            }

            return null;
        }
    }

    class Firmware
    {
        public static List<FirmwareSector> Sectors = new List<FirmwareSector>
        {
            new FirmwareSector(0x00000000, SectorImages.SectorZero),
            new FirmwareSector(0x00090000, SectorImages.SectorNine),
            new FirmwareSector(0x000A0000, SectorImages.SectorA),
            new FirmwareSector(0x000B0000, SectorImages.SectorB),
            new FirmwareSector(0x000C0000, SectorImages.SectorC),
            new FirmwareSector(0x000D0000, SectorImages.SectorD),
            new FirmwareSector(0x000E0000, SectorImages.SectorE),
            new FirmwareSector(0x000F0000, SectorImages.SectorF),
            new FirmwareSector(0x00100000, SectorImages.SectorOneZero),
            new FirmwareSector(0x00080000, SectorImages.SectorEight)
        };

        // Chipset limitations prevents us from writing more than 4k blocks
        public const UInt32 MaxBytesToWritePerControlTransfer = 0x1000;
    }

    class FirmwareUpdateTaskInformation
    {
        public const String TaskCanceled = "Canceled";
        public const String TaskCompleted = "Completed";
    }

    class LocalSettingKeys
    {
        public class FirmwareUpdateBackgroundTask
        {
            public const String TaskStatus = "FirmwareUpdateBackgroundTaskStatus";
            public const String NewFirmwareVersion = "FirmwareUpdateBackgroundTaskNewFirmwareVersion";
        }
    }
}
