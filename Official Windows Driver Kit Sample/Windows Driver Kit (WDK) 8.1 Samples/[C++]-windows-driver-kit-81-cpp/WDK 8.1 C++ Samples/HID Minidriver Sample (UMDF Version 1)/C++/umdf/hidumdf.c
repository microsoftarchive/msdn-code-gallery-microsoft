/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    HidUmdf.C

Abstract:

    Hid miniport to be used as an upper layer for supporting
    WDF based driver for HID devices.

    IOCTLs supported by hid minidriver are defined in hidport.h and hidclass.h.
    Hidport.h contains only those IOCTLs that are supported by minidriver interface.
    Hidclass.h contains IOCTLs that are supported by upper edge of hidclass, however
    some of these IOCTLs are redirected to hid minidriver as well.


Author:

Environment:

    Kernel mode only
--*/

#include <wdm.h>

#pragma warning(disable:4201)  // suppress nameless struct/union warning
#pragma warning(disable:4214)  // suppress bit field types other than int warning
#include <hidport.h>
#pragma warning(default:4201)  // suppress nameless struct/union warning
#pragma warning(default:4214)  // suppress bit field types other than int warning

#define HIDUMDF_POOL_TAG (ULONG) 'UdiH'
#define HIDUMDF_HARDWARE_IDS    L"\0\0"
#define HIDUMDF_HARDWARE_IDS_LENGTH sizeof (HIDUMDF_HARDWARE_IDS)

#define GET_NEXT_DEVICE_OBJECT(DO) \
    (((PHID_DEVICE_EXTENSION)(DO)->DeviceExtension)->NextDeviceObject)

#define GET_MINIDRIVER_DEVICE_EXTENSION(DO) \
    ((PHIDUMDF_DEVICE_EXTENSION) (((PHID_DEVICE_EXTENSION)(DO)->DeviceExtension)->MiniDeviceExtension))


//
// This type of function declaration is for Prefast for drivers. 
// Because this declaration specifies the function type, PREfast for Drivers
// does not need to infer the type or to report an inference. The declaration
// also prevents PREfast for Drivers from misinterpreting the function type 
// and applying inappropriate rules to the function. For example, PREfast for
// Drivers would not apply rules for completion routines to functions of type
// DRIVER_CANCEL. The preferred way to avoid Warning 28101 is to declare the
// function type explicitly. In the following example, the DriverEntry function
// is declared to be of type DRIVER_INITIALIZE.
//
DRIVER_INITIALIZE   DriverEntry;
DRIVER_ADD_DEVICE   HidUmdfAddDevice;
DRIVER_UNLOAD       HidUmdfUnload;
IO_COMPLETION_ROUTINE UserIoctlCompletion;
IO_COMPLETION_ROUTINE SyncIrpCompletion;

_Dispatch_type_(IRP_MJ_PNP)
DRIVER_DISPATCH     HidUmdfPnp;

_Dispatch_type_(IRP_MJ_CREATE)
_Dispatch_type_(IRP_MJ_CREATE_NAMED_PIPE)
_Dispatch_type_(IRP_MJ_CLOSE)
_Dispatch_type_(IRP_MJ_READ)
_Dispatch_type_(IRP_MJ_WRITE)
_Dispatch_type_(IRP_MJ_QUERY_INFORMATION)
_Dispatch_type_(IRP_MJ_SET_INFORMATION)
_Dispatch_type_(IRP_MJ_QUERY_EA)
_Dispatch_type_(IRP_MJ_SET_EA)
_Dispatch_type_(IRP_MJ_FLUSH_BUFFERS)
_Dispatch_type_(IRP_MJ_QUERY_VOLUME_INFORMATION)
_Dispatch_type_(IRP_MJ_SET_VOLUME_INFORMATION)
_Dispatch_type_(IRP_MJ_DIRECTORY_CONTROL)
_Dispatch_type_(IRP_MJ_FILE_SYSTEM_CONTROL)
_Dispatch_type_(IRP_MJ_DEVICE_CONTROL)
_Dispatch_type_(IRP_MJ_INTERNAL_DEVICE_CONTROL)
_Dispatch_type_(IRP_MJ_SHUTDOWN)
_Dispatch_type_(IRP_MJ_LOCK_CONTROL)
_Dispatch_type_(IRP_MJ_CLEANUP)
_Dispatch_type_(IRP_MJ_CREATE_MAILSLOT)
_Dispatch_type_(IRP_MJ_QUERY_SECURITY)
_Dispatch_type_(IRP_MJ_SET_SECURITY)
_Dispatch_type_(IRP_MJ_POWER)
_Dispatch_type_(IRP_MJ_SYSTEM_CONTROL)
_Dispatch_type_(IRP_MJ_DEVICE_CHANGE)
_Dispatch_type_(IRP_MJ_QUERY_QUOTA)
_Dispatch_type_(IRP_MJ_SET_QUOTA)
_Dispatch_type_(IRP_MJ_PNP)
DRIVER_DISPATCH HidUmdfPassThrough;

_Dispatch_type_(IRP_MJ_POWER)
DRIVER_DISPATCH HidUmdfPowerPassThrough;

