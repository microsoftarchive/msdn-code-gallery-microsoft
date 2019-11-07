/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_oids.h

Abstract:
    ExtAP OID APIs with strong type
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-13-2007    Created

Notes:

--*/
#pragma once
    
#ifndef _AP_OIDAPI_H_
#define _AP_OIDAPI_H_

/** OID_DOT11_AUTO_CONFIG_ENABLED */
NDIS_STATUS
ApQueryAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG AutoConfigEnabled
    );

NDIS_STATUS
ApSetAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG AutoConfigEnabled
    );

/** OID_DOT11_NIC_POWER_STATE */
NDIS_STATUS
ApQueryNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ BOOLEAN * NicPowerState
    );

NDIS_STATUS
ApSetNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ BOOLEAN NicPowerState
    );

/** OID_DOT11_OPERATIONAL_RATE_SET */
NDIS_STATUS
ApQueryOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_RATE_SET OperationalRateSet
    );

NDIS_STATUS
ApSetOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_RATE_SET OperationalRateSet
    );

/** OID_DOT11_BEACON_PERIOD */
NDIS_STATUS
ApQueryBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG BeaconPeriod
    );

NDIS_STATUS
ApSetBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 65535) ULONG BeaconPeriod
    );

/** OID_DOT11_DTIM_PERIOD */
NDIS_STATUS
ApQueryDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG DTimPeriod
    );

NDIS_STATUS
ApSetDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 255) ULONG DTimPeriod
    );

/** OID_DOT11_RTS_THRESHOLD */
NDIS_STATUS
ApQueryRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG RtsThreshold
    );

NDIS_STATUS
ApSetRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(0, 2347) ULONG RtsThreshold
    );

/** OID_DOT11_SHORT_RETRY_LIMIT */
NDIS_STATUS
ApQueryShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG ShortRetryLimit
    );

NDIS_STATUS
ApSetShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 255) ULONG ShortRetryLimit
    );

/** OID_DOT11_LONG_RETRY_LIMIT */
NDIS_STATUS
ApQueryLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG LongRetryLimit
    );

NDIS_STATUS
ApSetLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 255) ULONG LongRetryLimit
    );

/** OID_DOT11_FRAGMENTATION_THRESHOLD */
NDIS_STATUS
ApQueryFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG FragmentationThreshold
    );

NDIS_STATUS
ApSetFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(256, 2346) ULONG FragmentationThreshold
    );

/** OID_DOT11_AVAILABLE_CHANNEL_LIST */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApQueryAvailableChannelList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AVAILABLE_CHANNEL_LIST AvailableChannelList
    );

/** Set not applicable */

/** OID_DOT11_CURRENT_CHANNEL */
NDIS_STATUS
ApQueryCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CurrentChannel
    );

NDIS_STATUS
ApSetCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 14) ULONG CurrentChannel
    );

/** OID_DOT11_AVAILABLE_FREQUENCY_LIST */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApQueryAvailableFrequencyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AVAILABLE_FREQUENCY_LIST AvailableFrequencyList
    );

/** Set not applicable */

/** OID_DOT11_CURRENT_FREQUENCY */
NDIS_STATUS
ApQueryCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CurrentFrequency
    );

NDIS_STATUS
ApSetCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(0, 200) ULONG CurrentFrequency
    );

/** OID_DOT11_DESIRED_SSID_LIST */
NDIS_STATUS
ApQueryDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_SSID_LIST SsidList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApSetDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_SSID_LIST SsidList
    );

/** OID_DOT11_EXCLUDE_UNENCRYPTED */
NDIS_STATUS
ApQueryExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ BOOLEAN * ExcludeUnencrypted
    );

NDIS_STATUS
ApSetExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ BOOLEAN ExcludeUnencrypted
    );

/** OID_DOT11_STATISTICS */
NDIS_STATUS
ApQueryStatistics(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_range_(sizeof(DOT11_STATISTICS), ULONG_MAX) ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */

/** OID_DOT11_PRIVACY_EXEMPTION_LIST */
NDIS_STATUS
ApQueryPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApSetPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList
    );

/** OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM */
NDIS_STATUS
ApQueryEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApSetEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm
    );

/** OID_DOT11_SUPPORTED_UNICAST_ALGORITHM_PAIR */
NDIS_STATUS
ApQuerySupportedUnicastAlgorithmPair(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AUTH_CIPHER_PAIR_LIST SupportedUnicastAlgorithmPair
    );

