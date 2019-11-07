/*++

Copyright (c) 1990-2001 Microsoft Corporation
All Rights Reserved

Module Name:

    mem.h

--*/

#ifndef _MEM_H_
#define _MEM_H_

LPWSTR
AllocSplStr(
    _In_ LPCWSTR pStr
    );

LPVOID
AllocSplMem(
    _In_ size_t cbAlloc
    );

BOOL
FreeSplStr(
    _In_opt_ LPWSTR pStr
    );

BOOL
FreeSplMem(
    _In_opt_ PVOID pMem
    );

#endif // _MEM_H_