_Dispatch_type_(IRP_MJ_INTERNAL_DEVICE_CONTROL)
DRIVER_DISPATCH HidUmdfInternalIoctl;

_Dispatch_type_(IRP_MJ_CREATE)
_Dispatch_type_(IRP_MJ_CLEANUP)
_Dispatch_type_(IRP_MJ_CLOSE)
DRIVER_DISPATCH HidUmdfCreateCleanupClose;

IO_WORKITEM_ROUTINE_EX IoctlWorkItemEx;

NTSTATUS
HidUmdfInternalIoctlWorker(
    _In_ PDEVICE_OBJECT DeviceObject,
    _Inout_ PIRP Irp
    );

PCHAR
DbgHidInternalIoctlString(
    _In_ ULONG        IoControlCode
    );

NTSTATUS
UpdateBufferLocationAndIoctl(
    _Inout_ PIRP Irp,
    _Out_ PULONG UpdatedIoctl
    );

NTSTATUS
HandleQueryId(
    _In_ PDEVICE_OBJECT Device,
    _Inout_ PIRP Irp
    );

NTSTATUS
SendIrpSynchronously(
    _In_ PDEVICE_OBJECT Device,
    _In_ PIRP Irp
    );

#ifdef ALLOC_PRAGMA
#pragma alloc_text( INIT, DriverEntry )
#pragma alloc_text( PAGE, HidUmdfAddDevice)
#pragma alloc_text( PAGE, HidUmdfUnload)
#pragma alloc_text( PAGE, HidUmdfPnp)
#pragma alloc_text( PAGE, HandleQueryId)
#pragma alloc_text( PAGE, IoctlWorkItemEx)
#endif

//
// A dummy buffer the size of MAX(UCHAR) (since ReportId is of type UCHAR)
// for use with certain IOCTLs. See comments in IOCTL dispatch routine for 
// more info on the use of this field. 
//
UCHAR G_ScratchBuffer[MAXUCHAR] = {0};

NTSTATUS
DriverEntry (
    _In_ PDRIVER_OBJECT  DriverObject,
    _In_ PUNICODE_STRING RegistryPath
    )
/*++

Routine Description:

    Installable driver initialization entry point.
    This entry point is called directly by the I/O system.

Arguments:

    DriverObject - pointer to the driver object

    RegistryPath - pointer to a unicode string representing the path,
                   to driver-specific key in the registry.

Return Value:

    STATUS_SUCCESS if successful,
    STATUS_UNSUCCESSFUL otherwise.

--*/
{
    HID_MINIDRIVER_REGISTRATION hidMinidriverRegistration;
    NTSTATUS status;
    ULONG i;

    //
    // Initialize the dispatch table to pass through all the IRPs.
    //
    for (i = 0; i <= IRP_MJ_MAXIMUM_FUNCTION; i++) {
        DriverObject->MajorFunction[i] = HidUmdfPassThrough;
    }

    DriverObject->MajorFunction[IRP_MJ_CREATE]                  =
    DriverObject->MajorFunction[IRP_MJ_CLEANUP]                 = 
    DriverObject->MajorFunction[IRP_MJ_CLOSE]                   = HidUmdfCreateCleanupClose;
    DriverObject->MajorFunction[IRP_MJ_INTERNAL_DEVICE_CONTROL] = HidUmdfInternalIoctl;
    DriverObject->MajorFunction[IRP_MJ_PNP] = HidUmdfPnp;

    //
    // Special case power irps so that we call PoCallDriver instead of IoCallDriver
    // when sending the IRP down the stack.
    //
    DriverObject->MajorFunction[IRP_MJ_POWER] = HidUmdfPowerPassThrough;

    DriverObject->DriverExtension->AddDevice = HidUmdfAddDevice;
    DriverObject->DriverUnload = HidUmdfUnload;

    RtlZeroMemory(&hidMinidriverRegistration,
                  sizeof(hidMinidriverRegistration));

    //
    // Revision must be set to HID_REVISION by the minidriver
    //
    hidMinidriverRegistration.Revision            = HID_REVISION;
    hidMinidriverRegistration.DriverObject        = DriverObject;
    hidMinidriverRegistration.RegistryPath        = RegistryPath;

    //
    // if "DevicesArePolled" is False then the hidclass driver does not do
    // polling and instead reuses a few Irps (ping-pong) if the device has
    // an Input item. Otherwise, it will do polling at regular interval. USB
    // HID devices do not need polling by the HID classs driver. Some leagcy
    // devices may need polling.
    //
    hidMinidriverRegistration.DevicesArePolled = FALSE;

    //
    // Register with hidclass
    //
    status = HidRegisterMinidriver(&hidMinidriverRegistration);
    if (!NT_SUCCESS(status) ){
        KdPrint(("HidRegisterMinidriver FAILED, returnCode=%x\n", status));
    }

    return status;
}


NTSTATUS
HidUmdfAddDevice(
    _In_ PDRIVER_OBJECT DriverObject,
    _In_ PDEVICE_OBJECT FunctionalDeviceObject
    )
