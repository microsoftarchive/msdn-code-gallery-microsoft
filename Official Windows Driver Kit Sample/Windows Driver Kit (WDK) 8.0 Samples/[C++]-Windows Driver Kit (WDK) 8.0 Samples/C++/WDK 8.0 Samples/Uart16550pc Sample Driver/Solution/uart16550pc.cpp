/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    uart16550pc.cpp

Abstract:

    This module contains the 16550 UART controller's DDI functions.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>
#include "ntddser.h"
#include "intsafe.h"

// Class Extension includes
#include "SerCx.h"

// Project includes
#include "uart16550pc.h"
#include "device.h"
#include "regfile.h"
#include "ioctls.h"
#include "flow.h"
#include "isr.h"

#include "tracing.h"
#include "uart16550pc.tmh"


//
// Uart16550pc Function: UartInitContext
//
NTSTATUS
UartInitContext(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt->WdfDevice;

    // The special characters do not matter except that Xon != Xoff.
    pDevExt->SpecialChars.XonChar = SERIAL_DEF_XON;
    pDevExt->SpecialChars.XoffChar = SERIAL_DEF_XOFF;
    pDevExt->HandFlow.ControlHandShake = 0;
    pDevExt->HandFlow.FlowReplace = 0;

    FuncExit(TRACE_FLAG_INIT);
    return status;
}

//
// Uart16550pc Event Handler: UartEvtFileOpen
//
NTSTATUS
UartEvtFileOpen(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_FILE);

    WdfSpinLockAcquire(pDevExt->DpcSpinLock);
    
    // Clear the current perf stats
    RtlZeroMemory(&pDevExt->PerfStats, sizeof(SERIALPERF_STATS));
    pDevExt->ErrorWord = 0;

    pDevExt->DeviceOpened = TRUE;

    // Reset FIFOs if already in D0, otherwise set flag so
    // FIFOs will reset the next time D0 is entered.
    if (pDevExt->DeviceActive)
    {
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        WRITE_FIFO_CONTROL(pDevExt, pDevExt->Controller, pDevExt->FifoControl |
            SERIAL_FCR_RCVR_RESET | SERIAL_FCR_TXMT_RESET);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }
    else
    {
        pDevExt->ResetFifoOnD0Entry = TRUE;
    }

    WdfSpinLockRelease(pDevExt->DpcSpinLock);

    if (NT_SUCCESS(status))
    {
        TraceMessage(TRACE_LEVEL_INFORMATION,
                        TRACE_FLAG_FILE,
                        "File opened");
    }

    FuncExit(TRACE_FLAG_FILE);
    return status;
}

//
// Uart16550pc Event Handler: UartEvtFileClose
//
VOID
UartEvtFileClose(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);

    FuncEntry(TRACE_FLAG_FILE);

    // Acquires the DPC spinlock
    WdfSpinLockAcquire(pDevExt->DpcSpinLock);

    pDevExt->DeviceOpened = FALSE;

    // Flush the software FIFO
    pDevExt->FIFOBufferNextByte = pDevExt->FIFOBuffer;
    pDevExt->FIFOBufferBytes = 0;

    WdfSpinLockRelease(pDevExt->DpcSpinLock);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_FILE,
                    "File closed");

    FuncExit(TRACE_FLAG_FILE);
}

//
// Uart16550pc Event Handler: UartEvtFileCleanup
//
VOID
UartEvtFileCleanup(
    _In_ WDFDEVICE Device
    )
{
    UNREFERENCED_PARAMETER(Device);

    FuncEntry(TRACE_FLAG_FILE);

    // TODO
    // Implement this function

    FuncExit(TRACE_FLAG_FILE);
}


//
// Uart16550pc Event Handler: UartEvtTransmit
//
NTSTATUS
UartEvtTransmit(
    _In_ WDFDEVICE Device,
    _In_ size_t Length
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    NTSTATUS status = STATUS_SUCCESS;
    BOOLEAN  usingPIO = TRUE;
    
    FuncEntry(TRACE_FLAG_TRANSMIT);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_TRANSMIT,
                    "Transmitting %lu bytes",
                    (ULONG)Length);

    WdfSpinLockAcquire(pDevExt->TransmitBufferSpinLock);

    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    // Determine whether transmit should be PIO or DMA
    if (UartUseDma(pDevExt, WdfDmaDirectionWriteToDevice, Length))
    {
        pDevExt->DmaTransmitEnabled = TRUE;
        usingPIO = FALSE;
    }
    else
    {
        pDevExt->DmaTransmitEnabled = FALSE;
    }

    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    //
    // Start a DMA transfer.
    // Do not need interrupt lock around this DmaTransmitEnabled check as this
    // value is only set in this function or on DMA completion.
    //

    if (pDevExt->DmaTransmitEnabled)
    {
        PMDL pMdl = nullptr;

        // Set the IER before acquiring the MDL to disable the THR interrupt.
        // The interrupt may already be set, so it shouldn't fire in the time
        // after the MDL is retrieved and before DMA begins.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        // Refires the state machine
        DISABLE_ALL_INTERRUPTS(pDevExt, pDevExt->Controller);
        UartEnableInterrupts(pDevExt);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        status = SerCxRetrieveTransmitMdl(Device, &pMdl);
        if (NT_SUCCESS(status))
        {
            pDevExt->TransmitMdl = pMdl;
            NTSTATUS progressStatus = STATUS_SUCCESS;

            PVOID virtualAddress = MmGetMdlVirtualAddress(pMdl);

            // Calculate maximum number of bytes that can be transferred using DMA
            NT_ASSERT(0 != pDevExt->DmaWriteInfo.V1.MinimumTransferUnit);

            pDevExt->DmaTransmitLength = (ULONG)Length / pDevExt->DmaWriteInfo.V1.MinimumTransferUnit;
            pDevExt->DmaTransmitLength *= pDevExt->DmaWriteInfo.V1.MinimumTransferUnit;

            if (pDevExt->DmaTransmitLength == 0)
            {
                status = STATUS_UNSUCCESSFUL;

                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_TRANSMIT,
                    "Transfer is 0 bytes, minimum transfer unit %u",
                    pDevExt->DmaWriteInfo.V1.MinimumTransferUnit);
            }

            if (NT_SUCCESS(status))
            {
                // Init the transaction
                status = WdfDmaTransactionInitialize(
                    pDevExt->DmaTransmitTransaction,
                    UartEvtTransmitProgramDma,
                    WdfDmaDirectionWriteToDevice,
                    pMdl,
                    virtualAddress,
                    pDevExt->DmaTransmitLength);
            }

            if (NT_SUCCESS(status))
            {
                WdfDmaTransactionSetTransferCompleteCallback(
                    pDevExt->DmaTransmitTransaction,
                    UartEvtTransmitDmaTransferComplete,
                    WDF_NO_CONTEXT);
                status = WdfDmaTransactionExecute(
                    pDevExt->DmaTransmitTransaction,
                    WDF_NO_CONTEXT);
            }

            // Failed after retreiving MDL, so must return it to class extension, will nullify MDL pointer later
            if (!NT_SUCCESS(status))
            {
                progressStatus = SerCxProgressTransmit(Device, 0, SerCxStatusSuccess);
            }

            if (!NT_SUCCESS(progressStatus))
            {
                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_INTERRUPT,
                    "%!FUNC! Failed to return buffer - %!STATUS!",
                    status);
                NT_ASSERTMSG("SerCxProgressTransmit shouldn't fail", NT_SUCCESS(progressStatus));
            }
        }

        if (!NT_SUCCESS(status))
        {
            usingPIO = TRUE;

            TraceMessage(
                TRACE_LEVEL_WARNING,
                TRACE_FLAG_TRANSMIT,
                "DMA transfer initialization failed");
        }
    }

    //
    // The transmit is using PIO.  It may have failed DMA initialization and is now falling back on PIO
    //

    if (usingPIO)
    {
        // Set the IER and reset DMA state variables
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        pDevExt->DmaTransmitEnabled = FALSE;
        pDevExt->TransmitMdl = nullptr;

        // Refires the state machine
        DISABLE_ALL_INTERRUPTS(pDevExt, pDevExt->Controller);
        UartEnableInterrupts(pDevExt);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        // Retrieve and cache buffer for this transmit request.  This buffer will be held
        // until filled or cancelled.
        if (Length <= ULONG_MAX)
        {
            status = SerCxRetrieveTransmitBuffer(Device, (ULONG)Length, &pDevExt->PIOTransmitBuffer);
        }
        else
        {
            status = STATUS_INVALID_PARAMETER;
        }

        if (NT_SUCCESS(status))
        {
            // Initialize byte counter
            pDevExt->TransmitProgress = 0;
        }
        else
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_INTERRUPT,
                "SerCxRetrieveTransmitBuffer failed - %!STATUS!",
                status);
        }
    }

    WdfSpinLockRelease(pDevExt->TransmitBufferSpinLock);
    
    FuncExit(TRACE_FLAG_TRANSMIT);
    return status;
}

