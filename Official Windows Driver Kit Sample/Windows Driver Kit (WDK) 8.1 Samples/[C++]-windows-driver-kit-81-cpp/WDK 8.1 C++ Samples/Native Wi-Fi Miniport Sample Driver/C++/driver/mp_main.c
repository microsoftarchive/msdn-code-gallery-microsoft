/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_main.c

Abstract:
    Implements most of the NDIS entry points into the driver
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "mp_main.h"
#include "mp_pnp.h"
#include "helper_port_intf.h"

#if DOT11_TRACE_ENABLED
#include "mp_main.tmh"
#endif

NDIS_HANDLE                     GlobalDriverContext = NULL;
NDIS_HANDLE                     GlobalDriverHandle = NULL;

ULONG                           FailDriverEntry = 0;

//
// NDIS and WDM Driver handlers
//
DRIVER_INITIALIZE DriverEntry;

NTSTATUS
DriverEntry(
    _In_    PDRIVER_OBJECT        DriverObject,
    _In_    PUNICODE_STRING       RegistryPath
    )
{
    NDIS_STATUS                 Status = NDIS_STATUS_FAILURE;
    NDIS_MINIPORT_DRIVER_CHARACTERISTICS    MChars;
    ULONG                       ndisVersion;

    if (FailDriverEntry)
    {
        DbgPrint("FAILING DRIVER ENTRY\n");
        return NDIS_STATUS_FAILURE;
    }

    #if DOT11_TRACE_ENABLED
        WPP_INIT_TRACING(DriverObject, RegistryPath);
    #endif

    MpTrace(COMP_INIT_PNP, DBG_SERIOUS, (__DATE__ " " __TIME__ " DriverEntry called!\n"));

    do
    {
        //
        // Identify the appropriate read/write lock API
        //
        Status = MpDetermineRWLockType();
        if (Status != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to determine type of read/write lock to use. Status = 0x%x\n", Status));
            break;
        }
    
        NdisZeroMemory(&MChars, sizeof(NDIS_MINIPORT_DRIVER_CHARACTERISTICS));

        //
        // Set the type and version of this structure. We select the appropriate version &
        // driver functionality based on NDIS version.
        //
        ndisVersion = NdisGetVersion();
        if (ndisVersion <= MP_NDIS_VERSION_NEEDS_COMPATIBILITY)
        {
            // NDIS Version 6.0
            MChars.Header.Type      = NDIS_OBJECT_TYPE_MINIPORT_DRIVER_CHARACTERISTICS;
            MChars.Header.Size      = NDIS_SIZEOF_MINIPORT_DRIVER_CHARACTERISTICS_REVISION_1;
            MChars.Header.Revision  = NDIS_MINIPORT_DRIVER_CHARACTERISTICS_REVISION_1;
            
            MChars.MajorNdisVersion = MP_MAJOR_NDIS_VERSION;
            MChars.MinorNdisVersion = 0;
        }
        else
        {
            // NDIS Version 6.2
            MChars.Header.Type      = NDIS_OBJECT_TYPE_MINIPORT_DRIVER_CHARACTERISTICS;
            MChars.Header.Size      = NDIS_SIZEOF_MINIPORT_DRIVER_CHARACTERISTICS_REVISION_2;
            MChars.Header.Revision  = NDIS_MINIPORT_DRIVER_CHARACTERISTICS_REVISION_2;
            
            MChars.MajorNdisVersion = MP_MAJOR_NDIS_VERSION;
            MChars.MinorNdisVersion = MP_MINOR_NDIS_VERSION;
        }
        
        MChars.MajorDriverVersion = HW11_MAJOR_DRIVER_VERSION;
        MChars.MinorDriverVersion = HW11_MINOR_DRIVER_VERSION;

        //
        // Init/PnP handlers
        //
        MChars.InitializeHandlerEx      = MPInitialize;
        MChars.RestartHandler           = MPRestart;
        MChars.PauseHandler             = MPPause;
        MChars.ShutdownHandlerEx        = MPAdapterShutdown;
        MChars.DevicePnPEventNotifyHandler  = MPDevicePnPEvent;
        MChars.HaltHandlerEx            = MPHalt;
        MChars.UnloadHandler            = DriverUnload;
        
        //
        // Query/Set/Method requests handlers
        //
        MChars.OidRequestHandler        = MPOidRequest;
        MChars.CancelOidRequestHandler  = MPCancelOidRequest;

        //
        // Set optional miniport services handler
        //
        MChars.SetOptionsHandler        = MPSetOptions;
        
        //
        // Send/Receive handlers
        //
        MChars.SendNetBufferListsHandler    = MPSendNetBufferLists;
        MChars.CancelSendHandler            = MPCancelSendNetBufferLists;
        MChars.ReturnNetBufferListsHandler  = MPReturnNetBufferLists;
        
        //
        // Fault handling handlers
        //
        MChars.CheckForHangHandlerEx        = MPCheckForHang;
        MChars.ResetHandlerEx               = MPReset;

        //
        // Direct OID request handlers
        //
        MChars.DirectOidRequestHandler          = MPDirectOidRequest;
        MChars.CancelDirectOidRequestHandler    = MPCancelDirectOidRequest;
        
        //
        // Register the miniport driver with NDIS
        //
        Status = NdisMRegisterMiniportDriver(
                    DriverObject,
                    RegistryPath,
                    GlobalDriverContext,
                    &MChars,
                    &GlobalDriverHandle
                    );
        if (Status != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to register miniport with NDIS. Status = 0x%x\n", Status));
            break;
        }

#if DBG
#if !DOT11_TRACE_ENABLED
        //
        // Read debug mask from registry
        //
        MpReadGlobalDebugMask(GlobalDriverHandle);
#endif
#endif

    }
    while (FALSE);
    
    if (Status != NDIS_STATUS_SUCCESS)
    {
        #if DOT11_TRACE_ENABLED
            WPP_CLEANUP(DriverObject);
        #endif
    }

    return(Status);
}