/*++

Routine Description:

    HidClass Driver calls our AddDevice routine after creating an FDO for us.
    We do not need to create a device object or attach it to the PDO.
    Hidclass driver will do it for us.

Arguments:

    DriverObject - pointer to the driver object.

    FunctionalDeviceObject -  pointer to the FDO created by the
                            Hidclass driver for us.

Return Value:

    NT status code.

--*/
{
    PAGED_CODE();
    UNREFERENCED_PARAMETER(DriverObject);

    
    FunctionalDeviceObject->Flags &= ~DO_DEVICE_INITIALIZING;

    return STATUS_SUCCESS;
}

NTSTATUS
SendIrpSynchronously(
    _In_ PDEVICE_OBJECT Device,
    _In_ PIRP Irp
    )
{
    NTSTATUS status;
    KEVENT completionEvent;

    KeInitializeEvent(&completionEvent, NotificationEvent, FALSE);
    IoSetCompletionRoutine(Irp,
        SyncIrpCompletion,
        (PVOID) &completionEvent,
        TRUE,
        TRUE,
        TRUE);

    status = IoCallDriver(Device, Irp);
    if (status == STATUS_PENDING) {
        KeWaitForSingleObject(&completionEvent,
                              Executive,
                              KernelMode,
                              FALSE,
                              NULL);
        status =Irp->IoStatus.Status;
    }

    return status;
}

VOID
HidUmdfUnload(
    _In_ PDRIVER_OBJECT DriverObject
    )
/*++

Routine Description:

    Free all the allocated resources, etc.

Arguments:

    DriverObject - pointer to a driver object.

Return Value:

    VOID.

--*/
{
    UNREFERENCED_PARAMETER(DriverObject);

    PAGED_CODE ();

    return;
}

NTSTATUS
HidUmdfInternalIoctl(
    _In_ PDEVICE_OBJECT DeviceObject,
    _Inout_ PIRP Irp
    )
{
    PIO_WORKITEM workitem;
    NTSTATUS status;

    //
    // If Irql is not dispatch, handle it in the current thread.
    // otherwise queue a workitem because UMDF cannot handle io request at 
    // dispatch level. HID minidriver receives IOCTLS normally at
    // passive level but it may receive ping pong irp at dispatch level in some
    // error conditions.  
    //
    if (KeGetCurrentIrql() < DISPATCH_LEVEL) {
        return HidUmdfInternalIoctlWorker(DeviceObject, Irp);        
    }

    //
    // allocate and queue workitem
    //
    workitem = IoAllocateWorkItem(DeviceObject);
    if (workitem == NULL) {
        status = STATUS_INSUFFICIENT_RESOURCES;
        KdPrint(("HidUmdf: Failed to allocate workitem to process IOCTL "
            "0x%p devobj 0x%p sent at dispatch level. status 0x%x\n", 
            Irp, DeviceObject, status));
        Irp->IoStatus.Status = status;
        IoCompleteRequest(Irp, IO_NO_INCREMENT);
        return status;
    }

    //
    // since we are going to return STATUS_PENDING, mark the irp pending.
    //
    IoMarkIrpPending(Irp);

    //
    // Queue the workitem. The workitem will be freed by the worker function.
    //
    IoQueueWorkItemEx(workitem, IoctlWorkItemEx, DelayedWorkQueue, Irp);

    return STATUS_PENDING;
}

VOID
IoctlWorkItemEx(
    _In_ PVOID  IoObject,
    _In_opt_ PVOID  Context,
    _In_ PIO_WORKITEM  IoWorkItem 
    )
{
    PDEVICE_OBJECT device = (PDEVICE_OBJECT) IoObject;
    PIRP irp = (PIRP) Context;
    NTSTATUS status;

    PAGED_CODE();

    //
    // Process the IOCTL. 
    //
    if (irp != NULL) {
        status = HidUmdfInternalIoctlWorker(device, irp);

        if (!NT_SUCCESS(status)) {
            //
            // The IRP has either been sent down the stack or been completed by the 
            // HidUmdfInternalIoctlWorker function. Also, we already returned pending 
            // status from original thread so ignore status here.
            //
        }
    }

    //
    // Free the workitem
    //
    IoFreeWorkItem(IoWorkItem);    
}

NTSTATUS
HidUmdfInternalIoctlWorker(
    _In_ PDEVICE_OBJECT DeviceObject,
    _Inout_ PIRP Irp
    )
