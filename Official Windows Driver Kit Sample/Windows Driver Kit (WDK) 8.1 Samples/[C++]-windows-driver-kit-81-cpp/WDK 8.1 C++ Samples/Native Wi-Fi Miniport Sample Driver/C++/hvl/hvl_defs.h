/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hvl_defs.h

Abstract:
    Contains HVL specific defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

#define NDIS_FAILURE(ndisStatus)       \
    ( (NDIS_STATUS_SUCCESS != ndisStatus) && (NDIS_STATUS_PENDING != ndisStatus))
    
#define HVL_TIMED_CTXS_BLOCKED          0x00000001
#define HVL_CTXS_IN_PROGRESS            0x00000002

#define HVL_NUM_CONTEXTS                (MP_MAX_NUMBER_OF_PORT+1)   // +1 for the helper port

typedef struct _HVL
{
    /** Lock used to protect the HVL data */
    NDIS_SPIN_LOCK              Lock;

    // The following boolean is used for debugging purposes and tracks whether the HVL is locked
    BOOLEAN                     fLocked;

    BOOLEAN                     fVirtualizationEnabled;    

    /*
      The handle by which NDIS recognizes this adapter. This handle needs to be passed
      in for many of the calls made to NDIS
     */
    NDIS_HANDLE                 MiniportAdapterHandle;

    /*
        handle for the context switch work item
        */
    NDIS_HANDLE                 CtxSWorkItemHandle;

    /*
        event that signals that the HVL is terminating
        */
    KEVENT                      TerminatingEvent;

    /*
        Event that signals that someone is requesting a forced context switch
        */
    KEVENT                      CtxSEvent;
    
    /*
        Event that signals that someone is requesting exclusive access
        */
    KEVENT                      ExAccessEvent;
    
    /*
        Event that signals that we need to notify the VNICs
        */
    KEVENT                      NotifyVNicEvent;
    
    /*
        variable to track threads that are still running. The HVL will not free up memory till this
        count goes to zero
        */
    ULONG                       ulNumThreadsPending;

    ULONG                       ulStatusFlags;
    PVNIC                       pExAccessVNic;
    PVNIC                       pExAccessDelegatedVNic;
    
    PADAPTER                    Adapter;

    PVOID                       Hw;

    // list of all VNICs (not including the helper VNIC) that have registered with the HVL
    LIST_ENTRY                  VNiclist;
    
    /*
        We utilize the fact that our sample miniport has a cap on the maximum number of ports it
        can support. Having an array simplifies our code a bit and also enables to not allocate/free
        memory whenever a context merge/split operation is to be done
        */
    HVL_CONTEXT                 HvlContexts[HVL_NUM_CONTEXTS]; 

    // the number of port contexts. This number will change due to context merge/splits
    ULONG                       ulNumPortCtxs;
    
    PHVL_CONTEXT                pActiveContext;
    LIST_ENTRY                  InactiveContextList;
    PHVL_CONTEXT                pHelperPortCtx;

    /**
     *Queue for pending operations. 
     */
    LIST_ENTRY                  PendingOpQueue;

    /**
     *Queue for notifications. 
     */
    LIST_ENTRY                  NotificationsQueue;

    /*
        handle for the notifications work item
        */
    NDIS_HANDLE                 NotificationsWorkItemHandle;
    BOOLEAN                     fNotificationsWorkItemRunning;

    OP_CHANNEL_NOTIFICATION     CachedChannelNotification;

    /*
        pPnpOpExReq points to the pre-allocated exclusive access request structure used for 
        PnP related exclusive access operations. It needs to be pre-allocated because we cannot 
        fail exclusive access request for certain operations e.g. MiniportPause
        */
    PHVL_EX_ACCESS_REQ          pPnpOpExReq;

    /*
        PnpPendingOp is the pending operation structure used for PnP related exclusive access 
        operations. The pvOpData field corresponds to pPnpOpExReq
        */
    HVL_PENDING_OP              PnpPendingOp;
} HVL, *PHVL;

_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_raises_(DISPATCH_LEVEL)
_At_((pHvl->Lock).OldIrql, _IRQL_saves_)
_Requires_lock_not_held_((pHvl->Lock).SpinLock)
_Acquires_lock_((pHvl->Lock).SpinLock)
VOID
HvlLock(
    _In_  PHVL       pHvl
    );

_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_min_(DISPATCH_LEVEL)
_At_((pHvl->Lock).OldIrql, _IRQL_restores_)
_Requires_lock_held_((pHvl->Lock).SpinLock)
_Releases_lock_((pHvl->Lock).SpinLock)
VOID
HvlUnlock(
    _Inout_  PHVL      pHvl
    );

BOOLEAN
HvlIsLocked(    
    _In_  PHVL      pHvl
    );

_Requires_lock_held_(pHvl->Lock.SpinLock)
_IRQL_requires_(DISPATCH_LEVEL)
VOID
HvlCtxSProcessing(
    PHVL pHvl,
    PHVL_CONTEXT pNextActiveContext,
    BOOLEAN fMerged,
    ULONG ulFlags
    );

NDIS_IO_WORKITEM_FUNCTION HvlCtxSWorkItem;

_IRQL_requires_(DISPATCH_LEVEL)
VOID
HvlNotifyAllVNics(
    PHVL pHvl,
    PVOID pvNotif
    );

BOOLEAN
HvlIsPreAllocatedRequest(
    PHVL pHvl, 
    PHVL_EX_ACCESS_REQ pExReq
    );

BOOLEAN
HvlIsPreAllocatedPendingOp(
    PHVL pHvl, 
    PHVL_PENDING_OP pPendingOp
    );

VOID
HvlClearCachedNotification(
    PHVL                    pHvl
    );

PVNIC    
HvlGetExAccessVNic(PHVL pHvl);

BOOLEAN
HvlIsCtxSInProgress(_In_ PHVL pHvl);

BOOLEAN
HvlIsHelperVNic(PHVL pHvl, PVNIC pVNic);
