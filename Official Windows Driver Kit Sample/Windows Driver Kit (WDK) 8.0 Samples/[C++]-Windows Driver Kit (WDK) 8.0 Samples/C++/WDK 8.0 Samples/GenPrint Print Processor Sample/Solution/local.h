/*++

Copyright (c) 1998-2003  Microsoft Corporation
All rights reserved

Module Name:

    local.h

--*/

#ifndef _LOCAL_H_
#define _LOCAL_H_

typedef long NTSTATUS;

#include <windows.h>
#include <winspool.h>
#include <winsplp.h>
#include <wchar.h>

#include "winprint.h"
#include "emf.h"

#include <winddiui.h>

#define CCHOF(x) (sizeof(x)/sizeof(*(x)))

_Success_(return)
BOOL
PrintProcGetJobAttributesEx(
    _In_  LPCWSTR           pPrinterName,
    _In_  LPDEVMODEW        pDevmode,
    _Out_ PATTRIBUTE_INFO_4 pAttributeInfo
    );

LPWSTR AllocSplStr(LPCWSTR pStr);
LPVOID AllocSplMem(SIZE_T cbAlloc);

VOID
FreeSplMem(
    _In_opt_ VOID *pMem
    );

VOID
FreeSplStr(
    _In_opt_ PWSTR lpStr
    );

//
//  DEBUGGING:
//

#if DBG


BOOL
DebugPrint(
    _In_ PCH pszFmt,
    ...
    );

//
// ODS - OutputDebugString
//
#define ODS( MsgAndArgs ) DebugPrint  MsgAndArgs

#else
//
// No debugging
//
#define ODS(x)
#endif             // DBG


#endif

