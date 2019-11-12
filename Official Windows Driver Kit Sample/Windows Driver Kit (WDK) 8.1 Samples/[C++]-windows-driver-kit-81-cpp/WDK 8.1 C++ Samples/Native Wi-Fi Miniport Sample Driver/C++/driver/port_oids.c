/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    port_oids.c

Abstract:
    Implements the OIDs handling that is common to all ports
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/


#include "precomp.h"
#include "port_defs.h"
#include "vnic_intf.h"
#include "base_port_intf.h"

#if DOT11_TRACE_ENABLED
#include "port_oids.tmh"
#endif


/* We use this OID table structure to check if the OID is supported at this point & what the 
 * size should
 */
typedef struct PORT_QUERY_SET_OID_ENTRY {
    NDIS_OID                    Oid;                // Oid value
    
    BOOLEAN                     InitSettable:1;     // Settable in Init state (for atleast one op mode)
    
    BOOLEAN                     OpSettable:1;       // Settable in Op state (for atleast one op mode)
    
    BOOLEAN                     Queryable:1;        // Queryable in Init/Op state
    
    BOOLEAN                     ExtSTASupported:1;  // Valid in ExtSTA
    
    BOOLEAN                     ExtAPSupported:1;   // Valid in ExtAP
    
    BOOLEAN                     NetmonSupported:1;  // Valid in Netmon mode

    BOOLEAN                     PhySpecific:1;      // Is this OID only acceptable for certain phys

    ULONG                       MinBufferSize;      // Minimum size required for the buffer (query & set)
    
} PORT_QUERY_SET_OID_ENTRY, * PPORT_QUERY_SET_OID_ENTRY;






