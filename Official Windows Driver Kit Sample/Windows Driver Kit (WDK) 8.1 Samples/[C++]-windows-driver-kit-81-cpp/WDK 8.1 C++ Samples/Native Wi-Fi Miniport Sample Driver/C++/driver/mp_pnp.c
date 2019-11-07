/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_pnp.c

Abstract:
    Implements most of the initialization/termination/pnp for MP layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "mp_pnp.h"
#include "mp_oids.h"
#include "helper_port_intf.h"

#if DOT11_TRACE_ENABLED
#include "mp_pnp.tmh"
#endif


MP_REG_ENTRY MPRegTable[1]; // We dont have anything to be read from the registry
#define MP_NUM_REG_PARAMS       0

/*
    The following global variable is only for debugging purposes. This should not be used for any
    other reason. It doesn't handle the two NIC scenario at all. 
    */
static PADAPTER g_pAdapter = NULL;

NDIS_STATUS 
MPInitialize(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _In_  NDIS_HANDLE             MiniportDriverContext,
    _In_  PNDIS_MINIPORT_INIT_PARAMETERS     MiniportInitParameters
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PADAPTER                    adapter = NULL;
    NDIS_ERROR_CODE             errorCode = NDIS_STATUS_SUCCESS;    
    ULONG                       errorValue = 0;
    BOOLEAN                     miniportInitialized = FALSE;
    BOOLEAN                     hardwareStarted = FALSE;
    BOOLEAN                     miniportStarted = FALSE;

    MpEntry;

    UNREFERENCED_PARAMETER(MiniportDriverContext);
    
    do
    {
        //
        // Allocate the adapter structure and associated HW/HVL, etc structures.
        // This does not start any actions on these structures
        //
        ndisStatus = MpAllocateAdapter(
                        MiniportAdapterHandle, 
                        &adapter
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Read information from the registry. If something is not present in the
        // registry, we will be using default values
        //
        ndisStatus = MpReadRegistryConfiguration(adapter);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // Catastrophic failure like memory allocation errors, etc
            break;
        }
        
        //
        // Find the adapter
        //
        ndisStatus = Hw11FindNic(
            adapter->Hw, 
            &errorCode, 
            &errorValue
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Find/map HW resources required by the adapter
        //
        ndisStatus = Hw11DiscoverNicResources(
            adapter->Hw, 
            MiniportInitParameters->AllocatedResources,
            &errorCode,
            &errorValue
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Read the hardware state and load it into the software
        //
        ndisStatus = Hw11ReadNicInformation(adapter->Hw);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Set registration attributes relevant for this miniport. This is necessary
        // for us make other NDIS calls
        //
        ndisStatus = MpSetRegistrationAttributes(adapter);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Initialize all the state variables
        //
        ndisStatus = MpInitializeAdapter(adapter, &errorCode, &errorValue);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to initialize miniport state. Status = 0x%08x\n",
                ndisStatus));
            break;
        }
        miniportInitialized = TRUE;

        //
        // Set attributes with NDIS
        //
        ndisStatus = MpSetMiniportAttributes(adapter);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Start the HW - For PCI, this would disable interrupts, register interrupts with NDIS,
        // start the hardware and then enable interrupts. Any failures after this point,
        // we must call Hw11Stop to ensure interrupts are disabled, etc
        //
        ndisStatus = Hw11Start(adapter->Hw, &errorCode, &errorValue);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to start HW. Status = 0x%08x\n",
                ndisStatus));
            break;
        }
        hardwareStarted = TRUE;
       
        //
        // Run a NIC self-test if neeeded
        //
        ndisStatus = Hw11SelfTest(adapter->Hw);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to start miniport instance. Status = 0x%08x\n",
                ndisStatus));
            break;
        }

        //
        // Start the MP, HVL and allocate and start the first port
        //
        ndisStatus = MpStart(adapter);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to start miniport instance. Status = 0x%08x\n",
                ndisStatus));
            break;
        }

        miniportStarted = TRUE;

        g_pAdapter = adapter;
    } while (FALSE);
    

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Cleanup
        if (adapter != NULL)
        {
            // We stop the hardware first
            if (hardwareStarted)
                Hw11Stop(adapter->Hw, NdisHaltDeviceInitializationFailed);

            if (miniportStarted) 
                MpStop(adapter, NdisHaltDeviceInitializationFailed);

            if (miniportInitialized)
                MpTerminateAdapter(adapter);

            MpFreeAdapter(adapter);
            adapter = NULL;
        }

        if (errorCode != 0)
        {
            NdisWriteErrorLogEntry(
                MiniportAdapterHandle,
                errorCode,
                1,
                errorValue
                );
        }

    }

    MpExit;
    
    return ndisStatus;
}

