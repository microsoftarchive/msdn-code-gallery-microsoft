/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    vnic_main.c

Abstract:
    Implements the PNP routines for the VNIC
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"


#if DOT11_TRACE_ENABLED
#include "vnic_main.tmh"
#endif

#define INC_CTXS_REF(pVNic) InterlockedIncrement(&pVNic->CtxtSRefCount)
#define DEC_CTXS_REF(pVNic) InterlockedDecrement(&pVNic->CtxtSRefCount)

__inline
PSTR
GetPortTypeString(
    MP_PORT_TYPE PortType
    )
{
    PSTR pszString = NULL;
    
    switch (PortType)
    {
        case HELPER_PORT:
            pszString = "Helper ";
            break;
        case EXTSTA_PORT:
            pszString = "Extensible Station ";
            break;
        case EXTAP_PORT:
            pszString = "Extensible AP ";
            break;
    }

    return pszString;
}


_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_raises_(DISPATCH_LEVEL)
_At_((pVNic->Lock).OldIrql, _IRQL_saves_)
_Requires_lock_not_held_((pVNic->Lock).SpinLock)
_Acquires_lock_((pVNic->Lock).SpinLock)
extern
__inline
VOID
VNicLock(
    _In_  PVNIC      pVNic
    )
{
    ACQUIRE_LOCK(pVNic, FALSE);                                               
}

_Acquires_lock_((&pVNic->Lock)->SpinLock)
extern
__inline
VOID
VNicLockAtDispatch(
    _In_  PVNIC      pVNic,
    _In_  BOOLEAN    fDispatch
    )
#pragma warning (suppress:28167) // IRQL change
{
    ACQUIRE_LOCK(pVNic, fDispatch);                                               
}

_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_min_(DISPATCH_LEVEL)
_At_((pVNic->Lock).OldIrql, _IRQL_restores_)
_Requires_lock_held_((pVNic->Lock).SpinLock)
_Releases_lock_((pVNic->Lock).SpinLock)
extern
__inline
VOID
VNicUnlock(
    _In_  PVNIC      pVNic
    )
{
    #pragma prefast(disable : 6001)
    
    ASSERT(VNicIsLocked(pVNic));
    RELEASE_LOCK(pVNic, FALSE);

    #pragma prefast(enable: 6001)
}

_Requires_lock_held_((&pVNic->Lock)->SpinLock)
_Releases_lock_((&pVNic->Lock)->SpinLock)
extern
__inline
VOID
VNicUnlockAtDispatch(
    _In_  PVNIC      pVNic,
    _In_  BOOLEAN    fDispatch
    )
#pragma warning (suppress:28167) // IRQL change
{
    RELEASE_LOCK(pVNic, fDispatch);                                               
}

extern
__inline    
BOOLEAN
VNicIsActive(    
    _In_  PVNIC      pVNic
    )
{
    ASSERT(VNicIsLocked(pVNic));
    return pVNic->fActive;
}

__inline    
VOID
VNicSetActive(    
    _In_  PVNIC      pVNic,
    _In_  BOOLEAN fActive
    )
{
    ASSERT(VNicIsLocked(pVNic));
    pVNic->fActive = fActive;
}

extern
__inline    
BOOLEAN
VNicHasExAccess(    
    _In_  PVNIC      pVNic
    )
{
    ASSERT(VNicIsLocked(pVNic));
    return pVNic->fExAccess;
}

__inline    
VOID
VNicSetExAccess(    
    _In_  PVNIC      pVNic,
    _In_  BOOLEAN fExAccess
    )
{
    ASSERT(VNicIsLocked(pVNic));
    if (fExAccess)
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNic now has exclusive access \n", VNIC_PORT_NO));
    }
    else
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNic no longer has exclusive access \n", VNIC_PORT_NO));
    }
    pVNic->fExAccess = fExAccess;
}

// this function can be called only when the VNic is locked
extern
__inline    
VOID
VNicIncCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  ULONG      ulNumRef, 
    _In_  VNIC_CTXS_REF_TYPE RefType
    )
{
    ASSERT(VNicIsLocked(pVNic));
    ASSERT(VNicIsActive(pVNic));

    ASSERT(pVNic->CtxtSRefCount >= 0);
    ASSERT(pVNic->CtxSRefCountTracker[RefType] >= 0);

    pVNic->CtxtSRefCount = pVNic->CtxtSRefCount + ulNumRef;
    pVNic->CtxSRefCountTracker[RefType] = pVNic->CtxSRefCountTracker[RefType] + ulNumRef;
}


extern
__inline    
VOID
VNicAddCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  VNIC_CTXS_REF_TYPE RefType
    )
{
    VNicIncCtxSRef(pVNic, 1, RefType);
}



// this function can be called only when the VNic is locked
extern
__inline    
VOID
VNicDecCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  ULONG      ulNumRef, 
    _In_  VNIC_CTXS_REF_TYPE RefType
    )
{
    ASSERT(pVNic->fLocked);
    pVNic->CtxtSRefCount = pVNic->CtxtSRefCount - ulNumRef;
    pVNic->CtxSRefCountTracker[RefType] = pVNic->CtxSRefCountTracker[RefType] - ulNumRef;

    ASSERT(pVNic->CtxtSRefCount >= 0);
    ASSERT(pVNic->CtxSRefCountTracker[RefType] >= 0);
}

extern
__inline    
VOID
VNicRemoveCtxSRef(
    _In_  PVNIC      pVNic,
    _In_  VNIC_CTXS_REF_TYPE RefType
    )
{
    VNicDecCtxSRef(pVNic, 1, RefType);
}

extern
__inline    
BOOLEAN
VNicIsLocked(    
    _In_  PVNIC      pVNic
    )
{
    return pVNic->fLocked;
}

extern
__inline
BOOLEAN
VNicIsInReset(
    _In_ PVNIC pVNic
    )
{
    ASSERT(VNicIsLocked(pVNic));
    return pVNic->fResetInProgress;
}

extern
__inline
BOOLEAN
VNicIsPreallocatedRequest(
    PVNIC pVNic,
    PVNIC_EX_ACCESS_REQ pExReq
    )
{
    return (pVNic->pPnpOpExReq == pExReq);
}

extern
__inline
BOOLEAN
VNicIsPreallocatedOperation(
    PVNIC pVNic,
    PPENDING_OP pPendingOp
    )
{
    return (&pVNic->PnpPendingOp== pPendingOp);
}

BOOLEAN
VNic11IsOkToCtxS(
    PVNIC pVNic
    )
{
    return pVNic->fVNicReadyToCtxS;
}

VOID
VNicSetReadyToCtxS(
    PVNIC pVNic,
    BOOLEAN fReadyToCtxS
    )
{
    if (fReadyToCtxS)
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Marking the VNIC as ready to acquire context \n", VNIC_PORT_NO));
    }
    else
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Marking the VNIC as not ready to acquire context \n", VNIC_PORT_NO));
    }
    
    pVNic->fVNicReadyToCtxS = fReadyToCtxS;
}

extern
__inline
NDIS_STATUS 
VNicCanProcessReqNow(PVNIC pVNic, BOOLEAN *pfProcessNow)
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ASSERT(VNicIsLocked(pVNic));

    do
    {
        *pfProcessNow = FALSE;
        
        if (VNicIsInReset(pVNic))
        {
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }
        
        *pfProcessNow = VNicIsActive(pVNic) && VNicIsPendingOpQueueEmpty(pVNic);
    } while (FALSE);

    return ndisStatus;
}