PORT_QUERY_SET_OID_ENTRY OidQuerySetTable[] = {
    // NDIS OIDs
    {
        OID_GEN_CURRENT_LOOKAHEAD,                  // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_CURRENT_PACKET_FILTER,              // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE ,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_HARDWARE_STATUS,                    // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(NDIS_HARDWARE_STATUS)                // MinBufferSize
    },
    {
        OID_GEN_INTERRUPT_MODERATION,               // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(NDIS_INTERRUPT_MODERATION_PARAMETERS)    // MinBufferSize
    },
    {
        OID_GEN_LINK_PARAMETERS,                    // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(NDIS_LINK_PARAMETERS)                // MinBufferSize
    },
    {
        OID_GEN_MAXIMUM_FRAME_SIZE,                 // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_MAXIMUM_TOTAL_SIZE,                 // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_RECEIVE_BLOCK_SIZE,                 // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_RECEIVE_BUFFER_SPACE,               // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_SUPPORTED_GUIDS,                    // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize // Variable
    },
    {
        OID_GEN_TRANSMIT_BLOCK_SIZE,                // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_TRANSMIT_BUFFER_SPACE,              // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_TRANSMIT_QUEUE_LENGTH,              // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_VENDOR_DESCRIPTION,                 // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize // Variable
    },
    {
        OID_GEN_VENDOR_DRIVER_VERSION,              // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_GEN_VENDOR_ID,                          // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_802_3_CURRENT_ADDRESS,                  // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_MAC_ADDRESS)                   // MinBufferSize
    },
    {
        OID_802_3_MULTICAST_LIST,                   // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize // Variable
    },

    // PNP handlers
    {
        OID_PNP_SET_POWER,                          // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(NDIS_DEVICE_POWER_STATE)             // MinBufferSize
    },
    {
        OID_PNP_QUERY_POWER,                        // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(NDIS_DEVICE_POWER_STATE)             // MinBufferSize
    },

    // Operation Oids
    {
        OID_DOT11_CURRENT_ADDRESS,                  // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_MAC_ADDRESS)                   // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_OPERATION_MODE,           // Oid
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_CURRENT_OPERATION_MODE)        // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_OPTIONAL_CAPABILITY,      // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_CURRENT_OPTIONAL_CAPABILITY)   // MinBufferSize
    },
    {
        OID_DOT11_DATA_RATE_MAPPING_TABLE,          // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_DATA_RATE_MAPPING_TABLE)       // MinBufferSize
    },
    {
        OID_DOT11_MAXIMUM_LIST_SIZE,                // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_MPDU_MAX_LENGTH,                  // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_MULTICAST_LIST,                   // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize // Variable
    },
    {
        OID_DOT11_NIC_POWER_STATE,                  // Oid: msDot11NICPowerState
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_OPERATION_MODE_CAPABILITY,        // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported (need not be supported, but is queried)
        FALSE,                                      // PhySpecific
        sizeof(DOT11_OPERATION_MODE_CAPABILITY)     // MinBufferSize
    },
    {
        OID_DOT11_OPTIONAL_CAPABILITY,              // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_OPTIONAL_CAPABILITY)           // MinBufferSize
    },
    {
        OID_DOT11_PERMANENT_ADDRESS,                // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_MAC_ADDRESS)                   // MinBufferSize
    },
    {
        OID_DOT11_RECV_SENSITIVITY_LIST,            // Oid
        FALSE,                                      // InitSettable // Method
        FALSE,                                      // OpSettable   // Method
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_RECV_SENSITIVITY_LIST)         // MinBufferSize
    },
    {
        OID_DOT11_RESET_REQUEST,                    // Oid
        FALSE,                                      // InitSettable // Method
        FALSE,                                      // OpSettable   // Method
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        MIN(sizeof(DOT11_RESET_REQUEST), sizeof(DOT11_STATUS_INDICATION))   // MinBufferSize
    },
    {
        OID_DOT11_RF_USAGE,                         // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_SCAN_REQUEST,                     // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_SCAN_REQUEST_V2, ucBuffer)   // MinBufferSize
    },
    {
        OID_DOT11_SUPPORTED_DATA_RATES_VALUE,       // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_SUPPORTED_DATA_RATES_VALUE_V2),// MinBufferSize
    },
    {
        OID_DOT11_SUPPORTED_PHY_TYPES,              // Oid: msDot11SupportedPhyTypes
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_SUPPORTED_PHY_TYPES)           // MinBufferSize
    },
    {
        OID_DOT11_SUPPORTED_POWER_LEVELS,           // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_SUPPORTED_POWER_LEVELS)        // MinBufferSize
    },
    {
        OID_DOT11_SUPPORTED_RX_ANTENNA,             // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_SUPPORTED_ANTENNA_LIST)        // MinBufferSize
    },
    {
        OID_DOT11_SUPPORTED_TX_ANTENNA,             // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_SUPPORTED_ANTENNA_LIST)        // MinBufferSize
    },

    // MIB Oids
    {
        OID_DOT11_BEACON_PERIOD,                    // Oid: dot11BeaconPeriod
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CCA_MODE_SUPPORTED,               // Oid: dot11CCAModeSupported
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CF_POLLABLE,                      // Oid: dot11CFPollable
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_CHANNEL_AGILITY_ENABLED,          // Oid: dot11ChannelAgilityEnabled
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_CHANNEL_AGILITY_PRESENT,          // Oid: dot11ChannelAgilityPresent
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_COUNTRY_STRING,                   // Oid: dot11CountryString
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_COUNTRY_OR_REGION_STRING)      // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_CCA_MODE,                 // Oid: dot11CurrentCCAMode
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_CHANNEL,                  // Oid: dot11CurrentChannel
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_FREQUENCY,                // Oid: dot11CurrentFrequency
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_REG_DOMAIN,               // Oid: dot11CurrentRegDomain
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_TX_POWER_LEVEL,           // Oid: dot11CurrentTxPowerLevel
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_DIVERSITY_SELECTION_RX,           // Oid: dot11DiversitySelectionRx
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_DIVERSITY_SELECTION_RX_LIST)   // MinBufferSize
    },
    {
        OID_DOT11_DIVERSITY_SUPPORT,                // Oid: dot11DiversitySupport 
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_DIVERSITY_SUPPORT)             // MinBufferSize
    },
    {
        OID_DOT11_DSSS_OFDM_OPTION_ENABLED,         // Oid: dot11DSSSOFDMOptionEnabled
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_DSSS_OFDM_OPTION_IMPLEMENTED,     // Oid: dot11DSSSOFDMOptionImplemented
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_ED_THRESHOLD,                     // Oid: dot11EDThreshold
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_ERP_PBCC_OPTION_ENABLED,          // Oid: dot11ERPBCCOptionEnabled
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_ERP_PBCC_OPTION_IMPLEMENTED,      // Oid: dot11ERPPBCCOptionImplemented
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_FRAGMENTATION_THRESHOLD,          // Oid: dot11FragmentationThreshold
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_FREQUENCY_BANDS_SUPPORTED,        // Oid: dot11FrequencyBandsSupported
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_LONG_RETRY_LIMIT,                 // Oid: dot11LongtRetryLimit
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_MAC_ADDRESS,                      // Oid: dot11MACAddress
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_MAC_ADDRESS)                   // MinBufferSize
    },
    {
        OID_DOT11_MAX_RECEIVE_LIFETIME,             // Oid: dot11MaxReceiveLifetime
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_MAX_TRANSMIT_MSDU_LIFETIME,       // Oid: dot11MaxTransmitMSDULifetime
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_MULTI_DOMAIN_CAPABILITY_ENABLED,  // Oid: dot11MultiDomainCapabilityEnabled
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_MULTI_DOMAIN_CAPABILITY_IMPLEMENTED,  // Oid: dot11MultiDomainCapabilityImplemented
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_OPERATIONAL_RATE_SET,             // Oid: dot11OperationalRateSet
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_RATE_SET)                      // MinBufferSize
    },
    {
        OID_DOT11_PBCC_OPTION_IMPLEMENTED,          // Oid: dot11PBCCOptionImplemented
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_REG_DOMAINS_SUPPORT_VALUE,        // Oid: dot11RegDomainValue
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_REG_DOMAINS_SUPPORT_VALUE)     // MinBufferSize
    },
    {
        OID_DOT11_RTS_THRESHOLD,                    // Oid: dot11RTSThreshold
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_SHORT_PREAMBLE_OPTION_IMPLEMENTED,// Oid: dot11ShortPreambleOptionImplemented
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_SHORT_RETRY_LIMIT,                // Oid: dot11ShortRetryLimit
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_SHORT_SLOT_TIME_OPTION_ENABLED,   // Oid: dot11ShortSlotTimeOptionEnabled
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE ,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_SHORT_SLOT_TIME_OPTION_IMPLEMENTED, // Oid: dot11ShortSlotTimeOptionImplemented
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        TRUE,                                       // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_STATION_ID,                       // Oid: dot11StationID
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_MAC_ADDRESS)                   // MinBufferSize
    },
    {
        OID_DOT11_TEMP_TYPE,                        // Oid: dot11TempType
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_TEMP_TYPE)                     // MinBufferSize
    },

    // ExtSTA Operation OIDs
    {
        OID_DOT11_ACTIVE_PHY_LIST,                  // Oid: msDot11ActivePhyList
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_PHY_ID_LIST)                   // MinBufferSize
    },
    {
        OID_DOT11_ATIM_WINDOW,                      // Oid
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_AUTO_CONFIG_ENABLED,              // Oid: msDot11AutoConfigEnabled
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported (need not be supported, but is set)
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CIPHER_DEFAULT_KEY,               // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_CIPHER_DEFAULT_KEY_VALUE, ucKey)     // MinBufferSize
    },
    {
        OID_DOT11_CIPHER_DEFAULT_KEY_ID,            // Oid: dot11DefaultKeyID
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_CIPHER_KEY_MAPPING_KEY,           // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer)    // MinBufferSize
    },
    {
        OID_DOT11_CONNECT_REQUEST,                  // Oid
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize
    },
    {
        OID_DOT11_CURRENT_PHY_ID,                   // Oid: msDot11CurrentPhyID
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_DESIRED_BSS_TYPE,                 // Oid: dot11DesiredBSSType
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_BSS_TYPE)                      // MinBufferSize
    },
    {
        OID_DOT11_DESIRED_BSSID_LIST,               // Oid: msDot11DesiredBSSIDList
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs)      // MinBufferSize
    },
    {
        OID_DOT11_DESIRED_PHY_LIST,                 // Oid: msDot11DesiredPhyList
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId)     // MinBufferSize
    },
    {
        OID_DOT11_DESIRED_SSID_LIST,                // Oid: msDot11DesiredSSIDList
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_SSID_LIST, SSIDs)        // MinBufferSize
    },
    {
        OID_DOT11_DISCONNECT_REQUEST,               // Oid
        FALSE,                                      // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize
    },
    {
        OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM, // Oid: msDot11EnabledAuthAlgo
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_AUTH_ALGORITHM_LIST, AlgorithmIds)    // MinBufferSize
    },
    {
        OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM,   // Oid: msDot11EnabledMulticastCipherAlgo
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds)    // MinBufferSize
    },
    {
        OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM, // Oid: msDot11EnabledUnicastCipherAlgo
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds)    // MinBufferSize
    },
    {
        OID_DOT11_ENUM_ASSOCIATION_INFO,            // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_ASSOCIATION_INFO_LIST)         // MinBufferSize
    },
    {
        OID_DOT11_ENUM_BSS_LIST,                    // Oid
        FALSE,                                      // InitSettable // Method
        FALSE,                                      // OpSettable    // Method
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer)    // MinBufferSize
    },
    {
        OID_DOT11_EXCLUDE_UNENCRYPTED,              // Oid: dot11ExcludeUnencrypted
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_EXCLUDED_MAC_ADDRESS_LIST,        // Oid: msDot11ExcludedMacAddressList
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs)  // MinBufferSize
    },
    {
        OID_DOT11_EXTSTA_CAPABILITY,                // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_EXTSTA_CAPABILITY)             // MinBufferSize
    },
    {
        OID_DOT11_FLUSH_BSS_LIST,                   // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize
    },
    {
        OID_DOT11_HARDWARE_PHY_STATE,               // Oid: msDot11HardwarePHYState
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_HIDDEN_NETWORK_ENABLED,           // Oid: msDot11HiddenNetworkEnabled
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_IBSS_PARAMS,                      // Oid
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_IBSS_PARAMS)                   // MinBufferSize
    },
    {
        OID_DOT11_MEDIA_STREAMING_ENABLED,          // Oid: msDot11MediaStreamingEnabled
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_PMKID_LIST,                       // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_PMKID_LIST,PMKIDs)       // MinBufferSize
    },
    {
        OID_DOT11_POWER_MGMT_REQUEST,               // Oid: msDot11PowerSavingLevel
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_PRIVACY_EXEMPTION_LIST,           // Oid: msDot11PrivacyExemptionList
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries)     // MinBufferSize
    },
    {
        OID_DOT11_SAFE_MODE_ENABLED,                // Oid: msDot11SafeModeEnabled
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_STATISTICS,                       // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        TRUE,                                       // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_STATISTICS)                    // MinBufferSize
    },
    {
        OID_DOT11_SUPPORTED_MULTICAST_ALGORITHM_PAIR,   // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs)  // MinBufferSize
    },
    {
        OID_DOT11_SUPPORTED_UNICAST_ALGORITHM_PAIR, // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs)  // MinBufferSize
    },
    {
        OID_DOT11_UNICAST_USE_GROUP_ENABLED,        // Oid: msDot11UnicastUseGroupEnabled
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_UNREACHABLE_DETECTION_THRESHOLD,  // Oid: msDot11UnreachableDetectionThreshold
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_ASSOCIATION_PARAMS,               // Oid
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        TRUE,                                       // ExtSTASupported
        FALSE,                                      // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_ASSOCIATION_PARAMS)            // MinBufferSize
    },

    // ExtAP specific OIDs
    {
        OID_DOT11_DTIM_PERIOD,                      // Oid: dot11DTIMPeriod
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(ULONG)                               // MinBufferSize
    },
    {
        OID_DOT11_AVAILABLE_CHANNEL_LIST,           // Oid: msDot11AvailableChannelList
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_AVAILABLE_CHANNEL_LIST, uChannelNumber)  // MinBufferSize
    },
    {
        OID_DOT11_AVAILABLE_FREQUENCY_LIST,         // Oid: msDot11AvailableFrequencyList
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_AVAILABLE_FREQUENCY_LIST, uFrequencyValue)   // MinBufferSize
    },
    {
        OID_DOT11_ENUM_PEER_INFO,                   // Oid
        FALSE,                                      // InitSettable
        FALSE,                                      // OpSettable
        TRUE,                                       // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        FIELD_OFFSET(DOT11_PEER_INFO_LIST, PeerInfo)    // MinBufferSize
    },
    {
        OID_DOT11_DISASSOCIATE_PEER_REQUEST,        // Oid
        FALSE,                                      // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_DISASSOCIATE_PEER_REQUEST)     // MinBufferSize
    },
    {
        OID_DOT11_PORT_STATE_NOTIFICATION,          // Oid
        FALSE,                                      // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_PORT_STATE_NOTIFICATION)       // MinBufferSize
    },
    {
        OID_DOT11_INCOMING_ASSOCIATION_DECISION,    // Oid
        FALSE,                                      // InitSettable
        TRUE,                                       // OpSettable
        FALSE,                                      // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_INCOMING_ASSOC_DECISION)       // MinBufferSize
    },
    {
        OID_DOT11_ADDITIONAL_IE,                    // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(DOT11_ADDITIONAL_IE)                 // MinBufferSize
    },
    {
        OID_DOT11_WPS_ENABLED,                      // Oid
        TRUE,                                       // InitSettable
        TRUE,                                       // OpSettable
        TRUE,                                       // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        sizeof(BOOLEAN)                             // MinBufferSize
    },
    {
        OID_DOT11_START_AP_REQUEST,                 // Oid
        TRUE,                                       // InitSettable
        FALSE,                                      // OpSettable
        FALSE,                                      // Queryable
        FALSE,                                      // ExtSTASupported
        TRUE,                                       // ExtAPSupported
        FALSE,                                      // NetmonSupported
        FALSE,                                      // PhySpecific
        0                                           // MinBufferSize
    },
};


