//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

#include "pch.h"

namespace BackgroundTask
{
    ref class FirmwareSector
    {
    internal:
        /// <summary>
        /// Sector number of where the BinaryArray belongs to
        /// </summary>
        uint32 Sector;

        /// <summary>
        /// Part of the firmware that belongs in this sector
        /// </summary>
        Platform::Array<uint8>^ BinaryArray;

        FirmwareSector(uint32 sectorNumber, Platform::Array<uint8>^ binaryForSector) :
            Sector(sectorNumber),
            BinaryArray(binaryForSector)
        {
        };

    };

    ref class ControlTransferSetupPacketsFactory
    {
    internal:
        enum SetupPacketPurpose
        {
            SetupDeviceForFirmwareUpdate,
            WriteSector,
            ResetDevice
        };

        static Windows::Devices::Usb::UsbSetupPacket^ CreateSetupPacket(SetupPacketPurpose purpose)
        {
            auto usbSetupPacket = ref new Windows::Devices::Usb::UsbSetupPacket();

            auto vendorControlOutRequestType = ref new Windows::Devices::Usb::UsbControlRequestType();
            vendorControlOutRequestType->ControlTransferType = Windows::Devices::Usb::UsbControlTransferType::Vendor;
            vendorControlOutRequestType->Direction = Windows::Devices::Usb::UsbTransferDirection::Out;
            vendorControlOutRequestType->Recipient = Windows::Devices::Usb::UsbControlRecipient::Device;

            usbSetupPacket->RequestType = vendorControlOutRequestType;

            switch (purpose)
            {
            case SetupPacketPurpose::SetupDeviceForFirmwareUpdate:
                usbSetupPacket->Request = 0xC5;
                usbSetupPacket->Value = 0;
                usbSetupPacket->Index = 0;
                usbSetupPacket->Length = 0;

                return usbSetupPacket;

            case SetupPacketPurpose::WriteSector:
                /// <summary>
                /// Value, Index, Length must be filled in later with information specifically about the data being transfered
                /// </summary>
                usbSetupPacket->Request = 0xC2;

                return usbSetupPacket;

            case SetupPacketPurpose::ResetDevice:
                usbSetupPacket->Request = 0xDA;
                usbSetupPacket->Value = 0;
                usbSetupPacket->Index = 0;
                usbSetupPacket->Length = 0;

                return usbSetupPacket;
            }

            return nullptr;
        };
    };

    class Firmware
    {
    public:
        static Platform::Array<FirmwareSector^>^ Sectors;

        // Chipset limitations prevents us from writing more than 4k blocks
        static const uint32 MaxBytesToWritePerControlTransfer = 0x1000;
    };

    namespace FirmwareUpdateUsbDevice
    {
        namespace FirmwareUpdateTaskInformation
        {
            static Platform::String^ TaskCanceled = "Canceled";
            static Platform::String^ TaskCompleted = "Completed";
        }

        namespace LocalSettingKeys
        {
            namespace FirmwareUpdateBackgroundTask
            {
                static Platform::String^ TaskStatus = "FirmwareUpdateBackgroundTaskStatus";
                static Platform::String^ NewFirmwareVersion = "FirmwareUpdateBackgroundTaskNewFirmwareVersion";
            }
        }
    }
}