NDIS_STATUS 
MPRestart(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PNDIS_MINIPORT_RESTART_PARAMETERS   MiniportRestartParameters
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;

    // Restart is handled by the helper port
    return HelperPortHandleMiniportRestart(adapter->HelperPort, MiniportRestartParameters);
}

NDIS_STATUS 
MPPause(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PNDIS_MINIPORT_PAUSE_PARAMETERS     MiniportPauseParameters
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;

    // Pause is handled by the helper port
    return HelperPortHandleMiniportPause(adapter->HelperPort, MiniportPauseParameters);
}

NDIS_STATUS 
Mp11PausePort(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    )
{
    // All Pauses are handled by the helper port
    return HelperPortHandlePortPause(Adapter->HelperPort, Port);
}

NDIS_STATUS 
Mp11RestartPort(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    )
{
    // All restarts are handled by the helper port
    return HelperPortHandlePortRestart(Adapter->HelperPort, Port);
}


VOID 
MPHalt(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  NDIS_HALT_ACTION        HaltAction
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;

    MpEntry;
    
    UNREFERENCED_PARAMETER(HaltAction);

    // All ports should already be paused. Meaning they are not using the hardware
    MPASSERT(MP_TEST_ADAPTER_STATUS(adapter, MP_ADAPTER_PAUSED));
    
    MP_SET_ADAPTER_STATUS(adapter, MP_ADAPTER_HALTING);

    //
    // If a reset operation is currently occuring, wait. We cannot halt till it completes
    //
    while (MP_TEST_ADAPTER_STATUS(adapter, MP_ADAPTER_IN_RESET))
    {
        NdisMSleep(20 * 1000);          // 20 milliseconds
    }
    
    // First thing we stop the hardware. For PCI this will disable & deregister interrupts
    Hw11Stop(adapter->Hw, HaltAction);

    // Stop the MP layer
    MpStop(adapter, HaltAction);

    // Cleanup all the state
    MpTerminateAdapter(adapter);

    // Delete the structures
    MpFreeAdapter(adapter);

    g_pAdapter = NULL;
    
    MpExit;
}

VOID 
MPAdapterShutdown(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  NDIS_SHUTDOWN_ACTION    ShutdownAction
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;

    //
    // Any place where we are reading registers and making major
    // decisions we should consider protecting against FFFF for surprise 
    // removal case
    //

    MpEntry;

    if (MP_TEST_ADAPTER_STATUS(adapter, MP_ADAPTER_SURPRISE_REMOVED) == FALSE)
    {
        //
        // Issue a shutdown to the NIC. NIC should go into a known state
        // and shut off power to the antenna. If surprise removal has
        // occurred, we will not do this.
        //
        Hw11Shutdown(adapter->Hw, ShutdownAction);
    }       

    MpExit;
}

VOID
MPDevicePnPEvent(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PNET_DEVICE_PNP_EVENT   NetDevicePnPEvent
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;
    NDIS_DEVICE_PNP_EVENT       devicePnPEvent = NetDevicePnPEvent->DevicePnPEvent;

    MpEntry;

    switch (devicePnPEvent)
    {
        case NdisDevicePnPEventQueryRemoved:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("MPDevicePnPEventNotify: NdisDevicePnPEventQueryRemoved\n"));
            break;

        case NdisDevicePnPEventRemoved:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("MPDevicePnPEventNotify: NdisDevicePnPEventRemoved\n"));
            break;       

        case NdisDevicePnPEventSurpriseRemoved:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("MPDevicePnPEventNotify: NdisDevicePnPEventSurpriseRemoved\n"));
            MP_SET_ADAPTER_STATUS(adapter, MP_ADAPTER_SURPRISE_REMOVED);
            break;

        case NdisDevicePnPEventQueryStopped:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("MPDevicePnPEventNotify: NdisDevicePnPEventQueryStopped\n"));
            break;

        case NdisDevicePnPEventStopped:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("MPDevicePnPEventNotify: NdisDevicePnPEventStopped\n"));
            break;      
            
        case NdisDevicePnPEventPowerProfileChanged:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("MPDevicePnPEventNotify: NdisDevicePnPEventPowerProfileChanged\n"));
            break;      
            
        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("MPDevicePnPEventNotify: unknown PnP event %x \n", devicePnPEvent));
            MpExit;
            return;
    }

    //
    // This is a valid PnPEvent. Notify Hw11 about it.
    //
    Hw11DevicePnPEvent(
        adapter->Hw,
        NetDevicePnPEvent
        );

    MpExit;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