NDIS_STATUS
PortPreprocessOid(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{
    PULONG                      bytesNeededLocation;
    NDIS_OID                    oid;
    ULONG                       infoBufferLength;
    PPORT_QUERY_SET_OID_ENTRY   oidTableEntry = NULL;
    ULONG                       i;
    DOT11_PHY_TYPE              currentPhy;

    oid = NdisOidRequest->DATA.QUERY_INFORMATION.Oid; // Oid is at same offset for all RequestTypes
    
    // Find the OID in the oid table
    for (i = 0; i < ((ULONG)sizeof(OidQuerySetTable)/(ULONG)sizeof(PORT_QUERY_SET_OID_ENTRY)); i++)
    {
        if (OidQuerySetTable[i].Oid == oid)
        {
            oidTableEntry = &OidQuerySetTable[i];
            break;
        }
    }

    if (oidTableEntry == NULL)
    {
        //
        // We do not have data to verify this OID. We let it through
        //
        MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x not found in oid table\n", oid));
        return NDIS_STATUS_SUCCESS;
    }

    // Verify that the OID is applicable for the current operation mode mode
    switch(Port->CurrentOpMode)
    {
        case DOT11_OPERATION_MODE_EXTENSIBLE_STATION:
            if (!oidTableEntry->ExtSTASupported)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x not supported in ExtSTA mode\n", oid));
                return NDIS_STATUS_INVALID_STATE;
            }
            break;
            
        case DOT11_OPERATION_MODE_NETWORK_MONITOR :
            if (!oidTableEntry->NetmonSupported)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x not supported in Network Monitor mode\n", oid));
                return NDIS_STATUS_INVALID_STATE;
            }
            break;

        case DOT11_OPERATION_MODE_EXTENSIBLE_AP:
            if (!oidTableEntry->ExtAPSupported)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x not supported in ExtAP mode\n", oid));
                return NDIS_STATUS_INVALID_STATE;
            }
            break;
            
        default:
            // This port is in an error state that can happen
            // if we failed an operation mode change. The only OID
            // we would support is change of op mode
            MPASSERT(Port->CurrentOpMode == DOT11_OPERATION_MODE_UNKNOWN);
            if ((oid != OID_DOT11_CURRENT_OPERATION_MODE) ||
                (NdisOidRequest->RequestType != NdisRequestSetInformation))
            {
                // Fail
                return NDIS_STATUS_INVALID_STATE;
            }        
            break;
    }

    // Verify if the OID is supported in current operating state
    switch(NdisOidRequest->RequestType)
    {
        case NdisRequestQueryInformation:
        case NdisRequestQueryStatistics:
            if (!oidTableEntry->Queryable)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Query of OID 0x%08x not supported\n", oid));
                return NDIS_STATUS_INVALID_STATE;
            }
            
            // Determine buffer length that will be used later
            bytesNeededLocation = (PULONG)&NdisOidRequest->DATA.QUERY_INFORMATION.BytesNeeded;
            infoBufferLength = NdisOidRequest->DATA.QUERY_INFORMATION.InformationBufferLength;
            break;

        case NdisRequestSetInformation:
            if (Port->CurrentOpState == INIT_STATE)
            {
                if (!oidTableEntry->InitSettable)
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("Set of OID 0x%08x not supported in INIT state\n", oid));
                    return NDIS_STATUS_INVALID_STATE;
                }
            }
            else
            {
                if (!oidTableEntry->OpSettable)
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("Set of OID 0x%08x not supported in  OP state\n", oid));
                    return NDIS_STATUS_INVALID_STATE;
                }
            }

            // Determine buffer length that will be used later
            bytesNeededLocation = (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesNeeded;
            infoBufferLength = NdisOidRequest->DATA.SET_INFORMATION.InformationBufferLength;
            break;

        case NdisRequestMethod:
            // Only these OIDs are supported as methods
            if ( (oid != OID_DOT11_RESET_REQUEST) &&
                 (oid != OID_DOT11_ENUM_BSS_LIST) &&
                 (oid != OID_DOT11_RECV_SENSITIVITY_LIST))
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Method request for OID 0x%08x not supported\n", oid));
                return NDIS_STATUS_INVALID_OID;
            }

            // Determine buffer length that will be used later
            bytesNeededLocation = (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded;
            
            // Larger of the two buffer lengths
            infoBufferLength = MAX(NdisOidRequest->DATA.METHOD_INFORMATION.InputBufferLength,
                                NdisOidRequest->DATA.METHOD_INFORMATION.OutputBufferLength);            
            break;

        default:
            MpTrace(COMP_OID, DBG_SERIOUS, ("NDIS_OID_REQUEST contains unknown request type %d\n", 
                NdisOidRequest->RequestType));
            return NDIS_STATUS_NOT_SUPPORTED;
    }


    // Verify length is big enough
    if (infoBufferLength < oidTableEntry->MinBufferSize)
    {
        MpTrace(COMP_OID, DBG_SERIOUS, ("InformationBufferLength %d for OID 0x%08x less than minimum required %d\n", 
            infoBufferLength,
            oid,
            oidTableEntry->MinBufferSize));

        // Set the bytes needed value
        *bytesNeededLocation = oidTableEntry->MinBufferSize;
        return NDIS_STATUS_BUFFER_OVERFLOW;
    }

    // Verify that this OID is applicable for the current PHY
    if (oidTableEntry->PhySpecific)
    {
        // Verify that this OID is valid for the currently SELECTED PHY
        currentPhy = BasePortGetPhyTypeFromId(Port, VNic11QuerySelectedPhyId(Port->VNic));
        switch (oid)
        {
            // OIDs valid only for ERP
            case OID_DOT11_DSSS_OFDM_OPTION_ENABLED:
            case OID_DOT11_DSSS_OFDM_OPTION_IMPLEMENTED:
            case OID_DOT11_ERP_PBCC_OPTION_ENABLED:
            case OID_DOT11_ERP_PBCC_OPTION_IMPLEMENTED:
            case OID_DOT11_SHORT_SLOT_TIME_OPTION_ENABLED:
            case OID_DOT11_SHORT_SLOT_TIME_OPTION_IMPLEMENTED:
                if (currentPhy != dot11_phy_type_erp)
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x only valid on ERP phy. Current phy type is %d\n", 
                        oid,
                        currentPhy));
                
                    return NDIS_STATUS_INVALID_DATA;
                }
                break;
                
            case OID_DOT11_PBCC_OPTION_IMPLEMENTED:
            case OID_DOT11_SHORT_PREAMBLE_OPTION_IMPLEMENTED:
            case OID_DOT11_CHANNEL_AGILITY_PRESENT:
            case OID_DOT11_CHANNEL_AGILITY_ENABLED:
                if ((currentPhy != dot11_phy_type_erp) && 
                    (currentPhy != dot11_phy_type_hrdsss))
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x only valid on ERP or HRDSS phy. Current phy type is %d\n", 
                        oid,
                        currentPhy));
                
                    return NDIS_STATUS_INVALID_DATA;
                }
                break;

            // OIDs valid only for ERP or DSSS or HRDSS
            case OID_DOT11_CCA_MODE_SUPPORTED:
            case OID_DOT11_CURRENT_CCA_MODE:
            case OID_DOT11_CURRENT_CHANNEL:
            case OID_DOT11_ED_THRESHOLD:
                if ((currentPhy != dot11_phy_type_erp) && 
                    (currentPhy != dot11_phy_type_hrdsss) && 
                    (currentPhy != dot11_phy_type_dsss))
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x only valid on ERP, HRDSS or DSS phy. Current phy type is %d\n", 
                        oid,
                        currentPhy));
                
                    return NDIS_STATUS_INVALID_DATA;
                }
                break;

            case OID_DOT11_FREQUENCY_BANDS_SUPPORTED:
            case OID_DOT11_CURRENT_FREQUENCY:
                if (currentPhy != dot11_phy_type_ofdm)
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("OID 0x%08x only valid on OFDM phy. Current phy type is %d\n", 
                        oid,
                        currentPhy));
                
                    return NDIS_STATUS_INVALID_DATA;
                }
                break;

            default:
                break;
        }

    }

    // OID must be at this time and has enough buffer to hold minimum data
    return NDIS_STATUS_SUCCESS;
}

