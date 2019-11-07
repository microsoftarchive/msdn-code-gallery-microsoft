/********************************** Module Header **********************************\
Module Name:  Program.cs
Project:      CSMailslotServer
Copyright (c) Microsoft Corporation.

Mailslot is a mechanism for one-way inter-process communication in the local machine
or across the computers in the intranet. Any clients can store messages in a mailslot. 
The creator of the slot, i.e. the server, retrieves the messages that are stored 
there:

Client (GENERIC_WRITE) ---> Server (GENERIC_READ)

This code sample demonstrates calling CreateMailslot to create a mailslot named 
"\\.\mailslot\SampleMailslot". The security attributes of the slot are customized to 
allow Authenticated Users read and write access to the slot, and to allow the 
Administrators group full access to it. The sample first creates such a mailslot, 
then it reads and displays new messages in the slot when user presses ENTER in the 
console.

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
using System.Security;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.ComponentModel;
#endregion


namespace CSMailslotServer
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
                // Prepare the security attributes (the lpSecurityAttributes parameter 
                // in CreateMailslot) for the mailslot. This is optional. If the 
                // lpSecurityAttributes parameter of CreateMailslot is NULL, the 
                // mailslot gets a default security descriptor and the handle cannot 
                // be inherited. The ACLs in the default security descriptor of a 
                // mailslot grant full control to the LocalSystem account, (elevated) 
                // administrators, and the creator owner. They also give only read 
                // access to members of the Everyone group and the anonymous account. 
                // However, if you want to customize the security permission of the 
                // mailslot, (e.g. to allow Authenticated Users to read from and 
                // write to the mailslot), you need to create a SECURITY_ATTRIBUTES 
                // structure.
                SECURITY_ATTRIBUTES sa = null;
                sa = CreateMailslotSecurity();

                // Create the mailslot.
                hMailslot = NativeMethod.CreateMailslot(
                    MailslotName,               // The name of the mailslot
                    0,                          // No maximum message size
                    MAILSLOT_WAIT_FOREVER,      // Waits forever for a message
                    sa                          // Mailslot security attributes
                    );

                if (hMailslot.IsInvalid)
                {
                    throw new Win32Exception();
                }

                Console.WriteLine("The mailslot ({0}) is created.", MailslotName);

                // Check messages in the mailslot.
                Console.Write("Press ENTER to check new messages or press Q to quit ...");
                string cmd = Console.ReadLine();
                while (!cmd.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Checking new messages...");
                    ReadMailslot(hMailslot);

                    Console.Write("Press ENTER to check new messages or press Q to quit ...");
                    cmd = Console.ReadLine();
                }
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine("The server throws the error: {0}", ex.Message);
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
        /// The CreateMailslotSecurity function creates and initializes a new 
        /// SECURITY_ATTRIBUTES object to allow Authenticated Users read and 
        /// write access to a mailslot, and to allow the Administrators group full 
        /// access to the mailslot.
        /// </summary>
        /// <returns>
        /// A SECURITY_ATTRIBUTES object that allows Authenticated Users read and 
        /// write access to a mailslot, and allows the Administrators group full 
        /// access to the mailslot.
        /// </returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa365600.aspx"/>
        static SECURITY_ATTRIBUTES CreateMailslotSecurity()
        {
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
            return sa;
        }


        /// <summary>
        /// Read the messages from a mailslot by using the mailslot handle in a call 
        /// to the ReadFile function. 
        /// </summary>
        /// <param name="hMailslot">The handle of the mailslot</param>
        /// <returns> 
        /// If the function succeeds, the return value is true.
        /// </returns>
        static bool ReadMailslot(SafeMailslotHandle hMailslot)
        {
            int cbMessageBytes = 0;         // Size of the message in bytes
            int cbBytesRead = 0;            // Number of bytes read from the mailslot
            int cMessages = 0;              // Number of messages in the slot
            int nMessageId = 0;             // Message ID

            bool succeeded = false;

            // Check for the number of messages in the mailslot.
            succeeded = NativeMethod.GetMailslotInfo(
                hMailslot,                  // Handle of the mailslot
                IntPtr.Zero,                // No maximum message size 
                out cbMessageBytes,         // Size of next message 
                out cMessages,              // Number of messages 
                IntPtr.Zero                 // No read time-out
                );
            if (!succeeded)
            {
                Console.WriteLine("GetMailslotInfo failed w/err 0x{0:X}",
                    Marshal.GetLastWin32Error());
                return succeeded;
            }

            if (cbMessageBytes == MAILSLOT_NO_MESSAGE)
            {
                // There are no new messages in the mailslot at present
                Console.WriteLine("No new messages.");
                return succeeded;
            }

            // Retrieve the messages one by one from the mailslot.
            while (cMessages != 0)
            {
                nMessageId++;

                // Declare a byte array to fetch the data
                byte[] bBuffer = new byte[cbMessageBytes];
                succeeded = NativeMethod.ReadFile(
                    hMailslot,              // Handle of mailslot
                    bBuffer,                // Buffer to receive data
                    cbMessageBytes,         // Size of buffer in bytes
                    out cbBytesRead,        // Number of bytes read from mailslot
                    IntPtr.Zero             // Not overlapped I/O
                    );
                if (!succeeded)
                {
                    Console.WriteLine("ReadFile failed w/err 0x{0:X}", 
                        Marshal.GetLastWin32Error());
                    break;
                }

                // Display the message. 
                Console.WriteLine("Message #{0}: {1}", nMessageId, 
                    Encoding.Unicode.GetString(bBuffer));

                // Get the current number of un-read messages in the slot. The number
                // may not equal the initial message number because new messages may 
                // arrive while we are reading the items in the slot.
                succeeded = NativeMethod.GetMailslotInfo(
                    hMailslot,              // Handle of the mailslot
                    IntPtr.Zero,            // No maximum message size 
                    out cbMessageBytes,     // Size of next message 
                    out cMessages,          // Number of messages 
                    IntPtr.Zero             // No read time-out 
                    );
                if (!succeeded)
                {
                    Console.WriteLine("GetMailslotInfo failed w/err 0x{0:X}",
                        Marshal.GetLastWin32Error());
                    break;
                }
            }

            return succeeded;
        }


        #region Native API Signatures and Types

        /// <summary>
        /// Mailslot waits forever for a message 
        /// </summary>
        internal const int MAILSLOT_WAIT_FOREVER = -1;

        /// <summary>
        /// There is no next message
        /// </summary>
        internal const int MAILSLOT_NO_MESSAGE = -1;


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
        /// The SECURITY_ATTRIBUTES structure contains the security descriptor for 
        /// an object and specifies whether the handle retrieved by specifying 
        /// this structure is inheritable. This structure provides security 
        /// settings for objects created by various functions, such as CreateFile, 
        /// CreateNamedPipe, CreateProcess, RegCreateKeyEx, or RegSaveKeyEx.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class SECURITY_ATTRIBUTES
        {
            public int nLength;
            public SafeLocalMemHandle lpSecurityDescriptor;
            public bool bInheritHandle;
        }


        /// <summary>
        /// Represents a wrapper class for a local memory pointer. 
        /// </summary>
        [SuppressUnmanagedCodeSecurity,
        HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
        internal sealed class SafeLocalMemHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeLocalMemHandle()
                : base(true)
            {
            }

            public SafeLocalMemHandle(IntPtr preexistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                base.SetHandle(preexistingHandle);
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success),
            DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr LocalFree(IntPtr hMem);

            protected override bool ReleaseHandle()
            {
                return (LocalFree(base.handle) == IntPtr.Zero);
            }
        }


        /// <summary>
        /// The class exposes Windows APIs to be used in this code sample.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal class NativeMethod
        {
            /// <summary>
            /// Creates an instance of a mailslot and returns a handle for subsequent 
            /// operations.
            /// </summary>
            /// <param name="mailslotName">Mailslot name</param>
            /// <param name="nMaxMessageSize">
            /// The maximum size of a single message
            /// </param>
            /// <param name="lReadTimeout">
            /// The time a read operation can wait for a message.
            /// </param>
            /// <param name="securityAttributes">Security attributes</param>
            /// <returns>
            /// If the function succeeds, the return value is a handle to the server 
            /// end of a mailslot instance.
            /// </returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern SafeMailslotHandle CreateMailslot(string mailslotName,
                uint nMaxMessageSize, int lReadTimeout,
                SECURITY_ATTRIBUTES securityAttributes);


            /// <summary>
            /// Retrieves information about the specified mailslot.
            /// </summary>
            /// <param name="hMailslot">A handle to a mailslot</param>
            /// <param name="lpMaxMessageSize">
            /// The maximum message size, in bytes, allowed for this mailslot.
            /// </param>
            /// <param name="lpNextSize">
            /// The size of the next message in bytes.
            /// </param>
            /// <param name="lpMessageCount">
            /// The total number of messages waiting to be read.
            /// </param>
            /// <param name="lpReadTimeout">
            /// The amount of time, in milliseconds, a read operation can wait for a 
            /// message to be written to the mailslot before a time-out occurs. 
            /// </param>
            /// <returns></returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetMailslotInfo(SafeMailslotHandle hMailslot,
                IntPtr lpMaxMessageSize, out int lpNextSize, out int lpMessageCount,
                IntPtr lpReadTimeout);


            /// <summary>
            /// Reads data from the specified file or input/output (I/O) device.
            /// </summary>
            /// <param name="handle">
            /// A handle to the device (for example, a file, file stream, physical 
            /// disk, volume, console buffer, tape drive, socket, communications 
            /// resource, mailslot, or pipe).
            /// </param>
            /// <param name="bytes">
            /// A buffer that receives the data read from a file or device.
            /// </param>
            /// <param name="numBytesToRead">
            /// The maximum number of bytes to be read.
            /// </param>
            /// <param name="numBytesRead">
            /// The number of bytes read when using a synchronous IO.
            /// </param>
            /// <param name="overlapped">
            /// A pointer to an OVERLAPPED structure if the file was opened with 
            /// FILE_FLAG_OVERLAPPED.
            /// </param> 
            /// <returns>
            /// If the function succeeds, the return value is true. If the function 
            /// fails, or is completing asynchronously, the return value is false.
            /// </returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReadFile(SafeMailslotHandle handle,
                byte[] bytes, int numBytesToRead, out int numBytesRead,
                IntPtr overlapped);


            /// <summary>
            /// The ConvertStringSecurityDescriptorToSecurityDescriptor function 
            /// converts a string-format security descriptor into a valid, 
            /// functional security descriptor.
            /// </summary>
            /// <param name="sddlSecurityDescriptor">
            /// A string containing the string-format security descriptor (SDDL) 
            /// to convert.
            /// </param>
            /// <param name="sddlRevision">
            /// The revision level of the sddlSecurityDescriptor string. 
            /// Currently this value must be 1.
            /// </param>
            /// <param name="pSecurityDescriptor">
            /// A pointer to a variable that receives a pointer to the converted 
            /// security descriptor.
            /// </param>
            /// <param name="securityDescriptorSize">
            /// A pointer to a variable that receives the size, in bytes, of the 
            /// converted security descriptor. This parameter can be IntPtr.Zero.
            /// </param>
            /// <returns>
            /// If the function succeeds, the return value is true.
            /// </returns>
            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(
                string sddlSecurityDescriptor, int sddlRevision,
                out SafeLocalMemHandle pSecurityDescriptor,
                IntPtr securityDescriptorSize);
        }

        #endregion
    }
}