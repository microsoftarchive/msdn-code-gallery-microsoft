/*++

Copyright (c) 1996-2003  Microsoft Corporation
All rights reserved

Module Name:

    local.h

Abstract:

    DDK version of local.h

Environment:

    User Mode -Win32

Revision History:

--*/


#define READTHREADTIMEOUT                5000
#define READ_THREAD_EOJ_TIMEOUT         60000   // 1 min
#define READ_THREAD_ERROR_WAITTIME       5000   // 5 sec
#define READ_THREAD_IDLE_WAITTIME       30000   // 30 sec

#define ALL_JOBS                    0xFFFFFFFF


// ---------------------------------------------------------------------
// EXTERN VARIABLES
// ---------------------------------------------------------------------
extern  HANDLE              hInst;
extern  DWORD               dwReadThreadErrorTimeout;
extern  __int64             nReadThreadEOJTimeout;
extern  DWORD               dwReadThreadIdleTimeoutOther;

extern  CRITICAL_SECTION    pjlMonSection;
extern  DWORD SplDbgLevel;


// ---------------------------------------------------------------------
// FUNCTION PROTOTYPE
// ---------------------------------------------------------------------
_Acquires_lock_(pjlMonSection)
VOID
EnterSplSem(
   VOID
    );

_Releases_lock_(pjlMonSection)
VOID
LeaveSplSem(
   VOID
    );

VOID
SplInSem(
   VOID
    );


VOID
SplOutSem(
    VOID
    );

DWORD
UpdateTimeoutsFromRegistry(
    _In_    LPTSTR      pszRegistryRoot
    );

PINIPORT
FindIniPort(
    _In_    LPTSTR pszName
    );

PINIPORT
CreatePortEntry(
    _In_    LPTSTR  pszPortName
    );

VOID
DeletePortEntry(
    _In_    PINIPORT pIniPort
    );

VOID
FreeIniJobs(
    _Inout_    PINIPORT pIniPort
    );

VOID
SendJobLastPageEjected(
    _In_    PINIPORT    pIniPort,
            __int64     nValue,
            BOOL        bTime
    );
VOID
FreeIniJob(
    _Inout_ PINIJOB pIniJob
    );

#include <string.h>
#include <stdlib.h>
#include <stdio.h>

LPSTR
mystrstr(
    _In_    LPSTR cs,
    _In_    LPSTR ct
);

LPSTR
mystrrchr(
    _In_    LPSTR cs,
            char c
);

LPSTR
mystrchr(
    _In_    LPSTR cs,
            char c
);

int
mystrncmp(
    _In_    LPSTR cs,
    _In_    LPSTR ct,
            int n
);


extern  CRITICAL_SECTION    pjlMonSection;

LPWSTR AllocSplStr(_In_ LPWSTR pStr);
LPVOID AllocSplMem(size_t cbAlloc);

#define FreeSplMem( pMem )        (GlobalFree( pMem ) ? FALSE:TRUE)
#define FreeSplStr( lpStr )       ((lpStr) ? (GlobalFree(lpStr) ? FALSE:TRUE):TRUE)


//
// Needed by DDK
//
#define DBG_TRACE   0x0001
#define DBG_WARN    0x0002
#define DBG_ERROR   0x0004
#define DBG_MSG(level,message)
#define SPLASSERT(exp)
#define COUNTOF(x) (sizeof(x)/sizeof *(x))