MP_PORT_TYPE
PortConvertOpModetoPortType(
    _In_  ULONG                   OpMode
    )
{
    switch (OpMode)
    {
        case DOT11_OPERATION_MODE_EXTENSIBLE_STATION:
        case DOT11_OPERATION_MODE_NETWORK_MONITOR:
            return EXTSTA_PORT;

        case DOT11_OPERATION_MODE_EXTENSIBLE_AP:
            return EXTAP_PORT;

        default:
            return EXTSTA_PORT;
    }
}

NDIS_STATUS
PortChangePortType(
    _In_  PMP_PORT                Port,
    _In_  MP_PORT_TYPE            NewPortType
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_PORT_REG_INFO           regInfo = Port->Adapter->PortRegInfo;

    do
    {
        // First terminate the old port type (without terminating
        // the base port)
        Port11TerminatePort(Port, FALSE);

        // Free the old port type (without deleting
        // the base port)
        Port11FreePort(Port, FALSE);

        // From this point, the old child port is no longer valid. If 
        // we fail, we will delete the whole driver & bail out
        Port->ChildPort = NULL;
        
        // Now its the new port type
        Port->PortType = NewPortType;
        // update the VNIC with the new port type
        VNic11UpdatePortType(Port->VNic, NewPortType);

        // Allocate the new port type
        switch (NewPortType)
        {
            case EXTSTA_PORT:
                ndisStatus = Sta11AllocatePort(Port->MiniportAdapterHandle, Port->Adapter, Port);
                break;

            case EXTAP_PORT:
                ndisStatus = Ap11AllocatePort(Port->MiniportAdapterHandle, Port->Adapter, Port);
                break;

            default:
                ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
                break;

        }
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Failed to allocate port %d of type %d . Status = 0x%08x\n", 
                Port->PortNumber, NewPortType, ndisStatus));
            break;
        }

        // Initialize the new port type
        switch (NewPortType)
        {
            case EXTSTA_PORT:
                ndisStatus = Sta11InitializePort(Port, regInfo->ExtStaRegInfo);
                break;

            case EXTAP_PORT:
                ndisStatus = Ap11InitializePort(Port, regInfo->ExtAPRegInfo);
                break;

            default:
                ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
                break;
        }
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Failed to initialize port %d of type %d . Status = 0x%08x\n", 
                Port->PortNumber, NewPortType, ndisStatus));
            break;
        }

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // If we have allocated the new port, free it
        if (Port->ChildPort != NULL)
        {
            Port11FreePort(Port, FALSE);
        }
        Port->ChildPort = NULL;
    }
    
    return ndisStatus;
}

