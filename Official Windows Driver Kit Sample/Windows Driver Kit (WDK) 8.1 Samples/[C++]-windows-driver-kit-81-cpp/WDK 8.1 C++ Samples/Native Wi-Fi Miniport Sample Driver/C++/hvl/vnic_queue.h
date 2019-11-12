#pragma once

typedef struct _VNIC VNIC, *PVNIC;

typedef enum _PENDING_OP_TYPE
{
    PENDING_OP_JOIN_REQ,
    PENDING_OP_CONN_START,
    PENDING_OP_EX_ACCESS_REQ,
    PENDING_OP_RESET_REQ,
    PENDING_OP_CH_SW_REQ,
    PENDING_OP_START_BSS_REQ,
    PENDING_OP_STOP_BSS_REQ,
    PENDING_OP_DEF_KEY,
    PENDING_OP_OPERATING_PHY_ID,
    PENDING_OP_DESIRED_PHY_ID_LIST,
    PENDING_OP_NIC_POWER_STATE
}PENDING_OP_TYPE;

__inline
PSTR
GetPendingOpString(
    PENDING_OP_TYPE OpType
    )
{
    PSTR pszString = NULL;
    
    switch (OpType)
    {
        case PENDING_OP_JOIN_REQ:
            pszString = "Join request";
            break;
        case PENDING_OP_CONN_START:
            pszString = "Connection start";
            break;
        case PENDING_OP_EX_ACCESS_REQ:
            pszString = "Exclusive access request";
            break;
        case PENDING_OP_RESET_REQ:
            pszString = "Reset request";
            break;
        case PENDING_OP_CH_SW_REQ:
            pszString = "Channel switch request";
            break;
        case PENDING_OP_START_BSS_REQ:
            pszString = "Start BSS request";
            break;
        case PENDING_OP_STOP_BSS_REQ:
            pszString = "Stop BSS request";
            break;
        case PENDING_OP_DEF_KEY:
            pszString = "Set default key request";
            break;
        case PENDING_OP_OPERATING_PHY_ID:
            pszString = "Set operating phyID request";
            break;
        case PENDING_OP_DESIRED_PHY_ID_LIST:
            pszString = "Set desired phyID list request";
            break;
        case PENDING_OP_NIC_POWER_STATE:
            pszString = "Set NIC power state";
            break;        
        default:
            ASSERT(FALSE);
            break;
    }

    return pszString;
}

typedef struct _PENDING_OP
{
    LIST_ENTRY Link;
    
    PENDING_OP_TYPE Type;
    PVOID pvOpData;
}PENDING_OP, *PPENDING_OP;

typedef struct _JOIN_REQ
{    
    PMP_BSS_DESCRIPTION     BSSDescription;
    ULONG                   JoinFailureTimeout;
    PORT11_GENERIC_CALLBACK_FUNC CompletionHandler;
} JOIN_REQ, *PJOIN_REQ;

typedef struct _VNIC_EX_ACCESS_REQ
{    
    PORT11_GENERIC_CALLBACK_FUNC    CallbkFn;
    PVOID                           pvCtx;
} VNIC_EX_ACCESS_REQ, *PVNIC_EX_ACCESS_REQ;

typedef struct _CH_SWITCH_REQ
{    
    ULONG  ulChannel;
    ULONG  ulPhyId;
    BOOLEAN  fSwitchPhy;
    PORT11_GENERIC_CALLBACK_FUNC pfnCompletionHandler;
} CH_SWITCH_REQ, *PCH_SWITCH_REQ;

typedef struct _VNIC_REQ
{
    NDIS_EVENT      CompletionEvent;
    NDIS_STATUS     ndisStatus;
    //ULONG           ulFlags;
} VNIC_REQ, *PVNIC_REQ;

typedef struct _RESET_REQ
{    
    PVNIC_REQ pReq;
    PDOT11_RESET_REQUEST pDot11ResetReq;
} RESET_REQ, *PRESET_REQ;

typedef struct _START_BSS_REQ
{    
    PMP_BSS_DESCRIPTION     BSSDescription;
    PORT11_GENERIC_CALLBACK_FUNC CompletionHandler;
} START_BSS_REQ, *PSTART_BSS_REQ;

