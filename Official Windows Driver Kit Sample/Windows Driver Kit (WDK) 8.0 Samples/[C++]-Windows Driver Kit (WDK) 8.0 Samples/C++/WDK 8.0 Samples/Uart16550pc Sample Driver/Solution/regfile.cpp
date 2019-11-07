/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    regfile.cpp

Abstract:

    This module contains the 16550 UART controller's register access functions.
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
#include "uart16550pc.h"
#include "dma.h"

#include "tracing.h"
#include "regfile.tmh"

//
// Uart16550pc Function: UartMapHardwareResources
//
NTSTATUS
UartMapHardwareResources(
    _In_ WDFDEVICE Device,
    _In_ WDFCMRESLIST Resources,
    _In_ WDFCMRESLIST ResourcesTranslated,
    _Out_ PUART_CONFIG_DATA PConfig
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    NTSTATUS status = STATUS_SUCCESS;

    PCM_PARTIAL_RESOURCE_DESCRIPTOR pPartialResourceDescRaw;
    PCM_PARTIAL_RESOURCE_DESCRIPTOR pPartialResourceDescTrans;
    ULONG intResourcesFound = 0;
    ULONG ioResourcesFound = 0;
    ULONG memResourcesFound = 0;
    ULONG dmaResourcesFound = 0;
    ULONG resourceListCountRaw = 0;
    ULONG resourceListCountTrans = 0;
    ULONG i = 0;
   
    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(Device);

    // Determines how many resources we will iterate over.
    resourceListCountRaw = WdfCmResourceListGetCount(Resources);
    resourceListCountTrans = WdfCmResourceListGetCount(ResourcesTranslated);

    NT_ASSERT(resourceListCountRaw == resourceListCountTrans);

    if (resourceListCountRaw != resourceListCountTrans)
    {
        status = STATUS_UNSUCCESSFUL;
    }

    if (NT_SUCCESS(status))
    {
        for (i = 0; i < resourceListCountRaw; i++)
        {
            // Gets the raw partial resource descriptor.
            pPartialResourceDescRaw = 
                            WdfCmResourceListGetDescriptor(Resources, i);

            // Gets the translated partial resource descriptor.
            pPartialResourceDescTrans = 
                            WdfCmResourceListGetDescriptor(ResourcesTranslated, i);

            // Switch based upon the type of resource.
            switch (pPartialResourceDescTrans->Type)
            {
            case CmResourceTypePort:
                if (ioResourcesFound == 0)
                {
                    PConfig->ControllerRaw = 
                                    pPartialResourceDescRaw->u.Port.Start;
                    PConfig->ControllerTranslated = 
                                    pPartialResourceDescTrans->u.Port.Start;
                    PConfig->AddressSpace = 
                                    pPartialResourceDescTrans->Flags;
                }
                ioResourcesFound++;
                break;

            case CmResourceTypeMemory:
                if ((memResourcesFound == 0) && (ioResourcesFound == 0))
                {
                    PConfig->ControllerRaw = 
                                    pPartialResourceDescRaw->u.Memory.Start;
                    PConfig->ControllerTranslated = 
                                    pPartialResourceDescTrans->u.Memory.Start;
                    PConfig->AddressSpace = 
                                    CM_RESOURCE_PORT_MEMORY;
                    PConfig->SpanOfController = 
                                    SERIAL_REGISTER_SPAN;
                }
                memResourcesFound++;
                break;

            case CmResourceTypeInterrupt:
                if (intResourcesFound == 0)
                {
                    PConfig->VectorTranslated = 
                                    pPartialResourceDescTrans->u.Interrupt.Vector;
                    PConfig->IrqlTranslated = 
                                    pPartialResourceDescTrans->u.Interrupt.Level;
                    PConfig->Affinity = 
                                    pPartialResourceDescTrans->u.Interrupt.Affinity;
                }
                intResourcesFound++;
                break;

            case CmResourceTypeDma:
                if (dmaResourcesFound < 2)
                {
                    PConfig->DmaResourceDescriptor[dmaResourcesFound] = pPartialResourceDescTrans;
                }
                dmaResourcesFound++;
                break;

            default:
                break;
            }
        }

        // The UART is addressed either as an IO resource (ioResourceFound != 0)
        // or as a memory resource (memResourceFound != 0).  It cannot be both
        // and it cannot be neither.

        if (ioResourcesFound && memResourcesFound)
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "Both IO or memory resources found.");
            status = STATUS_INSUFFICIENT_RESOURCES;
        }
        else if ((!ioResourcesFound) && (!memResourcesFound))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "No IO or memory resource found.");
            status = STATUS_INSUFFICIENT_RESOURCES;
        }
    }
    
    if (NT_SUCCESS(status))
    {
        if (!intResourcesFound)
        {
           TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "No interrupt resource found.");
            status = STATUS_INSUFFICIENT_RESOURCES;
        }
        else
        {
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_INIT,
                            "Found interrupt resource");
        }
    }

    if (NT_SUCCESS(status))
    {
        if (ioResourcesFound)
        {
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_INIT,
                            "Found IO resource.  Using read/write port.");
            pDevExt->UartReadUChar = UartReadPortUChar;
            pDevExt->UartWriteUChar = UartWritePortUChar;
        }
        else if (memResourcesFound) 
        {
            TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_INIT,
                            "Found memory resource.  Using read/write register.");
            pDevExt->UartReadUChar = UartReadRegisterUChar;
            pDevExt->UartWriteUChar = UartWriteRegisterUChar;
        }
        else
        {
            NT_ASSERTMSG("Reached unreachable code.", FALSE);
        }
    }

    if (NT_SUCCESS(status))
    {
        // Initialize DMA device extension values
        pDevExt->DmaTransmitConfigured = FALSE;
        pDevExt->DmaReceiveConfigured = FALSE;
        pDevExt->DmaTransmitEnabled = FALSE;
        pDevExt->DmaReceiveEnabled = FALSE;

        // Initialize DMA if a resource was found
        if (dmaResourcesFound > 0)
        {
            // The behavior of this function is to enable DMA TX, and then DMA RX if
            // a second DMA resource was found.  This function should be modified if
            // DMA RX should be enabled first.
            // If either initialization fails then the driver initialization will also fail.
            // If no DMA resource was found, this function will not be called,
            // and subsequent I/O transfers will not attempt to use DMA.
            status = UartInitDMA(Device, PConfig, dmaResourcesFound);
        }
    }

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartGetMappedAddress
//
PVOID
UartGetMappedAddress(
    _In_ PHYSICAL_ADDRESS IoAddress,
    _In_ ULONG NumberOfBytes,
    _In_ ULONG AddressSpace,
    _Out_ PBOOLEAN MappedAddress
    )
{
    PVOID address = NULL;

    FuncEntry(TRACE_FLAG_INIT);
    // Maps the device base address into the virtual address space
    // if the address is in memory space.

    if (!AddressSpace)
    {
        address = MmMapIoSpace(IoAddress, NumberOfBytes, MmNonCached);

        *MappedAddress = (address != NULL);
    } 
    else
    {
        address = ULongToPtr(IoAddress.LowPart);
        *MappedAddress = FALSE;
    }
    
    FuncExit(TRACE_FLAG_INIT);

    return address;
}

