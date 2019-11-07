/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    winspool.c

Abstract:

    Implements the spooler supported apis for printing.

--*/

#include "precomp.h"


#pragma hdrstop

#include <DriverSpecs.h>
_Analysis_mode_(_Analysis_code_type_user_driver_)

_Null_terminated_ WCHAR   szNULL[] = L"";
_Null_terminated_ WCHAR   szLcmDeviceNameHeader[] = L"\\Device\\NamedPipe\\Spooler\\";
_Null_terminated_ WCHAR   szWindows[] = L"windows";
_Null_terminated_ WCHAR   szINIKey_TransmissionRetryTimeout[] = L"TransmissionRetryTimeout";


//
// Timeouts for serial printing
//
DWORD   g_COMWriteTimeoutConstant_ms = 30000; // 30 seconds

#define READ_TOTAL_TIMEOUT      5000    // 5 seconds
#define READ_INTERVAL_TIMEOUT   200     // 0.2 second


BOOL
DeletePortNode(
    _Inout_ PINILOCALMON pIniLocalMon,
    _In_    PLCMINIPORT  pIniPort
    )
{
    PLCMINIPORT    pPort       = NULL;
    PLCMINIPORT    pPrevPort   = NULL;


    for( pPort = pIniLocalMon->pIniPort;
         pPort && pPort != pIniPort;
         pPort = pPort->pNext){

        pPrevPort = pPort;
    }

    if (pPort) {    // found the port
        if (pPort == pIniLocalMon->pIniPort) {
            pIniLocalMon->pIniPort = pPort->pNext;
        } else {
            pPrevPort->pNext = pPort->pNext;
        }
        FreeSplMem(pPort);

        return TRUE;
    }
    else            // port not found
        return FALSE;
}


BOOL
RemoveDosDeviceDefinition(
    _In_    PLCMINIPORT    pIniPort
    )
/*++

Routine Description:
    Removes the NONSPOOLED.. dos device definition created by localmon

Arguments:
    pIniPort    : Pointer to the INIPORT

Return Value:
    TRUE on success, FALSE on error

--*/
{
    WCHAR   TempDosDeviceName[MAX_PATH] = {0};


    if( ERROR_SUCCESS != StrNCatBuffW( TempDosDeviceName, COUNTOF(TempDosDeviceName),
                                       L"NONSPOOLED_", pIniPort->pName, NULL ))
        return FALSE;

    LcmRemoveColon(TempDosDeviceName);

    return DefineDosDevice(DDD_REMOVE_DEFINITION, TempDosDeviceName, NULL);
}


BOOL
ValidateDosDevicePort(
    _Inout_ PLCMINIPORT    pIniPort
    )
