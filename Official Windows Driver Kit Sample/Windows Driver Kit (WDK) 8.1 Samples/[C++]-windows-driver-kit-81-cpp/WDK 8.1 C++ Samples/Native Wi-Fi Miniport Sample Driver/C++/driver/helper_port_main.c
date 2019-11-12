/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    helper_port_main.c

Abstract:
    Implements the interfaces for the helper port
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "port_defs.h"
#include "base_port_intf.h"
#include "helper_port_defs.h"
#include "helper_port_intf.h"
#include "vnic_intf.h"
#include "helper_port_main.h"
#include "helper_port_bsslist.h"
#include "helper_port_scan.h"
#include "glb_utils.h"

#if DOT11_TRACE_ENABLED
#include "helper_port_main.tmh"
#endif


MP_REG_ENTRY HelperRegTable[] = {
    {
        NDIS_STRING_CONST("BSSEntryMaxCount"), 
        NdisParameterInteger,
        0,      // This is the structure passed in
        FIELD_OFFSET(MP_HELPER_REG_INFO, BSSEntryMaxCount),
        sizeof(ULONG),
        MP_BSS_ENTRY_MAX_ENTRIES_DEFAULT,
        MP_BSS_ENTRY_MAX_ENTRIES_MIN,
        MP_BSS_ENTRY_MAX_ENTRIES_MAX 
    },

    {
        NDIS_STRING_CONST("BSSEntryExpireTime"), 
        NdisParameterInteger,
        0,      // This is the structure passed in
        FIELD_OFFSET(MP_HELPER_REG_INFO, BSSEntryExpireTime),
        sizeof(ULONG),
        MP_BSS_ENTRY_EXPIRE_TIME_DEFAULT,
        MP_BSS_ENTRY_EXPIRE_TIME_MIN,
        MP_BSS_ENTRY_EXPIRE_TIME_MAX    
    },

    {
        NDIS_STRING_CONST("RSSILinkQualityThreshold"), 
        NdisParameterInteger,
        0,      // This is the structure passed in
        FIELD_OFFSET(MP_HELPER_REG_INFO, RSSILinkQualityThreshold),
        sizeof(ULONG),
        MP_POOR_LINK_QUALITY_THRESHOLD_DEFAULT,
        MP_POOR_LINK_QUALITY_THRESHOLD_MIN,
        MP_POOR_LINK_QUALITY_THRESHOLD_MAX 
    },

    {
        NDIS_STRING_CONST("ActiveScanChannelCount"), 
        NdisParameterInteger,
        0,      // This is the structure passed in
        FIELD_OFFSET(MP_HELPER_REG_INFO, ActiveScanChannelCount),
        sizeof(ULONG),
        MP_SCAN_SET_CHANNEL_COUNT_ACTIVE_DEFAULT,
        MP_SCAN_SET_CHANNEL_COUNT_ACTIVE_MIN,
        MP_SCAN_SET_CHANNEL_COUNT_ACTIVE_MAX
    },

    {
        NDIS_STRING_CONST("PassiveScanChannelCount"), 
        NdisParameterInteger,
        0,
        FIELD_OFFSET(MP_HELPER_REG_INFO, PassiveScanChannelCount),
        sizeof(ULONG),
        MP_SCAN_SET_CHANNEL_COUNT_PASSIVE_DEFAULT,
        MP_SCAN_SET_CHANNEL_COUNT_PASSIVE_MIN,
        MP_SCAN_SET_CHANNEL_COUNT_PASSIVE_MAX
    },

    {
        NDIS_STRING_CONST("ScanRescheduleTime"), 
        NdisParameterInteger,
        0,
        FIELD_OFFSET(MP_HELPER_REG_INFO, ScanRescheduleTime),
        sizeof(ULONG),
        MP_SCAN_RESCHEDULE_TIME_MS_DEFAULT,
        MP_SCAN_RESCHEDULE_TIME_MS_MIN,
        MP_SCAN_RESCHEDULE_TIME_MS_MAX 
    },


    {
        NDIS_STRING_CONST("InterScanTime"), 
        NdisParameterInteger,
        0,
        FIELD_OFFSET(MP_HELPER_REG_INFO, InterScanTime),
        sizeof(ULONG),
        MP_INTER_SCAN_TIME_DEFAULT,
        MP_INTER_SCAN_TIME_MIN,
        MP_INTER_SCAN_TIME_MAX 
    }
    
};

#define HELPER_NUM_REG_PARAMS       (((ULONG)sizeof (HelperRegTable)) / ((ULONG)sizeof(MP_REG_ENTRY)))


NDIS_STATUS
HelperPortLoadRegistryInformation(
    _In_        NDIS_HANDLE             MiniportAdapterHandle,
    _In_opt_    NDIS_HANDLE             ConfigurationHandle,
    _Out_       PVOID*                  RegistryInformation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_REG_INFO         helperRegInfo = NULL;

    *RegistryInformation = NULL;
    do
    {
        //
        // Allocate memory for the registry info
        //
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle, &helperRegInfo, sizeof(MP_HELPER_REG_INFO), PORT_MEMORY_TAG);
        if (helperRegInfo == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for MP_HELPER_REG_INFO\n",
                                 sizeof(MP_HELPER_REG_INFO)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(helperRegInfo, sizeof(MP_HELPER_REG_INFO));

        //
        // read registry values 
        //
        MpReadRegistry((PVOID)helperRegInfo, ConfigurationHandle, HelperRegTable, HELPER_NUM_REG_PARAMS);

        *RegistryInformation = helperRegInfo;
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Free the buffer
        HelperPortFreeRegistryInformation(helperRegInfo);
        helperRegInfo = NULL;
    }
    
    return ndisStatus;
}

VOID
HelperPortFreeRegistryInformation(
    _In_opt_  PVOID              RegistryInformation
    )
{
    if (RegistryInformation != NULL)
    {
        MP_FREE_MEMORY(RegistryInformation);
    }
}

NDIS_STATUS
HelperPortAllocatePort(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_PORT             newHelperPort = NULL;

    UNREFERENCED_PARAMETER(Adapter);

    do
    {
        // Allocate the helper port specific structure
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle, &newHelperPort, sizeof(MP_HELPER_PORT), PORT_MEMORY_TAG);
        if (newHelperPort == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for helper port\n",
                                 sizeof(MP_HELPER_PORT)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Clear everything
        NdisZeroMemory(newHelperPort, sizeof(MP_HELPER_PORT));

        // Save pointer to the parent port & vice versa
        Port->ChildPort = newHelperPort;
        newHelperPort->ParentPort = Port;

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (Port != NULL)
        {
            HelperPortFreePort(Port);
        }
    }

    return ndisStatus;

}


VOID
HelperPortFreePort(
    _In_  PMP_PORT                Port
    )
{
    PMP_HELPER_PORT             helperPort = NULL;

    if (Port->ChildPort != NULL)
    {
        // Delete the helper port structure associated with this port
        helperPort = Port->ChildPort;
        MP_FREE_MEMORY(helperPort);
    }                
}


NDIS_STATUS
HelperPortInitializePort(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   RegistryInformation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    BOOLEAN                     freeBSSList = FALSE, freeScanContext = FALSE;

    do
    {
        // Save the registry value
        helperPort->RegInfo = (PMP_HELPER_REG_INFO)RegistryInformation;
    
        ndisStatus = HelperPortInitializeBSSList(helperPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        freeBSSList = TRUE;

        ndisStatus = HelperPortInitializeScanContext(helperPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        freeScanContext = TRUE;

        helperPort->DevicePowerState = NdisDeviceStateD0;
        
        // Setup the default handler functions
        Port->ReceiveEventHandler = HelperPortReceiveEventHandler;

    }while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (freeScanContext)
        {
            HelperPortTerminateScanContext(helperPort);
        }
        
        if (freeBSSList)
        {
            HelperPortTerminateBSSList(helperPort);
        }
    }
    
    return ndisStatus;
}

VOID
HelperPortTerminatePort(
    _In_  PMP_PORT                Port
    )
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);

    HelperPortTerminateScanContext(helperPort);

