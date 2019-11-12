/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    vnic_defs.h

Abstract:
    Contains VNIC specific defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

typedef struct _HVL_CONTEXT HVL_CONTEXT, *PHVL_CONTEXT;

typedef enum _VNIC_CTXS_REF_TYPE
{
    REF_CTXS_BARRIER = 0,
    REF_JOIN,
    REF_PENDING_OP_WORKITEM,
    REF_SEND_PKTS,
    REF_CH_SW,
    REF_HW_OP,
    REF_START_BSS,
    REF_STOP_BSS,
    REF_DEF_KEY,
    REF_DEF_KEY_ID,
    REF_OPERATING_PHY_ID,
    REF_DESIRED_PHY_ID_LIST,
    REF_SCAN,
    REF_NIC_POWER_STATE,
    REF_MAX
}VNIC_CTXS_REF_TYPE;

typedef struct _VNIC_COMPLETION_CTX
{
    PORT11_GENERIC_CALLBACK_FUNC CompletionFn;
} VNIC_COMPLETION_CTX, *PVNIC_COMPLETION_CTX;

typedef struct _VNIC
{
    /* 
        link for the next VNIC in the HVL context - this field is manipulated by the HVL - against
        the principles of data hiding but simplifies our code
        */
    LIST_ENTRY CtxLink;

    /*
        link for the next VNIC in the HVL's VNIC list. this field is manipulated by the HVL - against
        the principles of data hiding but simplifies our code 
        */
    LIST_ENTRY VNicLink;

    PHVL_CONTEXT pHvlCtx;
    
    /** Lock used to protect the VNIC data */
    NDIS_SPIN_LOCK              Lock;

    // The following boolean is used for debugging purposes and tracks whether the VNic is locked
    BOOLEAN                     fLocked;

    /** Refcount that blocks the VNIC from getting context switched out */
    LONG                        CtxtSRefCount;
    LONG                        CtxSRefCountTracker[REF_MAX];   

    /**
     * The handle by which NDIS recognizes this adapter. This handle needs to be passed
     * in for many of the calls made to NDIS
     */
    NDIS_HANDLE                 MiniportAdapterHandle;

    /**Is the VNIC terminating*/
    BOOLEAN fTerminating;

    /**Is the VNIC active right now */
    BOOLEAN fActive;

    /**Does the VNIC have exclusve access right now */
    BOOLEAN fExAccess;

    BOOLEAN fResetInProgress;
    
    MP_PORT_TYPE                PortType;
    NDIS_PORT_NUMBER            PortNumber;
    
    
    PVOID                       pvPort;

    PVOID                       pvHvl;

    PVOID                       pvHw;

    /**
     * This is MAC context in the HW layer that is specific to this VNIC. 
     */
    PVOID                       pvHwContext;

    /**
     *Queue for pending hardware operations. These operations will be processed once the 
     VNIC become active
     */
    LIST_ENTRY                  PendingOpQueue;

    /*
     * Data structure to queue NBL to be transmitted in. We will queue NBL
     * if we are not active or if the hardware cannot transmit right now (e.g. it is low on
      resources)
     */
    PKT_QUEUE                   TxQueue;

    /*
        Number of sends that have been submitted to the hardware but haven't yet been
        completed by it
        */
    LONG                       lOutstandingSends;
    
    /** handle for the work item that processes pending operations*/    
    NDIS_HANDLE                 PendingOpsWorkItemHandle;

    // flag that tells whether the pending operations work item is currently running
    BOOLEAN fPendingOpsRunning;
    NDIS_EVENT PendingOpWorkItemDoneEvent;

    BOOLEAN fVNicReadyToCtxS;

    /*
        The channel number that the VNIC prefers. This is useful in the following cases
        1. An AP might decide to move to the other channel in order to avoid context switches
        2. A station might decide to prefer an AP on the same channel over an AP  on some other 
          channel, possibly with a higher signal strength

        A value of zero means there is no preferred channel yet
          */
    ULONG ulPreferredChannel;
    PVNIC pPreferredChannelSrcVNic;

    /*
        pPnpOpExReq points to the pre-allocated exclusive access request structure used for 
        PnP related exclusive access operations. It needs to be pre-allocated because we cannot 
        fail exclusive access request for certain operations e.g. MiniportPause
        */
    PVNIC_EX_ACCESS_REQ  pPnpOpExReq;

    /*
        PnpPendingOp is the pending operation structure used for PnP related exclusive access 
        operations. The pvOpData field corresponds to pPnpOpExReq
        */
    PENDING_OP           PnpPendingOp;

    BOOLEAN LastLinkStateNotified;   // TRUE = MediaConnected, FALSE = Disconnected
} VNIC, *PVNIC;

