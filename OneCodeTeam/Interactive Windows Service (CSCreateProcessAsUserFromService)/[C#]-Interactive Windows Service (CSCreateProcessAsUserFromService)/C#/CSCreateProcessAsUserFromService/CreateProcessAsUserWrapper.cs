/************************************* Module Header ***********************************\
* Module Name:  CreateProcessAsUserWrapper.cs
* Project:      CSCreateProcessAsUserFromService
* Copyright (c) Microsoft Corporation.
* 
* The sample demonstrates how to create/launch a process interactively in the session of 
* the logged-on user from a service application written in C#.Net.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace CSCreateProcessAsUserFromService
{
    class CreateProcessAsUserWrapper
    {
        public static void LaunchChildProcess(string ChildProcName)
        {
            IntPtr ppSessionInfo = IntPtr.Zero;
            UInt32 SessionCount = 0;

            if (WTSEnumerateSessions(
                (IntPtr)WTS_CURRENT_SERVER_HANDLE,  // Current RD Session Host Server handle would be zero.
                0,                                  // This reserved parameter must be zero.
                1,                                  // The version of the enumeration request must be 1.
                ref ppSessionInfo,                  // This would point to an array of session info.
                ref SessionCount                    // This would indicate the length of the above array.
                ))
            {
                for (int nCount = 0; nCount < SessionCount; nCount++)
                {
                    // Extract each session info and check if it is the 
                    // "Active Session" of the current logged-on user.
                    WTS_SESSION_INFO tSessionInfo = (WTS_SESSION_INFO)Marshal.PtrToStructure(
                        ppSessionInfo + nCount * Marshal.SizeOf(typeof(WTS_SESSION_INFO)),
                        typeof(WTS_SESSION_INFO)
                        );

                    if (WTS_CONNECTSTATE_CLASS.WTSActive == tSessionInfo.State)
                    {
                        IntPtr hToken = IntPtr.Zero;
                        if (WTSQueryUserToken(tSessionInfo.SessionID, out hToken))
                        {
                            // Launch the child process interactively 
                            // with the token of the logged-on user.
                            PROCESS_INFORMATION tProcessInfo;
                            STARTUPINFO tStartUpInfo = new STARTUPINFO();
                            tStartUpInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                            bool ChildProcStarted = CreateProcessAsUser(
                                hToken,             // Token of the logged-on user.
                                ChildProcName,      // Name of the process to be started.
                                null,               // Any command line arguments to be passed.
                                IntPtr.Zero,        // Default Process' attributes.
                                IntPtr.Zero,        // Default Thread's attributes.
                                false,              // Does NOT inherit parent's handles.
                                0,                  // No any specific creation flag.
                                null,               // Default environment path.
                                null,               // Default current directory.
                                ref tStartUpInfo,   // Process Startup Info. 
                                out tProcessInfo    // Process information to be returned.
                                );

                            if (ChildProcStarted)
                            {
                                // The child process creation is successful!

                                // If the child process is created, it can be controlled via the out 
                                // param "tProcessInfo". For now, as we don't want to do any thing 
                                // with the child process, closing the child process' handles 
                                // to prevent the handle leak.
                                CloseHandle(tProcessInfo.hThread);
                                CloseHandle(tProcessInfo.hProcess);
                            }
                            else
                            {
                                // CreateProcessAsUser failed!
                            }

                            // Whether child process was created or not, close the token handle 
                            // and break the loop as processing for current active user has been done.
                            CloseHandle(hToken);
                            break;
                        }
                        else
                        {
                            // WTSQueryUserToken failed!
                        }
                    }
                    else
                    {
                        // This Session is not active!
                    }
                }

                // Free the memory allocated for the session info array.
                WTSFreeMemory(ppSessionInfo);
            }
            else
            {
                // WTSEnumerateSessions failed!
            }
        }


        #region P/Invoke WTS APIs
        /// <summary>
        /// Struct, Enum and P/Invoke Declarations of WTS APIs.
        /// </summary>
        /// 

        private const int WTS_CURRENT_SERVER_HANDLE = 0;
        private enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WTS_SESSION_INFO
        {
            public UInt32 SessionID;
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] UInt32 Reserved,
            [MarshalAs(UnmanagedType.U4)] UInt32 Version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref UInt32 pSessionInfoCount
            );

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);
        #endregion


        #region P/Invoke CreateProcessAsUser
        /// <summary>
        /// Struct, Enum and P/Invoke Declarations for CreateProcessAsUser.
        /// </summary>
        /// 

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("ADVAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            string lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation
            );

        [DllImport("KERNEL32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CloseHandle(IntPtr hHandle);
        #endregion
    }
}
