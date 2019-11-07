/*++

Copyright (c) 1990-2003 Microsoft Corporation
All Rights Reserved

Module Name:

    util.c

--*/

#include "precomp.h"


#pragma hdrstop

#include <DriverSpecs.h>
_Analysis_mode_(_Analysis_code_type_user_driver_)


extern PMONITORINIT g_pMonitorInit;
extern _Null_terminated_ WCHAR g_szPortsList[];
extern _Null_terminated_ WCHAR g_szPortsKey[];


VOID
LcmRemoveColon(
    _Inout_ LPWSTR  pName
    )
{
    DWORD   Length = 0;


    Length = (DWORD)wcslen(pName);

    if (pName[Length-1] == L':')
        pName[Length-1] = 0;
}


BOOL
IsCOMPort(
    _In_    LPWSTR pPort
)
{

    //
    // Must begin with szLcmCOM
    //
    if ( _wcsnicmp( pPort, szLcmCOM, 3 ) )
    {
        return FALSE;
    }

    //
    // wcslen guarenteed >= 3
    //
    return pPort[ wcslen( pPort ) - 1 ] == L':';
}

BOOL
IsLPTPort(
    _In_    LPWSTR pPort
)
{

    //
    // Must begin with szLcmLPT
    //
    if ( _wcsnicmp( pPort, szLcmLPT, 3 ) )
    {
        return FALSE;
    }

    //
    // wcslen guarenteed >= 3
    //
    return pPort[ wcslen( pPort ) - 1 ] == L':';
}

BOOL
GetIniCommValues(
    _In_ LPWSTR          pName,
         LPDCB          pdcb,
         LPCOMMTIMEOUTS pcto
)
{
    BOOL    bRet        = TRUE;
    LPWSTR  pszEntry    = NULL;


    if (bRet)
    {
        bRet = GetIniCommValuesFromRegistry (pName, &pszEntry);
    }
    if (bRet)
    {
        bRet =  BuildCommDCB (pszEntry, pdcb);
    }
    if (bRet)
    {
        GetTransmissionRetryTimeoutFromRegistry (&pcto-> WriteTotalTimeoutConstant);
        pcto->WriteTotalTimeoutConstant*=1000;
    }
    FreeSplMem(pszEntry);
    return bRet;
}

/* PortExists
 *
 * Calls EnumPorts to check whether the port name already exists.
 * This asks every monitor, rather than just this one.
 * The function will return TRUE if the specified port is in the list.
 * If an error occurs, the return is FALSE and the variable pointed
 * to by pError contains the return from GetLastError().
 * The caller must therefore always check that *pError == NO_ERROR.
 */
BOOL
PortExists(
    _In_opt_ LPWSTR pName,
    _In_     LPWSTR pPortName,
    _Out_    PDWORD pError
)
{
    DWORD           cbNeeded    = 0;
    DWORD           cReturned   = 0;
    DWORD           cbPorts     = 0;
    LPPORT_INFO_1   pPorts      = NULL;
    DWORD           i           = 0;
    BOOL            Found       = TRUE;


    *pError = NO_ERROR;

    if (!EnumPortsW(pName, 1, NULL, 0, &cbNeeded, &cReturned))
    {
        if (GetLastError() == ERROR_INSUFFICIENT_BUFFER)
        {
            cbPorts = cbNeeded;

            pPorts = (LPPORT_INFO_1)AllocSplMem(cbPorts);

            if (pPorts)
            {
                if (EnumPortsW(pName, 1, (LPBYTE)pPorts, cbPorts, &cbNeeded, &cReturned))
                {
                    Found = FALSE;

                    for (i = 0; i < cReturned; i++)
                    {
                        if (!lstrcmpi(pPorts[i].pName, pPortName))
                        {
                            Found = TRUE;
                            break;
                        }
                    }
                }
            }

            FreeSplMem(pPorts);
        }
    }
    else
    {
        Found = FALSE;
    }

    return Found;
}


VOID
LcmSplInSem(
   VOID
)
{

    if (LcmSpoolerSection.OwningThread != (HANDLE) UIntToPtr(GetCurrentThreadId())) {
    }
}

