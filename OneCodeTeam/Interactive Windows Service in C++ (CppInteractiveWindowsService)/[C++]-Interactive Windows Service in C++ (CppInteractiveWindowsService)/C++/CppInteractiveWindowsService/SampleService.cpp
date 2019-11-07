/****************************** Module Header ******************************\
* Module Name:  SampleService.cpp
* Project:      CppInteractiveWindowsService
* Copyright (c) Microsoft Corporation.
* 
* Provides a sample service class that derives from the service base class - 
* CServiceBase. The sample service logs the service start and stop 
* information to the Application event log, and shows how to run the main 
* function of the service in a thread pool worker thread.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include "SampleService.h"
#include "ThreadPool.h"
#include <wtsapi32.h>
#pragma comment(lib, "wtsapi32.lib")
#include <userenv.h>
#pragma comment(lib, "userenv.lib")
#pragma endregion


DWORD GetSessionIdOfUser(PCWSTR, PCWSTR);
BOOL DisplayInteractiveMessage(DWORD, PWSTR, PWSTR, DWORD, BOOL, DWORD, DWORD *);
BOOL CreateInteractiveProcess(DWORD, PWSTR, BOOL, DWORD, DWORD *);


CSampleService::CSampleService(PWSTR pszServiceName, 
                               BOOL fCanStop, 
                               BOOL fCanShutdown, 
                               BOOL fCanPauseContinue)
: CServiceBase(pszServiceName, fCanStop, fCanShutdown, fCanPauseContinue)
{
    // Create a manual-reset event that is not signaled at first. It is 
    // signaled when the main function of the service is finished.
    m_hFinishedEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    if (m_hFinishedEvent == NULL)
    {
        throw GetLastError();
    }
}


CSampleService::~CSampleService(void)
{
    if (m_hFinishedEvent)
    {
        CloseHandle(m_hFinishedEvent);
        m_hFinishedEvent = NULL;
    }
}


//
//   FUNCTION: CSampleService::OnStart(DWORD, LPWSTR *)
//
//   PURPOSE: The function is executed when a Start command is sent to the 
//   service by the SCM or when the operating system starts (for a service 
//   that starts automatically). It specifies actions to take when the 
//   service starts. In this code sample, OnStart logs a service-start 
//   message to the Application log, and queues the main service function for 
//   execution in a thread pool worker thread.
//
//   PARAMETERS:
//   * dwArgc   - number of command line arguments
//   * lpszArgv - array of command line arguments
//
//   NOTE: A service application is designed to be long running. Therefore, 
//   it usually polls or monitors something in the system. The monitoring is 
//   set up in the OnStart method. However, OnStart does not actually do the 
//   monitoring. The OnStart method must return to the operating system after 
//   the service's operation has begun. It must not loop forever or block. To 
//   set up a simple monitoring mechanism, one general solution is to create 
//   a timer in OnStart. The timer would then raise events in your code 
//   periodically, at which time your service could do its monitoring. The 
//   other solution is to spawn a new thread to perform the main service 
//   functions, which is demonstrated in this code sample.
//
void CSampleService::OnStart(DWORD dwArgc, LPWSTR *lpszArgv)
{
    // Log a service start message to the Application log.
    WriteEventLogEntry(L"CppInteractiveWindowsService in OnStart", 
        EVENTLOG_INFORMATION_TYPE);

    // Queue the main service function for execution in a worker thread.
    CThreadPool::QueueUserWorkItem(&CSampleService::ServiceWorkerThread, this);
}


//
//   FUNCTION: CSampleService::ServiceWorkerThread(void)
//
//   PURPOSE: The method performs the main function of the service. It runs 
//   on a thread pool worker thread.
//
void CSampleService::ServiceWorkerThread(void)
{
    // Get the ID of the session attached to the physical console.
    DWORD dwSessionId = GetSessionIdOfUser(NULL, NULL);
    if (dwSessionId == 0xFFFFFFFF)
    {
        // Log the error and exit.
        WriteErrorLogEntry(L"GetSessionIdOfUser", GetLastError());
        goto Exit;
    }

    // Display an interactive message in the session.
    wchar_t szTitle[] = L"CppInteractiveWindowsService";
    wchar_t szMessage[] = L"Do you want to start Notepad?";
    DWORD dwResponse;
    if (!DisplayInteractiveMessage(dwSessionId, szTitle, szMessage, MB_YESNO, 
        TRUE, 5 /*5 seconds*/, &dwResponse))
    {
        // Log the error and exit.
        WriteErrorLogEntry(L"DisplayInteractiveMessage", GetLastError());
        goto Exit;
    }

    if (IDYES == dwResponse) // If the user choose 'Yes'
    {
        // Launch notepad.
        wchar_t szCommandLine[] = L"notepad.exe";
        DWORD dwExitCode;
        if (!CreateInteractiveProcess(dwSessionId, szCommandLine, FALSE, 0, 
            &dwExitCode))
        {
            // Log the error and exit.
            WriteErrorLogEntry(L"CreateInteractiveProcess", GetLastError());
            goto Exit;
        }
    }

