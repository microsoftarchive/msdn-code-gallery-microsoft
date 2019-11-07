/*++

Copyright (c) 1990-2000  Microsoft Corporation

Module Name:

    queue.c

Abstract:

    This is a C version of a very simple sample driver that illustrates
    how to use the driver framework and demonstrates best practices.

--*/

#include "driver.h"

#ifdef ALLOC_PRAGMA
#pragma alloc_text (PAGE, PortIOQueueInitialize)
#pragma alloc_text (PAGE, PortIOEvtIoDeviceControl)
#pragma alloc_text (PAGE, PortIOIoctlReadPort)
#pragma alloc_text (PAGE, PortIOIoctlWritePort)
#endif

NTSTATUS
PortIOQueueInitialize(
    _In_ WDFDEVICE Device
    )
/*++

Routine Description:


     The I/O dispatch callbacks for the frameworks device object
     are configured in this function.

     A single default I/O Queue is configured for serial request
     processing, and a driver context memory allocation is created
     to hold our structure QUEUE_CONTEXT.

     This memory may be used by the driver automatically synchronized
     by the Queue's presentation lock.

     The lifetime of this memory is tied to the lifetime of the I/O
     Queue object, and we register an optional destructor callback
     to release any private allocations, and/or resources.


Arguments:

    Device - Handle to a framework device object.

Return Value:

    VOID

--*/
{
    WDFQUEUE queue;
    NTSTATUS status;
    WDF_IO_QUEUE_CONFIG    queueConfig;

    PAGED_CODE();

    //
    // Configure a default queue so that requests that are not
    // configure-fowarded using WdfDeviceConfigureRequestDispatching to goto
    // other queues get dispatched here.
    //
    WDF_IO_QUEUE_CONFIG_INIT_DEFAULT_QUEUE(
        &queueConfig,
        WdfIoQueueDispatchSequential
        );

    // 
    // By registering the IOCTL handler for the default queue,
    // the default queue will fail all other requests
    //
    queueConfig.EvtIoDeviceControl = PortIOEvtIoDeviceControl;    

    //
    // NOTE: If you need to have the queue's callbacks synchronized 
    // with one another then you should set the WDF_SYNCHRONIZATION_SCOPE
    // enum (member of WDF_OBJECT_ATTRIBUTES) to WdfSynchronizationScopeQueue.
    // When that is done ONE of the following should also be done: 
    // 1) Remove the pragmas that cause the queue's callback functions to be paged
    //    out. Also remove the PAGED_CODE macros from the functions.
    // 2) If you would like to have the callbacks pageable, then set
    //    the WDF_EXECUTION_LEVEL enum (member of WDF_OBJECT_ATTRIBUTES)
    //    to WdfExecutionLevelPassive.
    // 

    //
    // By default, Static Driver Verifier (SDV) displays a warning if it 
    // doesn't find the EvtIoStop callback on a power-managed queue. 
    // The 'assume' below causes SDV to suppress this warning. If the driver 
    // has not explicitly set PowerManaged to WdfFalse, the framework creates
    // power-managed queues when the device is not a filter driver.  Normally 
    // the EvtIoStop is required for power-managed queues, but for this driver
    // it is not needed b/c the driver doesn't hold on to the requests or 
    // forward them to other drivers. This driver completes the requests 
    // directly in the queue's handlers. If the EvtIoStop callback is not 
    // implemented, the framework waits for all driver-owned requests to be
    // done before moving in the Dx/sleep states or before removing the 
    // device, which is the correct behavior for this type of driver.
    // If the requests were taking an indeterminate amount of time to complete,
    // or if the driver forwarded the requests to a lower driver/another stack,
    // the queue should have an EvtIoStop/EvtIoResume.
    //
    __analysis_assume(queueConfig.EvtIoStop != 0);
    status = WdfIoQueueCreate(
                 Device,
                 &queueConfig,
                 WDF_NO_OBJECT_ATTRIBUTES,
                 &queue
                 );
    __analysis_assume(queueConfig.EvtIoStop == 0);

    if( !NT_SUCCESS(status) ) {
        KdPrint(("WdfIoQueueCreate failed 0x%x\n",status));
        return status;
    }

    return status;
}

VOID
PortIOEvtIoDeviceControl(
    _In_ WDFQUEUE     Queue,
    _In_ WDFREQUEST   Request,
    _In_ size_t       OutputBufferLength,
    _In_ size_t       InputBufferLength,
    _In_ ULONG        IoControlCode
    )
{

    PDEVICE_CONTEXT devContext = NULL;
    WDFDEVICE device;
    NTSTATUS status;

    PAGED_CODE();
    
    device = WdfIoQueueGetDevice(Queue);
    
    devContext = PortIOGetDeviceContext(device);
    
    switch(IoControlCode){
        case IOCTL_GPD_READ_PORT_UCHAR:
        case IOCTL_GPD_READ_PORT_USHORT:
        case IOCTL_GPD_READ_PORT_ULONG:
            PortIOIoctlReadPort(devContext,
                                Request,
                                OutputBufferLength,
                                InputBufferLength,
                                IoControlCode);
            break;


        case IOCTL_GPD_WRITE_PORT_UCHAR:
        case IOCTL_GPD_WRITE_PORT_USHORT:
        case IOCTL_GPD_WRITE_PORT_ULONG:    
            PortIOIoctlWritePort(devContext,
                                 Request,
                                 OutputBufferLength,
                                 InputBufferLength,
                                 IoControlCode);
            break;

        default:
            status = STATUS_INVALID_PARAMETER;
            WdfRequestComplete(Request, status);

            break;

                               
    }
        
}