MpAllocateAdapter(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_nullonfailure_   PADAPTER*     Adapter
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PADAPTER                    newAdapter = NULL;

    *Adapter = NULL;
    do
    {
        // Allocate a ADAPTER data structure
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle, &newAdapter, sizeof(ADAPTER), MP_MEMORY_TAG);
        if (newAdapter == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for ADAPTER\n",
                                 sizeof(ADAPTER)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Clear everything
        NdisZeroMemory(newAdapter, sizeof(ADAPTER));
        newAdapter->NumberOfPorts = 0;
        
        // Allocate memory for fields inside the ADAPTER structure
        NdisAllocateSpinLock(&(newAdapter->Lock));

        newAdapter->OidWorkItem = NdisAllocateIoWorkItem(MiniportAdapterHandle);
        if(newAdapter->OidWorkItem == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate deferred OID workitem\n"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }        

        // Save the miniport adapter handler
        newAdapter->MiniportAdapterHandle = MiniportAdapterHandle;

        // Allocate the HVL
        ndisStatus = Hvl11Allocate(MiniportAdapterHandle, &(newAdapter->Hvl), newAdapter);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // Allocate a HW data structure
        ndisStatus = Hw11Allocate(MiniportAdapterHandle, &(newAdapter->Hw), newAdapter);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // Allocate the helper port
        ndisStatus = Port11AllocatePort(newAdapter, 
                        HELPER_PORT, 
                        &(newAdapter->HelperPort)
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // Return the newly created Adapter structure to the caller
        *Adapter = newAdapter;

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newAdapter != NULL)
        {
            MpFreeAdapter(newAdapter);
        }
    }

    return ndisStatus;
}

VOID
MpFreeAdapter(
    _In_ __drv_freesMem(Mem) PADAPTER                Adapter
    )
{

    if (Adapter->HelperPort != NULL)
        Port11FreePort(Adapter->HelperPort, TRUE);

    if (Adapter->Hw != NULL)
        Hw11Free(Adapter->Hw);

    if (Adapter->Hvl != NULL)
        Hvl11Free(Adapter->Hvl);

    NdisFreeSpinLock(&(Adapter->Lock));

    if (Adapter->OidWorkItem)
    {
        NdisFreeIoWorkItem(Adapter->OidWorkItem);
        Adapter->OidWorkItem = NULL;
    }
    
    if (Adapter->PortRegInfo != NULL)
    {
        Port11FreeRegistryInformation(Adapter->PortRegInfo);
        Adapter->PortRegInfo = NULL;
    }
        
    MP_FREE_MEMORY(Adapter);
}



NDIS_STATUS
MpReadRegistryConfiguration(
    _In_  PADAPTER                Adapter
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
/*
    ULONG                       i, valueRead;
    PUCHAR                      destination;
    PMP_REG_ENTRY               regEntry;
    PNDIS_CONFIGURATION_PARAMETER   parameter = NULL;
*/
    BOOLEAN                     registryOpened;
    NDIS_HANDLE                 registryConfigurationHandle;
    NDIS_CONFIGURATION_OBJECT   configObject;

    do
    {
        configObject.Header.Type = NDIS_OBJECT_TYPE_CONFIGURATION_OBJECT;
        configObject.Header.Revision = NDIS_CONFIGURATION_OBJECT_REVISION_1;
        configObject.Header.Size = sizeof( NDIS_CONFIGURATION_OBJECT );
        configObject.NdisHandle = Adapter->MiniportAdapterHandle;
        configObject.Flags = 0;

        ndisStatus = NdisOpenConfigurationEx(
                        &configObject,
                        &registryConfigurationHandle
                        );

        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            registryOpened = TRUE;
        }
        else
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to Open Configuration. Will revert to defaults for all values\n"));
            registryOpened = FALSE;
            registryConfigurationHandle = NULL;
        }

        //
        // Read registry information for ports. This is read and saved as an opaque
        // pointer and passed to the ports when needed
        //
        ndisStatus = Port11LoadRegistryInformation(Adapter,
            registryConfigurationHandle,
            &Adapter->PortRegInfo
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to load registry configuration for ports\n"));
            break;        
        }

        //
        // Now allow the hardware to read its own parameters from registry
        //
        Hw11ReadRegistryConfiguration(Adapter->Hw, registryConfigurationHandle);

    } while (FALSE);
    
    //
    // Close the handle to the registry
    //
    if (registryConfigurationHandle)
    {
        NdisCloseConfiguration(registryConfigurationHandle);
    }

    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace (COMP_INIT_PNP, DBG_NORMAL, ("Failed to read from registry. Status = 0x%08x\n", ndisStatus));
    }

    return ndisStatus;
}

