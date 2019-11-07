/*--

Copyright (C) Microsoft Corporation. All rights reserved.

Module Name:

    pnppower.c

Abstract:

    Functions to handle PnP and Power IRPs.

Environment:

    kernel mode only

Notes:
    optical devices do not need to issue SPIN UP when power up. 
    The device itself should SPIN UP to process commands.


Revision History:

--*/


#include "ntddk.h"
#include "wdfcore.h"

#include "cdrom.h"
#include "ioctl.h"
#include "scratch.h"
#include "mmc.h"

#ifdef DEBUG_USE_WPP
#include "pnppower.tmh"
#endif


NTSTATUS
DeviceScratchSyncCache(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension
    );

NTSTATUS
DeviceScratchPreventMediaRemoval(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension,
    _In_ BOOLEAN                 Prevent
    );

NTSTATUS
RequestIssueShutdownFlush(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension,
    _In_ PIRP                    Irp
    );

#pragma warning(push)
#pragma warning(disable:4152) // nonstandard extension, function/data pointer conversion in expression


NTSTATUS 
RequestProcessShutdownFlush(
    WDFDEVICE  Device,
    PIRP       Irp
    )
/*++

Routine Description:

    process IRP: IRP_MJ_SHUTDOWN, IRP_MJ_FLUSH_BUFFERS

Arguments:

    Device - device object
    Irp - the irp

Return Value:

    NTSTATUS

--*/
{
    NTSTATUS                status = STATUS_SUCCESS;
    PIO_STACK_LOCATION      currentStack = NULL;
    PCDROM_DEVICE_EXTENSION deviceExtension = DeviceGetExtension(Device);

    //add trace info

    // acquire the shutdown/flush lock
    WdfWaitLockAcquire(deviceExtension->ShutdownFlushWaitLock, NULL);

    currentStack = IoGetCurrentIrpStackLocation(Irp);

    // finish all current requests
    WdfIoQueueStopSynchronously(deviceExtension->SerialIOQueue);

    // sync cache
    if (NT_SUCCESS(status))
    {
        // safe to use scratch srb to send the request.
        status = DeviceScratchSyncCache(deviceExtension);
    }

    // For SHUTDOWN, allow media removal.
    if (NT_SUCCESS(status))
    {
        if (currentStack->MajorFunction == IRP_MJ_SHUTDOWN)
        {
            // safe to use scratch srb to send the request.
            status = DeviceScratchPreventMediaRemoval(deviceExtension, FALSE);
        }
    }

    // Use original IRP, send SRB_FUNCTION_SHUTDOWN or SRB_FUNCTION_FLUSH (no retry)
    if (NT_SUCCESS(status))
    {
        status = RequestIssueShutdownFlush(deviceExtension, Irp);
    }

    // restart queue to allow processing further requests.
    WdfIoQueueStart(deviceExtension->SerialIOQueue);

    // release the shutdown/flush lock
    WdfWaitLockRelease(deviceExtension->ShutdownFlushWaitLock);

    // 6. complete the irp
    Irp->IoStatus.Status = status;
    IoCompleteRequest(Irp, 0);

    return status;
}

// use scratch SRB to issue SYNC CACHE command.
NTSTATUS
DeviceScratchSyncCache(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension
    )
/*++

Routine Description:

    use scratch buffer to send SYNC CACHE command

Arguments:

    DeviceExtension - device context

Return Value:

    NTSTATUS

--*/
{
    NTSTATUS    status = STATUS_SUCCESS;
    ULONG       transferSize = 0;
    CDB         cdb;

    ScratchBuffer_BeginUse(DeviceExtension);

    RtlZeroMemory(&cdb, sizeof(CDB));
    // Set up the CDB
    cdb.SYNCHRONIZE_CACHE10.OperationCode = SCSIOP_SYNCHRONIZE_CACHE;
    //srb->QueueTag = SP_UNTAGGED;
    //srb->QueueAction = SRB_SIMPLE_TAG_REQUEST;

    status = ScratchBuffer_ExecuteCdbEx(DeviceExtension, NULL, transferSize, FALSE, &cdb, 10, TimeOutValueGetCapValue(DeviceExtension->TimeOutValue, 4));

    ScratchBuffer_EndUse(DeviceExtension);

    return status;
}

