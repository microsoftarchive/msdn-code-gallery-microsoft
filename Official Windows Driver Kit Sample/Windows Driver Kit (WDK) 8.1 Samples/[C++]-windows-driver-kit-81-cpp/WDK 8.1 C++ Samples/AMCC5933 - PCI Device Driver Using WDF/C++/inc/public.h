/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    TPublic.h

Abstract:
 

Environment:

    Kernel & User mode
 
--*/

#include <initguid.h>

//
// Define an Interface Guid for AMCC PCI device.
// This GUID is used to register (IoRegisterDeviceInterface) 
// an instance of an interface so that user application 
// can control the toaster device.
//

DEFINE_GUID (GUID_DEVINTERFACE_AMCC_PCI, 
    0x23c238df, 0xea2a, 0x4f59, 0x80, 0x14, 0xdc, 0xbc, 0x15, 0x6, 0xdb, 0xac);
// {23C238DF-EA2A-4f59-8014-DCBC1506DBAC}

//
// Define an Interface Guid for AMCC ISA device.
// This GUID is used to register (IoRegisterDeviceInterface) 
// an instance of an interface so that user application 
// can control the toaster device.
//

DEFINE_GUID (GUID_DEVINTERFACE_AMCC_ISA, 
    0x90874e48, 0xa217, 0x4738, 0xb5, 0xd2, 0x54, 0xbf, 0x48, 0x68, 0x8a, 0xd5);
// {90874E48-A217-4738-B5D2-54BF48688AD5}


//
// GUID definition are required to be outside of header inclusion pragma to avoid
// error during precompiled headers.
//

#ifndef PUBLIC_H
#define PUBLIC_H

#define IOCTL_GET_VERSION        CTL_CODE(FILE_DEVICE_UNKNOWN, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_RESET                CTL_CODE(FILE_DEVICE_UNKNOWN, 0x801, METHOD_NEITHER, FILE_ANY_ACCESS)
#define IOCTL_READWRITE_MAILBOX    CTL_CODE(FILE_DEVICE_UNKNOWN, 0x802, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_MAILBOX_WAIT        CTL_CODE(FILE_DEVICE_UNKNOWN, 0x803, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_READ_DMA            CTL_CODE(FILE_DEVICE_UNKNOWN, 0x804, METHOD_OUT_DIRECT, FILE_READ_ACCESS)
#define IOCTL_WRITE_DMA            CTL_CODE(FILE_DEVICE_UNKNOWN, 0x805, METHOD_IN_DIRECT, FILE_WRITE_ACCESS)

typedef struct _READWRITE_MAILBOX_PARAMS {
    unsigned char buffer[4];    // buffer to r/w into/from
    unsigned char mailbox;        // mailbox index (1-4)
    unsigned char read;            // TRUE if read, FALSE if write
    unsigned char nbytes;        // # bytes to r/w (0-3)
} READWRITE_MAILBOX_PARAMS, *PREADWRITE_MAILBOX_PARAMS;

typedef struct _MAILBOX_WAIT_PARAMS {
    unsigned long mask;            // mask of MBEF bits to test
    unsigned long mbef;            // value to compare with masked MBEF
} MAILBOX_WAIT_PARAMS, *PMAILBOX_WAIT_PARAMS;

#endif
