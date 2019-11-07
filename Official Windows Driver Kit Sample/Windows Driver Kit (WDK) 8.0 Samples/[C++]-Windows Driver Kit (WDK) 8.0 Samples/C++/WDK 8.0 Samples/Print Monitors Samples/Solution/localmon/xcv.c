/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    xcv.c

Abstract:

    Implements xcv functions.

--*/

#include "precomp.h"


#pragma hdrstop

#include <DriverSpecs.h>
_Analysis_mode_(_Analysis_code_type_user_driver_)

//
// The ddk montior samples will be build with the name ddklocalmon and ddklocalui
// so they can be installed without clashing with existing files
//
//
// change this to the name of the dll that provides the ui for the monitor
//
#define SZLOCALUI  L"DDKLocalUI.dll"

_Success_(return == NO_ERROR)
DWORD
GetMonitorUI(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);

_Success_(return == NO_ERROR)
DWORD
DoPortExists(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);

_Success_(return == NO_ERROR)
DWORD
DoPortIsValid(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);

_Success_(return == NO_ERROR)
DWORD
DoDeletePort(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);

_Success_(return == NO_ERROR)
DWORD
DoAddPort(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);

_Success_(return == NO_ERROR)
DWORD
DoSetDefaultCommConfig(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE        pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);

_Success_(return == NO_ERROR)
DWORD
DoGetDefaultCommConfig(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);

_Success_(return == NO_ERROR)
DWORD
GetTransmissionRetryTimeout(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
);


typedef struct {
    PWSTR   pszMethod;
    DWORD   (*pfn)( PBYTE  pInputData,
                    DWORD  cbInputData,
                    PBYTE  pOutputData,
                    DWORD  cbOutputData,
                    PDWORD pcbOutputNeeded,
                    PINIXCVPORT pIniXcv
                    );
} XCV_METHOD, *PXCV_METHOD;


XCV_METHOD  gpLcmXcvMethod[] = {
                            {L"MonitorUI", GetMonitorUI},
                            {L"ConfigureLPTPortCommandOK", ConfigureLPTPortCommandOK},
                            {L"AddPort", DoAddPort},
                            {L"DeletePort", DoDeletePort},
                            {L"PortExists", DoPortExists},
                            {L"PortIsValid", DoPortIsValid},
                            {L"GetTransmissionRetryTimeout", GetTransmissionRetryTimeout},
                            {L"SetDefaultCommConfig", DoSetDefaultCommConfig},
                            {L"GetDefaultCommConfig", DoGetDefaultCommConfig},
                            {NULL, NULL}
                            };

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
    )
{
    DWORD dwRet = 0;
    DWORD i     = 0;


    if (!pcbOutputNeeded)
    {
        return ERROR_INVALID_PARAMETER;
    }

    *pcbOutputNeeded = 0;

    for(i = 0 ; gpLcmXcvMethod[i].pszMethod &&
                wcscmp(gpLcmXcvMethod[i].pszMethod, pszDataName) ; ++i)
        ;

    if (gpLcmXcvMethod[i].pszMethod) {
        dwRet = (*gpLcmXcvMethod[i].pfn)(  pInputData,
                                        cbInputData,
                                        pOutputData,
                                        cbOutputData,
                                        pcbOutputNeeded,
                                        (PINIXCVPORT) hXcv);

    } else {
        dwRet = ERROR_INVALID_PARAMETER;
    }

    return dwRet;
}

_Success_(return == TRUE)
BOOL
LcmXcvOpenPort(
    _In_    HANDLE hMonitor,
            LPCWSTR pszObject,
            ACCESS_MASK GrantedAccess,
            PHANDLE phXcv
    )
{
    PINILOCALMON pIniLocalMon = (PINILOCALMON)hMonitor;


    *phXcv = CreateXcvPortEntry(pIniLocalMon, pszObject, GrantedAccess);

    return !!*phXcv;
}

_Success_(return == TRUE)
BOOL
LcmXcvClosePort(
    _In_    HANDLE  hXcv
    )
{

    LcmEnterSplSem();
    DeleteXcvPortEntry((PINIXCVPORT) hXcv);
    LcmLeaveSplSem();

    return TRUE;
}

