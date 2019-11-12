/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    base_port_oids.c

Abstract:
    Implements the OID functionality for the base port class
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "port_defs.h"
#include "base_port.h"
#include "base_port_intf.h"
#include "vnic_intf.h"
#include "glb_utils.h"

#if DOT11_TRACE_ENABLED
#include "base_port_oids.tmh"
#endif

/*
Query OIDs handled here

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
    OID_802_3_CURRENT_ADDRESS,
    OID_802_3_MULTICAST_LIST,
    
    // Operation Oids
    OID_DOT11_CURRENT_ADDRESS,
    OID_DOT11_CURRENT_OPERATION_MODE,
    OID_DOT11_CURRENT_OPTIONAL_CAPABILITY,
    OID_DOT11_DATA_RATE_MAPPING_TABLE,
    OID_DOT11_MAXIMUM_LIST_SIZE,
    OID_DOT11_MPDU_MAX_LENGTH,
    OID_DOT11_MULTICAST_LIST,
    OID_DOT11_NIC_POWER_STATE,
    OID_DOT11_OPERATION_MODE_CAPABILITY,
    OID_DOT11_OPTIONAL_CAPABILITY,
    OID_DOT11_PERMANENT_ADDRESS,
    OID_DOT11_RF_USAGE,
    OID_DOT11_SUPPORTED_DATA_RATES_VALUE,
    OID_DOT11_SUPPORTED_PHY_TYPES,
    OID_DOT11_SUPPORTED_POWER_LEVELS,
    OID_DOT11_SUPPORTED_RX_ANTENNA,
    OID_DOT11_SUPPORTED_TX_ANTENNA,

    // MIB Oids
    OID_DOT11_BEACON_PERIOD,
    OID_DOT11_CCA_MODE_SUPPORTED,
    OID_DOT11_CF_POLLABLE,
    OID_DOT11_CHANNEL_AGILITY_ENABLED,
    OID_DOT11_CHANNEL_AGILITY_PRESENT,
    OID_DOT11_COUNTRY_STRING,
    OID_DOT11_CURRENT_CCA_MODE,
    OID_DOT11_CURRENT_CHANNEL,
    OID_DOT11_CURRENT_FREQUENCY,
    OID_DOT11_CURRENT_REG_DOMAIN,
    OID_DOT11_CURRENT_TX_POWER_LEVEL,
    OID_DOT11_DIVERSITY_SELECTION_RX,
    OID_DOT11_DIVERSITY_SUPPORT,
    OID_DOT11_DSSS_OFDM_OPTION_ENABLED,
    OID_DOT11_DSSS_OFDM_OPTION_IMPLEMENTED,
    OID_DOT11_ED_THRESHOLD,
    OID_DOT11_ERP_PBCC_OPTION_ENABLED,
    OID_DOT11_ERP_PBCC_OPTION_IMPLEMENTED,
    OID_DOT11_FRAGMENTATION_THRESHOLD,
    OID_DOT11_FREQUENCY_BANDS_SUPPORTED,
    OID_DOT11_LONG_RETRY_LIMIT,
    OID_DOT11_MAC_ADDRESS,
    OID_DOT11_MAX_RECEIVE_LIFETIME,
    OID_DOT11_MAX_TRANSMIT_MSDU_LIFETIME,
    OID_DOT11_MULTI_DOMAIN_CAPABILITY_ENABLED,
    OID_DOT11_MULTI_DOMAIN_CAPABILITY_IMPLEMENTED,
    OID_DOT11_OPERATIONAL_RATE_SET,
    OID_DOT11_PBCC_OPTION_IMPLEMENTED,
    OID_DOT11_REG_DOMAINS_SUPPORT_VALUE,
    OID_DOT11_RTS_THRESHOLD,
    OID_DOT11_SHORT_PREAMBLE_OPTION_IMPLEMENTED,
    OID_DOT11_SHORT_RETRY_LIMIT,
    OID_DOT11_SHORT_SLOT_TIME_OPTION_ENABLED,
    OID_DOT11_SHORT_SLOT_TIME_OPTION_IMPLEMENTED,
    OID_DOT11_STATION_ID,
    OID_DOT11_TEMP_TYPE,



Set Oids handled here
    OID_GEN_CURRENT_LOOKAHEAD,
    OID_GEN_CURRENT_PACKET_FILTER,
    OID_GEN_INTERRUPT_MODERATION,
    OID_GEN_LINK_PARAMETERS,
    OID_802_3_MULTICAST_LIST,

    OID_DOT11_MULTICAST_LIST,
    OID_DOT11_NIC_POWER_STATE,
    OID_DOT11_SCAN_REQUEST,
    OID_DOT11_BEACON_PERIOD,
    OID_DOT11_CURRENT_CHANNEL,
    OID_DOT11_CURRENT_FREQUENCY,
    OID_DOT11_FRAGMENTATION_THRESHOLD,
    OID_DOT11_MULTI_DOMAIN_CAPABILITY_ENABLED,
    OID_DOT11_OPERATIONAL_RATE_SET,
    OID_DOT11_RTS_THRESHOLD,

*/

NDIS_STATUS
BasePortQueryInterruptModerationSettings(
    _In_  PMP_PORT                Port,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(sizeof(NDIS_INTERRUPT_MODERATION_PARAMETERS), ULONG_MAX) ULONG InformationBufferLength
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_INTERRUPT_MODERATION_PARAMETERS      intModParams;
    
    NdisZeroMemory(InformationBuffer, InformationBufferLength);

    intModParams = (PNDIS_INTERRUPT_MODERATION_PARAMETERS)InformationBuffer;        
    MP_ASSIGN_NDIS_OBJECT_HEADER(intModParams->Header, 
        NDIS_OBJECT_TYPE_DEFAULT,
        NDIS_INTERRUPT_MODERATION_PARAMETERS_REVISION_1,
        sizeof(NDIS_INTERRUPT_MODERATION_PARAMETERS)
        );

    ndisStatus = VNic11QueryInterruptModerationSettings(
                    Port->VNic, 
                    intModParams
                    );

    return ndisStatus;
}



NDIS_STATUS
BasePortQueryLinkParameters(
    _In_  PMP_PORT                Port,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(sizeof(NDIS_LINK_PARAMETERS), ULONG_MAX) ULONG InformationBufferLength
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_LINK_PARAMETERS       linkParams;
    
    NdisZeroMemory(InformationBuffer, InformationBufferLength);

    linkParams = (PNDIS_LINK_PARAMETERS)InformationBuffer;

    MP_ASSIGN_NDIS_OBJECT_HEADER(linkParams->Header, 
        NDIS_OBJECT_TYPE_DEFAULT,
        NDIS_LINK_PARAMETERS_REVISION_1,
        sizeof(NDIS_LINK_PARAMETERS)
        );

    ndisStatus = VNic11QueryLinkParameters(
                    Port->VNic, 
                    linkParams
                    );

    return ndisStatus;
}

NDIS_STATUS
BasePortQueryCurrentOperationMode(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   OpModeBuffer
    )
{
    PDOT11_CURRENT_OPERATION_MODE dot11CurrentOperationMode = (PDOT11_CURRENT_OPERATION_MODE)OpModeBuffer;
    
    NdisMoveMemory(&(dot11CurrentOperationMode->uCurrentOpMode), &(Port->CurrentOpMode), sizeof(ULONG));

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
BasePortQueryDataRateMappingTable(
    _In_  PMP_PORT                Port,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_DATA_RATE_MAPPING_TABLE), ULONG_MAX) ULONG InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_DATA_RATE_MAPPING_TABLE  dataRateMappingTable = NULL;
    
    *BytesWritten = 0;
    *BytesNeeded = 0;

    NdisZeroMemory(InformationBuffer, InformationBufferLength);

    dataRateMappingTable = (PDOT11_DATA_RATE_MAPPING_TABLE)InformationBuffer;

    MP_ASSIGN_NDIS_OBJECT_HEADER(dataRateMappingTable->Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_DATA_RATE_MAPPING_TABLE_REVISION_1,
                                 sizeof(DOT11_DATA_RATE_MAPPING_TABLE));

    // Get the rate set from the VNIC
    ndisStatus = VNic11QueryDataRateMappingTable(
                    Port->VNic,
                    dataRateMappingTable,
                    InformationBufferLength
                    );    

    *BytesWritten = dataRateMappingTable->uDataRateMappingLength * sizeof(DOT11_DATA_RATE_MAPPING_ENTRY) + 
        FIELD_OFFSET(DOT11_DATA_RATE_MAPPING_TABLE, DataRateMappingEntries);
        
    *BytesNeeded = dataRateMappingTable->uDataRateMappingLength * sizeof(DOT11_DATA_RATE_MAPPING_ENTRY) +
        FIELD_OFFSET(DOT11_DATA_RATE_MAPPING_TABLE, DataRateMappingEntries);

    return ndisStatus;
}


