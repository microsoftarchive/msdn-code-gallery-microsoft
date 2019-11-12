//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "Constants.h"

using namespace BackgroundTask;

Platform::Array<FirmwareSector^>^ BackgroundTask::Firmware::Sectors = ref new Platform::Array<FirmwareSector^>
    {
        ref new FirmwareSector(0x00000000, ref new Platform::Array<uint8>(SectorImages::SectorZero, sizeof(SectorImages::SectorZero))),
        ref new FirmwareSector(0x00090000, ref new Platform::Array<uint8>(SectorImages::SectorNine, sizeof(SectorImages::SectorNine))),
        ref new FirmwareSector(0x000A0000, ref new Platform::Array<uint8>(SectorImages::SectorA, sizeof(SectorImages::SectorA))),
        ref new FirmwareSector(0x000B0000, ref new Platform::Array<uint8>(SectorImages::SectorB, sizeof(SectorImages::SectorB))),
        ref new FirmwareSector(0x000C0000, ref new Platform::Array<uint8>(SectorImages::SectorC, sizeof(SectorImages::SectorC))),
        ref new FirmwareSector(0x000D0000, ref new Platform::Array<uint8>(SectorImages::SectorD, sizeof(SectorImages::SectorD))),
        ref new FirmwareSector(0x000E0000, ref new Platform::Array<uint8>(SectorImages::SectorE, sizeof(SectorImages::SectorE))),
        ref new FirmwareSector(0x000F0000, ref new Platform::Array<uint8>(SectorImages::SectorF, sizeof(SectorImages::SectorF))),
        ref new FirmwareSector(0x00100000, ref new Platform::Array<uint8>(SectorImages::SectorOneZero, sizeof(SectorImages::SectorOneZero))),
        ref new FirmwareSector(0x00080000, ref new Platform::Array<uint8>(SectorImages::SectorEight, sizeof(SectorImages::SectorEight)))
    };