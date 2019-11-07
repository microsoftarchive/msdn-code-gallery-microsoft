//
//  Windows (Printing) Driver Development Kit Samples
//
//  Sample Print Provider template.
//
//  misc.c - Various handy utility functions.
//
//  Copyright (c) 1990 - 2005 Microsoft Corporation.
//  All Rights Reserved.
//
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//

#include "pp.h"

//
// Allocate's a buffer from the process heap and copies the specified NULL
// terminated UNICODE string to that buffer.
// The returned string will be NULL terminated.
//
// Returns a pointer to the new string buffer if successful.
// Returns NULL if an error occurred.
//
LPWSTR
AllocSplStr
(
    _In_ LPWSTR pStr
)

{
    LPWSTR pMem     = NULL;
    size_t uSize    = 0;

    if (!pStr)
    {
        return NULL;
    }

    uSize = wcslen(pStr) + 1;

    pMem = LocalAlloc(0, uSize * sizeof(WCHAR));

    if (pMem)
    {
        DWORD Status;

        Status = StatusFromHResult(StringCchCopy(pMem, uSize, pStr));

        if (Status != ERROR_SUCCESS)
        {
            LocalFree(pMem);

            pMem = NULL;

            SetLastError(Status);
        }
    }

    return pMem;
}


//
// Free's the string buffer obtained by calls to AllocSplStr().
//
VOID
FreeSplStr
(
   _In_ LPWSTR pStr
)

{
    if (pStr)
    {
        LocalFree(pStr);
    }
}


//
// Determines whether the passed in string is in the following UNC name format:
//   "\\servername\sharename".
//
// No attempt is made to validate the existence of the specified UNC name
// resource.
//
// Returns TRUE if the passed in name fits the UNC name format.
// Returns FALSE otherwise.
//
BOOL
ValidateUNCName
(
   _In_ LPWSTR pName
)

{
    if (   pName
       && (*pName++ == L'\\')
       && (*pName++ == L'\\')
       && (wcschr(pName, L'\\'))
       )
    {
        return TRUE;
    }

    return FALSE;
}


//
// Converts an HRESULT to Status format.
//
DWORD
StatusFromHResult(
    _In_ HRESULT hr
    )
{
    DWORD   Status = ERROR_SUCCESS;

    if (FAILED(hr))
    {
        if (HRESULT_FACILITY(hr) == FACILITY_WIN32)
        {
            Status = HRESULT_CODE(hr);
        }
        else
        {
            Status = hr;
        }
    }

    return Status;
}


//
// Returns TRUE if the passed in Status value indicates success.
// Returns FALSE and set the last error otherwise.
//
BOOL
BoolFromStatus(
    _In_ DWORD Status
    )
{
    BOOL bRet = TRUE;

    if (ERROR_SUCCESS != Status)
    {
        SetLastError(Status);

        bRet = FALSE;
    }

    return bRet;
}