NDIS_STATUS
BasePortQuerySupportedPHYTypes(
    _In_  PMP_PORT                Port,
    _Inout_ PVOID                InformationBuffer,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_SUPPORTED_PHY_TYPES  dot11SupportedPhyTypes = InformationBuffer;
    ULONG                       numMaxEntries = 0;

    InformationBufferLength -= FIELD_OFFSET(DOT11_SUPPORTED_PHY_TYPES, dot11PHYType);
    numMaxEntries = InformationBufferLength / sizeof(DOT11_PHY_TYPE);

    ndisStatus = VNic11QuerySupportedPHYTypes(Port->VNic, numMaxEntries, dot11SupportedPhyTypes);

    *BytesWritten = FIELD_OFFSET(DOT11_SUPPORTED_PHY_TYPES, dot11PHYType) +
                    dot11SupportedPhyTypes->uNumOfEntries * sizeof(DOT11_PHY_TYPE);
    
    *BytesNeeded = FIELD_OFFSET(DOT11_SUPPORTED_PHY_TYPES, dot11PHYType) +
                    dot11SupportedPhyTypes->uTotalNumOfEntries * sizeof(DOT11_PHY_TYPE);
    
    return ndisStatus;
}

NDIS_STATUS
BasePortQuerySupportedPowerLevels(
    _In_  PMP_PORT                Port,
    _Inout_ PVOID                InformationBuffer,
    _Out_ PULONG                  BytesWritten
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    PDOT11_SUPPORTED_POWER_LEVELS dot11SupportedPowerLevels = InformationBuffer;
    
    ndisStatus = VNic11QuerySupportedPowerLevels(Port->VNic, dot11SupportedPowerLevels);

    *BytesWritten = FIELD_OFFSET(DOT11_SUPPORTED_POWER_LEVELS, uTxPowerLevelValues) + 
        dot11SupportedPowerLevels->uNumOfSupportedPowerLevels * sizeof(ULONG);
    
    return ndisStatus;
}


NDIS_STATUS
BasePortQuerySupportedRXAntenna(
    _In_  PMP_PORT                Port,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(>=, sizeof(DOT11_SUPPORTED_ANTENNA_LIST)) ULONG InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_SUPPORTED_ANTENNA_LIST dot11SupportedAntennaList = InformationBuffer;
    ULONG                       numMaxEntries = 0;

    NdisZeroMemory(InformationBuffer, InformationBufferLength);

    InformationBufferLength -= FIELD_OFFSET(DOT11_SUPPORTED_ANTENNA_LIST, dot11SupportedAntenna);
    numMaxEntries = InformationBufferLength / sizeof(DOT11_SUPPORTED_ANTENNA);

    _Analysis_assume_(InformationBufferLength >= sizeof(DOT11_SUPPORTED_ANTENNA_LIST)); // remind PREFast and so work around a bug

    ndisStatus = VNic11QuerySupportedRXAntenna(Port->VNic, numMaxEntries, dot11SupportedAntennaList);

    *BytesWritten = FIELD_OFFSET(DOT11_SUPPORTED_ANTENNA_LIST, dot11SupportedAntenna) +
                    dot11SupportedAntennaList->uNumOfEntries * sizeof(DOT11_SUPPORTED_ANTENNA);
    *BytesNeeded = FIELD_OFFSET(DOT11_SUPPORTED_ANTENNA_LIST, dot11SupportedAntenna) +
                    dot11SupportedAntennaList->uTotalNumOfEntries * sizeof(DOT11_SUPPORTED_ANTENNA);

    return ndisStatus;
}



NDIS_STATUS
BasePortQuerySupportedTXAntenna(
    _In_  PMP_PORT                Port,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(>=, sizeof(DOT11_SUPPORTED_ANTENNA_LIST)) ULONG InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_SUPPORTED_ANTENNA_LIST dot11SupportedAntennaList = InformationBuffer;
    ULONG                       numMaxEntries = 0;

    NdisZeroMemory(InformationBuffer, InformationBufferLength);

    InformationBufferLength -= FIELD_OFFSET(DOT11_SUPPORTED_ANTENNA_LIST, dot11SupportedAntenna);
    numMaxEntries = InformationBufferLength / sizeof(DOT11_SUPPORTED_ANTENNA);

    _Analysis_assume_(InformationBufferLength >= sizeof(DOT11_SUPPORTED_ANTENNA_LIST)); // remind PREFast and so work around a bug

    ndisStatus = VNic11QuerySupportedTXAntenna(Port->VNic, numMaxEntries, dot11SupportedAntennaList);

    *BytesWritten = FIELD_OFFSET(DOT11_SUPPORTED_ANTENNA_LIST, dot11SupportedAntenna) +
                    dot11SupportedAntennaList->uNumOfEntries * sizeof(DOT11_SUPPORTED_ANTENNA);
    *BytesNeeded = FIELD_OFFSET(DOT11_SUPPORTED_ANTENNA_LIST, dot11SupportedAntenna) +
                    dot11SupportedAntennaList->uTotalNumOfEntries * sizeof(DOT11_SUPPORTED_ANTENNA);

    return ndisStatus;
}


NDIS_STATUS
BasePortQueryDiversitySelectionRX(
    _In_  PMP_PORT                Port,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(>=, sizeof(DOT11_DIVERSITY_SELECTION_RX_LIST)) ULONG InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_DIVERSITY_SELECTION_RX_LIST dot11DiversitySelectionRXList = InformationBuffer;
    ULONG                       numMaxEntries = 0;

    NdisZeroMemory(InformationBuffer, InformationBufferLength);

    InformationBufferLength -= FIELD_OFFSET(DOT11_DIVERSITY_SELECTION_RX_LIST, dot11DiversitySelectionRx);
    numMaxEntries = InformationBufferLength / sizeof(DOT11_DIVERSITY_SELECTION_RX);

    _Analysis_assume_(InformationBufferLength >= sizeof(DOT11_DIVERSITY_SELECTION_RX_LIST)); // remind PREFast and so work around a bug

    ndisStatus = VNic11QueryDiversitySelectionRX(Port->VNic, 
                    TRUE,
                    numMaxEntries, 
                    dot11DiversitySelectionRXList
                    );

    *BytesWritten = FIELD_OFFSET(DOT11_DIVERSITY_SELECTION_RX_LIST, dot11DiversitySelectionRx) +
                    dot11DiversitySelectionRXList->uNumOfEntries * sizeof(DOT11_DIVERSITY_SELECTION_RX);
    *BytesNeeded = FIELD_OFFSET(DOT11_DIVERSITY_SELECTION_RX_LIST, dot11DiversitySelectionRx) +
                    dot11DiversitySelectionRXList->uTotalNumOfEntries * sizeof(DOT11_DIVERSITY_SELECTION_RX);

    return ndisStatus;
}

NDIS_STATUS
BasePortQueryRegDomainsSupportValue(
    _In_  PMP_PORT                Port,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(>=, sizeof(DOT11_REG_DOMAINS_SUPPORT_VALUE)) ULONG InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_REG_DOMAINS_SUPPORT_VALUE dot11RegDomainsSupportValue = InformationBuffer;
    ULONG                       numMaxEntries = 0;

    NdisZeroMemory(InformationBuffer, InformationBufferLength);

    InformationBufferLength -= FIELD_OFFSET(DOT11_REG_DOMAINS_SUPPORT_VALUE, dot11RegDomainValue);
    numMaxEntries = InformationBufferLength / sizeof(DOT11_REG_DOMAIN_VALUE);

    _Analysis_assume_(InformationBufferLength >= sizeof(DOT11_REG_DOMAINS_SUPPORT_VALUE)); // remind PREFast and so work around a bug

    ndisStatus = VNic11QueryRegDomainsSupportValue(Port->VNic, TRUE, numMaxEntries, dot11RegDomainsSupportValue);

    *BytesWritten = dot11RegDomainsSupportValue->uNumOfEntries * sizeof(DOT11_REG_DOMAIN_VALUE) +
                    FIELD_OFFSET(DOT11_REG_DOMAINS_SUPPORT_VALUE, dot11RegDomainValue);
    *BytesNeeded = dot11RegDomainsSupportValue->uTotalNumOfEntries * sizeof(DOT11_REG_DOMAIN_VALUE) +
                    FIELD_OFFSET(DOT11_REG_DOMAINS_SUPPORT_VALUE, dot11RegDomainValue);

    return ndisStatus;
}




