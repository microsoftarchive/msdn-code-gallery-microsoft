// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Samples.HyperV.Migration
{
    using System;
    using System.IO;
    using System.Management;
    using System.Diagnostics;
    using Microsoft.Samples.HyperV.Common;

    class Migration
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
            const string usageVmSimple = "Usage: Migration vm-simple <sourceHost> <destinationHost> <vmName>";
            const string usageVmSimpleWithIp = "Usage: Migration vm-simple-with-ip <sourceHost> <destinationHost> <vmName>";
            const string usageVmSimpleWithNewRoot = "Usage: Migration vm-simple-with-new-root <sourceHost> <destinationHost> <vmName>";
            const string usageVmDetailed = "Usage: Migration vm-detailed <sourceHost> <destinationHost> <vmName>";
            const string usageSetting = "Usage: Migration modifyservice <sourceHost>";
            const string usageNetworks = "Usage: Migration modifynetworks <sourceHost>";
            const string usageVmCompatibility = "Usage: Migration checkcompatibility <sourceHost> <destinationHost> [<vmName>]";

            const string usageVmSimpleCheck = "Usage: Migration vm-simple-check <sourceHost> <destinationHost> <vmName>";

            const string usageStorageSimple = "Usage: Migration storage-simple <sourceHost> <vmName> <new-location>";
            const string usageStorageSimpleWithPool = "Usage: Migration storage-simple-with-pool <sourceHost> <vmName> <pool-id> <base-path>";

            const string usageVmAndStorageStorageSimple = "Usage: Migration vm-and-storage-simple <sourceHost> <destinationHost> <vmName>";
            const string usageVmAndStorageStorageNewLocation = "Usage: Migration vm-and-storage <sourceHost> <destinationHost> <vmName> <new-location>";

            try
            {
                VmMigration             vmMigration             = new VmMigration();
                MigrationSettings       migrationSettings       = new MigrationSettings();
                StorageMigration        storageMigration        = new StorageMigration();
                VmAndStorageMigration   vmAndStorageMigration   = new VmAndStorageMigration();

                if (args == null || args.Length == 0)
                {
                    Console.WriteLine(usageVmSimple);
                    Console.WriteLine(usageVmSimpleWithIp);
                    Console.WriteLine(usageVmSimpleWithNewRoot);
                    Console.WriteLine(usageVmDetailed);
                    Console.WriteLine(usageSetting);
                    Console.WriteLine(usageNetworks);
                    Console.WriteLine(usageVmCompatibility);
                    Console.WriteLine(usageStorageSimple);
                    Console.WriteLine(usageStorageSimpleWithPool);
                    Console.WriteLine(usageVmAndStorageStorageSimple);
                    Console.WriteLine(usageVmAndStorageStorageNewLocation);
                }
                else if (args[0].Equals("vm-simple", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 4)
                    {
                        Console.WriteLine(usageVmSimple);
                    }
                    else
                    {
                        vmMigration.VmMigrationSimple(args[1], args[2], args[3]);
                    }
                }
                else if (args[0].Equals("vm-simple-with-ip", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 4)
                    {
                        Console.WriteLine(usageVmSimpleWithIp);
                    }
                    else
                    {
                        vmMigration.VmMigrationSimpleWithIpList(args[1], args[2], args[3]);
                    }
                }
                else if (args[0].Equals("vm-simple-with-new-root", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 4)
                    {
                        Console.WriteLine(usageVmSimpleWithNewRoot);
                    }
                    else
                    {
                        vmMigration.VmMigrationSimpleWithNewDataRoot(args[1], args[2], args[3]);
                    }
                }
                else if (args[0].Equals("vm-detailed", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 4)
                    {
                        Console.WriteLine(usageVmDetailed);
                    }
                    else
                    {
                        vmMigration.VmMigrationDetailed(args[1], args[2], args[3]);
                    }
                }
                else if (args[0].Equals("modifyservice", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 2)
                    {
                        Console.WriteLine(usageSetting);
                    }
                    else
                    {
                        migrationSettings.MigrationServiceSettings(args[1]);
                    }
                }
                else if (args[0].Equals("modifynetworks", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 2)
                    {
                        Console.WriteLine(usageNetworks);
                    }
                    else
                    {
                        migrationSettings.MigrationServiceNetworks(args[1]);
                    }
                }
                else if (args[0].Equals("checkcompatibility", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length == 3)
                    {
                        vmMigration.CheckCompatibility(args[1], args[2]);
                    }
                    else if (args.Length == 4)
                    {
                        vmMigration.CheckCompatibility(args[1], args[2], args[3]);
                    }
                    else
                    {
                        Console.WriteLine(usageVmCompatibility);
                    }
                }
                else if (args[0].Equals("vm-simple-check", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 3)
                    {
                        Console.WriteLine(usageVmSimpleCheck);
                    }
                    else
                    {
                        vmMigration.VmMigrationSimpleCheck(args[1], args[2], args[3]);
                    }
                }
                else if (args[0].Equals("storage-simple", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 4)
                    {
                        Console.WriteLine(usageStorageSimple);
                    }
                    else
                    {
                        storageMigration.StorageMigrateSimple(args[1], args[2], args[3]);
                    }
                }
                else if (args[0].Equals("storage-simple-with-pool", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 5)
                    {
                        Console.WriteLine(usageStorageSimpleWithPool);
                    }
                    else
                    {
                        storageMigration.StorageMigrationSimpleWithPool(args[1], args[2], args[3], args[4]);
                    }
                }
                else if (args[0].Equals("vm-and-storage-simple", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 4)
                    {
                        Console.WriteLine(usageVmAndStorageStorageSimple);
                    }
                    else
                    {
                        vmAndStorageMigration.VmAndStorageMigrationSimple(args[1], args[2], args[3]);
                    }
                }
                else if (args[0].Equals("vm-and-storage", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length != 5)
                    {
                        Console.WriteLine(usageVmAndStorageStorageNewLocation);
                    }
                    else
                    {
                        vmAndStorageMigration.VmMigrationSimpleWithNewDataRoot(args[1], args[2], args[3], args[4]);
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