/** Set not applicable */

/** OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApQueryEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApSetEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm
    );

/** OID_DOT11_SUPPORTED_MULTICAST_ALGORITHM_PAIR */
NDIS_STATUS
ApQuerySupportedMulticastAlgorithmPair(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AUTH_CIPHER_PAIR_LIST SupportedMulticastAlgorithmPair
    );

/** Set not applicable */

/** OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApQueryEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApSetEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm
    );

/** OID_DOT11_CIPHER_DEFAULT_KEY_ID */
NDIS_STATUS
ApQueryCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CipherDefaultKeyId
    );

NDIS_STATUS
ApSetCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG CipherDefaultKeyId
    );

/** OID_DOT11_CIPHER_DEFAULT_KEY */
/** Query not applicable */

NDIS_STATUS
ApSetCipherDefaultKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_CIPHER_DEFAULT_KEY_VALUE CipherDefaultKey
    );

/** OID_DOT11_CIPHER_KEY_MAPPING_KEY */
/** Query not applicable */

NDIS_STATUS
ApSetCipherKeyMappingKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_BYTE_ARRAY CipherKeyMappingKeyByteArray
    );

/** OID_DOT11_ENUM_PEER_INFO */
NDIS_STATUS
ApEnumPeerInfo(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_PEER_INFO_LIST PeerInfo,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */

/** OID_DOT11_DISASSOCIATE_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApDisassociatePeerRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_DISASSOCIATE_PEER_REQUEST DisassociateRequest
    );

/** OID_DOT11_DESIRED_PHY_LIST */
NDIS_STATUS
ApQueryDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_PHY_ID_LIST DesiredPhyList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApSetDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_PHY_ID_LIST DesiredPhyList
    );

/** OID_DOT11_CURRENT_PHY_ID */
NDIS_STATUS
ApQueryCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CurrentPhyId
    );

NDIS_STATUS
ApSetCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG CurrentPhyId
    );

/** OID_DOT11_PORT_STATE_NOTIFICATION */
/** Query not applicable */

NDIS_STATUS
ApSetPortStateNotification(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_PORT_STATE_NOTIFICATION PortStateNotification
    );

/** OID_DOT11_SCAN_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApScanRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PVOID ScanRequestId,
    _In_ PDOT11_SCAN_REQUEST_V2 ScanRequest,
    _In_ ULONG ScanRequestBufferLength
    );

/** OID_DOT11_INCOMING_ASSOCIATION_DECISION */
/** Query not applicable */

NDIS_STATUS
ApSetIncomingAssociationDecision(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_INCOMING_ASSOC_DECISION IncomingAssociationDecision
    );

/** OID_DOT11_ADDITIONAL_IE */
NDIS_STATUS
ApQueryAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_ADDITIONAL_IE AdditionalIe,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApSetAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_ADDITIONAL_IE AdditionalIe
    );

/** OID_DOT11_WPS_ENABLED */
NDIS_STATUS
ApQueryWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ BOOLEAN * WpsEnabled
    );

NDIS_STATUS
ApSetWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ BOOLEAN WpsEnabled
    );


/** OID_DOT11_START_AP_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApStartApRequest(
    _In_ PMP_EXTAP_PORT ApPort
    );

/** OID_GEN_CURRENT_PACKET_FILTER */
/** Query is handled by the base port */

NDIS_STATUS
ApSetPacketFilter(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG PacketFilter
    );


/** OID_DOT11_CURRENT_OPERATION_MODE */
/** OID_DOT11_FLUSH_BSS_LIST */
/** OID_DOT11_SCAN_REQUEST */
/** Handled by MP Shim layer */

/** Request methods */
/** OID_DOT11_RESET_REQUEST */
NDIS_STATUS
ApResetRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_RESET_REQUEST ResetRequest,
    _Out_ PDOT11_STATUS_INDICATION StatusIndication
    );

/** OID_DOT11_ENUM_BSS_LIST */
// TODO: move this OID to base port
NDIS_STATUS
ApEnumerateBSSList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Inout_ PVOID InformationBuffer,
    _In_ ULONG InputBufferLength,
    _In_ ULONG OutputBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

#endif  // _AP_OIDAPI_H_