VOID
LcmSplOutSem(
   VOID
)
{

    if (LcmSpoolerSection.OwningThread == (HANDLE) UIntToPtr(GetCurrentThreadId())) {
    }
}

_Acquires_lock_(LcmSpoolerSection)
VOID
LcmEnterSplSem(
   VOID
)
{
    EnterCriticalSection(&LcmSpoolerSection);
}

_Releases_lock_(LcmSpoolerSection)
VOID
LcmLeaveSplSem(
   VOID
)
{
#if DBG
    LcmSplInSem();
#endif
    LeaveCriticalSection(&LcmSpoolerSection);
}

PINIENTRY
LcmFindIniKey(
   _In_ PINIENTRY pIniEntry,
   _In_ LPWSTR pName
)
{

   if (!pName)
      return NULL;

   LcmSplInSem();

   while (pIniEntry && lstrcmpi(pName, pIniEntry->pName))
      pIniEntry = pIniEntry->pNext;

   return pIniEntry;
}

LPBYTE
LcmPackStrings(
    _In_                            DWORD   dwElementsCount,
    _In_reads_(dwElementsCount)     LPCWSTR *pSource,
    _Out_writes_bytes_(pDest-pEnd)  LPBYTE  pDest,
    _In_reads_(dwElementsCount)     DWORD   *DestOffsets,
    _Inout_updates_(_Inexpressible_("Involves negative offsets."))               LPBYTE  pEnd
)
{

    DWORD dwCount = 0;

    for (dwCount = 0; dwCount < dwElementsCount; dwCount++)
    {
        if (*pSource)
        {
#pragma prefast(suppress:__WARNING_POTENTIAL_BUFFER_OVERFLOW_NULLTERMINATED, "Just getting the length of current string, index of psource is correctly limited")
            size_t cbString = wcslen(*pSource)*sizeof(WCHAR) + sizeof(WCHAR);
            pEnd-= cbString;
            (VOID) StringCbCopy ((LPWSTR) pEnd, cbString, *pSource);
            *(LPWSTR UNALIGNED *)(pDest+*DestOffsets)= (LPWSTR) pEnd;
        }
        else
        {
            *(LPWSTR UNALIGNED *)(pDest+*DestOffsets)=0;
        }
      pSource++;
      DestOffsets++;
   }

   return pEnd;
}

DWORD
WINAPIV
StrNCatBuffW(
    _Out_writes_(cchBuffer) PWSTR       pszBuffer,
    UINT        cchBuffer,
    ...
    )
/*++

Description:

    This routine concatenates a set of null terminated strings
    into the provided buffer.  The last argument must be a NULL
    to signify the end of the argument list.  This only called
        from LocalMon by functions that use WCHARS.

Arguments:

    pszBuffer  - pointer buffer where to place the concatenated
                 string.
    cchBuffer  - character count of the provided buffer including
                 the null terminator.
    ...        - variable number of string to concatenate.

Returns:

    ERROR_SUCCESS if new concatenated string is returned,
    or ERROR_XXX if an error occurred.

Notes:

    The caller must pass valid strings as arguments to this routine,
    if an integer or other parameter is passed the routine will either
    crash or fail abnormally.  Since this is an internal routine
    we are not in try except block for performance reasons.

--*/
{
    DWORD   dwRetval    = ERROR_INVALID_PARAMETER;
    PCWSTR  pszTemp     = NULL;
    PWSTR   pszDest     = NULL;
    va_list pArgs;


    //
    // Validate the pointer where to return the buffer.
    //
    if (pszBuffer && cchBuffer)
    {
        //
        // Assume success.
        //
        dwRetval = ERROR_SUCCESS;

        //
        // Get pointer to argument frame.
        //
        va_start(pArgs, cchBuffer);

        //
        // Get temp destination pointer.
        //
        pszDest = pszBuffer;

        //
        // Insure we have space for the null terminator.
        //
        cchBuffer--;

        //
        // Collect all the arguments.
        //
        for ( ; ; )
        {
            //
            // Get pointer to the next argument.
            //
            pszTemp = va_arg(pArgs, PCWSTR);

            if (!pszTemp)
            {
                break;
            }

            //
            // Copy the data into the destination buffer.
            //
            for ( ; cchBuffer; cchBuffer-- )
            {
                *pszDest = *pszTemp;
                if (!(*pszDest))
                {
                    break;
                }

                pszDest++, pszTemp++;
            }

            //
            // If were unable to write all the strings to the buffer,
            // set the error code and delete the incomplete copied strings.
            //
            if (!cchBuffer && pszTemp && *pszTemp)
            {
                dwRetval = ERROR_BUFFER_OVERFLOW;
                *pszBuffer = L'\0';
                break;
            }
        }

        //
        // Terminate the buffer always.
        //
        *pszDest = L'\0';

        va_end(pArgs);
    }

    //
    // Set the last error in case the caller forgets to.
    //
    if (dwRetval != ERROR_SUCCESS)
    {
        SetLastError(dwRetval);
    }

    return dwRetval;

}

