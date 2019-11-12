/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Private.h  - Private header file for the AMCC5933 driver.

Abstract:

Environment:

    Kernel mode

--*/

#include "reg5933.h"
#include "public.h"
#include "common.h"


// ----------------------------------------------------------------------------
// Software related defines and equates
// ----------------------------------------------------------------------------

//
// Set the device specific information.
//
#define MAXIMUM_REQUEST_CONTEXT_LENGTH     (1*1024)
#define MAXIMUM_PHYSICAL_PAGES      16

//
// To specify an alignment of 32-bits: 4 Bytes - 1 = 3
// See the section in the MSDN on
// "Initializing a Device Object" for details.
//
#define AMCC5933_ALIGNMENT__32BITS    FILE_LONG_ALIGNMENT

//
// The device extension.
//
typedef struct _AMCC_DEVICE_EXTENSION {

    WDFDEVICE           Device;                 // A WDFDEVICE handle
    WDFDMAENABLER       DmaEnabler;             // A WDFDMAENABLER handle
    WDFREQUEST          CurrentRequest;         // A WDFREQUEST handle
    WDFINTERRUPT        WdfInterrupt;

    ULONG               MaximumTransferLength;  // Maximum transfer length for adapter
    ULONG               MaximumPhysicalPages;   // Maximum number of breaks adapter

    ULONG               Intcsr;                 // Accumulated interrupt flags

    PREG5933            Regs;                   // S5933 registers ptr

    PUCHAR              PortBase;               // I/O port base address
    ULONG               PortCount;              // Number of assigned ports
    BOOLEAN             PortMapped;             // TRUE if mapped port addr

} AMCC_DEVICE_EXTENSION, * PAMCC_DEVICE_EXTENSION;

//
// This will generate the function named AmccPciGetDevExt to be use for
// retreiving the AMCC_DEVICE_EXTENSION pointer.
//
WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(AMCC_DEVICE_EXTENSION, AmccPciGetDevExt)

typedef struct _INTERRUPT_DATA {
    PVOID Context; // Keep here the context information accessed by the ISR
                    // or with ISR lock held. Not used in this sample.
} INTERRUPT_DATA, *PINTERRUPT_DATA;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(INTERRUPT_DATA, AmccPciGetInterruptData)

// ----------------------------------------------------------------------------
// Function prototypes
// ----------------------------------------------------------------------------

EVT_WDF_DEVICE_PREPARE_HARDWARE AmccPciEvtDevicePrepareHardware;
EVT_WDF_DEVICE_RELEASE_HARDWARE AmccPciEvtDeviceReleaseHardware;

EVT_WDF_IO_QUEUE_IO_DEFAULT AmccPciEvtIoDefault;

EVT_WDF_DEVICE_CONTEXT_CLEANUP AmccPciContextCleanup;

EVT_WDF_INTERRUPT_DPC AmccPciEvtInterruptDpc;
EVT_WDF_INTERRUPT_ISR AmccPciEvtInterruptIsr;

EVT_WDF_PROGRAM_DMA AmccPciProgramDma;

EVT_WDF_DEVICE_D0_ENTRY AmccPciEvtDeviceD0Entry;
EVT_WDF_DEVICE_D0_EXIT AmccPciEvtDeviceD0Exit;