#if DBG
    HelperPortCheckForExtraRef(helperPort);
#endif

    HelperPortFlushBSSList(Port);

    // Cleanup the BSS list
    HelperPortTerminateBSSList(helperPort);
}


NDIS_STATUS
HelperPortRequestExclusiveAccess(
    _In_  PMP_HELPER_PORT       HelperPort,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CallbackFunction,
    _In_  PVOID                 Context,
    _In_  BOOLEAN               PnPOperation
    )
{
    NDIS_STATUS                 ndisStatus;

    NdisInterlockedIncrement(&HelperPort->Tracking_ExclusiveAccessAcquireCount);
    ndisStatus = VNic11ReqExAccess(HELPPORT_GET_VNIC(HelperPort), 
                     CallbackFunction, 
                     Context,
                     PnPOperation
                     );
    if ((ndisStatus != NDIS_STATUS_SUCCESS) && (ndisStatus != NDIS_STATUS_PENDING))
    {
        // Request failed
        
        // Update the exclusive release access for ref debugging
        NdisInterlockedIncrement(&HelperPort->Tracking_ExclusiveAccessReleaseCount);    
    }

    return ndisStatus;
}

VOID
HelperPortReleaseExclusiveAccess(
    _In_  PMP_HELPER_PORT       HelperPort
    )
{
    // We have exclusive access
    NdisInterlockedDecrement(&HelperPort->Debug_ExclusiveAccessCount);

    NdisInterlockedIncrement(&HelperPort->Tracking_ExclusiveAccessReleaseCount);    
    VNic11ReleaseExAccess(HELPPORT_GET_VNIC(HelperPort));
}


VOID
HelperPortExclusiveAccessGranted(
    _In_  PMP_HELPER_PORT       HelperPort
    )
{
    // We have exclusive access
    NdisInterlockedIncrement(&HelperPort->Debug_ExclusiveAccessCount);

    NdisInterlockedIncrement(&HelperPort->Tracking_ExclusiveAccessGrantCount);    
}

VOID
HelperPortDelegateExclusiveAccess(
    _In_  PMP_HELPER_PORT       HelperPort,
    _In_  PMP_PORT              PortToActivate
    )
{
    // If we have multiple exclusive accesses, we cannot delegate
    MPASSERT(HelperPort->Debug_ExclusiveAccessCount == 1);
    
    Hvl11ActivatePort(HELPPORT_GET_MP_PORT(HelperPort)->Adapter->Hvl, PORT_GET_VNIC(PortToActivate));
}

NDIS_STATUS 
HelperPortPauseMiniport(
    _In_        PMP_HELPER_PORT                 HelperPort,
    _In_        BOOLEAN                         IsInternal,
    _In_opt_    PNDIS_MINIPORT_PAUSE_PARAMETERS MiniportPauseParameters
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PADAPTER                    adapter = HELPPORT_GET_MP_PORT(HelperPort)->Adapter;
    ULONG                       i;

    MpEntry;
    UNREFERENCED_PARAMETER(MiniportPauseParameters);

//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Helper Port Pause\n"));        

    HELPPORT_GET_MP_PORT(HelperPort)->PauseCount++;
    
    if (HELPPORT_GET_MP_PORT(HelperPort)->PauseCount > 1)
    {
        // We are already paused, no need to do anything

        MpExit;
//        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Helper Port Pause\n"));        

        return NDIS_STATUS_SUCCESS;
    }
        
    // Set the pausing flag on the adapter
    MP_ACQUIRE_ADAPTER_LOCK(adapter, FALSE);
    MP_SET_ADAPTER_STATUS(adapter, MP_ADAPTER_PAUSING);
    MP_RELEASE_ADAPTER_LOCK(adapter, FALSE);

    for (i = 0; i < adapter->NumberOfPorts; i++)
    {
        // Ask the HVL to active the specific port. This is
        // a synchronous private API call into the HVL
        HelperPortDelegateExclusiveAccess(HelperPort, adapter->PortList[i]);

        // Ask the port to pause
        ndisStatus = Port11PausePort(adapter->PortList[i], IsInternal);
        
        MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    }

    // Reobtain active context
    HelperPortDelegateExclusiveAccess(HelperPort, HELPPORT_GET_MP_PORT(HelperPort));

    //
    // Wait for the port list refcount in the adapter to go down to 0
    //
    MP_WAIT_FOR_PORTLIST_REFCOUNT(adapter);

    // Set the paused flag on us
    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
    MP_SET_PORT_STATUS(HELPPORT_GET_MP_PORT(HelperPort), MP_PORT_PAUSED);
    MP_RELEASE_ADAPTER_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    //
    // Let the VNic know we have paused
    //
    ndisStatus = VNic11Pause(HELPPORT_GET_VNIC(HelperPort));
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

    //
    // Pause the hardware last
    //
    ndisStatus = Hw11Pause(adapter->Hw);
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

    // stop timed context switches
    Hvl11BlockTimedCtxS(adapter->Hvl);

    // Set paused & clear the pausing flag
    MP_ACQUIRE_ADAPTER_LOCK(adapter, FALSE);
    MP_SET_ADAPTER_STATUS(adapter, MP_ADAPTER_PAUSED);
    MP_CLEAR_ADAPTER_STATUS(adapter, MP_ADAPTER_PAUSING);
    MP_RELEASE_ADAPTER_LOCK(adapter, FALSE);

    MpExit;
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Helper Port Pause\n"));        

    return ndisStatus;
}

NDIS_STATUS
HelperPortPauseMPExAccessCallback(PMP_PORT Port, PVOID Ctx)
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_MINIPORT_PAUSE_PARAMETERS pauseParameters = (PMP_MINIPORT_PAUSE_PARAMETERS) Ctx;

    HelperPortExclusiveAccessGranted(helperPort);
    
    ndisStatus = HelperPortPauseMiniport(helperPort, 
                        (pauseParameters->NdisParameters == NULL)? TRUE : FALSE, 
                        pauseParameters->NdisParameters
                        );

    NdisSetEvent(&pauseParameters->CompleteEvent);

    return ndisStatus;
}

NDIS_STATUS 
HelperPortHandleMiniportPause(
    _In_  PMP_PORT                Port,
    _In_opt_  PNDIS_MINIPORT_PAUSE_PARAMETERS     MiniportPauseParameters
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    MP_MINIPORT_PAUSE_PARAMETERS    pauseParameters;

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Miniport Pause Started\n"));
    
    // Populate the parameters to pass to the handler
    NdisZeroMemory(&pauseParameters, sizeof(MP_MINIPORT_PAUSE_PARAMETERS));
    pauseParameters.NdisParameters = MiniportPauseParameters;
    NdisInitializeEvent(&pauseParameters.CompleteEvent);
    NdisResetEvent(&pauseParameters.CompleteEvent);

    // The lock is acquired here since the VNIC expects us to have only
    // one PNP operation requesting exclusive access from it
    PORT_ACQUIRE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    // get exclusive access
    ndisStatus = HelperPortRequestExclusiveAccess(helperPort,
                     HelperPortPauseMPExAccessCallback, 
                     &pauseParameters,
                     TRUE
                     );

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // the function completed synchronously, call the callback ourselves
        HelperPortPauseMPExAccessCallback(Port, &pauseParameters);
    }
    else if (ndisStatus == NDIS_STATUS_PENDING)
    {
        // Will be processed asynchronously
        ndisStatus = NDIS_STATUS_SUCCESS;
    }

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // wait for the pause to complete
        NdisWaitEvent(&pauseParameters.CompleteEvent, 0);
            
        // release exclusive access
        HelperPortReleaseExclusiveAccess(helperPort);
    }
    else    // Exclusive access request failed
    {
        // Update the exclusive release access for ref debugging
        NdisInterlockedIncrement(&helperPort->Tracking_ExclusiveAccessReleaseCount);    
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Exclusive access request for Miniport Pause failed with status 0x%08x\n", ndisStatus));        
    }

    // Exclusive access is released before this is released
    PORT_RELEASE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Miniport Pause Finished\n"));
    
    return ndisStatus;
}