/* PortIsValid
 *
 * Validate the port by attempting to create/open it.
 */
BOOL
PortIsValid(
    _In_    LPWSTR pPortName
)
{
    HANDLE hFile            = INVALID_HANDLE_VALUE;
    BOOL   Valid            = FALSE;
    LPWSTR pAdjustedName    = NULL;


    //
    // For COM and LPT ports, no verification
    //
    if ( IS_COM_PORT( pPortName ) ||
        IS_LPT_PORT( pPortName ) ||
        IS_FILE_PORT( pPortName ) )
    {
        return TRUE;
    }

    pAdjustedName = AdjustFileName( pPortName);

    if (pAdjustedName)
    {
        hFile = CreateFile(pAdjustedName,
                           GENERIC_WRITE,
                           FILE_SHARE_READ,
                           NULL,
                           OPEN_EXISTING,
                           FILE_ATTRIBUTE_NORMAL,
                           NULL);

        if (hFile == INVALID_HANDLE_VALUE) {
            hFile = CreateFile(pAdjustedName,
                               GENERIC_WRITE,
                               FILE_SHARE_READ,
                               NULL,
                               OPEN_ALWAYS,
                               FILE_ATTRIBUTE_NORMAL | FILE_FLAG_DELETE_ON_CLOSE,
                               NULL);
        }

        if (hFile != INVALID_HANDLE_VALUE) {
            CloseHandle(hFile);
            Valid = TRUE;
        } else {
            Valid = FALSE;
        }

        FreeSplMem(pAdjustedName);
    }
    else
    {
        SetLastError(ERROR_FILE_NOT_FOUND);
    }

    return Valid;
}

_Success_(return == TRUE)
BOOL
GetIniCommValuesFromRegistry (
    _In_     LPCWSTR  pszPortName,
    _Out_    LPWSTR*  ppszCommValues
    )