NDIS_STATUS
MpSetRegistrationAttributes(
    _In_  PADAPTER                Adapter
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_MINIPORT_ADAPTER_ATTRIBUTES    miniportAttributes;

    //
    // First we we set the registration attributes
    //
    NdisZeroMemory(&miniportAttributes, sizeof(miniportAttributes));
    miniportAttributes.RegistrationAttributes.Header.Type = NDIS_OBJECT_TYPE_MINIPORT_ADAPTER_REGISTRATION_ATTRIBUTES;
    miniportAttributes.RegistrationAttributes.Header.Revision = NDIS_MINIPORT_ADAPTER_REGISTRATION_ATTRIBUTES_REVISION_1;
    miniportAttributes.RegistrationAttributes.Header.Size = sizeof(NDIS_MINIPORT_ADAPTER_REGISTRATION_ATTRIBUTES);
    miniportAttributes.RegistrationAttributes.MiniportAdapterContext = Adapter;

    // Default port gets automatically activated by NDIS
    miniportAttributes.RegistrationAttributes.AttributeFlags = HW11_NDIS_MINIPORT_ATTRIBUTES;
    miniportAttributes.RegistrationAttributes.CheckForHangTimeInSeconds = HW11_CHECK_FOR_HANG_TIME_IN_SECONDS;
    miniportAttributes.RegistrationAttributes.InterfaceType = HW11_BUS_INTERFACE_TYPE;

    ndisStatus = NdisMSetMiniportAttributes(
        Adapter->MiniportAdapterHandle,
        &miniportAttributes
        );
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Failed to set miniport registration attributes. Status = 0x%08x\n", ndisStatus));
    }

    return ndisStatus;
}

