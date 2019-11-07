/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_utils.h

Abstract:
    Contains defines for helper routines for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

NDIS_STATUS
HwGetAdapterStatus(
    _In_  PHW                     Hw
    );

PHW_PEER_NODE
HwFindPeerNode(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  BOOLEAN                 Allocate
    );

VOID
HwFreePeerNode(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PHW_PEER_NODE           PeerNode
    );

NDIS_STATUS
HwFindKeyMappingKeyIndex(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  BOOLEAN                 Allocate,
    _Out_ PULONG                  ReturnIndex
    );

ULONG64
HwDataRateToLinkSpeed(
    UCHAR  rate
    );

UCHAR
HwLinkSpeedToDataRate(
    ULONG64 linkSpeed
    );


ULONG
HwChannelToFrequency(
    _In_  UCHAR               Channel
    );

NDIS_STATUS
HwTranslateChannelFreqToLogicalID(
    _In_reads_(NumChannels)  PULONG  ChannelFreqList,
    _In_  ULONG                   NumChannels,
    _Out_writes_(NumChannels) PULONG  LogicalChannelList
    );

BOOLEAN
HwSelectLowestMatchingRate(
    PUCHAR          remoteRatesIE,
    UCHAR           remoteRatesIELength,
    PDOT11_RATE_SET localRateSet,
    PUSHORT         SelectedRate
    );

VOID
HwIndicateLinkSpeed(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  USHORT                  DataRate
    );

VOID
HwIndicateMICFailure(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_DATA_SHORT_HEADER Header,
    _In_  ULONG                   KeyId,
    _In_  BOOLEAN                 IsDefaultKey
    );

VOID
HwIndicatePhyPowerState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

