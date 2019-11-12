/*++

Copyright (c) 1998  Microsoft Corporation

Module Name: 

    debug.c
--*/

#define _DEBUG_C
#include "pch.h"
#undef _DEBUG_C

void 
DbgPrt(
    _In_ HANDLE   hWnd,
    _In_ PSTR     lpszFormat,
    ... 
    )
{
    char    buf[STRING_SIZE] = {"\0"}; // = "WIN1394: ";
    HRESULT hr = S_OK; 

    va_list ap;

    va_start(ap, lpszFormat);

    hr = StringCbVPrintf( &buf[0] , (STRING_SIZE * sizeof(buf[0])) , lpszFormat, ap);
    if (FAILED(hr))
    {
        // not much we can do here for logging, but we shouldn't try to send
        // the results of a failure down to the output streams.
        return;
    }

#if defined(DBG)
    OutputDebugStringA(buf);
#endif

    if (hWnd)
        WriteTextToEditControl(hWnd, buf);

    va_end(ap);
}


