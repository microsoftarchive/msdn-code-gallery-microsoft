# Hyper-V virtual machine migration sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Hyper-V
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the Hyper-V WMI APIs to manage virtual machine migration. The sample demonstrates how to perform each of the following operations:</p>
<ul>
<li>Perform a simple migration of a virtual machine using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh859765">
<b>MigrateVirtualSystemToHost</b></a> method. </li><li>Perform a simple migration of a virtual machine using destination host IP addresses.
</li><li>Perform a simple migration of a virtual machine that has a new data root. </li><li>Migrate a planned virtual machine, perform fixup, and migrate the running state to the planned virtual machine.
</li><li>Modify the migration service settings using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh859767">
<b>ModifyServiceSettings</b></a> method. </li><li>Modify the migration service networks using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh859755">
<b>AddNetworkSettings</b></a> method. </li><li>Check the migration compatibility for a virtual machine, or for two hosts, using the
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh859760"><b>GetSystemCompatibilityInfo</b></a> and
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh859756"><b>CheckSystemCompatibilityInfo</b></a> methods.
</li><li>Perform a simple check of the migration capability of a virtual machine. </li><li>Migrate the VHDs and data roots for a virtual machine. </li><li>Migrate the first VHD for a virtual machine to a new resource pool. </li><li>Perform a simple migration of a virtual machine, using resource pools to obtain the correct VHD paths.
</li><li>Migrate all virtual machine files to a new location at the destination host. </li></ul>
<p></p>
<p>This sample is written in C# and requires some experience with WMI programming.</p>
<p>The Windows Samples Gallery contains a variety of code samples that demonstrate the use of various new programming features for managing Hyper-V that are available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples are provided as compressed
 ZIP files that contain a Microsoft Visual Studio&nbsp;2010 solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming models,
 platforms, languages, and APIs demonstrated in this sample, please refer to the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850319">
