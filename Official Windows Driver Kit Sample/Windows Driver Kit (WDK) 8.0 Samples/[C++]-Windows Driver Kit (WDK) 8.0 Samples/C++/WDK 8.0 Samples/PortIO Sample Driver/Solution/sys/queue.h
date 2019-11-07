/*++

Copyright (c) 1990-2000  Microsoft Corporation

Module Name:

    queue.h

Abstract:

    This is a C version of a very simple sample driver that illustrates
    how to use the driver framework and demonstrates best practices.

--*/

#include "gpioctl.h"        // Get IOCTL interface definitions


NTSTATUS
PortIOQueueInitialize(
    _In_ WDFDEVICE hDevice
    );

VOID
PortIOIoctlWritePort(
    _In_ PDEVICE_CONTEXT devContext,
    _In_ WDFREQUEST Request,
    _In_ size_t OutBufferSize,
    _In_ size_t InBufferSize,
    _In_ ULONG IoctlCode);    

VOID 
PortIOIoctlReadPort(
    _In_ PDEVICE_CONTEXT devContext,
    _In_ WDFREQUEST Request,
    _In_ size_t OutBufferSize,
    _In_ size_t InBufferSize,
    _In_ ULONG IoctlCode);

//
// Events from the IoQueue object
//
EVT_WDF_IO_QUEUE_IO_DEVICE_CONTROL PortIOEvtIoDeviceControl;
EVT_WDF_DEVICE_FILE_CREATE PortIOEvtFileCreate;
EVT_WDF_FILE_CLOSE PortIOEvtFileClose;