//
// Uart16550pc DMA Event Handler: UartEvtTransmitProgramDma
//
BOOLEAN
UartEvtTransmitProgramDma(
    _In_  WDFDMATRANSACTION Transaction,
    _In_  WDFDEVICE Device,
    _In_  WDFCONTEXT Context,
    _In_  WDF_DMA_DIRECTION Direction,
    _In_  PSCATTER_GATHER_LIST SgList
    )
{
    //
    // Called by WDF DMA framework before transfer begins for purpose of
    // configuring device.  Return TRUE to signal succesful programing
    // of device.
    //

    UNREFERENCED_PARAMETER(Transaction);
    UNREFERENCED_PARAMETER(Context);
    UNREFERENCED_PARAMETER(Direction);
    UNREFERENCED_PARAMETER(SgList);

    FuncEntry(TRACE_FLAG_TRANSMIT);

    // Set internal THR state to not empty as DMA will fill THR
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    pDevExt->HoldingEmpty = FALSE;

    FuncExit(TRACE_FLAG_TRANSMIT);

    return TRUE;
}

//
// Uart16550pc DMA Event Handler: UartEvtTransmitDmaTransferComplete
//
VOID
UartEvtTransmitDmaTransferComplete(
    _In_ WDFDMATRANSACTION     DmaTransaction,
    _In_ WDFDEVICE             Device,
    _In_ PVOID                 Context,
    _In_ WDF_DMA_DIRECTION     Direction,
    _In_ DMA_COMPLETION_STATUS DmaStatus
    )
{
    //
    // Called by WDF DMA framework once a transfer has completed.
    //
    UNREFERENCED_PARAMETER(Context);
    UNREFERENCED_PARAMETER(Direction);

    FuncEntry(TRACE_FLAG_TRANSMIT);

    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    NTSTATUS status;

    // Calculate the number of bytes that should have been transferred by DMA
    ULONG bytesScheduled = pDevExt->DmaTransmitLength;

    // Calculate the number of bytes not transferred by DMA.  This could be greater than 0 if the transfer
    // timed out.
    ULONG bytesNotTransferred = pDevExt->writeAdapter->DmaOperations->ReadDmaCounter(pDevExt->writeAdapter);

    // Total number of bytes actually transferred with DMA
    ULONG bytesTransferred = bytesScheduled - bytesNotTransferred;

    // Calculate the number of bytes remaining by taking the length of the original request ( GetTransmitBufferLength() ).
    // Subtract the number of bytes scheduled, which for the request not being an even power of
    // the minimum transfer unit (bytesScheduled).  Subtract the number of bytes not transferred by DMA due to timeout
    // (bytesNotTransferred).  The rest of these bytes will be transferred using PIO.
    ULONG bytesTransferredLeftOver = GetTransmitBufferLength(pDevExt) - bytesTransferred;

    if (DmaComplete == DmaStatus)
    {
        WdfSpinLockAcquire(pDevExt->TransmitBufferSpinLock);

        NTSTATUS transactionStatus;
        BOOLEAN dmaDone = WdfDmaTransactionDmaCompleted(DmaTransaction, &transactionStatus);

        if (TRUE == dmaDone)
        {
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_TRANSMIT,
                            "DMA Transmitted %lu bytes",
                            bytesTransferred);

            WdfDmaTransactionRelease(DmaTransaction);

            WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
            pDevExt->DmaTransmitEnabled = FALSE;
            WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

            // Free the resource
            pDevExt->writeAdapter->DmaOperations->FreeAdapterChannel(pDevExt->writeAdapter);

            if (bytesTransferredLeftOver > 0)
            {
                TraceMessage(TRACE_LEVEL_INFORMATION,
                                TRACE_FLAG_TRANSMIT,
                                "%lu bytes left over after DMA, completing with PIO",
                                bytesTransferredLeftOver);

                // Use PIO to transmit left over bytes in the transmit MDL, unmask interrupts
                pDevExt->TransmitProgress = bytesTransferred;

                WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
                UartEnableInterrupts(pDevExt);
                WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
            }
            else
            {
                // All bytes were transferred, return MDL
                status = SerCxProgressTransmit(
                    Device, 
                    bytesTransferred, 
                    SerCxStatusSuccess);

                if (!NT_SUCCESS(status))
                {
                    TraceMessage(
                        TRACE_LEVEL_ERROR,
                        TRACE_FLAG_TRANSMIT,
                        "%!FUNC! Failed to return MDL - %!STATUS!",
                        status);
                    NT_ASSERTMSG("SerCxProgressTransmit shouldn't fail", NT_SUCCESS(status));
                }

                pDevExt->TransmitMdl = nullptr;
            }
        }
        else
        {
            // If not done, then the status should be more processing required.
            NT_ASSERT(STATUS_MORE_PROCESSING_REQUIRED == transactionStatus);
        }

        WdfSpinLockRelease(pDevExt->TransmitBufferSpinLock);
    }
    else if (DmaCancelled == DmaStatus)
    {
        WdfSpinLockAcquire(pDevExt->TransmitBufferSpinLock);

        NTSTATUS transactionStatus;
        BOOLEAN result = WdfDmaTransactionDmaCompletedFinal(pDevExt->DmaTransmitTransaction, bytesTransferred, &transactionStatus);
        UNREFERENCED_PARAMETER(result);
        NT_ASSERT(TRUE == result);
        SERCX_STATUS serCxStatus = SerCxStatusCancelled;

        // The transfer may have received all the bytes by the time the transfer
        // cancelled, or the completion routine may have already been invoked with
        // success status
        if (bytesTransferred == GetTransmitBufferLength(pDevExt))
        {
            serCxStatus = SerCxStatusSuccess;
        }

        status = SerCxProgressTransmit(
            Device,
            bytesTransferred,
            serCxStatus);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_TRANSMIT,
                "%!FUNC! Failed to return MDL - %!STATUS!",
                status);
            NT_ASSERTMSG("SerCxProgressTransmit shouldn't fail", NT_SUCCESS(status));
        }

        WdfDmaTransactionRelease(pDevExt->DmaTransmitTransaction);

        // Free the resource
        pDevExt->writeAdapter->DmaOperations->FreeAdapterChannel(pDevExt->writeAdapter);

        pDevExt->TransmitMdl = nullptr;

        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        pDevExt->DmaTransmitEnabled = FALSE;
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        WdfSpinLockRelease(pDevExt->TransmitBufferSpinLock);
    }
    else
    {
        //
        // DmaAborted or DmaError
        //

        // TODO: release transaction, any other action necessary to flush and free DMA line
        NT_ASSERTMSG("DMA transfer failure", FALSE);
    }

    FuncExit(TRACE_FLAG_TRANSMIT);

    return;
}