Hyper-V WMI provider (V2)</a> documentation.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850319">Hyper-V WMI provider (V2)</a>
<h3>Operating system requirements</h3>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
</tbody>
</table>
<h3>Build the sample</h3>
<ol>
<li>
<p>Start Visual Studio&nbsp;2010 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.</p>
</li><li>
<p>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file titled Migration.sln.</p>
</li><li>
<p>Press F7 (or F6 for Microsoft Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>
Build Solution</b> to build the sample.</p>
</li></ol>
<h3>Run the sample</h3>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample must be run as an administrator.</p>
<p></p>
<p>This sample can be run in several different modes.</p>
<h4><a id="Perform_a_simple_migration_of_a_virtual_machine"></a><a id="perform_a_simple_migration_of_a_virtual_machine"></a><a id="PERFORM_A_SIMPLE_MIGRATION_OF_A_VIRTUAL_MACHINE"></a>Perform a simple migration of a virtual machine</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe vm-simple </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine to migrate. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Perform_a_simple_migration_of_a_virtual_machine_using_destination_host_IP_addresses"></a><a id="perform_a_simple_migration_of_a_virtual_machine_using_destination_host_ip_addresses"></a><a id="PERFORM_A_SIMPLE_MIGRATION_OF_A_VIRTUAL_MACHINE_USING_DESTINATION_HOST_IP_ADDRESSES"></a>Perform
 a simple migration of a virtual machine using destination host IP addresses</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe vm-simple-with-ip </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine to migrate. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Perform_a_simple_migration_of_a_virtual_machine_with_a_new_data_root"></a><a id="perform_a_simple_migration_of_a_virtual_machine_with_a_new_data_root"></a><a id="PERFORM_A_SIMPLE_MIGRATION_OF_A_VIRTUAL_MACHINE_WITH_A_NEW_DATA_ROOT"></a>Perform
 a simple migration of a virtual machine with a new data root</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe vm-simple-with-new-root </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine to migrate. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Migrate_a_planned_virtual_machine__perform_fixup__and_migrate_the_running_state_to_________the_planned_virtual_machine"></a><a id="migrate_a_planned_virtual_machine__perform_fixup__and_migrate_the_running_state_to_________the_planned_virtual_machine"></a><a id="MIGRATE_A_PLANNED_VIRTUAL_MACHINE__PERFORM_FIXUP__AND_MIGRATE_THE_RUNNING_STATE_TO_________THE_PLANNED_VIRTUAL_MACHINE"></a>Migrate
 a planned virtual machine, perform fixup, and migrate the running state to the planned virtual machine</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe vm-detailed </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine to migrate. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Modify_the_migration_service_settings"></a><a id="modify_the_migration_service_settings"></a><a id="MODIFY_THE_MIGRATION_SERVICE_SETTINGS"></a>Modify the migration service settings</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe modifyservice </b><i>Host</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>Host</i> is the name of the host server. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Modify_the_migration_service_networks"></a><a id="modify_the_migration_service_networks"></a><a id="MODIFY_THE_MIGRATION_SERVICE_NETWORKS"></a>Modify the migration service networks</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe modifynetworks </b><i>Host</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>Host</i> is the name of the host server. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Check_the_migration_compatibility_of_two_hosts"></a><a id="check_the_migration_compatibility_of_two_hosts"></a><a id="CHECK_THE_MIGRATION_COMPATIBILITY_OF_TWO_HOSTS"></a>Check the migration compatibility of two hosts</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe checkcompatibility </b><i>SourceHost</i><b> </b><i>DestinationHost</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host. </li><li><i>DestinationHost</i> is the name of the destination host. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Check_the_migration_compatibility_of_a_virtual_machine"></a><a id="check_the_migration_compatibility_of_a_virtual_machine"></a><a id="CHECK_THE_MIGRATION_COMPATIBILITY_OF_A_VIRTUAL_MACHINE"></a>Check the migration compatibility of a virtual
 machine</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe checkcompatibility </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Perform_a_simple_check_of_the_migration_capability_of_a_virtual_machine"></a><a id="perform_a_simple_check_of_the_migration_capability_of_a_virtual_machine"></a><a id="PERFORM_A_SIMPLE_CHECK_OF_THE_MIGRATION_CAPABILITY_OF_A_VIRTUAL_MACHINE"></a>Perform
 a simple check of the migration capability of a virtual machine</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe vm-simple-check </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Migrate_the_VHDs_and_data_roots_for_a_virtual_machine"></a><a id="migrate_the_vhds_and_data_roots_for_a_virtual_machine"></a><a id="MIGRATE_THE_VHDS_AND_DATA_ROOTS_FOR_A_VIRTUAL_MACHINE"></a>Migrate the VHDs and data roots for a virtual machine</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe storage-simple </b><i>Host</i><b> </b><i>VmName</i><b>
</b><i>NewLocation</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>Host</i> is the name of the current host of the virtual machine. </li><li><i>VmName</i> is the name of the virtual machine. </li><li><i>NewLocation</i> is the new location for all VHDs and data roots. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Migrate_the_first_VHD_for_a_virtual_machine_to_a_new_resource_pool"></a><a id="migrate_the_first_vhd_for_a_virtual_machine_to_a_new_resource_pool"></a><a id="MIGRATE_THE_FIRST_VHD_FOR_A_VIRTUAL_MACHINE_TO_A_NEW_RESOURCE_POOL"></a>Migrate the
 first VHD for a virtual machine to a new resource pool</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe storage-simple-with-pool </b><i>Host</i><b> </b><i>VmName</i><b>
</b><i>NewPoolId</i><b> </b><i>BasePath</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>Host</i> is the name of the current host of the virtual machine. </li><li><i>VmName</i> is the name of the virtual machine. </li><li><i>NewPoolId</i> is the identifier of the target resource pool. </li><li><i>BasePath</i> is the base directory of the VHD for the resource pool. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Perform_a_simple_migration_of_a_virtual_machine__using_resource_pools_to_obtain_the_correct_VHD_paths"></a><a id="perform_a_simple_migration_of_a_virtual_machine__using_resource_pools_to_obtain_the_correct_vhd_paths"></a><a id="PERFORM_A_SIMPLE_MIGRATION_OF_A_VIRTUAL_MACHINE__USING_RESOURCE_POOLS_TO_OBTAIN_THE_CORRECT_VHD_PATHS"></a>Perform
 a simple migration of a virtual machine, using resource pools to obtain the correct VHD paths</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe vm-and-storage-simple </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Migrate_all_virtual_machine_files_to_a_new_location_at_the_destination_host"></a><a id="migrate_all_virtual_machine_files_to_a_new_location_at_the_destination_host"></a><a id="MIGRATE_ALL_VIRTUAL_MACHINE_FILES_TO_A_NEW_LOCATION_AT_THE_DESTINATION_HOST"></a>Migrate
 all virtual machine files to a new location at the destination host</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>MigrationSamples.exe vm-and-storage </b><i>SourceHost</i><b> </b><i>DestinationHost</i><b>
</b><i>VmName</i><b> </b><i>NewLocation</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SourceHost</i> is the name of the current host of the virtual machine. </li><li><i>DestinationHost</i> is the name of the destination host. </li><li><i>VmName</i> is the name of the virtual machine. </li><li><i>NewLocation</i> is the new location for all VHDs and data roots. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
</div>
