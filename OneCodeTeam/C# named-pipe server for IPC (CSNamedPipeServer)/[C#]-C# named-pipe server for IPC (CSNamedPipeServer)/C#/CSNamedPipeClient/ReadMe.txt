========================================================================
    CONSOLE APPLICATION : CSNamedPipeClient Project Overview
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

This code sample demonstrate two methods to use named pipe in Visual C#.

1. Use the System.IO.Pipes namespace

The System.IO.Pipes namespace contains types that provide a means for 
interprocess communication through anonymous and/or named pipes. 
http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx
These classes make the programming of named pipe in .NET much easier and 
safer than P/Invoking native APIs directly. However, the System.IO.Pipes 
namespace is not available before .NET Framework 3.5. So, if you are 
programming against .NET Framework 2.0, you have to P/Invoke native APIs 
to use named pipe.

The sample code in SystemIONamedPipeClient.Run() uses the 
Systen.IO.Pipes.NamedPipeClientStream class to connect to the named pipe 
"\\.\pipe\SamplePipe" to perform message-based duplex communication. The 
client then writes a message to the pipe and receives the response from the 
pipe server.

2. P/Invoke the native APIs related to named pipe operations.

The .NET interop services make it possible to call native APIs related to 
named pipe operations from .NET. The sample code in 
NativeNamedPipeClient.Run() demonstrates calling CreateFile to connect to 
the named pipe "\\.\pipe\SamplePipe" with the GENERIC_READ and 
GENERIC_WRITE accesses, and calling WriteFile and ReadFile to write a 
message to the pipe and receive the response from the pipe server. Please 
note that if you are programming against .NET Framework 3.5 or any newer 
releases of .NET Framework, it is safer and easier to use the types in the 
System.IO.Pipes namespace to operate named pipes.


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

CSNamedPipeClient -> CSNamedPipeServer/VBNamedPipeServer/CppNamedPipeServer
CSNamedPipeClient is the client end of the named pipe. CSNamedPipeServer, 
VBNamedPipeServer and CppNamedPipeServer can be the server ends that create 
the named pipe.

CSNamedPipeClient - VBNamedPipeClient - CppNamedPipeClient
CSNamedPipeClient, VBNamedPipeClient and CppNamedPipeServer are the same 
named pipe client ends written in three different programming languages.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

A. Named pipe client by using the System.IO.Pipes namespace. 
(SystemIONamedPipeClient.Run())

1. Create a NamedPipeClientStream object and specify the pipe server, name, 
pipe direction, options, etc. 

    pipeClient = new NamedPipeClientStream(
        Program.SERVER_NAME,        // The server name
        Program.PIPE_NAME,          // The unique pipe name
        PipeDirection.InOut,        // The pipe is duplex
        PipeOptions.None            // No additional parameters
        );

2. Connect to the named pipe by calling NamedPipeClientStream.Connect().

    pipeClient.Connect(5000);

3. Set the read mode and the blocking mode of the named pipe. In this sample, 
we set data to be read from the pipe as a stream of messages. This allows a 
reading process to read varying-length messages precisely as sent by the 
writing process. In this mode, you should not use StreamWriter to write the 
pipe, or use StreamReader to read the pipe. You can read more about the 
difference from http://go.microsoft.com/?linkid=9721786.

    pipeClient.ReadMode = PipeTransmissionMode.Message;

4. Send a message to the pipe server and receive its response through 
NamedPipeClientStream.Read and NamedPipeClientStream.Write. Because 
pipeClient.ReadMode = PipeTransmissionMode.Message, you should not use 
StreamWriter to write the pipe, or use StreamReader to read the pipe.

    // 
    // Send a request from client to server
    // 

    string message = Program.REQUEST_MESSAGE;
    byte[] bRequest = Encoding.Unicode.GetBytes(message);
    int cbRequest = bRequest.Length;

    pipeClient.Write(bRequest, 0, cbRequest);

    Console.WriteLine("Send {0} bytes to server: \"{1}\"",
        cbRequest, message.TrimEnd('\0'));

    //
    // Receive a response from server.
    // 

    do
    {
        byte[] bResponse = new byte[Program.BUFFER_SIZE];
        int cbResponse = bResponse.Length, cbRead;

        cbRead = pipeClient.Read(bResponse, 0, cbResponse);

        // Unicode-encode the received byte array and trim all the 
        // '\0' characters at the end.
        message = Encoding.Unicode.GetString(bResponse).TrimEnd('\0');
        Console.WriteLine("Receive {0} bytes from server: \"{1}\"",
            cbRead, message);
    }
    while (!pipeClient.IsMessageComplete);

