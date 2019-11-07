#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "vnic_queue.tmh"
#endif

extern
__inline
BOOLEAN
VNicIsPendingOpQueueEmpty(
    _In_  PVNIC                   pVNic
    )
{
    return IsListEmpty(&pVNic->PendingOpQueue);
}

VOID
VNicDeletePendingOperationData(
    _In_ PVNIC pVNic,
    _In_ PENDING_OP_TYPE OpType,
    _In_ PVOID pvOpData
    )
{
    switch (OpType)
    {
        case PENDING_OP_CONN_START:
        {
            FREE_MEM(pvOpData);
            break;
        }
        
        case PENDING_OP_JOIN_REQ:
        {
            PJOIN_REQ pJoinReq = (PJOIN_REQ)pvOpData;
            if (pJoinReq)
            {
                if (pJoinReq->BSSDescription)
                {
                    FREE_MEM(pJoinReq->BSSDescription);
                }
                FREE_MEM(pJoinReq);
            }
            break;            
        }

        case PENDING_OP_START_BSS_REQ:
        {
            PSTART_BSS_REQ pStartBssReq = (PSTART_BSS_REQ)pvOpData;
            if (pStartBssReq)
            {
                if (pStartBssReq->BSSDescription)
                {
                    FREE_MEM(pStartBssReq->BSSDescription);
                }
                FREE_MEM(pStartBssReq);
            }
            break;            
        }

        case PENDING_OP_STOP_BSS_REQ:
        {
            FREE_MEM(pvOpData);
            break;            
        }

        case PENDING_OP_DEF_KEY:
        {
            PDEF_KEY_REQ pDefKeyReq = (PDEF_KEY_REQ)pvOpData;
            VNicFreeDefKeyReq(pDefKeyReq);
            break;            
        }

        case PENDING_OP_EX_ACCESS_REQ:
        {
            PVNIC_EX_ACCESS_REQ pExReq = (PVNIC_EX_ACCESS_REQ)pvOpData;
            VNicFreeExAccessReq(pVNic, pExReq);
            break;
        }
        
        case PENDING_OP_RESET_REQ:
        {
            PRESET_REQ pResetReq = (PRESET_REQ)pvOpData;
            VNicFreeResetReq(pResetReq);
            break;
        }

        case PENDING_OP_CH_SW_REQ:
        {
            PCH_SWITCH_REQ pChSwReq = (PCH_SWITCH_REQ)pvOpData;
            VNicFreeChSwReq(pChSwReq);
            break;
        }
        
        case PENDING_OP_OPERATING_PHY_ID:
        {
            POPERATING_PHY_ID_REQ pPhyidReq = (POPERATING_PHY_ID_REQ)pvOpData;
            VNicFreeOperatingPhyIdReq(pPhyidReq);
            break;
        }

        case PENDING_OP_DESIRED_PHY_ID_LIST:
        {
            PDESIRED_PHY_ID_LIST_REQ pDesiredPhyIdListReq = (PDESIRED_PHY_ID_LIST_REQ)pvOpData;
            VNicFreeDesiredPhyIdListReq(pDesiredPhyIdListReq);
            break;
        }

        case PENDING_OP_NIC_POWER_STATE:
        {
            PNIC_POWER_STATE_REQ pPowerStateReq = (PNIC_POWER_STATE_REQ)pvOpData;
            VNicFreeNicPowerStateReq(pPowerStateReq);
            break;            
        }
        
        default:
            ASSERT(FALSE);
            break;
    };
}


VOID
VNicDeletePendingOperation(
    _In_  PVNIC                   pVNic,
    _In_  PPENDING_OP             pPendingOp
    )
{
    VNicDeletePendingOperationData(pVNic, pPendingOp->Type, pPendingOp->pvOpData);

    if (!VNicIsPreallocatedOperation(pVNic, pPendingOp))
    {
        FREE_MEM(pPendingOp);
    }
}

VOID
VNicDeleteAllPendingOperations(PVNIC pVNic)
{
    LIST_ENTRY *pEntryOp = NULL;
    PPENDING_OP pOp = NULL;

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): VNicDeleteAllPendingOperations called \n", VNIC_PORT_NO));

    pEntryOp = pVNic->PendingOpQueue.Flink;
    while (pEntryOp != &pVNic->PendingOpQueue)
    {
        pOp = CONTAINING_RECORD(pEntryOp, PENDING_OP, Link);
        RemoveEntryList (&pOp->Link);

        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Pending %s deleted\n", VNIC_PORT_NO, GetPendingOpString(pOp->Type)));
        VNicDeletePendingOperation(pVNic, pOp);

        pEntryOp = pEntryOp->Flink;
    }
}

