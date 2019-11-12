/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_pnp.h

Abstract:
    Contains defines relevant for MP layer initialize/halt
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

MINIPORT_INITIALIZE MPInitialize;

MINIPORT_RESTART MPRestart;

MINIPORT_PAUSE MPPause;

MINIPORT_SHUTDOWN MPAdapterShutdown;

MINIPORT_DEVICE_PNP_EVENT_NOTIFY MPDevicePnPEvent;

MINIPORT_HALT MPHalt;

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
MpAllocateAdapter(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_nullonfailure_ PADAPTER*     Adapter
    );

VOID
MpFreeAdapter(
    _In_ __drv_freesMem(Mem) PADAPTER  Adapter
    );


NDIS_STATUS
MpReadRegistryConfiguration(
    _In_  PADAPTER                Adapter
    );

NDIS_STATUS
MpSetRegistrationAttributes(
    _In_  PADAPTER                Adapter
    );
    
NDIS_STATUS
MpSetMiniportAttributes(
    _In_  PADAPTER                Adapter
    );

NDIS_STATUS
MpInitializeAdapter(
    _In_  PADAPTER                Adapter,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    );

VOID
MpTerminateAdapter(
    _In_  PADAPTER                Adapter
    );

NDIS_STATUS
MpStart(
    _In_  PADAPTER                Adapter
    );

VOID
MpStop(
    _In_  PADAPTER                Adapter,
    _In_  NDIS_HALT_ACTION        HaltAction
    );

BOOLEAN
MpRemoveAdapter(
    _In_  PADAPTER                Adapter
    );