NDIS_STATUS
BasePortSetInterruptModerationSettings(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   InformationBuffer
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_INTERRUPT_MODERATION_PARAMETERS   intModParams;
    
    do
    {
        intModParams = (PNDIS_INTERRUPT_MODERATION_PARAMETERS)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(intModParams->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                NDIS_INTERRUPT_MODERATION_PARAMETERS_REVISION_1,
                sizeof(NDIS_INTERRUPT_MODERATION_PARAMETERS)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Unsupported NDIS_OBJECT_HEADER for OID_GEN_INTERRUPT_MODERATION\n"));        
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        ndisStatus = VNic11SetInterruptModerationSettings(
                        Port->VNic, 
                        intModParams
                        );
    } while(FALSE);

    return ndisStatus;
}


NDIS_STATUS
BasePortSetLinkParameters(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   InformationBuffer
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_LINK_PARAMETERS       linkParams;
    
    do
    {
        linkParams = (PNDIS_LINK_PARAMETERS)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(linkParams->Header, 
                                          NDIS_OBJECT_TYPE_DEFAULT,
                                          NDIS_LINK_PARAMETERS_REVISION_1,
                                          sizeof(NDIS_LINK_PARAMETERS)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Unsupported NDIS_OBJECT_HEADER for OID_GEN_LINK_PARAMETERS\n"));        
        
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        ndisStatus = VNic11SetLinkParameters(
                        Port->VNic, 
                        linkParams
                        );
    } while(FALSE);

    return ndisStatus;
}

NDIS_STATUS
BasePortSetMulticastList(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   InformationBuffer,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesRead,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = sizeof(DOT11_MAC_ADDRESS);
        
        if (InformationBufferLength % sizeof(DOT11_MAC_ADDRESS)) 
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Multicast list buffer for set of OID MULTICAST_LIST "
                "not a multiple of sizeof(DOT11_MAC_ADDRESS)\n"));        
        
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        //
        // Verify that we can hold the multicast list
        //
        if (InformationBufferLength > (HW11_MAX_MULTICAST_LIST_SIZE * sizeof(DOT11_MAC_ADDRESS))) 
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Multicast list buffer for set of OID MULTICAST_LIST "
                "contains more entries than supported by this miniport\n"));        
            ndisStatus = NDIS_STATUS_MULTICAST_FULL;
            *BytesNeeded = HW11_MAX_MULTICAST_LIST_SIZE * sizeof(DOT11_MAC_ADDRESS);
            break;
        }

        *BytesRead = InformationBufferLength;

        //
        // Transfer the new multicast list to the card.
        // We need to serialize the call.
        //
        ndisStatus = VNic11SetMulticastList(
                        Port->VNic,
                        InformationBuffer,
                        InformationBufferLength
                        );
                    
    } while (FALSE);

    return ndisStatus;
}


NDIS_STATUS
BasePortSetOperationalRateSet(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   InformationBuffer,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesRead,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_RATE_SET             dot11RateSet = NULL;
    ULONG                       requiredSize = 0;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        requiredSize = FIELD_OFFSET(DOT11_RATE_SET, ucRateSet);
        
        dot11RateSet = InformationBuffer;
        if (dot11RateSet->uRateSetLength > DOT11_RATE_SET_MAX_LENGTH ||
            dot11RateSet->uRateSetLength == 0)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Operation Rates list for set of OID_DOT11_OPERATIONAL_RATE_SET "
                "contains invalid number of entries (> DOT11_RATE_SET_MAX_LENGTH or 0)\n"));        
        
            *BytesNeeded = requiredSize;
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        requiredSize += dot11RateSet->uRateSetLength;
        if (InformationBufferLength < requiredSize)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Mismatch between number of rates in operation rates list for "
                "OID_DOT11_OPERATIONAL_RATE_SET and information buffer length\n"));
        
            *BytesNeeded = requiredSize;
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        *BytesRead = requiredSize;
        
        ndisStatus = VNic11SetOperationalRateSet(Port->VNic, dot11RateSet, TRUE);
        
    } while(FALSE);

    return ndisStatus;
}


NDIS_STATUS
BasePortValidateScanRequest(
    _In_  PMP_PORT                Port,
    _In_  PDOT11_SCAN_REQUEST_V2  Dot11ScanRequest
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PHY_TYPE_INFO        dot11PhyTypeInfo;
    ULONG                       i, bytesParsed = 0;
    PDOT11_SSID                 dot11SSID;

    UNREFERENCED_PARAMETER(Port);
    
    //
    // Perform some validation on the scan request.
    //
    do
    {
        for (i=0; i<Dot11ScanRequest->uNumOfdot11SSIDs; i++)
        {
            dot11SSID = (PDOT11_SSID) (Dot11ScanRequest->ucBuffer + Dot11ScanRequest->udot11SSIDsOffset + bytesParsed);
            if (dot11SSID->uSSIDLength > DOT11_SSID_MAX_LENGTH)
            {
                MpTrace(COMP_SCAN, DBG_SERIOUS, ("The SSID length provided (%d) is greater than max SSID length (%d)\n", dot11SSID->uSSIDLength, DOT11_SSID_MAX_LENGTH));
                ndisStatus = NDIS_STATUS_INVALID_LENGTH;
                break;
            }
            bytesParsed += sizeof(DOT11_SSID);
        }
        
        if (Dot11ScanRequest->dot11BSSType != dot11_BSS_type_infrastructure &&
            Dot11ScanRequest->dot11BSSType != dot11_BSS_type_independent &&
            Dot11ScanRequest->dot11BSSType != dot11_BSS_type_any)
        {
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("BSS Type %d not supported\n", Dot11ScanRequest->dot11BSSType));
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
        }

        switch (Dot11ScanRequest->dot11ScanType)
        {
            case dot11_scan_type_active:
            case dot11_scan_type_active | dot11_scan_type_forced:
            case dot11_scan_type_passive:
            case dot11_scan_type_passive | dot11_scan_type_forced:
            case dot11_scan_type_auto:
            case dot11_scan_type_auto | dot11_scan_type_forced:
                break;

            default:
                MpTrace(COMP_SCAN, DBG_SERIOUS, ("Dot11 scan type %d not supported\n", Dot11ScanRequest->dot11ScanType));
                ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
                break;
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        bytesParsed = 0;
        
        for(i=0; i<Dot11ScanRequest->uNumOfPhyTypeInfos; i++)
        {
            dot11PhyTypeInfo = (PDOT11_PHY_TYPE_INFO) 
                (Dot11ScanRequest->ucBuffer + Dot11ScanRequest->uPhyTypeInfosOffset + bytesParsed);

            // ExtSTA mode, the OS does not control PHY specific parameters
            MPASSERT(dot11PhyTypeInfo->bUseParameters == FALSE);
            bytesParsed += (FIELD_OFFSET(DOT11_PHY_TYPE_INFO, ucChannelListBuffer) + dot11PhyTypeInfo->uChannelListSize);
        }
    } while (FALSE);

    return ndisStatus;
}

