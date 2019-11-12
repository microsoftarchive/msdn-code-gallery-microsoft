/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_oids.h

Abstract:
    ExtAP OID processing code
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-12-2007    Created

Notes:

--*/
#ifndef _AP_OIDS_H_

#define _AP_OIDS_H_

/** OID_DOT11_AUTO_CONFIG_ENABLED */
NDIS_STATUS
ApOidQueryAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_NIC_POWER_STATE */
NDIS_STATUS
ApOidQueryNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_OPERATIONAL_RATE_SET */
NDIS_STATUS
ApOidQueryOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_BEACON_PERIOD */
NDIS_STATUS
ApOidQueryBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_DTIM_PERIOD */
NDIS_STATUS
ApOidQueryDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_RTS_THRESHOLD */
NDIS_STATUS
ApOidQueryRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_SHORT_RETRY_LIMIT */
NDIS_STATUS
ApOidQueryShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_LONG_RETRY_LIMIT */
NDIS_STATUS
ApOidQueryLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_FRAGMENTATION_THRESHOLD */
NDIS_STATUS
ApOidQueryFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_AVAILABLE_CHANNEL_LIST */
NDIS_STATUS
ApOidQueryAvailableChannelList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */

/** OID_DOT11_CURRENT_CHANNEL */
NDIS_STATUS
ApOidQueryCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_AVAILABLE_FREQUENCY_LIST */
NDIS_STATUS
ApOidQueryAvailableFrequencyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */

/** OID_DOT11_CURRENT_FREQUENCY */
NDIS_STATUS
ApOidQueryCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_DESIRED_SSID_LIST */
NDIS_STATUS
ApOidQueryDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_EXCLUDE_UNENCRYPTED */
NDIS_STATUS
ApOidQueryExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_STATISTICS */
NDIS_STATUS
ApOidQueryStatistics(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */

/** OID_DOT11_PRIVACY_EXEMPTION_LIST */
NDIS_STATUS
ApOidQueryPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM */
NDIS_STATUS
ApOidQueryEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

#if 0
/** OID_DOT11_SUPPORTED_UNICAST_ALGORITHM_PAIR */
NDIS_STATUS
ApOidQuerySupportedUnicastAlgorithmPair(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */
#endif
/** OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApOidQueryEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

#if 0
/** OID_DOT11_SUPPORTED_MULTICAST_ALGORITHM_PAIR */
NDIS_STATUS
ApOidQuerySupportedMulticastAlgorithmPair(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */
#endif
/** OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApOidQueryEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_CIPHER_DEFAULT_KEY_ID */
NDIS_STATUS
ApOidQueryCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_CIPHER_DEFAULT_KEY */
/** Query not applicable */

NDIS_STATUS
ApOidSetCipherDefaultKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_CIPHER_KEY_MAPPING_KEY */
/** Query not applicable */

NDIS_STATUS
ApOidSetCipherKeyMappingKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_ENUM_PEER_INFO */
NDIS_STATUS
ApOidEnumPeerInfo(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */

/** OID_DOT11_DISASSOCIATE_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApOidDisassociateRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_DESIRED_PHY_LIST */
NDIS_STATUS
ApOidQueryDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_CURRENT_PHY_ID */
NDIS_STATUS
ApOidQueryCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_EXTAP_CAPABILITY */
NDIS_STATUS
ApOidQueryExtApCapability(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Set not applicable */

/** OID_DOT11_PORT_STATE_NOTIFICATION */
/** Query not applicable */

NDIS_STATUS
ApOidSetPortStateNotification(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_SCAN_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApOidScanRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_INCOMING_ASSOCIATION_DECISION */
/** Query not applicable */

NDIS_STATUS
ApOidSetIncomingAssociationDecision(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_ADDITIONAL_IE */
NDIS_STATUS
ApOidQueryAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_WPS_ENABLED */
NDIS_STATUS
ApOidQueryWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

NDIS_STATUS
ApOidSetWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );


/** OID_DOT11_START_AP_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApOidStartApRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_GEN_CURRENT_PACKET_FILTER */
/** Query is handled by the base port */

NDIS_STATUS
ApOidSetPacketFilter(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    );

/** OID_DOT11_CURRENT_OPERATION_MODE */
/** OID_DOT11_ENUM_BSS_LIST */
/** OID_DOT11_FLUSH_BSS_LIST */
/** Handled by MP Shim layer */

/** Request methods */
/** OID_DOT11_RESET_REQUEST */
NDIS_STATUS
ApOidResetRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PNDIS_OID_REQUEST NdisOidRequest
    );

/** OID_DOT11_ENUM_BSS_LIST */
// TODO: move this OID to base port
NDIS_STATUS
ApOidEnumerateBSSList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Inout_ PVOID InformationBuffer,
    _In_ ULONG InputBufferLength,
    _In_ ULONG OutputBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

#endif  // _AP_OIDS_H_
