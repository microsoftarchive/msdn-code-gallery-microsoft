/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    config.c

Abstract:

    Handles spooler entry points for adding, deleting, and configuring
    localui ports.

--*/
#include "precomp.h"
#pragma hdrstop

#include "localui.h"
#include "lmon.h"
#include "mem.h"

BOOL
ConfigLPTPort(
             HWND    hWnd,
             HANDLE  hXcv
             );

BOOL
ConfigCOMPort(
             HWND    hWnd,
             HANDLE  hXcv,
             PCWSTR  pszPortName
             );

LPWSTR
GetPortName(
           HWND    hWnd,
           HANDLE  hXcv
           );

_Success_(return != FALSE)
BOOL
WINAPI
AddPortUI(
         _In_opt_  PCWSTR  pszServer,
         _In_      HWND    hWnd,
         _In_      PCWSTR  pszMonitorNameIn,
         _Out_opt_ PWSTR  *ppszPortNameOut
         )
{
    PWSTR  pszPortName = NULL;
    PWSTR  pszServerName = NULL;
    BOOL   rc = TRUE;
    DWORD  dwReturn, dwStatus = NO_ERROR;
    DWORD  cbNeeded;
    PRINTER_DEFAULTS Default;
    HANDLE  hXcv = NULL;
    DWORD dwLastError = ERROR_SUCCESS;
    size_t cbSize = 0;

    if (ppszPortNameOut)
    {
        *ppszPortNameOut = NULL;
    }


    //
    //
    //
    if (hWnd && !IsWindow (hWnd))
    {
        //
        // Invalid parent window handle causes problems in function with DialogBoxParam call.
        // That function when the handle is bad returns ZERO, the same value as ERROR_SUCCEED.
        // PortNameDlg function calls EndDialog (ERROR_SUCCEES) if everything is alright.
        //
        SetLastError (ERROR_INVALID_WINDOW_HANDLE);
        return FALSE;
    }
    //
    //
    //
    /* Get the user to enter a port name:
     */

    pszServerName = ConstructXcvName(pszServer, pszMonitorNameIn, L"XcvMonitor");
    if (!pszServerName)
    {
        rc = FALSE;
        goto Done;
    }

    Default.pDatatype = NULL;
    Default.pDevMode = NULL;
    Default.DesiredAccess = SERVER_ACCESS_ADMINISTER;

    rc = OpenPrinter((PWSTR) pszServerName, &hXcv, &Default);
    if (!rc)
    {
        rc = FALSE;
        goto Done;
    }

    pszPortName = GetPortName(hWnd, hXcv);
    if (!pszPortName)
    {
        rc = FALSE;
        goto Done;
    }

    // We can't Add, Configure, or Delete Remote COM ports
    if (IS_COM_PORT(pszPortName) || IS_LPT_PORT(pszPortName))
    {
        SetLastError(ERROR_NOT_SUPPORTED);
        rc = FALSE;
        goto Done;
    }

    if (IS_COM_PORT(pszPortName))
        CharUpperBuff(pszPortName, 3);
    else if (IS_LPT_PORT(pszPortName))
        CharUpperBuff(pszPortName, 3);


    cbSize = (wcslen(pszPortName) + 1) * sizeof(WCHAR);

    if (cbSize <= DWORD_MAX)
    {
        rc = XcvData(   hXcv,
                        L"AddPort",
                        (PBYTE) pszPortName,
                        (DWORD)cbSize,
                        (PBYTE) &dwReturn,
                        0,
                        &cbNeeded,
                        &dwStatus);
    }
    else
    {
        SetLastError(ERROR_INVALID_DATA);
        rc = FALSE;
    }


    if (rc)
    {
        if (dwStatus == ERROR_SUCCESS)
        {
            if (ppszPortNameOut)
                *ppszPortNameOut = AllocSplStr(pszPortName);

            if (IS_LPT_PORT(pszPortName))
                rc = ConfigLPTPort(hWnd, hXcv);
            else if (IS_COM_PORT(pszPortName))
                rc = ConfigCOMPort(hWnd, hXcv, pszPortName);

        }
        else if (dwStatus == ERROR_ALREADY_EXISTS)
        {
            Message( hWnd, MSG_ERROR, IDS_LOCALMONITOR, IDS_PORTALREADYEXISTS_S, pszPortName );

        }
        else
        {
            SetLastError(dwStatus);
            rc = FALSE;
        }
    }


Done:
    dwLastError = GetLastError ();

    if (pszPortName)
        FreeSplStr(pszPortName);
    if (pszServerName)
        FreeSplMem(pszServerName);

    if (hXcv)
        ClosePrinter(hXcv);

    SetLastError (dwLastError);
    return rc;
}