NTSTATUS
DeviceScratchPreventMediaRemoval(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension,
    _In_ BOOLEAN                 Prevent
    )
/*++

Routine Description:

    use scratch SRB to issue ALLOW/PREVENT MEDIA REMOVAL command.

Arguments:

    DeviceExtension - device context
    Prevent - TRUE (prevent media removal); FALSE (allow media removal)

Return Value:

    NTSTATUS

--*/
{
    NTSTATUS    status = STATUS_SUCCESS;
    ULONG       transferSize = 0;
    CDB         cdb;

    ScratchBuffer_BeginUse(DeviceExtension);

    RtlZeroMemory(&cdb, sizeof(CDB));
    // Set up the CDB
    cdb.MEDIA_REMOVAL.OperationCode = SCSIOP_MEDIUM_REMOVAL;
    cdb.MEDIA_REMOVAL.Prevent = Prevent;
    //srb->QueueTag = SP_UNTAGGED;
    //srb->QueueAction = SRB_SIMPLE_TAG_REQUEST;

    status = ScratchBuffer_ExecuteCdb(DeviceExtension, NULL, transferSize, FALSE, &cdb, 6);

    ScratchBuffer_EndUse(DeviceExtension);

    return status;
}

NTSTATUS
RequestIssueShutdownFlush(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension,
    _In_ PIRP                    Irp
    )
/*++

Routine Description:

    issue SRB function Flush/Shutdown command.

Arguments:

    DeviceExtension - device context
    Irp - the irp

Return Value:

    NTSTATUS

--*/
{
    NTSTATUS            status = STATUS_SUCCESS;
    PSCSI_REQUEST_BLOCK srb = DeviceExtension->ScratchContext.ScratchSrb;
    PIO_STACK_LOCATION  currentStack = NULL;

    ULONG       transferSize = 0;
    BOOLEAN     shouldRetry = TRUE;
    ULONG       timesAlreadyRetried = 0;
    LONGLONG    retryIn100nsUnits = 0;


    currentStack = IoGetCurrentIrpStackLocation(Irp);


    ScratchBuffer_BeginUse(DeviceExtension);

    // no retry needed.
    {
        ScratchBuffer_SetupSrb(DeviceExtension, NULL, transferSize, FALSE);
    
        // Set up the SRB/CDB
        srb->QueueTag = SP_UNTAGGED;
        srb->QueueAction = SRB_SIMPLE_TAG_REQUEST;
        srb->TimeOutValue = TimeOutValueGetCapValue(DeviceExtension->TimeOutValue, 4);
        srb->CdbLength = 0;

        if (currentStack->MajorFunction == IRP_MJ_SHUTDOWN) 
        {
            srb->Function = SRB_FUNCTION_SHUTDOWN;
        } 
        else 
        {
            srb->Function = SRB_FUNCTION_FLUSH;
        }

        ScratchBuffer_SendSrb(DeviceExtension, TRUE, NULL);
    
        shouldRetry = RequestSenseInfoInterpretForScratchBuffer(DeviceExtension,
                                                                timesAlreadyRetried,
                                                                &status,
                                                                &retryIn100nsUnits);
        UNREFERENCED_PARAMETER(shouldRetry); //defensive coding, avoid PREFAST warning.
        UNREFERENCED_PARAMETER(status); //defensive coding, avoid PREFAST warning.

        // retrieve the real status from the request.
        status = WdfRequestGetStatus(DeviceExtension->ScratchContext.ScratchRequest);
    }

    ScratchBuffer_EndUse(DeviceExtension);


    return status;
}

_IRQL_requires_max_(APC_LEVEL)
NTSTATUS
PowerContextReuseRequest(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension
    )
