/*++

Copyright (c) 1990-2003 Microsoft Corporation
All Rights Reserved

Module Name:

    util.c

--*/

#include "precomp.h"

CRITICAL_SECTION pjlMonSection      = {0,0,0,0,0,0};

PINIPORT pIniFirstPort  = NULL;

DWORD   dwReadThreadErrorTimeout;
__int64 nReadThreadEOJTimeout;
DWORD   dwReadThreadIdleTimeoutOther;

TCHAR cszEOJTimeout[]        = TEXT("EOJTimeout");


VOID
SplInSem(
   VOID
    )
{
    if ( pjlMonSection.OwningThread != (HANDLE) UIntToPtr(GetCurrentThreadId()) ) {
        DBG_MSG(DBG_ERROR, ("Pjlmon: Not in spooler semaphore !!\n"));
    }
}


VOID
SplOutSem(
   VOID
    )
{
    if ( pjlMonSection.OwningThread == (HANDLE) UIntToPtr(GetCurrentThreadId()) ) {
        DBG_MSG(DBG_ERROR, ("Pjlmon: Inside spooler semaphore !!\n"));
    }
}

_Acquires_lock_(pjlMonSection)
VOID
EnterSplSem(
   VOID
    )
{
    EnterCriticalSection(&pjlMonSection);
}

_Releases_lock_(pjlMonSection)
VOID
LeaveSplSem(
   VOID
    )
{
    SplInSem();
    LeaveCriticalSection(&pjlMonSection);
}


_Success_(return == NO_ERROR)
DWORD
UpdateRegistryValue(
              HKEY     hKey,
              LPCTSTR  cszValueName,
    _Out_     __int64 *pnValue,
              __int64  nDefault,
    _Inout_   LPDWORD  pdwLastError
    )
/*++

Routine Description:
    Gets value associated with given value name from the registry. If value name
    is not found default value is written to registry.

    Note this function usage is currently restricted to 64-bit integer values,
    recorded to the Registry as REG_BINARY.

    On error last error value is set to pdwLastError.

Arguments:
    hKey         : Registry key under which value should be searched
    cszValueName : Value name to search in the registry
    pdwValue     : On return will have the value
    dwDefault    : If value name not found in the registry set to this value
    pdwLastError : On error set last error to this

Return Value:
    None

--*/
{
    DWORD dwErr = NO_ERROR;
    DWORD dwSize = 0;

    if (( *pdwLastError != ERROR_SUCCESS ) || (!pnValue) )
        return ERROR_INVALID_PARAMETER;

    dwSize = sizeof(*pnValue);
    *pnValue = 0;

    if ( ERROR_SUCCESS != RegQueryValueEx(hKey,
                                          cszValueName,
                                          NULL,
                                          NULL,
                                          (LPBYTE)pnValue,
                                          &dwSize) ) {

        *pnValue = nDefault;
        *pdwLastError = dwErr = RegSetValueEx(hKey,
                                      cszValueName,
                                      0,
                                      REG_BINARY,
                                      (LPBYTE)pnValue,
                                      sizeof(*pnValue));
    }

    return dwErr;
}


DWORD
UpdateTimeoutsFromRegistry(
    _In_    LPTSTR      pszRegistryRoot
    )
/*++

Routine Description:
    Get the timeout values from the registry, or initialize registry with
    default values if entries are not found.

    Users/apps can change the registry to change the behavior.

Arguments:
    pszRegistryRoot : Registry root to be used by this dll

Return Value:
    ERROR_SUCCESS on success, else last error value

--*/
{
    HKEY    hKey;
    DWORD   dwLastError = ERROR_SUCCESS;

    dwLastError = RegCreateKeyEx(HKEY_LOCAL_MACHINE,
                                 pszRegistryRoot,
                                 0,
                                 NULL,
                                 0,
                                 KEY_READ | KEY_WRITE,
                                 NULL,
                                 &hKey,
                                 NULL);

    if ( dwLastError != ERROR_SUCCESS ) {

        goto Cleanup;
    }

    UpdateRegistryValue(hKey,
                        cszEOJTimeout,
                        &nReadThreadEOJTimeout,
                        READ_THREAD_EOJ_TIMEOUT,
                        &dwLastError);

    dwReadThreadErrorTimeout        = READ_THREAD_ERROR_WAITTIME;
    dwReadThreadIdleTimeoutOther    = READ_THREAD_IDLE_WAITTIME;

    RegCloseKey(hKey);

Cleanup:

    if ( dwLastError != ERROR_SUCCESS ) {

        DBG_MSG(DBG_ERROR,
               ("UpdateTimeoutsFromRegistry: Failed with %d", dwLastError));
    }

    return dwLastError;
}