// ENTRY lock held
NDIS_STATUS
BasePortCopyEnumBSSIEBuffer(
    _In_ PMP_BSS_ENTRY       pStaBSSEntry,
    _Out_writes_bytes_opt_(DestinationLength) PUCHAR               pDestBuffer,
    _In_ ULONG                DestinationLength,
    _Out_ PULONG               pBytesWritten,
    _Out_ PULONG               pBytesNeeded
    )
{
    PUCHAR  PrimaryIEBlob = NULL;
    ULONG   SizeOfPrimaryIEBlob = 0;
    PUCHAR  SecondaryIEBlob = NULL;
    ULONG   SizeOfSecondaryIEBlob = 0;
    ULONG   IEEntrySize = 0;
    PDOT11_INFO_ELEMENT pInfoElemHdr = NULL, pTempIEHdr;
    PUCHAR  pVendorIEData = NULL;
    BOOLEAN bCopyIEEntry = FALSE;
    BOOLEAN bSSIDFound = FALSE;
    DOT11_SSID      probeResponseSSID;
    NDIS_STATUS     ndisStatus = NDIS_STATUS_SUCCESS;

    *pBytesWritten = 0;
    *pBytesNeeded = 0;
    probeResponseSSID.uSSIDLength = 0;

    //
    // Our logic below is to copy all the IEs from the most recently received IE blob. 
    // One additional check is if beacon blob does not have the SSID, we will copy the SSID from 
    // the probe response blob before copying rest of data from the beacon. At the end, we will 
    // copy the WCN IEs from the second IE blob into the return list. 
    //

    // 
    // Use the latest IE blob as our primary blob
    //
    PrimaryIEBlob = pStaBSSEntry->pDot11InfoElemBlob;
    SizeOfPrimaryIEBlob = pStaBSSEntry->InfoElemBlobSize;

    //
    // Determine which is the secondary blob
    //
    if ((pStaBSSEntry->BeaconFrameSize > FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements)) &&
        (PrimaryIEBlob == 
            (pStaBSSEntry->pDot11BeaconFrame + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements)))
       )
    {
        //
        // Latest IE blob is the beacon. The secondary blob would be the probe response (if available)
        //
        if (pStaBSSEntry->ProbeFrameSize > FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements))
        {
            SecondaryIEBlob = pStaBSSEntry->pDot11ProbeFrame 
                                    + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);
            SizeOfSecondaryIEBlob = pStaBSSEntry->ProbeFrameSize 
                                    - FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);
            
            //
            // Get the SSID IE from the probe response for use with hidden networks
            //
            ndisStatus = Dot11CopySSIDFromMemoryBlob(
                SecondaryIEBlob,
                SizeOfSecondaryIEBlob,
                &probeResponseSSID
                );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                // Couldnt find SSID IE in the probe response
                probeResponseSSID.uSSIDLength = 0;
                ndisStatus = NDIS_STATUS_SUCCESS;
            }        
        }
    }
    else
    {
        //
        // Lastest IE blob is the probe response. Check if the beacon is present & use that
        // as the secondary blob (for copying the WCN IEs)
        //
        if (pStaBSSEntry->BeaconFrameSize >= FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements))
        {
            SecondaryIEBlob = pStaBSSEntry->pDot11BeaconFrame 
                                    + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);
            SizeOfSecondaryIEBlob = pStaBSSEntry->BeaconFrameSize 
                                    - FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);
        }
    }

    // First walk the primary IE blob & copy all possible IEs
    while(SizeOfPrimaryIEBlob > 0) 
    {
        pInfoElemHdr = (PDOT11_INFO_ELEMENT)PrimaryIEBlob;
        if ((SizeOfPrimaryIEBlob < sizeof(DOT11_INFO_ELEMENT)) ||
            ((pInfoElemHdr->Length + sizeof(DOT11_INFO_ELEMENT)) > SizeOfPrimaryIEBlob))
        {
            // Shouldnt happen. The IE's must already be verified
            break;
        }

        // Copy the entry from beacon IE blob
        bCopyIEEntry = TRUE;

        switch (pInfoElemHdr->ElementID)
        {
            case DOT11_INFO_ELEMENT_ID_SSID:
                {
                    //
                    // For SSID, we copy cached Probe SSID if we do not have a SSID
                    // in the Beacon. This is to handle hidden SSID. For hidden SSID, 
                    // the OS could first do a scan and expect us to indicate the 
                    // found SSID so that it can do a connect. We copy the probe 
                    // response SSID instead of the hidden one from the beacon
                    //
                    bSSIDFound = TRUE;

                    if (Dot11IsHiddenSSID((((PUCHAR)pInfoElemHdr) + sizeof(DOT11_INFO_ELEMENT)), pInfoElemHdr->Length))
                    {
                        // Hidden SSID, check probe response
                        if (probeResponseSSID.uSSIDLength != 0)
                        {
                            // Will use probe response SSID and not copy source SSID IE
                            bCopyIEEntry = FALSE;

                            IEEntrySize = sizeof(DOT11_INFO_ELEMENT) + 
                                probeResponseSSID.uSSIDLength;
                            *pBytesNeeded += IEEntrySize;

                            MpTrace(COMP_SCAN, DBG_LOUD, ("Will use Probe SSID for %02X-%02X-%02X-%02X-%02X-%02X\n", 
                                    pStaBSSEntry->Dot11BSSID[0], pStaBSSEntry->Dot11BSSID[1], pStaBSSEntry->Dot11BSSID[2], 
                                    pStaBSSEntry->Dot11BSSID[3], pStaBSSEntry->Dot11BSSID[4], pStaBSSEntry->Dot11BSSID[5]));

                            if (IEEntrySize > DestinationLength)
                            {
                                ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
                                
                                // Stop copying, but continue going through the IEs
                                DestinationLength = 0;
                            }
                            else
                            {
                                pTempIEHdr = (PDOT11_INFO_ELEMENT)pDestBuffer;

                                pTempIEHdr->ElementID = DOT11_INFO_ELEMENT_ID_SSID;
                                pTempIEHdr->Length = (UCHAR)probeResponseSSID.uSSIDLength;

                                NdisMoveMemory(pDestBuffer + sizeof(DOT11_INFO_ELEMENT), 
                                    probeResponseSSID.ucSSID, 
                                    probeResponseSSID.uSSIDLength
                                    );

                                DestinationLength -= IEEntrySize;
                                pDestBuffer += IEEntrySize;
                                *pBytesWritten += IEEntrySize;
                            }
                        }
                    }
                }
                break;
                
            default:
                //
                // For everything else we copy what is in the
                // primary IE blob
                //

                //
                // The SSID IE should be first. If it has not yet been found
                // copy the SSID from the probe (if available) before copying the
                // other IEs
                //
                if (bSSIDFound == FALSE)
                {
                    bSSIDFound = TRUE;
                    if (probeResponseSSID.uSSIDLength != 0)
                    {
                        IEEntrySize = sizeof(DOT11_INFO_ELEMENT) + 
                            probeResponseSSID.uSSIDLength;
                        *pBytesNeeded += IEEntrySize;

                        MpTrace(COMP_SCAN, DBG_LOUD, ("Will use Probe SSID for %02X-%02X-%02X-%02X-%02X-%02X\n", 
                                pStaBSSEntry->Dot11BSSID[0], pStaBSSEntry->Dot11BSSID[1], pStaBSSEntry->Dot11BSSID[2], 
                                pStaBSSEntry->Dot11BSSID[3], pStaBSSEntry->Dot11BSSID[4], pStaBSSEntry->Dot11BSSID[5]));

                        if (IEEntrySize > DestinationLength)
                        {
                            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
                            // Stop copying, but continue going through the IEs
                            DestinationLength = 0;                            
                        }
                        else
                        {
                            pTempIEHdr = (PDOT11_INFO_ELEMENT)pDestBuffer;

                            pTempIEHdr->ElementID = DOT11_INFO_ELEMENT_ID_SSID;
                            pTempIEHdr->Length = (UCHAR)probeResponseSSID.uSSIDLength;

                            NdisMoveMemory(pDestBuffer + sizeof(DOT11_INFO_ELEMENT), 
                                probeResponseSSID.ucSSID, 
                                probeResponseSSID.uSSIDLength
                                );

                            DestinationLength -= IEEntrySize;
                            pDestBuffer += IEEntrySize;
                            
                            *pBytesWritten += IEEntrySize;  
                            
                            // We still copy the original IE
                        }
                    }
                }

                break;
        }

        //
        // Copy/skip past IE in original buffer
        // 
        IEEntrySize = sizeof(DOT11_INFO_ELEMENT) + 
            pInfoElemHdr->Length;

        if (bCopyIEEntry)
        {
            *pBytesNeeded += IEEntrySize;
            
            if (IEEntrySize > DestinationLength)
            {
                ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
                DestinationLength = 0;
            }
            else
            {
                //
                // Copy IE from source buffer to destination
                //
                NdisMoveMemory(pDestBuffer, PrimaryIEBlob, IEEntrySize);

                DestinationLength -= IEEntrySize;
                pDestBuffer += IEEntrySize;
                *pBytesWritten += IEEntrySize;
            }
        }

        // Move forward in the beacon IE
        SizeOfPrimaryIEBlob -= IEEntrySize;
        PrimaryIEBlob += IEEntrySize;
    }


    //
    // Now walk the secondary IEs and copy the WCN IEs. We dont 
    // attempt to merge these, but just place them at the end
    //
    while(SizeOfSecondaryIEBlob > 0) 
    {
        pInfoElemHdr = (PDOT11_INFO_ELEMENT)SecondaryIEBlob;
        if ((SizeOfSecondaryIEBlob < sizeof(DOT11_INFO_ELEMENT)) ||
            ((pInfoElemHdr->Length + sizeof(DOT11_INFO_ELEMENT)) > SizeOfSecondaryIEBlob))
        {
            // Shouldnt happen. The IE's must already be verified
            break;
        }

        // Dont copy all the entries from IE blob
        bCopyIEEntry = FALSE;

        switch (pInfoElemHdr->ElementID)
        {
            case DOT11_INFO_ELEMENT_ID_VENDOR_SPECIFIC:
                {
                    //
                    // Check the vendor specific IE type
                    // WCN IE contains 4 bytes WCN_IE_TAG (0x04f25000) at the very beginning of the IE data
                    // If we don't find the tag, it's not a WCN IE & we skip this IE
                    //
                    if (pInfoElemHdr->Length >= 4) 
                    {
                        pVendorIEData = SecondaryIEBlob + sizeof(DOT11_INFO_ELEMENT);
                        if (*((ULONG UNALIGNED *)pVendorIEData) == WCN_IE_TAG)
                        {
                            bCopyIEEntry = TRUE;
                        }
                    }                    
                }
                break;
                
            default:
                //
                // For everything else we dont copy what is in the
                // probe response
                //
                bCopyIEEntry = FALSE;
                break;
        }

        //
        // Copy/skip past IE in original buffer
        // 
        IEEntrySize = sizeof(DOT11_INFO_ELEMENT) + 
            pInfoElemHdr->Length;

        if (bCopyIEEntry)
        {
            *pBytesNeeded += IEEntrySize;
            
            if (IEEntrySize > DestinationLength)
            {
                ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
                DestinationLength = 0;
            }
            else
            {
                //
                // Copy IE from source buffer to destination
                //
                NdisMoveMemory(pDestBuffer, SecondaryIEBlob, IEEntrySize);

                DestinationLength -= IEEntrySize;
                pDestBuffer += IEEntrySize;
                *pBytesWritten += IEEntrySize;
            }
        }

        // Move forward in the IE blob
        SizeOfSecondaryIEBlob -= IEEntrySize;
        SecondaryIEBlob += IEEntrySize;
    }

    return ndisStatus;
}