/*++

Routine Description:

    reset fields for the request.

Arguments:

    DeviceExtension - device context

Return Value:

    NTSTATUS

--*/
{
    NTSTATUS                 status = STATUS_SUCCESS;
    WDF_REQUEST_REUSE_PARAMS reuseParams;
    PIRP                     irp = NULL;

    RtlZeroMemory(&(DeviceExtension->PowerContext.SenseData), sizeof(DeviceExtension->PowerContext.SenseData));
    RtlZeroMemory(&(DeviceExtension->PowerContext.Srb), sizeof(DeviceExtension->PowerContext.Srb));

    irp = WdfRequestWdmGetIrp(DeviceExtension->PowerContext.PowerRequest);

    // Re-use the previously created PowerRequest object and format it
    WDF_REQUEST_REUSE_PARAMS_INIT(&reuseParams, WDF_REQUEST_REUSE_NO_FLAGS, STATUS_NOT_SUPPORTED);
    status = WdfRequestReuse(DeviceExtension->PowerContext.PowerRequest, &reuseParams);
    if (NT_SUCCESS(status))
    {
        // This request was preformated during initialization so this call should never fail.
        status = WdfIoTargetFormatRequestForInternalIoctlOthers(DeviceExtension->IoTarget, 
                                                                DeviceExtension->PowerContext.PowerRequest,
                                                                IOCTL_SCSI_EXECUTE_IN,
                                                                NULL, NULL,
                                                                NULL, NULL,
                                                                NULL, NULL);

        if (!NT_SUCCESS(status))
        {
            TracePrint((TRACE_LEVEL_ERROR, TRACE_FLAG_GENERAL,  
                       "PowerContextReuseRequest: WdfIoTargetFormatRequestForInternalIoctlOthers failed, %!STATUS!\n",
                       status));
        }
    }

    // Do some basic initialization of the PowerRequest, the rest will be done by the caller
    // of this function
    if (NT_SUCCESS(status))
    {
        PIO_STACK_LOCATION  nextStack = NULL;

        nextStack = IoGetNextIrpStackLocation(irp);

        nextStack->MajorFunction = IRP_MJ_SCSI;
        nextStack->Parameters.Scsi.Srb = &(DeviceExtension->PowerContext.Srb);

        DeviceExtension->PowerContext.Srb.Length = sizeof(SCSI_REQUEST_BLOCK);
        DeviceExtension->PowerContext.Srb.OriginalRequest = irp;

        DeviceExtension->PowerContext.Srb.SenseInfoBuffer = &(DeviceExtension->PowerContext.SenseData);
        DeviceExtension->PowerContext.Srb.SenseInfoBufferLength = SENSE_BUFFER_SIZE;
    }

    return status;
}

_IRQL_requires_max_(APC_LEVEL)
NTSTATUS
PowerContextBeginUse(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension
    )
/*++

Routine Description:

    initialize fields in power context

Arguments:

    DeviceExtension - device context

Return Value:

    NTSTATUS

--*/
{
    NTSTATUS status = STATUS_SUCCESS;

    NT_ASSERT(!DeviceExtension->PowerContext.InUse);

    DeviceExtension->PowerContext.InUse = TRUE;
    DeviceExtension->PowerContext.RetryCount = MAXIMUM_RETRIES;
    DeviceExtension->PowerContext.RetryIntervalIn100ns = 0;
    
    KeQueryTickCount(&DeviceExtension->PowerContext.StartTime);
    
    RtlZeroMemory(&(DeviceExtension->PowerContext.Options), sizeof(DeviceExtension->PowerContext.Options));
    RtlZeroMemory(&(DeviceExtension->PowerContext.PowerChangeState), sizeof(DeviceExtension->PowerContext.PowerChangeState));

    status = PowerContextReuseRequest(DeviceExtension);

    RequestClearSendTime(DeviceExtension->PowerContext.PowerRequest);

    return status;
}

_IRQL_requires_max_(APC_LEVEL)
NTSTATUS
PowerContextEndUse(
    _In_ PCDROM_DEVICE_EXTENSION DeviceExtension
    )
/*++

Routine Description:

    inidate that power context using is finished.

Arguments:

    DeviceExtension - device context

Return Value:

    NTSTATUS

--*/
{
    NT_ASSERT(DeviceExtension->PowerContext.InUse);

    DeviceExtension->PowerContext.InUse = FALSE;

    KeQueryTickCount(&DeviceExtension->PowerContext.CompleteTime);
    
    return STATUS_SUCCESS;
}

#pragma warning(pop) // un-sets any local warning changes

