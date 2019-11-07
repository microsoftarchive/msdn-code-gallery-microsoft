/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hvl_main.c

Abstract:
    Implements initialization/PNP routines for the HVL
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "Hvl_main.tmh"
#endif

#define HVL_DEFAULT_CONTEXT_SWITCH_PARK_TIME_MSEC   (200 * 1000 * 10) // 200 milliseconds

_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_raises_(DISPATCH_LEVEL)
_At_((pHvl->Lock).OldIrql, _IRQL_saves_)
_Requires_lock_not_held_((pHvl->Lock).SpinLock)
_Acquires_lock_((pHvl->Lock).SpinLock)
VOID
HvlLock(
    _In_  PHVL       pHvl
    )
{
    ACQUIRE_LOCK(pHvl, FALSE);                                               
}


_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_min_(DISPATCH_LEVEL)
_At_((pHvl->Lock).OldIrql, _IRQL_restores_)
_Requires_lock_held_((pHvl->Lock).SpinLock)
_Releases_lock_((pHvl->Lock).SpinLock)
VOID
HvlUnlock(
    _Inout_  PHVL      pHvl
    )
{
   #pragma prefast(disable : 6001)

   RELEASE_LOCK(pHvl, FALSE);

   #pragma prefast(enable: 6001)
}
  
BOOLEAN
HvlIsLocked(    
    _In_  PHVL      pHvl
    )
{
    return pHvl->fLocked;
}

VOID
Hvl11BlockTimedCtxS(_Inout_ PHVL pHvl)
{
    HvlLock(pHvl);
    pHvl->ulStatusFlags |= HVL_TIMED_CTXS_BLOCKED;
    HvlUnlock(pHvl);
}

VOID
Hvl11UnblockTimedCtxS(_Inout_ PHVL pHvl)
{
    HvlLock(pHvl);
    pHvl->ulStatusFlags &= ~HVL_TIMED_CTXS_BLOCKED;
    HvlUnlock(pHvl);
}

__inline
BOOLEAN
HvlTimedCtxSBlocked(_In_ PHVL pHvl)
{
    ASSERT(HvlIsLocked(pHvl));
    
    return ((pHvl->ulStatusFlags & HVL_TIMED_CTXS_BLOCKED) ? TRUE : FALSE);
}

__inline
VOID
HvlSetCtxSInProgress(_Inout_ PHVL pHvl)
{
    ASSERT(HvlIsLocked(pHvl));
    pHvl->ulStatusFlags |= HVL_CTXS_IN_PROGRESS;
}

__inline
VOID
HvlCtxSDone(_In_ PHVL pHvl)
{
    ASSERT(HvlIsLocked(pHvl));
    pHvl->ulStatusFlags &= ~HVL_CTXS_IN_PROGRESS;
}

extern
__inline
BOOLEAN
HvlIsCtxSInProgress(_In_ PHVL pHvl)
{
    ASSERT(HvlIsLocked(pHvl));
    return ((pHvl->ulStatusFlags & HVL_CTXS_IN_PROGRESS) ? TRUE : FALSE);
}

_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_raises_(DISPATCH_LEVEL)
_At_((pHvl->Lock).OldIrql, _IRQL_saves_)
_Requires_lock_not_held_((pHvl->Lock).SpinLock)
_Acquires_lock_((pHvl->Lock).SpinLock)
VOID
HvlWaitForCtxSProcessingAndLock(_In_ PHVL pHvl)
{
    HvlLock(pHvl);
    while (HvlIsCtxSInProgress(pHvl))
    {
        HvlUnlock(pHvl);
        NdisMSleep(10 * 1000);  // 10 msec
        HvlLock(pHvl);
    }
}

__inline
BOOLEAN
HvlIsExAccessGranted(PHVL pHvl)
{
    ASSERT(HvlIsLocked(pHvl));

    return ((NULL != pHvl->pExAccessVNic) ? TRUE : FALSE);
}

__inline
VOID    
HvlGrantExAccess(PHVL pHvl, PVNIC pVNic)
{
    ASSERT(HvlIsLocked(pHvl));

    pHvl->pExAccessVNic = pVNic;   

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) was granted exclusive access\n", VNIC_PORT_NO));
}

extern
__inline
PVNIC    
HvlGetExAccessVNic(PHVL pHvl)
{
    PVNIC pVNic = NULL;
    ASSERT(HvlIsLocked(pHvl));

    pVNic = pHvl->pExAccessVNic;

    return pVNic;
}

__inline
VOID    
HvlReleaseExAccess(PHVL pHvl, PVNIC pVNic)
{
    ASSERT(HvlIsLocked(pHvl));

    ASSERT(pVNic == HvlGetExAccessVNic(pHvl));
    
    pHvl->pExAccessVNic = NULL;   

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) released exclusive access\n", VNIC_PORT_NO));
}

__inline
BOOLEAN
HvlIsPreAllocatedRequest(
    PHVL pHvl, 
    PHVL_EX_ACCESS_REQ pExReq
    )
{
    return (pExReq == pHvl->pPnpOpExReq);
}

__inline
BOOLEAN
HvlIsPreAllocatedPendingOp(
    PHVL pHvl, 
    PHVL_PENDING_OP pPendingOp
    )
{
    return (pPendingOp == &pHvl->PnpPendingOp);
}

