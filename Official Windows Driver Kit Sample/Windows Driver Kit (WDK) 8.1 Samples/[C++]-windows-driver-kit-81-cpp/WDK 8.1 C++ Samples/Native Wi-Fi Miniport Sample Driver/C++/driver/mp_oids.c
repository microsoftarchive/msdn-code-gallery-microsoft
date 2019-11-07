/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_oids.c

Abstract:
    Implements the OIDs for the MP layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/


#include "precomp.h"
#include "mp_main.h"
#include "mp_oids.h"
#include "helper_port_intf.h"

#if DOT11_TRACE_ENABLED
#include "mp_oids.tmh"
#endif

NDIS_OID MPSupportedOids[] =
{
    // NDIS OIDs
    OID_GEN_CURRENT_LOOKAHEAD,
    OID_GEN_CURRENT_PACKET_FILTER,
    OID_GEN_HARDWARE_STATUS,
    OID_GEN_INTERRUPT_MODERATION,
    OID_GEN_LINK_PARAMETERS,
    OID_GEN_MAXIMUM_FRAME_SIZE,
    OID_GEN_MAXIMUM_TOTAL_SIZE,
    OID_GEN_RECEIVE_BLOCK_SIZE,
    OID_GEN_RECEIVE_BUFFER_SPACE,
    OID_GEN_SUPPORTED_GUIDS,
    OID_GEN_TRANSMIT_BLOCK_SIZE,
    OID_GEN_TRANSMIT_BUFFER_SPACE,
    OID_GEN_TRANSMIT_QUEUE_LENGTH,
    OID_GEN_VENDOR_DESCRIPTION,
    OID_GEN_VENDOR_DRIVER_VERSION,
    OID_GEN_VENDOR_ID,
    OID_802_3_CURRENT_ADDRESS,          // This is needed for compatibility with some apps


    /* Non-802.11 specific statistics are handled by the nwifi filter, but we
     * report support for them
    */
    OID_GEN_RCV_OK,
    OID_GEN_STATISTICS,
    OID_GEN_XMIT_OK,

    // PNP handlers
    OID_PNP_SET_POWER,
    OID_PNP_QUERY_POWER,

    /* Driver does not support wake up from patterns
        OID_PNP_ADD_WAKE_UP_PATTERN,
        OID_PNP_REMOVE_WAKE_UP_PATTERN,
        OID_PNP_ENABLE_WAKE_UP,
    */

    // Operation Oids
    OID_DOT11_CURRENT_ADDRESS,
    OID_DOT11_CURRENT_OPERATION_MODE,
    OID_DOT11_CURRENT_OPTIONAL_CAPABILITY,
    OID_DOT11_DATA_RATE_MAPPING_TABLE,
    OID_DOT11_MAXIMUM_LIST_SIZE,
    OID_DOT11_MPDU_MAX_LENGTH, 
    OID_DOT11_MULTICAST_LIST,
    OID_DOT11_NIC_POWER_STATE,          // PHY: msDot11NICPowerState 
    OID_DOT11_OPERATION_MODE_CAPABILITY,
    OID_DOT11_OPTIONAL_CAPABILITY,
    OID_DOT11_PERMANENT_ADDRESS,
    OID_DOT11_RECV_SENSITIVITY_LIST,
    OID_DOT11_RESET_REQUEST,
    OID_DOT11_RF_USAGE,
    OID_DOT11_SCAN_REQUEST,
    OID_DOT11_SUPPORTED_DATA_RATES_VALUE,
    OID_DOT11_SUPPORTED_PHY_TYPES,      // msDot11SupportedPhyTypes
    OID_DOT11_SUPPORTED_POWER_LEVELS,
    OID_DOT11_SUPPORTED_RX_ANTENNA,
    OID_DOT11_SUPPORTED_TX_ANTENNA,

    // MIB Oids
    OID_DOT11_BEACON_PERIOD,            // dot11BeaconPeriod 
    OID_DOT11_CCA_MODE_SUPPORTED,       // PHY: dot11CCAModeSupported
    OID_DOT11_CF_POLLABLE,              // dot11CFPollable 
    OID_DOT11_CHANNEL_AGILITY_ENABLED,  // PHY: dot11ChannelAgilityEnabled 
    OID_DOT11_CHANNEL_AGILITY_PRESENT,  // PHY: dot11ChannelAgilityPresent 
    OID_DOT11_COUNTRY_STRING,           // dot11CountryString 
    OID_DOT11_CURRENT_CCA_MODE,         // PHY: dot11CurrentCCAMode
    OID_DOT11_CURRENT_CHANNEL,          // PHY: dot11CurrentChannel
    OID_DOT11_CURRENT_FREQUENCY,        // PHY: dot11CurrentFrequency
    OID_DOT11_CURRENT_REG_DOMAIN,       // dot11CurrentRegDomain 
    OID_DOT11_CURRENT_TX_POWER_LEVEL,   // PHY: dot11CurrentTxPowerLevel 
    OID_DOT11_DIVERSITY_SELECTION_RX,   // PHY: dot11DiversitySelectionRx 
    OID_DOT11_DIVERSITY_SUPPORT,        // PHY: dot11DiversitySupport 
    OID_DOT11_DSSS_OFDM_OPTION_ENABLED, // PHY: dot11DSSSOFDMOptionEnabled
    OID_DOT11_DSSS_OFDM_OPTION_IMPLEMENTED,         // PHY: dot11DSSSOFDMOptionImplemented
    OID_DOT11_ED_THRESHOLD,             // PHY: dot11EDThreshold
    OID_DOT11_ERP_PBCC_OPTION_ENABLED,  // PHY: dot11ERPBCCOptionEnabled
    OID_DOT11_ERP_PBCC_OPTION_IMPLEMENTED,          // PHY: dot11ERPPBCCOptionImplemented
    OID_DOT11_FRAGMENTATION_THRESHOLD,  // dot11FragmentationThreshold
    OID_DOT11_FREQUENCY_BANDS_SUPPORTED,            // PHY: dot11FrequencyBandsSupported
    OID_DOT11_LONG_RETRY_LIMIT,         // dot11LongRetryLimit 
    OID_DOT11_MAC_ADDRESS,              // dot11MACAddress 
    OID_DOT11_MAX_RECEIVE_LIFETIME,     // dot11MaxReceiveLifetime 
    OID_DOT11_MAX_TRANSMIT_MSDU_LIFETIME,           // dot11MaxTransmitMSDULifetime 
    OID_DOT11_MULTI_DOMAIN_CAPABILITY_ENABLED,      // dot11MultiDomainCapabilityEnabled 
    OID_DOT11_MULTI_DOMAIN_CAPABILITY_IMPLEMENTED,  // dot11MultiDomainCapabilityImplemented 
    OID_DOT11_OPERATIONAL_RATE_SET,     // dot11OperationalRateSet 
    OID_DOT11_PBCC_OPTION_IMPLEMENTED,  // PHY: dot11PBCCOptionImplemented
    OID_DOT11_REG_DOMAINS_SUPPORT_VALUE,            // PHY: dot11RegDomainValue 
    OID_DOT11_RTS_THRESHOLD,            // dot11RTSThreshold 
    OID_DOT11_SHORT_PREAMBLE_OPTION_IMPLEMENTED,    // PHY: dot11ShortPreambleOptionImplemented
    OID_DOT11_SHORT_RETRY_LIMIT,        // dot11ShortRetryLimit 
    OID_DOT11_SHORT_SLOT_TIME_OPTION_ENABLED,       // PHY: dot11ShortSlotTimeOptionEnabled
    OID_DOT11_SHORT_SLOT_TIME_OPTION_IMPLEMENTED,   // PHY: dot11ShortSlotTimeOptionImplemented
    OID_DOT11_STATION_ID,               // dot11StationID 
    OID_DOT11_TEMP_TYPE,                // PHY: dot11TempType 

    // ExtSTA Operation OIDs
    OID_DOT11_ACTIVE_PHY_LIST,          // msDot11ActivePhyList
    OID_DOT11_ATIM_WINDOW,
    OID_DOT11_AUTO_CONFIG_ENABLED,      // msDot11AutoConfigEnabled
    OID_DOT11_CIPHER_DEFAULT_KEY,
    OID_DOT11_CIPHER_DEFAULT_KEY_ID,    // dot11DefaultKeyID
    OID_DOT11_CIPHER_KEY_MAPPING_KEY,
    OID_DOT11_CONNECT_REQUEST,
    OID_DOT11_CURRENT_PHY_ID,           // msDot11CurrentPhyID
    OID_DOT11_DESIRED_BSS_TYPE,         // dot11DesiredBSSType
    OID_DOT11_DESIRED_BSSID_LIST,       // msDot11DesiredBSSIDList
    OID_DOT11_DESIRED_PHY_LIST,         // msDot11DesiredPhyList
    OID_DOT11_DESIRED_SSID_LIST,        // msDot11DesiredSSIDList
    OID_DOT11_DISCONNECT_REQUEST,
    OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM,     // msDot11EnabledAuthAlgo
    OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM,   // msDot11EnabledMulticastCipherAlgo
    OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM,     // msDot11EnabledUnicastCipherAlgo
    OID_DOT11_ENUM_ASSOCIATION_INFO,
    OID_DOT11_ENUM_BSS_LIST,
    OID_DOT11_EXCLUDE_UNENCRYPTED,      // dot11ExcludeUnencrypted
    OID_DOT11_EXCLUDED_MAC_ADDRESS_LIST,            // msDot11ExcludedMacAddressList
    OID_DOT11_EXTSTA_CAPABILITY,
    OID_DOT11_FLUSH_BSS_LIST,
    OID_DOT11_HARDWARE_PHY_STATE,       // msDot11HardwarePHYState
    OID_DOT11_HIDDEN_NETWORK_ENABLED,   // msDot11HiddenNetworkEnabled
    OID_DOT11_IBSS_PARAMS,
    OID_DOT11_MEDIA_STREAMING_ENABLED,  // msDot11MediaStreamingEnabled
    OID_DOT11_PMKID_LIST,
    OID_DOT11_POWER_MGMT_REQUEST,       // msDot11PowerSavingLevel
    OID_DOT11_PRIVACY_EXEMPTION_LIST,   // msDot11PrivacyExemptionList
    OID_DOT11_SAFE_MODE_ENABLED,        // msDot11SafeModeEnabled
    OID_DOT11_STATISTICS,
    OID_DOT11_SUPPORTED_MULTICAST_ALGORITHM_PAIR,
    OID_DOT11_SUPPORTED_UNICAST_ALGORITHM_PAIR,
    OID_DOT11_UNICAST_USE_GROUP_ENABLED,            // msDot11UnicastUseGroupEnabled
    OID_DOT11_UNREACHABLE_DETECTION_THRESHOLD,      // msDot11UnreachableDetectionThreshold
    OID_DOT11_ASSOCIATION_PARAMS,

    // ExtAP specific OIDs
    OID_DOT11_DTIM_PERIOD,                      // dot11DTIMPeriod
    OID_DOT11_AVAILABLE_CHANNEL_LIST,           // msDot11AvailableChannelList
    OID_DOT11_AVAILABLE_FREQUENCY_LIST,         // msDot11AvailableFrequencyList
    OID_DOT11_ENUM_PEER_INFO,                   
    OID_DOT11_DISASSOCIATE_PEER_REQUEST,        
    OID_DOT11_PORT_STATE_NOTIFICATION,
    OID_DOT11_INCOMING_ASSOCIATION_DECISION,
    OID_DOT11_ADDITIONAL_IE,
    OID_DOT11_WPS_ENABLED,
    OID_DOT11_START_AP_REQUEST,
    
    // Virtual WiFi specifc OIDs
    OID_DOT11_CREATE_MAC,
    OID_DOT11_DELETE_MAC,
};