NDIS_STATUS
MpSetMiniportAttributes(
    _In_  PADAPTER                Adapter
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_MINIPORT_ADAPTER_ATTRIBUTES    miniportAttributes;
    NDIS_PNP_CAPABILITIES       pnpCapabilities;
    NDIS_PM_CAPABILITIES        pmCapabilities;
    BOOLEAN                     cleanupNativeAttr = FALSE;
    ULONG                       ndisVersion;

    
    do
    {
        ndisVersion = NdisGetVersion();
    
        //
        // Next we set the miniport general attributes
        // 
        NdisZeroMemory(&miniportAttributes, sizeof(miniportAttributes));
        
        miniportAttributes.GeneralAttributes.Header.Type = NDIS_OBJECT_TYPE_MINIPORT_ADAPTER_GENERAL_ATTRIBUTES;
        if (ndisVersion <= MP_NDIS_VERSION_NEEDS_COMPATIBILITY)
        {
            // Windows Vista or Server 2008
            miniportAttributes.GeneralAttributes.Header.Revision = NDIS_MINIPORT_ADAPTER_GENERAL_ATTRIBUTES_REVISION_1;
            miniportAttributes.GeneralAttributes.Header.Size = NDIS_SIZEOF_MINIPORT_ADAPTER_GENERAL_ATTRIBUTES_REVISION_1;
        }
        else
        {
            miniportAttributes.GeneralAttributes.Header.Revision = NDIS_MINIPORT_ADAPTER_GENERAL_ATTRIBUTES_REVISION_2;
            miniportAttributes.GeneralAttributes.Header.Size = NDIS_SIZEOF_MINIPORT_ADAPTER_GENERAL_ATTRIBUTES_REVISION_2;
        }

        miniportAttributes.GeneralAttributes.MediaType = MP_MEDIA_TYPE;
        miniportAttributes.GeneralAttributes.PhysicalMediumType = MP_PHYSICAL_MEDIA_TYPE;    
        miniportAttributes.GeneralAttributes.MtuSize = HW11_MAX_FRAME_SIZE - DOT11_DATA_SHORT_HEADER_SIZE;
        miniportAttributes.GeneralAttributes.MaxXmitLinkSpeed = HW11_MAX_DATA_RATE * 500000;
        miniportAttributes.GeneralAttributes.MaxRcvLinkSpeed = HW11_MAX_DATA_RATE * 500000;
        miniportAttributes.GeneralAttributes.XmitLinkSpeed = NDIS_LINK_SPEED_UNKNOWN;
        miniportAttributes.GeneralAttributes.RcvLinkSpeed = NDIS_LINK_SPEED_UNKNOWN;
        miniportAttributes.GeneralAttributes.MediaConnectState = MediaConnectStateConnected;
        miniportAttributes.GeneralAttributes.MediaDuplexState = MediaDuplexStateHalf;
        miniportAttributes.GeneralAttributes.LookaheadSize = HW11_MAXIMUM_LOOKAHEAD;

        if (ndisVersion <= MP_NDIS_VERSION_NEEDS_COMPATIBILITY)
        {
            miniportAttributes.GeneralAttributes.PowerManagementCapabilities = &pnpCapabilities;
            Hw11QueryPnPCapabilities(Adapter->Hw, miniportAttributes.GeneralAttributes.PowerManagementCapabilities);
        }
        else
        {
            miniportAttributes.GeneralAttributes.PowerManagementCapabilitiesEx = &pmCapabilities;
            Hw11QueryPMCapabilities(Adapter->Hw, miniportAttributes.GeneralAttributes.PowerManagementCapabilitiesEx);
        }
        
        miniportAttributes.GeneralAttributes.MacOptions = NDIS_MAC_OPTION_COPY_LOOKAHEAD_DATA | 
                                                            NDIS_MAC_OPTION_TRANSFERS_NOT_PEND |
                                                            NDIS_MAC_OPTION_NO_LOOPBACK;
        
        miniportAttributes.GeneralAttributes.SupportedPacketFilters = NDIS_PACKET_TYPE_DIRECTED |
                                                            NDIS_PACKET_TYPE_MULTICAST |
                                                            NDIS_PACKET_TYPE_ALL_MULTICAST |
                                                            NDIS_PACKET_TYPE_BROADCAST;
        
        miniportAttributes.GeneralAttributes.MaxMulticastListSize = HW11_MAX_MULTICAST_LIST_SIZE;
        miniportAttributes.GeneralAttributes.MacAddressLength = sizeof(DOT11_MAC_ADDRESS);

        NdisMoveMemory(
            &miniportAttributes.GeneralAttributes.PermanentMacAddress,
            Hw11QueryHardwareAddress(Adapter->Hw),
            sizeof(DOT11_MAC_ADDRESS)
            );
    

        NdisMoveMemory(
            &miniportAttributes.GeneralAttributes.CurrentMacAddress,
            Hw11QueryCurrentAddress(Adapter->Hw),
            sizeof(DOT11_MAC_ADDRESS)
            );
    
        miniportAttributes.GeneralAttributes.RecvScaleCapabilities = NULL;
        miniportAttributes.GeneralAttributes.AccessType = NET_IF_ACCESS_BROADCAST;
        miniportAttributes.GeneralAttributes.DirectionType = NET_IF_DIRECTION_SENDRECEIVE;
        miniportAttributes.GeneralAttributes.IfType = IF_TYPE_IEEE80211;
        miniportAttributes.GeneralAttributes.IfConnectorPresent = TRUE;
        miniportAttributes.GeneralAttributes.DataBackFillSize = HW11_REQUIRED_BACKFILL_SIZE;

        MpQuerySupportedOidsList(
            &miniportAttributes.GeneralAttributes.SupportedOidList,
            &miniportAttributes.GeneralAttributes.SupportedOidListLength
            );

        ndisStatus = NdisMSetMiniportAttributes(
            Adapter->MiniportAdapterHandle,
            &miniportAttributes
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Failed to set miniport general attributes. Status = 0x%08x\n", ndisStatus));
            break;
        }


        //
        // Finally the 802.11 attributes
        // 
        NdisZeroMemory(&miniportAttributes, sizeof(miniportAttributes));

        miniportAttributes.Native_802_11_Attributes.Header.Type = NDIS_OBJECT_TYPE_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES;

        if (ndisVersion <= MP_NDIS_VERSION_NEEDS_COMPATIBILITY)
        {
            //
            // Vista or Server 2008: Set to revision 1 for ExtSTA support
            //
            miniportAttributes.Native_802_11_Attributes.Header.Revision = NDIS_MINIPORT_ADAPTER_802_11_ATTRIBUTES_REVISION_1;
            miniportAttributes.Native_802_11_Attributes.Header.Size = NDIS_SIZEOF_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES_REVISION_1;
        }
        else
        {
            //
            // Set to revision 2 for ExtAP support
            //
            miniportAttributes.Native_802_11_Attributes.Header.Revision = NDIS_MINIPORT_ADAPTER_802_11_ATTRIBUTES_REVISION_2;
            miniportAttributes.Native_802_11_Attributes.Header.Size = NDIS_SIZEOF_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES_REVISION_2;
        }

        //
        // We will cleanup the fields in the Native 802.11 attributes
        //
        cleanupNativeAttr = TRUE;        

        //
        // Call Hardware layer to fill the attribute structure.
        //
        ndisStatus = Hw11Fill80211Attributes(Adapter->Hw, &(miniportAttributes.Native_802_11_Attributes));
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to query native 802.11 attributes from HW layer. Status = 0x%08x\n", ndisStatus));
            break;
        }
        
        //
        // Fill virtualization specific attribute structure.
        //
        if (ndisVersion > MP_NDIS_VERSION_NEEDS_COMPATIBILITY)
        {
            ndisStatus = Hvl11Fill80211Attributes(Adapter->Hvl, &(miniportAttributes.Native_802_11_Attributes));
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to query vwifi attributes from HVL. Status = 0x%08x\n", ndisStatus));
                break;
            }        
        }
        
        //
        // Fill port specific attribute structure.
        //
        ndisStatus = Port11Fill80211Attributes(Adapter, &(miniportAttributes.Native_802_11_Attributes));
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to query native 802.11 attributes from ports. Status = 0x%08x\n", ndisStatus));
            break;
        }

        //
        // Register the 802.11 miniport attributes with NDIS
        //
        ndisStatus = NdisMSetMiniportAttributes(
            Adapter->MiniportAdapterHandle,
            &miniportAttributes
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Failed to set native 802.11 attributes. Status = 0x%08x\n", ndisStatus));
            break;
        }


    } while (FALSE);

    if (cleanupNativeAttr)
    {
        //
        // Call Hardware/Port layer to clean up the attribute structure.
        //
        Hw11Cleanup80211Attributes(Adapter->Hw, &(miniportAttributes.Native_802_11_Attributes));
        Hvl11Cleanup80211Attributes(Adapter->Hvl, &(miniportAttributes.Native_802_11_Attributes));
        Port11Cleanup80211Attributes(Adapter, &(miniportAttributes.Native_802_11_Attributes));        
    }

    return ndisStatus;
}

