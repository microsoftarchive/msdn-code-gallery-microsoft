/****************************** Module Header ******************************\
* Module Name:  CppNamedPipeClient.cpp
* Project:      CppNamedPipeClient
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
* This code sample demonstrates a named pipe client that attempts to connect 
* to the named pipe "\\.\pipe\SamplePipe" with the GENERIC_READ and 
* GENERIC_WRITE accesses. After the pipe is connected, the application calls 
* WriteFile and ReadFile to write a message to the pipe and receive the 
* server's response.
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
#pragma endregion


// The full name of the pipe in the format of \\servername\pipe\pipename.
#define SERVER_NAME         L"."
#define PIPE_NAME           L"SamplePipe"
#define FULL_PIPE_NAME      L"\\\\" SERVER_NAME L"\\pipe\\" PIPE_NAME

#define BUFFER_SIZE     1024

// Request message from client to server.
#define REQUEST_MESSAGE     L"Default request from client"


int wmain(int argc, wchar_t* argv[])
{
    HANDLE hPipe = INVALID_HANDLE_VALUE;
    DWORD dwError = ERROR_SUCCESS;

    // Try to open the named pipe identified by the pipe name.
    while (TRUE) 
    {
        hPipe = CreateFile( 
            FULL_PIPE_NAME,                 // Pipe name 
            GENERIC_READ | GENERIC_WRITE,   // Read and write access
            0,                              // No sharing 
            NULL,                           // Default security attributes
            OPEN_EXISTING,                  // Opens existing pipe
            0,                              // Default attributes
            NULL                            // No template file
            );

        // If the pipe handle is opened successfully ...
        if (hPipe != INVALID_HANDLE_VALUE)
        {
            wprintf(L"The named pipe (%s) is connected.\n", FULL_PIPE_NAME);
            break;
        }

        dwError = GetLastError();

        // Exit if an error other than ERROR_PIPE_BUSY occurs.
        if (ERROR_PIPE_BUSY != dwError)
        {
            wprintf(L"Unable to open named pipe w/err 0x%08lx\n", dwError);
            goto Cleanup;
        }

        // All pipe instances are busy, so wait for 5 seconds.
        if (!WaitNamedPipe(FULL_PIPE_NAME, 5000))
        {
            dwError = GetLastError();
            wprintf(L"Could not open pipe: 5 second wait timed out.");
            goto Cleanup;
        }
    }

    // Set the read mode and the blocking mode of the named pipe. In this 
    // sample, we set data to be read from the pipe as a stream of messages.
    DWORD dwMode = PIPE_READMODE_MESSAGE;
    if (!SetNamedPipeHandleState(hPipe, &dwMode, NULL, NULL))
    {
        dwError = GetLastError();
        wprintf(L"SetNamedPipeHandleState failed w/err 0x%08lx\n", dwError);
        goto Cleanup;
    }

    // 
    // Send a request from client to server
    // 

    wchar_t chRequest[] = REQUEST_MESSAGE;
    DWORD cbRequest, cbWritten;
    cbRequest = sizeof(chRequest);

    if (!WriteFile(
        hPipe,                      // Handle of the pipe
        chRequest,                  // Message to be written
        cbRequest,                  // Number of bytes to write
        &cbWritten,                 // Number of bytes written
        NULL                        // Not overlapped
        ))
    {
        dwError = GetLastError();
        wprintf(L"WriteFile to pipe failed w/err 0x%08lx\n", dwError);
        goto Cleanup;
    }

    wprintf(L"Send %ld bytes to server: \"%s\"\n", cbWritten, chRequest);

    //
    // Receive a response from server.
    // 

    BOOL fFinishRead = FALSE;
    do
    {
        wchar_t chResponse[BUFFER_SIZE];
        DWORD cbResponse, cbRead;
        cbResponse = sizeof(chResponse);

        fFinishRead = ReadFile(
            hPipe,                  // Handle of the pipe
            chResponse,             // Buffer to receive the reply
            cbResponse,             // Size of buffer in bytes
            &cbRead,                // Number of bytes read 
            NULL                    // Not overlapped 
            );

        if (!fFinishRead && ERROR_MORE_DATA != GetLastError())
        {
            dwError = GetLastError();
            wprintf(L"ReadFile from pipe failed w/err 0x%08lx\n", dwError);
            goto Cleanup;
        }

        wprintf(L"Receive %ld bytes from server: \"%s\"\n", cbRead, chResponse);

    } while (!fFinishRead); // Repeat loop if ERROR_MORE_DATA

Cleanup:

    // Centralized cleanup for all allocated resources.
    if (hPipe != INVALID_HANDLE_VALUE)
    {
        CloseHandle(hPipe);
        hPipe = INVALID_HANDLE_VALUE;
    }

    return dwError;
}