NDIS_STATUS 
HelperPortRestartMiniport(
    _In_        PMP_HELPER_PORT                     HelperPort,
    _In_        BOOLEAN                             IsInternal,
    _In_opt_    PNDIS_MINIPORT_RESTART_PARAMETERS   MiniportRestartParameters
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PADAPTER                    adapter = HelperPort->ParentPort->Adapter;
    ULONG                       i;

    MpEntry;
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Helper Port Restart\n"));        

    UNREFERENCED_PARAMETER(MiniportRestartParameters);

    HELPPORT_GET_MP_PORT(HelperPort)->PauseCount--;
    if (HELPPORT_GET_MP_PORT(HelperPort)->PauseCount > 0)
    {
        // We have multiple pauses pending. We wont restart
        // yet

        MpExit;
//        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Helper Port Restart\n"));        

        // We still return success to the restart request
        return NDIS_STATUS_SUCCESS;
    }

    //
    // Restart the hardware first
    //
    Hw11Restart(adapter->Hw);

    //
    // Let the VNic know we have restarted, so that ports can
    // start using us as soon as we restart them
    //
    ndisStatus = VNic11Restart(HELPPORT_GET_VNIC(HelperPort));
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

    // Clear the paused flag on us
    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
    MP_CLEAR_PORT_STATUS(HELPPORT_GET_MP_PORT(HelperPort), MP_PORT_PAUSED);
    MP_RELEASE_ADAPTER_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
    
    // Now restart all the other port
    for (i = 0; i < adapter->NumberOfPorts; i++)
    {
        // Ask the HVL to active the specific port. This is
        // a synchronous private API call into the HVL
        Hvl11ActivatePort(adapter->Hvl, PORT_GET_VNIC(adapter->PortList[i]));

        // Ask the port to restart. We dont active each individual
        // context yet        
        ndisStatus = Port11RestartPort(adapter->PortList[i], IsInternal);
        
        MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    }

    // Reobtain active context
    Hvl11ActivatePort(adapter->Hvl, HELPPORT_GET_VNIC(HelperPort));

    // re-enable timed context switches
    Hvl11UnblockTimedCtxS(adapter->Hvl);

    // Clear the paused flag on the adapter
    MP_ACQUIRE_ADAPTER_LOCK(adapter, FALSE);
    MP_CLEAR_ADAPTER_STATUS(adapter, MP_ADAPTER_PAUSED);
    MP_RELEASE_ADAPTER_LOCK(adapter, FALSE);

//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Helper Port Restart\n"));        

    MpExit;
    
    return ndisStatus;
}


NDIS_STATUS
HelperPortRestartMPExAccessCallback(PMP_PORT Port, PVOID Ctx)
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_MINIPORT_RESTART_PARAMETERS restartParameters = (PMP_MINIPORT_RESTART_PARAMETERS) Ctx;

    HelperPortExclusiveAccessGranted(helperPort);
    
    ndisStatus = HelperPortRestartMiniport(helperPort, 
                        (restartParameters->NdisParameters == NULL)? TRUE : FALSE, 
                        restartParameters->NdisParameters
                        );

    NdisSetEvent(&restartParameters->CompleteEvent);

    return ndisStatus;
}

NDIS_STATUS 
HelperPortHandleMiniportRestart(
    _In_  PMP_PORT                Port,
    _In_opt_  PNDIS_MINIPORT_RESTART_PARAMETERS   MiniportRestartParameters
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    MP_MINIPORT_RESTART_PARAMETERS    restartParameters;

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Miniport Restart Started\n"));

    // Populate the parameters to pass to the handler
    NdisZeroMemory(&restartParameters, sizeof(MP_MINIPORT_RESTART_PARAMETERS));
    restartParameters.NdisParameters = MiniportRestartParameters;
    NdisInitializeEvent(&restartParameters.CompleteEvent);
    NdisResetEvent(&restartParameters.CompleteEvent);

    // The lock is acquired here since the VNIC expects us to have only
    // one PNP operation requesting exclusive access from it
    PORT_ACQUIRE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    // get exclusive access
    ndisStatus = HelperPortRequestExclusiveAccess(helperPort, 
                     HelperPortRestartMPExAccessCallback, 
                     &restartParameters,
                     TRUE
                     );

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // the function completed synchronously, call the callback ourselves
        HelperPortRestartMPExAccessCallback(Port, &restartParameters);
    }
    else if (ndisStatus == NDIS_STATUS_PENDING)
    {
        // Will be processed asynchronously
        ndisStatus = NDIS_STATUS_SUCCESS;
    }

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // wait for the restart to complete
        NdisWaitEvent(&restartParameters.CompleteEvent, 0);
        
        // release exclusive access
        HelperPortReleaseExclusiveAccess(helperPort);
    }
    else    // Exclusive access request failed
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Exclusive access request for Miniport Restart failed with status 0x%08x\n", ndisStatus));        
    }

    // Exclusive access is released before this is released
    PORT_RELEASE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Miniport Restart Finished\n"));

    return ndisStatus;    
}


NDIS_STATUS 
HelperPortPausePort(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_PORT                PortToPause    
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    MpEntry;

    // Ask the HVL to active the specific port. This is
    // a synchronous private API call into the HVL
    HelperPortDelegateExclusiveAccess(HelperPort, PortToPause);

    // Ask the port to pause
    ndisStatus = Port11PausePort(PortToPause, TRUE);
    
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

    // Reobtain active context
    HelperPortDelegateExclusiveAccess(HelperPort, HELPPORT_GET_MP_PORT(HelperPort));
    
    MpExit;

    return ndisStatus;
}


NDIS_STATUS
HelperPortPausePortExAccessCallback(PMP_PORT Port, PVOID Ctx)
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;    
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_PORT_PAUSE_PARAMETERS   pauseParameters = (PMP_PORT_PAUSE_PARAMETERS) Ctx;

    HelperPortExclusiveAccessGranted(helperPort);
    
    ndisStatus = HelperPortPausePort(MP_GET_HELPPORT(Port), pauseParameters->PortToPause);

    NdisSetEvent(&pauseParameters->CompleteEvent);

    return ndisStatus;
}


NDIS_STATUS 
HelperPortHandlePortPause(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                PortToPause
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    MP_PORT_PAUSE_PARAMETERS    pauseParameters;

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Port Pause Started\n"));

    // Populate the parameters to pass to the handler
    NdisZeroMemory(&pauseParameters, sizeof(MP_PORT_PAUSE_PARAMETERS));
    pauseParameters.PortToPause = PortToPause;
    NdisInitializeEvent(&pauseParameters.CompleteEvent);
    NdisResetEvent(&pauseParameters.CompleteEvent);

    // The lock is acquired here since the VNIC expects us to have only
    // one PNP operation requesting exclusive access from it
    PORT_ACQUIRE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    // get exclusive access
    ndisStatus = HelperPortRequestExclusiveAccess(helperPort, 
        HelperPortPausePortExAccessCallback, 
        &pauseParameters,
        TRUE
        );
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // the function completed synchronously, call the callback ourselves
        HelperPortPausePortExAccessCallback(Port, &pauseParameters);
    }
    else if (ndisStatus == NDIS_STATUS_PENDING)
    {
        // Will be processed asynchronously
        ndisStatus = NDIS_STATUS_SUCCESS;
    }

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // wait for the pause to complete
        NdisWaitEvent(&pauseParameters.CompleteEvent, 0);

        // release exclusive access
        HelperPortReleaseExclusiveAccess(helperPort);
    }
    else    // Exclusive access request failed
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Exclusive access request for Port Pause failed with status 0x%08x\n", ndisStatus));        
    }

    // Exclusive access is released before this is released
    PORT_RELEASE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Port Pause Finished\n"));

    return ndisStatus;    
}



