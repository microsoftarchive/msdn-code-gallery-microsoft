# Hyper-V networking sample
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
<p>This sample demonstrates how to use the Hyper-V WMI networking APIs to create, configure, and remove networking related objects such as switches and ports. The sample demonstrates how to perform each of the following operations:</p>
<ul>
<li>Create a network switch using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850040">
<b>DefineSystem</b></a> method. </li><li>Delete a network switch using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850044">
<b>DestroySystem</b></a> method. </li><li>Modify an existing network switch using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850103">
<b>ModifySystemSettings</b></a> method. </li><li>Add network ports using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850019">
<b>AddResourceSettings</b></a> method, and remove network ports using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850277">
<b>RemoveResourceSettings</b></a> method. </li><li>Modify an existing network port using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850098">
<b>ModifyResourceSettings</b></a> method. </li><li>Gather and display information about a network switch using various classes and methods.
</li><li>Determine if an external adapter supports trunk mode using the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850260">
<b>Msvm_VLANEndpoint</b></a> class. </li><li>Connect a network adapter to a virtual machine, disconnect a network adapter from a virtual machine, or modify an existing a connected network adapter.
</li><li>Add, modify, or remove feature settings for a virtual machine's connections. </li><li>Connect a virtual machine to a network adapter from a resource pool. </li><li>Enable, disable, or reorder a network switch extension. </li><li>Modify the list of required features on all connections associated with a virtual machine.
</li><li>Add, modify, or remove advanced properties for a network switch. </li></ul>
<p></p>
<p>This sample is written in C# and requires some experience with WMI programming.</p>
<p>The Windows Samples Gallery contains a variety of code samples that demonstrate the use of various new programming features for managing Hyper-V that are available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples are provided as compressed
 ZIP files that contain a Microsoft Visual Studio&nbsp;2010 solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming models,
 platforms, languages, and APIs demonstrated in this sample, please refer to the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850319">