#define VNIC_PORT_NO (pVNic->PortNumber)

// VNIC lock manipulation macros

_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_raises_(DISPATCH_LEVEL)
_At_((pVNic->Lock).OldIrql, _IRQL_saves_)
_Requires_lock_not_held_((pVNic->Lock).SpinLock)
_Acquires_lock_((pVNic->Lock).SpinLock)
VOID
VNicLock(
    _In_  PVNIC      pVNic
    );

_Acquires_lock_((&pVNic->Lock)->SpinLock)
VOID
VNicLockAtDispatch(
    _In_  PVNIC      pVNic,
    _In_  BOOLEAN    fDispatch
    );

_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_min_(DISPATCH_LEVEL)
_At_((pVNic->Lock).OldIrql, _IRQL_restores_)
_Requires_lock_held_((pVNic->Lock).SpinLock)
_Releases_lock_((pVNic->Lock).SpinLock)
VOID
VNicUnlock(
    _In_  PVNIC      pVNic
    );

_Requires_lock_held_((&pVNic->Lock)->SpinLock)
_Releases_lock_((&pVNic->Lock)->SpinLock)
VOID
VNicUnlockAtDispatch(
    _In_  PVNIC      pVNic,
    _In_  BOOLEAN    fDispatch
    );

BOOLEAN
VNicIsLocked(    
    _In_  PVNIC      PVNIC
    );
    
VOID
VNicAddCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  VNIC_CTXS_REF_TYPE RefType
    );

VOID
VNicRemoveCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  VNIC_CTXS_REF_TYPE RefType
    );

VOID
VNicIncCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  ULONG      ulNumRef,
    _In_  VNIC_CTXS_REF_TYPE RefType
    );

VOID
VNicDecCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  ULONG      ulNumRef,
    _In_  VNIC_CTXS_REF_TYPE RefType
    );

BOOLEAN
VNicIsActive(    
    _In_  PVNIC      pVNic
    );

NDIS_STATUS
VNicPreHwSyncCallActions(PVNIC pVNic, BOOLEAN *pfProgramHw);

VOID
VNicPostHwSyncCallActions(PVNIC pVNic, BOOLEAN fProgramHw);

NDIS_STATUS 
VNicCanProcessReqNow(PVNIC pVNic, BOOLEAN *pfProcessNow);

BOOLEAN
VNicIsInReset(
    _In_ PVNIC pVNic
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicSwChHelper(
    _In_  PVNIC                   pVNic,
    _In_  ULONG                   ulChannel,
    _In_  ULONG                   ulPhyId,
    _In_  BOOLEAN                 fSwitchPhy,
    _In_  PVNIC_COMPLETION_CTX    pCtx
    );

NDIS_STATUS 
VNicReleaseExAccess(
    PVNIC pVNic
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicSetDefaultKeyHelper(
    _In_  PVNIC                   pVNic,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  ULONG                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue    
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicSetOperatingPhyIdHelper(
    _In_  PVNIC                   pVNic,
    _In_  ULONG                   PhyId
    );

_Requires_lock_held_(pVNic->Lock.SpinLock)
_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicSetNicPowerStateHelper(
    _In_  PVNIC                   pVNic,
    _In_  BOOLEAN                 PowerState,
    _In_  BOOLEAN                 SelectedPhy
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicCompleteExclusiveAccessOp(
    PVNIC pVNic,
    PENDING_OP_TYPE opType,
    PVOID pvOpData,
    BOOLEAN fCompleteReq
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS 
VNicChSwComplete(
    _In_  PVNIC                   pVNic,
    _In_  BOOLEAN                 fReferenced,
    _In_  PORT11_GENERIC_CALLBACK_FUNC ChSwCompleteHandler,
    _In_  PVOID                   Data
    );

BOOLEAN
VNicHasExAccess(    
    _In_  PVNIC      pVNic
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicSetDesiredPhyIdListHelper(
    _In_  PVNIC                   pVNic,
    _In_  PULONG                  PhyIDList,
    _In_  ULONG                   PhyIDCount
    );

VOID
VNicSetReadyToCtxS(
    PVNIC pVNic,
    BOOLEAN fReadyToCtxS
    );

BOOLEAN
VNicIsPreallocatedRequest(
    PVNIC pVNic,
    PVNIC_EX_ACCESS_REQ pExReq
    );

BOOLEAN
VNicIsPreallocatedOperation(
    PVNIC pVNic,
    PPENDING_OP pPendingOp
    );

VOID
VNicSendChannelNotification(
    PVNIC pVNic,
    ULONG ulChannel
    );

VOID
VNicSendLinkStateNotification(
    IN  PVNIC                   pVNic,
    IN  BOOLEAN                 MediaConnected
    );

