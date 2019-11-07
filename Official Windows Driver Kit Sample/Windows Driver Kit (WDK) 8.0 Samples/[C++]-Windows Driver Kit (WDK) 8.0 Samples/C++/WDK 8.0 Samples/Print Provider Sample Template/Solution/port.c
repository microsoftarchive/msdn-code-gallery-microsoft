//
//  Windows (Printing) Driver Development Kit Samples.
//
//  Sample Print Provider template.
//
//  Port.c - Various port utility functions.
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

PPORT pFirstPort = NULL;


//
// Determines if the specified name matches the name of the local machine in the
// following format:
//     "\\LocalMachineName"
//
// Returns TRUE if it does.
// Returns FALSE otherwise.
//
BOOL
IsLocalMachine
(
    _In_ LPWSTR pszName
)
{
    if (!pszName || !*pszName)
    {
        return TRUE;
    }

    if (*pszName == L'\\' && *(pszName+1) == L'\\')
    {
        if (!lstrcmpi(pszName, szMachineName))
        {
            return TRUE;
        }
    }

    return FALSE;
}


//
// Determines if the specified port name has already been added.
//
// Returns TRUE if the specified port name has already been added. pError will
// be set to NO_ERROR.
//
// Returns FALSE if the specified port name has not already been added.  pError
// will indicate any errors encountered.
//
BOOL
PortExists
(
    _In_  LPWSTR  pPortName,
    _Out_ LPDWORD pError
)

{
    DWORD cbNeeded = 0;
    DWORD cReturned = 0;
    DWORD cbPorts = 0;
    LPPORT_INFO_1W pPorts = NULL;
    DWORD i = 0;
    BOOL Found = FALSE;
    WCHAR szBuff[MAX_PATH] = {0};
    DWORD dwLen = 0;

    *pError = NO_ERROR;

    if (!hSpoolssDll)
    {
        dwLen = GetSystemDirectory (szBuff, MAX_PATH);

        if (dwLen && dwLen < MAX_PATH)
        {
            *pError = StatusFromHResult(StringCchCat(szBuff,
                                                     MAX_PATH,
                                                     L"\\SPOOLSS.DLL"));

            if (*pError == ERROR_SUCCESS)
            {
                hSpoolssDll = LoadLibrary(szBuff);
                if (hSpoolssDll)
                {
                    pfnSpoolssEnumPorts = GetProcAddress(hSpoolssDll,
                                                         "EnumPortsW");

                    if (!pfnSpoolssEnumPorts)
                    {
                        *pError = GetLastError();
                        FreeLibrary(hSpoolssDll);
                        hSpoolssDll = NULL;
                    }
                }
                else
                {
                    *pError = GetLastError();
                }
            }
        }
        else
        {
            *pError = ERROR_INSUFFICIENT_BUFFER;
        }
    }

    if (!pfnSpoolssEnumPorts)
    {
        return FALSE;
    }

    if (!(*pfnSpoolssEnumPorts)(NULL, 1, NULL, 0, &cbNeeded, &cReturned))
    {
        if (GetLastError() == ERROR_INSUFFICIENT_BUFFER)
        {
            cbPorts = cbNeeded;

            EnterCriticalSection(&SplSem);

            pPorts = LocalAlloc(LMEM_ZEROINIT, cbPorts);
            if (pPorts)
            {
                if ((*pfnSpoolssEnumPorts)(NULL,
                                           1,
                                           (LPBYTE)pPorts,
                                           cbPorts,
                                           &cbNeeded,
                                           &cReturned))
                {
                    for (i = 0; i < cReturned; i++)
                    {
                        if (!lstrcmpi( pPorts[i].pName, pPortName))
                        {
                            Found = TRUE;
                            break;
                        }
                    }
                }
                else
                {
                    *pError = GetLastError();
                }
                LocalFree(pPorts);
            }
            else
            {
                *pError = ERROR_NOT_ENOUGH_MEMORY;
            }
            LeaveCriticalSection(&SplSem);
        }
        else
        {
            *pError = GetLastError();
        }
    }
    return Found;
}


//
// Determines if the specified port name already exists in the internal port
// list.
//
// Returns TRUE if the specified port name exists in the list.
// Returns FALSE if the specified port name doens't exist in the list.
//
BOOL
PortKnown
(
    _In_ LPWSTR   pPortName
)
{
    PPORT pPort;

    EnterCriticalSection(&SplSem);

    pPort = pFirstPort;

    while (pPort)
    {
        if (!lstrcmpi( pPort->pName, pPortName))
        {
            LeaveCriticalSection(&SplSem);
            return TRUE;
        }

        pPort = pPort->pNext;
    }
    LeaveCriticalSection(&SplSem);
    return FALSE;
}


//
// Build the internal list of port names.
//
DWORD
InitializePortNames
(
    VOID
)

