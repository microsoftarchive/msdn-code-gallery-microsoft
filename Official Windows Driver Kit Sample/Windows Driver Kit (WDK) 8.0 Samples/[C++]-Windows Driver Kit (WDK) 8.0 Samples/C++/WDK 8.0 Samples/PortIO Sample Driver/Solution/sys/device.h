/*++

Copyright (c) 1990-2000  Microsoft Corporation

Module Name:

    device.h

Abstract:

    This is a C version of a very simple sample driver that illustrates
    how to use the driver framework and demonstrates best practices.

--*/



//
// The device context performs the same job as
// a WDM device extension in the driver frameworks
//
typedef struct _DEVICE_CONTEXT
{
    PVOID PortBase;       // base port address
    ULONG PortCount;      // Count of I/O addresses used.
    ULONG PortMemoryType;
    BOOLEAN PortWasMapped;  // If TRUE, we have to unmap on unload
} DEVICE_CONTEXT, *PDEVICE_CONTEXT;

//
// Function to initialize the device and its callbacks
//
NTSTATUS
PortIODeviceCreate(
    PWDFDEVICE_INIT DeviceInit
    );

//
// Device events
//
EVT_WDF_DEVICE_PREPARE_HARDWARE PortIOEvtDevicePrepareHardware;
EVT_WDF_DEVICE_RELEASE_HARDWARE PortIOEvtDeviceReleaseHardware;

