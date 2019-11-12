/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_context.h

Abstract:
    Contains the defines for MAC context management for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once


#define HW_WAIT_FOR_BSS_JOIN(_MacContext)     (_MacContext->JoinWaitForBeacon = 1)
#define HW_STOP_WAITING_FOR_JOIN(_MacContext)      \
    (InterlockedCompareExchange((PLONG)&_MacContext->JoinWaitForBeacon, 0, 1) == 1)

/** 
 * Duration of the per-context periodic timer (this is in milliseconds)
 */
#define HW_CONTEXT_PERIODIC_TIMER_DURATION      2000

NDIS_STATUS
HwInitializeMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PVNIC                   VNic,
    _In_  ULONG                   ContextId
    );

VOID
HwTerminateMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    );

NDIS_STATUS
HwResetMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    );

NDIS_STATUS
HwSetMACContextOnNic(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    );

NDIS_STATUS
HwClearMACContextFromNic(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    );

VOID
HwDot11ResetStep1(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    );

NDIS_STATUS
HwDot11ResetStep2(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    );

NDIS_STATUS
HwUpdateBSSDescription(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  BOOLEAN                 BSSStart
    );

NDIS_STATUS 
HwJoinChannelSwitchComplete(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_opt_  PVOID                   Data
    );

NDIS_TIMER_FUNCTION HwJoinTimeoutTimer;

VOID
HwJoinBSSComplete(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_opt_  PHW_RX_MPDU             Mpdu,
    _In_  NDIS_STATUS             Status
    );

VOID
HwScanComplete(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  NDIS_STATUS             Status
    );

NDIS_TIMER_FUNCTION HwMACContextPeriodicTimer;

