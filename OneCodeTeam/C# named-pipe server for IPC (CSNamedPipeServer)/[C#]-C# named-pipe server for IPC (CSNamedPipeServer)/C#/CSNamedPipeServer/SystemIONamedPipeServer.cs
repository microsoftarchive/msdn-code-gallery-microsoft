/******************************** Module Header ********************************\
* Module Name:  SystemIONamedPipeServer.cs
* Project:      CSNamedPipeServer
* Copyright (c) Microsoft Corporation.
* 
* The System.IO.Pipes namespace contains types that provide a means for 
* interprocess communication through anonymous and/or named pipes. 
* http://msdn.microsoft.com/en-us/library/system.io.pipes.aspx
* These classes make the programming of named pipe in .NET much easier and safer 
* than P/Invoking native APIs directly. However, the System.IO.Pipes namespace
* is not available before .NET Framework 3.5. So, if you are programming against 
* .NET Framework 2.0, you have to P/Invoke native APIs to use named pipe.
* 
* The sample code in SystemIONamedPipeServer.Run() uses the 
* Systen.IO.Pipes.NamedPipeServerStream class to create a pipe that is named 
* "\\.\pipe\SamplePipe" to perform message-based communication. The pipe supports 
* duplex connections, so both client and server can read from and write to the 
* pipe. The security attributes of the pipe are customized to allow Authenticated
* Users read and write access to a pipe, and to allow the Administrators group 
* full access to the pipe. When the pipe is connected by a client, the server 
* attempts to read the client's message from the pipe, and write a response.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#region Using directives
using System;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
#endregion


namespace CSNamedPipeServer
{
    class SystemIONamedPipeServer
    {
        /// <summary>
        /// Use the pipe classes in the System.IO.Pipes namespace to create the 
        /// named pipe. This solution is recommended.
        /// </summary>
        public static void Run()
        {
            NamedPipeServerStream pipeServer = null;

            try
            {
                // Prepare the security attributes (the pipeSecurity parameter in 
                // the constructor of NamedPipeServerStream) for the pipe. This 
                // is optional. If pipeSecurity of NamedPipeServerStream is null, 
                // the named pipe gets a default security descriptor.and the 
                // handle cannot be inherited. The ACLs in the default security 
                // descriptor of a pipe grant full control to the LocalSystem 
                // account, (elevated) administrators, and the creator owner. 
                // They also give only read access to members of the Everyone 
                // group and the anonymous account. However, if you want to 
                // customize the security permission of the pipe, (e.g. to allow 
                // Authenticated Users to read from and write to the pipe), you 
                // need to create a PipeSecurity object.
                PipeSecurity pipeSecurity = null;
                pipeSecurity = CreateSystemIOPipeSecurity();

                // Create the named pipe.
                pipeServer = new NamedPipeServerStream(
                    Program.PipeName,               // The unique pipe name.
                    PipeDirection.InOut,            // The pipe is duplex
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message,   // Message-based communication
                    PipeOptions.None,               // No additional parameters
                    Program.BufferSize,             // Input buffer size
                    Program.BufferSize,             // Output buffer size
                    pipeSecurity,                   // Pipe security attributes
                    HandleInheritability.None       // Not inheritable
                    );

                Console.WriteLine("The named pipe ({0}) is created.", 
                    Program.FullPipeName);

                // Wait for the client to connect.
                Console.WriteLine("Waiting for the client's connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Client is connected.");

                // 
                // Receive a request from client.
                // 
                // Note: The named pipe was created to support message-based
                // communication. This allows a reading process to read 
                // varying-length messages precisely as sent by the writing 
                // process. In this mode you should not use StreamWriter to write 
                // the pipe, or use StreamReader to read the pipe. You can read 
                // more about the difference from the article:
                // http://go.microsoft.com/?linkid=9721786.
                // 

                string message;
                do
                {
                    byte[] bRequest = new byte[Program.BufferSize];
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

                message = Program.ResponseMessage;
                byte[] bResponse = Encoding.Unicode.GetBytes(message);
                int cbResponse = bResponse.Length;

                pipeServer.Write(bResponse, 0, cbResponse);

                Console.WriteLine("Send {0} bytes to client: \"{1}\"",
                    cbResponse, message.TrimEnd('\0'));

                // Flush the pipe to allow the client to read the pipe's contents 
                // before disconnecting. Then disconnect the client's connection.
                pipeServer.WaitForPipeDrain();
                pipeServer.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("The server throws the error: {0}", ex.Message);
            }
            finally
            {
                if (pipeServer != null)
                {
                    pipeServer.Close();
                    pipeServer = null;
                }
            }
        }


        /// <summary>
        /// The CreateSystemIOPipeSecurity function creates a new PipeSecurity 
        /// object to allow Authenticated Users read and write access to a pipe, 
        /// and to allow the Administrators group full access to the pipe.
        /// </summary>
        /// <returns>
        /// A PipeSecurity object that allows Authenticated Users read and write 
        /// access to a pipe, and allows the Administrators group full access to 
        /// the pipe.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa365600(VS.85).aspx"/>
        static PipeSecurity CreateSystemIOPipeSecurity()
        {
            PipeSecurity pipeSecurity = new PipeSecurity();

            // Allow Everyone read and write access to the pipe.
            pipeSecurity.SetAccessRule(new PipeAccessRule("Authenticated Users",
                PipeAccessRights.ReadWrite, AccessControlType.Allow));

            // Allow the Administrators group full access to the pipe.
            pipeSecurity.SetAccessRule(new PipeAccessRule("Administrators",
                PipeAccessRights.FullControl, AccessControlType.Allow));

            return pipeSecurity;
        }
    }
}