NDIS_STATUS
Port11SetOperationMode(
    _In_  PMP_PORT                Port,
    _In_  ULONG                   OpMode
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    Port->CurrentOpMode = OpMode;
    switch (Port->PortType)
    {
        case EXTSTA_PORT:
            ndisStatus = Sta11SetOperationMode(Port, OpMode);
            break;

        case EXTAP_PORT:
            ndisStatus = NDIS_STATUS_SUCCESS;
            break;

        default:
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Attempting to notify op mode change event to unrecognized port type %d\n", Port->PortType));
            ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
            break;
    }

    return ndisStatus;
}


NDIS_STATUS
PortSetCurrentOperationMode(
    _In_ PMP_PORT                 Port,
    _In_ PVOID                    NdisOidRequest
    )
{    
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_STATUS                 returnNdisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_OID_REQUEST           OidRequest = (PNDIS_OID_REQUEST)NdisOidRequest;
    PDOT11_CURRENT_OPERATION_MODE dot11CurrentOperationMode = (PDOT11_CURRENT_OPERATION_MODE)OidRequest->DATA.SET_INFORMATION.InformationBuffer;
    MP_PORT_TYPE                newPortType;
    BOOLEAN                     typeChanged = FALSE;

    do
    {
        // Determine what the Port type should be for handling the new op mode
        newPortType = PortConvertOpModetoPortType(dot11CurrentOperationMode->uCurrentOpMode);

        if (Port->PortType != newPortType)
        {
            // First we pause the current port
            returnNdisStatus = Mp11PausePort(Port->Adapter, Port);
            if (returnNdisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Failed to pause port %d to change Op Mode. Status = 0x%08x\n", 
                    Port->PortNumber, returnNdisStatus));
                    
                break;
            }

            returnNdisStatus = PortChangePortType(Port, newPortType);
            if (returnNdisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Failed to change port type on port %d. Status = 0x%08x\n", 
                    Port->PortNumber, returnNdisStatus));
            }
            else
            {
                // Port type has been changed
                typeChanged = TRUE;
                
                // We set this in the port
                returnNdisStatus = Port11SetOperationMode(Port, dot11CurrentOperationMode->uCurrentOpMode);
                MPASSERT(returnNdisStatus == NDIS_STATUS_SUCCESS);

                // And pass it to the VNIC
                returnNdisStatus = VNic11SetOperationMode(PORT_GET_VNIC(Port), dot11CurrentOperationMode);
                MPASSERT(returnNdisStatus == NDIS_STATUS_SUCCESS);
            }

            if (returnNdisStatus != NDIS_STATUS_SUCCESS)
            {
                //
                // If port creation failure is catastrophic, we will invalidate
                // this port
                // 
                if (typeChanged)
                {
                    // Terminate & delete the allocated port
                    Port11TerminatePort(Port, FALSE);
                    Port11FreePort(Port, FALSE);

                    // Port is gone
                    Port->ChildPort = NULL;
                }

                // Invalidate this port. This 
                // will block everything from getting to this port
                BasePortInvalidatePort(Port);
            }
            
            // Now restart the new port
            ndisStatus = Mp11RestartPort(Port->Adapter, Port);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Failed to restart port %d after change of Op Mode. Status = 0x%08x\n", 
                    Port->PortNumber, ndisStatus));
            }

        }
        else
        {
            // Update the opmode. The port type remains the same
            returnNdisStatus = Port11SetOperationMode(Port, dot11CurrentOperationMode->uCurrentOpMode);
            MPASSERT(returnNdisStatus == NDIS_STATUS_SUCCESS);

            // And pass it to the VNIC
            returnNdisStatus = VNic11SetOperationMode(PORT_GET_VNIC(Port), dot11CurrentOperationMode);
            MPASSERT(returnNdisStatus == NDIS_STATUS_SUCCESS);
        }
        
    }while (FALSE);
    

    return returnNdisStatus;
}