__inline
BOOLEAN
HvlIsHelperVNic(PHVL pHvl, PVNIC pVNic)    
{
    return (pHvl->pHelperPortCtx == pVNic->pHvlCtx);
}


NDIS_STATUS
Hvl11Allocate(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_maybenull_ PHVL*         ppHvl,
    _In_  PADAPTER                pAdapter
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHVL pHvl = NULL;
    NDIS_HANDLE ctxSWorkItemHandle = NULL, notificationsWorkItemHandle = NULL;
    BOOLEAN fFreeCtxSWorkItemHandle = FALSE, fFreeNotifWorkItemHandle = FALSE;
    PHVL_EX_ACCESS_REQ pExReq = NULL;

    ASSERT(MiniportAdapterHandle && ppHvl && pAdapter);
    
    *ppHvl = NULL;

    do
    {
        ndisStatus = ALLOC_MEM(MiniportAdapterHandle, sizeof(HVL), &pHvl);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for a new HVL"));
            break;
        }

        // the list heads should be the first ones to be initialized. This allows us to free things 
        // correctly if Free is called without Initialize being called in between        
        InitializeListHead(&pHvl->VNiclist);
        InitializeListHead(&pHvl->InactiveContextList);
        InitializeListHead(&pHvl->PendingOpQueue);
        InitializeListHead(&pHvl->NotificationsQueue);        

        // Allocate memory for fields inside the HVL structure
        
        // pre-allocate the exclusive access request structure for PnP related exclusive accesses        
        ndisStatus = ALLOC_MEM(MiniportAdapterHandle, sizeof(HVL_EX_ACCESS_REQ), &pExReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for exclusive access request"));
            break;
        }

        NdisAllocateSpinLock(&(pHvl->Lock));

        // Allocate the context switch work item 
        ctxSWorkItemHandle = NdisAllocateIoWorkItem(MiniportAdapterHandle);
        if(NULL == ctxSWorkItemHandle)
        {
            MpTrace (COMP_HVL, DBG_SERIOUS, ("NdisAllocateIoWorkItem failed"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        fFreeCtxSWorkItemHandle = TRUE;
        
        notificationsWorkItemHandle = NdisAllocateIoWorkItem(MiniportAdapterHandle);
        if(NULL == notificationsWorkItemHandle)
        {
            MpTrace (COMP_HVL, DBG_SERIOUS, ("NdisAllocateIoWorkItem failed"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        fFreeNotifWorkItemHandle = TRUE;
        
        pHvl->MiniportAdapterHandle = MiniportAdapterHandle;
        pHvl->Adapter = pAdapter;
        
        pHvl->CtxSWorkItemHandle = ctxSWorkItemHandle;
        pHvl->NotificationsWorkItemHandle = notificationsWorkItemHandle;

        pHvl->fVirtualizationEnabled = TRUE;    
        pHvl->pPnpOpExReq = pExReq;

        *ppHvl = pHvl;
    }while (FALSE);
    
    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (fFreeNotifWorkItemHandle)
        {
            NdisFreeIoWorkItem(notificationsWorkItemHandle);
        }
        if (fFreeCtxSWorkItemHandle)
        {
            NdisFreeIoWorkItem(ctxSWorkItemHandle);
        }
        if (pExReq)
        {
            FREE_MEM(pExReq);
        }
        if (pHvl)
        {
            FREE_MEM(pHvl);
        }
    }    

    return ndisStatus;
}

VOID
HvlWaitForPendingThreads(    
    _In_  PHVL pHvl
    )
{
    ULONG ulNumIter = 0;
    BOOLEAN fThreadsFinished = FALSE;

    /*
        Wait for any pending threads to complete before freeing stuff
        */
    while (!fThreadsFinished)
    {
        if ( ++ulNumIter > 30000)
        {
            // 30 seconds have passed - something must be wrong
            ASSERTMSG("Hvl pending thread count hasn't gone to 0 in last 30 seconds. Check driver state\n", FALSE);
        }
        
        HvlLock(pHvl);
        if (pHvl->ulNumThreadsPending == 0)
        {
            fThreadsFinished = TRUE;
        }
        HvlUnlock(pHvl);
        if (!fThreadsFinished)
        {
            NdisMSleep(1000);
        }
    } 
}

VOID
Hvl11Free(
    _In_ _Post_ptr_invalid_  PHVL pHvl
    )
{
    if (NULL != pHvl)
    {
        HvlWaitForPendingThreads(pHvl);
        
        NdisFreeSpinLock(&(pHvl->Lock));
        
        if (pHvl->CtxSWorkItemHandle)
        {
            NdisFreeIoWorkItem(pHvl->CtxSWorkItemHandle);
        }
        
        if (pHvl->NotificationsWorkItemHandle)
        {
            NdisFreeIoWorkItem(pHvl->NotificationsWorkItemHandle);
        }

        if (pHvl->pPnpOpExReq)
        {
            FREE_MEM(pHvl->pPnpOpExReq);
        }

        // free the memory allocated for any pending operations
        HvlDeleteAllPendingOperations(pHvl);
        
        FREE_MEM(pHvl);
    }
}

NDIS_STATUS
Hvl11Initialize(
    _In_  PHVL                    pHvl,
    _In_  PHW                     Hw
    )
{
    ULONG i = 0;
    
    MpTrace(COMP_HVL, DBG_NORMAL, ("Hvl11Initialize called\n"));

    pHvl->Hw = Hw;

    pHvl->ulNumThreadsPending = 0;
    
    pHvl->pActiveContext = NULL;
    pHvl->pHelperPortCtx = NULL;
    pHvl->pExAccessVNic = NULL;
    pHvl->pExAccessDelegatedVNic = NULL;
    pHvl->ulStatusFlags = 0;
    pHvl->ulNumPortCtxs = 0;
    pHvl->fNotificationsWorkItemRunning = 0;
    
    HvlInitPreAllocatedOp(pHvl);
    
    // initialize all the pre-allocated contexts
    for (i = 0; i < HVL_NUM_CONTEXTS; i++)
    {
        HvlinitContext(&(pHvl->HvlContexts[i]), FALSE);
    }
    
    KeInitializeEvent(&pHvl->CtxSEvent, SynchronizationEvent , FALSE); // auto-reset event
    KeInitializeEvent(&pHvl->TerminatingEvent, NotificationEvent, FALSE); // manual reset event
    KeInitializeEvent(&pHvl->ExAccessEvent, SynchronizationEvent, FALSE); // auto-reset event

    HvlClearCachedNotification(pHvl);
    
    if (pHvl->fVirtualizationEnabled)
    {
        NdisQueueIoWorkItem(
            pHvl->CtxSWorkItemHandle,
            HvlCtxSWorkItem,
            pHvl
            );

        // increment the variable that keeps track of pending threads
        pHvl->ulNumThreadsPending++;

        // run the context switch logic for the first time
        KeSetEvent(&pHvl->CtxSEvent, 0, FALSE);
    }
    
    return NDIS_STATUS_SUCCESS;
}

VOID
Hvl11Terminate(
    _In_  PHVL                    pHvl
    )
{
    MpTrace(COMP_HVL, DBG_NORMAL, ("Hvl11Terminate called\n"));
    KeSetEvent(&pHvl->TerminatingEvent, 0, FALSE);
}

VOID
Hvl11EnableContextSwitches(
    _In_  PHVL                    pHvl
    )
{
    HvlLock(pHvl);

    if (!pHvl->fVirtualizationEnabled)
    {
        pHvl->fVirtualizationEnabled = TRUE;

        NdisQueueIoWorkItem(
            pHvl->CtxSWorkItemHandle,
            HvlCtxSWorkItem,
            pHvl
            );

        // run the context switch logic for the first time
        KeSetEvent(&pHvl->CtxSEvent, 0, FALSE);
    }
    
    HvlUnlock(pHvl);
}

NDIS_STATUS
Hvl11RegisterVNic(
    _In_  PHVL    pHvl,
    _In_  PVNIC   pVNic               
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHVL_CONTEXT pCtx = NULL;
    
    do
    {
        HvlLock(pHvl);

        // add it to the HVL's list of VNICs
        InsertTailList(&pHvl->VNiclist, &pVNic->VNicLink);
        
        HvlAssignVNicToContext(pHvl, pVNic, &pCtx);
        
        /*
            Initially make it part of the inactive context list. It will be picked up whenever we
            next context switch to it
            */
        InsertTailList(&pHvl->InactiveContextList, &pCtx->Link);

        HvlUnlock(pHvl);

        // notify it of any existing cached notifications
        if (pHvl->CachedChannelNotification.Header.pSourceVNic)
        {
            VNic11Notify(pVNic, &pHvl->CachedChannelNotification);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
Hvl11DeregisterVNic(
    _In_  PHVL    pHvl,
    _In_  PVNIC   pVNic               
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    HvlLock(pHvl);

    HvlRemoveVNicFromCtx(pHvl, pVNic);

    // remove it from the list of VNICs that the HVL maintains
    RemoveEntryList (&pVNic->VNicLink);
    InitializeListHead(&pVNic->VNicLink);

    // if there was a cached notification sourced by this VNIC clear it now
    if (pHvl->CachedChannelNotification.Header.pSourceVNic == pVNic)
    {
        HvlClearCachedNotification(pHvl);
    }
    
    HvlUnlock(pHvl);
    
    return ndisStatus;
}

NDIS_STATUS
Hvl11RegisterHelperPort(
    _In_  PHVL    pHvl,
    _In_  PVNIC   pVNic               
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHVL_CONTEXT pCtx = NULL;

    do
    {
        HvlLock(pHvl);

        HvlAssignVNicToContext(pHvl, pVNic, &pCtx);
        pHvl->pHelperPortCtx = pCtx;
        
        HvlUnlock(pHvl);
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
Hvl11DeregisterHelperPort(
    _In_  PHVL    pHvl,
    _In_  PVNIC   pVNic
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(pVNic);

    ASSERT(HvlIsHelperVNic(pHvl, pVNic));
    
    HvlLock(pHvl);

    HvlRemoveVNicFromCtx(pHvl, pVNic);

    HvlUnlock(pHvl);
    
    return ndisStatus;
}

/*
    Private API call provided to the helper port. 
    */
VOID
Hvl11ActivatePort(
    _In_  PHVL                    pHvl,
    _In_  PVNIC                   pVNic
    )
{
    PVNIC pHelperVNic = NULL;
    LIST_ENTRY *pHelperEntry = NULL;
        
    HvlWaitForCtxSProcessingAndLock(pHvl);

    /*
        Splitting the VNIC into a separate context can result in a different context pointer in the 
        VNIC. Hence acquire the pointer only after splitting is done
        */
    HvlPerformCtxSplit(pHvl, pVNic);

    pHelperEntry = pHvl->pHelperPortCtx->VNicList.Flink;
    pHelperVNic = CONTAINING_RECORD(pHelperEntry, VNIC, CtxLink);
    
    // The helper port VNIC must obtain exclusive access before calling Hvl11ActivatePort
    ASSERT(pHelperVNic == pHvl->pExAccessVNic);

    MpTrace(COMP_HVL, DBG_NORMAL, ("Helper port requested to activate VNic(%d)\n", VNIC_PORT_NO));
    
    HvlCtxSProcessing(
        pHvl, 
        pVNic->pHvlCtx, 
        FALSE,
        VNIC_FLAG_GRANTED_EX_ACCESS | VNIC_FLAG_EX_ACCESS_HVL_TRIGGERED | VNIC_FLAG_HVL_ACTIVATED);
    HvlUnlock(pHvl);
}

NDIS_STATUS
Hvl11RequestExAccess(
    _In_ PHVL pHvl, 
    _In_ PVNIC pVNic,
    _In_ BOOLEAN fPnPOperation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN fLocked = FALSE;
    
    do
    {
        HvlLock(pHvl);
        fLocked = TRUE;
        
        ndisStatus = HvlQueueExAccessRequest(pHvl, pVNic, fPnPOperation);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("HvlQueueExAccessRequest failed 0x%x", ndisStatus));
            break;
        }

        // now determine the return status to be given to the VNIC. 
        if (pHvl->pExAccessDelegatedVNic)
        {            
            // if the helper port has delegated exclusive access to some VNIC, new request 
            // by only that VNIC can succeed. Everyone else, including the helper port, should
            // wait
            if (pHvl->pExAccessDelegatedVNic == pVNic)
            {
                MpTrace(COMP_HVL, DBG_NORMAL, ("VNic (%d) has already been delegated exclusive access. Completing its request for exclusive access inline. \n", VNIC_PORT_NO));
                ndisStatus = NDIS_STATUS_SUCCESS;
            }
            else
            {
                ndisStatus = NDIS_STATUS_PENDING;
                MpTrace(COMP_HVL, DBG_NORMAL, ("VNic (%d) has been delegated exclusive access. Pending VNic(%d)'s request for exclusive access.\n", pHvl->pExAccessDelegatedVNic->PortNumber, VNIC_PORT_NO));
            }
        }
        else
        {
            if (HvlGetExAccessVNic(pHvl) == pVNic)
            {
                if (HvlIsHelperVNic(pHvl, pVNic))
                {
                    // helper port never gets exclusive access inline
                    MpTrace(COMP_HVL, DBG_NORMAL, ("Helper VNic (%d) has exclusive access. Still pending VNic(%d)'s exclusive access request \n", pHvl->pExAccessVNic->PortNumber, VNIC_PORT_NO));
                    ndisStatus = NDIS_STATUS_PENDING;
                }
                else
                {
                    // let the VNIC know that it already has exclusive access
                    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic (%d) already has exclusive access. Completing its request for exclusive access inline. \n", VNIC_PORT_NO));
                    ndisStatus = NDIS_STATUS_SUCCESS;
                }
            }
            else if (HvlGetExAccessVNic(pHvl))
            {
                // the VNIC will get exclusive access later
                MpTrace(COMP_HVL, DBG_NORMAL, ("VNic (%d) has exclusive access. Pending VNic(%d)'s exclusive access request \n", pHvl->pExAccessVNic->PortNumber, VNIC_PORT_NO));
                ndisStatus = NDIS_STATUS_PENDING;
            }
            else
            {
                // the VNIC will get exclusive access later
                MpTrace(COMP_HVL, DBG_NORMAL, ("No VNic has exclusive access. Pending VNic(%d)'s exclusive access request \n", VNIC_PORT_NO));
                ndisStatus = NDIS_STATUS_PENDING;
            }
        }
    } while (FALSE);

    if (fLocked)
    {
        HvlUnlock(pHvl);
    }
    
    if (NDIS_STATUS_PENDING == ndisStatus)
    {
        KeSetEvent(&pHvl->ExAccessEvent, 0, FALSE);
    }
    
    return ndisStatus;
}

NDIS_STATUS
Hvl11ReleaseExAccess(PHVL pHvl, PVNIC pVNic)
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC pExVNic = NULL;
    PHVL_EX_ACCESS_REQ pExReq = NULL;
    
    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) releasing exclusive access\n", VNIC_PORT_NO));

    HvlLock(pHvl);

    pExReq = HvlDequeueNextReqForVNic(pHvl, pVNic);
    
    if (pExReq && 0 == pExReq->ulRefCount)
    {
        /*
            This is the last release by this VNIC. Actually take away exclusive access if this is 
            the current owner of exclusive access
            */
        pExVNic = HvlGetExAccessVNic(pHvl);
        
        if (pExVNic == pVNic)
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) currently owns exclusive access. Giving it up and removing it from the queue\n", VNIC_PORT_NO));
            
            HvlReleaseExAccess(pHvl, pVNic);    

            // set the event so that some other VNIC waiting for access can now get it
            KeSetEvent(&pHvl->ExAccessEvent, 0, FALSE);   
        }
        else
        {
            /*
                The VNIC is releasing exclusive access even when it doesn't own it. This can
                happen - take for example the following
                1. There is a helper port and an extensible station port
                2. A Pause is happening and the helper port has exclusive access.
                3. The station asks its VNIC to perform a dot11 reset
                4. The station's VNIC requests exclusive access. Since the helper port has it, its request is queued
                5. The helper port activates the station as part of its Pause processing
                6. The station VNIC's CtxSToVNic is called and the VNIC learns it has exclusive access
                7. The station VNIC completes its reset operation and asks Hvl to release exclusive access
                8. The calls comes into the HVL and we would land up in this else clause.

                The correct thing to do here is to simply remove the station VNIC's request from
                the queue which is what we have done here
                */
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) doesn't own exclusive access currently. Just removing it from the queue\n", VNIC_PORT_NO));

            if (NULL == pExVNic)
            {
                //no one has exclusive access right now. set the event so that some other 
                //VNIC waiting for access can now get it
                KeSetEvent(&pHvl->ExAccessEvent, 0, FALSE);
            }
        }

        // delete the request
        if (HvlIsPreAllocatedRequest(pHvl, pExReq))
        {
            // it is a pre-allocated request - simply reset it
            HvlInitPreAllocatedOp(pHvl);
        }
        else
        {
            HvlDeleteExAccessRequest(pHvl, pExReq);        
        }
    }
    else
    {
        ASSERT(!HvlIsHelperVNic(pHvl, pVNic));
        
        /*
            This VNIC has more requests for exclusive access pending
            */
        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d) has %d more exclusive access requests. Decremented the refcount. Not removing it from the queue\n", VNIC_PORT_NO, pExReq ? pExReq->ulRefCount : -1));
        ndisStatus = NDIS_STATUS_PENDING;        
    }
    
    HvlUnlock(pHvl);

    return ndisStatus;
}


VOID 
Hvl11SendCompletePackets(
    _In_  PVNIC                   VNic,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   ulNumPkts,
    _In_  ULONG                   SendCompleteFlags
    )
{
    VNic11SendCompletePackets(VNic, PacketList, ulNumPkts, SendCompleteFlags);
}

VOID
Hvl11IndicateReceivePackets(
    _In_  PVNIC                   VNic,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    )
{
    VNic11IndicateReceivePackets(VNic, PacketList, ReceiveFlags);
}

VOID
Hvl11IndicateStatus(
    _In_  PVNIC                   VNic,
    _In_  NDIS_STATUS             StatusCode,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    )
{
    VNic11IndicateStatus(VNic, StatusCode, StatusBuffer, StatusBufferSize);
}

VOID
HvlProgramHw(PHVL pHvl, PHVL_CONTEXT pActiveCtx)
{
    UNREFERENCED_PARAMETER(pHvl);
    UNREFERENCED_PARAMETER(pActiveCtx);
}

/*
    This routine does that job of swapping contexts and calling appropriate functions in the VNICs
    to notify them. It leaves the Hvl lock before making calls into the VNIC. It is ok to do so because
    we guarantee that there is only one thread that can be executing this function at any given
    time. This guarantee is needed to ensure that the context lists in the HVL data structure do not
    get modified while this function is being executed. This function is entered when
    a. a timed context switch needs to happen
    b. an exclusive access is being granted to a VNIC
    c. Helper port calls HvlActivatePort

    Both a. and b. happen through the wait on events in HvlCtxSwitchWorkItem. This guarantees
    that only one of these paths would be executing this function. Also if a VNIC has exclusive
    access, timed context switches are not performed. Similarly HvlActivatePort is called only when
    the helper port has exclusive access
    */
_Requires_lock_held_(pHvl->Lock.SpinLock)
_IRQL_requires_(DISPATCH_LEVEL)
VOID
HvlCtxSProcessing(
    PHVL pHvl,
    PHVL_CONTEXT pNextActiveContext,
    BOOLEAN fMerged,
    ULONG        ulFlags
    )
{
    //NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHVL_CONTEXT pCurrCtx = NULL, pNextCtx = NULL;
    BOOLEAN fSameCtx = FALSE;
    LIST_ENTRY *pEntryVNic = NULL;
    PVNIC pVNic = NULL;
    PVNIC pHelperVNic = NULL;
    LIST_ENTRY *pHelperEntry = NULL;

    ASSERT(HvlIsLocked(pHvl));

    // we can have only one instance of this routine running
    ASSERT(!HvlIsCtxSInProgress(pHvl));

    HvlSetCtxSInProgress(pHvl);
    
    do
    {        
        // get the current active context
        pCurrCtx = pHvl->pActiveContext;

        if (NULL == pNextActiveContext)
        {
            // Use our logic to find the next active context
            pNextCtx = HvlFindNextCtx(pHvl);
        }
        else
        {
            // the caller specified a particular context to switch to
            pNextCtx = pNextActiveContext;
        }
        
        if (pCurrCtx && pNextCtx == pCurrCtx)
        {
            if (!fMerged)
            {
                /*
                    No context switch needs to happen.
                    */
                MpTrace(COMP_HVL, DBG_NORMAL, ("The next context (%p) is the same as the current one. Not doing a context switch\n", pNextCtx));
                #if DBG
                    pEntryVNic = pNextCtx->VNicList.Flink;
                    while (pEntryVNic != &pNextCtx->VNicList)
                    {
                        pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);
                        ASSERT(pVNic->fActive);
                        pEntryVNic = pEntryVNic->Flink;
                    }
                #endif                
                break;
            }
            else
            {
                MpTrace(COMP_HVL, DBG_NORMAL, ("The next context (%p) is the same as the current one. But we have just done a merge. Continuing with the context switch\n", pNextCtx));
                /*
                    remembe that our current and next contexts are the same. We need to do 
                    this so that we do not update the current context below
                    */
                fSameCtx = TRUE;
            }
        }

        if (NULL == pNextCtx)
        {
            /*
                No context switch needs to happen.
                */
            MpTrace(COMP_HVL, DBG_NORMAL, ("No contexts exist. Not doing a context switch\n"));
            break;
        }

        /*
            Now update the Hvl's state about which VNIC is loosing exclusive access. It
            is important to update this before VNic11CtxSFromVNic call has been made. This 
            ensures that the VNIC does not gain access while it is being moved away from
            active context
            */
        if (VNIC_FLAG_HVL_ACTIVATED & ulFlags)
        {
            // this is an activation by the helper port

            ASSERT(pNextActiveContext);
            ASSERT(pNextActiveContext->ulNumVNics == 1);

            if (NULL == pNextActiveContext)
            {
                return;
            }

            pEntryVNic = pNextActiveContext->VNicList.Flink;
            pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);

            pHelperEntry = pHvl->pHelperPortCtx->VNicList.Flink;
            pHelperVNic = CONTAINING_RECORD(pHelperEntry, VNIC, CtxLink);    

            // Helper port is delegating exclusive access to someone else. Take away the delegated access
            if (pHvl->pExAccessDelegatedVNic && pVNic != pHvl->pExAccessDelegatedVNic )
            {
                pHvl->pExAccessDelegatedVNic = NULL;
            }
        }

        // inform the active VNICs that they are going to be become inactive

        if (NULL != pCurrCtx)
        {
            HvlNotifyAllVNicsInContext(pHvl, pCurrCtx, VNic11CtxSFromVNic, 0);
        }
        
        /*
            Tell the hardware layer that we are now going to context switch. Note that this has
            to happen only after the VNICs have been told they are becoming inactive. The
            reason for this is that receives should continue to happen while the VNICs perform
            the tasks they need to become inactive (e.g. send PS packet to the AP)

            Also note that we need to give up our lock before we call into the hardware. This is
            because the Hw might need to wait for some operations to complete before it can
            return this call. Since the context switch processing runs in a single thread, we can
            safely leave the lock and re-acquire it
            */
        HvlUnlock(pHvl);                    
        Hw11CtxSStart(pHvl->Hw);
        HvlLock(pHvl);

        // ask the VNICs in the new context to program the hardware
        HvlNotifyAllVNicsInContext(pHvl, pNextCtx, VNic11ProgramHw, 0);

        /*
            program the hardware for the new context - this programs any settings that need
            to be merged across the VNICs
            */
        HvlProgramHw(pHvl, pNextCtx);        

        /*
            Now update the Hvl's state about which VNIC is active or has exclusive access. It
            is important to update this after the VNic11ProgramHw and HvlProgramHw calls
            have been made. This ensures that the VNIC does not gain access prematurely
            */
        if (VNIC_FLAG_HVL_ACTIVATED & ulFlags)
        {
            // this is an activation by the helper port

            // pHelperEntry and pHelperVNic must have been already calculated above
            ASSERT(pHelperEntry && pHelperVNic);
                    
            if (pVNic != pHelperVNic)
            {
                // Some VNIC is being delegated exclusive access
                pHvl->pExAccessDelegatedVNic = pVNic;
            }    
        }
        else if (VNIC_FLAG_GRANTED_EX_ACCESS & ulFlags)
        {
            // a VNIC is being given exclusive access. Update our state to reflect this
            ASSERT(pNextActiveContext);
            ASSERT(pNextActiveContext->ulNumVNics == 1);

            if (NULL == pNextActiveContext)
            {
                return;
            }

            pEntryVNic = pNextActiveContext->VNicList.Flink;
            pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);
            
            HvlGrantExAccess(pHvl, pVNic);
        }
        
        if (!fSameCtx)
        {
            HvlUpdateActiveCtx(pHvl, pCurrCtx, pNextCtx);
            MpTrace(COMP_HVL, DBG_NORMAL, ("Current context = %p, next context = %p, fSameCtx = %s\n", pCurrCtx, pNextCtx, fSameCtx?"TRUE":"FALSE"));
        }
        
        // tell the hardware that we are ready with the new context
        HvlUnlock(pHvl);                    
        Hw11CtxSComplete(pHvl->Hw);
        HvlLock(pHvl);

        // inform the VNICs that they are now active
        HvlNotifyAllVNicsInContext(pHvl, pNextCtx, VNic11CtxSToVNic, ulFlags);
    } while (FALSE);

    HvlCtxSDone(pHvl);

    return;
}