/*++

Description:
    This function returns the communication parameters from the Registry
    of the specified port in the format: "921600,8,n,1.5,x"

Arguments:
    pszPortName     - [in]  the name of the specific port
    ppszCommValues  - [out] address of the pointer that will point to the string
                            Note, if the function succeeds the caller is responsible for memory deletion by calling FreeSplMem.

Returns:
    Win32 error code
    Note, if not ERROR_SUCCESS the output parameter stays unchanged.

--*/
{
    LPWSTR pszCommValues    = NULL;
    DWORD  dwStatus         = ERROR_SUCCESS;
    DWORD  cbData           = 64,
           dwBlockSize      = 0;
    HKEY   hPorts           = NULL;


    if (!ppszCommValues)
    {
        SetLastError(ERROR_INVALID_PARAMETER);
        return FALSE;
    }

    *ppszCommValues = NULL;

    //
    // open the registry key
    //
    dwStatus = g_pMonitorInit->pMonitorReg->fpCreateKey(g_pMonitorInit->hckRegistryRoot,
                                                        g_szPortsKey,
                                                        REG_OPTION_NON_VOLATILE,
                                                        KEY_QUERY_VALUE,
                                                        NULL,
                                                        &hPorts,
                                                        NULL,
                                                        g_pMonitorInit->hSpooler);

    //
    // allocate the initial memory based on COM ports: "921600,8,n,1.5,x" with some extra space
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        pszCommValues = (PWSTR)AllocSplMem (cbData + sizeof(WCHAR));
        dwStatus = pszCommValues ? ERROR_SUCCESS : ERROR_OUTOFMEMORY;
    }

    //
    // get the string size
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        DWORD dwType = 0;
        dwBlockSize = cbData;
        pszCommValues[cbData / sizeof(WCHAR)] = UNICODE_NULL;

        dwStatus = g_pMonitorInit->pMonitorReg->fpQueryValue(hPorts,
                                                             pszPortName,
                                                             &dwType,
                                                             (BYTE*)pszCommValues,
                                                             &cbData,
                                                             g_pMonitorInit->hSpooler);
        //
        // check the result
        //
        if (dwStatus == ERROR_MORE_DATA)
        {
            dwStatus = ERROR_SUCCESS;
            //
            // release the buffer
            //
            FreeSplMem (pszCommValues);
            //
            // allocate precise amount of memory
            //
            if (dwStatus == ERROR_SUCCESS)
            {
                pszCommValues = (PWSTR)AllocSplMem (cbData + sizeof(WCHAR));
                dwStatus = pszCommValues ? ERROR_SUCCESS : ERROR_OUTOFMEMORY;
            }
            //
            // get data again
            //
            if (dwStatus == ERROR_SUCCESS)
            {
                dwBlockSize = cbData;
                pszCommValues[cbData / sizeof(WCHAR)] = UNICODE_NULL;
                dwStatus = g_pMonitorInit->pMonitorReg->fpQueryValue(hPorts,
                                                                     pszPortName,
                                                                     &dwType,
                                                                     (BYTE*)pszCommValues,
                                                                     &cbData,
                                                                     g_pMonitorInit->hSpooler);
                if (dwStatus == ERROR_SUCCESS && dwType != REG_SZ)
                {
                    //
                    // unexpected data type
                    //
                    dwStatus = ERROR_INVALID_DATA;
                }
            }
        }
        else if (dwStatus == ERROR_SUCCESS && dwType != REG_SZ)
        {
            //
            // unexpected data type
            //
            dwStatus = ERROR_INVALID_DATA;
        }
    }
    //
    // update output parameter
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        // Add a NULL terminator because RegQueryValueEx does not guarantee it
        DWORD dwNullIndex = (dwBlockSize/2)-1;
        pszCommValues[dwNullIndex] = 0x00;
        *ppszCommValues = pszCommValues;
        pszCommValues   = NULL;
    }
    //
    // cleanup
    //
    if (pszCommValues)
    {
        FreeSplMem (pszCommValues);
    }

    if (hPorts)
    {
        RegCloseKey (hPorts);
    }

    return dwStatus == ERROR_SUCCESS;
}

VOID
GetTransmissionRetryTimeoutFromRegistry (
    _Out_ DWORD* pdwTimeout
    )
