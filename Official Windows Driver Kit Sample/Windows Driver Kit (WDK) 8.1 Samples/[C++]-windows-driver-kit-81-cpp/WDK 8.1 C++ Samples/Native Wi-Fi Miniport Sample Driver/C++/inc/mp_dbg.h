/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_dbg.h

Abstract:
    Contains defines useful for driver debugging
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

#if DBG

//
//    ==============================
//    MEMORY MANAGEMENT DEBUG APIS
//    ==============================
//

#define  MP_HEADER_PATTERN          0x76028412
#define  MP_TRAILER_PATTERN         0x83395627
#define  MP_FREED_PATTERN           0x82962503

//
// Note: it's important that the size of MP_MEMORY_BLOCK structure
//       be multiple of 16 bytes.
//
typedef struct _MP_MEMORY_BLOCK
{
  /** Pointer to next and previous blocks */
  LIST_ENTRY  ListEntry;
  /** name of the file which is doing the allocation */
  PCHAR       File;
  /** pad to make the size of whole structure multiple of 16 bytes */
  PVOID       Pad;
  /** line number where allocated */
  ULONG       Line;
  /** ulong index of trailer (=(length/4)-1 relative to data start */
  ULONG       Length;
  /** Flags that can be used in future to do special allocations */
  ULONG       Flags;
  /** To help detect underflows. A trailer is also added to find overflows */
  ULONG       HeaderPattern;
} MP_MEMORY_BLOCK, *PMP_MEMORY_BLOCK;

PVOID 
MpAllocateMemory (
    NDIS_HANDLE         AllocateHandle,
    ULONG               Size,
    ULONG               Tag,
    EX_POOL_PRIORITY    Priority,
    _In_ PSTR           FileName,
    ULONG               LineNumber,
    ULONG               Flags
    );

VOID
MpFreeMemory (
    _In_  __drv_freesMem(Mem) PVOID Memory
    );

VOID 
MpFreeAllocatedBlocks();

BOOLEAN
MpDebugMatchPacketType(
    _In_reads_bytes_(sizeof(DOT11_DATA_LONG_HEADER))  PUCHAR                  PacketHeader,
    _In_  UCHAR                   Type,
    _In_  UCHAR                   SubType,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  PDOT11_MAC_ADDRESS      DestinationAddress,
    _In_reads_bytes_opt_(DOT11_ADDRESS_SIZE)  PDOT11_MAC_ADDRESS      SourceAddress
    );

/**
* Use this macro to allocate memory whenever needed. For debug versions of the driver,
* this macro expands to MpAllocateMemory and tracks all memory allocated with detection
* of overflow and underflow. For retail builds, this macro expands to a direct call to Ndis for
* memory allocation.
* 
* \param _Memory        Pointer to the memory allocated will be placed here
* \param _SizeInBytes   The number of bytes to allocate
* \param _Priority        The priority of the allocation
* \sa MP_FREE_MEMORY, MpAllocateMemory, NdisAllocateMemoryWithTag
*/
#define  MP_ALLOCATE_MEMORY_WITH_PRIORITY(_NdisHandle, _Memory, _SizeInBytes, _Tag, _Priority)        \
    *_Memory = MpAllocateMemory(_NdisHandle, _SizeInBytes, _Tag, _Priority, __FILE__, __LINE__, 0)


#define MP_ALLOCATE_MEMORY(_NdisHandle, _Memory, _SizeInBytes, _Tag)       \
    MP_ALLOCATE_MEMORY_WITH_PRIORITY(_NdisHandle, _Memory, _SizeInBytes, _Tag, NormalPoolPriority)

/**
* Use this macro to free memory previously allocated with MP_ALLOCATE_MEMORY. For debug
* versions of the driver, this macro expands to MpFreeMemory which checks for memory corruption,
* bad frees and other special memory operations. For retail builds, it directly macros to NDIS API for
* freeing memory.
* 
* \param _Memory      Memory to free. Must have been allocated using MP_ALLOCATE_MEMORY
* \sa MP_ALLOCATE_MEMORY, MpFreeMemory, NdisFreeMemory
*/
#define MP_FREE_MEMORY(_Memory)                                     \
    MpFreeMemory(_Memory); _Memory = NULL;

#define MP_DUMP_LEAKING_BLOCKS()        MpFreeAllocatedBlocks()

//
//    =========================
//    DEBUGGER BREAK/ASSERT APIS
//    =========================
//


// Break if expression returns false
#define MPASSERT(_Exp)                          NT_ASSERT(_Exp)

// Break with specified message if expression return false

//__WFUNCTION__ added because NT_ASSERTMSG only accepts wide strings
#define WIDEN2(X) L ## X
#define WIDEN(X) WIDEN2(X)
#define __WFUNCTION__ WIDEN(__FUNCTION__)

#define MPASSERTMSG(_Msg, _Exp)                 NT_ASSERTMSG("" __WFUNCTION__ L": " L#_Msg, _Exp)

// Break if operation returns false
#define MPASSERTOP(_Op)                         NT_ASSERT(_Op)

//
// These macros help verify that calls to a function is mutually exclusive
// We need these to verify that we are serializing calls to Hw11TransmitPacket
// as guaranteed. Since a lock is not protecting the region, this mechanism will
// help verify the behavior and catch potential synchronization issues
// Please note, these macros DO NOT guarantee to catch if mutual exclusion is
// violated but could potentially catch the issue if it occurs
//
typedef     LONG   MUTUAL_EXCLUSION_VERIFIER;
#define MP_ENTER_MUTEX_REGION(MutexVerifier)
#define MP_LEAVE_MUTEX_REGION(MutexVerifier)

// IRQL verification macros
#define MP_VERIFY_PASSIVE_IRQL()      \
    MPASSERTMSG("Driver expected IRQL to be passive. Use !irql to check current Irql\n", (NDIS_CURRENT_IRQL() == 0))


#else   // DBG

//
// Non-debug versions of the above macros
//

#define MP_ALLOCATE_MEMORY_WITH_PRIORITY(_NdisHandle, _Memory, _SizeInBytes, _Tag, _Priority)  \
    *_Memory = NdisAllocateMemoryWithTagPriority (                                \
     _NdisHandle,                                                                 \
     _SizeInBytes,                                                                \
     _Tag,                                                                        \
     _Priority                                                                    \
     )

#define  MP_ALLOCATE_MEMORY(_NdisHandle, _Memory, _SizeInBytes, _Tag)                \
    MP_ALLOCATE_MEMORY_WITH_PRIORITY(_NdisHandle, _Memory, _SizeInBytes, _Tag, NormalPoolPriority)

#define MP_FREE_MEMORY(_Memory)                    \
    NdisFreeMemory(_Memory, 0, 0); _Memory = NULL;

#define MP_DUMP_LEAKING_BLOCKS()

#define MPASSERT(_Exp)
#define MPASSERTMSG(_Msg, _Exp)
#define MPASSERTOP(_Op)                         (_Op)

#define MP_ENTER_MUTEX_REGION(MutexVerifier)
#define MP_LEAVE_MUTEX_REGION(MutexVerifier)

#define MP_VERIFY_PASSIVE_IRQL()


#endif  // !DBG
