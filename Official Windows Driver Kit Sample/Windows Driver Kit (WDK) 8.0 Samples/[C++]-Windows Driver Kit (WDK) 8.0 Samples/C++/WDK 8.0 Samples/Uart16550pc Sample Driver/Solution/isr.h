/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    isr.h

Abstract:

    This module contains the 16550 UART controller's interrupt service routine.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

#pragma once

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>

// Class Extension includes
#include "SerCx.h"

//
// Uart16550pc Interrupt Service Routine: UartISR
//
EVT_WDF_INTERRUPT_ISR UartISR;

//
// Uart16550pc DPC for ISR: UartTxRxDPCForISR
//
EVT_WDF_INTERRUPT_DPC UartTxRxDPCForISR;

//
// Uart16550pc Internal Function: UartISRWorker
//
BOOLEAN
UartISRWorker(
    _In_ WDFDEVICE Device
    );

//
// Uart16650pc Internal Function: UartDpcWorker
//
VOID
UartTxRxDPCWorker(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Internal Function: UartDoTimeoutWork
//
VOID
UartDoTimeoutWork(
    _In_ PUART_DEVICE_EXTENSION pDevExt,
    _In_ UART_TIMEOUT_WORK work
    );
