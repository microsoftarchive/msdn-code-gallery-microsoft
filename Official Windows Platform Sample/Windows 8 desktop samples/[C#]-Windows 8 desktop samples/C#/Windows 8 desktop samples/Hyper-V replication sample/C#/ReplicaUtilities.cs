// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Samples.HyperV.Replica
{
    using System;
    using System.Management;
    using System.Globalization;
    using Microsoft.Samples.HyperV.Common;

    static class ReplicaUtilities
    {
        /// <summary>
        /// Gets the virtual system replication service.
        /// </summary>
        /// <param name="scope">The scope to use when connecting to WMI.</param>
        /// <returns>The virtual machine replication service.</returns>
        public static ManagementObject
        GetVirtualMachineReplicationService(
            ManagementScope scope)
        {
            SelectQuery query = new SelectQuery("select * from Msvm_ReplicationService");

            using (ManagementObjectSearcher queryExecute = new ManagementObjectSearcher(scope, query))
            using (ManagementObjectCollection serviceCollection = queryExecute.Get())
            {
                if (serviceCollection.Count == 0)
                {
                    throw new ManagementException("Cannot find the replication service object. " +
                        "Please check that the Hyper-V Virtual Machine Management service is running.");
                }

                return WmiUtilities.GetFirstObjectFromCollection(serviceCollection);
            }
        }

        /// <summary>
        /// Gets the replication service settings object.
        /// </summary>
        /// <param name="replicationService">The replication service object.</param>
        /// <returns>The replication service settings object.</returns>
        internal static ManagementObject
        GetReplicationServiceSettings(
            ManagementObject replicationService)
        {
            using (ManagementObjectCollection settingsCollection =
                    replicationService.GetRelated("Msvm_ReplicationServiceSettingData"))
            {
                ManagementObject replicationServiceSettings = 
                    WmiUtilities.GetFirstObjectFromCollection(settingsCollection);

                return replicationServiceSettings;
            }
        }

        /// <summary>
        /// Gets object for authorization entry.
        /// </summary>
        /// <param name="primaryHostSystem">FQDN of the primary server.</param>
        internal static ManagementObject
        GetAuthorizationEntry(
            string primaryHostSystem)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            using (ManagementObject replicationService = 
                ReplicaUtilities.GetVirtualMachineReplicationService(scope))
            {
                ManagementObject serviceSetting = null;
                using (ManagementObjectCollection settingCollection =
                    replicationService.GetRelated("Msvm_ReplicationAuthorizationSettingData"))
                {
                    foreach (ManagementObject mgmtObj in settingCollection)
                    {
                        if (String.Equals(mgmtObj["AllowedPrimaryHostSystem"].ToString(),
                            primaryHostSystem,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            serviceSetting = mgmtObj;
                            break;
                        }
                        else
                        {
                            mgmtObj.Dispose();
                        }
                    }

                    if (serviceSetting == null)
                    {
                        Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                            "Msvm_ReplicationAuthorizationSettingData not found for \"{0}\"",
                            primaryHostSystem));
                    }
                }

                return serviceSetting;
            }
        }

        /// <summary>
        /// Gets the replication settings object for a virtual machine.
        /// </summary>
        /// <param name="virtualMachine">The virtual machine object.</param>
        /// <returns>The replication settings object.</returns>
        internal static ManagementObject
        GetReplicationSettings(
            ManagementObject virtualMachine)
        {
            using (ManagementObjectCollection settingsCollection =
                    virtualMachine.GetRelated("Msvm_ReplicationSettingData"))
            {
                ManagementObject replicationSettings = 
                    WmiUtilities.GetFirstObjectFromCollection(settingsCollection);

                return replicationSettings;
            }
        }
    }
}
