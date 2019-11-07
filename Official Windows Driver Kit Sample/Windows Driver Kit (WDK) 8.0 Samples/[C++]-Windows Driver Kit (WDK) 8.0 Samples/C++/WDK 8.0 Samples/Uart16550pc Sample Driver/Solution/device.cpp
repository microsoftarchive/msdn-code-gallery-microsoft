/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    device.c

Abstract:

    This module contains the 16550 UART controller's PNP device functions.
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
#include "uart16550pc.h"
#include "driver.h"
#include "regfile.h"
#include "isr.h"
#include "flow.h"
#include "dma.h"

#include "tracing.h"
#include "device.tmh"


//
// Uart16550pc Function: UartDeviceCreate
//
NTSTATUS
UartDeviceCreate(
    PWDFDEVICE_INIT DeviceInit
    )
{
    WDF_OBJECT_ATTRIBUTES attributes;
    WDF_PNPPOWER_EVENT_CALLBACKS pnpPowerCallbacks;
    WDFDEVICE device = NULL;
    NTSTATUS status = STATUS_UNSUCCESSFUL;

    SERCX_CONFIG serCxConfig;

    FuncEntry(TRACE_FLAG_INIT);

    WDF_OBJECT_ATTRIBUTES_INIT_CONTEXT_TYPE(&attributes, UART_DEVICE_EXTENSION);

    // Zero out the PnpPowerCallbacks structure.
    WDF_PNPPOWER_EVENT_CALLBACKS_INIT(&pnpPowerCallbacks);

    // Configure the prepare and release hardware callbacks.
    pnpPowerCallbacks.EvtDevicePrepareHardware = UartEvtPrepareHardware;
    pnpPowerCallbacks.EvtDeviceReleaseHardware = UartEvtReleaseHardware;
    pnpPowerCallbacks.EvtDeviceD0Entry = UartEvtD0Entry;
    pnpPowerCallbacks.EvtDeviceD0Exit = UartEvtD0Exit;
    pnpPowerCallbacks.EvtDeviceD0EntryPostInterruptsEnabled = UartEvtD0EntryPostInterruptsEnabled;
    pnpPowerCallbacks.EvtDeviceD0ExitPreInterruptsDisabled = UartEvtD0ExitPreInterruptsDisabled;
    WdfDeviceInitSetPnpPowerEventCallbacks(DeviceInit, &pnpPowerCallbacks);

    // Call SerCxDeviceInitConfig() to attach the SerCX to the WDF Pipeline.
    // Also creates the file object.
    status = SerCxDeviceInitConfig(DeviceInit);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(TRACE_LEVEL_ERROR,
                        TRACE_FLAG_INIT,
                        "SerCxDeviceInitConfig() returned error (%!STATUS!)",
                        status);
    }
    else
    {
        TraceMessage(TRACE_LEVEL_INFORMATION,
                        TRACE_FLAG_INIT,
                        "SerCxDeviceInitConfig() succeeded, class extension attached");
    }

    //
    // Note: The serial class extension sets a default 
    //       security descriptor to allow access to 
    //       user-mode drivers. This can be overridden 
    //       by calling WdfDeviceInitAssignSDDLString()
    //       with the desired setting. This must be done
    //       after calling SerCxDeviceInitConfig() but
    //       before WdfDeviceCreate().
    //    

    if (NT_SUCCESS(status))
    {
        attributes.SynchronizationScope = WdfSynchronizationScopeQueue;
        status = WdfDeviceCreate(&DeviceInit, &attributes, &device);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "WdfDeviceCreate() returned error (%!STATUS!)",
                            status);
        }
        
        if (NT_SUCCESS(status))
        {
            WDF_DEVICE_STATE deviceState;
            WDF_DEVICE_STATE_INIT(&deviceState);
            
            // Ensure device is disableable
            deviceState.NotDisableable = WdfFalse;
            WdfDeviceSetDeviceState(device, &deviceState);
        }
    }

    if (NT_SUCCESS(status))
    {
        status = UartInitContext(device);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "UartInitContext() returned error (%!STATUS!)",
                            status);
        }
    }

    if (NT_SUCCESS(status))
    {
        // Construct a SERCX_CONFIG structure and set the appropriate callbacks.
        SERCX_CONFIG_INIT(& serCxConfig);

        // Setup the serCxConfig data structure.
        serCxConfig.PowerManaged = WdfTrue;
        serCxConfig.EvtSerCxFileOpen = UartEvtFileOpen;
        serCxConfig.EvtSerCxFileClose = UartEvtFileClose;
        serCxConfig.EvtSerCxFileCleanup = UartEvtFileCleanup;
        serCxConfig.EvtSerCxTransmit = UartEvtTransmit;
        serCxConfig.EvtSerCxReceive = UartEvtReceive;
        serCxConfig.EvtSerCxTransmitCancel = UartEvtTransmitCancel;
        serCxConfig.EvtSerCxReceiveCancel = UartEvtReceiveCancel;
        serCxConfig.EvtSerCxWaitmask = UartEvtWaitmask;
        serCxConfig.EvtSerCxPurge = UartEvtPurge;
        serCxConfig.EvtSerCxControl = UartEvtControl;
        serCxConfig.EvtSerCxApplyConfig = UartEvtApplyConfig;

        // Fully initializes the class extension, configuring its internal
        // state and setup in the queues and event callbacks.
        status = SerCxInitialize(device, &serCxConfig);
        
        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "SerCxInitialize() returned error (%!STATUS!)",
                            status);
        }
        else
        {
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_INIT,
                            "SerCxInitialize() succeeded, class extension initialized and configured");
        }
    }

    if (NT_SUCCESS(status))
    {
        WDFSPINLOCK interruptLock;
        WDF_INTERRUPT_CONFIG interruptConfig;
        PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(device);

        // Setup a spinlock for the interrupt DPC.
        WDF_OBJECT_ATTRIBUTES_INIT(&attributes);
        attributes.ParentObject = device;

        status = WdfSpinLockCreate(&attributes, &pDevExt->DpcSpinLock);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "WdfWaitLockCreate() returned error (%!STATUS!)",
                            status);
        }
        else //if (NT_SUCCESS(status))
        {
            // Setup the Spinlock for the Interrupt itself
            WDF_OBJECT_ATTRIBUTES_INIT(&attributes);
            attributes.ParentObject = device;

            status = WdfSpinLockCreate(&attributes, &interruptLock);

            if (!NT_SUCCESS(status))
            {
                TraceMessage(TRACE_LEVEL_ERROR,
                                TRACE_FLAG_INIT,
                                "WdfSpinLockCreate() returned error (%!STATUS!)",
                                status);
            }
            else //if (NT_SUCCESS(status))
            {
                // Setup the Interrupt
                WDF_INTERRUPT_CONFIG_INIT(&interruptConfig, UartISR, UartTxRxDPCForISR);

                interruptConfig.SpinLock = interruptLock;
                interruptConfig.EvtInterruptEnable = UartEvtInterruptEnable;
                interruptConfig.EvtInterruptDisable = UartEvtInterruptDisable;

                status = WdfInterruptCreate(
                                device, 
                                &interruptConfig,
                                WDF_NO_OBJECT_ATTRIBUTES,
                                & pDevExt->WdfInterrupt);

                if (!NT_SUCCESS(status))
                {
                    TraceMessage(TRACE_LEVEL_ERROR,
                                    TRACE_FLAG_INIT,
                                    "WdfInterruptCreate() returned error (%!STATUS!)",
                                    status);
                }
                else
                {
                    TraceMessage(TRACE_LEVEL_INFORMATION,
                                    TRACE_FLAG_INIT,
                                    "WdfInterruptCreate() succeeded, interrupts configured");
                }
            }
        }

        if (NT_SUCCESS(status))
        {
            WDF_INTERRUPT_EXTENDED_POLICY policyAndGroup;
            WDF_INTERRUPT_EXTENDED_POLICY_INIT(& policyAndGroup);
            policyAndGroup.Priority = WdfIrqPriorityNormal;
            WdfInterruptSetExtendedPolicy(pDevExt->WdfInterrupt, &policyAndGroup);
        }
    }

    if (NT_SUCCESS(status))
    {
        // Configure idle settings to use system
        // managed idle timeout.
        WDF_DEVICE_POWER_POLICY_IDLE_SETTINGS idleSettings;
        WDF_DEVICE_POWER_POLICY_IDLE_SETTINGS_INIT(
            &idleSettings, 
            IdleCannotWakeFromS0);

        idleSettings.IdleTimeoutType = SystemManagedIdleTimeout;

        status = WdfDeviceAssignS0IdleSettings(device, &idleSettings);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "Failed to initalize S0 idle settings - %!STATUS!",
                            status);
        }
        else
        {
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_INIT,
                            "Idle settings configured. System managed idle timeout, idle cannot wake from S0");
        }
    }

    FuncExit(TRACE_FLAG_INIT);
    return status;
}