NDIS_STATUS 
HelperPortRestartPort(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_PORT                PortToRestart
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(HelperPort);
    MpEntry;

    // Ask the port to restart. We dont active the
    // context yet        
    ndisStatus = Port11RestartPort(PortToRestart, TRUE);
    
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    
    MpExit;
    
    return ndisStatus;
}

NDIS_STATUS
HelperPortRestartPortExAccessCallback(PMP_PORT Port, PVOID Ctx)
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;    
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_PORT_RESTART_PARAMETERS restartParameters = (PMP_PORT_RESTART_PARAMETERS) Ctx;

    HelperPortExclusiveAccessGranted(helperPort);
    
    ndisStatus = HelperPortRestartPort(MP_GET_HELPPORT(Port), restartParameters->PortToRestart);

    NdisSetEvent(&restartParameters->CompleteEvent);

    return ndisStatus;
}

NDIS_STATUS 
HelperPortHandlePortRestart(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                PortToRestart
    )
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    MP_PORT_RESTART_PARAMETERS  restartParameters;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Port Restart Started\n"));

    // Populate the parameters to pass to the handler
    NdisZeroMemory(&restartParameters, sizeof(MP_PORT_RESTART_PARAMETERS));
    restartParameters.PortToRestart = PortToRestart;
    NdisInitializeEvent(&restartParameters.CompleteEvent);
    NdisResetEvent(&restartParameters.CompleteEvent);

    // The lock is acquired here since the VNIC expects us to have only
    // one PNP operation requesting exclusive access from it
    PORT_ACQUIRE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    // get exclusive access
    ndisStatus = HelperPortRequestExclusiveAccess(helperPort, 
        HelperPortRestartPortExAccessCallback, 
        &restartParameters,
        TRUE
        );
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // the function completed synchronously, call the callback ourselves
        HelperPortRestartPortExAccessCallback(Port, &restartParameters);
    }
    else if (ndisStatus == NDIS_STATUS_PENDING)
    {
        // Will be processed asynchronously
        ndisStatus = NDIS_STATUS_SUCCESS;
    }

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // wait for the restart to complete
        NdisWaitEvent(&restartParameters.CompleteEvent, 0);

        // release exclusive access
        HelperPortReleaseExclusiveAccess(helperPort);
    }
    else    // Exclusive access request failed
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Exclusive access request for Port Restart failed with status 0x%08x\n", ndisStatus));
    }

    // Exclusive access is released before this is released
    PORT_RELEASE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Port Restart Finished\n"));

    return ndisStatus;    
}


NDIS_STATUS
HelperPortPortTerminateExAccessCallback(PMP_PORT Port, PVOID Ctx)
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_TERMINATE_PORT_PARAMETERS terminateParams = (PMP_TERMINATE_PORT_PARAMETERS) Ctx;

    HelperPortExclusiveAccessGranted(helperPort);

    // Ask the HVL to active the specific port. This is
    // a synchronous private API call into the HVL
    HelperPortDelegateExclusiveAccess(helperPort, terminateParams->PortToTerminate);

    // Invoke port terminate
    Port11TerminatePort(terminateParams->PortToTerminate, TRUE);

    // Reobtain active context
    HelperPortDelegateExclusiveAccess(helperPort, HELPPORT_GET_MP_PORT(helperPort));

    NdisSetEvent(&terminateParams->CompleteEvent);

    return NDIS_STATUS_SUCCESS;
}


VOID
HelperPortHandlePortTerminate(
    _In_ PMP_PORT                 Port,
    _In_ PMP_PORT                 PortToTerminate
    )
{
    NDIS_STATUS                 ndisStatus;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    MP_TERMINATE_PORT_PARAMETERS    terminateParams;
    
    NdisZeroMemory(&terminateParams, sizeof(MP_TERMINATE_PORT_PARAMETERS));
    terminateParams.PortToTerminate = PortToTerminate;
    NdisInitializeEvent(&terminateParams.CompleteEvent);
    NdisResetEvent(&terminateParams.CompleteEvent);

    // The lock is acquired here since the VNIC expects us to have only
    // one PNP operation requesting exclusive access from it. Note that
    // this is acquired after we call HwNdisReset
    PORT_ACQUIRE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));        
        
    // Now queue the exclusive access operation
    ndisStatus = HelperPortRequestExclusiveAccess(helperPort, 
        HelperPortPortTerminateExAccessCallback, 
        &terminateParams,
        TRUE
        );
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS || ndisStatus == NDIS_STATUS_PENDING);

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // the function completed synchronously, call the callback ourselves
        HelperPortPortTerminateExAccessCallback(Port, &terminateParams);
    }
    else if (ndisStatus == NDIS_STATUS_PENDING)
    {
        ndisStatus = NDIS_STATUS_SUCCESS;
    }

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // wait for the terminate to complete
        NdisWaitEvent(&terminateParams.CompleteEvent, 0);

        // release exclusive access
        HelperPortReleaseExclusiveAccess(helperPort);    
    }
    else    // Exclusive access request failed
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Exclusive access request for Port Terminate failed with status 0x%08x\n", ndisStatus));
    }

    // Exclusive access is released before this is released
    PORT_RELEASE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));
    
}


NDIS_STATUS 
HelperPortNdisReset(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PBOOLEAN                AddressingReset
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS, globalStatus = NDIS_STATUS_SUCCESS;
    PADAPTER                    adapter = HelperPort->ParentPort->Adapter;
    ULONG                       i;

    MpEntry;

    // Even if reset fails anywhere, we complete the full process
    
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Helper Port Reset\n"));        

    for (i = 0; i < adapter->NumberOfPorts; i++)
    {
        // Ask the HVL to active the specific port. This is
        // a synchronous private API call into the HVL
        HelperPortDelegateExclusiveAccess(HelperPort, adapter->PortList[i]);

        // Notify the port of reset start
        ndisStatus = Port11NdisResetStart(adapter->PortList[i]);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            globalStatus = ndisStatus;
        }
    }

    // Reobtain active context
    HelperPortDelegateExclusiveAccess(HelperPort, HELPPORT_GET_MP_PORT(HelperPort));

    //
    // Reset the hardware to cleanup stuff
    //
    Hw11NdisResetStep2(adapter->Hw);

    //
    // Restore HW state
    //
    ndisStatus = Hw11NdisResetStep3(adapter->Hw, AddressingReset);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        globalStatus = ndisStatus;
    }

    //
    // Now "un-reset" all the ports
    //
    for (i = 0; i < adapter->NumberOfPorts; i++)
    {
        // Notify the port of reset finish
        ndisStatus = Port11NdisResetEnd(adapter->PortList[i]);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            globalStatus = ndisStatus;
        }
    }
    
    MpExit;
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Helper Port Reset\n"));        

    return globalStatus;
}

