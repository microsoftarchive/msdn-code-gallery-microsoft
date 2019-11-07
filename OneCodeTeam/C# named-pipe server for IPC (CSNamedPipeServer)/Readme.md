# C# named-pipe server for IPC (CSNamedPipeServer)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- IPC and RPC
- Windows General
## Topics
- Named Pipe
- Inter-process Communication
## Updated
- 02/19/2013
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>CONSOLE APPLICATION : CSNamedPipeServer Project Overview</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
Named pipe is a mechanism for one-way or duplex inter-process communication <br>
between the pipe server and one or more pipe clients in the local machine or <br>
across the computers in the intranet:<br>
<br>
Client (GENERIC_WRITE) ---&gt; Server (GENERIC_READ)<br>
<br>
PIPE_ACCESS_OUTBOUND:<br>
Client (GENERIC_READ) &lt;--- Server (GENERIC_WRITE)<br>
<br>
PIPE_ACCESS_DUPLEX:<br>
Client (GENERIC_READ or GENERIC_WRITE, or both) &lt;--&gt; <br>
Server (GENERIC_READ and GENERIC_WRITE)<br>
<br>
This code sample demonstrate two methods to create named pipe in Visual C#.<br>
<br>
1. Use the System.IO.Pipes namespace<br>
<br>
The System.IO.Pipes namespace contains types that provide a means for <br>
interprocess communication through anonymous and/or named pipes. <br>
<a href="&lt;a target=" target="_blank">http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx</a>'&gt;<a href="http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx</a><br>
These classes make the programming of named pipe in .NET much easier and <br>
safer than P/Invoking native APIs directly. However, the System.IO.Pipes <br>
namespace is not available before .NET Framework 3.5. So, if you are <br>
programming against .NET Framework 2.0, you have to P/Invoke native APIs <br>
to use named pipe.<br>
<br>
The sample code in SystemIONamedPipeServer.Run() uses the <br>
Systen.IO.Pipes.NamedPipeServerStream class to create a pipe that is named <br>
&quot;\\.\pipe\SamplePipe&quot; to perform message-based communication. The pipe <br>
supports duplex connections, so both client and server can read from and <br>
write to the pipe. The security attributes of the pipe are customized to <br>
allow Authenticated Users read and write access to a pipe, and to allow the <br>
Administrators group full access to the pipe. When the pipe is connected by a <br>
client, the server attempts to read the client's message from the pipe, and <br>
write a response.<br>
<br>
2. P/Invoke the native APIs related to named pipe operations.<br>
<br>
The .NET interop services make it possible to call native APIs related to <br>
named pipe operations from .NET. The sample code in <br>
NativeNamedPipeServer.Run() demonstrates calling CreateNamedPipe to create <br>
a pipe named &quot;\\.\pipe\SamplePipe&quot;, which supports duplex connections so that <br>
both client and server can read from and write to the pipe. The security <br>
attributes of the pipe are customized to allow Authenticated Users read and <br>
write access to a pipe, and to allow the Administrators group full access to <br>
the pipe. When the pipe is connected by a client, the server attempts to read <br>
the client's message from the pipe by calling ReadFile, and write a response <br>
by calling WriteFile. Please note that if you are programming against .NET <br>
Framework 3.5 or any newer releases of .NET Framework, it is safer and easier <br>
to use the types in the System.IO.Pipes namespace to operate named pipes.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the named pipe sample.<br>
<br>
Step1. After you successfully build the CSNamedPipeClient and <br>
CSNamedPipeServer sample projects in Visual Studio 2008, you will get the <br>
applications: CSNamedPipeClient.exe and CSNamedPipeServer.exe. <br>
<br>
Step2. Run CSNamedPipeServer.exe in a command prompt to start up the server <br>
end of the named pipe. If the command is &quot;CSNamedPipeServer.exe&quot;, the pipe <br>
server is created by the types in the System.IO.Pipes namespace. If the <br>
command is &quot;CSNamedPipeServer.exe -native&quot;, the pipe server is created by <br>
P/Invoking the native APIs related to named pipe operations. In both cases, <br>
the application outputs the following information in the command prompt if <br>
the pipe is created successfully.<br>
<br>
Server:<br>
&nbsp;The named pipe (\\.\pipe\SamplePipe) is created.<br>
&nbsp;Waiting for the client's connection...<br>
<br>
Step3. Run CSNamedPipeClient.exe in another command prompt to start up the <br>
client end of the named pipe. If the command is &quot;CSNamedPipeClient.exe&quot;, the <br>
client connects to the pipe by using the types in the System.IO.Pipes <br>
namespace. If the command is &quot;CSNamedPipeClient.exe -native&quot;, the client <br>
connects to the pipe by P/Invoking the native APIs related to named pipe <br>
operations. In both cases, the application should output the message below <br>
in the command prompt when the client successfully connects to the named pipe.<br>
<br>
Client:<br>
&nbsp;The named pipe (\\.\pipe\SamplePipe) is connected.<br>
<br>
In the meantime, the server application outputs this message to indicate that <br>
the pipe is connected by a client.<br>
<br>
Server:<br>
&nbsp;Client is connected.<br>
<br>
Step4. The client attempts to write a message to the named pipe. You will see <br>
the message below in the commond prompt running the client application.<br>
<br>
Client:<br>
&nbsp;Send 56 bytes to server: &quot;Default request from client&quot;<br>
<br>
When the server application reads the message from the client, it prints:<br>
<br>
Server:<br>
&nbsp;Receive 56 bytes from client: &quot;Default request from client&quot;<br>
<br>
Next, the server writes a response to the pipe.<br>
<br>
Server:<br>
&nbsp;Send 58 bytes to client: &quot;Default response from server&quot;<br>
<br>
And the client receives the response:<br>
<br>
Client:<br>
&nbsp;Receive 58 bytes from server: &quot;Default response from server&quot;<br>
<br>
The connection is disconnected and the pipe is closed after that.<br>
<br>
</p>
<h3>Sample Relation:</h3>
<p style="font-family:Courier New">(The relationship between the current sample and the rest samples in
<br>
Microsoft All-In-One Code Framework <a href="http://1code.codeplex.com)" target="_blank">
http://1code.codeplex.com)</a><br>
<br>
CSNamedPipeClient/VBNamedPipeClient/CppNamedPipeClient -&gt; CSNamedPipeServer<br>
CSNamedPipeServer is the server end of the named pipe. CSNamedPipeClient, <br>
VBNamedPipeClient and CppNamedPipeClient can be the client ends that connect<br>
to the named pipe.<br>
<br>
CSNamedPipeServer - VBNamedPipeServer - CppNamedPipeServer<br>
CSNamedPipeServer, VBNamedPipeServer and CppNamedPipeServer are the same <br>
named pipe server ends written in three different programming languages.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
A. Named pipe server by using the System.IO.Pipes namespace. <br>
(SystemIONamedPipeServer.Run())<br>
<br>
1. Create a named pipe server (System.IO.Pipes.NamedPipeServerStream) object <br>
and specify pipe name, pipe direction, options, transmission mode, security <br>
attributes, etc.<br>
<br>
&nbsp; &nbsp;// Create the named pipe.<br>
&nbsp; &nbsp;pipeServer = new NamedPipeServerStream(<br>
&nbsp; &nbsp; &nbsp; &nbsp;Program.PIPE_NAME, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// The unique pipe name.<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeDirection.InOut, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// The pipe is duplex<br>
&nbsp; &nbsp; &nbsp; &nbsp;NamedPipeServerStream.MaxAllowedServerInstances,<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeTransmissionMode.Message, &nbsp; // Message-based communication<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeOptions.None, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // No additional parameters<br>
&nbsp; &nbsp; &nbsp; &nbsp;Program.BUFFER_SIZE, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Input buffer size<br>
&nbsp; &nbsp; &nbsp; &nbsp;Program.BUFFER_SIZE, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Output buffer size<br>
&nbsp; &nbsp; &nbsp; &nbsp;pipeSecurity, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Pipe security attributes<br>
&nbsp; &nbsp; &nbsp; &nbsp;HandleInheritability.None &nbsp; &nbsp; &nbsp; // Not inheritable<br>
&nbsp; &nbsp; &nbsp; &nbsp;);<br>
<br>
In this code sample, the pipe server support message-based duplex <br>
communications. The security attributes of the pipe are customized to allow <br>
Authenticated Users read and write access to a pipe, and to allow the <br>
Administrators group full access to the pipe.<br>
<br>
&nbsp; &nbsp;PipeSecurity pipeSecurity = new PipeSecurity();<br>
<br>
&nbsp; &nbsp;// Allow Everyone read and write access to the pipe.<br>
&nbsp; &nbsp;pipeSecurity.SetAccessRule(new PipeAccessRule(&quot;Authenticated Users&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeAccessRights.ReadWrite, AccessControlType.Allow));<br>
<br>
&nbsp; &nbsp;// Allow the Administrators group full access to the pipe.<br>
&nbsp; &nbsp;pipeSecurity.SetAccessRule(new PipeAccessRule(&quot;Administrators&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeAccessRights.FullControl, AccessControlType.Allow));<br>
<br>
2. Wait for the client to connect by calling <br>
NamedPipeServerStream.WaitForConnection().<br>
<br>
&nbsp; &nbsp;pipeServer.WaitForConnection();<br>
<br>
3. Read the client's request from the pipe and write a response by calling <br>
NamedPipeServerStream.Read and NamedPipeServerStream.Write. The named pipe <br>
was created to support message-based communication. This allows a reading <br>
process to read varying-length messages precisely as sent by the writing <br>
process. In this mode you should not use StreamWriter to write the pipe, or <br>
use StreamReader to read the pipe. You can read more about the difference <br>
from the article: <a href="http://go.microsoft.com/?linkid=9721786." target="_blank">
http://go.microsoft.com/?linkid=9721786.</a><br>
<br>
&nbsp; &nbsp;// <br>
&nbsp; &nbsp;// Receive a request from client.<br>
&nbsp; &nbsp;// <br>
&nbsp; &nbsp;string message;<br>
&nbsp; &nbsp;do<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;byte[] bRequest = new byte[Program.BUFFER_SIZE];<br>
&nbsp; &nbsp; &nbsp; &nbsp;int cbRequest = bRequest.Length, cbRead;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;cbRead = pipeServer.Read(bRequest, 0, cbRequest);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Unicode-encode the received byte array and trim all the
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// '\0' characters at the end.<br>
&nbsp; &nbsp; &nbsp; &nbsp;message = Encoding.Unicode.GetString(bRequest).TrimEnd('\0');<br>
&nbsp; &nbsp; &nbsp; &nbsp;Console.WriteLine(&quot;Receive {0} bytes from client: \&quot;{1}\&quot;&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;cbRead, message);<br>
&nbsp; &nbsp;}<br>
&nbsp; &nbsp;while (!pipeServer.IsMessageComplete);<br>
<br>
&nbsp; &nbsp;// <br>
&nbsp; &nbsp;// Send a response from server to client.<br>
&nbsp; &nbsp;// <br>
<br>
&nbsp; &nbsp;message = Program.RESPONSE_MESSAGE;<br>
&nbsp; &nbsp;byte[] bResponse = Encoding.Unicode.GetBytes(message);<br>
&nbsp; &nbsp;int cbResponse = bResponse.Length;<br>
<br>
&nbsp; &nbsp;pipeServer.Write(bResponse, 0, cbResponse);<br>
<br>
&nbsp; &nbsp;Console.WriteLine(&quot;Send {0} bytes to client: \&quot;{1}\&quot;&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp;cbResponse, message.TrimEnd('\0'));<br>
<br>
4. Flush the pipe to allow the client to read the pipe's contents before <br>
disconnecting. Then disconnect the client's connection.<br>
<br>
&nbsp; &nbsp;pipeServer.WaitForPipeDrain();<br>
&nbsp; &nbsp;pipeServer.Disconnect();<br>
<br>
5. Close the server end of the pipe by calling NamedPipeServerStream.Close().<br>
<br>
&nbsp; &nbsp;pipeServer.Close();<br>
<br>
-------------------------<br>
<br>
B. Named pipe server by P/Invoke the native APIs related to named pipe <br>
operations. (NativeNamedPipeServer.Run())<br>
<br>
1. Create a named pipe server by P/Invoking CreateNamedPipe and specifying <br>
the pipe name, pipe direction, options, transmission mode, security <br>
attributes, etc.<br>
<br>
&nbsp; &nbsp;// Create the named pipe.<br>
&nbsp; &nbsp;hNamedPipe = NativeMethod.CreateNamedPipe(<br>
&nbsp; &nbsp; &nbsp; &nbsp;Program.FULL_PIPE_NAME, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // The unique pipe name.<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeOpenMode.PIPE_ACCESS_DUPLEX, &nbsp; &nbsp;// The pipe is duplex<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeMode.PIPE_TYPE_MESSAGE | &nbsp; &nbsp; &nbsp; &nbsp;// Message type pipe
<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeMode.PIPE_READMODE_MESSAGE | &nbsp; &nbsp;// Message-read mode
<br>
&nbsp; &nbsp; &nbsp; &nbsp;PipeMode.PIPE_WAIT, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Blocking mode is on<br>
&nbsp; &nbsp; &nbsp; &nbsp;PIPE_UNLIMITED_INSTANCES, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Max server instances<br>
&nbsp; &nbsp; &nbsp; &nbsp;Program.BUFFER_SIZE, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Output buffer size<br>
&nbsp; &nbsp; &nbsp; &nbsp;Program.BUFFER_SIZE, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Input buffer size<br>
&nbsp; &nbsp; &nbsp; &nbsp;NMPWAIT_USE_DEFAULT_WAIT, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Time-out interval<br>
&nbsp; &nbsp; &nbsp; &nbsp;sa &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Pipe security attributes<br>
&nbsp; &nbsp; &nbsp; &nbsp;);<br>
<br>
In this code sample, the pipe server support message-based duplex <br>
communications. The security attributes of the pipe are customized to allow <br>
Authenticated Users read and write access to a pipe, and to allow the <br>
Administrators group full access to the pipe.<br>
<br>
&nbsp; &nbsp;// Define the SDDL for the security descriptor.<br>
&nbsp; &nbsp;string sddl = &quot;D:&quot; &#43; &nbsp; &nbsp; &nbsp; &nbsp;// Discretionary ACL<br>
&nbsp; &nbsp; &nbsp; &nbsp;&quot;(A;OICI;GRGW;;;AU)&quot; &#43; &nbsp;// Allow read/write to authenticated users<br>
&nbsp; &nbsp; &nbsp; &nbsp;&quot;(A;OICI;GA;;;BA)&quot;; &nbsp; &nbsp; // Allow full control to administrators<br>
<br>
&nbsp; &nbsp;SafeLocalMemHandle pSecurityDescriptor = null;<br>
&nbsp; &nbsp;if (!NativeMethod.ConvertStringSecurityDescriptorToSecurityDescriptor(<br>
&nbsp; &nbsp; &nbsp; &nbsp;sddl, 1, out pSecurityDescriptor, IntPtr.Zero))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;throw new Win32Exception();<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();<br>
&nbsp; &nbsp;sa.nLength = Marshal.SizeOf(sa);<br>
&nbsp; &nbsp;sa.lpSecurityDescriptor = pSecurityDescriptor;<br>
&nbsp; &nbsp;sa.bInheritHandle = false;<br>
<br>
2. Wait for the client to connect by calling ConnectNamedPipe.<br>
<br>
&nbsp; &nbsp;if (!NativeMethod.ConnectNamedPipe(hNamedPipe, IntPtr.Zero))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (Marshal.GetLastWin32Error() != ERROR_PIPE_CONNECTED)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;throw new Win32Exception();<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp;}<br>
<br>
3. Read the client's request from the pipe and write a response by calling <br>
ReadFile and WriteFile.<br>
<br>
&nbsp; &nbsp;// <br>
&nbsp; &nbsp;// Receive a request from client.<br>
&nbsp; &nbsp;// <br>
<br>
&nbsp; &nbsp;string message;<br>
&nbsp; &nbsp;bool finishRead = false;<br>
&nbsp; &nbsp;do<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;byte[] bRequest = new byte[Program.BUFFER_SIZE];<br>
&nbsp; &nbsp; &nbsp; &nbsp;int cbRequest = bRequest.Length, cbRead;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;finishRead = NativeMethod.ReadFile(<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;hNamedPipe, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Handle of the pipe<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;bRequest, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Buffer to receive data<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;cbRequest, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Size of buffer in bytes<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;out cbRead, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Number of bytes read
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;IntPtr.Zero &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Not overlapped
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (!finishRead &amp;&amp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Marshal.GetLastWin32Error() != ERROR_MORE_DATA)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;throw new Win32Exception();<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Unicode-encode the received byte array and trim all the
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// '\0' characters at the end.<br>
&nbsp; &nbsp; &nbsp; &nbsp;message = Encoding.Unicode.GetString(bRequest).TrimEnd('\0');<br>
&nbsp; &nbsp; &nbsp; &nbsp;Console.WriteLine(&quot;Receive {0} bytes from client: \&quot;{1}\&quot;&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;cbRead, message);<br>
&nbsp; &nbsp;}<br>
&nbsp; &nbsp;while (!finishRead); &nbsp;// Repeat loop if ERROR_MORE_DATA<br>
<br>
&nbsp; &nbsp;// <br>
&nbsp; &nbsp;// Send a response from server to client.<br>
&nbsp; &nbsp;// <br>
<br>
&nbsp; &nbsp;message = Program.RESPONSE_MESSAGE;<br>
&nbsp; &nbsp;byte[] bResponse = Encoding.Unicode.GetBytes(message);<br>
&nbsp; &nbsp;int cbResponse = bResponse.Length, cbWritten;<br>
<br>
&nbsp; &nbsp;if (!NativeMethod.WriteFile(<br>
&nbsp; &nbsp; &nbsp; &nbsp;hNamedPipe, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Handle of the pipe<br>
&nbsp; &nbsp; &nbsp; &nbsp;bResponse, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Message to be written<br>
&nbsp; &nbsp; &nbsp; &nbsp;cbResponse, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Number of bytes to write<br>
&nbsp; &nbsp; &nbsp; &nbsp;out cbWritten, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Number of bytes written<br>
&nbsp; &nbsp; &nbsp; &nbsp;IntPtr.Zero &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; // Not overlapped<br>
&nbsp; &nbsp; &nbsp; &nbsp;))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;throw new Win32Exception();<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;Console.WriteLine(&quot;Send {0} bytes to client: \&quot;{1}\&quot;&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp;cbWritten, message.TrimEnd('\0'));<br>
<br>
<br>
4. Flush the pipe to allow the client to read the pipe's contents before <br>
disconnecting. Then disconnect the client's connection.<br>
<br>
&nbsp; &nbsp;NativeMethod.FlushFileBuffers(hNamedPipe);<br>
&nbsp; &nbsp;NativeMethod.DisconnectNamedPipe(hNamedPipe);<br>
<br>
5. Close the server end of the pipe.<br>
<br>
&nbsp; &nbsp;hNamedPipe.Close();<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: System.IO.Pipes Namespace<br>
<a href="&lt;a target=" target="_blank">http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx</a>'&gt;<a href="http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx</a><br>
<br>
MSDN: NamedPipeServerStream<br>
<a href="http://msdn.microsoft.com/en-us/library/system.io.pipes.namedpipeserverstream.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/system.io.pipes.namedpipeserverstream.aspx</a><br>
<br>
How to: Use Named Pipes to Communicate Between Processes over a Network<br>
<a href="http://msdn.microsoft.com/en-us/library/bb546085.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/bb546085.aspx</a><br>
<br>
Introducing Pipes [Justin Van Patten]<br>
<a href="http://blogs.msdn.com/bclteam/archive/2006/12/07/introducing-pipes-justin-van-patten.aspx" target="_blank">http://blogs.msdn.com/bclteam/archive/2006/12/07/introducing-pipes-justin-van-patten.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