VOID
DriverUnload(
    PDRIVER_OBJECT          DriverObject
    )
{
    UNREFERENCED_PARAMETER(DriverObject);
    
    MpEntry;
    
    //
    // Deregister this miniport from NDIS
    //
    NdisMDeregisterMiniportDriver(GlobalDriverHandle);
    
    MpExit;
    
    MP_DUMP_LEAKING_BLOCKS();

    #if DOT11_TRACE_ENABLED
        WPP_CLEANUP(DriverObject);
    #endif    
}


NDIS_STATUS
MpGetAdapterStatus(
    _In_  PADAPTER                Adapter
    )
{
    NDIS_STATUS ndisStatus;

    if (MP_TEST_ADAPTER_STATUS(Adapter, MP_ADAPTER_PAUSED))
        ndisStatus = NDIS_STATUS_PAUSED;
    else if (MP_TEST_ADAPTER_STATUS(Adapter, MP_ADAPTER_PAUSING))
        ndisStatus = NDIS_STATUS_PAUSED;
    else if (MP_TEST_ADAPTER_STATUS(Adapter, MP_ADAPTER_IN_RESET))
        ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
    else if (MP_TEST_ADAPTER_STATUS(Adapter, MP_ADAPTER_HALTING))
        ndisStatus = NDIS_STATUS_CLOSING;
    else if (MP_TEST_ADAPTER_STATUS(Adapter, MP_ADAPTER_SURPRISE_REMOVED))
        ndisStatus = NDIS_STATUS_ADAPTER_REMOVED;
    else
        ndisStatus = NDIS_STATUS_FAILURE;       // return a generc error

    return ndisStatus;
}


NDIS_STATUS
MPSetOptions(
    NDIS_HANDLE             NdisMiniportDriverHandle,
    NDIS_HANDLE             MiniportDriverContext
    )
{
    UNREFERENCED_PARAMETER(NdisMiniportDriverHandle);
    UNREFERENCED_PARAMETER(MiniportDriverContext);

    return NDIS_STATUS_SUCCESS;

}

