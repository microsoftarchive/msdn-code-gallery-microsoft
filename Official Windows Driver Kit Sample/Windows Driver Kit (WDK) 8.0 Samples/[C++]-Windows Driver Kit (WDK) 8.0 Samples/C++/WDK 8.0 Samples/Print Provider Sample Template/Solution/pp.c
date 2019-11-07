//
//  Windows (Printing) Driver Development Kit Samples.
//
//  Sample Print Provider template.
//
//  pp.c - Print provider initialization functions and stubs for the required
//         supported API set.
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



#include "pp.h"

HMODULE hmod = NULL;
WCHAR *pszRegistryPath = NULL;
WCHAR *pszRegistryPortNames=L"PortNames";
WCHAR szMachineName[MAX_COMPUTERNAME_LENGTH + 3];
CRITICAL_SECTION SplSem;
HMODULE hSpoolssDll = NULL;
FARPROC pfnSpoolssEnumPorts = NULL;

//
// Define the supported API set.
//
// Should have corresponding entries in the _PRINTPROVIDOR structure definition
// in winsplp.h.
//
// See "NOTES on PRINTPROVIDOR struct" in pp.h for details on which API's must
// be supported by a print processor.
//
static
PRINTPROVIDOR PrintProvidor =
{
    PP_OpenPrinter,
    PP_SetJob,
    PP_GetJob,
    PP_EnumJobs,
    PP_AddPrinter,
    PP_DeletePrinter,
    PP_SetPrinter,
    PP_GetPrinter,
    PP_EnumPrinters,
    PP_AddPrinterDriver,
    PP_EnumPrinterDrivers,
    PP_GetPrinterDriver,
    PP_GetPrinterDriverDirectory,
    PP_DeletePrinterDriver,
    PP_AddPrintProcessor,
    PP_EnumPrintProcessors,
    PP_GetPrintProcessorDirectory,
    PP_DeletePrintProcessor,
    PP_EnumPrintProcessorDatatypes,
    PP_StartDocPrinter,
    PP_StartPagePrinter,
    PP_WritePrinter,
    PP_EndPagePrinter,
    PP_AbortPrinter,
    PP_ReadPrinter,
    PP_EndDocPrinter,
    PP_AddJob,
    PP_ScheduleJob,
    PP_GetPrinterData,
    PP_SetPrinterData,
    PP_WaitForPrinterChange,
    PP_ClosePrinter,
    PP_AddForm,
    PP_DeleteForm,
    PP_GetForm,
    PP_SetForm,
    PP_EnumForms,
    PP_EnumMonitors,
    PP_EnumPorts,
    PP_AddPort,
    PP_ConfigurePort,
    PP_DeletePort,
    PP_CreatePrinterIC,
    PP_PlayGdiScriptOnPrinterIC,
    PP_DeletePrinterIC,
    PP_AddPrinterConnection,
    PP_DeletePrinterConnection,
    PP_PrinterMessageBox,
    PP_AddMonitor,
    PP_DeleteMonitor
};

//
// Required for DLL initialization.
//
BOOL DllMain
(
    _In_ HINSTANCE hdll,
    _In_ DWORD     dwReason,
    _In_ LPVOID    lpReserved
)
{
    UNREFERENCED_PARAMETER(lpReserved);

    if (dwReason == DLL_PROCESS_ATTACH)
    {
        DisableThreadLibraryCalls(hdll);

        hmod = hdll;
    }

    else if (dwReason == DLL_PROCESS_DETACH)
    {

    }
    return TRUE;
}



//
// This function *must* be exported by a print provider.
//
_Use_decl_annotations_
BOOL
InitializePrintProvidor
(
    LPPRINTPROVIDOR pPrintProvidor,
    DWORD           cbPrintProvidor,
    LPWSTR          pszFullRegistryPath
)

{
    DWORD dwLen;

    if (!pPrintProvidor || !pszFullRegistryPath || !*pszFullRegistryPath)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    memcpy(pPrintProvidor, &PrintProvidor, min(sizeof(PRINTPROVIDOR), cbPrintProvidor));

    pszRegistryPath = AllocSplStr(pszFullRegistryPath);
    if (!pszRegistryPath)
    {
        return FALSE;
    }

    InitializeCriticalSection(&SplSem);
    szMachineName[0] = szMachineName[1] = L'\\';
    dwLen = MAX_COMPUTERNAME_LENGTH;
    GetComputerName(szMachineName + 2, &dwLen);

    InitializePortNames();

    return TRUE;
}