/*++

Routine Description:

    IOCTL handler.


Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

Return Value:

      NT status code

--*/
{
    PIO_STACK_LOCATION currStack, nextStack;
    ULONG ioctlCode, newIoctlCode;
    BOOLEAN setCompletionRoutine = FALSE;
    NTSTATUS status = STATUS_SUCCESS;
    BOOLEAN modeChanged = FALSE;
    PULONG temp = NULL;
    
    currStack = IoGetCurrentIrpStackLocation(Irp);

    //
    // Copy current stack to next instead of skipping. We do this to preserve 
    // current stack information provided by hidclass driver to the minidriver
    //
    IoCopyCurrentIrpStackLocationToNext(Irp);

    //
    // HIDclass uses INTERNAL_IOCTL but since UMDF doesn't yet have support for
    // internal IOCTLS we change the IOCTL type to DEVICE_CONTROL for next stack
    // so that UMDF stack can handle it as normal IOCTL.
    // Note that user mode apps cannot open a handle to minidriver since 
    // HIDClass doesn't allow that (it own's minidriver's dispatch table),
    // and therefore they can't send these IOCTLs to UMDF minidriver by calling 
    // win32 API.
    //
    nextStack = IoGetNextIrpStackLocation(Irp);
    nextStack->MajorFunction = IRP_MJ_DEVICE_CONTROL;

    //
    // Some IOCTLs are not of type METHOD_NEITHER and are forwarded by 
    // HIDClass to minidriver. We modify those IOCTLs to use METHOD_NEITHER
    // and send it down to UMDF driver.
    //
    ioctlCode = nextStack->Parameters.DeviceIoControl.IoControlCode;

    newIoctlCode = ioctlCode;
    
    switch(ioctlCode) {
    case IOCTL_HID_GET_DEVICE_DESCRIPTOR:           // METHOD_NEITHER, KM
    case IOCTL_HID_GET_REPORT_DESCRIPTOR:           // METHOD_NEITHER, KM
    case IOCTL_HID_READ_REPORT:                     // METHOD_NEITHER, KM
    case IOCTL_HID_ACTIVATE_DEVICE:                 // METHOD_NEITHER, KM
    case IOCTL_HID_DEACTIVATE_DEVICE:               // METHOD_NEITHER, KM
    case IOCTL_HID_GET_DEVICE_ATTRIBUTES:           // METHOD_NEITHER, KM
    case IOCTL_HID_SEND_IDLE_NOTIFICATION_REQUEST:  // METHOD_NEITHER, KM
        //
        // Nothing to do. These IOCTLs have been listed for completeness.
        //
        break;
    case IOCTL_HID_WRITE_REPORT:                    // METHOD_NEITHER, KM
    case IOCTL_HID_SET_FEATURE:                     // METHOD_IN_DIRECT, KM/UM
    case IOCTL_HID_GET_FEATURE:                     // METHOD_OUT_DIRECT, KM/UM
    case IOCTL_HID_GET_INPUT_REPORT:                // METHOD_OUT_DIRECT, KM/UM
    case IOCTL_HID_SET_OUTPUT_REPORT:               // METHOD_IN_DIRECT, KM/UM
        //
        // These IOCTLs use HID_XFER_PACKET. They need their buffer location 
        // updated. See comments in function for IOCTL specific updates.
        //
        status = UpdateBufferLocationAndIoctl(Irp, &newIoctlCode);
        if (!NT_SUCCESS(status)) {
            KdPrint(("HidUmdf: Ioctl %s failed status 0x%x\n", 
                DbgHidInternalIoctlString(ioctlCode), status));
            Irp->IoStatus.Status = status;
            IoCompleteRequest(Irp, IO_NO_INCREMENT);
            return status;
        }

        //
        // set completion routine
        //
        setCompletionRoutine = TRUE;
        break;

    case IOCTL_GET_PHYSICAL_DESCRIPTOR:             // METHOD_OUT_DIRECT, KM/UM 
        //
        // These IOCTLs are not METHOD_NEITHER but hidclass places buffers at
        // locations that are standard locations for METHOD_NEITHER 
        // (Type3InputBuffer and Irp->UserBuffer), so we modify the IOCTL type
        // to use METHOD_NEITHER so that UMDF can provide the buffers from 
        // standard METHOD_NEITHER buffer locations.
        // 
        newIoctlCode = IOCTL_UMDF_GET_PHYSICAL_DESCRIPTOR;
        break;

    case IOCTL_HID_GET_STRING:                      // METHOD_NEITHER, KM     
        //
        // This is a METHOD_NEITHER IOCTL. Hidclass places an input
        // ULONG value, and not a buffer, at Type3inputBuffer location. 
        // We store the input value in Irp->AssocatedIrp.SystemBuffer location
        // and store pointer to Irp->AssocatedIrp.SystemBuffer at 
        // Type3InputBuffer so that lower driver can access it as input buffer. 
        // 

        //
        // swap current SystemBuffer content with Type3inputBuffer
        //
        temp = Irp->AssociatedIrp.SystemBuffer;
        Irp->AssociatedIrp.SystemBuffer = 
            currStack->Parameters.DeviceIoControl.Type3InputBuffer;
        currStack->Parameters.DeviceIoControl.Type3InputBuffer = temp;

        //
        // store the address of SystemBuffer in next stack's Type3InputBuffer
        // and set buffer size
        //
        nextStack->Parameters.DeviceIoControl.Type3InputBuffer = 
            &Irp->AssociatedIrp.SystemBuffer;
        nextStack->Parameters.DeviceIoControl.InputBufferLength = sizeof(ULONG);

        setCompletionRoutine = TRUE;
        break;
    
    case IOCTL_HID_GET_INDEXED_STRING:              // METHOD_OUT_DIRECT, KM/UM
        //
        // This is a METHOD_OUT_DIRECT IOCTL however hidclass places buffer/value
        // in a mix of locations (Type3InputBuffer for input instead of 
        // Irp->AssociatedIrp.SystemBuffer and Irp->MdlAddress for output). 
        // Also, the input is not a buffer but a ULONG value.
        // 
        // We store the address of next stack's Type3InputBuffer that contains 
        // the input value at Irp->AssiciatedIrp.SystemBuffer 
        // (standard location for METHOD_OUT_DIRECT), and keep the output buffer
        // location (Irp->UserBuffer) unchanged  since it's already at standard
        // location. The input buffer location is reverted back to original in 
        // completion routine.
        // 

        //
        // store SystemBuffer in curr stack's Type3inputBuffer so we 
        // can get it back in completion routine.
        //
        currStack->Parameters.DeviceIoControl.Type3InputBuffer = 
            Irp->AssociatedIrp.SystemBuffer;

        //
        // store the address of next stack's Type3InputBuffer in  SystemBuffer
        // and set buffer size.
        //
        Irp->AssociatedIrp.SystemBuffer = 
            &nextStack->Parameters.DeviceIoControl.Type3InputBuffer;
        nextStack->Parameters.DeviceIoControl.InputBufferLength = sizeof(ULONG);

        setCompletionRoutine = TRUE;
        break;

    default:
        NT_ASSERTMSG("Unexpected IOCTL", FALSE);
        break;
    }

    //
    // update ioctl code for next stack location
    //
    nextStack->Parameters.DeviceIoControl.IoControlCode = newIoctlCode;

    if (Irp->RequestorMode == UserMode) {
        Irp->RequestorMode = KernelMode;
        setCompletionRoutine = TRUE;
        modeChanged = TRUE;
    }

    if (setCompletionRoutine) {
        IoSetCompletionRoutine(Irp,
                       UserIoctlCompletion,
                       (modeChanged ? (PVOID)Irp : NULL),  // context
                       TRUE,                       
                       TRUE,
                       TRUE );
    }

    return IoCallDriver(GET_NEXT_DEVICE_OBJECT(DeviceObject), Irp);
}

