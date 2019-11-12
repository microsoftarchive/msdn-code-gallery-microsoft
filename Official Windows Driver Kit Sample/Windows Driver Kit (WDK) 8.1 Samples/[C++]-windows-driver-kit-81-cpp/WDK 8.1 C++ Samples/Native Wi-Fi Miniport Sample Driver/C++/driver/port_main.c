/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    port_main.c

Abstract:
    Implements the functionality the mux/demux functionality
    needed by ports
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "port_defs.h"
#include "base_port_intf.h"
#include "vnic_intf.h"
#include "helper_port_intf.h"

#if DOT11_TRACE_ENABLED
#include "port_main.tmh"
#endif

NDIS_STATUS
Port11Fill80211Attributes(
    _In_  PADAPTER                Adapter,
    _Inout_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     staAttributesAllocated = FALSE;
    ULONG                       ndisVersion;
    
    do
    {
        //
        // Fill ExtSTA specific attribute structure.
        //
        ndisStatus = Sta11Fill80211Attributes(Adapter->HelperPort, Attr);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to query native 802.11 attributes from STA layer. Status = 0x%08x\n", ndisStatus));
            break;
        }
        staAttributesAllocated = TRUE;

        ndisVersion = NdisGetVersion();
        if (ndisVersion > MP_NDIS_VERSION_NEEDS_COMPATIBILITY)
        {
            //
            // Fill ExtAP specific attribute structure.
            //
            ndisStatus = Ap11Fill80211Attributes(Adapter->HelperPort, Attr);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to query native 802.11 attributes from AP layer. Status = 0x%08x\n", ndisStatus));
                break;
            }
        }
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (staAttributesAllocated)
        {
            Sta11Cleanup80211Attributes(Adapter->HelperPort, Attr);
        }
    }

    return ndisStatus;
}

VOID
Port11Cleanup80211Attributes(
    _In_  PADAPTER                Adapter,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES attr
    )
{
    Sta11Cleanup80211Attributes(Adapter->HelperPort, attr);
    Ap11Cleanup80211Attributes(Adapter->HelperPort, attr);
}

// Failure only in case of unrecoverable errors. Not finding data in the registry is a
// recoverable error & should be handled by using default values
NDIS_STATUS
Port11LoadRegistryInformation(
    _In_                        PADAPTER                Adapter,
    _In_opt_                    NDIS_HANDLE             ConfigurationHandle,
    _Outptr_result_maybenull_   PVOID*        RegistryInformation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_PORT_REG_INFO           portRegInfo = NULL;

    *RegistryInformation = NULL;
    do
    {
        //
        // Allocate memory for the registry info
        //
        MP_ALLOCATE_MEMORY(Adapter->MiniportAdapterHandle, &portRegInfo, sizeof(MP_PORT_REG_INFO), PORT_MEMORY_TAG);
        if (portRegInfo == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for MP_PORT_REG_INFO\n",
                                 sizeof(MP_PORT_REG_INFO)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(portRegInfo, sizeof(MP_PORT_REG_INFO));
    
        //
        // Fill helper port registry configuration
        //
        ndisStatus = HelperPortLoadRegistryInformation(
                        Adapter->MiniportAdapterHandle,
                        ConfigurationHandle, 
                        &portRegInfo->HelperPortRegInfo
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to load helper port registry configuration\n"));
            break;
        }
        
        //
        // Fill ExtSTA registry configuration
        //
        ndisStatus = Sta11LoadRegistryInformation(
                        Adapter->MiniportAdapterHandle,
                        ConfigurationHandle,
                        &portRegInfo->ExtStaRegInfo
                        ); 
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to load helper port registry configuration\n"));
            break;
        }

        //
        // Fill ExtAP specific attribute structure.
        //
        ndisStatus = Ap11LoadRegistryInformation(
                        Adapter->MiniportAdapterHandle,
                        ConfigurationHandle,
                        &portRegInfo->ExtAPRegInfo
                        ); 
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to load helper port registry configuration\n"));
            break;
        }

        *RegistryInformation = portRegInfo;
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (portRegInfo != NULL)
        {
            Port11FreeRegistryInformation(portRegInfo);
            portRegInfo = NULL;
        }
    }

    return ndisStatus;

}

