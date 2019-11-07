/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_config.h

Abstract:
    ExtAP configuration definitions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-10-2007    Created
    
Notes:

--*/
#pragma once
    
#ifndef _AP_CONFIG_H
#define _AP_CONFIG_H

/** Forward declarations */
typedef struct _MP_EXTAP_PORT      MP_EXTAP_PORT, *PMP_EXTAP_PORT;

/**
 * Holds current ExtAP configuration of the miniport that
 * are not managed by association manager. 
 * These configurations can be updated and/or queried via OIDs request from the OS. 
 * A lock is NOT needed when updating/querying these configurations.
 * This data is stateless so we don't need a flag to indicate whether it is initialized or not.
 */

#define AP_DESIRED_PHY_MAX_COUNT    1

typedef struct _AP_CONFIG 
{
    /** ExtAP port */
    PMP_EXTAP_PORT          ApPort;

    /** The types of auto configuration for 802.11 parameters that are enabled */
    ULONG                   AutoConfigEnabled;
    
    /** Beacon period, in TUs */
    ULONG                   BeaconPeriod;

    /** DTIM period, in beacon intervals */
    ULONG                   DTimPeriod;
#if 0       // remove it after confirmed
    /** RTS threshold */
    ULONG                   RtsThreshold;

    /** Short retry limit */
    ULONG                   ShortRetryLimit;

    /** Long retry limit */
    ULONG                   LongRetryLimit;

    /** Fragmentation threshold */
    ULONG                   FragmentationThreshold;

    /** Current operating frequency channel list for the DSSS/HRDSSS/ERP PHY */
    ULONG                   CurrentChannel;
    
    /** Current operating frequency channel list for the OFDM PHY */
    ULONG                   CurrentFrequency;
    
    /** Current PHY ID */
    ULONG                   CurrentPhyId;
    
#endif
    
    /** Default key ID */
    ULONG                   CipherDefaultKeyId;
    
    /** Desired PHY ID list */
    ULONG                   DesiredPhyList[AP_DESIRED_PHY_MAX_COUNT];
    ULONG                   DesiredPhyCount;

    /** Additonal IEs for beacon */
    ULONG                   AdditionalBeaconIESize;
    PVOID                   AdditionalBeaconIEData;

    /** Additonal IEs for probe response */
    ULONG                   AdditionalResponseIESize;
    PVOID                   AdditionalResponseIEData;

    /** Saved copy of beacon IEs for association completion **/
    ULONG                   ApBeaconIESize;
    PVOID                   ApBeaconIEData;

    /** If we cannot sustain AP for some reason this counter is non-zero */
    ULONG                   CannotSustainApRef;
}AP_CONFIG, *PAP_CONFIG;

/** AP configurations */

/** Initialize AP configurations */
NDIS_STATUS
ApInitializeConfig(
    _In_ PAP_CONFIG ApConfig,
    _In_ PMP_EXTAP_PORT ApPort
    );

/** Deinitialize AP configurations */
VOID
ApDeinitializeConfig(
    _In_ PAP_CONFIG ApConfig
    );

/** Internal functions that are invoked by other configuration functions */

/** 
 * Set AP configuration to its default based on the hardware capability 
 * and registry settings
 */
VOID
CfgSetDefault(
    _In_ PAP_CONFIG ApConfig
    );

/** Clean up AP configuration */
VOID
CfgCleanup(
    _In_ PAP_CONFIG ApConfig
    );

/**
 * Internal functions for OIDs
 */
VOID
CfgQueryAutoConfigEnabled(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG AutoConfigEnabled
    );

#define ALLOWED_AUTO_CONFIG_FLAGS (DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG | DOT11_MAC_AUTO_CONFIG_ENABLED_FLAG)

NDIS_STATUS
CfgSetAutoConfigEnabled(
    _In_ PAP_CONFIG Config,
    _In_ ULONG AutoConfigEnabled
    );
  
VOID
FORCEINLINE
CfgQueryBeaconPeriod(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG BeaconPeriod
    )
{
    *BeaconPeriod = Config->BeaconPeriod;
}