/*++

Description:
    This function returns the timeout value stored in Registry at
    HKLM\Software\Microsoft\Windows NT\CurrentVersion\Windows:TransmissionRetryTimeout

Arguments:
    pdwTimeout  - [out] pointer to DWORD that will receive the timeout value

Returns:
    Win32 error code;
    Note, if not ERROR_SUCCESS the output parameter stays unchanged.

--*/
{
    extern _Null_terminated_ WCHAR gszWindows[];

    DWORD  dwStatus     = ERROR_SUCCESS;
    LPWSTR pszTimeout   = NULL;
    DWORD  dwTimeout    = 0;
    DWORD  cbData       = 10;
    DWORD  dwBlockSize  = 0;
    HKEY   hWindows     = NULL;


    _Analysis_assume_nullterminated_(gszWindows);

    //
    // open the registry key
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        dwStatus = RegOpenKeyEx (HKEY_LOCAL_MACHINE, gszWindows, 0, KEY_QUERY_VALUE, &hWindows);
    }
    //
    // allocate the initial memory for timeout string
    // this is a timeout period in seconds with initial value of 90
    // it shouldn't be larger then four digits (four WCHARs)
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        pszTimeout = (PWSTR)AllocSplMem (cbData + sizeof(WCHAR));
        dwStatus = pszTimeout ? ERROR_SUCCESS : ERROR_OUTOFMEMORY;
    }
    //
    // get data
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        DWORD dwType = 0;
        dwBlockSize = cbData;
        pszTimeout[cbData / sizeof(WCHAR)] = UNICODE_NULL;
        dwStatus = RegQueryValueEx (hWindows, szINIKey_TransmissionRetryTimeout, 0, &dwType, (BYTE*)pszTimeout, &cbData);
        //
        // check the result
        //
        if (dwStatus == ERROR_MORE_DATA)
        {
            dwStatus = ERROR_SUCCESS;
            //
            // release the buffer
            //
            FreeSplMem (pszTimeout);
            //
            // allocate precise amount of memory
            //
            if (dwStatus == ERROR_SUCCESS)
            {
                pszTimeout = (PWSTR)AllocSplMem (cbData + sizeof(WCHAR));
                dwStatus = pszTimeout ? ERROR_SUCCESS : ERROR_OUTOFMEMORY;
            }
            //
            // get data again
            //
            if (dwStatus == ERROR_SUCCESS)
            {
                dwBlockSize = cbData;
                pszTimeout[cbData / sizeof(WCHAR)] = UNICODE_NULL;
                dwStatus = RegQueryValueEx (hWindows, szINIKey_TransmissionRetryTimeout, 0, &dwType, (BYTE*)pszTimeout, &cbData);
                if (dwStatus == ERROR_SUCCESS && dwType != REG_SZ)
                {
                    //
                    // unexpected data type
                    //
                    dwStatus = ERROR_INVALID_DATA;
                }
            }
        }
        else if (dwStatus == ERROR_SUCCESS && dwType != REG_SZ)
        {
            //
            // unexpected data type
            //
            dwStatus = ERROR_INVALID_DATA;
        }
    }
    //
    // retrieve timeout value from string
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        // Add a NULL terminator because RegQueryValueEx does not guarantee it
        DWORD dwNullIndex = (dwBlockSize/2)-1;
        pszTimeout[dwNullIndex] = 0x00;
        dwStatus = (swscanf_s (pszTimeout, L"%lu", &dwTimeout) == 1)? ERROR_SUCCESS : ERROR_INVALID_DATA;
    }
    //
    // update output parameter
    //
    *pdwTimeout = (dwStatus == ERROR_SUCCESS)? dwTimeout : 45;
    //
    // cleanup
    //
    if (pszTimeout)
    {
        FreeSplMem (pszTimeout);
    }
    if (hWindows)
    {
        RegCloseKey (hWindows);
    }
}

DWORD
SetTransmissionRetryTimeoutInRegistry (
    _In_ LPCWSTR       pszTimeout
    )
/*++

Description:
    This function returns sets the timeout value in the Registry at
    HKLM\Software\Microsoft\Windows NT\CurrentVersion\Windows:TransmissionRetryTimeout

Arguments:
    pdwTimeout  - [in] new timeout value

Returns:
    Win32 error code

--*/
{
    extern _Null_terminated_ WCHAR gszWindows[];

    DWORD dwStatus = ERROR_SUCCESS;

    HANDLE hToken = NULL;


    _Analysis_assume_nullterminated_(gszWindows);

    hToken = RevertToPrinterSelf();

    if (hToken)
    {
        //
        // open the registry key
        //
        HKEY hWindows = NULL;
        if (dwStatus == ERROR_SUCCESS)
        {
            dwStatus = RegOpenKeyEx (HKEY_LOCAL_MACHINE, gszWindows, 0, KEY_SET_VALUE, &hWindows);
        }
        //
        // get data
        //
        if (dwStatus == ERROR_SUCCESS)
        {
            DWORD cbData = (DWORD) ((wcslen (pszTimeout) + 1) * sizeof pszTimeout [0]);
            dwStatus = RegSetValueEx (hWindows, szINIKey_TransmissionRetryTimeout, 0, REG_SZ, (BYTE*)pszTimeout, cbData);
        }
        //
        // cleanup
        //
        if (hWindows)
        {
            RegCloseKey (hWindows);
        }

        ImpersonatePrinterClient(hToken);
    }
    else
    {
        dwStatus = GetLastError();
    }

    return dwStatus;
}
BOOL
AddPortInRegistry (
    _In_ LPCWSTR pszPortName
    )