NDIS_STATUS
HelperPortResetExAccessCallback(PMP_PORT Port, PVOID Ctx)
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_NDIS_RESET_PARAMETERS   resetParams = (PMP_NDIS_RESET_PARAMETERS) Ctx;

    HelperPortExclusiveAccessGranted(helperPort);
    
    resetParams->ResetStatus = HelperPortNdisReset(helperPort, resetParams->AddressingReset);

    NdisSetEvent(&resetParams->CompleteEvent);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HelperPortHandleMiniportReset(
    _In_  PMP_PORT                Port,
    _Inout_ PBOOLEAN              AddressingReset
    )
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PADAPTER                    adapter = Port->Adapter;
    MP_NDIS_RESET_PARAMETERS    resetParams;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Miniport Reset Started\n"));
    
    NdisZeroMemory(&resetParams, sizeof(MP_NDIS_RESET_PARAMETERS));
    resetParams.AddressingReset = AddressingReset;
    NdisInitializeEvent(&resetParams.CompleteEvent);
    NdisResetEvent(&resetParams.CompleteEvent);
    
    // The first thing we do on a miniport reset is reset the
    // hardware. This would ensure that the hardware is in a good state
    // and hvl, etc wont get stuck waiting for some hardware operation
    // before giving the reset exclusive access    
    Hw11NdisResetStep1(adapter->Hw);

    // The lock is acquired here since the VNIC expects us to have only
    // one PNP operation requesting exclusive access from it. Note that
    // this is acquired after we call HwNdisReset
    PORT_ACQUIRE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));        
    
    // Now queue the exclusive access operation
    ndisStatus = HelperPortRequestExclusiveAccess(helperPort, 
        HelperPortResetExAccessCallback, 
        &resetParams,
        TRUE
        );
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // the function completed synchronously, call the callback ourselves
        HelperPortResetExAccessCallback(Port, &resetParams);
    }
    else if (ndisStatus == NDIS_STATUS_PENDING)
    {
        // Will be processed asynchronously
        ndisStatus = NDIS_STATUS_SUCCESS;
    }


    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // wait for the reset to complete
        NdisWaitEvent(&resetParams.CompleteEvent, 0);

        // release exclusive access
        HelperPortReleaseExclusiveAccess(helperPort);
    }
    else    // Exclusive access request failed
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Exclusive access request for Miniport Reset failed with status 0x%08x\n", ndisStatus));        
    }

    // Exclusive access is released before this is released
    PORT_RELEASE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Miniport Reset Finished\n"));
    
    return resetParams.ResetStatus;

}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
HelperPortAllocateNdisPort(
    _In_  PMP_PORT                HelperPort,
    _Out_ PNDIS_PORT_NUMBER       AllocatedPortNumber
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_PORT_CHARACTERISTICS   portChar;

    // Call NDIS to allocate the port
    NdisZeroMemory(&portChar, sizeof(NDIS_PORT_CHARACTERISTICS));

    MP_ASSIGN_NDIS_OBJECT_HEADER(portChar.Header, NDIS_OBJECT_TYPE_DEFAULT,
        NDIS_PORT_CHARACTERISTICS_REVISION_1, sizeof(NDIS_PORT_CHARACTERISTICS));

    portChar.Flags = NDIS_PORT_CHAR_USE_DEFAULT_AUTH_SETTINGS;
    portChar.Type = NdisPortTypeUndefined;
    portChar.MediaConnectState = MediaConnectStateConnected;
    portChar.XmitLinkSpeed = NDIS_LINK_SPEED_UNKNOWN;
    portChar.RcvLinkSpeed = NDIS_LINK_SPEED_UNKNOWN;
    portChar.Direction = NET_IF_DIRECTION_SENDRECEIVE;
    portChar.SendControlState = NdisPortControlStateUnknown;
    portChar.RcvControlState = NdisPortControlStateUnknown;
    portChar.SendAuthorizationState = NdisPortControlStateUncontrolled; // Ignored
    portChar.RcvAuthorizationState = NdisPortControlStateUncontrolled; // Ignored
               
    ndisStatus = NdisMAllocatePort(HelperPort->MiniportAdapterHandle, &portChar);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate NDIS port. Status = 0x%08x\n",
            ndisStatus));
    }
    else
    {
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Associated Port Number %d with allocated port\n", 
                    portChar.PortNumber));
                    
        // Return the NDIS port number that has been allocated to this port
        *AllocatedPortNumber = portChar.PortNumber;
    }
    
    return ndisStatus;
}

VOID
HelperPortFreeNdisPort(
    _In_  PMP_PORT                HelperPort,
    _In_  NDIS_PORT_NUMBER        PortNumberToFree
    )
{
    NDIS_STATUS                 ndisStatus;

    // Free the NDIS port
    ndisStatus = NdisMFreePort(HelperPort->MiniportAdapterHandle, PortNumberToFree);
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    _Analysis_assume_( ndisStatus == NDIS_STATUS_SUCCESS );

}

NDIS_STATUS
HelperPortActivateNdisPort(
    _In_  PMP_PORT                HelperPort,
    _In_  NDIS_PORT_NUMBER        PortNumberToActivate
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NET_PNP_EVENT_NOTIFICATION  netPnpEventNotification;
    NDIS_PORT                   ndisPort;
    PNDIS_PORT_CHARACTERISTICS  portChar;

    NdisZeroMemory(&netPnpEventNotification, sizeof(NET_PNP_EVENT_NOTIFICATION));
    NdisZeroMemory(&ndisPort, sizeof(NDIS_PORT));

    MP_ASSIGN_NDIS_OBJECT_HEADER(netPnpEventNotification.Header, NDIS_OBJECT_TYPE_DEFAULT,
        NET_PNP_EVENT_NOTIFICATION_REVISION_1, sizeof(NET_PNP_EVENT_NOTIFICATION));

    netPnpEventNotification.NetPnPEvent.NetEvent = NetEventPortActivation;

    // Refill the characteristics structure for the port
    portChar = &(ndisPort.PortCharacteristics);
    MP_ASSIGN_NDIS_OBJECT_HEADER(portChar->Header, NDIS_OBJECT_TYPE_DEFAULT,
        NDIS_PORT_CHARACTERISTICS_REVISION_1, sizeof(NDIS_PORT_CHARACTERISTICS));

    portChar->Flags = NDIS_PORT_CHAR_USE_DEFAULT_AUTH_SETTINGS;
    portChar->Type = NdisPortTypeUndefined;
    portChar->MediaConnectState = MediaConnectStateConnected;
    portChar->XmitLinkSpeed = NDIS_LINK_SPEED_UNKNOWN;
    portChar->RcvLinkSpeed = NDIS_LINK_SPEED_UNKNOWN;
    portChar->Direction = NET_IF_DIRECTION_SENDRECEIVE;
    portChar->SendControlState = NdisPortControlStateUnknown;
    portChar->RcvControlState = NdisPortControlStateUnknown;
    portChar->SendAuthorizationState = NdisPortControlStateUncontrolled; // Ignored
    portChar->RcvAuthorizationState = NdisPortControlStateUncontrolled; // Ignored
    portChar->PortNumber = PortNumberToActivate;
    
    // Single port is being activated
    ndisPort.Next = NULL;

    // We need to save a pointer to the NDIS_PORT in the NetPnPEvent::Buffer field
    netPnpEventNotification.NetPnPEvent.Buffer = (PVOID)&ndisPort;
    netPnpEventNotification.NetPnPEvent.BufferLength = sizeof(NDIS_PORT);

    ndisStatus = NdisMNetPnPEvent(HelperPort->MiniportAdapterHandle, &netPnpEventNotification);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to activate NDIS port %d. Status = 0x%08x\n", 
            PortNumberToActivate, ndisStatus));
    }
    else
    {
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Activated Port Number %d\n", PortNumberToActivate));
    } 

    return ndisStatus;
}

