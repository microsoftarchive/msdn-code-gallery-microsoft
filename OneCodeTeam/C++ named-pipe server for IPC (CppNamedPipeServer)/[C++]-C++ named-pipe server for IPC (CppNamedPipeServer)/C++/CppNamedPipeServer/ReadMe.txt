=============================================================================
        CONSOLE APPLICATION : CppNamedPipeServer Project Overview
=============================================================================

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

This code sample demonstrates calling CreateNamedPipe to create a pipe 
named "\\.\pipe\SamplePipe", which supports duplex connections so that both 
client and server can read from and write to the pipe. The security 
attributes of the pipe are customized to allow Authenticated Users read and 
write access to a pipe, and to allow the Administrators group full access to 
the pipe. When the pipe is connected by a client, the server attempts to read 
the client's message from the pipe by calling ReadFile, and write a response 
by calling WriteFile.


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

CppNamedPipeClient/CSNamedPipeClient/VBNamedPipeClient -> CppNamedPipeServer
CppNamedPipeServer is the server end of the named pipe. CSNamedPipeClient, 
VBNamedPipeClient and CppNamedPipeClient can be the client ends that connect
to the named pipe.

CppNamedPipeServer - CSNamedPipeServer - VBNamedPipeServer
CppNamedPipeServer, CSNamedPipeServer and VBNamedPipeServer are the same 
named pipe server ends written in three different programming languages.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Create a named pipe server by calling CreateNamedPipe and specifying the 
pipe name, pipe direction, transmission mode, security attributes, etc.

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

In this code sample, the pipe server support message-based duplex 
communications. The security attributes of the pipe are customized to allow 
Authenticated Users read and write access to a pipe, and to allow the 
Administrators group full access to the pipe.

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
    //   RETURN VALUE: Returns TRUE if the function succeeds..
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

2. Wait for the client to connect by calling ConnectNamedPipe.

    if (!ConnectNamedPipe(hNamedPipe, NULL))
    {
        if (ERROR_PIPE_CONNECTED != GetLastError())
        {
            dwError = GetLastError();
            wprintf(L"ConnectNamedPipe failed w/err 0x%08lx\n", dwError);
            goto Cleanup;
        }
    }

3. Read the client's request from the pipe and write a response by calling 
ReadFile and WriteFile.

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

4. Flush the pipe to allow the client to read the pipe's contents before 
disconnecting. Then disconnect the client's connection.

    FlushFileBuffers(hNamedPipe);
    DisconnectNamedPipe(hNamedPipe);

5. Close the server end of the pipe.

    CloseHandle(hNamedPipe);


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Named Pipes
http://msdn.microsoft.com/en-us/library/aa365590.aspx

MSDN: Using Pipes / Multithreaded Pipe Server
http://msdn.microsoft.com/en-us/library/aa365588.aspx

How to create an anonymous pipe that gives access to everyone
http://support.microsoft.com/kb/813414


/////////////////////////////////////////////////////////////////////////////