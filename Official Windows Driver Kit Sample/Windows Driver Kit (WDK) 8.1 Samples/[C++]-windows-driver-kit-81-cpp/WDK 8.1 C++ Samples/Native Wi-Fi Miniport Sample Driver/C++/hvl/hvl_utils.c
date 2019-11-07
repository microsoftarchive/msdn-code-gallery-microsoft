/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hvl_utils.c

Abstract:
    Implements helper routines for the HVL
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "hvl_utils.tmh"
#endif

extern
__inline
NDIS_STATUS
AllocateMemory(NDIS_HANDLE ndisHandle, ULONG ulSize, PVOID *ppvMem)
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVOID pvMem = NULL;

    ASSERT(ppvMem);

    do
    {
        *ppvMem = NULL;
        
        MP_ALLOCATE_MEMORY(ndisHandle, &pvMem, ulSize, HVL_MEMORY_TAG);
        if (pvMem == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            MpTrace(COMP_HVL, DBG_SERIOUS, ("Failed to allocate %d bytes", ulSize));
            break;
        }

        // Clear everything
        NdisZeroMemory(pvMem, ulSize);        
    
        *ppvMem = pvMem;
    } while (FALSE);
    
    return ndisStatus;
}

extern
__inline
VOID
FreeMemory(PVOID pvMem)
{
    if (NULL != pvMem)
    {
        MP_FREE_MEMORY(pvMem);
    }
}