NDIS_STATUS
MpInitializeAdapter(
    _In_  PADAPTER                Adapter,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     hvlInit = FALSE, hwInit = FALSE, defaultPortInit = FALSE;
    BOOLEAN                     sendInit = FALSE, recvInit = FALSE;

    *ErrorCode = NDIS_STATUS_SUCCESS;
    *ErrorValue = 0;

    MPASSERT(Adapter);
    
    do
    {
        // Initialize all the state variables to their initial value
        Adapter->NumberOfPorts = 0;
        
        //
        // The miniport will start in the Paused PnP state
        //
        Adapter->Status = MP_ADAPTER_PAUSED;

        Adapter->PortRefCount = 0;

        // Initialize the other layers
        ndisStatus = Hvl11Initialize(Adapter->Hvl, Adapter->Hw);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        hvlInit = TRUE;

        ndisStatus = Hw11Initialize(Adapter->Hw, Adapter->Hvl, ErrorCode, ErrorValue);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        hwInit = TRUE;

        ndisStatus = Port11InitializePort(Adapter->HelperPort, 
                        Adapter->Hvl, 
                        Adapter->PortRegInfo
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        defaultPortInit = TRUE;

        //
        // Initialize the send specific state
        //
        ndisStatus = Hw11InitializeSendEngine(Adapter->Hw, ErrorCode, ErrorValue);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        sendInit = TRUE;

        //
        // Initialize the receive specific state
        //
        ndisStatus = Hw11InitializeReceiveEngine(Adapter->Hw, ErrorCode, ErrorValue);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        recvInit = TRUE;

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (recvInit)
            Hw11TerminateReceiveEngine(Adapter->Hw);
        if (sendInit)
            Hw11TerminateSendEngine(Adapter->Hw);
        if (defaultPortInit)
            Port11TerminatePort(Adapter->HelperPort, TRUE);
        if (hwInit)
            Hw11Terminate(Adapter->Hw);
        if (hvlInit)
            Hvl11Terminate(Adapter->Hvl);
    }

    return ndisStatus;
}


VOID
MpTerminateAdapter(
    _In_  PADAPTER                Adapter
    )
{
    // All ports must be deleted by this time
    MPASSERT(Adapter->NumberOfPorts == 0);
    
    Port11TerminatePort(Adapter->HelperPort, TRUE);

    // Halt everything on the h/w
    Hw11Halt(Adapter->Hw);
    
    Hw11TerminateReceiveEngine(Adapter->Hw);
    Hw11TerminateSendEngine(Adapter->Hw);
    Hw11Terminate(Adapter->Hw);
    Hvl11Terminate(Adapter->Hvl);
}


NDIS_STATUS
MpStart(
    _In_  PADAPTER                Adapter
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_PORT                    newPort = NULL;
    BOOLEAN                     portInit = FALSE;
    
    MPASSERT(MP_TEST_ADAPTER_STATUS(Adapter, MP_ADAPTER_PAUSED));

    do
    {
        //
        // Create the first port we would be using for all the operations. 
        // By default, our ports are EXTSTA_PORTs only. The first port does not
        // get registered with NDIS. NDIS treats it as the default port
        //
        ndisStatus = Port11AllocatePort(Adapter, 
                        EXTSTA_PORT, 
                        &newPort
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate initial port\n"));
            break;
        }

        // Initialize the port
        ndisStatus = Port11InitializePort(newPort, Adapter->Hvl, Adapter->PortRegInfo);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        portInit = TRUE;
        if (Adapter->NumberOfPorts >= MP_MAX_NUMBER_OF_PORT)
        {
            ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
            break;
        }

        // Add it to the list
        Adapter->PortList[Adapter->NumberOfPorts] = newPort;
        Adapter->NumberOfPorts++;

        
    } while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newPort != NULL)
        {
            if (portInit)
                HelperPortHandlePortTerminate(Adapter->HelperPort, newPort);

            Port11FreePort(newPort, TRUE);
            
            Adapter->NumberOfPorts = 0;
        }
    }

    return ndisStatus;
}