BOOL
WINAPI
DeletePortUI(
            _In_ PCWSTR pszServer,
            _In_  HWND   hWnd,
            _In_ PCWSTR pszPortName
            )
{
    PRINTER_DEFAULTS Default;
    PWSTR   pszServerName = NULL;
    DWORD   dwOutput;
    DWORD   cbNeeded;
    BOOL    bRet;
    HANDLE  hXcv = NULL;
    DWORD   dwStatus = 0;
    DWORD   dwLastError = ERROR_SUCCESS;
    size_t  cbSize = 0;

    if (hWnd && !IsWindow (hWnd))
    {
        SetLastError (ERROR_INVALID_WINDOW_HANDLE);
        return FALSE;
    }

    pszServerName = ConstructXcvName(pszServer, pszPortName, L"XcvPort");
    if (!pszServerName)
    {
        bRet = FALSE;
        goto Done;
    }

    Default.pDatatype = NULL;
    Default.pDevMode = NULL;
    Default.DesiredAccess = SERVER_ACCESS_ADMINISTER;

    bRet = OpenPrinter((PWSTR) pszServerName, &hXcv, &Default);
    if (!bRet)
        goto Done;

    // Since we can't Add or Configure Remote COM ports, let's not allow deletion either

    if (IS_COM_PORT(pszPortName) || IS_LPT_PORT(pszPortName))
    {
        SetLastError(ERROR_NOT_SUPPORTED);
        bRet = FALSE;

    }
    else
    {
        cbSize = (wcslen(pszPortName) + 1) * sizeof(WCHAR);

        if (cbSize <= DWORD_MAX)
        {
            bRet = XcvData( hXcv,
                            L"DeletePort",
                            (PBYTE) pszPortName,
                            (DWORD)cbSize,
                            (PBYTE) &dwOutput,
                            0,
                            &cbNeeded,
                            &dwStatus);
        }
        else
        {
            SetLastError(ERROR_INVALID_DATA);
            bRet = FALSE;
        }

        if (!bRet && (ERROR_BUSY == dwStatus))
        {
            //
            // Port cannot be deleted cause it is in use.
            //
            ErrorMessage (
                         hWnd,
                         dwStatus
                         );
            //
            // Error is handled here and caller does not need to do anything
            //
            SetLastError (ERROR_CANCELLED);
        }
        else if (bRet && (ERROR_SUCCESS != dwStatus))
        {
            SetLastError(dwStatus);
            bRet = FALSE;
        }
    }

Done:
    dwLastError = GetLastError ();
    if (hXcv)
        ClosePrinter(hXcv);

    if (pszServerName)
        FreeSplMem(pszServerName);

    SetLastError (dwLastError);
    return bRet;
}

/* ConfigurePortUI
 *
 */
BOOL
WINAPI
ConfigurePortUI(
               _In_ PCWSTR pName,
               _In_  HWND   hWnd,
               _In_ PCWSTR pPortName
               )
{
    BOOL   bRet;
    PRINTER_DEFAULTS Default;
    PWSTR  pServerName = NULL;
    HANDLE hXcv = NULL;
    DWORD  dwLastError = ERROR_SUCCESS;
    //
    //
    //
    if (hWnd && !IsWindow (hWnd))
    {
        SetLastError (ERROR_INVALID_WINDOW_HANDLE);
        return FALSE;
    }
    //
    //
    //
    pServerName = ConstructXcvName(pName, pPortName, L"XcvPort");
    if (!pServerName)
    {
        bRet = FALSE;
        goto Done;
    }

    Default.pDatatype = NULL;
    Default.pDevMode = NULL;
    Default.DesiredAccess = SERVER_ACCESS_ADMINISTER;

    bRet = OpenPrinter((PWSTR) pServerName, &hXcv, &Default);
    if (!bRet)
        goto Done;


    if ( IS_LPT_PORT( (PWSTR) pPortName ) )
        bRet = ConfigLPTPort(hWnd, hXcv);
    else if ( IS_COM_PORT( (PWSTR) pPortName ) )
        bRet = ConfigCOMPort(hWnd, hXcv, pPortName);
    else
    {
        Message( hWnd, MSG_INFORMATION, IDS_LOCALMONITOR,
                 IDS_NOTHING_TO_CONFIGURE );

        SetLastError(ERROR_CANCELLED);
        bRet = FALSE;
    }

Done:
    dwLastError = GetLastError ();

    if (pServerName)
        FreeSplMem(pServerName);

    if (hXcv)
    {
        ClosePrinter(hXcv);
        hXcv = NULL;
    }
    SetLastError (dwLastError);

    return bRet;
}

/* ConfigLPTPort
 *
 * Calls a dialog box which prompts the user to enter timeout and retry
 * values for the port concerned.
 * The dialog writes the information to the registry (win.ini for now).
 */