VOID
MPSendNetBufferLists(
    NDIS_HANDLE             MiniportAdapterContext,
    PNET_BUFFER_LIST        NetBufferLists,
    NDIS_PORT_NUMBER        PortNumber,
    ULONG                   SendFlags
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_PORT                    destinationPort = NULL;
    PNET_BUFFER_LIST            currentNetBufferList;

    //
    // We increment the port list refcount. This would avoid a pause from finishing
    // while we are processing this NBL and that ensures that the port list does not
    // change
    //    
    MP_INCREMENT_PORTLIST_REFCOUNT(adapter);


    do
    {
        //
        // If the adapter is paused, surprise removed, etc, fail the send
        //
        if (MP_TEST_ADAPTER_STATUS(adapter, MP_ADAPTER_CANNOT_SEND_MASK))
        {
            ndisStatus = MpGetAdapterStatus(adapter);
            MpTrace(COMP_SEND, DBG_NORMAL, ("Sends failed as adapter is not in a valid send state\n"));
            break;
        }

        //
        // First we would need to translate from the NDIS_PORT_NUMBER
        // to our port structure. This is done by walking the PortList
        //
        destinationPort = Port11TranslatePortNumberToPort(
                            adapter, 
                            PortNumber
                            );
        if (destinationPort == NULL)
        {
            MpTrace(COMP_SEND, DBG_SERIOUS, ("Unable to find Port corresponding to PortNumber %d\n", 
                PortNumber));

            ndisStatus = NDIS_STATUS_INVALID_PORT;
        }
        else
        {
            //
            // Pass it to the appropriate port for processing
            //
            Port11HandleSendNetBufferLists(
                destinationPort,
                NetBufferLists,
                SendFlags
                );
        }

    } while (FALSE);

    //
    // We were protecting the port list only until we hand it to the port. After this point, the port
    // is responsible for ensuring that it does not let the port get deleted while
    // it has packets pending on it
    //
    MP_DECREMENT_PORTLIST_REFCOUNT(adapter);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        #if DBG
        ULONG ulNumFailedNBLs = 0;
        #endif
        
        //
        // Send failed. Complete the NBLs back to NDIS
        //
        for(currentNetBufferList = NetBufferLists;
            currentNetBufferList != NULL;
            currentNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList))
        {
            #if DBG
            ulNumFailedNBLs++;
            #endif
            NET_BUFFER_LIST_STATUS(currentNetBufferList) = ndisStatus;
        }
        
        #if DBG
            MpTrace(COMP_SEND, DBG_NORMAL, ("NdisMSendNetBufferListsComplete called with %d NBLs\n", ulNumFailedNBLs));
        #endif

        if (NetBufferLists != NULL)
        {
            NdisMSendNetBufferListsComplete(
                adapter->MiniportAdapterHandle, 
                NetBufferLists, 
                (NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags) ? NDIS_SEND_COMPLETE_FLAGS_DISPATCH_LEVEL : 0)
                );
        }
    }

}

VOID 
MPCancelSendNetBufferLists(
    NDIS_HANDLE             MiniportAdapterContext,
    PVOID                   CancelId
    )
{
    // TODO: Unsure how to identify which port the packets have been
    // sent on
    UNREFERENCED_PARAMETER(MiniportAdapterContext);
    UNREFERENCED_PARAMETER(CancelId);
    MPASSERT(FALSE);
}

VOID 
MPReturnNetBufferLists(
    NDIS_HANDLE             MiniportAdapterContext,
    PNET_BUFFER_LIST        NetBufferLists,
    ULONG                   ReturnFlags
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;
    PMP_PORT                    destinationPort = NULL;
    PNET_BUFFER_LIST            currentNetBufferList = NetBufferLists, nextNetBufferList;

    //
    // We increment the port list refcount. This would avoid a pause from finishing
    // while we are processing this NBL and that ensures that the port list does not
    // change
    //    
    MP_INCREMENT_PORTLIST_REFCOUNT(adapter);

    //
    // Walk the chain of netbuffer lists
    //
    while (currentNetBufferList != NULL)
    {
        // 
        // Currently we return 1 NET_BUFFER_LIST at a time since we may be getting
        // back NET_BUFFER_LISTS indicated by different ports on 1 call. 
        // This can be optimized
        //        
        nextNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList);
        NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList) = NULL;
        
        destinationPort = MP_NBL_SOURCE_PORT(currentNetBufferList);
        MPASSERT(destinationPort != NULL);
        
        //
        // Pass it to the appropriate port for processing
        //
        Port11HandleReturnNetBufferLists(
            destinationPort,
            currentNetBufferList,
            ReturnFlags
            );

        currentNetBufferList = nextNetBufferList;
    }

    //
    // We were protecting the port list only until we hand it to the port. After this point, the port
    // is responsible for ensuring that it does not let the port get deleted while
    // it has packets pending on it
    //
    MP_DECREMENT_PORTLIST_REFCOUNT(adapter);

}

