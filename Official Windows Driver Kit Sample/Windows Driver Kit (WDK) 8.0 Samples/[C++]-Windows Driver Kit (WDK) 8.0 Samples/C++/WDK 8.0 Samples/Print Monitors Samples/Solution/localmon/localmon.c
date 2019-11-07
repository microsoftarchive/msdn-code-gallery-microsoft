/*++

Copyright (c) 1990-2003  Microsoft Corporation
All rights reserved

Module Name:

    localmon.c

--*/

#include "precomp.h"


#pragma hdrstop

#include <DriverSpecs.h>
_Analysis_mode_(_Analysis_code_type_user_driver_)

#include <lmon.h>
#include "irda.h"

HANDLE              LcmhMonitor;
HINSTANCE           LcmhInst;
CRITICAL_SECTION    LcmSpoolerSection;

PMONITORINIT        g_pMonitorInit;

DWORD LcmPortInfo1Strings[]={FIELD_OFFSET(PORT_INFO_1, pName),
                          (DWORD)-1};

DWORD LcmPortInfo2Strings[]={FIELD_OFFSET(PORT_INFO_2, pPortName),
                          FIELD_OFFSET(PORT_INFO_2, pMonitorName),
                          FIELD_OFFSET(PORT_INFO_2, pDescription),
                          (DWORD)-1};

_Null_terminated_ WCHAR g_szPortsKey[]  = L"Ports";
_Null_terminated_ WCHAR gszWindows[]= L"Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows";
_Null_terminated_ WCHAR szPortsEx[] = L"portsex"; /* Extra ports values */
_Null_terminated_ WCHAR szNUL[]     = L"NUL";
_Null_terminated_ WCHAR szNUL_COLON[] = L"NUL:";
_Null_terminated_ WCHAR szLcmCOM[]  = L"COM";
_Null_terminated_ WCHAR szLcmLPT[]  = L"LPT";
_Null_terminated_ WCHAR szIRDA[]    = L"IR";

extern DWORD g_COMWriteTimeoutConstant_ms;

BOOL
LocalMonInit(HINSTANCE hModule)
{

    LcmhInst = hModule;

    return InitializeCriticalSectionAndSpinCount(&LcmSpoolerSection, 0x80000000);
}


VOID
LocalMonCleanUp(
    VOID
    )
{

    DeleteCriticalSection(&LcmSpoolerSection);
}

_Success_(return != FALSE)
BOOL
LcmEnumPorts(
    _In_                        HANDLE  hMonitor,
    _In_opt_                    LPWSTR  pName,
                                DWORD   Level,
    _Out_writes_bytes_opt_(cbBuf)
                                LPBYTE  pPorts,
                                DWORD   cbBuf,
    _Out_                       LPDWORD pcbNeeded,
    _Out_                       LPDWORD pcReturned
    )
{
    PINILOCALMON    pIniLocalMon    = (PINILOCALMON)hMonitor;
    PLCMINIPORT     pIniPort        = NULL;
    DWORD           cb              = 0;
    LPBYTE          pEnd            = NULL;
    DWORD           LastError       = 0;

    UNREFERENCED_PARAMETER(pName);


    if (!pcbNeeded || !pcReturned || (!pPorts && (cbBuf > 0)))
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }
    else if ((1 != Level) && (2 != Level))
    {
        SetLastError(ERROR_INVALID_LEVEL);
        return FALSE;
    }

    *pcReturned = 0;

    LcmEnterSplSem();

    cb = 0;

    pIniPort = pIniLocalMon->pIniPort;

    CheckAndAddIrdaPort(pIniLocalMon);

    while (pIniPort)
    {
        if (!(pIniPort->Status & PP_FILEPORT))
        {
            cb += GetPortSize(pIniPort, Level);
        }
        pIniPort = pIniPort->pNext;
    }

    *pcbNeeded = cb;
    *pcReturned = 0;

    if (pPorts && (*pcbNeeded <= cbBuf))
    {
        pEnd = pPorts + cbBuf;

        pIniPort = pIniLocalMon->pIniPort;

        while (pIniPort)
        {
            if (!(pIniPort->Status & PP_FILEPORT))
            {
                #pragma prefast(suppress:__WARNING_POTENTIAL_BUFFER_OVERFLOW_HIGH_PRIORITY, "The total buffer size is not exceeded, *pcbNeeded is smaller than or equal with cbBuf")
                pEnd = CopyIniPortToPort(pIniPort, Level, pPorts, pEnd);
                if (!pEnd)
                {
                    LastError = GetLastError();
                    break;
                }

                pPorts += (1 == Level) ? sizeof(PORT_INFO_1) : sizeof(PORT_INFO_2);
                (*pcReturned)++;
            }

            pIniPort = pIniPort->pNext;
        }
    }
    else
    {
        LastError = ERROR_INSUFFICIENT_BUFFER;
    }

    LcmLeaveSplSem();

    if (LastError)
    {
        SetLastError(LastError);
        return FALSE;
    }
    else
    {
        return TRUE;
    }
}

