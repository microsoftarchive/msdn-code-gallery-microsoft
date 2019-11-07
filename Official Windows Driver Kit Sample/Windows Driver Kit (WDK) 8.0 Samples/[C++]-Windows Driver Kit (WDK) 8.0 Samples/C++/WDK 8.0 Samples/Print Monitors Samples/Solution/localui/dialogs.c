/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    dialogs.c

--*/

#include "precomp.h"
#pragma hdrstop

#include "localui.h"
#include "dialogs.h"
#include "mem.h"

WCHAR szINIKey_TransmissionRetryTimeout[] = L"TransmissionRetryTimeout";

#define MAX_LOCAL_PORTNAME  246

/*++

Routine Name:

    ConfigureLPTPortDlg

Routine Description:

    This function handles the UI for configuring the LPT ports.

Arguments:

    uMsg    - message
    hDlg    - handle to dialog
    wParam  - wparam
    lParam  - lparam

Return Value:

    TRUE if message handled, otherwise FALSE.

--*/
INT_PTR APIENTRY
ConfigureLPTPortDlg(
            HWND   hwnd,
            UINT   msg,
    _In_    WPARAM wparam,
    _In_    LPARAM lparam
    )
{
    switch(msg)
    {
    case WM_INITDIALOG:
        return ConfigureLPTPortInitDialog(hwnd, (PPORTDIALOG) lparam);

    case WM_COMMAND:
        switch (LOWORD(wparam))
        {
        case IDOK:
            return ConfigureLPTPortCommandOK(hwnd);

        case IDCANCEL:
            return ConfigureLPTPortCommandCancel(hwnd);

        case IDD_CL_EF_TRANSMISSIONRETRY:
            if( HIWORD(wparam) == EN_UPDATE )
                ConfigureLPTPortCommandTransmissionRetryUpdate(hwnd, LOWORD(wparam));
            break;
        }
        break;
    }

    return FALSE;
}

/*++

Routine Name:

    ConfigureLPTPortInitDialog

Routine Description:

    Initializes the lpt port configuration dialog variables.

Arguments:

    hDlg    - handle to dialog
    pPort   - pointer to port dialog data to initialize.

Return Value:

    TRUE data was initlaized, FALSE error occurred.

--*/
BOOL
ConfigureLPTPortInitDialog(
            HWND        hwnd,
    _In_    PPORTDIALOG pPort
    )
{
    DWORD dwTransmissionRetryTimeout;
    DWORD cbNeeded;
    DWORD dwDummy = 0;
    BOOL  rc;
    DWORD dwStatus = NO_ERROR;

    DBGMSG(DBG_TRACE, ("ConfigureLPTPortInitDialog\n"));

    SetWindowLongPtr(hwnd, GWLP_USERDATA, (LONG_PTR) pPort);

    SetForegroundWindow(hwnd);

    SendDlgItemMessage( hwnd, IDD_CL_EF_TRANSMISSIONRETRY,
                        EM_LIMITTEXT, TIMEOUT_STRING_MAX, 0 );

    //
    // Get the Transmission Retry Timeout from the host
    //
    rc = XcvData(   pPort->hXcv,
                    L"GetTransmissionRetryTimeout",
                    (PBYTE) &dwDummy,
                    0,
                    (PBYTE) &dwTransmissionRetryTimeout,
                    sizeof dwTransmissionRetryTimeout,
                    &cbNeeded,
                    &dwStatus);
    if(!rc) {
        DBGMSG(DBG_WARN, ("Error %d checking TransmissionRetryTimeout\n", GetLastError()));
    } else if(dwStatus != ERROR_SUCCESS) {
        DBGMSG(DBG_WARN, ("Error %d checking TransmissionRetryTimeout\n", dwStatus));
        SetLastError(dwStatus);
        rc = FALSE;

    } else {

        SetDlgItemInt( hwnd, IDD_CL_EF_TRANSMISSIONRETRY,
                       dwTransmissionRetryTimeout, FALSE );

        SET_LAST_VALID_ENTRY( hwnd, IDD_CL_EF_TRANSMISSIONRETRY,
                              dwTransmissionRetryTimeout );

    }

    return rc;
}

