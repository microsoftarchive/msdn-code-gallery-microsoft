/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_oids.h

Abstract:
    Contains the defines for OID handling for the Station layer layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

typedef NDIS_STATUS (*STA_QUERY_ALGO_PAIR_FUNC)(
    _In_  PMP_PORT                        Port,
    _In_  DOT11_BSS_TYPE                  BssType,
    _Out_ PDOT11_AUTH_CIPHER_PAIR_LIST    AuthCipherList,
    _In_  ULONG                           TotalLength
    );

/** Maximum number of MAC addresses we support in the excluded list */
#define STA_EXCLUDED_MAC_ADDRESS_MAX_COUNT      4
/** Max number of desired BSSIDs we can handle */
#define STA_DESIRED_BSSID_MAX_COUNT             8
/** Max number of desired PHYs we can handle */
#define STA_DESIRED_PHY_MAX_COUNT               5
/** Max number of PMKID we can handle */
#define STA_PMKID_MAX_COUNT                     3
/** Max number of enabled multicast cipher algorithms */
#define STA_MULTICAST_CIPHER_MAX_COUNT          6

VOID
StaInitializeStationConfig(
    _In_  PMP_EXTSTA_PORT         Station
    );

VOID
StaResetStationConfig(
    _In_  PMP_EXTSTA_PORT         Station
    );

NDIS_STATUS
StaGetAlgorithmPair(
    _In_  PMP_PORT                    Port,
    _In_  DOT11_BSS_TYPE              BssType,
    _In_  STA_QUERY_ALGO_PAIR_FUNC    QueryFunction,
    _Outptr_result_maybenull_ PDOT11_AUTH_CIPHER_PAIR *AlgoPairs,
    _Out_ PULONG                      NumAlgoPairs
    );

NDIS_STATUS
StaQuerySupportedUnicastAlgorithmPairCallback(
    _In_  PMP_PORT                Port,
    _In_  DOT11_BSS_TYPE          BssType,
    _Out_writes_bytes_(TotalLength) PDOT11_AUTH_CIPHER_PAIR_LIST    AuthCipherList,
    _In_ _In_range_(sizeof(DOT11_AUTH_CIPHER_PAIR_LIST) - sizeof(DOT11_AUTH_CIPHER_PAIR), ULONG_MAX)   ULONG                   TotalLength
    );

NDIS_STATUS
StaQuerySupportedMulticastAlgorithmPairCallback(
    _In_  PMP_PORT                Port,
    _In_  DOT11_BSS_TYPE          BssType,
    _Out_writes_bytes_(TotalLength) PDOT11_AUTH_CIPHER_PAIR_LIST    AuthCipherList,
    _In_ _In_range_(sizeof(DOT11_AUTH_CIPHER_PAIR_LIST) - sizeof(DOT11_AUTH_CIPHER_PAIR), ULONG_MAX)   ULONG                   TotalLength
    );

NDIS_STATUS
StaQueryExtStaCapability(
    _In_  PMP_PORT                Port,
    _Out_ PDOT11_EXTSTA_CAPABILITY   Dot11ExtStaCap
    );

VOID
StaSetDefaultAuthAlgo(
    _In_  PMP_EXTSTA_PORT         Station
    );

VOID
StaSetDefaultCipher(
    _In_  PMP_EXTSTA_PORT         Station
    );

NDIS_STATUS
StaSetPowerSavingLevel(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  ULONG                   PowerSavingLevel
    );

VOID
StaPowerSleep(
    _In_  PMP_EXTSTA_PORT         Station
    );

VOID
StaPowerWakeup(
    _In_  PMP_EXTSTA_PORT         Station
    );

