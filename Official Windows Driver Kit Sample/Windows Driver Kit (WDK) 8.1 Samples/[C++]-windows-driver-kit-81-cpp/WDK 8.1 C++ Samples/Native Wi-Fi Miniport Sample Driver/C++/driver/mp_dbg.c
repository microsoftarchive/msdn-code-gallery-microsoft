/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_dbg.c

Abstract:
    Implements helper routines useful for driver debugging
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "mp_dbg.tmh"
#endif

#if DBG

/**
 * This SpinLock is used to synchronize the list of allocated memory blocks.
 * This is a debug version only feature
 */
NDIS_SPIN_LOCK      GlobalMemoryLock;

/**
 * This is a list of all the memory allocations that were made by the driver.
 * This is a debug version only feature.
 */
LIST_ENTRY          GlobalMemoryList;

/** 
 * This variable is set if the memory mananger has been initialized.
 * This is available only for debug version of the driver
 */
BOOLEAN             GlobalMemoryManagerInitialized = FALSE;

/**
 * This is a debug only function that performs memory management operations for us.
 * Memory allocated using this function is tracked, flagged when leaked, and caught for
 * overflows and underflows.
 * \warning Do not use this function directly. Using MP_ALLOCATE_MEMORY ensures that
 * this function gets called for debug version of the driver. Retail builds will use Ndis API
 * for allocation of memory
 * 
 * \param Size            The size in bytes of memory to allocate
 * \param FileName  The full path of file where this function is invoked from
 * \param LineNumber The line number in the file where this method was called from
 * \param Flags           Flags for special memory insturctions. Currently unused.
 * \return Pointer to the allocated memory or NULL in case of a failure
 * \sa MpFreeMemory, MP_ALLOCATE_MEMORY, MP_FREE_MEMORY, NdisAllocateMemoryWithTagPriority, MpFreeAllocatedBlocks
 */
PVOID 
MpAllocateMemory (
    NDIS_HANDLE           AllocateHandle,
    ULONG                 Size,
    ULONG                 Tag,
    EX_POOL_PRIORITY      Priority,
    _In_ PSTR             FileName,
    ULONG                 LineNumber,
    ULONG                 Flags
    )
{
    PVOID   memory = NULL;

    //
    // If the memory manager has not been initialized, do so now
    //
    if (!GlobalMemoryManagerInitialized)
    {
        //
        // NOTE: If two thread allocate the very first allocation simultaneously
        // it could cause double initialization of the memory manager. This is a
        // highly unlikely scenario and will occur in debug versions only.
        //
        NdisAllocateSpinLock(&GlobalMemoryLock);
        InitializeListHead(&GlobalMemoryList);
        GlobalMemoryManagerInitialized = TRUE;
    }

    //
    // Allocate the required memory chunk plus header and trailer bytes
    //
    memory = NdisAllocateMemoryWithTagPriority(
        AllocateHandle,
        Size + sizeof(MP_MEMORY_BLOCK) + sizeof(ULONG),
        Tag,
        Priority
        );

    if (memory != NULL)
    {
        //
        // Memory allocation succeeded. Add information about the allocated
        // block in the list that tracks all allocations.
        //
        PMP_MEMORY_BLOCK  memoryBlockHeader;

        // Fill in the memory header and trailer
        memoryBlockHeader = (PMP_MEMORY_BLOCK) memory;
        memoryBlockHeader->File = FileName;
        memoryBlockHeader->Line = LineNumber;
        memoryBlockHeader->Length = Size;
        memoryBlockHeader->Flags = Flags;
        memoryBlockHeader->HeaderPattern = MP_HEADER_PATTERN;
        *((PULONG) (((PUCHAR)(memory))+Size + sizeof(MP_MEMORY_BLOCK))) = MP_TRAILER_PATTERN;

        // Jump ahead by memory header so pointer returned to caller points at the right place
        memory = ((PUCHAR)memory) + sizeof (MP_MEMORY_BLOCK);

        // Store a reference to this block in the list
        NdisAcquireSpinLock (&GlobalMemoryLock);
        InsertHeadList (&GlobalMemoryList, &memoryBlockHeader->ListEntry);
        NdisReleaseSpinLock (&GlobalMemoryLock);
    }

    return memory;
}



/**
 * This routine is called to free memory which was previously allocated using MpAllocateMemory function.
 * Before freeing the memory, this function checks and makes sure that no overflow or underflows have
 * happened and will also try to detect multiple frees of the same memory chunk.
 * 
 * \warning Do not use this function directly. Using MP_FREE_MEMORY ensures that
 * this function gets called for debug version of the driver. Retail builds will use Ndis API
 * for freeing of memory
 * \param Memory    Pointer to memory allocated using MP_FREE_MEMORY
 * \sa MpAllocateMemory, MP_ALLOCATE_MEMORY, MP_FREE_MEMORY, NdisFreeMemory, MpFreeAllocatedBlocks
 */