Hyper-V WMI provider (V2)</a> documentation.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850074">Hyper-V networking API</a>
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
<p>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file titled Networking.sln.</p>
</li><li>
<p>Press F7 (or F6 for Microsoft Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>
Build Solution</b> to build the sample.</p>
</li></ol>
<h3>Run the sample</h3>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample must be run as an administrator.</p>
<p></p>
<p>This sample can be run in several different modes. To obtain a list of the operations, use the following command line:</p>
<p><b>NetworkingSamples.exe /?</b></p>
<p>To obtain the parameters for each command, use the following command line format:</p>
<p><b>NetworkingSamples.exe </b><i>command</i><b> /?</b></p>
<p>where <i>command</i> is the specific command to obtain the parameters for. For example, the following command line will list the parameters for the
<code>CreateSwitch</code> operation:</p>
<p><b>NetworkingSamples.exe CreateSwitch /?</b></p>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
<h4><a id="Create_a_network_switch"></a><a id="create_a_network_switch"></a><a id="CREATE_A_NETWORK_SWITCH"></a>Create a network switch</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe CreateSwitch </b><i>Type</i><b> </b>[<i>ExternalNetwork</i>]<b>
</b>[<i>SwitchName</i>]<b> </b>[<i>SwitchNotes</i>]</p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>Type</i> specifies the type of switch to create. This can be one of the following values:
<ul>
<li>&quot;Private&quot; </li><li>&quot;Internal&quot; </li><li>&quot;ExternalOnly&quot; </li><li>&quot;External&quot; </li></ul>
</li><li><i>ExternalNetwork</i> the name of the external network. This parameter is required when
<i>Type</i> is &quot;External&quot; or &quot;ExternalOnly&quot;, and must not be present otherwise. </li><li><i>SwitchName</i> is the optional name for the switch. A default name will be used if this parameter is not specified.
</li><li><i>SwitchNotes</i> is the optional notes for the switch. This string must be within quotes if it contains any spaces.
</li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Delete_a_network_switch"></a><a id="delete_a_network_switch"></a><a id="DELETE_A_NETWORK_SWITCH"></a>Delete a network switch</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe DeleteSwitch </b><i>SwitchName</i></p>
<p>where <i>SwitchName</i> is the name of the switch to delete.</p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Modify_an_existing_network_switch"></a><a id="modify_an_existing_network_switch"></a><a id="MODIFY_AN_EXISTING_NETWORK_SWITCH"></a>Modify an existing network switch</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe ModifySwitch </b><i>SwitchName</i><b> </b><i>NewSwitchName</i><b>
</b>[<i>SwitchNotes</i>]</p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SwitchName</i> is the name of the switch to modify. </li><li><i>NewSwitchName</i> is the new name of the switch to be applied. </li><li><i>SwitchNotes</i> is the optional new notes for the switch. This string must be within quotes if it contains any spaces.
</li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Add_or_remove_network_ports"></a><a id="add_or_remove_network_ports"></a><a id="ADD_OR_REMOVE_NETWORK_PORTS"></a>Add or remove network ports</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe AddAndRemovePorts </b><i>Command</i><b> </b><i>SwitchName</i><b>
</b>[<i>ExternalNetwork</i>]</p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>Command</i> is the command to execute and must be one of the following values:
<ul>
<li>&quot;Add&quot; adds an external connection to <i>ExternalNetwork</i>. </li><li>&quot;Remove&quot; removes the ports from <i>SwitchName</i>. </li></ul>
</li><li><i>SwitchName</i> is the name of the switch. </li><li><i>ExternalNetwork</i> is the name of the external network to add a connection to. This parameter is not used if
<i>Command</i> is &quot;Remove&quot;. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Modify_network_ports"></a><a id="modify_network_ports"></a><a id="MODIFY_NETWORK_PORTS"></a>Modify network ports</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe ModifyPorts </b><i>SwitchName</i><b> </b><i>ExternalNetwork</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SwitchName</i> is the name of the switch to modify the ports for. </li><li><i>ExternalNetwork</i> is the name of the external network to connect the ports to.
</li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Display_information_about_a_network_switch"></a><a id="display_information_about_a_network_switch"></a><a id="DISPLAY_INFORMATION_ABOUT_A_NETWORK_SWITCH"></a>Display information about a network switch</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe EnumerateSwitch </b><i>SwitchName</i></p>
<p>where <i>SwitchName</i> is the name of the switch to display the information for.</p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Determine_if_an_external_adapter_supports_trunk_mode"></a><a id="determine_if_an_external_adapter_supports_trunk_mode"></a><a id="DETERMINE_IF_AN_EXTERNAL_ADAPTER_SUPPORTS_TRUNK_MODE"></a>Determine if an external adapter supports trunk mode</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe SupportsTrunkMode </b><i>ExternalNetwork</i></p>
<p>where <i>ExternalNetwork</i> is the name of the external adapter in question.</p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Connect_a_network_adapter_to_a_virtual_machine"></a><a id="connect_a_network_adapter_to_a_virtual_machine"></a><a id="CONNECT_A_NETWORK_ADAPTER_TO_A_VIRTUAL_MACHINE"></a>Connect a network adapter to a virtual machine</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe ConnectVmToSwitch </b>{<b>Connect</b>|<b>Disconnect</b>|<b>Modify</b>}<b>
</b><i>VirtualMachineName</i><b> </b><i>SwitchName</i><b> </b>[<i>NewSwitchName</i>]</p>
<p>The first parameter is one of the following:</p>
<ul>
<li>&quot;Connect&quot;: Adds <i>SwitchName</i> to <i>VirtualMachineName</i>. <i>NewSwitchName</i> is not used.
</li><li>&quot;Disconnect&quot;: Removes <i>SwitchName</i> from <i>VirtualMachineName</i>. <i>NewSwitchName</i> is not used.
</li><li>&quot;Modify&quot;: Changes the switch name from <i>SwitchName</i> to <i>NewSwitchName</i>.
</li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Add__modify__or_remove_feature_settings_for_a_virtual_machine_s_connections"></a><a id="add__modify__or_remove_feature_settings_for_a_virtual_machine_s_connections"></a><a id="ADD__MODIFY__OR_REMOVE_FEATURE_SETTINGS_FOR_A_VIRTUAL_MACHINE_S_CONNECTIONS"></a>Add,
 modify, or remove feature settings for a virtual machine's connections</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe AdvancedConnectionProperties </b>{<b>Add</b>|<b>Modify</b>|<b>Remove</b>}<b>
</b><i>VirtualMachineName</i><b> </b>[<i>FeatureSetting</i>]</p>
<p>The first parameter is one of the following:</p>
<ul>
<li>&quot;Add&quot;: Adds <i>FeatureSetting</i> to all connections from <i>VirtualMachineName</i>.
<i>FeatureSetting</i> can be either &quot;acl&quot; or &quot;security&quot;. </li><li>&quot;Modify&quot;: Changes the security setting for each connection from <i>VirtualMachineName</i> to enable MAC spoofing .
<i>FeatureSetting</i> is not used. </li><li>&quot;Remove&quot;: Removes <i>FeatureSetting</i> from all connections from <i>VirtualMachineName</i>.
<i>FeatureSetting</i> can be either &quot;acl&quot; or &quot;security&quot;. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Connect_a_virtual_machine_to_a_network_adapter_from_a_resource_pool"></a><a id="connect_a_virtual_machine_to_a_network_adapter_from_a_resource_pool"></a><a id="CONNECT_A_VIRTUAL_MACHINE_TO_A_NETWORK_ADAPTER_FROM_A_RESOURCE_POOL"></a>Connect
 a virtual machine to a network adapter from a resource pool</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe ConnectVmUsingResourcePool </b><i>VirtualMachineName</i><b>
</b><i>ResourcePoolName</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>VirtualMachineName</i> is the name of the virtual machine. </li><li><i>ResourcePoolName</i> is the name of the resource pool to use for the connection.
</li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Enable__disable__or_reorder_a_network_switch_extension"></a><a id="enable__disable__or_reorder_a_network_switch_extension"></a><a id="ENABLE__DISABLE__OR_REORDER_A_NETWORK_SWITCH_EXTENSION"></a>Enable, disable, or reorder a network switch extension</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe ManageExtension </b><i>SwitchName</i><b> </b><i>ExtensionName</i><b>
</b><i>Command</i></p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>SwitchName</i> is the name of the switch. </li><li><i>ExtensionName</i> is the name of the switch extension. </li><li><i>Command</i> is the command to execute on the switch extension. This must be one of the following values.
<table>
<tbody>
<tr>
<th>Command</th>
<th>Description</th>
</tr>
<tr>
<td>
<p>&quot;Enable&quot;</p>
</td>
<td>
<p>Enable the switch extension.</p>
</td>
</tr>
<tr>
<td>
<p>&quot;Disable&quot;</p>
</td>
<td>
<p>Disable the switch extension.</p>
</td>
</tr>
<tr>
<td>
<p>&quot;MoveUp&quot;</p>
</td>
<td>
<p>Move the switch extension up one place in the order.</p>
</td>
</tr>
<tr>
<td>
<p>&quot;MoveDown&quot;</p>
</td>
<td>
<p>Move the switch extension down one place in the order.</p>
</td>
</tr>
</tbody>
</table>
</li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Modify_the_list_of_required_features_on_all_connections_associated_with_a_virtual_machine"></a><a id="modify_the_list_of_required_features_on_all_connections_associated_with_a_virtual_machine"></a><a id="MODIFY_THE_LIST_OF_REQUIRED_FEATURES_ON_ALL_CONNECTIONS_ASSOCIATED_WITH_A_VIRTUAL_MACHINE"></a>Modify
 the list of required features on all connections associated with a virtual machine</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe SetRequiredFeature </b><i>VirtualMachineName</i><b> </b>
<i>FeatureName</i><b> </b>{<b>true</b>|<b>false</b>}</p>
<p>where the parameters are as follows:</p>
<ul>
<li><i>VirtualMachineName</i> is the name of the virtual machine. </li><li><i>FeatureName</i> is the name of the feature. </li><li>The last parameter is &quot;true&quot; to make the feature required, or &quot;false&quot; to not make the feature required.
</li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
<h4><a id="Add__modify__or_remove_advanced_properties_for_a_network_switch"></a><a id="add__modify__or_remove_advanced_properties_for_a_network_switch"></a><a id="ADD__MODIFY__OR_REMOVE_ADVANCED_PROPERTIES_FOR_A_NETWORK_SWITCH"></a>Add, modify, or remove
 advanced properties for a network switch</h4>
<p>To run this sample in this mode, follow these steps.</p>
<ol>
<li>
<p>Enter the debug command line arguments for the project. The usage of this sample is:</p>
<p><b>NetworkingSamples.exe AdvancedSwitchProperties </b>{<b>Add</b>|<b>Modify</b>|<b>Remove</b>}<b>
</b><i>SwitchName</i></p>
<p>The first parameter is one of the following:</p>
<ul>
<li>&quot;Add&quot;: Adds bandwidth settings to <i>SwitchName</i> to set the <b>DefaultFlowReservation</b> property of the
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850243"><b>Msvm_VirtualEthernetSwitchBandwidthSettingData</b></a> class.
</li><li>&quot;Modify&quot;: Increases the <b>DefaultFlowReservation</b> property of the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh850243">
<b>Msvm_VirtualEthernetSwitchBandwidthSettingData</b></a> class for <i>SwitchName</i> by 1Mbps.
</li><li>&quot;Remove&quot;: Removes the bandwidth settings from <i>SwitchName</i>. </li></ul>
<p></p>
</li><li>
<p>To debug the app and then run it from Visual Studio&nbsp;2010, press F5 or use <b>Debug</b> &gt;
<b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use <b>
Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</li><li>The final result of the operation will be displayed in the console window. </li></ol>
</div>