extern
__inline    
NDIS_STATUS
VNicPreHwSyncCallActions(PVNIC pVNic, BOOLEAN *pfProgramHw)
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    
    VNicLock(pVNic);

    do
    {
        ndisStatus = VNicCanProcessReqNow(pVNic, pfProgramHw);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            break;
        }

        // add the context switch ref count for the call to the hardware
        if (*pfProgramHw)
        {
            VNicAddCtxSRef(pVNic, REF_HW_OP);
        }
    } while (FALSE);
    
    VNicUnlock(pVNic);

    return ndisStatus;
}

extern
__inline    
VOID
VNicPostHwSyncCallActions(PVNIC pVNic, BOOLEAN fProgramHw)
{
    if (fProgramHw)
    {
        VNicLock(pVNic);

        // remove the context switch ref count for the call to the hardware
        VNicRemoveCtxSRef(pVNic, REF_HW_OP);        

        VNicUnlock(pVNic);
    }
    
    return;
}

VOID
VNicWaitForCtxSRefAndMark(    
    _In_  PVNIC      pVNic,
    _In_  BOOLEAN    fMark
    )
{
    ULONG ulNumIter = 0;
    
    VNicLock(pVNic);
    
    while(pVNic->CtxtSRefCount >= 1)
    {
        if ( ++ulNumIter > 3000)
        {
            // 30 seconds have passed - something must be wrong
            ASSERTMSG("VNic reference count hasn't gone to 0 in last 30 seconds. Check driver state\n", FALSE);
        }
        
        VNicUnlock(pVNic);
        NdisMSleep(10 * 1000);  // 10 msec
        VNicLock(pVNic);
    }

    if (fMark)
    {
        VNicSetActive(pVNic, FALSE);
        /*
            A non helper port might be given exclusive access even when it hasn't requested it.
            e.g. a port gets exclusive access when a Pause is happening (the helper port calls 
            Hvl11ActivatePort). There is no corresponding callback from HVL to let the VNIC know
            that it no longer has exclusive access. Hence assume that if the context is moving
            away from you, you do not have exclusive access.
            A helper port on the other hand will explicitly request and release exclusive access
            through VNic11RequestExAccess and VNic11ReleaseExAccess
            */
        if (pVNic->PortType != HELPER_PORT)
        {
            VNicSetExAccess(pVNic, FALSE);
        }
    }
    
    VNicUnlock(pVNic);
}

VOID
VNicWaitForCtxSRefAndMarkInactive(    
    _In_  PVNIC      pVNic
    )
{
    VNicWaitForCtxSRefAndMark(pVNic, TRUE);
}

VOID
VNicWaitForCtxSRef(    
    _In_  PVNIC      pVNic
    )
{
    VNicWaitForCtxSRefAndMark(pVNic, FALSE);
}

VOID
VNicWaitForResetToComplete(    
    _In_  PVNIC      pVNic
    )
{
    ULONG ulNumIter = 0;
    
    VNicLock(pVNic);

    // wait for reset only after it has acquired exclusive access
    while(pVNic->fResetInProgress)
    {
        if ( ++ulNumIter > 3000)
        {
            // 30 seconds have passed - something must be wrong
            ASSERTMSG("VNic is processing reset since last 30 seconds. Check driver state\n", FALSE);
        }
        
        VNicUnlock(pVNic);
        NdisMSleep(10 * 1000);  // 10 msec
        VNicLock(pVNic);
    }

    VNicUnlock(pVNic);
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
VNicWaitForOutstandingSends(    
    _In_  PVNIC      pVNic
    )
{
    ULONG ulNumIter = 0;
    
    ASSERT(VNicIsLocked(pVNic));

    while(pVNic->lOutstandingSends >= 1)
    {
        if ( ++ulNumIter > 3000)
        {
            // 30 seconds have passed - something must be wrong
            ASSERTMSG("VNic outtanding sends count hasn't gone to 0 in last 30 seconds. Check driver state\n", FALSE);
        }

        _Analysis_assume_lock_held_(pVNic->Lock.SpinLock);
        VNicUnlock(pVNic);
        NdisMSleep(10 * 1000);  // 10 msec
        VNicLock(pVNic);
    }    
}

BOOLEAN
VNicOperationRequiresExAccess(PENDING_OP_TYPE OpType)
{
    if (PENDING_OP_EX_ACCESS_REQ == OpType   ||
      PENDING_OP_RESET_REQ == OpType         ||
      PENDING_OP_CH_SW_REQ == OpType         ||
      PENDING_OP_DEF_KEY == OpType           ||
      PENDING_OP_DESIRED_PHY_ID_LIST == OpType ||
      PENDING_OP_OPERATING_PHY_ID == OpType  ||
      PENDING_OP_NIC_POWER_STATE == OpType
      )
    {
        return TRUE;
    }
    else
    {
        return FALSE;
    }
}

NDIS_STATUS
VNic11Allocate(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_maybenull_ PVNIC*        ppVNic,
    _In_  PMP_PORT                pPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC pVNic = NULL;
    NDIS_HANDLE pendingOpsWorkItemHandle = NULL;
    BOOLEAN fFreeWorkItemHandle = FALSE;
    PVNIC_EX_ACCESS_REQ pExReq = NULL;
    
    do 
    {
        if ( NULL == ppVNic)
        {
            ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
            MpTrace(COMP_HVL, DBG_SERIOUS, ("ppVNic is NULL"));
            break;
        }
        
        *ppVNic = NULL;
            
        ndisStatus = ALLOC_MEM(MiniportAdapterHandle, sizeof(VNIC), &pVNic);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for a new VNIC"));
            break;
        }

        // the list heads should be the first ones to be initialized. This allows us to free things 
        // correctly if Free is called without Initialize being called in between        
        InitializeListHead(&pVNic->PendingOpQueue);

        // Allocate the pending operations work item 
        pendingOpsWorkItemHandle = NdisAllocateIoWorkItem(MiniportAdapterHandle);
        if(NULL == pendingOpsWorkItemHandle)
        {
            MpTrace (COMP_HVL, DBG_SERIOUS, ("NdisAllocateIoWorkItem failed"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        fFreeWorkItemHandle = TRUE;

        // pre-allocate the exclusive access request structure for PnP related exclusive accesses        
        ndisStatus = ALLOC_MEM(MiniportAdapterHandle, sizeof(VNIC_EX_ACCESS_REQ), &pExReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for exclusive access request"));
            break;
        }

        // Allocate memory for fields inside the VNIC structure
        NdisAllocateSpinLock(&(pVNic->Lock));

        InitPktQueue(&pVNic->TxQueue);

        pVNic->MiniportAdapterHandle = MiniportAdapterHandle;
        
        pVNic->PendingOpsWorkItemHandle = pendingOpsWorkItemHandle;
        
        // Save the pointers in the VNIC
        pVNic->pvPort = pPort;
        pVNic->pPnpOpExReq = pExReq;
    
        *ppVNic = pVNic;

        MpTrace(COMP_HVL, DBG_NORMAL, ("Allocated a new VNIC %p", pVNic));        
    } while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (NULL != pExReq)
        {
            FREE_MEM(pExReq);
        }
        if (fFreeWorkItemHandle)
        {
            NdisFreeIoWorkItem(pendingOpsWorkItemHandle);
        }
        if (NULL != pVNic)
        {
            FREE_MEM(pVNic);
        }
    }

    return ndisStatus;
}

VOID
VNic11Free(
    _In_  PVNIC                   pVNic
    )
{
    DeInitPktQueue(&pVNic->TxQueue);
    
    NdisFreeSpinLock(&(pVNic->Lock));

    if (pVNic->PendingOpsWorkItemHandle)
    {
        NdisFreeIoWorkItem(pVNic->PendingOpsWorkItemHandle);
    }

    if (pVNic->pPnpOpExReq)
    {
        FREE_MEM(pVNic->pPnpOpExReq);
    }
    
    FREE_MEM(pVNic);

    MpTrace(COMP_HVL, DBG_NORMAL, ("Freed the VNIC %p", pVNic));        
}

NDIS_STATUS
VNic11Initialize(
    _In_  PVNIC                   pVNic,
    _In_  PVOID                   pvHvl,
    _In_  PVOID                   pvHw,
    _In_  MP_PORT_TYPE            PortType,
    _In_  NDIS_PORT_NUMBER        PortNumber
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHW_MAC_CONTEXT pHwCtx = NULL;
    BOOLEAN fRegistered = FALSE;
    
    do
    {
        pVNic->pvHvl = pvHvl;
        pVNic->pvHw = pvHw;
        pVNic->PortType = PortType;        
        pVNic->CtxtSRefCount = 0;        // Initial RefCount = 0
        pVNic->fPendingOpsRunning = FALSE;
        pVNic->lOutstandingSends = 0;
        pVNic->fActive = FALSE;          // Port starts in inactive state
        pVNic->fExAccess = FALSE; 
        pVNic->fResetInProgress = FALSE;
        
        pVNic->PortNumber = PortNumber;

        VNicInitPreAllocatedOp(pVNic);
        
        VNicSetReadyToCtxS(pVNic, FALSE);
        NdisInitializeEvent(&pVNic->PendingOpWorkItemDoneEvent);
        
        /*
            This event is initialized to be set when the VNIC starts. Whenever the VNIC starts
            a pending operation VNIC, this event is reset. The event is set again whenever the
            pending ops work item completes
            */
        NdisSetEvent(&pVNic->PendingOpWorkItemDoneEvent); 
        
        // Allocate some MAC specific context structure in the HW
        ndisStatus = Hw11AllocateMACContext(pvHw, &pHwCtx, pVNic, PortNumber);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Hw11AllocateMACContext failed %!x!", VNIC_PORT_NO, ndisStatus));
            break;
        }

        pVNic->pvHwContext = pHwCtx;

        if (PortType == HELPER_PORT)
        {
            ndisStatus = Hvl11RegisterHelperPort(pvHvl, pVNic);
        }
        else
        {
            ndisStatus = Hvl11RegisterVNic(pvHvl, pVNic);
        }
        
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Hvl11RegisterVNic failed %!x!", VNIC_PORT_NO, ndisStatus));
            break;
        }

        fRegistered = TRUE;

        MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%p, port number = %d, type=%s): Initialized\n", pVNic, PortNumber, GetPortTypeString(PortType)));
        
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (fRegistered)
        {
            if (pVNic->PortType == HELPER_PORT)
            {
                Hvl11DeregisterHelperPort(pVNic->pvHvl, pVNic);
            }
            else
            {
                Hvl11DeregisterVNic(pVNic->pvHvl, pVNic);
            }    
        }

        if (pHwCtx)
        {
            Hw11FreeMACContext(pVNic->pvHw, pVNic->pvHwContext);
        }
    }
    
    return ndisStatus;
}

