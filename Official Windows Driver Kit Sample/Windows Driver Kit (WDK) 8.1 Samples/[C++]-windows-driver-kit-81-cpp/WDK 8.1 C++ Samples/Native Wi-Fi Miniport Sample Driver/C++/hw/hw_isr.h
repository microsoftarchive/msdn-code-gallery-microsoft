/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_isr.h

Abstract:
    Contains defines for interrupt processing in the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

NDIS_STATUS
HwRegisterInterrupt(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

VOID
HwDeregisterInterrupt(
    _In_  PHW                     Hw
    );

MINIPORT_ISR HWInterrupt;

// The pre 6.20 Interrupt DPC that does not support receive side throttling
MINIPORT_INTERRUPT_DPC HWInterruptDPCNoRST;

// Receive side throttling compatible interrupt DPC
MINIPORT_INTERRUPT_DPC HWInterruptDPC;

MINIPORT_ENABLE_INTERRUPT HWEnableInterrupt;

MINIPORT_DISABLE_INTERRUPT HWDisableInterrupt;

VOID
HwEnableInterrupt(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    );

VOID
HwEnableInterruptWithSync(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    );

VOID
HwDisableInterrupt(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    );

_Function_class_(MINIPORT_SYNCHRONIZE_INTERRUPT)
VOID
HwDisableInterruptWithSync(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    );

VOID
HwProcessBeaconInterrupt(
    _In_  PHW                     Hw,
    _In_  ULONG                   Isr
    );

VOID
HwProcessATIMWindowInterrupt(
    _In_  PHW                     Hw
    );

VOID
HwSetBeaconIEWithSync(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pBeaconIEBlob,
    _In_  ULONG                   uBeaconIEBlobSize
    );

