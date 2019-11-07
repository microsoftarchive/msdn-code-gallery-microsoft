/****************************** Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSNamedPipeClient
* Copyright (c) Microsoft Corporation.
* 
* Named pipe is a mechanism for one-way or duplex inter-process communication
*  between the pipe server and one or more pipe clients in the
* local machine or across the computers in the intranet:
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
* This code sample demonstrate two methods to use named pipe in Visual C#.
* 
* 1. Use the System.IO.Pipes namespace
* 
* The System.IO.Pipes namespace contains types that provide a means for 
* interprocess communication through anonymous and/or named pipes. 
* http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx
* These classes make the programming of named pipe in .NET much easier and 
* safer than P/Invoking native APIs directly. However, the System.IO.Pipes 
* namespace is not available before .NET Framework 3.5. So, if you are 
* programming against .NET Framework 2.0, you have to P/Invoke native APIs 
* to use named pipe.
* 
* The sample code in SystemIONamedPipeClient.Run() uses the 
* Systen.IO.Pipes.NamedPipeClientStream class to connect to the named pipe 
* "\\.\pipe\SamplePipe" to perform message-based duplex communication. The
* client then writes a message to the pipe and receives the response from the 
* pipe server.
* 
* 2. P/Invoke the native APIs related to named pipe operations.
* 
* The .NET interop services make it possible to call native APIs related to 
* named pipe operations from .NET. The sample code in 
* NativeNamedPipeClient.Run() demonstrates calling CreateFile to connect to 
* the named pipe "\\.\pipe\SamplePipe" with the GENERIC_READ and 
* GENERIC_WRITE accesses, and calling WriteFile and ReadFile to write a 
* message to the pipe and receive the response from the pipe server. Please 
* note that if you are programming against .NET Framework 3.5 or any newer 
* releases of .NET Framework, it is safer and easier to use the types in the 
* System.IO.Pipes namespace to operate named pipes.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;


namespace CSNamedPipeClient
{
    class Program
    {
        // The full name of the pipe in the format of 
        // \\servername\pipe\pipename.
        internal const string ServerName = ".";
        internal const string PipeName = "SamplePipe";
        internal const string FullPipeName = @"\\" + ServerName + @"\pipe\" + PipeName;

        internal const int BufferSize = 1024;

        // Request message from client to server. '\0' is appended in the end 
        // because the client may be a native C++ application that expects 
        // NULL termiated string.
        public const string RequestMessage = "Default request from client\0";


        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "-native")
            {
                // If the command line is "CSNamedPipeClient.exe -native", 
                // P/Invoke the native APIs related to named pipe operations 
                // to connect to the named pipe.
                NativeNamedPipeClient.Run();
            }
            else
            {
                // Use the types in the System.IO.Pipes namespace to connect 
                // to the named pipe. This solution is recommended when you 
                // program against .NET Framework 3.5 or any newer releases 
                // of .NET Framework.
                SystemIONamedPipeClient.Run();
            }
        }
    }
}