/*++

Routine Name:

    ConfigureLPTPortCommandOK

Routine Description:

    Handles the completion of the lpt port configuration dialog.

Arguments:

    hDlg    - handle to dialog

Return Value:

    TRUE success, FALSE an error occurred.

--*/
BOOL
ConfigureLPTPortCommandOK(
    HWND hwnd
    )
{
    WCHAR String[TIMEOUT_STRING_MAX+1];
    UINT  TransmissionRetryTimeout;
    BOOL  b = FALSE;
    DWORD cbNeeded;
    PPORTDIALOG pPort;
    DWORD dwStatus = NO_ERROR;
    size_t cbSize = 0;

    if ((pPort = (PPORTDIALOG) GetWindowLongPtr(hwnd, GWLP_USERDATA)) == NULL)
    {
        dwStatus = ERROR_INVALID_DATA;
        ErrorMessage (hwnd, dwStatus);
        SetLastError (dwStatus);
        return FALSE;
    }

    TransmissionRetryTimeout = GetDlgItemInt( hwnd,
                                              IDD_CL_EF_TRANSMISSIONRETRY,
                                              &b,
                                              FALSE );

    (VOID) StringCchPrintf (String, COUNTOF (String), L"%u", TransmissionRetryTimeout);

    cbSize = (wcslen(String) + 1) * sizeof(WCHAR);
    if (cbSize <= DWORD_MAX)
    {
        b = XcvData(pPort->hXcv,
                    L"ConfigureLPTPortCommandOK",
                    (PBYTE) String,
                    (DWORD)cbSize,
                    (PBYTE) &cbNeeded,
                    0,
                    &cbNeeded,
                    &dwStatus);
    }
    else
    {
        SetLastError(ERROR_INVALID_DATA);
    }

    EndDialog(hwnd, b ? dwStatus : GetLastError());

    return TRUE;
}

/*++

Routine Name:

    ConfigureLPTPortCommandCancel

Routine Description:

    Handles the cancelation of the lpt port configuration dialog.

Arguments:

    hDlg    - handle to dialog

Return Value:

    TRUE success, FALSE an error occurred.

--*/
BOOL
ConfigureLPTPortCommandCancel(
    HWND hwnd
    )
{
    EndDialog(hwnd, ERROR_CANCELLED);
    return TRUE;
}

/*++

Routine Name:

    ConfigureLPTPortCommandTransmissionRetryUpdate

Routine Description:

    Updates and validates the lpt port trasmission delay
    which the user typed.

Arguments:

    hDlg    - handle to dialog
    CtlId   - control id of the transmission edit control

Return Value:

    TRUE success, FALSE an error occurred.

--*/
BOOL
ConfigureLPTPortCommandTransmissionRetryUpdate(
    HWND hwnd,
    WORD CtlId
    )
{
    INT  Value;
    BOOL OK;

    Value = GetDlgItemInt( hwnd, CtlId, &OK, FALSE );

    if( WITHINRANGE( Value, TIMEOUT_MIN, TIMEOUT_MAX ) )
    {
        SET_LAST_VALID_ENTRY( hwnd, CtlId, Value );
    }

    else
    {
        SetDlgItemInt( hwnd, CtlId, (UINT) GET_LAST_VALID_ENTRY( hwnd, CtlId ), FALSE );
        SendDlgItemMessage( hwnd, CtlId, EM_SETSEL, 0, (LPARAM)-1 );
    }

    return TRUE;
}

/*++

Routine Name:

    PortNameDlg

Routine Description:

    This is the dialog proc for handeling a local port name modification

Arguments:

    uMsg    - message
    hDlg    - handle to dialog
    wParam  - wparam
    lParam  - lparam

Return Value:

    TRUE if message handled, otherwise FALSE.

--*/
INT_PTR CALLBACK
PortNameDlg(
            HWND   hwnd,
            WORD   msg,
    _In_    WPARAM wparam,
    _In_    LPARAM lparam
    )
{
    switch(msg)
    {
    case WM_INITDIALOG:
        return PortNameInitDialog(hwnd, (PPORTDIALOG)lparam);

    case WM_COMMAND:
        switch (LOWORD(wparam))
        {
        case IDOK:
            return PortNameCommandOK(hwnd);

        case IDCANCEL:
            return PortNameCommandCancel(hwnd);
        }
        break;
    }

    return FALSE;
}