Exit:
    // Signal the finished event.
    SetEvent(m_hFinishedEvent);
}


//
//   FUNCTION: CSampleService::OnStop(void)
//
//   PURPOSE: The function is executed when a Stop command is sent to the 
//   service by SCM. It specifies actions to take when a service stops 
//   running. In this code sample, OnStop logs a service-stop message to the 
//   Application log, and waits for the finish of the main service function.
//
//   COMMENTS:
//   Be sure to periodically call ReportServiceStatus() with 
//   SERVICE_STOP_PENDING if the procedure is going to take long time. 
//
void CSampleService::OnStop()
{
    // Log a service stop message to the Application log.
    WriteEventLogEntry(L"CppInteractiveWindowsService in OnStop", 
        EVENTLOG_INFORMATION_TYPE);

    // Wait for the finish of the main service function (ServiceWorkerThread).
    if (WaitForSingleObject(m_hFinishedEvent, INFINITE) != WAIT_OBJECT_0)
    {
        throw GetLastError();
    }
}


DWORD GetSessionIdOfUser(PCWSTR pszUserName, 
                         PCWSTR pszDomain)
{
    DWORD dwSessionId = 0xFFFFFFFF;
    
    if (pszUserName == NULL)
    {
        // If the user name is not provided, try to get the session attached 
        // to the physical console. The physical console is the monitor, 
        // keyboard, and mouse.
        dwSessionId = WTSGetActiveConsoleSessionId();
    }
    else
    {
        // If the user name is provided, get the session of the provided user. 
        // The same user could have more than one session, this sample just 
        // retrieves the first one found. You can add more sophisticated 
        // checks by requesting different types of information from 
        // WTSQuerySessionInformation.

        PWTS_SESSION_INFO *pSessionsBuffer = NULL;
        DWORD dwSessionCount = 0;

        // Enumerate the sessions on the current server.
        if (WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, 
            pSessionsBuffer, &dwSessionCount))
        {
            for (DWORD i = 0; (dwSessionId == -1) && (i < dwSessionCount); i++)
            {
                DWORD sid = pSessionsBuffer[i]->SessionId;

                // Get the user name from the session ID.
                PWSTR pszSessionUserName = NULL;
                DWORD dwSize;
                if (WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, sid, 
                    WTSUserName, &pszSessionUserName, &dwSize))
                {
                    // Compare with the provided user name (case insensitive).
                    if (_wcsicmp(pszUserName, pszSessionUserName) == 0)
                    {
                        // Get the domain from the session ID.
                        PWSTR pszSessionDomain = NULL;
                        if (WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, 
                            sid, WTSDomainName, &pszSessionDomain, &dwSize))
                        {
                            // Compare with the provided domain (case insensitive).
                            if (_wcsicmp(pszDomain, pszSessionDomain) == 0)
                            {
                                // The session of the provided user is found.
                                dwSessionId = sid;
                            }
                            WTSFreeMemory(pszSessionDomain);
                        }
                    }
                    WTSFreeMemory(pszSessionUserName);
                }
            }

            WTSFreeMemory(pSessionsBuffer);
            pSessionsBuffer = NULL;
            dwSessionCount = 0;

            // Cannot find the session of the provided user.
            if (dwSessionId == 0xFFFFFFFF)
            {
                SetLastError(ERROR_INVALID_PARAMETER);
            }
        }
    }

    return dwSessionId;
}