VOID
MpStop(
    _In_  PADAPTER                Adapter,
    _In_  NDIS_HALT_ACTION        HaltAction
    )
{
    ULONG                       i = 0;

    UNREFERENCED_PARAMETER(HaltAction);
    
    MPASSERT(MP_TEST_ADAPTER_STATUS(Adapter, MP_ADAPTER_PAUSED));
    
    // Let the helper port know about the miniport stop so that it can delete the ports 
    // it has created for OIDs
    HelperPortHandleMacCleanup(Adapter->HelperPort);

    // Now destroy the port we created automatically
    for (i = 0; i < Adapter->NumberOfPorts; i++)
    {
        // Terminate the port
        HelperPortHandlePortTerminate(Adapter->HelperPort, Adapter->PortList[i]);
        
        Port11FreePort(Adapter->PortList[i], TRUE);
    }

    Adapter->NumberOfPorts = 0;
}

BOOLEAN
MpRemoveAdapter(
    _In_  PADAPTER                Adapter
    )
{
    if (!(MpInterlockedSetClearBits(&Adapter->Status,
            MP_ADAPTER_REMOVING,      // Set this bit
            MP_ADAPTER_IN_RESET        // Clear this bit
            ) & MP_ADAPTER_REMOVING))  // Test this bit
    {
        //
        // Request a removal.
        //
        NdisWriteErrorLogEntry(
            Adapter->MiniportAdapterHandle,
            NDIS_ERROR_CODE_HARDWARE_FAILURE,
            0
            );
        
        NdisMRemoveMiniport(Adapter->MiniportAdapterHandle);

        return TRUE;
    }
    else
    {
        //
        // Adapter is already in removal. No need to request one.
        //
        return FALSE;
    }
}


