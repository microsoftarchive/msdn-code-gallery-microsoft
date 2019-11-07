/*++

Copyright (c) 1995-2003 Microsoft Corporation
All rights reserved.

Module Name:

    Spltypes.h

Abstract:

    PJLMON header file


--*/

typedef struct _INIJOB {
    DWORD   signature;
    struct _INIJOB FAR *pNext;
    LPTSTR  pszPrinterName;
    HANDLE  hPrinter;
    DWORD   JobId;
    DWORD   status;
    __int64 nTimeoutCount;
} INIJOB, FAR *PINIJOB;

typedef struct _INIPORT {       /* ipo */
    DWORD   signature;
    struct  _INIPORT FAR *pNext;
    LPTSTR  pszPortName;

    DWORD   cRef;

    DWORD   status;
    PINIJOB pIniJob;

    HANDLE  hPort;
    HANDLE  WakeUp;
    HANDLE  DoneReading;
    HANDLE  DoneWriting;
    HANDLE  hUstatusThread;

    DWORD   PrinterStatus;
    __int64 nLastReadTime;
    DWORD   dwAvailableMemory;
    DWORD   dwInstalledMemory;

    MONITOR fn;

} INIPORT, FAR *PINIPORT;

#define PJ_SIGNATURE   0x4F4A  /* 'PJ' is the signature value */

//
// PP_PJL_SENT, PP_SEND_PJL, PP_IS_PJL, PP_LJ4L, PP_RESETDEV
//      are set/cleared on per job basis.
// PP_DONT_TRY_PJL is set/cleared on per printer basis.
//
#define PP_INSTARTDOC       0x00000001  // Inside StartDoc, sending data to the printer
#define PP_RUN_THREAD       0x00000002  // Tell the ustatus thread to start running
#define PP_THREAD_RUNNING   0x00000004  // Tell the main thread that the ustatus thread is running

//
// If PP_RUN_THREAD is set and PP_THREAD_RUNNING is not that means UStatus
// thread is being created or it is already running but has not determined if
// printer is PJL or not yet
//
#define PP_PRINTER_OFFLINE  0x00000008  // The printer is OFFLINE
#define PP_PJL_SENT         0x00000010  // PJL Command was sent to the printer
#define PP_SEND_PJL         0x00000020  // Set at StartDoc so that we initialize PJL
                                        // commands during the first write port
#define PP_IS_PJL           0x00000040  // Port is PJL
#define PP_DONT_TRY_PJL     0x00000080  // Don't try again...
#define PP_WRITE_ERROR      0x00000100  // A write was not succesful

// PP_PJL_SENT, PP_SEND_PJL, PP_IS_PJL, PP_PORT_OPEN are set/cleared on
//           per job basis.
// PP_DONT_TRY_PJL is set/cleared on per printer basis.

//
// The elapsed time returned by GetTickCount is a DWORD value. Therefore, the time 
// will wrap around to zero if the system is run continuously for 49.7 days. To avoid 
// this problem we use the new GetTickCount64 when available (Windows Vista or later):
//

#if (_WIN32_WINNT < 0x0600)
#define GetTickCount64() ((ULONGLONG)GetTickCount())
#define MAX_PJLMON_TIMEOUT 4294967295
#else
#define MAX_PJLMON_TIMEOUT 922337203685477
#endif