_Success_(return == NO_ERROR)
DWORD
DoDeletePort(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    DWORD dwRet = ERROR_SUCCESS;

    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pOutputData);
    UNREFERENCED_PARAMETER(cbOutputData);


    if (!pcbOutputNeeded)
    {
        return ERROR_INVALID_PARAMETER;
    }

    *pcbOutputNeeded = 0;

    if (!(pIniXcv->GrantedAccess & SERVER_ACCESS_ADMINISTER))
    {
        return ERROR_ACCESS_DENIED;
    }

    LcmEnterSplSem();

    if (LcmDeletePortEntry( pIniXcv->pIniLocalMon, (PWSTR)pInputData))
    {
        DeletePortFromRegistry ((PWSTR) pInputData);
    }
    else
        dwRet = ERROR_FILE_NOT_FOUND;

    LcmLeaveSplSem();

    return dwRet;
}

_Success_(return == NO_ERROR)
DWORD
DoPortExists(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    DWORD dwRet         = ERROR_SUCCESS;
    BOOL  bPortExists   = FALSE;

    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pIniXcv);


    *pcbOutputNeeded = sizeof(DWORD);

    if (cbOutputData < sizeof(DWORD)) {
        return ERROR_INSUFFICIENT_BUFFER;
    }

    bPortExists = PortExists(NULL, (PWSTR) pInputData, &dwRet);

    if (dwRet == ERROR_SUCCESS)
        *(DWORD *) pOutputData = bPortExists;

    return dwRet;
}

_Success_(return == NO_ERROR)
DWORD
DoPortIsValid(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    BOOL bRet = FALSE;

    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pOutputData);
    UNREFERENCED_PARAMETER(cbOutputData);
    UNREFERENCED_PARAMETER(pIniXcv);


    if (!pcbOutputNeeded)
    {
        return ERROR_INVALID_PARAMETER;
    }

    *pcbOutputNeeded = 0;

    bRet = PortIsValid((PWSTR) pInputData);

    return bRet ? ERROR_SUCCESS : GetLastError();
}

_Success_(return == NO_ERROR)
DWORD
DoAddPort(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    DWORD       dwRet       = ERROR_SUCCESS;
    BOOL        bPortExists = FALSE;
    PLCMINIPORT pIniPort    = NULL;

    UNREFERENCED_PARAMETER(pOutputData);
    UNREFERENCED_PARAMETER(cbOutputData);


    if (!(pIniXcv->GrantedAccess & SERVER_ACCESS_ADMINISTER))
    {
        return ERROR_ACCESS_DENIED;
    }

    if (!pcbOutputNeeded || !cbInputData || !pInputData ||
        (((wcslen((LPWSTR)pInputData) + 1) * sizeof(WCHAR)) != cbInputData))
    {
        return ERROR_INVALID_PARAMETER;
    }

    *pcbOutputNeeded = 0;

    bPortExists = PortExists(NULL, (PWSTR) pInputData, &dwRet);

    if (dwRet == ERROR_SUCCESS) {
        if (bPortExists) {
            SetLastError(ERROR_ALREADY_EXISTS);
        } else {
            pIniPort = LcmCreatePortEntry( pIniXcv->pIniLocalMon, (PWSTR)pInputData );

            if (pIniPort) {
                if (!AddPortInRegistry ((PWSTR) pInputData))
                {
                    LcmDeletePortEntry(pIniXcv->pIniLocalMon, (PWSTR) pInputData);
                    pIniPort = NULL;
                }
            }
        }
    }

    return pIniPort ? ERROR_SUCCESS : GetLastError();
}