typedef struct _STOP_BSS_REQ
{    
    PORT11_GENERIC_CALLBACK_FUNC CompletionHandler;
} STOP_BSS_REQ, *PSTOP_BSS_REQ;

typedef struct _DEF_KEY_REQ
{    
    PNDIS_EVENT             CompletionEvent;
    DOT11_MAC_ADDRESS       MacAddr;
    ULONG                   KeyID;
    BOOLEAN                 Persistent;
    DOT11_CIPHER_ALGORITHM  AlgoId;
    ULONG                   KeyLength;
    PUCHAR                  KeyValue;   
} DEF_KEY_REQ, *PDEF_KEY_REQ;

typedef struct _DEF_KEY_ID_REQ
{
    ULONG KeyId;
}DEF_KEY_ID_REQ, *PDEF_KEY_ID_REQ;

typedef struct _OPERATING_PHY_ID_REQ
{
    PNDIS_EVENT             CompletionEvent;
    ULONG PhyId;
}OPERATING_PHY_ID_REQ, *POPERATING_PHY_ID_REQ;

typedef struct _DESIRED_PHY_ID_LIST_REQ
{
    PNDIS_EVENT             CompletionEvent;
    PULONG                  PhyIDList;
    ULONG                   PhyIDCount;
} DESIRED_PHY_ID_LIST_REQ, *PDESIRED_PHY_ID_LIST_REQ;

typedef struct _NIC_POWER_STATE_REQ
{    
    PNDIS_EVENT             CompletionEvent;
    BOOLEAN                 PowerState;
    BOOLEAN                 SelectedPhy;
} NIC_POWER_STATE_REQ, *PNIC_POWER_STATE_REQ;

__inline
VOID
VNicInitReq(
    PVNIC_REQ pReq
    )
{
    pReq->ndisStatus = NDIS_STATUS_SUCCESS;
    //pReq->ulFlags = 0;
    NdisInitializeEvent(&pReq->CompletionEvent);
}
    
__inline
VOID
VNicWaitForReq(
    PVNIC_REQ pReq
    )
{
    NdisWaitEvent(&pReq->CompletionEvent, 0);
}
    
__inline
NDIS_STATUS
VNicGetReqStatus(
    PVNIC_REQ pReq
    )
{
    return pReq->ndisStatus;
}

/*    
__inline
ULONG
VNicGetReqFlags(
    PVNIC_REQ pReq
    )
{
    return pReq->ulFlags;
}
*/

__inline
VOID
VNicCompleteReq(
    PVNIC_REQ pReq,
    NDIS_STATUS ndisStatus
    )
{
    pReq->ndisStatus = ndisStatus;
    //pReq->ulFlags = ulFlags;
    NdisSetEvent(&pReq->CompletionEvent);
}


NDIS_STATUS
VNicQueueJoinRequest(
    _In_  PVNIC                   pVNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  ULONG                   JoinFailureTimeout,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    );

NDIS_STATUS
VNicQueueConnectionStart(
    _In_  PVNIC                   pVNic
    );

VOID
VNicDeletePendingOperation(
    _In_  PVNIC                   pVNic,
    _In_  PPENDING_OP             pPendingOp
    );

BOOLEAN
VNicIsPendingOpQueueEmpty(
    _In_  PVNIC                   pVNic
    );

VOID
VNicDeletePendingJoinRequest(
    _In_  PVNIC                   pVNic
    );

NDIS_STATUS
VNicQueueExAccessReq(
    _In_  PVNIC                           pVNic,
    _In_  PORT11_GENERIC_CALLBACK_FUNC    pCallbkFn,
    _In_  PVOID                           pvCtx
    );

NDIS_STATUS
VNicHandleExAccessOp(
    PVNIC pVNic,
    PENDING_OP_TYPE opType,
    PVOID pOp,
    BOOLEAN fCheckForReset,
    BOOLEAN fPnPOperation
    );