//
// Uart16550pc Event Handler: UartEvtReceive
//
NTSTATUS
UartEvtReceive(
    _In_ WDFDEVICE Device,
    _In_ size_t Length
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    NTSTATUS status = STATUS_SUCCESS;
    BOOLEAN usingPIO = TRUE;
    BOOLEAN usingFlowControl;
    
    FuncEntry(TRACE_FLAG_RECEIVE);

    WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_RECEIVE,
                    "Receiving %lu bytes",
                    (ULONG)Length);    

    //
    // Determine whether receive should be PIO or DMA
    //

    // If software FIFO not empty or not using DMA, use PIO 
    if (pDevExt->FIFOBufferBytes > 0 || !UartUseDma(pDevExt, WdfDmaDirectionReadFromDevice, Length))
    {
        pDevExt->DmaReceiveEnabled = FALSE;
    }
    else
    {
        pDevExt->DmaReceiveEnabled = TRUE;
        usingPIO = FALSE;
    }

    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
    usingFlowControl = UsingRXFlowControl(pDevExt);
    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    // Save interval timeout length
    pDevExt->TimeoutLengthMs = SerCxGetReadIntervalTimeout(Device);

    //
    // Start a DMA transfer.
    // Do not need interrupt lock around this DmaReceiveEnabled check as this
    // value is only set in this function or on DMA completion.
    //

    if (pDevExt->DmaReceiveEnabled)
    {
        // Set the IER before acquiring the MDL to disable the RDA interrupt.
        // The interrupt may already be set, so it shouldn't fire in the time
        // after the MDL is retrieved and before DMA begins.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        UartEnableInterrupts(pDevExt);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        PMDL pMdl = nullptr;
        status = SerCxRetrieveReceiveMdl(Device, &pMdl);

        if (NT_SUCCESS(status))
        {
            pDevExt->ReceiveMdl = pMdl;
            pDevExt->TimeoutTimerTimedOut = FALSE;
            NTSTATUS progressStatus = STATUS_SUCCESS;

            PVOID virtualAddress = MmGetMdlVirtualAddress(pMdl);

            // Calculate maximum number of bytes that can be transferred using DMA
            NT_ASSERT(0 != pDevExt->DmaReadInfo.V1.MinimumTransferUnit);

            pDevExt->DmaReceiveLength = (ULONG)Length / pDevExt->DmaReadInfo.V1.MinimumTransferUnit;
            pDevExt->DmaReceiveLength *= pDevExt->DmaReadInfo.V1.MinimumTransferUnit;

            if (pDevExt->DmaReceiveLength == 0)
            {
                status = STATUS_UNSUCCESSFUL;

                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_RECEIVE,
                    "Transfer is 0 bytes, minimum transfer unit %u",
                    pDevExt->DmaReadInfo.V1.MinimumTransferUnit);
            }

            // Init the transaction
            if (NT_SUCCESS(status))
            {
                status = WdfDmaTransactionInitialize(
                    pDevExt->DmaReceiveTransaction,
                    UartEvtReceiveProgramDma,
                    WdfDmaDirectionReadFromDevice,
                    pMdl,
                    virtualAddress,
                    pDevExt->DmaReceiveLength);
            }

            if (NT_SUCCESS(status))
            {
                WdfDmaTransactionSetTransferCompleteCallback(
                    pDevExt->DmaReceiveTransaction,
                    UartEvtReceiveDmaTransferComplete,
                    WDF_NO_CONTEXT);
                status = WdfDmaTransactionExecute(
                    pDevExt->DmaReceiveTransaction,
                    WDF_NO_CONTEXT);
            }

            // Failed after retreiving MDL, so must return it to class extension, will nullify MDL pointer later            
            if (!NT_SUCCESS(status))
            {
                progressStatus = SerCxProgressReceive(Device, 0, SerCxStatusSuccess);
            }

            if (!NT_SUCCESS(progressStatus))
            {
                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_RECEIVE,
                    "%!FUNC! Failed to return MDL - %!STATUS!",
                    status);
                NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(progressStatus));
            }
        }

        if (!NT_SUCCESS(status))
        {
            usingPIO = TRUE;

            TraceMessage(
                TRACE_LEVEL_WARNING,
                TRACE_FLAG_RECEIVE,
                "DMA transfer initialization failed");
        }
    }

    //
    // The receive is using PIO.  It may have failed DMA initialization and is now falling back on PIO
    //
    if (usingPIO)
    {
        //
        // Cache the buffer only if using flow control.  A buffer that is cached
        // here will be held until filled or cancelled.  Otherwise, the DPC will acquire
        // and release the receive buffer to ensure the RX Fifo does not overflow.
        //

        if (usingFlowControl)
        {
            // Buffer should have been returned
            NT_ASSERTMSG("SerCx buffer already cached", !HasCachedReceiveBuffer(pDevExt));

            // Store PIO buffer
            if (Length <= ULONG_MAX)
            {
                status = SerCxRetrieveReceiveBuffer(Device, (ULONG)Length, &pDevExt->PIOReceiveBuffer);
            }
            else
            {
                status = STATUS_INVALID_PARAMETER;
            }

            if (!NT_SUCCESS(status))
            {
                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_RECEIVE,
                    "SerCxRetrieveReceiveBuffer failed - %!STATUS!",
                    status);
            }

            pDevExt->ReceiveProgress = 0;

            // If software FIFO not empty, copy bytes into cached buffer.
            // The cached buffer will be returned if filled.  Otherwise,
            // start the interval timeout timer.  If the software FIFO is
            // emptied, flow control lines will be re-asserted
            if (pDevExt->FIFOBufferBytes > 0)
            {
                UartReceiveBytesFromSoftwareFIFO(pDevExt);

                // Return the buffer if full, otherwise start the interval
                // timer.
                if (pDevExt->ReceiveProgress >= GetReceiveBufferLength(pDevExt))
                {
                    status = SerCxProgressReceive(
                        Device, 
                        pDevExt->ReceiveProgress, 
                        SerCxStatusSuccess);

                    if (!NT_SUCCESS(status))
                    {
                        TraceMessage(
                            TRACE_LEVEL_ERROR,
                            TRACE_FLAG_RECEIVE,
                            "%!FUNC! Failed to return buffer - %!STATUS!",
                            status);
                        NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(status));
                    }

                    pDevExt->PerfStats.ReceivedCount += pDevExt->ReceiveProgress;
                    pDevExt->ReceiveProgress = 0;
                    pDevExt->PIOReceiveBuffer.Buffer = NULL;
                }
                else
                {
                    if (HasCachedReceiveBuffer(pDevExt) && pDevExt->TimeoutLengthMs > 0)
                    {
                        UartDoTimeoutWork(pDevExt, TIMEOUT_START);
                    }
                }

                // Automatic flow control MUST be re-enabled here if it's being used.
                if (pDevExt->FIFOBufferBytes == 0)
                {
                    UartFlowReceiveAvailable(Device);
                }
            }
        }

        // Set the IER and reset DMA state variables
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        pDevExt->DmaReceiveEnabled = FALSE;
        pDevExt->ReceiveMdl = nullptr;
        UartEnableInterrupts(pDevExt);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);
    
    FuncExit(TRACE_FLAG_RECEIVE);    
    return status;
}

