/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_main.c

Abstract:
    Implements the PNP handling for the Station layer layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "st_aplst.h"
#include "st_conn.h"
#include "st_adhoc.h"
#include "st_scan.h"
#include "st_oids.h"

#if DOT11_TRACE_ENABLED
#include "st_main.tmh"
#endif

MP_REG_ENTRY STARegTable[] = {

    {
        NDIS_STRING_CONST("AdhocStationMaxCount"), 
        NdisParameterInteger,
        0,
        FIELD_OFFSET(STA_REG_INFO, AdhocStationMaxCount),
        sizeof(ULONG),
        STA_ADHOC_STA_MAX_ENTRIES_DEFAULT,
        STA_ADHOC_STA_MAX_ENTRIES_MIN,
        STA_ADHOC_STA_MAX_ENTRIES_MAX 
    },

    {
        NDIS_STRING_CONST("BSSEntryExpireTime"), 
        NdisParameterInteger,
        0,
        FIELD_OFFSET(STA_REG_INFO, BSSEntryExpireTime),
        sizeof(ULONG),
        STA_BSS_ENTRY_EXPIRE_TIME_DEFAULT,
        STA_BSS_ENTRY_EXPIRE_TIME_MIN,
        STA_BSS_ENTRY_EXPIRE_TIME_MAX 
    },

    {
        NDIS_STRING_CONST("LostAPRoamBeaconCount"), 
        NdisParameterInteger,
        0,  // Field off set is 0
        FIELD_OFFSET(STA_REG_INFO, LostAPRoamBeaconCount),
        sizeof(ULONG),
        STA_INFRA_ROAM_NO_BEACON_COUNT_DEFAULT,
        STA_INFRA_ROAM_NO_BEACON_COUNT_MIN,
        STA_INFRA_ROAM_NO_BEACON_COUNT_MAX 
    },

    {
        NDIS_STRING_CONST("RSSIRoamBeaconCount"), 
        NdisParameterInteger,
        0,
        FIELD_OFFSET(STA_REG_INFO, RSSIRoamBeaconCount),
        sizeof(ULONG),
        STA_INFRA_RSSI_ROAM_BEACON_COUNT_DEFAULT,
        STA_INFRA_RSSI_ROAM_BEACON_COUNT_MIN,
        STA_INFRA_RSSI_ROAM_BEACON_COUNT_MAX
    }
};



#define STA_NUM_REG_PARAMS (sizeof (STARegTable) / sizeof(MP_REG_ENTRY))

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Sta11LoadRegistryInformation(
    _In_      NDIS_HANDLE             MiniportAdapterHandle,
    _In_opt_  NDIS_HANDLE             ConfigurationHandle,
    _Out_     PVOID*                  RegistryInformation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PSTA_REG_INFO               staRegInfo = NULL;

    *RegistryInformation = NULL;

    do
    {
        //
        // Allocate memory for the registry info
        //
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle, &staRegInfo, sizeof(STA_REG_INFO), EXTSTA_MEMORY_TAG);
        if (staRegInfo == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for STA_REG_INFO\n",
                                 sizeof(STA_REG_INFO)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(staRegInfo, sizeof(STA_REG_INFO));

        //
        // read registry values 
        //
        MpReadRegistry((PVOID)staRegInfo, ConfigurationHandle, STARegTable, STA_NUM_REG_PARAMS);

        *RegistryInformation = staRegInfo;
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Free the buffer
        Sta11FreeRegistryInformation(staRegInfo);
    }
    
    return ndisStatus;
}

VOID
Sta11FreeRegistryInformation(
    _In_opt_  PVOID                   RegistryInformation
    )
{
    if (RegistryInformation != NULL)
    {
        MP_FREE_MEMORY(RegistryInformation);
    }
}

NDIS_STATUS
Sta11AllocatePort(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_EXTSTA_PORT             newExtSTAPort = NULL;

    UNREFERENCED_PARAMETER(Adapter);
    
    do
    {

        // Allocate the helper port specific structure
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle, &newExtSTAPort, sizeof(MP_EXTSTA_PORT), EXTSTA_MEMORY_TAG);
        if (newExtSTAPort == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for extsta port\n",
                                 sizeof(MP_EXTSTA_PORT)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Clear everything
        NdisZeroMemory(newExtSTAPort, sizeof(MP_EXTSTA_PORT));

        // Save pointer to the parent port & vice versa
        Port->ChildPort = newExtSTAPort;
        newExtSTAPort->ParentPort = Port;

        // Set op mode and op state value
        Port->CurrentOpMode = DOT11_OPERATION_MODE_EXTENSIBLE_STATION;
        Port->CurrentOpState = INIT_STATE;

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newExtSTAPort != NULL)
        {
            Sta11FreePort(Port);
        }
    }

    return ndisStatus;
}


