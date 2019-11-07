========================================================================
    CONSOLE APPLICATION : CSNamedPipeServer Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

Named pipe is a mechanism for one-way or duplex inter-process communication 
between the pipe server and one or more pipe clients in the local machine or 
across the computers in the intranet:

Client (GENERIC_WRITE) ---> Server (GENERIC_READ)

PIPE_ACCESS_OUTBOUND:
Client (GENERIC_READ) <--- Server (GENERIC_WRITE)

PIPE_ACCESS_DUPLEX:
Client (GENERIC_READ or GENERIC_WRITE, or both) <--> 
Server (GENERIC_READ and GENERIC_WRITE)

This code sample demonstrate two methods to create named pipe in Visual C#.

1. Use the System.IO.Pipes namespace

The System.IO.Pipes namespace contains types that provide a means for 
interprocess communication through anonymous and/or named pipes. 
http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx
These classes make the programming of named pipe in .NET much easier and 
safer than P/Invoking native APIs directly. However, the System.IO.Pipes 
namespace is not available before .NET Framework 3.5. So, if you are 
programming against .NET Framework 2.0, you have to P/Invoke native APIs 
to use named pipe.

The sample code in SystemIONamedPipeServer.Run() uses the 
Systen.IO.Pipes.NamedPipeServerStream class to create a pipe that is named 
"\\.\pipe\SamplePipe" to perform message-based communication. The pipe 
supports duplex connections, so both client and server can read from and 
write to the pipe. The security attributes of the pipe are customized to 
allow Authenticated Users read and write access to a pipe, and to allow the 
Administrators group full access to the pipe. When the pipe is connected by a 
client, the server attempts to read the client's message from the pipe, and 
write a response.

2. P/Invoke the native APIs related to named pipe operations.

The .NET interop services make it possible to call native APIs related to 
named pipe operations from .NET. The sample code in 
NativeNamedPipeServer.Run() demonstrates calling CreateNamedPipe to create 
a pipe named "\\.\pipe\SamplePipe", which supports duplex connections so that 
both client and server can read from and write to the pipe. The security 
attributes of the pipe are customized to allow Authenticated Users read and 
write access to a pipe, and to allow the Administrators group full access to 
the pipe. When the pipe is connected by a client, the server attempts to read 
the client's message from the pipe by calling ReadFile, and write a response 
by calling WriteFile. Please note that if you are programming against .NET 
Framework 3.5 or any newer releases of .NET Framework, it is safer and easier 
to use the types in the System.IO.Pipes namespace to operate named pipes.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the named pipe sample.

Step1. After you successfully build the CSNamedPipeClient and 
CSNamedPipeServer sample projects in Visual Studio 2008, you will get the 
applications: CSNamedPipeClient.exe and CSNamedPipeServer.exe. 

Step2. Run CSNamedPipeServer.exe in a command prompt to start up the server 
end of the named pipe. If the command is "CSNamedPipeServer.exe", the pipe 
server is created by the types in the System.IO.Pipes namespace. If the 
command is "CSNamedPipeServer.exe -native", the pipe server is created by 
P/Invoking the native APIs related to named pipe operations. In both cases, 
the application outputs the following information in the command prompt if 
the pipe is created successfully.

 Server:
  The named pipe (\\.\pipe\SamplePipe) is created.
  Waiting for the client's connection...

Step3. Run CSNamedPipeClient.exe in another command prompt to start up the 
client end of the named pipe. If the command is "CSNamedPipeClient.exe", the 
client connects to the pipe by using the types in the System.IO.Pipes 
namespace. If the command is "CSNamedPipeClient.exe -native", the client 
connects to the pipe by P/Invoking the native APIs related to named pipe 
operations. In both cases, the application should output the message below 
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

CSNamedPipeClient/VBNamedPipeClient/CppNamedPipeClient -> CSNamedPipeServer
CSNamedPipeServer is the server end of the named pipe. CSNamedPipeClient, 
VBNamedPipeClient and CppNamedPipeClient can be the client ends that connect
to the named pipe.

CSNamedPipeServer - VBNamedPipeServer - CppNamedPipeServer
CSNamedPipeServer, VBNamedPipeServer and CppNamedPipeServer are the same 
named pipe server ends written in three different programming languages.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

