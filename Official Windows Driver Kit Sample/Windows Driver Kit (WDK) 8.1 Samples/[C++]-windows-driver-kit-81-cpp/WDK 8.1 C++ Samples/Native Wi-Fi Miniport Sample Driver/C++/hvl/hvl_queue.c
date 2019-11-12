#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "hvl_queue.tmh"
#endif

extern
__inline
BOOLEAN
HvlIsPendingOpQueueEmpty(
    _In_  PHVL                   pHvl
    )
{
    return IsListEmpty(&pHvl->PendingOpQueue);
}

VOID
HvlInitPreAllocatedOp(
    PHVL pHvl
    )
{
    pHvl->pPnpOpExReq->pVNic = NULL;
    pHvl->pPnpOpExReq->ulRefCount = 0;
    
    pHvl->PnpPendingOp.Link.Blink = NULL;
    pHvl->PnpPendingOp.Link.Flink = NULL;
    pHvl->PnpPendingOp.Type = HVL_PENDING_OP_EX_ACCESS;
    pHvl->PnpPendingOp.pvOpData = pHvl->pPnpOpExReq;
}

VOID
HvlDeleteExAccessRequest(
    _In_  PHVL                        pHvl,
    _In_  PHVL_EX_ACCESS_REQ          pExReq
    )
{
    if (pExReq && !HvlIsPreAllocatedRequest(pHvl, pExReq))
    {
        FREE_MEM(pExReq);
    }
}

VOID
HvlDeletePendingOperation(
    _In_  PHVL                        pHvl,
    _In_  PHVL_PENDING_OP             pPendingOp
    )
{
    switch (pPendingOp->Type)
    {
        case HVL_PENDING_OP_EX_ACCESS:
        {
            PHVL_EX_ACCESS_REQ pExReq = (PHVL_EX_ACCESS_REQ)pPendingOp->pvOpData;
            HvlDeleteExAccessRequest(pHvl, pExReq);
        }
        break;            
    }

    if (&pHvl->PnpPendingOp != pPendingOp)
    {
        FREE_MEM(pPendingOp);
    }
}

NDIS_STATUS
HvlQueuePendingOperation(
    _In_  PHVL                    pHvl,
    _In_  HVL_PENDING_OP_TYPE     OpType,
    _In_  PVOID                   pvOpData,
    _In_  BOOLEAN                 fPnpOperation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHVL_PENDING_OP pPendingOp = NULL;

    do
    {
        if (fPnpOperation)
        {
            ASSERT(pHvl->PnpPendingOp.Link.Blink == NULL && pHvl->PnpPendingOp.Link.Flink == NULL);
            pPendingOp = &pHvl->PnpPendingOp;
        }
        else
        {
            ndisStatus = ALLOC_MEM(pHvl->MiniportAdapterHandle, sizeof(HVL_PENDING_OP), &pPendingOp);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for a new pending operation"));
                break;
            }

            pPendingOp->Type = OpType;
            pPendingOp->pvOpData= pvOpData;
        }
        
        InsertTailList(&pHvl->PendingOpQueue, &pPendingOp->Link);
        
    } while (FALSE);
    
    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (pPendingOp)
        {
            if (fPnpOperation)
            {
                HvlInitPreAllocatedOp(pHvl);
            }
            else
            {
                HvlDeletePendingOperation(pHvl, pPendingOp);
            }
        }
    }
    
    return ndisStatus;
    
}

