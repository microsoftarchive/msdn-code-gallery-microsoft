# Cluster Aware Updating plug-in sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Clustering
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample demonstrates a Cluster Aware Updating (CAU) plug-in that runs an arbitrary, administrator-specified, command on each cluster node.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh873034"><b>ClusterAwareUpdating</b></a>
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
<p>This sample requires the ClusterAwareUpdating.dll assembly which is available for Windows&nbsp;8.1 in the RSAT package download. The FailOver Cluster feature must be installed in order to build this sample on Windows Server&nbsp;2012&nbsp;R2.</p>
<p>To build this sample, open the solution (.sln) file titled CauSamplePlugin.sln from Visual Studio Professional&nbsp;2012 or Visual Studio&nbsp;2013. Press F6 or go to
<b>Build-&gt;Build Solution</b> from the top menu after the sample has loaded. </p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Visual Studio Professional&nbsp;2012 or Visual Studio&nbsp;2013 and does not compile in Microsoft Visual Studio Express&nbsp;2013 for Windows. It also requires .NET Framework&nbsp;4.5.</p>
<h3>Run the sample</h3>
<p>The CAU plug-in has one required argument, Command, which is the command that will be run. Here's an example command line.</p>
<p><b>Invoke-CauRun -ClusterName MyTestCluster -CauPluginName FabrikamCauPlugin -CauPluginArguments @{ &quot;Command&quot;=&quot;cmd.exe /c echo Hello.&quot; } -Verbose</b></p>
<p></p>
<p class="note"><b>Important</b>&nbsp;&nbsp;The &quot;Command&quot; string is case-sensitive, so &quot;command&quot; returns an error.</p>
<p></p>
<p>When the plug-in performs a scan, it detects a single &quot;update,&quot; which is the command to run, applicable to each node. Staging doesn't do anything except report that it's ready to install the update (run the command). When the plug-in &quot;installs&quot; the update,
 it runs the command on the target machine using WMI, and waits for it to exit. If the remote process exits with a non-zero error code, the update is considered to have failed.
</p>
<p>Because this plug-in uses WMI, it requires that your firewalls on both the orchestrator machine and the cluster nodes have the appropriate rules enabled. It's easy to overlook that the orchestrator must allow inbound WMI connections in order to receive events
 from the cluster nodes when the commands finish. The plug-in tests its ability to use WMI with each node.</p>
</div>
