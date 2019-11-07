/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    driver.c

Abstract:

    This module contains the 16550 UART controller driver entry functions.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

#include "driver.h"

#include "tracing.h"
#include "driver.tmh"

WDF_EXTERN_C_START

//
// Uart16550pc Event Handler: DriverEntry
//
NTSTATUS
DriverEntry(
    IN PDRIVER_OBJECT  DriverObject,
    IN PUNICODE_STRING RegistryPath
    )
{
    WDF_DRIVER_CONFIG config;
    NTSTATUS status;

    WPP_INIT_TRACING(DriverObject, RegistryPath);

    FuncEntry(TRACE_FLAG_INIT);

    WDF_DRIVER_CONFIG_INIT(&config, NULL);

    config.EvtDriverUnload = UartEvtDriverUnload;
    config.EvtDriverDeviceAdd = UartEvtDeviceAdd;

    // Creates the driver.
    status = WdfDriverCreate(DriverObject,
                            RegistryPath,
                            WDF_NO_OBJECT_ATTRIBUTES,
                            &config,
                            WDF_NO_HANDLE);
    
    if (!NT_SUCCESS(status))
    {
        KdPrint(("Error: WdfDriverCreate failed 0x%x\n", status));

        FuncExit(TRACE_FLAG_INIT);
        WPP_CLEANUP(DriverObject);
        return status;
    }

    FuncExit(TRACE_FLAG_INIT);
    return status;
}

WDF_EXTERN_C_END

//
// Uart16550pc Event Handler: UartEvtDriverUnload
//
VOID
UartEvtDriverUnload(
    _In_ WDFDRIVER DriverObject
    )
{
    FuncEntry(TRACE_FLAG_UNLOAD);
    
    FuncExit(TRACE_FLAG_UNLOAD);
    WPP_CLEANUP(DriverObject);
}

//
// Uart16550pc Event Handler: UartEvtDeviceAdd
//
NTSTATUS
UartEvtDeviceAdd(
    IN WDFDRIVER       Driver,
    IN PWDFDEVICE_INIT DeviceInit
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Driver);

    FuncEntry(TRACE_FLAG_INIT);

    status = UartDeviceCreate(DeviceInit);

    FuncExit(TRACE_FLAG_INIT);
    return status;
}
