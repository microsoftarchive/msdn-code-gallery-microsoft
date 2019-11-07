/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_intf.h

Abstract:
    Contains interfaces into the HW layer
    
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
typedef struct _HW_MAC_CONTEXT  HW_MAC_CONTEXT, *PHW_MAC_CONTEXT;


// First call into the HW
NDIS_STATUS
Hw11Allocate(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_maybenull_ PHW*          Hw,
    _In_  PADAPTER                Adapter
    );

VOID
Hw11Free(
    _In_  PHW                     Hw
    );

// This is called after the Hw11FindNic & Hw11DiscoverNicResources calls
NDIS_STATUS
Hw11Initialize(
    _In_  PHW                     Hw,
    _In_  PHVL                    Hvl,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

VOID
Hw11Terminate(
    _In_  PHW                     Hw
    );

VOID
Hw11Shutdown(
    _In_  PHW                     Hw,
    _In_  NDIS_SHUTDOWN_ACTION    ShutdownAction
    );

VOID
Hw11Halt(
    _In_  PHW                     Hw
    );
    
NDIS_STATUS
Hw11Pause(
    _In_  PHW         Hw
    );

VOID
Hw11Restart(
    _In_  PHW         Hw
    );

VOID
Hw11DevicePnPEvent(
    _In_  PHW                     Hw,
    _In_  PNET_DEVICE_PNP_EVENT   NetDevicePnPEvent
    );

BOOLEAN
Hw11CheckForHang(
    _In_  PHW                     Hw
    );

VOID
Hw11NdisResetStep1(
    _In_  PHW                     Hw
    );

VOID
Hw11NdisResetStep2(
    _In_  PHW                     Hw
    );

NDIS_STATUS
Hw11NdisResetStep3(
    _In_  PHW                     Hw,
    _Out_ PBOOLEAN                AddressingReset
    );


NDIS_STATUS
Hw11Dot11Reset(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    );


NDIS_STATUS
Hw11AllocateMACContext(
    _In_  PHW                     Hw,
    _Outptr_result_maybenull_ PHW_MAC_CONTEXT* MacContext,
    _In_  PVNIC                   VNic,
    _In_  NDIS_PORT_NUMBER        PortNumber    
    );

VOID
Hw11FreeMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    );

/* 
 * Called to inform the HW that this MAC context should be used (is active).
 * 
 */
NDIS_STATUS
Hw11EnableMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    );
    
/* 
 * Called to inform the HW that this MAC context should not be used (is NOT active) 
 * If the HW is using this context, it would block the call
 */
NDIS_STATUS
Hw11DisableMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    );

NDIS_STATUS
Hw11PauseMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext
    );

VOID
Hw11RestartMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext
    );