NDIS_STATUS
VNicQueuePendingOperation(
    _In_  PVNIC                   pVNic,
    _In_  PENDING_OP_TYPE         OpType,
    _In_opt_  PVOID                   pvOpData,
    _In_  BOOLEAN                 fPnPOperation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PPENDING_OP pPendingOp = NULL;

    do
    {
        if (fPnPOperation)
        {
            ASSERT(pVNic->PnpPendingOp.Link.Blink == NULL && pVNic->PnpPendingOp.Link.Flink == NULL);
            pPendingOp = &pVNic->PnpPendingOp;
        }
        else
        {
            ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(PENDING_OP), &pPendingOp);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new pending operation", VNIC_PORT_NO));
                break;
            }

            pPendingOp->Type = OpType;
            pPendingOp->pvOpData= pvOpData;
        }
                
        InsertTailList(&pVNic->PendingOpQueue, &pPendingOp->Link);
        
    } while (FALSE);
    
    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (!fPnPOperation && pPendingOp)
        {
            VNicDeletePendingOperation(pVNic, pPendingOp);
        }
    }
    
    return ndisStatus;
    
}

NDIS_STATUS
VNicQueueJoinRequest(
    _In_  PVNIC                   pVNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  ULONG                   JoinFailureTimeout,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PJOIN_REQ pJoinReq = NULL;
    PMP_BSS_DESCRIPTION pBSSDescCopy = NULL;
    ULONG ulBufferSize = 0;

    ASSERT(VNicIsLocked(pVNic));
    
    do
    {
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(JOIN_REQ), &pJoinReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new join request\n", VNIC_PORT_NO));
            break;
        }

        ulBufferSize = FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) 
                            + BSSDescription->IEBlobsSize;
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, ulBufferSize, &pBSSDescCopy);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new BSS description\n", VNIC_PORT_NO));
            break;
        }

        NdisMoveMemory(pBSSDescCopy, BSSDescription, ulBufferSize);
        
        pJoinReq->BSSDescription = pBSSDescCopy;
        pJoinReq->CompletionHandler = CompletionHandler;
        pJoinReq->JoinFailureTimeout = JoinFailureTimeout;

        ndisStatus = VNicQueuePendingOperation(pVNic, PENDING_OP_JOIN_REQ, pJoinReq, FALSE);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicQueuePendingOperation failed 0x%x\n", VNIC_PORT_NO, ndisStatus));
            break;
        }

        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Queued a join request\n", VNIC_PORT_NO));
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (pBSSDescCopy)
        {
            FREE_MEM(pBSSDescCopy);
        }
        if (pJoinReq)
        {
            FREE_MEM(pJoinReq);
        }
    }
    
    return ndisStatus;
}

// BUGBUG: Remove
/*
    This function is called with the VNic locked
    */
VOID
VNicDeletePendingJoinRequest(
    _In_  PVNIC                   pVNic
    )
{
    LIST_ENTRY *pEntryOp = NULL;
    PPENDING_OP pOp = NULL;

    ASSERT(VNicIsLocked(pVNic));

    pEntryOp = pVNic->PendingOpQueue.Flink;
    while (pEntryOp != &pVNic->PendingOpQueue)
    {
        pOp = CONTAINING_RECORD(pEntryOp, PENDING_OP, Link);

        if (PENDING_OP_JOIN_REQ == pOp->Type)
        {
            // when can this happen?
            
            ASSERT(FALSE);
            RemoveEntryList (&pOp->Link);
            VNicDeletePendingOperation(pVNic, pOp);
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Pending join request deleted\n", VNIC_PORT_NO));
            break;
        }
        else
        {
            pEntryOp = pEntryOp->Flink;
        }
    }
}

/*
    This function is called with the VNic locked
    */
VOID
VNicDeletePendingConnStartRequest(
    _In_  PVNIC                   pVNic,
    _Out_ BOOLEAN                 *pfDeleted
    )
{
    LIST_ENTRY *pEntryOp = NULL;
    PPENDING_OP pOp = NULL;

    ASSERT(VNicIsLocked(pVNic));

    *pfDeleted = FALSE;
    
    pEntryOp = pVNic->PendingOpQueue.Flink;
    while (pEntryOp != &pVNic->PendingOpQueue)
    {
        pOp = CONTAINING_RECORD(pEntryOp, PENDING_OP, Link);

        if (PENDING_OP_CONN_START == pOp->Type)
        {
            RemoveEntryList (&pOp->Link);
            VNicDeletePendingOperation(pVNic, pOp);
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Pending connection start deleted\n", VNIC_PORT_NO));
            *pfDeleted = TRUE;
            break;
        }
        else
        {
            pEntryOp = pEntryOp->Flink;
        }
    }
}
  