//
// Uart16550pc Function: UartInitController
//
NTSTATUS
UartInitController(
    _In_ WDFDEVICE Device,
    _In_ PUART_CONFIG_DATA PConfig
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    NTSTATUS status = STATUS_UNSUCCESSFUL;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(Device);

    // Maps the serial device's memory into virtual memory.
    pDevExt->Controller = (PUCHAR) UartGetMappedAddress(
                    PConfig->ControllerTranslated,
                    PConfig->SpanOfController,
                    (BOOLEAN) PConfig->AddressSpace,
                    &pDevExt->UnMapRegisters);

    if (!pDevExt->Controller)
    {
        TraceMessage(TRACE_LEVEL_ERROR,
                        TRACE_FLAG_INIT,
                        "Could not map device memory to virtual memory.");
        pDevExt->UnMapRegisters = FALSE;
        status = STATUS_NONE_MAPPED;
    }
    else
    {
        TraceMessage(TRACE_LEVEL_INFORMATION,
                        TRACE_FLAG_INIT,
                        "Device memory mapped to virtual memory");
        status = STATUS_SUCCESS;
    }

    if (NT_SUCCESS(status))
    {
        pDevExt->AddressSpace = PConfig->AddressSpace;
        pDevExt->SpanOfController = PConfig->SpanOfController;

        pDevExt->Vector = PConfig->VectorTranslated;
        pDevExt->Irql = (UCHAR) PConfig->IrqlTranslated;
        pDevExt->InterruptMode = PConfig->InterruptMode;
        pDevExt->Affinity = PConfig->Affinity;

        // This code assumes a FIFO is present.
        UCHAR fifoControlRegister = (UCHAR)(SERIAL_FCR_ENABLE
                            | SERIAL_8_BYTE_HIGH_WATER
                            | SERIAL_FCR_RCVR_RESET
                            | SERIAL_FCR_TXMT_RESET);
        WRITE_FIFO_CONTROL(pDevExt, pDevExt->Controller, fifoControlRegister);
        pDevExt->FifoControl = fifoControlRegister & 
            (~(SERIAL_FCR_RCVR_RESET | SERIAL_FCR_TXMT_RESET));
        pDevExt->LineStatus = READ_LINE_STATUS(pDevExt, pDevExt->Controller);
        pDevExt->ModemStatus = READ_MODEM_STATUS(pDevExt, pDevExt->Controller);

        pDevExt->LineControl = 0;

        // TODO: For each transfer direction, DMA must have automatic
        // hardware flow control enabled and the corresponding flag set
        // to TRUE in order to use DMA.  This is platform specific.
        pDevExt->AutoTXFlowControlEnabled = FALSE;
        pDevExt->AutoRXFlowControlEnabled = FALSE;
        pDevExt->ModemControl = 0;

        pDevExt->DivisorLatch = 0;
        pDevExt->DeviceOpened = FALSE;
        pDevExt->DeviceActive = FALSE;
        pDevExt->NewConfigOutstanding = FALSE;
        pDevExt->ResetFifoOnD0Entry = FALSE;

        pDevExt->TxFifoSize = SERIAL_TX_FIFO_SIZE_DEFAULT;
        pDevExt->RxFifoSize = SERIAL_RX_FIFO_SIZE_DEFAULT;

        TraceMessage(TRACE_LEVEL_INFORMATION,
                        TRACE_FLAG_INIT,
                        "Controller device initialized");
    }
    else
    {
        TraceMessage(TRACE_LEVEL_ERROR,
                        TRACE_FLAG_INIT,
                        "Controller device not initialized");
    }

    if (NT_SUCCESS(status))
    {
        status = UartInitCachedBuffers(Device);
    }

    // This driver uses a small buffer to store the contents of the
    // FIFO when flow control is enabled, in which case buffers obtained
    // from the class extension are cached until filled or cancelled.
    // This controller driver maintained buffer, rather than the class
    // extension ring buffer, is necessary because the controller driver
    // is responsible for handling interval timeout in this case.  This 
    // driver does not know whether an obtained request buffer has been
    // partially filled by the ring buffer's contents, so it will be unable
    // to start the interval timer until the first byte it receives AFTER
    // obtaining the buffer.  The interval timeout must be started
    // when data stops arriving after the first byte has been read into
    // the request buffer.
    // For the purposes of this sample the buffer has a fixed size and
    // will NOT resize itself if the RX FIFO size changes.
    if (NT_SUCCESS(status))
    {
        pDevExt->FIFOBuffer = (PUCHAR) ExAllocatePoolWithTag(NonPagedPoolNx,
            SERIAL_SOFTWARE_FIFO_SIZE,
            SERIAL_SOFTWARE_FIFO_TAG);

        if (pDevExt->FIFOBuffer == NULL)
        {
            status = STATUS_INSUFFICIENT_RESOURCES;

            TraceMessage(TRACE_LEVEL_ERROR,
                            TRACE_FLAG_INIT,
                            "Software FIFO not initialized");
        }
    }

    if (NT_SUCCESS(status))
    {
        pDevExt->FIFOBufferNextByte = pDevExt->FIFOBuffer;
        pDevExt->FIFOBufferBytes = 0;
    }

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartSaveStatePreInterruptsDisabled
//
NTSTATUS
UartSaveStatePreInterruptsDisabled(
   _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    UCHAR regModemControl;
    BOOLEAN canReceive = FALSE;
    BOOLEAN canTransmit = FALSE;
    BOOLEAN hasErrors = FALSE;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(Device);

    WdfSpinLockAcquire(pDevExt->DpcSpinLock);

    // Mark device inactive
    pDevExt->DeviceActive = FALSE;

    // Now that the device has been marked inactive,
    // this flag is reset to indicate the saved register
    // state does not contain new config (yet). If this
    // flag is asserted, UartSaveState will know not to
    // overwrite the saved state with the HW register
    // values.
    pDevExt->NewConfigOutstanding = FALSE;

    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

    // Save MCR to device context here rather than in D0Exit.  This is so if
    // RTS or DTR control hardware flow control is used, the current RTS and DTR
    // bits will be restored upon D0 entry.  After saving the MCR register, RTS and
    // DTR bits will be set to 0 regardless of flow control type to prevent more
    // bytes from entering the FIFO before the FIFO is saved.
    pDevExt->ModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);

    // Deassert the RTS and DTR lines to prevent any more data from entering the FIFO before saving
    // those bytes, if any. If RTS or DTR control hardware flow control is used, the lines are still
    // deasserted, but the current RTS and DTR state will be restored in UartRestoreState.
    // Automatic flow control MUST be disabled here if it's being used.
    regModemControl = pDevExt->ModemControl;
    regModemControl &= ~(SERIAL_MCR_RTS | SERIAL_MCR_DTR);
    WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);

    // Poll the line status register to check whether there are bytes to read
    UartEvaluateLineStatus(Device, &canReceive, &canTransmit, &hasErrors);

    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    // Receive the contents from the RX FIFO to cached buffer, or retrieve chunk of ring buffer from
    // the class extension and read into that.
    if (canReceive)
    {
        TraceMessage(TRACE_LEVEL_INFORMATION,
                            TRACE_FLAG_INIT,
                            "Saving bytes in Rx FIFO before D0 exit");


        WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);

        // Attempt to receive bytes from the FIFO, sharing the same code path exercised in the Tx/Rx DPC.
        // In this case those bytes will be read into a buffer, even if RX flow control is being used,
        // and then immediately read into the next received request.  This is needed because there may be
        // bytes in the FIFO that will be lost in the transition to DX if they are not read out here.
        // Please note that if RX flow control is not being used, bytes may still be received after interrupts
        // are disabled and after UartEvtD0Exit() is called, but before power to the UART is cut.  These bytes
        // may be lost.  If this is unacceptable, consider using pended reads to keep the UART from going idle,
        // or not using system idle.

        // The software FIFO will be used instead of ring buffer if flow control is used.  If flow control
        // is not used, the ring buffer must be used in order for the class extension to correctly start
        // the interval timeout timer.
        if (UsingRXFlowControl(pDevExt))
        {
            // The FIFO should only be saved if the software FIFO is empty.  Otherwise, flow control
            // should still be deasserted, and there shouldn't be any bytes in the FIFO.
            NT_ASSERT(pDevExt->FIFOBufferBytes == 0);

            //If the file handle is closed, the software FIFO has already
            // been flushed, so the bytes should not be saved.
            if (pDevExt->DeviceOpened)
            {
                // Read bytes from FIFO to software FIFO
                UartReceiveBytesToSoftwareFIFO(Device, pDevExt);
            }
        }
        else
        {
            NTSTATUS fifoReadStatus = UartReceiveBytesFromRXFIFO(Device, pDevExt, TRUE, FALSE);

            if (!NT_SUCCESS(fifoReadStatus))
            {
                TraceMessage(
                    TRACE_LEVEL_WARNING,
                    TRACE_FLAG_RECEIVE,
                    "%!FUNC! Error reading from fifo - %!STATUS!",
                    fifoReadStatus);
            }
        }

        WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);
    }

    WdfSpinLockRelease(pDevExt->DpcSpinLock);
    
    FuncExit(TRACE_FLAG_INIT);

    // Failure to empty the FIFO is traced, but ultimately ignored.  The most likely
    // reason the FIFO will fail to empty is that the class extension was unable to
    // provide a buffer to read into.  This can happen if the file has already closed.
    return STATUS_SUCCESS;
}

