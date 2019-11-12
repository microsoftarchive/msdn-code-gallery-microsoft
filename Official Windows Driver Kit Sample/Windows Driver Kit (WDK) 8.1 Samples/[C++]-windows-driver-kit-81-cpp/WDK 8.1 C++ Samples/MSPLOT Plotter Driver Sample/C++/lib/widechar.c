/*++

Copyright (c) 1990-2003  Microsoft Corporation


Module Name:

    widechar.c


Abstract:

    This module contains all NLS unicode / ansi translation code


Author:

    18-Nov-1993 Thu 08:21:37 created  


[Environment:]

    GDI Device Driver - Plotter.


[Notes:]


Revision History:


--*/

#include "precomp.h"
#pragma hdrstop



_Success_(return != NULL)
LPWSTR
str2Wstr(
    _Out_writes_(cchDest) LPWSTR  pwStr,
                          size_t  cchDest,
                          LPCSTR  pbStr
    )

/*++

Routine Description:

    This function copies a NULL-terminated ANSI string to an
    equivalent NULL-terminated Unicode string

Arguments:

    pwStr   - Destination string location
    cchDest - Size of destination string, should be atleast (strlen(pstr) + 1) * sizeof(WCHAR)
    pbStr   - Source string location

Return Value:

    pwStr

Author:

    18-Nov-1993 Thu 08:36:00 created  

Revision History:


--*/

{
    size_t    cch;

    if (NULL == pbStr || NULL == pwStr)
    {
        return NULL;
    }

    //
    // Make sure that the size of dest buffer is large enough.
    //
    if (SUCCEEDED(StringCchLengthA(pbStr, cchDest, &cch)))
    {
        AnsiToUniCode(pbStr, pwStr, (int)cch);

        // Force NULL terminator
        pwStr[cch] = 0;
        return pwStr;
    }
    else
    {
        return NULL;
    }
}


_Success_(return != NULL)
LPSTR
WStr2Str(
    _Out_writes_(cchDest) LPSTR   pbStr,
                          size_t  cchDest,
                          LPCWSTR pwStr
    )

/*++

Routine Description:

    This function copies a NULL-terminated Unicode string to an
    equivalent NULL-terminated ANSI string

Arguments:

    pbStr   - Destination string location
    cchDest - Size of destination string, should be atleast wcslen(pwStr) + 1
    pwStr   - Source string location


Return Value:

    pbStr


Author:

    06-Dec-1993 Mon 13:06:12 created  

Revision History:


--*/

{

    size_t    cch;

    if (NULL == pbStr || NULL == pwStr)
    {
        return NULL;
    }

    if (SUCCEEDED(StringCchLengthW(pwStr, cchDest, &cch)))
    {
        UniCodeToAnsi(pbStr, pwStr, (int)cch);

        // Force NULL terminator
        pbStr[cch] = 0;

        return pbStr;
    }
    else
    {
        return NULL;
    }
}