BOOLEAN
MPCheckForHang(
    NDIS_HANDLE             MiniportAdapterContext
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;
    BOOLEAN                     needReset = FALSE;

    // TODO: Does the HVL layer need check for hang

    // Pass this call to the HW
    needReset = Hw11CheckForHang(adapter->Hw);
    if (needReset)
    {
        MpTrace(COMP_MISC, DBG_SERIOUS, ("HW is hung and requested a reset\n"));
        return TRUE;
    }

    return FALSE;
}

_Function_class_(NDIS_IO_WORKITEM_FUNCTION)
NTSTATUS
MpResetWorkItem(
    _In_  PVOID                   Context,
    _In_  NDIS_HANDLE             NdisIoWorkItemHandle
    )    
{
    PADAPTER                    adapter = (PADAPTER)Context;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     addressingReset = FALSE;

    MP_VERIFY_PASSIVE_IRQL();

    //
    // Pass the reset request to the helper port
    //
    ndisStatus = HelperPortHandleMiniportReset(adapter->HelperPort, &addressingReset);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_MISC, DBG_SERIOUS, ("Requesting adapter removal because Hw Reset has failed\n"));
        MpRemoveAdapter(adapter);            
    }

    MP_CLEAR_ADAPTER_STATUS(adapter, MP_ADAPTER_IN_RESET);

    NdisFreeIoWorkItem(NdisIoWorkItemHandle);

    // Inform NDIS about reset complete
    NdisMResetComplete(
        adapter->MiniportAdapterHandle,
        ndisStatus,
        addressingReset
        );

    return STATUS_SUCCESS;
}


NDIS_STATUS
MPReset(
    NDIS_HANDLE             MiniportAdapterContext,
    PBOOLEAN                AddressingReset
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_PENDING;
    NDIS_HANDLE                 workitemHandle;

    *AddressingReset = TRUE;
    
#if DBG
    if (adapter->Debug_BreakOnReset)
    {
        DbgPrint("Received NdisReset\n");
        DbgBreakPoint();
    }
#endif

    do
    {
        //
        // Set the flag so that other routines stop proceeding
        //
        MP_SET_ADAPTER_STATUS(adapter, MP_ADAPTER_IN_RESET);
        
        //
        // If our halt handler has been called, we should not reset
        //
        if (MP_TEST_ADAPTER_STATUS(adapter, MP_ADAPTER_HALTING))
        {
            MPASSERT(FALSE);    // Would be an interesting scenario to investigate
            ndisStatus = NDIS_STATUS_SUCCESS;
            break;
        }

        //
        // Handle the reset asynchronously since we can be called at either dispatch
        // or passive IRQL
        //
        workitemHandle = NdisAllocateIoWorkItem(adapter->MiniportAdapterHandle);
        if(workitemHandle == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate Reset workitem\n"));
            NdisWriteErrorLogEntry(adapter->MiniportAdapterHandle,
                NDIS_ERROR_CODE_OUT_OF_RESOURCES,
                0
                );
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Queue the workitem
        NdisQueueIoWorkItem(workitemHandle, 
            MpResetWorkItem,
            adapter
            );

    } while (FALSE);   

    if (ndisStatus != NDIS_STATUS_PENDING)
    {
        // Something failed, clear the in reset flag
        MP_CLEAR_ADAPTER_STATUS(adapter, MP_ADAPTER_IN_RESET);
    }
    return ndisStatus;
}

NDIS_STATUS
Mp11CtxSStart(   
    _In_  PADAPTER                Adapter
    )
{
    UNREFERENCED_PARAMETER(Adapter);
    
    return NDIS_STATUS_SUCCESS;
}

VOID
Mp11CtxSComplete(   
    _In_  PADAPTER                Adapter
    )
{
    UNREFERENCED_PARAMETER(Adapter);
    
    return;
}



