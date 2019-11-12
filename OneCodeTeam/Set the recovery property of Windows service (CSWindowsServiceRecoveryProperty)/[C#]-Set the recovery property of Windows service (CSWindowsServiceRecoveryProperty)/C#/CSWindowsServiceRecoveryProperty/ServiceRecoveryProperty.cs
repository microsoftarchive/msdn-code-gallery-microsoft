/********************************** Module Header **********************************\
* Module Name:  ServiceRecoveryProperties.cs
* Project:      CSWindowsServiceRecoveryProperty
* Copyright (c) Microsoft Corporation.
* 
* This file demonstrates how to configure service recovery property include grant 
* shutdown privilege to the process, so that we can configure a special option in 
* "Recovery" tab - "Restart Computer Options...".
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

#region Using directives
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
#endregion


namespace CSWindowsServiceRecoveryProperty
{
    internal class ServiceRecoveryProperty
    {
        /// <summary>
        /// Change the recovery property of a Windows service.
        /// </summary>
        /// <param name="scName">The name of the Windows service</param>
        /// <param name="scActions">
        /// A list of SC_ACTION representing the actions that the service control 
        /// manager can perform.
        /// </param>
        /// <param name="resetPeriod">
        /// The time after which to reset the failure count to zero if there are no 
        /// failures, in seconds. 
        /// </param>
        /// <param name="command">
        /// The command line of the process for the CreateProcess function to execute 
        /// in response to the SC_ACTION_RUN_COMMAND service controller action. This 
        /// process runs under the same account as the service. 
        /// </param>
        /// <param name="fFailureActionsOnNonCrashFailures">
        /// If this member is true and the service has configured failure actions, 
        /// the failure actions are queued if the service process terminates without 
        /// reporting a status of SERVICE_STOPPED or if it enters the SERVICE_STOPPED 
        /// state but the dwWin32ExitCode member of the SERVICE_STATUS structure is 
        /// not ERROR_SUCCESS (0). If this member is false and the service has 
        /// configured failure actions, the failure actions are queued only if the 
        /// service terminates without reporting a status of SERVICE_STOPPED.
        /// </param>
        /// <param name="rebootMsg">
        /// The message to be broadcast to server users before rebooting in response 
        /// to the SC_ACTION_REBOOT service controller action.
        /// </param>
        public static void ChangeRecoveryProperty(string scName,
            List<SC_ACTION> scActions, int resetPeriod, string command,
            bool fFailureActionsOnNonCrashFailures, string rebootMsg)
        {
            SafeServiceHandle hSCManager = null;
            SafeServiceHandle hService = null;
            IntPtr hGlobal = IntPtr.Zero;

            try
            {
                // Open the service control manager.
                hSCManager = Win32.OpenSCManager(null, null, Win32.SERVICE_QUERY_CONFIG);
                if (hSCManager.IsInvalid)
                {
                    throw new Win32Exception();
                }

                // Open the service.
                hService = Win32.OpenService(hSCManager, scName, Win32.SERVICE_ALL_ACCESS);
                if (hService.IsInvalid)
                {
                    throw new Win32Exception();
                }

                int numActions = scActions.Count;
                int[] falureActions = new int[numActions * 2];
                bool needShutdownPrivilege = false;
                int i = 0;

                // We need to copy the actions in scFailureActionArray to an 
                // unmanaged memory through Marshal.Copy.

                foreach (SC_ACTION scAction in scActions)
                {
                    falureActions[i] = scAction.Type;
                    falureActions[++i] = scAction.Delay;
                    i++;

                    if (scAction.Type == (int)SC_ACTION_TYPE.RebootComputer)
                    {
                        needShutdownPrivilege = true;
                    }
                }

                // If we need shutdown privilege, then grant it to this process.
                if (needShutdownPrivilege)
                {
                    GrantShutdownPrivilege();
                }

                // Allocate memory.
                hGlobal = Marshal.AllocHGlobal(falureActions.Length * Marshal.SizeOf(typeof(int)));

                // Copies data from a one-dimensional, managed 32-bit signed integer 
                // array to an unmanaged memory pointer.
                Marshal.Copy(falureActions, 0, hGlobal, falureActions.Length);

                // Set the SERVICE_FAILURE_ACTIONS struct.
                SERVICE_FAILURE_ACTIONS scFailureActions = new SERVICE_FAILURE_ACTIONS();
                scFailureActions.cActions = numActions;
                scFailureActions.dwResetPeriod = resetPeriod;
                scFailureActions.lpCommand = command;
                scFailureActions.lpRebootMsg = rebootMsg;
                scFailureActions.lpsaActions = hGlobal;

                // Call the ChangeServiceFailureActions function abstraction of the 
                // ChangeServiceConfig2 function. 
                if (!Win32.ChangeServiceFailureActions(hService,
                    Win32.SERVICE_CONFIG_FAILURE_ACTIONS, ref scFailureActions))
                {
                    throw new Win32Exception();
                }

                // Restart Computer Options....
                SERVICE_FAILURE_ACTIONS_FLAG flag = new SERVICE_FAILURE_ACTIONS_FLAG();
                flag.fFailureActionsOnNonCrashFailures = fFailureActionsOnNonCrashFailures;

                // Call the FailureActionsOnNonCrashFailures function, the 
                // abstraction of the ChangeServiceConfig2 function.
                if (!Win32.FailureActionsOnNonCrashFailures(hService,
                    Win32.SERVICE_CONFIG_FAILURE_ACTIONS_FLAG, ref flag))
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                // Close the service control manager handle.
                if (hSCManager != null && !hSCManager.IsInvalid)
                {
                    hSCManager.Dispose();
                    hSCManager = null;
                }

                // Close the service handle.
                if (hService != null && !hService.IsInvalid)
                {
                    hService.Dispose();
                    hService = null;
                }

                // Free the unmanaged memory.
                if (hGlobal != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hGlobal);
                    hGlobal = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Grant shutdown privilege to the process.
        /// </summary>
        private static void GrantShutdownPrivilege()
        {
            SafeTokenHandle hToken = null;

            try
            {
                // Open the access token associated with the current process.
                if (!Win32.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    Win32.TOKEN_ADJUST_PRIVILEGES | Win32.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Retrieve the locally unique identifier (LUID) used on a specified 
                // system to locally represent the specified privilege name.
                long Luid = 0;
                if (!Win32.LookupPrivilegeValue(null, Win32.SE_SHUTDOWN_NAME, ref Luid))
                {
                    throw new Win32Exception();
                }

                TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Privileges.Luid = Luid;
                tokenPrivileges.Privileges.Attributes = Win32.SE_PRIVILEGE_ENABLED;

                // Enable privileges in the specified access token.
                int retLen = 0;
                if (!Win32.AdjustTokenPrivileges(hToken, false, ref tokenPrivileges,
                    0, IntPtr.Zero, ref retLen))
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                if (hToken != null && !hToken.IsInvalid)
                {
                    hToken.Dispose();
                    hToken = null;
                }
            }
        }
    }
}