//
// Uart16550pc DMA Event Handler: UartEvtReceiveProgramDma
//
BOOLEAN
UartEvtReceiveProgramDma(
    _In_  WDFDMATRANSACTION Transaction,
    _In_  WDFDEVICE Device,
    _In_  WDFCONTEXT Context,
    _In_  WDF_DMA_DIRECTION Direction,
    _In_  PSCATTER_GATHER_LIST SgList
    )
{
    //
    // Called by WDF DMA framework before transfer begins for purpose of
    // configuring device.  Return TRUE to signal succesfull programing
    // of device.
    //
    UNREFERENCED_PARAMETER(Transaction);
    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(Context);
    UNREFERENCED_PARAMETER(Direction);
    UNREFERENCED_PARAMETER(SgList);

    return TRUE;
}

//
// Uart16550pc DMA Event Handler: UartEvtReceiveDmaTransferComplete
//
VOID
UartEvtReceiveDmaTransferComplete(
    _In_ WDFDMATRANSACTION     DmaTransaction,
    _In_ WDFDEVICE             Device,
    _In_ PVOID                 Context,
    _In_ WDF_DMA_DIRECTION     Direction,
    _In_ DMA_COMPLETION_STATUS DmaStatus
    )
{
    //
    // Called by WDF DMA framework once a transfer has completed.
    //
    UNREFERENCED_PARAMETER(Context);
    UNREFERENCED_PARAMETER(Direction);

    FuncEntry(TRACE_FLAG_RECEIVE);

    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    NTSTATUS status;

    // Calculate the number of bytes that should have been transferred by DMA
    ULONG bytesScheduled = pDevExt->DmaReceiveLength;

    // Calculate the number of bytes not transferred by DMA.  This could be greater than 0 if the transfer
    // timed out.
    ULONG bytesNotTransferred = pDevExt->readAdapter->DmaOperations->ReadDmaCounter(pDevExt->readAdapter);

    // Total number of bytes actually transferred with DMA
    ULONG bytesTransferred = bytesScheduled - bytesNotTransferred;

    // Calculate the number of bytes remaining by taking the length of the original request ( GetReceiveBufferLength() ).
    // Subtract the number of bytes scheduled, which for the request not being an even power of
    // the minimum transfer unit (bytesScheduled).  Subtract the number of bytes not transferred by DMA due to timeout
    // (bytesNotTransferred).  The rest of these bytes will be transferred using PIO.
    ULONG bytesTransferredLeftOver = GetReceiveBufferLength(pDevExt) - bytesTransferred;

    if (DmaComplete == DmaStatus)
    {
        WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

        NTSTATUS transactionStatus;

        BOOLEAN dmaDone = WdfDmaTransactionDmaCompleted(DmaTransaction, &transactionStatus);
        if (TRUE == dmaDone)
        {
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_RECEIVE,
                            "DMA Received %lu bytes",
                            bytesTransferred);

            WdfDmaTransactionRelease(DmaTransaction);

            WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
            pDevExt->DmaReceiveEnabled = FALSE;
            WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

            // Free the resource
            pDevExt->readAdapter->DmaOperations->FreeAdapterChannel(pDevExt->readAdapter);

            // DMA transfer has been cancelled by the HAL extension.  Start the interval timeout if at least one byte was received.
            // Rather than schedule another DMA transfer, try to fill the rest of the buffer using PIO.  This will prevent excessive
            // overhead caused by repeatedly initializing DMA transfers if data is being received sporadically.
            if (bytesTransferredLeftOver > 0)
            {
                TraceMessage(TRACE_LEVEL_INFORMATION,
                                TRACE_FLAG_RECEIVE,
                                "%lu bytes left over after DMA, completing with PIO",
                                bytesTransferredLeftOver);

                // Use PIO to transmit left over bytes in the receive MDL, unmask interrupts
                pDevExt->ReceiveProgress = bytesTransferred;

                WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
                UartEnableInterrupts(pDevExt);
                WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

                // If at least 1 byte was received, start interval timer, will be stopped as bytes are received in PIO
                if (pDevExt->ReceiveProgress > 0 && pDevExt->TimeoutLengthMs > 0)
                {
                    TraceMessage(
                        TRACE_LEVEL_VERBOSE,
                        TRACE_FLAG_RECEIVE,
                        "Starting interval timeout");

                    UartDoTimeoutWork(pDevExt, TIMEOUT_START);
                }
            }
            else
            {
                // All bytes were transferred, return MDL
                status = SerCxProgressReceive(
                    Device, 
                    bytesTransferred, 
                    SerCxStatusSuccess);

                if (!NT_SUCCESS(status))
                {
                    TraceMessage(
                        TRACE_LEVEL_ERROR,
                        TRACE_FLAG_RECEIVE,
                        "%!FUNC! Failed to return MDL - %!STATUS!",
                        status);
                    NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(status));
                }

                pDevExt->ReceiveMdl = nullptr;
            }
        }
        else
        {
            // If not done, then the status should be more processing required.
            NT_ASSERT(STATUS_MORE_PROCESSING_REQUIRED == transactionStatus);
        }

        WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);
    }
    else if (DmaCancelled == DmaStatus)
    {
        WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

        NTSTATUS transactionStatus;
        BOOLEAN result = WdfDmaTransactionDmaCompletedFinal(pDevExt->DmaReceiveTransaction, bytesTransferred, &transactionStatus);
        UNREFERENCED_PARAMETER(result);
        NT_ASSERT(TRUE == result);

        pDevExt->ReceiveProgress = bytesTransferred;

        SERCX_STATUS serCxStatus = SerCxStatusCancelled;
        if (pDevExt->TimeoutTimerTimedOut)
        {
            serCxStatus = SerCxStatusTimeout;
        }

        // The transfer may have received all the bytes by the time the transfer
        // cancelled, or the completion routine may have already been invoked with
        // success status
        if (bytesTransferred == GetReceiveBufferLength(pDevExt))
        {
            serCxStatus = SerCxStatusSuccess;
        }

        // UartReceiveBytesFromRXFIFO won't do anything if DmaReceiveEnabled
        // is set to true
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        pDevExt->DmaReceiveEnabled = FALSE;

        // Re-enable receive interrupt
        UartEnableInterrupts(pDevExt);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        // Read remaining bytes from FIFO
        status = UartReceiveBytesFromRXFIFO(Device, pDevExt, FALSE, TRUE);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_WARNING,
                TRACE_FLAG_RECEIVE,
                "%!FUNC! Error reading from fifo - %!STATUS!",
                status);
        }

        // Stop the interval timer if reading the FIFO didn't fill the buffer
        if (pDevExt->TimeoutLengthMs > 0)
        {
            TraceMessage(
                TRACE_LEVEL_VERBOSE,
                TRACE_FLAG_RECEIVE,
                "Stopping interval timeout");

            UartDoTimeoutWork(pDevExt, TIMEOUT_STOP);
        }

        // If reading the FIFO didn't fill the buffer, return it here
        if (pDevExt->ReceiveMdl != NULL)
        {
            status = SerCxProgressReceive(
                Device, 
                pDevExt->ReceiveProgress, 
                serCxStatus);

            if (!NT_SUCCESS(status))
            {
                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_RECEIVE,
                    "%!FUNC! Failed to return MDL - %!STATUS!",
                    status);
                NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(status));
            }
        }

        WdfDmaTransactionRelease(pDevExt->DmaReceiveTransaction);

        // Free the resource
        pDevExt->readAdapter->DmaOperations->FreeAdapterChannel(pDevExt->readAdapter);

        pDevExt->ReceiveMdl = nullptr;
        pDevExt->TimeoutTimerTimedOut = FALSE;

        WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);
    }
    else
    {
        //
        // DmaAborted or DmaError
        //

        // TODO: release transaction, any other action necessary to flush and free DMA line
        NT_ASSERTMSG("DMA transfer failure", FALSE);
    }

    FuncExit(TRACE_FLAG_RECEIVE);

    return;
}

