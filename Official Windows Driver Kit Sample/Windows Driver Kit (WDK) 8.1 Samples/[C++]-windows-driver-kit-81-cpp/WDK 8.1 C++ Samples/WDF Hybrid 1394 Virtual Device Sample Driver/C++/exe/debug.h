/*++

Copyright (c) 1998  Microsoft Corporation

Module Name: 

    debug.h
--*/

#define TL_TRACE    0
#define TL_WARNING  1
#define TL_ERROR    2
#define TL_FATAL    3

#ifdef _DEBUG_C
unsigned char TraceLevel = TL_TRACE;
#else
extern unsigned char TraceLevel;
#endif

#define TRACE(tl, x)                \
    if ( (tl) >= TraceLevel ) {     \
        DbgPrt x ;                  \
    }

void 
DbgPrt(
    _In_ HANDLE   hWnd,
    _In_ PSTR     lpszFormat,
    ... 
    );
    