VOID
VNic11UpdatePortType(
    _In_  PVNIC                   pVNic,
    _In_  MP_PORT_TYPE            PortType
    )
{
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Updating port type %s  ---> %s\n", VNIC_PORT_NO, GetPortTypeString(pVNic->PortType), GetPortTypeString(PortType)));
    pVNic->PortType = PortType;
}

VOID
VNic11Terminate(
    _In_  PVNIC                   pVNic
    )
{
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Terminate called\n", VNIC_PORT_NO));
    
    VNicLock(pVNic);
    pVNic->fTerminating = TRUE;
    VNicUnlock(pVNic);
    
    if (pVNic->PortType == HELPER_PORT)
    {
        Hvl11DeregisterHelperPort(pVNic->pvHvl, pVNic);
    }
    else
    {
        Hvl11DeregisterVNic(pVNic->pvHvl, pVNic);
    }
    
    // wait for context switch ref count to go to zero 
    VNicWaitForCtxSRef(pVNic);

    // wait for any pending work items to complete
    NdisWaitEvent(&pVNic->PendingOpWorkItemDoneEvent, 0);
    ASSERT(!pVNic->fPendingOpsRunning);
        
    // Free the MAC context that has been previously allocated in the HW for this port
    if (pVNic->pvHwContext)
    {
        Hw11FreeMACContext(pVNic->pvHw, pVNic->pvHwContext);
    }

    // free the memory allocated for any pending operations
    VNicDeleteAllPendingOperations(pVNic);
}

NDIS_STATUS
VNic11Pause(
    _In_  PVNIC                   pVNic
    )
{
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Pause called \n", VNIC_PORT_NO));

    VNicLock(pVNic);

    // the port should have gotten us active before calling us
    ASSERT(VNicIsActive(pVNic) || (HELPER_PORT == pVNic->PortType));

    // cancel any pending sends in our queue
    VNicCancelPendingSends(pVNic);

    // Wait for the hardware to complete all our sends
    VNicWaitForOutstandingSends(pVNic);

    VNicUnlock(pVNic);
    
    Hw11PauseMACContext(pVNic->pvHwContext);
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
VNic11Restart(
    _In_  PVNIC                   pVNic
    )
{
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Restart called \n", VNIC_PORT_NO));

    Hw11RestartMACContext(pVNic->pvHwContext);
    
    // nothing to do
    return NDIS_STATUS_SUCCESS;
}

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicPerformReset(
    PVNIC pVNic,
    PDOT11_RESET_REQUEST pDot11ResetReq
)
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN fUnlocked = FALSE;

    ASSERT(VNicIsLocked(pVNic));

    do
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Performing VNIC Reset items\n", VNIC_PORT_NO));

        // BUGBUG: Once the send queue is merged with the pending ops, we don't need to cancel any pending sends
        // cancel any pending sends in our queue
        VNicCancelPendingSends(pVNic);

        // Wait for the hardware to complete all our sends
        VNicWaitForOutstandingSends(pVNic);

        _Analysis_assume_lock_held_(pVNic->Lock.SpinLock);
        VNicUnlock(pVNic);
        fUnlocked = TRUE;
        
        // wait for all the other references to go away
        VNicWaitForCtxSRef(pVNic);
                
        // now ask the hardware to reset itself
        ndisStatus = Hw11Dot11Reset(pVNic->pvHwContext, pDot11ResetReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Hw11Reset failed %!x!\n", VNIC_PORT_NO, ndisStatus));
            break;
        }        
    } while (FALSE);

    if (fUnlocked)
    {
        VNicLock(pVNic);
    }
    
    return ndisStatus;
}