//
// Uart16550pc Event Handler: UartEvtPrepareHardware
//
NTSTATUS
UartEvtPrepareHardware(
    WDFDEVICE Device,
    WDFCMRESLIST Resources,
    WDFCMRESLIST ResourcesTranslated
    )
{
    UART_CONFIG_DATA config;
    PUART_CONFIG_DATA pConfig = &config;
    NTSTATUS status = STATUS_UNSUCCESSFUL;

    FuncEntry(TRACE_FLAG_INIT);

    status = UartMapHardwareResources(
                    Device, 
                    Resources, 
                    ResourcesTranslated,
                    pConfig);

    // If we got the resources we need, then status == STATUS_SUCCESS.
    // Proceeds with assigning values in the device extension.
    if (NT_SUCCESS(status))
    {
        status = UartInitController(Device, pConfig);
    }

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Event Handler: UartEvtReleaseHardware
//
NTSTATUS
UartEvtReleaseHardware(
    IN  WDFDEVICE Device,
    IN  WDFCMRESLIST ResourcesTranslated
    )
{
    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(ResourcesTranslated);

    
    FuncEntry(TRACE_FLAG_INIT);

    // TODO
    // Implement this function
    PUART_DEVICE_EXTENSION pDevExt;
    pDevExt = UartGetDeviceExtension(Device);

    //
    // Reset and put the device into a known initial state before releasing the hw resources.
    // Since we have already reset the device in our close handler, we don't have to
    // do anything other than unmapping the I/O resources.
    //

    //
    // Unmap any Memory-Mapped registers. Disconnecting from the interrupt will
    // be done automatically by the framework.
    //
    if (pDevExt->UnMapRegisters)
    {
        MmUnmapIoSpace(pDevExt->Controller, pDevExt->SpanOfController);

        pDevExt->Controller = NULL;
        pDevExt->SpanOfController = 0;
    }

    if (pDevExt->readAdapter != NULL)
    {
        pDevExt->readAdapter->DmaOperations->FreeAdapterChannel(pDevExt->readAdapter);
        pDevExt->readAdapter->DmaOperations->PutDmaAdapter(pDevExt->readAdapter);
    }

    if (pDevExt->writeAdapter != NULL)
    {
        pDevExt->writeAdapter->DmaOperations->FreeAdapterChannel(pDevExt->writeAdapter);
        pDevExt->writeAdapter->DmaOperations->PutDmaAdapter(pDevExt->writeAdapter);
    }

    // Release software FIFO
    if (pDevExt->FIFOBuffer != NULL)
    {
        ExFreePoolWithTag(pDevExt->FIFOBuffer, SERIAL_SOFTWARE_FIFO_TAG);
        pDevExt->FIFOBuffer = NULL;
    }

    FuncExit(TRACE_FLAG_INIT);

    return STATUS_SUCCESS;
}

//
// Uart16550pc Event Handler: UartEvtD0Entry
//
NTSTATUS
UartEvtD0Entry(
    _In_ WDFDEVICE Device,
    _In_ WDF_POWER_DEVICE_STATE PreviousState
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(PreviousState);

    FuncEntry(TRACE_FLAG_INIT);

    status = UartRestoreState(Device);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Event Handler: UartEvtD0Exit
//
NTSTATUS
UartEvtD0Exit(
    _In_ WDFDEVICE Device,
    _In_ WDF_POWER_DEVICE_STATE TargetState
    )
{
    NTSTATUS status;
    
    UNREFERENCED_PARAMETER(TargetState);

    FuncEntry(TRACE_FLAG_INIT);

    status = UartSaveState(Device);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Event Handler: UartEvtD0EntryPostInterruptsEnabled
//
NTSTATUS
UartEvtD0EntryPostInterruptsEnabled(
    _In_ WDFDEVICE Device,
    _In_ WDF_POWER_DEVICE_STATE PreviousState
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(PreviousState);

    FuncEntry(TRACE_FLAG_INIT);

    status = UartRestoreStatePostInterruptsEnabled(Device);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Event Handler: UartEvtD0ExitPreInterruptsDisabled
//
NTSTATUS
UartEvtD0ExitPreInterruptsDisabled(
    _In_ WDFDEVICE Device,
    _In_ WDF_POWER_DEVICE_STATE TargetState
    )
{
    NTSTATUS status;
    
    UNREFERENCED_PARAMETER(TargetState);

    FuncEntry(TRACE_FLAG_INIT);

    status = UartSaveStatePreInterruptsDisabled(Device);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartEvtInterruptEnable
//
NTSTATUS 
UartEvtInterruptEnable(
    _In_  WDFINTERRUPT /* Interrupt */,
    _In_  WDFDEVICE AssociatedDevice
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    UCHAR regModemControl;
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(AssociatedDevice);

    // This is necessary to enable the interrupt out line
    regModemControl = (UCHAR)(READ_MODEM_CONTROL(pDevExt, pDevExt->Controller));
    regModemControl = regModemControl | SERIAL_MCR_OUT2;
    WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);

    UartEnableInterrupts(pDevExt);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartEvtInterruptDisable
//
NTSTATUS 
UartEvtInterruptDisable(
    _In_  WDFINTERRUPT /* Interrupt */,
    _In_  WDFDEVICE AssociatedDevice
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    UCHAR regModemControl;
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(AssociatedDevice);
    
    // Disables the interrupt line by ANDing with bitwise-NOT of SERIAL_MCR_OUT2.
    regModemControl = (UCHAR)(READ_MODEM_CONTROL(pDevExt, pDevExt->Controller));
    regModemControl = regModemControl & (~SERIAL_MCR_OUT2);
    WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);

    DISABLE_ALL_INTERRUPTS(pDevExt, pDevExt->Controller);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartEnableInterrupts
//
VOID
UartEnableInterrupts(
    _In_ PUART_DEVICE_EXTENSION pDevExt
    )
{
    //
    // Note: This function must be called with the interrupt lock held
    //

    UCHAR interrupts = SERIAL_IER_RLS | SERIAL_IER_MS;

    if (!pDevExt->DmaReceiveEnabled)
    {
        // Listen for receive interrupts if receive DMA is not being used
        interrupts |= SERIAL_IER_RDA;
    }

    if (!pDevExt->DmaTransmitEnabled)
    {
        // Listen for transmit interrupts if transmit DMA is not being used 
        interrupts |= SERIAL_IER_THR;
    }

    WRITE_INTERRUPT_ENABLE(pDevExt, pDevExt->Controller, interrupts);
    }

//
// Uart16550pc Function: UartReceiveBytesFromRXFIFO
//
NTSTATUS
UartReceiveBytesFromRXFIFO(
    _In_ WDFDEVICE Device,
    _In_ PUART_DEVICE_EXTENSION pDevExt,
    _In_ BOOLEAN canRetrieveBuffer,
    _In_ BOOLEAN canChangeReceiveFlow)
{
    //
    // NOTE: This function must be called with DpcSpinLock and ReceiveBufferSpinLock held
    //

    NTSTATUS status = STATUS_SUCCESS;
    BOOLEAN usingFlowControl;
    BOOLEAN hasAcquiredBuffer = FALSE;
    BOOLEAN canReceive = FALSE;
    BOOLEAN canTransmit = FALSE;
    BOOLEAN hasErrors = FALSE;

    // Synchronize with interrupt lock
    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
    usingFlowControl = UsingRXFlowControl(pDevExt);
    UartEvaluateLineStatus(Device, &canReceive, &canTransmit, &hasErrors);
    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    // Check for cached buffer (PIO receive buffer or MDL used in prior DMA transaction)
    if (!HasCachedReceiveBuffer(pDevExt))
    {
        // If not using hardware flow control, get new temporary buffer to ensure
        // Rx Fifo does not overflow.
        // All buffer caching takes place in transmit and receive callbacks.
        // A buffer that is retrieved here will be released once filled or
        // no more data has arrived, so it will not be subject to interval timeout.
        if (canRetrieveBuffer)
        {
            status = SerCxRetrieveReceiveBuffer(
                Device, 
                pDevExt->RxFifoSize,
                &pDevExt->PIOReceiveBuffer);
            
            if (NT_SUCCESS(status))
            {
                // Buffer was retrieved in this function, and should therefore be released
                // before exiting
                hasAcquiredBuffer = TRUE;

                pDevExt->ReceiveProgress = 0;
            }
            else if (status == STATUS_INSUFFICIENT_RESOURCES)
            {
                WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

                // Discard the byte.
                READ_RECEIVE_BUFFER(pDevExt, pDevExt->Controller);
                pDevExt->PerfStats.BufferOverrunErrorCount++;
                pDevExt->ErrorWord |= SERIAL_ERROR_QUEUEOVERRUN;

                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_RECEIVE,
                    "No receive buffer available, discarding data - "
                    "%!STATUS!",
                    status);

                WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
            }
            else
            {
                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_RECEIVE,
                    "SerCxRetrieveReceiveBuffer failed - %!STATUS!",
                    status);

                NT_ASSERTMSG("SerCxRetrieveReceiveBuffer shouldn't fail", NT_SUCCESS(status));
            }
        }
        else
        {
            // Data is available to receive, but we chose not to retrieve a buffer
            // and don't have a pended read request
            TraceMessage(TRACE_LEVEL_WARNING,
                            TRACE_FLAG_RECEIVE,
                            "Data received using flow control and no pended buffer.  Data may be lost.");
        }
    }

    // If we have a valid rxBuffer (perhaps by grabbing one a moment ago).
    // Again, this will be DMA MDL if DMA couldn't transfer all the bytes
    // due to a minimum unit length greater than 1, or DMA timed out.
    if (HasCachedReceiveBuffer(pDevExt)  && !pDevExt->DmaReceiveEnabled)
    {
        ULONG bufferLength = GetReceiveBufferLength(pDevExt);

        // Stop the interval timeout timer as it might be running.
        // Timer shouldn't have started if given length of 0ms.
        // Let class extension take care of stopping timeout if not using flow control
        if (pDevExt->TimeoutLengthMs > 0 && usingFlowControl)
        {
            TraceMessage(
                TRACE_LEVEL_VERBOSE,
                TRACE_FLAG_RECEIVE,
                "Stopping interval timeout");

            UartDoTimeoutWork(pDevExt, TIMEOUT_STOP);
        }

        // Receive available bytes (copy from the interrupt record)
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        while (canReceive && (pDevExt->ReceiveProgress < bufferLength))
        {
            *GetNextReceiveBufferByteAddress(pDevExt) = READ_RECEIVE_BUFFER(
                pDevExt, 
                pDevExt->Controller);

            pDevExt->ReceiveProgress++;
                    
            UartEvaluateLineStatus(Device, &canReceive, &canTransmit, &hasErrors);
        }

        if (canChangeReceiveFlow)
        {
            // This line shouldn't be reached when this function is called from the DPC
            // with flow control enabled, so it is not expected that the software FIFO
            // may be non-empty.
            NT_ASSERT(pDevExt->FIFOBufferBytes == 0);

            UartFlowReceiveAvailable(Device);
        }

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        if (pDevExt->PIOReceiveBuffer.Buffer != NULL)
        {
            TraceMessage(TRACE_LEVEL_VERBOSE,
                            TRACE_FLAG_RECEIVE,
                            "Received %lu bytes to buffer %p",
                            pDevExt->ReceiveProgress,
                            pDevExt->PIOReceiveBuffer.Buffer);
        }
        else
        {
            TraceMessage(TRACE_LEVEL_VERBOSE,
                            TRACE_FLAG_RECEIVE,
                            "Received %lu bytes to mdl %p",
                            pDevExt->ReceiveProgress,
                            pDevExt->ReceiveMdl);
        }

        // Always return buffer if it was retrieved in this function, or return buffer if full
        if (hasAcquiredBuffer || pDevExt->ReceiveProgress >= bufferLength)
        {
            // Release cached buffer
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

            // Since both buffers will not be used simultaneously, it's safe to set both to NULL
            pDevExt->PIOReceiveBuffer.Buffer = NULL;
            pDevExt->ReceiveMdl = NULL;
        }
        else
        {
            // Buffer was not returned and there are still bytes left to read,
            // start interval timer if necessary
            if (pDevExt->TimeoutLengthMs > 0)
            {
                TraceMessage(
                    TRACE_LEVEL_VERBOSE,
                    TRACE_FLAG_RECEIVE,
                    "Starting interval timeout");

                UartDoTimeoutWork(pDevExt, TIMEOUT_START);
            }
        }
    }

    return status;
}

//
// Uart16550pc Function: UartReceiveBytesToSoftwareFIFO
//
VOID
UartReceiveBytesToSoftwareFIFO(
    _In_ WDFDEVICE Device,
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    //
    // NOTE: This function must be called with DpcSpinLock and ReceiveBufferSpinLock held
    //
    BOOLEAN canReceive = FALSE;
    BOOLEAN canTransmit = FALSE;
    BOOLEAN hasErrors = FALSE;

    // Receive available bytes (copy from the interrupt record)
    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    // Reset software FIFO pointer to first byte;
    pDevExt->FIFOBufferNextByte = pDevExt->FIFOBuffer;

    UartEvaluateLineStatus(Device, &canReceive, &canTransmit, &hasErrors);

    while (canReceive && (pDevExt->FIFOBufferBytes < SERIAL_SOFTWARE_FIFO_SIZE))
    {
        pDevExt->FIFOBuffer[pDevExt->FIFOBufferBytes] = READ_RECEIVE_BUFFER(
            pDevExt, 
            pDevExt->Controller);

        pDevExt->FIFOBufferBytes++;

        UartEvaluateLineStatus(Device, &canReceive, &canTransmit, &hasErrors);
    }

    // The software FIFO should be at least the size of the hardware FIFO,
    // so the hardware FIFO is expected to be empty.  However, any bytes
    // remaining in the hardware FIFO may be lost.
    if (canReceive)
    {
        TraceMessage(
            TRACE_LEVEL_WARNING,
            TRACE_FLAG_RECEIVE,
            "Bytes remaining in hardware FIFO may be lost");
    }

    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
}

//
// Uart16550pc Function: UartReceiveBytesFromSoftwareFIFO
//
VOID
UartReceiveBytesFromSoftwareFIFO(
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    //
    // NOTE: This function must be called with DpcSpinLock and ReceiveBufferSpinLock held
    //

    ULONG bufferLength = GetReceiveBufferLength(pDevExt);

    while (pDevExt->FIFOBufferBytes > 0 && pDevExt->ReceiveProgress < bufferLength)
    {
        pDevExt->FIFOBufferBytes--;

        *GetNextReceiveBufferByteAddress(pDevExt) = *pDevExt->FIFOBufferNextByte;

        // Advance FIFO buffer pointer
        pDevExt->FIFOBufferNextByte++;

        pDevExt->ReceiveProgress++;
    }
}

//
// Uart16550pc Function: UartUseDma
//
BOOLEAN
UartUseDma(
    _In_ PUART_DEVICE_EXTENSION pDevExt,
    _In_ WDF_DMA_DIRECTION Direction,
    _In_ size_t Length
    )
{
    //
    // Helper function to determine if DMA can be used for a specific transmit or receive IO
    // Note: DpcSpinLock and interrupt lock should be held when then function is called
    //

    UNREFERENCED_PARAMETER(Length);
    BOOLEAN useDma = FALSE;

    // Ensure that DMA has been configured for the direction of the transfer
    if (pDevExt->DmaTransmitConfigured && Direction == WdfDmaDirectionWriteToDevice)
    {
        // Ensure that TX HW flow control and automatic TX flow control are enabled
        if (UsingTXFlowControl(pDevExt) && pDevExt->AutoTXFlowControlEnabled)
        {
            // Don't use DMA if length greater than cap
            if (Length <= UART_DMA_MAX_TRANSFER_LENGTH_BYTES)
            {
                useDma = TRUE;
            }
        }
    }

    // Ensure that DMA has been configured for the direction of the transfer
    if (pDevExt->DmaReceiveConfigured && Direction == WdfDmaDirectionReadFromDevice)
    {
        // Ensure that RX HW flow control and automatic RX flow control are enabled
        if (UsingRXFlowControl(pDevExt) && pDevExt->AutoRXFlowControlEnabled)
        {
            // Don't use DMA if length greater than cap
            if (Length <= UART_DMA_MAX_TRANSFER_LENGTH_BYTES)
            {
                useDma = TRUE;
            }
        }
    }

    return useDma;
}

//
// Uart16550pc Function: HasCachedTransmitBuffer
//
BOOLEAN
HasCachedTransmitBuffer(
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    // PIO Buffer and MDL shouldn't both be cached at the same time
    NT_ASSERT((pDevExt->PIOTransmitBuffer.Buffer == NULL) || (pDevExt->TransmitMdl == NULL));

    return ((pDevExt->PIOTransmitBuffer.Buffer != NULL) || (pDevExt->TransmitMdl != NULL));
}

//
// Uart16550pc Function: HasCachedReceiveBuffer
//
BOOLEAN
HasCachedReceiveBuffer(
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    // PIO Buffer and MDL shouldn't both be cached at the same time
    NT_ASSERT((pDevExt->PIOReceiveBuffer.Buffer == NULL) || (pDevExt->ReceiveMdl == NULL));

    return ((pDevExt->PIOReceiveBuffer.Buffer != NULL) || (pDevExt->ReceiveMdl != NULL));
}

//
// Uart16550pc Function: GetNextTransmitBufferByte
//
UCHAR GetNextTransmitBufferByte(
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    UCHAR byte = 0;

    // Only PIO Buffer or MDL should be cached
    NT_ASSERT((pDevExt->PIOTransmitBuffer.Buffer == NULL) != (pDevExt->TransmitMdl == NULL));

    if (pDevExt->PIOTransmitBuffer.Buffer != NULL)
    {
        // PIO Buffer
        byte = (pDevExt->PIOTransmitBuffer.Buffer[pDevExt->TransmitProgress]);
    }
    else
    {
        // Transmit MDL
        byte = *GetMdlByteAddressAt(pDevExt->TransmitMdl, pDevExt->TransmitProgress);
    }

    return byte;
}

//
// Uart16550pc Function: GetNextReceiveBufferByteAddress
//
PUCHAR GetNextReceiveBufferByteAddress(
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    PUCHAR byteAddress = NULL;

    // Only PIO Buffer or MDL should be cached
    NT_ASSERT((pDevExt->PIOReceiveBuffer.Buffer == NULL) != (pDevExt->ReceiveMdl == NULL));

    if (pDevExt->PIOReceiveBuffer.Buffer != NULL)
    {
        // PIO Buffer
        byteAddress = (pDevExt->PIOReceiveBuffer.Buffer + pDevExt->ReceiveProgress);
    }
    else
    {
        // Receive MDL
        byteAddress = GetMdlByteAddressAt(pDevExt->ReceiveMdl, pDevExt->ReceiveProgress);
    }

    return byteAddress;
}

//
// Uart16550pc Function: GetTransmitBufferLength
//
ULONG
GetTransmitBufferLength(
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    ULONG length = 0;

    // Only PIO Buffer or MDL should be cached
    NT_ASSERT((pDevExt->PIOTransmitBuffer.Buffer == NULL) != (pDevExt->TransmitMdl == NULL));

    // PIO Buffer
    if (pDevExt->PIOTransmitBuffer.Buffer != NULL)
    {
        length = pDevExt->PIOTransmitBuffer.Length;
    }
    else
    {
        // Transmit MDL
        length = pDevExt->DmaTransmitLength;
    }

    return length;
}

//
// Uart16550pc Function: GetReceiveBufferLength
//
ULONG
GetReceiveBufferLength(
    _In_ PUART_DEVICE_EXTENSION pDevExt)
{
    ULONG length = 0;

    // Only PIO Buffer or MDL should be cached
    NT_ASSERT((pDevExt->PIOReceiveBuffer.Buffer == NULL) != (pDevExt->ReceiveMdl == NULL));

    // PIO Buffer
    if (pDevExt->PIOReceiveBuffer.Buffer != NULL)
    {
        length = pDevExt->PIOReceiveBuffer.Length;
    }
    else
    {
        // Transmit MDL
        length = pDevExt->DmaReceiveLength;
    }

    return length;
}

//
// Uart16550pc Function: GetMdlByteAddressAt
//
PUCHAR
GetMdlByteAddressAt(
    _In_ PMDL baseMdl,
    _In_ ULONG index)
{
    PMDL mdl = baseMdl;
    ULONG indexCount = 0;
    PUCHAR byteAddress = NULL;

    while (mdl != NULL && byteAddress == NULL)
    {
        indexCount += mdl->ByteCount;

        // Byte address is on this MDL node
        if (indexCount > index)
        {
            byteAddress = (PUCHAR) MmGetSystemAddressForMdlSafe(
                mdl,
                NormalPagePriority);

            byteAddress += mdl->ByteCount - (indexCount - index);
        }

        mdl = mdl->Next;
    }

    NT_ASSERT(byteAddress != NULL);

    return byteAddress;
}

//
// Uart16550pc Function: GetMdlLength
//
ULONG 
GetMdlLength(
    _In_ PMDL baseMdl)
{
    ULONG length = 0;
    PMDL mdl = baseMdl;

    while (mdl != NULL)
    {
        length += mdl->ByteCount;
        mdl = mdl->Next;
    }

    return length;
}