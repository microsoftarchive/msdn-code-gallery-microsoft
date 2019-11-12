# MIDI Windows Runtime sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
## Topics
- Media
- Audio
## Updated
- 04/04/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the <b>WindowsPreview.Devices.Midi</b> API in a Windows Runtime app.
</p>
<p class="note"><b>Important</b>&nbsp;&nbsp;</p>
<p class="note">The <b>WindowsPreview.Devices.Midi</b> API are preview API and are not included in the Windows Software Development Kit (SDK). The API are distributed as a
<a href="http://www.nuget.org/">NuGet</a> package.</p>
<p class="note"><b>WindowsPreview.Devices.Midi</b> API are supported on Windows&nbsp;8.1 Update and later.</p>
<p>To download and install the NuGet package in your project, do the following steps.</p>
<ul>
<li>In Microsoft Visual Studio, select <b>PROJECT</b>&gt;<b>Manage NuGet Packages</b>.
</li><li>In the <b>Online</b> section, select <b>nuget.org</b>. Search for <b>Microsoft.WindowsPreview.MidiRT</b>.
</li><li>Click the <b>Install</b> button. </li><li>Select <b>BUILD</b>&gt;<b>Configuration Manager</b> to change the build configuration of your project from
<b>AnyCPU</b> to either x86, x64, or ARM. </li><li>The <b>WindowsPreview.Devices.Midi</b> API do not support running 32-bit apps on 64-bit Windows. You should deploy your app to the Windows Store with versions for x86, x64, and ARM.
</li><li>If intellisense does not work after you have installed the NuGet pacakge, unload and reload the Visual Studio project. In
<b>Solution Explorer</b>, right click the project and select <b>Unload Project</b>. Right click the project again and select
<b>Reload Project</b>. </li></ul>
<p></p>
<p>This sample demonstrates the following features:</p>
<ul>
<li>Enumerate MIDI In and MIDI Out ports. </li><li>Attach a device watcher to monitor port arrival and removal. </li><li>Query the properties of a MIDI port. </li><li>Open MIDI In ports. </li><li>Open MIDI Out ports. </li><li>Add a message watcher to MIDI In ports and process MIDI messages. </li><li>Create MIDI messages and send them to a MIDI Out ports. </li></ul>
<p>See the <a href="http://go.microsoft.com/fwlink/p/?LinkID=394282 ">WindowsPreview.Devices.Midi Documentation</a> for more info.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=394282 ">WindowsPreview.Devices.Midi Documentation</a>
</dt></dl>
<h2>Operating system requirements</h2>
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
<h2>Build the sample</h2>
<p></p>
<ol>
<li>Start Visual Studio Express&nbsp;2013 for Windows --&gt; and select <b>File</b> &gt;
<b>Open</b> &gt; <b>Project/Solution</b>. </li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio Express&nbsp;2013 for Windows Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </p>
</div>