// ENTRY lock held
ULONG
BasePortGetEnumBSSIELength(
    _In_ PMP_BSS_ENTRY       pStaBSSEntry
    )
{
    ULONG                   bytesNeeded = 0;
    ULONG                   bytesWritten = 0;

    // Use the copy routine with a zero length output buffer to get the length
    BasePortCopyEnumBSSIEBuffer(pStaBSSEntry, 
        NULL,
        0,
        &bytesWritten,
        &bytesNeeded
        );
    
    return bytesNeeded;

}

NDIS_STATUS
BasePortCopyBSSList(
    _In_  PMP_PORT                Port,
    _In_  DOT11_COUNTRY_OR_REGION_STRING  CountryRegionString,
    _In_  ULONG                   ExpireTime,          // Max entry age in 100 nano-seconds
    _Inout_updates_bytes_(TotalLength)
          PDOT11_BYTE_ARRAY    Dot11ByteArray,
    _In_  ULONG                   TotalLength    
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PLIST_ENTRY         pHead = NULL, pEntry = NULL;
    PMP_BSS_ENTRY      pStaBSSEntry = NULL;
    MP_RW_LOCK_STATE          LockState;
    ULONG               RemainingBytes = 0;
    ULONG               BSSEntrySize = 0;
    ULONG               BytesNeeded = 0;
    PDOT11_BSS_ENTRY    pDot11BSSEntry = NULL;
    PUCHAR              pCurrPtr = Dot11ByteArray->ucBuffer;
    ULONGLONG           ullOldestAllowedEntry;
    ULONG               IEBlobSize;
    PMP_BSS_LIST       pDiscoveredBSSList;

    UNREFERENCED_PARAMETER(CountryRegionString);

    MP_ASSIGN_NDIS_OBJECT_HEADER(Dot11ByteArray->Header, 
        NDIS_OBJECT_TYPE_DEFAULT,
        DOT11_BSS_ENTRY_BYTE_ARRAY_REVISION_1,
        sizeof(DOT11_BYTE_ARRAY));
        

    Dot11ByteArray->uNumOfBytes = 0;
    Dot11ByteArray->uTotalNumOfBytes = 0;

    //
    // Obtain a reference to the global BSS list
    //   
    pDiscoveredBSSList = Mp11QueryAndRefBSSList(
                            Port->Adapter, 
                            Port,
                            &LockState
                            );
    if (pDiscoveredBSSList == NULL)
    {
        // Empty list
        return NDIS_STATUS_SUCCESS;
    }

    //
    // Determine time to use for expiring AP's
    //
    NdisGetCurrentSystemTime((PLARGE_INTEGER)&ullOldestAllowedEntry);
    if (ExpireTime <= ullOldestAllowedEntry)
        ullOldestAllowedEntry -= ExpireTime;

    RemainingBytes = TotalLength 
        - FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer);

    pHead = &(pDiscoveredBSSList->List);
    pEntry = pHead->Flink;
    while(pEntry != pHead) 
    {
        pStaBSSEntry = CONTAINING_RECORD(pEntry, MP_BSS_ENTRY, Link);
        pEntry = pEntry->Flink;

        NdisAcquireSpinLock(&(pStaBSSEntry->Lock));
        
        //
        // Ignore stale entries
        //
        if (pStaBSSEntry->HostTimestamp < ullOldestAllowedEntry)
        {
            NdisReleaseSpinLock(&(pStaBSSEntry->Lock));
            continue;
        }

        //
        // Determine number of bytes needed for writing this entry
        //
        BSSEntrySize = FIELD_OFFSET(DOT11_BSS_ENTRY, ucBuffer)
            + BasePortGetEnumBSSIELength(pStaBSSEntry);

        Dot11ByteArray->uTotalNumOfBytes += BSSEntrySize;

        if (RemainingBytes >= BSSEntrySize)
        {
            //
            // Copy this AP information to caller buffer
            //
            pDot11BSSEntry = (PDOT11_BSS_ENTRY)pCurrPtr;

            pDot11BSSEntry->uPhyId = pStaBSSEntry->PhyId;
            pDot11BSSEntry->usBeaconPeriod = pStaBSSEntry->BeaconInterval;
            pDot11BSSEntry->ullTimestamp = pStaBSSEntry->BeaconTimestamp;
            pDot11BSSEntry->ullHostTimestamp = pStaBSSEntry->HostTimestamp;
            pDot11BSSEntry->dot11BSSType = pStaBSSEntry->Dot11BSSType;
            pDot11BSSEntry->usCapabilityInformation = pStaBSSEntry->Dot11Capability.usValue;
            pDot11BSSEntry->lRSSI = pStaBSSEntry->RSSI;
            pDot11BSSEntry->uLinkQuality = pStaBSSEntry->LinkQuality;
            pDot11BSSEntry->PhySpecificInfo.uChCenterFrequency 
                = pStaBSSEntry->ChannelCenterFrequency;

            //
            // NOTE: We assume that we are always in regulatory domain
            //
            pDot11BSSEntry->bInRegDomain = TRUE;
            
            NdisMoveMemory(
                pDot11BSSEntry->dot11BSSID,
                pStaBSSEntry->Dot11BSSID,
                sizeof(DOT11_MAC_ADDRESS)
                );

            BSSEntrySize = FIELD_OFFSET(DOT11_BSS_ENTRY, ucBuffer);
            Dot11ByteArray->uNumOfBytes += BSSEntrySize;
            pCurrPtr+= BSSEntrySize;
            RemainingBytes -= BSSEntrySize;

            //
            // Copy the IEs
            //
            ndisStatus = BasePortCopyEnumBSSIEBuffer(
                pStaBSSEntry,
                pDot11BSSEntry->ucBuffer,
                RemainingBytes,
                &IEBlobSize,
                &BytesNeeded
                );

            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                pDot11BSSEntry->uBufferLength = IEBlobSize;

                Dot11ByteArray->uNumOfBytes += IEBlobSize;
                pCurrPtr+= IEBlobSize;
                RemainingBytes -= IEBlobSize;
            }
            else
            {
                pDot11BSSEntry->uBufferLength = 0;
                RemainingBytes = 0;
                //
                // We continue walking through the list to determine the total
                // space required for this OID
                //
            }
        }
        else
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            RemainingBytes = 0;
            //
            // We continue walking through the list to determine the total
            // space required for this OID
            //
        }
        NdisReleaseSpinLock(&(pStaBSSEntry->Lock));
    }

    Mp11ReleaseBSSListRef(Port->Adapter, 
        pDiscoveredBSSList, 
        &LockState
        );

    return ndisStatus;
}


