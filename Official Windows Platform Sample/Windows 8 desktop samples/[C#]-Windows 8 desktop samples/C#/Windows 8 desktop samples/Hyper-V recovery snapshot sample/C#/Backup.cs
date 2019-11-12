// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Samples.HyperV.Backup
{
    using System;
    using System.Management;
    using Microsoft.Samples.HyperV.Common;

    class BackupOperations
    {
        private const UInt16 SnapshotTypeRecovery = 32768;

        private const UInt16 SettingTypeSnapshot = 5;
        private const UInt16 SettingSubTypeRecovery = 1;

        /// <summary>
        /// Toggles the incremental backup enable/disable status.
        /// </summary>
        /// <param name="hostMachine">The host name of the computer on which
        /// the VM is running.</param>
        /// <param name="vmName">The VM name.</param>
        public
        void
        ToggleIncrementalBackup(
            string hostMachine,
            string vmName
            )
        {
            ManagementScope scope = new ManagementScope(
                @"\\" + hostMachine + @"\root\virtualization\v2", null);

            // Get the management service, VM object and its settings.
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(vmName, scope))
            using (ManagementObject vmSettings = WmiUtilities.GetVirtualMachineSettings(vm))
            using (ManagementObject service = WmiUtilities.GetVirtualMachineManagementService(scope))
            {
                // Get incremental backup status.
                bool isEnabled = (bool)vmSettings["IncrementalBackupEnabled"];

                Console.WriteLine("Current Incremental bakcup status: {0}", isEnabled);

                Console.WriteLine("Changing Incremental backup status to '{0}'...", !isEnabled);
                vmSettings["IncrementalBackupEnabled"] = !isEnabled;

                // Modify the VM settings.
                using (ManagementBaseObject inParams = service.GetMethodParameters("ModifySystemSettings"))
                {
                    inParams["SystemSettings"] = vmSettings.GetText(TextFormat.CimDtd20);

                    using (ManagementBaseObject outParams =
                        service.InvokeMethod("ModifySystemSettings", inParams, null))
                    {
                        WmiUtilities.ValidateOutput(outParams, scope);
                        Console.WriteLine("Successfully toggled incremental backup status.");
                    }
                }
            }
        }

        /// <summary>
        /// Creates a recovery snapshot.
        /// </summary>
        /// <param name="hostMachine">The host name of the computer on which
        /// the VM is running.</param>
        /// <param name="vmName">The VM name.</param>
        public
        void
        CreateRecoverySnapshot(
            string hostMachine,
            string vmName
            )
        {
            ManagementScope scope = new ManagementScope(
                @"\\" + hostMachine + @"\root\virtualization\v2", null);

            // Get the management service and the VM object.
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(vmName, scope))
            using (ManagementObject service = WmiUtilities.GetVirtualMachineSnapshotService(scope))
            using (ManagementBaseObject inParams = service.GetMethodParameters("CreateSnapshot"))
            {
                inParams["AffectedSystem"] = vm.Path.Path;
                inParams["SnapshotSettings"] = "";
                inParams["SnapshotType"] = SnapshotTypeRecovery;

                using (ManagementBaseObject outParams = service.InvokeMethod(
                    "CreateSnapshot",
                    inParams,
                    null))
                {
                    WmiUtilities.ValidateOutput(outParams, scope);
                    Console.WriteLine("Successfully created a recovery snapshot.");
                }
            }
        }

        
        /// <summary>
        /// Lists all recovery snapshots.
        /// </summary>
        /// <param name="hostMachine">The host name of the computer on which
        /// the VM is running.</param>
        /// <param name="vmName">The VM name.</param>
        public
        void
        ListRecoverySnapshots(
            string hostMachine,
            string vmName
            )
        {
            ManagementScope scope = new ManagementScope(
                @"\\" + hostMachine + @"\root\virtualization\v2", null);

            // Get the VM object and snapshot settings.
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(vmName, scope))
            using (ManagementObjectCollection settingsCollection =
                vm.GetRelated("Msvm_VirtualSystemSettingData", "Msvm_SnapshotOfVirtualSystem",
                null, null, null, null, false, null))
            {
                foreach (ManagementObject setting in settingsCollection)
                {
                    using (setting)
                    {
                        string systemType = (string)setting["VirtualSystemType"];

                        if (string.Compare(systemType, VirtualSystemTypeNames.RecoverySnapshot,
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // It is a recovery snapshot.
                            DateTime time = ManagementDateTimeConverter.ToDateTime(setting["CreationTime"].ToString());
                            Console.WriteLine("Recovery snapshot creation time: {0}", time);
                        }
                    }
                }
            }
        }
    }
}