NTSTATUS
UpdateBufferLocationAndIoctl(
    _Inout_ PIRP Irp,
    _Out_ PULONG UpdatedIoctl
    )
/*++

Routine Description:

    The buffers are swapped in the following manner.
     
    Incoming Irp:
    ==============
    Irp->UserBuffer contains HID_XFER_PACKET which may have both input and 
        output data/buffer.

        struct HID_XFER_PACKET {
            PUCHAR  reportBuffer;   // output buffer
            ULONG  reportBufferLen; // output buffer length
            UCHAR  reportId;        // input data
        };

    Input/output buffer location: Irp->UserBuffer

    Updated Irp that is sent down:
    ==============================

    For IOCTLs that have both input and output:
        Input buffer location : Type3InputBuffer (= PHID_XFER_PACKET->reportId)
        Output buffer location: Irp->UserBuffer (= PHID_XFER_PACKET->reportBuffer) 
                                

    For IOCTLs that have two inputs:
        Input buffer location: Type3InputBuffer (= PHID_XFER_PACKET->reportBuffer)
        Input data           : Parameters.DeviceIoControl.OutputBufferLength (= PHID_XFER_PACKET->reportId)
    
--*/
{
    PHID_XFER_PACKET hidPacket;
    PIO_STACK_LOCATION currStack, nextStack;
    NTSTATUS status;
    ULONG ioctlCode, newIoctlCode;
    
    currStack = IoGetCurrentIrpStackLocation(Irp);
    nextStack = IoGetNextIrpStackLocation(Irp);
    status = STATUS_SUCCESS;
    ioctlCode = nextStack->Parameters.DeviceIoControl.IoControlCode;

    hidPacket = (PHID_XFER_PACKET) Irp->UserBuffer;
    if (hidPacket == NULL) {
        status = STATUS_INVALID_PARAMETER;
        return status;
    }

    //
    // Store original buffer pointer in current stack location so we can revert 
    // it back in completion routine
    //
    NT_ASSERTMSG("Type3InputBuffer is not NULL, not expected", 
        currStack->Parameters.DeviceIoControl.Type3InputBuffer == NULL);
    currStack->Parameters.DeviceIoControl.Type3InputBuffer = Irp->UserBuffer;

    switch (currStack->Parameters.DeviceIoControl.IoControlCode) {
    case IOCTL_HID_GET_FEATURE:          // METHOD_OUT_DIRECT, KM/UM
    case IOCTL_HID_GET_INPUT_REPORT:     // METHOD_OUT_DIRECT, KM/UM
        //
        // These IOCTLs have input and output buffer combined together
        // we separate them out.
        //

        if (currStack->Parameters.DeviceIoControl.OutputBufferLength 
            < sizeof(HID_XFER_PACKET)) {
            status = STATUS_INVALID_PARAMETER;
            return status;
        }

        //
        // Setup input data
        //
        nextStack->Parameters.DeviceIoControl.Type3InputBuffer =
            (PVOID)&hidPacket->reportId;
        nextStack->Parameters.DeviceIoControl.InputBufferLength = 
            sizeof(hidPacket->reportId);
             
        //
        // Setup output buffer
        //
        Irp->UserBuffer = (PVOID) hidPacket->reportBuffer;
        nextStack->Parameters.DeviceIoControl.OutputBufferLength = 
            hidPacket->reportBufferLen;

        break;
        
    case IOCTL_HID_WRITE_REPORT:        // METHOD_NEITHER, KM
    case IOCTL_HID_SET_FEATURE:         // METHOD_IN_DIRECT, KM/UM
    case IOCTL_HID_SET_OUTPUT_REPORT:   // METHOD_IN_DIRECT, KM/UM
        //
        // These IOCTLS have two inputs: 
        // 1) input buffer (PHID_XFER_PACKET->reportBuffer & reportBufferlength)
        // 2) input data (PHID_XFER_PACKET->reportId)
        // Input buffer is placed in Type3InputBuffer and its size in 
        //     InputBufferLength
        // Input data is placed in outputBufferlength 
        //

        if (currStack->Parameters.DeviceIoControl.InputBufferLength 
            < sizeof(HID_XFER_PACKET)) {
            status = STATUS_INVALID_PARAMETER;
            return status;
        }

        nextStack->Parameters.DeviceIoControl.Type3InputBuffer = 
            (PVOID) hidPacket->reportBuffer;
        nextStack->Parameters.DeviceIoControl.InputBufferLength = 
            hidPacket->reportBufferLen;
        nextStack->Parameters.DeviceIoControl.OutputBufferLength = 
            hidPacket->reportId;

        //
        // Use a scratch buffer allocated as a global buffer, sized 
        // to maximum possible reportlength (MAXUCHAR) and store it temporarily 
        // at Irp->UserBuffer since we've set a non-zero output buffer length. 
        // Note that UMDF will not copy the buffer content to user-mode driver's
        // buffer because it's an output buffer meant to written to, by the 
        // user-mode driver and not read.
        //
        Irp->UserBuffer = G_ScratchBuffer;
        
        break;

    default:
        NT_ASSERTMSG("Unexpected IOCTL", FALSE);
        break;
    }

    //
    // For those IOCTLs that are not METHOD_NEITHER, we map to new IOCTL 
    // code that is METHOD_NEITHER.
    //
    switch(ioctlCode) {
    case IOCTL_HID_SET_FEATURE:                          // METHOD_IN_DIRECT, KM/UM
        newIoctlCode = IOCTL_UMDF_HID_SET_FEATURE;       // METHOD_NEITHER
        break;
    case IOCTL_HID_GET_FEATURE:                          // METHOD_OUT_DIRECT, KM/UM
        newIoctlCode = IOCTL_UMDF_HID_GET_FEATURE;       // METHOD_NEITHER
        break;
    case IOCTL_HID_GET_INPUT_REPORT:                     // METHOD_OUT_DIRECT, KM/UM
        newIoctlCode = IOCTL_UMDF_HID_GET_INPUT_REPORT;  // METHOD_NEITHER 
        break;
    case IOCTL_HID_SET_OUTPUT_REPORT:                    // METHOD_IN_DIRECT, KM/UM
        newIoctlCode = IOCTL_UMDF_HID_SET_OUTPUT_REPORT; // METHOD_NEITHER 
        break;
    default:
        // 
        // keep the ioctl code unchanged
        //
        newIoctlCode = ioctlCode;
        break;
    }

    *UpdatedIoctl = newIoctlCode;

    return status;
}

