/************************************ Module Header ***********************************\
* Module Name:  Program.cs
* Project:      CSWindowsServiceRecoveryProperty
* Copyright (c) Microsoft Corporation.
* 
* CSWindowsServiceRecoveryProperty example demonstrates how to use ChangeServiceConfig2
* to configure the service "Recovery" properties in C#. This example operates all the 
* options you can see on the service "Recovery" tab, including setting the “Enable 
* actions for stops with errors” option in Windows Vista and later operating systems. 
* This example also include how to grant the shut down privilege to the process, so 
* that we can configure a special option in the "Recovery" tab - "Restart Computer 
* Options...".
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************************/

using System;
using System.Collections.Generic;


namespace CSWindowsServiceRecoveryProperty
{
    static class Program
    {
        static void Main()
        {
            List<SC_ACTION> FailureActions = new List<SC_ACTION>();

            // First Failure Actions and Delay (msec).
            FailureActions.Add(new SC_ACTION()
            {
                Type = (int)SC_ACTION_TYPE.RestartService,
                Delay = 1000 * 60 * 5
            });

            // Second Failure Actions and Delay (msec).
            FailureActions.Add(new SC_ACTION()
            {
                Type = (int)SC_ACTION_TYPE.Run_Command,
                Delay = 1000 * 2
            });

            // Subsequent Failures Actions and Delay (msec).
            FailureActions.Add(new SC_ACTION()
            {
                Type = (int)SC_ACTION_TYPE.RebootComputer,
                Delay = 1000 * 60 * 3
            });

            // Configure service recovery property.
            try
            {
                ServiceRecoveryProperty.ChangeRecoveryProperty("CSWindowsService",
                    FailureActions, 60 * 60 * 24 * 4,
                    "C:\\Windows\\System32\\cmd.exe /help /fail=%1%",
                    true, "reboot message");

                Console.WriteLine("The service recovery property is modified successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}