VOID
Port11FreeRegistryInformation(
    _In_ __drv_freesMem(Mem)   PVOID   RegistryInformation
    )
{
    PMP_PORT_REG_INFO  portRegInfo = (PMP_PORT_REG_INFO)RegistryInformation;

    if (portRegInfo)
    {
        if (portRegInfo->HelperPortRegInfo)
            HelperPortFreeRegistryInformation(portRegInfo->HelperPortRegInfo);
        if (portRegInfo->ExtStaRegInfo)
            Sta11FreeRegistryInformation(portRegInfo->ExtStaRegInfo);
        if (portRegInfo->ExtAPRegInfo)
            Ap11FreeRegistryInformation(portRegInfo->ExtAPRegInfo);

        MP_FREE_MEMORY(portRegInfo);
    }
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Port11AllocatePort(
    _In_  PADAPTER                Adapter,
    _In_  MP_PORT_TYPE            PortType,
    _Outptr_result_nullonfailure_ PMP_PORT*     Port
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_PORT                    newPort = NULL;
    BOOLEAN                     childPortAllocated = FALSE;

    *Port = NULL;
    do
    {
        //
        // Allocate the basic structure for the port
        //
        ndisStatus = BasePortAllocatePort(Adapter, PortType, &newPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate base port structure\n"));
            break;
        }

        //
        // Depending on the port type, allocate the child port
        //
        switch (PortType)
        {
            case HELPER_PORT:
                ndisStatus = HelperPortAllocatePort(Adapter->MiniportAdapterHandle, 
                                Adapter, 
                                newPort
                                );
                break;

            case EXTSTA_PORT:
                ndisStatus = Sta11AllocatePort(Adapter->MiniportAdapterHandle, 
                                Adapter, 
                                newPort
                                );
                break;

            case EXTAP_PORT:
                ndisStatus = Ap11AllocatePort(Adapter->MiniportAdapterHandle, 
                                Adapter, 
                                newPort
                                );
                break;

            default:
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to allocated unrecognized port type %d\n", PortType));
                ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
                break;
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        childPortAllocated = TRUE;

        // Every port other than the helper port gets the default NDIS port number. This
        // will change later
        if (HELPER_PORT == PortType)
        {
            newPort->PortNumber = HELPER_PORT_PORT_NUMBER;
        }
        else
        {
            newPort->PortNumber = DEFAULT_NDIS_PORT_NUMBER;
        }
        
        //
        // Save the return port
        //
        *Port = newPort;
        
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newPort != NULL)
        {
            if (childPortAllocated)
            {
                // Free the port that has been allocated
                switch (newPort->PortType)
                {
                    case HELPER_PORT:
                        HelperPortFreePort(newPort);
                        break;

                    case EXTSTA_PORT:
                        Sta11FreePort(newPort);
                        break;

                    case EXTAP_PORT:
                        Ap11FreePort(newPort);
                        break;

                    default:
                        break;
                }
            }

            // Free the base port
            BasePortFreePort(newPort);

        }
    }

    return ndisStatus;
}


VOID
Port11FreePort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 FreeBasePort
    )
{
    // Free the child port structure
    switch (Port->PortType)
    {
        case HELPER_PORT:
            HelperPortFreePort(Port);
            break;

        case EXTSTA_PORT:
            Sta11FreePort(Port);
            break;

        case EXTAP_PORT:
            Ap11FreePort(Port);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to free unrecognized port type %d\n", Port->PortType));
            break;
    }

    if (FreeBasePort)
    {
        // Free the base port structure
        BasePortFreePort(Port);
    }
}

NDIS_STATUS
Port11InitializePort(
    _In_  PMP_PORT                Port,
    _In_  PHVL                    Hvl,
    _In_  PVOID                   RegistryInformation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     baseInitialized = FALSE;
    PMP_PORT_REG_INFO           regInfo = (PMP_PORT_REG_INFO)RegistryInformation;

    do
    {
        //
        // Initialize the base port structure
        //
        ndisStatus = BasePortInitializePort(Port, Hvl);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        baseInitialized = TRUE;
        
        switch (Port->PortType)
        {
            case HELPER_PORT:
                ndisStatus = HelperPortInitializePort(Port, regInfo->HelperPortRegInfo);
                break;

            case EXTSTA_PORT:
                ndisStatus = Sta11InitializePort(Port, regInfo->ExtStaRegInfo);
                break;

            case EXTAP_PORT:
                ndisStatus = Ap11InitializePort(Port, regInfo->ExtAPRegInfo);
                break;

            default:
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to initialize unrecognized port type %d\n", Port->PortType));
                ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
                break;
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (baseInitialized)
        {
            BasePortTerminatePort(Port);
        }
    }

    return ndisStatus;
}

VOID
Port11TerminatePort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 TerminateBasePort
    )
{
    // Terminate the port
    switch (Port->PortType)
    {
        case HELPER_PORT:
            HelperPortTerminatePort(Port);
            break;

        case EXTSTA_PORT:
            Sta11TerminatePort(Port);
            break;

        case EXTAP_PORT:
            Ap11TerminatePort(Port);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to terminate unrecognized port type %d\n", Port->PortType));
            break;
    }

    if (TerminateBasePort)
    {
        // Terminate the base port
        BasePortTerminatePort(Port);
    }
}