_Success_(return == NO_ERROR)
DWORD
DoSetDefaultCommConfig(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    BOOL        bRet            = FALSE;
    DWORD       dwLength        = 0;
    PWSTR       pszPortName     = NULL;
    COMMCONFIG  *pCommConfig    = (COMMCONFIG *) pInputData;
    HANDLE      hToken          = NULL;

    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pOutputData);
    UNREFERENCED_PARAMETER(cbOutputData);
    UNREFERENCED_PARAMETER(pcbOutputNeeded);


    if (!pcbOutputNeeded)
    {
        return ERROR_INVALID_PARAMETER;
    }

    *pcbOutputNeeded = 0;

    if (!(pIniXcv->GrantedAccess & SERVER_ACCESS_ADMINISTER))
    {
        return ERROR_ACCESS_DENIED;
    }

    dwLength = (DWORD)wcslen(pIniXcv->pszName);
    if (pIniXcv->pszName[dwLength - 1] == L':')
    {
        pszPortName = AllocSplStr(pIniXcv->pszName);
        if (pszPortName)
        {
            pszPortName[dwLength - 1] = L'\0';

            hToken = RevertToPrinterSelf();

            if (hToken)
            {
                bRet = SetDefaultCommConfig(pszPortName,
                                            pCommConfig,
                                            pCommConfig->dwSize);

                ImpersonatePrinterClient(hToken);
            }
        }
    }

    FreeSplStr(pszPortName);

    return bRet ? ERROR_SUCCESS : GetLastError();
}

_Success_(return == NO_ERROR)
DWORD
DoGetDefaultCommConfig(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    PWSTR       pszPortName     = (PWSTR) pInputData;
    COMMCONFIG  *pCommConfig    = (COMMCONFIG *) pOutputData;
    BOOL        bRet            = FALSE;

    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pIniXcv);


    if (cbOutputData < sizeof(COMMCONFIG))
    {
        //
        // at least size of COMMCONFIG is needed
        //
        if (pcbOutputNeeded)
        {
            *pcbOutputNeeded = sizeof (COMMCONFIG);
        }
        return ERROR_INSUFFICIENT_BUFFER;
    }

    pCommConfig->dwProviderSubType = PST_RS232;
    *pcbOutputNeeded = cbOutputData;

    bRet = GetDefaultCommConfig(pszPortName, pCommConfig, pcbOutputNeeded);

    return bRet ? ERROR_SUCCESS : GetLastError();
}

_Success_(return == NO_ERROR)
DWORD
GetTransmissionRetryTimeout(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    UNREFERENCED_PARAMETER(pInputData);
    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pIniXcv);


    *pcbOutputNeeded = sizeof(DWORD);

    if (cbOutputData < sizeof(DWORD))
        return ERROR_INSUFFICIENT_BUFFER;

    GetTransmissionRetryTimeoutFromRegistry ((PDWORD) pOutputData);

    return ERROR_SUCCESS;
}

_Success_(return == NO_ERROR)
DWORD
GetMonitorUI(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    DWORD dwRet = ERROR_SUCCESS;

    UNREFERENCED_PARAMETER(pInputData);
    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pIniXcv);


    if (!pcbOutputNeeded)
    {
        return ERROR_INVALID_PARAMETER;
    }

    *pcbOutputNeeded = sizeof(SZLOCALUI);

    if (cbOutputData < *pcbOutputNeeded)
    {
        dwRet =  ERROR_INSUFFICIENT_BUFFER;
    }
    else
    {
        dwRet = HRESULT_CODE(StringCbCopy ((PWSTR) pOutputData, cbOutputData, SZLOCALUI));
    }

    return dwRet;
}

_Success_(return == NO_ERROR)
DWORD
ConfigureLPTPortCommandOK(
    _In_reads_bytes_(cbInputData)    PBYTE       pInputData,
    _In_                             DWORD       cbInputData,
    _Out_writes_bytes_(cbOutputData) PBYTE       pOutputData,
    _In_                             DWORD       cbOutputData,
    _Out_                            PDWORD      pcbOutputNeeded,
    _Inout_                          PINIXCVPORT pIniXcv
)
{
    DWORD dwStatus  = ERROR_SUCCESS;

    UNREFERENCED_PARAMETER(cbInputData);
    UNREFERENCED_PARAMETER(pOutputData);
    UNREFERENCED_PARAMETER(cbOutputData);


    if (!pcbOutputNeeded)
    {
        return ERROR_INVALID_PARAMETER;
    }

    *pcbOutputNeeded = 0;

    if (!(pIniXcv->GrantedAccess & SERVER_ACCESS_ADMINISTER))
    {
        dwStatus = ERROR_ACCESS_DENIED;
    }
    //
    // set timeout value in Registry
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        dwStatus = SetTransmissionRetryTimeoutInRegistry ((LPWSTR) pInputData);
    }
    return dwStatus;
}