VOID 
MpFreeMemory (
    _In_  __drv_freesMem(Mem) PVOID Memory
    )
{
    PMP_MEMORY_BLOCK  memoryBlockHeader;

    MPASSERTMSG ("NULL memory being freed", Memory);
    MPASSERTMSG ("Allocated blocks list is empty. This is an extra free\n", !IsListEmpty(&GlobalMemoryList));

    // Jump back by memory header size so we can get to the header
    memoryBlockHeader = (PMP_MEMORY_BLOCK) (((PUCHAR)Memory) - sizeof(MP_MEMORY_BLOCK));

    //
    // Check that header was not corrupted
    //
    if (memoryBlockHeader->HeaderPattern != MP_HEADER_PATTERN)
    {
        if (memoryBlockHeader->HeaderPattern == MP_FREED_PATTERN)
        {
            MpTrace(COMP_DBG, DBG_SERIOUS, ("Possible double free of memory block at %p\n", memoryBlockHeader));
        }
        else
        {
            MpTrace(COMP_DBG, DBG_SERIOUS, ("Memory corruption due to underflow detected at memory block %p\n", memoryBlockHeader));
        }

        MpTrace(COMP_DBG, DBG_SERIOUS, ("Dumping information about memory block. This information may itself have been corrupted and could cause machine to bugcheck.\n"));
        MpTrace(COMP_DBG, DBG_SERIOUS, ("Memory was allocated from %s at line %d\n", memoryBlockHeader->File, memoryBlockHeader->Length));
        MPASSERT (FALSE);
    }

    //
    // Now corrupt the header so that double frees will fail.
    // Note simultaneous frees of same memory will not get caught this way!
    //
    memoryBlockHeader->HeaderPattern = MP_FREED_PATTERN;

    //
    // Check that trailer was not corrupted
    //
    if (*(PULONG) ((PUCHAR)Memory + memoryBlockHeader->Length) != MP_TRAILER_PATTERN)
    {
        MpTrace(COMP_DBG, DBG_SERIOUS, ("Memory corruption due to overflow detected at %p\n", Memory));
        MpTrace(COMP_DBG, DBG_SERIOUS, ("Dumping information about memory block. This information may itself have been corrupted and could cause machine to bugcheck.\n"));
        MpTrace(COMP_DBG, DBG_SERIOUS, ("Memory was allocated from %s at line %d\n", memoryBlockHeader->File, memoryBlockHeader->Length));
        MPASSERT (FALSE);
    }

    //
    // Remove this memory block from the list of allocations
    //
    NdisAcquireSpinLock (&GlobalMemoryLock);
    RemoveEntryList (&memoryBlockHeader->ListEntry);
    NdisReleaseSpinLock (&GlobalMemoryLock);

    //
    // Zero out data and trailer and then free the memory chunk back to the OS
    //
    NdisZeroMemory (Memory, sizeof (ULONG) + (memoryBlockHeader->Length));
    NdisFreeMemory (memoryBlockHeader, 0, 0);
}


/**
 * This function will dump out the contents of the GlobalMemoryList and free them as well.
 * Used to dump out leaking memory when driver is exiting
 * 
 * \sa MpAllocateMemory, MpFreeMemory
 */
VOID 
MpFreeAllocatedBlocks ()
{
    if (GlobalMemoryManagerInitialized)
    {
        NdisAcquireSpinLock (&GlobalMemoryLock);   
        if (!IsListEmpty (&GlobalMemoryList))
        {
            PLIST_ENTRY currentEntry;
            PMP_MEMORY_BLOCK currentMemory;

            while (!IsListEmpty (&GlobalMemoryList))
            {
                currentEntry = RemoveHeadList(&GlobalMemoryList);

                currentMemory = CONTAINING_RECORD (currentEntry, MP_MEMORY_BLOCK, ListEntry);
                MpTrace(COMP_DBG, DBG_SERIOUS, ("LEAK in %s on line %u\n", currentMemory->File,
                                      currentMemory->Line));

                NdisFreeMemory (currentMemory, 0, 0);
            }
        }
        NdisReleaseSpinLock (&GlobalMemoryLock);

        NdisFreeSpinLock(&GlobalMemoryLock);
        GlobalMemoryManagerInitialized = FALSE;
    }
}



/**
 * Internal debugging helper function
 */