_Success_(return != FALSE)
BOOL
LcmOpenPort(
    _In_    HANDLE  hMonitor,
    _In_    LPWSTR  pName,
    _Out_   PHANDLE pHandle
    )
{
    PINILOCALMON    pIniLocalMon    = (PINILOCALMON)hMonitor;
    PLCMINIPORT     pIniPort        = NULL;
    BOOL            bRet            = FALSE;


    LcmEnterSplSem();

    if (!pName || !pHandle)
    {
        SetLastError (ERROR_INVALID_PARAMETER);
        goto Done;
    }

    *pHandle = NULL;

    if ( IS_FILE_PORT(pName) ) {


        //
        // We will always create multiple file port
        // entries, so that the spooler can print
        // to multiple files.
        //
        pIniPort = LcmCreatePortEntry( pIniLocalMon, pName );
        if ( !pIniPort )
            goto Done;

        pIniPort->Status |= PP_FILEPORT;
        *pHandle = pIniPort;
        bRet = TRUE;
        goto Done;
    }

    pIniPort = FindPort(pIniLocalMon, pName);

    if ( !pIniPort )
        goto Done;

    //
    // For LPT ports language monitors could do reads outside Start/End doc
    // port to do bidi even when there are no jobs printing. So we do a
    // CreateFile and keep the handle open all the time.
    //
    // But for COM ports you could have multiple devices attached to a COM
    // port (ex. a printer and some other device with a switch)
    // To be able to use the other device they write a utility which will
    // do a net stop serial and then use the other device. To be able to
    // stop the serial service spooler should not have a handle to the port.
    // So we need to keep handle to COM port open only when there is a job
    // printing
    //
    //
    if ( IS_COM_PORT(pName) ) {

        bRet = TRUE;
        goto Done;
    }

    //
    // If it is not a port redirected we are done (succeed the call)
    //
    if ( ValidateDosDevicePort(pIniPort) ) {

        bRet = TRUE;

        //
        // If it isn't a true dosdevice port (ex. net use lpt1 \\<server>\printer)
        // then we need to do CreateFile and CloseHandle per job so that
        // StartDoc/EndDoc is issued properly for the remote printer
        //
        if ( (pIniPort->Status & PP_DOSDEVPORT) &&
            !(pIniPort->Status & PP_COMM_PORT) ) {

            CloseHandle(pIniPort->hFile);
            pIniPort->hFile = INVALID_HANDLE_VALUE;

            (VOID)RemoveDosDeviceDefinition(pIniPort);
        }
    }

Done:
    if ( !bRet && pIniPort && (pIniPort->Status & PP_FILEPORT) )
        DeletePortNode(pIniLocalMon, pIniPort);

    if ( bRet )
        *pHandle = pIniPort;

    LcmLeaveSplSem();
    return bRet;
}

BOOL
LcmStartDocPort(
    _In_             HANDLE  hPort,
    _In_             LPWSTR  pPrinterName,
                     DWORD   JobId,
    _In_range_(1, 2) DWORD   Level,
    _When_(Level == 1, _In_reads_bytes_(sizeof(DOC_INFO_1)))
    _When_(Level == 2, _In_reads_bytes_(sizeof(DOC_INFO_2)))
                     LPBYTE  pDocInfo)
{
    PLCMINIPORT pIniPort        = (PLCMINIPORT)hPort;
    PDOC_INFO_1 pDocInfo1       = (PDOC_INFO_1)pDocInfo;
    DWORD       Error           = 0;
    LPWSTR      pAdjustedName   = NULL;

    UNREFERENCED_PARAMETER(Level);


    if (pIniPort->Status & PP_STARTDOC)
    {
        return TRUE;
    }

    LcmEnterSplSem();
    pIniPort->Status |= PP_STARTDOC;
    LcmLeaveSplSem();

    pIniPort->hPrinter = NULL;
    pIniPort->pPrinterName = AllocSplStr(pPrinterName);

    if (pIniPort->pPrinterName)
    {

        if (OpenPrinter(pPrinterName, &pIniPort->hPrinter, NULL))
        {

            pIniPort->JobId = JobId;

            //
            // For COMx port we need to validates dos device now since
            // we do not do it during OpenPort
            //
            if (IS_COM_PORT(pIniPort->pName))
            {
                if (!GetCOMPort(pIniPort))
                {
                    goto Fail;
                }
            }
            else if ( IS_FILE_PORT(pIniPort->pName) )
            {

                HANDLE hFile = INVALID_HANDLE_VALUE;

                if (pDocInfo1                 &&
                    pDocInfo1->pOutputFile    &&
                    pDocInfo1->pOutputFile[0])
                {

                    hFile = CreateFile( pDocInfo1->pOutputFile,
                                         GENERIC_WRITE,
                                         FILE_SHARE_READ | FILE_SHARE_WRITE,
                                         NULL,
                                         OPEN_ALWAYS,
                                         FILE_ATTRIBUTE_NORMAL | FILE_FLAG_SEQUENTIAL_SCAN,
                                         NULL );


                }
                else
                {
                    Error = ERROR_INVALID_PARAMETER;
                }

                if (hFile != INVALID_HANDLE_VALUE)
                {
                    SetEndOfFile(hFile);
                }
                pIniPort->hFile = hFile;
            }
            else if ( IS_IRDA_PORT(pIniPort->pName) )
            {
                if ( !IrdaStartDocPort(pIniPort) )
                {
                    goto Fail;
                }
            }
            else if ( !(pIniPort->Status & PP_DOSDEVPORT) )
            {

                pAdjustedName = AdjustFileName(pIniPort->pName);

                if (pAdjustedName)
                {
                    //
                    // For non dosdevices CreateFile on the name of the port
                    //
                    pIniPort->hFile = CreateFile(pAdjustedName,
                                                 GENERIC_WRITE,
                                                 FILE_SHARE_READ,
                                                 NULL,
                                                 OPEN_ALWAYS,
                                                 FILE_ATTRIBUTE_NORMAL  |
                                                 FILE_FLAG_SEQUENTIAL_SCAN,
                                                 NULL);

                    if ( pIniPort->hFile != INVALID_HANDLE_VALUE )
                    {
                        SetEndOfFile(pIniPort->hFile);
                    }
                    FreeSplMem(pAdjustedName);

                }
            }
            else
            {
                if ( !FixupDosDeviceDefinition(pIniPort) )
                {
                    goto Fail;
                }
            }

            //
            // For LPTX ports we reset port timeouts since they may have changed.
            //
            if ( IS_LPT_PORT (pIniPort->pName) && (pIniPort->Status & PP_COMM_PORT) )
            {
                COMMTIMEOUTS    cto     = {0};
                HANDLE          hToken  = NULL;

                if ( GetCommTimeouts(pIniPort->hFile, &cto) )
                {
                    hToken = RevertToPrinterSelf();

                    if (hToken)
                    {
                        cto.WriteTotalTimeoutConstant = GetProfileInt(szWindows,
                                                                      szINIKey_TransmissionRetryTimeout,
                                                                      45 );

                        cto.WriteTotalTimeoutConstant*=1000;

                        SetCommTimeouts(pIniPort->hFile, &cto);

                        ImpersonatePrinterClient(hToken);
                    }
                }
                else
                {

                }
            }
        }
    } // end of if (pIniPort->pPrinterName)

    if (pIniPort->hFile == INVALID_HANDLE_VALUE)
    {
        goto Fail;
    }
    return TRUE;


Fail:
    SPLASSERT(pIniPort->hFile == INVALID_HANDLE_VALUE);

    LcmEnterSplSem();
    pIniPort->Status &= ~PP_STARTDOC;
    LcmLeaveSplSem();

    if (pIniPort->hPrinter)
    {
        ClosePrinter(pIniPort->hPrinter);
    }

    if (pIniPort->pPrinterName)
    {
        FreeSplStr(pIniPort->pPrinterName);
    }

    if (Error)
    {
        SetLastError(Error);
    }
    return FALSE;
}

_Success_(return != FALSE)
BOOL
LcmWritePort(
            _In_                    HANDLE  hPort,
            _In_reads_bytes_(cbBuf) LPBYTE  pBuffer,
                                    DWORD   cbBuf,
            _Out_                   LPDWORD pcbWritten)
{
    PLCMINIPORT pIniPort    = (PLCMINIPORT)hPort;
    BOOL        rc          = TRUE;


    if (!pcbWritten)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbWritten = 0;

    if ( IS_IRDA_PORT(pIniPort->pName) )
    {
        rc = IrdaWritePort(pIniPort, pBuffer, cbBuf, pcbWritten);
    }
    else if (IS_COM_PORT(pIniPort->pName))
    {
        if (GetCOMPort(pIniPort))
        {
            rc = WriteFile(pIniPort->hFile, pBuffer, cbBuf, pcbWritten, NULL);
            if ( rc && *pcbWritten == 0 )
            {

                SetLastError(ERROR_TIMEOUT);
                rc = FALSE;
            }

            ReleaseCOMPort(pIniPort);
        }
        else
        {
            *pcbWritten = 0;
        }
    }
    else if ( !pIniPort->hFile || pIniPort->hFile == INVALID_HANDLE_VALUE )
    {
        SetLastError(ERROR_INVALID_HANDLE);
        return FALSE;
    }
    else
    {
        rc = WriteFile(pIniPort->hFile, pBuffer, cbBuf, pcbWritten, NULL);
        if ( rc && *pcbWritten == 0 )
        {

            SetLastError(ERROR_TIMEOUT);
            rc = FALSE;
        }
    }


    return rc;
}

_Success_(return != FALSE)
BOOL
LcmReadPort(
    _In_                HANDLE      hPort,
    _Out_writes_bytes_(cbBuf)
                        LPBYTE      pBuffer,
                        DWORD       cbBuf,
    _Out_               LPDWORD     pcbRead)
{
    PLCMINIPORT pIniPort    = (PLCMINIPORT)hPort;
    BOOL        rc          = FALSE;


    if (!pcbRead)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *pcbRead = 0;

    // since COM ports can be read outside of StartDoc/EndDoc
    // additional work needs to be done.
    if (IS_COM_PORT(pIniPort->pName))
    {
        if (GetCOMPort(pIniPort))
        {
            rc = ReadFile(pIniPort->hFile, pBuffer, cbBuf, pcbRead, NULL);


            ReleaseCOMPort(pIniPort);
        }
    }
    else if ( pIniPort->hFile &&
         (pIniPort->hFile != INVALID_HANDLE_VALUE) &&
         pIniPort->Status & PP_COMM_PORT )
    {
        rc = ReadFile(pIniPort->hFile, pBuffer, cbBuf, pcbRead, NULL);

    }
    else
    {
        SetLastError(ERROR_INVALID_HANDLE);
    }
    return rc;
}

BOOL
LcmEndDocPort(
    _In_    HANDLE   hPort
    )
{
    PLCMINIPORT    pIniPort = (PLCMINIPORT)hPort;


    if (!(pIniPort->Status & PP_STARTDOC))
    {
        return TRUE;
    }

    // The flush here is done to make sure any cached IO's get written
    // before the handle is closed.   This is particularly a problem
    // for Intelligent buffered serial devices

    FlushFileBuffers(pIniPort->hFile);

    //
    // For any ports other than real LPT ports we open during StartDocPort
    // and close it during EndDocPort
    //
    if ( !(pIniPort->Status & PP_COMM_PORT) || IS_COM_PORT(pIniPort->pName) )
    {
        if ( IS_IRDA_PORT(pIniPort->pName) )
        {
            IrdaEndDocPort(pIniPort);
        }
        else if (IS_COM_PORT(pIniPort->pName))
        {
            ReleaseCOMPort(pIniPort);
        }
        else
        {
            CloseHandle(pIniPort->hFile);
            pIniPort->hFile = INVALID_HANDLE_VALUE;

            if ( pIniPort->Status & PP_DOSDEVPORT )
            {
                (VOID)RemoveDosDeviceDefinition(pIniPort);
            }
        }
    }

    SetJob(pIniPort->hPrinter, pIniPort->JobId, 0, NULL, JOB_CONTROL_SENT_TO_PRINTER);

    ClosePrinter(pIniPort->hPrinter);

    FreeSplStr(pIniPort->pPrinterName);

    //
    // Startdoc no longer active.
    //
    pIniPort->Status &= ~PP_STARTDOC;

    return TRUE;
}

BOOL
LcmClosePort(
    _In_    HANDLE  hPort
    )
{
    PLCMINIPORT pIniPort = (PLCMINIPORT)hPort;


    FreeSplStr(pIniPort->pDeviceName);
    pIniPort->pDeviceName = NULL;

    if (pIniPort->Status & PP_FILEPORT) {

        LcmEnterSplSem();
        DeletePortNode(pIniPort->pIniLocalMon, pIniPort);
        LcmLeaveSplSem();
    } else if ( pIniPort->Status & PP_COMM_PORT ) {

        (VOID) RemoveDosDeviceDefinition(pIniPort);
        if ( pIniPort->hFile != INVALID_HANDLE_VALUE ) {

            // no COM ports should hit this path
            SPLASSERT(!IS_COM_PORT(pIniPort->pName));


            CloseHandle(pIniPort->hFile);
            pIniPort->hFile = INVALID_HANDLE_VALUE;
        }
        pIniPort->Status &= ~(PP_COMM_PORT | PP_DOSDEVPORT);
    }

    return TRUE;
}


BOOL
LcmAddPortEx(
    _In_              HANDLE   hMonitor,
    _In_opt_          LPWSTR   pName,
    _In_range_(-1, 1) DWORD    Level,
    _When_(Level < 1, _In_reads_bytes_(sizeof(PORT_INFO_FF)))
    _When_(Level == 1, _In_reads_bytes_(sizeof(PORT_INFO_1)))
                      LPBYTE   pBuffer,
    _In_opt_          LPWSTR   pMonitorName
)
{
    PINILOCALMON    pIniLocalMon    = (PINILOCALMON)hMonitor;
    LPWSTR          pPortName       = NULL;
    DWORD           Error           = NO_ERROR;
    LPPORT_INFO_1   pPortInfo1      = NULL;
    LPPORT_INFO_FF  pPortInfoFF     = NULL;

    UNREFERENCED_PARAMETER(pMonitorName);


    switch (Level) {
    case (DWORD)-1:
        pPortInfoFF = (LPPORT_INFO_FF)pBuffer;
        pPortName = pPortInfoFF->pName;
        break;

    case 1:
        pPortInfo1 =  (LPPORT_INFO_1)pBuffer;
        pPortName = pPortInfo1->pName;
        break;

    default:
        SetLastError(ERROR_INVALID_LEVEL);
        return(FALSE);
    }
    if (!pPortName) {
        SetLastError(ERROR_INVALID_PARAMETER);
        return(FALSE);
    }
    if (PortExists(pName, pPortName, &Error)) {
        SetLastError(ERROR_INVALID_PARAMETER);
        return(FALSE);
    }
    if (Error != NO_ERROR) {
        SetLastError(Error);
        return(FALSE);
    }
    if (!LcmCreatePortEntry(pIniLocalMon, pPortName)) {
        return(FALSE);
    }

    if (!AddPortInRegistry (pPortName)) {
        LcmDeletePortEntry( pIniLocalMon, pPortName );
        return(FALSE);
    }
    return TRUE;
}

_Success_(return != FALSE)
BOOL
LcmGetPrinterDataFromPort(
    _In_                            HANDLE  hPort,
                                    DWORD   ControlID,
    _In_opt_                        LPWSTR  pValueName,
    _In_reads_bytes_opt_(cbInBuffer)
                                    LPWSTR  lpInBuffer,
                                    DWORD   cbInBuffer,
    _Out_writes_bytes_opt_(cbOutBuffer)
                                    LPWSTR  lpOutBuffer,
                                    DWORD   cbOutBuffer,
    _Out_                           LPDWORD lpcbReturned)
{
    PLCMINIPORT  pIniPort  = (PLCMINIPORT)hPort;
    BOOL         rc        = FALSE;

    UNREFERENCED_PARAMETER(pValueName);

    if (!lpcbReturned)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *lpcbReturned = 0;


    if (ControlID &&
        (pIniPort->Status & PP_DOSDEVPORT) &&
        IS_COM_PORT(pIniPort->pName))
    {
        if (GetCOMPort(pIniPort))
        {
            rc = DeviceIoControl(pIniPort->hFile,
                                ControlID,
                                lpInBuffer,
                                cbInBuffer,
                                lpOutBuffer,
                                cbOutBuffer,
                                lpcbReturned,
                                NULL);

            ReleaseCOMPort(pIniPort);
        }
    }
    else if (ControlID &&
             pIniPort->hFile &&
             (pIniPort->hFile != INVALID_HANDLE_VALUE) &&
             (pIniPort->Status & PP_DOSDEVPORT))
    {
        rc = DeviceIoControl(pIniPort->hFile,
                            ControlID,
                            lpInBuffer,
                            cbInBuffer,
                            lpOutBuffer,
                            cbOutBuffer,
                            lpcbReturned,
                            NULL);
    }
    else
    {
        SetLastError(ERROR_INVALID_PARAMETER);
    }

    return rc;
}

BOOL
LcmSetPortTimeOuts(
    _In_        HANDLE          hPort,
    _In_        LPCOMMTIMEOUTS  lpCTO,
    _Reserved_  DWORD           reserved)    // must be set to 0
{
    PLCMINIPORT     pIniPort    = (PLCMINIPORT)hPort;
    COMMTIMEOUTS    cto         = {0};
    BOOL            rc          = FALSE;


    if (reserved != 0)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        goto done;
    }

    if ( !(pIniPort->Status & PP_DOSDEVPORT) )
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        goto done;
    }

    if (IS_COM_PORT(pIniPort->pName))
    {
        GetCOMPort(pIniPort);
    }

    if ( GetCommTimeouts(pIniPort->hFile, &cto) )
    {
        cto.ReadTotalTimeoutConstant = lpCTO->ReadTotalTimeoutConstant;
        cto.ReadIntervalTimeout = lpCTO->ReadIntervalTimeout;
        rc = SetCommTimeouts(pIniPort->hFile, &cto);
    }

    if (IS_COM_PORT(pIniPort->pName))
    {
        ReleaseCOMPort(pIniPort);
    }