NDIS_STATUS
CfgSetBeaconPeriod(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 65535) ULONG BeaconPeriod
    );
    
VOID
FORCEINLINE
CfgQueryDTimPeriod(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG DTimPeriod
    )
{
    *DTimPeriod = Config->DTimPeriod;
}

NDIS_STATUS
CfgSetDTimPeriod(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 255) ULONG DTimPeriod
    );

#if 0
VOID
FORCEINLINE
CfgQueryRtsThreshold(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG RtsThreshold
    )
{
    *RtsThreshold = Config->RtsThreshold;
}

NDIS_STATUS
CfgSetRtsThreshold(
    _In_ PAP_CONFIG Config,
    _In_range_(0, 2347) ULONG RtsThreshold
    );

VOID
FORCEINLINE
CfgQueryShortRetryLimit(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG ShortRetryLimit
    )
{
    *ShortRetryLimit = Config->ShortRetryLimit;
}

NDIS_STATUS
CfgSetShortRetryLimit(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 255) ULONG ShortRetryLimit
    );

VOID
FORCEINLINE
CfgQueryLongRetryLimit(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG LongRetryLimit
    )
{
    *LongRetryLimit = Config->LongRetryLimit;
}

NDIS_STATUS
CfgSetLongRetryLimit(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 255) ULONG LongRetryLimit
    );

VOID
FORCEINLINE
CfgQueryFragmentationThreshold(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG FragmentationThreshold
    )
{
    *FragmentationThreshold = Config->FragmentationThreshold;
}

NDIS_STATUS
CfgSetFragmentationThreshold(
    _In_ PAP_CONFIG Config,
    _In_range_(256, 2346) ULONG FragmentationThreshold
    );
#endif

VOID
CfgQueryCurrentChannel(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG CurrentChannel
    );

NDIS_STATUS
CfgSetCurrentChannel(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 14) ULONG CurrentChannel
    );

VOID
CfgQueryCurrentFrequency(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG CurrentFrequency
    );

NDIS_STATUS
CfgSetCurrentFrequency(
    _In_ PAP_CONFIG Config,
    _In_range_(0, 200) ULONG CurrentFrequency
    );

VOID
FORCEINLINE
CfgQueryCipherDefaultKeyId(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG CipherDefaultKeyId
    )
{
    *CipherDefaultKeyId = Config->CipherDefaultKeyId;
}

NDIS_STATUS
CfgSetCipherDefaultKeyId(
    _In_ PAP_CONFIG Config,
    _In_ ULONG CipherDefaultKeyId
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
CfgQueryDesiredPhyList(
    _In_  PAP_CONFIG Config,
    _Out_ PDOT11_PHY_ID_LIST DesiredPhyList,
    _In_  ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_
          PULONG BytesNeeded
    );

NDIS_STATUS
CfgSetDesiredPhyList(
    _In_ PAP_CONFIG Config,
    _In_ PDOT11_PHY_ID_LIST DesiredPhyList
    );

VOID
CfgQueryCurrentPhyId(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG CurrentPhyId
    );

NDIS_STATUS
CfgSetCurrentPhyId(
    _In_ PAP_CONFIG Config,
    _In_ ULONG CurrentPhyId
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
CfgQueryAdditionalIe(
    _In_    PAP_CONFIG Config,
    _Out_   PDOT11_ADDITIONAL_IE AdditionalIe,
    _In_    ULONG InformationBufferLength,
    _Out_   PULONG BytesWritten,
    _Out_when_invalid_ndis_length_
            PULONG BytesNeeded
    );

NDIS_STATUS
CfgSetAdditionalIe(
    _In_ PAP_CONFIG Config,
    _In_ PDOT11_ADDITIONAL_IE AdditionalIe
    );

VOID
CfgResetAdditionalIe(
    _In_ PAP_CONFIG Config
    );

VOID
CfgSaveBeaconIe(
    _In_ PAP_CONFIG Config,
    _In_ PVOID ApBeaconIEData,
    _In_ ULONG ApBeaconIESize
    );

#endif // _AP_CONFIG_H

