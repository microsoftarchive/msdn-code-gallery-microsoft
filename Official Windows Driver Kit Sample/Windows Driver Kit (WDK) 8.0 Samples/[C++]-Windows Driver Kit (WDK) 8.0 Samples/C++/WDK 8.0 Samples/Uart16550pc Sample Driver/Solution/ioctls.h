/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    ioctls.h

Abstract:

    This module contains the 16550 UART controller's IOCTL implementations.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

    These macros borrow heavily from code in the WDF Serial example.

--*/

#pragma once

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>

// Class Extension includes
#include "SerCx.h"


// Internal Function: UartCtlSetBaudRate
//
VOID
UartCtlSetBaudRate(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetBaudRate
//
VOID
UartCtlGetBaudRate(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetModemControl
//
VOID
UartCtlGetModemControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetModemControl
//
VOID
UartCtlSetModemControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetFifoControl
//
VOID
UartCtlSetFifoControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetLineControl
//
VOID
UartCtlSetLineControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetLineControl
//
VOID
UartCtlGetLineControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetChars
//
VOID
UartCtlSetChars(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetChars
//
VOID
UartCtlGetChars(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetDtr
//
VOID
UartCtlSetDtr(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlClrDtr
//
VOID
UartCtlClrDtr(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetRts
//
VOID
UartCtlSetRts(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlClrRts
//
VOID
UartCtlClrRts(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetXoff
//
VOID
UartCtlSetXoff(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetXon
//
VOID
UartCtlSetXon(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetBreakOn
//
VOID
UartCtlSetBreakOn(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetBreakOff
//
VOID
UartCtlSetBreakOff(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetHandflow
//
VOID
UartCtlGetHandflow(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlSetHandflow
//
VOID
UartCtlSetHandflow(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetModemstatus
//
VOID
UartCtlGetModemstatus(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetDtrrts
//
VOID
UartCtlGetDtrrts(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetCommstatus
//
VOID
UartCtlGetCommstatus(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetProperties
//
VOID
UartCtlGetProperties(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlXoffCounter
//
VOID
UartCtlXoffCounter(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlLsrmstInsert
//
VOID
UartCtlLsrmstInsert(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlGetStats
//
VOID
UartCtlGetStats(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlClearStats
//
VOID
UartCtlClearStats(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );

//
// Internal Function: UartCtlImmediateChar
//
VOID
UartCtlImmediateChar(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    );
