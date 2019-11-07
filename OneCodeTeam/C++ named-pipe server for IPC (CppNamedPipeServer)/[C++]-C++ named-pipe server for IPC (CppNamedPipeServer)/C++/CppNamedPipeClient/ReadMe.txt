========================================================================
    CONSOLE APPLICATION : CppNamedPipeClient Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

Named pipe is a mechanism for one-way or duplex inter-process communication 
between the pipe server and one or more pipe clients in the local machine or 
across the computers in the intranet:

PIPE_ACCESS_INBOUND:
Client (GENERIC_WRITE) ---> Server (GENERIC_READ)

PIPE_ACCESS_OUTBOUND:
Client (GENERIC_READ) <--- Server (GENERIC_WRITE)

PIPE_ACCESS_DUPLEX:
Client (GENERIC_READ or GENERIC_WRITE, or both) <--> 
Server (GENERIC_READ and GENERIC_WRITE)

This code sample demonstrates a named pipe client that attempts to connect to 
the named pipe "\\.\pipe\SamplePipe" with the GENERIC_READ and 
GENERIC_WRITE accesses. After the pipe is connected, the application calls 
WriteFile and ReadFile to write a message to the pipe and receive the 
server's response.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the named pipe sample.

Step1. After you successfully build the CppNamedPipeClient and 
CppNamedPipeServer sample projects in Visual Studio 2008, you will get the 
applications: CppNamedPipeClient.exe and CppNamedPipeServer.exe. 

Step2. Run CppNamedPipeServer.exe in a command prompt to start up the server 
end of the named pipe. The application will output the following information 
in the command prompt if the pipe is created successfully.

 Server:
  The named pipe (\\.\pipe\SamplePipe) is created.
  Waiting for the client's connection...

Step3. Run CppNamedPipeClient.exe in another command prompt to start up the 
client end of the named pipe. The application should output the message below 
in the command prompt when the client successfully connects to the named pipe.

 Client:
  The named pipe (\\.\pipe\SamplePipe) is connected.

In the meantime, the server application outputs this message to indicate that 
the pipe is connected by a client.

 Server:
  Client is connected.

Step4. The client attempts to write a message to the named pipe. You will see 
the message below in the commond prompt running the client application.

 Client:
  Send 56 bytes to server: "Default request from client"

When the server application reads the message from the client, it prints:

 Server:
  Receive 56 bytes from client: "Default request from client"

Next, the server writes a response to the pipe.

 Server:
  Send 58 bytes to client: "Default response from server"

And the client receives the response:

 Client:
  Receive 58 bytes from server: "Default response from server"

The connection is disconnected and the pipe is closed after that.


/////////////////////////////////////////////////////////////////////////////
Sample Relation:
(The relationship between the current sample and the rest samples in 
Microsoft All-In-One Code Framework http://1code.codeplex.com)

CppNamedPipeClient -> CppNamedPipeServer/CSNamedPipeServer/VBNamedPipeServer
CppNamedPipeClient is the client end of the named pipe. CSNamedPipeServer, 
VBNamedPipeServer and CppNamedPipeServer can be the server ends that create 
the named pipe.

CppNamedPipeClient - CSNamedPipeClient - VBNamedPipeClient
CppNamedPipeClient, CSNamedPipeClient and VBNamedPipeClient are the same 
named pipe client ends written in three different programming languages.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Try to connect to a named pipe by calling CreateFile and specifying the 
target pipe server, name, desired access, etc. If all pipe instances are busy, 
wait for 5 seconds and connect again.

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

2. Set the read mode and the blocking mode of the named pipe. In this sample, 
we set data to be read from the pipe as a stream of messages.

    DWORD dwMode = PIPE_READMODE_MESSAGE;
    if (!SetNamedPipeHandleState(hPipe, &dwMode, NULL, NULL))
    {
        dwError = GetLastError();
        wprintf(L"SetNamedPipeHandleState failed w/err 0x%08lx\n", dwError);
        goto Cleanup;
    }

3. Send a message to the pipe server and receive its response by calling 
the WriteFile and ReadFile functions.

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

4. Close the pipe.

    CloseHandle(hPipe);


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Named Pipes
http://msdn.microsoft.com/en-us/library/aa365590.aspx

MSDN: Using Pipes / Named Pipe Client
http://msdn.microsoft.com/en-us/library/aa365592.aspx


/////////////////////////////////////////////////////////////////////////////