VOID
PortIOIoctlReadPort(
    _In_ PDEVICE_CONTEXT devContext,
    _In_ WDFREQUEST Request,
    _In_ size_t OutBufferSize,
    _In_ size_t InBufferSize,
    _In_ ULONG IoctlCode)
/*++

Routine Description:
    This routine processes the IOCTLs which read from the ports.

Arguments:

    devContext        -  local device data
    Request        - WDF request object
    OutBufferSize    - Size of buffer for data to be sent to application
    InBufferSize - Size of buffer containing data from application
    IoctlCode   - The ioctl code from the IRP

Return Value:
    STATUS_SUCCESS           -- OK

    STATUS_INVALID_PARAMETER -- The buffer sent to the driver
                                was too small to contain the
                                port, or the buffer which
                                would be sent back to the driver
                                was not a multiple of the data size.

    STATUS_ACCESS_VIOLATION  -- An illegal port number was given.

--*/    
{
    NTSTATUS status = STATUS_SUCCESS;
    ULONG minDataBufferSize; // minimum output buffer size
    ULONG nPort; // Port number to read                                                
    PULONG pOutBuffer;  // Pointer to transfer buffer
                                   //  (treated as an array of longs).
    PULONG pInBuffer;

    UNREFERENCED_PARAMETER(OutBufferSize);
    UNREFERENCED_PARAMETER(InBufferSize);
    
    PAGED_CODE();
                                   
    switch (IoctlCode){
        case IOCTL_GPD_READ_PORT_UCHAR:
            minDataBufferSize = sizeof(UCHAR);
            break;
        case IOCTL_GPD_READ_PORT_USHORT:
            minDataBufferSize = sizeof(USHORT);
            break;
        case IOCTL_GPD_READ_PORT_ULONG:
            minDataBufferSize = sizeof(ULONG);
            break;
        default:
            status = STATUS_INVALID_PARAMETER;
            goto exit;    
    }
    status = WdfRequestRetrieveInputBuffer(
                 Request,
                 sizeof(ULONG), 
                 &((PVOID) pInBuffer),
                 NULL);
    if (!NT_SUCCESS(status)) {
        goto exit;
        
    }
    status = WdfRequestRetrieveOutputBuffer(
                                                                Request,
                                                                minDataBufferSize,
                                                                &((PVOID) pOutBuffer),
                                                                NULL);
    if (!NT_SUCCESS(status)) {
        goto exit;        
    }
                                                                
    nPort = *pInBuffer;

    if (nPort >= devContext -> PortCount ||
        (nPort + minDataBufferSize) > devContext -> PortCount ||
        (((ULONG_PTR)devContext->PortBase + nPort) & (minDataBufferSize- 1)) != 0){

        status = STATUS_ACCESS_VIOLATION;
        goto exit;
    }

    if (devContext->PortMemoryType == 1){
        // Address is in I/O space

        switch (IoctlCode){
        case IOCTL_GPD_READ_PORT_UCHAR:
            *(PUCHAR)pOutBuffer = READ_PORT_UCHAR(
                            (PUCHAR)((ULONG_PTR)devContext->PortBase + nPort) );
            break;
        case IOCTL_GPD_READ_PORT_USHORT:
            *(PUSHORT)pOutBuffer = READ_PORT_USHORT(
                            (PUSHORT)((ULONG_PTR)devContext->PortBase + nPort) );
            break;
        case IOCTL_GPD_READ_PORT_ULONG:
            *(PULONG)pOutBuffer = READ_PORT_ULONG(
                            (PULONG)((ULONG_PTR)devContext->PortBase + nPort) );
            break;

        default:
            status =  STATUS_INVALID_PARAMETER;
            goto exit;


        }
    }
    else if (devContext->PortMemoryType == 0)
    {
        // Address is in Memory space

        switch (IoctlCode)
        {
        case IOCTL_GPD_READ_PORT_UCHAR:
            *(PUCHAR)pOutBuffer = READ_REGISTER_UCHAR(
                            (PUCHAR)((ULONG_PTR)devContext->PortBase + nPort) );
            break;
        case IOCTL_GPD_READ_PORT_USHORT:
            *(PUSHORT)pOutBuffer = READ_REGISTER_USHORT(
                            (PUSHORT)((ULONG_PTR)devContext->PortBase + nPort) );
            break;
        case IOCTL_GPD_READ_PORT_ULONG:
            *(PULONG)pOutBuffer = READ_REGISTER_ULONG(
                            (PULONG)((ULONG_PTR)devContext->PortBase + nPort) );
            break;
        default:
            status = STATUS_INVALID_PARAMETER;
            goto exit;

        }
    }
    else
    {
        status = STATUS_UNSUCCESSFUL;
        goto exit;
    }

    WdfRequestCompleteWithInformation(Request, status, minDataBufferSize);
    return;

exit:

    WdfRequestComplete(Request, status);
}