//
// Uart16550pc Function: UartSaveState
//
NTSTATUS
UartSaveState(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(Device);

    WdfSpinLockAcquire(pDevExt->DpcSpinLock);

    // Save hardware registers. Note, This function is called
    // with interrupts disconnected and IO gated, so there
    // is no need to synchronize with the interrupt lock. We do,
    // however, use the spinlock to synchronize with the open callback.
    if (pDevExt->NewConfigOutstanding == TRUE)
    {
        // Line control and divisor latch members already
        // contain the most up-to-date state. The latest 
        // break condition needs to be applied.

        // Combine latest break condition with latest line control config
        pDevExt->LineControl = 
            (READ_LINE_CONTROL(pDevExt, pDevExt->Controller) & SERIAL_LCR_BREAK) |
            (pDevExt->LineControl & ~SERIAL_LCR_BREAK);

        pDevExt->NewConfigOutstanding = FALSE;
    }
    else
    {
        pDevExt->LineControl = READ_LINE_CONTROL(pDevExt, pDevExt->Controller);
        READ_DIVISOR_LATCH(pDevExt, pDevExt->Controller, &pDevExt->DivisorLatch);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                        TRACE_FLAG_INIT,
                        "Controller device state saved. Device idle");

    WdfSpinLockRelease(pDevExt->DpcSpinLock);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartRestoreState
//
NTSTATUS
UartRestoreState(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(Device);
    
    WdfSpinLockAcquire(pDevExt->DpcSpinLock);

    // Restore hardware registers. Note, This function is called
    // with interrupts disconnected and IO gated, so there
    // is no need to synchronize with the interrupt lock. We do,
    // however, use the spinlock to synchronize with the open callback.
    WRITE_FIFO_CONTROL(pDevExt, pDevExt->Controller, pDevExt->FifoControl);
    WRITE_LINE_CONTROL(pDevExt, pDevExt->Controller, pDevExt->LineControl);
    WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, pDevExt->ModemControl);
    WRITE_DIVISOR_LATCH(pDevExt, pDevExt->Controller, pDevExt->DivisorLatch);

    // Now that the HW registers have been restored, this flag 
    // is reset to indicate the saved register state does not 
    // contain new config (yet). If this flag is asserted, 
    // UartRestoreStatePostInterruptsEnabled will know to re-restore
    // the appropriate registers with the latest config.
    pDevExt->NewConfigOutstanding = FALSE;

    // Update modem status
    pDevExt->ModemStatus = (pDevExt->ModemStatus & SERIAL_MSR_EVENTS) |
        READ_MODEM_STATUS(pDevExt, pDevExt->Controller);

    if (pDevExt->DeviceOpened && pDevExt->FIFOBufferBytes == 0)
    {
        // RTS and/or DTR line will be asserted if handshake hardware flow control
        // is being used, allowing bytes to enter the FIFO.  This happens even if
        // RTS and/or DTR were previously restored to 0 because the FIFO
        // was emptied on D0 exit.

       // If the software FIFO is not empty,  RTS and/or DTR will NOT be restored until
        // a read request empties the software FIFO in UartEvtReceive.
        // Automatic flow control MUST be re-enabled here if it's being used.
        UartFlowReceiveAvailable(Device);

        // Reset the FIFOs after creating the file object to start from a clean
        // state.
        if (pDevExt->ResetFifoOnD0Entry)
        {
            WRITE_FIFO_CONTROL(pDevExt, pDevExt->Controller, pDevExt->FifoControl |
                SERIAL_FCR_RCVR_RESET | SERIAL_FCR_TXMT_RESET);

            pDevExt->ResetFifoOnD0Entry = FALSE;
        }
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_INIT,
                    "Controller device state restored.  Device no longer idle");

    WdfSpinLockRelease(pDevExt->DpcSpinLock);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartRestoreStatePostInterruptsEnabled
//
NTSTATUS
UartRestoreStatePostInterruptsEnabled(
   _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt;
    NTSTATUS status = STATUS_SUCCESS;

    FuncEntry(TRACE_FLAG_INIT);

    pDevExt = UartGetDeviceExtension(Device);

    WdfSpinLockAcquire(pDevExt->DpcSpinLock);

    // Mark device active
    pDevExt->DeviceActive = TRUE;

    if (pDevExt->NewConfigOutstanding == TRUE)
    {
        // New config has been applied since the last
        // call to UartRestoreState, so we must re-restore
        // the related registers from the device context.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        WRITE_LINE_CONTROL(pDevExt, pDevExt->Controller, pDevExt->LineControl);
        WRITE_DIVISOR_LATCH(pDevExt, pDevExt->Controller, pDevExt->DivisorLatch);

        // RTS and/or DTR line will be asserted if handshake hardware flow control
        // is being used, allowing bytes to enter the FIFO.  This happens even if
        // RTS and/or DTR were restored to 0 in UartRestoreState because the FIFO
        // was emptied on D0 exit.

        // If the software FIFO is not empty,  RTS and/or DTR will NOT be restored until
        // a read request empties the software FIFO in UartEvtReceive.
        // Automatic flow control MUST be re-enabled here if it's being used.
        if (pDevExt->DeviceOpened && pDevExt->FIFOBufferBytes == 0)
        {
            UartFlowReceiveAvailable(Device);
        }
        
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        pDevExt->NewConfigOutstanding = FALSE;
    }

    WdfSpinLockRelease(pDevExt->DpcSpinLock);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartInitCachedBuffers
//
NTSTATUS UartInitCachedBuffers(
    _In_ WDFDEVICE Device
    )
{
    FuncEntry(TRACE_FLAG_INIT);

    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt;
    WDF_OBJECT_ATTRIBUTES attributes;

    pDevExt = UartGetDeviceExtension(Device);

    // Init buffer caching device extension values
    WDF_OBJECT_ATTRIBUTES_INIT(&attributes);
    attributes.ParentObject = Device;
    status = WdfSpinLockCreate(&attributes, &pDevExt->TransmitBufferSpinLock);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_INIT,
            "%!FUNC! WdfSpinLockCreate() returned error (%!STATUS!)",
            status);
    }
    else
    {
        status = WdfSpinLockCreate(&attributes, &pDevExt->ReceiveBufferSpinLock);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_INIT,
            "%!FUNC! WdfSpinLockCreate() returned error (%!STATUS!)",
            status);
        }
    }

    // Create the interval timeout timer
    if (NT_SUCCESS(status))
    {
        WDF_TIMER_CONFIG timerConfig;

        WDF_TIMER_CONFIG_INIT(&timerConfig, UartIntervalTimeoutTimer);
        timerConfig.AutomaticSerialization = TRUE;

        WDF_OBJECT_ATTRIBUTES_INIT(&attributes);
        attributes.ParentObject = Device;
        attributes.SynchronizationScope = WdfSynchronizationScopeInheritFromParent;

        status = WdfTimerCreate(&timerConfig, &attributes, &(pDevExt->TimeoutTimer));

        pDevExt->TimeoutTimerTimedOut = FALSE;
    }

    SERCX_BUFFER_DESCRIPTOR_INIT(&pDevExt->PIOTransmitBuffer);
    SERCX_BUFFER_DESCRIPTOR_INIT(&pDevExt->PIOReceiveBuffer);

    FuncExit(TRACE_FLAG_INIT);

    return status;
}

