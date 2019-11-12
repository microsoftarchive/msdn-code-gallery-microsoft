/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Transfer.c- Driver for the AMCC S5933 PCI chipset reference kit.

Abstract:

Environment:

    Kernel mode

--*/

#include "AMCC5933.h"

#include "Transfer.tmh"


VOID
AmccPciEvtIoDefault(
    _In_ WDFQUEUE       Queue,
    _In_ WDFREQUEST     Request
    )
/*++

Routine Description:

    Start the IRP on the device.  This driver allows only one I/O to
    be active on the adapter at any one time.  If multiple I/Os are sent
    to the driver, they will be queued and completed as they complete on
    the adapter (one IRP per interrupt).

Arguments:

    Queue - Default queue handle
    Request - Handle to the write request
    Parameters - Contains current stack location information from the IRP

Return Value:

    None

--*/
{
    PAMCC_DEVICE_EXTENSION    devExt;
    REQUEST_CONTEXT         * transfer;
    NTSTATUS                  status;
    size_t                    length;
    WDF_DMA_DIRECTION         direction;
    WDFDMATRANSACTION         dmaTransaction;
    WDF_REQUEST_PARAMETERS    params;

    WDF_REQUEST_PARAMETERS_INIT(&params);

    WdfRequestGetParameters(
        Request,
        &params
        );

    //
    // Get the device extension.
    //
    devExt = AmccPciGetDevExt(WdfIoQueueGetDevice( Queue ));

    //
    // Validate and gather parameters.
    //
    switch (params.Type) {

        case WdfRequestTypeRead:
            length    = params.Parameters.Read.Length;
            direction = WdfDmaDirectionReadFromDevice;
            break;

        case WdfRequestTypeWrite:
            length    = params.Parameters.Write.Length;
            direction = WdfDmaDirectionWriteToDevice;
            break;

        default:
            TraceEvents(TRACE_LEVEL_WARNING, AMCC_TRACE_IO,
                        "Request type not Read or Write\n");
            WdfRequestComplete(Request, STATUS_INVALID_PARAMETER);
            return;
    }

    TraceEvents(TRACE_LEVEL_INFORMATION, AMCC_TRACE_IO,
                "Request %p: %s %d bytes",
                Request, (direction)?"Write":"Read", (ULONG)length);

    //
    // The length must be non-zero.
    //
    if (length == 0) {
        TraceEvents(TRACE_LEVEL_WARNING, AMCC_TRACE_IO,
                    "Zero transfer length input to read/write");
        WdfRequestComplete(Request, STATUS_INVALID_PARAMETER);
        return;
    }

    transfer = GetRequestContext(Request);

    //
    // Create new DmaRequst to conduct this DMA transaction.
    //
    status = WdfDmaTransactionCreate( devExt->DmaEnabler,
                                      WDF_NO_OBJECT_ATTRIBUTES,
                                      &dmaTransaction );

    if(!NT_SUCCESS(status)) {
        TraceEvents(TRACE_LEVEL_ERROR, AMCC_TRACE_IO,
                    "WdfDmaRequestCreate failed: %X", status);
        WdfRequestComplete(Request, status);
        return;
    }

    //
    // Create new DmaTransaction.
    //
    status = WdfDmaTransactionInitializeUsingRequest( dmaTransaction,
                                                      Request,
                                                      AmccPciProgramDma,
                                                      direction );

    if(!NT_SUCCESS(status)) {
        TraceEvents(TRACE_LEVEL_ERROR, AMCC_TRACE_IO,
                    "WdfDmaRequestInitializeWithRequest failed: %X",
                    status);

        WdfObjectDelete(dmaTransaction);

        WdfRequestComplete(Request, status);
        return;
    }

    //
    // Fill transfer context structure
    //
    transfer->Request        = Request;
    transfer->DmaTransaction = dmaTransaction;

    //
    // Save the current Request as the "in-progress" request.
    //
    devExt->CurrentRequest = Request;

    //
    // Execute this dmaTransaction transaction.
    //
    status = WdfDmaTransactionExecute( dmaTransaction, WDF_NO_CONTEXT);
    if (!NT_SUCCESS(status)) {
        TraceEvents(TRACE_LEVEL_ERROR, AMCC_TRACE_IO,
                    "WdfDmaTransactionExecute failed: %X",
                    status);

        WdfObjectDelete(dmaTransaction);
        WdfRequestComplete(Request, status);
        return;
    }

    return;
}


BOOLEAN
AmccPciEvtInterruptIsr(
    _In_ WDFINTERRUPT Interrupt,
    _In_ ULONG        MessageID
    )