NDIS_STATUS
VNic11Dot11Reset(
    _In_  PVNIC                   pVNic,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    VNIC_REQ Req;
    BOOLEAN fLocked = FALSE;
    PRESET_REQ pResetReq = NULL;
    BOOLEAN fFreeResetReq = FALSE;

    do
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Reset called\n", VNIC_PORT_NO));

        VNicInitReq(&Req);

        ndisStatus = VNicAllocateResetReq(pVNic, &Req, Dot11ResetRequest, &pResetReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNIC(%d): VNicAllocateResetReq failed %!x!\n", VNIC_PORT_NO, ndisStatus));
            break;
        }
        fFreeResetReq = TRUE;
        
        ndisStatus = VNicHandleExAccessOp(pVNic, PENDING_OP_RESET_REQ, pResetReq, FALSE, FALSE);
        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicHandleExAccessOp called %!x!\n", VNIC_PORT_NO, ndisStatus));

        // wait for us to get exclusive access and all the pending operations to complete
        if (NDIS_STATUS_PENDING == ndisStatus)
        {
            // the pending ops item would free the reset request
            fFreeResetReq = FALSE;

            VNicWaitForReq(&Req);            
            ndisStatus = VNicGetReqStatus(&Req);
        }
        else if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNIC(%d): VNicHandleExAccessOp failed %!x!\n", VNIC_PORT_NO, ndisStatus));
            break;
        }
        
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): The pending reset request was completed with status %!x!\n", VNIC_PORT_NO, ndisStatus));

        VNicLock(pVNic);
        fLocked = TRUE;

        ndisStatus = VNicPerformReset(pVNic, Dot11ResetRequest);
    } while (FALSE);

    if (NDIS_FAILURE(ndisStatus))
    {
        // clear the reset flag
        pVNic->fResetInProgress = FALSE;        
    }
    
    if (fLocked)
    {
        VNicUnlock(pVNic);
    }

    if (fFreeResetReq)
    {
        VNicFreeResetReq(pResetReq);
    }
    
    return ndisStatus;
}

NDIS_STATUS
VNic11Dot11ResetComplete(
    _In_  PVNIC                   pVNic
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): ResetComplete called\n", VNIC_PORT_NO));

    VNicLock(pVNic);

    pVNic->fResetInProgress = FALSE;

    VNicSetReadyToCtxS(pVNic, FALSE);
    
    // done with the reset operation. Release exclusive access
    VNicReleaseExAccess(pVNic);

    VNicUnlock(pVNic);

    /*
        Invalidate any channel & media notifications we may have sent out earlier. 0 means no preferred channel
        */
    VNicSendChannelNotification(pVNic, 0);
    VNicSendLinkStateNotification(pVNic, FALSE);

    return ndisStatus;
}

VOID
VNic11CtxSFromVNic(    
    _In_  PVNIC      pVNic,
    _In_  ULONG       ulFlags
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN fConnected = FALSE;
    PVOID pvHwMacCtx = pVNic->pvHwContext;

    UNREFERENCED_PARAMETER(ulFlags);

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Hvl asked to move context away from us \n", VNIC_PORT_NO));

    // wait for the context switch ref count to go to zero and mark ourselves inactive
    VNicWaitForCtxSRefAndMarkInactive(pVNic);

    // Wait for any ongoing reset to complete. If we do not wait, it is possible that we would 
    // become inactive (and lose exclusive access) before the reset processing is complete.
    VNicWaitForResetToComplete(pVNic);
    
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Done waiting for outstanding references.\n", VNIC_PORT_NO));
    
    // send a null packet to the AP with the power save bit set, if we are currently 
    // connected to an infrastructure network
    if (pVNic->PortType == EXTSTA_PORT && (dot11_BSS_type_independent != Hw11QueryCurrentBSSType(pVNic->pvHwContext)))
    {
        fConnected = Hw11IsConnected(pvHwMacCtx);
        if (fConnected)
        {
            Hw11SendNullPkt(pvHwMacCtx, TRUE);
        }
    }
    
    ndisStatus = Hw11DisableMACContext(pVNic->pvHw, pVNic->pvHwContext);
    ASSERT(NDIS_STATUS_SUCCESS == ndisStatus);

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Disabled the hardware MAC context.\n", VNIC_PORT_NO));
}

VOID
VNicHandlePendingConnReq(
    PVNIC pVNic
    )
{
    do
    {    
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Discarding a pending connection start operation since the VNIC is in reset\n", VNIC_PORT_NO));
            break;
        }
        
        VNicAddCtxSRef(pVNic, REF_CTXS_BARRIER);
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Completed a pending connection start operation\n", VNIC_PORT_NO));

    } while (FALSE);
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
VNicHandlePendingJoinReq(
    PVNIC pVNic,
    PJOIN_REQ pJoinReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_PENDING;
    PVNIC_COMPLETION_CTX pVNicCompletionCtx = NULL;
    
    do
    {    
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): VNic is in reset. Completing a pending join request with failure\n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }

        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(VNIC_COMPLETION_CTX), &pVNicCompletionCtx);
        if (NDIS_FAILURE(ndisStatus))
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new completion context \n", VNIC_PORT_NO));
            break;
        }
        
        pVNicCompletionCtx->CompletionFn = pJoinReq->CompletionHandler;
        
        ndisStatus = VNicJoinBSSHelper(
                    pVNic, 
                    pJoinReq->BSSDescription, 
                    pJoinReq->JoinFailureTimeout, 
                    pVNicCompletionCtx
                    );

        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicJoinBSSHelper returned %!x!\n", VNIC_PORT_NO, ndisStatus));

    } while (FALSE);
    
    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        VNicJoinComplete(pVNic, FALSE, pJoinReq->CompletionHandler, &ndisStatus);
        
        if (pVNicCompletionCtx)
        {
            FREE_MEM(pVNicCompletionCtx);
        }
    }
    
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Completed a pending Join operation\n", VNIC_PORT_NO));
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
VNicHandlePendingStartBSSReq(
    PVNIC pVNic,
    PSTART_BSS_REQ pStartBss 
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_PENDING;
    
    do
    {    
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Discarding a pending start BSS operation since the VNIC is in reset\n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }
        
        ndisStatus = VNicStartBSSHelper(
                    pVNic, 
                    pStartBss->BSSDescription
                    );
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicStartBSSHelper returned %!x!\n", VNIC_PORT_NO, ndisStatus));

    } while (FALSE);
    
    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        // notify the port that the startBss operation has completed
        if (pStartBss->CompletionHandler)
        {
            _Analysis_assume_lock_held_(pVNic->Lock.SpinLock);
            VNicUnlock(pVNic);
            pStartBss->CompletionHandler(pVNic->pvPort, &ndisStatus);
            VNicLock(pVNic);
        }
    }
    
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Completed a pending start BSS operation %!x!\n", VNIC_PORT_NO, ndisStatus));
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
VNicHandlePendingStopBSSReq(
    PVNIC pVNic,
    PSTOP_BSS_REQ pStopBss 
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_PENDING;
    
    do
    {    
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Discarding a pending stop BSS operation since the VNIC is in reset\n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }
        
        ndisStatus = VNicStopBSSHelper(pVNic);
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicStopBSSHelper returned %!x!\n", VNIC_PORT_NO, ndisStatus));

    } while (FALSE);

    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        // notify the port that the stopBss operation has completed
        if (pStopBss->CompletionHandler)
        {
            _Analysis_assume_lock_held_((& pVNic->Lock)->SpinLock);
            VNicUnlock(pVNic);
            pStopBss->CompletionHandler(pVNic->pvPort, &ndisStatus);
            VNicLock(pVNic);
        }
    }
    
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Completed a pending stop BSS operation %!x!\n", VNIC_PORT_NO, ndisStatus));    
}

/*
    This routine processes all the pending operations in the VNIC. It goes through the pending
    operations in a loop, completing each one by one. It does give up the VNIC lock as part of
    the processing e.g. to callback into the port or call into the hardware. However that is fine 
    because we always manipulate the list under the VNIC lock 
    */
