/****************************** Module Header ******************************\
* Module Name:  CppNamedPipeServer.cpp
* Project:      CppNamedPipeServer
* Copyright (c) Microsoft Corporation.
* 
* Named pipe is a mechanism for one-way or duplex inter-process communication 
* between the pipe server and one or more pipe clients in the local machine 
* or across the computers in the intranet:
* 
* PIPE_ACCESS_INBOUND:
* Client (GENERIC_WRITE) ---> Server (GENERIC_READ)
* 
* PIPE_ACCESS_OUTBOUND:
* Client (GENERIC_READ) <--- Server (GENERIC_WRITE)
* 
* PIPE_ACCESS_DUPLEX:
* Client (GENERIC_READ or GENERIC_WRITE, or both) <--> 
* Server (GENERIC_READ and GENERIC_WRITE)
* 
* This code sample demonstrates calling CreateNamedPipe to create a pipe 
* named "\\.\pipe\SamplePipe", which supports duplex connections so that both 
* client and server can read from and write to the pipe. The security 
* attributes of the pipe are customized to allow Authenticated Users read and 
* write access to a pipe, and to allow the Administrators group full access 
* to the pipe. When the pipe is connected by a client, the server attempts to 
* read the client's message from the pipe by calling ReadFile, and write a 
* response by calling WriteFile.
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
#include <stdio.h>
#include <windows.h>
#include <sddl.h>
#pragma endregion


// The full name of the pipe in the format of \\servername\pipe\pipename.
#define SERVER_NAME         L"."
#define PIPE_NAME           L"SamplePipe"
#define FULL_PIPE_NAME      L"\\\\" SERVER_NAME L"\\pipe\\" PIPE_NAME

#define BUFFER_SIZE     1024

// Response message from client to server. '\0' is appended in the end 
// because the client may be a native C++ application that expects NULL 
// termiated string.
#define RESPONSE_MESSAGE    L"Default response from server"


BOOL CreatePipeSecurity(PSECURITY_ATTRIBUTES *);
void FreePipeSecurity(PSECURITY_ATTRIBUTES);


int wmain(int argc, wchar_t* argv[])
{
    DWORD dwError = ERROR_SUCCESS;
    PSECURITY_ATTRIBUTES pSa = NULL;
    HANDLE hNamedPipe = INVALID_HANDLE_VALUE;

    // Prepare the security attributes (the lpSecurityAttributes parameter in 
    // CreateNamedPipe) for the pipe. This is optional. If the 
    // lpSecurityAttributes parameter of CreateNamedPipe is NULL, the named 
    // pipe gets a default security descriptor and the handle cannot be 
    // inherited. The ACLs in the default security descriptor of a pipe grant 
    // full control to the LocalSystem account, (elevated) administrators, 
    // and the creator owner. They also give only read access to members of 
    // the Everyone group and the anonymous account. However, if you want to 
    // customize the security permission of the pipe, (e.g. to allow 
    // Authenticated Users to read from and write to the pipe), you need to 
    // create a SECURITY_ATTRIBUTES structure.
    if (!CreatePipeSecurity(&pSa))
    {
        dwError = GetLastError();
        wprintf(L"CreatePipeSecurity failed w/err 0x%08lx\n", dwError);
        goto Cleanup;
    }

    // Create the named pipe.
    hNamedPipe = CreateNamedPipe(
        FULL_PIPE_NAME,             // Pipe name.
        PIPE_ACCESS_DUPLEX,         // The pipe is duplex; both server and 
                                    // client processes can read from and 
                                    // write to the pipe
        PIPE_TYPE_MESSAGE |         // Message type pipe 
        PIPE_READMODE_MESSAGE |     // Message-read mode 
        PIPE_WAIT,                  // Blocking mode is enabled
        PIPE_UNLIMITED_INSTANCES,   // Max. instances
        BUFFER_SIZE,                // Output buffer size in bytes
        BUFFER_SIZE,                // Input buffer size in bytes
        NMPWAIT_USE_DEFAULT_WAIT,   // Time-out interval
        pSa                         // Security attributes
        );

    if (hNamedPipe == INVALID_HANDLE_VALUE)
    {
        dwError = GetLastError();
        wprintf(L"Unable to create named pipe w/err 0x%08lx\n", dwError);
        goto Cleanup;
    }

    wprintf(L"The named pipe (%s) is created.\n", FULL_PIPE_NAME);

    // Wait for the client to connect.
    wprintf(L"Waiting for the client's connection...\n");
    if (!ConnectNamedPipe(hNamedPipe, NULL))
    {
        if (ERROR_PIPE_CONNECTED != GetLastError())
        {
            dwError = GetLastError();
            wprintf(L"ConnectNamedPipe failed w/err 0x%08lx\n", dwError);
            goto Cleanup;
        }
    }
    wprintf(L"Client is connected.\n");

    // 
    // Receive a request from client.
    // 

    BOOL fFinishRead = FALSE;
    do
    {
        wchar_t chRequest[BUFFER_SIZE];
        DWORD cbRequest, cbRead;
        cbRequest = sizeof(chRequest);

        fFinishRead = ReadFile(
            hNamedPipe,     // Handle of the pipe
            chRequest,      // Buffer to receive data
            cbRequest,      // Size of buffer in bytes
            &cbRead,        // Number of bytes read
            NULL            // Not overlapped I/O
            );

        if (!fFinishRead && ERROR_MORE_DATA != GetLastError())
        {
            dwError = GetLastError();
            wprintf(L"ReadFile from pipe failed w/err 0x%08lx\n", dwError);
            goto Cleanup;
        }

        wprintf(L"Receive %ld bytes from client: \"%s\"\n", cbRead, chRequest);

    } while (!fFinishRead); // Repeat loop if ERROR_MORE_DATA

    // 
    // Send a response from server to client.
    // 

    wchar_t chResponse[] = RESPONSE_MESSAGE;
    DWORD cbResponse, cbWritten;
    cbResponse = sizeof(chResponse);

    if (!WriteFile(
        hNamedPipe,     // Handle of the pipe
        chResponse,     // Buffer to write
        cbResponse,     // Number of bytes to write 
        &cbWritten,     // Number of bytes written 
        NULL            // Not overlapped I/O
        ))
    {
        dwError = GetLastError();
        wprintf(L"WriteFile to pipe failed w/err 0x%08lx\n", dwError);
        goto Cleanup;
    }

    wprintf(L"Send %ld bytes to client: \"%s\"\n", cbWritten, chResponse);

    // Flush the pipe to allow the client to read the pipe's contents 
    // before disconnecting. Then disconnect the client's connection. 
    FlushFileBuffers(hNamedPipe);
    DisconnectNamedPipe(hNamedPipe);

Cleanup:

    // Centralized cleanup for all allocated resources.
    if (pSa != NULL)
    {
        FreePipeSecurity(pSa);
        pSa = NULL;
    }
    if (hNamedPipe != INVALID_HANDLE_VALUE)
    {
        CloseHandle(hNamedPipe);
        hNamedPipe = INVALID_HANDLE_VALUE;
    }

    return dwError;
}


//
//   FUNCTION: CreatePipeSecurity(PSECURITY_ATTRIBUTES *)
//
//   PURPOSE: The CreatePipeSecurity function creates and initializes a new 
//   SECURITY_ATTRIBUTES structure to allow Authenticated Users read and 
//   write access to a pipe, and to allow the Administrators group full 
//   access to the pipe.
//
//   PARAMETERS:
//   * ppSa - output a pointer to a SECURITY_ATTRIBUTES structure that allows 
//     Authenticated Users read and write access to a pipe, and allows the 
//     Administrators group full access to the pipe. The structure must be 
//     freed by calling FreePipeSecurity.
//
//   RETURN VALUE: Returns TRUE if the function succeeds.
//
//   EXAMPLE CALL:
//
//     PSECURITY_ATTRIBUTES pSa = NULL;
//     if (CreatePipeSecurity(&pSa))
//     {
//         // Use the security attributes
//         // ...
//
//         FreePipeSecurity(pSa);
//     }
//
BOOL CreatePipeSecurity(PSECURITY_ATTRIBUTES *ppSa)
{
    BOOL fSucceeded = TRUE;
    DWORD dwError = ERROR_SUCCESS;

    PSECURITY_DESCRIPTOR pSd = NULL;
    PSECURITY_ATTRIBUTES pSa = NULL;

    // Define the SDDL for the security descriptor.
    PCWSTR szSDDL = L"D:"       // Discretionary ACL
        L"(A;OICI;GRGW;;;AU)"   // Allow read/write to authenticated users
        L"(A;OICI;GA;;;BA)";    // Allow full control to administrators

    if (!ConvertStringSecurityDescriptorToSecurityDescriptor(szSDDL, 
        SDDL_REVISION_1, &pSd, NULL))
    {
        fSucceeded = FALSE;
        dwError = GetLastError();
        goto Cleanup;
    }
    
    // Allocate the memory of SECURITY_ATTRIBUTES.
    pSa = (PSECURITY_ATTRIBUTES)LocalAlloc(LPTR, sizeof(*pSa));
    if (pSa == NULL)
    {
        fSucceeded = FALSE;
        dwError = GetLastError();
        goto Cleanup;
    }

    pSa->nLength = sizeof(*pSa);
    pSa->lpSecurityDescriptor = pSd;
    pSa->bInheritHandle = FALSE;

    *ppSa = pSa;

Cleanup:
    // Clean up the allocated resources if something is wrong.
    if (!fSucceeded)
    {
        if (pSd)
        {
            LocalFree(pSd);
            pSd = NULL;
        }
        if (pSa)
        {
            LocalFree(pSa);
            pSa = NULL;
        }

        SetLastError(dwError);
    }
    
    return fSucceeded;
}


//
//   FUNCTION: FreePipeSecurity(PSECURITY_ATTRIBUTES)
//
//   PURPOSE: The FreePipeSecurity function frees a SECURITY_ATTRIBUTES 
//   structure that was created by the CreatePipeSecurity function. 
//
//   PARAMETERS:
//   * pSa - pointer to a SECURITY_ATTRIBUTES structure that was created by 
//     the CreatePipeSecurity function. 
//
void FreePipeSecurity(PSECURITY_ATTRIBUTES pSa)
{
    if (pSa)
    {
        if (pSa->lpSecurityDescriptor)
        {
            LocalFree(pSa->lpSecurityDescriptor);
        }
        LocalFree(pSa);
    }
}