NDIS_STATUS
VNicAllocateChSwReq(
    _In_  PVNIC                   pVNic,
    _In_  ULONG                   ulChannel,
    _In_  ULONG                   ulPhyId,
    _In_  BOOLEAN                 fSwitchPhy,
    _In_  PORT11_GENERIC_CALLBACK_FUNC pfnCompletionHandler,
    PCH_SWITCH_REQ              *ppChSwReq
    );

void
VNicFreeChSwReq(
    PCH_SWITCH_REQ              pChSwReq
    );

NDIS_STATUS
VNicAllocateResetReq(
    _In_  PVNIC                   pVNic,
    _In_  PVNIC_REQ               pReq,
    _In_  PDOT11_RESET_REQUEST    pDot11ResetReq,
    PRESET_REQ                  *ppResetReq
    );

VOID
VNicFreeResetReq(
    PRESET_REQ                  pResetReq
    );

NDIS_STATUS
VNicQueuePendingOperation(
    _In_  PVNIC                   pVNic,
    _In_  PENDING_OP_TYPE         OpType,
    _In_opt_  PVOID                   pvOpData,
    _In_  BOOLEAN                 fPnPOperation
    );

NDIS_STATUS
VNicAllocateExAccessReq(
    _In_  PVNIC                           pVNic,
    _In_  PORT11_GENERIC_CALLBACK_FUNC    pCallbkFn,
    _In_  PVOID                           pvCtx,
    _In_  BOOLEAN                         fPnPOperation,
    _Out_ PVNIC_EX_ACCESS_REQ             *ppExReq 
    );

VOID
VNicFreeExAccessReq(
    _In_  PVNIC                           pVNic,
    _In_ PVNIC_EX_ACCESS_REQ             pExReq 
    );

VOID
VNicDeletePendingConnStartRequest(
    _In_  PVNIC                   pVNic,
    _Out_ BOOLEAN                 *pfDeleted
    );

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
    );

VOID
VNicFreeDefKeyReq(
    PDEF_KEY_REQ pDefKeyReq
    );

NDIS_STATUS
VNicAllocateOperatingPhyIdReq(
    _In_  PVNIC                   pVNic,
    _In_  PNDIS_EVENT             CompletionEvent,
    _In_  ULONG                   OperatingPhyId,
    POPERATING_PHY_ID_REQ       *ppPhyIdReq
    );

VOID
VNicFreeOperatingPhyIdReq(
    POPERATING_PHY_ID_REQ pPhyIdReq
    );

VOID
VNicDeletePendingOperationData(
    _In_ PVNIC pVNic,
    _In_ PENDING_OP_TYPE OpType,
    _In_ PVOID pvOpData
    );

NDIS_STATUS
VNicQueueStartBSSRequest(
    _In_  PVNIC                   pVNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    );

NDIS_STATUS
VNicQueueStopBSSRequest(
    _In_  PVNIC                   pVNic,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CompletionHandler
    );

VOID
VNicDeleteAllPendingOperations(PVNIC pVNic);

VOID
VNicFreeDesiredPhyIdListReq(
    PDESIRED_PHY_ID_LIST_REQ pDesiredPhyIdListReq 
    );

NDIS_STATUS
VNicAllocateDesiredPhyIdListReq(
    _In_  PVNIC                   pVNic,
    _In_  PNDIS_EVENT             CompletionEvent,
    _In_reads_(PhyIDCount)  PULONG                  PhyIDList,
    _In_  ULONG                   PhyIDCount,
    PDESIRED_PHY_ID_LIST_REQ       *ppDesiredPhyIdListReq
    );

NDIS_STATUS
VNicAllocateNicPowerStateReq(
    _In_  PVNIC                   pVNic,
    _In_  PNDIS_EVENT             CompletionEvent,
    _In_  BOOLEAN                 PowerState,
    _In_  BOOLEAN                 SelectedPhy,
    PNIC_POWER_STATE_REQ          *ppPowerStateReq
    );

VOID
VNicFreeNicPowerStateReq(
    PNIC_POWER_STATE_REQ pPowerStateReq
    );

VOID
VNicInitPreAllocatedOp(
    PVNIC pVNic
    );

