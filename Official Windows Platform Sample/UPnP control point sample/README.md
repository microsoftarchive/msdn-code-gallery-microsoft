# UPnP control point sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Devices and sensors
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample shows device discovery and subscription modes for Universal Plug and Play (UPnP), along with querying variables, events, and invoking actions.
</p>
<p>This sample uses the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa381109">
UPnP Control Point API</a> provided by <b>upnp.dll</b>. The sample can discover devices on the network by using one of the three types of searches available:
<b>FindByType</b>, <b>FindByUDN</b> and <b>AsyncFind</b>. </p>
<p>The devices found by these search capabilities are instantiated in the device list. When a device is selected, service objects for the device are enumerated and listed in the service list. If the
<b>Delay Subscription</b> check box is selected when the desired device is selected, the
<b>Subscribe</b> button will become active and SCPD download and event subscription will be delayed when the services are enumerated. If the
<b>Delay Subscription</b> check box is not selected, the SCPD will be downloaded and the subscriptions will be done while enumerating services.
</p>
<p>One of the services can be selected and controlled by invoking actions against it. The events relevant to the service are displayed in the events field. If the
<b>Delay Subscription</b> check box was selected when the device was selected, click the
<b>Subscribe</b> button to start event subscriptions. If <b>Asynchronous Control</b> check box is selected, the asynchronous control methods will be used. If the
<b>Asynchronous Control</b> check box is not selected, the normal synchronous methods will be used.
</p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013 and will not compile in Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa381109">UPnP Control Point API</a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa381109">UPnP Control Point API</a>
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
<p>Start Visual Studio and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.</p>
</li><li>
<p>2. Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.</p>
</li><li>
<p>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.</p>
</li></ol>
<h3>Run the sample</h3>
<p>To run the sample:</p>
<ol>
<li>Navigate to the directory that contains the new executable, using the command prompt or Windows Explorer.
</li><li>Copy the <b>devType.txt</b> and <b>Udn.txt</b> files from the CPP directory to the current directory.
</li><li>Run <b>GenericUcp.exe</b>. The device types and the UDNs that appear in the drop down menu are from
<b>devType.txt</b> and <b>Udn.txt</b> respectively. </li></ol>
</div>
