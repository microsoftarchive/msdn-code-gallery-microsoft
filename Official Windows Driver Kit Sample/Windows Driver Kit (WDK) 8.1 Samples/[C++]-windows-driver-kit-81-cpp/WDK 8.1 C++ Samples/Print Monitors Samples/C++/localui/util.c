/*++

Copyright (c) 1990-2003 Microsoft Corporation
All Rights Reserved

Module Name:

    util.c

Abstract:

    This module provides all the utility functions for localui.

--*/

#include "precomp.h"
#pragma hdrstop

#include "localui.h"
#include "mem.h"
#include <intsafe.h>

PWSTR
ConstructXcvName(
    PCWSTR pServerName,
    PCWSTR pObjectName,
    PCWSTR pObjectType
    )
{
    size_t  cbSizeToAlloc = 0;
    size_t  cchOutput = 0;
    PWSTR   pOut = NULL;

    cchOutput = pServerName ? (wcslen(pServerName) + 2) : 1;   /* "\\Server\," */
    cchOutput += wcslen(pObjectType) + 2;                        /* "\\Server\,XcvPort _" */
    cchOutput += pObjectName ? wcslen(pObjectName) : 0;      /* "\\Server\,XcvPort Object_" */
    if (FAILED(SizeTMult(cchOutput, sizeof (pOut [0]), &cbSizeToAlloc)) || (cbSizeToAlloc > DWORD_MAX))
    {
        return NULL;
    }

    pOut = (PWSTR)AllocSplMem(cbSizeToAlloc);
    if (pOut)
    {
        if (pServerName)
        {
            (VOID) StringCchCopy(pOut, cchOutput, pServerName);
            (VOID) StringCchCat (pOut, cchOutput, L"\\,");
        }
        else
        {
            (VOID) StringCchCopy (pOut, cchOutput, L",");
        }

        (VOID) StringCchCat (pOut, cchOutput, pObjectType);
        (VOID) StringCchCat (pOut, cchOutput, L" ");

        if (pObjectName)
        {
            (VOID) StringCchCat (pOut, cchOutput, pObjectName);
        }
    }

    return pOut;
}


BOOL
IsCOMPort(
    PCWSTR pPort
    )
{
    //
    // Must begin with szCom
    //
    if ( _wcsnicmp( pPort, szCOM, 3 ) )
    {
        return FALSE;
    }

    //
    // wcslen guarenteed >= 3
    //
    return pPort[ wcslen( pPort ) - 1 ] == L':';
}

BOOL
IsLPTPort(
    PCWSTR pPort
    )
{
    //
    // Must begin with szLPT
    //
    if ( _wcsnicmp( pPort, szLPT, 3 ) )
    {
        return FALSE;
    }

    //
    // wcslen guarenteed >= 3
    //
    return pPort[ wcslen( pPort ) - 1 ] == L':';
}




/* Message
 *
 * Displays a message by loading the strings whose IDs are passed into
 * the function, and substituting the supplied variable argument list
 * using the varargs macros.
 *
 */
INT
WINAPIV
Message(
    HWND    hwnd,
    DWORD   Type,
    INT     CaptionID,
    INT     TextID,
    ...
    )
{
    WCHAR   MsgText[2*MAX_PATH + 1];
    WCHAR   MsgFormat[256];
    WCHAR   MsgCaption[40];
    va_list vargs;

    if( ( LoadString( hInst, TextID, MsgFormat,
                      sizeof MsgFormat / sizeof *MsgFormat ) > 0 )
     && ( LoadString( hInst, CaptionID, MsgCaption,
                      sizeof MsgCaption / sizeof *MsgCaption ) > 0 ) )
    {
        va_start( vargs, TextID );
        (VOID) StringCchVPrintf ( MsgText, COUNTOF(MsgText), MsgFormat, vargs );
        va_end( vargs );

        MsgText[COUNTOF(MsgText) - 1] = L'\0';

        return MessageBox(hwnd, MsgText, MsgCaption, Type);
    }
    else
    {
        return 0;
    }
}


INT
ErrorMessage(
    HWND hwnd,
    DWORD dwStatus
    )
{
    WCHAR   MsgCaption[MAX_PATH];
    PWSTR   pBuffer = NULL;
    INT     iRet = 0;

    FormatMessage(  FORMAT_MESSAGE_FROM_SYSTEM |
                    FORMAT_MESSAGE_ALLOCATE_BUFFER,
                    NULL,
                    dwStatus,
                    0,
                    (PWSTR) &pBuffer,
                    0,
                    NULL);

    if (pBuffer)
    {
        if (LoadString( hInst, IDS_LOCALMONITOR, MsgCaption,
                  sizeof MsgCaption / sizeof *MsgCaption) > 0)
        {

             iRet = MessageBox(hwnd, pBuffer, MsgCaption, MSG_ERROR);
        }

        LocalFree(pBuffer);
    }

    return iRet;
}