BOOLEAN
MpDebugMatchPacketType(
    _In_reads_bytes_(sizeof(DOT11_DATA_LONG_HEADER))  PUCHAR                  PacketHeader,
    _In_  UCHAR                   Type,
    _In_  UCHAR                   SubType,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  PDOT11_MAC_ADDRESS      DestinationAddress,
    _In_reads_bytes_opt_(DOT11_ADDRESS_SIZE)  PDOT11_MAC_ADDRESS      SourceAddress
    )
{
    PDOT11_DATA_LONG_HEADER     macHeader = (PDOT11_DATA_LONG_HEADER)PacketHeader;
    PUCHAR                      originalSource, finalDestination;

    if (Type != macHeader->FrameControl.Type)
    {
        return FALSE;
    }
    
    if (SubType != macHeader->FrameControl.Subtype)
    {
        return FALSE;
    }

    // Determine the location of the source & destination address
    if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_DATA)
    {
        if (macHeader->FrameControl.ToDS)
        {
            if (macHeader->FrameControl.FromDS)
            {
                // ToDS = 1, FromDS = 1 RA TA DA SA
                finalDestination = macHeader->Address3;
                originalSource = macHeader->Address4;
            }
            else
            {
                // ToDS = 1, FromDS = 0 BSSID SA DA
                finalDestination = macHeader->Address3;
                originalSource = macHeader->Address2;
            }
        }
        else
        {
            if (macHeader->FrameControl.FromDS)
            {
                // ToDS = 0, FromDS = 1 DA BSSID SA
                finalDestination = macHeader->Address1;
                originalSource = macHeader->Address3;
            }
            else
            {
                // ToDS = 0, FromDS = 0 DA SA BSSID
                finalDestination = macHeader->Address1;
                originalSource = macHeader->Address2;
            }
        }        
    }
    else if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT)
    {
        // Management packets
        finalDestination = macHeader->Address1;
        originalSource = macHeader->Address2;
    }
    else
    {
        // Control, we only check one address
        finalDestination = macHeader->Address1;
        originalSource = macHeader->Address1;
    }

    // Compare destination address
    if (DestinationAddress != NULL)
    {
        if (!MP_COMPARE_MAC_ADDRESS(finalDestination, DestinationAddress))
        {
            return FALSE;
        }
    }

    // If applicable, compare source address
    if ((Type != DOT11_FRAME_TYPE_CONTROL) && (SourceAddress != NULL))
    {
        if (!MP_COMPARE_MAC_ADDRESS(originalSource, SourceAddress))
        {
            return FALSE;
        }
    }

    return TRUE;
}


#if !DOT11_TRACE_ENABLED

//
// If tracing is not turned on, Initialize these global macro
//
ULONG       GlobalDebugLevel = DBG_LOUD;
ULONG       GlobalDebugComponents = COMP_INIT_PNP | COMP_ASSOC;


/**
 * This helper function uses NDIS API to read debug mask information from the
 * registry (and uses defaults it not available in the registry)
 *
 * \param pAdapter          The adapter whose configuration will be read
 * \sa 
 */
VOID 
MpReadGlobalDebugMask(
    NDIS_HANDLE  NdisMiniportDriverHandle
    )
{
    NDIS_STATUS                     ndisStatus = NDIS_STATUS_SUCCESS;
    PNDIS_CONFIGURATION_PARAMETER   parameter = NULL;
    NDIS_HANDLE                     registryConfigurationHandle;
    NDIS_CONFIGURATION_OBJECT       configObject;
    NDIS_STRING                     debugLevelRegName = NDIS_STRING_CONST("MPGlobalDebugLevel");
    NDIS_STRING                     debugComponentRegName = NDIS_STRING_CONST("MPGlobalDebugComponents");
    
    configObject.Header.Type = NDIS_OBJECT_TYPE_CONFIGURATION_OBJECT;
    configObject.Header.Revision = NDIS_CONFIGURATION_OBJECT_REVISION_1;
    configObject.Header.Size = sizeof( NDIS_CONFIGURATION_OBJECT );
    configObject.NdisHandle = NdisMiniportDriverHandle;
    configObject.Flags = 0;

    ndisStatus = NdisOpenConfigurationEx(
                    &configObject,
                    &registryConfigurationHandle
                    );

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to Open Configuration. Will revert to defaults for all values\n"));

        // Defaults are globals        
        return;
    }

    //
    // Read GlobalDebugLevel
    //
    NdisReadConfiguration(
        &ndisStatus,
        &parameter,
        registryConfigurationHandle,
        &debugLevelRegName,
        NdisParameterInteger
        );
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // We dont do strict verification of data (as long as its an integer)
        GlobalDebugLevel = parameter->ParameterData.IntegerData;
        MpTrace (COMP_INIT_PNP, DBG_NORMAL, ("GlobalDebugLevel = 0x%x\n", GlobalDebugLevel));
    }

    //
    // Read GlobalDebugComponents
    //
    NdisReadConfiguration(
        &ndisStatus,
        &parameter,
        registryConfigurationHandle,
        &debugComponentRegName,
        NdisParameterInteger
        );
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // We dont do strict verification of data (as long as its an integer)
        GlobalDebugComponents = parameter->ParameterData.IntegerData;
        MpTrace (COMP_INIT_PNP, DBG_NORMAL, ("GlobalDebugComponents = 0x%x\n", GlobalDebugComponents));
    }

    //
    // Close the handle to the registry
    //
    if (registryConfigurationHandle)
    {
        NdisCloseConfiguration(registryConfigurationHandle);
    }
}
    
#endif  // !DOT11_TRACE_LEVEL

#endif  // DBG

