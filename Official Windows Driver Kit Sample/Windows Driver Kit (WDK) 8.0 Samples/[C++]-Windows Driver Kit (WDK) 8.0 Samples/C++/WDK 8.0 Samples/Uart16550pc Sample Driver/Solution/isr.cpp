/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    isr.cpp

Abstract:

    This module contains the 16550 UART controller's interrupt service routine.
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

#include "device.h"
#include "isr.h"
#include "regfile.h"
#include "flow.h"

#include "tracing.h"
#include "isr.tmh"

//
// Uart16550pc Interrupt Service Routine: UartISR
//
BOOLEAN
UartISR(
    _In_ WDFINTERRUPT Interrupt,
    _In_ ULONG MessageID
    )
{
    WDFDEVICE device;
    BOOLEAN servicedAnInterrupt = FALSE;
    
    UNREFERENCED_PARAMETER(MessageID);

    FuncEntry(TRACE_FLAG_INTERRUPT);

    device = WdfInterruptGetDevice(Interrupt);

    servicedAnInterrupt = UartISRWorker(device);

    FuncExit(TRACE_FLAG_INTERRUPT);

    return servicedAnInterrupt;
}

BOOLEAN
UartISRWorker(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    BOOLEAN servicedAnInterrupt = FALSE;
    BOOLEAN queuedDpc = FALSE;

    FuncEntry(TRACE_FLAG_INTERRUPT);

    pDevExt = UartGetDeviceExtension(Device);

    servicedAnInterrupt = UartRecordInterrupt(Device);

    if (servicedAnInterrupt)
    {
        queuedDpc = WdfInterruptQueueDpcForIsr(pDevExt->WdfInterrupt);

        if (queuedDpc)
        {
            TraceMessage(TRACE_LEVEL_VERBOSE,
                            TRACE_FLAG_INTERRUPT,
                            "DpcForIsr added to queue (in ISR)");
        }
        else
        {
            TraceMessage(TRACE_LEVEL_VERBOSE,
                            TRACE_FLAG_INTERRUPT,
                            "DpcForIsr is already queued (in ISR)");
        }
    }

    FuncExit(TRACE_FLAG_INTERRUPT);

    return servicedAnInterrupt;
}

//
// Uart16550pc DPC for ISR: UartTxRxDPCForISR
//
VOID
UartTxRxDPCForISR(
    _In_ WDFINTERRUPT Interrupt,
    _In_ WDFOBJECT AssociatedObject
    )
{
    WDFDEVICE device;
    PUART_DEVICE_EXTENSION pDevExt = NULL;

    UNREFERENCED_PARAMETER(AssociatedObject);

    FuncEntry(TRACE_FLAG_WORKERDPC);

    device = WdfInterruptGetDevice(Interrupt);

    pDevExt = UartGetDeviceExtension(device);

    // Acquires the DPC spinlock
    WdfSpinLockAcquire(pDevExt->DpcSpinLock);
    UartTxRxDPCWorker(device);
    WdfSpinLockRelease(pDevExt->DpcSpinLock);

    FuncExit(TRACE_FLAG_WORKERDPC);
}

