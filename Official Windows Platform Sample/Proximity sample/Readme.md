# Proximity sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Devices and sensors
- universal app
## Updated
- 11/26/2014
## Description

<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br241203">
<strong>PeerFinder</strong></a>, <a href="http://msdn.microsoft.com/library/windows/apps/br241212">
<strong>ProximityDevice</strong></a>, and <a href="http://msdn.microsoft.com/library/windows/apps/dn279099">
<strong>PeerWatcher</strong></a> classes to communicate with nearby devices.__ABSTRACT__</p>
<p>&nbsp;</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info
 about how to build apps that target Windows and Windows Phone with Visual Studio, see
<a href="http://msdn.microsoft.com/library/windows/apps/dn609832">Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/br212057">Windows.Networking.Proximity</a> APIs allow the same app running on two or more machines to communicate with each other in one of two ways. If the devices have Near Field Communications
 (NFC) radios, then you can align the radios and &quot;tap&quot; your devices together to exchange small messages. Alternatively, if you have two or more PCs which all support Wi-Fi Direct, or two or more phones which all support Bluetooth, then you can set up a socket
 connection between your devices. This socket connection can also be initiated using NFC.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/br241203"><strong>PeerFinder</strong></a> scenario (1) demonstrates how to use the Proximity APIs to create socket connections between peer apps on two or more devices, either by using a tap gesture
 or by first browsing for peers. &ldquo;Browsing&rdquo; refers to the process of discovering peers, whether over Bluetooth or Wi-Fi Direct, which is only successful when peer apps advertise for connections.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/dn279099"><strong>PeerWatcher</strong></a> scenario (2) demonstrates how to dynamically browse for peers. You should see the list of available peers shrink or expand as they start or stop advertising,
 or as they move out of range of your wireless radio.</p>
<p>The <a href="http://msdn.microsoft.com/library/windows/apps/br241212"><strong>ProximityDevice</strong></a> scenarios (3 and 4) demonstrate how to send small message payloads between two devices during a tap gesture. They also show how to process
<a href="http://msdn.microsoft.com/library/windows/apps/br241213"><strong>DeviceArrived</strong></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br241214"><strong>DeviceDeparted</strong></a> events when another device's NFC radio comes in and out of range.</p>
<p>This sample requires the use of multiple devices. For scenarios that use a tap gesture, each device must have an NFC radio installed.</p>
<p><strong>Windows</strong>&nbsp;For scenarios that require wireless peer discovery, each device must have a wireless radio that supports Wi-Fi Direct. Proximity apps running on Windows devices will not be able to discover Proximity apps running on Windows
 phones.</p>
<p><strong>Windows Phone</strong>&nbsp;For scenarios that require wireless peer discovery, each device must have a wireless chip that supports Bluetooth. Proximity apps running on Windows phones will not be able to discover Proximity apps running on Windows</p>
<p><strong>Windows</strong>&nbsp;If your device does not have an NFC radio, you can use the Proximity driver sample that is part of the Windows Driver Kit (WDK) samples. You can use the sample driver to simulate a tap gesture over a network connection between
 two devices. For info on how to download the WDK, see <a href="http://go.microsoft.com/fwlink/p/?linkid=136508">
Windows Driver Kit (WDK)</a>. After you have installed the WDK and samples, you can find the Proximity driver sample in the src\nfp directory in the location where you installed the WDK samples. See the NetNfpProvider.html file in the src\nfp\net directory
 for instructions on building and running the simulator. After you start the simulator, it runs in the background while your Proximity app is running in the foreground. Your app must be in the foreground for the tap simulation to work.</p>
<p>&nbsp;</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to
<a href="http://go.microsoft.com/fwlink/p/?linkid=301697">Microsoft Visual Studio&nbsp;2013</a>.</p>
<p>&nbsp;</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the
<a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p>&nbsp;</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465207">Quickstart: Connecting applications using tapping or browsing (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465205">Quickstart: Connecting applications using tapping or browsing (C#)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465223">Quickstart: Publishing and subscribing to messages using tapping (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465213">Quickstart: Publishing and subscribing to messages using tapping (C#)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465215">Guidelines and checklist for proximity</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241250"><strong>Windows.Networking.Proximity</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">StreamSocket</a> </dt></dl>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1</dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2</dt></td>
</tr>
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1</dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p>&nbsp;</p>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <strong>File</strong> &gt;
<strong>Open</strong> &gt; <strong>Project/Solution</strong>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <strong>Proximity.Windows</strong> in <strong>Solution Explorer</strong>.
</li><li>Press Ctrl&#43;Shift&#43;B, or use <strong>Build</strong> &gt; <strong>Build Solution</strong>, or use
<strong>Build</strong> &gt; <strong>Build Proximity.Windows</strong>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <strong>Proximity.WindowsPhone</strong> in <strong>Solution Explorer</strong>.
</li><li>Press Ctrl&#43;Shift&#43;B or use <strong>Build</strong> &gt; <strong>Build Solution</strong> or use
<strong>Build</strong> &gt; <strong>Build Proximity.WindowsPhone</strong>. </li></ol>
</li></ul>
</li></ol>
<p>&nbsp;</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;The C# versions of the sample have a CS suffix. For example, the C#-Windows Phone version of the sample is
<strong>ProximityCS.WindowsPhone</strong>.</p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><strong>Deploying the sample</strong></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <strong>Proximity.Windows</strong> in <strong>Solution Explorer</strong>.
</li><li>Use <strong>Build</strong> &gt; <strong>Deploy Solution</strong> or <strong>Build</strong> &gt;
<strong>Deploy Proximity.Windows</strong>. </li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <strong>Proximity.WindowsPhone</strong> in <strong>Solution Explorer</strong>.
</li><li>Use <strong>Build</strong> &gt; <strong>Deploy Solution</strong> or <strong>Build</strong> &gt;
<strong>Deploy Proximity.WindowsPhone</strong>. </li></ol>
</li></ul>
<p><strong>Deploying and running the sample</strong></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <strong>Proximity.Windows</strong> in <strong>Solution Explorer</strong> and select
<strong>Set as StartUp Project</strong>. </li><li>To debug the sample and then run it, press F5 or use <strong>Debug</strong> &gt;
<strong>Start Debugging</strong>. To run the sample without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <strong>Proximity.WindowsPhone</strong> in <strong>Solution Explorer</strong> and select
<strong>Set as StartUp Project</strong>. </li><li>To debug the sample and then run it, press F5 or use <strong>Debug</strong> &gt;
<strong>Start Debugging</strong>. To run the sample without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </li></ol>
</li></ul>
