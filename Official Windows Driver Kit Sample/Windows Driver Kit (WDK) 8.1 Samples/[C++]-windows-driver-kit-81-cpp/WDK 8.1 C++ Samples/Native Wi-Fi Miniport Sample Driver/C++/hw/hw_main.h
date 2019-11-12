/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_main.h

Abstract:
    Contains defines for initialization/PNP routines in the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once


VOID
HwResetSoftwareMacState(
    _In_  PHW                     Hw
    );

VOID
HwResetSoftwarePhyState(
    _In_  PHW                     Hw
    );

NDIS_STATUS
HwSetNicState(
    _In_  PHW                     Hw
    );

NDIS_STATUS
HwClearNicState(
    _In_  PHW                     Hw
    );

_IRQL_requires_(PASSIVE_LEVEL)
NDIS_STATUS
HwResetHAL(
    _In_  PHW                     Hw,
    _In_  PHW_HAL_RESET_PARAMETERS ResetParams,
    _In_  BOOLEAN                 DispatchLevel
    );

/**
 * This function is called by NDIS when a NetBuffer, that had previously been
 * submitted to NDIS, is successfully scatter gathered.
 * 
 * \sa NdisMAllocateNetBufferSGList from the DDK
 */
MINIPORT_PROCESS_SG_LIST HWProcessSGList;

MINIPORT_ALLOCATE_SHARED_MEM_COMPLETE HWAllocateComplete;

/**
 * Helper routine to initialize Scatter Gather DMA engine during
 * Initialize.
 */
NDIS_STATUS
HwInitializeScatterGatherDma(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );


