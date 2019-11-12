/********************************** Module Header **********************************\
Module Name:  Program.cs
Project:      CSMailslotClient
Copyright (c) Microsoft Corporation.

Mailslot is a mechanism for one-way inter-process communication in the local machine
or across the computers in the intranet. Any clients can store messages in a mailslot. 
The creator of the slot, i.e. the server, retrieves the messages that are stored 
there:

Client (GENERIC_WRITE) ---> Server (GENERIC_READ)

This sample demonstrates a mailslot client that connects and writes to the mailslot 
"\\.\mailslot\SampleMailslot". 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

#region Using directives
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security;
using System.Security.Permissions;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
#endregion


namespace CSNamedPipeClient
{
    class Program
    {
        // The full name of the mailslot is in the format of \\.\mailslot\[path]name
        // The name field must be unique. The name may include multiple levels of 
        // pseudo directories separated by backslashes. For example, both 
        // \\.\mailslot\mailslot_name and \\.\mailslot\abc\def\ghi are valid.
        internal const string MailslotName = @"\\.\mailslot\SampleMailslot";


        static void Main(string[] args)
        {
            SafeMailslotHandle hMailslot = null;

            try
            {
                // Try to open the mailslot with the write access.
                hMailslot = NativeMethod.CreateFile(
                    MailslotName,                           // The name of the mailslot
                    FileDesiredAccess.GENERIC_WRITE,        // Write access 
                    FileShareMode.FILE_SHARE_READ,          // Share mode
                    IntPtr.Zero,                            // Default security attributes
                    FileCreationDisposition.OPEN_EXISTING,  // Opens existing mailslot
                    0,                                      // No other attributes set
                    IntPtr.Zero                             // No template file
                    );
                if (hMailslot.IsInvalid)
                {
                    throw new Win32Exception();
                }

                Console.WriteLine("The mailslot ({0}) is opened.", MailslotName);

                // Write messages to the mailslot.

                // Append '\0' at the end of each message considering the native C++ 
                // Mailslot server (CppMailslotServer).
                WriteMailslot(hMailslot, "Message 1 for mailslot\0");
                WriteMailslot(hMailslot, "Message 2 for mailslot\0");
                Thread.Sleep(3000); // Sleep for 3 seconds for the demo purpose
                WriteMailslot(hMailslot, "Message 3 for mailslot\0");

            }
            catch (Win32Exception ex)
            {
                Console.WriteLine("The client throws the error: {0}", ex.Message);
            }
            finally
            {
                if (hMailslot != null)
                {
                    hMailslot.Close();
                    hMailslot = null;
                }
            }
        }


        /// <summary>
        /// Write a message to the specified mailslot
        /// </summary>
        /// <param name="hMailslot">Handle to the mailslot</param>
        /// <param name="lpszMessage">The message to be written to the slot</param>
        static void WriteMailslot(SafeMailslotHandle hMailslot, string message)
        {
            int cbMessageBytes = 0;         // Message size in bytes
            int cbBytesWritten = 0;         // Number of bytes written to the slot

            byte[] bMessage = Encoding.Unicode.GetBytes(message);
            cbMessageBytes = bMessage.Length;

            bool succeeded = NativeMethod.WriteFile(
                hMailslot,                  // Handle to the mailslot
                bMessage,                   // Message to be written
                cbMessageBytes,             // Number of bytes to write
                out cbBytesWritten,         // Number of bytes written
                IntPtr.Zero                 // Not overlapped
                );
            if (!succeeded || cbMessageBytes != cbBytesWritten)
            {
                Console.WriteLine("WriteFile failed w/err 0x{0:X}", 
                    Marshal.GetLastWin32Error());
            }
            else
            {
                Console.WriteLine("The message \"{0}\" is written to the slot", 
                    message);
            }
        }


        #region Native API Signatures and Types

        /// <summary>
        /// Desired Access of File/Device
        /// </summary>
        [Flags]
        internal enum FileDesiredAccess : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000
        }

        /// <summary>
        /// File share mode
        /// </summary>
        [Flags]
        internal enum FileShareMode : uint
        {
            Zero = 0x00000000,  // No sharing
            FILE_SHARE_DELETE = 0x00000004,
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_WRITE = 0x00000002
        }

        /// <summary>
        /// File Creation Disposition
        /// </summary>
        internal enum FileCreationDisposition : uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5
        }


        /// <summary>
        /// Represents a wrapper class for a mailslot handle. 
        /// </summary>
        [SecurityCritical(SecurityCriticalScope.Everything),
        HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true),
        SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal sealed class SafeMailslotHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeMailslotHandle()
                : base(true)
            {
            }

            public SafeMailslotHandle(IntPtr preexistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                base.SetHandle(preexistingHandle);
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success),
            DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle()
            {
                return CloseHandle(base.handle);
            }
        }


        /// <summary>
        /// The class exposes Windows APIs to be used in this code sample.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal class NativeMethod
        {
            /// <summary>
            /// Creates or opens a file, directory, physical disk, volume, console 
            /// buffer, tape drive, communications resource, mailslot, or named pipe.
            /// </summary>
            /// <param name="fileName">
            /// The name of the file or device to be created or opened.
            /// </param>
            /// <param name="desiredAccess">
            /// The requested access to the file or device, which can be summarized 
            /// as read, write, both or neither (zero).
            /// </param>
            /// <param name="shareMode">
            /// The requested sharing mode of the file or device, which can be read, 
            /// write, both, delete, all of these, or none (refer to the following 
            /// table). 
            /// </param>
            /// <param name="securityAttributes">
            /// A SECURITY_ATTRIBUTES object that contains two separate but related 
            /// data members: an optional security descriptor, and a Boolean value 
            /// that determines whether the returned handle can be inherited by 
            /// child processes.
            /// </param>
            /// <param name="creationDisposition">
            /// An action to take on a file or device that exists or does not exist.
            /// </param>
            /// <param name="flagsAndAttributes">
            /// The file or device attributes and flags.
            /// </param>
            /// <param name="hTemplateFile">Handle to a template file.</param>
            /// <returns>
            /// If the function succeeds, the return value is an open handle to the 
            /// specified file, device, named pipe, or mail slot.
            /// If the function fails, the return value is an invalid handle.
            /// </returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern SafeMailslotHandle CreateFile(string fileName,
                FileDesiredAccess desiredAccess, FileShareMode shareMode,
                IntPtr securityAttributes,
                FileCreationDisposition creationDisposition,
                int flagsAndAttributes, IntPtr hTemplateFile);


            /// <summary>
            /// Writes data to the specified file or input/output (I/O) device.
            /// </summary>
            /// <param name="handle">
            /// A handle to the file or I/O device (for example, a file, file stream,
            /// physical disk, volume, console buffer, tape drive, socket, 
            /// communications resource, mailslot, or pipe). 
            /// </param>
            /// <param name="bytes">
            /// A buffer containing the data to be written to the file or device.
            /// </param>
            /// <param name="numBytesToWrite">
            /// The number of bytes to be written to the file or device.
            /// </param>
            /// <param name="numBytesWritten">
            /// The number of bytes written when using a synchronous IO.
            /// </param>
            /// <param name="overlapped">
            /// A pointer to an OVERLAPPED structure is required if the file was 
            /// opened with FILE_FLAG_OVERLAPPED.
            /// </param>
            /// <returns>
            /// If the function succeeds, the return value is true. If the function 
            /// fails, or is completing asynchronously, the return value is false.
            /// </returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WriteFile(SafeMailslotHandle handle, 
                byte[] bytes, int numBytesToWrite, out int numBytesWritten, 
                IntPtr overlapped);
        }

        #endregion
    }
}