BOOL
ConfigLPTPort(
             HWND    hWnd,
             HANDLE  hXcv
             )
{
    PORTDIALOG  Port;
    INT         iRet;
    //
    //
    ZeroMemory (&Port, sizeof (Port));
    iRet = -1;
    //
    //
    Port.hXcv = hXcv;

    iRet = (INT)DialogBoxParam(hInst, MAKEINTRESOURCE( DLG_CONFIGURE_LPT ),
                               hWnd, (DLGPROC)ConfigureLPTPortDlg, (LPARAM) &Port);

    if (iRet == ERROR_SUCCESS)
    {
        //
        // DialogBoxParam returns zero if hWnd is invalid.
        // ERROR_SUCCESS is equal to zero.
        // => We need to check LastError too.
        //
        return ERROR_SUCCESS == GetLastError ();
    }

    if (iRet == -1)
        return FALSE;

    SetLastError(iRet);
    return FALSE;
}

/*
 * ConfigCOMPort
 */
BOOL
ConfigCOMPort(
             HWND    hWnd,
             HANDLE  hXcv,
             PCWSTR  pszPortName
             )
{
    DWORD       dwStatus;
    BOOL        bRet = FALSE;
    COMMCONFIG  CommConfig = {0};
    COMMCONFIG  *pCommConfig = &CommConfig;
    COMMCONFIG  *pCC = NULL;
    PWSTR       pszPort = NULL;
    DWORD       cbNeeded;
    size_t      cbSize = 0;

    // GetDefaultCommConfig can't handle trailing :, so remove it!
    pszPort = (PWSTR) AllocSplStr(pszPortName);
    if (!pszPort)
        goto Done;
    pszPort[wcslen(pszPort) - 1] = L'\0';

    cbNeeded = sizeof CommConfig;

    cbSize = (wcslen(pszPort) + 1) * sizeof(*pszPort);

    if (cbSize <= DWORD_MAX)
    {
        if (!XcvData(   hXcv,
                        L"GetDefaultCommConfig",
                        (PBYTE) pszPort,
                        (DWORD)cbSize,
                        (PBYTE) pCommConfig,
                        cbNeeded,
                        &cbNeeded,
                        &dwStatus))
            goto Done;
    }
    else
    {
        goto Done;
    }

    if (dwStatus != ERROR_SUCCESS)
    {
        if (dwStatus != ERROR_INSUFFICIENT_BUFFER)
        {
            SetLastError(dwStatus);
            goto Done;
        }

        pCommConfig = pCC = (COMMCONFIG *)AllocSplMem(cbNeeded);
        if (!pCommConfig)
            goto Done;

        cbSize = (wcslen(pszPort) + 1) * sizeof(*pszPort);
        if (cbSize <= DWORD_MAX)
        {
            if (!XcvData(   hXcv,
                            L"GetDefaultCommConfig",
                            (PBYTE) pszPort,
                            (DWORD)cbSize,
                            (PBYTE) pCommConfig,
                            cbNeeded,
                            &cbNeeded,
                            &dwStatus))
                goto Done;
        }
        else
        {
            goto Done;
        }

        if (dwStatus != ERROR_SUCCESS)
        {
            SetLastError(dwStatus);
            goto Done;
        }
    }

    if (CommConfigDialog(pszPort, hWnd, pCommConfig))
    {
        if (!XcvData(   hXcv,
                        L"SetDefaultCommConfig",
                        (PBYTE) pCommConfig,
                        pCommConfig->dwSize,
                        (PBYTE) NULL,
                        0,
                        &cbNeeded,
                        &dwStatus))
            goto Done;

        if (dwStatus != ERROR_SUCCESS)
        {
            SetLastError(dwStatus);
            goto Done;
        }
        bRet = TRUE;
    }


    Done:

    if (pCC)
        FreeSplMem(pCC);
    if (pszPort)
        FreeSplStr(pszPort);

    return bRet;
}

/* GetPortName
 *
 * Puts up a dialog containing a free entry field.
 * The dialog allocates a string for the name, if a selection is made.
 */
LPWSTR
GetPortName(
           HWND    hWnd,
           HANDLE  hXcv
           )
{
    PORTDIALOG Port;
    INT        Result;
    LPWSTR     pszPort = NULL;
    //
    //
    ZeroMemory (&Port, sizeof (Port));
    Result = -1;
    //
    //
    Port.hXcv = hXcv;
    Port.dwRet = ERROR_INVALID_HANDLE;

    Result = (INT)DialogBoxParam(hInst,
                                 MAKEINTRESOURCE(DLG_PORTNAME),
                                 hWnd,
                                 (DLGPROC)PortNameDlg,
                                 (LPARAM)&Port);

    if (Result == ERROR_SUCCESS)
    {
        //
        // DialogBoxParam returns zero if hWnd is invalid.
        // ERROR_SUCCESS is equal to zero.
        // => We need to check LastError too.
        //
        if (Port.dwRet == ERROR_SUCCESS)
        {
            //
            // DialogBoxParam executed successfully and a port name was retrieved
            //
            pszPort = Port.pszPortName;
        }
    }
    else if (Result != -1)
    {
        //
        // DialogBoxParam executed successfully, but the user canceled the dialog
        //
        SetLastError(Result);
    }

    return pszPort;
}

