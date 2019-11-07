/*****************************************************************************
 * adapter.cpp - DMusic adapter implementation
 *
 *  This includes the adapter driver for the
 *  kernel mode DirectMusic DLS 1.0 SW synthesizer
 *
 *****************************************************************************
 * Copyright (c) 1998-2000 Microsoft Corporation.  All rights reserved.
 *
 *  06/08/98    MartinP
 *
 */

//
// All the GUIDS for all the miniports end up in this object.
//
#define PUT_GUIDS_HERE

#define STR_MODULENAME "DDKSynth.sys:Adapter: "

#define PC_NEW_NAMES 1

#include "common.h"
#include "private.h"


/*****************************************************************************
 * Defines
 */

#define MAX_MINIPORTS 1

#if (DBG)
#define SUCCEEDS(s) ASSERT(NT_SUCCESS(s))
#else
#define SUCCEEDS(s) (s)
#endif

/*****************************************************************************
 * Referenced forward
 */
DRIVER_ADD_DEVICE AddDevice;

NTSTATUS
StartDevice
(
    IN      PDEVICE_OBJECT  pDeviceObject,  // Context for the class driver.
    IN      PIRP            pIrp,           // Context for the class driver.
    IN      PRESOURCELIST   ResourceList    // List of hardware resources.
);

#pragma code_seg("INIT")
/*****************************************************************************
 * DriverEntry()
 *****************************************************************************
 * This function is called by the operating system when the driver is loaded.
 * All adapter drivers can use this code without change.
 */
extern "C"
NTSTATUS
DriverEntry
(
    IN      PVOID   Context1,   // Context for the class driver.
    IN      PVOID   Context2    // Context for the class driver.
)
{
    PAGED_CODE();
    _DbgPrintF(DEBUGLVL_VERBOSE, ("DriverEntry"));

    //
    // Tell the class driver to initialize the driver.
    //
    return PcInitializeAdapterDriver((PDRIVER_OBJECT)Context1,
                                     (PUNICODE_STRING)Context2,
                                     (PDRIVER_ADD_DEVICE)AddDevice);
}


#pragma code_seg(push, "PAGE")

/*****************************************************************************
 * AddDevice()
 *****************************************************************************
 * This function is called by the operating system when the device is added.
 * All adapter drivers can use this code without change.
 */
#pragma prefast (suppress: __WARNING_INIT_NOT_CLEARED, "disable prefast warning 28152 because DO_DEVICE_INITIALIZING is cleared in PcAddAdapterDevice")
NTSTATUS AddDevice
(
    _In_ struct _DRIVER_OBJECT* DriverObject,   // Context for the class driver.
    _In_ struct _DEVICE_OBJECT* PhysicalDeviceObject    // Context for the class driver.
)
{
    PAGED_CODE();
    _DbgPrintF(DEBUGLVL_VERBOSE, ("AddDevice"));

    //
    // Tell the class driver to add the device.
    //
    return PcAddAdapterDevice(DriverObject, PhysicalDeviceObject, StartDevice, MAX_MINIPORTS, 0);
}


/*****************************************************************************
 * StartDevice()
 *****************************************************************************
 *
 *  This function is called by the operating system when the device is started.
 *  It is responsible for starting the miniports.  This code is specific to
 *  the adapter because it calls out miniports for functions that are specific
 *  to the adapter.  A list of no resources is not the same as a NULL list ptr.
 *
 */
NTSTATUS
StartDevice
(
    IN      PDEVICE_OBJECT  pDeviceObject,  // Context for the class driver.
    IN      PIRP            pIrp,           // Context for the class driver.
    IN      PRESOURCELIST   ResourceList    // List of hardware resources.
)
{
    PAGED_CODE();

    _DbgPrintF(DEBUGLVL_VERBOSE, ("StartDevice"));

    ASSERT(ResourceList);
    if (!ResourceList)
    {
        return STATUS_INVALID_PARAMETER;
    }

    PPORT       port;
    NTSTATUS    ntStatus = PcNewPort(&port, CLSID_PortDMus);

    if (!NT_SUCCESS(ntStatus))
    {
        _DbgPrintF(DEBUGLVL_TERSE,("StartDevice PcNewPort failed (0x%08x)", ntStatus));
        return ntStatus;
    }
    ASSERT(port);

    PUNKNOWN miniport;
#ifdef USE_OBSOLETE_FUNCS
    ntStatus = CreateMiniportDmSynth(&miniport, NULL, NonPagedPool);
#else
    ntStatus = CreateMiniportDmSynth(&miniport, NULL, NonPagedPool, pDeviceObject);
#endif

    if (!NT_SUCCESS(ntStatus))
    {
        _DbgPrintF(DEBUGLVL_TERSE,("StartDevice CreateMiniportDmSynth failed (0x%08x)", ntStatus));
        port->Release();
        return ntStatus;
    }
    ASSERT(miniport);

    ntStatus =
        port->Init
        (
            pDeviceObject,
            pIrp,
            miniport,
            NULL,
            ResourceList
        );

    if (!NT_SUCCESS(ntStatus))
    {
        _DbgPrintF(DEBUGLVL_TERSE,("StartDevice port Init failed (0x%08x)", ntStatus));
        port->Release();
        miniport->Release();
        return ntStatus;
    }

    ntStatus = PcRegisterSubdevice( pDeviceObject,
                                    L"DDKSynth",
                                    port);
    if (!NT_SUCCESS(ntStatus))
    {
        _DbgPrintF(DEBUGLVL_TERSE,("StartDevice PcRegisterSubdevice failed (0x%08x)", ntStatus));
    }

    //
    // We don't need the miniport any more.  Either the port has it,
    // or we've failed, and it should be deleted.
    //
    miniport->Release();

    //
    // Release the reference which existed when PcNewPort() gave us the
    // pointer in the first place.  This is the right thing to do
    // regardless of the outcome.
    //
    port->Release();

    return ntStatus;
}

#pragma code_seg(pop)


