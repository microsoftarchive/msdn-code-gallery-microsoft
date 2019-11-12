/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    flow.h

Abstract:

    This module contains the 16550 UART controller's flow control implementation.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

#pragma once

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>

// TODO: Consistent return types for these functions.

//
// Uart16550pc Function: UartFlowCanTransmit
//
BOOLEAN
UartFlowCanTransmit(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UartFlowReceiveAvailable
//
VOID
UartFlowReceiveAvailable(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UartFlowReceiveFull
//
VOID
UartFlowReceiveFull(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UsingRXFlowControl
//
BOOLEAN UsingRXFlowControl(
    _In_ PUART_DEVICE_EXTENSION pDevExt
    );

//
// Uart16550pc Function: UsingTXFlowControl
//
BOOLEAN UsingTXFlowControl(
    _In_ PUART_DEVICE_EXTENSION pDevExt
    );