VOID
MpQuerySupportedOidsList(
    _Inout_ PNDIS_OID            *SupportedOidList,
    _Inout_ PULONG               SupportedOidListLength
    )
{
    *SupportedOidList = MPSupportedOids;
    *SupportedOidListLength = sizeof(MPSupportedOids);
}

VOID
Mp11CompleteOidRequest(
    _In_  PADAPTER                Adapter,
    _In_opt_  PMP_PORT                CompletingPort,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest,
    _In_  NDIS_STATUS             CompletionStatus
    )
{
    UNREFERENCED_PARAMETER(CompletingPort);
    
    // Clear the deferred OID request field. This would help is debugging
    Adapter->DeferredOidRequest = NULL;

    NdisMOidRequestComplete(
        Adapter->MiniportAdapterHandle,
        NdisOidRequest,
        CompletionStatus
        );

}

_Function_class_(NDIS_IO_WORKITEM_FUNCTION)
NTSTATUS
MpOidRequestWorkItem(
    _In_  PVOID                   Context,
    _In_  NDIS_HANDLE             NdisIoWorkItemHandle
    )    
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PADAPTER                    adapter = (PADAPTER)Context;
    PNDIS_OID_REQUEST           NdisOidRequest = adapter->DeferredOidRequest;
    NDIS_OID                    oid;
    PMP_PORT                    destinationPort = NULL;
    BOOLEAN                     fOidCompleted = FALSE;
    
    UNREFERENCED_PARAMETER(NdisIoWorkItemHandle);

    // Depending on the OID, hand it to the appropriate component for processing
    oid = NdisOidRequest->DATA.QUERY_INFORMATION.Oid; // Oid is at same offset for all RequestTypes
    
    MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%d): Processing NDIS_OID_REQUEST for OID 0x%08x\n", NdisOidRequest->PortNumber, oid));

    //
    // Now determine if the OID is something the helper port would process or one of the
    // ports from the port list should process
    //
    switch (oid)
    {
        // Oids that the helper port should handle since they have an impact on 
        // virtualization
        case OID_PNP_SET_POWER:
        case OID_PNP_QUERY_POWER:
        case OID_DOT11_CREATE_MAC:
        case OID_DOT11_DELETE_MAC:
            ndisStatus = HelperPortHandleOidRequest(
                            adapter->HelperPort,
                            NdisOidRequest,
                            &fOidCompleted
                            );
            break;


        default:
            {
                // All other OIDs would need to get forwarded to the appropriate port
                // for processing. 

                //
                // First we would need to translate from the NDIS_PORT_NUMBER
                // to our port structure. This is done by walking the PortList
                // Since OID calls are serialized, we do not expect the Portlist to change
                // while we are trying to find the port or for the port to get deleted
                // until this OID is completed. So we do not need to protect the Port/PortList
                // in any way
                //
                destinationPort = Port11TranslatePortNumberToPort(
                                    adapter, 
                                    NdisOidRequest->PortNumber
                                    );
                if (destinationPort == NULL)
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("Unable to find Port corresponding to PortNumber %d\n", 
                        NdisOidRequest->PortNumber));

                    ndisStatus = NDIS_STATUS_INVALID_PORT;
                }
                else
                {
                    //
                    // Pass it to the appropriate port for processing
                    //
                    ndisStatus = Port11HandleOidRequest(
                                    destinationPort,
                                    NdisOidRequest
                                    );
                }
            }
            break;
    }

    if ((ndisStatus != NDIS_STATUS_SUCCESS) && (ndisStatus != NDIS_STATUS_PENDING))
    {
        MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%d): NDIS_OID_REQUEST for OID 0x%08x failed. Status = 0x%08x\n", 
            NdisOidRequest->PortNumber,
            oid, ndisStatus));
    }
    else
    {
        MpTrace(COMP_OID, DBG_SERIOUS, ("NDIS_OID_REQUEST for OID 0x%08x succeeded. Status = 0x%08x\n", 
            oid, ndisStatus));
    }

    // complete the Oid request if it hasn't already been completed
    if (!fOidCompleted && NDIS_STATUS_PENDING != ndisStatus)
    {
        // Complete the OID request
        Mp11CompleteOidRequest(
            adapter,
            NULL,
            NdisOidRequest,
            ndisStatus
            );        
    }
    
    return STATUS_SUCCESS;
}