PINIPORT
FindIniPort(
   _In_ LPTSTR pszName
    )
{
    PINIPORT    pIniPort = pIniFirstPort;

    if ( !pszName || !*pszName )
        return NULL;

    SplInSem();

    while ( pIniPort && lstrcmpi(pszName, pIniPort->pszPortName))
      pIniPort = pIniPort->pNext;

   return pIniPort;
}


PINIPORT
CreatePortEntry(
    _In_    LPTSTR  pszPortName
    )
/*++

Routine Description:
    Creates a IniPort entry for a port. Needs to be called inside monitor
    critical section.

Arguments:
    pszPortName       : Name of the port

Return Value:
      On success pointer to the IniPort stucture.
      On failure NULL

--*/
{
    PINIPORT    pIniPort    = NULL;
    HANDLE      DoneWriting = NULL;
    HANDLE      DoneReading = NULL;
    HANDLE      WakeUp      = NULL;
    LPWSTR      pszString   = NULL;

    SplInSem();


    DoneWriting = CreateEvent(NULL, FALSE, TRUE, NULL);
    if (!DoneWriting)
    {
        goto Fail;
    }

    WakeUp = CreateEvent(NULL, FALSE, FALSE, NULL);
    if (!WakeUp)
    {
        goto Fail;
    }
    //
    // manual-reset event, initially signal state
    //
    DoneReading = CreateEvent(NULL, TRUE, TRUE, NULL);
    if (!DoneReading)
    {
        goto Fail;
    }
    pIniPort = (PINIPORT) AllocSplMem(sizeof(*pIniPort));
    if (!pIniPort)
    {
        goto Fail;
    }
    pszString = AllocSplStr(pszPortName);
    if (!pszString)
    {
        goto Fail;
    }
    pIniPort->pNext         = NULL;
    pIniPort->signature     = PJ_SIGNATURE;
    pIniPort->DoneWriting   = DoneWriting;
    pIniPort->DoneReading   = DoneReading;
    pIniPort->WakeUp        = WakeUp;
    pIniPort->hUstatusThread= NULL;
    pIniPort->pszPortName   = pszString;

    pIniPort->pNext         = pIniFirstPort;
    pIniFirstPort           = pIniPort;
    return pIniPort;

Fail:
    if (DoneWriting)
    {
        CloseHandle (DoneWriting);
    }
    if (DoneReading)
    {
        CloseHandle (DoneReading);
    }
    if (WakeUp)
    {
        CloseHandle (WakeUp);
    }
    FreeSplMem (pszString);
    FreeSplMem (pIniPort);
    return NULL;
}


VOID
DeletePortEntry(
    _In_    PINIPORT pIniPort
    )