_IRQL_requires_same_
VOID
HvlProcessTimedCtxSwitch(
    PHVL pHvl
    )
{
    PVNIC pVNic = NULL;
    BOOLEAN fMerged = FALSE;
    
    HvlWaitForCtxSProcessingAndLock(pHvl);
    
    do
    {
        // are timed context switches currently blocked?
        if (HvlTimedCtxSBlocked(pHvl))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("Timed context switches are currently blocked.\n"));
            break;
        }
        
        if (HvlIsCtxSInProgress(pHvl))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("Context switch is already in progress. Not performing timed context switch.\n"));
            break;
        }
        
        // does a VNIC have exclusive access. If so we should not do any timed context switches
        pVNic = HvlGetExAccessVNic(pHvl);
        if (NULL != pVNic)
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("The VNic(%d) has requested or has exclusive access. Not doing the timed context switch\n", VNIC_PORT_NO));
            break;
        }

        /*
            perform any merge operations that might be needed. We do this only when we have
            more than two contexts (because the helper port context is never merged/split)
            */
        HvlPerformCtxMerge(pHvl, &fMerged);
         
        // do the context switch processing
        HvlCtxSProcessing(pHvl, NULL, fMerged, 0);
    } while (FALSE);
    
    HvlUnlock(pHvl);
}