ULONG
BasePortGetPhyIdFromType(
    _In_  PMP_PORT                Port,
    _In_  DOT11_PHY_TYPE          PhyType
    )
{
    ULONG                       index;
    PDOT11_SUPPORTED_PHY_TYPES  supportedPhyTypes;
    UCHAR                       buffer[(sizeof(DOT11_SUPPORTED_PHY_TYPES) + \
                                 sizeof(DOT11_PHY_TYPE) * HW11_MAX_PHY_COUNT)];  
    //
    // Get supported PHY types.
    //
    supportedPhyTypes = (PDOT11_SUPPORTED_PHY_TYPES) buffer;
    supportedPhyTypes->uNumOfEntries = 0;
    VNic11QuerySupportedPHYTypes(PORT_GET_VNIC(Port), 
                               HW11_MAX_PHY_COUNT, 
                               supportedPhyTypes);

    //
    // Go through the list to find the matching type
    //
    for (index = 0; index < supportedPhyTypes->uNumOfEntries; index++)
    {
        if (PhyType == supportedPhyTypes->dot11PHYType[index])
            return (index);
    }

    // 
    // No match, return an invalid PhyId value.
    //
    return DOT11_PHY_ID_ANY;
}


DOT11_PHY_TYPE
BasePortGetPhyTypeFromId(
    _In_  PMP_PORT                Port,
    _In_  ULONG                   PhyId
    )
{
    PDOT11_SUPPORTED_PHY_TYPES  supportedPhyTypes;
    UCHAR                       buffer[(sizeof(DOT11_SUPPORTED_PHY_TYPES) + \
                                 sizeof(DOT11_PHY_TYPE) * HW11_MAX_PHY_COUNT)];  

    //
    // Get supported PHY types.
    //
    supportedPhyTypes = (PDOT11_SUPPORTED_PHY_TYPES) buffer;
    supportedPhyTypes->uNumOfEntries = 0;
    VNic11QuerySupportedPHYTypes(PORT_GET_VNIC(Port), 
                               HW11_MAX_PHY_COUNT, 
                               supportedPhyTypes);

    //
    // Validate PhyId 
    //
    if (PhyId >= supportedPhyTypes->uNumOfEntries)
        return dot11_phy_type_unknown;

    //
    // Return the phy type
    //
    return supportedPhyTypes->dot11PHYType[PhyId];
}