/*++

Routine Description:
    Checks if the given port corresponds to a dos device.
    For a dos device port the following is done:
        -- Dos device definition for the NONSPOOLED.. is created
        -- CreateFile is done on the NONSPOOLED.. port

Arguments:
    pIniPort    : Pointer to the INIPORT

Return Value:
    TRUE on all validations passing, FALSE otherwise

    Side effect:
        For dos devices :
        a. CreateFile is called on the NONSPOOLED.. name
        b. PP_DOSDEVPORT flag is set
        c. pIniPort->pDeviceName is set to the first string found on
           QueryDosDefition this could be used to see if the definition changed
           (ex. when user did a net use lpt1 \\server\printer the connection
                is effective only when the user is logged in)
        d. PP_COMM_PORT is set for real LPT/COM port
           (ie. GetCommTimeouts worked, not a net use lpt1 case)

--*/
{
    DCB             dcb                         = {0};
    COMMTIMEOUTS    cto                         = {0};
    LPWSTR          pszTempDosDeviceName        = NULL;
    HANDLE          hToken                      = NULL;
    LPWSTR          pszDeviceNames              = NULL;
    LPWSTR          pszDosDeviceName            = NULL;
    LPWSTR          pszNewNtDeviceName          = NULL;
    WCHAR          *pDeviceNames                = NULL;
    BOOL            bRet                        = FALSE;
    LPWSTR          pDeviceName                 = NULL;


    // if this is a COM port and we already have a file handle cached,
    // bump the count and get out.
    if (IS_COM_PORT(pIniPort->pName))
    {
        if ( (pIniPort->hFile != NULL ) &&
             (pIniPort->hFile != INVALID_HANDLE_VALUE) )
        {
            SPLASSERT(pIniPort->cRef > 0);
            bRet = TRUE;
            goto Done;
        }
    }

    hToken = RevertToPrinterSelf();
    if (!hToken)
       goto Done;

    pszDosDeviceName = (LPWSTR)AllocSplMem(MAX_PATH * sizeof(WCHAR));

    if(!pszDosDeviceName)
        goto Done;

    if( ERROR_SUCCESS != StrNCatBuffW( pszDosDeviceName, MAX_PATH,
                                       pIniPort->pName, NULL ))
        goto Done;

    LcmRemoveColon(pszDosDeviceName);

    pszDeviceNames = (LPWSTR)AllocSplMem(MAX_PATH * sizeof(WCHAR));

    if(!pszDeviceNames)
        goto Done;

    pDeviceNames = pszDeviceNames;

    //
    // If the port is not a dos device port nothing to do -- return success
    //
    if ( !QueryDosDevice(pszDosDeviceName, pszDeviceNames, MAX_PATH) ) {

        bRet = TRUE;
        goto Done;
    }

    pDeviceName = AllocSplStr(pDeviceNames);
    if ( !pDeviceName )
        goto Done;

    pszNewNtDeviceName = (LPWSTR)AllocSplMem(MAX_PATH * sizeof(WCHAR));

    if(!pszNewNtDeviceName)
        goto Done;

    if( ERROR_SUCCESS != StrNCatBuffW( pszNewNtDeviceName, MAX_PATH,
                                       szLcmDeviceNameHeader, pIniPort->pName, NULL ))
        goto Done;

    LcmRemoveColon(pszNewNtDeviceName);

    //
    // Search for the first non-matching name in pDeviceNames list.
    //
    while ( lstrcmpi(pDeviceNames, pszNewNtDeviceName) == 0 ) {

        pDeviceNames+=wcslen(pDeviceNames)+1;
    }

    pszTempDosDeviceName = (LPWSTR)AllocSplMem(MAX_PATH * sizeof(WCHAR));

    if(!pszTempDosDeviceName)
        goto Done;

    if( ERROR_SUCCESS != StrNCatBuffW( pszTempDosDeviceName, MAX_PATH,
                                       L"NONSPOOLED_", pIniPort->pName, NULL ))
        goto Done;

    LcmRemoveColon(pszTempDosDeviceName);

    //
    // Delete any existing definition for pszTempDosDeviceName. This ensures that
    // there exists only one definition for the nonspooled_port device name.
    //
    DefineDosDevice(DDD_REMOVE_DEFINITION, pszTempDosDeviceName, NULL);
    DefineDosDevice(DDD_RAW_TARGET_PATH, pszTempDosDeviceName, pDeviceNames);

    ImpersonatePrinterClient(hToken);
    hToken = NULL;

    if( ERROR_SUCCESS != StrNCatBuffW( pszTempDosDeviceName, MAX_PATH,
                                       L"\\\\.\\NONSPOOLED_", pIniPort->pName, NULL ))
        goto Done;

    LcmRemoveColon(pszTempDosDeviceName);

    pIniPort->hFile = CreateFile(pszTempDosDeviceName,
                                 GENERIC_READ | GENERIC_WRITE,
                                 FILE_SHARE_READ,
                                 NULL,
                                 OPEN_ALWAYS,
                                 FILE_ATTRIBUTE_NORMAL |
                                 FILE_FLAG_SEQUENTIAL_SCAN,
                                 NULL);

    //
    // If CreateFile fails remove redirection and fail the call
    //
    if ( pIniPort->hFile == INVALID_HANDLE_VALUE ) {

        (VOID)RemoveDosDeviceDefinition(pIniPort);
        goto Done;
    }

    pIniPort->Status |= PP_DOSDEVPORT;

    SetEndOfFile(pIniPort->hFile);

    if ( IS_COM_PORT (pIniPort->pName) ) {

        if ( GetCommState(pIniPort->hFile, &dcb) ) {

            GetCommTimeouts(pIniPort->hFile, &cto);
            GetIniCommValues (pIniPort->pName, &dcb, &cto);
            SetCommState (pIniPort->hFile, &dcb);
            cto.WriteTotalTimeoutConstant   = g_COMWriteTimeoutConstant_ms;
            cto.WriteTotalTimeoutMultiplier = 0;
            cto.ReadTotalTimeoutConstant    = READ_TOTAL_TIMEOUT;
            cto.ReadIntervalTimeout         = READ_INTERVAL_TIMEOUT;
            SetCommTimeouts(pIniPort->hFile, &cto);

            pIniPort->Status |= PP_COMM_PORT;
        } else {

        }
    } else if ( IS_LPT_PORT (pIniPort->pName) ) {

        if ( GetCommTimeouts(pIniPort->hFile, &cto) ) {

            GetTransmissionRetryTimeoutFromRegistry (&cto.WriteTotalTimeoutConstant);

            cto.WriteTotalTimeoutConstant*=1000;
            SetCommTimeouts(pIniPort->hFile, &cto);


            pIniPort->Status |= PP_COMM_PORT;
        } else {

        }
    }

    FreeSplStr( pIniPort->pDeviceName );

    pIniPort->pDeviceName = pDeviceName;
    bRet = TRUE;

Done:
    if (hToken)
        ImpersonatePrinterClient(hToken);

    if ( !bRet && pDeviceName )
        FreeSplStr(pDeviceName);

    if (pszTempDosDeviceName)
    {
        FreeSplMem(pszTempDosDeviceName);
        pszTempDosDeviceName = NULL;
    }

    if (pszDeviceNames)
    {
        FreeSplMem(pszDeviceNames);
        pszDeviceNames = NULL;
    }

    if (pszDosDeviceName)
    {
        FreeSplMem(pszDosDeviceName);
        pszDosDeviceName = NULL;
    }

    if (pszNewNtDeviceName)
    {
        FreeSplMem(pszNewNtDeviceName);
        pszNewNtDeviceName = NULL;
    }

    return bRet;
}


