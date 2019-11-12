# Magnetic stripe reader sample
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
<p>This sample shows how to create a magnetic stripe reader, claim it for exclusive use, enable it to receive data, and read data from a bank card or a motor vehicle card. This sample uses
<a href="http://msdn.microsoft.com/library/windows/apps/dn298071"><b>Windows.Devices.PointOfService</b></a> API.
</p>
<p>This sample demonstrates these tasks: </p>
<ol>
<li>
<p><b>Get the magnetic stripe reader</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/dn297987"><b>MagneticStripeReader.GetDefaultAsync</b></a> method to get the first available magnetic stripe reader.</p>
</li><li>
<p><b>Claim the magnetic stripe reader for exclusive use</b></p>
<p>Uses <a href="http://msdn.microsoft.com/library/windows/apps/dn297979"><b>ClaimReaderAsync</b></a> to claim the device.</p>
</li><li>
<p><b>Add event handlers</b></p>
<p>Uses <a href="http://msdn.microsoft.com/library/windows/apps/dn278599"><b>BankCardDataReceived</b></a>,
<a href="http://msdn.microsoft.com/library/windows/apps/dn278595"><b>AamvaCardDataReceived</b></a>, and
<a href="http://msdn.microsoft.com/library/windows/apps/dn278626"><b>ReleaseDeviceRequested</b></a> events.
</p>
<p>When an application gets a request from another application to release its exclusive claim to the magnetic stripe reader, it must handle the request by retaining the device; otherwise, it will lose its claim. The event handler for
<a href="http://msdn.microsoft.com/library/windows/apps/dn278626"><b>ReleaseDeviceRequested</b></a> shows how to do this.</p>
</li><li><b>Enable the device to receive data</b>
<p>Uses <a href="http://msdn.microsoft.com/library/windows/apps/dn278612"><b>EnableAsync</b></a>.</p>
</li></ol>
<p>The app package manifest shows how to specify the device capability name for the Point of Service (POS) devices. All POS apps are required declare
<a href="http://msdn.microsoft.com/library/windows/apps/br211430"><b>DeviceCapability</b></a> in the app package manifest, either by using &quot;PointofService&quot; as shown in this sample or by using device specific GUID, such as &quot;2A9FE532-0CDC-44F9-9827-76192F2CA2FB&quot;
 for a magnetic stripe reader.</p>
<p></p>
<p>The following list shows the magnetic stripe readers that were used with this code sample:</p>
<ul>
<li>MagTek MagneSafe HID USB reader (VID 0801, PID 0011) </li><li>MagTek SureSwipe HID USB reader ( VID 0801, PID 0002) </li><li>MagTek BulleT Bluetooth/USB reader, when connected via USB (VID 0801, PID 0011)
</li><li>ID TECH SecureMag HID USB reader (VID 0ACD, PID 2010) </li><li>ID TECH MiniMag HID USB reader (VID 0ACD, PID 0500) </li></ul>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/">Barcode Scanner Sample</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298071"><b>Windows.Devices.PointOfService</b></a>
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