A. Named pipe server by using the System.IO.Pipes namespace. 
(SystemIONamedPipeServer.Run())

1. Create a named pipe server (System.IO.Pipes.NamedPipeServerStream) object 
and specify pipe name, pipe direction, options, transmission mode, security 
attributes, etc.

    // Create the named pipe.
    pipeServer = new NamedPipeServerStream(
        Program.PIPE_NAME,              // The unique pipe name.
        PipeDirection.InOut,            // The pipe is duplex
        NamedPipeServerStream.MaxAllowedServerInstances,
        PipeTransmissionMode.Message,   // Message-based communication
        PipeOptions.None,               // No additional parameters
        Program.BUFFER_SIZE,            // Input buffer size
        Program.BUFFER_SIZE,            // Output buffer size
        pipeSecurity,                   // Pipe security attributes
        HandleInheritability.None       // Not inheritable
        );

In this code sample, the pipe server support message-based duplex 
communications. The security attributes of the pipe are customized to allow 
Authenticated Users read and write access to a pipe, and to allow the 
Administrators group full access to the pipe.

    PipeSecurity pipeSecurity = new PipeSecurity();

    // Allow Everyone read and write access to the pipe.
    pipeSecurity.SetAccessRule(new PipeAccessRule("Authenticated Users",
        PipeAccessRights.ReadWrite, AccessControlType.Allow));

    // Allow the Administrators group full access to the pipe.
    pipeSecurity.SetAccessRule(new PipeAccessRule("Administrators",
        PipeAccessRights.FullControl, AccessControlType.Allow));

2. Wait for the client to connect by calling 
NamedPipeServerStream.WaitForConnection().

    pipeServer.WaitForConnection();

3. Read the client's request from the pipe and write a response by calling 
NamedPipeServerStream.Read and NamedPipeServerStream.Write. The named pipe 
was created to support message-based communication. This allows a reading 
process to read varying-length messages precisely as sent by the writing 
process. In this mode you should not use StreamWriter to write the pipe, or 
use StreamReader to read the pipe. You can read more about the difference 
from the article: http://go.microsoft.com/?linkid=9721786.

    // 
    // Receive a request from client.
    // 
    string message;
    do
    {
        byte[] bRequest = new byte[Program.BUFFER_SIZE];
        int cbRequest = bRequest.Length, cbRead;

        cbRead = pipeServer.Read(bRequest, 0, cbRequest);

        // Unicode-encode the received byte array and trim all the 
        // '\0' characters at the end.
        message = Encoding.Unicode.GetString(bRequest).TrimEnd('\0');
        Console.WriteLine("Receive {0} bytes from client: \"{1}\"",
            cbRead, message);
    }
    while (!pipeServer.IsMessageComplete);

    // 
    // Send a response from server to client.
    // 

    message = Program.RESPONSE_MESSAGE;
    byte[] bResponse = Encoding.Unicode.GetBytes(message);
    int cbResponse = bResponse.Length;

    pipeServer.Write(bResponse, 0, cbResponse);

    Console.WriteLine("Send {0} bytes to client: \"{1}\"",
        cbResponse, message.TrimEnd('\0'));

4. Flush the pipe to allow the client to read the pipe's contents before 
disconnecting. Then disconnect the client's connection.

    pipeServer.WaitForPipeDrain();
    pipeServer.Disconnect();

5. Close the server end of the pipe by calling NamedPipeServerStream.Close().

    pipeServer.Close();

-------------------------

B. Named pipe server by P/Invoke the native APIs related to named pipe 
operations. (NativeNamedPipeServer.Run())

1. Create a named pipe server by P/Invoking CreateNamedPipe and specifying 
the pipe name, pipe direction, options, transmission mode, security 
attributes, etc.

    // Create the named pipe.
    hNamedPipe = NativeMethod.CreateNamedPipe(
        Program.FULL_PIPE_NAME,             // The unique pipe name.
        PipeOpenMode.PIPE_ACCESS_DUPLEX,    // The pipe is duplex
        PipeMode.PIPE_TYPE_MESSAGE |        // Message type pipe 
        PipeMode.PIPE_READMODE_MESSAGE |    // Message-read mode 
        PipeMode.PIPE_WAIT,                 // Blocking mode is on
        PIPE_UNLIMITED_INSTANCES,           // Max server instances
        Program.BUFFER_SIZE,                // Output buffer size
        Program.BUFFER_SIZE,                // Input buffer size
        NMPWAIT_USE_DEFAULT_WAIT,           // Time-out interval
        sa                                  // Pipe security attributes
        );