/*++

Routine Description:

    This routine assumes that only a single I/O can be completed at a
    time on the hardware (i.e. at most one I/O completed per interrupt).

Arguments:

    Interupt - Address of the framework interrupt object
    MessageID -

Return Value:

    TRUE - Interrupt belongs to this device.

--*/
{
    PAMCC_DEVICE_EXTENSION   devExt = NULL;
    WDFDEVICE                hDevice;

    union {
        ULONG      ulong;
        INTCSR_REG bits;
    } intcsr;

    union {
        ULONG      ulong;
        MCSR_REG   bits;
    } mcsr;

    UNREFERENCED_PARAMETER( MessageID );

    hDevice = WdfInterruptGetDevice(Interrupt);
    devExt = AmccPciGetDevExt(hDevice);

    //
    // Read interrupt control/status register and see if an interrupt is pending.
    // If not, return FALSE immediately.
    //
    intcsr.ulong = READ_PORT_ULONG((PULONG) &devExt->Regs->INTCSR);

    if (!intcsr.bits.InterruptAsserted) {
        return FALSE;
    }

    //
    // Disable bus-mastering
    //
    mcsr.ulong = READ_PORT_ULONG((PULONG) &devExt->Regs->MCSR);

    mcsr.bits.WriteTransferEnable = FALSE;
    mcsr.bits.ReadTransferEnable  = FALSE;

    WRITE_PORT_ULONG((PULONG) &devExt->Regs->MCSR, mcsr.ulong );

    //
    // This will take effect when INTCSR is rewritten later.
    //
    intcsr.bits.IntOnWriteTransferComplete = FALSE;
    intcsr.bits.IntOnReadTransferComplete  = FALSE;

    //
    // Process pending interrupts. We're expecting an interrupt due
    // to a transfer count going to zero, but we might be getting a
    // master or target abort instead.
    //
    while (intcsr.bits.InterruptAsserted) {

        //
        // Merge new interrupts with old
        //
        _InterlockedOr((PLONG) &devExt->Intcsr, (LONG) intcsr.ulong );

        //
        // Interrupt flags on the S5933 are cleared by writing a "1" bit
        // to them, so clear all the interrupts just examined.
        //
        WRITE_PORT_ULONG((PULONG) &devExt->Regs->INTCSR, intcsr.ulong);

        //
        // Check for additional interrupts
        //
        intcsr.ulong = READ_PORT_ULONG((PULONG) &devExt->Regs->INTCSR);
    }

    //
    // Check if there is a current Request.  If not, then this interrupt cannot
    // do anything.  This driver design requires an I/O to be pending in order
    // to queue the DPC.  If there is no I/O current, then there is no need
    // to have a DPC queued.  This driver also assumes one I/O per interrupt.
    //
    // IMPORTANT: Before returning TRUE, the interrupt must have been cleared
    // on the device or the system will hang trying to service this level
    // sensitive interrupt.
    //
    if (!devExt->CurrentRequest) {
        TraceEvents(TRACE_LEVEL_WARNING, AMCC_TRACE_IO,
                    "Hardware generated interrupt with no request pending");
        return TRUE;
    }

    //
    // Request the DPC to complete the transfer.
    //
    WdfInterruptQueueDpcForIsr( Interrupt );

    //
    // Indicate that this adapter was interrupting.
    //
    return TRUE;
}


VOID
AmccPciEvtInterruptDpc(
    _In_ WDFINTERRUPT WdfInterrupt,
    _In_ WDFOBJECT    WdfDevice
    )
