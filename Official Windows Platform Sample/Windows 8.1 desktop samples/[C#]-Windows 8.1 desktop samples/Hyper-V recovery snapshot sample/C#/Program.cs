// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Samples.HyperV.Backup
{
    using System;

    class Backup
    {
        /// <summary>
        /// The entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static
        void
        Main(
            string[] args
            )
        {
            const string usageToggleIncemental = "Usage: Backup toggle-incremental <hostMachine> <vmName>";
            const string usageCreateRecoverySnapshot = "Usage: Backup create-recovery-snapshot <hostMachine> <vmName>";
            const string usageListRecoverySnapshot = "Usage: Backup list-recovery-snapshot <hostMachine> <vmName>";

            try
            {
                BackupOperations backupOper = new BackupOperations();

                if (args == null || args.Length == 0)
                {
                    Console.WriteLine(usageToggleIncemental);
                    Console.WriteLine(usageCreateRecoverySnapshot);
                    Console.WriteLine(usageListRecoverySnapshot);
                }
                else if (args[0].Equals("toggle-incremental", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 3)
                    {
                        Console.WriteLine(usageToggleIncemental);
                    }
                    else
                    {
                        backupOper.ToggleIncrementalBackup(args[1], args[2]);
                    }
                }
                else if (args[0].Equals("create-recovery-snapshot", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 3)
                    {
                        Console.WriteLine(usageCreateRecoverySnapshot);
                    }
                    else
                    {
                        backupOper.CreateRecoverySnapshot(args[1], args[2]);
                    }
                }
                else if (args[0].Equals("list-recovery-snapshot", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 3)
                    {
                        Console.WriteLine(usageListRecoverySnapshot);
                    }
                    else
                    {
                        backupOper.ListRecoverySnapshots(args[1], args[2]);
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect option");
                }

                Console.WriteLine("Please press ENTER to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}