In this code sample, the pipe server support message-based duplex 
communications. The security attributes of the pipe are customized to allow 
Authenticated Users read and write access to a pipe, and to allow the 
Administrators group full access to the pipe.

    // Define the SDDL for the security descriptor.
    string sddl = "D:" +        // Discretionary ACL
        "(A;OICI;GRGW;;;AU)" +  // Allow read/write to authenticated users
        "(A;OICI;GA;;;BA)";     // Allow full control to administrators

    SafeLocalMemHandle pSecurityDescriptor = null;
    if (!NativeMethod.ConvertStringSecurityDescriptorToSecurityDescriptor(
        sddl, 1, out pSecurityDescriptor, IntPtr.Zero))
    {
        throw new Win32Exception();
    }

    SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
    sa.nLength = Marshal.SizeOf(sa);
    sa.lpSecurityDescriptor = pSecurityDescriptor;
    sa.bInheritHandle = false;

2. Wait for the client to connect by calling ConnectNamedPipe.

    if (!NativeMethod.ConnectNamedPipe(hNamedPipe, IntPtr.Zero))
    {
        if (Marshal.GetLastWin32Error() != ERROR_PIPE_CONNECTED)
        {
            throw new Win32Exception();
        }
    }

3. Read the client's request from the pipe and write a response by calling 
ReadFile and WriteFile.

    // 
    // Receive a request from client.
    // 

    string message;
    bool finishRead = false;
    do
    {
        byte[] bRequest = new byte[Program.BUFFER_SIZE];
        int cbRequest = bRequest.Length, cbRead;

        finishRead = NativeMethod.ReadFile(
            hNamedPipe,             // Handle of the pipe
            bRequest,               // Buffer to receive data
            cbRequest,              // Size of buffer in bytes
            out cbRead,             // Number of bytes read 
            IntPtr.Zero             // Not overlapped 
            );

        if (!finishRead &&
            Marshal.GetLastWin32Error() != ERROR_MORE_DATA)
        {
            throw new Win32Exception();
        }

        // Unicode-encode the received byte array and trim all the 
        // '\0' characters at the end.
        message = Encoding.Unicode.GetString(bRequest).TrimEnd('\0');
        Console.WriteLine("Receive {0} bytes from client: \"{1}\"",
            cbRead, message);
    }
    while (!finishRead);  // Repeat loop if ERROR_MORE_DATA

    // 
    // Send a response from server to client.
    // 

    message = Program.RESPONSE_MESSAGE;
    byte[] bResponse = Encoding.Unicode.GetBytes(message);
    int cbResponse = bResponse.Length, cbWritten;

    if (!NativeMethod.WriteFile(
        hNamedPipe,                 // Handle of the pipe
        bResponse,                  // Message to be written
        cbResponse,                 // Number of bytes to write
        out cbWritten,              // Number of bytes written
        IntPtr.Zero                 // Not overlapped
        ))
    {
        throw new Win32Exception();
    }

    Console.WriteLine("Send {0} bytes to client: \"{1}\"",
        cbWritten, message.TrimEnd('\0'));


4. Flush the pipe to allow the client to read the pipe's contents before 
disconnecting. Then disconnect the client's connection.

    NativeMethod.FlushFileBuffers(hNamedPipe);
    NativeMethod.DisconnectNamedPipe(hNamedPipe);

5. Close the server end of the pipe.

    hNamedPipe.Close();


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: System.IO.Pipes Namespace
http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx

MSDN: NamedPipeServerStream
http://msdn.microsoft.com/en-us/library/system.io.pipes.namedpipeserverstream.aspx

How to: Use Named Pipes to Communicate Between Processes over a Network
http://msdn.microsoft.com/en-us/library/bb546085.aspx

Introducing Pipes [Justin Van Patten]
http://blogs.msdn.com/bclteam/archive/2006/12/07/introducing-pipes-justin-van-patten.aspx


/////////////////////////////////////////////////////////////////////////////