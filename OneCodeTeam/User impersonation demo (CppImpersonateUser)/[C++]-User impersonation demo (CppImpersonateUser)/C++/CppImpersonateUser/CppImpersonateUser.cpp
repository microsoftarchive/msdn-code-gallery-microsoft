/***************************** Module Header *******************************\
* Module Name:  CppImpersonateUser.cpp
* Project:      CppImpersonateUser
* Copyright (c) Microsoft Corporation.
* 
* Windows Impersonation is a powerful feature Windows uses frequently in its 
* security model. In general Windows also uses impersonation in its client/
* server programming model.Impersonation lets a server to temporarily adopt 
* the security profile of a client making a resource request. The server can
* then access resources on behalf of the client, and the OS carries out the 
* access validations.
* A server impersonates a client only within the thread that makes the 
* impersonation request. Thread-control data structures contain an optional 
* entry for an impersonation token. However, a thread's primary token, which
* represents the thread's real security credentials, is always accessible in 
* the process's control structure.
* 
* After the server thread finishes its task, it reverts to its primary 
* security profile. These forms of impersonation are convenient for carrying 
* out specific actions at the request of a client and for ensuring that object
* accesses are audited correctly.
* 
* In this code sample we use the LogonUser API and the ImpersonateLoggedOnUser
* API to impersonate the user represented by the specified user token. Then 
* display the current user via the GetUserName API to show user impersonation.
* LogonUser can only be used to log onto the local machine; it cannot log you
* onto a remote computer. The account that you use in the LogonUser() call 
* must also be known to the local machine, either as a local account or as a
* domain account that is visible to the local computer. If LogonUser is 
* successful, then it will give you an access token that specifies the 
* credentials of the user account you chose.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include <stdio.h>
#include <windows.h>
#pragma endregion


#define INFO_BUFFER_SIZE	260


//
//   FUNCTION: ReportError(LPWSTR, DWORD)
//
//   PURPOSE: Display an error message for the failure of a certain function.
//
//   PARAMETERS:
//   * pszFunction - the name of the function that failed.
//   * dwError - the Win32 error code. Its default value is the calling 
//   thread's last-error code value.
//
//   NOTE: The failing function must be immediately followed by the call of 
//   ReportError if you do not explicitly specify the dwError parameter of 
//   ReportError. This is to ensure that the calling thread's last-error code 
//   value is not overwritten by any calls of API between the failing 
//   function and ReportError.
//
void ReportError(LPCWSTR pszFunction, DWORD dwError = GetLastError())
{
    wprintf(L"%s failed w/err 0x%08lx\n", pszFunction, dwError);
}


int wmain(int argc, wchar_t* argv[])
{
	wchar_t szCurrentUserName[INFO_BUFFER_SIZE] = {};
	wchar_t szUserName[INFO_BUFFER_SIZE] = {};
	wchar_t szDomain[INFO_BUFFER_SIZE] = {};
	wchar_t szPassword[INFO_BUFFER_SIZE] = {};
    wchar_t *pc = NULL;
    HANDLE hToken = NULL;
    BOOL fSucceeded = FALSE;

    // Print the name of the user associated with the current thread.
	wprintf(L"Before the impersonation ...\n");
	DWORD nSize = ARRAYSIZE(szCurrentUserName);
	if (!GetUserName(szCurrentUserName, &nSize))
	{
        ReportError(L"GetUserName");
        goto Cleanup;
	}
    wprintf(L"The current user is %s\n\n", szCurrentUserName);

    // Gather the credential information of the impersonated user.

    wprintf(L"Enter the name of the impersonated user: ");
	fgetws(szUserName, ARRAYSIZE(szUserName), stdin);
	pc = wcschr(szUserName, L'\n');
	if (pc != NULL) *pc = L'\0';  // Remove the trailing L'\n'

    wprintf(L"Enter the domain name: ");
	fgetws(szDomain, ARRAYSIZE(szDomain), stdin);
	pc = wcschr(szDomain, L'\n');
	if (pc != NULL) *pc = L'\0';  // Remove the trailing L'\n'

    wprintf(L"Enter the password: ");
	fgetws(szPassword, ARRAYSIZE(szPassword), stdin);
	pc = wcschr(szPassword, L'\n');
    if (pc != NULL) *pc = L'\0';  // Remove the trailing L'\n'

	// Attempt to log on the user.
	if (!LogonUser(szUserName, szDomain, szPassword, 
        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, &hToken))
	{
		ReportError(L"LogonUser");
		goto Cleanup;
	}

	// Impersonate the logged on user.
	if (!ImpersonateLoggedOnUser(hToken))
	{
		ReportError(L"ImpersonateLoggedOnUser");
		goto Cleanup;
	}

    // The impersonation is successful.
    fSucceeded = TRUE;
    wprintf(L"\nThe impersonation is successful\n");

    // Print the name of the user associated with the current thread.
	ZeroMemory(szCurrentUserName, sizeof(szCurrentUserName));
	nSize = ARRAYSIZE(szCurrentUserName);
	if (!GetUserName(szCurrentUserName, &nSize))
	{
        ReportError(L"GetUserName");
        goto Cleanup;
	}
    wprintf(L"The current user is %s\n\n", szCurrentUserName);

	// Work as the impersonated user.
    // ...

Cleanup:

    // Clean up the buffer containing sensitive password.
	SecureZeroMemory(szPassword, sizeof(szPassword));

    // If the impersonation was successful, undo the impersonation.
    if (fSucceeded)
    {
        wprintf(L"Undo the impersonation ...\n");
        if (!RevertToSelf())
        {
            ReportError(L"RevertToSelf");
        }

        // Print the name of the user associated with the current thread.
        ZeroMemory(szCurrentUserName, sizeof(szCurrentUserName));
        nSize = ARRAYSIZE(szCurrentUserName);
        if (!GetUserName(szCurrentUserName, &nSize))
        {
            ReportError(L"GetUserName");
        }
        wprintf(L"The current user is %s\n\n", szCurrentUserName);
    }
    
	if (hToken)
	{
		CloseHandle(hToken);
		hToken = NULL;
	}

	return 0;
}