# Bluetooth Generic Attribute Profile - Heart Rate Service
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Bluetooth Gatt
- universal app
## Updated
- 09/03/2014
## Description

<div id="mainSection">
<p>Demonstrates use of the <a href="http://msdn.microsoft.com/library/windows/apps/dn297685">
<b>Bluetooth Generic Attribute Profile (Gatt) Windows Runtime API</b></a> to interact with a Bluetooth Gatt device, which contains a Heart Rate Service.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p><b>Device capabilities</b></p>
<p>This sample requires that device capabilities be set in the <i>Package.appxmanifest</i> file to allow the app to access the Bluetooth generic attribute profile at runtime. Device capabilities can be set in the app manifest using Microsoft Visual Studio.
</p>
<p>To build the sample, the following device capabilities are needed:</p>
<dl><dd>
<p><b>Name</b> = &quot;bluetooth.genericAttributeProfile&quot; </p>
<p><b>Device Id</b> = &quot;any&quot; </p>
<p><b>Function Type</b> = &quot;name:heartRate&quot; </p>
</dd></dl>
These capabilities are already set in the <i>Package.appxmanifest</i> file for this sample.
<p></p>
<p>For more information, see <a href="http://msdn.microsoft.com/library/windows/apps/dn263090">
How to specify device capabilities for Bluetooth (Windows Runtime apps)</a>.</p>
<p>Encryption is not required by all devices. If encryption is supported by the device, it can be enabled by setting the
<a href="http://msdn.microsoft.com/library/windows/apps/dn263747"><b>GattCharacteristic.ProtectionLevel</b></a> property to
<b>EncryptionRequired</b>. All subsequent operations on the characteristic will work over an encrypted link. The current sample enables encryption. If the device doesn't support encryption, then the app will fail.</p>
<p>You should disable encryption in the sample for devices that do not support encryption.
</p>
<p>The following shows the code change needed in the heart-rate-service.js file.</p>
<pre class="syntax"><code>    characteristic.protectionLevel = gatt.GattProtectionLevel.plain;
</code></pre>
<p>The following shows the code change needed in the <i>HeartRateService.cs</i> file.</p>
<pre class="syntax"><code>    characteristic.ProtectionLevel = GattProtectionLevel.Plain;
</code></pre>
<p>The following shows the code change needed in the <i>HeartRateService.cpp</i> file.</p>
<pre class="syntax"><code>    this-&gt;characteristic-&gt;ProtectionLevel = GattProtectionLevel::Plain;</code></pre>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample app is not a qualified Bluetooth product. Meeting necessary Bluetooth Qualification requirements, if any, are the responsibility of the app developer. Visit the
<a href="http://go.microsoft.com/fwlink/p/?linkid=320531">Qualification Listing Process</a> webpage to learn more.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn297685"><b>Windows.Devices.Bluetooth.GenericAttributeProfile</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn263090">How to specify device capabilities for Bluetooth (Windows Runtime apps)</a>
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
<td><dt>None supported </dt></td>
</tr>
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>BluetoothGattHeartRate.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build BluetoothGattHeartRate.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>BluetoothGattHeartRate.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build BluetoothGattHeartRate.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>BluetoothGattHeartRate.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy BluetoothGattHeartRate.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>BluetoothGattHeartRate.WindowsPhone</b> in <b>Solution Explorer</b>.
</li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy BluetoothGattHeartRate.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>BluetoothGattHeartRate.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>BluetoothGattHeartRate.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