NDIS_STATUS
Port11HandleOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{
    NDIS_STATUS                 ndisStatus;
    NDIS_OID                    oid;
    NDIS_REQUEST_TYPE           requestType;
    BOOLEAN                     forwardToPort = TRUE;
    
    // 
    // First we preprocess the OID to validate the buffers, etc
    //
    ndisStatus = PortPreprocessOid(Port, NdisOidRequest);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_OID, DBG_SERIOUS, ("Preprocessing of NDIS_OID_REQUEST failed. Status = 0x%08x\n", 
            ndisStatus));

        MPASSERT(ndisStatus != NDIS_STATUS_PENDING);
        return ndisStatus;
    }
    
    // Depending on the OID, hand it to the appropriate component for processing
    oid = NdisOidRequest->DATA.QUERY_INFORMATION.Oid; // Oid is at same offset for all RequestTypes
    requestType = NdisOidRequest->RequestType;

    // Hold this in the pending OID request structure
    Port->PendingOidRequest = NdisOidRequest;

    //
    // Some port specific requests need to be treated special
    //
    switch (oid)
    {
        case OID_DOT11_CURRENT_OPERATION_MODE:
            switch (requestType)
            {
                case NdisRequestSetInformation:
                    {
                        NdisOidRequest->DATA.SET_INFORMATION.BytesRead = sizeof(DOT11_CURRENT_OPERATION_MODE);   
                        
                        //
                        // Handle the OID
                        //
                        ndisStatus = PortSetCurrentOperationMode(Port, NdisOidRequest);
                        if (ndisStatus != NDIS_STATUS_SUCCESS)
                        {
                            MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("PortSetCurrentOperationMode failed. Status = 0x%08x\n", ndisStatus));
                            break;
                        }

                        forwardToPort = FALSE;
                    }
                    break;
                    
                default:
                    break;
            }
            break;
            
        default:
            break;
    }

    if (forwardToPort)
    {
        // Pass to the port for processing
        ndisStatus = Port->RequestHandler(Port, NdisOidRequest);
    }

    if (ndisStatus != NDIS_STATUS_PENDING)
    {
        // No OID requests pending
        Port->PendingOidRequest = NULL;
    }
    
    return ndisStatus;
}



VOID
Port11CompletePendingOidRequest(
    _In_  PMP_PORT                Port,
    _In_  NDIS_STATUS             NdisStatus
    )
{
    PNDIS_OID_REQUEST           pendingRequest;
    
    MpEntry;
    
    MPASSERT(Port->PendingOidRequest != NULL);

    //
    // Complete this OID to the OS
    //
    pendingRequest = Port->PendingOidRequest;
    Port->PendingOidRequest = NULL;

    Mp11CompleteOidRequest(
        Port->Adapter,
        Port,
        pendingRequest,
        NdisStatus
        );

    MpExit;
}



NDIS_STATUS
Port11HandleDirectOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{
    // Pass to the port for processing
    return Port->DirectRequestHandler(Port, NdisOidRequest);
}


VOID
Port11CompletePendingDirectOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest,
    _In_  NDIS_STATUS             NdisStatus
    )
{
    // Just call NDis
    NdisMDirectOidRequestComplete(
        Port->MiniportAdapterHandle, 
        NdisOidRequest, 
        NdisStatus
        );
}