NDIS_STATUS
MPOidRequest(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
    NDIS_OID                    oid;

    // Depending on the OID, hand it to the appropriate component for processing
    oid = NdisOidRequest->DATA.QUERY_INFORMATION.Oid; // Oid is at same offset for all RequestTypes

    // Save the request (for tracking purpose)
    adapter->Tracking_LastRequest = NdisOidRequest;
    adapter->Tracking_LastOid = oid;

#if DBG
    if (adapter->Debug_BreakOnOid == oid)
    {
        DbgPrint("Received OID request %p for desired OID\n", NdisOidRequest);
        adapter->Debug_BreakOnOid = 0;  // Only break once
        DbgBreakPoint();
    }
#endif

    //
    // If the adapter has been surprise removed, fail request
    //
    if (MP_TEST_ADAPTER_STATUS(adapter, MP_ADAPTER_SURPRISE_REMOVED))
    {
        ndisStatus = NDIS_STATUS_FAILURE;
        MpTrace(COMP_OID, DBG_SERIOUS, ("NDIS_OID_REQUEST failed as surprise removal is in progress\n"));
        return ndisStatus;
    }

    // Save the deferred OID request
    //
    adapter->DeferredOidRequest = NdisOidRequest;

    // 
    // Queue the workitem. We always defer the Oid requests
    //
    NdisQueueIoWorkItem(
        adapter->OidWorkItem,
        MpOidRequestWorkItem,
        adapter 
        );

    return NDIS_STATUS_PENDING;
}