VOID
HelperPortDeactivateNdisPort(
    _In_  PMP_PORT                HelperPort,
    _In_  NDIS_PORT_NUMBER        PortNumberToDeactivate
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NET_PNP_EVENT_NOTIFICATION  netPnpEventNotification;
    NDIS_PORT_NUMBER            portNumberArray[1];

    NdisZeroMemory(&netPnpEventNotification, sizeof(NET_PNP_EVENT_NOTIFICATION));

    MP_ASSIGN_NDIS_OBJECT_HEADER(netPnpEventNotification.Header, NDIS_OBJECT_TYPE_DEFAULT,
        NET_PNP_EVENT_NOTIFICATION_REVISION_1, sizeof(NET_PNP_EVENT_NOTIFICATION));

    netPnpEventNotification.NetPnPEvent.NetEvent = NetEventPortDeactivation;

    // We need to save a pointer to the NDIS_PORT_NUMBER in the NetPnPEvent::Buffer field
    portNumberArray[0] = PortNumberToDeactivate;            
    netPnpEventNotification.NetPnPEvent.Buffer = (PVOID)portNumberArray;
    netPnpEventNotification.NetPnPEvent.BufferLength = sizeof(NDIS_PORT_NUMBER);

    ndisStatus = NdisMNetPnPEvent(HelperPort->MiniportAdapterHandle, &netPnpEventNotification);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to deactivate NDIS port %d. Status = 0x%08x\n", 
            PortNumberToDeactivate, ndisStatus));
    }
    else
    {
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Deactivated Port Number %d\n", PortNumberToDeactivate));
    }        
}


NDIS_STATUS
HelperPortCreateMacHandler(
    _In_ PMP_PORT                 Port,
    _In_ PNDIS_OID_REQUEST        OidRequest
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_PORT                    newPort = NULL;
    BOOLEAN                     portInit = FALSE, portInserted = FALSE;
    BOOLEAN                     miniportPaused = FALSE;
    BOOLEAN                     ndisPortAllocated = FALSE, ndisPortActivated = FALSE;
    PDOT11_MAC_INFO             dot11MacInfo = NULL;
    PDOT11_MAC_ADDRESS          macAddr = NULL;
    NDIS_PORT_NUMBER            portNumber = DEFAULT_NDIS_PORT_NUMBER;
    
    do
    {
        OidRequest->DATA.METHOD_INFORMATION.BytesWritten = 0;
        OidRequest->DATA.METHOD_INFORMATION.BytesRead = 0;
        OidRequest->DATA.METHOD_INFORMATION.BytesNeeded = 0;


        /*
            Pause the miniport before we modify the adapter's port list. 
            This is an expensive way to achieve this but it is simple to implement
            */
    
        ndisStatus = HelperPortHandleMiniportPause(Port, NULL);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HelperPortHandleMiniportPause failed 0x%x\n", ndisStatus));
            break;
        }        
        miniportPaused = TRUE;

        // By default, our ports are EXTSTA_PORTs only. 
        ndisStatus = Port11AllocatePort(Port->Adapter, 
                        EXTSTA_PORT, 
                        &newPort
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate a new port\n"));
            break;
        }

        //
        // We will allocate an NDIS port corresponding to this port
        //
        ndisStatus = HelperPortAllocateNdisPort(Port, &portNumber);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        newPort->PortNumber = portNumber;
        ndisPortAllocated = TRUE;

        // Initialize the port
        ndisStatus = Port11InitializePort(newPort, 
                        Port->Adapter->Hvl, 
                        Port->Adapter->PortRegInfo
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        portInit = TRUE;
        
        // Add it to the adapter list
        Port->Adapter->PortList[Port->Adapter->NumberOfPorts] = newPort;
        Port->Adapter->NumberOfPorts++;
        portInserted = TRUE;

        /*
            Restart so that normal operation may continue
            */
        miniportPaused = FALSE; // We clear this before we attempt to restart
        ndisStatus = HelperPortHandleMiniportRestart(Port, NULL);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("HelperPortHandleMiniportRestart failed 0x%x", ndisStatus));
            break;
        } 
        
        /*
            Done creating the new virtual adapter
            */
        dot11MacInfo = (PDOT11_MAC_INFO)OidRequest->DATA.METHOD_INFORMATION.InformationBuffer;
        dot11MacInfo->uNdisPortNumber = newPort->PortNumber;
        macAddr = VNic11QueryMACAddress(PORT_GET_VNIC(newPort));
        
        NdisMoveMemory(dot11MacInfo->MacAddr, macAddr, sizeof(DOT11_MAC_ADDRESS));
        
        OidRequest->DATA.METHOD_INFORMATION.BytesWritten = sizeof(DOT11_MAC_INFO);

        MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Created a new Port \n" ));
    } while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newPort != NULL)
        {
            if (portInserted)
            {
                Port11RemovePortFromAdapterList(newPort);
            }
            
            if (portInit)
            {
                HelperPortHandlePortTerminate(Port, newPort);
            }
            
            Port11FreePort(newPort, TRUE);            
        }

        if (miniportPaused)
        {
            if (HelperPortHandleMiniportRestart(Port, NULL) != NDIS_STATUS_SUCCESS)
            {
                MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("HelperPortHandleMiniportRestart failed"));
            }
        }
    }

    //
    // Complete the OID
    //
    Port11CompletePendingOidRequest(Port, ndisStatus);

    //
    // We can now activate/deactivate the port. We cannot do this while we are processing
    // the OID, else the OS would deadlock. 
    //
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // If this port has been allocated with NDIS, activate it. This notification
        // goes upto the OS and it would handle the request
        ndisStatus = HelperPortActivateNdisPort(Port, portNumber);
        MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    }
    else
    {
        if (IS_ALLOCATED_PORT_NUMBER(portNumber))
        {
            // If activated, deactivate the NDIS port
            if (ndisPortActivated)
            {
                HelperPortDeactivateNdisPort(Port, portNumber);
            }

            // If allocated free the NDIS port
            if (ndisPortAllocated)
            {
                HelperPortFreeNdisPort(Port, portNumber);
            }
        }
    }
    
    return ndisStatus;
}