//
// Uart16550pc Function: UartReceiveDmaCancelCleanup
//
NTSTATUS
UartReceiveDmaCancelCleanup(
    _In_ WDFDEVICE Device,
    _In_ ULONG BytesTransferred
    )
{
    //
    // Note: This function must be called with ReceiveBufferSpinLock and DpcSpinLock held
    //

    FuncEntry(TRACE_FLAG_RECEIVE);

    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    NTSTATUS status;
    NTSTATUS transactionStatus;
    BOOLEAN result = WdfDmaTransactionDmaCompletedFinal(pDevExt->DmaReceiveTransaction, BytesTransferred, &transactionStatus);
    UNREFERENCED_PARAMETER(result);
    NT_ASSERT(TRUE == result);

    pDevExt->ReceiveProgress = BytesTransferred;

    SERCX_STATUS serCxStatus = SerCxStatusCancelled;
    if (pDevExt->TimeoutTimerTimedOut)
    {
        serCxStatus = SerCxStatusTimeout;
    }

    // The transfer may have received all the bytes by the time the transfer
    // cancelled, or the completion routine may have already been invoked with
    // success status
    if (BytesTransferred == GetReceiveBufferLength(pDevExt))
    {
        serCxStatus = SerCxStatusSuccess;
    }

    // UartReceiveBytesFromRXFIFO won't do anything if DmaReceiveEnabled
    // is set to true
    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
    pDevExt->DmaReceiveEnabled = FALSE;
    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    // Read remaining bytes from FIFO
    status = UartReceiveBytesFromRXFIFO(Device, pDevExt, FALSE, TRUE);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_WARNING,
            TRACE_FLAG_RECEIVE,
            "%!FUNC! Error reading from fifo - %!STATUS!",
            status);
    }

    // Stop the interval timer if reading the FIFO didn't fill the buffer
    if (pDevExt->TimeoutLengthMs > 0)
    {
        TraceMessage(
            TRACE_LEVEL_VERBOSE,
            TRACE_FLAG_RECEIVE,
            "Stopping interval timeout");

        UartDoTimeoutWork(pDevExt, TIMEOUT_STOP);
    }

    // If reading the FIFO didn't fill the buffer, return it here
    if (pDevExt->ReceiveMdl != NULL)
    {
        status = SerCxProgressReceive(
            Device, 
            pDevExt->ReceiveProgress, 
            serCxStatus);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_RECEIVE,
                "%!FUNC! Failed to return MDL - %!STATUS!",
                status);
            NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(status));
        }
    }

    WdfDmaTransactionRelease(pDevExt->DmaReceiveTransaction);

    // Free the resource
    pDevExt->readAdapter->DmaOperations->FreeAdapterChannel(pDevExt->readAdapter);

    pDevExt->ReceiveMdl = nullptr;
    pDevExt->TimeoutTimerTimedOut = FALSE;

    FuncExit(TRACE_FLAG_RECEIVE);

    return status;
}

//
// Uart16550pc Event Handler: UartEvtTransmitCancel
//
VOID
UartEvtTransmitCancel(
    _In_ WDFDEVICE Device
    )
{
    UNREFERENCED_PARAMETER(Device);
    
    FuncEntry(TRACE_FLAG_TRANSMIT);

    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    BOOLEAN dmaCancel = FALSE;

    WdfSpinLockAcquire(pDevExt->TransmitBufferSpinLock);

    // Synchronously check whether we need to cancel a DMA transfer
    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    if (pDevExt->DmaTransmitEnabled)
    {
         dmaCancel = TRUE;

        TraceMessage(
            TRACE_LEVEL_INFORMATION,
            TRACE_FLAG_TRANSMIT,
            "Cancelling DMA Transfer");
    }

    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    // WdfDmaTransactionStopSystemTransfer doesn't block, so it should be all right
    // to hold the lock before calling the function
    if (dmaCancel)
    {
        WdfDmaTransactionStopSystemTransfer(pDevExt->DmaTransmitTransaction);
    }

    //
    // Cancel PIO transfer
    //

    if (HasCachedTransmitBuffer(pDevExt) && !dmaCancel)
    {
        // Report number of bytes copied and return the cached buffer
        NTSTATUS status;

        status = SerCxProgressTransmit(
            Device,
            pDevExt->TransmitProgress,
            SerCxStatusCancelled);

        pDevExt->TransmitProgress = 0;
        pDevExt->PIOTransmitBuffer.Buffer = NULL;
        pDevExt->TransmitMdl = NULL;

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_TRANSMIT,
                "%!FUNC! Failed to return buffer - %!STATUS!",
                status);
            NT_ASSERTMSG("SerCxProgressTransmit shouldn't fail", NT_SUCCESS(status));
        }
    }

    WdfSpinLockRelease(pDevExt->TransmitBufferSpinLock);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_TRANSMIT,
                    "Transmit cancelled");

    FuncExit(TRACE_FLAG_TRANSMIT);
}

//
// Uart16550pc Event Handler: UartEvtReceiveCancel
//
VOID
UartEvtReceiveCancel(
    _In_ WDFDEVICE Device
    )
{
    UNREFERENCED_PARAMETER(Device);

    FuncEntry(TRACE_FLAG_RECEIVE);

    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    BOOLEAN dmaCancel = FALSE;

    WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

    //
    // Cancel DMA transfer
    //

    // Synchronously check whether we need to cancel a DMA transfer
    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    if (pDevExt->DmaReceiveEnabled)
    {
        dmaCancel = TRUE;

        TraceMessage(
            TRACE_LEVEL_INFORMATION,
            TRACE_FLAG_RECEIVE,
            "Cancelling DMA Transfer");
    }

    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    // WdfDmaTransactionStopSystemTransfer doesn't block, so it should be all right
    // to hold the lock before calling the function
    if (dmaCancel)
    {
        WdfDmaTransactionStopSystemTransfer(pDevExt->DmaReceiveTransaction);
    }

    //
    // Cancel PIO receive transfer
    //

    if (HasCachedReceiveBuffer(pDevExt) && !dmaCancel)
    {
        // Report number of bytes copied and return the cached buffer
        NTSTATUS status;
        SERCX_STATUS serCxStatus = SerCxStatusCancelled;

        status = SerCxProgressReceive(
            Device,
            pDevExt->ReceiveProgress,
            serCxStatus);

        pDevExt->ReceiveProgress = 0;
        pDevExt->PIOReceiveBuffer.Buffer = NULL;
        pDevExt->ReceiveMdl = NULL;

        // Stop the interval timeout timer as it might be running.
        // Timer shouldn't have started if given length of 0ms
        if (pDevExt->TimeoutLengthMs > 0)
        {
            TraceMessage(
                TRACE_LEVEL_VERBOSE,
                TRACE_FLAG_RECEIVE,
                "Stopping interval timeout");

            UartDoTimeoutWork(pDevExt, TIMEOUT_STOP);
        }

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_RECEIVE,
                "%!FUNC! Failed to return buffer - %!STATUS!",
                status);
            NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(status));
        }
    }

    WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_RECEIVE,
                    "Receive cancelled");
    
    FuncExit(TRACE_FLAG_RECEIVE);
}