PMP_PORT
Port11TranslatePortNumberToPort(
    _In_  PADAPTER                Adapter,
    _In_  NDIS_PORT_NUMBER        PortNumber
    )
{
    ULONG                       i = 0;
    PMP_PORT                    tempPort;

    for (i = 0; i < Adapter->NumberOfPorts; i++)
    {
        tempPort = Adapter->PortList[i];

        if (tempPort->PortNumber == PortNumber)
        {
            return tempPort;
        }
    }

    return NULL;
}

VOID
Port11RemovePortFromAdapterList(
    _In_ PMP_PORT                 PortToRemove
    )
{
    ULONG i = 0;
    PADAPTER Adapter = PortToRemove->Adapter;
    ULONG NumberOfPorts = Adapter->NumberOfPorts;

    
    if (NumberOfPorts == 0)
    {
        return;
    }
        
    if (NumberOfPorts > MP_MAX_NUMBER_OF_PORT)
    {
        ASSERT(Adapter->NumberOfPorts <= MP_MAX_NUMBER_OF_PORT);        
        return;    
    }

    
    // first find the index of the port to be deleted in the adapter list
    for (i = 0; i < NumberOfPorts; i++)
    {
        if (Adapter->PortList[i] == PortToRemove)
        {
            Adapter->PortList[i] = NULL;
            break;
        }
    }

    _Analysis_assume_(NumberOfPorts  > 0);

    _Analysis_assume_(NumberOfPorts == MP_MAX_NUMBER_OF_PORT);

    NumberOfPorts = NumberOfPorts - 1;
    
    // now shift all the ports aftet this port to the left
    for (; i < NumberOfPorts; i++)
    {
        Adapter->PortList[i] = Adapter->PortList[i+1];
        Adapter->PortList[i+1] = NULL;
    }

    // decrement the count of ports
    Adapter->NumberOfPorts--;
}


NDIS_STATUS
Port11PausePort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 IsInternal
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Port Pause\n"));        

    UNREFERENCED_PARAMETER(IsInternal);
    
    PORT_ACQUIRE_PNP_MUTEX(Port);
    Port->PauseCount++;
    if (Port->PauseCount > 1)
    {
        // We are already paused, no need to do anything
        PORT_RELEASE_PNP_MUTEX(Port);

        MpExit;
//        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Port Pause\n"));        

        return NDIS_STATUS_SUCCESS;
    }

    // Set the pausing flag on the port
    MP_ACQUIRE_PORT_LOCK(Port, FALSE);
    MP_SET_PORT_STATUS(Port, MP_PORT_PAUSING);
    MP_RELEASE_ADAPTER_LOCK(Port, FALSE);

    switch (Port->PortType)
    {
        case EXTSTA_PORT:
            ndisStatus = Sta11PausePort(Port);
            break;

        case EXTAP_PORT:
            ndisStatus = Ap11PausePort(Port);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to pause unrecognized port type %d\n", Port->PortType));
            break;
    }

    // Set the pausing flag on the adapter
    MP_ACQUIRE_PORT_LOCK(Port, FALSE);
    MP_SET_PORT_STATUS(Port, MP_PORT_PAUSED);
    MP_CLEAR_PORT_STATUS(Port, MP_PORT_PAUSING);
    MP_RELEASE_ADAPTER_LOCK(Port, FALSE);

    PORT_RELEASE_PNP_MUTEX(Port);
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Port Pause\n"));        

    return ndisStatus;
}

NDIS_STATUS
Port11RestartPort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 IsInternal
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(IsInternal);

//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Port Restart\n"));        

    PORT_ACQUIRE_PNP_MUTEX(Port);
    Port->PauseCount--;
    if (Port->PauseCount > 0)
    {
        // We have multiple pauses pending. We wont restart
        // yet
        PORT_RELEASE_PNP_MUTEX(Port);

        MpExit;
//        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Port Restart\n"));        

        // We still return success to the restart request
        return NDIS_STATUS_SUCCESS;
    }

    
    switch (Port->PortType)
    {
        case EXTSTA_PORT:
            ndisStatus = Sta11RestartPort(Port);
            break;

        case EXTAP_PORT:
            ndisStatus = Ap11RestartPort(Port);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to restart unrecognized port type %d\n", Port->PortType));
            break;
    }

    // Clear the paused flag
    MP_ACQUIRE_PORT_LOCK(Port, FALSE);
    MP_CLEAR_PORT_STATUS(Port, MP_PORT_PAUSED);
    MP_RELEASE_ADAPTER_LOCK(Port, FALSE);

    PORT_RELEASE_PNP_MUTEX(Port);
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("<== Port Restart\n"));        

    return ndisStatus;
}