NDIS_STATUS
VNicQueueConnectionStart(
    _In_  PVNIC                   pVNic
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ASSERT(VNicIsLocked(pVNic));
    
    do
    {
        ndisStatus = VNicQueuePendingOperation(pVNic, PENDING_OP_CONN_START, NULL, FALSE);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicQueuePendingOperation failed 0x%x", VNIC_PORT_NO, ndisStatus));
            break;
        }
        
        MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Connection start notification queued\n", VNIC_PORT_NO));
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
VNicAllocateExAccessReq(
    _In_  PVNIC                           pVNic,
    _In_  PORT11_GENERIC_CALLBACK_FUNC    pCallbkFn,
    _In_  PVOID                           pvCtx,
    _In_  BOOLEAN                         fPnPOperation,
    _Out_ PVNIC_EX_ACCESS_REQ             *ppExReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC_EX_ACCESS_REQ pExReq = NULL;

    do
    {
        if (fPnPOperation)
        {
            pExReq = pVNic->pPnpOpExReq;
        }
        else
        {
            ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(VNIC_EX_ACCESS_REQ), &pExReq);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new exclusive access request\n", VNIC_PORT_NO));
                break;
            }
        }
        
        pExReq->CallbkFn = pCallbkFn;
        pExReq->pvCtx = pvCtx;
    } while (FALSE);   

    *ppExReq = pExReq;

    return ndisStatus;
}

VOID
VNicFreeExAccessReq(
    _In_  PVNIC                           pVNic,
    _In_ PVNIC_EX_ACCESS_REQ             pExReq 
    )
{
    if (pExReq && !VNicIsPreallocatedRequest(pVNic, pExReq))
    {
        FREE_MEM(pExReq);
    }
}

NDIS_STATUS
VNicAllocateChSwReq(
    _In_  PVNIC                   pVNic,
    _In_  ULONG                   ulChannel,
    _In_  ULONG                   ulPhyId,
    _In_  BOOLEAN                 fSwitchPhy,
    _In_  PORT11_GENERIC_CALLBACK_FUNC pfnCompletionHandler,
    PCH_SWITCH_REQ              *ppChSwReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PCH_SWITCH_REQ pChSwReq = NULL;

    do
    {
        *ppChSwReq = NULL;
        
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(CH_SWITCH_REQ), &pChSwReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new channel switch request\n", VNIC_PORT_NO));
            break;
        }
    
        pChSwReq->ulChannel = ulChannel;
        pChSwReq->ulPhyId = ulPhyId;
        pChSwReq->fSwitchPhy = fSwitchPhy;
        pChSwReq->pfnCompletionHandler = pfnCompletionHandler;

        *ppChSwReq = pChSwReq;
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (pChSwReq)
        {
            FREE_MEM(pChSwReq);
        }
    }
    
    return ndisStatus;
}

void
VNicFreeChSwReq(
    PCH_SWITCH_REQ              pChSwReq
    )
{
    if (pChSwReq)
    {
        FREE_MEM(pChSwReq);
    }
}


NDIS_STATUS
VNicAllocateResetReq(
    _In_  PVNIC                   pVNic,
    _In_  PVNIC_REQ               pReq,
    _In_  PDOT11_RESET_REQUEST    pDot11ResetReq,
    PRESET_REQ                  *ppResetReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PRESET_REQ pResetReq = NULL;

    do
    {
        *ppResetReq = NULL;
        
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(RESET_REQ), &pResetReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new dot11 reset request\n", VNIC_PORT_NO));
            break;
        }

        pResetReq->pReq = pReq;
        pResetReq->pDot11ResetReq = pDot11ResetReq;
        
        *ppResetReq = pResetReq;
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (pResetReq)
        {
            FREE_MEM(pResetReq);
        }
    }
    
    return ndisStatus;
}

VOID
VNicFreeResetReq(
    PRESET_REQ                  pResetReq
    )
{
    if (pResetReq)
    {
        FREE_MEM(pResetReq);
    }
}

