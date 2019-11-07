/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_mac.h

Abstract:
    Contains defines used for MAC functionality 
    in the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once


NDIS_STATUS
HwSetATIMWindow(
    _In_  PHW                     Hw,
    _In_  ULONG                   Value
    );

VOID
HwUpdateUnicastCipher(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
HwSetPowerMgmtMode(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_POWER_MGMT_MODE  PMMode
    );

NDIS_STATUS
HwUpdatePacketFilter(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
HwUpdateMulticastAddressList(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );

/** Add the key to the NIC */
NDIS_STATUS
HwAddKeyEntry(
    _In_  PHW                     Hw,
    _In_  PHW_KEY_ENTRY           KeyEntry
    );

VOID
HwUpdateKeyEntry(
    _In_  PHW                     Hw,
    _In_  PHW_KEY_ENTRY           KeyEntry
    );

VOID
HwRemoveKeyEntry(
    _In_  PHW                     Hw,
    _In_  PHW_KEY_ENTRY           KeyEntry
    );

VOID
HwFreeKey(
    _In_  PHW_KEY_ENTRY           KeyEntry
    );

UCHAR
HwQueryDefaultKeyMask(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

VOID
HwSetDefaultKeyId(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   KeyId
    );

VOID
HwDeleteKeyFromContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  UCHAR                   KeyIndex,
    _In_  BOOLEAN                 fProgramHardware
    );

VOID
HwDeleteAllKeysFromContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 fProgramHardware
    );

VOID
HwDeleteAllKeysFromHw(
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
HwSetKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PHW_KEY_ENTRY           KeyEntry,
    _In_  UCHAR                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
HwSetDefaultKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  UCHAR                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS 
HwSetSoftwareDefaultKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  UCHAR                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue
    );

NDIS_STATUS 
HwSetKeyMappingKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  DOT11_DIRECTION         Direction,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    );

NDIS_STATUS
HwUpdateOperationMode(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );


NDIS_STATUS
HwUpdateBSSType(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
HwUpdateBSSID(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );


NDIS_STATUS
HwSetBeaconPeriod(
    _In_  PHW                     Hw,
    _In_  ULONG                   BeaconPeriod
    );

NDIS_STATUS
HwUpdateLinkState(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
HwUpdateAssociateId(
    _In_  PHW                     Hw,
    _In_  USHORT                  AssociateId
    );

VOID
HwSetBeaconIE(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pBeaconIEBlob,
    _In_  ULONG                   uBeaconIEBlobSize
    );

VOID
HwPopulateBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         BeaconMac,
    _In_  PHW_TX_MSDU             BeaconMsdu
    );

NDIS_STATUS
HwSetupBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         BeaconMac
    );

VOID
HwEnableBSSBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );

VOID
HwDisableBSSBeacon(
    _In_  PHW                     Hw
    );

NDIS_STATUS
HwStartBSS(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 BeaconEnabled
    );


VOID
HwStopBSS(
    _In_  PHW                     Hw
    );

VOID
HwPauseBSS(
    _In_  PHW                     Hw
    );

VOID
HwResumeBSS(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 BeaconEnabled
    );


VOID
HwPrepareToJoinBSS(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    );

NDIS_STATUS
HwProcessProbeRequest(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_RX_MPDU             Mpdu
    );

NDIS_STATUS
HwSendProbeRequest(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_MAC_ADDRESS      BSSID,
    _In_opt_  PDOT11_SSID             SSID,
    _In_reads_bytes_(ScanAppendIEByteArrayLength)  PUCHAR                  ScanAppendIEByteArray,
    _In_  USHORT                  ScanAppendIEByteArrayLength
    );

NDIS_STATUS
HwStartScan(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_SCAN_REQUEST        ScanRequest
    );

VOID
HwCancelScan(
    _In_  PHW                     Hw
    );

NDIS_STATUS 
HwScanChannelSwitchComplete(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PVOID                   Data
    );

NDIS_STATUS 
HwScanChannelRestoreComplete(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PVOID                   Data
    );

NDIS_TIMER_FUNCTION HwScanTimer;

