/*++

Copyright (c) 1990-2001  Microsoft Corporation
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
    size_t cbAlloc
    );

BOOL
FreeSplStr(
    _In_    LPWSTR pStr
    );

BOOL
FreeSplMem(
    _In_    PVOID pMem
    );

#endif // _MEM_H_