_Function_class_(NDIS_IO_WORKITEM_FUNCTION)
VOID
VNicPendingOpsWorkItem(
    _In_ PVOID            Context,
    _In_ NDIS_HANDLE      NdisIoWorkItemHandle
    )
{
    PVNIC pVNic = NULL;
    LIST_ENTRY *pEntryOp = NULL;
    PPENDING_OP pOp = NULL;
    PVOID pvPort = NULL;
    BOOLEAN fExAccessOpFound = FALSE;
    BOOLEAN fOpsSkipped = FALSE;
    PENDING_OP_TYPE Type;
    
    UNREFERENCED_PARAMETER(NdisIoWorkItemHandle);

    pVNic = (PVNIC)Context;

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicPendingOpsWorkItem called\n", VNIC_PORT_NO));

    VNicLock(pVNic);

    pvPort = pVNic->pvPort;

    pEntryOp = pVNic->PendingOpQueue.Flink;
    while (pEntryOp != &pVNic->PendingOpQueue)
    {
        pOp = CONTAINING_RECORD(pEntryOp, PENDING_OP, Link);

        /*
            If we are not being granted exclusive access and the operation requires it, we should
            stop doing more processing for now. The HVL will call us again with the exclusive
            access and these operations can be processed then. Note that we can not simply
            skip over these operations since all the operations must be performed in a sequence
            */
        if (!VNicHasExAccess(pVNic) && VNicOperationRequiresExAccess(pOp->Type))
        {
            fOpsSkipped = TRUE;
            break;
        }

        // This item can now be removed from the list        
        RemoveEntryList (&pOp->Link);

        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Found a pending %s operation\n", VNIC_PORT_NO, GetPendingOpString(pOp->Type)));

        Type = pOp->Type;
        
        switch (Type)
        {
            case PENDING_OP_CONN_START:
                VNicHandlePendingConnReq(pVNic);
                break;

            case PENDING_OP_JOIN_REQ:
            {
                VNicHandlePendingJoinReq(pVNic, pOp->pvOpData);
                break;
            }

            case PENDING_OP_START_BSS_REQ:
            {
                VNicHandlePendingStartBSSReq(pVNic, pOp->pvOpData);
                break;
            }

            case PENDING_OP_STOP_BSS_REQ:
            {
                VNicHandlePendingStopBSSReq(pVNic, pOp->pvOpData);
                break;
            }

            case PENDING_OP_EX_ACCESS_REQ:
            case PENDING_OP_RESET_REQ:
            case PENDING_OP_CH_SW_REQ:
            case PENDING_OP_DEF_KEY:
            case PENDING_OP_DESIRED_PHY_ID_LIST:
            case PENDING_OP_OPERATING_PHY_ID:
            case PENDING_OP_NIC_POWER_STATE:
            {
                ASSERT(VNicHasExAccess(pVNic));
                fExAccessOpFound = TRUE;
                VNicCompleteExclusiveAccessOp(pVNic, pOp->Type, pOp->pvOpData, TRUE);
                break;
            }

            default:
                ASSERT(FALSE);
        };

        VNicDeletePendingOperation(pVNic, pOp);

        // we should be locked again
        ASSERT(VNicIsLocked(pVNic));

        if (Type == PENDING_OP_EX_ACCESS_REQ && (pVNic->PortType == HELPER_PORT))
        {
            // This is a special case for the helper port. The helper port needs to serialize all
            // its exclusive access request callbacks. The HVL will call the VNIC separately for
            // each exclusive access request. The VNIC needs to do its job and complete no
            // more than one exclusive access request each time
            break;
        }
        
        pEntryOp = pVNic->PendingOpQueue.Flink;
    }

    /*
        BUGBUG 
        Handle any sends that had been queued. However if we bailed before completing all the
        operations then we should not handle the sends since they might rely on one of the 
        operations e.g. channel switch. This special casing is an artifact of the fact that we have
        separate queues for pending ops and data. We should perhaps merge the two
        */
    if (!fOpsSkipped && !PktQueueIsEmpty(&pVNic->TxQueue))
    {
        VNicProcessQueuedPkts(pVNic, FALSE);
    }
    else if (fOpsSkipped)
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Not processing sends since some operations were skipped.\n", VNIC_PORT_NO));
    }

    // remove the context switch ref count added for this work item
    if (pVNic->PortType != HELPER_PORT)
    {
        VNicRemoveCtxSRef(pVNic, REF_PENDING_OP_WORKITEM);
    }
    
    pVNic->fPendingOpsRunning = FALSE;

    NdisSetEvent(&pVNic->PendingOpWorkItemDoneEvent);

    // unlock before we leave
    VNicUnlock(pVNic);
}

VOID
VNic11CtxSToVNic(    
    _In_  PVNIC       pVNic,
    _In_  ULONG       ulFlags
    )
{
    BOOLEAN fLocked = FALSE, fConnected = FALSE;

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNic11CtxSToVNic called\n", VNIC_PORT_NO));

    do
    {
        VNicLock(pVNic);
        fLocked = TRUE;

        if (pVNic->fTerminating)
        {
            // can this happen?
            ASSERT(FALSE);
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): The VNIC is terminating. Simply returning the call\n", VNIC_PORT_NO));
            break;
        }
        
        VNicSetActive(pVNic, TRUE);

        // send a null packet to the AP with the power save bit not set, if we are currently 
        // connected to an infrastructure network
        if (pVNic->PortType == EXTSTA_PORT && (dot11_BSS_type_independent != Hw11QueryCurrentBSSType(pVNic->pvHwContext)))
        {
            fConnected = Hw11IsConnected(pVNic->pvHwContext);
            if (fConnected)
            {
                Hw11SendNullPkt(pVNic->pvHwContext, FALSE);
            }
        }
    
        if ( (VNIC_FLAG_HVL_ACTIVATED & ulFlags) && (pVNic->PortType == HELPER_PORT) && pVNic->fPendingOpsRunning)
        {
            /*
                This is an activation triggered by the helper port itself. The pending operation
                work item must be already running
                */
            ASSERT(pVNic->fPendingOpsRunning);

            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): The helper port is activating itself. Not launching the pending operations workitem\n", VNIC_PORT_NO));
            break;
        }

        // wait for any pending work items already running
        if (pVNic->fPendingOpsRunning)
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): A pending operations work item is already running. Waiting...\n", VNIC_PORT_NO));

            VNicUnlock(pVNic);
            NdisWaitEvent(&pVNic->PendingOpWorkItemDoneEvent, 0);
            VNicLock(pVNic);

            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Finished waiting for the pending operations work item \n", VNIC_PORT_NO));

            ASSERT(!pVNic->fPendingOpsRunning);
        }

        /*
            If the HVL gave us exclusive access set our internal state to reflect that. Note that
            this should be done only after any pending work items have completed so as to not
            confuse the already running work item thread
            */
        if (VNIC_FLAG_GRANTED_EX_ACCESS & ulFlags)
        {
            VNicSetExAccess(pVNic, TRUE);
        }
        
        /*
            IMPORTANT: Check the queues only after we have waited for pending work items to
            complete. This is because the pending work item will empty the queue before 
            returning
            */            
        if (!VNicIsPendingOpQueueEmpty(pVNic) || !PktQueueIsEmpty(&pVNic->TxQueue))
        {                
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Launching a work item for pending operations\n", VNIC_PORT_NO));

            // add a context switch ref count for the pending operations work item
            
            /*
                special casing for the helper port. The helper port can context switch out while 
                the pending operations work item is running
                */
            if (pVNic->PortType != HELPER_PORT)
            {
                VNicAddCtxSRef(pVNic, REF_PENDING_OP_WORKITEM);
            }
            
            // reset the done event before launching the work item
            NdisResetEvent(&pVNic->PendingOpWorkItemDoneEvent);
            pVNic->fPendingOpsRunning = TRUE;
            
            // schedule the work item for pending operations
            NdisQueueIoWorkItem(
                pVNic->PendingOpsWorkItemHandle,
                VNicPendingOpsWorkItem,
                pVNic
                );
        }
    } while (FALSE);

    if (fLocked)
    {
        VNicUnlock(pVNic);
    }
}