NDIS_STATUS
VNicQueueStartBSSRequest(
    _In_  PVNIC                   pVNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PSTART_BSS_REQ pStartBssReq = NULL;
    PMP_BSS_DESCRIPTION pBSSDescCopy = NULL;
    ULONG ulBufferSize = 0;

    ASSERT(VNicIsLocked(pVNic));
    
    do
    {
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(START_BSS_REQ), &pStartBssReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new start BSS request\n", VNIC_PORT_NO));
            break;
        }

        ulBufferSize = FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) 
                        + BSSDescription->IEBlobsSize;
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, ulBufferSize, &pBSSDescCopy);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new BSS description\n", VNIC_PORT_NO));
            break;
        }

        NdisMoveMemory(pBSSDescCopy, BSSDescription, ulBufferSize);
        
        pStartBssReq->BSSDescription = pBSSDescCopy;
        pStartBssReq->CompletionHandler = CompletionHandler;

        ndisStatus = VNicQueuePendingOperation(pVNic, PENDING_OP_START_BSS_REQ, pStartBssReq, FALSE);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicQueuePendingOperation failed 0x%x\n", VNIC_PORT_NO, ndisStatus));
            break;
        }

        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Queued a start BSS request\n", VNIC_PORT_NO));
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (pBSSDescCopy)
        {
            FREE_MEM(pBSSDescCopy);
        }
        if (pStartBssReq)
        {
            FREE_MEM(pStartBssReq);
        }
    }
    
    return ndisStatus;
}

NDIS_STATUS
VNicQueueStopBSSRequest(
    _In_  PVNIC                   pVNic,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PSTOP_BSS_REQ pStopBssReq = NULL;

    ASSERT(VNicIsLocked(pVNic));
    
    do
    {
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(STOP_BSS_REQ), &pStopBssReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a new stop BSS request\n", VNIC_PORT_NO));
            break;
        }

        pStopBssReq->CompletionHandler = CompletionHandler;

        ndisStatus = VNicQueuePendingOperation(pVNic, PENDING_OP_STOP_BSS_REQ, pStopBssReq, FALSE);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): VNicQueuePendingOperation failed 0x%x\n", VNIC_PORT_NO, ndisStatus));
            break;
        }

        MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Queued a stop BSS request\n", VNIC_PORT_NO));
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        if (pStopBssReq)
        {
            FREE_MEM(pStopBssReq);
        }
    }
    
    return ndisStatus;
}


NDIS_STATUS
VNicAllocateDefaultKeyReq(
    _In_  PVNIC                   pVNic,
    _In_  PNDIS_EVENT             CompletionEvent,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  ULONG                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,    
    PDEF_KEY_REQ                *ppDefKeyReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDEF_KEY_REQ pDefKeyReq = NULL;

    do
    {
        *ppDefKeyReq = NULL;
        
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(DEF_KEY_REQ), &pDefKeyReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a set default key request\n", VNIC_PORT_NO));
            break;
        }

        if (KeyLength)
        {
            ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, KeyLength, &pDefKeyReq->KeyValue);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for key value \n", VNIC_PORT_NO));
                break;
            }
        }
        
        pDefKeyReq->CompletionEvent = CompletionEvent;
        NdisMoveMemory(&pDefKeyReq->MacAddr, MacAddr, sizeof(DOT11_MAC_ADDRESS));        
        pDefKeyReq->KeyID = KeyID;
        pDefKeyReq->Persistent = Persistent;
        pDefKeyReq->AlgoId = AlgoId;
        pDefKeyReq->KeyLength = KeyLength;
        if (KeyLength)
        {
            NdisMoveMemory(pDefKeyReq->KeyValue, KeyValue, KeyLength);
        }
        else
        {
            pDefKeyReq->KeyValue = NULL;
        }

        *ppDefKeyReq = pDefKeyReq;
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        VNicFreeDefKeyReq(pDefKeyReq);
    }
    
    return ndisStatus;
}

VOID
VNicFreeDefKeyReq(
    PDEF_KEY_REQ pDefKeyReq
    )
{
    if (pDefKeyReq)
    {
        if (pDefKeyReq->KeyValue)
        {
            FREE_MEM(pDefKeyReq->KeyValue);
        }
        FREE_MEM(pDefKeyReq);
    }
}

NDIS_STATUS
VNicAllocateOperatingPhyIdReq(
    _In_  PVNIC                   pVNic,
    _In_  PNDIS_EVENT             CompletionEvent,
    _In_  ULONG                   OperatingPhyId,
    POPERATING_PHY_ID_REQ       *ppPhyIdReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    POPERATING_PHY_ID_REQ pPhyIdReq = NULL;

    do
    {
        *ppPhyIdReq = NULL;
        
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(OPERATING_PHY_ID_REQ), &pPhyIdReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a set operating Phy ID request\n", VNIC_PORT_NO));
            break;
        }

        pPhyIdReq->CompletionEvent = CompletionEvent;
        pPhyIdReq->PhyId = OperatingPhyId;

        *ppPhyIdReq = pPhyIdReq;
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        VNicFreeOperatingPhyIdReq(pPhyIdReq);
    }
    
    return ndisStatus;
}