NDIS_STATUS
HvlQueueExAccessRequest(
    _In_  PHVL                    pHvl,
    _In_  PVNIC                   pVNic,
    _In_  BOOLEAN                 fPnpOperation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHVL_EX_ACCESS_REQ pExReq = NULL, pPnpExReq = NULL;
    PHVL_PENDING_OP pPendingOp = NULL;

    ASSERT(HvlIsLocked(pHvl));
    
    do
    {
        // All requests by the helper port are considered different
        if (!HvlIsHelperVNic(pHvl, pVNic))
        {
            /*
                Does an exclusive access request for this VNIC already exist?
                */
            pExReq = HvlGetNextReqForVNic(pHvl, pVNic, FALSE);
        }

        if (NULL != pExReq)
        {
            /*
                There is already an exclusive access request existing for this VNIC. 
                */

            /*
                If the request is not a PnP request simply ref-up
                */
            if (!HvlIsPreAllocatedRequest(pHvl, pExReq))
            {
                pExReq->ulRefCount++;
                MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC (%d): Incremented refcount for an exclusive access requet. New ref = %d \n", VNIC_PORT_NO, pExReq->ulRefCount));
            }
            else
            {
                /*
                    a request already exists. However it is the PnP request. Make sure we do 
                    not use it for non-Pnp operation. If we did, we might get another PnP 
                    operation after this PnP operation is done but we won't be able to use the
                    pre-allocated request. Instead try to allocate a new request now
                    */

                // pnp operations must be serialized
                ASSERT(!fPnpOperation);

                pPnpExReq = pExReq;
                pExReq = NULL;

                // allocate exclusive access request                
                ndisStatus = ALLOC_MEM(pHvl->MiniportAdapterHandle, sizeof(HVL_EX_ACCESS_REQ), &pExReq);
                if (NDIS_STATUS_SUCCESS != ndisStatus)
                {
                    MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for a new exclusive access request\n"));
                    break;
                }

                // allocate pending operation
                ndisStatus = ALLOC_MEM(pHvl->MiniportAdapterHandle, sizeof(HVL_PENDING_OP), &pPendingOp);
                if (NDIS_STATUS_SUCCESS != ndisStatus)
                {
                    MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for a new pending operation"));
                    break;
                }
                
                pExReq->pVNic = pVNic;
                pExReq->ulRefCount = pPnpExReq->ulRefCount + 1;

                pPendingOp->Type = HVL_PENDING_OP_EX_ACCESS;
                pPendingOp->pvOpData= pExReq;

                // now insert this new request in the same place as the old request
                pPendingOp->Link.Flink = pHvl->PnpPendingOp.Link.Flink;
                pPendingOp->Link.Blink = pHvl->PnpPendingOp.Link.Blink;
                pHvl->PnpPendingOp.Link.Flink->Blink = &pPendingOp->Link;
                pHvl->PnpPendingOp.Link.Blink->Flink = &pPendingOp->Link;

                // re-init the pre-allocated request since we are no longer using it
                HvlInitPreAllocatedOp(pHvl);
            }
        }
        else
        {
            /*
                This is the only exclusive access request for the HVL. Create a new request
                */
            if (fPnpOperation)
            {
                ASSERT(pHvl->PnpPendingOp.Link.Blink == NULL && pHvl->PnpPendingOp.Link.Flink == NULL);
                
                pExReq = pHvl->pPnpOpExReq;
            }
            else
            {
                ndisStatus = ALLOC_MEM(pHvl->MiniportAdapterHandle, sizeof(HVL_EX_ACCESS_REQ), &pExReq);
                if (NDIS_STATUS_SUCCESS != ndisStatus)
                {
                    MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for a new exclusive access request\n"));
                    break;
                }
            }
            
            pExReq->pVNic = pVNic;
            pExReq->ulRefCount = 1;

            ndisStatus = HvlQueuePendingOperation(pHvl, HVL_PENDING_OP_EX_ACCESS, pExReq, fPnpOperation);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_HVL, DBG_SERIOUS, ("HvlQueuePendingOperation failed 0x%x\n", ndisStatus));
                break;
            }
            
            MpTrace(COMP_HVL, DBG_NORMAL, ("Queued an exclusive access requet for VNIC (%d)\n", VNIC_PORT_NO));
        }
    } while (FALSE);

    if (NDIS_FAILURE(ndisStatus))
    {
        if (pExReq && !fPnpOperation)
        {
            HvlDeleteExAccessRequest(pHvl, pExReq);
        }

        if (pPendingOp)
        {
            FREE_MEM(pPendingOp);
        }
    }
    
    return ndisStatus;
}

PHVL_EX_ACCESS_REQ
HvlGetNextReq(PHVL pHvl, BOOLEAN fDequeue)
{
    PHVL_EX_ACCESS_REQ pExReq = NULL;
    LIST_ENTRY *pEntryOp = NULL;
    PHVL_PENDING_OP pOp = NULL;

    ASSERT(HvlIsLocked(pHvl));

    if (!HvlIsPendingOpQueueEmpty(pHvl))
    {
        pEntryOp = pHvl->PendingOpQueue.Flink;
        pOp = CONTAINING_RECORD(pEntryOp, HVL_PENDING_OP, Link);
        pExReq = (PHVL_EX_ACCESS_REQ)pOp->pvOpData;

        if (fDequeue)
        {
            RemoveEntryList (&pOp->Link);
            pOp->pvOpData = NULL; // already aliased to pExReq
            HvlDeletePendingOperation(pHvl, pOp);        
        }    
    }

    return pExReq;
}

