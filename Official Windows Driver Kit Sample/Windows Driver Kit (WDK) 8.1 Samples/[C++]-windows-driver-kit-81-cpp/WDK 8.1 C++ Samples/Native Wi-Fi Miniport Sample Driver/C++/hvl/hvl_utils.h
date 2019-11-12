#pragma once

// BUGBUG: Would it ever be called at dispatch?
#define ACQUIRE_LOCK(pStruct, DispatchLevel)                \
{                                                           \
    if (DispatchLevel)                                      \
    {                                                       \
        NdisDprAcquireSpinLock(&pStruct->Lock);             \
    }                                                       \
    else                                                    \
    {                                                       \
        NdisAcquireSpinLock(&pStruct->Lock);                \
    }                                                       \
    pStruct->fLocked = TRUE;                                \
}                                                           \

#define RELEASE_LOCK(pStruct, DispatchLevel)                \
{                                                           \
    pStruct->fLocked = FALSE;                               \
    if (DispatchLevel)                                      \
    {                                                       \
        NdisDprReleaseSpinLock(&pStruct->Lock);             \
    }                                                       \
    else                                                    \
    {                                                       \
        NdisReleaseSpinLock(&pStruct->Lock);                \
    }                                                       \
}                                                           \



NDIS_STATUS
AllocateMemory(NDIS_HANDLE ndisHandle, ULONG ulSize, PVOID *ppvMem);

VOID
FreeMemory(PVOID pvMem);

#define ALLOC_MEM(ndisHandle, cb, ppvMem)     AllocateMemory(ndisHandle, cb, (PVOID*)ppvMem)
#define FREE_MEM(pvMem)                FreeMemory((PVOID)pvMem)