NDIS_STATUS
BasePortQueryInformation(
    _In_  PMP_PORT                Port,
    _In_  NDIS_OID                Oid,
    PVOID                         InformationBuffer,    // Required length is verified by port_oids
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       ulongInfo = 0;
    BOOLEAN                     boolInfo = FALSE;

    // The length for all OIDs has already been verified using the OID table in port_oids.c
    UNREFERENCED_PARAMETER(InformationBufferLength);

    // Initialize the result
    *BytesWritten = 0;
    *BytesNeeded = 0;

    MpTrace(COMP_OID, DBG_TRACE,  ("Querying OID: 0x%08x\n", Oid));

    //
    // Assume OID succeeds by default. Failure cases will set it as failure.
    //
    ndisStatus = NDIS_STATUS_SUCCESS;

    switch (Oid)
    {
        case OID_GEN_CURRENT_LOOKAHEAD:
            {
                ulongInfo = VNic11QueryLookahead(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_GEN_CURRENT_PACKET_FILTER: // Port
            {
                ulongInfo = Port->PacketFilter;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_GEN_HARDWARE_STATUS:   // VNic
            {
                ulongInfo = VNic11QueryHardwareStatus(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(NDIS_HARDWARE_STATUS));
                *BytesWritten = sizeof(NDIS_HARDWARE_STATUS);
            }
            break;
            
        case OID_GEN_INTERRUPT_MODERATION:  // VNic
            {
                ndisStatus = BasePortQueryInterruptModerationSettings(Port, 
                                InformationBuffer, 
                                InformationBufferLength
                                );
                *BytesWritten = sizeof(NDIS_INTERRUPT_MODERATION_PARAMETERS);
            }
            break;
            
        case OID_GEN_LINK_PARAMETERS:   // VNic
            {
                ndisStatus = BasePortQueryLinkParameters(Port, 
                                InformationBuffer, 
                                InformationBufferLength
                                );
                *BytesWritten = sizeof(NDIS_LINK_PARAMETERS);
            }
            break;
            
        case OID_GEN_MAXIMUM_FRAME_SIZE:
            {
                ulongInfo = HW11_MAX_FRAME_SIZE - DOT11_DATA_SHORT_HEADER_SIZE;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_GEN_MAXIMUM_TOTAL_SIZE:
        case OID_GEN_RECEIVE_BLOCK_SIZE:
        case OID_GEN_TRANSMIT_BLOCK_SIZE:
            {
                ulongInfo = HW11_MAX_FRAME_SIZE;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_GEN_RECEIVE_BUFFER_SPACE:  // VNic
            {
                ulongInfo = VNic11QueryReceiveBufferSpace(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;

        case OID_GEN_SUPPORTED_GUIDS:
            {
                // No custom GUIDs supported
                *BytesWritten = 0;
            }
            break;
            
        case OID_GEN_TRANSMIT_BUFFER_SPACE: // VNic
            {
                ulongInfo = VNic11QueryTransmitBufferSpace(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_GEN_TRANSMIT_QUEUE_LENGTH: // Port
            {
                MpTrace(COMP_OID, DBG_SERIOUS,  ("OID_GEN_TRANSMIT_QUEUE_LENGTH cannot be handled by BasePort\n"));
                MPASSERT(FALSE);
                ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            }
            break;
            
        case OID_GEN_VENDOR_DESCRIPTION:    // VNic
            {
                ndisStatus = VNic11QueryVendorDescription(Port->VNic,
                    InformationBuffer, 
                    InformationBufferLength, 
                    BytesWritten,
                    BytesNeeded
                    );
            }
            break;
            
        case OID_GEN_VENDOR_DRIVER_VERSION:
            {
                ulongInfo = HW11_MAJOR_DRIVER_VERSION << 16 | HW11_MINOR_DRIVER_VERSION;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_GEN_VENDOR_ID:             // VNic
            {
                ulongInfo = VNic11QueryVendorId(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                 *BytesWritten = sizeof(ULONG);
            }
            break;

        case OID_DOT11_CURRENT_ADDRESS:
        case OID_DOT11_MAC_ADDRESS:
        case OID_DOT11_STATION_ID:
        case OID_802_3_CURRENT_ADDRESS:
            {
                NdisMoveMemory(
                    InformationBuffer,
                    VNic11QueryMACAddress(Port->VNic),
                    sizeof(DOT11_MAC_ADDRESS)
                    );

                *BytesWritten = sizeof(DOT11_MAC_ADDRESS);
            }
            break;
            
        case OID_DOT11_CURRENT_OPERATION_MODE:
            {
                ndisStatus = BasePortQueryCurrentOperationMode(Port, InformationBuffer);
                *BytesWritten = sizeof(DOT11_CURRENT_OPERATION_MODE);
            }
            break;
            
        case OID_DOT11_CURRENT_OPTIONAL_CAPABILITY:
            {
                ndisStatus = VNic11QueryCurrentOptionalCapability(Port->VNic,
                                (PDOT11_CURRENT_OPTIONAL_CAPABILITY)InformationBuffer
                                );
                *BytesWritten = sizeof(DOT11_CURRENT_OPTIONAL_CAPABILITY);
            }
            break;
            
        case OID_DOT11_DATA_RATE_MAPPING_TABLE:
            {
                ndisStatus = BasePortQueryDataRateMappingTable(Port, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_MAXIMUM_LIST_SIZE:
            {
                ulongInfo = HW11_MAX_MULTICAST_LIST_SIZE;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_MPDU_MAX_LENGTH:
            {
                ulongInfo = VNic11QueryMaxMPDULength(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_MULTICAST_LIST:
        case OID_802_3_MULTICAST_LIST:
            {
                ndisStatus = VNic11QueryMulticastList(Port->VNic,
                                InformationBuffer,
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_NIC_POWER_STATE:
            {
                boolInfo = VNic11QueryNicPowerState(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_OPERATION_MODE_CAPABILITY:
            {
                ndisStatus = VNic11QueryOperationModeCapability(Port->VNic,
                                (PDOT11_OPERATION_MODE_CAPABILITY)InformationBuffer
                                );
                *BytesWritten = sizeof(DOT11_OPERATION_MODE_CAPABILITY);
            }
            break;
            
        case OID_DOT11_OPTIONAL_CAPABILITY:
            {
                ndisStatus = VNic11QueryOptionalCapability(Port->VNic,
                                (PDOT11_OPTIONAL_CAPABILITY)InformationBuffer
                                );
                *BytesWritten = sizeof(DOT11_OPTIONAL_CAPABILITY);
            }
            break;
            
        case OID_DOT11_PERMANENT_ADDRESS:
            {
                NdisMoveMemory(
                    InformationBuffer,
                    VNic11QueryHardwareAddress(Port->VNic),
                    sizeof(DOT11_MAC_ADDRESS)
                    );

                *BytesWritten = sizeof(DOT11_MAC_ADDRESS);
            }
            break;
            
        case OID_DOT11_RF_USAGE:
            {
                ulongInfo = VNic11QueryRFUsage(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_SUPPORTED_DATA_RATES_VALUE:
            {
                ndisStatus = VNic11QuerySupportedDataRatesValue(
                    Port->VNic,
                    (PDOT11_SUPPORTED_DATA_RATES_VALUE_V2) InformationBuffer,
                    TRUE
                    );

                *BytesWritten = sizeof(DOT11_SUPPORTED_DATA_RATES_VALUE_V2);
            }
            break;
            
        case OID_DOT11_SUPPORTED_PHY_TYPES:
            {
                ndisStatus = BasePortQuerySupportedPHYTypes(Port, 
                    InformationBuffer, 
                    InformationBufferLength,
                    BytesWritten, 
                    BytesNeeded
                    );
            }
            break;
            
        case OID_DOT11_SUPPORTED_POWER_LEVELS:
            {
                ndisStatus = BasePortQuerySupportedPowerLevels(Port, 
                    InformationBuffer, 
                    BytesWritten
                    );            
            }
            break;
            
        case OID_DOT11_SUPPORTED_RX_ANTENNA:
            {
                ndisStatus = BasePortQuerySupportedRXAntenna(Port, 
                    InformationBuffer, 
                    InformationBufferLength,
                    BytesWritten, 
                    BytesNeeded
                    );
            }
            break;
            
        case OID_DOT11_SUPPORTED_TX_ANTENNA:
            {
                ndisStatus = BasePortQuerySupportedTXAntenna(Port, 
                    InformationBuffer, 
                    InformationBufferLength,
                    BytesWritten, 
                    BytesNeeded
                    );
            }
            break;

        case OID_DOT11_BEACON_PERIOD:
            {
                ulongInfo = VNic11QueryBeaconPeriod(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_CCA_MODE_SUPPORTED:
            {
                ulongInfo = VNic11QueryCCAModeSupported(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_CF_POLLABLE:
            {
                boolInfo = VNic11QueryCFPollable(Port->VNic);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }        
            break;

        case OID_DOT11_CHANNEL_AGILITY_ENABLED:
            {
                boolInfo = VNic11QueryChannelAgilityEnabled(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }        
            break;

        case OID_DOT11_CHANNEL_AGILITY_PRESENT:
            {
                boolInfo = VNic11QueryChannelAgilityPresent(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }        
            break;
            
        case OID_DOT11_COUNTRY_STRING:
            {
                ndisStatus = VNic11QueryCountryString(Port->VNic, 
                                (PDOT11_COUNTRY_OR_REGION_STRING)InformationBuffer
                                );
                *BytesWritten = sizeof(DOT11_COUNTRY_OR_REGION_STRING);
            }
            break;
            
        case OID_DOT11_CURRENT_CCA_MODE:
            {
                ulongInfo = VNic11QueryCurrentCCAMode(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_CURRENT_CHANNEL:
            {
                ulongInfo = VNic11QueryCurrentChannel(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_CURRENT_FREQUENCY:
            {
                // all hardware calls work with channel
                ulongInfo = VNic11QueryCurrentChannel(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_CURRENT_REG_DOMAIN:
            {
                ulongInfo = VNic11QueryCurrentRegDomain(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_CURRENT_TX_POWER_LEVEL:
            {
                ulongInfo = VNic11QueryCurrentTXPowerLevel(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_DIVERSITY_SELECTION_RX:
            {
                ndisStatus = BasePortQueryDiversitySelectionRX(Port, 
                    InformationBuffer, 
                    InformationBufferLength,
                    BytesWritten, 
                    BytesNeeded
                    );
            }
            break;
            
        case OID_DOT11_DIVERSITY_SUPPORT:
            {
                ulongInfo = VNic11QueryDiversitySupport(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(DOT11_DIVERSITY_SUPPORT));
                *BytesWritten = sizeof(DOT11_DIVERSITY_SUPPORT);
            }
            break;
            
        case OID_DOT11_DSSS_OFDM_OPTION_ENABLED:
            {
                boolInfo = VNic11QueryDsssOfdmOptionEnabled(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }        
            break;
            
        case OID_DOT11_DSSS_OFDM_OPTION_IMPLEMENTED:
            {
                boolInfo = VNic11QueryDsssOfdmOptionImplemented(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }        
            break;
            
        case OID_DOT11_ED_THRESHOLD:
            {
                ulongInfo = VNic11QueryEDThreshold(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_ERP_PBCC_OPTION_ENABLED:
            {
                boolInfo = VNic11QueryErpPbccOptionEnabled(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }        
            break;
            
        case OID_DOT11_ERP_PBCC_OPTION_IMPLEMENTED:
            {
                boolInfo = VNic11QueryErpPbccOptionImplemented(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_FRAGMENTATION_THRESHOLD:
            {
                ulongInfo = VNic11QueryFragmentationThreshold(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_FREQUENCY_BANDS_SUPPORTED:
            {
                ulongInfo = VNic11QueryFrequencyBandsSupported(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_LONG_RETRY_LIMIT:
            {
                ulongInfo = VNic11QueryLongRetryLimit(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_MAX_RECEIVE_LIFETIME:
            {
                ulongInfo = VNic11QueryMaxReceiveLifetime(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_MAX_TRANSMIT_MSDU_LIFETIME:
            {
                ulongInfo = VNic11QueryMaxTransmitMSDULifetime(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_MULTI_DOMAIN_CAPABILITY_ENABLED:
            {
                boolInfo = VNic11QueryMultiDomainCapabilityEnabled(Port->VNic);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_MULTI_DOMAIN_CAPABILITY_IMPLEMENTED:
            {
                boolInfo = VNic11QueryMultiDomainCapabilityImplemented(Port->VNic);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_OPERATIONAL_RATE_SET:
            {
                VNic11QueryOperationalRateSet(Port->VNic, InformationBuffer, TRUE);
                *BytesWritten = sizeof(DOT11_RATE_SET);
            }
            break;
            
        case OID_DOT11_PBCC_OPTION_IMPLEMENTED:
            {
                boolInfo = VNic11QueryPbccOptionImplemented(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_REG_DOMAINS_SUPPORT_VALUE:
            {
                ndisStatus = BasePortQueryRegDomainsSupportValue(Port, 
                    InformationBuffer, 
                    InformationBufferLength,
                    BytesWritten, 
                    BytesNeeded
                    );
            }
            break;
            
        case OID_DOT11_RTS_THRESHOLD:
            {
                ulongInfo = VNic11QueryRTSThreshold(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_SHORT_PREAMBLE_OPTION_IMPLEMENTED:
            {
                boolInfo = VNic11QueryShortPreambleOptionImplemented(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_SHORT_RETRY_LIMIT:
            {
                ulongInfo = VNic11QueryShortRetryLimit(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }            
            break;
            
        case OID_DOT11_SHORT_SLOT_TIME_OPTION_ENABLED:
            {
                boolInfo = VNic11QueryShortSlotTimeOptionEnabled(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_SHORT_SLOT_TIME_OPTION_IMPLEMENTED:
            {
                boolInfo = VNic11QueryShortSlotTimeOptionEnabled(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_TEMP_TYPE:
            {
                
                ulongInfo = VNic11QueryTempType(Port->VNic, TRUE);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(DOT11_TEMP_TYPE));
                *BytesWritten = sizeof(DOT11_TEMP_TYPE);
            }
            break;

        default:
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
    }


    MpTrace(COMP_OID, DBG_NORMAL,  ("BasePortOidHandler OID query completed! Port %p, OID 0x%08x, ndisStatus = 0x%08x\n",
                Port, Oid, ndisStatus));

    return ndisStatus;
}





NDIS_STATUS
BasePortSetInformation(
    _In_  PMP_PORT                Port,
    _In_  NDIS_OID                Oid,
    _In_  PVOID                   InformationBuffer,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesRead,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       ulongInfo = 0;
    BOOLEAN                     boolInfo = FALSE;


//    MpEntry;

    // The length for all OIDs has already been verified using the OID table in port_oids.c
    UNREFERENCED_PARAMETER(InformationBufferLength);
    UNREFERENCED_PARAMETER(InformationBuffer);  // Not yet implemented

    *BytesRead = 0;
    *BytesNeeded = 0;

    MpTrace(COMP_OID, DBG_TRACE,  ("Setting OID: 0x%08x\n", Oid));
    
    //
    // Assume OID succeeds by default. Failure cases will set it as failure.
    //
    ndisStatus = NDIS_STATUS_SUCCESS;

    switch (Oid)
    {
        case OID_GEN_CURRENT_LOOKAHEAD: // Vnic
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                ndisStatus = VNic11SetLookahead(Port->VNic, ulongInfo);
            }
            break;
            
        case OID_GEN_CURRENT_PACKET_FILTER:
            {
                MpTrace(COMP_OID, DBG_SERIOUS,  ("OID_GEN_CURRENT_PACKET_FILTER cannot be handled by BasePort\n"));
                MPASSERT(FALSE);
                ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            }
            break;
            
        case OID_GEN_INTERRUPT_MODERATION:  // VNic
            {
                ndisStatus = BasePortSetInterruptModerationSettings(Port, 
                                InformationBuffer
                                );
                *BytesRead = sizeof(NDIS_INTERRUPT_MODERATION_PARAMETERS);
            }
            break;
            
        case OID_GEN_LINK_PARAMETERS:   // VNic
            {
                ndisStatus = BasePortSetLinkParameters(Port, 
                                InformationBuffer
                                );
                *BytesRead = sizeof(NDIS_LINK_PARAMETERS);
            }
            break;

        case OID_DOT11_MULTICAST_LIST:  // VNic
        case OID_802_3_MULTICAST_LIST:
            {
                ndisStatus = BasePortSetMulticastList(Port,
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_NIC_POWER_STATE: // VNic
            {
                NdisMoveMemory(&boolInfo, InformationBuffer, sizeof(BOOLEAN));
                *BytesRead = sizeof(BOOLEAN);
                ndisStatus = VNic11SetNicPowerState(Port->VNic, boolInfo, TRUE);
            }
            break;

        case OID_DOT11_SCAN_REQUEST:    // Per Port
            {
                MpTrace(COMP_OID, DBG_SERIOUS,  ("OID_DOT11_SCAN_REQUEST cannot be handled by BasePort\n"));
                MPASSERT(FALSE);
                ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            }
            break;
            
        case OID_DOT11_BEACON_PERIOD:   // VNic
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                if ((ulongInfo < 1) || (ulongInfo > 65535))
                {
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
                }
                else
                {                
                    ndisStatus = VNic11SetBeaconPeriod(Port->VNic, ulongInfo);
                }
            }
            break;
            
        case OID_DOT11_CURRENT_CHANNEL: // VNic
            {
                MpTrace(COMP_OID, DBG_SERIOUS,  ("OID_DOT11_CURRENT_CHANNEL cannot be handled by BasePort\n"));
                MPASSERT(FALSE);
                ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            }
            break;
            
        case OID_DOT11_CURRENT_FREQUENCY:   // VNic
            {
                MpTrace(COMP_OID, DBG_SERIOUS,  ("OID_DOT11_CURRENT_FREQUENCY cannot be handled by BasePort\n"));
                MPASSERT(FALSE);
                ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            }
            break;
            
        case OID_DOT11_FRAGMENTATION_THRESHOLD: // VNic
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                if ((ulongInfo < 256) || (ulongInfo > 2346))
                {
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
                }
                else
                {                
                    ndisStatus = VNic11SetFragmentationThreshold(Port->VNic, ulongInfo);
                    *BytesRead = sizeof(ULONG);
                }
            }
            break;
            
        case OID_DOT11_MULTI_DOMAIN_CAPABILITY_ENABLED: // VNic
            {
                NdisMoveMemory(&boolInfo, InformationBuffer, sizeof(BOOLEAN));
                *BytesRead = sizeof(BOOLEAN);
                ndisStatus = VNic11SetMultiDomainCapabilityEnabled(Port->VNic, boolInfo);
            }
            break;
            
        case OID_DOT11_OPERATIONAL_RATE_SET:    // VNic
            {
                ndisStatus = BasePortSetOperationalRateSet(Port,
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_RTS_THRESHOLD:   // VNic
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                if (ulongInfo > 2347)
                {
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
                }
                else
                {                
                    ndisStatus = VNic11SetRTSThreshold(Port->VNic, ulongInfo);
                    *BytesRead = sizeof(ULONG);
                }
            }
            break;

        default:
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
    }


    MpTrace(COMP_OID, DBG_NORMAL,  ("BasePortOidHandler OID set completed! Port %p, OID 0x%08x, ndisStatus = 0x%08x\n",
                Port, Oid, ndisStatus));
    
//    MpExit;
    return ndisStatus;
}



// MiniportOidRequest - Called from MP layer to Port
NDIS_STATUS 
BasePortOidHandler(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       OidRequest
    )
{
    NDIS_STATUS                 ndisStatus;

    switch(OidRequest->RequestType)
    {
        case NdisRequestQueryInformation:
        case NdisRequestQueryStatistics:
            ndisStatus = BasePortQueryInformation(
                            Port,
                            OidRequest->DATA.QUERY_INFORMATION.Oid,
                            OidRequest->DATA.QUERY_INFORMATION.InformationBuffer,
                            OidRequest->DATA.QUERY_INFORMATION.InformationBufferLength,
                            (PULONG)&OidRequest->DATA.QUERY_INFORMATION.BytesWritten,
                            (PULONG)&OidRequest->DATA.QUERY_INFORMATION.BytesNeeded
                            );
            break;

        case NdisRequestSetInformation:
            ndisStatus = BasePortSetInformation(
                            Port,
                            OidRequest->DATA.SET_INFORMATION.Oid,
                            OidRequest->DATA.SET_INFORMATION.InformationBuffer,
                            OidRequest->DATA.SET_INFORMATION.InformationBufferLength,
                            (PULONG)&OidRequest->DATA.SET_INFORMATION.BytesRead,
                            (PULONG)&OidRequest->DATA.SET_INFORMATION.BytesNeeded
                            );
            break;

        default:
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
    }

    // All OIDs handled here cannot pend
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_OID, DBG_NORMAL, ("NDIS_OID_REQUEST for failed in Base Port. Status = 0x%08x\n", 
            ndisStatus));
    }
    
    return ndisStatus;
}

NDIS_STATUS 
BasePortDirectOidHandler(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       OidRequest
    )
{
    UNREFERENCED_PARAMETER(Port);
    UNREFERENCED_PARAMETER(OidRequest);

    return NDIS_STATUS_NOT_SUPPORTED;
}