NDIS_STATUS
HelperPortCreateMac(
    _In_ PMP_PORT                 Port,
    _In_ PVOID                    NdisOidRequest,
    _Out_ BOOLEAN                 *pfOidCompleted
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_OID_REQUEST           oidRequest = (PNDIS_OID_REQUEST)NdisOidRequest;

    *pfOidCompleted = FALSE;
    
    do
    {
        oidRequest->DATA.METHOD_INFORMATION.BytesWritten = 0;
        oidRequest->DATA.METHOD_INFORMATION.BytesRead = 0;
        oidRequest->DATA.METHOD_INFORMATION.BytesNeeded = 0;

        if (oidRequest->RequestType != NdisRequestMethod)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Invalid request type %d for OID_DOT11_CREATE_MAC\n", 
                oidRequest->RequestType));
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
        }
    
        if (oidRequest->DATA.METHOD_INFORMATION.OutputBufferLength < sizeof(DOT11_MAC_INFO))
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("The buffer being passed into OID_DOT11_CREATE_MAC is too small(%d)\n", 
                oidRequest->DATA.METHOD_INFORMATION.OutputBufferLength));
            oidRequest->DATA.METHOD_INFORMATION.BytesNeeded = sizeof(DOT11_MAC_INFO);
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        //
        // Since OID calls are serialized, we do not expect the NumberOfPorts to change
        // while we are checking the following until this OID is completed. So we do not need 
        // to protect the NumberOfPorts in any way
        //
        if (Port->Adapter->NumberOfPorts >= MP_MAX_NUMBER_OF_PORT)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Number of existing ports exceed max supported. Failing new port creation\n"));            
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
        }

        ndisStatus = HelperPortCreateMacHandler(Port, oidRequest);
        // regardless of the return result, HelperPortCreateMacHandler always completes the Oid
        *pfOidCompleted = TRUE;
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("HelperPortCreateMacHandler failed. Status = 0x%08x\n", ndisStatus));
            break;            
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
HelperPortDeleteMacHandler(
    _In_ PMP_PORT                 Port,
    _In_ PVOID                    NdisOidRequest
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_OID_REQUEST           oidRequest = (PNDIS_OID_REQUEST)NdisOidRequest;
    PDOT11_MAC_INFO             dot11MacInfo = NULL;
    PMP_PORT                    destinationPort = NULL;
    NDIS_PORT_NUMBER            portNumber;
    BOOLEAN                     destroyPortNumber = FALSE;
    
    do
    {
        dot11MacInfo = (PDOT11_MAC_INFO)oidRequest->DATA.SET_INFORMATION.InformationBuffer;
        portNumber = dot11MacInfo->uNdisPortNumber;
        
        //
        // First we would need to translate from the NDIS_PORT_NUMBER
        // to our port structure. This is done by walking the PortList
        // Since OID calls are serialized, we do not expect the Portlist to change
        // while we are trying to find the port or for the port to get deleted
        // until this OID is completed. So we do not need to protect the Port/PortList
        // in any way
        //
        destinationPort = Port11TranslatePortNumberToPort(
                            Port->Adapter, 
                            portNumber
                            );
        if (destinationPort == NULL)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Unable to find Port corresponding to PortNumber %d\n", 
                dot11MacInfo->uNdisPortNumber));

            ndisStatus = NDIS_STATUS_INVALID_PORT;
            break;
        }

        if (IS_ALLOCATED_PORT_NUMBER(portNumber))
        {
            // This port has been allocate with NDIS. When we are done, delete the
            // port
            destroyPortNumber = TRUE;
        }

        /*
            Pause the miniport before we modify the adapter's port list. 
            This is an expensive way to achieve this but it is simple to implement
            */
    
        ndisStatus = HelperPortHandleMiniportPause(Port, NULL);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HelperPortHandleMiniportPause failed 0x%x\n", ndisStatus));
            break;
        }        

        // remove it from the adapter list
        Port11RemovePortFromAdapterList(destinationPort);

        // now free up this port
        HelperPortHandlePortTerminate(Port, destinationPort);
        Port11FreePort(destinationPort, TRUE);

        /*
            Restart so that normal operation may continue
            */
        ndisStatus = HelperPortHandleMiniportRestart(Port, NULL);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("HelperPortHandleMiniportRestart failed 0x%x", ndisStatus));
            break;
        }        

        /*
            Done deleting the mac state
            */
        oidRequest->DATA.SET_INFORMATION.BytesRead = sizeof(DOT11_MAC_INFO);

        MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Deleted the Mac with port number %d", dot11MacInfo->uNdisPortNumber));
    } while (FALSE);

    //
    // Complete the OID
    //
    Port11CompletePendingOidRequest(Port, ndisStatus);

    //
    // We can now deactivate the port. We cannot do this while we are processing
    // the OID, else the OS would deadlock. 
    //
    if ((ndisStatus == NDIS_STATUS_SUCCESS) && (destroyPortNumber == TRUE))
    {
        // Deactivate & free the NDIS_PORT
        HelperPortDeactivateNdisPort(Port, portNumber);
        HelperPortFreeNdisPort(Port, portNumber);
    }
    
    return ndisStatus;
}


NDIS_STATUS
HelperPortDeleteMac(
    _In_ PMP_PORT                 Port,
    _In_ PNDIS_OID_REQUEST        OidRequest,
    _Out_ BOOLEAN                 *pfOidCompleted
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MAC_INFO             dot11MacInfo = NULL;

    *pfOidCompleted = FALSE;
    
    do
    {
        OidRequest->DATA.SET_INFORMATION.BytesRead = 0;
        OidRequest->DATA.SET_INFORMATION.BytesNeeded = 0;

        if (OidRequest->RequestType != NdisRequestSetInformation)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Invalid request type %d for OID_DOT11_DELETE_MAC\n", 
                OidRequest->RequestType));
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
        }

        if (OidRequest->DATA.SET_INFORMATION.InformationBufferLength < sizeof(DOT11_MAC_INFO))
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("The buffer being passed into OID_DOT11_DELETE_MAC is too small(%d)", 
                OidRequest->DATA.SET_INFORMATION.InformationBufferLength));
            OidRequest->DATA.SET_INFORMATION.BytesNeeded = sizeof(DOT11_MAC_INFO);
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        dot11MacInfo = (PDOT11_MAC_INFO)OidRequest->DATA.SET_INFORMATION.InformationBuffer;

        if (dot11MacInfo->uNdisPortNumber > MP_MAX_NUMBER_OF_PORT ||
          dot11MacInfo->uNdisPortNumber == DEFAULT_NDIS_PORT_NUMBER ||
          dot11MacInfo->uNdisPortNumber == HELPER_PORT_PORT_NUMBER)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("The port number (%d) being passed in is invalid", dot11MacInfo->uNdisPortNumber));
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        ndisStatus = HelperPortDeleteMacHandler(Port, OidRequest);

        // regardless of the return result, HelperPortCreateMacHandler always completes the Oid
        *pfOidCompleted = TRUE;
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("HelperPortDeleteMacHandler failed. Status = 0x%08x\n", ndisStatus));
            break;            
        }
    }while (FALSE);

    return ndisStatus;
}



VOID
HelperPortHandleMacCleanup(
    _In_ PMP_PORT                 HelperPort
    )
{
    PMP_PORT                    destinationPort = NULL;
    NDIS_PORT_NUMBER            portNumber;
    BOOLEAN                     destroyPortNumber = FALSE;

    // The only thing we do here is delete any port that was created on an OID
    // but the OS did not delete.

    // Port at index 0 is auto created. All others are manually created
    while (HelperPort->Adapter->NumberOfPorts > 1)
    {
        // The one we want to delete is always at index 1 since
        // Port11RemovePortFromAdapterList moves ports around
        destinationPort = HelperPort->Adapter->PortList[1];
        portNumber = destinationPort->PortNumber;

        destroyPortNumber = FALSE;
        if (IS_ALLOCATED_PORT_NUMBER(portNumber))
        {
            // This port has been allocate with NDIS. When we are done, delete the
            // port
            destroyPortNumber = TRUE;
        }

        // remove it from the adapter list. This also decrements
        // Adapter->NumberOfPorts
        Port11RemovePortFromAdapterList(destinationPort);

        // now free up this port
        HelperPortHandlePortTerminate(HelperPort, destinationPort);
        Port11FreePort(destinationPort, TRUE);

        if (destroyPortNumber == TRUE)
        {
            // Deactivate & free the NDIS_PORT
            HelperPortDeactivateNdisPort(HelperPort, portNumber);
            HelperPortFreeNdisPort(HelperPort, portNumber);
        }

    }

}


