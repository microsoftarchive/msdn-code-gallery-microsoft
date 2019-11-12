/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_rwlock.c

Abstract:
    Implements read write lock routines. 
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "mp_rwlock.h"


#if DOT11_TRACE_ENABLED
#include "mp_rwlock.tmh"
#endif

// Signatures of the NDIS functions for the new Read/Write lock API
typedef PNDIS_RW_LOCK_EX
(*MP_NDIS_ALLOCATE_RW_LOCK)(
    NDIS_HANDLE             Handle
);

typedef VOID
(*MP_NDIS_FREE_RW_LOCK)(
    PNDIS_RW_LOCK_EX        Lock
);

typedef VOID
(*MP_NDIS_ACQUIRE_RW_LOCK_READ)(
    PNDIS_RW_LOCK_EX        Lock,
    PLOCK_STATE_EX          LockState,
    UCHAR                   Flags
);

typedef VOID
(*MP_NDIS_ACQUIRE_RW_LOCK_WRITE)(
    PNDIS_RW_LOCK_EX        Lock,
    PLOCK_STATE_EX          LockState,
    UCHAR                   Flags
);

typedef VOID
(*MP_NDIS_RELEASE_RW_LOCK)(
    PNDIS_RW_LOCK_EX        Lock,
    PLOCK_STATE_EX          LockState
);

// The addresses of the NDIS functions for the new Read/Write lock API
MP_NDIS_ALLOCATE_RW_LOCK MpNdisAllocateRWLock;
MP_NDIS_FREE_RW_LOCK MpNdisFreeRWLock;
MP_NDIS_ACQUIRE_RW_LOCK_READ MpNdisAcquireRWLockRead;
MP_NDIS_ACQUIRE_RW_LOCK_WRITE MpNdisAcquireRWLockWrite;
MP_NDIS_RELEASE_RW_LOCK MpNdisReleaseRWLock; 

//
// Read/Write API called from within the driver on new NDIS versions
//
NDIS_STATUS
MpAllocateNewRWLock(
    PMP_READ_WRITE_LOCK     Lock,
    NDIS_HANDLE             Handle    
    ) 
{
    if (Handle == NULL)
    {
        // Caller does not have the miniport handle. We use the driver handle
        Handle = GlobalDriverHandle;
    }
    
    Lock->NewRWLock = MpNdisAllocateRWLock(Handle);
    if (Lock->NewRWLock == NULL)
    {
        return NDIS_STATUS_RESOURCES;
    }
    
    return NDIS_STATUS_SUCCESS;
}

VOID
MpFreeNewRWLock(
    PMP_READ_WRITE_LOCK     Lock    
    ) 
{
    if (Lock->NewRWLock != NULL)
    {
        MpNdisFreeRWLock(Lock->NewRWLock);
        Lock->NewRWLock = NULL;
    }
}

VOID 
MpAcquireNewRWLockForRead(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    ) 
{
    LockState->ReadLock = TRUE;
    MpNdisAcquireRWLockRead(Lock->NewRWLock, &(LockState->NewLockState), 0);
}

VOID
MpAcquireNewRWLockForWrite(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    ) 
{
    LockState->ReadLock = FALSE;
    MpNdisAcquireRWLockWrite(Lock->NewRWLock, &(LockState->NewLockState), 0);
}


VOID
MpReleaseNewRWLock(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    )  
{
    MpNdisReleaseRWLock(Lock->NewRWLock, &(LockState->NewLockState));
}

//
// Read/Write API called from within the driver on old NDIS versions
//

NDIS_STATUS
MpAllocateOldRWLock(
    PMP_READ_WRITE_LOCK     Lock,
    NDIS_HANDLE             Handle    
    ) 
{
    UNREFERENCED_PARAMETER(Handle);
    NdisInitializeReadWriteLock(&Lock->OldRWLock);    
    return NDIS_STATUS_SUCCESS;
}

VOID
MpFreeOldRWLock(
    PMP_READ_WRITE_LOCK     Lock    
    ) 
{
    UNREFERENCED_PARAMETER(Lock);
}

_Acquires_shared_lock_(&Lock->OldRWLock)
_Acquires_shared_lock_(LockState->OldLockState)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_raises_(DISPATCH_LEVEL)
VOID
MpAcquireOldRWLockForRead(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    ) 
#pragma warning (suppress:28167) // IRQL change
{
    LockState->ReadLock = TRUE;
    NdisAcquireReadWriteLock(&Lock->OldRWLock, FALSE, &(LockState->OldLockState));
}

_Acquires_exclusive_lock_(&Lock->OldRWLock)
_Acquires_exclusive_lock_(LockState->OldLockState)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_raises_(DISPATCH_LEVEL)
VOID
MpAcquireOldRWLockForWrite(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    ) 
#pragma warning (suppress:28167) // IRQL change
{
    LockState->ReadLock = FALSE;
    NdisAcquireReadWriteLock(&Lock->OldRWLock, TRUE, &(LockState->OldLockState));
}