_IRQL_requires_same_
VOID
HvlProcessExAccessReq(
    PHVL pHvl
    )
{
    PHVL_EX_ACCESS_REQ pExReq = NULL;
    PVNIC pExVNic = NULL;
    
    HvlWaitForCtxSProcessingAndLock(pHvl);
    
    do
    {
        pExVNic = HvlGetExAccessVNic(pHvl);
        if (NULL == pExVNic)
        {
            // no one has exclusive access right now

            // get the next pending exclusive access request. We dequeue it from the queue on a release
            pExReq = HvlGetNextReq(pHvl, FALSE);
            if (NULL != pExReq)
            {
                // split the VNIC into a separate context before giving exclusive access
                HvlPerformCtxSplit(pHvl, pExReq->pVNic);

                if (pHvl->pActiveContext == pExReq->pVNic->pHvlCtx)
                {
                    HvlGrantExAccess(pHvl, pExReq->pVNic);

                    // the VNIC is already active. Simply call its CtxSToVNic function
                    HvlSetCtxSInProgress(pHvl);                    
                    HvlNotifyAllVNicsInContext(pHvl, pHvl->pActiveContext, VNic11CtxSToVNic, VNIC_FLAG_GRANTED_EX_ACCESS);
                    HvlCtxSDone(pHvl);
                }
                else
                {                    
                    HvlCtxSProcessing(pHvl, pExReq->pVNic->pHvlCtx, FALSE, VNIC_FLAG_GRANTED_EX_ACCESS);
                }
            }
            else
            {
                MpTrace(COMP_HVL, DBG_NORMAL, ("There are no more exclusive access requests pending. Triggering timed context switches\n"));
                KeSetEvent(&pHvl->CtxSEvent, 0, FALSE);
            }
        }
        else
        {
            /*
                some other VNIC already acquired exclusive access. We will be run again 
                when that VNIC releases access
                */
            MpTrace(COMP_HVL, DBG_NORMAL, ("Another VNIC (%d) acquired exclusive access. Not doing anything for now\n", pExVNic->PortNumber));
        }
    } while (FALSE);
    
    HvlUnlock(pHvl);
}

