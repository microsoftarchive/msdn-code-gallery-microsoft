/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Private.h  - Header file common to both the devices.

Abstract:

Environment:

    Kernel mode

--*/

#include <ntddk.h>
#pragma warning(disable:4201)  // disable nameless struct/union warnings
#include <wdf.h>
#pragma warning(default:4201)

#define NTSTRSAFE_LIB
#include <ntstrsafe.h>
#include "trace.h"

// 4127 -- Conditional Expression is Constant warning
#define WHILE(constant) \
__pragma(warning(disable: 4127)) while(constant); __pragma(warning(default: 4127))

//
// DMA Tranfer information structure.
// Contains data possibly needed to do dma transfer (see StartPacket())
//
typedef struct _REQUEST_CONTEXT{

    WDFREQUEST          Request;
    WDFDMATRANSACTION   DmaTransaction;

} REQUEST_CONTEXT, *PREQUEST_CONTEXT;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(REQUEST_CONTEXT, GetRequestContext)


DRIVER_INITIALIZE DriverEntry;

EVT_WDF_DRIVER_DEVICE_ADD CommonEvtDeviceAdd;
EVT_WDF_DRIVER_DEVICE_ADD AmccIsaEvtDeviceAdd;
EVT_WDF_DRIVER_DEVICE_ADD AmccPciAddDevice;

EVT_WDF_OBJECT_CONTEXT_CLEANUP CommonEvtDriverContextCleanup;

