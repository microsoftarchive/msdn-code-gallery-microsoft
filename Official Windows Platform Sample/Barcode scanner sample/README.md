# Barcode scanner sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Devices and sensors
- Point of service
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample shows how to create a barcode scanner, claim it for exclusive use, enable it to receive data, and read a barcode. This sample uses
<a href="http://msdn.microsoft.com/library/windows/apps/dn298071"><b>Windows.Devices.PointOfService</b></a> API.
</p>
<p>This sample demonstrates these tasks: </p>
<ol>
<li>
<p><b>Create the barcode scanner</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/dn263790"><b>BarcodeScanner.GetDefaultAsync</b></a> to get the first available barcode scanner.</p>
</li><li>
<p><b>Claim the barcode scanner for exclusive use</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/dn297696"><b>ClaimScannerAsync</b></a> to claim the device.</p>
</li><li><b>Add event handlers</b>
<p>Uses <a href="http://msdn.microsoft.com/library/windows/apps/dn278556"><b>DataReceived</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/dn278578"><b>ReleaseDeviceRequested</b></a> events.</p>
<p>When an application gets a request to release its exclusive claim to the barcode scanner, it must handle the request by retaining the device; otherwise, it will lose its claim. The second scenario in this sample shows the release and retain functionality.
 The event handler for <a href="http://msdn.microsoft.com/library/windows/apps/dn278578">
<b>ReleaseDeviceRequested</b></a> shows how retain the device.</p>
</li></ol>
<p>The app package manifest shows how to specify the device capability name for the Point of Service (POS) devices. All POS apps are required declare
<a href="http://msdn.microsoft.com/library/windows/apps/br211430"><b>DeviceCapability</b></a> in the app package manifest, either by using &quot;PointofService&quot; as shown in this sample or by using a device specific GUID, such as &quot;C243FFBD-3AFC-45E9-B3D3-2BA18BC7EBC5&quot;
 for a barcode scanner.</p>
<p>The following list shows the barcode scanners that were used with this sample:</p>
<ul>
<li>Honeywell 1900GSR-2 </li><li>Honeywell 1200g-2 </li><li>Intermec SG20 </li></ul>
<p>In addition to the devices listed, you can use barcode scanners from various manufacturers that adhere to the
<a href="http://go.microsoft.com/fwlink/p/?linkid=309230">USB HID POS Scanner specification</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298071"><b>Windows.Devices.PointOfService</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=309230">USB HID POS Scanner specification</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows app samples</a>
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
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </p>
</div>
