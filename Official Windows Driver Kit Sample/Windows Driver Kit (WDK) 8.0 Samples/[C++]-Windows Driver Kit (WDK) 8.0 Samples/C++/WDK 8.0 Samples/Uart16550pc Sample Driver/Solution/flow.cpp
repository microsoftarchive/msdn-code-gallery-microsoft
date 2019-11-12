/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    flow.cpp

Abstract:

    This module contains the 16550 UART controller's flow control implementation.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>

// Class Extension includes
#include "SerCx.h"

#include "regfile.h"
#include "device.h"
#include "flow.h"

#include "tracing.h"
#include "flow.tmh"

//
// Uart16550pc Function: UartFlowCanTransmit
//
BOOLEAN
UartFlowCanTransmit(
    _In_ WDFDEVICE Device
    )
{
    BOOLEAN canTransmit = TRUE;
    PUART_DEVICE_EXTENSION pDevExt;
    ULONG outputFlowControl;

    FuncEntry(TRACE_FLAG_FLOWCONTROL);

    pDevExt = UartGetDeviceExtension(Device);

    //
    // The HandFlow data structure is set by an IOCTL and tells us if we should
    // be using flow control.  Checks to see if output flow control is enabled.
    //

    outputFlowControl = pDevExt->HandFlow.ControlHandShake & 
        SERIAL_OUT_HANDSHAKEMASK;

    // Only return true if the CTS bit in the MSR is set.  The MSR is actually
    // updated by the ISR if it changes, so no need to read it directly.
    if (((outputFlowControl & SERIAL_CTS_HANDSHAKE) > 0) &&
        ((pDevExt->ModemStatus & SERIAL_MSR_CTS) == 0))
    {
        canTransmit = FALSE;
    }
    
    if (((outputFlowControl & SERIAL_DSR_HANDSHAKE) > 0) &&
        ((pDevExt->ModemStatus & SERIAL_MSR_DSR) == 0))
    {
        canTransmit = FALSE;
    }

    FuncExit(TRACE_FLAG_FLOWCONTROL);

    return canTransmit;
}

//
// Uart16550pc Function: UartFlowReceiveAvailable
//
VOID
UartFlowReceiveAvailable(
    _In_ WDFDEVICE Device
    )
{
    //
    // *Note: This function should be called while 
    //        holding the interrupt lock
    //

    PUART_DEVICE_EXTENSION pDevExt;
    UCHAR regModemControl;

    FuncEntry(TRACE_FLAG_FLOWCONTROL);

    pDevExt = UartGetDeviceExtension(Device);

    //
    // The HandFlow data structure is set by an IOCTL and tells us if we should
    // be using flow control.  For SERIAL_RTS_HANDSHAKE and SERIAL_DTR_HANDSHAKE
    // we assert the corresponding line when there is space in the FIFO.
    //

    if ((pDevExt->HandFlow.ControlHandShake & SERIAL_DTR_MASK) == 
        SERIAL_DTR_HANDSHAKE)
    {
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl |= SERIAL_MCR_DTR;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
    }

    if ((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) == 
        SERIAL_RTS_HANDSHAKE)
    {
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl |= SERIAL_MCR_RTS;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
    }

    pDevExt->ReceiveFlowControlAsserted = TRUE;

    FuncExit(TRACE_FLAG_FLOWCONTROL);
}

//
// Uart16550pc Function: UartFlowReceiveFull
//
VOID
UartFlowReceiveFull(
    _In_ WDFDEVICE Device
    )
{
    //
    // *Note: This function should be called while 
    //        holding the interrupt lock
    //

    PUART_DEVICE_EXTENSION pDevExt;
    UCHAR regModemControl;

    FuncEntry(TRACE_FLAG_FLOWCONTROL);

    pDevExt = UartGetDeviceExtension(Device);

    //
    // The HandFlow data structure is set by an IOCTL and tells us if we should
    // be using flow control.  For SERIAL_RTS_HANDSHAKE and SERIAL_DTR_HANDSHAKE
    // we de-assert the corresponding line when data in the FIFO reaches the
    // high water mark.
    //

    if ((pDevExt->HandFlow.ControlHandShake & SERIAL_DTR_MASK) == 
        SERIAL_DTR_HANDSHAKE)
    {
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl &= ~SERIAL_MCR_DTR;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
    }

    if ((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) == 
        SERIAL_RTS_HANDSHAKE)
    {
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl &= ~SERIAL_MCR_RTS;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
    }

    pDevExt->ReceiveFlowControlAsserted = FALSE;

    FuncExit(TRACE_FLAG_FLOWCONTROL);
}

//
// Uart16550pc Function: UsingRXFlowControl
//
BOOLEAN UsingRXFlowControl(
    _In_ PUART_DEVICE_EXTENSION pDevExt
    )
{
    //
    // Serial hardare flow control for RX can be either RTS or DTR handshaking (controller driver
    // controls RTS or DTR), or RTS or DTR control (client controls RTS or DTR).  This function returns whether
    // either method is used. The driver's buffer caching design supports changing between either
    // hardware flow control method after the buffer is cached, but care must be taken if
    // RTS or DTR control is used during a transfer, so the RX FIFO is not overrun.  If RTS or DTR control
    // is used, the driver will drive the corresponding client-controlled line to prevent bytes from being
    // received after the device exits D0.
    //

    BOOLEAN usingFlowControl = FALSE;

    if ((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) == SERIAL_RTS_HANDSHAKE)
    {
        // Driver is using RTS handshaking
        usingFlowControl = TRUE;
    }    
    else if ((pDevExt->HandFlow.ControlHandShake & SERIAL_DTR_MASK) == SERIAL_DTR_HANDSHAKE)
    {
        // Driver is using DTR handshaking
        usingFlowControl = TRUE;
    }    
    else if ((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) == SERIAL_RTS_CONTROL)
    {
        // Driver is using RTS control
        usingFlowControl = TRUE;
    }    
    else if ((pDevExt->HandFlow.ControlHandShake & SERIAL_DTR_MASK) == SERIAL_DTR_CONTROL)
    {
        // Driver is using DTR control
        usingFlowControl = TRUE;
    }

    return usingFlowControl;
}

//
// Uart16550pc Function: UsingTXFlowControl
//
BOOLEAN UsingTXFlowControl(
    _In_ PUART_DEVICE_EXTENSION pDevExt
    )
{
    //
    // Serial hardare flow control for TX can be either CTS or DSR handshaking.  
    // This function returns whether either method is used.
    //

    BOOLEAN usingFlowControl = FALSE;

    if ((pDevExt->HandFlow.ControlHandShake & SERIAL_CTS_HANDSHAKE) > 0)
    {
        // Driver is using CTS handshaking
        usingFlowControl = TRUE;
    }    
    else if ((pDevExt->HandFlow.ControlHandShake & SERIAL_DSR_HANDSHAKE) > 0)
    {
        // Driver is using DSR handshaking
        usingFlowControl = TRUE;
    }    

    return usingFlowControl;
}