//
// Uart16550pc Event Handler: UartEvtWaitmask
//
NTSTATUS
UartEvtWaitmask(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    ULONG waitmask;

    FuncEntry(TRACE_FLAG_CONTROL);

    waitmask = SerCxGetWaitMask(Device);
    
    WdfSpinLockAcquire(pDevExt->DpcSpinLock);

    pDevExt->IsrWaitMask = waitmask;

    WdfSpinLockRelease(pDevExt->DpcSpinLock);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "Waitmask set");  

    FuncExit(TRACE_FLAG_CONTROL);
    return STATUS_SUCCESS;
}

//
// Uart16550pc Event Handler: UartEvtPurge
//
NTSTATUS
UartEvtPurge(
    _In_ WDFDEVICE Device,
    _In_ ULONG PurgeMask
    )
{
    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(PurgeMask);

    //
    // The serial class extension is handling various flags for
    // IOCTL_SERIAL_PURGE. There is currently nothing to do here.
    //
    
    return STATUS_SUCCESS;
}

//
// Uart16550pc Event Handler: UartEvtControl
//
NTSTATUS
UartEvtControl(
    _In_ WDFDEVICE Device,
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength,
    _In_ ULONG IoControlCode
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    
    FuncEntry(TRACE_FLAG_CONTROL);

    NT_ASSERT(Device != NULL);
    NT_ASSERT(Request != NULL);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "Switching on IOCTL %s",
                    UartIOCTLtoString(IoControlCode));

    // The Great Switch:
    switch (IoControlCode)
    {
    case IOCTL_SERIAL_SET_BAUD_RATE:
        UartCtlSetBaudRate(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_BAUD_RATE:
        UartCtlGetBaudRate(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_MODEM_CONTROL:
        UartCtlGetModemControl(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_MODEM_CONTROL:
        UartCtlSetModemControl(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_FIFO_CONTROL:
        UartCtlSetFifoControl(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_LINE_CONTROL:
        UartCtlSetLineControl(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_LINE_CONTROL:
        UartCtlGetLineControl(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_CHARS:
        UartCtlSetChars(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_CHARS:
        UartCtlGetChars(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_DTR:
        UartCtlSetDtr(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_CLR_DTR:
        UartCtlClrDtr(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_RTS:
        UartCtlSetRts(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_CLR_RTS:
        UartCtlClrRts(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_XOFF:
        UartCtlSetXoff(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_XON:
        UartCtlSetXon(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_BREAK_ON:
        UartCtlSetBreakOn(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_BREAK_OFF:
        UartCtlSetBreakOff(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_HANDFLOW:
        UartCtlGetHandflow(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_SET_HANDFLOW:
        UartCtlSetHandflow(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_MODEMSTATUS:
        UartCtlGetModemstatus(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_DTRRTS:
        UartCtlGetDtrrts(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_COMMSTATUS:
        UartCtlGetCommstatus(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_PROPERTIES: 
        UartCtlGetProperties(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_XOFF_COUNTER:
        UartCtlXoffCounter(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_LSRMST_INSERT:
        UartCtlLsrmstInsert(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_GET_STATS:
        UartCtlGetStats(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_CLEAR_STATS:
        UartCtlClearStats(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    case IOCTL_SERIAL_IMMEDIATE_CHAR:
        UartCtlImmediateChar(Device, 
                        Request, 
                        OutputBufferLength, 
                        InputBufferLength);
        break;
    default:
        {
            status = STATUS_INVALID_DEVICE_REQUEST;
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_CONTROL,
                            "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                            Request, 
                            status);

            WdfRequestComplete(Request, status);
            break;
        }
    }

    FuncExit(TRACE_FLAG_CONTROL);
    return status;
}

//
// Uart16550pc Event Handler: UartEvtApplyConfig
//
NTSTATUS
UartEvtApplyConfig(
    _In_ WDFDEVICE Device,
    _In_ PVOID ConnectionParameters
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PPNP_UART_SERIAL_BUS_DESCRIPTOR uartDescriptor;
    
    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Parse the connection parameters
    //

    status = UartGetDescriptorFromConnectionParameters(
        ConnectionParameters,
        &uartDescriptor);

    if (NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_INFORMATION,
            TRACE_FLAG_FILE,
            "UART Connection Descriptor %p "
            "Baudrate:%lu "
            "RxBufferSize:%u "
            "TxBufferSize:%u "
            "Parity:%x "
            "Flags:%hx",
            uartDescriptor,
            uartDescriptor->BaudRate,
            uartDescriptor->RxBufferSize,
            uartDescriptor->TxBufferSize,
            uartDescriptor->Parity,
            uartDescriptor->SerialBusDescriptor.TypeSpecificFlags);

        //
        // Configure the controller
        //

        status = UartApplyConfig(Device, uartDescriptor);
    }

    FuncExit(TRACE_FLAG_CONTROL);
    return status;
}

//
// Uart16550pc Function: UartGetDescriptorFromConnectionParameters
//
NTSTATUS
UartGetDescriptorFromConnectionParameters(
    _In_ PVOID ConnectionParameters,
    _Out_ PPNP_UART_SERIAL_BUS_DESCRIPTOR * Descriptor
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PRH_QUERY_CONNECTION_PROPERTIES_OUTPUT_BUFFER connection = NULL;
    PPNP_SERIAL_BUS_DESCRIPTOR descriptor = NULL;
    
    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Parse the connection parameters
    //

    if (ConnectionParameters == NULL)
    {
        status = STATUS_INVALID_PARAMETER;

        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_FILE,
            "Connection parameters %p should not be NULL - "
            "%!STATUS!",
            ConnectionParameters,
            status);
    }

    if (NT_SUCCESS(status))
    {
        connection =
            (PRH_QUERY_CONNECTION_PROPERTIES_OUTPUT_BUFFER)ConnectionParameters;

        if (connection->PropertiesLength < sizeof(PPNP_SERIAL_BUS_DESCRIPTOR))
        {
            status = STATUS_INVALID_PARAMETER;

            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_FILE,
                "Invalid connection properties (length = %d, "
                "expected = %d) - %!STATUS!",
                connection->PropertiesLength,
                sizeof(PPNP_SERIAL_BUS_DESCRIPTOR),
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        descriptor = (PPNP_SERIAL_BUS_DESCRIPTOR)
            connection->ConnectionProperties;

        if (descriptor->SerialBusType != UART_SERIAL_BUS_TYPE)
        {
            status = STATUS_INVALID_PARAMETER;

            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_FILE,
                "Bus type %c not supported, only UART - %!STATUS!",
                descriptor->SerialBusType,
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        *Descriptor = (PPNP_UART_SERIAL_BUS_DESCRIPTOR)
            connection->ConnectionProperties;
    }
    
    FuncExit(TRACE_FLAG_CONTROL);
    return status;
}

//
// Uart16550pc Function: UartApplyConfig
//
NTSTATUS
UartApplyConfig(
    _In_ WDFDEVICE Device,
    _In_ PPNP_UART_SERIAL_BUS_DESCRIPTOR Descriptor
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    USHORT divsorLatchReg = 0;
    SERIAL_HANDFLOW flow = {0};
    SERIAL_CHARS chars = {0};
    SERIAL_LINE_CONTROL lineControl = {0};
    UCHAR lineControlReg = 0;
    
    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Configure baudrate
    //

    status = UartRegConvertAndValidateBaud(
        Descriptor->BaudRate, 
        &divsorLatchReg);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to convert and validate baudrate %lu - "
            "%!STATUS!",
            Descriptor->BaudRate,
            status);
        goto exit;
    }

    //
    // Configure flow control
    //

    pDevExt->SpecialChars.XonChar = SERIAL_DEF_XON;
    pDevExt->SpecialChars.XoffChar = SERIAL_DEF_XOFF;

    switch (Descriptor->SerialBusDescriptor.TypeSpecificFlags & 
        UART_SERIAL_FLAG_FLOW_CTL_MASK)
    {
    case UART_SERIAL_FLAG_FLOW_CTL_NONE:

        if (Descriptor->SerialLinesEnabled & 
            UART_SERIAL_LINES_RTS)
        {
            flow.FlowReplace |= SERIAL_RTS_CONTROL;
        }

        if (Descriptor->SerialLinesEnabled & 
            UART_SERIAL_LINES_DTR)
        {
            flow.ControlHandShake |= SERIAL_DTR_CONTROL;
        }
        
        break;

    case UART_SERIAL_FLAG_FLOW_CTL_XONXOFF:
        
        status = STATUS_NOT_IMPLEMENTED;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Software flow control has not been implemented - "
            "%!STATUS!",
            status);
        goto exit;

    case UART_SERIAL_FLAG_FLOW_CTL_HW:

        if (Descriptor->SerialLinesEnabled & 
            UART_SERIAL_LINES_RTS)
        {
            flow.FlowReplace |= SERIAL_RTS_HANDSHAKE;
        }

        if (Descriptor->SerialLinesEnabled & 
            UART_SERIAL_LINES_CTS)
        {
            flow.ControlHandShake |= SERIAL_CTS_HANDSHAKE;
        }

        if (Descriptor->SerialLinesEnabled & 
            UART_SERIAL_LINES_DTR)
        {
            flow.ControlHandShake |= SERIAL_DTR_HANDSHAKE;
        }

        if (Descriptor->SerialLinesEnabled & 
            UART_SERIAL_LINES_DSR)
        {
            flow.ControlHandShake |= SERIAL_DSR_HANDSHAKE;
        }
        
        break;

    default:
        
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Invalid flow control setting in bus descriptor flags %lu - "
            "%!STATUS!",
            Descriptor->SerialBusDescriptor.TypeSpecificFlags,
            status);
        goto exit;

    }

    //
    // Configure FIFOs.  The current connection settings 
    // for FIFO size have limited meaning for this controller. 
    // On the 16550, the FIFO is either on or off.
    //

    //
    // Configure data bits, stop bits, parity
    //

    switch (Descriptor->SerialBusDescriptor.TypeSpecificFlags & 
        UART_SERIAL_FLAG_DATA_BITS_MASK)
    {
    case UART_SERIAL_FLAG_DATA_BITS_5:
        lineControl.WordLength = 5;
        break;
    case UART_SERIAL_FLAG_DATA_BITS_6:
        lineControl.WordLength = 6;
        break;
    case UART_SERIAL_FLAG_DATA_BITS_7:
        lineControl.WordLength = 7;
        break;
    case UART_SERIAL_FLAG_DATA_BITS_8:
        lineControl.WordLength = 8;
        break;
    case UART_SERIAL_FLAG_DATA_BITS_9:
        lineControl.WordLength = 9;
    default:
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Invalid data bits setting in UART descriptor flags %hu - "
            "%!STATUS!",
            Descriptor->SerialBusDescriptor.TypeSpecificFlags,
            status);
        goto exit;
    }

    switch (Descriptor->SerialBusDescriptor.TypeSpecificFlags & 
        UART_SERIAL_FLAG_STOP_BITS_MASK)
    {
    case UART_SERIAL_FLAG_STOP_BITS_1:
        lineControl.StopBits = STOP_BIT_1;
        break;
    case UART_SERIAL_FLAG_STOP_BITS_1_5:
        lineControl.StopBits = STOP_BITS_1_5;
        break;
    case UART_SERIAL_FLAG_STOP_BITS_2:
        lineControl.StopBits = STOP_BITS_2;
        break;
    case UART_SERIAL_FLAG_STOP_BITS_0:
        // 0 stop bits is invalid
    default:
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Invalid stop bits setting in UART descriptor flags %hu - "
            "%!STATUS!",
            Descriptor->SerialBusDescriptor.TypeSpecificFlags,
            status);
        goto exit;
    }

    switch (Descriptor->Parity)
    {
    case UART_SERIAL_PARITY_NONE:
        lineControl.Parity = NO_PARITY;
        break;
    case UART_SERIAL_PARITY_ODD:
        lineControl.Parity = ODD_PARITY;
        break;
    case UART_SERIAL_PARITY_EVEN:
        lineControl.Parity = EVEN_PARITY;
        break;
    case UART_SERIAL_PARITY_MARK:
        lineControl.Parity = MARK_PARITY;
        break;
    case UART_SERIAL_PARITY_SPACE:
        lineControl.Parity = SPACE_PARITY;
        break;
    default:
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Invalid parity setting in UART descriptor flags %hu - "
            "%!STATUS!",
            Descriptor->SerialBusDescriptor.TypeSpecificFlags,
            status);
        goto exit;
    }

    if (NT_SUCCESS(status))
    {
        status = UartRegStructToLCR(&lineControl, &lineControlReg);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_CONTROL,
                "Failed to calculate LCR from SERIAL_LINE_CONTROL %p  - "
                "%!STATUS!",
                &lineControl,
                status);
            goto exit;
        }
    }

    //
    // Apply the settings if we have succeeded this far.
    // All must be modified under synchronization of the
    // DPC spinlock, and possibly the interrupt lock, 
    // which is used to gate access to the hardware registers.
    //

    WdfSpinLockAcquire(pDevExt->DpcSpinLock);

    // Save the FIFO sizes
    pDevExt->TxFifoSize = Descriptor->TxBufferSize;
    pDevExt->RxFifoSize = Descriptor->RxBufferSize;

    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    // Check previous flow control configuration
    BOOLEAN prevFlowControl = UsingRXFlowControl(pDevExt);
    
    // Set the baudrate, special characters, and handflow
    pDevExt->CurrentBaud = Descriptor->BaudRate;
    pDevExt->SpecialChars = chars;
    pDevExt->HandFlow = flow;

    // Check new flow control configuration
    BOOLEAN newFlowControl = UsingRXFlowControl(pDevExt);

    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

    // Empty software FIFO before changing flow control
    if (newFlowControl != prevFlowControl)
    {
        if (!newFlowControl)
        {
            if (pDevExt->FIFOBufferBytes > 0)
            {
                // If switching from flow control to no flow control,
                // read bytes from the software FIFO to ring buffer before
                // asserting flow control lines.

                // Shouldn't have a cached buffer and bytes in the software FIFO
                NT_ASSERT(!HasCachedReceiveBuffer(pDevExt));
                
                // Using a new status variable so the IOCTL doesn't fail if
                // the driver can't read the software FIFO to SerCx ring buffer, which
                // may happen after the file has closed.
                NTSTATUS fifoStatus = SerCxRetrieveReceiveBuffer(Device, SERIAL_SOFTWARE_FIFO_SIZE, &pDevExt->PIOReceiveBuffer);

                // Read bytes from software FIFO and return the buffer
                if (NT_SUCCESS(fifoStatus))
                {
                    // Read the software FIFO bytes into the ring buffer.
                    // This function won't return the buffer.
                    UartReceiveBytesFromSoftwareFIFO(pDevExt);

                    // The software FIFO has been read out and should be empty now.
                    NT_ASSERT(pDevExt->FIFOBufferBytes == 0);

                    fifoStatus = SerCxProgressReceive(
                        Device, 
                        pDevExt->ReceiveProgress, 
                        SerCxStatusSuccess);

                    if (!NT_SUCCESS(fifoStatus))
                    {
                        TraceMessage(
                            TRACE_LEVEL_ERROR,
                            TRACE_FLAG_CONTROL,
                            "%!FUNC! Failed to return buffer - %!STATUS!",
                            fifoStatus);
                        NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(fifoStatus));
                    }

                    pDevExt->PerfStats.ReceivedCount += pDevExt->ReceiveProgress;
                    pDevExt->ReceiveProgress = 0;
                    pDevExt->PIOReceiveBuffer.Buffer = NULL;
                }
                else
                {
                    TraceMessage(
                        TRACE_LEVEL_WARNING,
                        TRACE_FLAG_CONTROL,
                        "SerCxRetrieveReceiveBuffer failed - %!STATUS!",
                        fifoStatus);
                }
            }
        }
        else
        {
            // If switching from no flow control to flow control,
            // the software FIFO should already be empty.
            NT_ASSERT(pDevExt->FIFOBufferBytes == 0);
        }
    }

    WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);

    // Apply the new register settings.
    if (pDevExt->DeviceActive == TRUE)
    {
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        WRITE_DIVISOR_LATCH(pDevExt, pDevExt->Controller, divsorLatchReg);
        
        lineControlReg = lineControlReg | 
            (READ_LINE_CONTROL(pDevExt, pDevExt->Controller) & SERIAL_LCR_BREAK);
        WRITE_LINE_CONTROL(pDevExt, pDevExt->Controller, lineControlReg);

        // If software FIFO empty, re-assert flow control
        // Automatic flow control MUST be re-enabled here if it's being used.
        if (pDevExt->FIFOBufferBytes == 0)
        {
            UartFlowReceiveAvailable(Device);
        }

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }
    else
    {        
        // If the device has been marked as inactive, 
        // use saved register values instead and set 
        // the NewConfigOutstanding flag.
        pDevExt->DivisorLatch = divsorLatchReg;
        pDevExt->LineControl = lineControlReg | 
            (pDevExt->LineControl & SERIAL_LCR_BREAK);
        pDevExt->NewConfigOutstanding = TRUE;
    }

    WdfSpinLockRelease(pDevExt->DpcSpinLock);

exit:
    
    FuncExit(TRACE_FLAG_CONTROL);
    return status;
}

//
// Uart16550pc Event Handler: UartIntervalTimeoutTimer;
//
VOID
UartIntervalTimeoutTimer(
    _In_ WDFTIMER Timer
    )
{
    FuncEntry(TRACE_FLAG_RECEIVE);

    WDFDEVICE Device = (WDFDEVICE) WdfTimerGetParentObject(Timer);
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);

    WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

    TraceMessage(
        TRACE_LEVEL_VERBOSE,
        TRACE_FLAG_RECEIVE,
        "Stopping interval timeout");

    UartDoTimeoutWork(pDevExt, TIMEOUT_STOP);

    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    // Interval timeout only set duing PIO in this driver, DMA should not be enabled.
    NT_ASSERT(pDevExt->DmaReceiveEnabled == FALSE);

    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    //
    // Cancel PIO transfer receive transfer if buffer is still being held
    //

    if (HasCachedReceiveBuffer(pDevExt) && !pDevExt->DmaReceiveEnabled)
    {
        // Report number of bytes copied and return the cached buffer
        NTSTATUS status;

        TraceMessage(
            TRACE_LEVEL_INFORMATION,
            TRACE_FLAG_RECEIVE,
            "Interval timeout of %llums",
            pDevExt->TimeoutLengthMs);

        status = SerCxProgressReceive(
            Device,
            pDevExt->ReceiveProgress,
            SerCxStatusTimeout);

        pDevExt->ReceiveProgress = 0;

        // Set both buffers to NULL in case the buffer was originally used for DMA
        pDevExt->PIOReceiveBuffer.Buffer = NULL;
        pDevExt->ReceiveMdl = NULL;

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_RECEIVE,
                "%!FUNC! Failed to return buffer - %!STATUS!",
                status);
            NT_ASSERTMSG("SerCxProgressTransmit shouldn't fail", NT_SUCCESS(status));
        }
    }

    WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);

    FuncExit(TRACE_FLAG_RECEIVE);
}

//
// Uart16550pc Function: UartIOCTLtoString
//
PCHAR
UartIOCTLtoString(
    _In_ ULONG        IoControlCode
    )
{
    //
    //Returns IOCTL string for debugging
    //
    switch(IoControlCode)
    {
    case IOCTL_SERIAL_SET_BAUD_RATE:
        return "IOCTL_SERIAL_SET_BAUD_RATE";
    case IOCTL_SERIAL_GET_BAUD_RATE:
        return "IOCTL_SERIAL_GET_BAUD_RATE";
    case IOCTL_SERIAL_GET_MODEM_CONTROL:
        return "IOCTL_SERIAL_GET_MODEM_CONTROL";
    case IOCTL_SERIAL_SET_MODEM_CONTROL:
        return "IOCTL_SERIAL_SET_MODEM_CONTROL";
    case IOCTL_SERIAL_SET_FIFO_CONTROL:
        return "IOCTL_SERIAL_SET_FIFO_CONTROL";
    case IOCTL_SERIAL_SET_LINE_CONTROL:
        return "IOCTL_SERIAL_SET_LINE_CONTROL";
    case IOCTL_SERIAL_GET_LINE_CONTROL:
        return "IOCTL_SERIAL_GET_LINE_CONTROL";
    case IOCTL_SERIAL_SET_CHARS:
        return "IOCTL_SERIAL_SET_CHARS";
    case IOCTL_SERIAL_GET_CHARS:
        return "IOCTL_SERIAL_GET_CHARS";
    case IOCTL_SERIAL_SET_DTR:
        return "IOCTL_SERIAL_SET_DTR";
    case IOCTL_SERIAL_CLR_DTR:
        return "IOCTL_SERIAL_CLR_DTR";
    case IOCTL_SERIAL_SET_RTS:
        return "IOCTL_SERIAL_SET_RTS";
    case IOCTL_SERIAL_CLR_RTS:
        return "IOCTL_SERIAL_CLR_RTS";
    case IOCTL_SERIAL_SET_XOFF:
        return "IOCTL_SERIAL_SET_XOFF";
    case IOCTL_SERIAL_SET_XON:
        return "IOCTL_SERIAL_SET_XON";
    case IOCTL_SERIAL_SET_BREAK_ON:
        return "IOCTL_SERIAL_SET_BREAK_ON";
    case IOCTL_SERIAL_SET_BREAK_OFF:
        return "IOCTL_SERIAL_SET_BREAK_OFF";
    case IOCTL_SERIAL_GET_HANDFLOW:
        return "IOCTL_SERIAL_GET_HANDFLOW";
    case IOCTL_SERIAL_SET_HANDFLOW:
        return "IOCTL_SERIAL_SET_HANDFLOW";
    case IOCTL_SERIAL_GET_MODEMSTATUS:
        return "IOCTL_SERIAL_GET_MODEMSTATUS";
    case IOCTL_SERIAL_GET_DTRRTS:
        return "IOCTL_SERIAL_GET_DTRRTS";
    case IOCTL_SERIAL_GET_COMMSTATUS:
        return "IOCTL_SERIAL_GET_COMMSTATUS";
    case IOCTL_SERIAL_GET_PROPERTIES:
        return "IOCTL_SERIAL_GET_PROPERTIES";
    case IOCTL_SERIAL_XOFF_COUNTER:
        return "IOCTL_SERIAL_XOFF_COUNTER";
    case IOCTL_SERIAL_LSRMST_INSERT:
        return "IOCTL_SERIAL_LSRMST_INSERT";
    case IOCTL_SERIAL_GET_STATS:
        return "IOCTL_SERIAL_GET_STATS";
    case IOCTL_SERIAL_CLEAR_STATS:
        return "IOCTL_SERIAL_CLEAR_STATS";
    default:
        return "Unknown IOCTL";
    }
}