{
    DWORD err;
    HKEY  hkeyPath;
    HKEY  hkeyPortNames;

    err = RegOpenKeyEx( HKEY_LOCAL_MACHINE,
                        pszRegistryPath,
                        0,
                        KEY_READ,
                        &hkeyPath );

    if (!err)
    {
        err = RegOpenKeyEx( hkeyPath,
                            pszRegistryPortNames,
                            0,
                            KEY_READ,
                            &hkeyPortNames );

        if (!err)
        {
            DWORD i = 0;
            WCHAR Buffer[MAX_PATH];
            DWORD BufferSize;

            while (!err)
            {
                BufferSize = MAX_PATH;

                err = RegEnumValue( hkeyPortNames,
                                    i,
                                    Buffer,
                                    &BufferSize,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL );

                if (!err)
                {
                    CreatePortEntry(Buffer);
                }

                i++;
            }

            if (ERROR_NO_MORE_ITEMS == err)
            {
                err = NO_ERROR;
            }

            RegCloseKey(hkeyPortNames);
        }

        RegCloseKey(hkeyPath);
    }
    return err;
}



//
// Creates a new port entry in the internal list of ports.
//
PPORT
CreatePortEntry
(
    _In_ LPWSTR   pPortName
)
{
    PPORT pPort, pPortTemp;

    DWORD cb = (DWORD)(sizeof(PORT) + (wcslen(pPortName) + 1) * sizeof(WCHAR));

    pPort = LocalAlloc(LMEM_ZEROINIT, cb);
    if (pPort)
    {
        DWORD Status;

        Status = StatusFromHResult(StringCbCopy((LPWSTR)(pPort+1),
                                                cb-sizeof(PORT),
                                                pPortName));

        if (Status == ERROR_SUCCESS)
        {
            pPort->pName = (LPWSTR)(pPort+1);
            pPort->cb = cb;
            pPort->pNext = NULL;

            EnterCriticalSection(&SplSem);

            pPortTemp = pFirstPort;
            if (pPortTemp)
            {
                while (pPortTemp->pNext)
                {
                    pPortTemp = pPortTemp->pNext;
                }
                pPortTemp->pNext = pPort;
            }
            else
            {
                pFirstPort = pPort;
            }

            LeaveCriticalSection(&SplSem);
        }
        else
        {
            LocalFree(pPort);

            pPort = NULL;

            SetLastError(Status);
        }
    }

    return pPort;
}


//
// Removes the port entry for the specified port name from the internal port
// list.
//
BOOL
DeletePortEntry
(
    _In_ LPWSTR   pPortName
)

{
    BOOL fRetVal;
    PPORT pPort = NULL, pPrevPort = NULL;

    EnterCriticalSection(&SplSem);

    pPort = pPrevPort = pFirstPort;
    while (pPort && lstrcmpi(pPort->pName, pPortName))
    {
        pPrevPort = pPort;
        pPort = pPort->pNext;
    }

    if (pPort)
    {
        if (pPort == pFirstPort)
        {
            pFirstPort = pPort->pNext;
        }
        else
        {
            pPrevPort->pNext = pPort->pNext;
        }
        LocalFree(pPort);
        fRetVal = TRUE;
    }
    else
    {
        fRetVal = FALSE;
    }
    LeaveCriticalSection(&SplSem);
    return fRetVal;
}


//
// Deletes all the port entries from the internal port list.
//
VOID
DeleteAllPortEntries
(
    VOID
)
{
    PPORT pPort, pNextPort;

    for (pPort = pFirstPort; pPort; pPort = pNextPort)
    {
        pNextPort = pPort->pNext;
        LocalFree(pPort);
    }
}


//
// Creates a new registry key for the specified port name.
//
DWORD
CreateRegistryEntry
(
    _In_ LPWSTR pPortName
)
{
    DWORD  err;
    HKEY   hkeyPath;
    HKEY   hkeyPortNames;

    err = RegCreateKeyEx( HKEY_LOCAL_MACHINE, pszRegistryPath, 0,
                          NULL, 0, KEY_WRITE, NULL, &hkeyPath, NULL );

    if (!err)
    {
        err = RegCreateKeyEx( hkeyPath, pszRegistryPortNames, 0,
                              NULL, 0, KEY_WRITE, NULL, &hkeyPortNames, NULL );

        if (!err)
        {
            err = RegSetValueEx( hkeyPortNames,
                                 pPortName,
                                 0,
                                 REG_SZ,
                                 (LPBYTE) L"",
                                 0 );

            RegCloseKey(hkeyPortNames);
        }
        RegCloseKey(hkeyPath);
    }
    return err;
}


//
// Deletes the registry key for the specified port name.
//
DWORD
DeleteRegistryEntry
(
    _In_ LPWSTR pPortName
)
{
    DWORD  err;
    HANDLE hToken;
    HKEY   hkeyPath;
    HKEY   hkeyPortNames;

    err = RegOpenKeyEx( HKEY_LOCAL_MACHINE, pszRegistryPath, 0,
                        KEY_WRITE, &hkeyPath
                      );

    if (!err)
    {

        err = RegOpenKeyEx( hkeyPath, pszRegistryPortNames, 0,
                            KEY_WRITE, &hkeyPortNames );

        if (!err)
        {
            err = RegDeleteValue(hkeyPortNames, pPortName);
            RegCloseKey(hkeyPortNames);
        }
        RegCloseKey(hkeyPath);

    }

    return err;
    UNREFERENCED_PARAMETER(hToken);
}