NDIS_STATUS
HelperPortHandleQueryPower(
    _In_ PMP_PORT                 Port,
    _Inout_ PVOID                InformationBuffer,
    _In_ ULONG                    InformationBufferLength,
    _Inout_ PULONG               BytesWritten,
    _Inout_ PULONG               BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_DEVICE_POWER_STATE     devicePowerState;

    do
    {
        *BytesNeeded = sizeof(NDIS_DEVICE_POWER_STATE);
        if (InformationBufferLength < *BytesNeeded)
        {
            //
            // Too little data. Bail out.
            //
            MpTrace(COMP_POWER, DBG_SERIOUS,  ("Bad length received for the QUERY_POWER request\n"));
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }
        
        NdisMoveMemory(&devicePowerState, InformationBuffer, sizeof(NDIS_DEVICE_POWER_STATE));
        
        if (devicePowerState <= NdisDeviceStateUnspecified || devicePowerState >= NdisDeviceStateMaximum)
        {
            //
            // An invalid device state has been specified. Bail out.
            //
            MpTrace(COMP_POWER, DBG_SERIOUS,  ("Invalid data received in QUERY_POWER request\n"));
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        *BytesWritten = 0;

        // This request gets forwarded to the HW
        ndisStatus = Hw11CanTransitionPower(Port->Adapter->Hw, devicePowerState);
    } while(FALSE);

    return ndisStatus;
}


NDIS_STATUS 
HelperPortSetPower(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  NDIS_DEVICE_POWER_STATE NewDeviceState
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PADAPTER                    adapter = HelperPort->ParentPort->Adapter;
    ULONG                       i;

    //
    // If we are going to sleep, notify the powers about the new power state
    // now before we do stuff on the hardware
    //
    if (NewDeviceState != NdisDeviceStateD0)
    {
        for (i = 0; i < adapter->NumberOfPorts; i++)
        {
            // Activate the ports since they may want to do stuff like
            // StopBSS, etc
            HelperPortDelegateExclusiveAccess(HelperPort, adapter->PortList[i]);
                        
            // Notify the port of power state
            ndisStatus = Port11SetPower(adapter->PortList[i], NewDeviceState);
            
            MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
        }

        // Reobtain active context
        HelperPortDelegateExclusiveAccess(HelperPort, HELPPORT_GET_MP_PORT(HelperPort));
    }

    //
    // The current DevicePowerState has been set to NewDeviceState before
    // calling the hardware intentionally. The call to Hw11SetPower
    // cannot fail. Before the call returns, Interrupts may be enabled for this
    // device (when going to D0)
    //
    HelperPort->DevicePowerState = NewDeviceState;

    //
    // Change hardware power state
    //
    ndisStatus = Hw11SetPower(adapter->Hw, NewDeviceState);

    //
    // If we are waking up from sleep, notify the powers about the new power state
    // now that the hardware is ready
    //
    if (NewDeviceState == NdisDeviceStateD0)
    {
        for (i = 0; i < adapter->NumberOfPorts; i++)
        {
            // We dont activate the ports here. There is no real
            // point since the miniport is anyways pause & ports cannot
            // do anything yet
            
            // Notify the port of power state
            ndisStatus = Port11SetPower(adapter->PortList[i], NewDeviceState);
            
            MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
        }
    }
    
    return ndisStatus;
}

NDIS_STATUS
HelperPortSetPowerExAccessCallback(PMP_PORT Port, PVOID Ctx)
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_POWER_PARAMETERS        powerParameters = (PMP_POWER_PARAMETERS)Ctx;

    HelperPortExclusiveAccessGranted(helperPort);
    HelperPortSetPower(helperPort, powerParameters->NewDeviceState);

    NdisSetEvent(&powerParameters->CompleteEvent);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HelperPortHandleSetPower(
    _In_ PMP_PORT                 Port,
    _Inout_ PVOID                InformationBuffer,
    _In_ ULONG                    InformationBufferLength,
    _Inout_ PULONG               BytesRead,
    _Inout_ PULONG               BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_DEVICE_POWER_STATE     newDeviceState;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    MP_POWER_PARAMETERS         powerParameters;

    do
    {
        //
        // Store new power state and succeed the request.
        //
        *BytesNeeded = sizeof(NDIS_DEVICE_POWER_STATE);
        if (InformationBufferLength < *BytesNeeded)
        {
            //
            // Too little data. Bail out.
            //
            MpTrace(COMP_POWER, DBG_SERIOUS,  ("Bad length received for the SET_POWER request\n"));
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }
        
        NdisMoveMemory(&newDeviceState, InformationBuffer, sizeof(NDIS_DEVICE_POWER_STATE));
        *BytesRead = sizeof(NDIS_DEVICE_POWER_STATE);
        
        if (newDeviceState <= NdisDeviceStateUnspecified || newDeviceState >= NdisDeviceStateMaximum)
        {
            //
            // An invalid device state has been specified. Bail out.
            //
            MpTrace(COMP_POWER, DBG_SERIOUS,  ("Invalid data received in SET_POWER request\n"));
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        if (newDeviceState == helperPort->DevicePowerState)
        {
            //
            // We are already in the device state being set. Succeed the call.
            //
            MpTrace(COMP_POWER, DBG_NORMAL,  ("Device is already in power state D%d\n", newDeviceState));
            ndisStatus = NDIS_STATUS_SUCCESS;
            break;
        }

        // Populate the parameters to pass to the handler
        NdisZeroMemory(&powerParameters, sizeof(MP_POWER_PARAMETERS));
        powerParameters.NewDeviceState = newDeviceState;
        NdisInitializeEvent(&powerParameters.CompleteEvent);
        NdisResetEvent(&powerParameters.CompleteEvent);

        PORT_ACQUIRE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

        //
        // Request exclusive access. Since this is called after a pause,
        // in most cases we would already own the exclusive access
        //
        ndisStatus = HelperPortRequestExclusiveAccess(helperPort, 
            HelperPortSetPowerExAccessCallback, 
            &powerParameters,
            TRUE
            );
        
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            // the function completed synchronously, call the callback ourselves
            HelperPortSetPowerExAccessCallback(Port, &powerParameters);
        }
        else if (ndisStatus == NDIS_STATUS_PENDING)
        {
            // Will be processed asynchronously
            ndisStatus = NDIS_STATUS_SUCCESS;
        }

        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            // wait for the power event to complete
            NdisWaitEvent(&powerParameters.CompleteEvent, 0);

            // release exclusive access
            HelperPortReleaseExclusiveAccess(helperPort);
        }
        else    // Exclusive access request failed
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Exclusive access request for Set Power failed with status 0x%08x\n", ndisStatus));        
        }

        PORT_RELEASE_PNP_MUTEX(HELPPORT_GET_MP_PORT(helperPort));

        // We must succeed the power request
        ndisStatus = NDIS_STATUS_SUCCESS;
    } while(FALSE);

    return ndisStatus;
}

NDIS_STATUS
HelperPortHandleOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest,
    _Out_ BOOLEAN                 *pfOidCompleted
    )
{
    NDIS_OID                    oid;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
    
    oid = NdisOidRequest->DATA.QUERY_INFORMATION.Oid; // Oid is at same offset for all RequestTypes

    // Hold this in the pending OID request structure
    Port->PendingOidRequest = NdisOidRequest;

    *pfOidCompleted = FALSE;

    switch (oid)
    {
        case OID_DOT11_CREATE_MAC:
            {
                ndisStatus = HelperPortCreateMac(Port, NdisOidRequest, pfOidCompleted);                
            }
            break;
                
        case OID_DOT11_DELETE_MAC:
            {
                ndisStatus = HelperPortDeleteMac(Port, NdisOidRequest, pfOidCompleted);                
            }                            
            break;


        case OID_PNP_QUERY_POWER:
            if (NdisOidRequest->RequestType != NdisRequestQueryInformation)
            {
                MpTrace(COMP_POWER, DBG_SERIOUS, ("Invalid request type %d for OID_PNP_QUERY_POWER\n", 
                    NdisOidRequest->RequestType));
            }
            else
            {
                ndisStatus = HelperPortHandleQueryPower(
                                Port,
                                NdisOidRequest->DATA.QUERY_INFORMATION.InformationBuffer,
                                NdisOidRequest->DATA.QUERY_INFORMATION.InformationBufferLength,
                                (PULONG)&NdisOidRequest->DATA.QUERY_INFORMATION.BytesWritten,
                                (PULONG)&NdisOidRequest->DATA.QUERY_INFORMATION.BytesNeeded
                                );
            }
            break;

        case OID_PNP_SET_POWER:
            if (NdisOidRequest->RequestType != NdisRequestSetInformation)
            {
                MpTrace(COMP_POWER, DBG_SERIOUS, ("Invalid request type %d for OID_PNP_SET_POWER\n", 
                    NdisOidRequest->RequestType));
            }
            else
            {
                ndisStatus = HelperPortHandleSetPower(
                                Port,
                                NdisOidRequest->DATA.SET_INFORMATION.InformationBuffer,
                                NdisOidRequest->DATA.SET_INFORMATION.InformationBufferLength,
                                (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesRead,
                                (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesNeeded
                                );
            }
            break;
        default:
            MPASSERT(FALSE);
    };

    if ( (FALSE == *pfOidCompleted) && (NDIS_STATUS_PENDING != ndisStatus))
    {
        // No OID requests pending
        Port->PendingOidRequest = NULL;
    }
    
    return ndisStatus;
}