//
// Uart16550pc Function: UartInitDMA
//
NTSTATUS
UartInitDMA(
    _In_ WDFDEVICE Device,
    _In_ PUART_CONFIG_DATA PConfig,
    _In_ ULONG DmaResourceCount
    )
{
    // Create DMA objects: enabler, systemconfig, transaction.
    // This sets up System DMA.
    NTSTATUS status = STATUS_SUCCESS;
    PUART_DEVICE_EXTENSION pDevExt;

    pDevExt = UartGetDeviceExtension(Device);

    //
    // Initialize TX DMA
    //

    // Create DMA Enabler
    WDF_DMA_ENABLER_CONFIG dmaEnablerConfig;
    WDF_DMA_ENABLER_CONFIG_INIT(
        &dmaEnablerConfig,
        WdfDmaProfileSystem,
        UART_DMA_MAX_TRANSFER_LENGTH_BYTES
        );

    status = WdfDmaEnablerCreate(
        Device,
        &dmaEnablerConfig,
        WDF_NO_OBJECT_ATTRIBUTES,
        &(pDevExt->TxDmaEnabler));

    if (!NT_SUCCESS(status))
    {
        TraceMessage(TRACE_LEVEL_ERROR,
            TRACE_FLAG_INIT,
            "WdfDmaEnablerCreate() for Transmit returned error (%!STATUS!)",
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Initialize System DMA configuration
        WDF_DMA_SYSTEM_PROFILE_CONFIG dmaSystemProfileConfig;

        PHYSICAL_ADDRESS address;
        address.HighPart = UART_DMA_ADDRESS_HIGH_PART;
        address.LowPart = UART_DMA_ADDRESS_LOW_PART;

        WdfDmaEnablerSetMaximumScatterGatherElements(pDevExt->TxDmaEnabler, UART_DMA_MAX_FRAGMENTS);

        WDF_DMA_SYSTEM_PROFILE_CONFIG_INIT(
            &dmaSystemProfileConfig,
            address,
            Width8Bits,
            PConfig->DmaResourceDescriptor[0]);

        status = WdfDmaEnablerConfigureSystemProfile(
            pDevExt->TxDmaEnabler,
            &dmaSystemProfileConfig,
            WdfDmaDirectionWriteToDevice);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                TRACE_FLAG_INIT,
                "WdfDmaEnablerConfigureSystemProfile() for Transmit returned error (%!STATUS!)",
                status);
        }
    }

    // Create DMA Transaction
    if (NT_SUCCESS(status))
    {
        status = WdfDmaTransactionCreate(
                pDevExt->TxDmaEnabler,
                WDF_NO_OBJECT_ATTRIBUTES,
                &pDevExt->DmaTransmitTransaction);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                TRACE_FLAG_INIT,
                "WdfDmaTransactionCreate() for Transmit returned error (%!STATUS!)",
                status);
        }
    }

    // Save DMA Adapter information
    if (NT_SUCCESS(status))
    {
        pDevExt->writeAdapter = WdfDmaEnablerWdmGetDmaAdapter(
            pDevExt->TxDmaEnabler,
            WdfDmaDirectionWriteToDevice);

        status = (pDevExt->writeAdapter != NULL) ? 
            pDevExt->writeAdapter->DmaOperations->GetDmaAdapterInfo(pDevExt->writeAdapter, &pDevExt->DmaWriteInfo) :
            STATUS_UNSUCCESSFUL;

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                TRACE_FLAG_INIT,
                "Retrieving Transmit DMA Adapter info returned error (%!STATUS!)",
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        pDevExt->DmaTransmitConfigured = TRUE;

        TraceMessage(TRACE_LEVEL_INFORMATION,
            TRACE_FLAG_INIT,
            "DMA transmit configured");
    }

    //
    // Initialize RX DMA
    //

    if (NT_SUCCESS(status) && DmaResourceCount >= 2)
    {
        // Create DMA Enabler
        status = WdfDmaEnablerCreate(
            Device,
            &dmaEnablerConfig,
            WDF_NO_OBJECT_ATTRIBUTES,
            &(pDevExt->RxDmaEnabler));

        if (!NT_SUCCESS(status))
        {
            TraceMessage(TRACE_LEVEL_ERROR,
                TRACE_FLAG_INIT,
                "WdfDmaEnablerCreate() for Receive returned error (%!STATUS!)",
                status);
        }

        // Initialize System DMA Configuration
        if (NT_SUCCESS(status))
        {
            WDF_DMA_SYSTEM_PROFILE_CONFIG dmaSystemProfileConfig;

            // TODO: Is this platform specific?
            PHYSICAL_ADDRESS address;
            RtlZeroMemory(&address, sizeof(PHYSICAL_ADDRESS));
            address.HighPart = UART_DMA_ADDRESS_HIGH_PART;
            address.LowPart = UART_DMA_ADDRESS_LOW_PART;

            WdfDmaEnablerSetMaximumScatterGatherElements(pDevExt->RxDmaEnabler, UART_DMA_MAX_FRAGMENTS);

            WDF_DMA_SYSTEM_PROFILE_CONFIG_INIT(
                &dmaSystemProfileConfig,
                address,
                DMA_WIDTH::Width8Bits,
                PConfig->DmaResourceDescriptor[1]);

            status = WdfDmaEnablerConfigureSystemProfile(
                pDevExt->RxDmaEnabler,
                &dmaSystemProfileConfig,
                WdfDmaDirectionReadFromDevice);

            if (!NT_SUCCESS(status))
            {
                TraceMessage(TRACE_LEVEL_ERROR,
                    TRACE_FLAG_INIT,
                    "WdfDmaEnablerConfigureSystemProfile() for Receive returned error (%!STATUS!)",
                    status);
            }
        }

        // Create DMA Transaction
        if (NT_SUCCESS(status))
        {
            status = WdfDmaTransactionCreate(
                    pDevExt->RxDmaEnabler,
                    WDF_NO_OBJECT_ATTRIBUTES,
                    &pDevExt->DmaReceiveTransaction);

            if (!NT_SUCCESS(status))
            {
                TraceMessage(TRACE_LEVEL_ERROR,
                    TRACE_FLAG_INIT,
                    "WdfDmaTransactionCreate() for Receive returned error (%!STATUS!)",
                    status);
            }
        }

        // Save DMA Adapter information
        if (NT_SUCCESS(status))
        {
            pDevExt->readAdapter = WdfDmaEnablerWdmGetDmaAdapter(
                pDevExt->RxDmaEnabler,
                WdfDmaDirectionReadFromDevice);

            status = (pDevExt->readAdapter != NULL) ? 
                pDevExt->readAdapter->DmaOperations->GetDmaAdapterInfo(pDevExt->readAdapter, &pDevExt->DmaReadInfo) :
                STATUS_UNSUCCESSFUL;

            if (!NT_SUCCESS(status))
            {
                TraceMessage(TRACE_LEVEL_ERROR,
                    TRACE_FLAG_INIT,
                    "Retrieving Receive DMA Adapter info returned error (%!STATUS!)",
                    status);
            }
        }

        if (NT_SUCCESS(status))
        {
            pDevExt->DmaReceiveConfigured = TRUE;

            TraceMessage(TRACE_LEVEL_INFORMATION,
                TRACE_FLAG_INIT,
                "DMA receive configured");
        }
    }

    return status;
}

