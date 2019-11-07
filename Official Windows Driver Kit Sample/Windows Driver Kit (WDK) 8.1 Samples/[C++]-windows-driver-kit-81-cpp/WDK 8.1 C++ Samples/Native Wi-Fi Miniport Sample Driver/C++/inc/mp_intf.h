/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_intf.h

Abstract:
    Contains interfaces into the miniport layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once


// Maximum number of ports that can be created on this adapter (excludes the helper port)
#define MP_MAX_NUMBER_OF_PORT   2



//
// Forward declaration
//
typedef struct _ADAPTER         ADAPTER, *PADAPTER;
typedef struct _HVL             HVL, *PHVL;
typedef struct _HW              HW, *PHW;
typedef struct _MP_PORT         MP_PORT, *PMP_PORT;
typedef struct _VNIC            VNIC, *PVNIC;
typedef struct _MP_BSS_LIST     MP_BSS_LIST, *PMP_BSS_LIST;
typedef struct _MP_RW_LOCK_STATE    MP_RW_LOCK_STATE, *PMP_RW_LOCK_STATE;


NDIS_STATUS
Mp11CtxSStart(   
    _In_  PADAPTER                Adapter
    );

VOID
Mp11CtxSComplete(   
    _In_  PADAPTER                Adapter
    );

NDIS_STATUS 
Mp11PausePort(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    );

NDIS_STATUS 
Mp11RestartPort(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Mp11Scan(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port,
    _In_  PMP_SCAN_REQUEST        ScanRequest,
    _In_  PORT11_GENERIC_CALLBACK_FUNC    CompletionHandler
    );

VOID
Mp11CancelScan(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    );

VOID
Mp11FlushBSSList(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    );

/** Returns a read-only reference to the BSS list. Individual entries in the list
 * can be modified */
PMP_BSS_LIST
Mp11QueryAndRefBSSList(
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port,
    _Out_ PMP_RW_LOCK_STATE       LockState
    );

/** Release read-only access to the BSS list */
VOID
Mp11ReleaseBSSListRef(
    _In_  PADAPTER                Adapter,
    _In_  PMP_BSS_LIST            BSSList,
    _In_  PMP_RW_LOCK_STATE       LockState
    );

/** Complete an OID request */
VOID
Mp11CompleteOidRequest(
    _In_  PADAPTER                Adapter,
    _In_opt_  PMP_PORT                CompletingPort,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest,
    _In_  NDIS_STATUS             CompletionStatus
    );