/*++

Routine Description:
    Deletes a port entry. Needs to be called inside monitor critical section

Arguments:
    pIniPort    : Pointer to the IniPort structure to be deleted

Return Value:

--*/
{
    SplInSem();

    if ( pIniPort == pIniFirstPort ) {

        pIniFirstPort = pIniPort->pNext;
    } else {

        PINIPORT    pPort;

        pPort = pIniFirstPort;
        while ( pPort && pPort->pNext != pIniPort )
            pPort = pPort->pNext;

        if (pPort) {

            pPort->pNext = pIniPort->pNext;
        } else {

            DBG_MSG(DBG_ERROR, ("pjlmon: DeletePortEntry port not found\n"));
            return;
        }
    }
    if (pIniPort-> DoneWriting)
    {
        CloseHandle(pIniPort->DoneWriting);
        pIniPort->DoneWriting = NULL;
    }
    if (pIniPort-> DoneReading)
    {
        CloseHandle(pIniPort->DoneReading);
        pIniPort->DoneReading = NULL;
    }
    if (pIniPort-> WakeUp)
    {
        CloseHandle(pIniPort->WakeUp);
        pIniPort->WakeUp = NULL;
    }
    if (pIniPort-> hUstatusThread)
    {
        CloseHandle (pIniPort-> hUstatusThread);
        pIniPort->hUstatusThread = NULL;
    }
    FreeIniJobs(pIniPort);
    FreeSplStr(pIniPort->pszPortName);
    pIniPort->pszPortName = NULL;
    FreeSplMem(pIniPort);

    return;
}


VOID
FreeIniJob(
    _Inout_ PINIJOB pIniJob
    )
/*++

Routine Description:
    Deletes a job entry.

Arguments:
    pIniJob    : Pointer to the IniJob structure to be deleted

Return Value:
    None

--*/
{
    SPLASSERT(pIniJob);
    if ( pIniJob->hPrinter )
        ClosePrinter(pIniJob->hPrinter);
    FreeSplMem(pIniJob);
}


VOID
FreeIniJobs(
    _Inout_ PINIPORT pIniPort
    )
/*++

Routine Description:
    Free all the InJob structures assigned to this port

Arguments:
    pIniPort    : IniPort for the port for which all jobs need to be freed

--*/
{
    PINIJOB pIniJob, pIniNextJob;

    EnterSplSem();
    pIniJob = pIniPort->pIniJob;
    while ( pIniJob ) {

        pIniNextJob = pIniJob->pNext;
        FreeIniJob(pIniJob);
        pIniJob = pIniNextJob;
    }

    pIniPort->pIniJob = NULL;
    LeaveSplSem();
}


VOID
SendLastPageEjectedForIniJob(
    _In_    PINIPORT    pIniPort,
    _In_    PINIJOB     pIniJob
    )
{
    UNREFERENCED_PARAMETER(pIniPort);

    SplInSem();

    if ( !SetJob(pIniJob->hPrinter, pIniJob->JobId, 0,
                 NULL, JOB_CONTROL_LAST_PAGE_EJECTED) ) {

        DBG_MSG(DBG_WARN,
               ("SetJob failed with last error %d\n", GetLastError()));
    }
}


PINIJOB
FindIniJobFromJobId(
    _In_            PINIPORT    pIniPort,
                    DWORD       dwJobId,
    _Outptr_result_maybenull_ PINIJOB    *ppPrevIniJob
    )
{
    PINIJOB pCur, pPre, pIniJob;

    SplInSem();

    //
    // If JOB_RESTART is given there will be multiple jobs with same id
    // we need to find the last entry with given id in the list
    //
    for ( pCur = pIniPort->pIniJob, pPre = pIniJob = *ppPrevIniJob = NULL ;
          pCur ;
          pPre = pCur, pCur = pCur->pNext ) {

        if ( pCur->JobId == dwJobId ) {

            *ppPrevIniJob   = pPre;
            pIniJob         = pCur;
        }
    }

    return pIniJob;
}


PINIJOB
FindFirstIniJobTimedOut(
    _In_            PINIPORT    pIniPort,
                    __int64     nTime,
    _Outptr_result_maybenull_ PINIJOB    *ppPrevIniJob
    )
{
    PINIJOB pIniJob = pIniPort->pIniJob;

    SplInSem();
    *ppPrevIniJob = NULL;

    //
    // Look for a job not in STARTDOC and timedout
    //
    while ( pIniJob &&
            ( (pIniJob->status & PP_INSTARTDOC) ||
               pIniJob->nTimeoutCount > nTime ) ) {

        *ppPrevIniJob = pIniJob;
        pIniJob = pIniJob->pNext;
    }

    if ( !pIniJob )
        *ppPrevIniJob = NULL;

    return pIniJob;
}


