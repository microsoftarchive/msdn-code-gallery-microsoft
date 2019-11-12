//
//  Windows Server (Printing) Driver Development Kit Samples.
//
//  Sample Print Provider template.
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

#include <windows.h>
#include <winspool.h>
#include <strsafe.h>
#include <common.ver>

//
// NOTES on PRINTPROVIDOR struct:
//
// Get the list of API's that a Print Provider can provide as defined in the
// _PRINTPROVIDOR structure definition.  This list will vary on a platform by
// platform basis.
//
// Not all the API's in the list need to be supported. See "Functions Defined
// by Print Providers" in the SDK documentation to get a list of whcih API's
// from the list *must* be supported by a print provider on a given platform.
//
#include <winsplp.h>


//
//
// Function prototypes for the stubs for the various print provider API's
//
//

_Success_(return == TRUE)
BOOL PP_OpenPrinter
(
    _In_                                      LPWSTR             pszPrinterName,
    _Out_                                     LPHANDLE           phPrinter,
    _In_reads_bytes_opt_(sizeof(PRINTER_DEFAULTS)) LPPRINTER_DEFAULTS pDefault
);

_Success_(return == TRUE)
BOOL PP_SetJob
(
    _In_ HANDLE hPrinter,
    _In_ DWORD  JobId,
    _In_ DWORD  Level,
    _In_ LPBYTE pJob,
    _In_ DWORD  Command
);

_Success_(return == TRUE)
BOOL PP_GetJob
(
    _In_                    HANDLE   hPrinter,
    _In_                    DWORD    dwJobId,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbJob,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
);

_Success_(return == TRUE)
BOOL PP_EnumJobs
(
    _In_                    HANDLE  hPrinter,
    _In_                    DWORD   dwFirstJob,
    _In_                    DWORD   dwNoJobs,
    _In_                    DWORD   dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE  pbJob,
    _In_                    DWORD   cbBuf,
    _Out_                   LPDWORD pcbNeeded,
    _Out_                   LPDWORD pcReturned
);

_Success_(return == TRUE)
HANDLE PP_AddPrinter
(
    _In_                                LPWSTR  pszName,
    _In_                                DWORD   dwLevel,
    _In_reads_bytes_(sizeof(PRINTER_INFO_2)) LPBYTE  pbPrinter
);

_Success_(return == TRUE)
BOOL PP_DeletePrinter
(
    _In_ HANDLE  hPrinter
);

_Success_(return == TRUE)
BOOL PP_SetPrinter
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   Level,
    _In_ LPBYTE  pPrinter,
    _In_ DWORD   Command
);

_Success_(return == TRUE)
BOOL PP_GetPrinter
(
    _In_                            HANDLE  hPrinter,
    _In_                            DWORD   dwLevel,
    _Out_writes_bytes_opt_(cbBuf)         LPBYTE  pbPrinter,
    _In_                            DWORD   cbBuf,
    _Out_                           LPDWORD pcbNeeded
);

_Success_(return == TRUE)
BOOL PP_EnumPrinters
(
    _In_                    DWORD   dwFlags,
    _In_                    LPWSTR  pszName,
    _In_                    DWORD   dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE  pbPrinter,
    _In_                    DWORD   cbBuf,
    _Out_                   LPDWORD pcbNeeded,
    _Out_opt_               LPDWORD pcReturned
);

_Success_(return == TRUE)
BOOL PP_AddPrinterDriver
(
    _In_ LPWSTR  pName,
    _In_ DWORD   Level,
    _In_ LPBYTE  pDriverInfo
);

_Success_(return == TRUE)
BOOL PP_EnumPrinterDrivers
(
    _In_                    LPWSTR   pszName,
    _In_                    LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDriverInfo,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
);

_Success_(return == TRUE)
BOOL PP_GetPrinterDriver
(
    _In_                    HANDLE   hPrinter,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDriverInfo,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
);

_Success_(return == TRUE)
BOOL PP_GetPrinterDriverDirectory
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDriverDirectory,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
);

_Success_(return == TRUE)
BOOL PP_DeletePrinterDriver
(
    _In_ LPWSTR  pszName,
    _In_ LPWSTR  pszEnvironment,
    _In_ LPWSTR  pszDriverName
);

_Success_(return == TRUE)
BOOL PP_AddPrintProcessor
(
    _In_ LPWSTR  pszName,
    _In_ LPWSTR  pszEnvironment,
    _In_ LPWSTR  pszPathName,
    _In_ LPWSTR  pszPrintProcessorName
);

_Success_(return == TRUE)
BOOL PP_EnumPrintProcessors
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbPrintProcessorInfo,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
);

_Success_(return == TRUE)
BOOL PP_GetPrintProcessorDirectory
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbPrintProcessorDirectory,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
);

_Success_(return == TRUE)
BOOL PP_DeletePrintProcessor
(
    _In_ LPWSTR  pName,
    _In_ LPWSTR  pEnvironment,
    _In_ LPWSTR  pPrintProcessorName
);