NDIS_STATUS
Hw11InitializeSendEngine(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

VOID
Hw11TerminateSendEngine(
    _In_  PHW                     Hw
    );


NDIS_STATUS
Hw11InitializeReceiveEngine(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

VOID
Hw11TerminateReceiveEngine(
    _In_  PHW                     Hw
    );

// 2nd call into the HW
VOID
Hw11ReadRegistryConfiguration(
    _In_        PHW             Hw,
    _In_opt_    NDIS_HANDLE     ConfigurationHandle
    );


// 3rd call into the HW
/**
 * This looks through the PCI configuration space and tries to find the hardware
 * that this driver should control
 */ 
NDIS_STATUS
Hw11FindNic(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

// 4th call into the HW
NDIS_STATUS
Hw11DiscoverNicResources(
    _In_  PHW                     Hw,
    _In_  PNDIS_RESOURCE_LIST     ResList,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

// 5th call into the HW
NDIS_STATUS
Hw11ReadNicInformation(
    _In_  PHW                     Hw
    );

NDIS_STATUS
Hw11Start(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

// First disable interrupts and then deregister interrupts
VOID
Hw11Stop(
    _In_  PHW                     Hw,
    _In_  NDIS_HALT_ACTION        HaltAction
    );


NDIS_STATUS
Hw11SelfTest(
    _In_  PHW                     Hw
    );

// Maps to interrupts for PCI
VOID
Hw11DisableHardwareNotifications(
    _In_  PHW                     Hw
    );

// Maps to interrupts for PCI
VOID
Hw11EnableHardwareNotifications(
    _In_  PHW                     Hw
    );


BOOLEAN 
Hw11WEP104Implemented(
    _In_  PHW                     Hw
    );

BOOLEAN 
Hw11WEP40Implemented(
    _In_  PHW                     Hw
    );

BOOLEAN 
Hw11TKIPImplemented(
    _In_  PHW                     Hw
    );

BOOLEAN 
Hw11CCMPImplemented(
    _In_  PHW                     Hw,
    _In_  DOT11_BSS_TYPE          bssType
    );

ULONG
Hw11DefaultKeyTableSize(
    _In_  PHW                     Hw
    );

ULONG
Hw11KeyMappingKeyTableSize(
    _In_  PHW                     Hw
    );

ULONG
Hw11PerStaKeyTableSize(
    _In_  PHW                     Hw
    );

NDIS_STATUS
Hw11Fill80211Attributes(
    _In_  PHW                     Hw,
    _Out_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES attr
    );

VOID
Hw11Cleanup80211Attributes(
    _In_  PHW                     Hw,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES attr
    );

PDOT11_MAC_ADDRESS
Hw11QueryHardwareAddress(
    _In_  PHW                     Hw
    );

PDOT11_MAC_ADDRESS
Hw11QueryCurrentAddress(
    _In_  PHW                     Hw
    );


PDOT11_MAC_ADDRESS
Hw11QueryMACAddress(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

VOID
Hw11QueryPnPCapabilities(
    _In_  PHW                     Hw,
    _Out_ PNDIS_PNP_CAPABILITIES  NdisPnPCapabilities
    );

VOID
Hw11QueryPMCapabilities(
    _In_  PHW                     Hw,
    _Out_ PNDIS_PM_CAPABILITIES   NdisPmCapabilities
    );

PDOT11_MAC_ADDRESS
Hw11QueryCurrentBSSID(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

    
ULONG
Hw11QueryBeaconPeriod(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

DOT11_PHY_TYPE
Hw11QueryCurrentPhyType(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

BOOLEAN
Hw11QueryShortSlotTimeOptionImplemented(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryShortSlotTimeOptionEnabled(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryDsssOfdmOptionImplemented(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryDsssOfdmOptionEnabled(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryShortPreambleOptionImplemented(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryPbccOptionImplemented(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryChannelAgilityPresent(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );


BOOLEAN
Hw11QueryChannelAgilityEnabled(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryCFPollable(
    _In_  PHW                     Hw
    );

VOID
Hw11QueryBasicRateSet(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PDOT11_RATE_SET         Dot11RateSet,
    _In_  BOOLEAN                 SelectedPhy
    );

VOID
Hw11QueryOperationalRateSet(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PDOT11_RATE_SET         Dot11RateSet,
    _In_  BOOLEAN                 SelectedPhy
    );

NDIS_STATUS
Hw11QuerySupportedPHYTypes(
    _In_  PHW                     Hw,
    _In_  ULONG                   NumMaxEntries,
    _Out_ PDOT11_SUPPORTED_PHY_TYPES Dot11SupportedPhyTypes
    );

NDIS_STATUS
Hw11QuerySupportedChannels(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   PhyId,
    _Out_ PULONG                  ChannelCount,
    _Out_opt_ PULONG                  ChannelList
    );

ULONG
Hw11QueryCurrentChannel(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
Hw11QueryATIMWindow(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
Hw11SetATIMWindow(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   Value,
    _In_  BOOLEAN                 fProgramHardware
    );

VOID
Hw11SetCipher(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 IsUnicast,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  BOOLEAN                 fProgramHardware
    );

UCHAR
Hw11SelectTXDataRate(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_RATE_SET         PeerRateSet,
    _In_  ULONG                   LinkQuality,
    _In_  BOOLEAN                 fProgramHardware
    );

VOID
Hw11DeleteNonPersistentKey(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 fProgramHardware
    );

VOID
Hw11DeleteNonPersistentMappingKey(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  BOOLEAN                 fProgramHardware
    );

ULONG
Hw11QueryOperatingPhyId(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

BOOLEAN
Hw11IsConnected(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
Hw11SetOperatingPhyId(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   PhyId,
    _In_  BOOLEAN                 fProgramHardware
    );

USHORT
Hw11QueryRSNCapabilityField(
    _In_  PHW                     Hw
    );

/** Control */

DOT11_PHY_TYPE
Hw11DeterminePHYType(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  DOT11_CAPABILITY        Capability,
    _In_  UCHAR                   Channel
    );


NDIS_STATUS
Hw11ValidateOperationalRates(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   PhyId,
    _In_reads_bytes_(RateSetLength)  PUCHAR                  DataRateSet,
    _In_  ULONG                   RateSetLength
    );

NDIS_STATUS
Hw11SetPowerMgmtMode(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_POWER_MGMT_MODE  PMMode,
    _In_  BOOLEAN                 fProgramHardware
    );

VOID
Hw11SetCTSToSelfOption(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 Enable,
    _In_  BOOLEAN                 fProgramHardware
    );

BOOLEAN
Hw11IsKeyMappingKeyAvailable(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr
    );

VOID
Hw11StopBSS(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
Hw11StartBSS(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription
    );

NDIS_STATUS
Hw11JoinBSS(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  ULONG                   JoinFailureTimeout,
    _In_  VNIC11_GENERIC_CALLBACK_FUNC    CompletionHandler,
    _In_  PVOID                   JoinContext
    );

VOID
Hw11NotifyEventConnectionState(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 Connected,
    _In_  ULONG                   StatusType,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize,
    _In_  BOOLEAN                 fProgramHardware
    );

// Scan happen on the HW & not the MAC_CONTEXT
NDIS_STATUS
Hw11StartScan(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_SCAN_REQUEST        ScanRequest,
    _In_  VNIC11_GENERIC_CALLBACK_FUNC    CompletionHandler,
    _In_  PVOID                   ScanContext
    );

// Cancel scan happens on the HW & not the MAC context
VOID
Hw11CancelScan(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

/** Send */

BOOLEAN
Hw11CanTransmit(
    _In_  PHW_MAC_CONTEXT         HwMac
    );


VOID
Hw11SendPackets(
    _In_ PHW_MAC_CONTEXT         HwMac,
    _In_ PMP_TX_MSDU             PacketList,
    _In_ ULONG                   SendFlags
    );


/** Receive */

UCHAR
Hw11GetFragmentChannel(
    _In_  PMP_RX_MPDU             Fragment
    );

PVOID
Hw11GetFragmentDataStart(
    _In_  PMP_RX_MPDU             Fragment
    );

ULONG
Hw11GetFragmentDataLength(
    _In_  PMP_RX_MPDU             Fragment
    );

VOID 
Hw11ReturnPackets(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Hw11QueryDot11Statistics(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesWritten)
          PDOT11_STATISTICS       Dot11Stats,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_when_invalid_ndis_length_
          PULONG                  BytesNeeded
    );

NDIS_STATUS
Hw11SetPacketFilter(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   PacketFilter,
    _In_  BOOLEAN                 fProgramHardware
    );

BOOLEAN
Hw11QueryNicPowerState(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

NDIS_STATUS
Hw11SetChannel(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   ChannelOrFrequency,
    _In_  ULONG                   PhyId,
    _In_  BOOLEAN                 SwitchPhy,
    _In_  VNIC11_GENERIC_CALLBACK_FUNC    CompletionHandler,
    _In_  PVOID                   ChannelSwitchContext
    );

NDIS_STATUS
Hw11SetDefaultKey(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  ULONG                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetKeyMappingKey(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  ULONG                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetOperationMode(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_CURRENT_OPERATION_MODE Dot11CurrentOperationMode,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetCurrentBSSType(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  DOT11_BSS_TYPE          Type,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetDesiredPhyIdList(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PULONG                  PhyIDList,
    _In_  ULONG                   PhyIDCount,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetAuthentication(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  DOT11_AUTH_ALGORITHM    AlgoId,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11QueryRecvSensitivityList(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   MaxEntries,
    _Inout_ PDOT11_RECV_SENSITIVITY_LIST Dot11RecvSensitivityList
    );


ULONG
Hw11QueryDefaultKeyId(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

ULONG
Hw11QuerySelectedPhyId(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

BOOLEAN
Hw11QueryHardwarePhyState(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
Hw11SetDefaultKeyId(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   KeyId,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetSelectedPhyId(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   PhyId,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetSafeModeOption(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SafeModeEnabled,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11QueryInterruptModerationSettings(
    _In_  PHW                     Hw,
    _Out_ PNDIS_INTERRUPT_MODERATION_PARAMETERS   IntModParams
    );

NDIS_STATUS
Hw11QueryLinkParameters(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PNDIS_LINK_PARAMETERS   LinkParams
    );

NDIS_STATUS
Hw11QueryDataRateMappingTable(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PDOT11_DATA_RATE_MAPPING_TABLE  DataRateMappingTable,
    _In_  ULONG                   TotalLength
    );

NDIS_STATUS
Hw11QuerySupportedPowerLevels(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PDOT11_SUPPORTED_POWER_LEVELS   Dot11SupportedPowerLevels
    );

NDIS_STATUS
Hw11QuerySupportedRXAntenna(
    _In_  PHW                     Hw,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_SUPPORTED_ANTENNA_LIST   Dot11SupportedAntennaList    
    );

NDIS_STATUS
Hw11QuerySupportedTXAntenna(
    _In_  PHW                     Hw,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_SUPPORTED_ANTENNA_LIST   Dot11SupportedAntennaList
    );

NDIS_STATUS
Hw11QueryDiversitySelectionRX(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_DIVERSITY_SELECTION_RX_LIST Dot11DiversitySelectionRXList
    );

NDIS_STATUS
Hw11QueryRegDomainsSupportValue(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_REG_DOMAINS_SUPPORT_VALUE    Dot11RegDomainsSupportValue
    );

NDIS_STATUS
Hw11SetInterruptModerationSettings(
    _In_  PHW                     Hw,
    _In_  PNDIS_INTERRUPT_MODERATION_PARAMETERS   IntModParams
    );

NDIS_STATUS
Hw11SetLinkParameters(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PNDIS_LINK_PARAMETERS   LinkParams,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetMulticastList(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   AddressBuffer,
    _In_  ULONG                   AddressBufferLength,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetOperationalRateSet(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_RATE_SET         Dot11RateSet,
    _In_  BOOLEAN                 SelectedPhy,
    _In_  BOOLEAN                 fProgramHardware 
    );

ULONG
Hw11QueryLookahead(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

ULONG
Hw11QueryHardwareStatus(
    _In_  PHW                     Hw
    );

ULONG
Hw11QueryReceiveBufferSpace(
    _In_  PHW                     Hw
    );

ULONG
Hw11QueryTransmitBufferSpace(
    _In_  PHW                     Hw
    );

NDIS_STATUS
Hw11QueryVendorDescription(
    _In_                                    PHW     Hw,
    _Out_writes_bytes_(InformationBufferLength)  PVOID   InformationBuffer,
    _In_                                    ULONG   InformationBufferLength,
    _Out_                                   PULONG  BytesWritten,
    _Out_                                   PULONG  BytesNeeded
    );

ULONG
Hw11QueryVendorId(
    _In_  PHW                     Hw
     );

NDIS_STATUS
Hw11QueryCurrentOptionalCapability(
    _In_  PHW                     Hw,
    _In_ PDOT11_CURRENT_OPTIONAL_CAPABILITY   Dot11CurrentOptionalCapability    
    );

ULONG
Hw11QueryMaxMPDULength(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
Hw11QueryMulticastList(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PVOID                   AddressBuffer,
    _In_  ULONG                   AddressBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    );

NDIS_STATUS
Hw11QueryOperationModeCapability(
    _In_  PHW                     Hw,
    _Out_ PDOT11_OPERATION_MODE_CAPABILITY    Dot11OpModeCapability
    );

NDIS_STATUS
Hw11QueryOptionalCapability(
    _In_  PHW                     Hw,
    _Out_ PDOT11_OPTIONAL_CAPABILITY  Dot11OptionalCapability
    );

ULONG
Hw11QueryRFUsage(
    _In_  PHW                     Hw
    );

NDIS_STATUS
Hw11QuerySupportedDataRatesValue(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PDOT11_SUPPORTED_DATA_RATES_VALUE_V2    Dot11SupportedDataRatesValue,
    _In_  BOOLEAN                 SelectedPhy 
    );

ULONG
Hw11QueryCCAModeSupported(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

NDIS_STATUS
Hw11QueryCountryString(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _Out_ PDOT11_COUNTRY_OR_REGION_STRING Dot11CountryString
    );

ULONG
Hw11QueryCurrentCCAMode(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
Hw11QueryCurrentFrequency(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
Hw11QueryCurrentRegDomain(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

ULONG
Hw11QueryCurrentTXPowerLevel(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

DOT11_DIVERSITY_SUPPORT
Hw11QueryDiversitySupport(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
Hw11QueryEDThreshold(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryErpPbccOptionEnabled(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

BOOLEAN
Hw11QueryErpPbccOptionImplemented(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
Hw11QueryFragmentationThreshold(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

ULONG
Hw11QueryFrequencyBandsSupported(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
Hw11QueryLongRetryLimit(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

ULONG
Hw11QueryMaxReceiveLifetime(
    _In_  PHW                     Hw
    );

ULONG
Hw11QueryMaxTransmitMSDULifetime(
    _In_  PHW                     Hw
    );

BOOLEAN
Hw11QueryMultiDomainCapabilityEnabled(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

BOOLEAN
Hw11QueryMultiDomainCapabilityImplemented(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

ULONG
Hw11QueryRTSThreshold(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

ULONG
Hw11QueryShortRetryLimit(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

DOT11_TEMP_TYPE
Hw11QueryTempType(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 SelectedPhy
    );

ULONG
Hw11QueryPacketFilter(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

DOT11_CIPHER_ALGORITHM
Hw11QueryUnicastCipher(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

DOT11_CIPHER_ALGORITHM
Hw11QueryMulticastCipher(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

DOT11_AUTH_ALGORITHM
Hw11QueryAuthentication(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

UCHAR
Hw11QueryDefaultKeyMask(
    _In_  PHW_MAC_CONTEXT         HwMac
    );
    
NDIS_STATUS
Hw11SetLookahead(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   Lookahead,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetNicPowerState(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 PowerState,
    _In_  BOOLEAN                 SelectedPhy,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetBeaconPeriod(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   BeaconPeriod,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetFragmentationThreshold(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   FragmentationThreshold,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11SetMultiDomainCapabilityEnabled(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 MultiDomainCapabilityEnabled,
    _In_  BOOLEAN                 fProgramHardware
    );
NDIS_STATUS
Hw11SetRTSThreshold(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   RTSThreshold,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
Hw11CanTransitionPower(
    _In_  PHW                     Hw,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    );

NDIS_STATUS 
Hw11SetPower(
    _In_  PHW                     Hw,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    );

NDIS_STATUS
Hw11CtxSStart(
    _In_  PHW                     Hw
    );

VOID
Hw11CtxSComplete(
    _In_  PHW                     Hw
    );


NDIS_STATUS
Hw11SendNullPkt(
    _In_ PHW_MAC_CONTEXT          MacContext,
    _In_ BOOLEAN                  PowerBitSet
    );

NDIS_STATUS
Hw11SetBeaconEnabledFlag(    
    _In_  PHW_MAC_CONTEXT         HwMac,
    BOOLEAN                     BeaconEnabled,
    BOOLEAN                     fProgramHardware
    );

NDIS_STATUS 
Hw11SetBeaconIE(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pBeaconIEBlob,
    _In_  ULONG                   uBeaconIEBlobSize,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS 
Hw11SetProbeResponseIE(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pResponseIEBlob,
    _In_  ULONG                   uResponseIEBlobSize,
    _In_  BOOLEAN                 fProgramHardware
    );

DOT11_BSS_TYPE
Hw11QueryCurrentBSSType(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

BOOLEAN
Hw11ArePktsPending(
    _In_  PHW                     Hw
    );