done:
    return rc;
}

VOID
LcmShutdown(
    _In_    HANDLE hMonitor
    )
{
    PLCMINIPORT     pIniPort        = NULL;
    PLCMINIPORT     pIniPortNext    = NULL;
    PINILOCALMON    pIniLocalMon    = (PINILOCALMON)hMonitor;


    //
    // Delete the ports, then delete the LOCALMONITOR.
    //
    for( pIniPort = pIniLocalMon->pIniPort; pIniPort; pIniPort = pIniPortNext ){
        pIniPortNext = pIniPort->pNext;
        FreeSplMem( pIniPort );
    }

    FreeSplMem( pIniLocalMon );
}

_Success_(return == NO_ERROR)
DWORD
GetPortStrings(
    _Outptr_result_maybenull_ PWSTR *ppPorts
    )
{
    DWORD   sRetval  = ERROR_INVALID_PARAMETER;


    if (ppPorts)
    {
        DWORD dwcValues = 0;
        DWORD dwMaxValueName = 0;
        HKEY  hk = NULL;

        *ppPorts = NULL;

        //
        // open ports key
        //
        sRetval = g_pMonitorInit->pMonitorReg->fpCreateKey(g_pMonitorInit->hckRegistryRoot,
                                                           g_szPortsKey,
                                                           REG_OPTION_NON_VOLATILE,
                                                           KEY_READ,
                                                           NULL,
                                                           &hk,
                                                           NULL,
                                                           g_pMonitorInit->hSpooler);
        if (sRetval == ERROR_SUCCESS && hk)
        {
            sRetval = g_pMonitorInit->pMonitorReg->fpQueryInfoKey(hk,
                                                                  NULL,
                                                                  NULL,
                                                                  &dwcValues,
                                                                  &dwMaxValueName,
                                                                  NULL,
                                                                  NULL,
                                                                  NULL,
                                                                  g_pMonitorInit->hSpooler);
            if ((sRetval == ERROR_SUCCESS) && (dwcValues > 0))
            {
                PWSTR pPorts = NULL;
                DWORD cbMaxMemNeeded = ((dwcValues * (dwMaxValueName + 1) + 1) * sizeof(WCHAR));

                pPorts = (LPWSTR)AllocSplMem(cbMaxMemNeeded);

                if (pPorts)
                {
                    DWORD sTempRetval = ERROR_SUCCESS;
                    DWORD CharsAvail = cbMaxMemNeeded/sizeof(WCHAR);
                    INT   cIndex = 0;
                    PWSTR pPort = NULL;
                    DWORD dwCurLen = 0;

                    for (pPort = pPorts; sTempRetval == ERROR_SUCCESS; cIndex++)
                    {
                        dwCurLen = CharsAvail;
                        #pragma prefast(suppress:__WARNING_INVALID_PARAM_VALUE_1, "Seventh argument set to NULL on purpose")
                        sTempRetval = g_pMonitorInit->pMonitorReg->fpEnumValue(hk,
                                                                               cIndex,
                                                                               pPort,
                                                                               &dwCurLen,
                                                                               NULL,
                                                                               NULL,
                                                                               NULL,
                                                                               g_pMonitorInit->hSpooler);
                        // based on the results of current length,
                        // move pointers/counters for the next iteration.
                        if (sTempRetval == ERROR_SUCCESS)
                        {
                            // RegEnumValue only returns the char count.
                            // Add 1 for NULL.
                            dwCurLen++;

                            // decrement the count of available chars.
                            CharsAvail -= dwCurLen;

                            // prepare pPort for next string.
                            pPort += dwCurLen;
                        }

                    }

                    if (sTempRetval == ERROR_NO_MORE_ITEMS)
                    {
                        *pPort = L'\0';
                        *ppPorts = pPorts;
                    }
                    else
                    {
                        // set return value in error case.
                        sRetval = sTempRetval;
                    }
                }
                else
                {
                    sRetval = GetLastError();
                }
            }

            // close Reg key.
            g_pMonitorInit->pMonitorReg->fpCloseKey(hk,
                                                    g_pMonitorInit->hSpooler);
        }
    }

    return sRetval;
}


