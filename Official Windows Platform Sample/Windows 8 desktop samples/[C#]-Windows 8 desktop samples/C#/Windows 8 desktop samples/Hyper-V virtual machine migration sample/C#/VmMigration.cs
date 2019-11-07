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

    class VmMigration : MigrationCommon
    {
        /// <summary>
        /// Migrates a VM without any modification, using all defaults.
        /// </summary>
        /// <param name="sourceHost">Migration source host name.</param>
        /// <param name="destinationHost">
        /// Migration destination host name.
        /// </param>
        /// <param name="vmName">VM name.</param>
        public
        void
        VmMigrationSimple(
            string sourceHost,
            string destinationHost,
            string vmName
            )
        {
            ManagementScope srcScope = new ManagementScope(
                @"\\" + sourceHost + @"\root\virtualization\v2", null);

            using (ManagementObject migrationSettingData = GetMigrationSettingData(srcScope))
            {
                migrationSettingData["MigrationType"] = MigrationType.VirtualSystem;
                migrationSettingData["TransportType"] = TransportType.TCP;

                // Perform migration.
                Console.WriteLine("Performing migration...");
                Migrate(srcScope,
                    vmName,
                    destinationHost,
                    migrationSettingData.GetText(TextFormat.CimDtd20),
                    null,
                    null);
            }
        }

        /// <summary>
        /// Check migratability of a VM without any modification, using all defaults.
        /// </summary>
        /// <param name="sourceHost">Migration source host name.</param>
        /// <param name="destinationHost">
        /// Migration destination host name.
        /// </param>
        /// <param name="vmName">VM name.</param>
        public
        void
        VmMigrationSimpleCheck(
            string sourceHost,
            string destinationHost,
            string vmName
            )
        {
            ManagementScope srcScope = new ManagementScope(
                @"\\" + sourceHost + @"\root\virtualization\v2", null);

            using (ManagementObject migrationSettingData = GetMigrationSettingData(srcScope))
            {
                migrationSettingData["MigrationType"] = MigrationType.VirtualSystem;
                migrationSettingData["TransportType"] = TransportType.TCP;

                // Perform migration check.
                Console.WriteLine("Performing migratability check...");
                CheckMigratability(srcScope,
                    vmName,
                    destinationHost,
                    migrationSettingData.GetText(TextFormat.CimDtd20),
                    null,
                    null);
            }
        }

        /// <summary>
        /// Migrates a VM without any modification, using all defaults and
        /// provide destination host IP addresses on which to perform
        /// migration.
        /// </summary>
        /// <param name="sourceHost">Migration source host name.</param>
        /// <param name="destinationHost">
        /// Migration destination host name.
        /// </param>
        /// <param name="vmName">VM name.</param>
        public
        void
        VmMigrationSimpleWithIpList(
            string sourceHost,
            string destinationHost,
            string vmName
            )
        {
            ManagementScope srcScope = new ManagementScope(
                @"\\" + sourceHost + @"\root\virtualization\v2", null);

            using (ManagementObject migrationSettingData = GetMigrationSettingData(srcScope))
            {
                migrationSettingData["MigrationType"] = MigrationType.VirtualSystem;
                migrationSettingData["TransportType"] = TransportType.TCP;

                // Set IP address list.
                migrationSettingData["DestinationIPAddressList"] =
                    GetMigrationDestinationListenAddresses(destinationHost);

                // Perform migration.
                Console.WriteLine("Performing migration...");
                Migrate(srcScope,
                    vmName,
                    destinationHost,
                    migrationSettingData.GetText(TextFormat.CimDtd20),
                    null,
                    null);
            }
        }

        /// <summary>
        /// Migrates a VM to a new data root.
        /// </summary>
        /// <param name="sourceHost">Migration source host name.</param>
        /// <param name="destinationHost">
        /// Migration destination host name.
        /// </param>
        /// <param name="vmName">VM name.</param>
        public
        void
        VmMigrationSimpleWithNewDataRoot(
            string sourceHost,
            string destinationHost,
            string vmName
            )
        {
            string newDataRoot = "C:\\NewDataRoot";

            ManagementScope srcScope = new ManagementScope(
                @"\\" + sourceHost + @"\root\virtualization\v2", null);

            using (ManagementObject migrationSettingData = GetMigrationSettingData(srcScope))
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(vmName, srcScope))
            using (ManagementObject vssd = WmiUtilities.GetVirtualMachineSettings(vm))
            {
                migrationSettingData["MigrationType"] = MigrationType.VirtualSystem;
                migrationSettingData["TransportType"] = TransportType.TCP;

                vssd["ConfigurationDataRoot"] = newDataRoot;

                // Perform migration.
                Console.WriteLine("Performing migration...");
                Migrate(srcScope,
                    vmName,
                    destinationHost,
                    migrationSettingData.GetText(TextFormat.CimDtd20),
                    vssd.GetText(TextFormat.CimDtd20),
                    null);
            }
        }

        /// <summary>
        /// Migrate a planned VM, perform fixup, migrate running state to
        /// planned VM.
        /// </summary>
        /// <param name="sourceHost">Migration source host name.</param>
        /// <param name="destinationHost">
        /// Migration destination host name.
        /// </param>
        /// <param name="vmName">VM name.</param>
        public
        void
        VmMigrationDetailed(
            string sourceHost,
            string destinationHost,
            string vmName
            )
        {
            ManagementScope srcScope = new ManagementScope(
                @"\\" + sourceHost + @"\root\virtualization\v2", null);

            using (ManagementObject migrationSettingData = GetMigrationSettingData(srcScope))
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(vmName, srcScope))
            {
                //
                // Step - 1: Create a planned VM at destination with the
                // VM configuration.
                //

                migrationSettingData["MigrationType"] = MigrationType.VirtualSystem;
                migrationSettingData["TransportType"] = TransportType.TCP;

                migrationSettingData["DestinationIPAddressList"] =
                    GetMigrationDestinationListenAddresses(destinationHost);

                Console.WriteLine("Creating planned VM at destination...");

                migrationSettingData["MigrationType"] = MigrationType.Staged;

                Migrate(srcScope,
                    vmName,
                    destinationHost,
                    migrationSettingData.GetText(TextFormat.CimDtd20),
                    null,
                    null);

                //
                // Step - 2: Check migratability. If the VM cannot be migrated to
                // the created planned VM, then check the reasons and fix the
                // created planned VM accordingly.
                //

                migrationSettingData["MigrationType"] = MigrationType.VirtualSystem;

                migrationSettingData["DestinationPlannedVirtualSystemId"] = vm["Name"];

                Console.WriteLine(
                    "Check VM migratability to the created Planned VM...");
                while (!CheckMigratability(srcScope,
                    vmName,
                    destinationHost,
                    migrationSettingData.GetText(TextFormat.CimDtd20),
                    null,
                    null))
                {
                    Console.WriteLine("The VM is not migratable to the planned VM. " +
                        "Please fix the planned VM.");

                    //
                    // To fix up the planned VM created at migration destination, see
                    // the documentation and samples for planned VM.
                    // Keep performing step-2 until it succeeds without any error.
                    //

                    Console.WriteLine("Please press ENTER after fixing the planned VM");
                    Console.ReadLine();
                }

                //
                // Step - 3: After CheckMigratability succeeds, perform the
                // migration, using the same migration parameters as the check.
                //

                Console.WriteLine("Migrating VM to the created Planned VM...");
                Migrate(srcScope,
                    vmName,
                    destinationHost,
                    migrationSettingData.GetText(TextFormat.CimDtd20),
                    null,
                    null);
            }
        }

        /// <summary>
        /// Performs compatibility check for the VM or for the two host
        /// computer systems.
        /// </summary>
        /// <param name="sourceHost">Source host computer system.</param>
        /// <param name="destinationHost">Destination host computer system.</param>
        /// <param name="vmName">Optional, the virtual machine name.</param>
        public
        void
        CheckCompatibility(
            string sourceHost,
            string destinationHost,
            string systemName = null
            )
        {
            if (systemName == null)
            {
                systemName = sourceHost;
            }

            ManagementScope sourceScope = new ManagementScope(
                @"\\" + sourceHost + @"\root\virtualization\v2", null);

            object compatibilityInfo = null;

            // Get compatibility information for the source host.
            using (ManagementObject computerSystem = WmiUtilities.GetVirtualMachine(systemName, sourceScope))
            using (ManagementObject sourceService = GetVirtualMachineMigrationService(sourceScope))
            using (ManagementBaseObject sourceInParams =
                sourceService.GetMethodParameters("GetSystemCompatibilityInfo"))
            {
                sourceInParams["ComputerSystem"] = computerSystem.Path.Path;

                using (ManagementBaseObject sourceOutParams =
                    sourceService.InvokeMethod("GetSystemCompatibilityInfo", sourceInParams, null))
                {
                    WmiUtilities.ValidateOutput(sourceOutParams, sourceScope);
                    compatibilityInfo = sourceOutParams["CompatibilityInfo"];
                }
            }

            //
            // Pass the compatibility blob to the destination host and check
            // for compatibility.
            //

            ManagementScope destinationScope = new ManagementScope(
                @"\\" + destinationHost + @"\root\virtualization\v2", null);

            // Get compatibility information for the source host.
            using (ManagementObject destinationService = GetVirtualMachineMigrationService(destinationScope))
            using (ManagementBaseObject destinationInParams =
                destinationService.GetMethodParameters("CheckSystemCompatibilityInfo"))
            {
                destinationInParams["CompatibilityInfo"] = compatibilityInfo;

                using (ManagementBaseObject destinationOutParams =
                    destinationService.InvokeMethod("CheckSystemCompatibilityInfo", destinationInParams, null))
                {
                    uint returnValue = (uint)destinationOutParams["ReturnValue"];
                    if (returnValue == 0)
                    {
                        Console.WriteLine("The VM or the systems are compatible");
                    }
                    else if (returnValue == 32784)
                    {
                        Console.WriteLine("The VM or the systems are not compatible");
                    }
                    else
                    {
                        throw new ManagementException("The method call failed.");
                    }
                }
            }
        }
    }
}