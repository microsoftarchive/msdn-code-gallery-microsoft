/*++

Copyright (c) 1990-2000  Microsoft Corporation

Module Name:

    driver.h

Abstract:

    This is a C version of a very simple sample driver that illustrates
    how to use the driver framework and demonstrates best practices.

--*/

#define INITGUID

#include <ntddk.h>
#include <wdf.h>


#include "device.h"
#include "queue.h"

#define GPD_DEVICE_NAME L"\\Device\\Gpd0"
#define GPD_TYPE 40000
#define DOS_DEVICE_NAME L"\\DosDevices\\GpdDev"
//
// WDFDRIVER Events
//

DRIVER_INITIALIZE DriverEntry;

EVT_WDF_DRIVER_DEVICE_ADD PortIOEvtDeviceAdd;
WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(DEVICE_CONTEXT, PortIOGetDeviceContext)