VOID
VNic11ProgramHw(    
    _In_  PVNIC      pVNic,
    _In_  ULONG       ulFlags
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(ulFlags);

    // activate the hardware context
    ndisStatus = Hw11EnableMACContext(pVNic->pvHw, pVNic->pvHwContext);
    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Hw11EnableMACContext failed %!x! \n", VNIC_PORT_NO, ndisStatus));
    }
}

NDIS_STATUS 
VNic11ReqExAccess(
    PVNIC pVNic,
    PORT11_GENERIC_CALLBACK_FUNC pCallbkFn,
    PVOID pvCtx,
    BOOLEAN fPnPOperation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC_EX_ACCESS_REQ pExReq = NULL;
    
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNic was asked to request exclusive access \n", VNIC_PORT_NO));

    do
    {
        ndisStatus = VNicAllocateExAccessReq(pVNic, pCallbkFn, pvCtx, fPnPOperation, &pExReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicAllocateExAccessReq failed %!x!\n", VNIC_PORT_NO, ndisStatus));
            break;
        }

        ndisStatus = VNicHandleExAccessOp(pVNic, PENDING_OP_EX_ACCESS_REQ, pExReq, FALSE, fPnPOperation);
        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicHandleExAccessOp called %!x!\n", VNIC_PORT_NO, ndisStatus));

        if (NDIS_STATUS_PENDING != ndisStatus)
        {
            VNicFreeExAccessReq(pVNic, pExReq);
        }

    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS 
VNicReleaseExAccess(
    PVNIC pVNic
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ASSERT(VNicIsLocked(pVNic));

    ndisStatus = Hvl11ReleaseExAccess(pVNic->pvHvl, pVNic);
    ASSERT(!NDIS_FAILURE(ndisStatus));
    
    if (NDIS_STATUS_SUCCESS == ndisStatus)
    {
        // the HVL has marked us as not having exclusive access
        VNicSetExAccess(pVNic, FALSE);
    }
    else if (NDIS_STATUS_PENDING == ndisStatus)
    {
        ndisStatus = NDIS_STATUS_SUCCESS;
    }

    return ndisStatus;
}


NDIS_STATUS 
VNic11ReleaseExAccess(
    PVNIC pVNic
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNic was asked to release exclusive access \n", VNIC_PORT_NO));

    VNicLock(pVNic);    
    ASSERT(VNicHasExAccess(pVNic));
    ndisStatus = VNicReleaseExAccess(pVNic);    
    VNicUnlock(pVNic);

    return ndisStatus;
}

BOOLEAN
VNic11AreCompatibleSignatures(
    PVNIC_SIGNATURE pSig1,
    PVNIC_SIGNATURE pSig2
    )
{
    BOOLEAN fCompatible = FALSE;

    do
    {
        if (pSig1->ulPhyId != pSig2->ulPhyId)
        {
            break;
        }

        if (pSig1->ulChannel != pSig2->ulChannel)
        {
            break;
        }

        /*
            the signatures can be incompatible only if both of them have non overlapping 
            default keys
            */
        if (pSig1->ucDefKeyMask & pSig2->ucDefKeyMask)
        {
            break;
        }

        fCompatible = TRUE;
    } while (FALSE);

//exitPt:    
    return fCompatible;
}

VNIC_SIGNATURE
VNic11GetSignature(
    PVNIC pVNic
    )
{
    VNIC_SIGNATURE sig = {0};
    PVOID pvHwMacCtx = pVNic->pvHwContext;

    sig.ulPhyId = Hw11QueryOperatingPhyId(pvHwMacCtx);
    sig.ulChannel = Hw11QueryCurrentChannel(pvHwMacCtx, FALSE);
    sig.ucDefKeyMask = Hw11QueryDefaultKeyMask(pvHwMacCtx);
    
    return sig;
}

/*
    Merge signature1 with signature2
    */
VNIC_SIGNATURE
VNic11MergeSignatures(
    PVNIC_SIGNATURE pSig1,
    PVNIC_SIGNATURE pSig2
    )
{
    VNIC_SIGNATURE mergedSig = {0};
    
    ASSERT(VNic11AreCompatibleSignatures(pSig1, pSig2));

    mergedSig.ulPhyId = pSig1->ulPhyId;
    mergedSig.ulChannel = pSig1->ulChannel;
    mergedSig.ucDefKeyMask = pSig1->ucDefKeyMask | pSig2->ucDefKeyMask;
    
    return mergedSig;
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID 
VNicCompleteExAccessReq(
    PVNIC pVNic,
    PVNIC_EX_ACCESS_REQ pExReq,
    BOOLEAN fCompleteReq
    )
{   
    PORT11_GENERIC_CALLBACK_FUNC pCallbkFn = pExReq->CallbkFn;
    PVOID pvCtx = pExReq->pvCtx;

    if (fCompleteReq)
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Completing the exclusive access request\n", VNIC_PORT_NO));

        /*
            If we are using one of the dedicated exclusive access requests, reset them now. As
            soon as we return the call, the Port can decide to make another exclusive access
            request
            */
        if (pExReq && VNicIsPreallocatedRequest(pVNic, pExReq))
        {
            VNicInitPreAllocatedOp(pVNic);
        }
        
        // unlock before calling into the port
        _Analysis_assume_lock_held_((& pVNic->Lock)->SpinLock);
        VNicUnlock(pVNic);
        pCallbkFn(pVNic->pvPort, pvCtx);
        VNicLock(pVNic);

        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d) Completed the exclusive access request.\n", VNIC_PORT_NO));
    }
}

VOID 
VNicCompleteResetReq(
    PVNIC pVNic,
    PRESET_REQ pResetReq,
    BOOLEAN fCompleteReq
    )
{   
    // we should not be in reset already
    ASSERT(!VNicIsInReset(pVNic));
    
    // set the reset flag so that no new operations can queue. This flag also makes sure that the
    // context does not switch out before we have completed our reset processing 
    // (i.e. VNic11Dot11ResetComplete is called)
    pVNic->fResetInProgress = TRUE;

    if (pVNic->CtxSRefCountTracker[REF_CTXS_BARRIER])
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Removing the connection start ref count\n", VNIC_PORT_NO));
        VNicRemoveCtxSRef(pVNic, REF_CTXS_BARRIER);
    }
    ASSERT(0 == pVNic->CtxSRefCountTracker[REF_CTXS_BARRIER]);

    if (fCompleteReq)
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Completing the reset request\n", VNIC_PORT_NO));

        VNicCompleteReq(pResetReq->pReq, NDIS_STATUS_SUCCESS);

        // the exclusive access is released after all reset processing is completed
    }    
}

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS 
VNicCompleteChSwReq(
    PVNIC pVNic,
    PCH_SWITCH_REQ pChSwReq,
    BOOLEAN fCompleteReq
    )
{   
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC_COMPLETION_CTX pCtx = NULL;

    do
    {
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): VNic is in reset. Completing a pending join request with failure\n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }

        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(VNIC_COMPLETION_CTX), &pCtx);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new completion context \n", VNIC_PORT_NO));
            break;
        }
        
        pCtx->CompletionFn = pChSwReq->pfnCompletionHandler;

        ndisStatus = VNicSwChHelper(
                    pVNic, 
                    pChSwReq->ulChannel, 
                    pChSwReq->ulPhyId, 
                    pChSwReq->fSwitchPhy,
                    pCtx
                    );
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicSwChHelper called %!x!\n", VNIC_PORT_NO, ndisStatus));

    } while (FALSE);
    
    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        if (fCompleteReq)
        {
            VNicChSwComplete(pVNic, FALSE, pChSwReq->pfnCompletionHandler, &ndisStatus);
        }
        
        if (pCtx)
        {
            FREE_MEM(pCtx);
        }
    }
    
    // always release the exclusive access we acquired for channel switch
    VNicReleaseExAccess(pVNic);

    return ndisStatus;
}

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS 
VNicCompleteDefKeyReq(
    PVNIC pVNic,
    PDEF_KEY_REQ pDefKeyReq,
    BOOLEAN fCompleteReq
    )
{   
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): VNic is in reset. Completing a pending join request with failure\n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }

        ndisStatus = VNicSetDefaultKeyHelper(
                        pVNic,
                        pDefKeyReq->MacAddr,
                        pDefKeyReq->KeyID,
                        pDefKeyReq->Persistent,
                        pDefKeyReq->AlgoId,
                        pDefKeyReq->KeyLength,
                        pDefKeyReq->KeyValue
                        );
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicSetDefaultKeyHelper called %!x!\n", VNIC_PORT_NO, ndisStatus));

        ASSERT(NDIS_STATUS_PENDING != ndisStatus);
    } while (FALSE);
    
    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        if (fCompleteReq)
        {
            // notify that the set default key request is complete
            NdisSetEvent(pDefKeyReq->CompletionEvent);
        }
    }

    // release the exclusive access we acquired for setting the default key
    VNicReleaseExAccess(pVNic);

    return ndisStatus;
}

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS 
VNicCompleteOpPhyIdReq(
    PVNIC pVNic,
    POPERATING_PHY_ID_REQ pPhyIdReq,
    BOOLEAN fCompleteReq
    )
{   
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): VNic is in reset. Completing a pending join request with failure\n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }

        ndisStatus = VNicSetOperatingPhyIdHelper(
                        pVNic,
                        pPhyIdReq->PhyId
                        );
        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicSetOperatingPhyIdHelper called %!x!\n", VNIC_PORT_NO, ndisStatus));

        ASSERT(NDIS_STATUS_PENDING != ndisStatus);

    } while (FALSE); 

    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        if (fCompleteReq)
        {
            // notify that the set phy id request is complete
            NdisSetEvent(pPhyIdReq->CompletionEvent);
        }
    }

    // release the exclusive access we acquired for setting the operating Phy Id
    VNicReleaseExAccess(pVNic);

    return ndisStatus;
}

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS 
VNicCompleteDesiredPhyIdReq(
    PVNIC pVNic,
    PDESIRED_PHY_ID_LIST_REQ pDesiredPhyIdListReq,
    BOOLEAN fCompleteReq
    )
{   
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): VNic is in reset. Completing a pending join request with failure\n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }

        ndisStatus = VNicSetDesiredPhyIdListHelper(
                        pVNic,
                        pDesiredPhyIdListReq->PhyIDList,
                        pDesiredPhyIdListReq->PhyIDCount
                        );
        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicSetDesiredPhyIdListHelper called %!x!\n", VNIC_PORT_NO, ndisStatus));

        ASSERT(NDIS_STATUS_PENDING != ndisStatus);
        
    } while (FALSE);
    
    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        if (fCompleteReq)
        {
            // notify that the set phy id request is complete
            NdisSetEvent(pDesiredPhyIdListReq->CompletionEvent);
        }        
    }

    // release the exclusive access we acquired for setting the operating Phy Id
    VNicReleaseExAccess(pVNic);

    return ndisStatus;
}


