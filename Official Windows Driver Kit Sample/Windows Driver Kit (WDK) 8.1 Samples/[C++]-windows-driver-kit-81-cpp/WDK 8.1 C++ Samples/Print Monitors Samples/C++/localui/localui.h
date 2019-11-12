/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    localui.h

--*/

#ifndef _LOCALUI_H_
#define _LOCALUI_H_

typedef struct _PORTDIALOG {
    HANDLE  hXcv;
    PWSTR   pszServer;
    PWSTR   pszPortName;
    DWORD   dwRet;
} PORTDIALOG, *PPORTDIALOG;

extern WCHAR szPorts[];
extern WCHAR szWindows[];
extern WCHAR szINIKey_TransmissionRetryTimeout[];
extern WCHAR szDeviceNameHeader[];
extern WCHAR szFILE[];
extern WCHAR szCOM[];
extern WCHAR szLPT[];

extern HINSTANCE    hInst;
extern WCHAR        szCOM[];
extern WCHAR        szLPT[];

#define MSG_ERROR           MB_OK | MB_ICONSTOP
#define MSG_WARNING         MB_OK | MB_ICONEXCLAMATION
#define MSG_YESNO           MB_YESNO | MB_ICONQUESTION
#define MSG_INFORMATION     MB_OK | MB_ICONINFORMATION
#define MSG_CONFIRMATION    MB_OKCANCEL | MB_ICONEXCLAMATION

#define TIMEOUT_MIN         1
#define TIMEOUT_MAX         999999
#define TIMEOUT_STRING_MAX  6

#define WITHINRANGE( val, lo, hi ) \
    ( ( val <= hi ) && ( val >= lo ) )

#define IS_COM_PORT(pName) \
    IsCOMPort( pName )

#define IS_LPT_PORT(pName) \
    IsLPTPort( pName )

BOOL
IsCOMPort(
    PCWSTR pPort
    );

BOOL
IsLPTPort(
    PCWSTR pPort
    );

INT_PTR APIENTRY
ConfigureLPTPortDlg(
            HWND   hwnd,
            UINT   msg,
    _In_    WPARAM wparam,
    _In_    LPARAM lparam
    );

INT
WINAPIV
Message(
    HWND    hwnd,
    DWORD   Type,
    INT     CaptionID,
    INT     TextID,
    ...
    );

INT_PTR CALLBACK
PortNameDlg(
            HWND   hwnd,
            WORD   msg,
    _In_    WPARAM wparam,
    _In_    LPARAM lparam
    );

PWSTR
ConstructXcvName(
    PCWSTR pServerName,
    PCWSTR pObjectName,
    PCWSTR pObjectType
    );

INT
ErrorMessage(
    HWND    hwnd,
    DWORD   dwStatus
    );

#endif // _LOCALUI_H_