_Success_(return == TRUE)
BOOL PP_EnumPrintProcessorDatatypes
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszPrintProcessorName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDatatypes,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
);

_Success_(return == TRUE)
DWORD PP_StartDocPrinter
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   Level,
    _In_ LPBYTE  pDocInfo
);

_Success_(return == TRUE)
BOOL PP_StartPagePrinter
(
    _In_ HANDLE  hPrinter
);

_Success_(return == TRUE)
BOOL PP_WritePrinter
(
    _In_                HANDLE  hPrinter,
    _In_reads_bytes_(cbBuf)  LPVOID  pBuf,
    _In_                DWORD   cbBuf,
    _Out_               LPDWORD pcbWritten
);

_Success_(return == TRUE)
BOOL PP_EndPagePrinter
(
    _In_ HANDLE  hPrinter
);

_Success_(return == TRUE)
BOOL PP_AbortPrinter
(
    _In_ HANDLE  hPrinter
);

_Success_(return == TRUE)
BOOL PP_ReadPrinter
(
    _In_  HANDLE   hPrinter,
    _Out_ LPVOID   pBuf,
    _In_  DWORD    cbBuf,
    _Out_ LPDWORD  pcbRead
);

_Success_(return == TRUE)
BOOL PP_EndDocPrinter
(
    _In_ HANDLE  hPrinter
);

_Success_(return == TRUE)
BOOL PP_AddJob
(
    _In_                    HANDLE  hPrinter,
    _In_                    DWORD   dwLevel,
    _In_reads_bytes_opt_(cbBuf) LPBYTE  pbAddJob,
    _In_                    DWORD   cbBuf,
    _Out_                   LPDWORD pcbNeeded
);

_Success_(return == TRUE)
BOOL PP_ScheduleJob
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   JobId
);

_Success_(return == TRUE)
DWORD PP_GetPrinterData
(
    _In_                    HANDLE   hPrinter,
    _In_                    LPWSTR   pszValueName,
    _Out_                   LPDWORD  pdwType,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbData,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
);

_Success_(return == TRUE)
DWORD PP_SetPrinterData
(
    _In_                HANDLE  hPrinter,
    _In_                LPWSTR  pszValueName,
    _In_                DWORD   dwType,
    _In_reads_bytes_(cbData) LPBYTE  pbData,
    _In_                DWORD   cbData
);

_Success_(return == TRUE)
DWORD PP_WaitForPrinterChange
(
    _In_ HANDLE hPrinter,
    _In_ DWORD  Flags
);

_Success_(return == TRUE)
BOOL PP_ClosePrinter
(
    _In_ HANDLE hPrinter
);

_Success_(return == TRUE)
BOOL PP_AddForm
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   Level,
    _In_ LPBYTE  pForm
);

_Success_(return == TRUE)
BOOL PP_DeleteForm
(
    _In_ HANDLE  hPrinter,
    _In_ LPWSTR  pFormName
);

_Success_(return == TRUE)
BOOL PP_GetForm
(
    _In_                    HANDLE   hPrinter,
    _In_                    LPWSTR   pszFormName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbForm,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
);

_Success_(return == TRUE)
BOOL PP_SetForm
(
    _In_ HANDLE  hPrinter,
    _In_ LPWSTR  pFormName,
    _In_ DWORD   Level,
    _In_ LPBYTE  pForm
);

_Success_(return == TRUE)
BOOL PP_EnumForms
(
    _In_                    HANDLE   hPrinter,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbForm,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
);

_Success_(return == TRUE)
BOOL PP_EnumMonitors
(
    _In_                    LPWSTR   pszName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbMonitor,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
);

_Success_(return == TRUE)
BOOL PP_EnumPorts
(
    _In_                    LPWSTR   pszName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbPort,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
);

_Success_(return == TRUE)
BOOL PP_AddPort
(
    _In_ LPWSTR  pName,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pMonitorName
);

_Success_(return == TRUE)
BOOL PP_ConfigurePort
(
    _In_ LPWSTR  pName,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pPortName
);

_Success_(return == TRUE)
BOOL PP_DeletePort
(
    _In_ LPWSTR  pName,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pPortName
);

_Success_(return != NULL)
HANDLE PP_CreatePrinterIC
(
    _In_ HANDLE     hPrinter,
    _In_ LPDEVMODEW pDevMode
);

_Success_(return == TRUE)
BOOL PP_PlayGdiScriptOnPrinterIC
(
    _In_                    HANDLE  hPrinterIC,
    _In_reads_bytes_(cbIn)       LPBYTE  pbIn,
    _In_                    DWORD   cbIn,
    _Out_writes_bytes_opt_(cbOut) LPBYTE  pbOut,
    _In_                    DWORD   cbOut,
    _In_                    DWORD   ul
);

_Success_(return == TRUE)
BOOL PP_DeletePrinterIC
(
    _In_ HANDLE  hPrinterIC
);