_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS 
VNicCompleteNicPowerStateReq(
    PVNIC pVNic,
    PNIC_POWER_STATE_REQ pNicPowerStateReq,
    BOOLEAN fCompleteReq
    )
{   
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        _Analysis_assume_lock_held_(pVNic->Lock.SpinLock);
        ndisStatus = VNicSetNicPowerStateHelper(
                        pVNic,
                        pNicPowerStateReq->PowerState,
                        pNicPowerStateReq->SelectedPhy
                        );
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicSetNicPowerStateHelper called %!x!\n", VNIC_PORT_NO, ndisStatus));

        ASSERT(NDIS_STATUS_PENDING != ndisStatus);
    } while (FALSE);
    
    if (NDIS_STATUS_PENDING != ndisStatus)
    {
        if (fCompleteReq)
        {
            // notify that the set power request is complete
            NdisSetEvent(pNicPowerStateReq->CompletionEvent);
        }
    }

    // release the exclusive access we acquired for setting the nic power state
    VNicReleaseExAccess(pVNic);

    return ndisStatus;
}


_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicCompleteExclusiveAccessOp(
    PVNIC pVNic,
    PENDING_OP_TYPE opType,
    PVOID pvOpData,
    BOOLEAN fCompleteReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ASSERT(VNicOperationRequiresExAccess(opType));
    
    switch (opType)
    {
        case PENDING_OP_EX_ACCESS_REQ:
        {
            VNicCompleteExAccessReq(pVNic, pvOpData, fCompleteReq);
            break;
        }
        
        case PENDING_OP_RESET_REQ:
        {
            VNicCompleteResetReq(pVNic, pvOpData, fCompleteReq);
            break;
        }

        case PENDING_OP_CH_SW_REQ:
        {
            ndisStatus = VNicCompleteChSwReq(pVNic, pvOpData, fCompleteReq);
            break;
        }

        case PENDING_OP_DEF_KEY:
        {
            ndisStatus = VNicCompleteDefKeyReq(pVNic, pvOpData, fCompleteReq);
            break;
        }
            
        case PENDING_OP_OPERATING_PHY_ID:
        {
            ndisStatus = VNicCompleteOpPhyIdReq(pVNic, pvOpData, fCompleteReq);
            break;
        }
            
        case PENDING_OP_DESIRED_PHY_ID_LIST:
        {
            ndisStatus = VNicCompleteDesiredPhyIdReq(pVNic, pvOpData, fCompleteReq);            
            break;
        }

        case PENDING_OP_NIC_POWER_STATE:
        {
            ndisStatus = VNicCompleteNicPowerStateReq(pVNic, pvOpData, fCompleteReq);            
            break;
        }
        
        default:
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            ASSERT(FALSE);
            break;
        }
    };

    return ndisStatus;
}