NTSTATUS
HidUmdfPnp(
    _In_ PDEVICE_OBJECT DeviceObject,
    _Inout_ PIRP Irp
    )
/*++

Routine Description:

    Pass through routine for all the IRPs except power.

Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

Return Value:

      NT status code

--*/
{
    NTSTATUS status;
    BUS_QUERY_ID_TYPE queryIdType;
    PIO_STACK_LOCATION  currStack;

    PAGED_CODE();

    currStack = IoGetCurrentIrpStackLocation(Irp);

    //
    // Copy current stack to next instead of skipping to preserve 
    // current stack information provided by hidclass driver to the minidriver
    //
    IoCopyCurrentIrpStackLocationToNext(Irp);

    switch (currStack->MinorFunction) {
    case IRP_MN_QUERY_ID: 
        //
        // Handle Query ID. HIDClass only sends QueryDeviceId and QueryHardwareIDs
        // to minidriver. Root enumerated minidriver needs special handling.
        //
        queryIdType = currStack->Parameters.QueryId.IdType;
        switch (queryIdType) {
        case BusQueryDeviceID:
        case BusQueryHardwareIDs:
            status = HandleQueryId(DeviceObject, Irp);
            return status;
        default:
            break;
        }
 
        break;


    default:
        break;
    }
   
    return IoCallDriver(GET_NEXT_DEVICE_OBJECT(DeviceObject), Irp);
}

NTSTATUS
HidUmdfPassThrough(
    _In_ PDEVICE_OBJECT DeviceObject,
    _Inout_ PIRP Irp
    )
/*++

Routine Description:

    Pass through routine for all the IRPs except power.

Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

Return Value:

      NT status code

--*/
{
    PIO_STACK_LOCATION irpStack;

    irpStack = IoGetCurrentIrpStackLocation(Irp);

    IoCopyCurrentIrpStackLocationToNext(Irp);
    return IoCallDriver(GET_NEXT_DEVICE_OBJECT(DeviceObject), Irp);
}


NTSTATUS
HidUmdfPowerPassThrough(
    _In_ PDEVICE_OBJECT DeviceObject,
    _Inout_ PIRP Irp
    )