//
//
// Stub functions for all the API's supported by this sample print providor.
//
//

_Success_(return == TRUE)
BOOL
PP_OpenPrinter
(
    _In_                                      LPWSTR             pszPrinterName,
    _Out_                                     LPHANDLE           phPrinter,
    _In_reads_bytes_opt_(sizeof(PRINTER_DEFAULTS)) LPPRINTER_DEFAULTS pDefault
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(pDefault);

    if (!pszPrinterName)
    {
        SetLastError(ERROR_INVALID_NAME);
        return FALSE;
    }

    if (!phPrinter)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *phPrinter = NULL;

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_ClosePrinter
(
    _In_ HANDLE  hPrinter
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);

    if (err)
    {
        SetLastError(err);
    }
    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_GetPrinter
(
    _In_                    HANDLE  hPrinter,
    _In_                    DWORD   dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE  pbPrinter,
    _In_                    DWORD   cbBuf,
    _Out_                   LPDWORD pcbNeeded
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbPrinter);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(hPrinter);

    if (!pcbNeeded)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbNeeded = 0;

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_SetPrinter
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   dwLevel,
    _In_ LPBYTE  pbPrinter,
    _In_ DWORD   dwCommand
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(pbPrinter);
    UNREFERENCED_PARAMETER(dwCommand);
    UNREFERENCED_PARAMETER(hPrinter);

    switch (dwLevel)
    {
        case 0:
        case 1:
        case 2:
        case 3:
            break;

        default:
            SetLastError(ERROR_INVALID_LEVEL);
            return FALSE;
    }

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_EnumPrinters
(
    _In_                    DWORD   dwFlags,
    _In_                    LPWSTR  pszName,
    _In_                    DWORD   dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE  pbPrinter,
    _In_                    DWORD   cbBuf,
    _Out_                   LPDWORD pcbNeeded,
    _Out_opt_               LPDWORD pcReturned
)

{
    DWORD  err = NO_ERROR;

    UNREFERENCED_PARAMETER(dwFlags);
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pbPrinter);
    UNREFERENCED_PARAMETER(cbBuf);

    if ((dwLevel != 1) && (dwLevel != 2))
    {
        SetLastError(ERROR_INVALID_NAME);
        return FALSE;
    }

    if (!pcbNeeded)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbNeeded = 0;

    if (pcReturned)
    {
        *pcReturned = 0;
    }

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
DWORD
PP_StartDocPrinter
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   dwLevel,
    _In_ LPBYTE  lpbDocInfo
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(lpbDocInfo);

    if (dwLevel != 1)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_WritePrinter
(
    _In_                HANDLE  hPrinter,
    _In_reads_bytes_(cbBuf)  LPVOID  pBuf,
    _In_                DWORD   cbBuf,
    _Out_               LPDWORD pcbWritten
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pBuf);
    UNREFERENCED_PARAMETER(cbBuf);

    if (!pcbWritten)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbWritten = 0;

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_AbortPrinter
(
    _In_ HANDLE  hPrinter
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_EndDocPrinter
(
    _In_ HANDLE   hPrinter
)
{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_GetJob
(
    _In_                    HANDLE   hPrinter,
    _In_                    DWORD    dwJobId,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbJob,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwJobId);
    UNREFERENCED_PARAMETER(pbJob);
    UNREFERENCED_PARAMETER(cbBuf);

    if ((dwLevel != 1) && (dwLevel != 2))
    {
        SetLastError(ERROR_INVALID_LEVEL);
        return FALSE;
    }

    if (!pcbNeeded)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbNeeded = 0;

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_EnumJobs
(
    _In_                    HANDLE  hPrinter,
    _In_                    DWORD   dwFirstJob,
    _In_                    DWORD   dwNoJobs,
    _In_                    DWORD   dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE  pbJob,
    _In_                    DWORD   cbBuf,
    _Out_                   LPDWORD pcbNeeded,
    _Out_                   LPDWORD pcReturned
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwFirstJob);
    UNREFERENCED_PARAMETER(dwNoJobs);
    UNREFERENCED_PARAMETER(pbJob);
    UNREFERENCED_PARAMETER(cbBuf);

    if ((dwLevel != 1) && (dwLevel != 2))
    {
        SetLastError(ERROR_INVALID_LEVEL);
        return FALSE;
    }

    if (!pcbNeeded || !pcReturned)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbNeeded = 0;
    *pcReturned = 0;

    if (err)
    {
        SetLastError(err);
    }
    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_SetJob
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   dwJobId,
    _In_ DWORD   dwLevel,
    _In_ LPBYTE  pbJob,
    _In_ DWORD   dwCommand
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwJobId);
    UNREFERENCED_PARAMETER(dwCommand);

    if ((dwLevel != 0) && (dwLevel != 1) && (dwLevel != 2))
    {
        SetLastError(ERROR_INVALID_LEVEL);
        return FALSE;
    }

    if ((dwLevel == 0) && (pbJob != NULL))
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    if (err)
    {
        SetLastError(err);
    }
    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_AddJob
(
    _In_                    HANDLE  hPrinter,
    _In_                    DWORD   dwLevel,
    _In_reads_bytes_opt_(cbBuf) LPBYTE  pbAddJob,
    _In_                    DWORD   cbBuf,
    _Out_                   LPDWORD pcbNeeded
)

{
    DWORD err = NO_ERROR;

    ADDJOB_INFO_1W TempBuffer;
    PADDJOB_INFO_1W OutputBuffer;
    DWORD OutputBufferSize;

    UNREFERENCED_PARAMETER(hPrinter);

    if (dwLevel != 1)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    if (!pcbNeeded)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbNeeded = 0;

    if (cbBuf < sizeof(ADDJOB_INFO_1W))
    {
        OutputBuffer = &TempBuffer;
        OutputBufferSize = sizeof(ADDJOB_INFO_1W);
    }
    else
    {
        OutputBuffer = (LPADDJOB_INFO_1W) pbAddJob;
        OutputBufferSize = cbBuf;
    }

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_ScheduleJob
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   dwJobId
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwJobId);

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
DWORD
PP_WaitForPrinterChange
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   dwFlags
)

{
    DWORD err = NO_ERROR;

    UNREFERENCED_PARAMETER(hPrinter);

    if (err)
    {
        SetLastError(err);
        return 0;
    }

    return dwFlags;
}

_Success_(return == TRUE)
BOOL
PP_EnumPorts
(
    _In_                    LPWSTR   pszName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbPorts,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
)

{
    DWORD         err = NO_ERROR;
    DWORD         cb = 0;
    PPORT         pPort;
    WCHAR        *pbEnd;
    PORT_INFO_1W *pInfoStruct;
    HRESULT       hr = S_OK;

    if (dwLevel != 1)
    {
        SetLastError(ERROR_INVALID_LEVEL);
        return FALSE;
    }

    if (!IsLocalMachine(pszName))
    {
        SetLastError(ERROR_INVALID_NAME);
        return FALSE;
    }

    if (!pcbNeeded || !pcReturned || (cbBuf && !pbPorts))
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbNeeded = FALSE;
    *pcReturned = FALSE;

    EnterCriticalSection(&SplSem);

    pPort = pFirstPort;
    while (pPort)
    {
        cb += (DWORD)(sizeof(PORT_INFO_1W) + (wcslen(pPort->pName) + 1) * sizeof(WCHAR));
        pPort = pPort->pNext;
    }

    *pcbNeeded = cb;
    *pcReturned = 0;

    if (cb && cb <= cbBuf)
    {
        pbEnd = (WCHAR *)(pbPorts + cb);
        pInfoStruct = (PORT_INFO_1W *)pbPorts;

        cb = 0;
        pPort = pFirstPort;
        while (pPort && cb < cbBuf)
        {
            int cwbName = (int)((wcslen(pPort->pName)) + 1);
            pbEnd -= cwbName;
            cb    += cwbName * sizeof(WCHAR);

            if (cb > cbBuf)
            {
                err = ERROR_INSUFFICIENT_BUFFER;
                break;
            }

            //
            // Set a NULL just in case pPort->pName is NULL
            //
            *pbEnd = L'\0';

            hr = StringCchCopy(pbEnd, cwbName, pPort->pName);

            if (FAILED(hr))
            {
                err = StatusFromHResult(hr);
                break;
            }

            if (cb + sizeof(PORT_INFO_1W) <= cbBuf)
            {
                pInfoStruct->pName = pbEnd;
                pInfoStruct++;
                cb += sizeof(PORT_INFO_1W);
            }
            else
            {
                err = ERROR_INSUFFICIENT_BUFFER;
                break;
            }

            pPort = pPort->pNext;
            (*pcReturned)++;
        }
    }
    else
    {
        err = ERROR_INSUFFICIENT_BUFFER;
    }

    LeaveCriticalSection(&SplSem);

    if (err)
    {
        SetLastError(err);
    }

    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_DeletePort
(
    _In_ LPWSTR  pszName,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pszPortName
)

{
    DWORD err = NO_ERROR;
    BOOL fPortDeleted;

    UNREFERENCED_PARAMETER(hWnd);

    if (!IsLocalMachine(pszName))
    {
        SetLastError(ERROR_NOT_SUPPORTED);
        return FALSE;
    }

    fPortDeleted = DeletePortEntry(pszPortName);

    if (fPortDeleted)
    {
        err = DeleteRegistryEntry(pszPortName);
    }
    else
    {
        err = ERROR_UNKNOWN_PORT;
    }

    if (err)
    {
        SetLastError(err);
    }
    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_ConfigurePort
(
    _In_ LPWSTR  pszName,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pszPortName
)

{
    UNREFERENCED_PARAMETER(hWnd);

    if (!IsLocalMachine(pszName))
    {
        SetLastError(ERROR_NOT_SUPPORTED);
        return FALSE;
    }
    else if (!PortKnown(pszPortName))
    {
        SetLastError(ERROR_UNKNOWN_PORT);
        return FALSE;
    }

    return TRUE;
}

_Success_(return == TRUE)
BOOL
PP_AddPrinterConnection
(
    _In_ LPWSTR  pszPrinterName
)
{
    UNREFERENCED_PARAMETER(pszPrinterName);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_EnumMonitors
(
    _In_                    LPWSTR   pszName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbMonitor,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pbMonitor);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pcbNeeded);
    UNREFERENCED_PARAMETER(pcReturned);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_AddPort
(
    _In_ LPWSTR  pszName,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pszMonitorName
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(hWnd);
    UNREFERENCED_PARAMETER(pszMonitorName);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
HANDLE
PP_AddPrinter
(
    _In_                                LPWSTR  pszName,
    _In_                                DWORD   dwLevel,
    _In_reads_bytes_(sizeof(PRINTER_INFO_2)) LPBYTE  pbPrinter
)
{
    LPTSTR     pszPrinterName = NULL;
    LPTSTR     pszQueue  =  NULL;
    HANDLE     hPrinter = NULL;
    size_t     uSize = 0;
    DWORD      err;
    PPRINTER_INFO_2 pPrinterInfo;

    UNREFERENCED_PARAMETER(pszName);

    pPrinterInfo = (PPRINTER_INFO_2)pbPrinter;

    if (dwLevel != 2)
    {
        err = ERROR_INVALID_PARAMETER;
        goto ErrorExit;
    }

    uSize = wcslen(((PRINTER_INFO_2 *)pbPrinter)->pPrinterName) + 1;

    pszPrinterName = (LPTSTR)LocalAlloc(LPTR, uSize * sizeof(WCHAR));
    if (!pszPrinterName)
    {
        err = ERROR_NOT_ENOUGH_MEMORY;
        goto ErrorExit;
    }

    err = StatusFromHResult(StringCchCopy(pszPrinterName,
                                          uSize,
                                          pPrinterInfo->pPrinterName));

    if (err != ERROR_SUCCESS)
    {
        goto ErrorExit;
    }


    if (  ( !ValidateUNCName(pszPrinterName))
       || ( (pszQueue = wcschr(pszPrinterName + 2, L'\\')) == NULL)
       || ( pszQueue == (pszPrinterName + 2))
       || ( *(pszQueue + 1) == L'\0')
       )
    {
        err =  ERROR_INVALID_NAME;
        goto ErrorExit;
    }

    if (pszPrinterName == NULL)
    {
        SetLastError(ERROR_INVALID_NAME);
        goto ErrorExit;
    }

    if (PortExists(pszPrinterName, &err) && !err)
    {
        SetLastError(ERROR_ALREADY_ASSIGNED);
        goto ErrorExit;
    }

    return hPrinter;

ErrorExit:
    if (!pszPrinterName)
    {
        (void)LocalFree((HLOCAL)pszPrinterName);
    }

    SetLastError(err);
    return (HANDLE)0x0;
}

_Success_(return == TRUE)
BOOL
PP_DeletePrinter
(
    _In_ HANDLE  hPrinter
)
{
    LPWSTR pszPrinterName = NULL ;
    DWORD err = NO_ERROR;
    DWORD DoesPortExist = FALSE;

    UNREFERENCED_PARAMETER(hPrinter);

    pszPrinterName = (LPWSTR)LocalAlloc(LPTR,sizeof(WCHAR)*MAX_PATH);

    if (pszPrinterName == NULL)
    {
        err = ERROR_NOT_ENOUGH_MEMORY;
        return FALSE;
    }

    if (!err && PortExists(pszPrinterName, &DoesPortExist) && DoesPortExist)
    {
        if (DeleteRegistryEntry(pszPrinterName))
        {
            (void) DeletePortEntry(pszPrinterName);
        }
    }

    if (err)
    {
        SetLastError(err);
    }
    return (err == NO_ERROR);
}

_Success_(return == TRUE)
BOOL
PP_DeletePrinterConnection
(
    _In_ LPWSTR  pszName
)
{
    UNREFERENCED_PARAMETER(pszName);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_AddPrinterDriver
(
    _In_ LPWSTR  pszName,
    _In_ DWORD   dwLevel,
    _In_ LPBYTE  pbPrinter
)
{
    UNREFERENCED_PARAMETER(pbPrinter);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pszName);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_EnumPrinterDrivers
(
    _In_                    LPWSTR   pszName,
    _In_                    LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDriverInfo,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pbDriverInfo);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pcbNeeded);
    UNREFERENCED_PARAMETER(pcReturned);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_GetPrinterDriver
(
    _In_                    HANDLE   hPrinter,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDriverInfo,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbDriverInfo);
    UNREFERENCED_PARAMETER(pcbNeeded);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_GetPrinterDriverDirectory
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDriverDirectory,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbDriverDirectory);
    UNREFERENCED_PARAMETER(pcbNeeded);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_DeletePrinterDriver
(
    _In_ LPWSTR  pszName,
    _In_ LPWSTR  pszEnvironment,
    _In_ LPWSTR  pszDriverName
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(pszDriverName);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_AddPrintProcessor
(
    _In_ LPWSTR  pszName,
    _In_ LPWSTR  pszEnvironment,
    _In_ LPWSTR  pszPathName,
    _In_ LPWSTR  pszPrintProcessorName
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(pszPathName);
    UNREFERENCED_PARAMETER(pszPrintProcessorName);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_EnumPrintProcessors
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbPrintProcessorInfo,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbPrintProcessorInfo);
    UNREFERENCED_PARAMETER(pcbNeeded);
    UNREFERENCED_PARAMETER(pcReturned);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_EnumPrintProcessorDatatypes
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszPrintProcessorName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbDatatypes,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszPrintProcessorName);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbDatatypes);
    UNREFERENCED_PARAMETER(pcbNeeded);
    UNREFERENCED_PARAMETER(pcReturned);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_GetPrintProcessorDirectory
(
    _In_                    LPWSTR   pszName,
    _In_opt_                LPWSTR   pszEnvironment,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbPrintProcessorDirectory,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbPrintProcessorDirectory);
    UNREFERENCED_PARAMETER(pcbNeeded);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_StartPagePrinter
(
    _In_ HANDLE  hPrinter
)
{
    UNREFERENCED_PARAMETER(hPrinter);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_EndPagePrinter
(
    _In_ HANDLE  hPrinter
)
{
    UNREFERENCED_PARAMETER(hPrinter);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_ReadPrinter
(
    _In_  HANDLE   hPrinter,
    _Out_ LPVOID   pBuf,
    _In_  DWORD    cbBuf,
    _Out_ LPDWORD  pcbRead
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pBuf);
    UNREFERENCED_PARAMETER(pcbRead);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
DWORD
PP_GetPrinterData
(
    _In_                    HANDLE   hPrinter,
    _In_                    LPWSTR   pszValueName,
    _Out_                   LPDWORD  pdwType,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbData,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pszValueName);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pdwType);
    UNREFERENCED_PARAMETER(pbData);
    UNREFERENCED_PARAMETER(pcbNeeded);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
DWORD
PP_SetPrinterData
(
    _In_                HANDLE  hPrinter,
    _In_                LPWSTR  pszValueName,
    _In_                DWORD   dwType,
    _In_reads_bytes_(cbData) LPBYTE  pbData,
    _In_                DWORD   cbData
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pszValueName);
    UNREFERENCED_PARAMETER(dwType);
    UNREFERENCED_PARAMETER(pbData);
    UNREFERENCED_PARAMETER(cbData);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_AddForm
(
    _In_   HANDLE  hPrinter,
    _In_   DWORD   dwLevel,
    _In_   LPBYTE  pbForm
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pbForm);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_DeleteForm
(
    _In_   HANDLE  hPrinter,
    _In_   LPWSTR  pszFormName
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pszFormName);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_GetForm
(
    _In_                    HANDLE   hPrinter,
    _In_                    LPWSTR   pszFormName,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbForm,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pszFormName);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbForm);
    UNREFERENCED_PARAMETER(pcbNeeded);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_SetForm
(
    _In_ HANDLE  hPrinter,
    _In_ LPWSTR  pszFormName,
    _In_ DWORD   dwLevel,
    _In_ LPBYTE  pbForm
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pszFormName);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pbForm);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_EnumForms
(
    _In_                    HANDLE   hPrinter,
    _In_                    DWORD    dwLevel,
    _Out_writes_bytes_opt_(cbBuf) LPBYTE   pbForm,
    _In_                    DWORD    cbBuf,
    _Out_                   LPDWORD  pcbNeeded,
    _Out_                   LPDWORD  pcReturned
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(cbBuf);
    UNREFERENCED_PARAMETER(pbForm);
    UNREFERENCED_PARAMETER(pcbNeeded);
    UNREFERENCED_PARAMETER(pcReturned);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return != NULL)
HANDLE
PP_CreatePrinterIC
(
    _In_ HANDLE     hPrinter,
    _In_ LPDEVMODE  pDevMode
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(pDevMode);

    SetLastError(ERROR_NOT_SUPPORTED);
    return NULL;
}

_Success_(return == TRUE)
BOOL
PP_PlayGdiScriptOnPrinterIC
(
    _In_                    HANDLE  hPrinterIC,
    _In_reads_bytes_(cbIn)       LPBYTE  pbIn,
    _In_                    DWORD   cbIn,
    _Out_writes_bytes_opt_(cbOut) LPBYTE  pbOut,
    _In_                    DWORD   cbOut,
    _In_                    DWORD   ul
)
{
    UNREFERENCED_PARAMETER(hPrinterIC);
    UNREFERENCED_PARAMETER(pbIn);
    UNREFERENCED_PARAMETER(cbIn);
    UNREFERENCED_PARAMETER(cbOut);
    UNREFERENCED_PARAMETER(ul);
    UNREFERENCED_PARAMETER(pbOut);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_DeletePrinterIC
(
    _In_ HANDLE  hPrinterIC
)
{
    UNREFERENCED_PARAMETER(hPrinterIC);

    SetLastError(ERROR_NOT_SUPPORTED);
    return FALSE;
}

_Success_(return == NO_ERROR)
DWORD
PP_PrinterMessageBox
(
    _In_ HANDLE  hPrinter,
    _In_ DWORD   dwError,
    _In_ HWND    hWnd,
    _In_ LPWSTR  pszText,
    _In_ LPWSTR  pszCaption,
    _In_ DWORD   dwType
)
{
    UNREFERENCED_PARAMETER(hPrinter);
    UNREFERENCED_PARAMETER(dwError);
    UNREFERENCED_PARAMETER(hWnd);
    UNREFERENCED_PARAMETER(pszText);
    UNREFERENCED_PARAMETER(pszCaption);
    UNREFERENCED_PARAMETER(dwType);

    SetLastError(ERROR_NOT_SUPPORTED);
    return ERROR_NOT_SUPPORTED;
}

_Success_(return == TRUE)
BOOL
PP_AddMonitor
(
    _In_ LPWSTR  pszName,
    _In_ DWORD   dwLevel,
    _In_ LPBYTE  pbMonitorInfo
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(dwLevel);
    UNREFERENCED_PARAMETER(pbMonitorInfo);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_DeleteMonitor
(
    _In_ LPWSTR  pszName,
    _In_ LPWSTR  pszEnvironment,
    _In_ LPWSTR  pszMonitorName
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(pszMonitorName);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}

_Success_(return == TRUE)
BOOL
PP_DeletePrintProcessor
(
    _In_ LPWSTR  pszName,
    _In_ LPWSTR  pszEnvironment,
    _In_ LPWSTR  pszPrintProcessorName
)
{
    UNREFERENCED_PARAMETER(pszName);
    UNREFERENCED_PARAMETER(pszEnvironment);
    UNREFERENCED_PARAMETER(pszPrintProcessorName);

    SetLastError(ERROR_INVALID_NAME);
    return FALSE;
}