5. Close the client end of the pipe by calling NamedPipeClientStream.Close().

    pipeClient.Close();

-------------------------

B. Named pipe client by P/Invoke the native APIs related to named pipe 
operations. (NativeNamedPipeClient.Run())

1. Try to connect to a named pipe by P/Invoking CreateFile and specifying the 
target pipe server, name, desired access, etc. 

    hNamedPipe = NativeMethod.CreateFile(
        Program.FULL_PIPE_NAME,                 // Pipe name
        FileDesiredAccess.GENERIC_READ |        // Read access
        FileDesiredAccess.GENERIC_WRITE,        // Write access
        FileShareMode.Zero,                     // No sharing 
        null,                                   // Default security attributes
        FileCreationDisposition.OPEN_EXISTING,  // Opens existing pipe
        0,                                      // Default attributes
        IntPtr.Zero                             // No template file
        );

If all pipe instances are busy, wait for 5 seconds and connect again.

    if (!NativeMethod.WaitNamedPipe(Program.FULL_PIPE_NAME, 5000))
    {
        throw new Win32Exception();
    }

2. Set the read mode and the blocking mode of the named pipe. In this sample, 
we set data to be read from the pipe as a stream of messages.

    PipeMode mode = PipeMode.PIPE_READMODE_MESSAGE;
    if (!NativeMethod.SetNamedPipeHandleState(hNamedPipe, ref mode,
        IntPtr.Zero, IntPtr.Zero))
    {
        throw new Win32Exception();
    }

3. Send a message to the pipe server and receive its response by calling 
the WriteFile and ReadFile functions.

    // 
    // Send a request from client to server
    // 

    string message = Program.REQUEST_MESSAGE;
    byte[] bRequest = Encoding.Unicode.GetBytes(message);
    int cbRequest = bRequest.Length, cbWritten;

    if (!NativeMethod.WriteFile(
        hNamedPipe,     // Handle of the pipe
        bRequest,       // Message to be written
        cbRequest,      // Number of bytes to write
        out cbWritten,  // Number of bytes written
        IntPtr.Zero     // Not overlapped
        ))
    {
        throw new Win32Exception();
    }

    Console.WriteLine("Send {0} bytes to server: \"{1}\"", 
        cbWritten, message.TrimEnd('\0'));

    //
    // Receive a response from server.
    // 

    bool finishRead = false;
    do
    {
        byte[] bResponse = new byte[Program.BUFFER_SIZE];
        int cbResponse = bResponse.Length, cbRead;

        finishRead = NativeMethod.ReadFile(
            hNamedPipe,             // Handle of the pipe
            bResponse,              // Buffer to receive data
            cbResponse,             // Size of buffer in bytes
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
        message = Encoding.Unicode.GetString(bResponse).TrimEnd('\0');
        Console.WriteLine("Receive {0} bytes from server: \"{1}\"",
            cbRead, message);
    }
    while (!finishRead);  // Repeat loop if ERROR_MORE_DATA

4. Close the pipe.

    hNamedPipe.Close();


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: System.IO.Pipes Namespace
http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx

MSDN: NamedPipeClientStream
http://msdn.microsoft.com/en-us/library/system.io.pipes.namedpipeclientstream.aspx

How to: Use Named Pipes to Communicate Between Processes over a Network
http://msdn.microsoft.com/en-us/library/bb546085.aspx

Introducing Pipes [Justin Van Patten]
http://blogs.msdn.com/bclteam/archive/2006/12/07/introducing-pipes-justin-van-patten.aspx


/////////////////////////////////////////////////////////////////////////////