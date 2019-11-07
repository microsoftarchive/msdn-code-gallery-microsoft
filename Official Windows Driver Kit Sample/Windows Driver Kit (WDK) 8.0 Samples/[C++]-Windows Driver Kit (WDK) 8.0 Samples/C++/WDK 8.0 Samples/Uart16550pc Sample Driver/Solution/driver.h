/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    driver.h

Abstract:

    This module contains the 16550 UART controller driver entry functions.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

#pragma once

#define INITGUID

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>

// Class Extension includes
#include "SerCx.h"

#include "uart16550pc.h"
#include "device.h"


//
// Uart16550pc Event Handler: DriverEntry
//
WDF_EXTERN_C_START
DRIVER_INITIALIZE DriverEntry;
WDF_EXTERN_C_END

//
// Uart16550pc Event Handler: UartEvtDriverUnload
//
EVT_WDF_DRIVER_UNLOAD UartEvtDriverUnload;

//
// Uart16550pc Event Handler: UartEvtDeviceAdd
//
EVT_WDF_DRIVER_DEVICE_ADD UartEvtDeviceAdd;