VOID
HvlCtxSWorkItem(
    PVOID            Context,
    NDIS_HANDLE      NdisIoWorkItemHandle
    )
{
    NTSTATUS ntStatus = STATUS_SUCCESS;
    PHVL pHvl = NULL;
    LARGE_INTEGER waitTime = {0};
    #define NUM_EVENTS  3
    PVOID EventArray[NUM_EVENTS];  
    BOOLEAN fTerminating = FALSE;
    
    UNREFERENCED_PARAMETER(NdisIoWorkItemHandle);

    if (NULL == Context)
    {
        MPASSERT(Context != NULL);
        return;
    }

    pHvl = (PHVL) Context;
    EventArray[0] = &pHvl->TerminatingEvent;
    EventArray[1] = &pHvl->CtxSEvent;
    EventArray[2] = &pHvl->ExAccessEvent;

    /*
        The waitTime should ideally be a function of the beacon period. For now 
        assume a constant
        */
    waitTime.QuadPart = HVL_DEFAULT_CONTEXT_SWITCH_PARK_TIME_MSEC * -1;
    
    do
    {
        ntStatus = KeWaitForMultipleObjects(NUM_EVENTS, EventArray, WaitAny, Executive, KernelMode, FALSE, &waitTime, NULL);
        switch (ntStatus)
        {
            case STATUS_TIMEOUT:
                // timed context switch processing. 
                HvlProcessTimedCtxSwitch(pHvl);

                // optionally set a different waitTime for the next wait
                break;

            case STATUS_WAIT_0:
                // we are terminating. Time to get rid of this work item processing
                HvlLock(pHvl);
                pHvl->ulNumThreadsPending--;
                HvlUnlock(pHvl);  
                fTerminating = TRUE;
                break;

            case STATUS_WAIT_1:
                // we are asked to do context switch processing
                HvlProcessTimedCtxSwitch(pHvl);
                break;

            case STATUS_WAIT_2:
                // we have a new request for exclusive access
                HvlProcessExAccessReq(pHvl);
                break;
                
            default:
                MpTrace(COMP_HVL, DBG_SERIOUS, ("KeWaitForMultipleObjects returned error 0x%x", ntStatus));
                break;
        }        
    } while (!fTerminating);
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
HvlNotifyAllVNics(
    PHVL pHvl,
    PVOID pvNotif
    )
{
    LIST_ENTRY *pEntryVNic = NULL;
    PVNIC pVNic = NULL;
    PNOTIFICATION_DATA_HEADER pHdr = NULL;

    ASSERT(HvlIsLocked(pHvl));

    pHdr = (PNOTIFICATION_DATA_HEADER)pvNotif;
    
    pEntryVNic = pHvl->VNiclist.Flink;
    while (pEntryVNic != &pHvl->VNiclist)
    {
        pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, VNicLink);

        // do not notify if this VNIC is the source of the notification
        if (pVNic != pHdr->pSourceVNic)
        {
            _Analysis_assume_lock_held_(pHvl->Lock.SpinLock);

            HvlUnlock(pHvl);
            VNic11Notify(pVNic, pvNotif);
            HvlLock(pHvl);
        }
        
        pEntryVNic = pEntryVNic->Flink;
    }

    // Also send the notification to the helper port
    pEntryVNic = pHvl->pHelperPortCtx->VNicList.Flink;
    pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);

    _Analysis_assume_lock_held_(pHvl->Lock.SpinLock);
    HvlUnlock(pHvl);
    VNic11Notify(pVNic, pvNotif);
    HvlLock(pHvl);
}

