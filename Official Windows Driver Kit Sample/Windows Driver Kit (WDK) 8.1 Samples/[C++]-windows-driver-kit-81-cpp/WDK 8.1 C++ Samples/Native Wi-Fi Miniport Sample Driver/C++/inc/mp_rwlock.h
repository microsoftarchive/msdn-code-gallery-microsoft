/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_rwlock.h

Abstract:
    Contains defines for the read write lock
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

/**
 * This is read write lock structure
 */
typedef struct _MP_READ_WRITE_LOCK
{
    /** Old Read/Write lock structure. Used with NDIS 6.0 */
    NDIS_RW_LOCK        OldRWLock;

    /** New Read/Write lock structure */
    PNDIS_RW_LOCK_EX    NewRWLock;
}MP_READ_WRITE_LOCK, *PMP_READ_WRITE_LOCK;

/**
 * This is read write lock state
 */
typedef struct _MP_RW_LOCK_STATE
{
    /** Old Read/Write lock state. Used with NDIS 6.2*/
    LOCK_STATE          OldLockState;

    /** New Read/Write lock state */
    LOCK_STATE_EX       NewLockState;

    /** My variable tracking current lock state */
    BOOLEAN             ReadLock;
    
}MP_RW_LOCK_STATE, *PMP_RW_LOCK_STATE;

typedef NDIS_STATUS
(*MP_RW_LOCK_ALLOCATE)(
    PMP_READ_WRITE_LOCK     Lock,
    NDIS_HANDLE             Handle    
    );

typedef VOID
(*MP_RW_LOCK_FREE)(
    PMP_READ_WRITE_LOCK     Lock    
    );
    
typedef VOID 
(*MP_RW_LOCK_ACQUIRE_FOR_READ)(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    );

typedef VOID
(*MP_RW_LOCK_ACQUIRE_FOR_WRITE)(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    );

typedef VOID
(*MP_RW_LOCK_RELEASE)(
    PMP_READ_WRITE_LOCK     Lock,
    PMP_RW_LOCK_STATE       LockState
    );

/** Table containinng the RW lock handling functions */
typedef struct _MP_RW_LOCK_HANDLERS
{
    MP_RW_LOCK_ALLOCATE     AllocateHandler;
    MP_RW_LOCK_FREE         FreeHandler;
    MP_RW_LOCK_ACQUIRE_FOR_READ AcquireReadHandler;
    MP_RW_LOCK_ACQUIRE_FOR_WRITE    AcquireWriteHandler;
    MP_RW_LOCK_RELEASE      ReleaseHandler;
    
}MP_RW_LOCK_HANDLERS, *PMP_RW_LOCK_HANDLERS;

/** Current handlers list */
MP_RW_LOCK_HANDLERS         GlobalRWLockHandlers;

NDIS_STATUS
MpDetermineRWLockType();


#define MP_ALLOCATE_READ_WRITE_LOCK(_Lock, _Handle)      \
    GlobalRWLockHandlers.AllocateHandler(_Lock, _Handle)

#define MP_FREE_READ_WRITE_LOCK(_Lock)          \
    GlobalRWLockHandlers.FreeHandler(_Lock)

#define MP_ACQUIRE_READ_LOCK(_Lock, _LockState) \
    GlobalRWLockHandlers.AcquireReadHandler(_Lock, _LockState)

#define MP_ACQUIRE_WRITE_LOCK(_Lock, _LockState)\
    GlobalRWLockHandlers.AcquireWriteHandler(_Lock, _LockState)
                
#define MP_RELEASE_READ_LOCK(_Lock, _LockState) \
    GlobalRWLockHandlers.ReleaseHandler(_Lock, _LockState)
                
#define MP_RELEASE_WRITE_LOCK(_Lock, _LockState)\
    GlobalRWLockHandlers.ReleaseHandler(_Lock, _LockState)