UCHAR
UartReadPortUChar(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset
    )
{
    UCHAR readChar = READ_PORT_UCHAR(BaseAddress + Offset);
    TraceMessage(TRACE_LEVEL_VERBOSE,
                    TRACE_FLAG_REGUTIL,
                    "UartReadPortUChar(0x%p + 0x%lx) = 0x%x",
                    BaseAddress, Offset, readChar);
    return readChar;
}

VOID
UartWritePortUChar(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset,
    _In_ UCHAR y
    )
{
    WRITE_PORT_UCHAR(BaseAddress + Offset, y);
    TraceMessage(TRACE_LEVEL_VERBOSE,
                    TRACE_FLAG_REGUTIL,
                    "UartWritePortUChar(0x%p + 0x%lx) = 0x%x",
                    BaseAddress, Offset, y);
}

UCHAR
UartReadRegisterUChar(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset
    )
{
    UCHAR readChar = READ_REGISTER_UCHAR(BaseAddress + Offset);
    TraceMessage(TRACE_LEVEL_VERBOSE,
                    TRACE_FLAG_REGUTIL,
                    "UartReadRegisterUChar(0x%p + 0x%lx) = 0x%x",
                    BaseAddress, Offset, readChar);
    return readChar;
}

VOID
UartWriteRegisterUChar (
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset,
    _In_ UCHAR y
    )
{
    WRITE_REGISTER_UCHAR(BaseAddress + Offset, y);
    TraceMessage(TRACE_LEVEL_VERBOSE,
                    TRACE_FLAG_REGUTIL,
                    "UartWriteRegisterUChar(0x%p + 0x%lx) = 0x%x",
                    BaseAddress, Offset, y);
}