VOID 
MPCancelOidRequest(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PVOID                   RequestId
    )
{
    UNREFERENCED_PARAMETER(MiniportAdapterContext);
    UNREFERENCED_PARAMETER(RequestId);

}

NDIS_STATUS
MPDirectOidRequest(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{
    PADAPTER                    adapter = (PADAPTER)MiniportAdapterContext;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
    NDIS_OID                    oid;
    PMP_PORT                    destinationPort = NULL;

    //
    // If the adapter has been surprise removed, fail request
    //
    if (MP_TEST_ADAPTER_STATUS(adapter, MP_ADAPTER_SURPRISE_REMOVED))
    {
        ndisStatus = NDIS_STATUS_FAILURE;
        MpTrace(COMP_OID, DBG_SERIOUS, ("NDIS_OID_REQUEST failed as surprise removal is in progress\n"));
        return ndisStatus;
    }

    // Depending on the OID, hand it to the appropriate component for processing
    oid = NdisOidRequest->DATA.QUERY_INFORMATION.Oid; // Oid is at same offset for all RequestTypes

    MpTrace(COMP_OID, DBG_SERIOUS, ("Processing NDIS_OID_REQUEST for Direct OID 0x%08x\n", oid));

    //
    // Now determine if the OID is something the helper port would process or one of the
    // ports from the port list should process
    //
    switch (oid)
    {            
        default:
            {
                // All OIDs would need to get forwarded to the appropriate port
                // for processing. 

                //
                // First we would need to translate from the NDIS_PORT_NUMBER
                // to our port structure. This is done by walking the PortList
                //
                destinationPort = Port11TranslatePortNumberToPort(
                                    adapter, 
                                    NdisOidRequest->PortNumber
                                    );
                if (destinationPort == NULL)
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("Unable to find Port corresponding to PortNumber %d\n", 
                        NdisOidRequest->PortNumber));

                    ndisStatus = NDIS_STATUS_INVALID_PORT;
                }
                else
                {
                    //
                    // Pass it to the appropriate port for processing
                    //
                    ndisStatus = Port11HandleDirectOidRequest(
                                    destinationPort,
                                    NdisOidRequest
                                    );
                }
            }
            break;
    }

    if ((ndisStatus != NDIS_STATUS_SUCCESS) && (ndisStatus != NDIS_STATUS_PENDING))
    {
        MpTrace(COMP_OID, DBG_SERIOUS, ("NDIS_OID_REQUEST for Direct OID 0x%08x failed. Status = 0x%08x\n", 
            oid, ndisStatus));
    }
    else
    {
        MpTrace(COMP_OID, DBG_SERIOUS, ("NDIS_OID_REQUEST for Direct OID 0x%08x succeeded. Status = 0x%08x\n", 
            oid, ndisStatus));
    }

    return ndisStatus;
}
VOID 
MPCancelDirectOidRequest(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PVOID                   RequestId
    )
{
    UNREFERENCED_PARAMETER(MiniportAdapterContext);
    UNREFERENCED_PARAMETER(RequestId);
}

