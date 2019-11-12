#pragma once

typedef enum _HVL_PENDING_OP_TYPE
{
    HVL_PENDING_OP_EX_ACCESS
}HVL_PENDING_OP_TYPE;

typedef struct _HVL_PENDING_OP
{
    LIST_ENTRY Link;
    
    HVL_PENDING_OP_TYPE Type;
    PVOID pvOpData;
}HVL_PENDING_OP, *PHVL_PENDING_OP;

typedef struct _HVL_EX_ACCESS_REQ
{    
    PVNIC               pVNic;
    ULONG               ulRefCount;
} HVL_EX_ACCESS_REQ, *PHVL_EX_ACCESS_REQ;

typedef struct _HVL_NOTIFICATION
{
    LIST_ENTRY Link;
    
    PVOID pvNotif;
}HVL_NOTIFICATION, *PHVL_NOTIFICATION;

NDIS_STATUS
HvlQueueExAccessRequest(
    _In_  PHVL                pHvl,
    _In_  PVNIC               pVNic,
    _In_  BOOLEAN             fPnPOperation
    );

VOID
HvlDeletePendingOperation(
    _In_  PHVL                 pHvl,
    _In_  PHVL_PENDING_OP      pPendingOp
    );

BOOLEAN
HvlIsPendingOpQueueEmpty(
    _In_  PHVL                   pHvl
    );

PHVL_EX_ACCESS_REQ
HvlDequeueNextReqForVNic(PHVL pHvl, PVNIC pVNic);

PHVL_EX_ACCESS_REQ
HvlGetNextReqForVNic(PHVL pHvl, PVNIC pVNic, BOOLEAN fDequeue);

PHVL_EX_ACCESS_REQ
HvlGetNextReq(PHVL pHvl, BOOLEAN fDequeue);

NDIS_STATUS
HvlQueueNotification(
    PHVL pHvl,
    PVOID pvNotif
    );

VOID
HvlFreeNotification(
    PHVL_NOTIFICATION pHvlNotif
    );

VOID
HvlInitPreAllocatedOp(
    PHVL pHvl
    );

VOID
HvlDeleteExAccessRequest(
    _In_  PHVL                        pHvl,
    _In_  PHVL_EX_ACCESS_REQ          pExReq
    );

VOID
HvlDeleteAllPendingOperations(
    PHVL pHvl
    );