VOID
HvlCacheNotification(
    PHVL                    pHvl,
    PVOID                   pvNotif
    )
{
    PNOTIFICATION_DATA_HEADER pNotifHdr = NULL;
    pNotifHdr = (PNOTIFICATION_DATA_HEADER)pvNotif;
    switch (pNotifHdr->Type)
    {
        case NotificationOpChannel:
        {
            NdisMoveMemory(&pHvl->CachedChannelNotification, pvNotif, sizeof(OP_CHANNEL_NOTIFICATION));
        }
        break;

        case NotificationOpLinkState:
        {
            // No need to cache this
        }
        break;                
                
        default:
        {
            // not handled
            ASSERT(FALSE);
        }
        break;
    };
}

VOID
HvlClearCachedNotification(
    PHVL                    pHvl
    )
{
    NdisZeroMemory(&pHvl->CachedChannelNotification, sizeof(OP_CHANNEL_NOTIFICATION));
}

_Function_class_(NDIS_IO_WORKITEM_FUNCTION)
VOID
HvlNotificationsWorkItem(
    _In_ PVOID            Context,
    _In_ NDIS_HANDLE      NdisIoWorkItemHandle
    )
{
    PHVL pHvl = NULL;
    LIST_ENTRY *pEntryNotif = NULL;
    PHVL_NOTIFICATION pNotification = NULL;
    
    UNREFERENCED_PARAMETER(NdisIoWorkItemHandle);

    pHvl = (PHVL) Context;

    HvlLock(pHvl);
    
    pEntryNotif = pHvl->NotificationsQueue.Flink;
    
    while (pEntryNotif != &pHvl->NotificationsQueue)
    {
        pNotification = CONTAINING_RECORD(pEntryNotif, HVL_NOTIFICATION, Link);
        RemoveEntryList (&pNotification->Link);

        HvlNotifyAllVNics(pHvl, pNotification->pvNotif);

        pEntryNotif = pEntryNotif->Flink;
        
        HvlFreeNotification(pNotification);
    }
    
    pHvl->fNotificationsWorkItemRunning = FALSE;
    pHvl->ulNumThreadsPending--;

    HvlUnlock(pHvl);
}

VOID
Hvl11Notify(
    PHVL                    pHvl,
    PVOID                   pvNotif
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;    
    PNOTIFICATION_DATA_HEADER pHdr = NULL;

    pHdr = (PNOTIFICATION_DATA_HEADER)pvNotif;
    
    HvlLock(pHvl);

    do
    {
        // update our internal state
        HvlCacheNotification(pHvl, pvNotif);

        ndisStatus = HvlQueueNotification(pHvl, pvNotif);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("HvlQueueNotification failed %!x!\n", ndisStatus));
            break;
        }

        if (!pHvl->fNotificationsWorkItemRunning)
        {
            pHvl->fNotificationsWorkItemRunning = TRUE;
            pHvl->ulNumThreadsPending++;
            
            NdisQueueIoWorkItem(
                pHvl->NotificationsWorkItemHandle,
                HvlNotificationsWorkItem,
                pHvl
                );
        }
        else
        {
            // the work item is already running. It will take care of processing this notification
        }
    } while (FALSE);
    
    HvlUnlock(pHvl);
}