NDIS_STATUS
Port11NdisResetStart(
    _In_  PMP_PORT                Port
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Port Reset Start\n"));        

    PORT_ACQUIRE_PNP_MUTEX(Port);
    
    switch (Port->PortType)
    {
        case EXTSTA_PORT:
            ndisStatus = Sta11NdisResetStart(Port);
            break;

        case EXTAP_PORT:
            ndisStatus = Ap11NdisResetStart(Port);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to notify reset start to unrecognized port type %d\n", Port->PortType));
            break;
    }

    PORT_RELEASE_PNP_MUTEX(Port);
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Port Reset Start\n"));        

    return ndisStatus;
}

NDIS_STATUS
Port11NdisResetEnd(
    _In_  PMP_PORT                Port
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Port Reset End\n"));        

    PORT_ACQUIRE_PNP_MUTEX(Port);
    
    switch (Port->PortType)
    {
        case EXTSTA_PORT:
            ndisStatus = Sta11NdisResetEnd(Port);
            break;

        case EXTAP_PORT:
            ndisStatus = Ap11NdisResetEnd(Port);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to notify reset end to unrecognized port type %d\n", Port->PortType));
            break;
    }

    PORT_RELEASE_PNP_MUTEX(Port);
//    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("==> Port Reset End\n"));        

    return ndisStatus;
}

NDIS_STATUS
Port11SetPower(
    _In_  PMP_PORT                Port,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

//    MpTrace(COMP_POWER, DBG_NORMAL, ("==> Port Set Power \n"));        

    switch (Port->PortType)
    {
        case EXTSTA_PORT:
            ndisStatus = Sta11SetPower(Port, NewDevicePowerState);
            break;

        case EXTAP_PORT:
            ndisStatus = Ap11SetPower(Port, NewDevicePowerState);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to notify power event to unrecognized port type %d\n", Port->PortType));
            break;
    }

//    MpTrace(COMP_POWER, DBG_NORMAL, ("==> Port Set Power\n"));        

    return ndisStatus;
}

VOID
Port11HandleSendNetBufferLists(
    _In_  PMP_PORT                Port,
    _In_  PNET_BUFFER_LIST        NetBufferLists,
    _In_  ULONG                   SendFlags
    )
{
    BasePortHandleSendNetBufferLists(Port, NetBufferLists, SendFlags);
}

NDIS_STATUS
Port11NotifySend(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    )
{
    return Port->SendEventHandler(Port, PacketList, SendFlags);
}

VOID
Port11SendCompletePackets(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    )
{
    BasePortSendCompletePackets(Port, PacketList, SendCompleteFlags);
}

VOID
Port11NotifySendComplete(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    )
{
    Port->SendCompleteEventHandler(Port, PacketList, SendCompleteFlags);
}


VOID
Port11IndicateReceivePackets(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    )
{
    // We just pass this to the port specific handler
    BasePortIndicateReceivePackets(Port, PacketList, ReceiveFlags);
}

NDIS_STATUS
Port11NotifyReceive(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    )
{
    return Port->ReceiveEventHandler(Port, PacketList, ReceiveFlags);
}


VOID
Port11HandleReturnNetBufferLists(
    _In_  PMP_PORT                Port,
    _In_  PNET_BUFFER_LIST        NetBufferLists,
    _In_  ULONG                   ReturnFlags
    )
{
    // We just pass this to the port specific handler
    BasePortHandleReturnNetBufferLists(Port, NetBufferLists, ReturnFlags);
}

VOID
Port11NotifyReturn(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    )
{
    Port->ReturnEventHandler(Port, PacketList, ReturnFlags);
}

VOID
Port11IndicateStatus(
    _In_  PMP_PORT                Port,
    _In_  NDIS_STATUS             StatusCode,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    )
{
    // Free the child port structure
    switch (Port->PortType)
    {
        case HELPER_PORT:
            MPASSERT(FALSE);
            break;

        case EXTSTA_PORT:
            Sta11IndicateStatus(Port, StatusCode, StatusBuffer, StatusBufferSize);
            break;

        case EXTAP_PORT:
            Ap11IndicateStatus(Port, StatusCode, StatusBuffer, StatusBufferSize);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to indicate status to an unrecognized port type %d\n", Port->PortType));
            break;
    }
}

VOID
Port11Notify(
    _In_  PMP_PORT        Port,
    _In_  PVOID           Notif
)
{
    switch (Port->PortType)
    {
        case HELPER_PORT:
            HelperPortNotify(Port, Notif);
            break;

        case EXTSTA_PORT:
            Sta11Notify(Port, Notif);
            break;

        case EXTAP_PORT:
            Ap11Notify(Port, Notif);
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to indicate status to an unrecognized port type %d\n", Port->PortType));
            break;
    }
}


