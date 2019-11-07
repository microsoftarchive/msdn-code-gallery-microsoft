/*++

Copyright (c) 1990-1998  Microsoft Corporation
All rights reserved

Module Name:

    localmon.h

--*/
#ifndef _LOCALMON_H_
#define _LOCALMON_H_


#define COUNTOF(x)                (sizeof(x)/sizeof *(x))

#include "mem.h"

#define SPLASSERT(arg)


extern HINSTANCE   LcmhInst;
extern CRITICAL_SECTION    LcmSpoolerSection;
extern DWORD    LcmPortInfo1Strings[];
extern DWORD    LcmPortInfo2Strings[];
extern PLCMINIPORT pIniFirstPort;
extern PINIXCVPORT pIniFirstXcvPort;

extern _Null_terminated_ WCHAR szNULL[];
extern _Null_terminated_ WCHAR szWindows[];
extern _Null_terminated_ WCHAR szINIKey_TransmissionRetryTimeout[];
extern _Null_terminated_ WCHAR szLcmDeviceNameHeader[];
extern _Null_terminated_ WCHAR szNUL[];
extern _Null_terminated_ WCHAR szNUL_COLON[];
extern _Null_terminated_ WCHAR szLcmCOM[];
extern _Null_terminated_ WCHAR szLcmLPT[];
extern _Null_terminated_ WCHAR szIRDA[];

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

#define IS_FILE_PORT(pName) \
    (!_wcsicmp(pName, L"FILE:") || !_wcsicmp(pName, L"PORTPROMPT:"))

#define IS_NUL_PORT(pName) \
    (!_wcsicmp( pName, szNUL ) || !_wcsicmp( pName, szNUL_COLON ) )

#define IS_IRDA_PORT(pName) \
    !_wcsicmp( pName, szIRDA )

#define IS_COM_PORT(pName) \
    IsCOMPort( pName )

#define IS_LPT_PORT(pName) \
    IsLPTPort( pName )

VOID
CompleteRead(
            DWORD Error,
            DWORD ByteCount,
    _In_    LPOVERLAPPED pOverlapped
    );

BOOL
PortExists(
    _In_opt_ LPWSTR pName,
    _In_     LPWSTR pPortName,
    _Out_    PDWORD pError
    );

BOOL
PortIsValid(
    _In_    LPWSTR pPortName
    );

BOOL
IsCOMPort(
    _In_    LPWSTR pPort
    );

BOOL
IsLPTPort(
    _In_    LPWSTR pPort
    );

_Acquires_lock_(LcmSpoolerSection)
VOID
LcmEnterSplSem(
    VOID
    );

_Releases_lock_(LcmSpoolerSection)
VOID
LcmLeaveSplSem(
    VOID
    );

VOID
LcmSplOutSem(
    VOID
    );

PINIENTRY
LcmFindIniKey(
    _In_    PINIENTRY pIniEntry,
    _In_    LPWSTR lpName
    );

LPBYTE
LcmPackStrings(
    _In_                            DWORD   dwElementsCount,
    _In_reads_(dwElementsCount)     LPCWSTR *pSource,
    _Out_writes_bytes_(pDest-pEnd)  LPBYTE  pDest,
    _In_reads_(dwElementsCount)     DWORD   *DestOffsets,
    _Inout_updates_(_Inexpressible_("Involves negative offsets.")) LPBYTE  pEnd
    );

VOID
LcmRemoveColon(
    _Inout_ LPWSTR  pName
    );

PLCMINIPORT
LcmCreatePortEntry(
    _Inout_ PINILOCALMON pIniLocalMon,
    _In_    LPCWSTR   pPortName
    );

BOOL
LcmDeletePortEntry(
    _Inout_ PINILOCALMON pIniLocalMon,
    _In_    LPWSTR   pPortName
    );

PINIXCVPORT
CreateXcvPortEntry(
    _Inout_ PINILOCALMON pIniLocalMon,
            LPCWSTR pszName,
            ACCESS_MASK GrantedAccess
    );

BOOL
DeleteXcvPortEntry(
    _In_ PINIXCVPORT  pIniXcvPort
    );

BOOL
GetIniCommValues(
    _In_ LPWSTR         pName,
         LPDCB          pdcb,
         LPCOMMTIMEOUTS pcto
    );

BOOL
LocalAddPortEx(
    _In_ LPWSTR   pName,
         DWORD    Level,
         LPBYTE   pBuffer,
    _In_ LPWSTR   pMonitorName
    );

_Success_(return == NO_ERROR)
DWORD
ConfigureLPTPortCommandOK(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
    );

DWORD
GetPortSize(
    _In_    PLCMINIPORT pIniPort,
            DWORD       Level
    );

_Success_(return != NULL)
LPBYTE
CopyIniPortToPort(
    _In_             PLCMINIPORT pIniPort,
    _In_range_(1, 2) DWORD       Level,
    _When_(Level == 1, _Out_writes_bytes_(sizeof(PORT_INFO_1)))
    _When_(Level == 2, _Out_writes_bytes_(sizeof(PORT_INFO_2)))
                     LPBYTE      pPortInfo,
    _Inout_updates_(_Inexpressible_("Involves negative offsets."))
                     LPBYTE      pEnd
    );

BOOL
GetCOMPort(
    _Inout_ PLCMINIPORT pIniPort
    );

BOOL
ReleaseCOMPort(
    _Inout_ PLCMINIPORT pIniPort
    );

BOOL
ValidateDosDevicePort(
    _Inout_ PLCMINIPORT    pIniPort
    );

BOOL
RemoveDosDeviceDefinition(
    _In_    PLCMINIPORT pIniPort
    );

BOOL
DeletePortNode(
    _Inout_ PINILOCALMON pIniLocalMon,
    _In_    PLCMINIPORT  pIniPort
    );

BOOL
FixupDosDeviceDefinition(
    _Inout_ PLCMINIPORT  pIniPort
    );

_Success_(return == NO_ERROR)
DWORD
LcmXcvDataPort(
    _In_                             HANDLE  hXcv,
    _In_                             LPCWSTR pszDataName,
    _In_reads_bytes_(cbInputData)    PBYTE   pInputData,
    _In_                             DWORD   cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE   pOutputData,
    _In_                             DWORD   cbOutputData,
    _Out_                            PDWORD  pcbOutputNeeded
    );

_Success_(return == TRUE)
BOOL
LcmXcvOpenPort(
    _In_    HANDLE hMonitor,
            LPCWSTR pszObject,
            ACCESS_MASK GrantedAccess,
            PHANDLE phXcv
    );

_Success_(return == TRUE)
BOOL
LcmXcvClosePort(
    _In_    HANDLE  hXcv
    );

DWORD
WINAPIV
StrNCatBuffW(
    _Out_writes_(cchBuffer) PWSTR pszBuffer,
    UINT        cchBuffer,
    ...
    );

#endif // _LOCALMON_H_

_Success_(return == TRUE)
BOOL
GetIniCommValuesFromRegistry (
    _In_  LPCWSTR     pszPortName,
    _Out_ LPWSTR*     ppszCommValues
    );

VOID
GetTransmissionRetryTimeoutFromRegistry (
    _Out_ DWORD*      pdwTimeout
    );

DWORD
SetTransmissionRetryTimeoutInRegistry (
    _In_ LPCWSTR     pszTimeout
    );

BOOL
AddPortInRegistry (
    _In_ LPCWSTR     pszPortName
    );

VOID
DeletePortFromRegistry (
    _In_ LPCWSTR     pszPortName
    );

LPWSTR
AdjustFileName(
    _In_ LPWSTR pFileName
    );