VOID
SendJobLastPageEjected(
    _In_    PINIPORT    pIniPort,
            __int64     nValue,
            BOOL        bTime
    )
/*++

Routine Description:
    Send LastPageEjected notification for 1 or more jobs to spooler

Arguments:
    pIniPort    : IniPort for the port for which all jobs need to be freed
    nValue      : if bTime is TRUE send EOJ to any jobs rcvd before nValue
                  else nValue is JobId -- ALL_JOBS is for all jobs
    bTime       : Tells how to interpret nValue (FALSE means it is a DWORD JobId)

--*/
{
    PINIJOB pIniJob;

    EnterSplSem();
    //
    // JobId == ALL_JOBS is a special case where we want to send LastPage
    // ejected for all jobs pending
    //
    if ( (!bTime) && (ALL_JOBS == nValue) ) {

        pIniJob = pIniPort->pIniJob;
        pIniPort->pIniJob = NULL;

        while ( pIniJob ) {

            PINIJOB pTempJob = pIniJob;

            SendLastPageEjectedForIniJob(pIniPort, pIniJob);
            pIniJob = pIniJob->pNext;
            FreeIniJob(pTempJob);
        }

    } else {

        PINIJOB pPrevIniJob = NULL;

        pIniJob = pIniPort->pIniJob;

        //
        // If bTime we want to send LastPageEjected for all jobs timedout
        //
        if ( bTime )  {

            pIniJob = FindFirstIniJobTimedOut(pIniPort, nValue, &pPrevIniJob);
        } else {

            pIniJob = FindIniJobFromJobId(pIniPort, (DWORD)nValue, &pPrevIniJob);
        }

        if ( pIniJob ) {

            //
            // Send notifications for any previous jobs too
            //
            if ( pIniPort->pIniJob == pIniJob )
                pIniPort->pIniJob = NULL;
            else if (pPrevIniJob)
                pPrevIniJob->pNext = NULL;

            do {

                SendLastPageEjectedForIniJob(pIniPort, pIniJob);

                pPrevIniJob = pIniJob;
                pIniJob = pIniJob->pNext;
                FreeIniJob(pPrevIniJob);
            } while ( pIniJob );
        }
    }

    LeaveSplSem();
}

// -----------------------------------------------------------------------
//
// String helper function to remove crt dependency
//
// -----------------------------------------------------------------------
LPSTR
mystrchr(
    _In_    LPSTR cs,
            char c
)
{
    while (*cs != 0)
    {
        if (IsDBCSLeadByte(*cs))
        {
            cs++;
            if (*cs == 0)
            {
                return NULL;
            }
        }
        else
        if (*cs == c)
            return cs;
        cs++;
    }

    // fail to find c in cs
    return NULL;
}


int
mystrncmp(
    _In_    LPSTR cs,
    _In_    LPSTR ct,
            int n
)
{
    char ret = 0;

    while (n--)
    {
        ret = *cs - *ct;

        if (ret)
            break;

        cs++;
        ct++;
    }

    return (int)ret;
}


LPWSTR
AllocSplStr(
    _In_    LPWSTR pStr
    )

/*++

Routine Description:

    This function will allocate enough local memory to store the specified
    string, and copy that string to the allocated memory

Arguments:

    pStr - Pointer to the string that needs to be allocated and stored

Return Value:

    NON-NULL - A pointer to the allocated memory containing the string

    FALSE/NULL - The operation failed. Extended error status is available
    using GetLastError.

--*/

{
    LPWSTR pMem;
    size_t cbStr = 0;

    if (!pStr) {
        return NULL;
    }

    cbStr = wcslen(pStr)*sizeof(WCHAR) + sizeof(WCHAR);

    pMem = AllocSplMem( cbStr );
    if (pMem) {
        CopyMemory( pMem, pStr, cbStr );
    }
    return pMem;
}


LPVOID
AllocSplMem(
    size_t cbAlloc
    )

{
    PVOID pvMemory;

    pvMemory = GlobalAlloc(GMEM_FIXED, cbAlloc);

    if( pvMemory ){
        ZeroMemory( pvMemory, cbAlloc );
    }

    return pvMemory;
}


