// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Samples.HyperV.Replica
{
    using System;
    using System.Globalization;
    using System.Management;
    using Microsoft.Samples.HyperV.Common;

    static class ManageReplication
    {
        /// <summary>
        /// Enables replication for a virtual machine to a specified server using 
        /// integrated authentication.
        /// </summary>
        /// <param name="name">The name of the virtual machine to enable replication.</param>
        /// <param name="recoveryServerName">The name of the recovery server.</param>
        internal static void
        CreateReplicationRelationship(
            string name,
            string recoveryServerName)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            //
            // Retrieve the Msvm_ComputerSystem.
            //
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(name, scope))
            {
                string vmPath = vm.Path.Path;

                using (ManagementObject replicationSettingData = 
                    ReplicaUtilities.GetReplicationSettings(vm))
                {
                    replicationSettingData["RecoveryConnectionPoint"] = recoveryServerName;
                    replicationSettingData["AuthenticationType"] = 1;
                    replicationSettingData["RecoveryServerPortNumber"] = 80;
                    replicationSettingData["CompressionEnabled"] = 1;

                    // Keep 4 recovery points.
                    replicationSettingData["RecoveryHistory"] = 4;

                    // Take VSS snapshot every one hour.
                    replicationSettingData["ApplicationConsistentSnapshotInterval"] = 1;

                    // Include all disks for replication.
                    replicationSettingData["IncludedDisks"] = WmiUtilities.GetVhdSettings(vm);

                    string settingDataEmbedded = 
                        replicationSettingData.GetText(TextFormat.WmiDtd20);

                    using (ManagementObject replicationService = 
                        ReplicaUtilities.GetVirtualMachineReplicationService(scope))
                    {
                        using (ManagementBaseObject inParams =
                            replicationService.GetMethodParameters("CreateReplicationRelationship"))
                        {
                            inParams["ComputerSystem"] = vmPath;
                            inParams["ReplicationSettingData"] = settingDataEmbedded;

                            using (ManagementBaseObject outParams =
                                replicationService.InvokeMethod("CreateReplicationRelationship", 
                                    inParams, 
                                    null))
                            {
                                WmiUtilities.ValidateOutput(outParams, scope);
                            }
                        }
                    }

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                        "Replication is successfully enabled for virtual machine \"{0}\"", name));
                }
            }
        }

        /// <summary>
        /// Removes replication for a virtual machine.
        /// </summary>
        /// <param name="name">The name of the virtual machine to remove replication for.</param>
        internal static void
        RemoveReplicationRelationship(
            string name)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            //
            // Retrieve the Msvm_ComputerSystem.
            //
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(name, scope))
            {
                string vmPath = vm.Path.Path;

                using (ManagementObject replicationService =
                    ReplicaUtilities.GetVirtualMachineReplicationService(scope))
                {
                    using (ManagementBaseObject inParams =
                        replicationService.GetMethodParameters("RemoveReplicationRelationship"))
                    {
                        inParams["ComputerSystem"] = vmPath;

                        using (ManagementBaseObject outParams =
                            replicationService.InvokeMethod("RemoveReplicationRelationship",
                                inParams,
                                null))
                        {
                            WmiUtilities.ValidateOutput(outParams, scope);
                        }
                    }
                }

                Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                    "Replication is successfully removed for virtual machine \"{0}\"", name));
            }
        }
        
        /// <summary>
        /// Reverses replication for a virtual machine to original primary server.
        /// Virtual machine on primary should be in correct state and should be associated with
        /// the recovery server.
        /// </summary>
        /// <param name="name">The name of the virtual machine to reverse replication.</param>
        internal static void
        ReverseReplicationRelationship(
            string name)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            //
            // Retrieve the Msvm_ComputerSystem.
            //
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(name, scope))
            {
                string vmPath = vm.Path.Path;

                using (ManagementObject replicationSettingData = ReplicaUtilities.GetReplicationSettings(vm))
                {
                    //
                    // Simply reverse the recovery server name with that of primary, other
                    // properties are already populated.
                    //
                    replicationSettingData["RecoveryConnectionPoint"] = 
                        replicationSettingData["PrimaryConnectionPoint"];

                    string settingDataEmbedded = replicationSettingData.GetText(TextFormat.WmiDtd20);

                    using (ManagementObject replicationService = 
                        ReplicaUtilities.GetVirtualMachineReplicationService(scope))
                    {
                        using (ManagementBaseObject inParams =
                            replicationService.GetMethodParameters("ReverseReplicationRelationship"))
                        {
                            inParams["ComputerSystem"] = vmPath;
                            inParams["ReplicationSettingData"] = settingDataEmbedded;

                            using (ManagementBaseObject outParams =
                                replicationService.InvokeMethod("ReverseReplicationRelationship", 
                                    inParams, 
                                    null))
                            {
                                WmiUtilities.ValidateOutput(outParams, scope);
                            }
                        }
                    }

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                        "Replication is successfully reversed for virtual machine \"{0}\"", name));
                }
            }
        }

        /// <summary>
        /// Starts replication over network for a given virtual machine.
        /// </summary>
        /// <param name="name">The name of the virtual machine to start replication for.</param>
        internal static void
        StartReplication(
            string name)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            //
            // Retrieve the Msvm_ComputerSystem.
            //
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(name, scope))
            {
                //
                // Call the Msvm_ReplicationService::StartReplication method. 
                // Note the input parameters values are as below -
                // InitialReplicationType - 1 for transfer over network
                //                          2 for exporting the initial replication to the location specified
                //                            in InitialReplicationExportLocation parameter.
                //                          3 for replication with a restored copy on recovery.
                // InitialReplicationExportLocation - null or export location path when InitialReplicationType is 2.
                // StartTime - null or scheduled start time in UTC.
                //
                string vmPath = vm.Path.Path;

                using (ManagementObject replicationService = 
                    ReplicaUtilities.GetVirtualMachineReplicationService(scope))
                {
                    using (ManagementBaseObject inParams =
                        replicationService.GetMethodParameters("StartReplication"))
                    {
                        inParams["ComputerSystem"] = vmPath;
                        inParams["InitialReplicationType"] = 1;
                        inParams["InitialReplicationExportLocation"] = null;
                        inParams["StartTime"] = null;

                        using (ManagementBaseObject outParams =
                            replicationService.InvokeMethod("StartReplication", inParams, null))
                        {
                            WmiUtilities.ValidateOutput(outParams, scope);
                        }
                    }
                }

                Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                    "Replication is successfully started for virtual machine \"{0}\"", name));
            }
        }

        /// <summary>
        /// Creates test replica virtual machine for a given replica virtual machine.
        /// </summary>
        /// <param name="name">The name of the virtual machine to create test replica 
        /// virtual machine for.</param>
        internal static void
        TestReplicaSystem(
            string name)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            //
            // Retrieve the Msvm_ComputerSystem.
            //
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(name, scope))
            {
                //
                // Call the Msvm_ReplicationService::TestReplicaSystem method. 
                // Note the input paramters values are as below -
                // SnapshotSettingData - null for latest recovery point.
                //                       OR Embedded instance of CIM_VirtualSystemSettingData 
                //                       pointing to recovery snapshot.
                //
                string vmPath = vm.Path.Path;

                using (ManagementObject replicationService =
                    ReplicaUtilities.GetVirtualMachineReplicationService(scope))
                {
                    using (ManagementBaseObject inParams =
                        replicationService.GetMethodParameters("TestReplicaSystem"))
                    {
                        inParams["ComputerSystem"] = vmPath;
                        inParams["SnapshotSettingData"] = null;

                        using (ManagementBaseObject outParams =
                            replicationService.InvokeMethod("TestReplicaSystem", inParams, null))
                        {
                            WmiUtilities.ValidateOutput(outParams, scope);
                        }
                    }
                }

                Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                    "Test replica virtual machine \"{0} - Test\" is successfully created for virtual machine \"{0}\"", name));
            }
        }
        
        /// <summary>
        /// Initiates failover for a given virtual machine.
        /// </summary>
        /// <param name="name">The name of the virtual machine to initiate failover for.</param>
        internal static void
        InitiateFailover(
            string name)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            //
            // Retrieve the Msvm_ComputerSystem.
            //
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(name, scope))
            {
                //
                // Call the Msvm_ReplicationService::InitiateFailover method. 
                // Note the input paramters values are as below -
                // SnapshotSettingData - null for latest recovery point.
                //                       OR Embedded instance of CIM_VirtualSystemSettingData 
                //                       pointing to recovery snapshot.
                //
                string vmPath = vm.Path.Path;

                using (ManagementObject replicationService = 
                    ReplicaUtilities.GetVirtualMachineReplicationService(scope))
                {
                    using (ManagementBaseObject inParams =
                        replicationService.GetMethodParameters("InitiateFailover"))
                    {
                        inParams["ComputerSystem"] = vmPath;
                        inParams["SnapshotSettingData"] = null;

                        using (ManagementBaseObject outParams =
                            replicationService.InvokeMethod("InitiateFailover", inParams, null))
                        {
                            WmiUtilities.ValidateOutput(outParams, scope);
                        }
                    }
                }

                Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                    "Failover is successfully completed for virtual machine \"{0}\"", name));
            }
        }

        /// <summary>
        /// Changes replication state of a virtual machine.
        /// </summary>
        /// <param name="name">The name of the virtual machine to change replication state.</param>
        /// <param name="requestedState">Requested replication state.</param>
        internal static void
        RequestReplicationStateChange(
            string name,
            UInt16 requestedState)
        {
            ManagementScope scope = new ManagementScope(@"root\virtualization\v2");

            //
            // Retrieve the Msvm_ComputerSystem.
            //
            using (ManagementObject vm = WmiUtilities.GetVirtualMachine(name, scope))
            {
                using (ManagementBaseObject inParams =
                    vm.GetMethodParameters("RequestReplicationStateChange"))
                {
                    inParams["RequestedState"] = requestedState;

                    using (ManagementBaseObject outParams =
                        vm.InvokeMethod("RequestReplicationStateChange", inParams, null))
                    {
                        WmiUtilities.ValidateOutput(outParams, scope);
                    }

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                        "Replication state for virtual machine \"{0}\" is changed to \"{1}\"", 
                        name, requestedState));
                }
            }
        }
    }
}