MONITOR2 Monitor2 = {
    sizeof(MONITOR2),
    LcmEnumPorts,
    LcmOpenPort,
    NULL,           // OpenPortEx is not supported
    LcmStartDocPort,
    LcmWritePort,
    LcmReadPort,
    LcmEndDocPort,
    LcmClosePort,
    NULL,           // AddPort is not supported
    LcmAddPortEx,
    NULL,           // ConfigurePort is not supported
    NULL,           // DeletePort is not supported
    LcmGetPrinterDataFromPort,
    LcmSetPortTimeOuts,
    LcmXcvOpenPort,
    LcmXcvDataPort,
    LcmXcvClosePort,
    LcmShutdown
};

LPMONITOR2
InitializePrintMonitor2(
    _In_    PMONITORINIT pMonitorInit,
    _Out_   PHANDLE phMonitor
    )
{
    LPWSTR   pPortTmp = NULL;
    DWORD    rc = 0, i = 0;
    PINILOCALMON pIniLocalMon = NULL;
    LPWSTR   pPorts = NULL;
    DWORD    sRetval = ERROR_SUCCESS;


    *phMonitor = NULL;

    // cache pointer
    pIniLocalMon = (PINILOCALMON)AllocSplMem( sizeof( INILOCALMON ));
    if( !pIniLocalMon )
    {
        goto Fail;
    }

    pIniLocalMon->signature = ILM_SIGNATURE;
    pIniLocalMon->pMonitorInit = pMonitorInit;
    g_pMonitorInit = pMonitorInit;

    // get ports
    sRetval = GetPortStrings(&pPorts);
    if (sRetval != ERROR_SUCCESS)
    {
        SetLastError(sRetval);
        goto Fail;
    }


    LcmEnterSplSem();

    //
    // We now have all the ports
    //
    for(pPortTmp = pPorts; pPortTmp && *pPortTmp; pPortTmp += rc + 1){

        rc = (DWORD)wcslen(pPortTmp);

        if (!_wcsnicmp(pPortTmp, L"Ne", 2)) {

            i = 2;

            //
            // For Ne- ports
            //
            if ( rc > 2 && pPortTmp[2] == L'-' )
                ++i;
            for ( ; i < rc - 1 && iswdigit(pPortTmp[i]) ; ++i )
            ;

            if ( i == rc - 1 && pPortTmp[rc-1] == L':' ) {
                continue;
            }
        }

        LcmCreatePortEntry(pIniLocalMon, pPortTmp);
    }

    FreeSplMem(pPorts);

    LcmLeaveSplSem();

    CheckAndAddIrdaPort(pIniLocalMon);

    *phMonitor = (HANDLE)pIniLocalMon;


    return &Monitor2;

Fail:

    FreeSplMem( pPorts );
    FreeSplMem( pIniLocalMon );

    return NULL;
}



BOOL
DllMain(
    HINSTANCE hModule,
    DWORD dwReason,
    LPVOID lpRes)
{
    static BOOL bLocalMonInit = FALSE;

    switch (dwReason)
    {
    case DLL_PROCESS_ATTACH:

        bLocalMonInit = LocalMonInit(hModule);
        DisableThreadLibraryCalls(hModule);
        return TRUE;

    case DLL_PROCESS_DETACH:

        if (bLocalMonInit)
        {
            LocalMonCleanUp();
            bLocalMonInit = FALSE;
        }

        return TRUE;
    }

    UNREFERENCED_PARAMETER(lpRes);

    return TRUE;
}