/*++

Routine Description:

    DPC callback for ISR.

Arguments:

    WdfInterrupt - Handle to the framework interrupt object

    WdfDevice - Associated device object.

Return Value:

--*/
{
    PAMCC_DEVICE_EXTENSION   devExt;
    WDFREQUEST               request;
    REQUEST_CONTEXT        * transfer;
    NTSTATUS                 status;
    size_t                   transferred;
    BOOLEAN                  transactionComplete;

    UNREFERENCED_PARAMETER( WdfInterrupt );

    devExt = AmccPciGetDevExt(WdfDevice);

    //
    // Retreive request and transfer.
    //
    request  = devExt->CurrentRequest;
    transfer = GetRequestContext(request);

    //
    // Check to see if the request is cancelled by the system. While
    // we are DMAing a large buffer into multiple transaction,
    // there is good possibilty for the request to get cancelled because
    // the originator of the request exited or cancelled the I/O explicitly.
    //
    if(WdfRequestIsCanceled(request)) {
        TraceEvents(TRACE_LEVEL_ERROR, AMCC_TRACE_IO,
                                    "Aborted DMA transaction 0x%p",  request);
        WdfObjectDelete( transfer->DmaTransaction );
        devExt->CurrentRequest = NULL;
        WdfRequestComplete(request, STATUS_CANCELLED);
        return;
    }

    //
    // The boolean transactionComplete indicates whether the transaction has
    // exited the transfer state, e.g. no further transfers are scheduled.
    //
    // If transactionComplete == FALSE, then the next DMA transfer has been
    // scheduled, e.g. the next interrrupt will drive the ISR again.
    //
    // If transactionComplete == TRUE, then status indicates the reason;
    // SUCCESS is the nomative case, while non-SUCCESS indicates the
    // DMA transaction failed for "status" reason.
    //
    transactionComplete = WdfDmaTransactionDmaCompleted( transfer->DmaTransaction,
                                                     &status );

    if (transactionComplete) {

        ASSERT(status != STATUS_MORE_PROCESSING_REQUIRED);

        //
        // No more data: request is complete
        //
        TraceEvents(TRACE_LEVEL_INFORMATION, AMCC_TRACE_IO,
                    "Request %p completed: status %X",
                    request, status);

        //
        // Get the final bytes transferred count.
        //
        transferred =
            WdfDmaTransactionGetBytesTransferred( transfer->DmaTransaction );

        TraceEvents(TRACE_LEVEL_INFORMATION, AMCC_TRACE_IO,
                    "Bytes transfered %d", (int) transferred );

        //
        // Delete this DmaTransaction transaction.
        //
        WdfObjectDelete( transfer->DmaTransaction );

        //
        // Clean-up for this request.
        //
        devExt->CurrentRequest = NULL;

        //
        // Complete this IO request.
        //
        WdfRequestCompleteWithInformation( request,
                                           status,
                                           (NT_SUCCESS(status)) ?
                                           transferred : 0 );
    }

}


BOOLEAN
AmccPciProgramDma(
    _In_ WDFDMATRANSACTION       Transaction,
    _In_ WDFDEVICE               Device,
    _In_ PVOID                   Context,
    _In_ WDF_DMA_DIRECTION       Direction,
    _In_ PSCATTER_GATHER_LIST    SgList
    )
/*++

Routine Description:

Arguments:

Return Value:

--*/
{
    AMCC_DEVICE_EXTENSION  * devExt;
    ULONG                    address;
    ULONG                    length;

    union {
        ULONG      ulong;
        MCSR_REG   bits;
    } mcsr;

    union {
        ULONG      ulong;
        INTCSR_REG bits;
    } intcsr;


    UNREFERENCED_PARAMETER( Transaction );
    UNREFERENCED_PARAMETER( Context );

    //
    // Reestablish working parameters.
    //
    devExt = AmccPciGetDevExt(Device);

    //
    // The S5933 used only 32-bit packet mode DMA operations.
    //
    ASSERT(SgList->NumberOfElements == 1);
    ASSERT(SgList->Elements[0].Address.HighPart == 0);

    //
    // Only the first Scatter/Gather element is relevant for packet mode.
    // S5933 only does 32-bit DMA transfer operations: only low part of
    // physical address is usable.
    //
    address = SgList->Elements[0].Address.LowPart;
    length  = SgList->Elements[0].Length;

    TraceEvents(TRACE_LEVEL_INFORMATION, AMCC_TRACE_IO,
                "Address 0x%08X, Length %d", address, length);

    //
    // Read the Master Control/Status Register (MCSR) and
    // the Interrupt Control/Status Register (INTCSR).
    //
    mcsr.ulong   = READ_PORT_ULONG((PULONG) &(devExt->Regs->MCSR));
    intcsr.ulong = READ_PORT_ULONG((PULONG) &devExt->Regs->INTCSR);

    //
    // Setup read or write transfer registers.
    //
    // NOTE: The S5933 calls a transfer from memory to the device a "read".
    //
    if (Direction == WdfDmaDirectionWriteToDevice) {

        mcsr.bits.ReadFifoMgmtScheme = TRUE;
        mcsr.bits.ReadTransferEnable = TRUE;

        intcsr.bits.IntOnReadTransferComplete = TRUE;

        WRITE_PORT_ULONG((PULONG) &devExt->Regs->MRTC, length);
        WRITE_PORT_ULONG((PULONG) &devExt->Regs->MRAR, address);

    } else {

        mcsr.bits.WriteFifoMgmtScheme = TRUE;
        mcsr.bits.WriteTransferEnable = TRUE;

        intcsr.bits.IntOnWriteTransferComplete = TRUE;

        WRITE_PORT_ULONG((PULONG) &devExt->Regs->MWTC, length);
        WRITE_PORT_ULONG((PULONG) &devExt->Regs->MWAR, address);
    }

    //
    // Write modified INTCSR to enable the appropriate interrupt and
    // the MCSR to actually start the transfer.
    //
    WRITE_PORT_ULONG((PULONG) &devExt->Regs->INTCSR, intcsr.ulong);
    WRITE_PORT_ULONG((PULONG) &devExt->Regs->MCSR,   mcsr.ulong);

    return TRUE;
}