BOOL
FixupDosDeviceDefinition(
    _Inout_ PLCMINIPORT    pIniPort
    )
/*++

Routine Description:
    Called before every StartDocPort for a DOSDEVPORT. The routine will check if
    the dos device defintion has changed (if a user logged and his connection
    is remembered). Also for a connection case the CreateFile is called since
    that needs to be done per job

Arguments:
    pIniPort    : Pointer to the INIPORT

Return Value:
    TRUE on all validations passing, FALSE otherwise

--*/
{
    LPWSTR  pszDeviceNames          = NULL;
    LPWSTR  pszDosDeviceName        = NULL;
    HANDLE  hToken                  = NULL;
    BOOL    bRetValue               = FALSE;


    //
    // If the port is not a real LPT port we open it per job
    //

    if ( !(pIniPort->Status & PP_COMM_PORT) ||
         pIniPort->hFile == INVALID_HANDLE_VALUE )
    {
        bRetValue = ValidateDosDevicePort(pIniPort);
        goto Done;
    }

    pszDosDeviceName = (LPWSTR)AllocSplMem(MAX_PATH * sizeof(WCHAR));

    if (!pszDosDeviceName)
    {
        goto Done;
    }

    if( ERROR_SUCCESS != StrNCatBuffW( pszDosDeviceName, MAX_PATH,
                                       pIniPort->pName, NULL ))
        goto Done;

    LcmRemoveColon(pszDosDeviceName);

    hToken = RevertToPrinterSelf();

    if (!hToken) {
        goto Done;
    }

    pszDeviceNames = (LPWSTR)AllocSplMem(MAX_PATH * sizeof(WCHAR));

    if (!pszDeviceNames)
    {
        goto Done;
    }

    if ( !QueryDosDevice(pszDosDeviceName, pszDeviceNames, MAX_PATH) ) {

        ImpersonatePrinterClient(hToken);
        goto Done;
    }

    //
    // If strings are same then definition has not changed
    //
    if ( !lstrcmpi(pszDeviceNames, pIniPort->pDeviceName) )
    {
        ImpersonatePrinterClient(hToken);
        bRetValue = TRUE;
        goto Done;
    }

    (VOID)RemoveDosDeviceDefinition(pIniPort);

    CloseHandle(pIniPort->hFile);
    pIniPort->hFile = INVALID_HANDLE_VALUE;


    pIniPort->Status &= ~(PP_COMM_PORT | PP_DOSDEVPORT);

    FreeSplStr(pIniPort->pDeviceName);
    pIniPort->pDeviceName = NULL;

    ImpersonatePrinterClient(hToken);

    bRetValue = ValidateDosDevicePort(pIniPort);

Done:

    if (pszDeviceNames)
    {
        FreeSplMem(pszDeviceNames);
        pszDeviceNames = NULL;
    }

    if (pszDosDeviceName)
    {
        FreeSplMem(pszDosDeviceName);
        pszDosDeviceName = NULL;
    }

    return bRetValue;
}


BOOL
GetCOMPort(
    _Inout_ PLCMINIPORT    pIniPort
    )
{
    BOOL frc = FALSE;


    LcmEnterSplSem();

    frc =  ValidateDosDevicePort(pIniPort);
    if (frc && (pIniPort->hFile != INVALID_HANDLE_VALUE))
    {
        pIniPort->cRef++;
    }
    else
    {
        // if we did not get a file handle, fail the call.
        if (pIniPort->hFile == INVALID_HANDLE_VALUE)
        {
            frc = FALSE;
            SetLastError(ERROR_PRINTER_NOT_FOUND);
        }
        else
        {
        }
    }

    LcmLeaveSplSem();

    return frc;
} // GetCOMPort()


BOOL
ReleaseCOMPort(
    _Inout_ PLCMINIPORT    pIniPort
    )
{

    LcmEnterSplSem();
    pIniPort->cRef--;

    // we should always have a valid file handle
    SPLASSERT(pIniPort->hFile != INVALID_HANDLE_VALUE);


    if (pIniPort->cRef > 0)
    {
        goto done;
    }

    CloseHandle(pIniPort->hFile);
    pIniPort->hFile = INVALID_HANDLE_VALUE;


    if ( pIniPort->Status & PP_DOSDEVPORT )
    {
        (VOID)RemoveDosDeviceDefinition(pIniPort);
    }

    pIniPort->Status &= ~(PP_COMM_PORT | PP_DOSDEVPORT);
    FreeSplStr(pIniPort->pDeviceName);
    pIniPort->pDeviceName = NULL;

done:
    LcmLeaveSplSem();
    return TRUE;
} // ReleaseCOMPort