BOOL DisplayInteractiveMessage(DWORD dwSessionId,
                               PWSTR pszTitle, 
                               PWSTR pszMessage,
                               DWORD dwStyle, 
                               BOOL fWait, 
                               DWORD dwTimeoutSeconds, 
                               DWORD *pResponse)
{
    DWORD cbTitle = wcslen(pszTitle) * sizeof(*pszTitle);
    DWORD cbMessage = wcslen(pszMessage) * sizeof(*pszMessage);

    return WTSSendMessage(
        WTS_CURRENT_SERVER_HANDLE,  // The current server
        dwSessionId,                // Identify the session to display message
        pszTitle,                   // Title bar of the message box
        cbTitle,                    // Length, in bytes, of the title
        pszMessage,                 // Message to display
        cbMessage,                  // Length, in bytes, of the message
        dwStyle,                    // Contents and behavior of the message
        dwTimeoutSeconds,           // Timeout of the message in seconds
        pResponse,                  // Receive the user's response
        fWait                       // Whether wait for user's response or not
        );
}


BOOL CreateInteractiveProcess(DWORD dwSessionId,
                              PWSTR pszCommandLine, 
                              BOOL fWait, 
                              DWORD dwTimeout, 
                              DWORD *pExitCode)
{
    DWORD dwError = ERROR_SUCCESS;
    HANDLE hToken = NULL;
    LPVOID lpvEnv = NULL;
    wchar_t szUserProfileDir[MAX_PATH];
    DWORD cchUserProfileDir = ARRAYSIZE(szUserProfileDir);
    STARTUPINFO si = { sizeof(si) };
    PROCESS_INFORMATION pi = { 0 };
    DWORD dwWaitResult;

    // Obtain the primary access token of the logged-on user specified by the 
    // session ID.
    if (!WTSQueryUserToken(dwSessionId, &hToken))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // Run the command line in the session that we found by using the default 
    // values for working directory and desktop.

    // This creates the default environment block for the user.
    if (!CreateEnvironmentBlock(&lpvEnv, hToken, TRUE))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // Retrieve the path to the root directory of the user's profile.
    if (!GetUserProfileDirectory(hToken, szUserProfileDir, 
        &cchUserProfileDir))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // Specify that the process runs in the interactive desktop.
    si.lpDesktop = L"winsta0\\default";

    // Launch the process.
    if (!CreateProcessAsUser(hToken, NULL, pszCommandLine, NULL, NULL, FALSE, 
        CREATE_UNICODE_ENVIRONMENT, lpvEnv, szUserProfileDir, &si, &pi))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    if (fWait)
    {
        // Wait for the exit of the process.
        dwWaitResult = WaitForSingleObject(pi.hProcess, dwTimeout);
        if (dwWaitResult == WAIT_OBJECT_0)
        {
            // If the process exits before timeout, get the exit code.
            GetExitCodeProcess(pi.hProcess, pExitCode);
        }
        else if (dwWaitResult == WAIT_TIMEOUT)
        {
            // If it times out, terminiate the process.
            TerminateProcess(pi.hProcess, IDTIMEOUT);
            *pExitCode = IDTIMEOUT;
        }
        else
        {
            dwError = GetLastError();
            goto Cleanup;
        }
    }
    else
    {
        *pExitCode = IDASYNC;
    }

Cleanup:

    // Centralized cleanup for all allocated resources.
    if (hToken)
    {
        CloseHandle(hToken);
        hToken = NULL;
    }
    if (lpvEnv)
    {
        DestroyEnvironmentBlock(lpvEnv);
        lpvEnv = NULL;
    }
    if (pi.hProcess)
    {
        CloseHandle(pi.hProcess);
        pi.hProcess = NULL;
    }
    if (pi.hThread)
    {
        CloseHandle(pi.hThread);
        pi.hThread = NULL;
    }

    // Set the last error if something failed in the function.
    if (dwError != ERROR_SUCCESS)
    {
        SetLastError(dwError);
        return FALSE;
    }
    else
    {
        return TRUE;
    }
}