_Success_(return == TRUE)
BOOL PP_AddPrinterConnection
(
    _In_ LPWSTR  pName
);

_Success_(return == TRUE)
BOOL PP_DeletePrinterConnection
(
    _In_ LPWSTR pName
);

_Success_(return == NO_ERROR)
DWORD PP_PrinterMessageBox
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   Error,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pText,
    _In_ LPWSTR  pCaption,
    _In_ DWORD   dwType
);

_Success_(return == TRUE)
BOOL PP_AddMonitor
(
    _In_ LPWSTR  pName,
    _In_ DWORD   Level,
    _In_ LPBYTE  pMonitorInfo
);

_Success_(return == TRUE)
BOOL PP_DeleteMonitor
(
    _In_ LPWSTR  pName,
    _In_ LPWSTR  pEnvironment,
    _In_ LPWSTR  pMonitorName
);

_Success_(return == TRUE)
BOOL PP_ResetPrinter
(
    _In_                                  HANDLE             hPrinter,
    _In_reads_bytes_(sizeof(PRINTER_DEFAULTS)) LPPRINTER_DEFAULTS pDefault
);

_Success_(return == TRUE)
BOOL PP_AddPortEx
(
    _In_     LPWSTR pName,
    _In_     DWORD  Level,
    _In_     LPBYTE lpBuffer,
    _In_opt_ LPWSTR lpMonitorName
);

_Success_(return == TRUE)
BOOL PP_OpenPrinterEx
(
    _In_                                      LPWSTR             pPrinterName,
    _Out_                                     LPHANDLE           phPrinter,
    _In_reads_bytes_opt_(sizeof(PRINTER_DEFAULTS)) LPPRINTER_DEFAULTS pDefault,
    _In_opt_                                  LPBYTE             pClientInfo,
    _In_                                      DWORD              Level
);

_Success_(return != NULL)
HANDLE PP_AddPrinterEx
(
    _In_ LPWSTR  pName,
    _In_ DWORD   Level,
    _In_ LPBYTE  pPrinter,
    _In_ LPBYTE  pClientInfo,
    _In_ DWORD   ClientInfoLevel
);

_Success_(return == TRUE)
BOOL PP_SetPort
(
    _In_ LPWSTR  pName,
    _In_ LPWSTR  pPortName,
    _In_ DWORD   Level,
    _In_ LPBYTE  pPortInfo
);

_Success_(return != NO_ERROR)
DWORD PP_EnumPrinterData
(
    _In_                      HANDLE  hPrinter,
    _In_                      DWORD   dwIndex,
    _Out_writes_bytes_(cbValueName) LPWSTR  pValueName,
    _In_                      DWORD   cbValueName,
    _Out_                     LPDWORD pcbValueName,
    _Out_                     LPDWORD pType,
    _Out_writes_bytes_opt_(cbData)  LPBYTE  pData,
    _In_                      DWORD   cbData,
    _Out_                     LPDWORD pcbData
);

_Success_(return != NO_ERROR)
DWORD PP_DeletePrinterData
(
    _In_ HANDLE   hPrinter,
    _In_ LPWSTR   pValueName
);




//
// Simple port structure to maintain an internal list of added ports.
//
typedef struct _PORT
{
    DWORD   cb;
    struct  _PORT *pNext;
    LPWSTR  pName;
} PORT, *PPORT;


//
// Function prototypes for various port utility functions.
//
DWORD
InitializePortNames
(
    VOID
);

PPORT CreatePortEntry
(
    _In_ LPWSTR pPortName
);

BOOL DeletePortEntry
(
    _In_ LPWSTR pPortName
);

DWORD
CreateRegistryEntry
(
    _In_ LPWSTR pPortName
);

DWORD DeleteRegistryEntry
(
    _In_ LPWSTR pPortName
);

BOOL PortKnown
(
    _In_ LPWSTR pPortName
);

BOOL PortExists
(
    _In_  LPWSTR pPortName,
    _Out_ LPDWORD pError
);

BOOL IsLocalMachine
(
    _In_ LPWSTR pszName
);



//
// Function prototypes for miscellaneous utility functions.
//
LPWSTR AllocSplStr
(
    _In_ LPWSTR pStr
);

VOID
FreeSplStr
(
   _In_ LPWSTR pStr
);

BOOL ValidateUNCName
(
    _In_ LPWSTR pName
);

DWORD
StatusFromHResult
(
    _In_ HRESULT hr
);

BOOL
BoolFromStatus
(
    _In_ DWORD Status
);



//
// Make global variables available to other files in the sample.
//
extern HMODULE hmod;
extern HMODULE hSpoolssDll;

extern CRITICAL_SECTION SplSem;

extern FARPROC pfnSpoolssEnumPorts;

extern WCHAR *pszRegistryPath;
extern WCHAR *pszRegistryPortNames;
extern WCHAR szMachineName[MAX_COMPUTERNAME_LENGTH + 3];

extern PPORT pFirstPort;