PHVL_EX_ACCESS_REQ
HvlGetNextReqForVNic(PHVL pHvl, PVNIC pVNic, BOOLEAN fDequeue)
{
    PHVL_EX_ACCESS_REQ pExReq = NULL;
    LIST_ENTRY *pEntryOp = NULL;
    PHVL_PENDING_OP pOp = NULL;
    BOOLEAN fFound = FALSE;

    ASSERT(HvlIsLocked(pHvl));

    pEntryOp = pHvl->PendingOpQueue.Flink;

    while (pEntryOp != &pHvl->PendingOpQueue)
    {
        pOp = CONTAINING_RECORD(pEntryOp, HVL_PENDING_OP, Link);
        
        pExReq = (PHVL_EX_ACCESS_REQ)pOp->pvOpData;

        if (pExReq->pVNic == pVNic)
        {
            fFound = TRUE;
            
            if (fDequeue)
            {
                pExReq->ulRefCount--;
                if (0 == pExReq->ulRefCount)
                {
                    RemoveEntryList (&pOp->Link);
                    
                    pOp->pvOpData = NULL; // already aliased to pExReq

                    HvlDeletePendingOperation(pHvl, pOp);        
                }
                else
                {
                    // nothing to be done since we have already decremented ref count
                }
            }
            
            break;
        }

        pEntryOp = pEntryOp->Flink;
    }

    if (fFound)
    {
        return pExReq;
    }
    else
    {
        return NULL;
    }
}

PHVL_EX_ACCESS_REQ
HvlDequeueNextReqForVNic(PHVL pHvl, PVNIC pVNic)
{
    return HvlGetNextReqForVNic(pHvl, pVNic, TRUE);
}

VOID
HvlFreeNotification(
    PHVL_NOTIFICATION pHvlNotif
    )
{
    if (pHvlNotif)
    {
        if (pHvlNotif->pvNotif)
        {
            FREE_MEM(pHvlNotif->pvNotif);
        }
        
        FREE_MEM(pHvlNotif);
    }
}

NDIS_STATUS
HvlQueueNotification(
    PHVL pHvl,
    PVOID pvNotif
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PHVL_NOTIFICATION pHvlNotif = NULL;
    PNOTIFICATION_DATA_HEADER pHdr = (PNOTIFICATION_DATA_HEADER)pvNotif;
    PVOID       pVNicNotif = NULL;

    ASSERT(HvlIsLocked(pHvl));
    
    do
    {
        ndisStatus = ALLOC_MEM(pHvl->MiniportAdapterHandle, sizeof(HVL_NOTIFICATION), &pHvlNotif);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for a HVL notification\n"));
            break;
        }

        ndisStatus = ALLOC_MEM(pHvl->MiniportAdapterHandle, pHdr->Size, &pVNicNotif);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate memory for notification\n"));
            break;
        }

        NdisMoveMemory(pVNicNotif , pvNotif, pHdr->Size);
        
        pHvlNotif->pvNotif = pVNicNotif;
        InsertTailList(&pHvl->NotificationsQueue, &pHvlNotif->Link);
            
    } while (FALSE);

    if (NDIS_FAILURE(ndisStatus))
    {
        HvlFreeNotification(pHvlNotif);
    }

    return ndisStatus;
}

VOID
HvlDeleteAllPendingOperations(PHVL pHvl)
{
    LIST_ENTRY *pEntryOp = NULL;
    PHVL_PENDING_OP pOp = NULL;

    MpTrace(COMP_HVL, DBG_NORMAL, ("HvlDeleteAllPendingOperations called \n"));

    pEntryOp = pHvl->PendingOpQueue.Flink;
    while (pEntryOp != &pHvl->PendingOpQueue)
    {
        pOp = CONTAINING_RECORD(pEntryOp, HVL_PENDING_OP, Link);
        RemoveEntryList (&pOp->Link);

        MpTrace(COMP_HVL, DBG_NORMAL, ("Deleting pending operation \n"));
        HvlDeletePendingOperation(pHvl, pOp);

        // set the entry to point to the beginning of the queue
        pEntryOp = pHvl->PendingOpQueue.Flink;
    }
}