_Requires_lock_held_(&Lock->OldRWLock)
_Releases_shared_lock_(&Lock->OldRWLock)
_Releases_shared_lock_(LockState->OldLockState)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_min_(DISPATCH_LEVEL)
VOID
MpReleaseOldRWLock(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    )  
#pragma warning (suppress:28167) // IRQL change
{
    _Analysis_assume_lock_acquired_(LockState->OldLockState);

    NdisReleaseReadWriteLock(&Lock->OldRWLock, &(LockState->OldLockState));
}




/**
 This implementation picks the NDIS 6.2 Read/Write lock structures & API on OS versions 
 where NDIS support those and would pick the older API on NDIS 6.0. Since the new API 
 are not available on NDIS 6.0 OS, it does not link the new API into the binary, instead 
 it dynamically loads the API address & uses them. This way the same binary can run on 
 both NDIS 6.0 & 6.2 OSes, using the correct API for that OS.
 */
NDIS_STATUS
MpDetermineRWLockType()
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;    
    ULONG                       ndisVersion;
    NDIS_STRING                 allocateRoutineName;
    NDIS_STRING                 freeRoutineName;
    NDIS_STRING                 acquireReadRoutineName;
    NDIS_STRING                 acquireWriteRoutineName;
    NDIS_STRING                 releaseRoutineName;

    do
    {
        NdisZeroMemory(&GlobalRWLockHandlers, sizeof(MP_RW_LOCK_HANDLERS));
            
        // Start with pre NDIS 6.2 using the old API. If this is NDIS 6.2+ and
        // we cannot find address of new API, we will fail
        GlobalRWLockHandlers.AllocateHandler = MpAllocateOldRWLock;
        GlobalRWLockHandlers.FreeHandler = MpFreeOldRWLock;
        GlobalRWLockHandlers.AcquireReadHandler = MpAcquireOldRWLockForRead;
        GlobalRWLockHandlers.AcquireWriteHandler = MpAcquireOldRWLockForWrite;
        GlobalRWLockHandlers.ReleaseHandler = MpReleaseOldRWLock;
        
        ndisVersion = NdisGetVersion();
        if (ndisVersion > MP_NDIS_VERSION_NEEDS_COMPATIBILITY)        
        {
            // NDIS 6.2 or above. Use the new API

            // Load the address of the new NDisRead/Write lock API
            NdisInitUnicodeString(&allocateRoutineName, L"NdisAllocateRWLock");
            
            NdisInitUnicodeString(&freeRoutineName, L"NdisFreeRWLock");
            NdisInitUnicodeString(&acquireReadRoutineName, L"NdisAcquireRWLockRead");
            NdisInitUnicodeString(&acquireWriteRoutineName, L"NdisAcquireRWLockWrite");
            NdisInitUnicodeString(&releaseRoutineName, L"NdisReleaseRWLock");
            
            *((PVOID *)&MpNdisAllocateRWLock) = NdisGetRoutineAddress(&allocateRoutineName);
            if (MpNdisAllocateRWLock == NULL)
            {
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to find address of NdisAllocateRWLock\n"));
                break;                
            }

            *((PVOID *)&MpNdisFreeRWLock) = NdisGetRoutineAddress(&freeRoutineName);
            if (MpNdisFreeRWLock == NULL)
            {
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to find address of NdisFreeRWLock\n"));
                break;                
            }

            *((PVOID *)&MpNdisAcquireRWLockRead) = NdisGetRoutineAddress(&acquireReadRoutineName);
            if (MpNdisAcquireRWLockRead == NULL)
            {
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to find address of NdisAcquireRWLockRead\n"));
                break;                
            }

            *((PVOID *)&MpNdisAcquireRWLockWrite) = NdisGetRoutineAddress(&acquireWriteRoutineName);
            if (MpNdisAcquireRWLockWrite == NULL)
            {
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to find address of NdisAcquireRWLockWrite\n"));
                break;                
            }

            *((PVOID *)&MpNdisReleaseRWLock) = NdisGetRoutineAddress(&releaseRoutineName);
            if (MpNdisReleaseRWLock == NULL)
            {
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to find address of NdisReleaseRWLock\n"));
                break;                
            }

            // Set our pointers
            GlobalRWLockHandlers.AllocateHandler = MpAllocateNewRWLock;
            GlobalRWLockHandlers.FreeHandler = MpFreeNewRWLock;
            GlobalRWLockHandlers.AcquireReadHandler = MpAcquireNewRWLockForRead;
            GlobalRWLockHandlers.AcquireWriteHandler = MpAcquireNewRWLockForWrite;
            GlobalRWLockHandlers.ReleaseHandler = MpReleaseNewRWLock;

        }
    }while (FALSE);

    return ndisStatus;
}