//
// Uart16550pc DPC for ISR: UartTxRxDPCWorker
//
VOID
UartTxRxDPCWorker(
    _In_ WDFDEVICE Device
)
{
    SERCX_ACTIVITY activity;
    BOOLEAN canLoop = FALSE;
    BOOLEAN canReceive = FALSE;
    BOOLEAN canReceiveIntoBuffer = TRUE;
    BOOLEAN canTransmitIntoBuffer = TRUE;
    BOOLEAN canTransmit = FALSE;
    BOOLEAN hasErrors = FALSE;
    USHORT waitEvents = 0;
    UCHAR errors = 0;
    BOOLEAN queuedDpc = FALSE;
    PUART_DEVICE_EXTENSION pDevExt = NULL;
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_WORKERDPC);

    SERCX_ACTIVITY_INIT(& activity);

    pDevExt = UartGetDeviceExtension(Device);

    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    UartEvaluateLineStatus(Device, &canReceive, &canTransmit, &hasErrors);
    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    SerCxGetActivity(Device, & activity);
    // We can loop if we can receive, we have errors, or we can transmit.
    canLoop = canReceive || hasErrors || 
                    (canTransmit && activity.Transmitting);

    TraceMessage(TRACE_LEVEL_VERBOSE,
                    TRACE_FLAG_WORKERDPC,
                    "TxRxDPCWorker Before Loop (RX=%x, HE=%x, TX=%x)",
                    canReceive,
                    hasErrors,
                    canTransmit);

    // Loops while canLoop == TRUE, indicating there exists work we can do.
    // Stops looping if a function that returns NT_STATUS does not succeed.
    while (canLoop && NT_SUCCESS(status))
    {
        TraceMessage(TRACE_LEVEL_VERBOSE,
                        TRACE_FLAG_WORKERDPC,
                        "TxRxDPCWorker Looping (RX=%x, HE=%x, TX=%x)",
                        canReceive,
                        hasErrors,
                        canTransmit);

        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        waitEvents = UartWaitableEvents(Device);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        //
        // Wait events
        //
        if ((waitEvents & SerCxGetWaitMask(Device)) != 0)
        {
            SerCxCompleteWait(Device, waitEvents);
        }

        //
        // Errors
        //
        if (hasErrors)
        {
            WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
            errors = pDevExt->LineStatus & SERIAL_LSR_ERROR;
            pDevExt->LineStatus = pDevExt->LineStatus & (~SERIAL_LSR_ERROR);
            WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

            // Attempt to handle the errors.
            TraceMessage(TRACE_LEVEL_WARNING,
                            TRACE_FLAG_WORKERDPC,
                            "TxRxDPCWorker Handling ERRORS (LSR=%x)",
                            errors);

            if (errors & SERIAL_LSR_OE)
            {
                pDevExt->PerfStats.SerialOverrunErrorCount++;
                pDevExt->ErrorWord |= SERIAL_ERROR_OVERRUN;
            }

            if (errors & SERIAL_LSR_BI)
            {
                pDevExt->ErrorWord |= SERIAL_ERROR_BREAK;
            }
            else
            {
                //
                // Parity and framing and errors only count 
                // if they occur exclusive of a break being received.
                //

                if (errors & SERIAL_LSR_PE)
                {
                    pDevExt->PerfStats.ParityErrorCount++;
                    pDevExt->ErrorWord |= SERIAL_ERROR_PARITY;
                }

                if (errors & SERIAL_LSR_FE)
                {
                    pDevExt->PerfStats.FrameErrorCount++;
                    pDevExt->ErrorWord |= SERIAL_ERROR_FRAMING;
                }
            }
        }

        //
        // Receive
        //
        if (canReceive)
        {
            BOOLEAN usingFlowControl;

            WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

            // Synchronize with interrupt lock
            WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
            usingFlowControl = UsingRXFlowControl(pDevExt);
            WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

            if ((!HasCachedReceiveBuffer(pDevExt) && usingFlowControl) ||
                pDevExt->DmaReceiveEnabled)
            {
                // Data is available to receive, but we are using flow control
                // and don't have a pended read request, or DMA RX is currently being used.
                // Set flag to exit loop anyway because we don't have anywhere to read
                // the data to (ring buffer won't be used), or data is currently being read by DMA.
                // Data may be lost if DMA is not being used.
                canReceiveIntoBuffer = FALSE;
            }

            // Read bytes from FIFO into appropriate buffer.  If a buffer is not already
            // cached and flow control is not being used, this function will retrieve a 
            // piece of the ring buffer from the class extension and return it after filling.
            // A separate helper function is used here because the same RX FIFO copy logic
            // is also used after completing a DMA receive transfer and before the controller exits D0
            status = UartReceiveBytesFromRXFIFO(Device, pDevExt, !usingFlowControl, TRUE);

            if (!NT_SUCCESS(status))
            {
                TraceMessage(
                    TRACE_LEVEL_WARNING,
                    TRACE_FLAG_RECEIVE,
                    "%!FUNC! Error reading from fifo - %!STATUS!",
                    status);
            }

            WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);
        }

        //
        // Transmit
        //
        if (canTransmit)
        {
            WdfSpinLockAcquire(pDevExt->TransmitBufferSpinLock);

            // If we have a valid txBuffer (perhaps by grabbing one a moment ago). Again, this will be DMA MDL
            // if DMA couldn't transfer all the bytes due to a minimum unit length greater than 1, or DMA timed
            // out.
            if (HasCachedTransmitBuffer(pDevExt) && !pDevExt->DmaTransmitEnabled)
            {
                ULONG bufferLength = GetTransmitBufferLength(pDevExt);

                // Transmit a byte
                WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

                WRITE_TRANSMIT_HOLDING(pDevExt,
                    pDevExt->Controller, 
                    GetNextTransmitBufferByte(pDevExt));
                pDevExt->HoldingEmpty = FALSE;
                pDevExt->TransmitProgress++;

                WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

                if (pDevExt->TransmitProgress >= bufferLength)
                {
                    if (pDevExt->PIOTransmitBuffer.Buffer != NULL)
                    {
                        TraceMessage(TRACE_LEVEL_VERBOSE,
                                        TRACE_FLAG_TRANSMIT,
                                        "Transmitted %lu bytes from buffer %p",
                                        pDevExt->TransmitProgress,
                                        pDevExt->PIOTransmitBuffer.Buffer);
                    }
                    else
                    {
                        TraceMessage(TRACE_LEVEL_VERBOSE,
                                        TRACE_FLAG_TRANSMIT,
                                        "Transmitted %lu bytes from mdl %p",
                                        pDevExt->TransmitProgress,
                                        pDevExt->TransmitMdl);
                    }

                    // No space left in this txBuffer
                    status = SerCxProgressTransmit(
                        Device, 
                        pDevExt->TransmitProgress, 
                        SerCxStatusSuccess);

                    if (!NT_SUCCESS(status))
                    {
                        TraceMessage(
                            TRACE_LEVEL_ERROR,
                            TRACE_FLAG_TRANSMIT,
                            "%!FUNC! Failed to return buffer - %!STATUS!",
                            status);
                        NT_ASSERTMSG("SerCxProgressTransmit shouldn't fail", NT_SUCCESS(status));
                    }

                    pDevExt->PerfStats.TransmittedCount += pDevExt->TransmitProgress;
                    pDevExt->TransmitProgress = 0;

                    // Since both buffers will not be used simultaneously, it's safe to set both to NULL
                    pDevExt->PIOTransmitBuffer.Buffer = NULL;
                    pDevExt->TransmitMdl = NULL;
                }
            }
            else
            {
                // DMA TX is currently running so DPC won't be transmitting bytes
                // using PIO.  Set a flag to help exit so the loop won't get stuck
                // trying to transmit bytes.
                canTransmitIntoBuffer = FALSE;
            }

            WdfSpinLockRelease(pDevExt->TransmitBufferSpinLock);
        }

        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        UartEvaluateLineStatus(Device, &canReceive, &canTransmit, &hasErrors);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        // Check for more work.
        SerCxGetActivity(Device, & activity);
        // We can loop if we can receive, we have errors, or we can transmit.
        canLoop = (canReceive && canReceiveIntoBuffer) || hasErrors || 
                    (canTransmit && canTransmitIntoBuffer && activity.Transmitting);
    }

    // If there was still work to do, queue another DPC.
    if (canLoop)
    {
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        queuedDpc = WdfInterruptQueueDpcForIsr(pDevExt->WdfInterrupt);

        if (queuedDpc)
        {
            TraceMessage(TRACE_LEVEL_VERBOSE,
                            TRACE_FLAG_WORKERDPC,
                            "DpcForIsr added to queue (in DPC)");
        }
        else
        {
            TraceMessage(TRACE_LEVEL_VERBOSE,
                            TRACE_FLAG_WORKERDPC,
                            "DpcForIsr is already queued (in DPC)");
        }

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }
    else
    {
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        
        BYTE ier = READ_INTERRUPT_ENABLE(pDevExt, pDevExt->Controller);
        
        if (activity.Transmitting && !pDevExt->DmaTransmitEnabled)
        {
            // Reenable the THR interrupt
            ier |= SERIAL_IER_THR;
        }

        // Ensure that receive flow control has been re-asserted
        // if there is no more data in the receive buffer.
        // Automatic flow control MUST be re-enabled here if it's being used.
        if (!canReceive && !pDevExt->ReceiveFlowControlAsserted && pDevExt->FIFOBufferBytes == 0)
        {
            UartFlowReceiveAvailable(Device);
        }

        // Re-enable the RDA interrupt now that available data has been read
        // out of the FIFO, and if not using DMA.
        //
        // However, receive interrupts won't be reenabled at the end of this DPC if the RDA
        // interrupt was fired once and the FIFO was not emptied because flow control
        // is being used and there is no pended buffer.  This optimization will prevent
        // the receive interrupt from firing repeatedly while the FIFO remains unempty until
        // a new request is received.  The RDA interrupt will be enabled again when a new
        // request or the interrupt callback calls UartEnableInterrupts.
        if (pDevExt->DmaReceiveEnabled || 
            (!canReceiveIntoBuffer && !pDevExt->ReceiveFlowControlAsserted))
        {
            ier &= ~SERIAL_IER_RDA;
        }
        else
        {
            ier |= SERIAL_IER_RDA;
        }

        WRITE_INTERRUPT_ENABLE(pDevExt, pDevExt->Controller, ier);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    FuncExit(TRACE_FLAG_WORKERDPC);
}

//
// Uart16550pc Function: UartDoTimeoutWork
//
VOID
UartDoTimeoutWork(
    _In_ PUART_DEVICE_EXTENSION pDevExt,
    _In_ UART_TIMEOUT_WORK work
    )
{
    FuncEntry(TRACE_FLAG_WORKERDPC);

    if (work == TIMEOUT_START)
    {
        NT_ASSERT(pDevExt->TimeoutLengthMs > 0);
        WdfTimerStart(
            pDevExt->TimeoutTimer,
            WDF_REL_TIMEOUT_IN_MS(pDevExt->TimeoutLengthMs));
    }
    else if (work == TIMEOUT_STOP)
    {
        // Received data before interval timeout expired, stop timer
        WdfTimerStop(
            pDevExt->TimeoutTimer,
            FALSE);
    }

    FuncExit(TRACE_FLAG_WORKERDPC);
}