VOID
Sta11FreePort(
    _In_  PMP_PORT                Port
    )
{
    PMP_EXTSTA_PORT             extStaPort = MP_GET_STA_PORT(Port);
    
    // Cleanup anything we create for scan
    StaFreeScanContext(extStaPort);

    // Cleanup connection context
    StaFreeConnectionContext(extStaPort);

    // Free any memory allocated for Adhoc
    StaFreeAdHocStaInfo(extStaPort);

    MP_FREE_MEMORY(extStaPort);    
}

NDIS_STATUS
Sta11InitializePort(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   RegistryInformation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     freeAdhoc = FALSE, freeScanContext = FALSE;
    PMP_EXTSTA_PORT             extStaPort = MP_GET_STA_PORT(Port);

    do
    {
        // Save the registry value
        extStaPort->RegInfo = (PSTA_REG_INFO)RegistryInformation;
    
        // Setup default config
        StaInitializeStationConfig(extStaPort);
        
        ndisStatus = StaInitializeAdHocStaInfo(extStaPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        freeAdhoc = TRUE;

        ndisStatus = StaInitializeScanContext(extStaPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        freeScanContext = TRUE;

        ndisStatus = StaInitializeConnectionContext(extStaPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // Reset all the state
        StaResetStationConfig(extStaPort);
        
        //
        // Now that we are ready, setup the handlers for other data handlers
        //
        Port->RequestHandler = Sta11OidHandler;
        Port->SendEventHandler = Sta11SendEventHandler;
        Port->SendCompleteEventHandler = BasePortSendCompleteEventHandler;
        Port->ReceiveEventHandler = Sta11ReceiveEventHandler;
        Port->ReturnEventHandler = BasePortReturnEventHandler;
 
        
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (freeAdhoc)
        {
            if (freeScanContext)
            {
                StaFreeScanContext(extStaPort);
            }
            
            StaFreeAdHocStaInfo(extStaPort);
        }
    }

    return ndisStatus;
}

VOID
Sta11TerminatePort(
    _In_  PMP_PORT                Port
    )
{
    PMP_EXTSTA_PORT             extStaPort = MP_GET_STA_PORT(Port);

    //
    // We should have been paused and have already
    // stopped periodic scanning. Ensure that that is the case
    //
    MPASSERT(STA_TEST_SCAN_FLAG(extStaPort, STA_STOP_PERIODIC_SCAN));

    //
    // Cleanup all the connection state
    //
    StaResetConnection(extStaPort, FALSE);

    // Cleanup station configuration
    StaInitializeStationConfig(extStaPort);

    // Cleanup the AdHoc station list
    StaResetAdHocStaInfo(extStaPort, TRUE);
}

NDIS_STATUS
Sta11PausePort(
    _In_  PMP_PORT                Port
    )
{
    PMP_EXTSTA_PORT             extStaPort = MP_GET_STA_PORT(Port);

    //
    // Wait for roam/scan operations to finish
    //
    StaStopScan(extStaPort);

    //
    // Wait for all asynchronous threads to complete. This waits for connection
    // attempt to complete
    //
    while (extStaPort->ConnectContext.AsyncFuncCount > 0)
    {
        NdisMSleep(100);
    }
   
    while (extStaPort->AdHocStaInfo.AsyncFuncCount > 0)
    {
        NdisMSleep(100);
    }
   
    if (extStaPort->Config.BSSType == dot11_BSS_type_independent && Port->CurrentOpState == OP_STATE)
    {
        StaStopAdHocBeaconing(extStaPort);
    }

    return BasePortPausePort(Port);
}

NDIS_STATUS
Sta11RestartPort(
    _In_  PMP_PORT                Port
    )
{
    PMP_EXTSTA_PORT             extStaPort = MP_GET_STA_PORT(Port);

    StaStartPeriodicScan(extStaPort);

    // Restart beaconing if we were beaconing before the pause
    if (extStaPort->Config.BSSType == dot11_BSS_type_independent && Port->CurrentOpState == OP_STATE)
    {
        // BUGBUG: What if the start beaconing fails?
        StaStartAdHocBeaconing(extStaPort);
    }
    
    return BasePortRestartPort(Port);
}

NDIS_STATUS
Sta11NdisResetStart(
    _In_  PMP_PORT                Port
    )
{
    UNREFERENCED_PARAMETER(Port);

    // Currently we dont implement anything. This implies that we wont lose connectivity
    // on an NdisReset    
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
Sta11NdisResetEnd(
    _In_  PMP_PORT                Port
    )
{
    UNREFERENCED_PARAMETER(Port);

    // Currently we dont implement anything. This implies that we wont lose connectivity
    // on an NdisReset    

    return NDIS_STATUS_SUCCESS;
}