NDIS_STATUS
VNicHandleExAccessOp(
    PVNIC pVNic,
    PENDING_OP_TYPE opType,
    PVOID pvOpData,
    BOOLEAN fCheckForReset,
    BOOLEAN fPnPOperation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN fLocked = FALSE;
    BOOLEAN fQueueOp = FALSE;

    do
    {
        VNicLock(pVNic);
        fLocked = TRUE;

        if (fCheckForReset && VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) is in reset. Not doing anything for the exclusive access operation \n", VNIC_PORT_NO));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }        

        // request the Hvl to grant us exclusive access
        ndisStatus = Hvl11RequestExAccess(pVNic->pvHvl, pVNic, fPnPOperation);
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            /*
                According to the HVL we already have exclusive access. However we might 
                know it yet (the context switch callback hasn't yet reached us. Regardless 
                remember our most current state
                */
            VNicSetExAccess(pVNic, TRUE);
            // having exclusive access implies being active
            VNicSetActive(pVNic, TRUE);
            
            if (VNicIsPendingOpQueueEmpty(pVNic))
            {
                MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d) already has exclusive access. Completing the operation %s ourselves.\n", VNIC_PORT_NO, GetPendingOpString(opType)));

                // we already have exclusive access. simply complete the disruptive operation
                ndisStatus = VNicCompleteExclusiveAccessOp(pVNic, opType, pvOpData, FALSE);

                // the VNicCompleteExclusiveAccessOp function will release exclusive access. 
                // transfer the responsibility                
                break;
            }
            else
            {
                /*
                    we already have exclusive access but there are still pending items in our 
                    queue. We cannot comlpete this operation yet since that might be out of
                    order. Simply queue the operation as in the case of pending exclusive access
                    */
                MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d) already has exclusive access but the queue is not empty.\n", VNIC_PORT_NO));
                
                fQueueOp = TRUE;
            }
        }
        else if (NDIS_STATUS_PENDING == ndisStatus)
        {
            fQueueOp = TRUE;
        }
        else
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Hvl11RequestExAccess failed %!x!\n", VNIC_PORT_NO, ndisStatus));
            break;
        }
        
        ASSERT(fQueueOp);

        /*
            HVL will call us back with exclusive access or we will process it with other 
            pending items in our queue
            */
        ndisStatus = VNicQueuePendingOperation(pVNic, opType, pvOpData, fPnPOperation);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicQueuePendingOperation failed %!x!\n", VNIC_PORT_NO, ndisStatus));
            break;
        }

        // we should return a pending status to our caller since the operation will be completed later
        ndisStatus = NDIS_STATUS_PENDING;
        
        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d) Queued the operation %s .\n", VNIC_PORT_NO, GetPendingOpString(opType)));
    } while (FALSE);

    if (NDIS_FAILURE(ndisStatus))
    {
        if (fQueueOp)
        {
            // we wanted to queue the operation but hit a failure. Release the exclusive access
            // we requested. If fQueueOp is FALSE, we have transferred the responsibility to
            // release exclusive access
            Hvl11ReleaseExAccess(pVNic->pvHvl, pVNic);
        }
    }
    
    if (fLocked)
    {
        VNicUnlock(pVNic);
    }

    return ndisStatus;
}

VOID
VNic11Notify(
    PVNIC pVNic,
    PVOID pvNotif
)
{
    PNOTIFICATION_DATA_HEADER pNotifHdr = NULL;
    BOOLEAN fNotifyPort = FALSE;
    
    pNotifHdr = (PNOTIFICATION_DATA_HEADER)pvNotif;
    switch (pNotifHdr->Type)
    {
        case NotificationOpChannel:
        {
            POP_CHANNEL_NOTIFICATION pChNotif = (POP_CHANNEL_NOTIFICATION)pvNotif;

            VNicLock(pVNic);

            if (pChNotif->ulChannel == 0)
            {
                // invalidate the preferred channel only if this is the same VNIC whose preferred
                // channel we are using. 
                if (pVNic->pPreferredChannelSrcVNic && (pVNic->pPreferredChannelSrcVNic == pChNotif->Header.pSourceVNic))
                {
                    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) Received channel invalidation from VNic (%d). Invalidating preferred channel (%d) \n", VNIC_PORT_NO, pVNic->pPreferredChannelSrcVNic->PortNumber, pVNic->ulPreferredChannel));
                    pVNic->ulPreferredChannel = 0;
                    pVNic->pPreferredChannelSrcVNic = NULL;
                }
                else
                {
                    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) Received channel invalidation from VNic (%d). However our source VNIC is different. Ignoring update.  \n", VNIC_PORT_NO, pChNotif->Header.pSourceVNic->PortNumber));
                }
            }
            else
            {
                MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) updating preferred channel: %d ----> %d\n", VNIC_PORT_NO, pVNic->ulPreferredChannel, pChNotif->ulChannel));
                pVNic->ulPreferredChannel = pChNotif->ulChannel;
                pVNic->pPreferredChannelSrcVNic = pChNotif->Header.pSourceVNic;

                fNotifyPort = TRUE;
            }
            
            VNicUnlock(pVNic);

            // also notify the port
            if (fNotifyPort)
            {
                Port11Notify(pVNic->pvPort, pvNotif);
            }
        }
        break;

        case NotificationOpLinkState:
        case NotificationOpRateChange:
        {
            // Notify the port. We dont maintain this state
            Port11Notify(pVNic->pvPort, pvNotif);
            break;
        }

        default:
        {
            // not handled
            ASSERT(FALSE);
        }
        break;
    };
}

ULONG
VNic11QueryPreferredChannel(
    _In_  PVNIC               pVNic,
    _Out_ PBOOLEAN            pPreferredChannel
    )
{
    *pPreferredChannel = (pVNic->ulPreferredChannel != 0);
    return pVNic->ulPreferredChannel ? pVNic->ulPreferredChannel : Hw11QueryCurrentChannel(pVNic->pvHwContext, FALSE);
}

VOID
VNic11AcquireCtxSBarrier(
    PVNIC pVNic
    )
{
    BOOLEAN fProcessReqNow = FALSE;

    VNicLock(pVNic);
    
    fProcessReqNow = VNicIsActive(pVNic) && VNicIsPendingOpQueueEmpty(pVNic);
    if (fProcessReqNow)
    {
        VNicAddCtxSRef(pVNic, REF_CTXS_BARRIER);
    }
    else
    {
        VNicQueueConnectionStart(pVNic);
    }
    
    VNicUnlock(pVNic);
}

VOID
VNic11ReleaseCtxSBarrier(
    PVNIC pVNic
    )
{
    BOOLEAN fPendingConnStartDeleted = FALSE;    

    VNicLock(pVNic);

    VNicDeletePendingConnStartRequest(pVNic, &fPendingConnStartDeleted);
    if (fPendingConnStartDeleted)
    {
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Removed a pending connection start request\n", VNIC_PORT_NO));
    }
    
    if (pVNic->CtxSRefCountTracker[REF_CTXS_BARRIER])
    {
        ASSERT(!fPendingConnStartDeleted);
        
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Removing the connection start ref count\n", VNIC_PORT_NO));
        VNicRemoveCtxSRef(pVNic, REF_CTXS_BARRIER);
        ASSERT(0 == pVNic->CtxSRefCountTracker[REF_CTXS_BARRIER]);
    }

    VNicUnlock(pVNic);
}

VOID
VNicSendChannelNotification(
    PVNIC pVNic,
    ULONG ulChannel
    )
{
    OP_CHANNEL_NOTIFICATION ChNotif = {0};
    
    /*
        We have successfully connected. Notify the HVL about our channel. Other 
        VNICs can use this information to optimize e.g. an AP can decide to move to
        the same channel as us
        */
    ChNotif.Header.pSourceVNic = pVNic;
    ChNotif.Header.Type = NotificationOpChannel;
    ChNotif.Header.Size = sizeof(OP_CHANNEL_NOTIFICATION);
    ChNotif.ulChannel = ulChannel;

    Hvl11Notify(pVNic->pvHvl, &ChNotif);            
}

VOID
VNicSendLinkStateNotification(
    IN  PVNIC                   pVNic,
    IN  BOOLEAN                 MediaConnected
    )
{

    OP_LINK_STATE_NOTIFICATION MediaNotif = {0};

    // We pass the notification through only if it has
    // changed
    if (pVNic->LastLinkStateNotified != MediaConnected)
    {
        pVNic->LastLinkStateNotified = MediaConnected;
        
        MediaNotif.Header.pSourceVNic = pVNic;
        MediaNotif.Header.Type = NotificationOpLinkState;
        MediaNotif.Header.Size = sizeof(OP_LINK_STATE_NOTIFICATION);
        MediaNotif.MediaConnected = MediaConnected;
        Hvl11Notify(pVNic->pvHvl, &MediaNotif);            
    }
}