VOID
VNicFreeOperatingPhyIdReq(
    POPERATING_PHY_ID_REQ pPhyIdReq
    )
{
    if (pPhyIdReq)
    {
        FREE_MEM(pPhyIdReq);
    }
}

NDIS_STATUS
VNicAllocateDesiredPhyIdListReq(
    _In_  PVNIC                   pVNic,
    _In_  PNDIS_EVENT             CompletionEvent,
    _In_reads_(PhyIDCount)  PULONG                  PhyIDList,
    _In_  ULONG                   PhyIDCount,
    PDESIRED_PHY_ID_LIST_REQ       *ppDesiredPhyIdListReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDESIRED_PHY_ID_LIST_REQ pDesiredPhyIdListReq = NULL;

    do
    {
        *ppDesiredPhyIdListReq = NULL;
        
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(DESIRED_PHY_ID_LIST_REQ), &pDesiredPhyIdListReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a set desired Phy ID list request\n", VNIC_PORT_NO));
            break;
        }

        if (PhyIDCount)
        {
            ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(ULONG) * PhyIDCount, &pDesiredPhyIdListReq->PhyIDList);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for desired Phy ID list request\n", VNIC_PORT_NO));
                break;
            }
        }
        
        pDesiredPhyIdListReq->CompletionEvent = CompletionEvent;
        pDesiredPhyIdListReq->PhyIDCount = PhyIDCount;
        if (PhyIDCount)
        {
            NdisMoveMemory(pDesiredPhyIdListReq->PhyIDList, PhyIDList, sizeof(ULONG) * PhyIDCount);
        }

        *ppDesiredPhyIdListReq = pDesiredPhyIdListReq;
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        VNicFreeDesiredPhyIdListReq(pDesiredPhyIdListReq);
    }
    
    return ndisStatus;
}

VOID
VNicFreeDesiredPhyIdListReq(
    PDESIRED_PHY_ID_LIST_REQ pDesiredPhyIdListReq 
    )
{
    if (pDesiredPhyIdListReq)
    {
        if (pDesiredPhyIdListReq->PhyIDList)
        {
            FREE_MEM(pDesiredPhyIdListReq->PhyIDList);
        }
        FREE_MEM(pDesiredPhyIdListReq);
    }
}


NDIS_STATUS
VNicAllocateNicPowerStateReq(
    _In_  PVNIC                   pVNic,
    _In_  PNDIS_EVENT             CompletionEvent,
    _In_  BOOLEAN                 PowerState,
    _In_  BOOLEAN                 SelectedPhy,
    PNIC_POWER_STATE_REQ          *ppPowerStateReq
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PNIC_POWER_STATE_REQ pPowerStateReq = NULL;

    do
    {
        *ppPowerStateReq = NULL;
        
        ndisStatus = ALLOC_MEM(pVNic->MiniportAdapterHandle, sizeof(NIC_POWER_STATE_REQ), &pPowerStateReq);
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_HVL, DBG_SERIOUS, ("VNic(%d): Failed to allocate memory for a set NIC power state request\n", VNIC_PORT_NO));
            break;
        }

        pPowerStateReq->CompletionEvent = CompletionEvent;
        pPowerStateReq->PowerState = PowerState;
        pPowerStateReq->SelectedPhy = SelectedPhy;

        *ppPowerStateReq = pPowerStateReq;
    } while (FALSE);

    if (NDIS_STATUS_SUCCESS != ndisStatus)
    {
        VNicFreeNicPowerStateReq(pPowerStateReq);
    }
    
    return ndisStatus;
}

VOID
VNicFreeNicPowerStateReq(
    PNIC_POWER_STATE_REQ pPowerStateReq
    )
{
    if (pPowerStateReq)
    {
        FREE_MEM(pPowerStateReq);
    }
}


VOID
VNicInitPreAllocatedOp(
    PVNIC pVNic
    )
{
    pVNic->pPnpOpExReq->pvCtx = NULL;
    pVNic->pPnpOpExReq->CallbkFn = 0;
    
    pVNic->PnpPendingOp.Link.Blink = NULL;
    pVNic->PnpPendingOp.Link.Flink = NULL;
    pVNic->PnpPendingOp.Type = PENDING_OP_EX_ACCESS_REQ;
    pVNic->PnpPendingOp.pvOpData = pVNic->pPnpOpExReq;
}