/*++

Routine Description:

    Pass through routine for power IRPs .

Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

Return Value:

      NT status code

--*/
{
    PoStartNextPowerIrp(Irp);
    IoCopyCurrentIrpStackLocationToNext(Irp);
    return PoCallDriver(GET_NEXT_DEVICE_OBJECT(DeviceObject), Irp);
}

NTSTATUS 
HidUmdfCreateCleanupClose(
    _In_ PDEVICE_OBJECT DeviceObject, 
    _Inout_ PIRP Irp
)
/*++

Routine Description:

   Process the Create and close IRPs sent to this device.

Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

Return Value:

      NT status code

--*/
{
    PIO_STACK_LOCATION   irpStack;
    NTSTATUS             ntStatus;

    UNREFERENCED_PARAMETER(DeviceObject);

    irpStack = IoGetCurrentIrpStackLocation(Irp);

    switch(irpStack->MajorFunction) {
    case IRP_MJ_CREATE:
    case IRP_MJ_CLOSE:
    case IRP_MJ_CLEANUP:
        Irp->IoStatus.Information = 0;
        ntStatus = STATUS_SUCCESS;
        break;

    default:
        ntStatus = STATUS_INVALID_PARAMETER;
        break;
    }

    Irp->IoStatus.Status = ntStatus;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);

    return ntStatus;
}

_Function_class_(IO_COMPLETION_ROUTINE)
_IRQL_requires_same_
NTSTATUS 
UserIoctlCompletion(
    _In_ PDEVICE_OBJECT DeviceObject,
    _In_ PIRP Irp,
    _In_reads_opt_(_Inexpressible_("varies")) PVOID Context
    )
/*++

Routine Description:

   Irp completion routine for those IOCTLs that were updtated in the dispatch
   routine and need to have those updates reverted back.

   The updtaes that were made are:
   1. Revert mode
   2. Revert buffer location

Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

   Context - driver speific context

Return Value:

      NT status code

--*/
{
    PIO_STACK_LOCATION currStack;
    BOOLEAN modeChanged = FALSE;

    UNREFERENCED_PARAMETER (DeviceObject);

    if (Irp->PendingReturned) {
        IoMarkIrpPending(Irp);
    }

    //
    // RequestorMode for user IOCTLs was changed to KemnleMode in the dispatch 
    // routine. We revert it back here. 
    //
    modeChanged = (Context == NULL) ? FALSE : TRUE;
    if (modeChanged) {
        Irp->RequestorMode = UserMode;
    }

    //
    // Revert buffer location to original. We stored original buffer pointer
    // in current stack's Type3InputBuffer.
    //
    currStack = IoGetCurrentIrpStackLocation(Irp);
    switch(currStack->Parameters.DeviceIoControl.IoControlCode) {
    case IOCTL_HID_WRITE_REPORT:                    // METHOD_NEITHER, KM
    case IOCTL_HID_SET_FEATURE:                     // METHOD_IN_DIRECT, KM/UM
    case IOCTL_HID_GET_FEATURE:                     // METHOD_OUT_DIRECT, KM/UM
    case IOCTL_HID_GET_INPUT_REPORT:                // METHOD_OUT_DIRECT, KM/UM
    case IOCTL_HID_SET_OUTPUT_REPORT:               // METHOD_IN_DIRECT, KM/UM
        Irp->UserBuffer = currStack->Parameters.DeviceIoControl.Type3InputBuffer;
        break;

    case IOCTL_HID_GET_STRING:                      // METHOD_NEITHER
        //
        // revert the swap done in dispatch routine. Type3InputBuffer has the
        // original value.
        //
        Irp->AssociatedIrp.SystemBuffer = 
            currStack->Parameters.DeviceIoControl.Type3InputBuffer;
        break;

    case IOCTL_HID_GET_INDEXED_STRING:              // METHOD_OUT_DIRECT, KM/UM
        //
        // revert the swap done in dispatch routine. Type3InputBuffer has the
        // original value.
        //
        Irp->AssociatedIrp.SystemBuffer = 
            currStack->Parameters.DeviceIoControl.Type3InputBuffer;
        break;

    default:
        break;
    }

    return STATUS_CONTINUE_COMPLETION;
}

NTSTATUS
HandleQueryId(
    _In_ PDEVICE_OBJECT Device,
    _Inout_ PIRP Irp
    )
