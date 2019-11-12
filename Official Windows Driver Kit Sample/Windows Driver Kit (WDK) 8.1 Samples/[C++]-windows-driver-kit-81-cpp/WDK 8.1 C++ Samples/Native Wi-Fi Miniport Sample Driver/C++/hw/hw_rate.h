/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_rate.h

Abstract:
    Contains defines used for the rate adaptation logic in the H/W
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once


USHORT
HwSelectTXDataRate(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_RATE_SET         RemoteRateSet,
    _In_  ULONG                   LinkQuality
    );


USHORT
HwDetermineStartTxRate(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_PEER_NODE           PeerNode,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PDOT11_MAC_HEADER       FragmentHeader
    );

VOID
HwUpdatePeerTxStatistics(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  HAL_TX_DESC_STATUS      DescStatus
    );

VOID 
HwUpdateTxDataRate(
    _In_  PHW_MAC_CONTEXT         MacContext
    );

VOID
HwFillTxRateTable(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  USHORT                  InitialTxRate
    );