/*++

Description:
    This function adds the port name in the Registry as
    Note, no communcation parameters will be added.

Arguments:
    pszPortName     - [in] port name

Returns:
    Win32 error code

--*/
{
    DWORD        dwStatus = ERROR_SUCCESS;
    HANDLE       hToken = NULL;
    HKEY         hPorts = NULL;


    if (pszPortName == NULL)
    {
        dwStatus = ERROR_INVALID_DATA;
        goto done;
    }

    // Note:  Port monitor writers need to think about what resources are being used and
    //        what level of security should be required in the creation of a port.
    hToken = RevertToPrinterSelf();

    //
    // open the key
    //
    dwStatus = g_pMonitorInit->pMonitorReg->fpCreateKey(g_pMonitorInit->hckRegistryRoot,
                                                        g_szPortsKey,
                                                        REG_OPTION_NON_VOLATILE,
                                                        KEY_SET_VALUE,
                                                        NULL,
                                                        &hPorts,
                                                        NULL,
                                                        g_pMonitorInit->hSpooler);
    //
    // add the new value
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        WCHAR szEmpty [] = L"";
        dwStatus = g_pMonitorInit->pMonitorReg->fpSetValue(hPorts,
                                                           pszPortName,
                                                           REG_SZ,
                                                           (BYTE*) szEmpty,
                                                           sizeof szEmpty,
                                                           g_pMonitorInit->hSpooler);
    }

done:
    //
    // cleanup
    //
    if (hPorts)
    {
        RegCloseKey (hPorts);
    }

    if (hToken)
        ImpersonatePrinterClient(hToken);
    return dwStatus == ERROR_SUCCESS;
}


VOID
DeletePortFromRegistry (
    _In_ LPCWSTR         pszPortName
    )
/*++

Description:
    This function deletes the port from the Registry. Basicaly, it deletes the Registry value

Arguments:
    pszPortName     - [in] port name

Returns:
    Win32 error code

--*/
{
    DWORD  dwStatus = ERROR_SUCCESS;
    HANDLE hToken   = NULL;
    HKEY   hPorts   = NULL;


    if (pszPortName == NULL)
    {
        dwStatus = ERROR_INVALID_DATA;
        goto done;
    }

    // Note:  Port monitor writers need to think about what resources are being used and
    //        what level of security should be required in the deletion of a port.
    hToken = RevertToPrinterSelf();

    //
    // open the key
    //
    dwStatus = g_pMonitorInit->pMonitorReg->fpCreateKey(g_pMonitorInit->hckRegistryRoot,
                                                        g_szPortsKey,
                                                        REG_OPTION_NON_VOLATILE,
                                                        KEY_SET_VALUE,
                                                        NULL,
                                                        &hPorts,
                                                        NULL,
                                                        g_pMonitorInit->hSpooler);

    //
    // remove the new value
    //
    if (dwStatus == ERROR_SUCCESS)
    {
        dwStatus = g_pMonitorInit->pMonitorReg->fpDeleteValue(hPorts,
                                                              pszPortName,
                                                              g_pMonitorInit->hSpooler);
    }

done:
    //
    // cleanup
    //
    if (hPorts)
    {
        RegCloseKey (hPorts);
    }

    if (hToken)
        ImpersonatePrinterClient(hToken);
}


LPWSTR
AdjustFileName(
    _In_ LPWSTR pFileName
    )
{
    LPWSTR pNewName         = NULL;

    pNewName = AllocSplStr( pFileName);

    return pNewName;
}

