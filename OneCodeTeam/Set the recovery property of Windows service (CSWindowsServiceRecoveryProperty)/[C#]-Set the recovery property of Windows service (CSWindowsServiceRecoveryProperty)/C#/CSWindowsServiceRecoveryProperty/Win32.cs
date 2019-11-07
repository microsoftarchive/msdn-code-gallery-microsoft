/************************************ Module Header ***********************************\
* Module Name:  Win32.cs
* Project:      CSWindowsServiceRecoveryProperty
* Copyright (c) Microsoft Corporation.
* 
* The file declares the P/Invoke signatures of the Win32 APIs and structs.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************************/

#region Using directives
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
#endregion


namespace CSWindowsServiceRecoveryProperty
{
    // Enumeration for SC_ACTION
    // The SC_ACTION_TYPE enumeration specifies the actions that the SCM can perform.
    internal enum SC_ACTION_TYPE
    {
        None = 0,
        RestartService = 1,
        RebootComputer = 2,
        Run_Command = 3
    }

    // Struct for SERVICE_FAILURE_ACTIONS
    // Represents an action that the service control manager can perform.
    [StructLayout(LayoutKind.Sequential)]
    internal struct SC_ACTION
    {
        public int Type;
        public int Delay;
    }

    // Struct for ChangeServiceFailureActions
    // Represents the action the service controller should take on each failure of a 
    // service.
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SERVICE_FAILURE_ACTIONS
    {
        public int dwResetPeriod;
        public string lpRebootMsg;
        public string lpCommand;
        public int cActions;
        // A pointer to an array of SC_ACTION structures
        public IntPtr lpsaActions;
    }

    // Struct for FailureActionsOnNonCrashFailures
    // Contains the failure actions flag setting of a service.
    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_FAILURE_ACTIONS_FLAG
    {
        public bool fFailureActionsOnNonCrashFailures;
    }

    // Struct required to set shutdown privileges
    // The LUID_AND_ATTRIBUTES structure represents a locally unique identifier 
    // (LUID) and its attributes.
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID_AND_ATTRIBUTES
    {
        public long Luid;
        public int Attributes;
    }

    // Struct for AdjustTokenPrivileges
    // The TOKEN_PRIVILEGES structure contains information about a set of privileges 
    // for an access token. Struct required to set shutdown privileges. The Pack 
    // attribute specified here is important. We are in essence cheating here because 
    // the Privileges field is actually a variable size array of structs.  We use the 
    // Pack=1 to align the Privileges field exactly after the PrivilegeCount field 
    // when marshalling this struct to Win32. You do not want to know how many hours 
    // I had to spend on this alone!!!
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TOKEN_PRIVILEGES
    {
        public int PrivilegeCount;
        public LUID_AND_ATTRIBUTES Privileges;
    }

    /// <summary>
    /// Represents a wrapper class for a service handle.
    /// </summary>
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class SafeServiceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeServiceHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return Win32.CloseServiceHandle(base.handle);
        }
    }

    /// <summary>
    /// Represents a wrapper class for a token handle.
    /// </summary>
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        internal SafeTokenHandle(IntPtr handle)
            : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return Win32.CloseHandle(base.handle);
        }
    }

    [SuppressUnmanagedCodeSecurity()]
    internal class Win32
    {
        public const int SERVICE_ALL_ACCESS = 0xF01FF;
        public const int SERVICE_QUERY_CONFIG = 0x0001;
        public const int SERVICE_CONFIG_FAILURE_ACTIONS = 0x2;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 0x4;

        public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        public const int SE_PRIVILEGE_ENABLED = 2;
        public const int TOKEN_ADJUST_PRIVILEGES = 32;
        public const int TOKEN_QUERY = 8;

        // Win32 function to open the service control manager.
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeServiceHandle OpenSCManager(
            string lpMachineName,
            string lpDatabaseName,
            int dwDesiredAccess);

        // Win32 function to open a service instance.
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeServiceHandle OpenService(
            SafeServiceHandle hSCManager,
            string lpServiceName,
            int dwDesiredAccess);

        // Win32 function to close a service related handle.
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        // Win32 function to change the service config for the failure actions.
        // If the service controller handles the SC_ACTION_REBOOT action, 
        // the caller must have the SE_SHUTDOWN_NAME privilege.
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2",
            CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceFailureActions(
            SafeServiceHandle hService,
            int dwInfoLevel,
            [MarshalAs(UnmanagedType.Struct)]
            ref SERVICE_FAILURE_ACTIONS lpInfo);

        // This setting is ignored unless the service has configured failure actions.
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2",
            CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FailureActionsOnNonCrashFailures(
            SafeServiceHandle hService,
            int dwInfoLevel,
            [MarshalAs(UnmanagedType.Struct)]
            ref SERVICE_FAILURE_ACTIONS_FLAG lpInfo);

        // This method adjusts privileges for this process which is needed when
        // specifying the reboot option for a service failure recover action.
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(
            SafeTokenHandle TokenHandle,
            bool DisableAllPrivileges,
            [MarshalAs(UnmanagedType.Struct)]
            ref TOKEN_PRIVILEGES NewState,
            int BufferLength,
            IntPtr PreviousState,
            ref int ReturnLength);

        // Looks up the privilege code for the privilege name
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(
            string lpSystemName,
            string lpName,
            ref long lpLuid);

        // Opens the security/privilege token for a process handle
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            int DesiredAccess,
            out SafeTokenHandle TokenHandle);

        // Close the handle.
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);
    }
}