VOID
PortIOIoctlWritePort(
    _In_ PDEVICE_CONTEXT devContext,
    _In_ WDFREQUEST Request,
    _In_ size_t OutBufferSize,
    _In_ size_t InBufferSize,
    _In_ ULONG IoctlCode)
    /*++

Routine Description:
    This routine processes the IOCTLs which write to the ports.

Arguments:

    devContext        -  local device data
    Request        - WDF request object
    OutBufferSize    - Size of buffer for data to be sent to application
    InBufferSize - Size of buffer containing data from application
    IoctlCode   - The ioctl code from the IRP

Return Value:
    STATUS_SUCCESS           -- OK

    STATUS_INVALID_PARAMETER -- The buffer sent to the driver
                                was too small to contain the
                                port, or the buffer which
                                would be sent back to the driver
                                was not a multiple of the data size.

    STATUS_ACCESS_VIOLATION  -- An illegal port number was given.    
    --*/
{
    NTSTATUS status = STATUS_SUCCESS;
    ULONG minDataBufferSize; // minimum output buffer size
    ULONG nPort; // Port number to read                                                
    PULONG pInBuffer;
    
    UNREFERENCED_PARAMETER(OutBufferSize);
    UNREFERENCED_PARAMETER(InBufferSize);

    PAGED_CODE();
                                   
    switch (IoctlCode){
        case IOCTL_GPD_WRITE_PORT_UCHAR:
            minDataBufferSize = sizeof(UCHAR);
            break;
        case IOCTL_GPD_WRITE_PORT_USHORT:
            minDataBufferSize = sizeof(USHORT);
            break;
        case IOCTL_GPD_WRITE_PORT_ULONG:
            minDataBufferSize = sizeof(ULONG);
            break;
        default:
            status = STATUS_INVALID_PARAMETER;
            goto exit;    
    }
    status = WdfRequestRetrieveInputBuffer(
                Request,
                sizeof(ULONG), 
                &((PVOID) pInBuffer),
                NULL);
    if (!NT_SUCCESS(status)) {
        goto exit;
        
    }
    
    nPort = *pInBuffer++;

    if (nPort >= devContext -> PortCount ||
        (nPort + minDataBufferSize) > devContext -> PortCount ||
        (((ULONG_PTR)devContext->PortBase + nPort) & (minDataBufferSize- 1)) != 0){

        status = STATUS_ACCESS_VIOLATION;
        goto exit;
    }

    if (devContext->PortMemoryType == 1){
        // Address is in I/O space

        switch (IoctlCode){
        case IOCTL_GPD_WRITE_PORT_UCHAR:
            WRITE_PORT_UCHAR(
                (PUCHAR)((ULONG_PTR)devContext->PortBase + nPort),
                *(PUCHAR)pInBuffer);
            break;
        case IOCTL_GPD_WRITE_PORT_USHORT:
            WRITE_PORT_USHORT(
                (PUSHORT)((ULONG_PTR)devContext->PortBase + nPort),
                *(PUSHORT)pInBuffer);
            break;
        case IOCTL_GPD_WRITE_PORT_ULONG:
            WRITE_PORT_ULONG(
                (PULONG)((ULONG_PTR)devContext->PortBase + nPort),
                *(PULONG)pInBuffer);
            break;

        default:
            status =  STATUS_INVALID_PARAMETER;
            goto exit;
        }
    }
    else if (devContext->PortMemoryType == 0)
    {
        // Address is in Memory space

        switch (IoctlCode)
        {
        case IOCTL_GPD_WRITE_PORT_UCHAR:
            WRITE_REGISTER_UCHAR(
                    (PUCHAR)((ULONG_PTR)devContext->PortBase + nPort),
                    *(PUCHAR)pInBuffer);
            break;
        case IOCTL_GPD_WRITE_PORT_USHORT:
            WRITE_REGISTER_USHORT(
                    (PUSHORT)((ULONG_PTR)devContext->PortBase + nPort),
                    *(PUSHORT)pInBuffer );
            break;
        case IOCTL_GPD_WRITE_PORT_ULONG:
            WRITE_REGISTER_ULONG(
                    (PULONG)((ULONG_PTR)devContext->PortBase + nPort),
                    *(PULONG)pInBuffer );
            break;
        default:
            status = STATUS_INVALID_PARAMETER;
            goto exit;

        }
    }
    else
    {
        status = STATUS_UNSUCCESSFUL;
        goto exit;
    }

    WdfRequestCompleteWithInformation(Request, status, minDataBufferSize);
    return;
    
exit:
    WdfRequestComplete(Request, status);
    return;
}