/*++

Routine Description:

   This routine handles IRP_MN_QUERY_ID pnp irp.

Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

Return Value:

      NT status code

--*/
{
    PIO_STACK_LOCATION  irpStack;
    BUS_QUERY_ID_TYPE queryIdType;
    NTSTATUS status;
    PWCHAR buffer;

    PAGED_CODE();

    irpStack = IoGetCurrentIrpStackLocation(Irp);
    queryIdType = irpStack->Parameters.QueryId.IdType;

    //
    // send irp synchronously
    // 
    IoCopyCurrentIrpStackLocationToNext(Irp);
    status = SendIrpSynchronously(GET_NEXT_DEVICE_OBJECT(Device), Irp);

    switch (queryIdType) {
    case BusQueryDeviceID:
        if (!NT_SUCCESS(status)) {
            //
            // Lower driver should have provided a device ID. For 
            // root-enumerated devices, pnp manager provides the device ID 
            // created from the class name. e.g. "root\sample" where sample is 
            // the setup class name. Hidclass updates the ID by replacing 
            // enumerator with "HID" (HID\sample).
            //
            NT_ASSERTMSG("Lower driver did not provide a device ID. Not expected.", FALSE);
        }
        break;

    case BusQueryHardwareIDs:
        if (!NT_SUCCESS(status)) {
            //
            // Lower driver did not provide hardware ID. This would typically 
            // happen for root enumerated devices. Hidclass will not 
            // process this request further if we let this go in failed state.
            // Provide a dummy id. HIDclass will add more hardware IDs based 
            // on top-level collection information from hid descriptor.
            // e.g. "HID_DEVICE_UP:FF00_U:0001, HID_DEVICE" is added as 
            // hardware IDs by hidclass for a top-level collection PDO
            // that has USAGE_PAGE=FF00 and USAGE=0001.
            //
            NT_ASSERTMSG("Irp Information field is non-zero, not expected",
                Irp->IoStatus.Information == 0);

            buffer = (PWCHAR)ExAllocatePoolWithTag(
                          PagedPool,
                          HIDUMDF_HARDWARE_IDS_LENGTH,
                          HIDUMDF_POOL_TAG
                          );

            if (buffer) {
                //
                // Do the copy, store the buffer in the Irp
                //
                RtlCopyMemory(buffer,
                              HIDUMDF_HARDWARE_IDS,
                              HIDUMDF_HARDWARE_IDS_LENGTH
                              );

                Irp->IoStatus.Information = (ULONG_PTR)buffer;
                Irp->IoStatus.Status = STATUS_SUCCESS;
            }
            else {
                //
                //  No memory, just let previous status go through.
                //
            }
        }
        break;

    default:
        break;
    }

    //
    // we stopped completion in completion routine so complete the irp here.
    //
    status = Irp->IoStatus.Status;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);

    return status;
}

_Function_class_(IO_COMPLETION_ROUTINE)
_IRQL_requires_same_
NTSTATUS
SyncIrpCompletion(
    _In_ PDEVICE_OBJECT DeviceObject,
    _In_ PIRP Irp,
    _In_reads_opt_(_Inexpressible_("varies")) PVOID Context
    )
/*++

Routine Description:

   Irp completion routine for IRP_MN_QUERY_ID.

Arguments:

   DeviceObject - pointer to a device object.

   Irp - pointer to an I/O Request Packet.

   Context - driver speific context

Return Value:

      NT status code

--*/
{ 
    UNREFERENCED_PARAMETER(DeviceObject);
    
    if (Context != NULL && Irp->PendingReturned == TRUE) {
        KeSetEvent ((PKEVENT) Context, IO_NO_INCREMENT, FALSE);
    }

    return STATUS_MORE_PROCESSING_REQUIRED;  
}

PCHAR
DbgHidInternalIoctlString(
    _In_ ULONG        IoControlCode
    )
/*++

Routine Description:

    Returns Ioctl string helpful for debugging

Arguments:

    IoControlCode - IO Control code

Return Value:

    Name String

--*/
{
    switch (IoControlCode) {
    case IOCTL_HID_GET_DEVICE_DESCRIPTOR:
        return "IOCTL_HID_GET_DEVICE_DESCRIPTOR";
    case IOCTL_HID_GET_REPORT_DESCRIPTOR:
        return "IOCTL_HID_GET_REPORT_DESCRIPTOR";
    case IOCTL_HID_READ_REPORT:
        return "IOCTL_HID_READ_REPORT";
    case IOCTL_HID_GET_DEVICE_ATTRIBUTES:
        return "IOCTL_HID_GET_DEVICE_ATTRIBUTES";
    case IOCTL_HID_WRITE_REPORT:
        return "IOCTL_HID_WRITE_REPORT";
    case IOCTL_HID_SET_FEATURE:
        return "IOCTL_HID_SET_FEATURE";
    case IOCTL_HID_GET_FEATURE:
        return "IOCTL_HID_GET_FEATURE";
    case IOCTL_HID_GET_STRING:
        return "IOCTL_HID_GET_STRING";
    case IOCTL_HID_ACTIVATE_DEVICE:
        return "IOCTL_HID_ACTIVATE_DEVICE";
    case IOCTL_HID_DEACTIVATE_DEVICE:
        return "IOCTL_HID_DEACTIVATE_DEVICE";
    case IOCTL_HID_SEND_IDLE_NOTIFICATION_REQUEST:
        return "IOCTL_HID_SEND_IDLE_NOTIFICATION_REQUEST";
    case IOCTL_GET_PHYSICAL_DESCRIPTOR:              
        return "IOCTL_GET_PHYSICAL_DESCRIPTOR";
    case IOCTL_HID_GET_INDEXED_STRING:              
        return "IOCTL_HID_GET_INDEXED_STRING";
    case IOCTL_HID_GET_INPUT_REPORT:                
        return "IOCTL_HID_GET_INPUT_REPORT";
    case IOCTL_HID_SET_OUTPUT_REPORT:               
        return "IOCTL_HID_SET_OUTPUT_REPORT";
    default:
        return "Unknown IOCTL";
    }
}

