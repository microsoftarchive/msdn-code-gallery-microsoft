#pragma once

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicJoinBSSHelper(
    _In_  PVNIC                   pVNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  ULONG                   JoinFailureTimeout,
    _In_  PVNIC_COMPLETION_CTX    pCtx
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS 
VNicJoinComplete(
    _In_ PVNIC pVNic,
    _In_ BOOLEAN fReferenced,
    _In_ PORT11_GENERIC_CALLBACK_FUNC JoinCompleteHandler,
    _In_ PVOID Data
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicStartBSSHelper(
    _In_  PVNIC                   pVNic,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription
    );

_IRQL_requires_(DISPATCH_LEVEL)
NDIS_STATUS
VNicStopBSSHelper(
    _In_  PVNIC                   pVNic
    );