NDIS_STATUS
Mp11Scan(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port,
    _In_  PMP_SCAN_REQUEST        ScanRequest,
    _In_  PORT11_GENERIC_CALLBACK_FUNC    CompletionHandler
    )
{
    //
    // Helper port handles the scan
    //
    return HelperPortHandleScan(Adapter->HelperPort, 
                Port, 
                ScanRequest, 
                CompletionHandler
                );
}

VOID
Mp11CancelScan(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    )
{
    HelperPortCancelScan(Adapter->HelperPort, Port);
}

VOID
Mp11FlushBSSList(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    )
{
    UNREFERENCED_PARAMETER(Port);
    
    HelperPortFlushBSSList(Adapter->HelperPort);
}

PMP_BSS_LIST
Mp11QueryAndRefBSSList(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port,
    _Out_ PMP_RW_LOCK_STATE       LockState
    )
{
    // The helper port maintains the BSS list
    return HelperPortQueryAndRefBSSList(Adapter->HelperPort, Port, LockState);
}

VOID
Mp11ReleaseBSSListRef(
    _In_  PADAPTER                Adapter,
    _In_  PMP_BSS_LIST            BSSList,
    _In_  PMP_RW_LOCK_STATE       LockState
    )
{
    HelperPortReleaseBSSListRef(Adapter->HelperPort, BSSList, LockState);
}

