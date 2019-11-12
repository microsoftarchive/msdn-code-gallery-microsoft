/*++
Copyright (c) 1990-2003  Microsoft Corporation
All Rights Reserved


Abstract:

    Support routines for WinPrint.

--*/

/*++
*******************************************************************
    G e t P r i n t e r I n f o

    Routine Description:
        This routine allocates the required memory for a
        PRINTER_INFO_? structure and retrieves the information
        from NT.  This returns a pointer to the structure, which
        must be freed by the calling routine.

    Arguments:
                hPrinter    HANDLE to the printer the job is in
                StructLevel The structure level to get
                pErrorCode   => field to place error, if one

    Return Value:
                PUCHAR => buffer where devmode info is if okay
                NULL if error - pErrorCode returns error
*******************************************************************
--*/

#include "local.h"

PUCHAR
GetPrinterInfo(IN  HANDLE   hPrinter,
               IN  ULONG    StructLevel,
               OUT PULONG   pErrorCode)
{
    ULONG   reqbytes, alloc_size;
    PUCHAR  ptr_info, pTempPtr_info = NULL;
    USHORT  retry = 2;

    alloc_size = BASE_PRINTER_BUFFER_SIZE;

    ptr_info = (PUCHAR)AllocSplMem(alloc_size);

    if (!ptr_info) {
        *pErrorCode = ERROR_NOT_ENOUGH_MEMORY;
        return NULL;
    }

    while (retry--) {

        if (GetPrinter(
              hPrinter,
              StructLevel,
              (PUCHAR)ptr_info,
              alloc_size,
              &reqbytes) == TRUE) {

            *pErrorCode = 0;
            return (PUCHAR)ptr_info;
        }

        //
        // GetPrinter failed - if not because of insufficient buffer, fail
        // the call.  Otherwise, up our hint, re-allocate and try again.
        //

        *pErrorCode = GetLastError();

        if (*pErrorCode != ERROR_INSUFFICIENT_BUFFER) {
            FreeSplMem(ptr_info);
            return NULL;
        }

        //
        // Reallocate the buffer and re-try (note that, because we
        // allocated the buffer as LMEM_FIXED, the LMEM_MOVABLE does
        // not return a movable allocation, it just allows realloc
        // to return a different pointer.
        //

        FreeSplMem(ptr_info);
        ptr_info      = NULL;
        pTempPtr_info = (PUCHAR)AllocSplMem(reqbytes);
        alloc_size    = reqbytes;

        if (pTempPtr_info) {

            ptr_info = pTempPtr_info;
            pTempPtr_info = NULL;
        } else {
            //
            // If the realloc failed we break out of the loop so that we
            // can cleanup and return error code.
            //
            break;
        }

    }

    if (ptr_info) {
        FreeSplMem(ptr_info);
    }

    *pErrorCode = ERROR_NOT_ENOUGH_MEMORY;
    return NULL;
}



