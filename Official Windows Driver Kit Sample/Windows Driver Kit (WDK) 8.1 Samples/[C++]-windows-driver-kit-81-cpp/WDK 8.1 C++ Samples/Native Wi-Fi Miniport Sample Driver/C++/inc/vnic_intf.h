/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    vnic_intf.h

Abstract:
    Contains interfaces into the VNIC
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

//
// Forward declaration
//
typedef struct _ADAPTER         ADAPTER, *PADAPTER;
typedef struct _HVL             HVL, *PHVL;
typedef struct _HW              HW, *PHW;
typedef struct _MP_PORT         MP_PORT, *PMP_PORT;
typedef struct _VNIC            VNIC, *PVNIC;

/**
 * PNP functionality
 */
NDIS_STATUS
VNic11Allocate(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_maybenull_ PVNIC*        ppVNic,
    _In_  PMP_PORT                Port
    );

VOID
VNic11Free(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11Initialize(
    _In_  PVNIC                   VNic,
    _In_  PVOID                   Hvl,
    _In_  PVOID                   Hw,
    _In_  MP_PORT_TYPE            PortType,
    _In_  NDIS_PORT_NUMBER        PortNumber
    );

VOID
VNic11Terminate(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11Pause(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11Restart(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11Dot11Reset(
    _In_  PVNIC                   VNic,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest
    );

NDIS_STATUS
VNic11Dot11ResetComplete(
    _In_  PVNIC                   pVNic
    );
    
/**
 * Capability queries
 */
BOOLEAN 
VNic11WEP104Implemented(
    _In_  PVNIC                   VNic
    );

BOOLEAN 
VNic11WEP40Implemented(
    _In_  PVNIC                   VNic
    );

BOOLEAN 
VNic11TKIPImplemented(
    _In_  PVNIC                   VNic
    );

BOOLEAN 
VNic11CCMPImplemented(
    _In_  PVNIC                   VNic,
    _In_  DOT11_BSS_TYPE          bssType
    );

ULONG
VNic11DefaultKeyTableSize(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11KeyMappingKeyTableSize(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11PerStaKeyTableSize(
    _In_  PVNIC                   VNic
    );

USHORT
VNic11QueryRSNCapabilityField(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11QueryReceiveBufferSpace(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11QueryTransmitBufferSpace(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11QueryMaxMPDULength(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11QueryOperationModeCapability(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_OPERATION_MODE_CAPABILITY    Dot11OpModeCapability
    );

NDIS_STATUS
VNic11QueryOptionalCapability(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_OPTIONAL_CAPABILITY  Dot11OptionalCapability
    );


/*
 * Generic OIDs
 */
PDOT11_MAC_ADDRESS
VNic11QueryMACAddress(
    _In_  PVNIC                   VNic
    );

PDOT11_MAC_ADDRESS
VNic11QueryHardwareAddress(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11QueryHardwareStatus(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetPacketFilter(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   PacketFilter
    );

NDIS_STATUS
VNic11QueryMulticastList(
    _In_  PVNIC                   VNic,
    _Out_ PVOID                   AddressBuffer,
    _In_  ULONG                   AddressBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    );

NDIS_STATUS
VNic11SetMulticastList(
    _In_  PVNIC                   VNic,
    _In_  PVOID                   AddressBuffer,
    _In_  ULONG                   AddressBufferLength
    );

ULONG
VNic11QueryLookahead(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetLookahead(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   Lookahead
    );

NDIS_STATUS
VNic11QueryInterruptModerationSettings(
    _In_  PVNIC                   VNic,
    _Out_ PNDIS_INTERRUPT_MODERATION_PARAMETERS   IntModParams
    );

NDIS_STATUS
VNic11SetInterruptModerationSettings(
    _In_  PVNIC                   VNic,
    _In_  PNDIS_INTERRUPT_MODERATION_PARAMETERS   IntModParams
    );

NDIS_STATUS
VNic11QueryLinkParameters(
    _In_  PVNIC                   VNic,
    _Out_ PNDIS_LINK_PARAMETERS   LinkParams
    );

NDIS_STATUS
VNic11SetLinkParameters(
    _In_  PVNIC                   VNic,
    _In_  PNDIS_LINK_PARAMETERS   LinkParams
    );

NDIS_STATUS
VNic11QueryVendorDescription(
    _In_  PVNIC                   VNic,
    _Out_ PVOID                   InformationBuffer,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    );

ULONG
VNic11QueryVendorId(
    _In_  PVNIC                   VNic
     );

/** 
 * Dot11 OIDs
 */

PDOT11_MAC_ADDRESS
VNic11QueryCurrentBSSID(
    _In_  PVNIC                   VNic
    );


NDIS_STATUS
VNic11SetOperationMode(
    _In_  PVNIC                   pVNic,
    _In_  PDOT11_CURRENT_OPERATION_MODE Dot11CurrentOperationMode
    );

NDIS_STATUS
VNic11SetCurrentBSSType(
    _In_  PVNIC                   VNic,
    _In_  DOT11_BSS_TYPE          Type
    );
    
ULONG
VNic11QueryBeaconPeriod(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetBeaconPeriod(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   BeaconPeriod
    );

BOOLEAN
VNic11QueryCFPollable(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11QueryCurrentOptionalCapability(
    _In_  PVNIC                   VNic,
    _In_ PDOT11_CURRENT_OPTIONAL_CAPABILITY   Dot11CurrentOptionalCapability    
    );

NDIS_STATUS
VNic11QuerySupportedPowerLevels(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_SUPPORTED_POWER_LEVELS   Dot11SupportedPowerLevels
    );

ULONG
VNic11QueryCurrentTXPowerLevel(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
VNic11QueryATIMWindow(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetATIMWindow(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   Value
    );

NDIS_STATUS
VNic11SetPowerMgmtMode(
    _In_  PVNIC                   VNic,
    _In_  PDOT11_POWER_MGMT_MODE  PMMode
    );

BOOLEAN
VNic11QueryNicPowerState(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

NDIS_STATUS
VNic11SetNicPowerState(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 PowerState,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryHardwarePhyState(
    _In_  PVNIC                   VNic
    );

VOID
VNic11SetCTSToSelfOption(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 Enable
    );

ULONG
VNic11QueryRTSThreshold(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetRTSThreshold(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   BeaconPeriod
    );

ULONG
VNic11QueryFragmentationThreshold(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetFragmentationThreshold(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   BeaconPeriod
    );

ULONG
VNic11QueryLongRetryLimit(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11QueryShortRetryLimit(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11QueryCountryString(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_COUNTRY_OR_REGION_STRING Dot11CountryString
    );

ULONG
VNic11QueryCurrentRegDomain(
    _In_  PVNIC                   VNic
    );

BOOLEAN
VNic11QueryMultiDomainCapabilityImplemented(
    _In_  PVNIC                   VNic
    );

BOOLEAN
VNic11QueryMultiDomainCapabilityEnabled(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetMultiDomainCapabilityEnabled(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 MultiDomainCapabilityEnabled
    );

NDIS_STATUS
VNic11SetSafeModeOption(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SafeModeEnabled
    );

NDIS_STATUS
VNic11QueryDot11Statistics(
    _In_  PVNIC                   VNic,
    _In_  PDOT11_STATISTICS       Dot11Stats,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    );

NDIS_STATUS
VNic11QueryRecvSensitivityList(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   MaxEntries,
    _Inout_ PDOT11_RECV_SENSITIVITY_LIST Dot11RecvSensitivityList
    );

NDIS_STATUS
VNic11QuerySupportedRXAntenna(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_SUPPORTED_ANTENNA_LIST   Dot11SupportedAntennaList    
    );

NDIS_STATUS
VNic11QuerySupportedTXAntenna(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_SUPPORTED_ANTENNA_LIST   Dot11SupportedAntennaList
    );

NDIS_STATUS
VNic11QueryDiversitySelectionRX(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_DIVERSITY_SELECTION_RX_LIST Dot11DiversitySelectionRXList
    );

NDIS_STATUS
VNic11QueryRegDomainsSupportValue(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_REG_DOMAINS_SUPPORT_VALUE    Dot11RegDomainsSupportValue
    );

ULONG
VNic11QueryRFUsage(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11QueryMaxReceiveLifetime(
    _In_  PVNIC                   VNic
    );

ULONG
VNic11QueryMaxTransmitMSDULifetime(
    _In_  PVNIC                   VNic
    );

DOT11_TEMP_TYPE
VNic11QueryTempType(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

/*
 * PHY selection related
 */
NDIS_STATUS
VNic11QuerySupportedPHYTypes(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   NumMaxEntries,
    _Out_ PDOT11_SUPPORTED_PHY_TYPES Dot11SupportedPhyTypes
    );

DOT11_PHY_TYPE
VNic11QueryCurrentPhyType(
    _In_  PVNIC                   VNic
    );

DOT11_PHY_TYPE
VNic11DeterminePHYType(
    _In_  PVNIC                   VNic,
    _In_  DOT11_CAPABILITY        Capability,
    _In_  UCHAR                   Channel
    );

NDIS_STATUS
VNic11SetDesiredPhyIdList(
    _In_  PVNIC                   VNic,
    _In_reads_(PhyIDCount)  PULONG                  PhyIDList,
    _In_  ULONG                   PhyIDCount
    );

NDIS_STATUS
VNic11SetOperatingPhyId(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   PhyId
    );

ULONG
VNic11QueryOperatingPhyId(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetOperationalRateSet(
    _In_  PVNIC                   VNic,
    _In_  PDOT11_RATE_SET         Dot11RateSet,
    _In_  BOOLEAN                 SelectedPhy 
    );

ULONG
VNic11QuerySelectedPhyId(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetSelectedPhyId(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   PhyId
    );

/*
 * Rates
 */
VOID
VNic11QueryBasicRateSet(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_RATE_SET         Dot11RateSet,
    _In_  BOOLEAN                 SelectedPhy
    );

VOID
VNic11QueryOperationalRateSet(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_RATE_SET         Dot11RateSet,
    _In_  BOOLEAN                 SelectedPhy
    );

UCHAR
VNic11SelectTXDataRate(
    _In_  PVNIC                   VNic,
    _In_  PDOT11_RATE_SET         PeerRateSet,
    _In_  ULONG                   LinkQuality
    );

NDIS_STATUS
VNic11QuerySupportedDataRatesValue(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_SUPPORTED_DATA_RATES_VALUE_V2    Dot11SupportedDataRatesValue,
    _In_  BOOLEAN                 SelectedPhy 
    );

NDIS_STATUS
VNic11ValidateOperationalRates(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   PhyId,
    _In_reads_bytes_(RateSetLength)  PUCHAR                  DataRateSet,
    _In_  ULONG                   RateSetLength
    );

NDIS_STATUS
VNic11QueryDataRateMappingTable(
    _In_  PVNIC                   VNic,
    _Out_ PDOT11_DATA_RATE_MAPPING_TABLE  DataRateMappingTable,
    _In_  ULONG                   TotalLength
    );

/*
 * Phy specific configuration parameters
 */
BOOLEAN
VNic11QueryShortSlotTimeOptionImplemented(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryShortSlotTimeOptionEnabled(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryDsssOfdmOptionImplemented(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryDsssOfdmOptionEnabled(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryShortPreambleOptionImplemented(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryPbccOptionImplemented(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryChannelAgilityPresent(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryChannelAgilityEnabled(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
VNic11QueryCCAModeSupported(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
VNic11QueryCurrentCCAMode(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

DOT11_DIVERSITY_SUPPORT
VNic11QueryDiversitySupport(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
VNic11QueryEDThreshold(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryErpPbccOptionEnabled(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
VNic11QueryErpPbccOptionImplemented(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
VNic11QueryFrequencyBandsSupported(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

NDIS_STATUS
VNic11QuerySupportedChannels(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   PhyId,
    _Out_ PULONG                  ChannelCount,
    _Out_opt_ PULONG                  ChannelList
    );

/*
 * Current channel/frequency
 */
ULONG
VNic11QueryCurrentChannel(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 SelectedPhy
    );

NDIS_STATUS
VNic11SetChannel(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   ChannelOrFrequency,
    _In_  ULONG                   PhyId,
    _In_  BOOLEAN                 SwitchPhy,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler    
    );

/*
 * Auth cipher
 */
NDIS_STATUS
VNic11SetAuthentication(
    _In_  PVNIC                   VNic,
    _In_  DOT11_AUTH_ALGORITHM    AlgoId
    );

VOID
VNic11SetCipher(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 IsUnicast,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId
    );

NDIS_STATUS
VNic11SetDefaultKey(
    _In_  PVNIC                   VNic,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  ULONG                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue    
    );

ULONG
VNic11QueryDefaultKeyId(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetDefaultKeyId(
    _In_  PVNIC                   VNic,
    _In_  ULONG                   KeyId
    );

VOID
VNic11DeleteNonPersistentKey(
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
VNic11SetKeyMappingKey(
    _In_  PVNIC                   VNic,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  ULONG                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue    
    );

BOOLEAN
VNic11IsKeyMappingKeyAvailable(
    _In_  PVNIC                   VNic,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr
    );

VOID
VNic11DeleteNonPersistentMappingKey(
    _In_  PVNIC                   VNic,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr
    );


/*
 * Adhoc & Infrastructure
 */
NDIS_STATUS
VNic11JoinBSS(
    _In_  PVNIC                   VNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  ULONG                   JoinFailureTimeout,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    );

NDIS_STATUS
VNic11StopBSS(
    _In_  PVNIC                   pVNic,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    );

NDIS_STATUS
VNic11StartBSS(
    _In_  PVNIC                   pVNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    );

NDIS_STATUS
VNic11SetBeaconEnabledFlag(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 BeaconEnabled
    );



/*
 * Scan
 */

NDIS_STATUS
VNic11StartScan(
    _In_  PVNIC                   VNic,
    _In_  PMP_SCAN_REQUEST        ScanRequest,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    );

VOID
VNic11CancelScan(
    _In_  PVNIC                   VNic
    );


/*
 * Control Notifications
 */

//
// NDIS_STATUS_DOT11_ASSOCIATION_COMPLETION - Informs the VNIC about association of a peer
// Caleld before the OS is informed
//
// NDIS_STATUS_DOT11_DISASSOCIATION - Informs the VNIC about loss of connectivity.
// Called after the OS has been informed
//
// NDIS_STATUS_DOT11_CONNECTION_START - Called after OS
//
// NDIS_STATUS_DOT11_CONNECTION_COMPLETION - Before OS
//
// On a Reset, the status should be reset
VOID
VNic11NotifyConnectionStatus(
    _In_  PVNIC                   VNic,
    _In_  BOOLEAN                 Connected,
    _In_  ULONG                   StatusType,
    _In_reads_bytes_opt_(StatusBufferSize)  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    );

VOID
VNic11IndicateStatus(
    _In_  PVNIC                   VNic,
    _In_  NDIS_STATUS             StatusCode,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    );


/*
 * Send
 */
BOOLEAN
VNic11CanTransmit(
    _In_  PVNIC                   VNic
    );


VOID
VNic11SendPackets(
    _In_ PVNIC                   VNic,
    _In_ PMP_TX_MSDU             PacketList,
    _In_ ULONG                   NumPkts,
    _In_ ULONG                   SendFlags
    );


VOID 
VNic11SendCompletePackets(
    _In_  PVNIC                   VNic,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   NumPkts,
    _In_  ULONG                   SendCompleteFlags
    );

/*
 * Receive
 */

VOID 
VNic11ReturnPackets(
    _In_  PVNIC                   VNic,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    );

VOID 
VNic11IndicateReceivePackets(
    _In_  PVNIC                   VNic,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    );
VOID
VNic11CtxSFromVNic(    
    _In_  PVNIC      VNic,
    _In_  ULONG      Flags
    );

#define VNIC_FLAG_GRANTED_EX_ACCESS             0x00000001
#define VNIC_FLAG_EX_ACCESS_HVL_TRIGGERED       0x00000002
#define VNIC_FLAG_HVL_ACTIVATED                 0x00000004

VOID
VNic11CtxSToVNic(    
    _In_  PVNIC      VNic,
    _In_  ULONG      Flags
    );

VOID
VNic11ProgramHw(    
    _In_  PVNIC      VNic,
    _In_  ULONG      Flags
    );

NDIS_STATUS 
VNic11ReqExAccess(
    PVNIC VNic,
    PORT11_GENERIC_CALLBACK_FUNC CallbkFn,
    PVOID FnCtx,
    BOOLEAN fPnPOperation
    );

NDIS_STATUS 
VNic11ReleaseExAccess(
    PVNIC VNic
    );

BOOLEAN
VNic11IsOkToCtxS(
    PVNIC VNic
    );

typedef struct _VNIC_SIGNATURE
{
    ULONG ulChannel;
    ULONG ulPhyId;
    UCHAR ucDefKeyMask;
} VNIC_SIGNATURE, *PVNIC_SIGNATURE;


VNIC_SIGNATURE
VNic11GetSignature(
    PVNIC pVNic
    );

VNIC_SIGNATURE
VNic11MergeSignatures(
    PVNIC_SIGNATURE pSig1,
    PVNIC_SIGNATURE pSig2
    );  

BOOLEAN
VNic11AreCompatibleSignatures(
    PVNIC_SIGNATURE pSig1,
    PVNIC_SIGNATURE pSig2
    );

typedef enum _NOTIFICATION_TYPE
{
    NotificationOpChannel,
    NotificationOpLinkState,
    NotificationOpRateChange
}NOTIFICATION_TYPE, *PNOTIFICATION_TYPE;

typedef struct _NOTIFICATION_DATA_HEADER
{
    PVNIC pSourceVNic;
    NOTIFICATION_TYPE Type;
    UCHAR Size;
}NOTIFICATION_DATA_HEADER, *PNOTIFICATION_DATA_HEADER;

typedef struct _OP_CHANNEL_NOTIFICATION
{
    NOTIFICATION_DATA_HEADER Header;
    ULONG ulChannel;
}OP_CHANNEL_NOTIFICATION, *POP_CHANNEL_NOTIFICATION;

typedef struct OP_LINK_STATE_NOTIFICATION
{
    NOTIFICATION_DATA_HEADER Header;
    BOOLEAN     MediaConnected;
}OP_LINK_STATE_NOTIFICATION, *POP_LINK_STATE_NOTIFICATION;

typedef struct OP_RATE_CHANGE_NOTIFICATION
{
    NOTIFICATION_DATA_HEADER Header;
    USHORT      OldRate;
    USHORT      NewRate;
    BOOLEAN     LowestRate;
}OP_RATE_CHANGE_NOTIFICATION, *POP_RATE_CHANGE_NOTIFICATION;



VOID
VNic11Notify(
    PVNIC               VNic,
    PVOID                   pvNotif
);

ULONG
VNic11QueryPreferredChannel(
    _In_  PVNIC               pVNic,
    _Out_ PBOOLEAN            pPreferredChannel
    );

VOID
VNic11UpdatePortType(
    _In_  PVNIC                   pVNic,
    _In_  MP_PORT_TYPE            PortType
    );

VOID
VNic11AcquireCtxSBarrier(
    PVNIC pVNic
    );

VOID
VNic11ReleaseCtxSBarrier(
    PVNIC pVNic
    );

NDIS_STATUS 
VNic11SetBeaconIE(
    _In_  PVNIC   pVNic,
    _In_  PVOID   pBeaconIEBlob,
    _In_  ULONG   uBeaconIEBlobSize
    );

NDIS_STATUS 
VNic11SetProbeResponseIE(
    _In_  PVNIC   pVNic,
    _In_  PVOID   pResponseIEBlob,
    _In_  ULONG   uResponseIEBlobSize
    );