/*++

Routine Name:

    PortNameInitDialog

Routine Description:

    Initializes the port name dialog data

Arguments:

    pPort   - pointer to port dialog data to initialize.

Return Value:

    TRUE success, FALSE an error occurred.

--*/
BOOL
PortNameInitDialog(
            HWND        hwnd,
    _In_    PPORTDIALOG pPort
    )
{
    SetForegroundWindow(hwnd);

    SetWindowLongPtr(hwnd, GWLP_USERDATA, (LONG_PTR) pPort);

    // Number used to limit the port name length
    SendDlgItemMessage (hwnd, IDD_PN_EF_PORTNAME, EM_LIMITTEXT, MAX_LOCAL_PORTNAME, 0);

    return TRUE;
}

/*++

Routine Name:

    PortNameCommandOK

Routine Description:

    Handles the case when the user clicks ok on the port name dialog

Arguments:

    hDlg    - handle to dialog

Return Value:

    TRUE success, FALSE an error occurred.

--*/
BOOL
PortNameCommandOK(
    HWND    hwnd
    )
{
    PPORTDIALOG pPort;
    WCHAR   string [MAX_LOCAL_PORTNAME + 1] = L"";
    WCHAR   trimmedString [MAX_LOCAL_PORTNAME + 1] = L"";
    WCHAR   szWideSpace[] = L" ";
    BOOL    rc;
    DWORD   cbNeeded;
    DWORD   dwStatus = NO_ERROR;
    DWORD   indexString = 0;
    DWORD   indexTrimmedString = 0;
    SHORT   end;
    size_t  cbSize = 0;

    if ((pPort = (PPORTDIALOG) GetWindowLongPtr( hwnd, GWLP_USERDATA )) == NULL)
    {
        dwStatus = ERROR_INVALID_DATA;
        ErrorMessage (hwnd, dwStatus);
        SetLastError (dwStatus);
        return FALSE;
    }

    GetDlgItemText( hwnd, IDD_PN_EF_PORTNAME, string, COUNTOF(string));
    string [COUNTOF (string) - 1] = L'\0';

    //
    // Trim the string for Spaces (Front & Back)
    //
    // First Cut off any spaces at the front
    //
    while ((indexString < COUNTOF(string)) &&
           (string[indexString] == szWideSpace[0]))
    {
        indexString++;
    }
    while ((indexString < COUNTOF(string)-1) && (string[indexString]))
    {
        trimmedString[indexTrimmedString++] = string[indexString++];
    }
    trimmedString[indexTrimmedString] = 0x00;

    //
    // Next Cut off any spaces at the end
    //
    end = (SHORT) wcslen(trimmedString)-1;
    while ((end >= 0) && (trimmedString[end] == szWideSpace[0]))
    {
        trimmedString[end--] = 0x00;
    }

    cbSize = (wcslen(string) + 1) * sizeof(*string);

    if (cbSize <= DWORD_MAX)
    {
        rc = XcvData(   pPort->hXcv,
                        L"PortIsValid",
                        (PBYTE) trimmedString,
                        (DWORD)cbSize,
                        (PBYTE) NULL,
                        0,
                        &cbNeeded,
                        &dwStatus);
    }
    else
    {
        rc = FALSE;
    }

    if (!rc) {
        return FALSE;

    } else if (dwStatus != ERROR_SUCCESS) {
        SetLastError(dwStatus);

        if ( (dwStatus == ERROR_INVALID_NAME) || (*trimmedString == 0x00) )
            Message( hwnd, MSG_ERROR, IDS_LOCALMONITOR, IDS_INVALIDPORTNAME_S, string );
        else
            ErrorMessage(hwnd, dwStatus);

        return FALSE;

    } else {
        pPort->pszPortName = AllocSplStr( trimmedString );
        pPort->dwRet = dwStatus;
        EndDialog( hwnd, ERROR_SUCCESS );
        return TRUE;
    }

}

/*++

Routine Name:

    PortNameCommandCancel

Routine Description:

    Handles the case when the user cancels the port name dialog

Arguments:

    hDlg    - handle to dialog

Return Value:

    TRUE success, FALSE an error occurred.

--*/
BOOL
PortNameCommandCancel(
    HWND hwnd
    )
{
    EndDialog(hwnd, ERROR_CANCELLED);

    return TRUE;
}


