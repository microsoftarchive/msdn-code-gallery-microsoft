/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    helper_port_intf.h

Abstract:
    Defines the interfaces for the helper port
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

typedef struct _MP_HELPER_PORT  MP_HELPER_PORT, *PMP_HELPER_PORT;
typedef struct _MP_BSS_LIST     MP_BSS_LIST, *PMP_BSS_LIST;
typedef struct _MP_RW_LOCK_STATE    MP_RW_LOCK_STATE, *PMP_RW_LOCK_STATE;

NDIS_STATUS
HelperPortAllocatePort(
	_In_  NDIS_HANDLE             MiniportAdapterHandle,
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    );

VOID
HelperPortFreePort(
    _In_  PMP_PORT                Port
    );


NDIS_STATUS
HelperPortInitializePort(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   RegistryInformation
    );

VOID
HelperPortTerminatePort(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
HelperPortLoadRegistryInformation(
    _In_        NDIS_HANDLE             MiniportAdapterHandle,
    _In_opt_    NDIS_HANDLE             ConfigurationHandle,
    _Out_       PVOID*                  RegistryInformation
    );

VOID
HelperPortFreeRegistryInformation(
    _In_opt_  PVOID              RegistryInformation
    );

VOID
HelperPortHandleMacCleanup(
    _In_ PMP_PORT                 Port
    );

NDIS_STATUS 
HelperPortHandleMiniportPause(
    _In_  PMP_PORT                Port,
    _In_opt_  PNDIS_MINIPORT_PAUSE_PARAMETERS     MiniportPauseParameters
    );

NDIS_STATUS 
HelperPortHandleMiniportRestart(
    _In_  PMP_PORT                Port,
    _In_opt_  PNDIS_MINIPORT_RESTART_PARAMETERS   MiniportRestartParameters
    );

NDIS_STATUS 
HelperPortHandlePortPause(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                PortToPause
    );

NDIS_STATUS 
HelperPortHandlePortRestart(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                PortToRestart
    );

VOID
HelperPortHandlePortTerminate(
    _In_ PMP_PORT                 Port,
    _In_ PMP_PORT                 PortToTerminate
    );

NDIS_STATUS
HelperPortHandleMiniportReset(
    _In_  PMP_PORT                Port,
    _Inout_ PBOOLEAN              AddressingReset
    );

NDIS_STATUS
HelperPortHandleOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest,
    _Out_ BOOLEAN                 *pfOidCompleted
    );

NDIS_STATUS 
HelperPortReceiveEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    );

NDIS_STATUS
HelperPortHandleScan(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                RequestingPort,
    _In_  PMP_SCAN_REQUEST        ScanRequest,
    _In_  PORT11_GENERIC_CALLBACK_FUNC    CompletionHandler
    );

VOID
HelperPortCancelScan(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                RequestingPort
    );

NDIS_STATUS
HelperPortFlushBSSList(
    _In_  PMP_PORT                Port
    );

PMP_BSS_LIST
HelperPortQueryAndRefBSSList(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                RequestingPort,
    _Out_  PMP_RW_LOCK_STATE      LockState
    );

VOID
HelperPortReleaseBSSListRef(
    _In_  PMP_PORT                Port,
    _In_  PMP_BSS_LIST            BSSList,
    _Out_  PMP_RW_LOCK_STATE      LockState
    );

VOID
HelperPortNotify(
    _In_  PMP_PORT      Port,
    PVOID               Notif
    );

